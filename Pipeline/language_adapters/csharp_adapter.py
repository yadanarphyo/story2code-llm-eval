"""CSharpAdapter — build/test/sonar for .NET (PIPELINE_PLAN.md §5.3)."""
from __future__ import annotations

import re
import shutil
import subprocess
import xml.etree.ElementTree as ET
from pathlib import Path

from language_adapters.base import BuildResult, SonarProperties, TestResult

TEST_SUMMARY_PATTERN = re.compile(
    r"(?:Passed|Failed)!\s*-\s*Failed:\s*(?P<failed>\d+),\s*Passed:\s*(?P<passed>\d+),\s*"
    r"Skipped:\s*(?P<skipped>\d+),\s*Total:\s*(?P<total>\d+)"
)


def _run(cmd: list[str], cwd: Path, timeout: int = 600) -> tuple[int, str]:
    proc = subprocess.run(
        cmd, cwd=cwd, capture_output=True, text=True, timeout=timeout
    )
    output = proc.stdout + "\n" + proc.stderr
    return proc.returncode, output


def _find_csproj(directory: Path, stem: str | None = None) -> Path | None:
    candidates = sorted(directory.rglob("*.csproj"))
    if stem is not None:
        for c in candidates:
            if c.stem == stem:
                return c
        return None
    return candidates[0] if candidates else None


class CSharpAdapter:
    def build(self, project_dir: Path) -> BuildResult:
        csproj = _find_csproj(project_dir, stem="Implementation")
        if csproj is None:
            return BuildResult(
                success=False,
                log="",
                error_summary="No Implementation.csproj found under generated project directory",
            )
        try:
            code, output = _run(["dotnet", "build", str(csproj), "-c", "Release"], cwd=project_dir)
        except subprocess.TimeoutExpired as e:
            return BuildResult(success=False, log=str(e), error_summary="dotnet build timed out")
        success = code == 0
        error_summary = "" if success else "\n".join(output.splitlines()[-15:])
        return BuildResult(success=success, log=output, error_summary=error_summary)

    def run_tests(self, project_dir: Path, test_dir: Path) -> TestResult:
        impl_csproj = _find_csproj(project_dir, stem="Implementation")
        if impl_csproj is None:
            return TestResult(ran=False, error_summary="Implementation.csproj not found")

        test_csproj = _find_csproj(test_dir)
        if test_csproj is None:
            return TestResult(ran=False, error_summary="No test .csproj found in Unit-Tests dir")

        # Wire the pre-written (white-box) test project to the generated implementation.
        ref_code, ref_output = _run(
            ["dotnet", "add", str(test_csproj), "reference", str(impl_csproj)],
            cwd=test_dir,
        )
        if ref_code != 0:
            return TestResult(
                ran=False,
                log=ref_output,
                error_summary="Failed to add project reference (naming-contract mismatch?)",
            )

        results_dir = test_dir / "TestResults"
        shutil.rmtree(results_dir, ignore_errors=True)
        try:
            code, output = _run(
                [
                    "dotnet", "test", str(test_csproj),
                    "--logger", "trx;LogFileName=results.trx",
                    "--collect", "XPlat Code Coverage",
                    "--results-directory", str(results_dir),
                ],
                cwd=test_dir,
                timeout=900,
            )
        except subprocess.TimeoutExpired as e:
            return TestResult(ran=False, log=str(e), error_summary="dotnet test timed out")

        passed, total = self._parse_summary(output)
        coverage = self._parse_coverage(results_dir)

        # dotnet test exits 1 both for "some tests failed" and "the test project failed to
        # compile" (e.g. a naming-contract mismatch such as a sync/async signature mismatch
        # between the generated service and the pre-written tests). Exit code alone can't
        # distinguish those, so treat "no summary line found" as a real failure to run,
        # regardless of exit code, and surface the underlying compiler/test-host error.
        ran = TEST_SUMMARY_PATTERN.search(output) is not None
        error_summary = "" if ran else "\n".join(output.splitlines()[-15:])
        return TestResult(
            ran=ran,
            passed=passed,
            total=total,
            coverage_percent=coverage,
            log=output,
            error_summary=error_summary,
        )

    def sonar_properties(self, project_dir: Path) -> SonarProperties:
        return SonarProperties(project_dir=project_dir, extra_args={})

    @staticmethod
    def _parse_summary(output: str) -> tuple[int, int]:
        m = TEST_SUMMARY_PATTERN.search(output)
        if not m:
            return 0, 0
        return int(m.group("passed")), int(m.group("total"))

    @staticmethod
    def _parse_coverage(results_dir: Path) -> float | None:
        coverage_files = list(results_dir.rglob("coverage.cobertura.xml"))
        if not coverage_files:
            return None
        try:
            root = ET.parse(coverage_files[0]).getroot()
            line_rate = root.attrib.get("line-rate")
            return round(float(line_rate) * 100, 2) if line_rate is not None else None
        except (ET.ParseError, ValueError):
            return None
