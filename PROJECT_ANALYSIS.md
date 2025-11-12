# 🧠 AI Prompt Generator - Project Deep Dive & Memory Retention

**Date:** 2025
**Author:** ruslanlap
**Repository:** PowerToysRun-AIPromptGenerator

---

## 📋 Executive Summary

This repository contains **TWO distinct but related projects**:

1. **AI Prompt Generator Plugin** - A PowerToys Run plugin that uses AI to expand short prompts into detailed, structured instructions
2. **PowerToys Run Plugin Templates** - A dotnet template system for scaffolding new PowerToys Run plugins

Both projects target .NET 9.0 and are designed for Windows 10/11 (x64 and ARM64).

---

## 🎯 Project 1: AI Prompt Generator Plugin

### Core Purpose
Transform brief, rough ideas into comprehensive, well-structured AI prompts using AI assistance. Integrated into PowerToys Run for instant access via keyboard shortcut.

### Location in Repository
```
AIPromptGenerator/
├── Community.PowerToys.Run.Plugin.AIPromptGenerator/
│   ├── Main.cs                                    # Plugin entry point (400+ lines)
│   ├── plugin.json                                # Plugin metadata
│   ├── *.csproj                                   # Project file
│   ├── Images/                                    # Icons (dark/light)
│   │   ├── aipromptgenerator.dark.png
│   │   └── aipromptgenerator.light.png
│   ├── Models/
│   │   ├── AIProvider.cs                          # Provider configuration
│   │   └── PluginSettings.cs                      # Settings management
│   └── Services/
│       └── AIService.cs                           # AI API integration
└── Community.PowerToys.Run.Plugin.AIPromptGenerator.UnitTests/
    └── (test files)
```

### Technical Architecture

#### Plugin Metadata
- **Plugin ID:** `EED50CE03A114CB18CA940D0550B6988`
- **Action Keyword:** `aipromptgenerator`
- **Name:** AI Prompt Generator
- **Version:** 1.0.0
- **Target Framework:** .NET 9.0 (net9.0-windows10.0.22621.0)
- **Platforms:** x64, ARM64

#### Key Components

**1. Main.cs** (400+ lines)
- Implements: `IPlugin`, `IContextMenu`, `ISettingProvider`, `IReloadable`, `IDisposable`
- Responsible for:
  - Query handling and validation (3-500 character limit)
  - Result generation and display
  - Context menu integration
  - Settings synchronization
  - Theme-aware icon management
  - Async AI expansion with Task.Run

**2. AIService.cs** (Services/)
- OpenAI-compatible API client
- Features:
  - HttpClient with compression support
  - Request/response models (ChatCompletionRequest, ChatCompletionResponse)
  - Smart caching system (dictionary-based)
  - Timeout management (default 30s, configurable 5-120s)
  - Error handling with detailed messages
- Cache Key Format: `{prompt}|{model}|{temperature}|{systemPrompt}`

**3. PluginSettings.cs** (Models/)
- PowerToys AdditionalOptions integration
- Configuration options:
  - **Provider Selection:** Dropdown (OpenAI, Groq, OpenRouter, HuggingFace, SambaNova)
  - **API Key:** Secure text input (stored in PowerToys settings)
  - **Model:** Optional text (uses provider default if empty)
  - **Custom Endpoint:** Optional URL
  - **Temperature:** 0.0-2.0 (default: 0.7)
  - **Max Tokens:** 100-4000 (default: 2000)
  - **System Prompt:** Long text (default: advanced prompt engineering template)
  - **Timeout:** 5-120 seconds (default: 30)
  - **Enable Caching:** Boolean (default: true)
  - **Cache Duration:** 1-60 minutes (default: 10)
  - **Show Token Count:** Boolean (default: false)

**4. AIProvider.cs** (Models/)
- Provider enumeration: OpenAI, Groq, OpenRouter, HuggingFace, SambaNova
- Provider configuration includes:
  - Name
  - Default API endpoint
  - Default model list
  - API key requirement flag

#### AI Provider Details

