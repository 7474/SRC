# Change Log

## 0.5.1 (pre-release)

- Show the game screen of the debugged scenario in the "SRC# 画面" view. It refreshes when the debugger stops and when the scenario ends, and every 0.5 seconds while running. Requires a srcsharp that supports the `srcsharpScreen` request (later than 0.5.1).
- Add the SRC# control panel to the activity bar: status of srcsharp, the language server and the SRC folder, and buttons to run, debug, show the screen, lint, make the balance report and open the settings.
- Add a settings page (`SRC#: 設定を開く`) to set the srcsharp path, its arguments, the SRC folder and the display language with a form.
- Find srcsharp in `~/.dotnet/tools` even when it is not on PATH, set `DOTNET_ROOT` when dotnet is installed in a non-default location, and install or update srcsharp (`dotnet tool update -g SRCSharpTools`) from a notification or the control panel.
- Suggest the SRC folder (`srcsharp.appPath`) found in the workspace, and pick it from candidates (`SRC#: SRC 本体のフォルダを選ぶ`).
- Restart the language server when the settings change, without reloading the window.
- Run or debug the open event from the editor title, and with F5 without launch.json.
- Add `srcsharp.language` and `srcsharp.screen.autoOpen`.

## 0.5.0 (pre-release)

The first version of SRC# Authoring. It replaces the earlier syntax-only extension of the same ID (`koudenpa.src-vscode` 0.0.x, developed in [7474/SRC-VSCode](https://github.com/7474/SRC-VSCode)). Development has moved to [7474/SRC](https://github.com/7474/SRC/tree/master/SRC.Sharp/SRCSharpVSCode).

- Syntax highlighting, the language server (diagnostics, outline, go to definition, hover, completion), the event debugger and the lint / balance / auto-play commands, through the `srcsharp` .NET tool.
- Keep the SRC data language (`src-data`, `#` comments). It is now associated with SRC's data file names (`robot.txt`, `unit.txt`, `pilot.txt`, ...) instead of every `.txt` file. Use `files.associations` for other files.
- Bundle the extension with esbuild so the packaged .vsix includes its runtime dependency (vscode-languageclient).
- Add Marketplace metadata (English description, keywords, gallery banner, homepage, bugs), LICENSE and this change log.

## 0.0.5 and earlier

- Syntax highlighting for SRC event files (`*.eve`) and data files (published from 7474/SRC-VSCode).
