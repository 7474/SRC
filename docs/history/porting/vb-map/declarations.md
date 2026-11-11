# モジュールの宣言の突き合わせ

元の SRC（`SRC/SRC_20121125`）の手続きの外にある宣言（`Const`・`Type`・`Enum`・モジュールの変数）を、C# と突き合わせた結果です（7474/SRC#1498 の最後の項目）。手続きの対応表（[README](README.md)）には載らないので、ここにまとめます。

- 対象: `*.bas`・`*.cls`・`*.frm`（`.frm` は `Attribute VB_Name` より後）の、Const 118・Type 19・Enum 8・変数 602。`Declare`（Win32 API）は対象外。
- 状態の書き方は対応表と同じ（一致・修正済み・違いあり・SRC#で不要・C#に無い）。
- 型の幅: VB の `Integer`（16 ビット）・`Long`（32 ビット）を C# は `int` にしている。値の範囲に収まるものは「一致」とした。
- 文字列の初期値: VB の String は `""` で始まる。C# で `null` で始まるものは、使う箇所が `""` と同じに扱うことを確かめて「一致」とした。そうでなかったもの（システム変数の「フェイズ」「選択」「対象ユニット使用スペシャルパワー」に出ていたもの）は `""` に直した。

## 標準モジュール（GUI・描画以外）と CmdData.cls・LabelData.cls

### Command.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `MoveCmdID = 0` 〜 `OrderCmdID = 21`（22 個） | `SRCCore/Commands/Command.define.cs` Command.MoveCmdID 〜 OrderCmdID | 一致 | 値はすべて同じ |
| Const | `FeatureListCmdID = 22` | Command.FeatureListCmdID（23） | SRC#で不要 | VB ではメニューの部品の配列の添字。C# は UiCommand の識別子なので、値に意味はない。C# は換装を ExchangeFormCmdID = 22 に分けている（VB は OrderCmdID の項目の Caption を「換装」に変える） |
| Const | `WeaponListCmdID = 23` | Command.WeaponListCmdID（24） | SRC#で不要 | 同上 |
| Const | `AbilityListCmdID = 24` | Command.AbilityListCmdID（25） | SRC#で不要 | 同上 |
| Const | `UnitCommand1CmdID = 25` 〜 `UnitCommand10CmdID = 34` | Command.UnitCommandCmdID（127）と MaxUserCommandCount = 10 | SRC#で不要 | 1 つの ID にまとめ、ラベルは UiCommand に持たせる。10 個で打ち切る処理は Command.process.cs の AddUserUnitCommand にある |
| Const | `WaitCmdID = 35` | Command.WaitCmdID（255） | SRC#で不要 | メニューの識別子 |
| Const | `EndTurnCmdID = 0` 〜 `OperationObjectCmdID = 5` | Command.EndTurnCmdID 〜 OperationObjectCmdID | 一致 | |
| Const | `MapCommand1CmdID = 6` 〜 `MapCommand10CmdID = 15` | Command.MapCommandCmdID（6）と MaxUserCommandCount | SRC#で不要 | 1 つの ID にまとめた。10 個で打ち切る処理はある |
| Const | `AutoDefenseCmdID = 16` 〜 `QuickSaveCmdID = 20` | Command.AutoDefenseCmdID 〜 QuickSaveCmdID | 一致 | C# だけにある AutoPlayCmdID = 21 は SRC# で足したコマンド |
| 変数 | `CommandState As String`、`WaitClickMode`・`ViewMode As Boolean` | `SRCCore/Commands/Command.props.cs` Command.CommandState など | 一致 | CommandState は null で始まるが、使う箇所は `?? ""` で扱っている |
| 変数 | `MapCommandLabelList(10)`・`UnitCommandLabelList(10) As String` | （無し） | SRC#で不要 | ラベル（LabelData）を UiCommand に持たせる |
| 変数 | `SelectedUnit` 〜 `SelectedUnitMoveCost`（17 個） | `SRCCore/Commands/SelectedState.cs` SelectedState（Command のプロパティから参照） | 修正済み | 文字列（SelectedCommand・SelectedWeaponName・SelectedTWeaponName・SelectedDefenseOption・SelectedAbilityName・SelectedSpecialPower）が null で始まり、「対象ユニット使用スペシャルパワー」が null を返していたので、"" にした。SelectedPartners は List（VB は動的配列） |
| 変数 | `SelectionStackIndex`・`SavedSelected*()`（14 個） | Command.SavedStateStack（`Stack<SelectedState>`） | 一致 | スタックにまとめた。戻すのは VB が記録する項目だけ（Command.state.cs） |
| 変数 | `UseSupportAttack`・`UseSupportGuard`・`WithDoubleSPConsumption As Boolean` | Command の同名の変数 | 一致 | |
| 変数 | `AttackUnit`・`SupportAttackUnit`・`SupportGuardUnit`・`SupportGuardUnit2 As Unit`、`SupportGuardUnitHPRatio`・`SupportGuardUnitHPRatio2 As Double` | Command の同名の変数 | 一致 | |
| 変数 | `PrevUnitX`・`PrevUnitY`・`PrevUnitEN As Integer`、`PrevUnitArea`・`PrevCommand As String` | Command の同名の変数（private） | 一致 | |
| 変数 | `MovedUnit As Unit`、`MovedUnitSpeed As Integer` | Command の同名の変数 | 一致 | |

