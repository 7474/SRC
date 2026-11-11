# 実機との比較のプローブ（#1550）

元の SRC（VB5 の Src.exe。Ver.2.2.33）と SRC# で同じシナリオを流し、シナリオが書き出した値を比べるためのもの（#1550。親は #1548）。コードの読み比べだけを根拠にしている判断を、実機の出力で裏付ける。

- シナリオは、確かめたい動作を並べ、結果を `Open`・`Print`・`Close` で 1 つのファイルに「項目名<TAB>値」の行で書き出す。乱数に左右されない値だけを書く
- SRC# では `SRCRenderTests` の `CompatProbeTests` が同じシナリオを流す（CI で毎回）。実機の出力（期待値）が `probe-expected/` にあれば比べ、無ければ SRC# の出力を書き出すだけで成功する
- Src.exe を動かすのは手作業なので、1 回の実行（下の「[Windows で流す手順](#windows-で流す手順)」）で必要な値をすべて取れるようにしている。画面と音の見比べ（#1553。[SCREENS.md](SCREENS.md)）のスクリーンショットも、同じ作業の続きで撮る

**未確認**：このシナリオはまだ元の SRC で流していない。元の SRC（`SRC/SRC_20121125` の `CmdData.cls`・`Expression.bas`）にあるコマンドと関数だけを使い、書式をそのソースに合わせて書いたが、元の SRC で動くかは確かめていない。

## 置き場所と構成

| 置き場所 | 中身 |
|---|---|
| `probe/probe.eve` | 主なプローブ。VB5 のランタイムの動作・式と関数・戦闘の予測値・イベントコマンドを確かめ、`out_probe.txt` に書き出す。マップ（`probe.map`）を使い、スタートのイベントのあと、1 ターン目の味方フェイズのターンのイベントで `Quit` して終わる（プレイヤーの操作は要らない） |
| `probe/Data/` | プローブのデータ（パイロット・ユニット・アイテム・メッセージ・非戦闘員・地形・スペシャルパワー）。値は計算を確かめやすい数にしている。`Data/System/sp.txt` で、SRC 本体のスペシャルパワーの版に左右されないようにしている |
| `probe/Bitmap/` | `torres/` と同じ画像（`make_assets.py` が作ったもの）のうち、プローブが使うものを写した |
| `probe-bounds/incr.eve`・`unset.eve`・`ride.eve` | 範囲外の引数の参照（#1511）。引数の無い `Incr`・`UnSet`・`Ride`。Src.exe ごと落ちるかもしれないので、コマンドごとにイベントファイルを分けた。`out_incr.txt` などに書き出す |
| `probe-errors/<ケース>/probe.eve` | データの読み込みのエラー。ケースごとに誤ったデータ（またはイベントファイル）を置き、元の SRC が出すメッセージと、読み込みを打ち切るか続けるかを見る。続ければ `out_probe.txt` に、どのデータが読めたかと値を書き出す |
| `probe-expected/` | 実機の出力（期待値）を置く。`probe_pack.py import` で取り込む。`known-differences.txt` に理由のある違いを書く |
| `probe_pack.py` | 元の SRC に渡す形（Shift_JIS・CRLF。`export_sjis.py` と同じ変換）に書き出す（`export`）。Windows から持ち帰った結果を `probe-expected/` に取り込む（`import`）。画面と音の見比べ（`screens/`。#1553。[SCREENS.md](SCREENS.md)）も一緒に書き出し、スクリーンショットと音のメモを `screens-expected/`・`screens-form/` に取り込む |

`probe/` は `Data` を持つので、CI の `test-compat-scenario`（`tools/compat_play.py`）が `srcsharp lint` と SRCDataLinter に掛ける。`probe-bounds/`・`probe-errors/` は誤りをわざと入れているので、`Data` を直下に置かず、lint に掛からないようにしている。

## 出力のファイル

```
項目名<TAB>値
```

- 同じ項目名が 2 回出たら、後の行を使う。エラーになるかもしれない項目は、先に値を「`<エラー>`」として書き、次の行で値を書き直す。元の SRC はイベントのコマンドがエラーになると、そのラベルの残りを飛ばして次の常時イベントのラベル（`*スタート:`）へ進む（`Event.bas` の `HandleEvent`。`CmdData.cls` の `Exec` は -1 を返す）ので、値が「`<エラー>`」のまま残った項目は、エラーになったもの
- エラーになるかもしれない項目は、`*スタート:` のラベルを 1 つずつ分けている（1 つがエラーになっても、ほかの項目は進む）
- 「`参考.`」で始まる項目は、実行する環境（画面の大きさ・フォント・フォルダ・時刻・Src.ini の設定）で変わるので、比べない
- 書き出しは項目ごとにファイルを開き直す（シナリオが途中で落ちても、そこまでの行が残る）。最後の行は「`終了<TAB>1`」。これが無ければ途中で止まった（落ちた）
- 文字コードは Shift_JIS・CRLF（元の SRC の `Print #`。SRC# の `Open` も Shift_JIS で書く）

エラーのダイアログの文言は、`probe-bounds/`・`probe-errors/` では書き写す（`errors_<イベント>.txt`。下の手順）。`probe/probe.eve` では書き写さなくてよい（エラーになったかは「`<エラー>`」で分かる）。

## Windows で流す手順

1 回で済むように、#1550 のプローブと、#1553 の画面と音の見比べ（[SCREENS.md](SCREENS.md)）の手順を 1 つにまとめた。日本語版の Windows で、元の SRC（Ver.2.2.33 の Src.exe）を使う。見比べには、スピーカー（BGM の MIDI と効果音を聞く）と、30 分ほどの時間が要る。

1. **書き出す**（Python 3 があればどの OS でもよい。リポジトリのルートで）

   ```sh
   python3 SRC.Sharp/Compat/probe_pack.py export --zip
   ```

   `SRC.Sharp/Compat/out/probe-pack/`（と `SRC.Sharp/Compat/out/probe-pack.zip`。`out/` は git に入らない）ができる。中身は `probe/`・`probe-bounds/`・`probe-errors/`・`screens/` と、すべてのケースを順に開いてから見比べを始める `run_probe.bat`、見比べだけを始める `run_screens.bat`、場面ごとに画面を撮る `capture_screens.ps1`。テキストは Shift_JIS・CRLF になっている
2. **Windows に写す**。`probe-pack.zip` を写して展開する。場所はどこでもよい（例: `C:\probe-pack`。SRC 本体のフォルダの下でなくてよい）。シナリオが `out_*.txt` などを書くので、書き込めるフォルダにする
3. **SRC 本体を確かめる**。Src.exe のフォルダに `Data\System\terrain.txt` があること（元の SRC は本体の地形データが無いと起動しない。中身はどの版の汎用データのものでもよい。プローブと見比べは 1000 番台の自前の地形だけを使う）。汎用データ・汎用戦闘アニメ・効果音集は要らない（見比べの場面 31 は本体の `Lib\ユニットステータス表示.eve` を使う。本体にあるかを控える）。Src.exe がすでに起動していたら終わらせる（2 つは起動できない）。画面の拡大率（DPI）は 100% にしておく（スクリーンショットの大きさが変わらないように）
4. **プローブを流す**。`run_probe.bat` をメモ帳で開き、`set SRC_EXE=C:\SRC\Src.exe` を Src.exe の場所に直して保存し、ダブルクリックで実行する。12 のケースを順に Src.exe で開く（`start /wait "" Src.exe "<フォルダ>\<イベント>.eve"`。元の SRC は起動の引数に渡したイベントファイルを開き、そのフォルダをシナリオのフォルダにする。`SRC.bas` の `Main`）。1 つ終わるたびに「続行するには何かキーを押してください」で止まる
   - `probe\probe.eve`：エラーのダイアログが出たら OK を押す（書き写さなくてよい）。戦闘のメッセージはクリックで進める。最後に Src.exe が自分で終わる
   - `probe-bounds\*.eve`・`probe-errors\*\probe.eve`：エラーのダイアログが出たら、ダイアログの文言を選んで（ダイアログの文字の欄をクリックして Ctrl+A、Ctrl+C）、同じフォルダの `errors_<イベント>.txt`（例: `probe-bounds\errors_incr.txt`・`probe-errors\unit-hp-text\errors_probe.txt`）に貼り付けてから OK を押す。ダイアログが 2 つ以上出たら、間に `----` だけの行を入れて続けて貼る。ダイアログが出なければ、ファイルを作らなくてよい
   - Src.exe が落ちた（「問題が発生したため、Src.exe を終了します」などが出た）ら、その文言も `errors_<イベント>.txt` に貼り、最後の行に `（Src.exe が落ちた）` と書く
   - 読み込みのエラーで Src.exe が自分で終わったときは、`out_*.txt` ができない。それも結果なので、そのまま次へ進む
   - 手で 1 つずつ流すときは、コマンドプロンプトで `"C:\SRC\Src.exe" "C:\probe-pack\probe\probe.eve"` のように開く（Src.exe のファイルを開くダイアログで選んでもよい）
5. **画面と音の見比べ**（#1553）。プローブが終わると、`run_probe.bat` が続けて `capture_screens.ps1` を動かす（見比べだけをやり直すときは `run_screens.bat`。`SRC_EXE` を同じく直す）。Src.exe が `screens\screens.eve` を開き、PowerShell のウィンドウに場面の ID・名前・操作が 1 つずつ出る
   - 画面と PowerShell のウィンドウが重ならないように並べる。場面ごとに、示した操作（Src.exe のクリック・右クリック・メニューの選択）でその場面を出し、**PowerShell のウィンドウをクリックして Enter** を押す。スクリプトが Src.exe のウィンドウを前に出し、ウィンドウの範囲を `screens-shots\<ID>.png` に保存する。撮れた画像を見て、よければ Enter、撮り直すなら `r`
   - 前半（場面 01〜16）は、場面の名前のメッセージをクリックで閉じると場面が出て止まる。撮ってから Src.exe をクリックすると次の場面へ進む（撮る前に Src.exe をクリックしないこと）。場面 04〜07 はメッセージそのものを撮る。場面 06 で「クリティカル」と出たら（1%）、撮り終えてから最初からやり直す（`-From 06-battle-damage` は、Src.exe をもう一度開いてその場面まで進めてから）
   - 音の場面（`snd-1` など）は、聞いたこと（鳴ったか・切り替わったか・繰り返すか）を書いて Enter。`screens-sounds.txt` に残る。何が鳴るはずかは [SCREENS.md の音の一覧](SCREENS.md#音の一覧)。場面 23（武器の一覧）で BGM が変わるか、その後のコマンドで戻るかも、次の音の場面のメモに書く
   - 後半（場面 20〜29）は、メニューや一覧を出して撮る。戻るときは右クリック（メニューが残ったら、メニューの外をクリックして閉じる）。最後にマップコマンドの「ターン終了」で、インターミッション（場面 30〜32）へ進む
   - 途中で止めたら、`powershell -ExecutionPolicy Bypass -File capture_screens.ps1 -From <場面の ID>` で続きから撮れる（`-SrcExe` を付けなければ Src.exe を起動しない）
6. **SRC#Form の画面**（Windows で SRC#Form を動かせるとき）。SRC#Form の `Src.ini` 相当の設定はシナリオの `screens\Src.ini` から読むので、同じ設定になる。

   ```bat
   powershell -NoProfile -ExecutionPolicy Bypass -File capture_screens.ps1 -SrcExe <SRCSharpForm.exe の場所> -ProcessName SRCSharpForm -OutDir screens-form-shots
   ```

   で、同じ手順で `screens-form-shots\<ID>.png` と `screens-form-shots\sounds.txt` を作る（SRC#Form は Shift_JIS のシナリオも読める）
7. **回収する**。`C:\probe-pack` のフォルダをまとめて zip にして持ち帰る（要るのは各フォルダの `out_*.txt`・`errors_*.txt` と、`screens-shots\`・`screens-sounds.txt`・`screens-form-shots\`）。あわせて、Windows の版・画面の拡大率・Src.exe の版（タイトル画面かバージョン情報）・本体の `Lib` と `Midi`・`Sound` の有無・流した日を控える
8. **リポジトリに置く**。持ち帰った zip を展開し、リポジトリのルートで

   ```sh
   python3 SRC.Sharp/Compat/probe_pack.py import <展開したフォルダ>/probe-pack
   ```

   を実行すると、`SRC.Sharp/Compat/probe-expected/<フォルダ>/<イベント>.txt`（出力）と `<イベント>.errors.txt`（エラーのダイアログ）が UTF-8・LF で、`screens-expected/<ID>.png`・`screens-expected/sounds.txt`（Src.exe）と `screens-form/<ID>.png`・`screens-form/sounds.txt`（SRC#Form）ができる。控えた環境は `probe-expected/environment.txt` と `screens-expected/environment.txt` に書く。これらをコミットする（PR に載せる）
9. **比べる**。
   - プローブ：`dotnet test SRC.Sharp/SRCRenderTests/SRCRenderTests.csproj --filter "FullyQualifiedName~CompatProbeTests"`。違う項目が出たら、直すか、理由を `probe-expected/known-differences.txt` と [SRCとの互換性](../../SRC.Sharp.Help/src/SRCとの互換性.md) に書く。VB5 のランタイムの 4 つの動作は、対応表（`docs/history/porting/vb-map/`）の説明の「推定」を「VB5 の実機で確かめた」に書き換える
   - 画面と音：`dotnet test SRC.Sharp/SRCRenderTests/SRCRenderTests.csproj --filter "FullyQualifiedName~CompatScreensTests"` の後に `python3 SRC.Sharp/Compat/screens_report.py` で並べた一覧（`SRC.Sharp/Compat/out/screens-report/index.html`）を作る（CI の `compat-screens` の成果物にも出る）。配置・色・文字・大きさの違いと、音の違いを [SCREENS.md の「見比べた結果」](SCREENS.md#見比べた結果) に書き、直すか互換性の一覧に載せる

出力に足りない項目があれば、シナリオに足して、もう一度だけ流す（#1550 の「必要なもの」）。


## SRC# で流す

```sh
# CompatProbeTests（CI と同じ）。SRC# の出力は bin/Debug/net10.0/TestResults/e2e/compat-probe/ に書き出す
dotnet test SRC.Sharp/SRCRenderTests/SRCRenderTests.csproj --filter "FullyQualifiedName~CompatProbeTests"

# 1 つだけ手で流す（out_probe.txt はシナリオのフォルダに書くので、写したフォルダで流すとよい）
srcsharp play <写したフォルダ>/probe/probe.eve --script <「auto 120」の 1 行だけのファイル> --app-path SRC.Sharp/Content/basic-data --skip-wait
```

`CompatProbeTests` が書き出すもの（CI ではスクリーンショットと一緒に `screenshots` の成果物に入る）:

| ファイル | 中身 |
|---|---|
| `<イベント>.txt` | SRC# の出力（UTF-8） |
| `<イベント>.errors.txt` | SRC# が出したエラーのダイアログ（期待値の `.errors.txt` と同じ形。1 行目のファイル名はフォルダを除く） |
| `<イベント>.dataerrors.txt` | SRC# がダイアログに出さずに集めたデータの誤り（`SRC.DataErrors`）と、読み込みを打ち切った例外 |
| `<イベント>.diff.txt` | 期待値との違い（期待値があるとき） |

期待値が無くても、`probe/probe.eve` と `probe-bounds/*.eve` は SRC# で最後（「終了」）まで進むことを確かめる。

## 項目と #1550 の「書き出す値」の対応

### 実機で確かめていない VB5 のランタイムの動作

| #1550 の表の動作 | 項目名（ケース） | 見ること |
|---|---|---|
| 範囲外の引数の参照（`BoundsCheck`。#1511） | `VB5.範囲外.Incr`・`.UnSet`・`.Ride`（`probe-bounds/*.eve`） | 引数の無いコマンド。Parse は `strArgs` を作らない。エラーのダイアログの文言（「Incrコマンドの引数の数が違います」か「イベントデータが不正です」か）と、Src.exe が落ちるか |
| 〃 | `VB5.範囲外.Args(0)`・`Args(-1)`・`Args(2)`（`probe.eve`） | `Args` 関数は引数の数より大きい番号だけを確かめる。0 以下は `ArgStack` の上（前の呼び出しが残した値。`a2`・`a1`）を読むと読んでいる。SRC# は空文字列 |
| 整数のあふれ（`OverflowCheck`） | `VB5.あふれ.RankUp`・`IncreaseMorale` | Integer の変数に 65541 を入れる。桁が落ちれば 5、エラーなら「`<エラー>`」 |
| 〃 | `VB5.あふれ.\のLong`・`ModのLong`・`\のLongの上限`・`\のLongの上限+1` | `\`・`Mod` は Long に変換してから計算する |
| 〃 | `VB5.あふれ.文字列の数値`・`VB5.数値.24.*`（`1e400`） | `StrToDbl`（`IsNumeric` の後に `CDbl`。エラー処理なし） |
| 0 での割り算（`FlPointCheck`） | `VB5.割り算.*`（`/0`・`0/0`・`\0`・`Mod0`・`Mod0.0`） | `/`・`\` は `Expression.bas` が 0 を確かめて 0 にする。`Mod` は確かめない（エラー 11 と読んでいる） |
| 〃 | `VB5.浮動小数点.*`（`10 ^ 400`・`1e308 * 10`・`(-8) ^ 0.5`・`Sqr(-1)`） | 無限大・非数になるか、エラーになるか。値の書き方（`FormatNum`） |
| （同じ表の続き。設定に関わらずエラーと読んでいるもの） | `VB5.実行時エラー.*`（`Mid(0)`・`Left(-1)`・`Right(-1)`・`Chr(-1)`・`Chr(70000)`・`Asc(空)`・`String(-1)`・`InStr(0)`・`LSet(-1)`） | VB の文字列関数の引数の誤り（エラー 5） |
| IsNumeric と数値の変換（#1534・#1545） | `VB5.数値.<番号>.文字列`・`.IsNumeric`・`.加算`・`.比較`・`.Int` | 下の表の文字列を変数に入れ、`IsNumeric` 関数（`GeneralLib.IsNumber`。`(` で始まるものは数値としない）・足し算（`StrToDbl`）・数値との比較・`Int` を見る |
| 〃 | `VB5.数値.直書き.1e5`・`.1d5` | 式に直に書いた数値（`EvalTerm` は先頭が数字なら `IsNumeric` の後に `CDbl`） |
| 〃 | `probe-errors/data-numeric` | データの数値の欄に `&H1000`・`1e3`・`1e2` を書く（データの読み込みも `IsNumeric` と `CLng`） |
| `For Each` の途中で Collection から今の要素を消す | `VB5.ForEach.特殊状態.付けた直後`・`.Attack後` | 残りターン数 0 の特殊状態を 4 つ付けて `Attack` する（`ExecAttackCmd` は終わりに `UpdateCondition` を呼び、`For Each colCondition` の中で残りターン数 0 のものを `Remove` する）。全部消えれば `0000`、消した次の要素を飛ばせば `0101`。残りターン数を減らす `Unit.Rest` は味方の 1 ターン目には動かない（`Stage = "味方" And Turn = 1` で抜ける）ので、期限切れそのものは使わない |
| 〃 | `VB5.ForEach.母艦.Escape前`・`.Escape後` | 2 機を載せた母艦を `Escape`（`Unit.Escape` は `For Each colUnitOnBoard` の中で `Remove`）。2 機とも「待機」になるか |
| 〃 | `VB5.ForEach.RemoveUnit`・`RemoveUnit後.*` | `ForEach` コマンドの途中で、まだ回っていないユニットを `RemoveUnit`（ForEach は始めに ID の一覧を作る） |
| 文字列の `Select Case` と `Case 0 To 127`（「(対x属性)」） | `VB5.SelectCase.対格属性.完了`・`対Ｂ属性.完了`・`対-属性.完了`・`VB5.SelectCase.台詞` | 反撃の武器の属性が「格」「Ｂ」「-」の相手を `Attack` で攻撃する。`MessageData.SelectMessage` は属性のまとまり（String）を数値の範囲と比べる。型の不一致（エラー 13）で `Attack` が止まるか（「完了」が「`<エラー>`」）、「(対格属性)」などの台詞が選ばれるか（`台詞` の 3 文字が、対格属性・対Ｂ属性・対-属性の順に 1 か 0）。SRC# は先頭の文字のコードが 127 より大きいかで判定するので `110` |

`VB5.数値` の番号と文字列:

| 番号 | 文字列 | 番号 | 文字列 | 番号 | 文字列 |
|---|---|---|---|---|---|
| 01 | `1e5` | 10 | `\100` | 19 | `1e` |
| 02 | `1E+5` | 11 | `1-` | 20 | （空） |
| 03 | `1d5` | 12 | `+1` | 21 | `abc` |
| 04 | `&H10` | 13 | ` 1 `（前後に空白） | 22 | `0x10` |
| 05 | `&HFFFF` | 14 | `1,000` | 23 | `&H` |
| 06 | `&O17` | 15 | `1.5` | 24 | `1e400` |
| 07 | `&o17` | 16 | `.5` | 25 | `2147483648` |
| 08 | `(1)` | 17 | `1.` | 26 | `-0` |
| 09 | `￥100`（全角の円記号） | 18 | `１２３`（全角の数字） | 27 | `1 2` |

### 式と関数

| #1550 の項目 | 項目名 |
|---|---|
| CallFunction の各関数 | `関数.*`。ヘルプの[関数](../../SRC.Sharp.Help/src/関数.md)の一覧（元の SRC の `Expression.bas` の `CallFunction` の `Case`）を、すべて 1 回以上呼ぶ。`Random` は `Random(1)`（必ず 1）、`GetTime` は 0 以上か、時刻・画面・フォント・キーに左右される `WX`・`WY`・`Font`（フォント名・サイズ）・`TextHeight`・`TextWidth`・`WindowWidth`・`WindowHeight`・`KeyState` は `参考.` にした。`LoadFileDialog`・`SaveFileDialog` はダイアログを出すので呼ばない |
| Info の各項目 | `Info.ユニット.*`・`Info.パイロット.*`・`Info.パイロットデータ.*`・`Info.ユニットデータ.*`・`Info.非戦闘員.*`・`Info.アイテム.*`・`Info.スペシャルパワー.*`・`Info.マップ.*`・`Info.オプション.*`。ヘルプの [Info関数](../../SRC.Sharp.Help/src/Info関数.md) の情報種類 |
| Format | `Format.*`。数値の書式（`#`・`0`・`,`・`%`・`E+`・`;`）、丸め（`0.125`・`2.5`・`-3.5`・`-0.4`）、名前の付いた書式（`Fixed`・`Standard`・`Percent`・`Currency`）、文字列（`@`・`!`）、日付の文字列（`yyyy年m月d日`・`aaa`） |
| 変数の型（数値と文字列） | `型.*`。`Set a 01` と `Set a "01"`、数値と文字列の比較・連結・演算子の優先順位、`\`・`Mod` の丸め、`FormatNum` の書き方（`1/3`・`1e20`・`1e-5`）、`HP()` に 0 を入れる |
| 未定義の変数 | `未定義.*`。値・足し算・比較・`Len`・`IsVarDefined`・配列・`Count`、サブルーチンで作った変数と `Local`、`UnSet` |
| システム変数 | `システム変数.*`。スタートのときと 1 ターン目（`システム変数.ターン.*`）。`AppPath`・`ExtDataPath`・`MouseX` は `参考.` |
| 日付の関数 | `日付.*`。`Year`〜`Second`・`Weekday`・`DiffTime` に固定の文字列を渡す。区切り（`/`・`-`・`年月日`）・和暦（`H24/11/25`）・時刻だけ・無い日・数字だけ |

### 戦闘の予測値と能力値

| #1550 の項目 | 項目名 |
|---|---|
| 命中率・ダメージ・クリティカル率 | `戦闘.予測.*`・`Info.ユニット.武器.<番号>.攻撃力`・`.命中率`・`.クリティカル率`（パイロットの能力・強化を足した値）。相手に対する命中率そのものを返す関数は元の SRC に無いので、ダメージは、必ず当たりクリティカルの出ない武器（命中 +999・クリティカル -999）で無抵抗の標的を `Attack` し、ＨＰの差で見る（`戦闘.草原.ダメージ`・`戦闘.林.ダメージ`・`戦闘.浅瀬.ダメージ`・`戦闘.強化後.ダメージ`）。攻撃の後の気力・経験値も見る |
| 能力値・地形適応・特殊能力・強化 | `能力.*`。レベル 10 のパイロットの能力値（`Info.パイロット.格闘` など。超反応・念力の修正）、強化パーツの装備（最大ＨＰ・ＥＮ・装甲・運動性・移動力・射撃強化）、パイロット地形適応変更、改造（`RankUp`）、気力と格闘強化・必要気力 |

### イベントコマンド

| #1550 の項目 | 項目名 |
|---|---|
| Sort | `コマンド.Sort.*`（昇順・降順、数値・文字）、`コマンド.ForEach.配列.*`（文字インデックス・インデックスのみの後に ForEach で回る順） |
| ForEach | `コマンド.ForEach.配列.*`、`VB5.ForEach.RemoveUnit` |
| リストの関数 | `関数.List`・`関数.LLength.*`・`関数.LIndex.*`・`関数.LSearch.*`（閉じない括弧・閉じすぎた括弧）、`コマンド.Array.*`・`コマンド.CopyArray.*` |
| ファイルの読み書き | `コマンド.ファイル.*`（`Print` の式の扱い・バッククォート・`;`、`Write` と `Read`、`追加出力`、`LineRead`、`EOF`）、`コマンド.CopyFile`・`RenameFile`・`RemoveFile`・`CreateFolder`・`RemoveFolder`、`関数.Dir.*` |
| 文字列の比較 | `コマンド.比較.*`（式の `<`・`=`・`<>`、`StrComp`、`If`、`Switch` の `Case`） |

### データの読み込みのエラー

`probe-errors/` のケース。元の SRC の読み込み（`UnitDataList.cls`・`PilotDataList.cls` の `Load`）は、`err_msg` を作って `Error 0` にした誤りでは `DataErrorMessage` を出して `TerminateSRC`（打ち切り）、`DataErrorMessage` を直に呼んだ誤りではメッセージを出して続けると読んでいる。

| ケース | 誤り | 元の SRC について読んでいること |
|---|---|---|
| `unit-hp-text` | ユニットの最大ＨＰが `abc` | 「最大ＨＰの設定が間違っています。」を出し、1000 にして続ける |
| `unit-name-space` | ユニットの名称に半角スペース | 「名称に半角スペースは使用出来ません。」を出して打ち切る |
| `unit-missing-size` | ユニットの 3 行目にサイズから後が無い | 「サイズの設定が抜けています。」を出して打ち切る |
| `unit-weapon-short` | 武器の行の項目が足りない | （読んでいない） |
| `pilot-stat-text` | パイロットの格闘が `abc` | 「格闘攻撃力の設定が間違っています。」を出して続ける |
| `data-numeric` | 数値の欄に `&H1000`・`1e3`・`1e2` | `IsNumeric` と `CLng` で読み、エラーにしない |
| `eve-endif` | 対応する If の無い EndIf | 「EndIfに対応するIfがありません」を出し、読み終えてから打ち切る |
| `eve-unknown` | 定義されていないコマンド | 代入（`a = 1 + 2`）か、サブルーチンの呼び出しとして読む。ラベルが無ければ実行したときにエラー |

出力の項目: `到達`・`ユニット.前`・`ユニット.誤り`・`ユニット.後`（読めたか）・`ユニット.誤り.<情報>`・`パイロット.*`・`終了`。

## SRC# の出力と、元の SRC のコードから読んだ値の違い

実機で流す前に、SRC# の出力と元の SRC のコードを見比べて、違いそうだと分かったもの（推定。実機の出力で確かめる）。

| 項目 | SRC# | 元の SRC のコードから読んだ値 |
|---|---|---|
| `日付.DiffTime.*` | 秒の差（75・86400・-75） | `Second(d2 - d1)`（差の秒の桁だけ。15・0・15） |
| `Format.Fixed`・`Standard`・`Percent`・`Currency` | 書式の名前をそのまま返す | VB の `Format$` の名前の付いた書式（`1234.57`・`1,234.57`・`12.30%`・通貨記号の付いた `1,235`） |
| `Format.日付`・`Format.日付.曜日` | 元の文字列（`2012/11/25`） | `Format$` は日付の文字列を日付として書く（`2012年11月25日`・`日`） |
| `VB5.実行時エラー.Mid(0)`・`Left(-1)`・`Right(-1)`・`String(-1)`・`InStr(0)`・`Chr(70000)` | エラーにならない（空文字列・0・`?`） | VB の関数の引数の誤り（エラー 5）で「イベントデータが不正です」 |
| `VB5.数値.08`（`(1)`）・`04`（`&H10`）・`09`（`￥100`）・`11`（`1-`）の `.加算` | 0 | `StrToDbl` は `IsNumeric` が真なら `CDbl` で読む（-1・16・100・-1。#1545） |
| `probe-errors/data-numeric` | `&H1000` を誤り（最大ＨＰの設定が間違っています。）にする | `IsNumeric` が真なので `CLng` で 4096 と読む |
| `probe-errors/*` のエラーのダイアログ | データの誤りをダイアログに出さない（`SRC.DataErrors` に集める。打ち切る誤りは例外で終わる） | `DataErrorMessage` でダイアログを出す |
