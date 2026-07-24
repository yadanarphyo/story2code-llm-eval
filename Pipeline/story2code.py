"""CLI orchestrator (PIPELINE_PLAN.md §3, §11 step 8).

Usage:
    python3 story2code.py <userStoryNN.md> [llm-model-name]

For each configured LLM (or just the one named by the optional second argument — matched
against a model's `id` in Config/llm.config, e.g. `kimi-k2.6`, or its display name), runs
`runs_per_model` trials (default 3) through Stages 1-5 (spec generation -> full project
generation -> build -> test -> SonarQube), then writes one Excel report row per trial (Stage 6).
"""
from __future__ import annotations

import sys
import traceback
from pathlib import Path

from config_loader import ModelConfig, load_llm_config, load_sonar_config
from language_adapters.csharp_adapter import CSharpAdapter
from language_adapters.base import LanguageAdapter
from project_generator import generate_project
from report_generator import TrialResult, write_report
from run_manager import RunManager
from sonar_runner import run_analysis
from spec_generator import generate_spec
from test_runner import run_tests_for_trial

REPO_ROOT = Path(__file__).resolve().parent.parent
USER_STORIES_DIR = REPO_ROOT / "UserStories"
PROMPTS_DIR = REPO_ROOT / "Prompts"
RESULTS_DIR = REPO_ROOT / "Results"


def resolve_user_story_path(arg: str) -> Path:
    candidate = Path(arg)
    if candidate.exists():
        return candidate
    fallback = USER_STORIES_DIR / arg
    if fallback.exists():
        return fallback
    raise FileNotFoundError(f"Could not find user story file: {arg} (also tried {fallback})")


def sanitize_key_component(text: str) -> str:
    return "".join(c if c.isalnum() or c in "-_." else "-" for c in text)


def resolve_model_filter(models: list[ModelConfig], name: str) -> ModelConfig:
    for model_config in models:
        if model_config.id == name or model_config.display_name.lower() == name.lower():
            return model_config
    valid_ids = ", ".join(m.id for m in models)
    raise ValueError(f"Unknown model '{name}'. Valid options: {valid_ids}")