### SRC.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `DEFAULT_LEVEL = -1000` | `SRCCore/Constants.cs` Constants.DEFAULT_LEVEL | 一致 | |
| 変数 | `PDList` 〜 `BCList`、`PList`・`UList`・`IList`（As New。16 個） | `SRCCore/SRC.cs` SRC.PDList など | 一致 | SRC のコンストラクタで作る |
| 変数 | `ScenarioFileName`・`ScenarioPath As String` | SRC.ScenarioFileName・ScenarioPath | 一致 | "" で始まる |
| 変数 | `SaveDataFileNumber As Integer`、`SaveDataVersion As Long` | SRC.SaveDataFileNumber・SaveDataVersion（int） | 一致 | |
| 変数 | `IsScenarioFinished`・`IsSubStage`・`IsCanceled`・`IsLocalDataLoaded`・`IsRestartSaveDataAvailable`・`IsQuickSaveDataAvailable As Boolean` | SRC の同名の変数 | 一致 | |
| 変数 | `Stage As String` | SRC.Stage | 修正済み | null で始まり、システム変数「フェイズ」が null を返していたので "" にした |
| 変数 | `Turn As Integer`、`TotalTurn`・`Money As Long` | SRC.Turn・TotalTurn・Money（int） | 一致 | Turn が 32767 を超えるような使い方はしない |
| 変数 | `Titles() As String` | SRC.Titles（`IList<string>`） | 一致 | null で始まるが、ClearEventData の前には使わない |
| 変数 | `LastSaveDataFileName As String` | SRC.LastSaveDataFileName | 修正済み | "" で始まるようにした（StartScenario でも "" にする） |
| 変数 | `ShowSquareLine` 〜 `KeepStretchedImage`、`AppPath`（設定の値） | SRC.ShowSquareLine など（Config から読むプロパティ） | 一致 | Src.ini の代わりに設定（ISystemConfig）から読む |
| 変数 | `MidiResetType As String`、`UseTransparentBlt As Boolean` | （無し） | SRC#で不要 | MIDI のリセット・TransparentBlt は使わない（MidiResetType は 7474/SRC#483 で廃止した） |

### Event.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `MaxCallDepth = 50` | `SRCCore/Events/Event.props.cs` Event.MaxCallDepth | 一致 | |
| Const | `MaxArgIndex = 200` | Event.MaxArgIndex | 一致 | |
| Const | `MaxVarIndex = 2000` | Event.MaxVarIndex | 一致 | |
| Type | `Type HotPoint` | `SRCCore/Events/HotPoint.cs` HotPoint | 一致 | 項目の名前・並びが同じ（width → Width）。Integer → int |
| Enum | `Enum CmdType` | `SRCCore/Events/CmdType.cs` CmdType | 一致 | 190 項目の名前と並びを機械で比べた |
| Enum | `Enum LabelType` | `SRCCore/Events/LabelType.cs` LabelType | 一致 | 27 項目 |
| 変数 | `EventData()`・`EventFileID()`・`EventLineNum() ` | Event.EventData（`IList<EventDataLine>`） | 一致 | 1 行に 3 つをまとめた |
| 変数 | `EventCmd() As CmdData`、`EventFileNames()`・`AdditionalEventFileNames() As String` | Event.EventCmd・EventFileNames・AdditionalEventFileNames | 一致 | |
| 変数 | `SysEventDataSize As Long`、`SysEventFileNum As Integer`、`ScenarioLibChecked As Boolean` | Event の同名の変数（private） | 一致 | |
| 変数 | `colEventLabelList`・`colSysNormalLabelList`・`colNormalLabelList`・`GlobalVariableList`・`LocalVariableList As New Collection` | Event の同名の SrcCollection | 一致 | |
| 変数 | `CurrentLineNum As Long` | Event.CurrentLineNum | 一致 | |
| 変数 | `SelectedUnitForEvent`・`SelectedTargetForEvent As Unit` | Event のプロパティ（SelectedState に持つ） | 一致 | |
| 変数 | `EventQue() As String` | Event.EventQue | 一致 | |
| 変数 | `CurrentLabel As Long` | Event.CurrentLabel（-1 で始まる） | 一致 | C# は行番号が 0 から始まるので、VB の 0（ラベル無し）を -1 にしている（既存の判断） |
| 変数 | `SelectedAlternative As String` | Event.SelectedAlternative | 修正済み | "" で始まるようにした |
| 変数 | `CallDepth`・`ArgIndex`・`UpVarLevel`・`VarIndex`・`ForIndex As Integer` と、`CallStack(MaxCallDepth)` 〜 `ForLimitStack(MaxCallDepth)`・`ArgStack(MaxArgIndex)`・`VarStack(MaxVarIndex)` | Event の同名の変数とリスト | 一致 | 要素の数（0〜上限）が同じ。Long → int |
| 変数 | `ForEachIndex As Integer`、`ForEachSet() As String` | Event.ForEachIndex・ForEachSet | 一致 | |
| 変数 | `LastUnitName As String`、`LastPilotID() As String` | Event.LastUnitName・LastPilotID | 一致 | |
| 変数 | `WaitStartTime`・`WaitTimeCount As Long` | Event.WaitStartTime・WaitTimeCount（int） | 一致 | |
| 変数 | `BaseX`・`BaseY`・`BasePointIndex As Long`、`SavedBaseX(10)`・`SavedBaseY(10)` | Event の同名の変数（配列は 11 個） | 一致 | |
| 変数 | `ObjColor`・`ObjDrawWidth`・`ObjFillColor`・`ObjFillStyle As Long`、`ObjDrawOption As String` | Event の同名の変数（Color・FillStyle） | 一致 | LoadEventData・HandleEvent・ClearEventData で白・1・透明・"" に戻すのは C# も同じ |
| 変数 | `HotPointList() As HotPoint` | Event.HotPointList | 一致 | |
| 変数 | `EventErrorMessage As String` | （無し） | SRC#で不要 | エラーは EventErrorException の Message で渡す |
| 変数 | `IsUnitCenter As Boolean` | Event.IsUnitCenter | 一致 | |

### Expression.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Enum | `Enum OperatorType` | `SRCCore/Expressions/OperatorType.cs` OperatorType | 一致 | 18 項目 |
| Enum | `Enum ValueType` | `SRCCore/Expressions/ValueType.cs` ValueType | 一致 | |
| 変数 | `RegEx`・`Matches As Object` | `SRCCore/Expressions/Functions/Regex.cs` RegExp.lastMatch | 一致 | VBScript.RegExp の代わりに .NET の Regex（ECMAScript）を使う。結果は関数のインスタンスに持つ |

