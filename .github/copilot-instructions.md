# SRC# (Simulation RPG Construction Sharp)

SRC# is a C# port of the original SRC (Simulation RPG Construction) game creation system. It's a .NET 8 multi-project solution that includes a core library, Windows Forms application, Blazor web application, CLI data validation tool, and various utilities.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Bootstrap and Build Process
- Install .NET 8 SDK (minimum required):
  - `dotnet --version` should show 8.0.x or higher
  - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
- Install Node.js 20+ for Blazor CSS building:
  - `node --version` should show v20.x or higher  
  - `npm --version` should show 10.x or higher
- Navigate to SRC.Sharp directory: `cd SRC.Sharp/`
- Restore dependencies: `dotnet restore SRC.Sharp.sln` -- takes 45-60 seconds. NEVER CANCEL.
- Build core projects: `dotnet build SRCCore/SRCCore.csproj SRCDataLinter/SRCDataLinter.csproj SRCTestBlazor/SRCTestBlazor.csproj --no-restore` -- takes 30-45 seconds. NEVER CANCEL.

### Platform-Specific Build Limitations
**IMPORTANT**: Some projects require Windows and will fail on Linux/macOS:
- `SRCSharpForm` (Windows Forms app) - requires Microsoft.NET.Sdk.WindowsDesktop
- `SRCTestForm` (Windows Forms test app) - requires Microsoft.NET.Sdk.WindowsDesktop  
- Building the entire solution (`SRC.Sharp.sln`) will fail on Linux/macOS due to these projects

**Cross-platform projects that work everywhere:**
- `SRCCore` (.NET Standard 2.1) - Core library
- `SRCDataLinter` (.NET 8) - CLI data validation tool
- `SRCTestBlazor` (.NET 8) - Blazor WebAssembly application
- Tool projects in `/tools/` directory

### Running Tests
- **CRITICAL**: Tests target .NET 8, should work on systems with .NET 8 SDK
- Run tests: `dotnet test SRCCoreTests/SRCCoreTests.csproj` -- takes 5-10 seconds

### Running Applications

#### Blazor Web Application (SRCTestBlazor)
- **ALWAYS build Blazor CSS first**: 
  - `cd SRCTestBlazor/Npm && npm install` -- takes 45-60 seconds. NEVER CANCEL.
  - `npm run build` -- takes 5-10 seconds
- Start the web application: `cd .. && dotnet run --urls "http://localhost:5123"` -- starts in 10-15 seconds
- Access at: http://localhost:5123
- **MANUAL VALIDATION**: Navigate between Unit/Pilot/Item tabs, verify data displays correctly

#### CLI Data Linter (SRCDataLinter)  
- Test help: `dotnet run --project SRCDataLinter/SRCDataLinter.csproj -- --help`
- Docker alternative: `docker run --rm -v /path/to/data:/app/data -t koudenpa/srcdatalinter data`

#### Windows Forms Applications (Windows only)
- `SRCSharpForm` - Main Windows Forms application
- `SRCTestForm` - Data viewing test form
- Both require Windows and .NET 8 runtime

## Validation Scenarios

### Always Test After Changes
1. **Core Library**: Build `SRCCore` successfully
2. **Data Linter**: Build `SRCDataLinter` successfully  
3. **Blazor App**: Build CSS, start application, navigate tabs, verify UI responsiveness
4. **Unit Tests**: Run `SRCCoreTests` (adjust .NET version if needed)
5. **Platform Check**: Verify builds work on target platform (Windows vs Linux/macOS)

### Time Expectations and Timeouts
- **NEVER CANCEL**: All build operations complete within expected timeframes
- Package restore: 45-60 seconds (set timeout to 120+ seconds)
- Individual project builds: 5-30 seconds each (set timeout to 60+ seconds)  
- Full cross-platform build: 60-90 seconds (set timeout to 180+ seconds)
- Test execution: 5-10 seconds (set timeout to 30+ seconds)
- Blazor app startup: 10-15 seconds (set timeout to 30+ seconds)

### CI/CD Requirements
- **Always run on appropriate platform**: CI uses Windows for full builds, Linux for web deployments
- **Before committing**: Verify your platform-specific builds pass
- **Blazor deployments**: Require both .NET publish and CSS build steps

## Common Tasks

### Build Individual Projects (Cross-platform)
```bash
# Core library - works everywhere
dotnet build SRCCore/SRCCore.csproj

# Data linter CLI - works everywhere  
dotnet build SRCDataLinter/SRCDataLinter.csproj

# Blazor web app - works everywhere
dotnet build SRCTestBlazor/SRCTestBlazor.csproj

# Utility tools - work everywhere
dotnet build ../tools/Template/Template.csproj
```

### Build CSS for Blazor
```bash
cd SRCTestBlazor/Npm
npm install  # Only needed once or when package.json changes
npm run build
```

### Repository Structure Reference
```
SRC.Sharp/                    # Main C# projects
├── SRCCore/                  # Core library (.NET Standard 2.1)
├── SRCSharpForm/            # Windows Forms app (.NET 8, Windows only)  
├── SRCTestForm/             # Test Forms app (.NET 8, Windows only)
├── SRCTestBlazor/           # Blazor WebAssembly app (.NET 8)
│   ├── Npm/                 # CSS build tools (Node.js/npm)
│   └── E2E/                 # Cypress end-to-end tests
├── SRCDataLinter/           # CLI data validation (.NET 8)
├── SRCCoreTests/            # Unit tests (.NET 8)
└── SRC.Sharp.sln            # Main solution (has Windows dependencies)

tools/                       # Utility projects (.NET 8)
├── Template/                # Code generation template
├── ReplaceArgx/            # Argument replacement tool
└── ToMackerel/             # Mackerel integration tool

terraform/                   # Infrastructure as code
SRC/                        # Original VB source (reference)
SRC.NET/                    # Legacy .NET port
```

### Key Files to Monitor
- Always check `SRCCore/` after making changes to core game logic
- Always rebuild CSS (`npm run build` in `SRCTestBlazor/Npm/`) after styling changes  
- Always test `SRCDataLinter` functionality after data validation changes
- Check `.github/workflows/CI.yml` for Windows-specific build requirements

## Known Issues and Workarounds

### .NET 8 Runtime Availability
- **Updated**: All projects now target .NET 8, which provides better compatibility and performance
- All applications and tools now run consistently on .NET 8

### Windows Desktop Dependencies  
- **Issue**: `Microsoft.NET.Sdk.WindowsDesktop` projects fail on Linux/macOS
- **Workaround**: Build individual cross-platform projects instead of full solution on non-Windows systems

### Network-Dependent Tools
- **Issue**: Cypress E2E tests and some npm packages require internet connectivity  
- **Workaround**: Skip E2E tests in offline environments, focus on unit tests and manual UI validation

### CSS Build Dependencies
- **Issue**: Node.js/npm required for Blazor styling, uses deprecated node-sass
- **Expected Warnings**: node-sass deprecation warnings are normal and don't affect functionality
- **Workaround**: CSS builds still work despite deprecation warnings

## Development Workflow Best Practices
- Always build on your target platform before submitting changes
- Use cross-platform projects for core functionality development  
- Test Blazor UI changes by running the application and validating user scenarios
- Validate data linter changes with sample SRC data files if available
- Monitor build times and report if they exceed expected ranges significantly