| Provider | Default Endpoint | Default Models | Notes |
|----------|------------------|----------------|-------|
| **OpenAI** | `api.openai.com/v1/chat/completions` | gpt-4o, gpt-4o-mini, gpt-4-turbo, gpt-3.5-turbo | Industry standard |
| **Groq** | `api.groq.com/openai/v1/chat/completions` | llama-3.3-70b-versatile, mixtral-8x7b-32768 | Ultra-fast inference |
| **OpenRouter** | `openrouter.ai/api/v1/chat/completions` | openai/gpt-4o, anthropic/claude-3.5-sonnet | Multi-provider gateway |
| **HuggingFace** | `api-inference.huggingface.co/models/` | Llama-3.1-70B-Instruct, Mixtral-8x7B | Open-source models |
| **SambaNova** | `api.sambanova.ai/v1/chat/completions` | Meta-Llama-3.1-70B-Instruct | Enterprise inference |

#### Default System Prompt
The plugin includes a sophisticated 100+ line system prompt that instructs the AI to:
- Act as a prompt architecture expert
- Follow a 7-step framework:
  1. Role Definition
  2. Task Specification
  3. Contextual Information
  4. Output Requirements
  5. Constraints & Guardrails
  6. Processing Instructions
  7. Examples & Clarifications
- Include quality checklist
- Ensure prompts are unambiguous, scalable, and consistent

#### User Flow
1. User presses `Alt+Space` (PowerToys Run)
2. Types `aipromptgenerator write a blog post`
3. Plugin validates input (3-500 chars)
4. Checks API key configuration
5. Shows preview result: "Expand: write a blog post"
6. User presses Enter
7. Plugin calls AIService asynchronously
8. AI expands prompt using system instructions
9. Result copied to clipboard automatically
10. Notification shown with token count (if enabled)

#### Dependencies
```xml
<PackageReference Include="Community.PowerToys.Run.Plugin.Dependencies" Version="0.93.0" />
<PackageReference Include="System.Text.Json" Version="9.0.0" />
```

---

## 🎯 Project 2: PowerToys Run Plugin Templates

### Core Purpose
Provide dotnet CLI templates for quickly scaffolding new PowerToys Run plugins with best practices and proper structure.

### Location in Repository
```
src/
├── Community.PowerToys.Run.Plugin.Templates.csproj  # Template package project
└── templates/
    ├── project/                                     # Single project template
    │   ├── .template.config/
    │   │   └── template.json                        # Template configuration
    │   ├── Images/
    │   │   ├── plugin1.dark.png
    │   │   └── plugin1.light.png
    │   ├── Main.cs                                  # Starter plugin code
    │   ├── plugin.json
    │   └── Namespace.Plugin1.csproj
    ├── solution/                                    # Full solution template
    │   ├── .template.config/
    │   │   └── template.json
    │   ├── Community.PowerToys.Run.Plugin.Plugin1/
    │   │   ├── Images/
    │   │   ├── Main.cs
    │   │   ├── plugin.json
    │   │   └── *.csproj
    │   ├── Community.PowerToys.Run.Plugin.Plugin1.UnitTests/
    │   │   ├── MainTests.cs
    │   │   └── *.csproj
    │   └── Plugin1.sln
    └── scripts/                                     # Helper scripts
```

### Template Metadata

**Project Template:**
- **Identity:** `Community.PowerToys.Run.Plugin.Templates.Project`
- **Short Name:** `ptrun-proj`
- **Type:** project
- **Author:** Henrik Lau Eriksson
- **Default Name:** MyPlugin

**Solution Template:**
- **Identity:** `Community.PowerToys.Run.Plugin.Templates.Solution`
- **Short Name:** `ptrun-sln`
- **Type:** solution
- **Default Name:** MyPlugin
- **Optional:** Test project (default: true)

### Template Parameters

**Common Parameters:**
- `PluginAuthor` - Author name (default: "hlaueriksson")
- `PluginName` - Derived from project name
- `TestProject` - Include unit test project (solution template only)

**Name Transformations:**
- Input: `MyCompany.MyAwesomePlugin`
- `PluginName` → `MyAwesomePlugin` (removes namespace prefix)
- `PluginNameLowerCase` → `myawesomeplugin` (for file names)

### Package Details
```xml
<PropertyGroup>
  <PackageId>Community.PowerToys.Run.Plugin.Templates</PackageId>
  <PackageVersion>0.3.0</PackageVersion>
  <PackageType>Template</PackageType>
  <Title>PowerToys Run Plugin Templates</Title>
  <Authors>Henrik Lau Eriksson</Authors>
</PropertyGroup>
```

### Usage Examples

**Install Templates:**
```bash
dotnet new install Community.PowerToys.Run.Plugin.Templates
```