### Map.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `MAX_TERRAIN_DATA_NUM = 2000` | `SRCCore/Maps/Map.cs` Map.MAX_TERRAIN_DATA_NUM | 一致 | |
| Const | `NO_LAYER_NUM = 10000` | `SRCCore/Maps/MapCell.cs` MapCell.NO_LAYER_NUM | 一致 | |
| Enum | `Enum MapDataIndex` | `SRCCore/Maps/MapCell.cs` MapCell のプロパティ | SRC#で不要 | MapData の 3 次元目の添字を MapCell のプロパティ（TerrainType 〜 BoxType）にした |
| Enum | `Enum BoxTypes` | `SRCCore/Maps/Enums.cs` BoxTypes | 一致 | |
| Enum | `Enum MapImageFileType` | `SRCCore/Maps/Enums.cs` MapImageFileType | 一致 | |
| 変数 | `MapFileName`・`MapDrawMode As String` | Map.MapFileName・MapDrawMode | 一致 | null で始まるが、使う箇所は `?? ""`・IsNullOrEmpty・Len で扱っている。セーブデータの JSON に関わるので変えていない |
| 変数 | `MapWidth`・`MapHeight As Integer`、`MapDrawFilterColor As Long`、`MapDrawFilterTransPercent As Double`、`MapDrawIsMapOnly`・`IsMapDirty As Boolean` | Map の同名の変数 | 一致 | MapDrawFilterColor は Color.Black（VB の 0） |
| 変数 | `MapData() As Integer` | Map.MapData（`Src2DArray<MapCell>`） | 一致 | |
| 変数 | `MapImageFileTypeData() As MapImageFileType` | （無し） | SRC#で不要 | 必要なとき（Info 関数）に画像のパスから求める（Info.cs MapImageFileTypeOf） |
| 変数 | `MapDataForUnit() As Unit`、`MaskData() As Boolean`、`TotalMoveCost()`・`PointInZOC() As Long` | Map の同名の変数 | 一致 | Long → int |

### Status.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | `DisplayedUnit As Unit`、`DisplayedPilotInd As Integer` | `SRCRender/ScreenGUI.status.cs` ScreenGUI.DisplayedUnit・DisplayedPilotInd | 一致 | |
| 変数 | `IsStatusWindowDisabled As Boolean` | ScreenGUI.IsStatusWindowDisabled（static） | 一致 | |
| 変数 | `StatusWindowBackBolor` 〜 `StatusFontColorNormalString As Long`（7 個） | ScreenGUI の同名の変数 | 一致 | 新ＧＵＩのときだけ使う設定。7474/SRC#1520 で扱い済み |

### Effect.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | `WeaponInHand`・`CurrentWeaponType As String` | `SRCCore/Effect.cs` Effect.WeaponInHand・CurrentWeaponType | 一致 | null で始まるが、使う箇所は `?? ""`・InStr で扱っている |

### Sound.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `SND_SYNC = &H0` 〜 `SND_NOSTOP = &H10`（6 個） | （無し） | SRC#で不要 | Win32 の sndPlaySound の引数 |
| 変数 | `BGMFileName As String`、`RepeatMode`・`KeepBGM`・`BossBGM`・`IsWavePlayed`・`IsMidiSearchPathInitialized As Boolean` | `SRCCore/Sound.cs` Sound の同名の変数 | 一致 | BGMFileName は null で始まるが、Len・`?? ""` で扱っている |
| 変数 | `UseMCI`・`UseDirectMusic`・`UseDirectSound`・`IsMP3Supported As Boolean`、`MP3Volume As Integer` | （無し） | SRC#で不要 | 再生は IPlaySound が受け持つ。音量は Player.SoundVolume |
| 変数 | `DXObject`・`DMLoader`・`DMPerformance`・`DMSegment`・`DSObject`・`DSBuffer` | （無し） | SRC#で不要 | DirectX の部品 |

### BCVariable.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | `IsConfig As Boolean`、`MeUnit`・`AtkUnit`・`DefUnit As Unit`、`WeaponNumber As Integer` | `SRCCore/Events/BCVariable.cs` BCVariable の同名の変数 | 一致 | |
| 変数 | `AttackExp`・`AttackVariable`・`DffenceVariable`・`LastVariable`・`WeaponPower`・`Armor`・`CommonEnemy As Long`、`TerrainAdaption`・`SizeMod As Double` | BCVariable の同名の変数（int・double） | 一致 | C# はコンストラクタで DataReset を呼び TerrainAdaption・SizeMod が 1 で始まる（VB は 0）。ただし VB も Calculate の前に必ず DataReset を呼び、値を読むのは IsConfig のときだけなので見え方は同じ |

### GeneralLib.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Type | `Type OSVERSIONINFO` | （無し） | SRC#で不要 | GetVersionEx 用 |
| Const | `VER_PLATFORM_WIN32_NT = 2` | （無し） | SRC#で不要 | 同上 |
| 変数 | `RndSeed As Long`、`RndIndex As Integer` | `SRCCore/Lib/GeneralLib.cs` GeneralLib.RndSeed・RndIndex（static int） | 一致 | |
| 変数 | `RndHistory(4096) As Single` | GeneralLib.RndHistory（double[4097]） | 一致 | 要素の数は同じ。Single と double で違う。ただし乱数の系列はもともと VB と違う（.NET の Random を使う。記録済み）。VB は Single なので、最大値が大きいと `Int(max * r + 1)` が max + 1 になることがある（VB の不具合）。C# では起きない |

### FileDialog.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Type | `Type OPENFILENAME` | （無し） | SRC#で不要 | Win32 のファイルを選ぶ画面。GUI の側で実装する |

### CmdData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | `CmdName As CmdType` | `SRCCore/CmdDatas/CmdData.cs` CmdData.Name | 一致 | コマンドごとのクラスにしたので読み取り専用 |
| 変数 | `ArgNum As Integer` | CmdData.ArgNum | 一致 | |
| 変数 | `LineNum As Long` | CmdData.EventData（EventDataLine） | 一致 | 行の情報をまとめた |
| 変数 | `lngArgs()`・`dblArgs()`・`strArgs()`・`ArgsType()` | `SRCCore/CmdDatas/CmdArgument.cs` CmdArgument.lngArg・dblArg・strArg・argType | 一致 | 引数ごとのオブジェクトにまとめた |

