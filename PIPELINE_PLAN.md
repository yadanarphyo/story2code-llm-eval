# Story2Code Pipeline — Design Document

## 1. Overview & Goals

This pipeline supports the dissertation *"A Comparative Analysis of LLM-Driven Code Generation from User Stories."* It automates, end-to-end, the process of:

1. Generating a C# API backend implementation from a user story, using 4 different LLMs:
   - Kimi-k2.6:cloud
   - Qwen-3.5:cloud
   - GPT-5.4
   - Claude Sonnet 4.6
2. Running each model **3 times** per user story (to capture run-to-run variance).
3. Verifying **functional correctness** of each generated implementation against pre-written unit tests.
4. Analysing **code quality** of each generated implementation with SonarQube.
5. Producing a single Excel report per pipeline run summarising build status, test results, coverage, and all SonarQube metrics — one row per (user story × model × run).

The pipeline is invoked as:

```bash
python3 story2code.py <userStoryNN.md>
```

where `<userStoryNN.md>` contains the user story description, its API interface contract, and its data model (see sample in §9).

### Design principle: language-agnostic by construction

The dissertation's current scope is C#, but the pipeline must **not** hard-code a C#-only architecture. Each LLM generates the **entire project** from scratch (build files, folder layout, everything) rather than filling in a fixed template — this is a deliberate choice so the same pipeline can later target other languages/stacks without a redesign. The only place language-specific logic lives is behind a small `LanguageAdapter` interface (§5.3).

---

## 2. Folder Structure

```
Story2Code/
├── UserStories/
│   └── UserStory-01.md            # user story + interface contract + data model
├── Prompts/
│   └── rules-file                 # prompt rules given to every LLM (naming contract, see §6)
├── Config/
│   ├── llm.config                 # per-model routing/credentials (see §4.1)
│   └── sonar.config               # SonarQube server/token/project-key convention (see §4.2)
├── Unit-Tests/
│   └── UserStory-01/               # pre-written xUnit/NUnit test project for US-01
├── Pipeline/                       # python pipeline source
│   ├── story2code.py               # CLI entry point / orchestrator
│   ├── llm_client.py                # LangChain-based model abstraction
│   ├── spec_generator.py            # Stage 1
│   ├── project_generator.py         # Stage 2
│   ├── language_adapters/
│   │   ├── base.py                  # LanguageAdapter interface
│   │   └── csharp_adapter.py        # build/test/sonar for .NET
│   ├── test_runner.py               # Stage 4 (merges + runs pre-written tests)
│   ├── sonar_runner.py              # Stage 5
│   ├── report_generator.py          # Stage 6
│   └── run_manager.py               # run/repeat folder bookkeeping
├── Runs/
│   └── 2026-07-14_run1/
│       ├── US-01_gpt5.4/
│       │   ├── run1/
│       │   │   ├── spec.md
│       │   │   └── src/             # generated project (full solution)
│       │   ├── run2/
│       │   └── run3/
│       ├── US-01_qwen3.5-cloud/
│       ├── US-01_kimi-k2.6-cloud/
│       └── US-01_claude-sonnet-4.6/
└── Results/
    └── results_2026-07-14.xlsx
```

Each top-level `Runs/<date>_runN/` corresponds to one invocation of `story2code.py`. Inside it, one folder per model, and inside that, one folder per repeat (`run1`–`run3`), each holding that trial's generated spec and full generated project.

---

## 3. Pipeline Stages

```
UserStory.md
     │
     ▼
[Stage 1] Spec Generation ───────────► spec.md   (per model, per run)
     │
     ▼
[Stage 2] Full Project Generation ───► src/       (per model, per run — entire project)
     │
     ▼
[Stage 3] Build ──────────────────────► build status (pass/fail + errors)
     │
     ▼
[Stage 4] Test Execution ─────────────► pass/fail counts + coverage %
     │
     ▼
[Stage 5] SonarQube Analysis ─────────► code smells, ratings, complexity, maintainability
     │
     ▼
[Stage 6] Report Generation ──────────► Results/results_<date>.xlsx
```

