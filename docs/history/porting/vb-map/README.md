# VB 版と C# の手続きの対応表

VB 版（`SRC/SRC_20121125`）の手続き（Function・Sub・Property）を、C# の対応するメソッドと 1 つずつ突き合わせるための表（7474/SRC#1498）。

- 表は `python3 tools/vb_port_map.py` で作る。作り直しても、手で書いた「C#」「状態」「メモ」は残る。行番号は VB 版の行
- CI（`port-map-check`）の `python3 tools/vb_port_map.py --check` で、VB のすべての手続きが表にあり、状態の書き方が正しいか、[TODO・コメントアウトの一覧](todo.md)に「未確認」が無いかを確かめる
- 対象は `SRC/SRC_20121125` の `*.bas`・`*.cls`・`*.frm` のすべての手続き。選んで外した手続きは無い（手続きの外にあるものは下の「表に載っていないもの」）
- 「C#」の候補は名前だけで選んでいる。`（見つからない）` は名前の違う C# に移したか、移していないもの。突き合わせのときに直す
- 「一致」の 50 件を抜き取って読み直した結果は [resample-1556.md](resample-1556.md)（7474/SRC#1556。食い違いは 2 件で、打ち切った）

## 突き合わせの手順

1. VB の手続きと C# のメソッドを並べ、分岐（`If`・`Select Case`・ループの抜け方・`Exit`・`On Error`）ごとに同じかを確かめる
2. 違いは、元の SRC の動作を期待値にしたテストにして直す（[判断の記録](../../../decisions/2026-10-15-vb-behavior-as-test-oracle.md)）。すぐに直せないものは Issue にして、テストに `[Ignore("移植の不具合: …")]` を付ける
3. その手続きに当たる C# の `TODO`・`XXX`・コメントアウトされた VB のコード（[一覧](todo.md)）を判断する（直す・SRC# では要らない・Issue にする）
4. 表の「状態」と「メモ」を書く。機械で見つけられる型の誤りなら、[SRCPortingAnalyzers](../../../../SRC.Sharp/SRCPortingAnalyzers/README.md) に規則を足す
5. あとの単位で扱う手がかりは、その手続きの行の「メモ」に書く（状態は「未確認」のまま）

## 表に載っていないもの（突き合わせのときに一緒に見る）

表は手続き（Function・Sub・Property）を単位にしているので、次のものは載っていない。手続きを突き合わせるときに、そのモジュールの分を一緒に見る。

`Const`・`Type`・`Enum`・モジュールの変数を突き合わせた結果は [declarations.md](declarations.md) にある（7474/SRC#1498 の最後の項目）。

| 種類 | 見ること |
|---|---|
| モジュールの変数の宣言（手続きの外。約 600） | 型（Integer・Long・Single・Double・String・Variant）。代入の丸め・あふれ・既定値が型で決まる。初期値は `Class_Initialize` の行で見る |
| `Const`（約 120）・`Type`・`Enum`（約 30） | 値と項目が C# の定数・型と同じか。既定値・上限などの数値が違えば動作が変わる（#1498 のチェックリストの「モジュールの宣言」の項目でまとめて見る） |
| `Declare`（Win32 API の宣言。約 120） | SRC# では使わない。それを呼ぶ手続きの行で、代わりの処理を見る |
| `.frm` の画面の定義（`Begin VB.Form` 〜 `Attribute VB_Name` の前）・`.frx` | ボタン・リストなどの配置・大きさ・既定の文言・タブの順。フォームの手続きを突き合わせるときに、SRCRender・SRCSharpForm の画面と比べる |
| 各ファイルの先頭の `Option` 文 | `Option Compare` を書いたファイルは無い。文字列の比較は既定の Binary（大文字・小文字・全角半角・ひらがなカタカナを区別する。ただし Collection のキーは区別しない） |

## 元の SRC の実行ファイルのコンパイルの設定（`SRC/SRC_20121125/SRC.VBP`）

「VB では実行時エラーになる（ならない）」と判断するときは、配布された Src.exe のコンパイルの設定と、下の実機で確かめた結果を考える。`SRC.VBP` には次の指定がある。