### LabelData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | `Name As LabelType`、`Enable As Boolean`、`AsterNum As Integer` | `SRCCore/Events/LabelData.cs` LabelData の同名の変数 | 一致 | |
| 変数 | `LineNum As Long` | LabelData.EventDataId | 一致 | C# の行番号（0 から） |
| 変数 | `StrData`、`intParaNum`、`strParas()`、`blnConst()` | LabelData の同名の変数（private） | 一致 | |

## GUI・描画・フォーム

### GUI.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `Const GauageWidth = 88` | `SRCRender/ScreenGUI.status.cs` ScreenGUI.DrawStatusGauge の局所定数 gauageWidth・`SRCSharpForm/Forms/Main.guistatus.cs` も同じ | 一致 | 88。Status.bas の Line の座標と同じ式で描いている |
| Const | `BLACKNESS`〜`WHITENESS`（ラスタ演算 15 個。BLACKNESS=&H42、SRCCOPY=&HCC0020 など） | （無し） | SRC#で不要 | BitBlt・StretchBlt（Win32 API）に渡す値。SRC# は SkiaSharp・GDI+ で描くので使わない |
| Const | `Const STATUSBACK = &HC0C0C0` | （無し） | SRC#で不要 | GUI.bas の新ＧＵＩ（NewGUIMode）のときだけ使う。SRC# は新ＧＵＩに対応しない |
| Const | `STRETCH_ANDSCANS = 1`〜`STRETCH_HALFTONE = 4`（4 個） | （無し） | SRC#で不要 | SetStretchBltMode（Win32 API）の引数 |
| Const | `Const SW_SHOWNA = 8` | （無し） | SRC#で不要 | ShowWindow（Win32 API）の引数 |
| Const | `Const SM_SWAPBUTTON = 23` | （無し） | SRC#で不要 | GetSystemMetrics で左右のボタンの入れ替えを調べるため。SRC# は各フレームワークが論理ボタンで通知する |
| Const | `Const ENUM_CURRENT_SETTINGS = -1` | （無し） | SRC#で不要 | EnumDisplaySettings（画面の色数の確認）用 |
| Const | `CDS_UPDATEREGISTRY = &H1`・`CDS_TEST = &H2`・`CDS_FULLSCREEN = &H4` | （無し） | SRC#で不要 | ChangeDisplaySettings（画面の解像度の変更）用 |
| Const | `DISP_CHANGE_SUCCESSFUL = 0`・`DISP_CHANGE_RESTART = 1` | （無し） | SRC#で不要 | ChangeDisplaySettings の戻り値 |
| Const | `Private Const BITSPIXEL = 12` | （無し） | SRC#で不要 | GetDeviceCaps の引数 |
| Const | `SPI_GETFONTSMOOTHING = 74`・`SPI_SETFONTSMOOTHING = 75` | （無し） | SRC#で不要 | SystemParametersInfo（フォントのスムージングの切り替え）用 |
| Const | `SPIF_UPDATEINIFILE = &H1`・`SPIF_SENDWININICHANGE = &H2` | （無し） | SRC#で不要 | 同上 |
| Type | `Type POINTAPI`（X・Y As Long） | （無し） | SRC#で不要 | GetCursorPos・MoveToEx・Polygon（Win32 API）用。C# は PointF などを使う |
| Type | `Type DEVMODE` | （無し） | SRC#で不要 | EnumDisplaySettings・ChangeDisplaySettings（Win32 API）用 |
| 変数 | `MainForm As Form` | `SRCSharpForm/SRCSharpFormGUI.cs` SRCSharpFormGUI.MainForm | 一致 | SRCRender は ScreenGUI 自身が画面を持つ |
| 変数 | `IsFlashAvailable As Boolean` | （無し） | SRC#で不要 | Flash（FlashControl.ocx）は使わない |
| 変数 | `NewGUIMode As Boolean` | （無し） | SRC#で不要 | SRC# は新ＧＵＩに対応しない（Command.unitform.cs などにコメントあり） |
| 変数 | `MainWidth`・`MainHeight`・`MainPWidth`・`MainPHeight`・`MapPWidth`・`MapPHeight`（Integer 6 つ） | `SRCCore/UIInterface/IGUI.cs` IGUI.MainWidth 〜 MapPHeight（int） | 一致 | P の付く 4 つは C# ではマス数・マップの大きさから計算する読み取りのプロパティ（SRC#Form の setter は NotSupportedException）。VB でも LoadForms・マップの読み込みで同じ値を入れるだけ |
| 変数 | `ScreenIsMasked`・`ScreenIsSaved`（Boolean） | IGUI.ScreenIsMasked・ScreenIsSaved | 一致 | |
| 変数 | `MapX`・`MapY`（Integer） | IGUI.MapX・MapY（int） | 一致 | SRCRender は初期値 1（VB は 0）。マップの読み込み（Map.cs で `(MainWidth + 1) / 2`）より前には読まれないので動作は同じ |
| 変数 | `PrevMapX`・`PrevMapY`（Integer） | IGUI.PrevMapX・PrevMapY | 一致 | |
| 変数 | `MouseButton As Integer` | IGUI.MouseButton（GuiButton） | 一致 | GuiButton は None=0・Left=1・Right=2 で vbLeftButton・vbRightButton と同じ値。VB は `= 2` で比べるだけ |
| 変数 | `MouseX`・`MouseY`・`PrevMouseX`・`PrevMouseY`（Single） | IGUI.MouseX 〜 PrevMouseY（double） | 一致 | ピクセル座標なので精度の違いで値は変わらない |
| 変数 | `Private PrevCursorX`・`PrevCursorY`・`NewCursorX`・`NewCursorY`（Integer） | `SRCRender/ScreenGUI.animation.cs` ScreenGUI.PrevCursorX 〜 NewCursorY（float）・SRCSharpFormGUI（int） | 一致 | SRCRender は画面の拡大のため小数の座標。0 で「未設定」を表すのは同じ |
| 変数 | `PrevUnitX`・`PrevUnitY`（Integer）・`PrevUnitArea`・`PrevCommand`（String） | IGUI.PrevUnitX 〜 PrevCommand | 一致 | VB でも C# でも使われていない（Command.bas の Private の同名の変数が隠す。C# は Command.props.cs の private のフィールド） |
| 変数 | `IsPictureDrawn`・`IsPictureVisible`・`IsCursorVisible`（Boolean）・`PaintedAreaX1`〜`PaintedAreaY2`（Integer 4 つ） | IGUI の同名のプロパティ | 一致 | |
| 変数 | `BGColor As Long` | IGUI.BGColor（Color）・SRC#Form は DrawPicture の局所変数 | 一致 | VB は DrawPicture・SetupBackground の先頭で毎回 vbWhite を入れてから使うので、局所変数にしても同じ |
| 変数 | `Private PicBufDateCount`・`PicBufDate()` 〜 `PicBufIsMask()`（画像の一時保存 13 個） | （無し） | SRC#で不要 | 画像のキャッシュは `SRCRender/ImageCache.cs`・SRC#Form の ImageBuffer が持つ |
| 変数 | `IsGUILocked As Boolean` | IGUI.IsGUILocked | 一致 | |
| 変数 | `TopItem As Integer` | IGUI.TopItem（int）・ScreenGUI.TopItem・SRCSharpFormGUI.TopItem | 修正済み | SRC#Form は short の TopItem と IGUI の明示的な実装の 2 つに分かれていた。3 の 1 |
| 変数 | `Private DisplayedPilot`・`DisplayMode`（String）・`RightUnit`・`LeftUnit`（Unit）・`RightUnitHPRatio` 〜 `LeftUnitENRatio`（Double 4 つ） | `SRCRender/ScreenGUI.cs` ScreenGUI.messageDisplayedPilot・DisplayMode・RightUnit 〜 LeftUnitENRatio・SRCSharpFormGUI の同名のフィールド | 一致 | |
| 変数 | `MessageWindowIsOut As Boolean` | IGUI.MessageWindowIsOut | 一致 | |
| 変数 | `Private IsMessageFormVisible As Boolean`・`SavedLeftUnit`・`SavedRightUnit` | ScreenGUI._savedMessageFormVisible・_savedLeftUnit・_savedRightUnit | 一致 | SaveMessageFormStatus・KeepMessageFormStatus 用 |
| 変数 | `IsFormClicked`・`IsMordal`・`AutoMessageMode`・`HCentering`・`VCentering`・`PermanentStringMode`・`KeepStringMode`（Boolean） | IGUI の同名のプロパティ | 一致 | |
| 変数 | `MessageWait As Long` | IGUI.MessageWait（int） | 一致 | 最大 10000000 で int に収まる。既定の 700 は SRC.bas の設定の読み込み（担当 A）で入る。ScreenGUI はコンストラクタでも 700 にする |
| 変数 | `ListItemFlag()`・`ListItemComment()`・`ListItemID()` | `SRCCore/UIInterface/IGUI.cs` ListBoxItem.ListItemFlag・ListItemComment・ListItemID | 一致 | 配列を項目のクラスにまとめた（GUI の抽象化による違い） |
| 変数 | `MaxListItem As Integer` | `SRCSharpForm/Forms/MultiSelectListBox.cs` frmMultiSelectListBox.MaxListItem・SRCRender は MultiSelectListBox の引数 | 一致 | |
| 変数 | `RButtonID`・`LButtonID`（Long） | （無し） | SRC#で不要 | GetAsyncKeyState で物理ボタンを調べるときの左右の入れ替え用。SRC# の GetKeyState は各フレームワークの論理ボタンで判定するので不要 |

