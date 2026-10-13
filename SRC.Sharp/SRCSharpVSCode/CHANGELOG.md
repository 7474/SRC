# Change Log

## 0.5.0 (pre-release)

The first version of SRC# Authoring. It replaces the earlier syntax-only extension of the same ID (`koudenpa.src-vscode` 0.0.x, developed in [7474/SRC-VSCode](https://github.com/7474/SRC-VSCode)). Development has moved to [7474/SRC](https://github.com/7474/SRC/tree/master/SRC.Sharp/SRCSharpVSCode).

- Syntax highlighting, the language server (diagnostics, outline, go to definition, hover, completion), the event debugger and the lint / balance / auto-play commands, through the `srcsharp` .NET tool.
- Keep the SRC data language (`src-data`, `#` comments). It is now associated with SRC's data file names (`robot.txt`, `unit.txt`, `pilot.txt`, ...) instead of every `.txt` file. Use `files.associations` for other files.
- Bundle the extension with esbuild so the packaged .vsix includes its runtime dependency (vscode-languageclient).
- Add Marketplace metadata (English description, keywords, gallery banner, homepage, bugs), LICENSE and this change log.

## 0.0.5 and earlier

- Syntax highlighting for SRC event files (`*.eve`) and data files (published from 7474/SRC-VSCode).
