# 🎯 PowerToysRun-AIPromptGenerator - Executive Summary

> **Quick reference guide for understanding this dual-purpose repository**

---

## 📦 What This Repository Contains

This repository hosts **TWO distinct projects**:

### 1. 🤖 AI Prompt Generator Plugin
A PowerToys Run plugin that transforms short prompts into detailed, professional AI prompts using multiple AI providers.

### 2. 🛠️ PowerToys Run Plugin Templates
A dotnet template package for scaffolding new PowerToys Run plugins quickly.

---

## 🚀 Quick Stats

| Metric | Value |
|--------|-------|
| **Primary Language** | C# 12 |
| **Framework** | .NET 9.0 |
| **Target OS** | Windows 10/11 (x64, ARM64) |
| **License** | MIT |
| **AI Providers Supported** | 5 (OpenAI, Groq, OpenRouter, HuggingFace, SambaNova) |
| **Template Types** | 2 (project, solution) |
| **Lines of Code** | ~2,500 |

---

## 🎯 Project 1: AI Prompt Generator Plugin

### Core Functionality
- **What:** Expands short ideas into comprehensive, structured AI prompts
- **How:** Integrates with PowerToys Run launcher via keyboard shortcut
- **Why:** Makes prompt engineering faster and more consistent

### Key Features
✅ Multi-provider AI support (5 providers)
✅ Smart response caching (configurable)
✅ Secure API key storage (PowerToys encrypted)
✅ Theme-aware UI (light/dark icons)
✅ One-click clipboard copy
✅ Configurable parameters (temperature, tokens, timeout)
✅ Custom system prompts

### Technical Details
- **Plugin ID:** `EED50CE03A114CB18CA940D0550B6988`
- **Action Keyword:** `aipromptgenerator`
- **Main Components:**
  - `Main.cs` (~400 lines) - Plugin entry point
  - `AIService.cs` (~250 lines) - HTTP client & API integration
  - `PluginSettings.cs` (~350 lines) - Configuration management
  - `AIProvider.cs` (~130 lines) - Provider definitions

### User Workflow
```
1. Press Alt+Space (PowerToys Run)
2. Type: aipromptgenerator write a blog post
3. Press Enter
4. AI expands prompt (2-5 seconds)
5. Result automatically copied to clipboard
6. Notification shows success with token count
```

### Configuration Options
| Setting | Default | Range/Options |
|---------|---------|---------------|
| Provider | OpenAI | 5 providers |
| API Key | (required) | Encrypted storage |
| Model | (auto) | Provider-specific |
| Temperature | 0.7 | 0.0 - 2.0 |
| Max Tokens | 2000 | 100 - 4000 |
| Timeout | 30s | 5 - 120 seconds |
| Caching | Enabled | On/Off |
| Cache Duration | 10 min | 1 - 60 minutes |

### Installation Path
```
%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\AIPromptGenerator\
```

---

## 🛠️ Project 2: PowerToys Run Plugin Templates

### Core Functionality
- **What:** dotnet CLI templates for creating PowerToys Run plugins
- **How:** `dotnet new ptrun-proj` or `dotnet new ptrun-sln`
- **Why:** Accelerates plugin development with best practices built-in

### Template Types

**1. Project Template (`ptrun-proj`)**
- Single project structure
- Minimal setup
- Quick prototyping
```bash
dotnet new ptrun-proj -n MyPlugin -o MyPlugin
```

**2. Solution Template (`ptrun-sln`)**
- Full solution with .sln file
- Plugin project + Unit test project
- Production-ready structure
```bash
dotnet new ptrun-sln -n MyPlugin -o MyPlugin
```

### What's Generated
✅ Project/solution files (.csproj, .sln)
✅ Plugin boilerplate (Main.cs)
✅ Configuration (plugin.json)
✅ Theme icons (dark/light placeholders)
✅ Unit test setup (solution template)
✅ NuGet package references

### Package Details
- **Package ID:** `Community.PowerToys.Run.Plugin.Templates`
- **Version:** 0.3.0
- **Author:** Henrik Lau Eriksson
- **Installation:** `dotnet new install Community.PowerToys.Run.Plugin.Templates`

---

## 📁 Repository Structure

