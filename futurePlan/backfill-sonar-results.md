# Future Plan: Backfill SonarQube Results Into an Existing Excel Report

## Context

If a pipeline run happens while the SonarQube Docker container isn't running, every trial that
otherwise built and tested successfully ends up with empty Sonar metric columns and a stale
`Stage 5 (SonarQube) failed: ...connection refused...` note in "Build Error Summary" in the
report. The generated source code for those trials is still on disk under `Runs/<run_id>/...`,
so nothing needs to be regenerated — Stage 5 just needs to be re-run against that existing code
and the results merged into the existing report.

Today there's no way to do this: `story2code.py` (`Pipeline/story2code.py`) always runs the full
6-stage pipeline and writes a brand-new workbook (`main()` → `write_report`, `report_generator.py`).
There's no standalone "just Sonar, update in place" path — this is a real, reusable gap
(SonarQube being down mid-run is exactly the kind of thing that will happen again), not a
one-off fix.

## Approach

### 1. Extract reusable Stage 5 logic in `Pipeline/story2code.py`

Currently the Sonar-calling logic (build the project key, find `Implementation.csproj`, call
`sonar_runner.run_analysis`, map `SonarResult` fields) is inlined in `run_trial()`
(story2code.py:99-125). Pull it into a standalone function:

```python
def run_sonar_stage(model_config, run_number, user_story_id, run_id, src_dir, adapter, sonar_config) -> SonarResult
```

that always returns a `SonarResult` (with `.error` set on failure, never raises). `run_trial()`
calls this instead of inlining the logic — behavior unchanged, just reusable.

Also extract the repeated `(existing + " | " if existing else "") + f"Stage N (...) failed: {x}"`
pattern (used for both Stage 4 and Stage 5 in `run_trial()`) into a small helper:

```python
def _set_stage_error(existing: str, stage_label: str, error_text: str | None) -> str
```

which replaces any prior segment for that stage label (splitting/rejoining on `" | "`), or
clears it when `error_text` is `None`. This is what lets the backfill script cleanly remove the
stale "connection refused" note when a row's re-analysis succeeds, instead of stacking a second
note on top of it.

### 2. New `Pipeline/backfill_sonar.py` (+ root wrapper `backfill_sonar.py`, mirroring `story2code.py`'s shim)

```
python3 backfill_sonar.py <path-to-results.xlsx> [--force]
```

- Opens the workbook with `openpyxl`, reads the header row to map column names to indices
  (`report_generator.COLUMNS`).
- For each data row: skip unless `Build Status == "Success"`. Skip further unless `--force`
  is passed if `Reliability Rating` is already a real rating (`A`-`E`) — i.e. only reprocess rows
  that never got a successful Sonar analysis (empty or `"Unknown"`), so re-running the backfill
  is idempotent by default.
- Resolves `model_config` from the row's "Model" (display name) via `load_llm_config()`, and the
  trial's `src` dir from the row's "Generated Source Path" (relative to repo root).
- Calls `run_sonar_stage(...)` (same function `run_trial()` uses) and writes the returned metrics
  into that row's cells: Code Smells (Total), Code Smells - by Severity, Reliability Rating, Bug
  Count, Security Rating, Vulnerability Count, Cyclomatic Complexity, Maintainability Index,
  Sonar Project Key / Dashboard Link. Updates "Build Error Summary" via `_set_stage_error` (clears
  the stale note on success, replaces it on a new failure). Leaves every other column (build/test
  results, paths, timestamp) untouched.
- Saves the workbook back to the same path and prints a per-row summary.

### 3. Verify end-to-end

Run it for real against a report with known SonarQube-skipped rows, with `SONAR_TOKEN` set and
the SonarQube container up:
- Confirm all previously-skipped "Success" rows get real metrics and the stale connection-error
  text is gone from "Build Error Summary".
- Confirm "Not run" rows (e.g. models without API keys) are untouched (no Sonar columns
  populated, no crash from missing source path).
- Re-run the script a second time with no `--force` and confirm it skips all rows (already
  populated / not applicable) — proving idempotency.

## Verification

- `python3 backfill_sonar.py Results/results_<userstory>_<runid>.xlsx` updates exactly the
  successful-build rows lacking real Sonar metrics, visible both in the saved `.xlsx` and at
  `http://localhost:9000/projects`.
- A second run with no flags is a no-op (idempotency check).
