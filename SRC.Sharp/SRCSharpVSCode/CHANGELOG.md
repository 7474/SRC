# Change Log

## 0.6.0

This version replaces the earlier syntax-only extension of the same ID (`koudenpa.src-vscode` 0.0.x, developed in [7474/SRC-VSCode](https://github.com/7474/SRC-VSCode)). Development has moved to [7474/SRC](https://github.com/7474/SRC/tree/master/SRC.Sharp/SRCSharpVSCode).

- Add the language server (diagnostics, outline, go to definition, hover, completion), the event debugger and the lint / balance / auto-play commands from SRC# Authoring (`srcsharp`).
- Keep the SRC data language (`src-data`, `#` comments). It is now associated with SRC's data file names (`robot.txt`, `unit.txt`, `pilot.txt`, ...) instead of every `.txt` file. Use `files.associations` for other files.
- Bundle the extension with esbuild so the packaged .vsix includes its runtime dependency (vscode-languageclient).
- Add Marketplace metadata (English description, keywords, gallery banner, homepage, bugs), LICENSE and this change log.

## 0.5.0 (not published)

- First version of SRC# Authoring: syntax highlighting, diagnostics, outline, go to definition and completion for SRC event files (`*.eve`) through the `srcsharp` language server, an event debugger, and commands to lint, create a balance report and auto-play.

## 0.0.5 and earlier

- Syntax highlighting for SRC event files (`*.eve`) and data files (published from 7474/SRC-VSCode).
