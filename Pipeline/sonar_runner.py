"""Stage 5 — SonarQube Analysis (PIPELINE_PLAN.md §3.5).

Runs dotnet-sonarscanner around a `dotnet build`, then polls the SonarQube Web API for the
metrics table documented in PIPELINE_PLAN.md: code smells (+ severity breakdown), reliability
rating/bugs, security rating/vulnerabilities, cyclomatic complexity, maintainability.

Note on "Maintainability Index": SonarQube doesn't expose a literal Maintainability Index —
its closest equivalent is the maintainability rating (sqale_rating, A-E) derived from technical
debt ratio. This module maps that rating to High/Medium/Low (A/B -> High, C -> Medium, D/E ->
Low) to match the requirements table's output format. This mapping is a documented assumption,
not a SonarQube-native metric.

Note on cyclomatic complexity: the Sonar Web API's `complexity` measure is an aggregate over
the project/component, not literally per-function ("foo: 4, bar: 7" from the requirements
table). This module additionally fetches per-file complexity via the component tree as the
closest practical approximation; true per-function breakdown would require parsing Sonar's
internal symbol data, which isn't exposed through the public Web API.
"""
from __future__ import annotations

import os
import shutil
import subprocess
import time
from dataclasses import dataclass, field
from pathlib import Path

import requests

from config_loader import SonarConfig
from language_adapters.base import SonarProperties

MEASURE_KEYS = [
    "code_smells",
    "reliability_rating",
    "bugs",
    "security_rating",
    "vulnerabilities",
    "complexity",
    "sqale_rating",
]

MAINTAINABILITY_MAP = {"1.0": "High", "2.0": "High", "3.0": "Medium", "4.0": "Low", "5.0": "Low"}


class SonarError(RuntimeError):
    pass


@dataclass
class SonarResult:
    project_key: str
    dashboard_url: str
    code_smells_total: int = 0
    code_smells_by_severity: dict[str, int] = field(default_factory=dict)
    reliability_rating: str = ""
    bug_count: int = 0
    security_rating: str = ""
    vulnerability_count: int = 0
    complexity_total: int = 0
    complexity_by_file: dict[str, int] = field(default_factory=dict)
    maintainability: str = ""
    error: str = ""


def _subprocess_env() -> dict[str, str]:
    # `dotnet tool install --global` puts binaries in ~/.dotnet/tools, but a shell profile's
    # PATH entry for it (often a literal "~/.dotnet/tools") isn't expanded for subprocesses,
    # only by an interactive shell. Resolve it explicitly so dotnet-sonarscanner is found
    # regardless of how the caller's shell is configured.
    env = os.environ.copy()
    dotnet_tools_dir = str(Path.home() / ".dotnet" / "tools")
    env["PATH"] = f"{dotnet_tools_dir}:{env.get('PATH', '')}"

    # dotnet-sonarscanner's apphost also needs DOTNET_ROOT to locate the runtime; without a
    # shell-configured DOTNET_ROOT it can fail to resolve even with `dotnet` on PATH.
    dotnet_bin = shutil.which("dotnet")
    if dotnet_bin and "DOTNET_ROOT" not in env:
        env["DOTNET_ROOT"] = str(Path(dotnet_bin).resolve().parent)

    return env


def _run(cmd: list[str], cwd: Path, timeout: int = 600) -> tuple[int, str]:
    proc = subprocess.run(
        cmd, cwd=cwd, capture_output=True, text=True, timeout=timeout, env=_subprocess_env()
    )
    return proc.returncode, proc.stdout + "\n" + proc.stderr


def _sonarscanner_entrypoint() -> list[str]:
    """Resolve how to invoke dotnet-sonarscanner.

    `dotnet tool install --global` can produce an x86_64 apphost even on Apple Silicon
    (e.g. if it was installed via an x64/Rosetta dotnet at some point), which then fails to
    load an arm64-only host runtime. The tool's actual payload is an architecture-agnostic
    managed DLL, so invoking it via `dotnet <dll>` — using this machine's native `dotnet` —
    sidesteps that mismatch entirely.
    """
    dll_candidates = sorted(
        (Path.home() / ".dotnet" / "tools" / ".store" / "dotnet-sonarscanner").glob(
            "*/dotnet-sonarscanner/*/tools/*/any/SonarScanner.MSBuild.dll"
        )
    )
    if dll_candidates:
        return ["dotnet", str(dll_candidates[-1])]
    return ["dotnet-sonarscanner"]


