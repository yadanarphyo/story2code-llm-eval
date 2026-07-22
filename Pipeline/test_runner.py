"""Stage 4 — Test Execution (PIPELINE_PLAN.md §3.4)."""
from __future__ import annotations

import shutil
from pathlib import Path

from language_adapters.base import LanguageAdapter, TestResult

REPO_ROOT = Path(__file__).resolve().parent.parent
UNIT_TESTS_DIR = REPO_ROOT / "Unit-Tests"


def run_tests_for_trial(
    adapter: LanguageAdapter,
    user_story_id: str,
    src_dir: Path,
    trial_dir: Path,
) -> TestResult:
    source_test_project = UNIT_TESTS_DIR / user_story_id
    if not source_test_project.exists():
        return TestResult(
            ran=False,
            error_summary=f"No pre-written tests found at Unit-Tests/{user_story_id}",
        )

    # Fresh copy per trial: dotnet add reference mutates the test .csproj.
    working_test_dir = trial_dir / "tests"
    shutil.rmtree(working_test_dir, ignore_errors=True)
    shutil.copytree(source_test_project, working_test_dir)

    return adapter.run_tests(project_dir=src_dir, test_dir=working_test_dir)
