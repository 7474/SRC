#!/usr/bin/env bash
# tools/pack-stats・tools/pack-analysis（GSC データパックの集計と組の評価の試作）を、リポジトリの中の小さなデータで動かす（7474/SRC#1616）。
# この 2 つは srcsharp の機能ではないが、7474/SRC-Data の GitHub Pages（site/build.sh）が master のものをビルドして使う。
# SRCCore・SRCAuthoring の変更で壊れたら、マージの前に分かるようにする。結果の中身は確かめず、ビルドできて落ちないことだけを見る。
# 動かすのは SRC-Data の site/build.sh が使うもの（組の評価・site）と check。ほかのコマンドは、小さなデータでは
# 集計の層が空になって落ちるものがある（試作の性質で、GSC データパックでは起きない）ので動かさない。
# 使い方: tools/pack_tools_check.sh [作業フォルダ]
set -euo pipefail
ROOT=$(cd "$(dirname "$0")/.." && pwd)
WORK=${1:-$(mktemp -d)}
mkdir -p "$WORK"
DATA="$ROOT/SRC.Sharp/Compat/torres/Data"

dotnet build "$ROOT/tools/pack-stats" -c Release -v q
dotnet build "$ROOT/tools/pack-analysis" -c Release -v q
PS="$ROOT/tools/pack-stats/bin/Release/net10.0/PackStats.dll"
PA="$ROOT/tools/pack-analysis/bin/Release/net10.0/PackAnalysis.dll"

# GSC データパックと同じ形（<パック>/ロボ・<パック>/拡張 の下に System と作品のフォルダ）にする
rm -rf "$WORK/pack"
mkdir -p "$WORK/pack"
cp -r "$DATA" "$WORK/pack/ロボ"
cp -r "$DATA" "$WORK/pack/拡張"
ls "$WORK/pack/拡張" | grep -v '^System$' | grep -v '\.txt$' > "$WORK/works.txt" || true

run() {
  echo "== $*"
  "$@" > "$WORK/out.txt" 2> "$WORK/err.txt" || { cat "$WORK/err.txt"; echo "失敗: $*" >&2; exit 1; }
  test -s "$WORK/out.txt" || { echo "出力が無い: $*" >&2; exit 1; }
}

run dotnet "$PS" bands "$WORK/pack/拡張"
run dotnet "$PS" check "$WORK/pack/拡張" "$DATA"
run dotnet "$PA" "$WORK/pack/拡張" 拡張 "$ROOT/tools/pack-analysis/roles-ext.csv" "$WORK/works.txt" --tiers "$ROOT/tools/pack-stats/tiers-ext.tsv" --sample 5 --cards 1
cp "$WORK/out.txt" "$WORK/拡張.md"
run dotnet "$PA" "$WORK/pack/ロボ" ロボ "$ROOT/tools/pack-analysis/roles-robo.csv" "$WORK/works.txt" --tiers "$ROOT/tools/pack-stats/tiers-robo.tsv" --sample 5 --cards 1
cp "$WORK/out.txt" "$WORK/ロボ.md"
echo "== site"
rm -rf "$WORK/site"
dotnet "$PS" site "$WORK/site" "$WORK/pack" --analysis "$WORK/拡張.md" "$WORK/ロボ.md" > /dev/null
test -n "$(find "$WORK/site" -name '*.md' -print -quit)" || { echo "site のページが無い" >&2; exit 1; }
echo "OK"