### Graphics.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Const | `Const SRCCOPY = &HCC0020` | （無し） | SRC#で不要 | BitBlt（Win32 API）用 |
| Const | `Const DIB_RGB_COLORS = 0` | （無し） | SRC#で不要 | GetDIBits・SetDIBits 用 |
| Const | `Const BI_RGB = 0` | （無し） | SRC#で不要 | BITMAPINFOHEADER の圧縮の種類 |
| Type | `Type BITMAPINFOHEADER` | （無し） | SRC#で不要 | DIB（Win32 API）用。画素は SkiaSharp・GDI+ で直接扱う（`SRCRender/ImageFilters.cs`） |
| Type | `Type RGBQUAD` | （無し） | SRC#で不要 | 同上 |
| Type | `Type BITMAPINFO` | （無し） | SRC#で不要 | 同上 |
| Type | `Type Bitmap` | （無し） | SRC#で不要 | GetObject（Win32 API）用 |
| Type | `Type RGBq`（Blue・Green・Red As Byte） | （無し） | SRC#で不要 | 画素のバッファ用。C# は SKColor・Color |
| 変数 | `PixBuf()`・`PixBuf2()`（RGBq）・`PixWidth`・`PixHeight`・`PicWidth`・`PicHeight`（Long） | （無し） | SRC#で不要 | 画像の加工の作業用バッファ。C# はフィルターの関数の中で画像を直接読み書きする |
| 変数 | `BmpInfo()`・`NewDC`・`MemDC`・`OrigPicDC`・`lpBit`（Long） | （無し） | SRC#で不要 | デバイスコンテキスト・DIB のハンドル |
| 変数 | `FadeCMap() As Byte` | （無し） | SRC#で不要 | VB でも宣言だけで使われていない |

### VBMP3.bas
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| Type | `Type InputInfo`・`TAG_INFO`・`MPEG_INFO`・`DEC_OPTION`・`VBMP3_OPTION`・`LIST_INFO`・`LYRICS_INFO`・`WAVE_DATA`・`WAVE_FORM`（9 個。表では 1 行にまとめた） | （無し） | SRC#で不要 | vbmp3.dll に渡す構造体。SRC# は MP3 を SRCSound（NAudio など）で鳴らし、vbmp3.dll を使わない |
| Const | `FT_NOMAL = 0`〜`FT_ID3V1 = 16`（6 個） | （無し） | SRC#で不要 | vbmp3.dll のファイルの種類。VB でも名前では使われていない |
| Const | `MSG_ERROR = 0`〜`MSG_PLAYDONE = 4`（5 個） | （無し） | SRC#で不要 | vbmp3_getState の戻り値。Sound.bas は `= 2` と数値で書いている。C# は再生中かどうかを再生ライブラリに問い合わせる |
| Enum | `Enum vbmp3_errNo`（ERR_MP3_FILE_OPEN = 1 〜 ERR_ACM_OPEN = 32） | （無し） | SRC#で不要 | vbmp3.dll のエラー番号。VB でも使われていない |

