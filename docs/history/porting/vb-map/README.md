# VB 版と C# の手続きの対応表

VB 版（`SRC/SRC_20121125`）の手続き（Function・Sub・Property）を、C# の対応するメソッドと 1 つずつ突き合わせるための表（7474/SRC#1498）。

- 表は `python3 tools/vb_port_map.py` で作る。作り直しても、手で書いた「C#」「状態」「メモ」は残る。行番号は VB 版の行
- CI（`port-map-check`）の `python3 tools/vb_port_map.py --check` で、VB のすべての手続きが表にあり、状態の書き方が正しいかを確かめる
- 対象は `SRC/SRC_20121125` の `*.bas`・`*.cls`・`*.frm` のすべての手続き。選んで外した手続きは無い（手続きの外にあるものは下の「表に載っていないもの」）
- 「C#」の候補は名前だけで選んでいる。`（見つからない）` は名前の違う C# に移したか、移していないもの。突き合わせのときに直す

## 突き合わせの手順

1. VB の手続きと C# のメソッドを並べ、分岐（`If`・`Select Case`・ループの抜け方・`Exit`・`On Error`）ごとに同じかを確かめる
2. 違いは、元の SRC の動作を期待値にしたテストにして直す（[判断の記録](../../../decisions/2026-10-15-vb-behavior-as-test-oracle.md)）。すぐに直せないものは Issue にして、テストに `[Ignore("移植の不具合: …")]` を付ける
3. その手続きに当たる C# の `TODO`・`XXX`・コメントアウトされた VB のコード（[一覧](todo.md)）を判断する（直す・SRC# では要らない・Issue にする）
4. 表の「状態」と「メモ」を書く。機械で見つけられる型の誤りなら、[SRCPortingAnalyzers](../../../../SRC.Sharp/SRCPortingAnalyzers/README.md) に規則を足す
5. あとの単位で扱う手がかりは、その手続きの行の「メモ」に書く（状態は「未確認」のまま）

## 表に載っていないもの（突き合わせのときに一緒に見る）

表は手続き（Function・Sub・Property）を単位にしているので、次のものは載っていない。手続きを突き合わせるときに、そのモジュールの分を一緒に見る。

| 種類 | 見ること |
|---|---|
| モジュールの変数の宣言（手続きの外。約 600） | 型（Integer・Long・Single・Double・String・Variant）。代入の丸め・あふれ・既定値が型で決まる。初期値は `Class_Initialize` の行で見る |
| `Const`（約 120）・`Type`・`Enum`（約 30） | 値と項目が C# の定数・型と同じか。既定値・上限などの数値が違えば動作が変わる（#1498 のチェックリストの「モジュールの宣言」の項目でまとめて見る） |
| `Declare`（Win32 API の宣言。約 120） | SRC# では使わない。それを呼ぶ手続きの行で、代わりの処理を見る |
| `.frm` の画面の定義（`Begin VB.Form` 〜 `Attribute VB_Name` の前）・`.frx` | ボタン・リストなどの配置・大きさ・既定の文言・タブの順。フォームの手続きを突き合わせるときに、SRCRender・SRCSharpForm の画面と比べる |
| 各ファイルの先頭の `Option` 文 | `Option Compare` を書いたファイルは無い。文字列の比較は既定の Binary（大文字・小文字・全角半角・ひらがなカタカナを区別する。ただし Collection のキーは区別しない） |

## 元の SRC の実行ファイルのコンパイルの設定（`SRC/SRC_20121125/SRC.VBP`）

「VB では実行時エラーになる」と判断するときは、配布された Src.exe のコンパイルの設定を考える。`SRC.VBP` には次の指定がある。

```
BoundsCheck=-1      ' 配列の範囲チェックをしない
OverflowCheck=-1    ' 整数のオーバーフローチェックをしない
FDIVCheck=-1        ' Pentium の FDIV の不具合のチェックをしない
NoAliasing=-1       ' 別名（エイリアス）が無いとみなす
```

