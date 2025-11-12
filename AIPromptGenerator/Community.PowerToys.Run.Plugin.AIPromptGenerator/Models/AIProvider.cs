#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Community.PowerToys.Run.Plugin.AIPromptGenerator.Models;

/// <summary>
/// Supported AI service providers
/// </summary>
public enum AIProvider
{
    OpenAI,
    Groq,
    OpenRouter,
    HuggingFace,
    SambaNova
}

/// <summary>
/// AI Provider configuration
/// </summary>
public class AIProviderConfig
{
    public AIProvider Provider { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DefaultEndpoint { get; set; } = string.Empty;
    public List<string> DefaultModels { get; set; } = new();
    public bool RequiresApiKey { get; set; } = true;

    /// <summary>
    /// Get all available providers
    /// </summary>
    public static List<AIProviderConfig> GetAllProviders()
    {
        return new List<AIProviderConfig>
        {
            new()
            {
                Provider = AIProvider.OpenAI,
                Name = "OpenAI",
                DefaultEndpoint = "https://api.openai.com/v1/chat/completions",
                DefaultModels = new() { "gpt-4o", "gpt-4o-mini", "gpt-4-turbo", "gpt-4", "gpt-3.5-turbo" },
                RequiresApiKey = true
            },
            new()
            {
                Provider = AIProvider.Groq,
                Name = "Groq",
                DefaultEndpoint = "https://api.groq.com/openai/v1/chat/completions",
                DefaultModels = new()
                {
                    "llama-3.3-70b-versatile",
                    "llama-3.1-70b-versatile",
                    "llama-3.1-8b-instant",
                    "mixtral-8x7b-32768",
                    "gemma2-9b-it"
                },
                RequiresApiKey = true
            },
            new()
            {
                Provider = AIProvider.OpenRouter,
                Name = "OpenRouter",
                DefaultEndpoint = "https://openrouter.ai/api/v1/chat/completions",
                DefaultModels = new()
                {
                    "openai/gpt-4o",
                    "anthropic/claude-3.5-sonnet",
                    "google/gemini-pro-1.5",
                    "meta-llama/llama-3.1-70b-instruct",
                    "mistralai/mixtral-8x7b-instruct"
                },
                RequiresApiKey = true
            },
            new()
            {
                Provider = AIProvider.HuggingFace,
                Name = "HuggingFace",
                DefaultEndpoint = "https://api-inference.huggingface.co/models/",
                DefaultModels = new()
                {
                    "meta-llama/Llama-3.1-70B-Instruct",
                    "mistralai/Mixtral-8x7B-Instruct-v0.1",
                    "google/gemma-2-9b-it",
                    "Qwen/Qwen2.5-72B-Instruct"
                },
                RequiresApiKey = true
            },
            new()
            {
                Provider = AIProvider.SambaNova,
                Name = "SambaNova",
                DefaultEndpoint = "https://api.sambanova.ai/v1/chat/completions",
                DefaultModels = new()
                {
                    "Meta-Llama-3.1-70B-Instruct",
                    "Meta-Llama-3.1-8B-Instruct",
                    "Meta-Llama-3.2-3B-Instruct"
                },
                RequiresApiKey = true
            }
        };
    }

    /// <summary>
    /// Get provider configuration by type
    /// </summary>
    public static AIProviderConfig GetProvider(AIProvider provider)
    {
        return GetAllProviders().First(p => p.Provider == provider);
    }

    /// <summary>
    /// Get provider configuration by name
    /// </summary>
    public static AIProviderConfig? GetProviderByName(string name)
    {
        return GetAllProviders().FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}