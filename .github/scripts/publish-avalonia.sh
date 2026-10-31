#!/usr/bin/env bash
# SRC#Avalonia を、配る形（自己完結の単一ファイル）で発行して zip にする。
# Publish-Avalonia.yml（リリース）と Packages.yml（master への push ごとの確かめ）が同じ手順で作る（#1359）
# 使い方: .github/scripts/publish-avalonia.sh <RID> → SRCSharpAvalonia-<RID>.zip
# https://learn.microsoft.com/dotnet/core/deploying/single-file/overview
set -euo pipefail
rid="$1"
out="out/${rid}"
dotnet publish SRC.Sharp/SRCSharpAvalonia/SRCSharpAvalonia.csproj -c Release -r "${rid}" \
  --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:DebugType=none -o "${out}"
# ネイティブライブラリの pdb（SkiaSharp など）は要らない
rm -f "${out}"/*.pdb
cp LICENSE "${out}/"
# zip は実行権限を保つ（macOS・Linux 向け）
(cd "${out}" && zip -qr "../../SRCSharpAvalonia-${rid}.zip" .)
