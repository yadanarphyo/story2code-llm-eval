"""Creates and tracks the Runs/<date>_run<n>/US-xx_<model>/run<1-3>/ hierarchy."""
from __future__ import annotations

import re
from dataclasses import dataclass
from datetime import date
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
RUNS_DIR = REPO_ROOT / "Runs"


def _next_run_number(today: str) -> int:
    pattern = re.compile(rf"^{re.escape(today)}_run(\d+)$")
    existing = []
    if RUNS_DIR.exists():
        for entry in RUNS_DIR.iterdir():
            m = pattern.match(entry.name)
            if m:
                existing.append(int(m.group(1)))
    return max(existing, default=0) + 1


@dataclass
class RunManager:
    user_story_id: str
    run_root: Path

    @classmethod
    def start_new_run(cls, user_story_id: str) -> "RunManager":
        today = date.today().isoformat()
        run_number = _next_run_number(today)
        run_root = RUNS_DIR / f"{today}_run{run_number}"
        run_root.mkdir(parents=True, exist_ok=False)
        return cls(user_story_id=user_story_id, run_root=run_root)

    def trial_dir(self, model_id: str, trial_number: int) -> Path:
        """Return (creating if needed) the working dir for one (model, trial) execution."""
        model_folder = f"{self.user_story_id}_{model_id}"
        trial_dir = self.run_root / model_folder / f"run{trial_number}"
        trial_dir.mkdir(parents=True, exist_ok=True)
        return trial_dir

    @property
    def run_id(self) -> str:
        return self.run_root.name