- この読み方（-1 は「チェックをしない」に印が付いた状態）は、VB6 のプロジェクトの設定の画面（コンパイル → 詳細な最適化）の項目と対応させたもの。資料ではまだ確かめていない。最初にこの設定に関わる判断をするときに確かめ、ここを直す
- この読み方が正しければ、Src.exe では配列の範囲外の参照（エラー 9）と整数のあふれ（エラー 6）は**実行時エラーにならない**。範囲外の読み書きは別のメモリを読み書きし、あふれは桁が落ちる。IDE で実行したときとは違う
- 0 での割り算（エラー 11）・`Nothing` の参照（エラー 91）・型の不一致（エラー 13）・`CDbl` などの変換の失敗は、この設定に関わらずエラーになる。Collection（`colItem` など）はランタイムの部品で配列ではないので、キーが無い・番号が範囲外のときはこの設定に関わらずエラー（5・9）になる
- メモに「VB は実行時エラー（なので SRC# は動かす）」と書いた行のうち、範囲外・あふれを理由にしたものは、見直すときにこの設定を考える（例: レベルの無い連属性の `\ 0` は 0 での割り算なのでエラーのまま。範囲外の `Weapon()` は範囲チェックの話）。7474/SRC#1511 もこの設定を確かめてから進める

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
| [AbilityData.cls](AbilityData_cls.md) | 11 | 11 | 0 | 0 | 0 | 0 | 0 |
| [AliasData.cls](AliasData_cls.md) | 10 | 10 | 0 | 0 | 0 | 0 | 0 |
| [AliasDataList.cls](AliasDataList_cls.md) | 8 | 8 | 0 | 0 | 0 | 0 | 0 |
| [BCVariable.bas](BCVariable_bas.md) | 1 | 1 | 0 | 0 | 0 | 0 | 0 |
| [BattleConfigData.cls](BattleConfigData_cls.md) | 2 | 2 | 0 | 0 | 0 | 0 | 0 |
| [BattleConfigDataList.cls](BattleConfigDataList_cls.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [COM.bas](COM_bas.md) | 18 | 0 | 7 | 11 | 0 | 0 | 0 |
| [CmdData.cls](CmdData_cls.md) | 195 | 76 | 84 | 29 | 2 | 4 | 0 |
| [Command.bas](Command_bas.md) | 52 | 52 | 0 | 0 | 0 | 0 | 0 |
| [Configuration.frm](Configuration_frm.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [Dialog.cls](Dialog_cls.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [DialogData.cls](DialogData_cls.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [DialogDataList.cls](DialogDataList_cls.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [Effect.bas](Effect_bas.md) | 23 | 23 | 0 | 0 | 0 | 0 | 0 |
| [Event.bas](Event_bas.md) | 31 | 31 | 0 | 0 | 0 | 0 | 0 |
| [Expression.bas](Expression_bas.md) | 25 | 25 | 0 | 0 | 0 | 0 | 0 |
| [FileDialog.bas](FileDialog_bas.md) | 2 | 2 | 0 | 0 | 0 | 0 | 0 |
| [Flash.bas](Flash_bas.md) | 3 | 3 | 0 | 0 | 0 | 0 | 0 |
| [GUI.bas](GUI_bas.md) | 71 | 71 | 0 | 0 | 0 | 0 | 0 |
| [GeneralLib.bas](GeneralLib_bas.md) | 33 | 33 | 0 | 0 | 0 | 0 | 0 |
| [Graphics.bas](Graphics_bas.md) | 25 | 25 | 0 | 0 | 0 | 0 | 0 |
| [Help.bas](Help_bas.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [Intermission.bas](Intermission_bas.md) | 8 | 8 | 0 | 0 | 0 | 0 | 0 |
| [Item.cls](Item_cls.md) | 31 | 31 | 0 | 0 | 0 | 0 | 0 |
| [ItemData.cls](ItemData_cls.md) | 21 | 21 | 0 | 0 | 0 | 0 | 0 |
| [ItemDataList.cls](ItemDataList_cls.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [Items.cls](Items_cls.md) | 18 | 18 | 0 | 0 | 0 | 0 | 0 |
| [LabelData.cls](LabelData_cls.md) | 4 | 4 | 0 | 0 | 0 | 0 | 0 |
| [ListBox.frm](ListBox_frm.md) | 10 | 10 | 0 | 0 | 0 | 0 | 0 |
| [Main.frm](Main_frm.md) | 14 | 14 | 0 | 0 | 0 | 0 | 0 |
| [Map.bas](Map_bas.md) | 36 | 36 | 0 | 0 | 0 | 0 | 0 |
| [MapSize.frm](MapSize_frm.md) | 3 | 3 | 0 | 0 | 0 | 0 | 0 |
| [Message.frm](Message_frm.md) | 8 | 8 | 0 | 0 | 0 | 0 | 0 |
| [MessageData.cls](MessageData_cls.md) | 6 | 6 | 0 | 0 | 0 | 0 | 0 |
| [MessageDataList.cls](MessageDataList_cls.md) | 8 | 8 | 0 | 0 | 0 | 0 | 0 |
| [MultiColumListBox.frm](MultiColumListBox_frm.md) | 6 | 6 | 0 | 0 | 0 | 0 | 0 |
| [MultiSelectListBox.frm](MultiSelectListBox_frm.md) | 10 | 10 | 0 | 0 | 0 | 0 | 0 |
| [NonPilotData.cls](NonPilotData_cls.md) | 6 | 6 | 0 | 0 | 0 | 0 | 0 |
| [NonPilotDataList.cls](NonPilotDataList_cls.md) | 9 | 9 | 0 | 0 | 0 | 0 | 0 |
| [NowLoading.frm](NowLoading_frm.md) | 1 | 1 | 0 | 0 | 0 | 0 | 0 |
| [Pilot.cls](Pilot_cls.md) | 67 | 67 | 0 | 0 | 0 | 0 | 0 |
| [PilotData.cls](PilotData_cls.md) | 36 | 36 | 0 | 0 | 0 | 0 | 0 |
| [PilotDataList.cls](PilotDataList_cls.md) | 9 | 9 | 0 | 0 | 0 | 0 | 0 |
| [Pilots.cls](Pilots_cls.md) | 20 | 20 | 0 | 0 | 0 | 0 | 0 |
| [SRC.bas](SRC_bas.md) | 17 | 17 | 0 | 0 | 0 | 0 | 0 |
| [SafeMain.frm](SafeMain_frm.md) | 14 | 14 | 0 | 0 | 0 | 0 | 0 |
| [Sound.bas](Sound_bas.md) | 18 | 18 | 0 | 0 | 0 | 0 | 0 |
| [SpecialPowerData.cls](SpecialPowerData_cls.md) | 13 | 13 | 0 | 0 | 0 | 0 | 0 |
| [SpecialPowerDataList.cls](SpecialPowerDataList_cls.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [Status.bas](Status_bas.md) | 7 | 7 | 0 | 0 | 0 | 0 | 0 |
| [Susie.bas](Susie_bas.md) | 1 | 1 | 0 | 0 | 0 | 0 | 0 |
| [TerrainData.cls](TerrainData_cls.md) | 11 | 11 | 0 | 0 | 0 | 0 | 0 |
| [TerrainDataList.cls](TerrainDataList_cls.md) | 16 | 16 | 0 | 0 | 0 | 0 | 0 |
| [Title.frm](Title_frm.md) | 1 | 1 | 0 | 0 | 0 | 0 | 0 |
| [ToolTip.frm](ToolTip_frm.md) | 2 | 2 | 0 | 0 | 0 | 0 | 0 |
| [Unit.cls](Unit_cls.md) | 276 | 192 | 54 | 30 | 0 | 0 | 0 |
| [UnitData.cls](UnitData_cls.md) | 24 | 24 | 0 | 0 | 0 | 0 | 0 |
| [UnitDataList.cls](UnitDataList_cls.md) | 8 | 8 | 0 | 0 | 0 | 0 | 0 |
| [Units.cls](Units_cls.md) | 25 | 25 | 0 | 0 | 0 | 0 | 0 |
| [VBMP3.bas](VBMP3_bas.md) | 1 | 1 | 0 | 0 | 0 | 0 | 0 |
| [WeaponData.cls](WeaponData_cls.md) | 2 | 2 | 0 | 0 | 0 | 0 | 0 |
| 計 | 1333 | 1112 | 145 | 70 | 2 | 4 | 0 |