### *.frm（Attribute VB_Name より後の変数）
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Configuration.frm `Private SavedMP3Volume As Integer` | `SRCSharpForm/Forms/Configuration.cs` frmConfiguration.SavedMP3Volume（float） | 一致 | 音量を再生ライブラリの 0〜1 の値で持つ（SRC# の音声の抽象化による違い）。キャンセルで戻す動作は同じ |
| 変数 | ListBox.frm `HorizontalSize`・`VerticalSize`（String） | `SRCRender/Compat/ListWindow.cs` ListWindow.HorizontalSize・VerticalSize・`SRCSharpForm/Forms/ListBox.cs` frmListBox の同名のフィールド | 一致 | VB は Form_Load で "M"。C# もコンストラクタ・Load で "M" |
| 変数 | ListBox.frm `CurrentTime`・`TimeLimit`（Integer） | `SRCRender/ScreenGUI.list.cs` ScreenGUI.listCurrentTime・listTimeLimit・frmListBox.CurrentTime・TimeLimit（int） | 一致 | SRCRender は経過時間をミリ秒で持つ（Timer の 100ms × time_limit と同じ長さ） |
| 変数 | ListBox.frm `Private LastSelectedItem As Integer` | ScreenGUI.lastSelectedItem・frmListBox.LastSelectedItem | 一致 | |
| 変数 | Main.frm・SafeMain.frm `Private IsDragging As Boolean` | `SRCRender/ScreenGUI.cs` ScreenGUI.isDragging・`SRCSharpForm/Forms/Main.cs` frmMain.IsDragging | 一致 | SafeMain は Main の代わりの画面で、C# は 1 つにまとめた |
| 変数 | MultiSelectListBox.frm `Private SelectedItemNum As Integer` | `SRCSharpForm/Forms/MultiSelectListBox.cs` SelectedItemNum・SRCRender は選択数を項目から数える | 一致 | |
| 変数 | MultiSelectListBox.frm `Private ItemFlag() As Boolean` | ListBoxItem.ListItemFlag | 一致 | 配列を項目のクラスにまとめた |
| 変数 | NowLoading.frm `Size`・`Value`（Integer） | `SRCSharpForm/Forms/NowLoading.cs` frmNowLoading.Value（ProgressBar の値）・Max | 一致 | 進み具合をプログレスバーに持たせた |
| （確認） | ErrorMessage・MapSize・Message・MultiColumListBox・Telop・Title・ToolTip の各 .frm | ― | ― | Attribute VB_Name より後に変数・Const・Type は無い |

## クラスモジュール

クラスモジュール（*.cls。CmdData.cls・LabelData.cls は標準モジュールの節に書いた）は、どれも Const・Type・Enum は無く、メンバー変数だけだった。手続きの間に宣言を書いたファイルも無かった。

全体に共通する判断は次のとおり。
- **文字列が null のままになる件**: C# には初期値を書いていない string のフィールドが多く（VB は ""）、null になりうる。次のことを確かめたので、まとめて「一致」とした。
  - `Strings.InStr`・`Left`・`Mid`・`Right`・`Len`・`Trim` と `GeneralLib.LIndex`・`LLength`・`ToL`・`ToList`・`ListTail`・`InStrNotNest` は、null を "" として扱う。
  - SRCCore には、これらのフィールドを `== ""`・`!= ""`・`== null` で比べる箇所が無い。`.Length`・`.Split` などで直接呼ぶ箇所は 2 つあるが、どちらも必ず値が入った後に呼ぶ（AliasData の `StrData.Replace`、SpecialPowerData の `Animation.Split`）。
  - Info 関数が null を返しても、式の評価では "" になることを試して確かめた（`Info(u,武器,1,必要技能) = ""` は 1）。画面の `ListItemComment` が null でも、表示は空になる。
- **型の幅**: VB の Integer・Long を C# は int にしている。値の範囲（HP・EN・能力値・気力・ID の数）に収まるので「一致」とした。Single は無い。Double は C# でも double。
- **保存と読み込み**: C# は JSON で、VB の Dump が書く項目はすべて保存している。Unit は Name・ID・Party0・Rank・BossRank・x・y・Area・UsedAction・Mode・Status・Used* と、関連する ID・特殊状態・残弾・使用回数・HP・EN。Pilot は Level・Exp・SP・Morale・Plana・Alive・Away・SupportIndex・Unit。Item は Exist・Unit。C# はほかに Activated なども保存するが、Unit.Update で計算し直すので、結果は VB と同じになる。

### AbilityData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・Class・NecessarySkill・NecessaryCondition As String | `SRCCore/Models/AbilityData.cs` AbilityData.Name ほか | 一致 | Class・NecessarySkill・NecessaryCondition は "" で初期化している。Name は読み込みのときに入る |
| 変数 | Stock・ENConsumption・NecessaryMorale・MinRange・MaxRange As Integer | AbilityData.Stock ほか（int） | 一致 | 初期値は 0 |
| 変数 | colEffects As New Collection | AbilityData.colEffects（SrcCollection） | 一致 | 生成済み |

### AbilityEffect.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String・Level As Double・Data As String | `SRCCore/Models/AbilityEffect.cs` AbilityEffect.Name・Level・Data | 一致 | |

### AliasData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String | `SRCCore/Models/AliasData.cs` AliasDataType.Name | 一致 | |
| 変数 | strAliasType()・dblAliasLevel()・blnAliasLevelIsPlusMod()・blnAliasLevelIsMultMod()・strAliasData()・strAliasNecessarySkill()・strAliasNecessaryCondition()（Class_Initialize で ReDim (0)） | AliasDataType.Elements（AliasDataElement のリスト） | 一致 | 並んだ配列を要素のクラスにまとめた（1 から数える番号は Elements の添字 + 1）。NecessarySkill・NecessaryCondition の null は、IsNecessarySkillSatisfied の IsNullOrEmpty で "" と同じに扱われる |

