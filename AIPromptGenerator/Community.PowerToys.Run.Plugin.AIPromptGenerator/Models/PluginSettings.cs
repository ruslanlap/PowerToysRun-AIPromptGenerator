#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin;
using Microsoft.PowerToys.Settings.UI.Library;

namespace Community.PowerToys.Run.Plugin.AIPromptGenerator.Models;

/// <summary>
/// Settings for the AI Prompt Generator plugin
/// Integrated with PowerToys AdditionalOptions UI
/// </summary>
public class PluginSettings
{
    // Enhanced system prompt for advanced prompt engineering
    private const string DefaultSystemPrompt = @"You are an expert prompt architect specializing in crafting high-precision, reusable prompts that maximize AI model performance across diverse use cases.

## Your Primary Objective
Transform user inputs into comprehensive, well-structured prompts that yield consistent, high-quality AI outputs while maintaining clarity and actionability.

## Prompt Engineering Framework

### 1. **Role Definition**
- Establish the AI's persona, expertise level, and perspective
- Example: ""You are a senior software architect..."" or ""Act as a technical writer...""
- Consider domain-specific knowledge requirements

### 2. **Task Specification**
- Clearly articulate the desired outcome
- Define deliverable format (document, code, analysis, etc.)
- Set success criteria and measurable goals

### 3. **Contextual Information**
- Provide relevant background, domain knowledge, and situational details
- Include technical constraints, target audience, or environmental factors
- Reference existing standards, frameworks, or methodologies if applicable

### 4. **Output Requirements**
- **Format**: Specify structure (markdown, JSON, prose, code blocks, etc.)
- **Style**: Define tone (formal, conversational, technical, creative)
- **Length**: Set word/character limits or section requirements
- **Language**: Indicate preferred terminology level and jargon usage

### 5. **Constraints & Guardrails**
- Technical limitations (compatibility, dependencies, versions)
- Content restrictions (avoid certain topics, maintain neutrality)
- Quality standards (accuracy, completeness, validation requirements)
- Time or resource boundaries

### 6. **Processing Instructions**
- Provide step-by-step methodology for complex tasks
- Include decision trees or conditional logic when needed
- Specify verification steps or self-validation checks
- Define how to handle edge cases or ambiguities

### 7. **Examples & Clarifications** (Optional but Recommended)
- Provide input/output examples to illustrate expectations
- Show both ideal cases and common pitfalls to avoid

## Before Generating Output
Always restate your understanding of the request in 2-3 sentences to confirm alignment with user intent.

## Quality Checklist
Ensure the generated prompt:
- ✓ Is unambiguous and self-contained
- ✓ Minimizes need for follow-up questions
- ✓ Scales across similar use cases
- ✓ Produces consistent results with same inputs
- ✓ Includes all necessary context without redundancy";

    public string ApiKey { get; set; } = string.Empty;
    public string ProviderName { get; set; } = "OpenAI";
    public string ApiEndpoint { get; set; } = string.Empty;
    public string SelectedModel { get; set; } = string.Empty;
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 2000;
    public string SystemPrompt { get; set; } = DefaultSystemPrompt;
    public int TimeoutSeconds { get; set; } = 30;
    public bool EnableCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 10;
    public bool ShowTokenCount { get; set; } = false;

    /// <summary>
    /// Get current provider configuration
    /// </summary>
    public AIProviderConfig GetCurrentProvider()
    {
        var provider = AIProviderConfig.GetProviderByName(ProviderName);
        if (provider == null)
        {
            // Default to OpenAI if provider not found
            provider = AIProviderConfig.GetProvider(AIProvider.OpenAI);
            ProviderName = provider.Name;
        }
        return provider;
    }

    /// <summary>
    /// Get effective API endpoint
    /// </summary>
    public string GetEffectiveEndpoint()
    {
        if (!string.IsNullOrWhiteSpace(ApiEndpoint))
        {
            return ApiEndpoint;
        }

        var provider = GetCurrentProvider();
        return provider.DefaultEndpoint;
    }

    /// <summary>
    /// Get effective model
    /// </summary>
    public string GetEffectiveModel()
    {
        if (!string.IsNullOrWhiteSpace(SelectedModel))
        {
            return SelectedModel;
        }

        var provider = GetCurrentProvider();
        return provider.DefaultModels.FirstOrDefault() ?? "gpt-4o";
    }