```
PowerToysRun-AIPromptGenerator/
│
├── AIPromptGenerator/                              # Plugin Project
│   ├── Community.PowerToys.Run.Plugin.AIPromptGenerator/
│   │   ├── Main.cs                                 # Entry point
│   │   ├── plugin.json                             # Metadata
│   │   ├── Images/                                 # Icons
│   │   ├── Models/                                 # Data models
│   │   │   ├── AIProvider.cs
│   │   │   └── PluginSettings.cs
│   │   └── Services/
│   │       └── AIService.cs                        # AI integration
│   ├── Community.PowerToys.Run.Plugin.AIPromptGenerator.UnitTests/
│   └── AIPromptGenerator.sln
│
├── src/                                            # Template Project
│   ├── templates/
│   │   ├── project/                                # ptrun-proj
│   │   │   ├── .template.config/
│   │   │   ├── Images/
│   │   │   ├── Main.cs
│   │   │   └── plugin.json
│   │   ├── solution/                               # ptrun-sln
│   │   │   ├── .template.config/
│   │   │   ├── Community.PowerToys.Run.Plugin.Plugin1/
│   │   │   └── Community.PowerToys.Run.Plugin.Plugin1.UnitTests/
│   │   └── scripts/
│   └── Community.PowerToys.Run.Plugin.Templates.csproj
│
├── tests/                                          # Template tests
├── assets/                                         # README assets
│   └── logo.png
│
├── .github/workflows/
│   └── build-and-release.yml                       # CI/CD
│
├── build-and-zip.sh                                # Build script
├── install-local.bat                               # Install script
├── Templates.sln                                   # Template solution
├── README.md                                       # Main documentation
├── PROJECT_ANALYSIS.md                             # Deep dive
├── TEMPLATE_GUIDE.md                               # Template usage
└── SUMMARY.md                                      # This file
```

---

## 🔑 Key Design Decisions

### Why OpenAI-Compatible API?
- ✅ Industry standard format
- ✅ Most providers support it
- ✅ Single client implementation
- ✅ Easy to add new providers

### Why In-Memory Cache?
- ✅ Fast (instant for repeated queries)
- ✅ Simple (no file I/O)
- ✅ Private (cleared on restart)
- ✅ Cost-effective (reduces API calls)

### Why PowerToys AdditionalOptions?
- ✅ Consistent with PowerToys UI
- ✅ Secure (leverages Windows encryption)
- ✅ Less code to maintain
- ✅ Familiar user experience

### Why No Persistent History?
- ✅ Privacy (no sensitive data on disk)
- ✅ Simplicity (less complexity)
- ✅ Focus (one-off prompt generation)
- ✅ Different use case from calculators

---

## 🚀 Getting Started

### For End Users (Plugin)

**1. Download:**
```
https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator/releases/latest
```

**2. Install:**
```
Extract to: %LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\
```

**3. Configure:**
- Restart PowerToys
- Open Settings → PowerToys Run → AI Prompt Generator
- Add API key and select provider

**4. Use:**
```
Alt+Space → aipromptgenerator your idea → Enter
```

### For Developers (Templates)

**1. Install Templates:**
```bash
dotnet new install Community.PowerToys.Run.Plugin.Templates
```

**2. Create Plugin:**
```bash
dotnet new ptrun-sln -n MyPlugin -o MyPlugin
```

**3. Build:**
```bash
dotnet build -c Release -p:Platform=x64
```

**4. Test:**
```bash
# Copy to PowerToys directory and restart
```

---

## 🔧 Build Commands

### Plugin
```bash
cd AIPromptGenerator
dotnet restore
dotnet build -c Release -p:Platform=x64    # or ARM64
dotnet test                                 # Run tests
```

### Templates
```bash
cd src
dotnet pack -c Release                      # Create NuGet package
```

### Scripts
```bash
./build-and-zip.sh                          # Linux/WSL build + ZIP
install-local.bat                           # Windows install
pack.bat                                    # Windows pack templates
```

---

## 🤖 AI Provider Comparison

| Provider | Speed | Cost (per 1M tokens) | Best For |
|----------|-------|---------------------|----------|
| **OpenAI** | Medium | $5.00 (GPT-4o) | Quality, reliability |
| **Groq** | Ultra Fast | $0.59 | Speed, cost-efficiency |
| **OpenRouter** | Varies | Varies | Multi-model access |
| **HuggingFace** | Medium | Free tier | Open-source, research |
| **SambaNova** | Fast | TBD | Enterprise workloads |

---

## 📊 Performance Metrics

### Response Times (Typical)
- OpenAI GPT-4o: 2-5 seconds
- Groq Llama 3.3: 0.5-2 seconds
- HuggingFace: 3-8 seconds (cold start)

### Token Usage (Average)
- System Prompt: ~700 tokens
- User Input: ~50 tokens
- AI Output: ~1000 tokens
- **Total:** ~1750 tokens per request

### Cache Impact
- **Cache Hit:** Instant (<10ms)
- **Cache Miss:** API call required
- **Cache Size:** In-memory (cleared on restart)

---

## 🔒 Security & Privacy

### API Keys
✅ Stored in PowerToys encrypted settings
✅ Never written to disk by plugin
✅ Transmitted only via HTTPS
✅ User controls which provider to use

### Data Flow
1. User input → Plugin (local)
2. Plugin → AI Provider (HTTPS)
3. AI Provider → Plugin (HTTPS)
4. Result → Clipboard (local)

