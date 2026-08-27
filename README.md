# Story2Code

An automated research pipeline that drives multiple LLMs through the same
software-engineering task — turning a user story into a working C# Web API — and
measures the result on functional correctness and code quality. Built for the
dissertation *"A Comparative Analysis of LLM-Driven Code Generation from User
Stories."*

For each user story, the pipeline runs every configured model **3 times** through
spec generation → full project generation → build → unit tests → SonarQube static
analysis, and writes one Excel report row per (user story × model × run). Full design
rationale and schemas: [PIPELINE_PLAN.md](PIPELINE_PLAN.md).

## How it works

```
UserStory.md
     │
     ▼
[1] Spec Generation ───────► spec.md              (LLM: story + rules → implementation spec)
     ▼
[2] Project Generation ────► src/                 (LLM: spec + rules → full buildable project)
     ▼
[3] Build ──────────────────► pass/fail + compiler errors
     ▼
[4] Test Execution ─────────► pre-written unit tests run white-box against the generated code
     ▼
[5] SonarQube Analysis ─────► code smells, ratings, complexity, maintainability
     ▼
[6] Report Generation ──────► Results/results_<userstory>_<runid>.xlsx
```

Each model generates the **entire project from scratch** — no fixed template — so the
comparison reflects a model's own architectural choices. A shared prompt contract
(`Prompts/rules-file`) pins the naming conventions (project/assembly name, namespace,
method signatures) needed so the pre-written test project can be compiled directly
against whatever the model produces.

## Technology stack

**Pipeline / orchestration**
- Python 3.13
- [LangChain](https://python.langchain.com/) (`langchain`, `langchain-community`) as the
  uniform LLM interface across providers
- `langchain-ollama`, `langchain-openai`, `langchain-anthropic`, `langchain-google-genai`
  — provider integrations selected per model via `Config/llm.config`
- `pandas` / `openpyxl` for the Excel results report
- `pyyaml` for config parsing, `requests` for the SonarQube Web API

**Generated code target**
- C# / .NET 8 SDK (`dotnet build`, `dotnet test`)
- xUnit + `xunit.runner.visualstudio` for the pre-written test suites
- `coverlet.collector` for code coverage

**Code quality analysis**
- [SonarQube](https://www.sonarsource.com/products/sonarqube/) (Community Edition, via
  Docker) + `dotnet-sonarscanner`, queried through its Web API for code smells,
  reliability/security ratings, complexity, and maintainability

**LLMs under comparison** (configured in `Config/llm.config`, routed via LangChain to
either Ollama's cloud catalog or a native provider SDK)
- Kimi-k2.6, Qwen-3.5, DeepSeek-V4-Pro *(Ollama cloud)*
- GPT-5.4 *(OpenAI)*
- Claude Sonnet 4.6 *(Anthropic)*
- Gemini 3.1 Pro *(Google)*

## Project layout

```
Story2Code/
├── story2code.py           # entry point
├── Pipeline/                # pipeline source (Python)
│   ├── story2code.py         # CLI orchestrator
│   ├── llm_client.py          # LangChain model routing
│   ├── spec_generator.py      # Stage 1
│   ├── project_generator.py   # Stage 2
│   ├── language_adapters/     # build/test/sonar per target language (C# today)
│   ├── test_runner.py         # Stage 4
│   ├── sonar_runner.py        # Stage 5
│   ├── report_generator.py    # Stage 6
│   └── run_manager.py         # run/repeat folder bookkeeping
├── UserStories/              # input user stories (description + API contract + data model)
├── Unit-Tests/                # pre-written xUnit test project per user story
├── Prompts/rules-file        # naming/structure contract given to every LLM
├── Config/                   # llm.config, sonar.config
├── Runs/                     # generated spec + source per (date, model, run)
└── Results/                  # Excel reports, one per pipeline invocation
```

## Quick start

Prerequisites: Python 3.13, .NET 8 SDK, a running SonarQube server, and Ollama (for
the cloud-routed models) or the relevant provider API keys.

```bash
pip install -r Pipeline/requirements.txt
export SONAR_TOKEN=<your sonarqube token>
# and whichever of these the configured models need:
export OPENAI_API_KEY=<...>
export ANTHROPIC_API_KEY=<...>
export GEMINI_API_KEY=<...>

python3 story2code.py UserStory-01.md
```

This runs every model in `Config/llm.config`, 3 trials each, and writes
`Results/results_UserStory-01_<runid>.xlsx`.

To iterate on a single model (e.g. while tuning `Prompts/rules-file`), pass its `id`
from `Config/llm.config` as a second argument:

```bash
python3 story2code.py UserStory-01.md kimi-k2.6
```

### Adding a new user story

Drop `UserStories/UserStory-NN.md` (same format as `UserStory-01.md`: description +
HTTP interface contract + data model) and a matching pre-written xUnit test project at
`Unit-Tests/UserStory-NN/`, following the naming contract in `Prompts/rules-file`.

The pipeline degrades gracefully: a model without credentials, a project that fails to
build, or an unreachable Sonar server all produce a report row recording the failure
rather than crashing the run.
