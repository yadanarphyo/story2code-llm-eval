"""LangChain-based model abstraction (PIPELINE_PLAN.md §5.1).

Every model, regardless of whether it's reached through Ollama or a native
provider SDK, resolves to a LangChain BaseChatModel. Callers only ever use
`.invoke(prompt)` — swapping models is a Config/llm.config edit, not a code
change.
"""
from __future__ import annotations

from langchain_core.language_models.chat_models import BaseChatModel

from config_loader import ModelConfig


class LlmConfigError(RuntimeError):
    """Raised when a model can't be reached because of missing setup (API key, tag, etc)."""


def get_chat_model(model_config: ModelConfig) -> BaseChatModel:
    if model_config.provider == "ollama":
        from langchain_ollama import ChatOllama

        if not model_config.ollama_tag:
            raise LlmConfigError(f"{model_config.id}: provider=ollama but no ollama_tag set")
        return ChatOllama(model=model_config.ollama_tag, temperature=model_config.temperature)

    if model_config.provider == "openai":
        from langchain_openai import ChatOpenAI

        api_key = model_config.resolved_api_key()
        if not api_key:
            raise LlmConfigError(
                f"{model_config.id}: provider=openai requires env var "
                f"{model_config.api_key_env} to be set"
            )
        return ChatOpenAI(
            model=model_config.model_name,
            temperature=model_config.temperature,
            api_key=api_key,
        )

    if model_config.provider == "anthropic":
        from langchain_anthropic import ChatAnthropic

        api_key = model_config.resolved_api_key()
        if not api_key:
            raise LlmConfigError(
                f"{model_config.id}: provider=anthropic requires env var "
                f"{model_config.api_key_env} to be set"
            )
        return ChatAnthropic(
            model=model_config.model_name,
            temperature=model_config.temperature,
            api_key=api_key,
        )

    raise LlmConfigError(f"{model_config.id}: unknown provider '{model_config.provider}'")


def invoke_text(model_config: ModelConfig, system_prompt: str, user_prompt: str) -> str:
    """Send a system+user prompt to the configured model and return the text response."""
    chat_model = get_chat_model(model_config)
    response = chat_model.invoke(
        [
            ("system", system_prompt),
            ("human", user_prompt),
        ]
    )
    return response.content if isinstance(response.content, str) else str(response.content)
