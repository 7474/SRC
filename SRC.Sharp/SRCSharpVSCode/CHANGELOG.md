# Change Log

## 0.5.5

- The version follows the SRC# library version (srcsharp 0.5.5).
- No changes to the extension itself. srcsharp 0.5.5 adds the "オートプレイ" map command and the fixes listed in the SRC# 3.5.5 release notes.

## 0.5.4

- The version follows the SRC# library version (srcsharp 0.5.4).
- Add "開いているイベントを自動プレイ（画面付き・早送り）" (`srcsharp.autoPlay`) to the editor title, the control panel and the command palette. It runs the open event in the debugger with `skipWait`, with a `.play` script picked from the workspace or without one, and shows the play in the "SRC# 画面" tab.
- Add the `skipWait`, `policy` and `turns` launch attributes. Without `script` and `interactive`, the debugger now plays the player phases with `policy` (default `smart`) and picks "次のステージへ" in the intermission, instead of only sending messages. `maxSteps` is no longer used.
- Without `interactive`, the screen tab shows the frames sent by the debug adapter instead of refreshing every 0.5 seconds (srcsharp 0.5.4 or later).
- Add the `scenarioRoot` launch attribute: the scenario folder (the base of Data, Lib and Continue). By default, an event file in a subfolder without Data runs with the nearest parent folder that has Data, so chapters kept in a subfolder can be run and debugged on their own.
- Add the launch.json snippet "SRC#: 自動プレイ（画面付き・早送り）", and set `skipWait` in the ".play スクリプトで実行" snippet.

## 0.5.3

0.5.2 was not published to the Marketplace. This release includes its changes.

- The version now follows the SRC# library version (srcsharp 0.5.3).
- Add the extension icon and change the activity bar icon to "SRC#".
- When srcsharp runs in WSL, the game screen uses the Windows Japanese fonts (ＭＳ ゴシック・ＭＳ 明朝). When no Japanese font is found, the debugger shows a notice (srcsharp 0.5.3).
- Add `srcsharp.logLevel` (also on the settings page) and the `logLevel` launch attribute. The language server's log goes to the "SRC#" output panel and the debugger's log to the debug console (warnings and errors as stderr), both also to log files. Requires a srcsharp that supports logging (0.5.3 or later).
- The "SRC#" output panel is now a log output channel (levels and timestamps, filtered with "Developer: Set Log Level...").

## 0.5.2 (not published)

- The version now follows the SRC# library version (srcsharp 0.5.2).
- Add the extension icon and change the activity bar icon to "SRC#".

## 0.5.1

- Play the debugged scenario in the "SRC# 画面" tab (`interactive` launch attribute, used by the run buttons, F5 and the launch.json template): frames are sent from the debug adapter, and clicks, keys, the mouse wheel, text input (`Input`) and file dialogs are passed to the scenario. No sound. Breakpoints and stepping work as before.
- Without `interactive`, show the game screen, refreshed when the debugger stops and when the scenario ends, and every 0.5 seconds while running.
- Both require a srcsharp that supports the screen requests (0.5.2 or later).
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
