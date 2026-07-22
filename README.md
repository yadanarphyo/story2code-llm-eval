# Story2Code

Pipeline automating LLM-driven C# API code generation, functional-correctness testing, and
SonarQube quality analysis for the dissertation *"A Comparative Analysis of LLM-Driven Code
Generation from User Stories."* Full design rationale: [PIPELINE_PLAN.md](PIPELINE_PLAN.md).

## Quick start

```bash
python3 story2code.py UserStory-01.md
```

This runs all 4 configured models (`Config/llm.config`), 3 trials each, through spec
generation → full project generation → build → tests → SonarQube analysis, and writes
`Results/results_<userstory>_<runid>.xlsx`.

To run just one model (e.g. while iterating on a `Prompts/rules-file` change), pass its `id`
from `Config/llm.config` as a second argument:

```bash
python3 story2code.py UserStory-01.md kimi-k2.6
```

## Environment status (this machine)

| Component | Status |
|---|---|
| Python 3.13 + LangChain (`langchain`, `langchain-ollama`, `langchain-openai`, `langchain-anthropic`) | ✅ Installed |
| .NET SDK 8.0.121 | ✅ Installed |
| Ollama server | ✅ Running locally, with `qwen3.5:cloud` and `kimi-k2.6:cloud` already pulled |
| SonarQube server | ✅ Running via Docker (v26.7.0), `dotnet-sonarscanner` installed, `SONAR_TOKEN` configured — Stage 5 verified live (see below) |
| GPT-5.4 / Claude Sonnet 4.6 | ❌ Not reachable — no `OPENAI_API_KEY` / `ANTHROPIC_API_KEY` set, and these aren't in Ollama's local cloud catalog. Set the env vars to enable the native LangChain fallback already wired in `Config/llm.config`. |

The pipeline is written to degrade gracefully: a model without credentials, or a Sonar server
that isn't reachable, produces a report row recording the failure rather than crashing the run.

### SonarQube token setup (already done on this machine)

1. Log into `http://localhost:9000` (default `admin`/`admin`, forced password change on first login).
2. **My Account → Security** → generate a token (e.g. `story2code-pipeline`) → copy it.
3. `export SONAR_TOKEN=<token>` in the shell that runs `story2code.py` (add to `~/.zshrc` to persist).

No manual "create project" step is needed — the pipeline mints a fresh SonarQube project key
per trial (`story2code-<userstory>-<model>-run<N>-<runID>`) and SonarQube auto-provisions each
one on first analysis, since the `admin`-issued token has the "Create Projects" permission.

### To enable GPT-5.4 / Claude Sonnet 4.6

```bash
export OPENAI_API_KEY=<your key>
export ANTHROPIC_API_KEY=<your key>
```

## What's been verified end-to-end on this machine

Two full runs of `python3 story2code.py UserStory-01.md` across all 4 configured models (3
trials each) — the second with `SONAR_TOKEN` set, producing real Sonar metrics:

| Model | Runs | Result |
|---|---|---|
| Kimi-k2.6:cloud | 3/3 | Build succeeded, 2/2 tests passed, 74-80% coverage, 1-2 code smells, A/A ratings, High maintainability |
| Qwen-3.5:cloud | 2/3 | Build + 2/2 tests + Sonar metrics on 2 runs; 1 run failed to build (genuine model variance — forgot the Swashbuckle package) |
| GPT-5.4 | 0/3 | Correctly short-circuited at Stage 1 — no `OPENAI_API_KEY` set |
| Claude Sonnet 4.6 | 0/3 | Correctly short-circuited at Stage 1 — no `ANTHROPIC_API_KEY` set |

This confirms all 6 stages work end-to-end against real models and a real SonarQube server, and
that partial failures (missing credentials, a model producing a project that doesn't build, a
naming-contract mismatch) degrade gracefully into a recorded report row instead of crashing the
run. Every trial's `build.log` / `test.log` is written to its `Runs/.../run<N>/` folder for
debugging.

### Bugs found and fixed during verification

- **Qwen used an `async`/`Task<...>` method signature**, which compiled fine on its own but
  failed to compile against the pre-written (synchronous) tests — and the adapter was
  misreading that compile failure as "0 tests, no error" because it trusted `dotnet test`'s
  exit code, which is `1` for both "some tests failed" and "test project didn't compile."
  Fixed `CSharpAdapter.run_tests` to key off whether a test summary line was actually parsed,
  and pinned synchronous method signatures explicitly in `Prompts/rules-file` (rule 3).
- **`dotnet-sonarscanner` wasn't found by subprocesses** even though it worked from an
  interactive shell — the shell's `PATH` had a literal, unexpanded `~/.dotnet/tools` entry.
  Fixed `sonar_runner.py` to resolve `~/.dotnet/tools` and set `DOTNET_ROOT` explicitly for
  every subprocess it launches.
- **`dotnet-sonarscanner`'s installed apphost was x86_64** but this machine only has the arm64
  .NET runtime (a leftover from however the global tool was originally installed) — it failed
  to load `libhostfxr.dylib` with an architecture mismatch. Fixed by invoking the tool's
  architecture-agnostic managed DLL directly via `dotnet <dll>` instead of the apphost binary.
- **A race between SonarQube's background analysis task and its measures index**: the CE task
  reports `SUCCESS` a moment before `/api/measures/component` has anything queryable, so the
  first fetch came back all-empty. Fixed `_poll_and_fetch` to retry the measures fetch briefly,
  and to treat a non-`SUCCESS` task status as an explicit error rather than silently proceeding.

## Layout

See [PIPELINE_PLAN.md](PIPELINE_PLAN.md) §2 for the full folder structure and §3–§7 for the
stage-by-stage design, config schemas, and report schema.

To add more user stories: drop `UserStories/UserStory-NN.md` (same format as `UserStory-01.md`)
and a matching pre-written test project at `Unit-Tests/UserStory-NN/`, following the naming
contract in `Prompts/rules-file`.