**Create Project:**
```bash
dotnet new ptrun-proj -n MyCompany.MyPlugin -o MyPlugin --PluginAuthor "John Doe"
```

**Create Solution:**
```bash
dotnet new ptrun-sln -n MyPlugin -o MyPlugin --TestProject true
```

**Uninstall:**
```bash
dotnet new uninstall Community.PowerToys.Run.Plugin.Templates
```

---

## 🏗️ Build System

### Build Scripts

**1. build-and-zip.sh** (Linux/WSL)
- Builds for x64 and ARM64
- Creates distribution ZIP files
- Output: `QuickBrain-{version}-{platform}.zip`

**2. install-local.bat** (Windows)
- Builds the plugin
- Copies to PowerToys plugin directory
- Restarts PowerToys automatically

**3. pack.bat**
- Packages the template NuGet package
- Output: `Community.PowerToys.Run.Plugin.Templates.{version}.nupkg`

**4. ptrun-lint.sh**
- Code quality checks
- Linting validation

### Manual Build Commands

**Plugin:**
```bash
cd AIPromptGenerator
dotnet restore
dotnet build -c Release -p:Platform=x64
# Output: bin/x64/Release/net9.0-windows10.0.22621.0/
```

**Templates:**
```bash
cd src
dotnet pack -c Release
# Output: bin/Release/Community.PowerToys.Run.Plugin.Templates.{version}.nupkg
```

### Installation Paths

**Windows Plugin Directory:**
```
%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\AIPromptGenerator\
```

**Typical Full Path:**
```
C:\Users\{Username}\AppData\Local\Microsoft\PowerToys\PowerToys Run\Plugins\AIPromptGenerator\
```

---

## 🔐 Security & Privacy

### API Key Storage
- Stored in PowerToys settings (encrypted by Windows)
- NOT stored in plugin directory
- Managed through PowerToys Settings UI
- Uses `PluginAdditionalOption` with `AdditionalOptionType.Textbox`

### Data Flow
1. User input stays local until Enter is pressed
2. Only the prompt is sent to AI provider (via HTTPS)
3. Response cached locally (in-memory dictionary)
4. No telemetry or tracking
5. History NOT stored (unlike QuickBrain plugin)

### Privacy Considerations
- Plugin code is open source (MIT License)
- AI providers have their own privacy policies
- API calls go directly to chosen provider
- No middleware or logging servers
- Cache cleared on plugin reload

---

## 🎨 UI/UX Features

### Theme Support
- Automatic light/dark mode icons
- Theme change detection via `Context.API.ThemeChanged` event
- Icon selection logic: `Theme.Light || Theme.HighContrastWhite ? light : dark`

### Icons
- **aipromptgenerator.dark.png** - Dark theme icon
- **aipromptgenerator.light.png** - Light theme icon
- Stored in `Images/` directory
- Copied to output via `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`

### Result Display
```
Title: Expand: "write a blog post"
SubTitle: Generate with OpenAI (gpt-4o) | Press Enter
IcoPath: {theme-appropriate icon}
Score: 100
```

### Context Menu Actions
1. **Copy to clipboard (Ctrl+C)** - Copy original prompt
2. **Expand prompt (Enter)** - Execute AI expansion

### Notifications
- Success: "Success | Copied! Tokens: {count} | Model: {model}"
- Error: "Error | {error message}"
- Configuration: "Configuration required | Please configure API key..."

---

## 📊 Performance Characteristics

### Caching Strategy
- **Key:** Hash of prompt + model + temperature + system prompt
- **Storage:** In-memory Dictionary<string, AIResponse>
- **Lifetime:** Until plugin reload or cache clear
- **Configurable:** Can be disabled via settings
- **Duration:** 1-60 minutes (configurable, default: 10)

### Timeouts
- **Default:** 30 seconds
- **Range:** 5-120 seconds
- **Implementation:** CancellationTokenSource with linked cancellation
- **Error Message:** "Request timed out. Try increasing the timeout in settings."

### Response Times (Estimated)
- **OpenAI GPT-4o:** 2-5 seconds
- **Groq Llama 3.3:** 0.5-2 seconds (ultra-fast)
- **OpenRouter:** Varies by backend
- **HuggingFace:** 3-8 seconds (cold start)
- **SambaNova:** 1-3 seconds

