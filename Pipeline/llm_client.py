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

    if model_config.provider == "google":
        from langchain_google_genai import ChatGoogleGenerativeAI

        api_key = model_config.resolved_api_key()
        if not api_key:
            raise LlmConfigError(
                f"{model_config.id}: provider=google requires env var "
                f"{model_config.api_key_env} to be set"
            )
        return ChatGoogleGenerativeAI(
            model=model_config.model_name,
            temperature=model_config.temperature,
            google_api_key=api_key,
        )

    raise LlmConfigError(f"{model_config.id}: unknown provider '{model_config.provider}'")


def _content_to_text(content) -> str:
    """Flatten a LangChain message's content to plain text.

    Most providers return a plain string, but some (e.g. Gemini 3.x) return a list of
    structured content blocks like [{"type": "text", "text": "..."}]. Concatenate the
    text of those blocks rather than str()-ing the whole list, which would leak Python
    repr syntax (escaped newlines, provider "signature"/"extras" metadata) into the
    downstream parsers.
    """
    if isinstance(content, str):
        return content
    if isinstance(content, list):
        parts: list[str] = []
        for block in content:
            if isinstance(block, str):
                parts.append(block)
            elif isinstance(block, dict) and isinstance(block.get("text"), str):
                parts.append(block["text"])
        return "".join(parts)
    return str(content)


def invoke_text(model_config: ModelConfig, system_prompt: str, user_prompt: str) -> str:
    """Send a system+user prompt to the configured model and return the text response."""
    chat_model = get_chat_model(model_config)
    response = chat_model.invoke(
        [
            ("system", system_prompt),
            ("human", user_prompt),
        ]
    )
    return _content_to_text(response.content)
