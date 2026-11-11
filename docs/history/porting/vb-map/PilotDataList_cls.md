# PilotDataList.cls

VB 版の `SRC/SRC_20121125/PilotDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 23 | 12 | `SRCCore/Models/PilotDataList.cs` PilotDataList.AddDummyData | 一致 | ダミーパイロットの登録（コンストラクタから呼ぶ） |
| Sub Class_Terminate | 37 | 10 | （見つからない） | SRC#で不要 | Collection の解放 |
| Function Add | 49 | 7 | `SRCCore/Models/PilotDataList.cs` PilotDataList.Add | 一致 |  |
| Function Count | 58 | 3 | `SRCCore/Models/PilotDataList.cs` PilotDataList.Count | 一致 |  |
| Sub Delete | 63 | 3 | `SRCCore/Models/PilotDataList.cs` PilotDataList.Delete | 一致 |  |
| Function Item | 68 | 17 | `SRCCore/Models/PilotDataList.cs` PilotDataList.Item | 一致 |  |
| Function IsDefined | 87 | 19 | `SRCCore/Models/PilotDataList.cs` PilotDataList.IsDefined | 一致 |  |
| Function IsDefined2 | 108 | 11 | `SRCCore/Models/PilotDataList.cs` PilotDataList.IsDefined2 | 一致 |  |
| Sub Load | 121 | 1250 | `SRCCore/Models/PilotDataList.cs` PilotDataList.Load・LoadPilot<br>`SRCCore/Models/UnitDataList.cs` UnitDataList.LoadFeature・LoadWepon・LoadAbility | 修正済み | 5 点を直した。(1) 新形式の「Lvn」だけの指定を CInt と同じく偶数丸めの整数にする。(2) 特殊能力のレベルが数値でないとき、VB は CInt・CDbl の実行時エラーになり、原因を書かないデータエラーで読み込みを打ち切る。C# も同じく打ち切る。(3) 大文字・小文字・全角半角だけが違う名前のデータは、VB は Collection のキーの重複で打ち切る。(4) 「===」の直後の空行を特殊能力として読み、「行頭から1番目の特殊能力の設定が間違っています。」を出して次の行へ進む。(5) アビリティの「,」が無いときのエラーを「終りには」にする。ビットマップに .png なども受け付けるのは SRC# の拡張（#1212）。武器・アビリティを「*」で始まる行で終えるのはアイテムだけにしたので、VB と同じく武器・アビリティとして読む（#1498 のデータの項目）。ErrorHandler（開けないときの「fname が開けません。」、打ち切る誤りの DataErrorMessage と TerminateSRC）は SRC.LoadDataFile が受け持つ。ゲームとして実行するとき（SRC.ShowDataErrorDialogs）だけダイアログを出して終了し、続ける誤りも SRC.AddDataError でダイアログに出す（#1550）。開けないときは VB と同じく「。」の無い「fname が開けません」 |
