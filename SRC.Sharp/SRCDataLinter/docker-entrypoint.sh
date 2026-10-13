#!/bin/sh
# 最初の引数が srcsharp のコマンドなら srcsharp（lint・自動プレイ・バランス集計など）、
# それ以外は従来どおり SRCDataLinter（引数のディレクトリ・ファイルを検査する）を動かす。
case "$1" in
  lint|play|balance|lsp|dap|help|-h|--help)
    if [ "$1" = "help" ]; then
      shift
      set -- --help "$@"
    fi
    exec dotnet /app/srcsharp.dll "$@"
    ;;
  *)
    exec dotnet /app/SRCDataLinter.dll "$@"
    ;;
esac