Orchestration loop (`story2code.py`): for the given user story, for each of the 4 models, for each of 3 repeats, run Stages 1–5 and collect a result row; after all 12 executions complete, run Stage 6 once.

### 3.1 Stage 1 — Spec Generation
Input: user story markdown (description + interface contract + data model) + `Prompts/rules-file`.
The LLM produces an implementation spec (`spec.md`) — a more detailed technical design translating the story/contract into concrete architectural decisions (project name, namespace, layering, endpoint routing) *while still obeying the fixed naming contract from the rules-file* (see §6).

### 3.2 Stage 2 — Full Project Generation
Input: `spec.md` + rules-file.
The LLM generates the **complete project**: build/project files (e.g. `.csproj`/`.sln` for C#), source files, configuration — everything needed to build and run standalone. Written to `Runs/.../<model>/run<N>/src/`.

### 3.3 Stage 3 — Build
Delegated to the `LanguageAdapter` for the detected/configured language (§5.3). For C#: `dotnet build` against the generated solution. Captures success/failure and any compiler errors/warnings.

### 3.4 Stage 4 — Test Execution
The pre-written test project (`Unit-Tests/UserStory-01/`) is copied into the generated project's working copy and wired in as a **white-box** dependency:
1. Copy the test project alongside the generated `src/`.
2. Add a project reference from the test project to the generated implementation project — deterministic because the rules-file (§6) pins the exact project/assembly name and namespace the LLM must use.
3. Run tests (`dotnet test`) with coverage collection (`coverlet`), capturing pass/fail counts and % line coverage.

### 3.5 Stage 5 — SonarQube Analysis
Runs `dotnet-sonarscanner begin` → build → `dotnet-sonarscanner end` against the generated project (test coverage report is fed in so Sonar can report coverage alongside its own metrics). After analysis completes, poll the SonarQube Web API for the project's measures:

| Metric | Output Format | Example |
|---|---|---|
| Code Smells | Count + severity breakdown | 42 code smells (12 major, 30 minor) |
| Reliability Rating (Bugs) | Letter rating A–E; bug count | A, 5 bugs |
| Security Rating | Letter rating A–E; vulnerability count | B, 3 vulnerabilities |
| Cyclomatic Complexity | Integer per function | foo: 4, bar: 7 |
| Maintainability Index | Composite score | High / Medium / Low |

Each generated project gets a unique Sonar project key (convention: `<userstory>-<model>-run<N>-<runID>`) so concurrent/sequential analyses don't collide.

### 3.6 Stage 6 — Report Generation
One Excel workbook per pipeline run (`Results/results_<date>.xlsx`), one row per (user story × model × run). Columns (§7).

---

## 4. Config Schemas

### 4.1 `Config/llm.config`
Per-model entry defining how the pipeline reaches that model. Since routing may be Ollama *or* a native SDK depending on availability, each entry declares its own route:

```yaml
models:
  - id: kimi-k2.6
    display_name: "Kimi-k2.6:cloud"
    provider: ollama
    ollama_tag: "kimi-k2:cloud"
    temperature: 0.2

  - id: qwen-3.5
    display_name: "Qwen-3.5:cloud"
    provider: ollama
    ollama_tag: "qwen3.5:cloud"
    temperature: 0.2

  - id: gpt-5.4
    display_name: "GPT-5.4"
    provider: openai        # native fallback if not in Ollama's cloud catalog
    model_name: "gpt-5.4"
    api_key_env: OPENAI_API_KEY
    temperature: 0.2

  - id: claude-sonnet-4.6
    display_name: "Claude Sonnet 4.6"
    provider: anthropic      # native fallback if not in Ollama's cloud catalog
    model_name: "claude-sonnet-4-6"
    api_key_env: ANTHROPIC_API_KEY
    temperature: 0.2

runs_per_model: 3
```

`provider` is the routing switch consumed by `llm_client.py` (§5.1) — `ollama`, `openai`, or `anthropic` all resolve to a LangChain chat-model instance, so the rest of the pipeline only ever calls a uniform `.invoke(prompt)`.

### 4.2 `Config/sonar.config`
```yaml
host_url: "http://localhost:9000"
token_env: SONAR_TOKEN
project_key_prefix: "story2code"
quality_profile: "Sonar way"
poll_interval_seconds: 5
poll_timeout_seconds: 300
```

---

## 5. Key Components

### 5.1 LLM Abstraction (`llm_client.py`)
A single LangChain-based interface used by both Stage 1 and Stage 2:

```python
def get_chat_model(model_config: dict) -> BaseChatModel:
    if model_config["provider"] == "ollama":
        return ChatOllama(model=model_config["ollama_tag"], temperature=model_config["temperature"])
    if model_config["provider"] == "openai":
        return ChatOpenAI(model=model_config["model_name"], temperature=model_config["temperature"])
    if model_config["provider"] == "anthropic":
        return ChatAnthropic(model=model_config["model_name"], temperature=model_config["temperature"])
```

Every caller downstream only interacts with the LangChain `BaseChatModel` interface (`.invoke(...)`) — swapping which models are used for the comparison is a `llm.config` edit, not a code change. **Preference order**: attempt Ollama's cloud catalog for all 4 models first; where a model isn't available there, fall back to the native provider SDK via LangChain, as already encoded in the sample config above.

### 5.2 Run Manager (`run_manager.py`)
Creates and tracks the `Runs/<date>_run<n>/US-xx_<model>/run<1-3>/` folder hierarchy, assigns run IDs, and hands each stage the correct working directory.

### 5.3 Language Adapter (`language_adapters/`)
```python
class LanguageAdapter(Protocol):
    def build(self, project_dir: Path) -> BuildResult: ...
    def run_tests(self, project_dir: Path, test_dir: Path) -> TestResult: ...
    def sonar_properties(self, project_dir: Path) -> dict: ...
```
`CSharpAdapter` is the only implementation needed now (`dotnet build`, `dotnet test` + coverlet, `dotnet-sonarscanner` properties). Future languages (Python, Java, TS, …) plug in as additional adapters without touching orchestration code. Adapter selection is driven by a `language` field in `llm.config` or the rules-file (defaulted to `csharp` for this study).

### 5.4 Report Generator (`report_generator.py`)
Aggregates the 12 per-run result objects (per user story) into rows and writes/updates the workbook via `openpyxl`/`pandas`. Designed to accept results incrementally, so a partial run (e.g. one model failing) still produces a usable report.

---

## 6. Rules-file / Prompt Contract (`Prompts/rules-file`)

Because Stage 2 lets each LLM generate a full, freely-structured project, and Stage 4 wires pre-written tests in **white-box** (compiled directly against the generated code), the rules-file is the single point that keeps generated output test-compatible. It must pin down, deterministically, for every model:

- The main project/assembly name (e.g. always `Implementation`) so the test project can `dotnet add reference` to it unambiguously.
- The root namespace to use.
- Exact class name(s), method name(s), and signatures — already partially constrained by the interface contract in the user story (e.g. `GetNearbyRecyclingFacilities`, `RecyclingFacility` data model), but the rules-file must state explicitly that these must be reproduced verbatim, not paraphrased.
- Any other structural conventions the test project relies on (e.g. a specific `Program.cs`/host entry point name if tests spin up a test server).

This is the highest-risk part of the design (§10) — it depends entirely on LLMs reliably following naming instructions.

---

## 7. Excel Report Schema

One row per (user story × model × run):

| Column | Description |
|---|---|
| Run Date / Run ID | Top-level pipeline run identifier |
| User Story ID | e.g. US-01 |
| Model | e.g. GPT-5.4 |
| Run Number | 1–3 |
| Build Status | Success / Fail |
| Build Error Summary | Truncated compiler error, if failed |
| Tests Passed / Total | e.g. 8/10 |
| Test Pass Rate (%) | |
| Code Coverage (%) | |
| Code Smells (Total) | |
| Code Smells — by Severity | e.g. 12 major, 30 minor |
| Reliability Rating | A–E |
| Bug Count | |
| Security Rating | A–E |
| Vulnerability Count | |
| Cyclomatic Complexity | Summary (avg + per-function detail link/path) |
| Maintainability Index | High / Medium / Low |
| Spec File Path | |
| Generated Source Path | |
| Sonar Project Key / Dashboard Link | |
| Timestamp | |

---

## 8. Environment Setup

Nothing is provisioned yet; the implementation phase must include:

1. **SonarQube server** — run via Docker (`docker run -d -p 9000:9000 sonarqube:lts-community`), create a project token for API/scanner use, store in `SONAR_TOKEN`.
2. **.NET SDK** — install the SDK version matching the target C# projects; install `dotnet-sonarscanner` (`dotnet tool install --global dotnet-sonarscanner`) and `coverlet.collector`/`coverlet.msbuild`.
3. **Ollama** — install locally, authenticate for cloud models, pull/verify `kimi-k2:cloud` and `qwen3.5:cloud` (or confirm actual available tags — see risk in §10).
4. **Python environment** — `langchain`, `langchain-community` (Ollama), `langchain-openai`, `langchain-anthropic`, `openpyxl`/`pandas`, `pyyaml`.
5. **API keys/tokens** — `OPENAI_API_KEY`, `ANTHROPIC_API_KEY` (native fallbacks), `SONAR_TOKEN`, any Ollama cloud auth token — all referenced by env var name from `Config/llm.config` / `sonar.config`, never hard-coded.

---

## 9. Sample Input (`UserStories/UserStory-01.md`)

Already provided by the user — a user story description, an HTTP interface contract (method, endpoint, parameters, response schema), and a data model table. This file format is the fixed input contract for Stage 1; see the pasted requirements for the full `UserStory-01.md` example (`GetNearbyRecyclingFacilities` / `RecyclingFacility`).

---

## 10. Open Risks / Assumptions

- **Naming-contract compliance**: White-box test compilation only works if every LLM obeys the rules-file's naming conventions exactly, every run. LLMs may drift on this — needs validation early, and a clear failure mode (e.g. "test project failed to compile" is itself a recorded result, not a pipeline crash) rather than assuming 100% compliance.
- **Ollama cloud catalog coverage**: It's unconfirmed whether GPT-5.4 and Claude Sonnet 4.6 are actually reachable through Ollama's cloud catalog under those names. The native-SDK fallback (`openai`/`anthropic` providers in `llm.config`) covers this, but should be validated before implementation starts.
- **SonarQube analysis latency**: 12 runs per user story, each requiring a full scanner cycle, will add up — Stage 5 should run analyses sequentially per Sonar server instance (or confirm the server can safely handle concurrent analyses) and the polling timeout (`sonar.config`) should be tuned accordingly.
- **Partial failures**: A model that fails to generate a buildable project, or whose tests fail to compile, must still produce a usable row in the report (with build/test status reflecting the failure) rather than aborting the whole run.
- **Project isolation**: Each of the 12 generated projects per user story needs a fully isolated working directory (already reflected in the `run<N>/src/` structure) to avoid cross-contamination between models/runs.

---

## 11. Implementation Roadmap

Suggested build order for the actual pipeline code (future session):

1. `Config/` schemas + `llm_client.py` (LangChain routing) — validate each of the 4 models is reachable end-to-end with a trivial prompt.
2. `spec_generator.py` (Stage 1) — validate spec output against one sample user story.
3. `project_generator.py` (Stage 2) — validate a full generated project builds manually before automating.
4. `language_adapters/csharp_adapter.py` + `Stage 3` build automation.
5. `test_runner.py` (Stage 4) — the highest-risk integration point; validate the naming-contract approach here first with a single model before wiring all 4.
6. `sonar_runner.py` (Stage 5) — SonarQube server setup + scanner integration + Web API polling.
7. `report_generator.py` (Stage 6) + `run_manager.py` — tie run bookkeeping and reporting together.
8. `story2code.py` — orchestration CLI wiring all stages together across the 4×3 execution matrix.
