#nullable enable

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Community.PowerToys.Run.Plugin.AIPromptGenerator.Models;

namespace Community.PowerToys.Run.Plugin.AIPromptGenerator.Services;

/// <summary>
/// Service for interacting with AI APIs (OpenAI compatible)
/// </summary>
public class AIService : IDisposable
{
    private static readonly HttpClient HttpClient = CreateHttpClient();
    private readonly Dictionary<string, AIResponse> _cache;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public AIService()
    {
        _cache = new Dictionary<string, AIResponse>();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        return new HttpClient(handler);
    }

    /// <summary>
    /// Expand a short prompt into a detailed one using AI
    /// </summary>
    public async Task<AIResponse> ExpandPromptAsync(string userPrompt, PluginSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userPrompt);
        ArgumentNullException.ThrowIfNull(settings);

        // Validate settings
        var (isValid, errorMessage) = settings.Validate();
        if (!isValid)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = errorMessage,
                ExpandedPrompt = string.Empty
            };
        }

        // Check cache if enabled
        var cacheKey = GenerateCacheKey(userPrompt, settings);
        if (settings.EnableCaching && _cache.TryGetValue(cacheKey, out var cachedResponse))
        {
            return cachedResponse;
        }

        try
        {
            // Prepare the request
            var request = new ChatCompletionRequest
            {
                Model = settings.SelectedModel,
                Messages = new List<ChatMessage>
                {
                    new() { Role = "system", Content = settings.SystemPrompt },
                    new() { Role = "user", Content = userPrompt }
                },
                Temperature = settings.Temperature,
                MaxTokens = settings.MaxTokens
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, settings.GetEffectiveEndpoint())
            {
                Content = JsonContent.Create(request, options: _jsonOptions)
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(settings.TimeoutSeconds));
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            // Send request
            var response = await HttpClient.SendAsync(httpRequest, linkedCts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return new AIResponse
                {
                    Success = false,
                    ErrorMessage = $"API request failed: {response.StatusCode} - {errorContent}",
                    ExpandedPrompt = string.Empty
                };
            }

            // Parse response
            var completionResponse = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
                _jsonOptions,
                cancellationToken);

            if (completionResponse?.Choices == null || completionResponse.Choices.Count == 0)
            {
                return new AIResponse
                {
                    Success = false,
                    ErrorMessage = "No response from AI model",
                    ExpandedPrompt = string.Empty
                };
            }

            var expandedPrompt = completionResponse.Choices[0].Message?.Content ?? string.Empty;
            var tokensUsed = completionResponse.Usage?.TotalTokens ?? 0;

            var aiResponse = new AIResponse
            {
                Success = true,
                ExpandedPrompt = expandedPrompt.Trim(),
                TokensUsed = tokensUsed,
                Model = settings.SelectedModel
            };

            // Cache the response if enabled
            if (settings.EnableCaching)
            {
                _cache[cacheKey] = aiResponse;
            }

            return aiResponse;
        }
        catch (TaskCanceledException)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = "Request timed out. Try increasing the timeout in settings.",
                ExpandedPrompt = string.Empty
            };
        }
        catch (HttpRequestException ex)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = $"Network error: {ex.Message}",
                ExpandedPrompt = string.Empty
            };
        }
        catch (Exception ex)
        {
            return new AIResponse
            {
                Success = false,
                ErrorMessage = $"Unexpected error: {ex.Message}",
                ExpandedPrompt = string.Empty
            };
        }
    }

    /// <summary>
    /// Generate a cache key based on user prompt and settings
    /// </summary>
    private static string GenerateCacheKey(string userPrompt, PluginSettings settings)
    {
        return $"{userPrompt}|{settings.SelectedModel}|{settings.Temperature}|{settings.SystemPrompt}";
    }

    /// <summary>
    /// Clear the cache
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cache?.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// AI service response
/// </summary>
public class AIResponse
{
    public bool Success { get; set; }
    public string ExpandedPrompt { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public int TokensUsed { get; set; }
    public string? Model { get; set; }
}

/// <summary>
/// Chat completion request model (OpenAI compatible)
/// </summary>
internal class ChatCompletionRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<ChatMessage> Messages { get; set; } = new();

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }
}

/// <summary>
/// Chat message model
/// </summary>
internal class ChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Chat completion response model
/// </summary>
internal class ChatCompletionResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("choices")]
    public List<Choice>? Choices { get; set; }

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}

/// <summary>
/// Choice model
/// </summary>
internal class Choice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("message")]
    public ChatMessage? Message { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

/// <summary>
/// Usage statistics model
/// </summary>
internal class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}