    /// <summary>
    /// Validate settings
    /// </summary>
    public (bool IsValid, string? ErrorMessage) Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            return (false, "API key is required. Please configure it in PowerToys settings (Additional Options).");
        }

        var endpoint = GetEffectiveEndpoint();
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return (false, "API endpoint is required.");
        }

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out _))
        {
            return (false, "API endpoint must be a valid URL.");
        }

        var model = GetEffectiveModel();
        if (string.IsNullOrWhiteSpace(model))
        {
            return (false, "Model selection is required.");
        }

        return (true, null);
    }

    /// <summary>
    /// Create PowerToys plugin additional options
    /// </summary>
    public static List<PluginAdditionalOption> GetAdditionalOptions()
    {
        var providers = AIProviderConfig.GetAllProviders();
        var providerNames = providers.Select(p => p.Name).ToList();

        return new List<PluginAdditionalOption>
        {
            // AI Provider Selection
            new PluginAdditionalOption
            {
                Key = "Provider",
                DisplayLabel = "AI Provider",
                DisplayDescription = "Select the AI service provider",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Combobox,
                ComboBoxItems = providerNames.Select((name, index) => 
                    new KeyValuePair<string, string>(name, index.ToString())).ToList(),
                ComboBoxValue = 0 // Default to OpenAI
            },

            // API Key
            new PluginAdditionalOption
            {
                Key = "ApiKey",
                DisplayLabel = "API Key",
                DisplayDescription = "Your API key for the selected provider (stored securely)",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = string.Empty
            },

            // Model Selection
            new PluginAdditionalOption
            {
                Key = "Model",
                DisplayLabel = "Model (Optional)",
                DisplayDescription = "AI model name. Leave empty for provider default. OpenAI: gpt-4o, Groq: llama-3.3-70b-versatile, SambaNova: Meta-Llama-3.1-70B-Instruct, OpenRouter: openai/gpt-4o",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = string.Empty
            },

            // Custom API Endpoint
            new PluginAdditionalOption
            {
                Key = "ApiEndpoint",
                DisplayLabel = "Custom API Endpoint (Optional)",
                DisplayDescription = "Leave empty to use provider's default endpoint automatically. OpenAI: api.openai.com, Groq: api.groq.com, OpenRouter: openrouter.ai, SambaNova: api.sambanova.ai, HuggingFace: api-inference.huggingface.co",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = string.Empty
            },

            // Temperature
            new PluginAdditionalOption
            {
                Key = "Temperature",
                DisplayLabel = "Temperature",
                DisplayDescription = "Creativity level (0.0 = focused, 2.0 = creative). Default: 0.7",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = "0.7"
            },

            // Max Tokens
            new PluginAdditionalOption
            {
                Key = "MaxTokens",
                DisplayLabel = "Max Tokens",
                DisplayDescription = "Maximum response length (100-4000). Default: 2000",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = "2000"
            },

            // System Prompt
            new PluginAdditionalOption
            {
                Key = "SystemPrompt",
                DisplayLabel = "System Prompt",
                DisplayDescription = "Instructions that define how AI expands prompts (leave empty for default advanced template)",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = string.Empty
            },

            // Timeout
            new PluginAdditionalOption
            {
                Key = "TimeoutSeconds",
                DisplayLabel = "Timeout (seconds)",
                DisplayDescription = "Request timeout in seconds (5-120). Default: 30",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = "30"
            },

            // Enable Caching
            new PluginAdditionalOption
            {
                Key = "EnableCaching",
                DisplayLabel = "Enable Caching",
                DisplayDescription = "Cache responses to reduce API calls and costs",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
                Value = true
            },

            // Cache Duration
            new PluginAdditionalOption
            {
                Key = "CacheDurationMinutes",
                DisplayLabel = "Cache Duration (minutes)",
                DisplayDescription = "How long to cache responses (1-60). Default: 10",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
                TextValue = "10"
            },

            // Show Token Count
            new PluginAdditionalOption
            {
                Key = "ShowTokenCount",
                DisplayLabel = "Show Token Usage",
                DisplayDescription = "Display token count in results",
                PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
                Value = false
            }
        };
    }

    /// <summary>
    /// Load settings from PowerToys additional options
    /// </summary>
    public static PluginSettings LoadFromAdditionalOptions(IEnumerable<PluginAdditionalOption> options)
    {
        var settings = new PluginSettings();
        var optionsList = options.ToList();

        foreach (var option in optionsList)
        {
            switch (option.Key)
            {
                case "Provider":
                    var providers = AIProviderConfig.GetAllProviders();
                    if (option.ComboBoxValue >= 0 && option.ComboBoxValue < providers.Count)
                    {
                        settings.ProviderName = providers[option.ComboBoxValue].Name;
                    }
                    break;

                case "ApiKey":
                    settings.ApiKey = option.TextValue ?? string.Empty;
                    break;

                case "Model":
                    settings.SelectedModel = option.TextValue ?? string.Empty;
                    break;

                case "ApiEndpoint":
                    settings.ApiEndpoint = option.TextValue ?? string.Empty;
                    break;

                case "Temperature":
                    if (double.TryParse(option.TextValue, out var temp))
                    {
                        settings.Temperature = Math.Clamp(temp, 0.0, 2.0);
                    }
                    break;

                case "MaxTokens":
                    if (int.TryParse(option.TextValue, out var tokens))
                    {
                        settings.MaxTokens = Math.Clamp(tokens, 100, 4000);
                    }
                    break;

                case "SystemPrompt":
                    var prompt = option.TextValue ?? string.Empty;
                    settings.SystemPrompt = string.IsNullOrWhiteSpace(prompt) ? DefaultSystemPrompt : prompt;
                    break;

                case "TimeoutSeconds":
                    if (int.TryParse(option.TextValue, out var timeout))
                    {
                        settings.TimeoutSeconds = Math.Clamp(timeout, 5, 120);
                    }
                    break;

                case "EnableCaching":
                    settings.EnableCaching = option.Value;
                    break;

                case "CacheDurationMinutes":
                    if (int.TryParse(option.TextValue, out var duration))
                    {
                        settings.CacheDurationMinutes = Math.Clamp(duration, 1, 60);
                    }
                    break;

                case "ShowTokenCount":
                    settings.ShowTokenCount = option.Value;
                    break;
            }
        }

        return settings;
    }
}