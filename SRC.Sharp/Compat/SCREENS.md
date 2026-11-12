# 画面と音の見比べ（#1553）

元の SRC（VB5 の Src.exe。Ver.2.2.33）と SRC# で、主な画面を決まった状態で出し、スクリーンショットを並べて見比べるためのシナリオと道具（#1553。親は #1548）。音は、効果音が鳴る場面と BGM が切り替わる場面を一覧にし、実機と聞き比べる。

- 画面はピクセルで比べない（フォント・ウィンドウの枠などで違う）。並べた一覧を人が見て、配置・色・文字・大きさの違いを表にする
- Windows での作業（Src.exe の画面を撮る・音を聞く）は、#1550 のプローブと 1 回で済むように [PROBE.md の手順](PROBE.md#windows-で流す手順) にまとめた

2026-11-12 に、日本語版の Windows 11（画面の拡大率 100%）で元の SRC（Ver.2.2.33）に流し、全場面のスクリーンショットを `screens-expected/` に、聞いた音を `screens-expected/sounds.txt` に置いた。シナリオは元の SRC で最後まで動き、`screens-scenes.txt` の操作で同じ場面になった（場面 20 は、味方のフェイズの初めに表示の位置が動くので、動いた後の (8,12) を右クリックする）。見比べた結果は下の「[見比べた結果](#見比べた結果)」。

## 置き場所と構成

| 置き場所 | 中身 |
|---|---|
| `screens/screens.eve`・`screens.map` | 見比べのシナリオ。前半（スタートのイベント）は、場面ごとに場面の名前のメッセージを出し、クリックで閉じると場面を出して `Wait Click` で止まる。後半（1 ターン目の味方フェイズ）は、人がクリックしてメニュー・一覧・移動範囲を出す。ターンを終えると、敵のフェイズの初めにインターミッションへ進む |
| `screens/screens_end.eve` | インターミッションの「次のステージへ」で読む終わりのイベント（`Quit`） |
| `screens/Src.ini` | 見比べのための設定。元の SRC はシナリオのフォルダの `Src.ini` を本体のものより先に読む（`GeneralLib.bas` の `ReadIni`）。SRC# も読む。旧 GUI（`NewGUI=Off`）、戦闘アニメ・移動アニメ・スペシャルパワーのアニメを切り、自動のカーソル移動を切り、メッセージスピードを「手動送り」（`MessageWait=10000000`）にする |
| `screens/Data/`・`Bitmap/`・`Sound/`・`Midi/` | データと素材。素材は `torres/` と同じもの（`make_assets.py` が作ったもの）を写した |
| `screens/screens.play` | SRC# で同じ操作をする PlayScript（`SRCRenderTests` の `CompatScreensTests` が流す）。`snapshot` の名前が場面の ID。Windows には渡さない（`Compat/*.play` ではないので `tools/compat_play.py` も流さない） |
| `screens-scenes.txt` | 場面の一覧（ID・種類・場面・実機での操作）。`capture_screens.ps1`・`screens_report.py`・`CompatScreensTests` が読む |
| `capture_screens.ps1` | Windows で場面ごとに Src.exe（または SRC#Form）のウィンドウを撮る PowerShell のひな形。`probe_pack.py export` が場面を埋めて書き出す |
| `screens-expected/` | 実機（Src.exe）のスクリーンショット（`<ID>.png`）と、聞いた音のメモ（`sounds.txt`）を置く。`probe_pack.py import` で取り込む |
| `screens-form/` | SRC#Form（Windows）のスクリーンショット（`<ID>.png`）と音のメモ（`sounds.txt`）を置く |
| `screens_report.py` | 実機・SRC#（SRCRender）・SRC#Form の画像を場面ごとに並べた一覧（HTML）と、音の記録の表を作る |

乱数に左右されないようにしている。戦闘は必ず当たる武器（必中ハンマー。命中 +999）で、反撃しない標的を `Attack` で撃つ。回復の量は乱数を使わない（場面 07）。ただし、クリティカルは元の SRC でも SRC# でも最低 1% ある（`Unit.cls` の `CriticalProbability` は 1 より小さければ 1 にする）ので、場面 06 は 1% の確率で「クリティカル」になる。その時は撮り直す。

## 場面の一覧

ID と実機での操作は `screens-scenes.txt`。種類が「画面」のものはスクリーンショットを取る。「音」のものは耳で確かめる。

| ID | 場面 | 出し方 |
|---|---|---|
| `01-map` | マップ（ユニット・地形。草原・林・岩山・道・浅瀬、味方 2・敵 2） | イベント（`Center 8 8`） |
| `02-status-unit`・`03-status-enemy` | ステータスウィンドウ（味方・敵のユニットとパイロット。能力・特殊能力・アイテム・武器の欄） | `ShowUnitStatus` |
| `04-message` | メッセージウィンドウ（非戦闘員の顔の絵と名前、折り返し） | `Talk 管理官` |
| `05-battle-attack`・`06-battle-damage` | 戦闘のメッセージウィンドウ（両方のユニットの HP・EN の欄、パイロットの台詞、ダメージ） | `Attack ミナ 必中ハンマー 標的 無抵抗` |
| `07-battle-ability` | 戦闘のメッセージウィンドウ（アビリティで回復） | `UseAbility ミナ 応急処置 トウマ` |
| `08-paint-string` | PaintString（`Font` の既定・Ｐゴシック・明朝・ゴシック、大きさ、Bold・Italic・Regular、色、中央寄せ、座標の無い 2 行目） | 描画のコマンド |
| `09-paint-shapes` | Line（線・`B`・`BF`）・Circle・Oval・Arc・Polygon・PSet と、Color・DrawWidth・FillStyle（塗りつぶし・横線・網かけ・透明）・FillColor | 描画のコマンド |
| `10-paint-picture` | Cls と PaintPicture（顔とユニットの絵、拡大、左右反転・上下反転・白黒・セピア・透過・シルエット・ネガポジ反転・夕焼け・水中・明・暗・右回転） | 描画のコマンド |
| `11-filter-night`〜`16-filter-color` | Night・Sunset・Sepia・Monotone・Water・ColorFilter（#FF0000・50%） | フィルタのコマンド。場面ごとに `Noon` で戻す |
| `20-map-menu` | マップコマンドのメニュー | ユニットのいないマスを右クリック |
| `21-unit-menu` | ユニットコマンドのメニュー | 見本機アオをクリック |
| `22-move-range` | 移動範囲（範囲の外を暗くするマスク） | 「移動」 |
| `23-weapon-list` | 武器の一覧（使えない武器の色・弾数・EN・属性の欄） | 「攻撃」 |
| `24-attack-range` | 攻撃の範囲（マスク） | 一覧で「見本ライフル」 |
| `25-ability-list`・`26-feature-list`・`27-sp-list` | アビリティ・特殊能力・スペシャルパワーの一覧 | ユニットコマンド |
| `28-enemy-menu`・`29-enemy-weapons` | 敵のユニットのメニューと武装一覧 | 見本敵機をクリック |
| `30-intermission`〜`32-intermission-upgrade` | インターミッションのコマンド・ユニットステータス・機体改造のユニット選択 | ターン終了の後 |

`31-intermission-units` は、SRC 本体の `Lib\ユニットステータス表示.eve` を動かす（元の SRC は本体の、SRC# のテストは `Content/basic-data/Lib` の）ので、本体の Lib の中身によって画面が変わる。

## 音の一覧

どの場面で何が鳴るか。「元の SRC のコードから読んだこと」は推定（実機で聞いて確かめる）。「SRC# の記録」は `CompatScreensTests` が書き出す `sounds.txt`（前の場面からその場面までに鳴らしたもの）と同じ。実機で聞いたことは `screens-expected/sounds.txt` に「ID<TAB>聞いたこと」で書く（`capture_screens.ps1` が音の場面で書かせる）。

| ID（この場面までに） | 起きること | 元の SRC のコードから読んだこと（推定） | SRC# の記録 |
|---|---|---|---|
| `01-map` | タイトル・プロローグ・スタート | タイトルで `Opening`、プロローグの前に `Briefing` の BGM（本体の `Src.ini` の `[BGM]`。ファイルが本体の `Midi` にあれば。`SRC.bas`）。プロローグの `StartBGM cp_briefing.mid`（繰り返す）。スタートの前に `StopBGM`（`SRC.bas` の `StartScenario`）。`Create` のたびに本体の `Sound\UnitOn.wav`（あれば。`Unit.cls`） | `Opening.mid` → `Briefing.mid` → `cp_briefing.mid`（どれも繰り返す。前の 2 つは `Content/basic-data` のもの）→ 停止 → `UnitOn.wav` × 4 |
| `06-battle-damage` | 攻撃 | `effect.txt` の `必中ハンマー` で `cp_slash.wav`、命中で `cp_bite.wav`（戦闘アニメがオフ） | `cp_slash.wav` → `cp_bite.wav` |
| `snd-1` | `StartBGM cp_map.mid` | 切り替わって繰り返す | `cp_map.mid` を開始（繰り返す） |
| `snd-2` | もう一度 `StartBGM cp_map.mid` | 同じ BGM を演奏中なら、やり直さずに続く（`Sound.bas` の `StartBGM`） | なし（続く） |
| `snd-3` | `PlaySound cp_levelup.wav` | 1 回鳴る。BGM は止まらない | `cp_levelup.wav` |
| `snd-4` | `PlayMIDI cp_boss.mid` | 切り替わり、終わったら繰り返さない（`StartBGM fname, False`） | 停止 → `cp_boss.mid` を開始（1 回） |
| `snd-5` | `StartBGM cp_map.mid`・`Wait 30`・`StopBGM` | 3 秒鳴ってから止まる（前の `snd-4` の曲は繰り返さずに終わるので、止まったと分かるように鳴らし直す） | 停止 → `cp_map.mid` を開始（繰り返す）→ 停止 |
| `snd-6` | 味方のフェイズの初め | `Map1` の BGM（`RenameBGM` で `cp_map.mid`）。繰り返す | `cp_map.mid` を開始（繰り返す） |
| `23-weapon-list` | 攻撃のコマンドで武器の一覧を出す | パイロットの BGM（`cp_boss.mid`）に切り替わる（`Command.bas` の `StartAttackCommand`・`GUI.bas` の `WeaponListBox`） | 停止 → `cp_boss.mid` を開始（繰り返す） |
| `24`〜`29` | 攻撃をやめて、ほかのコマンド | （読んでいない。`cp_boss.mid` のままか、`cp_map.mid` に戻るかを聞く） | なし（`cp_boss.mid` のまま） |
| `snd-7` | 敵のフェイズの初め | `Map2` の BGM（`cp_enemy.mid`） | 停止 → `cp_enemy.mid` を開始（繰り返す） |
| `30-intermission` | インターミッション | `Intermission` の BGM（`cp_intermission.mid`。`Intermission.bas`） | 停止 → `cp_intermission.mid` を開始（繰り返す） |
| `31`・`32` | ユニットステータスから戻る | 本体の `Lib\ユニットステータス表示.eve` による | 停止 → 戻ると `cp_intermission.mid` を開始 |
| `snd-8` | 次のステージ（`screens_end.eve`） | 「次のステージへ」で `StopBGM`。プロローグの前に `Briefing`（`RenameBGM` の `cp_briefing.mid`。最初のプロローグと同じ曲） | 停止 → `cp_briefing.mid` を開始（繰り返す） |

## SRC# で流す

```sh
# CompatScreensTests（CI と同じ）。画像と sounds.txt を bin/Debug/net10.0/TestResults/e2e/compat-screens/ に書き出す
dotnet test SRC.Sharp/SRCRenderTests/SRCRenderTests.csproj --filter "FullyQualifiedName~CompatScreensTests"
# 実機・SRC#・SRC#Form を並べた一覧（out/screens-report/index.html。out/ は git に入らない）
python3 SRC.Sharp/Compat/screens_report.py
# 手で流す（画像は --out のフォルダに書く）
srcsharp play SRC.Sharp/Compat/screens/screens.eve --script SRC.Sharp/Compat/screens/screens.play --app-path SRC.Sharp/Content/basic-data --out <フォルダ>
```

CI の `test-srccore-linux` は、`CompatScreensTests` の後に `screens_report.py` で並べた一覧を作り、`compat-screens` の成果物に入れる（実機の画像がまだ無ければ、SRC# の列だけが埋まる）。画像は `screenshots` の成果物の `e2e/compat-screens/` にも入る。

場面を足すときは、`screens.eve`（場面を出して止める）・`screens.play`（同じ操作と `snapshot <ID>`）・`screens-scenes.txt`（ID と実機での操作）を揃えて変える。`CompatScreensTests` は、`screens.play` の `snapshot` の並びと `screens-scenes.txt` の ID の並びが同じことを確かめる。

## 見比べた結果

実機（`screens-expected/`）と SRC#（SRCRender）の画像を並べた一覧を見て、配置・色・文字・大きさの違いを表にした（2026-11-12）。ウィンドウの枠・タイトルバー・スクロールバーの見た目は、OS の描き方なので比べない。SRC#Form（`screens-form/`）はまだ撮っていない。

### 同じだったもの

- マップ（地形・ユニットの位置・表示の位置）、移動範囲・攻撃の範囲のマスク（場面 01・22・24）
- 描画のコマンドの位置と形（Line・Circle・Oval・Arc・Polygon・PSet・PaintPicture とそのオプション。場面 09・10）
- フィルタの色（Night・Sunset・Sepia・Monotone・Water・ColorFilter。場面 11〜16）
- ステータスウィンドウ・メッセージウィンドウ・一覧の項目と値（文字は同じ。違うのは下の表のフォント）
- 戦闘のメッセージウィンドウの、EN が 0 の標的の EN の欄は、実機でも「5/5」（場面 05。#1553 の「実機で確かめる点」）
- 音（下の「[実機で聞いたこと](#実機で聞いたこと)」）

### 違ったもの

| ID | 違い | 扱い |
|---|---|---|
| `23`〜`30`・`32` | 一覧を開いたとき、SRC# は先頭の項目を選んだ色にする。実機はどの項目も選ばない（`GUI.bas` の `ListBox` は `ListIndex = -1` にし、複数列のときだけ先頭を選ぶ） | SRCRender（SRC#Avalonia・SRC#Web）は、キーボードで操作するための違いとして [SRCとの互換性](../../SRC.Sharp.Help/src/SRCとの互換性.md) の「操作」に載せてあるので、そのまま。SRC#Form は元の SRC と同じく選ばないように直した |
| `20`・`21`・`28` | ポップアップメニューの位置。実機はマウスの X を横の中心にし（`PopupMenu` のフラグ 6 は `vbPopupMenuCenterAlign`）、ユニットコマンドはマウスの少し上（`MouseY - 6`）から開く。SRC# はマウスの位置を左上にしていた | 直した（SRCRender・SRC#Form）。キャンセルで開き直すときの `MouseY + 5` は区別しない |
| `20` | 実機のメニューはウィンドウの下にはみ出して開く。SRCRender は画面の中に収める | SRCRender は画面の外に描けないので、そのまま。SRCとの互換性の「操作」に載せた |
| `20` | マップのステータスで、SRC# は地形の画像を描き、地形の名前を右にずらす。実機（2.2.33）は画像を描かない | 比べたソース（`SRC_20121125` の `Status.bas` の `DisplayGlobalStatus`。「ADD START 240a」の追加）は画像を描くので、SRC# はソースのとおり。実機の版とソースの版の違いとして、SRCとの互換性の「操作」に載せた |
| `02`〜`08`・`20`〜`32` | フォント。実機はステータス・メッセージ・一覧を ＭＳ 明朝などの日本語のフォントで描く（プロポーショナルのフォントでは全角の英字・記号が詰まって見え、メッセージの「残りＨＰは」が「残りHPは」のように見える。文字そのものは元の SRC も全角の「ＨＰ」（`Unit.cls`））。SRCRender は同梱のフォントで描くので、字の幅・大きさが違う。一覧の桁を空白で揃えている所（武器・アビリティ・機体改造の一覧、ステータスの攻撃力・射程）は、SRC# では桁が揃わない | SRCRender のフォントの違いとして、SRCとの互換性の「操作」に載せた。近づけるなら別の Issue で（等幅のフォントを使うか） |
| `04`〜`07` | メッセージウィンドウのタイトルバー・枠の描き方 | OS の描き方なので比べない |

### 実機で聞いたこと

`screens-expected/sounds.txt` の要約。SRC# の記録（上の「音の一覧」）と食い違うものは無かった。

| ID | 実機で聞いたこと |
|---|---|
| `snd-1` | `cp_map.mid` が鳴り、繰り返す |
| `snd-2` | 頭に戻らずに続いた（はっきりしない） |
| `snd-3` | `cp_levelup.wav` が 1 回鳴り、BGM は続いた |
| `snd-4` | `cp_boss.mid` に切り替わり、終わると繰り返さない |
| `snd-5` | 確かめられない（`snd-4` の曲が終わって無音だった） |
| `snd-6` | 味方のフェイズの初めに `cp_map.mid` |
| `23`〜`29` | 武器の一覧を出すと別の曲（パイロットの BGM）に変わり、攻撃をやめても `cp_map.mid` に戻らない（#1553 の「実機で確かめる点」） |
| `snd-7` | 敵のフェイズの初めに `cp_enemy.mid` |
| `snd-8` | 次のステージへ進むと、別の曲が鳴る（曲の名前は聞き分けていない） |

`snd-2`・`snd-5`・`snd-8` は、聞き分けられるようにシナリオを直した（`snd-1` で 10 秒ほど聞いてから進める・`snd-5` の前に繰り返す曲を鳴らし直す・`snd-8` のメッセージに聞く曲を出す）。次に Windows で流すときに聞き直す（#1553）。
