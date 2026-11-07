# SRCCore の TODO・XXX・コメントアウトされたコード

VB 版との突き合わせ（[対応表](README.md)）のときに、その手続きに当たるものを 1 つずつ判断する。`python3 tools/vb_port_map.py` で作り直す（手で書いた「状態」「メモ」は、ファイルと本文が同じなら残る）。

- 種類: `TODO`・`XXX`・`FIXME` はコメント、`コード` はコメントアウトされたコード（続いた行は 1 つにまとめ、最初の行と行数を書く）
- 状態: `未確認`・`直した`（PR の番号）・`SRC#で不要`（理由）・`Issue`（番号）

| 場所 | 種類 | 本文 | 状態 | メモ |
|---|---|---|---|---|
| `SRCCore/CmdDatas/Commands/Control/DoCmd.cs:17` | コード | Do - Loop While | SRC#で不要 | 分岐の説明のコメントで、VB のコードの残りではない |
| `SRCCore/CmdDatas/Commands/Other/LoadCmd.cs:35` | XXX | XXX 読み込み順序の解決がシナリオ読み込み時と大分食い違っているはず | Issue | #1518 |
| `SRCCore/Commands/Command.attack.cs:302` | XXX | XXX COMの攻撃と同じところは切り出しておきたい。。。 | SRC#で不要 | 作り直しの希望で、動作の違いではない（VB も COM と同じ処理を重ねて書いている） |
| `SRCCore/Commands/Command.attack.cs:322`（3 行） | コード | { | SRC#で不要 | NewGUIMode の ClearUnitStatus。SRC# は新 GUI を持たない（今は 321 行目から） |
| `SRCCore/Commands/Command.unitmove.cs:29`（5 行） | コード | { | 直した | 大型マップの判定を戻した |
| `SRCCore/Commands/Command.unitmove.cs:177`（5 行） | コード | { | SRC#で不要 | NewGUIMode（今は 28 行目から） |
| `SRCCore/Commands/Command.unitmove.cs:335`（5 行） | コード | { | SRC#で不要 | NewGUIMode（今は 176 行目から） |
| `SRCCore/Config/LocalFileConfig.cs:10` | TODO | TODO 項目の説明を設定ファイルに書けるようにする | 未確認 |  |
| `SRCCore/Config/LocalFileConfig.cs:11` | XXX | XXX 説明をどこでエントリーするか？　別にJSONでなくてもいい。 | 未確認 |  |
| `SRCCore/Config/LocalFileConfig.cs:30` | XXX | XXX Configの余地ないしConfigのプロパティではないかもな。 | 未確認 |  |
| `SRCCore/Events/Event.data.cs:69` | XXX | XXX VB はシステム側のイベントラベルを消さずに残す（Enable も残る）ので、 | Issue | #1529 |
| `SRCCore/Expressions/VarData.cs:111` | XXX | XXX Undefind 時の参照、Numeric？ | 未確認 |  |
| `SRCCore/Expressions/VarData.cs:126` | XXX | XXX 逆の側の型に値入れなくていいの？ | 未確認 | VB の VarData は渡した値だけを持つが、C# の SetValue はもう一方の型の値も埋める。GetVariable の読み出しには表れないが、フィールドを直接読む処理（Sort など）では違いうる。データの項目（VarData.cls）で判断する（#1498 の Expression.bas の突き合わせで見つけた） |
| `SRCCore/Extensions/ListExtension.cs:11` | XXX | XXX EnumerableExtension | 未確認 |  |
| `SRCCore/Extensions/ListExtension.cs:12` | XXX | XXX 多分遅い | 未確認 |  |
| `SRCCore/Extensions/StreamExtension.cs:42` | XXX | XXX あまりいい同期化ではないはず | 未確認 |  |
| `SRCCore/Filesystem/LocalFilesystem.cs:33` | XXX | XXX NormalizePath で先頭 `\` を処理する？ | 未確認 |  |
| `SRCCore/Filesystem/LocalFilesystem.cs:76` | XXX | XXX Log | 未確認 |  |
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
| `SRCCore/Maps/Map.cs:37` | XXX | XXX シリアライズから復元するのか保存用データクラス作るのかはっきりしたほうがよさそう | SRC#で不要 | JSON の Map から復元する形で動いている。RestoreMapData と突き合わせ済み |
| `SRCCore/Maps/Map.cs:39`（11 行） | コード | FileSystem.Input(SRC.SaveDataFileNumber, sbuf1); | SRC#で不要 | VB のセーブファイルの読み込み。JSON に置き換えた |
| `SRCCore/Maps/Map.cs:127` | XXX | XXX | SRC#で不要 | IsStatusView は VB の `MapFileName = ""` の判定をまとめたもの |
| `SRCCore/Maps/Map.cs:156` | コード | public Src2DArray<MapImageFileType> MapImageFileTypeData; | SRC#で不要 | 格納形式の代わりに MapUnderImageFilePath・MapUpperImageFilePath にパスを残す |
| `SRCCore/Maps/Map.cs:481` | XXX | XXX MapのGUIインタフェースと実装に追い出す | SRC#で不要 | GUIMap.SetMapSize に移してある |
| `SRCCore/Maps/Map.cs:482`（2 行） | コード | GUI.MapPWidth = (32 * w); | SRC#で不要 | 同上 |
| `SRCCore/Maps/Map.cs:493` | XXX | XXX 1というよりは0にしたいが0長配列は作れない | SRC#で不要 | VB の ClearMap も MapWidth・MapHeight を 1 にする |
| `SRCCore/Models/BattleConfigDataList.cs:91` | XXX | XXX これでいいんか？　nullになったり上書きされそう。 | 未確認 |  |
| `SRCCore/Models/Condition.cs:14` | XXX | XXX 負数は無期限（多分） | 未確認 |  |
| `SRCCore/Models/Dialog.cs:193`（13 行） | コード | { | 未確認 |  |
| `SRCCore/Models/DialogData.cs:47`（13 行） | コード | { | 未確認 |  |
| `SRCCore/Models/DialogData.cs:67`（585 行） | コード | Dialog SelectDialogRet = default; | 未確認 |  |
| `SRCCore/Models/MessageData.cs:30` | XXX | XXX MessageDataItem#IsAvailable 常に真でいいか | 未確認 |  |
| `SRCCore/Models/NonPilotData.cs:38`（58 行） | コード | { | 未確認 |  |
| `SRCCore/Models/PilotData.cs:495` | XXX | XXX Readonlyにするとか | SRC#で不要 | SRC# で足した DefinedSpecialPowers の設計のメモ。VB に当たるコードは無い |
| `SRCCore/Models/PilotData.cs:688` | XXX | XXX 例外処理要らんの？ | SRC#で不要 | VB の Feature にもエラーの処理は無く、無ければ実行時エラーになる |
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
| `SRCCore/Pilots/Pilot.cs:74` | XXX | XXX こうしてるところシリアライズの時だけID解決のほうがいいかな。 | SRC#で不要 | Unit を UnitId から引くという SRC# の保存の設計についての覚え書き。VB の動作とは関係しない |
| `SRCCore/Pilots/Pilot.unit.cs:135` | XXX | XXX 外から操作したいものではない | SRC#で不要 | 作りについてのメモ。VB の `Set MapDataForUnit(.X, .Y) = Nothing` と同じ動作 |
| `SRCCore/Pilots/Pilots.cs:210` | XXX | XXX Clone List | SRC#で不要 | C# は列挙中にコレクションを変えられないので、写してから回す。VB と同じく全員を見る |
| `SRCCore/Pilots/Pilots.cs:274`（307 行） | コード | { | SRC#で不要 | Pilots.cls の Save〜Clear。セーブデータは JSON（SRC.save.cs）で、読み込んだ後の状態は対応表の Save〜Clear の行に書いた |
| `SRCCore/SRC.cs:244` | XXX | XXX 別な実装をするならコンストラクタでは設定しない | 未確認 |  |
| `SRCCore/SRC.main.cs:25`（9 行） | コード | string fname; | SRC#で不要 | VB の Main の 2 重起動の禁止。SRC# ではホスト（GUI）が受け持つ（SRC_bas.md の Main の行） |
| `SRCCore/SRC.main.cs:43`（22 行） | コード | bool localFileExists() { string argfname = AppPath + "Src.ini"; var ret = FileSystem.FileExists(argfname); return ret; } | SRC#で不要 | Src.ini の作成・乱数の初期化・時間解像度・フルスクリーン・マウスカーソル。設定は JSON（LocalFileConfig）で、画面は GUI が受け持つ。乱数は SRC が持つ Random で初期化する |
| `SRCCore/SRC.main.cs:212`（173 行） | コード | { | SRC#で不要 | Src.ini の LastFolder への書き込み。最後に開いたフォルダはホスト（GUI）が覚える |
| `SRCCore/SRC.main.cs:426`（212 行） | コード | buf = GeneralLib.ReadIni("Option", "SpecialPowerAnimation"); | SRC#で不要 | Src.ini の [Option] の読み込み。設定は JSON（LocalFileConfig・ComposedConfig）が持つ |
| `SRCCore/SRC.main.cs:643`（101 行） | コード | buf = GeneralLib.ReadIni("Option", "ImageBufferNum"); | SRC#で不要 | Src.ini の画像バッファ・マウスボタンなどの読み込み。設定は JSON で、描画は GUI が受け持つ |
| `SRCCore/SRC.main.cs:842` | コード | { | SRC#で不要 | Src.ini の LastFolder への書き込み。最後に開いたフォルダはホスト（GUI）が覚える |
| `SRCCore/SRC.main.cs:846`（128 行） | コード | i = 0; | SRC#で不要 | ファイルをダイアログで選ぶ分岐。SRC# はホストがファイルを渡す |
| `SRCCore/SRC.main.cs:975`（4 行） | コード | } | SRC#で不要 | ファイルをダイアログで選ぶ分岐の終わり |
| `SRCCore/SRC.main.cs:995`（2 行） | コード | ExtDataPath = GeneralLib.ReadIni("Option", "ExtDataPath"); | SRC#で不要 | ExtDataPath・ExtDataPath2 は JSON の設定（LoadScenarioConfig）から取る |
| `SRCCore/SRC.main.cs:1005`（2 行） | コード | GUI.ChangeStatus(GuiStatus.Default); | SRC#で不要 | マウスカーソルは GUI が受け持つ |
| `SRCCore/SRC.main.cs:1017`（4 行） | コード | { | SRC#で不要 | Src.ini の LastFolder への書き込み。最後に開いたフォルダはホスト（GUI）が覚える |
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
| `SRCCore/Units/Unit.ability.cs:32` | XXX | XXX 攻撃同様にしたけれど、UnitXxxを用意するならそっちが持ってるべきだったかもしれない。 | SRC#で不要 | メソッドの置き場所の設計のメモで、動作に関係しない |
| `SRCCore/Units/Unit.condition.cs:81` | コード | return colCondition[Index].Name; | SRC#で不要 | Condition は Condition オブジェクトを返すように変え、呼び出し側が .Name を読む |
| `SRCCore/Units/Unit.condition.cs:105`（7 行） | コード | { | SRC#で不要 | SetConditionLevel。呼び出し元で `Condition(name).Level` に直接代入している |
| `SRCCore/Units/Unit.feature.cs:18` | コード | For i = .AdditionalFeaturesNum + 1 To .CountAllFeature / fname = .AllFeatureName(i)) | SRC#で不要 | StatusFeatures の説明として VB の参照元（Status.bas）を書いたコメント。コメントアウトしたコードではない |
| `SRCCore/Units/Unit.item.cs:153` | コード | If Not itm2 Is Nothing の分岐があるが、VB の For Each は最後まで回ると | 未確認 |  |
| `SRCCore/Units/Unit.lookup.cs:1857` | XXX | XXX もうちょいいい感じにしたいもんだ | SRC#で不要 | 書き方についての感想で、動作は VB と同じ |
| `SRCCore/Units/Unit.pilot.cs:335` | TODO | TODO 使用している個所でもともと追加サポートを処理していたか精査 | 未確認 |  |
| `SRCCore/Units/Unit.recover.cs:26` | XXX | XXX ! 条件が複数あった時は？ そもそも修理不可Helpにも実装にもないけれどどういう存在なんだ？ | 未確認 |  |
| `SRCCore/Units/UnitAbility.cs:631` | XXX | XXX 変形技が妥当に定義されていなかった場合 | SRC#で不要 | VB もデータの誤りで実行時エラーになる |
| `SRCCore/Units/UnitWeapon.cs:58` | XXX | XXX アイテムとかでの追加があるとそっちが最強になっちゃうんじゃねぇか？ | 未確認 |  |
| `SRCCore/Units/UnitWeapon.cs:163` | XXX | XXX この辺の参照考えると UnitWeapon として切り出したのイマイチだったかもなぁ。 | SRC#で不要 | 設計の感想で、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:3972` | XXX | XXX この辺の判断切り出せそうな気はする | SRC#で不要 | 処理の切り出しの覚え書きで、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:4292` | XXX | XXX 処理簡便にしておきたい | SRC#で不要 | バリア・フィールド・プロテクションの整理の覚え書きで、動作に関係しない |
| `SRCCore/Units/UnitWeapon.cs:5096`（3 行） | コード | If IsWeaponClassifiedAs(w, "合") Then | SRC#で不要 | VB でもコメントアウトされているコード |
| `SRCCore/Units/Units.cs:736`（298 行） | コード | { | 未確認 |  |
| `SRCCore/VB/Conversions.cs:94` | XXX | XXX DateTime パースの精査 | 未確認 |  |
| `SRCCore/VB/Conversions.cs:99` | XXX | XXX 変換ですらないがパースの隣にあったほうが見通しがいいので。。。 | 未確認 |  |
| `SRCCore/VB/Information.cs:18` | XXX | XXX | 未確認 |  |
| `SRCCore/VB/Information.cs:19` | コード | Convert.ToDecimal(str); | 未確認 |  |
| `SRCCore/VB/SrcArray.cs:14` | XXX | XXX 他のIndexアクセスするやつ | 未確認 |  |
| `SRCCore/VB/SrcCollection.cs:143` | XXX | XXX 本当にこんな処理になるのか？ | 未確認 |  |
| `SRCCore/VB/Strings.cs:320` | コード | For dakuten/handakuten marks and other special cases, return as-is | 未確認 |  |
| `SRCCore/VB/Strings.cs:338` | コード | For special cases, return as-is | 未確認 |  |
