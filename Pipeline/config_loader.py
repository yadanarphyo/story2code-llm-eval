"""Loads and resolves Config/llm.config and Config/sonar.config."""
from __future__ import annotations

import os
from dataclasses import dataclass
from pathlib import Path

import yaml

REPO_ROOT = Path(__file__).resolve().parent.parent
CONFIG_DIR = REPO_ROOT / "Config"


@dataclass
class ModelConfig:
    id: str
    display_name: str
    provider: str  # "ollama" | "openai" | "anthropic" | "google"
    temperature: float
    ollama_tag: str | None = None
    model_name: str | None = None
    api_key_env: str | None = None

    def resolved_api_key(self) -> str | None:
        if self.api_key_env is None:
            return None
        return os.environ.get(self.api_key_env)


@dataclass
class LlmConfig:
    models: list[ModelConfig]
    runs_per_model: int


@dataclass
class SonarConfig:
    host_url: str
    token_env: str
    project_key_prefix: str
    quality_profile: str
    poll_interval_seconds: int
    poll_timeout_seconds: int

    def resolved_token(self) -> str | None:
        return os.environ.get(self.token_env)


def load_llm_config(path: Path | None = None) -> LlmConfig:
    path = path or (CONFIG_DIR / "llm.config")
    raw = yaml.safe_load(path.read_text())
    models = [ModelConfig(**m) for m in raw["models"]]
    return LlmConfig(models=models, runs_per_model=raw.get("runs_per_model", 3))


def load_sonar_config(path: Path | None = None) -> SonarConfig:
    path = path or (CONFIG_DIR / "sonar.config")
    raw = yaml.safe_load(path.read_text())
    return SonarConfig(**raw)