def run_trial(
    model_config: ModelConfig,
    run_number: int,
    user_story_id: str,
    user_story_text: str,
    rules_file_text: str,
    trial_dir: Path,
    run_id: str,
    adapter: LanguageAdapter,
    sonar_config,
) -> TrialResult:
    result = TrialResult(
        run_id=run_id,
        user_story_id=user_story_id,
        model_display_name=model_config.display_name,
        run_number=run_number,
    )

    # Stage 1: spec generation
    try:
        spec_path = generate_spec(model_config, user_story_text, rules_file_text, trial_dir)
        result.spec_file_path = str(spec_path.relative_to(REPO_ROOT))
        spec_text = spec_path.read_text()
    except Exception as e:  # noqa: BLE001 - record, don't crash the run
        result.build_error_summary = f"Stage 1 (spec generation) failed: {e}"
        return result

    # Stage 2: full project generation
    try:
        src_dir = generate_project(model_config, spec_text, rules_file_text, trial_dir)
        result.generated_source_path = str(src_dir.relative_to(REPO_ROOT))
    except Exception as e:  # noqa: BLE001 - record, don't crash the run
        result.build_error_summary = f"Stage 2 (project generation) failed: {e}"
        return result

    # Stage 3: build
    build_result = adapter.build(src_dir)
    (trial_dir / "build.log").write_text(build_result.log)
    result.build_success = build_result.success
    result.build_error_summary = build_result.error_summary
    if not build_result.success:
        return result

    # Stage 4: tests
    test_result = run_tests_for_trial(adapter, user_story_id, src_dir, trial_dir)
    (trial_dir / "test.log").write_text(test_result.log)
    result.tests_passed = test_result.passed
    result.tests_total = test_result.total
    result.coverage_percent = test_result.coverage_percent
    if not test_result.ran and test_result.error_summary:
        result.build_error_summary = (result.build_error_summary + " | " if result.build_error_summary else "") + \
            f"Stage 4 (tests) failed: {test_result.error_summary}"

    # Stage 5: SonarQube
    project_key = sanitize_key_component(
        f"{sonar_config.project_key_prefix}-{user_story_id}-{model_config.id}-run{run_number}-{run_id}"
    )
    try:
        sonar_props = adapter.sonar_properties(src_dir)
        csproj_path = next(src_dir.rglob("Implementation.csproj"), None)
        if csproj_path is None:
            raise RuntimeError("Implementation.csproj not found for Sonar analysis")
        sonar_result = run_analysis(sonar_config, sonar_props, project_key, csproj_path)
        result.sonar_project_key = sonar_result.project_key
        result.sonar_dashboard_url = sonar_result.dashboard_url
        if sonar_result.error:
            result.build_error_summary = (result.build_error_summary + " | " if result.build_error_summary else "") + \
                f"Stage 5 (SonarQube) failed: {sonar_result.error}"
        else:
            result.code_smells_total = sonar_result.code_smells_total
            result.code_smells_by_severity = sonar_result.code_smells_by_severity
            result.reliability_rating = sonar_result.reliability_rating
            result.bug_count = sonar_result.bug_count
            result.security_rating = sonar_result.security_rating
            result.vulnerability_count = sonar_result.vulnerability_count
            result.complexity_total = sonar_result.complexity_total
            result.maintainability = sonar_result.maintainability
    except Exception as e:  # noqa: BLE001 - Sonar issues shouldn't abort the whole run
        result.build_error_summary = (result.build_error_summary + " | " if result.build_error_summary else "") + \
            f"Stage 5 (SonarQube) failed: {e}"

    return result


def main(argv: list[str]) -> int:
    if not (1 <= len(argv) <= 2):
        print("Usage: python3 story2code.py <userStoryNN.md> [llm-model-name]", file=sys.stderr)
        return 2

    user_story_path = resolve_user_story_path(argv[0])
    user_story_id = user_story_path.stem
    user_story_text = user_story_path.read_text()
    rules_file_text = (PROMPTS_DIR / "rules-file").read_text()

    llm_config = load_llm_config()
    sonar_config = load_sonar_config()
    adapter = CSharpAdapter()

    models_to_run = llm_config.models
    if len(argv) == 2:
        try:
            models_to_run = [resolve_model_filter(llm_config.models, argv[1])]
        except ValueError as e:
            print(e, file=sys.stderr)
            return 2

    run_manager = RunManager.start_new_run(user_story_id)
    print(f"Started run {run_manager.run_id} for {user_story_id}")

    results: list[TrialResult] = []
    for model_config in models_to_run:
        for run_number in range(1, llm_config.runs_per_model + 1):
            print(f"--- {model_config.display_name} run {run_number}/{llm_config.runs_per_model} ---")
            trial_dir = run_manager.trial_dir(model_config.id, run_number)
            try:
                result = run_trial(
                    model_config, run_number, user_story_id, user_story_text, rules_file_text,
                    trial_dir, run_manager.run_id, adapter, sonar_config,
                )
            except Exception as e:  # noqa: BLE001 - one trial's crash must not lose the others
                traceback.print_exc()
                result = TrialResult(
                    run_id=run_manager.run_id,
                    user_story_id=user_story_id,
                    model_display_name=model_config.display_name,
                    run_number=run_number,
                    build_error_summary=f"Unhandled pipeline error: {e}",
                )
            results.append(result)
            print(f"    build={result.build_success} tests={result.tests_passed}/{result.tests_total}")

    report_path = RESULTS_DIR / f"results_{user_story_id}_{run_manager.run_id}.xlsx"
    write_report(results, report_path)
    print(f"Report written to {report_path.relative_to(REPO_ROOT)}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