### Token Consumption (Typical)
- **System Prompt:** ~600-800 tokens
- **User Input:** 5-100 tokens
- **AI Output:** 300-1500 tokens
- **Total per request:** ~1000-2400 tokens

---

## 🧪 Testing

### Unit Test Structure
```
Community.PowerToys.Run.Plugin.AIPromptGenerator.UnitTests/
└── (test files for validation, parsing, etc.)
```

### Test Categories
- Input validation (min/max length)
- Settings loading/saving
- API provider configuration
- Response parsing
- Cache behavior
- Error handling

---

## 🚀 CI/CD

### GitHub Actions Workflow
**File:** `.github/workflows/build-and-release.yml`

**Trigger:** Push to tags `v*`

**Jobs:**
1. Build for x64 and ARM64
2. Run tests
3. Create ZIP artifacts
4. Create GitHub release
5. Upload release assets

**Artifacts:**
- `AIPromptGenerator-{version}-x64.zip`
- `AIPromptGenerator-{version}-arm64.zip`

---

## 📚 Configuration Examples

### Minimal Configuration
```json
{
  "Provider": "OpenAI",
  "ApiKey": "sk-...",
  "Model": "",  // Uses default
  "Temperature": 0.7
}
```

### Advanced Configuration
```json
{
  "Provider": "Groq",
  "ApiKey": "gsk_...",
  "Model": "llama-3.3-70b-versatile",
  "ApiEndpoint": "",  // Uses provider default
  "Temperature": 0.8,
  "MaxTokens": 2500,
  "SystemPrompt": "Custom instructions...",
  "TimeoutSeconds": 45,
  "EnableCaching": true,
  "CacheDurationMinutes": 15,
  "ShowTokenCount": true
}
```

### Custom Endpoint Example
```json
{
  "Provider": "OpenAI",
  "ApiKey": "sk-...",
  "ApiEndpoint": "https://my-proxy.com/v1/chat/completions",
  "Model": "gpt-4o"
}
```

---

## 🔄 Update Strategy

### Version Update Process
1. Update version in `plugin.json`
2. Update version in `.csproj` files
3. Update README.md with new version
4. Create git tag: `git tag v1.x.x`
5. Push tag: `git push origin v1.x.x`
6. GitHub Actions automatically builds and releases

### User Update Process
1. Download new release ZIP
2. Extract to plugin directory (overwrite)
3. Restart PowerToys
4. Settings are preserved

---

## 🎯 Key Design Decisions

### Why OpenAI-Compatible API?
- **Standardization:** Most providers support this format
- **Simplicity:** Single client implementation
- **Flexibility:** Easy to add new providers
- **Community:** Well-documented, widely used

### Why In-Memory Cache?
- **Speed:** Instant response for repeated queries
- **Simplicity:** No file I/O or database
- **Privacy:** Cleared on restart
- **Cost:** Reduces API calls

### Why No History Storage?
- **Privacy:** No sensitive data written to disk
- **Simplicity:** Reduces complexity
- **Focus:** Designed for one-off expansions
- **Different from QuickBrain:** Calculations vs. generative AI

### Why AdditionalOptions vs. Custom UI?
- **Consistency:** Matches PowerToys design language
- **Security:** Leverages PowerToys encryption
- **Maintenance:** Less code, fewer bugs
- **User Experience:** Familiar interface

---

## 🔮 Future Enhancement Ideas

### Potential Features
- [ ] Template library (pre-made prompt templates)
- [ ] Multi-step expansion (refine → expand → optimize)
- [ ] Prompt comparison (A/B testing)
- [ ] Cost tracking (token usage over time)
- [ ] History with search (optional)
- [ ] Favorite prompts
- [ ] Export/import settings
- [ ] Local LLM support (Ollama, LM Studio)
- [ ] Streaming responses (real-time)
- [ ] Prompt quality scoring

### Technical Improvements
- [ ] Async caching with TTL expiration
- [ ] Circuit breaker for failing APIs
- [ ] Retry with exponential backoff
- [ ] Request queuing for rate limits
- [ ] Compression for large prompts
- [ ] Custom model parameters per provider
- [ ] API key validation on save
- [ ] Provider health check

---

## 📖 Learning Resources

