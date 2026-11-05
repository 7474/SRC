# SRCCore の TODO・XXX・コメントアウトされたコード

VB 版との突き合わせ（[対応表](README.md)）のときに、その手続きに当たるものを 1 つずつ判断する。`python3 tools/vb_port_map.py` で作り直す（手で書いた「状態」「メモ」は、ファイルと本文が同じなら残る）。

- 種類: `TODO`・`XXX`・`FIXME` はコメント、`コード` はコメントアウトされたコード（続いた行は 1 つにまとめ、最初の行と行数を書く）
- 状態: `未確認`・`直した`（PR の番号）・`SRC#で不要`（理由）・`Issue`（番号）

| 場所 | 種類 | 本文 | 状態 | メモ |
|---|---|---|---|---|
| `SRCCore/COM.cs:522` | コード | indirect_attack = selectedUnit.IsWeaponClassifiedAs(w, "間"); | 未確認 |  |
| `SRCCore/COM.cs:697` | TODO | TODO 武器選択に失敗してるケースがある（発生時は LogWarn を出してこのターンの行動を中断する） | 未確認 |  |
| `SRCCore/COM.cs:710` | コード | string[] list; | 未確認 |  |
| `SRCCore/COM.cs:3263` | XXX | XXX true でいいのか？ | 未確認 |  |
| `SRCCore/COM.cs:3408` | XXX | XXX Unit.CanFix | 未確認 |  |
| `SRCCore/COM.cs:3477` | XXX | XXX Unit.CanFix | 未確認 |  |
| `SRCCore/COM.cs:3536` | XXX | XXX false でいいのか？ | 未確認 |  |
| `SRCCore/COM.cs:4241` | XXX | XXX なんか移動前固定条件だったけれど、使えれば、でしょ。 | 未確認 |  |
| `SRCCore/COM.cs:4251` | XXX | XXX を意図して逆順なのだろう | 未確認 |  |
| `SRCCore/COM.cs:4257` | XXX | XXX ヒットはこの段階ではしてない | 未確認 |  |
| `SRCCore/COM.cs:4961` | XXX | XXX 自分の射程見なくていいのか？ | 未確認 |  |
| `SRCCore/CmdDatas/CmdParser.cs:636` | TODO | TODO Talkの中身が壊れる場面がある可能性あり（Issue #172 参照） | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Control/DoCmd.cs:17` | コード | Do - Loop While | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Control/ForEachCmd.cs:166` | XXX | XXX メインパイロットが差し変わっていたら除外？ | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Control/LocalCmd.cs:22` | XXX | XXX 定義済を再定義した時 | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Control/NextCmd.cs:67` | XXX | XXX Index再考するならする | 未確認 |  |
| `SRCCore/CmdDatas/Commands/File/OpenCmd.cs:23` | コード | var fname = SRC.ScenarioPath + GetArgAsString(2); | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Item/RemoveItemCmd.cs:56` | XXX | XXX このチェック何？ | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Other/LoadCmd.cs:35` | XXX | XXX 読み込み順序の解決がシナリオ読み込み時と大分食い違っているはず | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Other/MakePilotListCmd.cs:154` | TODO | TODO パイロットの愛称描画をGUIに切り出す（セルサイズ 32px がコア側にハードコードされている） | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Other/OptionCmd.cs:27` | XXX | XXX NewGUIMode | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Other/OptionCmd.cs:29`（4 行） | コード | { | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Pilot/RemovePilotCmd.cs:94` | XXX | XXX いい感じに消す | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Screan/PaintStringCmd.cs:15` | TODO | TODO PaintString文の処理の高速化のため、あらかじめ構文解析しておく | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Screan/PaintStringCmd.cs:33`（209 行） | コード | { | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs:34` | XXX | XXX | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs:71` | XXX | XXX これ要るの？ | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs:243` | XXX | XXX これ要るの？ | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs:264` | XXX | XXX これ要るの？ | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Talk/ATalkCmd.cs:274` | XXX | XXX これ要るの？ | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Talk/AutoTalkCmd.cs:20` | XXX | XXX 定数にしたほうがいいかな | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Unit/AttackCmd.cs:103` | XXX | XXX Push/Pop | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Unit/ChangePartyCmd.cs:68` | XXX | XXX 実装次第要素 | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Unit/OrganizeCmd.cs:18`（14 行） | コード | short unum; | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Unit/SplitCmd.cs:46` | XXX | XXX 元はSavedStateは更新していなかったけれど更新してる | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Unit/TransformCmd.cs:47` | XXX | XXX 元はSavedStateは更新していなかったけれど更新してる | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Variable/CopyArrayCmd.cs:101` | XXX | XXX 列挙時の順番がDictionaryだと問題になるかも | 未確認 |  |
| `SRCCore/CmdDatas/Commands/Variable/CopyArrayCmd.cs:124` | XXX | XXX 列挙時の順番がDictionaryだと問題になるかも | 未確認 |  |
| `SRCCore/Commands/Command.attack.cs:302` | XXX | XXX COMの攻撃と同じところは切り出しておきたい。。。 | 未確認 |  |
| `SRCCore/Commands/Command.attack.cs:322`（3 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.attack.cs:713`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.launch.cs:55` | XXX | XXX 発進後の合体がなんか変だった（バウ(ＭＡ)） | 未確認 |  |
| `SRCCore/Commands/Command.launch.cs:106` | XXX | XXX 着艦と合体の処理要らんの？ | 未確認 |  |
| `SRCCore/Commands/Command.launch.cs:156` | XXX | XXX してないよ。。。 | 未確認 |  |
| `SRCCore/Commands/Command.launch.cs:157`（2 行） | コード | Map.MapDataForUnit[withBlock1.x, withBlock1.y] = SelectedUnit; | 未確認 |  |
| `SRCCore/Commands/Command.mapcommend.cs:534` | XXX | XXX ここはフラグ立たない？ | 未確認 |  |
| `SRCCore/Commands/Command.mapcommend.cs:656` | XXX | XXX Streamを取った結果ファイル目が消え失せてしまった | 未確認 |  |
| `SRCCore/Commands/Command.preview.cs:715`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.preview.cs:735`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:112` | コード | GUI.GetAsyncKeyState(GUI.RButtonID); | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:140` | XXX | XXX ロックしたままのパターンをフォローする | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:267` | XXX | XXX | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:271`（14 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:343`（7 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:672` | XXX | XXX ユニットコマンドの初期選択？ | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:674`（7 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:1068`（10 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:1082`（7 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:1219`（7 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:1300`（15 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:1498`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.process.cs:1532`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitability.cs:360`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitability.cs:540`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitability.cs:724`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitcommand.cs:28` | コード | Application.DoEvents(); | 未確認 |  |
| `SRCCore/Commands/Command.unitcommand.cs:242` | XXX | XXX | 未確認 |  |
| `SRCCore/Commands/Command.unitcommand.cs:243` | コード | Application.DoEvents(); | 未確認 |  |
| `SRCCore/Commands/Command.unitcommand.cs:512`（7 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:23`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:76` | XXX | XXX RedrawScreen 元はしてなかった気がする | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:223` | XXX | XXX RedrawScreen 元はしてなかった気がする | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:255`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:387`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:455`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:621` | XXX | XXX RedrawScreen 元はしてなかった気がする | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:629`（11 行） | コード | int i, j; | 未確認 |  |
| `SRCCore/Commands/Command.unitform.cs:642`（2 行） | コード | list = new string[1]; | 未確認 |  |
| `SRCCore/Commands/Command.unitmove.cs:21` | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitmove.cs:23` | コード | } | 未確認 |  |
| `SRCCore/Commands/Command.unitmove.cs:29`（5 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitmove.cs:173`（5 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitmove.cs:331`（5 行） | コード | { | 未確認 |  |
| `SRCCore/Commands/Command.unitxxx.cs:71` | XXX | XXX 条件を満たす最初のアビリティ、武器、の順に選ばれる状態 | 未確認 |  |
| `SRCCore/Config/LocalFileConfig.cs:10` | TODO | TODO 項目の説明を設定ファイルに書けるようにする | 未確認 |  |
| `SRCCore/Config/LocalFileConfig.cs:11` | XXX | XXX 説明をどこでエントリーするか？　別にJSONでなくてもいい。 | 未確認 |  |
| `SRCCore/Config/LocalFileConfig.cs:30` | XXX | XXX Configの余地ないしConfigのプロパティではないかもな。 | 未確認 |  |
| `SRCCore/Events/Event.data.cs:18` | XXX | XXX 全般に1オフセットをどうするのかどっかで考えないといけない。 | 未確認 |  |
| `SRCCore/Events/Event.data.cs:41` | XXX | XXX List値それぞれの初期化 | 未確認 |  |
| `SRCCore/Events/Event.data.cs:47` | コード | CallStack.Clear(); | 未確認 |  |
| `SRCCore/Events/Event.data.cs:49`（2 行） | コード | ArgIndexStack.Clear(); | 未確認 |  |
| `SRCCore/Events/Event.data.cs:52` | コード | UpVarLevelStack.Clear(); | 未確認 |  |
| `SRCCore/Events/Event.data.cs:54`（2 行） | コード | VarIndexStack.Clear(); | 未確認 |  |
| `SRCCore/Events/Event.data.cs:57`（2 行） | コード | ForIndexStack.Clear(); | 未確認 |  |
| `SRCCore/Events/Event.data.cs:67` | XXX | XXX マッピング先が微妙。実装見て見直す。 | 未確認 |  |
| `SRCCore/Events/Event.data.cs:74` | XXX | XXX 本当か？　手抜きが祟っているだけでは？ | 未確認 |  |
| `SRCCore/Events/Event.data.cs:113` | XXX | XXX 要らんのでは | 未確認 |  |
| `SRCCore/Events/Event.data.cs:122` | XXX | XXX この辺がある時だけ再ロードするようにする | 未確認 |  |
| `SRCCore/Events/Event.data.cs:123`（3 行） | コード | bool localFileExists17() { string argfname = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve"; var ret = SRC.FileSystem.FileExi... | 未確認 |  |
| `SRCCore/Events/Event.data.cs:164` | XXX | XXX 要らんのでは | 未確認 |  |
| `SRCCore/Events/Event.data.cs:221` | XXX | XXX 多分要らん | 未確認 |  |
| `SRCCore/Events/Event.data.cs:223`（9 行） | コード | var loopTo8 = Information.UBound(EventData) - 1; | 未確認 |  |
| `SRCCore/Events/Event.data.cs:236` | XXX | XXX 多分要らん | 未確認 |  |
| `SRCCore/Events/Event.data.cs:239`（10 行） | コード | { | 未確認 |  |
| `SRCCore/Events/Event.data.cs:261` | XXX | XXX 多分要らん | 未確認 |  |
| `SRCCore/Events/Event.data.cs:264`（10 行） | コード | { | 未確認 |  |
| `SRCCore/Events/Event.data.cs:575` | XXX | XXX 条件こけてる気がする | 未確認 |  |
| `SRCCore/Events/Event.data.cs:810` | XXX | XXX これ消しとかないとまずいの？ | 未確認 |  |
| `SRCCore/Events/Event.data.cs:811` | コード | eventDataLine.Data = ""; | 未確認 |  |
| `SRCCore/Events/Event.data.cs:1087` | XXX | XXX | 未確認 |  |
| `SRCCore/Events/Event.data2.cs:15` | XXX | XXX 消し方。。。 | 未確認 |  |
| `SRCCore/Events/Event.data2.cs:48`（599 行） | コード | { | 未確認 |  |
| `SRCCore/Events/Event.handle.cs:120` | XXX | XXX 全滅の処理どこでやってんの？ | 未確認 |  |
| `SRCCore/Events/Event.handle.cs:391` | XXX | XXX 位置はリセットしない？ | 未確認 |  |
| `SRCCore/Events/Event.handle.cs:397` | XXX | XXX これはEventsが持ってていいのか？ | 未確認 |  |
| `SRCCore/Events/Event.label.cs:15` | XXX | XXX Indexがずれていて辛い。 | 未確認 |  |
| `SRCCore/Events/Event.label.cs:18` | XXX | XXX 死にたい | 未確認 |  |
| `SRCCore/Events/Event.label.cs:265` | XXX | XXX 考えさせられる | 未確認 |  |
| `SRCCore/Events/Event.props.cs:136` | XXX | XXX GUIの責務にしたい | 未確認 |  |
| `SRCCore/Expressions/Expression.function.cs:155` | コード | Item it; | 未確認 |  |
| `SRCCore/Expressions/Expression.function.cs:372`（4042 行） | コード | var PT = default(GUI.POINTAPI); | 未確認 |  |
| `SRCCore/Expressions/Expression.replace.cs:13` | TODO | TODO ref 取って戻り値か out 変数を受けるようにする決断をする。 | 未確認 |  |
| `SRCCore/Expressions/Functions/Info.cs:2584` | XXX | XXX 多分一度レンダリングしてないと取れない | 未確認 |  |
| `SRCCore/Expressions/Functions/Info.cs:2591` | XXX | XXX 未対応 | 未確認 |  |
| `SRCCore/Expressions/Functions/Other/Other.cs:103` | XXX | XXX 元は expr 切り出してたけれどなんでだろ | 未確認 |  |
| `SRCCore/Expressions/Functions/Other/Other.cs:107` | XXX | XXX このSwitch何を意図したものなのか分からん。。。 | 未確認 |  |
| `SRCCore/Expressions/Functions/Other/Other.cs:317` | XXX | XXX 元は expr 切り出してたけれどなんでだろ | 未確認 |  |
| `SRCCore/Expressions/Functions/Regex.cs:8` | XXX | XXX 全般に未疎通かつ互換性分からん | 未確認 |  |
| `SRCCore/Expressions/Functions/Regex.cs:11` | XXX | XXX 現状状態はここに持っていて動作上の問題ないがそうじゃないだろという感じ | 未確認 |  |
| `SRCCore/Expressions/VarData.cs:111` | XXX | XXX Undefind 時の参照、Numeric？ | 未確認 |  |
| `SRCCore/Expressions/VarData.cs:126` | XXX | XXX 逆の側の型に値入れなくていいの？ | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:58` | XXX | XXX 正規表現のほうが楽に書けそうではある | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:71` | XXX | XXX StringBuilder | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:859` | XXX | XXX 配列の取得 | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:1268` | XXX | XXX GetVariableObjectと実装が違う、そもそもこの辺はバグフィクスで修正したい | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:1269`（4 行） | コード | num = (short)(Event.ArgIndex - Event.ArgIndexStack[Event.CallDepth - 1 - Event.UpVarLevel]); | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:1339` | XXX | XXX Format DateTime | 未確認 |  |
| `SRCCore/Expressions/Variable.cs:1873` | XXX | XXX 0 と 未定義の区別ついていない | 未確認 |  |
| `SRCCore/Extensions/ListExtension.cs:11` | XXX | XXX EnumerableExtension | 未確認 |  |
| `SRCCore/Extensions/ListExtension.cs:12` | XXX | XXX 多分遅い | 未確認 |  |
| `SRCCore/Extensions/StreamExtension.cs:42` | XXX | XXX あまりいい同期化ではないはず | 未確認 |  |
| `SRCCore/Filesystem/LocalFilesystem.cs:33` | XXX | XXX NormalizePath で先頭 `\` を処理する？ | 未確認 |  |
| `SRCCore/Filesystem/LocalFilesystem.cs:76` | XXX | XXX Log | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:63` | XXX | XXX Map.ClearMap(); していれば要らんのでは | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:618` | XXX | XXX 分離ユニット選択肢に加えるの？ | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:620`（130 行） | コード | { | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:1307` | XXX | XXX 全半角の処理 | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:1318` | XXX | XXX 全半角の処理 | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:1402` | XXX | XXX return でいいの？ | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:1881` | XXX | XXX 何か微妙 | 未確認 |  |
| `SRCCore/Intermissions/Intermission.cs:2201` | XXX | XXX 処理系によってはファイル名が取れないかもしれない | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:23`（5 行） | コード | static extern int GetPrivateProfileString(string lpApplicationName, Any lpKeyName, string lpDefault, string lpReturne... | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:88` | XXX | XXX | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:112`（27 行） | コード | static extern int timeBeginPeriod(int uPeriod); | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:232`（35 行） | コード | { | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:355` | XXX | XXX リスト中のカッコ消えちゃうと困るしもとは消してないっぽいから消さないでおく | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:416`（52 行） | コード | { | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:558` | TODO | TODO 精査、 Byte length 系と統合する | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:581`（58 行） | コード | { | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:679`（120 行） | コード | { | 未確認 |  |
| `SRCCore/Lib/GeneralLib.cs:840` | XXX | XXX ヒットを配列にして返すのがよさそう | 未確認 |  |
| `SRCCore/Maps/Map.cs:37` | XXX | XXX シリアライズから復元するのか保存用データクラス作るのかはっきりしたほうがよさそう | 未確認 |  |
| `SRCCore/Maps/Map.cs:39`（11 行） | コード | FileSystem.Input(SRC.SaveDataFileNumber, sbuf1); | 未確認 |  |
| `SRCCore/Maps/Map.cs:127` | XXX | XXX | 未確認 |  |
| `SRCCore/Maps/Map.cs:136` | XXX | XXX この辺完全に未精査 | 未確認 |  |
| `SRCCore/Maps/Map.cs:157` | コード | public Src2DArray<MapImageFileType> MapImageFileTypeData; | 未確認 |  |
| `SRCCore/Maps/Map.cs:464` | XXX | XXX MapのGUIインタフェースと実装に追い出す | 未確認 |  |
| `SRCCore/Maps/Map.cs:465`（2 行） | コード | GUI.MapPWidth = (32 * w); | 未確認 |  |
| `SRCCore/Maps/Map.cs:476` | XXX | XXX 1というよりは0にしたいが0長配列は作れない | 未確認 |  |
| `SRCCore/Maps/Map.cs:486` | XXX | XXX | 未確認 |  |
| `SRCCore/Maps/Map.cs:489`（3 行） | コード | { | 未確認 |  |
| `SRCCore/Maps/Map.cs:2162` | XXX | XXX 合体制限は変形してる時だけでいい？ | 未確認 |  |
| `SRCCore/Maps/Map.cs:2183` | XXX | XXX 合体制限は変形してる時だけでいい？ | 未確認 |  |
| `SRCCore/Maps/Map.cs:3350` | XXX | XXX マップの最大サイズの制限になるはず | 未確認 |  |
| `SRCCore/Maps/Map.cs:3356` | XXX | XXX ジャンプとかテレポートは？ | 未確認 |  |
| `SRCCore/Maps/Map.cs:3436` | XXX | XXX MaskData の更新してない気がする | 未確認 |  |
| `SRCCore/Models/BattleConfigDataList.cs:91` | XXX | XXX これでいいんか？　nullになったり上書きされそう。 | 未確認 |  |
| `SRCCore/Models/Condition.cs:14` | XXX | XXX 負数は無期限（多分） | 未確認 |  |
| `SRCCore/Models/Dialog.cs:193`（13 行） | コード | { | 未確認 |  |
| `SRCCore/Models/DialogData.cs:47`（13 行） | コード | { | 未確認 |  |
| `SRCCore/Models/DialogData.cs:67`（585 行） | コード | Dialog SelectDialogRet = default; | 未確認 |  |
| `SRCCore/Models/MessageData.cs:30` | XXX | XXX MessageDataItem#IsAvailable 常に真でいいか | 未確認 |  |
| `SRCCore/Models/NonPilotData.cs:38`（58 行） | コード | { | 未確認 |  |
| `SRCCore/Models/PilotData.cs:506` | XXX | XXX Readonlyにするとか | 未確認 |  |
| `SRCCore/Models/PilotData.cs:699` | XXX | XXX 例外処理要らんの？ | 未確認 |  |
| `SRCCore/Models/SpecialPowerData.cs:199`（21 行） | コード | { | 未確認 |  |
| `SRCCore/Models/SpecialPowerData.cs:2374`（13 行） | コード | string anime; | 未確認 |  |
| `SRCCore/Models/TerrainData.cs:128`（19 行） | コード | { | 未確認 |  |
| `SRCCore/Models/TerrainData.cs:169`（10 行） | コード | { | 未確認 |  |
| `SRCCore/Models/TerrainData.cs:188`（37 行） | コード | { | 未確認 |  |
| `SRCCore/Models/TerrainDataList.cs:82`（78 行） | コード | { | 未確認 |  |
| `SRCCore/Models/UnitData.cs:277` | XXX | XXX 元は Name を返却 | 未確認 |  |
| `SRCCore/Models/UnitData.cs:282` | XXX | XXX 参照先はNameだった | 未確認 |  |
| `SRCCore/Models/UnitData.cs:283` | コード | FeatureRet = fd.Name; | 未確認 |  |
| `SRCCore/Models/UnitData.cs:287` | XXX | XXX このくらいなら Feature(string Index) に統合できる気はする | 未確認 |  |
| `SRCCore/Models/UnitDataList.cs:109` | コード | { | 未確認 |  |
| `SRCCore/Models/UnitDataList.cs:119`（5 行） | コード | } | 未確認 |  |
| `SRCCore/Models/UnitDataList.cs:311` | XXX | XXX 何で負数にしてるの？ | 未確認 |  |
| `SRCCore/Pilots/Pilot.cs:52` | XXX | XXX こうしてるところシリアライズの時だけID解決のほうがいいかな。 | 未確認 |  |
| `SRCCore/Pilots/Pilot.skill.cs:101`（20 行） | コード | { | 未確認 |  |
| `SRCCore/Pilots/Pilot.skill.cs:1228` | XXX | XXX 疎通してないから怪しいかもしれん | 未確認 |  |
| `SRCCore/Pilots/Pilot.unit.cs:135` | XXX | XXX 外から操作したいものではない | 未確認 |  |
| `SRCCore/Pilots/Pilots.cs:204` | XXX | XXX Clone List | 未確認 |  |
| `SRCCore/Pilots/Pilots.cs:268`（307 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.config.cs:19`（147 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.cs:244` | XXX | XXX 別な実装をするならコンストラクタでは設定しない | 未確認 |  |
| `SRCCore/SRC.data.cs:30` | XXX | XXX frmNowLoading | 未確認 |  |
| `SRCCore/SRC.data.cs:32` | コード | { | 未確認 |  |
| `SRCCore/SRC.data.cs:34` | コード | } | 未確認 |  |
| `SRCCore/SRC.data.cs:47`（3 行） | コード | ; | 未確認 |  |
| `SRCCore/SRC.data.cs:169`（3 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.main.cs:25`（9 行） | コード | string fname; | 未確認 |  |
| `SRCCore/SRC.main.cs:43`（22 行） | コード | bool localFileExists() { string argfname = AppPath + "Src.ini"; var ret = FileSystem.FileExists(argfname); return ret; } | 未確認 |  |
| `SRCCore/SRC.main.cs:198`（173 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.main.cs:412`（212 行） | コード | buf = GeneralLib.ReadIni("Option", "SpecialPowerAnimation"); | 未確認 |  |
| `SRCCore/SRC.main.cs:629`（101 行） | コード | buf = GeneralLib.ReadIni("Option", "ImageBufferNum"); | 未確認 |  |
| `SRCCore/SRC.main.cs:828` | コード | { | 未確認 |  |
| `SRCCore/SRC.main.cs:832`（64 行） | コード | i = 0; | 未確認 |  |
| `SRCCore/SRC.main.cs:909`（58 行） | コード | GUI.ChangeStatus(GuiStatus.Default); | 未確認 |  |
| `SRCCore/SRC.main.cs:968`（4 行） | コード | } | 未確認 |  |
| `SRCCore/SRC.main.cs:988`（2 行） | コード | ExtDataPath = GeneralLib.ReadIni("Option", "ExtDataPath"); | 未確認 |  |
| `SRCCore/SRC.main.cs:998`（17 行） | コード | GUI.ChangeStatus(GuiStatus.Default); | 未確認 |  |
| `SRCCore/SRC.play.cs:30` | XXX | XXX ウィンドウのタイトルを設定するなら IGUI.MainFormText を見直す | 未確認 |  |
| `SRCCore/SRC.play.cs:36`（29 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.play.cs:113` | XXX | XXX Mapサイズ処理が要りそうなら処理する | 未確認 |  |
| `SRCCore/SRC.play.cs:115`（3 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.play.cs:161` | コード | GUI.MainForm.Refresh(); | 未確認 |  |
| `SRCCore/SRC.play.cs:628` | XXX | XXX 二段階走査してるってこと？ | 未確認 |  |
| `SRCCore/SRC.play.cs:705` | XXX | XXX | 未確認 |  |
| `SRCCore/SRC.play.cs:706` | コード | Application.DoEvents(); | 未確認 |  |
| `SRCCore/SRC.play.cs:890` | コード | Exit.eveを探す | 未確認 |  |
| `SRCCore/SRC.play.cs:900` | コード | Exit.eveが無ければそのまま終了 | 未確認 |  |
| `SRCCore/SRC.play.cs:910` | コード | Exit.eveを読み込み | 未確認 |  |
| `SRCCore/SRC.play.cs:920` | コード | Exit.eveのプロローグイベントを実施 | 未確認 |  |
| `SRCCore/SRC.play.cs:932`（37 行） | コード | { | 未確認 |  |
| `SRCCore/SRC.save.cs:64` | TODO | TODO セーブ・ロードの精査（基本的な保存・読み込みは実装済み; 完全な互換性検証が残タスク） | 未確認 |  |
| `SRCCore/SRC.save.cs:72` | XXX | XXX EntryなのかCoreなのか | 未確認 |  |
| `SRCCore/SRC.save.cs:99` | XXX | XXX Version プロパティだけのオブジェクトでバージョンチェックなど | 未確認 |  |
| `SRCCore/SRC.save.cs:131`（4 行） | コード | PList.LoadLinkInfo(); | 未確認 |  |
| `SRCCore/SRC.save.cs:158` | XXX | XXX FileName | 未確認 |  |
| `SRCCore/SRC.save.cs:173` | XXX | XXX EntryなのかCoreなのか | 未確認 |  |
| `SRCCore/SRC.save.cs:269` | XXX | XXX FileName | 未確認 |  |
| `SRCCore/SRC.save.cs:296` | XXX | XXX Version プロパティだけのオブジェクトでバージョンチェックなど | 未確認 |  |
| `SRCCore/SRC.save.cs:298` | コード | SaveDataVersion = Conversions.ToInteger(fname2); | 未確認 |  |
| `SRCCore/SRC.save.cs:349` | XXX | XXX 仮処理 | 未確認 |  |
| `SRCCore/SRC.save.cs:388`（15 行） | コード | Event.SkipEventData(); | 未確認 |  |
| `SRCCore/Sound.cs:147` | XXX | XXX 止まってるってことはリピートしてないってことだろう。 | 未確認 |  |
| `SRCCore/Sound.cs:244` | XXX | XXX なんで？ | 未確認 |  |
| `SRCCore/Sound.cs:426` | XXX | XXX なんで wave_name 分解したんだろう？ | 未確認 |  |
| `SRCCore/Sound.cs:427`（4 行） | コード | var waveNames = GeneralLib.ToList(wave_name, true); | 未確認 |  |
| `SRCCore/Sound.cs:470` | コード | ResetBGM(); | 未確認 |  |
| `SRCCore/Sound.cs:501`（4 行） | コード | { | 未確認 |  |
| `SRCCore/Statuses/Status.cs:1`（5355 行） | コード | using System; | 未確認 |  |
| `SRCCore/UIInterface/IGUI.cs:23` | XXX | XXX 要プロパティ更新、戻り値で返したほうがいい気はする。そもそも実行をロックしたくない。 | 未確認 |  |
| `SRCCore/UIInterface/IGUI.cs:24` | コード | void StartWaitClick(); | 未確認 |  |
| `SRCCore/UIInterface/ISystemConfig.cs:18` | XXX | XXX GUIの関心毎 | 未確認 |  |
| `SRCCore/UIInterface/ISystemConfig.cs:34` | XXX | XXX この辺はSRCのコア的にはどうでもいい | 未確認 |  |
| `SRCCore/UIInterface/ISystemConfig.cs:41` | XXX | XXX /この辺はSRCのコア的にはどうでもいい | 未確認 |  |
| `SRCCore/UIInterface/ISystemConfig.cs:46` | XXX | XXX この辺に依存する処理はあんまり嬉しくない | 未確認 |  |
| `SRCCore/Units/Unit.ability.cs:32` | XXX | XXX 攻撃同様にしたけれど、UnitXxxを用意するならそっちが持ってるべきだったかもしれない。 | 未確認 |  |
| `SRCCore/Units/Unit.ability.cs:2644` | XXX | XXX 変形技 | 未確認 |  |
| `SRCCore/Units/Unit.attackcheck.cs:6771` | XXX | XXX 元は変数オブジェクト取得して直接設定していた。ローカル変数と競合した時向け？ | 未確認 |  |
| `SRCCore/Units/Unit.condition.cs:81` | コード | return colCondition[Index].Name; | 未確認 |  |
| `SRCCore/Units/Unit.condition.cs:105`（7 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:18` | コード | For i = .AdditionalFeaturesNum + 1 To .CountAllFeature / fname = .AllFeatureName(i)) | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:36` | XXX | XXX 参照先はNameだった | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:37` | コード | FeatureRet = fd.Name; | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:42` | XXX | XXX 参照先はNameだった | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:43` | コード | FeatureRet = fd.Name; | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:83`（20 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:118`（5 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:136`（114 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:254` | XXX | XXX 同一インスタンスじゃないかも | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:312`（20 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.feature.cs:340` | XXX | XXX colAllFeature の更新要らんの？ | 未確認 |  |
| `SRCCore/Units/Unit.item.cs:64` | XXX | XXX ケース考慮など | 未確認 |  |
| `SRCCore/Units/Unit.item.cs:454` | XXX | XXX 元は削除するアイテムの武器・アビリティの残弾数が引き継がれるのを防ぐため、 | 未確認 |  |
| `SRCCore/Units/Unit.lookup.cs:1851` | XXX | XXX もうちょいいい感じにしたいもんだ | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:45` | XXX | XXX Goto 潰しておきたい | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:338` | XXX | XXX 何が違うんだ？ | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:375` | XXX | XXX 合体制限は変形してる時だけでいい？ | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:388` | XXX | XXX そういう理由でエラーするの？」 | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:1802`（2 行） | コード | n = 1; | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:1875` | XXX | XXX 列挙子の複製どうすんだっけ | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:1909` | XXX | XXX Unit同士の比較の標準化 | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:1957` | XXX | XXX 要は先頭に乗る、だろ？ | 未確認 |  |
| `SRCCore/Units/Unit.map.cs:1958`（2 行） | コード | Unit localItem3() { object argIndex1 = GeneralLib.LIndex(buf, 2); | 未確認 |  |
| `SRCCore/Units/Unit.onboard.cs:24` | XXX | XXX 一致のケース考慮してなかったかも？ | 未確認 |  |
| `SRCCore/Units/Unit.pilot.cs:356` | TODO | TODO 使用している個所でもともと追加サポートを処理していたか精査 | 未確認 |  |
| `SRCCore/Units/Unit.recover.cs:26` | XXX | XXX ! 条件が複数あった時は？ そもそも修理不可Helpにも実装にもないけれどどういう存在なんだ？ | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:47`（25 行） | コード | WeaponData[] prev_wdata; | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:81` | コード | ubitmap = get_Bitmap(false); | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:405`（60 行） | コード | found = false; | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:489`（44 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:1082`（35 行） | コード | { | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:1736` | コード | intMaxBullet[i] = Weapon(i).Bullet; | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:1770`（17 行） | コード | Array.Resize(dblBullet, CountWeapon() + 1); | 未確認 |  |
| `SRCCore/Units/Unit.status.cs:1807`（149 行） | コード | prev_adata = new AbilityData[Information.UBound(adata) + 1]; | 未確認 |  |
| `SRCCore/Units/UnitAbility.cs:620` | XXX | XXX 変形技が妥当に定義されていなかった場合 | 未確認 |  |
| `SRCCore/Units/UnitAbility.cs:1580` | XXX | XXX 条件によって check_formation 見直す | 未確認 |  |
| `SRCCore/Units/UnitWeapon.cs:58` | XXX | XXX アイテムとかでの追加があるとそっちが最強になっちゃうんじゃねぇか？ | 未確認 |  |
| `SRCCore/Units/UnitWeapon.cs:163` | XXX | XXX この辺の参照考えると UnitWeapon として切り出したのイマイチだったかもなぁ。 | SRC#で不要 | 設計の感想で、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:3968` | XXX | XXX この辺の判断切り出せそうな気はする | SRC#で不要 | 処理の切り出しの覚え書きで、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:4288` | XXX | XXX 処理簡便にしておきたい | SRC#で不要 | バリア・フィールド・プロテクションの整理の覚え書きで、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:5025` | XXX | XXX これどういう式なのかいまいち分からん | 未確認 |  |
| `SRCCore/Units/UnitWeapon.cs:5090` | XXX | XXX いつの仕様だろ | 未確認 |  |
| `SRCCore/Units/UnitWeapon.cs:5092`（3 行） | コード | If IsWeaponClassifiedAs(w, "合") Then | 未確認 |  |
| `SRCCore/Units/UnitWeapon.cs:5329` | XXX | XXX 条件によって check_formation 見直す | 未確認 |  |
| `SRCCore/Units/Units.cs:736`（298 行） | コード | { | 未確認 |  |
| `SRCCore/VB/Conversions.cs:94` | XXX | XXX DateTime パースの精査 | 未確認 |  |
| `SRCCore/VB/Conversions.cs:99` | XXX | XXX 変換ですらないがパースの隣にあったほうが見通しがいいので。。。 | 未確認 |  |
| `SRCCore/VB/Information.cs:18` | XXX | XXX | 未確認 |  |
| `SRCCore/VB/Information.cs:19` | コード | Convert.ToDecimal(str); | 未確認 |  |
| `SRCCore/VB/SrcArray.cs:14` | XXX | XXX 他のIndexアクセスするやつ | 未確認 |  |
| `SRCCore/VB/SrcCollection.cs:143` | XXX | XXX 本当にこんな処理になるのか？ | 未確認 |  |
| `SRCCore/VB/Strings.cs:320` | コード | For dakuten/handakuten marks and other special cases, return as-is | 未確認 |  |
| `SRCCore/VB/Strings.cs:338` | コード | For special cases, return as-is | 未確認 |  |
