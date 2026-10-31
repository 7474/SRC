# 準拠データパックの分析の試作 / Data-Pack Analysis Prototype

[準拠データパックに応じたバランスの検査（設計）](../../docs/tools/balance-pack.md) の「役割と格の決め方」「組の評価」を試しに実装したもの。GSC データパックにかけた結果は [試作結果](../../docs/tools/balance-pack-analysis.md) にある。srcsharp の機能ではなく、ソリューション・CI には入れない。

## 再現

GSC データパックは [7474/SRC-Data](https://github.com/7474/SRC-Data) の `GSCデータパック` を使う。試作結果は SRC-Data のコミット `a802b31`（2026-05-06）で計測した。

```bash
git clone https://github.com/7474/SRC-Data ../SRC-Data
```

```bash
git -C ../SRC-Data checkout a802b31
```

```bash
dotnet build tools/pack-analysis -c Release
```

```bash
dotnet tools/pack-analysis/bin/Release/net10.0/PackAnalysis.dll ../SRC-Data/GSCデータパック/ロボ ロボ tools/pack-analysis/roles-robo.csv tools/pack-analysis/works-robo.txt > robo.md
```

```bash
dotnet tools/pack-analysis/bin/Release/net10.0/PackAnalysis.dll ../SRC-Data/GSCデータパック/拡張 拡張 tools/pack-analysis/roles-ext.csv tools/pack-analysis/works-ext.txt > ext.md
```

それぞれ 10 分ほどかかる（ロボ・拡張とも、帯ごとに最大 400 組を評価する）。標準出力に Markdown、標準エラーに進み具合と、読み込めないデータ・評価できない組を出す。

```text
PackAnalysis <パックのフォルダ> <ロボ|拡張> <役割表.csv|-> <対象作品.txt> [--sample 数] [--seed 数] [--level 数]
```

| 引数 | 内容 |
|---|---|
| パックのフォルダ | `GSCデータパック/ロボ` か `GSCデータパック/拡張` |
| `ロボ`・`拡張` | 組の決め方を変える（拡張は名前の一致、ロボはクラスの一致と `#推奨ユニット`） |
| 役割表 | 下の「役割表」。使わなければ `-` |
| 対象作品 | 結果を出す作品のフォルダ名を 1 行に 1 つ。`#` で始まる行は結果の冒頭に注記として出す（選んだ根拠と出典を書く） |
| `--sample` | 帯ごとに比べる組の数（既定 400） |
| `--seed` | 比べる組を選ぶ乱数の種（既定 1） |
| `--level` | パイロットのレベル（既定 30） |

同じ SRC-Data・同じ引数なら同じ結果になる（ファイルの読み込み順と乱数の種を固定している）。

## 役割表

| 列 | 内容 |
|---|---|
| 種類 | `pilot` |
| 名前 | パイロットの名前 |
| 役割 | `戦闘`・`艦長`・`名前なし`（組にする）、`サブ`（乗るユニットにサブパイロットとして乗せる）、`ボス`（ボス戦の相手にする） |
| 格 | 主人公格・準主役格・サブキャラ格・端役格・名前なし（今は表示だけ） |
| 系統 | スーパー・リアル・グレー（今は使わない） |
| 乗るユニット | ユニットの名前。`ボス` で省くと、名前の一致などで決めた組のユニットを使う |

`#` で始まる行は読み飛ばす。

| ファイル | 内容 |
|---|---|
| `works-robo.txt` | スーパーロボット大戦シリーズの参戦回数の上位作品のうち、GSC のロボ基準にあるもの |
| `roles-robo.csv` | その作品の、最終的な編成の機体での組・サブパイロット・ボス戦の相手（6 組）。格は試作者の判断 |
| `works-ext.txt` | 原作の漫画の累計発行部数の上位作品のうち、GSC の拡張基準にあるもの |
| `roles-ext.csv` | ボス戦の相手（6 組）だけ。拡張は組を名前の一致で作る |

## 計算のあらまし

詳しくは設計と試作結果を参照。

- ゲームと同じ計算（SRCCore の `UnitWeapon.HitProbability`・`ExpDamage`）を、小さな仮のマップ（12×3）に組と相手を置いて行う。地形はユニットに合わせる
- SP は `Unit.MakeSpecialPowerInEffect` でユニットにかける（入れ子の SP も展開する）。ボス戦はダメージ・命中を上げる SP、回避・囮は回避を上げる・被ダメージを下げる SP を、効果の種類ごとに最も強いものを 1 つずつ使う。絶対回避の SP は使える回数を耐える回数に足す
- 評価（S〜D）は、同じ経験値の帯の組の中での順位

## 制約

- 追加パイロットとパイロット能力付加を両方持つユニットは評価しない（SRCCore が落ちる。https://github.com/7474/SRC/issues/1407 ）
- 試作なので、例外が出た組は「評価できない」として飛ばす