### AliasDataList.cls・BattleConfigDataList.cls・DialogDataList.cls・ItemDataList.cls・MessageDataList.cls・SpecialPowerDataList.cls・Pilots.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | colAliasDataList・colBattleConfigData・colDialogDataList・colItemDataList・colMessageDataList・colSpecialPowerDataList・colPilots As New Collection | 各 C# クラスの同名のフィールド（SrcCollection） | 一致 | 生成済み。MessageDataList は `Models/MassageDataList.cs` にある |

### BattleConfigData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String | `SRCCore/Models/BattleConfigData.cs` BattleConfigData.Name | 一致 | |
| 変数 | ConfigCalc As String（Class_Initialize で ""） | BattleConfigData.ConfigCalc = "" | 一致 | テストを足した |

### Condition.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String・Lifetime As Integer・Level As Double・StrData As String | `SRCCore/Models/Condition.cs` Condition の同名のフィールド | 一致 | |

### Dialog.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | intMessageNum（Variant）・strName()・strMessage()（Class_Initialize で 0 と ReDim (0)） | `SRCCore/Models/Dialog.cs` Dialog.dialogs（DialogItem のリスト）・Count | 一致 | 並んだ配列をリストにまとめた |

### DialogData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String | `SRCCore/Models/DialogData.cs` DialogData.Name | 一致 | |
| 変数 | intDialogNum As Integer・strSituation()・Dialoges() As Dialog | DialogData.Dialoges（Dialog のリスト）・Dialog.Situation | 一致 | シチュエーションは Dialog に持たせている |

### FeatureData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・StrData・NecessarySkill・NecessaryCondition As String、Level As Double | `SRCCore/Models/FeatureData.cs` FeatureData の同名のメンバー | 一致 | null のまま参照する箇所は `?? ""` か IsNullOrEmpty で受けている |

### Item.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | ID As String | `SRCCore/Items/Item.cs` Item.ID | 一致 | |
| 変数 | Data As ItemData・Unit As Unit（Nothing） | Item.Data・Unit（名前・ID を ItemName・UnitId に持って引くプロパティ） | 一致 | JSON で保存するため |
| 変数 | Exist・Activated As Boolean（Class_Initialize で True） | Item.Exist・Activated（コンストラクターで true） | 一致 | テストを足した |

### ItemData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・Class・Part・Comment・proNickname・proKanaName As String | `SRCCore/Models/ItemData.cs` ItemData の同名のメンバー | 一致 | Comment は、説明の行が無ければ null のまま。Info では "" になり、画面の表示も空 |
| 変数 | HP・Armor As Long、EN・Mobility・Speed As Integer | ItemData.HP ほか（int） | 一致 | |
| 変数 | colFeature・colWeaponData・colAbilityData As Collection（Nothing） | ItemData.colFeature ほか（コンストラクターで生成） | 一致 | VB は「Nothing なら 0 個」として扱うので、空のコレクションと同じ |

### Items.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | IDCount As Long | `SRCCore/Items/Items.cs` Items.IDCount（int・JSON に保存） | 一致 | VB も Dump・Restore で保存する |
| 変数 | colItems As New Collection | Items.colItems | 一致 | |

### MessageData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String | `SRCCore/Models/MessageData.cs` MessageData.Name | 一致 | |
| 変数 | intMessageNum・strSituation()・strMessage() | MessageData.Items（MessageDataItem のリスト） | 一致 | 並んだ配列をリストにまとめた |

### NonPilotData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・strNickname・proBitmap As String、IsBitmapMissing As Boolean | `SRCCore/Models/NonPilotData.cs` NonPilotData の同名のフィールド | 一致 | |

### NonPilotDataList.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | colNonPilotDataList As New Collection（Class_Initialize でナレーターを追加） | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.colNonPilotDataList とコンストラクター | 一致 | ナレーター（Nickname ナレーター・Bitmap .bmp）。テストを足した |

### Pilot.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Data As PilotData・Unit As Unit | `SRCCore/Pilots/Pilot.cs` Pilot.Data・Unit（PilotName・UnitId から引く） | 一致 | JSON で保存するため |
| 変数 | ID・Party・Adaption As String | Pilot.ID・Party・Adaption | 一致 | Adaption は Update で入る |
| 変数 | Alive・Away・IsAdditionalPilot・IsAdditionalSupport As Boolean | Pilot の同名のフィールド | 一致 | IsAdditionalPilot は、VB が保存しないのに合わせて Restore で false に戻す |
| 変数 | SupportIndex、proLevel 〜 proPlana（Integer 5 つ） | Pilot の同名のフィールド（int・JSON に保存） | 一致 | |
| 変数 | Infight 〜 Intuition・*Base・*Mod・*Mod2・MoraleMod（Integer 25 個） | Pilot の同名のフィールド（int） | 一致 | |
| 変数 | colSkill As New Collection | Pilot.colSkill | 一致 | |

### PilotData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・Sex・Class・Adaption・Personality・BGM・proNickname・proKanaName・proBitmap As String | `SRCCore/Models/PilotData.cs` PilotData の同名のメンバー | 一致 | |
| 変数 | ExpValue・Infight 〜 Technique・SP As Integer（8 つ）、IsBitmapMissing As Boolean | PilotData の同名のフィールド | 一致 | |
| 変数 | SpecialPowerName()・SpecialPowerNecessaryLevel()・SpecialPowerSPConsumption()（Class_Initialize で ReDim (0)） | PilotData.SpecialPowers（PilotDataSpecialPower のリスト） | 一致 | 並んだ配列をリストにまとめた |
| 変数 | colSkill As New Collection、colFeature・colWeaponData・colAbilityData As Collection | PilotData.colSkill ほか（コンストラクターで生成） | 一致 | |

### PilotDataList.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | colPilotDataList As New Collection（Class_Initialize でダミーのパイロットを追加） | `SRCCore/Models/PilotDataList.cs` colPilotDataList・AddDummyData | 一致 | 名前・Nickname・Adaption AAAA・Bitmap .bmp。テストを足した |

### SkillData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String・Level As Double・StrData As String・NecessaryLevel As Integer | `SRCCore/Models/SkillData.cs` SkillData の同名のプロパティ | 一致 | |

