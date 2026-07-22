#!/usr/bin/env python3
"""Entry point matching the documented CLI: python3 story2code.py <userStoryNN.md> [llm-model-name]"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent / "Pipeline"))

from story2code import main  # noqa: E402

if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