```
BoundsCheck=-1      ' 配列の範囲チェックをしない
OverflowCheck=-1    ' 整数のオーバーフローチェックをしない
FDIVCheck=-1        ' Pentium の FDIV の不具合のチェックをしない
NoAliasing=-1       ' 別名（エイリアス）が無いとみなす
FlPointCheck=-1     ' 浮動小数点のエラーのチェックをしない
UnroundedFP=0       ' 浮動小数点の途中の結果を丸める（丸めない指定はしていない）
```

- -1 は、VB5 のプロジェクトのプロパティ（コンパイル → 詳細な最適化）の項目に印が付いた状態（チェックをしない）と読んでいる。`UnroundedFP=0` なので、途中の結果は Double に丸められる（C# の double と同じ）
- 以前はこの読み方から「範囲外の参照・整数のあふれ・浮動小数点のあふれは実行時エラーにならない」と考えていた。元の SRC の実機（#1550。2026-11-12、Ver.2.2.33 の Src.exe、日本語版の Windows 11。[PROBE.md](../../../../SRC.Sharp/Compat/PROBE.md) の「実機で確かめた結果」）では、確かめた整数のあふれ・浮動小数点のあふれと非数は**どれも実行時エラーになった**（イベントのコマンドの中なので、`CmdData.cls` の `Exec` の ErrorHandler が「イベントデータが不正です」を出してイベントを止める。Src.exe は落ちない）
- これらの設定が外すのは、コンパイラーがその場に埋め込む確かめ（Integer・Long どうしの足し算などのあふれ、配列の添字の範囲、浮動小数点の演算の後の状態）だけで、型の変換などランタイム（MSVBVM50.DLL）の関数の中の確かめは残る、と考えると実機の結果と合う。ただし、どの命令でエラーになったかは確かめていない（下の表の「エラーの起きる所」は、元の SRC のコードから読んだ候補）

### 実機で確かめた結果