### SpecialPowerData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・KanaName・ShortName・TargetType・Duration・NecessaryCondition・Animation・Comment As String、SPConsumption As Integer | `SRCCore/Models/SpecialPowerData.cs` SpecialPowerData の同名のフィールド | 一致 | Animation は読み込みのときに必ず入る |
| 変数 | strEffectType()・dblEffectLevel()・strEffectData()（ReDim (0)） | SpecialPowerData.effects（SpecialPowerEffect のリスト） | 一致 | |

### TerrainData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | ID As Integer（Class_Initialize で -1） | `SRCCore/Models/TerrainData.cs` TerrainData.ID = -1 | 一致 | テストを足した |
| 変数 | Name・Bitmap・Class As String、MoveCost・HitMod・DamageMod As Integer | TerrainData の同名のフィールド | 一致 | |
| 変数 | colFeature As Collection | TerrainData.colFeature（生成済み） | 一致 | |

### TerrainDataList.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Count As Integer | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Count（OrderList.Count） | 一致 | VB にもクラスの外から代入する箇所は無い |
| 変数 | TerrainDataList(0 To MAX_TERRAIN_DATA_NUM) As TerrainData（全部 New） | TerrainDataList.TerrainList（大きさ 2001。登録するまで null） | 一致 | 未登録の番号は、VB では ID -1 の空の TerrainData、C# では null（範囲外は EmptyTerrain）。参照する箇所は Map の読み込みとコマンドだけで、どれも IsDefined か ItemByName を通すので結果は同じ。Clear の扱い（VB は ID を -1 に、C# は null に）も IsDefined の結果は同じ |
| 変数 | OrderList(1 To MAX_TERRAIN_DATA_NUM) As Integer | TerrainDataList.OrderList（List） | 一致 | |

### Unit.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Data As UnitData | `SRCCore/Units/Unit.cs` Unit.Data（UnitName から引く） | 一致 | |
| 変数 | ID・Area・Status・strMode・strName・strParty・strAdaption As String | Unit の同名のフィールド | 一致 | Status はコンストラクターで "待機"（テストを足した） |
| 変数 | BitmapID As Integer | （無し） | SRC#で不要 | SRC# は描画のときに画像を引く（Unit.status.cs にもそう書いてある） |
| 変数 | x・y・UsedAction・UsedSupportAttack・UsedSupportGuard・UsedSyncAttack・UsedCounterAttack・AdditionalFeaturesNum As Integer | Unit の同名のフィールド（int） | 一致 | |
| 変数 | intRank・intBossRank（Class_Initialize で -1）・intMaxEN・intEN・intMobility・intSpeed As Integer、lngMaxHP・lngHP・lngArmor・SelectedMapAttackPower As Long | Unit の同名のフィールド（int） | 一致 | intBossRank はコンストラクターで -1（テストを足した） |
| 変数 | colSpecialPowerInEffect・colPilot・colSupport・colOtherForm・colUnitOnBoard・colItem・colCondition・colFeature・colAllFeature・colServant・colSlave As New Collection | Unit の同名のフィールド（SrcCollection） | 一致 | |
| 変数 | dblBullet()・dblStock() As Double | Unit.dblBullet・dblStock（double[]） | 一致 | |
| 変数 | strAbsorb・strImmune・strResist・strWeakness・strEffective・strSpecialEffectImmune As String | Unit の同名のフィールド | 一致 | Update で "" から作り直す |
| 変数 | WData() As WeaponData・lngWeaponPower()・intWeaponMaxRange()・intWeaponPrecision()・intWeaponCritical()・strWeaponClass()・intMaxBullet()（ReDim WData(0)） | Unit.WData（UnitWeapon のリスト） | 一致 | 武器ごとの値を UnitWeapon にまとめた |
| 変数 | adata() As AbilityData（ReDim (0)） | Unit.AData（UnitAbility のリスト） | 一致 | |
| 変数 | IsMapAttackCanceled As Boolean | Unit.IsMapAttackCanceled | 一致 | |
| 変数 | Summoner・Master As Unit、pltAdditionalPilot・pltAdditionalSupport As Pilot（Nothing） | Unit の同名のフィールド（null） | 一致 | |

### UnitData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・Class・Adaption・Transportation・Size・proNickname・proKanaName・proBitmap As String | `SRCCore/Models/UnitData.cs` UnitData の同名のメンバー | 一致 | |
| 変数 | ID・HP・Armor・Value As Long、PilotNum・ItemNum・EN・Speed・Mobility・ExpValue As Integer、IsBitmapMissing As Boolean | UnitData の同名のフィールド（int・bool） | 一致 | |
| 変数 | colFeature・colWeaponData・colAbilityData As Collection | UnitData.colFeature ほか（コンストラクターで生成） | 一致 | |

### UnitDataList.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | colUnitDataList As New Collection（Class_Initialize でダミーのユニットを追加） | `SRCCore/Models/UnitDataList.cs` colUnitDataList・AddDummyData | 一致 | 項目は VB と同じ。テストを足した |
| 変数 | IDNum As Long | UnitDataList.IDNum（int・0） | 一致 | |

### Units.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | IDCount As Long（Class_Initialize で 0） | `SRCCore/Units/Units.cs` Units.IDCount = 0（JSON に保存） | 一致 | テストを足した |
| 変数 | colUnits As New Collection | Units.colUnits | 一致 | |

### VarData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name As String・VariableType As ValueType・StringValue As String・NumericValue As Double | `SRCCore/Expressions/VarData.cs` VarData の同名のプロパティ | 一致 | ValueType の初期値は VB と同じ 0（UndefinedType） |

### WeaponData.cls
| 種類 | VB の宣言 | C# | 状態 | メモ |
|---|---|---|---|---|
| 変数 | Name・Adaption・Class・NecessarySkill・NecessaryCondition As String | `SRCCore/Models/WeaponData.cs` WeaponData の同名のフィールド | 一致 | 必要技能が無い武器は NecessarySkill が null。`Info(…,武器,n,必要技能)` が "" になることを試して確かめた |
| 変数 | Power As Long、MinRange・MaxRange・Precision・Bullet・ENConsumption・NecessaryMorale・Critical As Integer | WeaponData の同名のフィールド（int） | 一致 | |
