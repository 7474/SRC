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
# パックの直下のデータ（小さなデータのパイロット・ユニットはここにある）は、パックのフォルダの名前の作品として扱われる
printf 'ロボ\n拡張\n' >> "$WORK/works.txt"

run() {
  echo "== $*"
  "$@" > "$WORK/out.txt" 2> "$WORK/err.txt" || { cat "$WORK/err.txt"; echo "失敗: $*" >&2; exit 1; }
  test -s "$WORK/out.txt" || { echo "出力が無い: $*" >&2; exit 1; }
}

run dotnet "$PS" bands "$WORK/pack/拡張"
run dotnet "$PS" check "$WORK/pack/拡張" "$DATA"
run dotnet "$PA" "$WORK/pack/拡張" 拡張 "$ROOT/tools/pack-analysis/roles-ext.csv" "$WORK/works.txt" --tiers "$ROOT/tools/pack-stats/tiers-ext.tsv" --sample 5 --cards 1
cp "$WORK/out.txt" "$WORK/拡張.md"
run dotnet "$PA" "$WORK/pack/ロボ" ロボ "$ROOT/tools/pack-analysis/roles-robo.csv" "$WORK/works.txt" --tiers "$ROOT/tools/pack-stats/tiers-robo.tsv" --sample 5 --cards 1 --json "$WORK/pairs-ロボ.json"
cp "$WORK/out.txt" "$WORK/ロボ.md"
# 組の評価の JSON（データビューアの「比べる」が読む）の形を確かめる
echo "== json"
python3 - "$WORK/pairs-ロボ.json" <<'PY'
import json, sys
d = json.load(open(sys.argv[1], encoding="utf-8"))
for k in ("format", "pack", "generatedAt", "uses", "indices", "bands", "opponents", "works", "pairs", "pilots", "units"):
    assert k in d, k
assert d["format"] == 2 and d["pack"] == "ロボ"
assert d["pairs"], "組が無い"
for p in d["pairs"]:
    for k in ("work", "how", "confidence", "band", "role", "rank", "pilot", "unit", "evaluated"):
        assert k in p, k
    assert p["pilot"]["name"] and p["unit"]["name"]
    if p["evaluated"]:
        assert set(p["grades"]) == set(d["uses"]), p["grades"]
        assert set(p["indices"]) == set(d["indices"]), p["indices"]
    assert p["confidence"] in ("確か", "推定"), p["confidence"]
assert d["pilots"] and d["units"], "パイロット・ユニットが無い"
for p in d["pilots"]:
    for k in ("name", "work", "band", "stats", "sp"):
        assert k in p, k
for u in d["units"]:
    for k in ("name", "work", "hp", "en", "armor", "mobility", "speed", "values"):
        assert k in u, k
print("json:", len(d["pairs"]), "組", len(d["pilots"]), "パイロット", len(d["units"]), "ユニット")
PY

# 役割表・格の一覧・対象作品の名前の確かめと、役割表のひな形
# 小さなデータには GSC の役割表の名前が無いので、名前の無いものを知らせる（終了コード 1）ことを確かめる
if dotnet "$PA" "$WORK/pack/ロボ" ロボ "$ROOT/tools/pack-analysis/roles-robo.csv" "$WORK/works.txt" --check-roles 1 > "$WORK/check-roles.txt"; then
  echo "役割表の名前の確かめが、無い名前を知らせない" >&2; exit 1
fi
grep -q 'roles-robo.csv([0-9]*): warning: パイロット' "$WORK/check-roles.txt" || { cat "$WORK/check-roles.txt"; echo "役割表の名前の確かめの書式が違う" >&2; exit 1; }
dotnet "$PA" "$WORK/pack/拡張" 拡張 - "$WORK/works.txt" --roles-template "$(head -1 "$WORK/works.txt")" > "$WORK/template.csv"
grep -q '^# 種類,名前,役割,格,系統,乗るユニット' "$WORK/template.csv" || { echo "役割表のひな形が出ない" >&2; exit 1; }

echo "== site"
rm -rf "$WORK/site"
dotnet "$PS" site "$WORK/site" "$WORK/pack" --analysis "$WORK/拡張.md" "$WORK/ロボ.md" > /dev/null
test -n "$(find "$WORK/site" -name '*.md' -print -quit)" || { echo "site のページが無い" >&2; exit 1; }
echo "OK"