| 項目（プローブの項目名） | 実機 | 元の SRC のコードと、エラーの起きる所 |
|---|---|---|
| 引数の無い `Incr`・`Ride`・`UnSet`（`VB5.範囲外.*`。`probe-bounds/`） | 「イベントデータが不正です」。Src.exe は落ちない | `CmdData.cls` の `Parse`（81〜82 行）は `ArgNum > 1` のときだけ `ReDim strArgs(2 To ArgNum)` する。引数が無いと `strArgs` は一度も ReDim されない（配列が無い）。`ExecIncrCmd`（6839 行）・`ExecRideCmd`（10727 行）・`ExecUnsetCmd`（12726 行）は引数の数を確かめずに `GetArg(2)`（`strArgs(2)`）を読む。範囲外だからではなく、配列が無いのを読んだためのエラーと読める。範囲チェックを外しても配列が無いときにエラーになる仕組み（ランタイムが確かめるのか）は確かめていない |
| `Args(0)`・`Args(-1)`（`VB5.範囲外.Args(0)`・`Args(-1)`） | エラーにならず `[a2]`・`[a1]`（前の呼び出しが残した値） | `Expression.bas` の `Args`（1053〜1057 行）は番号の上だけを確かめて `ArgStack(ArgIndex - num + 1)` を読む。0・-1 は `ArgStack(0 To 200)`（`Event.bas` 71 行）の**範囲の中**の、今の引数より上（前の呼び出しが残した所）を読むので、範囲チェックを外したことの裏付けにはならない（SRC# も同じ所を読んで同じ値を返す） |
| `RankUp あふれＩＤ 65541`（`VB5.あふれ.RankUp`） | エラー | `ExecRankUpCmd`（9539 行）の `rk` は Integer で、`rk = GetArgAsLong(3)`（9555 行）が Long から Integer への変換。続く `.Rank = .Rank + rk`（9567 行）は Integer の足し算。変換で起きたと考えられるが、確かめていない |
| `IncreaseMorale あふれ機 65541`（`VB5.あふれ.IncreaseMorale`） | エラー | `ExecIncreaseMoraleCmd`（6857 行）の `num` は String で、`.IncreaseMorale num, True`（6874 行）が Integer の引数（`Unit.cls` 26912 行の `ByVal num As Integer`）に渡すときに文字列から Integer へ変換する。文字列の変換はランタイムの関数なので、そこで起きたと考えられる |
| `1e10 \ 1`・`1e10 Mod 7`・`2147483648 \ 1`（`VB5.あふれ.*`） | エラー（`2147483647 \ 1` は 2147483647） | `Expression.bas` の `\`（393 行）・`Mod`（419・422 行）は Double の `lnum`・`rnum` をそのまま VB の演算子に渡す。VB は割る前に両辺を Long に丸めるので、Long に収まらない値の変換で起きたと考えられる。境目が Long の上限なのはこの読み方と合う |
| `10 ^ 400`・`1e308 * 10`（`VB5.浮動小数点.*`） | エラー | `Expression.bas` の `^`（439 行）・`*`（349 行）の結果を `FormatNum`（`GeneralLib.bas` 1006 行。`Int(n)` と `Format$`）で文字列にする。演算そのもの（`^` はランタイムの関数）か、無限大を受け取った `Int`・`Format$` か、どこで起きたかは確かめていない |
| `(-8) ^ 0.5`・`Sqr(-1)`（`VB5.浮動小数点.*`） | エラー | `^`（439 行）・`Sqr`（3130 行）。非数になる演算そのものか、`FormatNum` か、どこで起きたかは確かめていない |
| `5 Mod 0`・`5 Mod 0.4`（`VB5.割り算.*`） | エラー | `Mod` は 0 を確かめない（0 での割り算。エラー 11）。`/`・`\` は `Expression.bas` が除数の 0 を確かめて 0 にする（`5 / 0`・`5 \ 0` は 0） |

SRC# は、これらを元の SRC と同じくエラーにしている（`VBMath.Checked`・`VBMath.CInt`・`CmdData.GetArgRaw`。#1564・#1580）。

### 「VB では実行時エラーにならない」と判断するときの指針

- コンパイルの設定だけを理由に「エラーにならない」と判断しない。型の変換（Long・Double・String から Integer・Long へ。代入・`ByVal` の引数への受け渡し・`\`・`Mod` を含む）のあふれ、浮動小数点の無限大・非数は、実機でエラーになった。同じ形のものは、エラーになると読む
- 配列が無い（ReDim していない）ときの読みも、実機でエラーになった。確保した配列の範囲外（`strArgs(2 To 3)` の `strArgs(4)` など）の読み書きと、Integer・Long どうしの足し算・掛け算のあふれ（変換を含まないもの）は、実機で確かめていない。これらを理由にするときは「実機では確かめていない」と書き、SRC# は値の定まる側（エラーにする・空文字列にするなど）に寄せる。確かめる必要があれば、[プローブ](../../../../SRC.Sharp/Compat/PROBE.md)に項目を足す
- 整数の割り算（`\`・`Mod`）の 0 での割り算（エラー 11）・`Nothing` の参照（エラー 91）・型の不一致（エラー 13）・`CDbl` などの変換の失敗は、この設定に関わらずエラーになる。Collection（`colItem` など）はランタイムの部品で配列ではないので、キーが無い・番号が範囲外のときはこの設定に関わらずエラー（5・9）になる
- イベントのコマンドの外（インターミッションのコマンドなど、`On Error` の無い所）の実行時エラーは、Src.exe が終わると考えられる（確かめていない）
- 範囲外の引数の参照（7474/SRC#1511）は、引数の無いコマンドを元の SRC に合わせ、引数はあって番号だけが範囲外のものは SRC# の違いとして残した（[CmdData.cls](CmdData_cls.md) の GetArg の行）

## 状態

| 状態 | 意味 |
|---|---|
| 未確認 | まだ突き合わせていない |
| 一致 | 分岐ごとに突き合わせ、違いが無い |
| 修正済み | 違いを直した。Issue・PR の番号をメモに書く |
| 違いあり | 違いが残っている。Issue の番号をメモに書く |
| SRC#で不要 | SRC# では要らない（VB の画面の部品・Win32 API など）。理由をメモに書く |
| C#に無い | C# に当たる処理が無く、要るかを決めていない。Issue の番号をメモに書く |

## 進み具合

| モジュール | 手続き | 未確認 | 一致 | 修正済み | 違いあり | SRC#で不要 | C#に無い |
|---|---|---|---|---|---|---|---|
| [AbilityData.cls](AbilityData_cls.md) | 11 | 0 | 7 | 2 | 0 | 2 | 0 |
| [AliasData.cls](AliasData_cls.md) | 10 | 0 | 8 | 1 | 0 | 1 | 0 |
| [AliasDataList.cls](AliasDataList_cls.md) | 8 | 0 | 6 | 1 | 0 | 1 | 0 |
| [BCVariable.bas](BCVariable_bas.md) | 1 | 0 | 1 | 0 | 0 | 0 | 0 |
| [BattleConfigData.cls](BattleConfigData_cls.md) | 2 | 0 | 1 | 1 | 0 | 0 | 0 |
| [BattleConfigDataList.cls](BattleConfigDataList_cls.md) | 7 | 0 | 6 | 0 | 0 | 1 | 0 |
| [COM.bas](COM_bas.md) | 18 | 0 | 7 | 11 | 0 | 0 | 0 |
| [CmdData.cls](CmdData_cls.md) | 195 | 0 | 136 | 54 | 0 | 5 | 0 |
| [Command.bas](Command_bas.md) | 52 | 0 | 28 | 24 | 0 | 0 | 0 |
| [Configuration.frm](Configuration_frm.md) | 7 | 0 | 4 | 3 | 0 | 0 | 0 |
| [Dialog.cls](Dialog_cls.md) | 7 | 0 | 4 | 1 | 0 | 2 | 0 |
| [DialogData.cls](DialogData_cls.md) | 7 | 0 | 4 | 1 | 0 | 2 | 0 |
| [DialogDataList.cls](DialogDataList_cls.md) | 7 | 0 | 5 | 1 | 0 | 1 | 0 |
| [Effect.bas](Effect_bas.md) | 23 | 0 | 21 | 2 | 0 | 0 | 0 |
| [Event.bas](Event_bas.md) | 31 | 0 | 15 | 9 | 0 | 7 | 0 |
| [Expression.bas](Expression_bas.md) | 25 | 0 | 17 | 8 | 0 | 0 | 0 |
| [FileDialog.bas](FileDialog_bas.md) | 2 | 0 | 0 | 0 | 0 | 2 | 0 |
| [Flash.bas](Flash_bas.md) | 3 | 0 | 0 | 0 | 0 | 3 | 0 |
| [GUI.bas](GUI_bas.md) | 71 | 0 | 35 | 28 | 0 | 8 | 0 |
| [GeneralLib.bas](GeneralLib_bas.md) | 33 | 0 | 15 | 14 | 0 | 4 | 0 |
| [Graphics.bas](Graphics_bas.md) | 25 | 0 | 7 | 9 | 0 | 9 | 0 |
| [Help.bas](Help_bas.md) | 7 | 0 | 3 | 4 | 0 | 0 | 0 |
| [Intermission.bas](Intermission_bas.md) | 8 | 0 | 2 | 6 | 0 | 0 | 0 |
| [Item.cls](Item_cls.md) | 31 | 0 | 26 | 0 | 0 | 5 | 0 |
| [ItemData.cls](ItemData_cls.md) | 21 | 0 | 13 | 6 | 0 | 2 | 0 |
| [ItemDataList.cls](ItemDataList_cls.md) | 7 | 0 | 5 | 1 | 0 | 1 | 0 |
| [Items.cls](Items_cls.md) | 18 | 0 | 10 | 0 | 0 | 8 | 0 |
| [LabelData.cls](LabelData_cls.md) | 4 | 0 | 1 | 3 | 0 | 0 | 0 |
| [ListBox.frm](ListBox_frm.md) | 10 | 0 | 7 | 3 | 0 | 0 | 0 |
| [Main.frm](Main_frm.md) | 14 | 0 | 8 | 4 | 0 | 2 | 0 |
| [Map.bas](Map_bas.md) | 36 | 0 | 24 | 11 | 0 | 1 | 0 |
| [MapSize.frm](MapSize_frm.md) | 3 | 0 | 0 | 0 | 0 | 3 | 0 |
| [Message.frm](Message_frm.md) | 8 | 0 | 7 | 1 | 0 | 0 | 0 |
| [MessageData.cls](MessageData_cls.md) | 6 | 0 | 4 | 1 | 0 | 1 | 0 |
| [MessageDataList.cls](MessageDataList_cls.md) | 8 | 0 | 5 | 2 | 0 | 1 | 0 |
| [MultiColumListBox.frm](MultiColumListBox_frm.md) | 6 | 0 | 4 | 1 | 0 | 1 | 0 |
| [MultiSelectListBox.frm](MultiSelectListBox_frm.md) | 10 | 0 | 8 | 2 | 0 | 0 | 0 |
| [NonPilotData.cls](NonPilotData_cls.md) | 6 | 0 | 5 | 1 | 0 | 0 | 0 |
| [NonPilotDataList.cls](NonPilotDataList_cls.md) | 9 | 0 | 7 | 1 | 0 | 1 | 0 |
| [NowLoading.frm](NowLoading_frm.md) | 1 | 0 | 0 | 1 | 0 | 0 | 0 |
| [Pilot.cls](Pilot_cls.md) | 67 | 0 | 52 | 11 | 0 | 4 | 0 |
| [PilotData.cls](PilotData_cls.md) | 36 | 0 | 30 | 5 | 0 | 1 | 0 |
| [PilotDataList.cls](PilotDataList_cls.md) | 9 | 0 | 7 | 1 | 0 | 1 | 0 |
| [Pilots.cls](Pilots_cls.md) | 20 | 0 | 9 | 2 | 0 | 9 | 0 |
| [SRC.bas](SRC_bas.md) | 17 | 0 | 5 | 8 | 0 | 4 | 0 |
| [SafeMain.frm](SafeMain_frm.md) | 14 | 0 | 0 | 0 | 0 | 14 | 0 |
| [Sound.bas](Sound_bas.md) | 18 | 0 | 6 | 4 | 0 | 8 | 0 |
| [SpecialPowerData.cls](SpecialPowerData_cls.md) | 13 | 0 | 9 | 4 | 0 | 0 | 0 |
| [SpecialPowerDataList.cls](SpecialPowerDataList_cls.md) | 7 | 0 | 5 | 1 | 0 | 1 | 0 |
| [Status.bas](Status_bas.md) | 7 | 0 | 2 | 3 | 0 | 2 | 0 |
| [Susie.bas](Susie_bas.md) | 1 | 0 | 0 | 1 | 0 | 0 | 0 |
| [TerrainData.cls](TerrainData_cls.md) | 11 | 0 | 8 | 2 | 0 | 1 | 0 |
| [TerrainDataList.cls](TerrainDataList_cls.md) | 16 | 0 | 11 | 4 | 0 | 1 | 0 |
| [Title.frm](Title_frm.md) | 1 | 0 | 0 | 0 | 0 | 1 | 0 |
| [ToolTip.frm](ToolTip_frm.md) | 2 | 0 | 0 | 1 | 0 | 1 | 0 |
| [Unit.cls](Unit_cls.md) | 276 | 0 | 197 | 70 | 0 | 9 | 0 |
| [UnitData.cls](UnitData_cls.md) | 24 | 0 | 19 | 2 | 0 | 3 | 0 |
| [UnitDataList.cls](UnitDataList_cls.md) | 8 | 0 | 6 | 1 | 0 | 1 | 0 |
| [Units.cls](Units_cls.md) | 25 | 0 | 15 | 1 | 0 | 9 | 0 |
| [VBMP3.bas](VBMP3_bas.md) | 1 | 0 | 0 | 0 | 0 | 1 | 0 |
| [WeaponData.cls](WeaponData_cls.md) | 2 | 0 | 2 | 0 | 0 | 0 | 0 |
| 計 | 1333 | 0 | 849 | 339 | 0 | 145 | 0 |