### Privacy Features
✅ No telemetry or tracking
✅ No data logging by plugin
✅ No persistent storage of prompts
✅ Open-source code (auditable)

---

## 📚 Documentation Files

| File | Purpose | Audience |
|------|---------|----------|
| `README.md` | Main documentation, installation, usage | All users |
| `PROJECT_ANALYSIS.md` | Deep technical dive, architecture | Developers |
| `TEMPLATE_GUIDE.md` | Template usage, examples | Plugin developers |
| `SUMMARY.md` | Quick reference, overview | Everyone |
| `LICENSE` | MIT license terms | Legal/compliance |

---

## 🎓 Key Takeaways

### For End Users
1. **Easy to use:** Alt+Space → type → Enter
2. **Multiple providers:** Choose based on cost/quality
3. **Secure:** API keys encrypted by Windows
4. **Fast:** 2-5 second typical response
5. **Cost-effective:** Caching reduces API calls

### For Plugin Developers
1. **Templates available:** Scaffold in seconds
2. **Best practices:** Built-in from templates
3. **Well-documented:** Multiple guides available
4. **Open source:** Learn from working code
5. **Active maintenance:** Regular updates

### For Contributors
1. **Clean architecture:** Service-oriented design
2. **Testable:** Unit tests included
3. **Extensible:** Easy to add providers
4. **Documented:** Inline comments + guides
5. **CI/CD:** Automated builds and releases

---

## 🔮 Future Possibilities

### Plugin Enhancements
- [ ] Template library (pre-made prompts)
- [ ] Multi-step refinement (expand → optimize)
- [ ] Local LLM support (Ollama, LM Studio)
- [ ] Streaming responses (real-time)
- [ ] Prompt quality scoring
- [ ] Usage analytics (token tracking)

### Template Improvements
- [ ] More starter templates
- [ ] Visual Studio integration
- [ ] Interactive scaffolding
- [ ] Plugin marketplace integration
- [ ] Auto-update mechanism

---

## 🤝 Contributing

### Ways to Contribute
1. **Report bugs:** Open issues on GitHub
2. **Suggest features:** Create feature requests
3. **Submit code:** Pull requests welcome
4. **Write docs:** Improve documentation
5. **Share:** Tell others about the project

### Development Setup
```bash
git clone https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator.git
cd PowerToysRun-AIPromptGenerator
# Open in Visual Studio or VS Code
```

---

## 📞 Support & Community

### Get Help
- **GitHub Issues:** Bug reports, questions
- **Discussions:** Feature requests, ideas
- **README:** Comprehensive documentation
- **Examples:** Working code in repository

### Stay Updated
- **Watch:** GitHub repository for updates
- **Star:** Show appreciation and bookmark
- **Fork:** Create your own customizations
- **Share:** Help others discover the project

---

## 📄 License

**MIT License** - Free for personal and commercial use

```
Copyright (c) 2025 ruslanlap
Permission is hereby granted, free of charge...
See LICENSE file for full terms.
```

---

## 🌟 Credits

### Project Maintainer
- **ruslanlap** - Creator and maintainer

### Template Original Author
- **Henrik Lau Eriksson** - Template system creator

### Built With
- Microsoft PowerToys
- .NET 9.0
- OpenAI, Groq, OpenRouter, HuggingFace, SambaNova APIs
- Community contributions

---

## 📊 Project Status

| Aspect | Status |
|--------|--------|
| **Development** | ✅ Active |
| **Stability** | ✅ Stable (v1.0.0) |
| **Documentation** | ✅ Comprehensive |
| **Testing** | ✅ Unit tests included |
| **CI/CD** | ✅ Automated |
| **Support** | ✅ Maintained |

---

## 🎯 Success Metrics

### Plugin Adoption
- Downloads: Track via GitHub releases
- Stars: Community interest indicator
- Issues/PRs: Community engagement

### Template Usage
- NuGet downloads
- Community plugins created
- Fork count

---

## 📝 Version History

| Version | Date | Highlights |
|---------|------|------------|
| **1.0.0** | 2025 | Initial release with 5 AI providers |
| **0.3.0** | 2025 | Template system updates for .NET 9 |

---

## 🔗 Important Links

### Repository
- **Main:** https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator
- **Releases:** https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator/releases
- **Issues:** https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator/issues

### Related Projects
- **PowerToys:** https://github.com/microsoft/PowerToys
- **Templates:** https://github.com/hlaueriksson/Community.PowerToys.Run.Plugin.Templates

---

**Last Updated:** 2025
**Maintained By:** ruslanlap
**License:** MIT
**Status:** Active Development

---

<div align="center">

**⭐ Star this repo if you find it useful! ⭐**

</div>
