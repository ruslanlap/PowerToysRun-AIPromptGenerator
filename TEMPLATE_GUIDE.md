# 🎯 PowerToys Run Plugin Templates - Quick Start Guide

> **Create PowerToys Run plugins in seconds with pre-configured templates**

---

## 📦 What's Included

This repository provides two dotnet templates for creating PowerToys Run plugins:

1. **`ptrun-proj`** - Single project template (minimal setup)
2. **`ptrun-sln`** - Full solution template (with unit tests)

Both templates include:
- ✅ Pre-configured project structure
- ✅ PowerToys Run plugin boilerplate
- ✅ Theme-aware icons (light/dark)
- ✅ Example `Main.cs` implementation
- ✅ Proper `plugin.json` configuration
- ✅ .NET 9.0 targeting with Windows SDK
- ✅ NuGet package references

---

## 🚀 Quick Start

### Step 1: Install Templates

**From NuGet (Recommended):**
```bash
dotnet new install Community.PowerToys.Run.Plugin.Templates
```

**From Local Source:**
```bash
cd PowerToysRun-AIPromptGenerator
dotnet new install ./src
```

**Verify Installation:**
```bash
dotnet new list | grep ptrun
```

You should see:
```
ptrun-proj    PowerToys Run Plugin Project     [C#]    PowerToys
ptrun-sln     PowerToys Run Plugin Solution    [C#]    PowerToys
```

---

## 🎨 Creating Your First Plugin

### Option A: Simple Project Template

**Basic Usage:**
```bash
dotnet new ptrun-proj -n MyAwesomePlugin -o MyAwesomePlugin
```

**With Custom Author:**
```bash
dotnet new ptrun-proj -n MyAwesomePlugin -o MyAwesomePlugin --PluginAuthor "Your Name"
```

**Output Structure:**
```
MyAwesomePlugin/
├── Images/
│   ├── myawesomeplugin.dark.png
│   └── myawesomeplugin.light.png
├── Main.cs
├── plugin.json
└── MyAwesomePlugin.csproj
```

---

### Option B: Full Solution Template

**Basic Usage:**
```bash
dotnet new ptrun-sln -n MyAwesomePlugin -o MyAwesomePlugin
```

**Without Test Project:**
```bash
dotnet new ptrun-sln -n MyAwesomePlugin -o MyAwesomePlugin --TestProject false
```

**With Custom Author:**
```bash
dotnet new ptrun-sln -n MyAwesomePlugin -o MyAwesomePlugin --PluginAuthor "Your Name"
```

**Output Structure:**
```
MyAwesomePlugin/
├── MyAwesomePlugin.sln
├── Community.PowerToys.Run.Plugin.MyAwesomePlugin/
│   ├── Images/
│   │   ├── myawesomeplugin.dark.png
│   │   └── myawesomeplugin.light.png
│   ├── Main.cs
│   ├── plugin.json
│   └── Community.PowerToys.Run.Plugin.MyAwesomePlugin.csproj
└── Community.PowerToys.Run.Plugin.MyAwesomePlugin.UnitTests/
    ├── MainTests.cs
    └── Community.PowerToys.Run.Plugin.MyAwesomePlugin.UnitTests.csproj
```

---

## 🔧 Building Your Plugin

### Build Commands

**Restore Dependencies:**
```bash
dotnet restore
```

**Build for x64:**
```bash
dotnet build -c Release -p:Platform=x64
```

**Build for ARM64:**
```bash
dotnet build -c Release -p:Platform=ARM64
```

**Run Tests:**
```bash
dotnet test
```

---

## 📝 Customizing Your Plugin

### 1. Update plugin.json

```json
{
  "ID": "GENERATE_NEW_GUID_HERE",
  "ActionKeyword": "mycommand",
  "IsGlobal": false,
  "Name": "MyAwesomePlugin",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Language": "csharp",
  "Website": "https://github.com/yourname/your-plugin",
  "ExecuteFileName": "Community.PowerToys.Run.Plugin.MyAwesomePlugin.dll",
  "IcoPathDark": "Images\\myawesomeplugin.dark.png",
  "IcoPathLight": "Images\\myawesomeplugin.light.png",
  "DynamicLoading": false
}
```

