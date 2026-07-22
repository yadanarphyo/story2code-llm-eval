"""Stage 6 — Report Generation (PIPELINE_PLAN.md §3.6, §7)."""
from __future__ import annotations

from dataclasses import dataclass, field
from datetime import datetime
from pathlib import Path

from openpyxl import Workbook
from openpyxl.styles import Font

COLUMNS = [
    "Run ID",
    "User Story ID",
    "Model",
    "Run Number",
    "Build Status",
    "Build Error Summary",
    "Tests Passed / Total",
    "Test Pass Rate (%)",
    "Code Coverage (%)",
    "Code Smells (Total)",
    "Code Smells - by Severity",
    "Reliability Rating",
    "Bug Count",
    "Security Rating",
    "Vulnerability Count",
    "Cyclomatic Complexity",
    "Maintainability Index",
    "Spec File Path",
    "Generated Source Path",
    "Sonar Project Key / Dashboard Link",
    "Timestamp",
]


@dataclass
class TrialResult:
    run_id: str
    user_story_id: str
    model_display_name: str
    run_number: int
    build_success: bool | None = None
    build_error_summary: str = ""
    tests_passed: int = 0
    tests_total: int = 0
    coverage_percent: float | None = None
    code_smells_total: int = 0
    code_smells_by_severity: dict[str, int] = field(default_factory=dict)
    reliability_rating: str = ""
    bug_count: int = 0
    security_rating: str = ""
    vulnerability_count: int = 0
    complexity_total: int = 0
    maintainability: str = ""
    spec_file_path: str = ""
    generated_source_path: str = ""
    sonar_project_key: str = ""
    sonar_dashboard_url: str = ""
    timestamp: str = field(default_factory=lambda: datetime.now().isoformat(timespec="seconds"))

    def to_row(self) -> list:
        test_pass_rate = (
            round(100 * self.tests_passed / self.tests_total, 1) if self.tests_total else ""
        )
        severity_str = ", ".join(f"{v} {k}" for k, v in self.code_smells_by_severity.items())
        build_status = (
            "Success" if self.build_success is True
            else "Fail" if self.build_success is False
            else "Not run"
        )
        sonar_link = self.sonar_dashboard_url or self.sonar_project_key
        return [
            self.run_id,
            self.user_story_id,
            self.model_display_name,
            self.run_number,
            build_status,
            self.build_error_summary,
            f"{self.tests_passed}/{self.tests_total}" if self.tests_total else "",
            test_pass_rate,
            self.coverage_percent if self.coverage_percent is not None else "",
            self.code_smells_total,
            severity_str,
            self.reliability_rating,
            self.bug_count,
            self.security_rating,
            self.vulnerability_count,
            self.complexity_total,
            self.maintainability,
            self.spec_file_path,
            self.generated_source_path,
            sonar_link,
            self.timestamp,
        ]


def write_report(results: list[TrialResult], output_path: Path) -> Path:
    output_path.parent.mkdir(parents=True, exist_ok=True)
    wb = Workbook()
    ws = wb.active
    ws.title = "Results"

    ws.append(COLUMNS)
    for cell in ws[1]:
        cell.font = Font(bold=True)

    for result in results:
        ws.append(result.to_row())

    for col_cells in ws.columns:
        max_len = max((len(str(c.value)) for c in col_cells if c.value is not None), default=10)
        ws.column_dimensions[col_cells[0].column_letter].width = min(max_len + 2, 60)

    wb.save(output_path)
    return output_path
