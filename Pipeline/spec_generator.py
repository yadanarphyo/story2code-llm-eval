"""Stage 1 — Spec Generation (PIPELINE_PLAN.md §3.1)."""
from __future__ import annotations

from pathlib import Path

from config_loader import ModelConfig
from llm_client import invoke_text

SYSTEM_PROMPT_TEMPLATE = """You are a senior C# backend engineer producing an implementation \
specification, not code yet. Follow the rules file exactly.

--- RULES FILE ---
{rules_file}
--- END RULES FILE ---
"""

USER_PROMPT_TEMPLATE = """Here is the user story, API interface contract, and data model. \
Produce the Stage 1 spec markdown document described in the rules file's "Output format" \
section: your intended architecture, file list, and how it satisfies the naming contract. \
Do not include full source code.

--- USER STORY ---
{user_story}
--- END USER STORY ---
"""


def generate_spec(
    model_config: ModelConfig,
    user_story_text: str,
    rules_file_text: str,
    output_dir: Path,
) -> Path:
    """Calls the model to produce spec.md in output_dir; returns the file path."""
    system_prompt = SYSTEM_PROMPT_TEMPLATE.format(rules_file=rules_file_text)
    user_prompt = USER_PROMPT_TEMPLATE.format(user_story=user_story_text)

    spec_text = invoke_text(model_config, system_prompt, user_prompt)

    output_dir.mkdir(parents=True, exist_ok=True)
    spec_path = output_dir / "spec.md"
    spec_path.write_text(spec_text)
    return spec_path
