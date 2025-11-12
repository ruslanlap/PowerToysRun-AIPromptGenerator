#nullable enable

using Community.PowerToys.Run.Plugin.AIPromptGenerator.Models;
using Community.PowerToys.Run.Plugin.AIPromptGenerator.Services;
using ManagedCommon;
using Microsoft.PowerToys.Settings.UI.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.AIPromptGenerator
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IContextMenu, ISettingProvider, IReloadable, IDisposable
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "EED50CE03A114CB18CA940D0550B6988";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "AI Prompt Generator";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "Expand short prompts into detailed, structured prompts using AI";

        private const int MaxPromptLength = 500;
        private const int MinPromptLength = 3;

        private PluginInitContext Context { get; set; } = null!;

        private string IconPath { get; set; } = string.Empty;

        private AIService AiService { get; set; } = null!;

        private PluginSettings Settings { get; set; } = new();

        private bool Disposed { get; set; }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            if (Context == null || AiService == null)
            {
                return new List<Result>();
            }

            var search = query.Search;
            var results = new List<Result>();

            // Handle empty query
            if (string.IsNullOrWhiteSpace(search))
            {
                var provider = Settings.GetCurrentProvider();
                results.Add(CreateInfoResult(
                    "AI Prompt Generator",
                    $"Type a short prompt to expand with {provider.Name} AI | Provider: {Settings.ProviderName}",
                    "start-typing"
                ));
                return results;
            }

            var userPrompt = search.Trim();

            // Validate input length
            if (userPrompt.Length < MinPromptLength)
            {
                results.Add(CreateInfoResult(
                    "Prompt too short",
                    $"Please enter at least {MinPromptLength} characters",
                    "too-short"
                ));
                return results;
            }

            if (userPrompt.Length > MaxPromptLength)
            {
                results.Add(CreateInfoResult(
                    "Prompt too long",
                    $"Please keep your prompt under {MaxPromptLength} characters",
                    "too-long"
                ));
                return results;
            }

            // Check if API key is configured
            var (isValid, errorMessage) = Settings.Validate();
            if (!isValid)
            {
                results.Add(CreateErrorResult(
                    "Configuration required",
                    errorMessage ?? "Please configure API key in PowerToys settings (Additional Options)",
                    "config-required"
                ));
                return results;
            }

            // Show provider and model info
            var provider2 = Settings.GetCurrentProvider();
            var model = Settings.GetEffectiveModel();

            // Create result for expanding the prompt
            results.Add(new Result
            {
                Title = $"Expand: \"{TruncateText(userPrompt, 60)}\"",
                SubTitle = $"Generate with {provider2.Name} ({model}) | Press Enter",
                IcoPath = IconPath,
                Score = 100,
                Action = _ => ExpandPromptAction(userPrompt),
                ContextData = userPrompt,
            });

            // Add quick actions
            results.Add(new Result
            {
                Title = "Copy original prompt",
                SubTitle = userPrompt,
                IcoPath = IconPath,
                Score = 90,
                Action = _ =>
                {
                    try
                    {
                        Clipboard.SetDataObject(userPrompt);
                        Context?.API.ShowMsg("Copied", "Original prompt copied to clipboard", IconPath);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                },
                ContextData = userPrompt,
            });

            return results;
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());

            // Initialize AI service
            AiService = new AIService();
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult.ContextData is string search)
            {
                return
                [
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Copy to clipboard (Ctrl+C)",
                        FontFamily = "Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.C,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            Clipboard.SetDataObject(search);
                            return true;
                        },
                    },
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Expand prompt (Enter)",
                        FontFamily = "Segoe MDL2 Assets",
                        Glyph = "\xE8BA", // Expand
                        AcceleratorKey = Key.Return,
                        AcceleratorModifiers = ModifierKeys.None,
                        Action = _ => ExpandPromptAction(search),
                    }
                ];
            }

            return [];
        }


        /// <summary>
        /// ISettingProvider: Create settings panel (not used - we use AdditionalOptions)
        /// </summary>
        public Control CreateSettingPanel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ISettingProvider: Get additional options for PowerToys settings
        /// </summary>
        public IEnumerable<PluginAdditionalOption> AdditionalOptions => PluginSettings.GetAdditionalOptions();

        /// <summary>
        /// ISettingProvider: Update settings from PowerToys UI
        /// </summary>
        public void UpdateSettings(PowerLauncherPluginSettings settings)
        {
            if (settings?.AdditionalOptions != null)
            {
                Settings = PluginSettings.LoadFromAdditionalOptions(settings.AdditionalOptions);
            }
        }

        /// <summary>
        /// Reload plugin when settings change
        /// </summary>
        public void ReloadData()
        {
            // Clear cache when settings are reloaded
            AiService?.ClearCache();
        }

        /// <summary>
        /// Action to expand the prompt using AI
        /// </summary>
        private bool ExpandPromptAction(string userPrompt)
        {
            if (Context == null || AiService == null)
            {
                return false;
            }

            // Perform async operation without blocking UI
            Task.Run(async () =>
            {
                try
                {
                    var response = await AiService.ExpandPromptAsync(userPrompt, Settings);

                    if (response.Success)
                    {
                        // Copy to clipboard
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Clipboard.SetDataObject(response.ExpandedPrompt);
                        });

                        var subtitle = Settings.ShowTokenCount && response.TokensUsed > 0
                            ? $"Copied! Tokens: {response.TokensUsed} | Model: {response.Model}"
                            : "Expanded prompt copied to clipboard!";

                        Context.API.ShowMsg(
                            "Success",
                            subtitle,
                            IconPath
                        );
                    }
                    else
                    {
                        Context.API.ShowMsg(
                            "Error",
                            response.ErrorMessage ?? "Failed to generate prompt",
                            IconPath
                        );
                    }
                }
                catch (Exception ex)
                {
                    Context.API.ShowMsg(
                        "Error",
                        $"Unexpected error: {ex.Message}",
                        IconPath
                    );
                }
            });

            return true;
        }

        /// <summary>
        /// Create an informational result
        /// </summary>
        private Result CreateInfoResult(string title, string subtitle, string id)
        {
            return new Result
            {
                Title = title,
                SubTitle = subtitle,
                IcoPath = IconPath,
                Score = 100,
                Action = _ => false // Non-actionable
            };
        }

        /// <summary>
        /// Create an error result that opens settings when clicked
        /// </summary>
        private Result CreateErrorResult(string title, string subtitle, string id)
        {
            return new Result
            {
                Title = $"⚠️ {title}",
                SubTitle = $"{subtitle} | Click to open settings",
                IcoPath = IconPath,
                Score = 100,
                Action = _ =>
                {
                    // Open PowerToys settings
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "powertoys://settings",
                            UseShellExecute = true
                        });
                    }
                    catch
                    {
                        // Ignore if can't open settings
                    }
                    return true;
                }
            };
        }

        /// <summary>
        /// Truncate text to specified length
        /// </summary>
        private static string TruncateText(string text, int maxLength)
        {
            if (text.Length <= maxLength)
            {
                return text;
            }

            return text[..(maxLength - 3)] + "...";
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            AiService?.Dispose();

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/aipromptgenerator.light.png" : "Images/aipromptgenerator.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);
    }
}