def run_analysis(
    sonar_config: SonarConfig,
    sonar_props: SonarProperties,
    project_key: str,
    csproj_path: Path,
) -> SonarResult:
    token = sonar_config.resolved_token()
    if not token:
        return SonarResult(
            project_key=project_key,
            dashboard_url="",
            error=f"SonarQube token not set (env var {sonar_config.token_env})",
        )

    project_dir = sonar_props.project_dir
    scanner = _sonarscanner_entrypoint()
    begin_cmd = [
        *scanner, "begin",
        f"/k:{project_key}",
        f"/d:sonar.host.url={sonar_config.host_url}",
        f"/d:sonar.token={token}",
        # Generated trial code lives under Runs/, which .gitignore excludes wholesale. The
        # scanner treats git-ignored files as excluded from analysis by default, which would
        # silently produce an empty analysis for every trial — override that here.
        "/d:sonar.scm.exclusions.disabled=true",
    ]
    for k, v in sonar_props.extra_args.items():
        begin_cmd.append(f"/d:{k}={v}")

    code, output = _run(begin_cmd, cwd=project_dir)
    if code != 0:
        return SonarResult(project_key=project_key, dashboard_url="", error=output[-2000:])

    code, output = _run(["dotnet", "build", str(csproj_path), "-c", "Release"], cwd=project_dir)
    if code != 0:
        _run([*scanner, "end", f"/d:sonar.token={token}"], cwd=project_dir)
        return SonarResult(
            project_key=project_key, dashboard_url="", error=f"build failed during analysis: {output[-2000:]}"
        )

    code, output = _run([*scanner, "end", f"/d:sonar.token={token}"], cwd=project_dir)
    if code != 0:
        return SonarResult(project_key=project_key, dashboard_url="", error=output[-2000:])

    return _poll_and_fetch(sonar_config, project_key, token)


def _poll_and_fetch(sonar_config: SonarConfig, project_key: str, token: str) -> SonarResult:
    dashboard_url = f"{sonar_config.host_url}/dashboard?id={project_key}"
    deadline = time.time() + sonar_config.poll_timeout_seconds
    task_status = None

    while time.time() < deadline:
        resp = requests.get(
            f"{sonar_config.host_url}/api/ce/component",
            params={"component": project_key},
            auth=(token, ""),
            timeout=30,
        )
        if resp.ok:
            current = resp.json().get("current")
            task_status = current.get("status") if current else None
            if task_status not in ("PENDING", "IN_PROGRESS", None):
                break
        time.sleep(sonar_config.poll_interval_seconds)
    else:
        return SonarResult(
            project_key=project_key, dashboard_url=dashboard_url, error="timed out waiting for analysis"
        )

    if task_status != "SUCCESS":
        return SonarResult(
            project_key=project_key, dashboard_url=dashboard_url,
            error=f"SonarQube background task finished with status {task_status}",
        )

    result = SonarResult(project_key=project_key, dashboard_url=dashboard_url)

    # The CE task can report SUCCESS a moment before every measure is queryable — issue-count
    # metrics (bugs/code_smells/vulnerabilities) tend to appear before size/rating metrics like
    # complexity and sqale_rating do. Retry until all requested metrics are present rather than
    # accepting the first (possibly partial) non-empty response, which would silently read as
    # e.g. complexity=0 for a project that actually has real complexity a moment later.
    measures_resp = None
    for _ in range(10):
        measures_resp = requests.get(
            f"{sonar_config.host_url}/api/measures/component",
            params={"component": project_key, "metricKeys": ",".join(MEASURE_KEYS)},
            auth=(token, ""),
            timeout=30,
        )
        if measures_resp.ok and len(
            measures_resp.json().get("component", {}).get("measures", [])
        ) == len(MEASURE_KEYS):
            break
        time.sleep(2)

    if not measures_resp.ok:
        result.error = f"measures API error: {measures_resp.status_code} {measures_resp.text}"
        return result

    measures = {m["metric"]: m.get("value") for m in measures_resp.json().get("component", {}).get("measures", [])}
    result.code_smells_total = int(measures.get("code_smells", 0) or 0)
    result.reliability_rating = _rating_letter(measures.get("reliability_rating"))
    result.bug_count = int(measures.get("bugs", 0) or 0)
    result.security_rating = _rating_letter(measures.get("security_rating"))
    result.vulnerability_count = int(measures.get("vulnerabilities", 0) or 0)
    result.complexity_total = int(float(measures.get("complexity", 0) or 0))
    result.maintainability = MAINTAINABILITY_MAP.get(measures.get("sqale_rating", ""), "Unknown")

    result.code_smells_by_severity = _fetch_severity_breakdown(sonar_config, project_key, token)
    result.complexity_by_file = _fetch_complexity_by_file(sonar_config, project_key, token)

    return result


def _rating_letter(raw_value: str | None) -> str:
    mapping = {"1.0": "A", "2.0": "B", "3.0": "C", "4.0": "D", "5.0": "E"}
    return mapping.get(raw_value or "", "Unknown")


def _fetch_severity_breakdown(sonar_config: SonarConfig, project_key: str, token: str) -> dict[str, int]:
    resp = requests.get(
        f"{sonar_config.host_url}/api/issues/search",
        params={"componentKeys": project_key, "types": "CODE_SMELL", "facets": "severities", "ps": 1},
        auth=(token, ""),
        timeout=30,
    )
    if not resp.ok:
        return {}
    breakdown = {}
    for facet in resp.json().get("facets", []):
        if facet["property"] == "severities":
            for value in facet["values"]:
                breakdown[value["val"]] = value["count"]
    return breakdown


def _fetch_complexity_by_file(sonar_config: SonarConfig, project_key: str, token: str) -> dict[str, int]:
    resp = requests.get(
        f"{sonar_config.host_url}/api/measures/component_tree",
        params={"component": project_key, "metricKeys": "complexity", "qualifiers": "FIL", "ps": 500},
        auth=(token, ""),
        timeout=30,
    )
    if not resp.ok:
        return {}
    breakdown = {}
    for component in resp.json().get("components", []):
        for measure in component.get("measures", []):
            if measure["metric"] == "complexity":
                breakdown[component["path"]] = int(float(measure["value"]))
    return breakdown
