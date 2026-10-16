# Change Log

## 0.5.1 (pre-release)

- Play the debugged scenario in the "SRC# 画面" tab (`interactive` launch attribute, used by the run buttons, F5 and the launch.json template): frames are sent from the debug adapter, and clicks, keys, the mouse wheel, text input (`Input`) and file dialogs are passed to the scenario. No sound. Breakpoints and stepping work as before.
- Without `interactive`, show the game screen, refreshed when the debugger stops and when the scenario ends, and every 0.5 seconds while running.
- Both require a srcsharp that supports the screen requests (later than 0.5.1).
- Hot reload (`srcsharp.hotReload`): when event files, data or images of the debugged scenario change, reload them keeping the play state. Events and data are reloaded while the player phase waits for a map command, by saving the state like a quick save, reloading, and restoring it like a quick load. Events and data are checked first and not reloaded if they have errors. Images are reloaded at once. Reload manually from the screen tab or the debug toolbar.
- Pass `srcsharp.appPath` to the debugger, so the scenario loads the SRC folder's common files (Lib, Data/System, Bitmap).
- Add the SRC# control panel to the activity bar: status of srcsharp, the language server and the SRC folder, and buttons to run, debug, show the screen, lint, make the balance report and open the settings.
- Add a settings page (`SRC#: 設定を開く`) to set the srcsharp path, its arguments, the SRC folder and the display language with a form.
- Find srcsharp in `~/.dotnet/tools` even when it is not on PATH, set `DOTNET_ROOT` when dotnet is installed in a non-default location, and install or update srcsharp (`dotnet tool update -g SRCSharpTools`) from a notification or the control panel.
- Suggest the SRC folder (`srcsharp.appPath`) found in the workspace, and pick it from candidates (`SRC#: SRC 本体のフォルダを選ぶ`).
- Restart the language server when the settings change, without reloading the window.
- Run or debug the open event from the editor title, and with F5 without launch.json.
- Add `srcsharp.language` and `srcsharp.screen.autoOpen`.
- Change the default encoding of event and data files from Shift_JIS to UTF-8, which SRC# recommends. Shift_JIS files are still opened by `files.autoGuessEncoding`.

## 0.5.0 (pre-release)

The first version of SRC# Authoring. It replaces the earlier syntax-only extension of the same ID (`koudenpa.src-vscode` 0.0.x, developed in [7474/SRC-VSCode](https://github.com/7474/SRC-VSCode)). Development has moved to [7474/SRC](https://github.com/7474/SRC/tree/master/SRC.Sharp/SRCSharpVSCode).

- Syntax highlighting, the language server (diagnostics, outline, go to definition, hover, completion), the event debugger and the lint / balance / auto-play commands, through the `srcsharp` .NET tool.
- Keep the SRC data language (`src-data`, `#` comments). It is now associated with SRC's data file names (`robot.txt`, `unit.txt`, `pilot.txt`, ...) instead of every `.txt` file. Use `files.associations` for other files.
- Bundle the extension with esbuild so the packaged .vsix includes its runtime dependency (vscode-languageclient).
- Add Marketplace metadata (English description, keywords, gallery banner, homepage, bugs), LICENSE and this change log.

## 0.0.5 and earlier

- Syntax highlighting for SRC event files (`*.eve`) and data files (published from 7474/SRC-VSCode).