**⚠️ Important:** Generate a new GUID for the ID field!
```bash
# PowerShell
[guid]::NewGuid().ToString().ToUpper()

# Online
# Visit: https://www.guidgenerator.com/
```

### 2. Replace Icon Placeholders

Replace the placeholder PNG files in the `Images/` directory with your custom icons:
- **Size:** 64x64 or 128x128 pixels (PNG format)
- **Dark theme:** For dark backgrounds
- **Light theme:** For light backgrounds

**Tip:** Use tools like [Icons8](https://icons8.com) or [Figma](https://figma.com) to create icons.

### 3. Implement Your Logic in Main.cs

The template provides a basic structure. Customize the `Query()` method:

```csharp
public List<Result> Query(Query query)
{
    var results = new List<Result>();

    if (string.IsNullOrWhiteSpace(query.Search))
    {
        results.Add(new Result
        {
            Title = "Your Plugin Name",
            SubTitle = "Type something to search...",
            IcoPath = IconPath,
            Action = _ => false
        });
        return results;
    }

    // Your custom logic here
    var searchTerm = query.Search.Trim();

    results.Add(new Result
    {
        Title = $"Result for: {searchTerm}",
        SubTitle = "Press Enter to execute",
        IcoPath = IconPath,
        Score = 100,
        Action = _ =>
        {
            // Your action here
            return true;
        }
    });

    return results;
}
```

---

## 🧪 Testing Your Plugin

### Manual Testing

**1. Build the plugin:**
```bash
dotnet build -c Release -p:Platform=x64
```

**2. Copy to PowerToys plugin directory:**
```bash
# Windows
xcopy /E /I /Y "bin\x64\Release\net9.0-windows10.0.22621.0" "%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\MyAwesomePlugin"

# PowerShell
Copy-Item -Path "bin\x64\Release\net9.0-windows10.0.22621.0\*" -Destination "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins\MyAwesomePlugin" -Recurse -Force
```

**3. Restart PowerToys:**
```bash
# Kill and restart
taskkill /F /IM PowerToys.exe
start "" "C:\Program Files\PowerToys\PowerToys.exe"
```

**4. Test the plugin:**
- Press `Alt+Space`
- Type your action keyword (e.g., `mycommand`)
- Verify results appear

### Unit Testing

If you used the solution template, write tests in the `UnitTests` project:

```csharp
[Fact]
public void QueryReturnsResults()
{
    // Arrange
    var plugin = new Main();
    plugin.Init(new PluginInitContext());
    var query = new Query { Search = "test" };

    // Act
    var results = plugin.Query(query);

    // Assert
    Assert.NotEmpty(results);
}
```

---

## 📦 Template Parameters Reference

### Project Template (ptrun-proj)

| Parameter      | Description                | Default        | Example            |
| -------------- | -------------------------- | -------------- | ------------------ |
| `-n, --name`   | Project name               | MyPlugin       | MyAwesomePlugin    |
| `-o, --output` | Output directory           | Current dir    | ./MyAwesomePlugin  |
| `--PluginAuthor` | Author name              | hlaueriksson   | "Your Name"        |

### Solution Template (ptrun-sln)

| Parameter      | Description                | Default        | Example            |
| -------------- | -------------------------- | -------------- | ------------------ |
| `-n, --name`   | Solution name              | MyPlugin       | MyAwesomePlugin    |
| `-o, --output` | Output directory           | Current dir    | ./MyAwesomePlugin  |
| `--PluginAuthor` | Author name              | hlaueriksson   | "Your Name"        |
| `--TestProject`  | Include test project     | true           | false              |

---

## 🎯 Example: Complete Workflow

Here's a complete example of creating and testing a plugin:

```bash
# 1. Create the solution
dotnet new ptrun-sln -n WeatherPlugin -o WeatherPlugin --PluginAuthor "John Doe"

# 2. Navigate to project
cd WeatherPlugin/Community.PowerToys.Run.Plugin.WeatherPlugin

# 3. Update plugin.json
# - Generate new GUID
# - Change ActionKeyword to "weather"
# - Update metadata

# 4. Add your custom icons
# - Replace Images/weatherplugin.dark.png
# - Replace Images/weatherplugin.light.png

# 5. Implement logic in Main.cs
# - Fetch weather data
# - Return formatted results

# 6. Build
dotnet build -c Release -p:Platform=x64

# 7. Install locally
xcopy /E /I /Y "bin\x64\Release\net9.0-windows10.0.22621.0" "%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\WeatherPlugin"

# 8. Restart PowerToys
taskkill /F /IM PowerToys.exe && start "" "C:\Program Files\PowerToys\PowerToys.exe"

# 9. Test
# Alt+Space → weather Seattle
```

---

## 🛠️ Advanced Customization

### Adding Settings UI

Use `ISettingProvider` and `AdditionalOptions`:

```csharp
public IEnumerable<PluginAdditionalOption> AdditionalOptions => new List<PluginAdditionalOption>
{
    new()
    {
        Key = "ApiKey",
        DisplayLabel = "API Key",
        DisplayDescription = "Your API key for the service",
        PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Textbox,
        TextValue = string.Empty
    },
    new()
    {
        Key = "EnableFeature",
        DisplayLabel = "Enable Advanced Feature",
        DisplayDescription = "Enables the advanced feature",
        PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
        Value = true
    }
};
```

### Adding Context Menu

Implement `IContextMenu`:

```csharp
public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
{
    return new List<ContextMenuResult>
    {
        new()
        {
            PluginName = Name,
            Title = "Copy to clipboard (Ctrl+C)",
            FontFamily = "Segoe MDL2 Assets",
            Glyph = "\xE8C8",
            AcceleratorKey = Key.C,
            AcceleratorModifiers = ModifierKeys.Control,
            Action = _ =>
            {
                Clipboard.SetDataObject(selectedResult.Title);
                return true;
            }
        }
    };
}
```

### Theme-Aware Icons

Handle theme changes:

```csharp
private void UpdateIconPath(Theme theme) =>
    IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite
        ? "Images/plugin.light.png"
        : "Images/plugin.dark.png";

private void OnThemeChanged(Theme currentTheme, Theme newTheme) =>
    UpdateIconPath(newTheme);
```

---

## 🔍 Troubleshooting

### Template Not Found

```bash
# Verify installation
dotnet new list | grep ptrun

# Reinstall if needed
dotnet new uninstall Community.PowerToys.Run.Plugin.Templates
dotnet new install Community.PowerToys.Run.Plugin.Templates
```

### Plugin Not Showing in PowerToys

1. ✅ Verify files are in correct directory
2. ✅ Check `plugin.json` has valid JSON
3. ✅ Ensure DLL name matches `ExecuteFileName`
4. ✅ Restart PowerToys completely
5. ✅ Check PowerToys logs for errors

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build -c Release -p:Platform=x64
```

### GUID Collision

If you get a "Plugin with this ID already exists" error:
1. Generate a new GUID
2. Update `ID` field in `plugin.json`
3. Rebuild and reinstall

---

## 📚 Additional Resources

### Official Documentation
- [PowerToys Documentation](https://learn.microsoft.com/en-us/windows/powertoys/)
- [Plugin Development Guide](https://github.com/microsoft/PowerToys/blob/main/doc/devdocs/modules/launcher/plugins/README.md)
- [dotnet new Templates](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates)

### Example Plugins
- [AI Prompt Generator](https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator)
- [Community Plugins](https://github.com/topics/powertoys-run-plugin)

### Tools
- [Icons8](https://icons8.com) - Icon resources
- [NuGet Package Explorer](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer) - Inspect packages
- [GUID Generator](https://www.guidgenerator.com/) - Generate unique IDs

---

## 🤝 Contributing

Found a bug in the templates? Have a suggestion?

1. Open an issue: [GitHub Issues](https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator/issues)
2. Submit a pull request with improvements
3. Share your plugins with the community!

---

## 📄 License

Templates are provided under the MIT License. Generated code is yours to use however you like.

---

## 🌟 Template Credits

**Original Template Creator:** [Henrik Lau Eriksson](https://github.com/hlaueriksson)
**Package:** Community.PowerToys.Run.Plugin.Templates
**Version:** 0.3.0
**Repository:** https://github.com/ruslanlap/PowerToysRun-AIPromptGenerator

---

**Happy Plugin Development! 🚀**

*Last Updated: 2025*
