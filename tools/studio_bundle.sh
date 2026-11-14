#!/usr/bin/env bash
# SRC#Studio（SRC.Sharp/SRCStudio）を発行し、配る形にする（7474/SRC#1641）。
# 発行した wwwroot に、自作の最小の汎用データ（SRC.Sharp/Content/basic-data、CC0）の地形データとマップの画像を basic-data/ として足し、
# その目次 basic-data/files.json を書く（SRC 本体のフォルダを開かなくても試せるように）。
# GitHub Pages（/SRC/studio/）・VS Code 拡張（studio/）・E2E が使う。
# 使い方: tools/studio_bundle.sh <出力のフォルダ>   （出力のフォルダに wwwroot の中身を置く）
set -euo pipefail
ROOT=$(cd "$(dirname "$0")/.." && pwd)
OUT=${1:?出力のフォルダ}
WORK=$(mktemp -d)
dotnet publish "$ROOT/SRC.Sharp/SRCStudio/SRCStudio.csproj" -c Release -o "$WORK" -v q
mkdir -p "$OUT"
cp -r "$WORK/wwwroot/." "$OUT/"
BASIC="$ROOT/SRC.Sharp/Content/basic-data"
mkdir -p "$OUT/basic-data/Data/System" "$OUT/basic-data/Bitmap"
cp "$BASIC/Data/System/terrain.txt" "$OUT/basic-data/Data/System/"
cp -r "$BASIC/Bitmap/Map" "$OUT/basic-data/Bitmap/"
cp "$BASIC/LICENSE" "$OUT/basic-data/" 2>/dev/null || true
(cd "$OUT/basic-data" && find . -type f ! -name files.json | sed 's|^\./||' | sort | python3 -c 'import json,sys; print(json.dumps([l.strip() for l in sys.stdin if l.strip()], ensure_ascii=False))' > files.json)
rm -rf "$WORK"
echo "SRC#Studio を $OUT に置いた"