### Understanding PowerToys Run Plugins
- [PowerToys Documentation](https://learn.microsoft.com/en-us/windows/powertoys/)
- [Plugin Development Guide](https://github.com/microsoft/PowerToys/blob/main/doc/devdocs/modules/launcher/plugins/README.md)
- [Community Templates](https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Templates)

### Prompt Engineering
- [OpenAI Best Practices](https://platform.openai.com/docs/guides/prompt-engineering)
- [Anthropic Prompt Engineering Guide](https://docs.anthropic.com/en/docs/prompt-engineering)
- [Prompt Engineering Guide](https://www.promptingguide.ai/)

### .NET Development
- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [C# 12 Features](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12)
- [dotnet new Templates](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates)

---

## 🏆 Project Achievements

### Technical Excellence
✅ Clean, maintainable architecture
✅ Proper separation of concerns (Main, Service, Models)
✅ Async/await best practices
✅ Nullable reference types throughout
✅ Comprehensive error handling
✅ Theme-aware UI
✅ Multi-platform support (x64, ARM64)

### User Experience
✅ Instant access via keyboard
✅ Minimal configuration required
✅ Clear error messages
✅ Auto-copy to clipboard
✅ Non-blocking async operations
✅ Visual feedback (notifications)

### Developer Experience
✅ Well-documented code
✅ Template system for new plugins
✅ Build automation
✅ Open-source with MIT license
✅ Clear project structure
✅ Example implementations

---

## 📝 Important Notes for Future Reference

1. **Plugin ID must remain constant** (`EED50CE03A114CB18CA940D0550B6988`) - changing it breaks existing installations
2. **Target framework tied to PowerToys** - must match PowerToys' .NET version
3. **Theme icons are required** - plugin won't load without both dark/light variants
4. **Settings are NOT stored in plugin directory** - they're in PowerToys settings database
5. **Cache is NOT persistent** - cleared on every plugin reload/restart
6. **API keys in plaintext concern** - mitigated by using PowerToys encrypted storage
7. **HTTP client is static** - shared across all requests for connection pooling
8. **Temperature > 1.0 is valid** - some models support up to 2.0
9. **System prompt is extensive** - 100+ lines, forms core of expansion quality
10. **No background processing** - all operations triggered by user input

---

## 🎓 Lessons Learned

### Architecture
- **Service layer separation** - AIService.cs handles all HTTP, Main.cs handles UI
- **Provider abstraction** - Easy to add new AI providers via AIProvider.cs
- **Settings injection** - PowerToys AdditionalOptions system is powerful but complex
- **Async UI** - Must use Task.Run + Dispatcher for non-blocking operations

### PowerToys Integration
- **Plugin lifecycle** - Init → Query (multiple) → Dispose
- **Theme changes** - Must subscribe to ThemeChanged event
- **Context menus** - Provide quick actions, improve UX significantly
- **Score importance** - Higher scores appear first (use 100 for main results)

### AI Integration
- **Timeout is critical** - Some models are slow, users need feedback
- **Caching saves money** - Repeated queries common during exploration
- **System prompt quality** - Determines output quality more than model choice
- **Error messages matter** - Users need actionable guidance on failures

---

## 📊 Project Statistics (Approximate)

- **Total Lines of Code:** ~2,000 (plugin) + ~500 (templates)
- **Main.cs:** ~400 lines
- **AIService.cs:** ~250 lines
- **PluginSettings.cs:** ~350 lines
- **AIProvider.cs:** ~130 lines
- **Dependencies:** 2 NuGet packages
- **Supported AI Providers:** 5
- **Configuration Options:** 11
- **Build Targets:** 2 (x64, ARM64)
- **Template Types:** 2 (project, solution)

---

## 🌟 Conclusion

This project represents a sophisticated integration between:
- **Windows PowerToys** ecosystem
- **Modern .NET 9.0** development
- **AI/LLM** technology
- **Developer tooling** (templates)

The dual-purpose repository serves both end-users (via the plugin) and developers (via the templates), making it valuable to the PowerToys community at multiple levels.

The architecture is clean, extensible, and follows best practices for PowerToys plugin development. The caching system, multi-provider support, and comprehensive configuration options make it production-ready while remaining simple to use.

**Key Success Factors:**
1. Solves a real problem (prompt engineering is hard)
2. Integrates seamlessly into existing workflow (PowerToys Run)
3. Supports multiple AI providers (no vendor lock-in)
4. Open source with permissive license (MIT)
5. Well-documented for both users and developers
6. Includes templates to help others build plugins

---

**Last Updated:** 2025
**Maintained By:** ruslanlap
**Repository:** https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator
**License:** MIT
