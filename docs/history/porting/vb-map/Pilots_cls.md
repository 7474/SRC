# Pilots.cls

VB 版の `SRC/SRC_20121125/Pilots.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 23 | 10 | （見つからない） | SRC#で不要 | Collection の解放。GC に任せる |
| Function NewEnum | 35 | 5 | `SRCCore/Pilots/Pilots.cs` Pilots.Items | SRC#で不要 | For Each 用。Items・AlivePilots で列挙する |
| Function Add | 43 | 60 | `SRCCore/Pilots/Pilots.cs` Pilots.Add | 一致 | パイロットデータが無いとき VB は実行時エラー 91、C# は TerminateException。大文字・小文字だけ違う ID の重複は両方とも例外 |
| Function Count | 105 | 3 | `SRCCore/Pilots/Pilots.cs` Pilots.Count | 一致 |  |
| Sub Delete | 110 | 3 | `SRCCore/Pilots/Pilots.cs` Pilots.Delete | 一致 | キーが無いとき VB はエラー 5、C# は何もしない（呼び出し元はある ID だけを渡す） |
| Function Item | 115 | 31 | `SRCCore/Pilots/Pilots.cs` Pilots.Item | 修正済み | ID で見つかったパイロットが破棄されていて、名称・愛称で生きているパイロットも見つからなければ、その破棄されたパイロットを返す（VB は On Error の後も Item が代入されたまま）。C# は null を返していた |
| Function IsDefined | 148 | 32 | `SRCCore/Pilots/Pilots.cs` Pilots.IsDefined | 一致 | Item から切り離して VB と同じ判定にした（破棄されたパイロットは False） |
| Function Item2 | 182 | 8 | `SRCCore/Pilots/Pilots.cs` Pilots.Item2 | 一致 |  |
| Function IsDefined2 | 192 | 11 | `SRCCore/Pilots/Pilots.cs` Pilots.IsDefined2 | 一致 |  |
| Sub Update | 205 | 48 | `SRCCore/Pilots/Pilots.cs` Pilots.Update | 一致 | 削除しながら列挙するため、C# はリストを写してから回す |
| Sub Save | 256 | 37 | `SRCCore/SRC.save.cs` SRC.SaveData | SRC#で不要 | SRC# は PList を JSON でそのまま保存する（コメントアウトのコードあり） |
| Sub Load | 295 | 41 | `SRCCore/SRC.save.cs` SRC.LoadData | SRC#で不要 | JSON から戻して PList.Restore。VB は追加パイロット・サポートを除いて保存し、読み込むときに全員を「味方」・生存・全回復で Add し直す。SRC# は以前は PList.Update で味方以外・破棄・追加パイロットを消し、残りを全回復していた（シナリオの途中で SaveData コマンドを使ったときに違った）。VB の LoadData は PList.Update を呼ばないので、LoadData で追加パイロット・追加サポートを消し、残りを「味方」・生存にして全回復する（#1579） |
| Sub LoadLinkInfo | 338 | 65 | `SRCCore/SRC.save.cs` SRC.LoadData | SRC#で不要 | ユニットとのつながりは JSON（UnitId・Unit.Restore）で戻す。離脱（Away）も保存している |
| Sub Dump | 406 | 23 | `SRCCore/SRC.save.cs` SRC.DumpData | SRC#で不要 | JSON。VB と違い追加パイロットも保存する（VB は読み込んだ後に作り直す） |
| Sub Restore | 431 | 20 | `SRCCore/Pilots/Pilots.cs` Pilots.Restore | SRC#で不要 | JSON から戻した各パイロットに SRC を付け直す |
| Sub RestoreLinkInfo | 453 | 9 | `SRCCore/Pilots/Pilots.cs` Pilots.Restore | SRC#で不要 | JSON のつながりを Pilot.Restore・Unit.Restore で戻す |
| Sub RestoreParameter | 464 | 9 | `SRCCore/SRC.save.cs` SRC.RestoreData | 修正済み | パラメータは JSON に入っている。VB はこの後に PList.UpdateSupportMod を呼ぶが C# は呼んでおらず、支援効果の修正値（InfightMod2 など。保存しない）が 0 のままだった。RestoreData で呼ぶようにした |
| Sub Clear | 476 | 7 | （見つからない） | SRC#で不要 | VB でも呼ばれていない。SRC# は PList を作り直す |
| Sub UpdateSupportMod | 486 | 93 | `SRCCore/Pilots/Pilots.cs` Pilots.UpdateSupportMod | 一致 |  |
| Sub Clean | 581 | 12 | `SRCCore/Pilots/Pilots.cs` Pilots.Clean | 一致 |  |
