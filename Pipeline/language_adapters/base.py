"""LanguageAdapter interface (PIPELINE_PLAN.md §5.3)."""
from __future__ import annotations

from dataclasses import dataclass, field
from pathlib import Path
from typing import Protocol


@dataclass
class BuildResult:
    success: bool
    log: str
    error_summary: str = ""


@dataclass
class TestResult:
    ran: bool
    passed: int = 0
    total: int = 0
    coverage_percent: float | None = None
    log: str = ""
    error_summary: str = ""


@dataclass
class SonarProperties:
    project_dir: Path
    extra_args: dict[str, str] = field(default_factory=dict)


class LanguageAdapter(Protocol):
    """Implemented once per target language; Stage 3/4/5 only ever talk to this interface."""

    def build(self, project_dir: Path) -> BuildResult: ...

    def run_tests(self, project_dir: Path, test_dir: Path) -> TestResult: ...

    def sonar_properties(self, project_dir: Path) -> SonarProperties: ...
