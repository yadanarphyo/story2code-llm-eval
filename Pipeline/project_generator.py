"""Stage 2 — Full Project Generation (PIPELINE_PLAN.md §3.2)."""
from __future__ import annotations

import re
from pathlib import Path

from config_loader import ModelConfig
from llm_client import invoke_text

SYSTEM_PROMPT_TEMPLATE = """You are a senior C# backend engineer generating a complete, \
buildable ASP.NET Core project. Follow the rules file exactly, especially the naming \
contract and output format.

--- RULES FILE ---
{rules_file}
--- END RULES FILE ---
"""

USER_PROMPT_TEMPLATE = """Here is the implementation spec you previously produced for this \
user story. Generate the complete project now: every file needed to build it, using the \
`// FILE: <relative/path>` + fenced-code-block output format from the rules file. Include the \
.csproj file itself. Do not include the test project.

--- SPEC ---
{spec}
--- END SPEC ---
"""

FILE_BLOCK_PATTERN = re.compile(
    r"^//\s*FILE:\s*(?P<path>\S.*?)\s*$\n```[a-zA-Z0-9_+-]*\n(?P<content>.*?)\n```",
    re.MULTILINE | re.DOTALL,
)


class ProjectGenerationError(RuntimeError):
    """Raised when the LLM response can't be parsed into project files."""


def parse_file_blocks(raw_text: str) -> dict[str, str]:
    files = {}
    for match in FILE_BLOCK_PATTERN.finditer(raw_text):
        rel_path = match.group("path").strip()
        content = match.group("content")
        files[rel_path] = content
    return files


def generate_project(
    model_config: ModelConfig,
    spec_text: str,
    rules_file_text: str,
    output_dir: Path,
) -> Path:
    """Calls the model to produce a full project under output_dir/src/; returns that path."""
    system_prompt = SYSTEM_PROMPT_TEMPLATE.format(rules_file=rules_file_text)
    user_prompt = USER_PROMPT_TEMPLATE.format(spec=spec_text)

    raw_response = invoke_text(model_config, system_prompt, user_prompt)

    src_dir = output_dir / "src"
    src_dir.mkdir(parents=True, exist_ok=True)
    resolved_src_dir = src_dir.resolve()
    (output_dir / "raw_generation_response.md").write_text(raw_response)

    files = parse_file_blocks(raw_response)
    if not files:
        raise ProjectGenerationError(
            f"No '// FILE: <path>' blocks found in model response "
            f"(see raw_generation_response.md in {output_dir})"
        )

    for rel_path, content in files.items():
        file_path = src_dir / rel_path
        resolved_file_path = file_path.resolve()
        if resolved_src_dir != resolved_file_path.parent and resolved_src_dir not in resolved_file_path.parents:
            raise ProjectGenerationError(f"Refusing to write outside src/: {rel_path}")
        file_path.parent.mkdir(parents=True, exist_ok=True)
        file_path.write_text(content)

    return src_dir
