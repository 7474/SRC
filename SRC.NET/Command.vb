Option Strict Off
Option Explicit On
Module Commands
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ユニット＆マップコマンドの実行を行うモジュール
	
	'ユニットコマンドのメニュー番号
	Public Const MoveCmdID As Short = 0
	Public Const TeleportCmdID As Short = 1
	Public Const JumpCmdID As Short = 2
	Public Const TalkCmdID As Short = 3
	Public Const AttackCmdID As Short = 4
	Public Const FixCmdID As Short = 5
	Public Const SupplyCmdID As Short = 6
	Public Const AbilityCmdID As Short = 7
	Public Const ChargeCmdID As Short = 8
	Public Const SpecialPowerCmdID As Short = 9
	Public Const TransformCmdID As Short = 10
	Public Const SplitCmdID As Short = 11
	Public Const CombineCmdID As Short = 12
	Public Const HyperModeCmdID As Short = 13
	Public Const GroundCmdID As Short = 14
	Public Const SkyCmdID As Short = 15
	Public Const UndergroundCmdID As Short = 16
	Public Const WaterCmdID As Short = 17
	Public Const LaunchCmdID As Short = 18
	Public Const ItemCmdID As Short = 19
	Public Const DismissCmdID As Short = 20
	Public Const OrderCmdID As Short = 21
	Public Const FeatureListCmdID As Short = 22
	Public Const WeaponListCmdID As Short = 23
	Public Const AbilityListCmdID As Short = 24
	Public Const UnitCommand1CmdID As Short = 25
	Public Const UnitCommand2CmdID As Short = 26
	Public Const UnitCommand3CmdID As Short = 27
	Public Const UnitCommand4CmdID As Short = 28
	Public Const UnitCommand5CmdID As Short = 29
	Public Const UnitCommand6CmdID As Short = 30
	Public Const UnitCommand7CmdID As Short = 31
	Public Const UnitCommand8CmdID As Short = 32
	Public Const UnitCommand9CmdID As Short = 33
	Public Const UnitCommand10CmdID As Short = 34
	Public Const WaitCmdID As Short = 35
	
	'マップコマンドのメニュー番号
	Public Const EndTurnCmdID As Short = 0
	Public Const DumpCmdID As Short = 1
	Public Const UnitListCmdID As Short = 2
	Public Const SearchSpecialPowerCmdID As Short = 3
	Public Const GlobalMapCmdID As Short = 4
	Public Const OperationObjectCmdID As Short = 5
	Public Const MapCommand1CmdID As Short = 6
	Public Const MapCommand2CmdID As Short = 7
	Public Const MapCommand3CmdID As Short = 8
	Public Const MapCommand4CmdID As Short = 9
	Public Const MapCommand5CmdID As Short = 10
	Public Const MapCommand6CmdID As Short = 11
	Public Const MapCommand7CmdID As Short = 12
	Public Const MapCommand8CmdID As Short = 13
	Public Const MapCommand9CmdID As Short = 14
	Public Const MapCommand10CmdID As Short = 15
	Public Const AutoDefenseCmdID As Short = 16
	Public Const ConfigurationCmdID As Short = 17
	Public Const RestartCmdID As Short = 18
	Public Const QuickLoadCmdID As Short = 19
	Public Const QuickSaveCmdID As Short = 20
	
	'現在のコマンドの進行状況
	Public CommandState As String
	
	'クリック待ちモード
	Public WaitClickMode As Boolean
	'閲覧モード
	Public ViewMode As Boolean
	
	'マップコマンドラベルのリスト
	Private MapCommandLabelList(10) As String
	'ユニットコマンドラベルのリスト
	Private UnitCommandLabelList(10) As String
	
	'現在選択されているもの
	Public SelectedUnit As Unit 'ユニット
	Public SelectedCommand As String 'コマンド
	Public SelectedTarget As Unit 'ターゲット
	Public SelectedX As Short 'Ｘ座標
	Public SelectedY As Short 'Ｙ座標
	Public SelectedWeapon As Short '武器
	Public SelectedWeaponName As String
	Public SelectedTWeapon As Short '反撃武器
	Public SelectedTWeaponName As String
	Public SelectedDefenseOption As String '防御方法
	Public SelectedAbility As Short 'アビリティ
	Public SelectedAbilityName As String
	Public SelectedPilot As Pilot 'パイロット
	Public SelectedItem As Short 'リストボックス中のアイテム
	Public SelectedSpecialPower As String 'スペシャルパワー
	Public SelectedPartners() As Unit '合体技のパートナー
	' ADD START MARGE
	Public SelectedUnitMoveCost As Short '選択したユニットの移動力消費量
	' ADD END MARGE
	
	'選択状況の記録用変数
	Public SelectionStackIndex As Short
	Public SavedSelectedUnit() As Unit
	Public SavedSelectedTarget() As Unit
	Public SavedSelectedUnitForEvent() As Unit
	Public SavedSelectedTargetForEvent() As Unit
	Public SavedSelectedWeapon() As Short
	Public SavedSelectedWeaponName() As String
	Public SavedSelectedTWeapon() As Short
	Public SavedSelectedTWeaponName() As String
	Public SavedSelectedDefenseOption() As String
	Public SavedSelectedAbility() As Short
	Public SavedSelectedAbilityName() As String
	Public SavedSelectedX() As Short
	Public SavedSelectedY() As Short
	
	'援護を使うかどうか
	Public UseSupportAttack As Boolean
	Public UseSupportGuard As Boolean
	
	'「味方スペシャルパワー実行」を使ってスペシャルパワーを使用するかどうか
	Private WithDoubleSPConsumption As Boolean
	
	'攻撃を行うユニット
	Public AttackUnit As Unit
	'援護攻撃を行うユニット
	Public SupportAttackUnit As Unit
	'援護防御を行うユニット
	Public SupportGuardUnit As Unit
	'援護防御を行うユニットのＨＰ値
	Public SupportGuardUnitHPRatio As Double
	'援護防御を行うユニット(反撃時)
	Public SupportGuardUnit2 As Unit
	'援護防御を行うユニットのＨＰ値(反撃時)
	Public SupportGuardUnitHPRatio2 As Double
	
	'移動前のユニットの情報
	Private PrevUnitX As Short
	Private PrevUnitY As Short
	Private PrevUnitArea As String
	Private PrevUnitEN As Short
	Private PrevCommand As String
	
	'移動したユニットの情報
	Public MovedUnit As Unit
	Public MovedUnitSpeed As Short
	
	
	'コマンドの処理を進める
	'by_cancel = True の場合はコマンドをキャンセルした場合の処理
	Public Sub ProceedCommand(Optional ByVal by_cancel As Boolean = False)
		Dim j, i, n As Short
		Dim u As Unit
		Dim uname As String
		Dim p As Pilot
		Dim buf As String
		Dim lab As LabelData
		
		'閲覧モードはキャンセルで終了。それ以外の入力は無視
		If ViewMode Then
			If by_cancel Then
				ViewMode = False
			End If
			Exit Sub
		End If
		
		'処理が行われるまでこれ以降のコマンド受付を禁止
		'(スクロール禁止にしなければならないほどの時間はないため、LockGUIは使わない)
		IsGUILocked = True
		
		'コマンド実行を行うということはシナリオプレイ中ということなので毎回初期化する。
		IsScenarioFinished = False
		IsCanceled = False
		
		'ポップアップメニュー上で押したマウスボタンが左右どちらかを判定するため、
		'あらかじめGetAsyncKeyState()を実行しておく必要がある
		Call GetAsyncKeyState(RButtonID)
		
		Select Case CommandState
			Case "ユニット選択", "マップコマンド"
				'UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SelectedUnit = Nothing
				' ADD START MARGE
				SelectedUnitMoveCost = 0
				' ADD END MARGE
				If 1 <= PixelToMapX(MouseX) And PixelToMapX(MouseX) <= MapWidth And 1 <= PixelToMapY(MouseY) And PixelToMapY(MouseY) <= MapHeight Then
					SelectedUnit = MapDataForUnit(PixelToMapX(MouseX), PixelToMapY(MouseY))
				End If
				If SelectedUnit Is Nothing Then
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					If MapFileName <> "" Then
						'通常のステージ
						
						DisplayGlobalStatus()
						
						'ターン終了
						If ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "部隊編成に戻る"
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "ターン終了"
						End If
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(EndTurnCmdID).Visible = True
						
						'中断
						If IsOptionDefined("デバッグ") Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(DumpCmdID).Visible = True
						Else
							If Not IsOptionDefined("クイックセーブ不可") Then
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(DumpCmdID).Visible = True
							Else
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(DumpCmdID).Visible = False
							End If
						End If
						
						'全体マップ
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(GlobalMapCmdID).Visible = True
						
						'作戦目的
						If IsEventDefined("勝利条件") Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = True
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = False
						End If
						
						'自動反撃モード
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(AutoDefenseCmdID).Visible = True
						
						'設定変更
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(ConfigurationCmdID).Visible = True
						
						'リスタート
						If IsRestartSaveDataAvailable And Not ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(RestartCmdID).Visible = True
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(RestartCmdID).Visible = False
						End If
						
						'クイックロード
						If IsQuickSaveDataAvailable And Not ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = True
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = False
						End If
						
						'クイックセーブ
						If ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = False
						ElseIf IsOptionDefined("デバッグ") Then 
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = True
						Else
							If Not IsOptionDefined("クイックセーブ不可") Then
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = True
							Else
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = False
							End If
						End If
					Else
						'パイロットステータス・ユニットステータスのステージ
						With MainForm
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(EndTurnCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(DumpCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(GlobalMapCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(OperationObjectCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(AutoDefenseCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(ConfigurationCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(RestartCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(QuickLoadCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.mnuMapCommandItem(QuickSaveCmdID).Visible = False
						End With
					End If
					
					'スペシャルパワー検索
					'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = False
					'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Caption = Term("スペシャルパワー") & "検索"
					For	Each p In PList
						With p
							If .Party = "味方" Then
								If .CountSpecialPower > 0 Then
									'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = True
									Exit For
								End If
							End If
						End With
					Next p
					
					'イベントで定義されたマップコマンド
					With MainForm
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand1CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand2CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand3CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand4CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand5CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand6CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand7CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand8CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand9CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuMapCommandItem(MapCommand10CmdID).Visible = False
					End With
					If Not ViewMode Then
						i = MapCommand1CmdID
						For	Each lab In colEventLabelList
							With lab
								If .Name = Event_Renamed.LabelType.MapCommandEventLabel Then
									If .Enable Then
										If .CountPara = 2 Then
											'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuMapCommandItem(i).Visible = True
										ElseIf StrToLng(.Para(3)) <> 0 Then 
											'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuMapCommandItem(i).Visible = True
										End If
									End If
									
									'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									If MainForm.mnuMapCommandItem(i).Visible Then
										'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuMapCommandItem(i).Caption = .Para(2)
										MapCommandLabelList(i - MapCommand1CmdID + 1) = CStr(.LineNum)
										i = i + 1
										If i > MapCommand10CmdID Then
											Exit For
										End If
									End If
								End If
							End With
						Next lab
					End If
					
					CommandState = "マップコマンド"
					
					IsGUILocked = False
					' ADD START 240a
					'ここに来た時点でcancel=Trueはユニットのいないセルを右クリックした場合のみ
					If by_cancel Then
						If NewGUIMode And (Not MapFileName = "") Then
							If MouseX < MainPWidth \ 2 Then
								'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.picUnitStatus.Move(MainPWidth - 240, 10)
							Else
								'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.picUnitStatus.Move(5, 10)
							End If
							'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picUnitStatus.Visible = True
						End If
					End If
					' ADD  END  240a
					'UPGRADE_ISSUE: Control mnuMapCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					MainForm.PopupMenu(MainForm.mnuMapCommand, 6, MouseX, MouseY + 6)
					Exit Sub
				End If
				
				SelectedUnitForEvent = SelectedUnit
				SelectedWeapon = 0
				SelectedTWeapon = 0
				SelectedAbility = 0
				
				If by_cancel Then
					'ユニット上でキャンセルボタンを押した場合は武器一覧
					'もしくはアビリティ一覧を表示する
					With SelectedUnit
						'情報が隠蔽されている場合は表示しない
						If (IsOptionDefined("ユニット情報隠蔽") And (Not .IsConditionSatisfied("識別済み") And (.Party0 = "敵" Or .Party0 = "中立"))) Or .IsConditionSatisfied("ユニット情報隠蔽") Or .IsFeatureAvailable("ダミーユニット") Then
							IsGUILocked = False
							Exit Sub
						End If
						
						If .CountWeapon = 0 And .CountAbility > 0 Then
							AbilityListCommand()
						Else
							WeaponListCommand()
						End If
					End With
					IsGUILocked = False
					Exit Sub
				End If
				
				CommandState = "コマンド選択"
				ProceedCommand(by_cancel)
				
			Case "コマンド選択"
				'MOD START 240aClearUnitStatus
				'            If MainWidth <> 15 Then
				'                DisplayUnitStatus SelectedUnit
				'            End If
				If Not NewGUIMode Then
					DisplayUnitStatus(SelectedUnit)
				Else
					ClearUnitStatus()
				End If
				'MOD  END  240a
				
				'武装一覧以外は一旦消しておく
				With MainForm
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.mnuUnitCommandItem(WeaponListCmdID).Visible = True
					For i = 0 To WeaponListCmdID - 1
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(i).Visible = False
					Next 
					For i = WeaponListCmdID + 1 To WaitCmdID
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(i).Visible = False
					Next 
				End With
				
				SelectedUnitForEvent = SelectedUnit
				'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SelectedTarget = Nothing
				'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SelectedTargetForEvent = Nothing
				
				With SelectedUnit
					'特殊能力＆アビリティ一覧はどのユニットでも見れる可能性があるので
					'先に判定しておく
					
					'特殊能力一覧コマンド
					For i = 1 To .CountAllFeature
						If .AllFeatureName(i) <> "" Then
							Select Case .AllFeature(i)
								Case "合体"
									If UList.IsDefined(LIndex(.AllFeatureData(i), 2)) Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
										Exit For
									End If
								Case Else
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
									Exit For
							End Select
						ElseIf .AllFeature(i) = "パイロット能力付加" Or .AllFeature(i) = "パイロット能力強化" Then 
							If InStr(.AllFeatureData(i), "非表示") = 0 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
								Exit For
							End If
						ElseIf .AllFeature(i) = "武器クラス" Or .AllFeature(i) = "防具クラス" Then 
							If IsOptionDefined("アイテム交換") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
								Exit For
							End If
						End If
					Next 
					With .MainPilot
						For i = 1 To .CountSkill
							If .SkillName0(i) <> "非表示" And .SkillName0(i) <> "" Then
								Select Case .Skill(i)
									Case "耐久"
										If Not IsOptionDefined("防御力成長") And Not IsOptionDefined("防御力レベルアップ") Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
											Exit For
										End If
									Case "追加レベル", "格闘ＵＰ", "射撃ＵＰ", "命中ＵＰ", "回避ＵＰ", "技量ＵＰ", "反応ＵＰ", "ＳＰＵＰ", "格闘ＤＯＷＮ", "射撃ＤＯＷＮ", "命中ＤＯＷＮ", "回避ＤＯＷＮ", "技量ＤＯＷＮ", "反応ＤＯＷＮ", "ＳＰＤＯＷＮ", "メッセージ", "魔力所有"
									Case Else
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
										Exit For
								End Select
							End If
						Next 
					End With
					
					'アビリティ一覧コマンド
					For i = 1 To .CountAbility
						If .IsAbilityMastered(i) And Not .IsDisabled((.Ability(i).Name)) And (Not .IsAbilityClassifiedAs(i, "合") Or .IsCombinationAbilityAvailable(i, True)) And Not .Ability(i).IsItem Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(AbilityListCmdID).Caption = Term("アビリティ", SelectedUnit) & "一覧"
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = True
							Exit For
						End If
					Next 
					
					'味方じゃない場合
					If .Party <> "味方" Or .IsConditionSatisfied("非操作") Or ViewMode Then
						'召喚ユニットは命令コマンドを使用可能
						If .Party = "ＮＰＣ" And .IsFeatureAvailable("召喚ユニット") And Not .IsConditionSatisfied("魅了") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("恐怖") And Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("狂戦士") And Not ViewMode Then
							If Not .Summoner Is Nothing Then
								If .Summoner.Party = "味方" Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令"
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
								End If
							End If
						End If
						
						'魅了したユニットに対しても命令コマンドを使用可能
						If .Party = "ＮＰＣ" And .IsConditionSatisfied("魅了") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("恐怖") And Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("狂戦士") And Not ViewMode Then
							If Not .Master Is Nothing Then
								If .Master.Party = "味方" Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "命令"
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
								End If
							End If
						End If
						
						'ダミーユニットの場合はコマンド一覧を表示しない
						If .IsFeatureAvailable("ダミーユニット") Then
							'特殊能力一覧
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							If MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible Then
								UnitCommand(FeatureListCmdID)
							Else
								CommandState = "ユニット選択"
							End If
							
							IsGUILocked = False
							Exit Sub
						End If
						
						If MapFileName <> "" Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動範囲"
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True
							
							For i = 1 To .CountWeapon
								If .IsWeaponAvailable(i, "") And Not .IsWeaponClassifiedAs(i, "Ｍ") Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "射程範囲"
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
								End If
							Next 
						End If
						
						'ユニットステータスコマンド用
						If MapFileName = "" Then
							'変形コマンド
							If .IsFeatureAvailable("変形") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TransformCmdID).Caption = .FeatureName("変形")
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "" Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "変形"
								End If
								
								For i = 2 To LLength(.FeatureData("変形"))
									uname = LIndex(.FeatureData("変形"), i)
									If .OtherForm(uname).IsAvailable Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(TransformCmdID).Visible = True
										Exit For
									End If
								Next 
							End If
							
							'分離コマンド
							If .IsFeatureAvailable("分離") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Caption = .FeatureName("分離")
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "" Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "分離"
								End If
								
								buf = .FeatureData("分離")
								
								'分離形態が利用出来ない場合は分離を行わない
								For i = 2 To LLength(buf)
									If Not UList.IsDefined(LIndex(buf, i)) Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
										Exit For
									End If
								Next 
								
								'パイロットが足らない場合も分離を行わない
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(SplitCmdID).Visible Then
									n = 0
									For i = 2 To LLength(buf)
										With UList.Item(LIndex(buf, i)).Data
											If Not .IsFeatureAvailable("召喚ユニット") Then
												n = n + System.Math.Abs(.PilotNum)
											End If
										End With
									Next 
									If .CountPilot < n Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
									End If
								End If
							End If
							If .IsFeatureAvailable("パーツ分離") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Caption = .FeatureName("パーツ分離")
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "" Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "パーツ分離"
								End If
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
							End If
							
							'合体コマンド
							If .IsFeatureAvailable("合体") Then
								For i = 1 To .CountFeature
									If .Feature(i) = "合体" Then
										n = 0
										'パートナーが存在しているか？
										For j = 3 To LLength(.FeatureData(i))
											u = UList.Item(LIndex(.FeatureData(i), j))
											If u Is Nothing Then
												Exit For
											End If
											
											If u.Status_Renamed <> "出撃" And u.CurrentForm.IsFeatureAvailable("合体制限") Then
												Exit For
											End If
											n = n + 1
										Next 
										
										'合体先のユニットが作成されているか？
										If Not UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
											n = 0
										End If
										
										'すべての条件を満たしている場合
										If n = LLength(.FeatureData(i)) - 2 Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(CombineCmdID).Caption = LIndex(.FeatureData(i), 1)
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											If MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "非表示" Then
												'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "合体"
											End If
											Exit For
										End If
									End If
								Next 
							ElseIf .IsFeatureAvailable("パーツ合体") Then 
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "パーツ合体"
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
							End If
							
							If Not .IsConditionSatisfied("ノーマルモード付加") Then
								'ハイパーモードコマンド
								If .IsFeatureAvailable("ハイパーモード") Then
									uname = LIndex(.FeatureData("ハイパーモード"), 2)
									If .OtherForm(uname).IsAvailable Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = LIndex(.FeatureData("ハイパーモード"), 1)
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										If MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "非表示" Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ハイパーモード"
										End If
									End If
								ElseIf .IsFeatureAvailable("ノーマルモード") Then 
									uname = LIndex(.FeatureData("ノーマルモード"), 1)
									If .OtherForm(uname).IsAvailable Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "ノーマルモード"
										If uname = LIndex(.FeatureData("変形"), 2) Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = False
										End If
									End If
								End If
							Else
								'変身解除
								If InStr(.FeatureData("ノーマルモード"), "手動解除") > 0 Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
									If .IsFeatureAvailable("変身解除コマンド名") Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = .FeatureData("変身解除コマンド名")
									ElseIf .IsHero Then 
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除"
									Else
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除"
									End If
								End If
							End If
							
							'換装コマンド
							If .IsFeatureAvailable("換装") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "換装"
								
								For i = 1 To LLength(.FeatureData("換装"))
									uname = LIndex(.FeatureData("換装"), i)
									If .OtherForm(uname).IsAvailable Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
										Exit For
									End If
								Next 
								
								'エリアスで換装の名称が変更されている？
								With ALDList
									For i = 1 To .Count
										With .Item(i)
											If .AliasType(1) = "換装" Then
												'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												MainForm.mnuUnitCommandItem(OrderCmdID).Caption = .Name
												Exit For
											End If
										End With
									Next 
								End With
							End If
						End If
						
						'ユニットコマンド
						If Not ViewMode Then
							i = UnitCommand1CmdID
							For	Each lab In colEventLabelList
								With lab
									If .Name = Event_Renamed.LabelType.UnitCommandEventLabel And .Enable Then
										buf = GetValueAsString(.Para(3))
										If (SelectedUnit.Party = "味方" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "全" Then
											If .CountPara <= 3 Then
												'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												MainForm.mnuUnitCommandItem(i).Visible = True
											ElseIf GetValueAsLong(.Para(4)) <> 0 Then 
												'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												MainForm.mnuUnitCommandItem(i).Visible = True
											End If
										End If
										
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										If MainForm.mnuUnitCommandItem(i).Visible Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
											UnitCommandLabelList(i - UnitCommand1CmdID + 1) = CStr(.LineNum)
											i = i + 1
											If i > UnitCommand10CmdID Then
												Exit For
											End If
										End If
									End If
								End With
							Next lab
						End If
						
						'未確認ユニットの場合は情報を隠蔽
						If (IsOptionDefined("ユニット情報隠蔽") And (Not .IsConditionSatisfied("識別済み") And (.Party0 = "敵" Or .Party0 = "中立"))) Or .IsConditionSatisfied("ユニット情報隠蔽") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(WeaponListCmdID).Visible = False
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = False
							For i = 1 To WaitCmdID
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(i).Visible Then
									Exit For
								End If
							Next 
							If i > WaitCmdID Then
								'表示可能なコマンドがなかった
								CommandState = "ユニット選択"
								IsGUILocked = False
								Exit Sub
							End If
							'メニューコマンドを全て殺してしまうとエラーになるのでここで非表示
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
						End If
						
						IsGUILocked = False
						If by_cancel Then
							'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5)
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6)
						End If
						Exit Sub
						
						'行動終了している場合
					ElseIf .Action = 0 Then 
						'発進コマンドは使用可能
						If .IsFeatureAvailable("母艦") Then
							If .Area <> "地中" Then
								If .CountUnitOnBoard > 0 Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = True
								End If
							End If
						End If
						
						'ユニットコマンド
						i = UnitCommand1CmdID
						For	Each lab In colEventLabelList
							With lab
								If .Name = Event_Renamed.LabelType.UnitCommandEventLabel And (.AsterNum = 1 Or .AsterNum = 3) Then
									If .Enable Then
										buf = .Para(3)
										If (SelectedUnit.Party = "味方" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "全" Then
											If .CountPara <= 3 Then
												'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												MainForm.mnuUnitCommandItem(i).Visible = True
											ElseIf StrToLng(.Para(4)) <> 0 Then 
												'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												MainForm.mnuUnitCommandItem(i).Visible = True
											End If
										End If
									End If
									
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									If MainForm.mnuUnitCommandItem(i).Visible Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
										UnitCommandLabelList(i - UnitCommand1CmdID + 1) = CStr(.LineNum)
										i = i + 1
										If i > UnitCommand10CmdID Then
											Exit For
										End If
									End If
								End If
							End With
						Next lab
						
						IsGUILocked = False
						'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 5)
						Exit Sub
					End If
					
					'移動コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "移動"
					If .Speed <= 0 Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(WaitCmdID).Visible = True '待機
					Else
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True '移動
					End If
					
					'テレポートコマンド
					If .IsFeatureAvailable("テレポート") Then
						If Len(.FeatureData("テレポート")) > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = LIndex(.FeatureData("テレポート"), 1)
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = "テレポート"
						End If
						
						If LLength(.FeatureData("テレポート")) = 2 Then
							If .EN >= CShort(LIndex(.FeatureData("テレポート"), 2)) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = True
							End If
							'通常移動がテレポートの場合
							If .Speed0 = 0 Or (.FeatureLevel("テレポート") >= 0 And LIndex(.FeatureData("テレポート"), 2) = "0") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
							End If
						Else
							If .EN >= 40 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = True
							End If
						End If
						
						If .IsConditionSatisfied("移動不能") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = False
						End If
					End If
					
					'ジャンプコマンド
					If .IsFeatureAvailable("ジャンプ") And .Area <> "空中" And .Area <> "宇宙" Then
						If Len(.FeatureData("ジャンプ")) > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(JumpCmdID).Caption = LIndex(.FeatureData("ジャンプ"), 1)
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(JumpCmdID).Caption = "ジャンプ"
						End If
						
						If LLength(.FeatureData("ジャンプ")) = 2 Then
							If .EN >= CShort(LIndex(.FeatureData("ジャンプ"), 2)) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(JumpCmdID).Visible = True
							End If
							'通常移動がジャンプの場合
							If .Speed0 = 0 Or (.FeatureLevel("ジャンプ") >= 0 And LIndex(.FeatureData("ジャンプ"), 2) = "0") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
							End If
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(JumpCmdID).Visible = True
							If .FeatureLevel("ジャンプ") >= 0 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
							End If
						End If
						
						If .IsConditionSatisfied("移動不能") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(JumpCmdID).Visible = False
						End If
					End If
					
					'会話コマンド
					For i = 1 To 4
						'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						u = Nothing
						Select Case i
							Case 1
								If .X > 1 Then
									u = MapDataForUnit(.X - 1, .Y)
								End If
							Case 2
								If .X < MapWidth Then
									u = MapDataForUnit(.X + 1, .Y)
								End If
							Case 3
								If .Y > 1 Then
									u = MapDataForUnit(.X, .Y - 1)
								End If
							Case 4
								If .Y < MapHeight Then
									u = MapDataForUnit(.X, .Y + 1)
								End If
						End Select
						
						If Not u Is Nothing Then
							If IsEventDefined("会話 " & .MainPilot.ID & " " & u.MainPilot.ID) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TalkCmdID).Visible = True
								Exit For
							End If
						End If
					Next 
					
					'攻撃コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "攻撃"
					For i = 1 To .CountWeapon
						If .IsWeaponUseful(i, "移動前") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
							Exit For
						End If
					Next 
					If .Area = "地中" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
					End If
					If .IsConditionSatisfied("攻撃不能") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
					End If
					
					'修理コマンド
					If .IsFeatureAvailable("修理装置") And .Area <> "地中" Then
						For i = 1 To 4
							'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							u = Nothing
							Select Case i
								Case 1
									If .X > 1 Then
										u = MapDataForUnit(.X - 1, .Y)
									End If
								Case 2
									If .X < MapWidth Then
										u = MapDataForUnit(.X + 1, .Y)
									End If
								Case 3
									If .Y > 1 Then
										u = MapDataForUnit(.X, .Y - 1)
									End If
								Case 4
									If .Y < MapHeight Then
										u = MapDataForUnit(.X, .Y + 1)
									End If
							End Select
							
							If Not u Is Nothing Then
								With u
									If (.Party = "味方" Or .Party = "ＮＰＣ") And .HP < .MaxHP And Not .IsConditionSatisfied("ゾンビ") Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(FixCmdID).Visible = True
										Exit For
									End If
								End With
							End If
						Next 
						
						If Len(.FeatureData("修理装置")) > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(FixCmdID).Caption = LIndex(.FeatureData("修理装置"), 1)
							If IsNumeric(LIndex(.FeatureData("修理装置"), 2)) Then
								If .EN < CShort(LIndex(.FeatureData("修理装置"), 2)) Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
								End If
							End If
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置"
						End If
					End If
					
					'補給コマンド
					If .IsFeatureAvailable("補給装置") And .Area <> "地中" Then
						For i = 1 To 4
							'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							u = Nothing
							Select Case i
								Case 1
									If .X > 1 Then
										u = MapDataForUnit(.X - 1, .Y)
									End If
								Case 2
									If .X < MapWidth Then
										u = MapDataForUnit(.X + 1, .Y)
									End If
								Case 3
									If .Y > 1 Then
										u = MapDataForUnit(.X, .Y - 1)
									End If
								Case 4
									If .Y < MapHeight Then
										u = MapDataForUnit(.X, .Y + 1)
									End If
							End Select
							
							If Not u Is Nothing Then
								With u
									If .Party = "味方" Or .Party = "ＮＰＣ" Then
										If .EN < .MaxEN And Not .IsConditionSatisfied("ゾンビ") Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
										Else
											For j = 1 To .CountWeapon
												If .Bullet(j) < .MaxBullet(j) Then
													'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
													MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
													Exit For
												End If
											Next 
											For j = 1 To .CountAbility
												If .Stock(j) < .MaxStock(j) Then
													'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
													MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
													Exit For
												End If
											Next 
										End If
									End If
								End With
							End If
						Next 
						
						If Len(.FeatureData("補給装置")) > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = LIndex(.FeatureData("補給装置"), 1)
							If IsNumeric(LIndex(.FeatureData("補給装置"), 2)) Then
								If .EN < CShort(LIndex(.FeatureData("補給装置"), 2)) Or .MainPilot.Morale < 100 Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
								End If
							End If
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置"
						End If
					End If
					
					'アビリティコマンド
					n = 0
					For i = 1 To .CountAbility
						If Not .Ability(i).IsItem And .IsAbilityMastered(i) Then
							n = n + 1
							If .IsAbilityUseful(i, "移動前") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = True
							End If
						End If
					Next 
					If .Area = "地中" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
					End If
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Term("アビリティ", SelectedUnit)
					If n = 1 Then
						For i = 1 To .CountAbility
							If Not .Ability(i).IsItem And .IsAbilityMastered(i) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = .AbilityNickname(i)
								Exit For
							End If
						Next 
					End If
					
					'チャージコマンド
					If Not .IsConditionSatisfied("チャージ完了") Then
						For i = 1 To .CountWeapon
							If .IsWeaponClassifiedAs(i, "Ｃ") Then
								If .IsWeaponAvailable(i, "チャージ") Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = True
									Exit For
								End If
							End If
						Next 
						For i = 1 To .CountAbility
							If .IsAbilityClassifiedAs(i, "Ｃ") Then
								If .IsAbilityAvailable(i, "チャージ") Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = True
									Exit For
								End If
							End If
						Next 
					End If
					
					'スペシャルパワーコマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Caption = Term("スペシャルパワー", SelectedUnit)
					If .MainPilot.CountSpecialPower > 0 Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
					Else
						For i = 1 To .CountPilot
							If .Pilot(i).CountSpecialPower > 0 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
								Exit For
							End If
						Next 
						For i = 1 To .CountSupport
							If .Support(i).CountSpecialPower > 0 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
								Exit For
							End If
						Next 
						If .IsFeatureAvailable("追加サポート") Then
							If .AdditionalSupport.CountSpecialPower > 0 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
							End If
						End If
					End If
					If .IsConditionSatisfied("憑依") Or .IsConditionSatisfied("スペシャルパワー使用不能") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = False
					End If
					
					'変形コマンド
					If .IsFeatureAvailable("変形") And .FeatureName("変形") <> "" And Not .IsConditionSatisfied("形態固定") And Not .IsConditionSatisfied("機体固定") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(TransformCmdID).Caption = .FeatureName("変形")
						
						For i = 2 To LLength(.FeatureData("変形"))
							uname = LIndex(.FeatureData("変形"), i)
							If .OtherForm(uname).IsAvailable Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TransformCmdID).Visible = True
								Exit For
							End If
						Next 
					End If
					
					'分離コマンド
					If .IsFeatureAvailable("分離") And .FeatureName("分離") <> "" And Not .IsConditionSatisfied("形態固定") And Not .IsConditionSatisfied("機体固定") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SplitCmdID).Caption = .FeatureName("分離")
						
						buf = .FeatureData("分離")
						
						'分離形態が利用出来ない場合は分離を行わない
						For i = 2 To LLength(buf)
							If Not UList.IsDefined(LIndex(buf, i)) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
								Exit For
							End If
						Next 
						
						'パイロットが足らない場合も分離を行わない
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						If MainForm.mnuUnitCommandItem(SplitCmdID).Visible Then
							n = 0
							For i = 2 To LLength(buf)
								With UList.Item(LIndex(buf, i)).Data
									If Not .IsFeatureAvailable("召喚ユニット") Then
										n = n + System.Math.Abs(.PilotNum)
									End If
								End With
							Next 
							If .CountPilot < n Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
							End If
						End If
					End If
					If .IsFeatureAvailable("パーツ分離") And .FeatureName("パーツ分離") <> "" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SplitCmdID).Caption = .FeatureName("パーツ分離")
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
					End If
					
					'合体コマンド
					If .IsFeatureAvailable("合体") And Not .IsConditionSatisfied("形態固定") And Not .IsConditionSatisfied("機体固定") Then
						For i = 1 To .CountFeature
							'3体以上からなる合体能力を持っているか？
							If .Feature(i) = "合体" And .FeatureName(i) <> "" And LLength(.FeatureData(i)) > 3 Then
								n = 0
								'パートナーは隣接しているか？
								For j = 3 To LLength(.FeatureData(i))
									u = UList.Item(LIndex(.FeatureData(i), j))
									If u Is Nothing Then
										Exit For
									End If
									If Not u.IsOperational Then
										Exit For
									End If
									If u.Status_Renamed <> "出撃" And u.CurrentForm.IsFeatureAvailable("合体制限") Then
										Exit For
									End If
									If System.Math.Abs(.X - u.CurrentForm.X) + System.Math.Abs(.Y - u.CurrentForm.Y) > 2 Then
										Exit For
									End If
									n = n + 1
								Next 
								
								'合体先のユニットが作成され、かつ合体可能な状態にあるか？
								uname = LIndex(.FeatureData(i), 2)
								u = UList.Item(uname)
								If u Is Nothing Then
									n = 0
								ElseIf u.IsConditionSatisfied("行動不能") Then 
									n = 0
								End If
								
								'すべての条件を満たしている場合
								If n = LLength(.FeatureData(i)) - 2 Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(CombineCmdID).Caption = LIndex(.FeatureData(i), 1)
									Exit For
								End If
							End If
						Next 
					End If
					
					If Not .IsConditionSatisfied("ノーマルモード付加") Then
						'ハイパーモードコマンド
						If .IsFeatureAvailable("ハイパーモード") And (.MainPilot.Morale >= CShort(10# * .FeatureLevel("ハイパーモード")) + 100 Or (.HP <= .MaxHP \ 4 And InStr(.FeatureData("ハイパーモード"), "気力発動") = 0)) And InStr(.FeatureData("ハイパーモード"), "自動発動") = 0 And .FeatureName("ハイパーモード") <> "" And Not .IsConditionSatisfied("形態固定") And Not .IsConditionSatisfied("機体固定") Then
							uname = LIndex(.FeatureData("ハイパーモード"), 2)
							If Not .OtherForm(uname).IsConditionSatisfied("行動不能") And .OtherForm(uname).IsAbleToEnter(.X, .Y) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = LIndex(.FeatureData("ハイパーモード"), 1)
							End If
						End If
					Else
						'変身解除
						If InStr(.FeatureData("ノーマルモード"), "手動解除") > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
							If .IsFeatureAvailable("変身解除コマンド名") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = .FeatureData("変身解除コマンド名")
							ElseIf .IsHero Then 
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "変身解除"
							Else
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "特殊モード解除"
							End If
						End If
					End If
					
					'地上コマンド
					If TerrainClass(.X, .Y) = "陸" Or TerrainClass(.X, .Y) = "屋内" Or TerrainClass(.X, .Y) = "月面" Then
						If .Area <> "地上" And .IsTransAvailable("陸") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "地上"
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(GroundCmdID).Visible = True
						End If
					ElseIf TerrainClass(.X, .Y) = "水" Or TerrainClass(.X, .Y) = "深水" Then 
						If .Area <> "水上" And .IsTransAvailable("水上") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "水上"
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(GroundCmdID).Visible = True
						End If
					End If
					
					'空中コマンド
					Select Case TerrainClass(.X, .Y)
						Case "宇宙"
						Case "月面"
							If (.IsTransAvailable("空") Or .IsTransAvailable("宇宙")) And Not .Area = "宇宙" Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "宇宙"
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SkyCmdID).Visible = True
							End If
						Case Else
							If .IsTransAvailable("空") And Not .Area = "空中" Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "空中"
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SkyCmdID).Visible = True
							End If
					End Select
					
					'地中コマンド
					If .IsTransAvailable("地中") And Not .Area = "地中" And (TerrainClass(.X, .Y) = "陸" Or TerrainClass(.X, .Y) = "月面") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(UndergroundCmdID).Visible = True
					End If
					
					'水中コマンド
					If .Area <> "水中" Then
						If TerrainClass(.X, .Y) = "深水" And (.IsTransAvailable("水") Or .IsFeatureAvailable("水泳")) And Mid(.Data.Adaption, 3, 1) <> "-" Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(WaterCmdID).Visible = True
						ElseIf TerrainClass(.X, .Y) = "水" And Mid(.Data.Adaption, 3, 1) <> "-" Then 
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(WaterCmdID).Visible = True
						End If
					End If
					
					'発進コマンド
					If .IsFeatureAvailable("母艦") And .Area <> "地中" Then
						If .CountUnitOnBoard > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = True
						End If
					End If
					
					'アイテムコマンド
					For i = 1 To .CountAbility
						If .IsAbilityUseful(i, "移動前") And .Ability(i).IsItem Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(ItemCmdID).Visible = True
							Exit For
						End If
					Next 
					If .Area = "地中" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
					End If
					
					'召喚解除コマンド
					For i = 1 To .CountServant
						With .Servant(i).CurrentForm
							Select Case .Status_Renamed
								Case "出撃", "格納"
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(DismissCmdID).Visible = True
								Case "旧主形態", "旧形態"
									'合体後の形態が出撃中なら使用不可
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(DismissCmdID).Visible = True
									For j = 1 To .CountFeature
										If .Feature(j) = "合体" Then
											uname = LIndex(.FeatureData(j), 2)
											If UList.IsDefined(uname) Then
												With UList.Item(uname).CurrentForm
													If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
														'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
														MainForm.mnuUnitCommandItem(DismissCmdID).Visible = False
													End If
												End With
											End If
										End If
									Next 
							End Select
						End With
					Next 
					If .IsFeatureAvailable("召喚解除コマンド名") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(DismissCmdID).Caption = .FeatureData("召喚解除コマンド名")
					Else
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(DismissCmdID).Caption = "召喚解除"
					End If
					
					'ユニットコマンド
					i = UnitCommand1CmdID
					For	Each lab In colEventLabelList
						With lab
							If .Name = Event_Renamed.LabelType.UnitCommandEventLabel Then
								If .Enable Then
									buf = .Para(3)
									If (SelectedUnit.Party = "味方" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "全" Then
										If .CountPara <= 3 Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(i).Visible = True
										ElseIf StrToLng(.Para(4)) <> 0 Then 
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(i).Visible = True
										End If
									End If
								End If
								
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(i).Visible Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
									UnitCommandLabelList(i - UnitCommand1CmdID + 1) = CStr(.LineNum)
									i = i + 1
									If i > UnitCommand10CmdID Then
										Exit For
									End If
								End If
							End If
						End With
					Next lab
				End With
				
				If Not SelectedUnit Is DisplayedUnit Then
					'MOD START 240a
					'                DisplayUnitStatus SelectedUnit
					'新ＧＵＩ使用時はクリック時にユニットステータスを表示しない
					If Not NewGUIMode Then
						DisplayUnitStatus(SelectedUnit)
					End If
					'MOD  END  240a
				End If
				
				IsGUILocked = False
				If by_cancel Then
					'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5)
				Else
					'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6)
				End If
				
			Case "移動後コマンド選択"
				SelectedUnitForEvent = SelectedUnit
				
				With SelectedUnit
					'移動時にＥＮを消費している場合はステータスウィンドウを更新
					' MOD START MARGE
					'                If MainWidth = 15 Then
					If Not NewGUIMode Then
						' MOD END MARGE
						If PrevUnitEN <> .EN Then
							DisplayUnitStatus(SelectedUnit)
						End If
					End If
					
					With MainForm
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(WaitCmdID).Visible = True
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(MoveCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(TeleportCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(JumpCmdID).Visible = False
					End With
					
					'会話コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(TalkCmdID).Visible = False
					For i = 1 To 4
						'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						u = Nothing
						Select Case i
							Case 1
								If .X > 1 Then
									u = MapDataForUnit(.X - 1, .Y)
								End If
							Case 2
								If .X < MapWidth Then
									u = MapDataForUnit(.X + 1, .Y)
								End If
							Case 3
								If .Y > 1 Then
									u = MapDataForUnit(.X, .Y - 1)
								End If
							Case 4
								If .Y < MapHeight Then
									u = MapDataForUnit(.X, .Y + 1)
								End If
						End Select
						
						If Not u Is Nothing Then
							If IsEventDefined("会話 " & .MainPilot.ID & " " & u.MainPilot.ID) Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TalkCmdID).Visible = True
								Exit For
							End If
						End If
					Next 
					
					'攻撃コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "攻撃"
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
					For i = 1 To .CountWeapon
						If .IsWeaponUseful(i, "移動後") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
							Exit For
						End If
					Next 
					If .Area = "地中" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
					End If
					If .IsConditionSatisfied("攻撃不能") Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
					End If
					
					'修理コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
					If .IsFeatureAvailable("修理装置") And .Area <> "地中" Then
						For i = 1 To 4
							'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							u = Nothing
							Select Case i
								Case 1
									If .X > 1 Then
										u = MapDataForUnit(.X - 1, .Y)
									End If
								Case 2
									If .X < MapWidth Then
										u = MapDataForUnit(.X + 1, .Y)
									End If
								Case 3
									If .Y > 1 Then
										u = MapDataForUnit(.X, .Y - 1)
									End If
								Case 4
									If .Y < MapHeight Then
										u = MapDataForUnit(.X, .Y + 1)
									End If
							End Select
							
							If Not u Is Nothing Then
								With u
									If (.Party = "味方" Or .Party = "ＮＰＣ") And .HP < .MaxHP Then
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(FixCmdID).Visible = True
										Exit For
									End If
								End With
							End If
						Next 
						
						If Len(.FeatureData("修理装置")) > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(FixCmdID).Caption = LIndex(.FeatureData("修理装置"), 1)
							If IsNumeric(LIndex(.FeatureData("修理装置"), 2)) Then
								If .EN < CShort(LIndex(.FeatureData("修理装置"), 2)) Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
								End If
							End If
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(FixCmdID).Caption = "修理装置"
						End If
					End If
					
					'補給コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
					If .IsFeatureAvailable("補給装置") And .Area <> "地中" Then
						For i = 1 To 4
							'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							u = Nothing
							Select Case i
								Case 1
									If .X > 1 Then
										u = MapDataForUnit(.X - 1, .Y)
									End If
								Case 2
									If .X < MapWidth Then
										u = MapDataForUnit(.X + 1, .Y)
									End If
								Case 3
									If .Y > 1 Then
										u = MapDataForUnit(.X, .Y - 1)
									End If
								Case 4
									If .Y < MapHeight Then
										u = MapDataForUnit(.X, .Y + 1)
									End If
							End Select
							
							If Not u Is Nothing Then
								With u
									If .Party = "味方" Or .Party = "ＮＰＣ" Then
										If .EN < .MaxEN Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
										Else
											For j = 1 To .CountWeapon
												If .Bullet(j) < .MaxBullet(j) Then
													'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
													MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
													Exit For
												End If
											Next 
											For j = 1 To .CountAbility
												If .Stock(j) < .MaxStock(j) Then
													'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
													MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
													Exit For
												End If
											Next 
										End If
									End If
								End With
							End If
						Next 
						
						If Len(.FeatureData("補給装置")) > 0 Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = LIndex(.FeatureData("補給装置"), 1)
							If IsNumeric(LIndex(.FeatureData("補給装置"), 2)) Then
								If .EN < CShort(LIndex(.FeatureData("補給装置"), 2)) Or .MainPilot.Morale < 100 Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
								End If
							End If
						Else
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "補給装置"
						End If
						
						If IsOptionDefined("移動後補給不可") And Not SelectedUnit.MainPilot.IsSkillAvailable("補給") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
						End If
					End If
					
					'アビリティコマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
					n = 0
					For i = 1 To .CountAbility
						If Not .Ability(i).IsItem Then
							n = n + 1
							If .IsAbilityUseful(i, "移動後") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = True
							End If
						End If
					Next 
					If .Area = "地中" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
					End If
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Term("アビリティ", SelectedUnit)
					If n = 1 Then
						For i = 1 To .CountAbility
							If Not .Ability(i).IsItem Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = .AbilityNickname(i)
								Exit For
							End If
						Next 
					End If
					
					With MainForm
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(ChargeCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(SpecialPowerCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(TransformCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(SplitCmdID).Visible = False
					End With
					
					'合体コマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(CombineCmdID).Visible = False
					If .IsFeatureAvailable("合体") And Not .IsConditionSatisfied("形態固定") And Not .IsConditionSatisfied("機体固定") Then
						For i = 1 To .CountFeature
							'3体以上からなる合体能力を持っているか？
							If .Feature(i) = "合体" And .FeatureName(i) <> "" And LLength(.FeatureData(i)) > 3 Then
								n = 0
								For j = 3 To LLength(.FeatureData(i))
									u = UList.Item(LIndex(.FeatureData(i), j))
									If u Is Nothing Then
										Exit For
									End If
									If Not u.IsOperational Then
										Exit For
									End If
									If u.Status_Renamed <> "出撃" And u.CurrentForm.IsFeatureAvailable("合体制限") Then
										Exit For
									End If
									If System.Math.Abs(.X - u.CurrentForm.X) + System.Math.Abs(.Y - u.CurrentForm.Y) > 2 Then
										Exit For
									End If
									n = n + 1
								Next 
								
								uname = LIndex(.FeatureData(i), 2)
								u = UList.Item(uname)
								If u Is Nothing Then
									n = 0
								ElseIf u.IsConditionSatisfied("行動不能") Then 
									n = 0
								End If
								
								If n = LLength(.FeatureData(i)) - 2 Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(CombineCmdID).Caption = LIndex(.FeatureData(i), 1)
									Exit For
								End If
							End If
						Next 
					End If
					
					With MainForm
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(HyperModeCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(GroundCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(SkyCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UndergroundCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(WaterCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(LaunchCmdID).Visible = False
					End With
					
					'アイテムコマンド
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
					For i = 1 To .CountAbility
						If .IsAbilityUseful(i, "移動後") And .Ability(i).IsItem Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(ItemCmdID).Visible = True
							Exit For
						End If
					Next 
					If .Area = "地中" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
					End If
					
					With MainForm
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(DismissCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(OrderCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(FeatureListCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(WeaponListCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(AbilityListCmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand1CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand2CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand3CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand4CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand5CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand6CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand7CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand8CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand9CmdID).Visible = False
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.mnuUnitCommandItem(UnitCommand10CmdID).Visible = False
					End With
					
					'ユニットコマンド
					i = UnitCommand1CmdID
					For	Each lab In colEventLabelList
						With lab
							If .Name = Event_Renamed.LabelType.UnitCommandEventLabel And .AsterNum >= 2 Then
								If .Enable Then
									buf = .Para(3)
									If (SelectedUnit.Party = "味方" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "全" Then
										If .CountPara <= 3 Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(i).Visible = True
										ElseIf StrToLng(.Para(4)) <> 0 Then 
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(i).Visible = True
										End If
									End If
								End If
								
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								If MainForm.mnuUnitCommandItem(i).Visible Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
									UnitCommandLabelList(i - UnitCommand1CmdID + 1) = CStr(.LineNum)
									i = i + 1
									If i > UnitCommand10CmdID Then
										Exit For
									End If
								End If
							End If
						End With
					Next lab
				End With
				
				IsGUILocked = False
				If by_cancel Then
					'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5)
				Else
					'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6)
					System.Windows.Forms.Application.DoEvents()
					'ＰＣに負荷がかかったような状態だとポップアップメニューの選択が
					'うまく行えない場合があるのでやり直す
					Do While (CommandState = "移動後コマンド選択" And SelectedCommand = "移動")
						'UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6)
						System.Windows.Forms.Application.DoEvents()
					Loop 
				End If
				
			Case "ターゲット選択", "移動後ターゲット選択"
				If Not MaskData(PixelToMapX(MouseX), PixelToMapY(MouseY)) Then
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					'自分自身を選択された場合
					If SelectedUnit.X = SelectedX And SelectedUnit.Y = SelectedY Then
						If SelectedCommand = "スペシャルパワー" Then
							'下に抜ける
						ElseIf SelectedCommand = "アビリティ" Or SelectedCommand = "マップアビリティ" Or SelectedCommand = "アイテム" Or SelectedCommand = "マップアイテム" Then 
							If SelectedUnit.AbilityMinRange(SelectedAbility) > 0 Then
								'自分自身は選択不可
								IsGUILocked = False
								Exit Sub
							End If
						ElseIf SelectedCommand = "移動命令" Then 
							'下に抜ける
						Else
							'自分自身は選択不可
							IsGUILocked = False
							Exit Sub
						End If
					End If
					
					'場所を選択するコマンド
					Select Case SelectedCommand
						' MOD START MARGE
						'                    Case "移動"
						Case "移動", "再移動"
							' MOD END MARGE
							FinishMoveCommand()
							IsGUILocked = False
							Exit Sub
						Case "テレポート"
							FinishTeleportCommand()
							IsGUILocked = False
							Exit Sub
						Case "ジャンプ"
							FinishJumpCommand()
							IsGUILocked = False
							Exit Sub
						Case "マップ攻撃"
							MapAttackCommand()
							IsGUILocked = False
							Exit Sub
						Case "マップアビリティ", "マップアイテム"
							MapAbilityCommand()
							IsGUILocked = False
							Exit Sub
						Case "発進"
							FinishLaunchCommand()
							IsGUILocked = False
							Exit Sub
						Case "移動命令"
							FinishOrderCommand()
							IsGUILocked = False
							Exit Sub
					End Select
					
					'これ以降はユニットを選択するコマンド
					
					'指定した地点にユニットがいる？
					If MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
						IsGUILocked = False
						Exit Sub
					End If
					
					'ターゲットを選択
					SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
					
					Select Case SelectedCommand
						Case "攻撃"
							FinishAttackCommand()
						Case "アビリティ", "アイテム"
							FinishAbilityCommand()
						Case "会話"
							FinishTalkCommand()
						Case "修理"
							FinishFixCommand()
						Case "補給"
							FinishSupplyCommand()
						Case "スペシャルパワー"
							FinishSpecialPowerCommand()
						Case "攻撃命令", "護衛命令"
							FinishOrderCommand()
					End Select
				End If
				
			Case "マップ攻撃使用", "移動後マップ攻撃使用"
				If 1 <= PixelToMapX(MouseX) And PixelToMapX(MouseX) <= MapWidth Then
					If 1 <= PixelToMapY(MouseY) And PixelToMapY(MouseY) <= MapHeight Then
						If Not MaskData(PixelToMapX(MouseX), PixelToMapY(MouseY)) Then
							'効果範囲内でクリックされればマップ攻撃発動
							If SelectedCommand = "マップ攻撃" Then
								MapAttackCommand()
							Else
								MapAbilityCommand()
							End If
						End If
					End If
				End If
		End Select
		
		IsGUILocked = False
	End Sub
	
	'ＧＵＩの処理をキャンセル
	Public Sub CancelCommand()
		Dim tmp_x, tmp_y As Short
		
		With SelectedUnit
			Select Case CommandState
				Case "ユニット選択"
				Case "コマンド選択"
					CommandState = "ユニット選択"
					'ADD START
					'選択したコマンドを初期化
					SelectedCommand = ""
					' MOD START MARGE
					'                If MainWidth <> 15 Then
					If NewGUIMode Then
						' MOD  END  MARGE
						ClearUnitStatus()
					End If
					
				Case "ターゲット選択"
					' ADD START MARGE
					If SelectedCommand = "再移動" Then
						WaitCommand()
						Exit Sub
					End If
					' ADD END MARGE
					CommandState = "コマンド選択"
					DisplayUnitStatus(SelectedUnit)
					RedrawScreen()
					ProceedCommand(True)
					
				Case "移動後コマンド選択"
					CommandState = "ターゲット選択"
					.Area = PrevUnitArea
					.Move(PrevUnitX, PrevUnitY, True, True)
					.EN = PrevUnitEN
					If Not SelectedUnit Is MapDataForUnit(PrevUnitX, PrevUnitY) Then
						'発進をキャンセルした場合
						SelectedTarget = SelectedUnit
						PaintUnitBitmap(SelectedTarget)
						SelectedUnit = MapDataForUnit(PrevUnitX, PrevUnitY)
						' MOD START MARGE
						'                ElseIf MainWidth = 15 Then
					ElseIf Not NewGUIMode Then 
						' MOD END MARGE
						DisplayUnitStatus(SelectedUnit)
					End If
					' MOD START MARGE
					' 移動後コマンドをキャンセルした場合、MoveCostを0にリセットする
					SelectedUnitMoveCost = 0
					' MOD END MARGE
					Select Case SelectedCommand
						Case "移動"
							StartMoveCommand()
						Case "テレポート"
							StartTeleportCommand()
						Case "ジャンプ"
							StartJumpCommand()
						Case "発進"
							PaintUnitBitmap(SelectedTarget)
					End Select
					
				Case "移動後ターゲット選択"
					CommandState = "移動後コマンド選択"
					DisplayUnitStatus(SelectedUnit)
					
					tmp_x = .X
					tmp_y = .Y
					.X = PrevUnitX
					.Y = PrevUnitY
					Select Case PrevCommand
						Case "移動"
							AreaInSpeed(SelectedUnit)
						Case "テレポート"
							AreaInTeleport(SelectedUnit)
						Case "ジャンプ"
							AreaInSpeed(SelectedUnit, True)
						Case "発進"
							With SelectedTarget
								If .IsFeatureAvailable("テレポート") And (.Data.Speed = 0 Or LIndex(.FeatureData("テレポート"), 2) = "0") Then
									AreaInTeleport(SelectedTarget)
								ElseIf .IsFeatureAvailable("ジャンプ") And (.Data.Speed = 0 Or LLength(.FeatureData("ジャンプ")) < 2 Or LIndex(.FeatureData("ジャンプ"), 2) = "0") Then 
									AreaInSpeed(SelectedTarget, True)
								Else
									AreaInSpeed(SelectedTarget)
								End If
							End With
					End Select
					.X = tmp_x
					.Y = tmp_y
					SelectedCommand = PrevCommand
					
					MaskData(tmp_x, tmp_y) = False
					MaskScreen()
					ProceedCommand(True)
					
				Case "マップ攻撃使用", "移動後マップ攻撃使用"
					If CommandState = "マップ攻撃使用" Then
						CommandState = "ターゲット選択"
					Else
						CommandState = "移動後ターゲット選択"
					End If
					If SelectedCommand = "マップ攻撃" Then
						If .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ直") Then
							AreaInCross(.X, .Y, .WeaponMaxRange(SelectedWeapon), (.Weapon(SelectedWeapon).MinRange))
						ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ移") Then 
							AreaInMoveAction(SelectedUnit, .WeaponMaxRange(SelectedWeapon))
						Else
							AreaInRange(.X, .Y, .WeaponMaxRange(SelectedWeapon), .Weapon(SelectedWeapon).MinRange, "すべて")
						End If
					Else
						If .IsAbilityClassifiedAs(SelectedAbility, "Ｍ直") Then
							AreaInCross(.X, .Y, .AbilityMaxRange(SelectedAbility), .AbilityMinRange(SelectedAbility))
						ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Ｍ移") Then 
							AreaInMoveAction(SelectedUnit, .AbilityMaxRange(SelectedAbility))
						Else
							AreaInRange(.X, .Y, .AbilityMaxRange(SelectedAbility), .AbilityMinRange(SelectedAbility), "すべて")
						End If
					End If
					MaskScreen()
			End Select
		End With
	End Sub
	
	
	' ユニットコマンドを実行
	Public Sub UnitCommand(ByVal idx As Short)
		Dim prev_used_action As Short
		
		PrevCommand = SelectedCommand
		
		With SelectedUnit
			prev_used_action = .UsedAction
			Select Case idx
				Case MoveCmdID '移動
					'なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
					'移動後のコマンド選択をやり直す
					If CommandState = "移動後コマンド選択" Then
						System.Windows.Forms.Application.DoEvents()
						Exit Sub
					End If
					
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If MainForm.mnuUnitCommandItem.Item(MoveCmdID).Caption = "移動" Then
						StartMoveCommand()
					Else
						ShowAreaInSpeedCommand()
					End If
					
				Case TeleportCmdID 'テレポート
					StartTeleportCommand()
					
				Case JumpCmdID 'ジャンプ
					StartJumpCommand()
					
				Case TalkCmdID '会話
					StartTalkCommand()
					
				Case AttackCmdID '攻撃
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If MainForm.mnuUnitCommandItem.Item(AttackCmdID).Caption = "攻撃" Then
						StartAttackCommand()
					Else
						ShowAreaInRangeCommand()
					End If
					
				Case FixCmdID '修理
					StartFixCommand()
					
				Case SupplyCmdID '補給
					StartSupplyCommand()
					
				Case AbilityCmdID 'アビリティ
					StartAbilityCommand()
					
				Case ChargeCmdID 'チャージ
					ChargeCommand()
					
				Case SpecialPowerCmdID '精神
					StartSpecialPowerCommand()
					
				Case TransformCmdID '変形
					TransformCommand()
					
				Case SplitCmdID '分離
					SplitCommand()
					
				Case CombineCmdID '合体
					CombineCommand()
					
				Case HyperModeCmdID 'ハイパーモード・変身解除
					If InStr(.FeatureData("ノーマルモード"), "手動解除") > 0 Then
						CancelTransformationCommand()
					Else
						HyperModeCommand()
					End If
					
				Case GroundCmdID '地上
					LockGUI()
					If TerrainClass(.X, .Y) = "水" Or TerrainClass(.X, .Y) = "深水" Then
						.Area = "水上"
					Else
						.Area = "地上"
					End If
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "ユニット選択"
					UnlockGUI()
					
				Case SkyCmdID '空中
					LockGUI()
					If TerrainClass(.X, .Y) = "月面" Then
						.Area = "宇宙"
					Else
						.Area = "空中"
					End If
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "ユニット選択"
					UnlockGUI()
					
				Case UndergroundCmdID '地中
					LockGUI()
					.Area = "地中"
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "ユニット選択"
					UnlockGUI()
					
				Case WaterCmdID '水中
					LockGUI()
					.Area = "水中"
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "ユニット選択"
					UnlockGUI()
					
				Case LaunchCmdID '発進
					StartLaunchCommand()
					
				Case ItemCmdID 'アイテム
					StartAbilityCommand(True)
					
				Case DismissCmdID '召喚解除
					LockGUI()
					
					'召喚解除の使用イベント
					HandleEvent("使用", .MainPilot.ID, "召喚解除")
					If IsScenarioFinished Then
						IsScenarioFinished = False
						UnlockGUI()
						Exit Sub
					End If
					If IsCanceled Then
						IsCanceled = False
						Exit Sub
					End If
					
					'召喚ユニットを解放
					.DismissServant()
					
					'召喚解除の使用後イベント
					HandleEvent("使用後", .CurrentForm.MainPilot.ID, "召喚解除")
					If IsScenarioFinished Then
						IsScenarioFinished = False
					End If
					If IsCanceled Then
						IsCanceled = False
					End If
					
					UnlockGUI()
					
				Case OrderCmdID '命令/換装
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If MainForm.mnuUnitCommandItem.Item(OrderCmdID).Caption = "命令" Then
						StartOrderCommand()
					Else
						ExchangeFormCommand()
					End If
					
				Case FeatureListCmdID '特殊能力一覧
					FeatureListCommand()
					
				Case WeaponListCmdID '武器一覧
					WeaponListCommand()
					
				Case AbilityListCmdID 'アビリティ一覧
					AbilityListCommand()
					
				Case UnitCommand1CmdID To UnitCommand10CmdID 'ユニットコマンド
					LockGUI()
					
					'ユニットコマンドの使用イベント
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					HandleEvent("使用", .MainPilot.ID, MainForm.mnuUnitCommandItem.Item(idx).Caption)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						UnlockGUI()
						Exit Sub
					End If
					If IsCanceled Then
						IsCanceled = False
						WaitCommand()
						Exit Sub
					End If
					
					'ユニットコマンドを実行
					HandleEvent(UnitCommandLabelList(idx - UnitCommand1CmdID + 1))
					
					If IsCanceled Then
						IsCanceled = False
						CancelCommand()
						UnlockGUI()
						Exit Sub
					End If
					
					'ユニットコマンドの使用後イベント
					If .CurrentForm.CountPilot > 0 Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						HandleEvent("使用後", .CurrentForm.MainPilot.ID, MainForm.mnuUnitCommandItem.Item(idx).Caption)
						If IsScenarioFinished Then
							IsScenarioFinished = False
							UnlockGUI()
							Exit Sub
						End If
					End If
					
					'ステータスウィンドウを更新
					If .CurrentForm.CountPilot > 0 Then
						DisplayUnitStatus(.CurrentForm)
					End If
					
					'行動終了
					If .CurrentForm.UsedAction <= prev_used_action Then
						If CommandState = "移動後コマンド選択" Then
							WaitCommand()
						Else
							CommandState = "ユニット選択"
							UnlockGUI()
						End If
					ElseIf IsCanceled Then 
						IsCanceled = False
					Else
						WaitCommand(True)
					End If
					
				Case WaitCmdID '待機
					WaitCommand()
					
				Case Else
					'なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
					'移動後のコマンド選択をやり直す
					If CommandState = "移動後コマンド選択" Then
						System.Windows.Forms.Application.DoEvents()
						Exit Sub
					End If
			End Select
		End With
	End Sub
	
	
	'「移動」コマンドを開始
	' MOD START MARGE
	'Public Sub StartMoveCommand()
	Private Sub StartMoveCommand()
		' MOD END MARGE
		SelectedCommand = "移動"
		AreaInSpeed(SelectedUnit)
		If Not IsOptionDefined("大型マップ") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		End If
		CommandState = "ターゲット選択"
	End Sub
	
	'「移動」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishMoveCommand()
	Private Sub FinishMoveCommand()
		' MOD END MARGE
		Dim ret As Short
		
		LockGUI()
		
		With SelectedUnit
			PrevUnitX = .X
			PrevUnitY = .Y
			PrevUnitArea = .Area
			PrevUnitEN = .EN
			
			'移動後に着艦or合体する場合はプレイヤーに確認を取る
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("母艦") And Not .IsFeatureAvailable("母艦") Then
					ret = MsgBox("着艦しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "着艦")
				Else
					ret = MsgBox("合体しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "合体")
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'ユニットを移動
			.Move(SelectedX, SelectedY)
			
			'移動後に着艦または合体した？
			If Not MapDataForUnit(.X, .Y) Is SelectedUnit Then
				If MapDataForUnit(.X, .Y).IsFeatureAvailable("母艦") And Not .IsFeatureAvailable("母艦") And .CountPilot > 0 Then
					'着艦メッセージ表示
					If .IsMessageDefined("着艦(" & .Name & ")") Then
						OpenMessageForm()
						.PilotMessage("着艦(" & .Name & ")")
						CloseMessageForm()
					ElseIf .IsMessageDefined("着艦") Then 
						OpenMessageForm()
						.PilotMessage("着艦")
						CloseMessageForm()
					End If
					.SpecialEffect("着艦", .Name)
					
					'収納イベント
					SelectedTarget = MapDataForUnit(.X, .Y)
					HandleEvent("収納", .MainPilot.ID)
				Else
					'合体後のユニットを選択
					SelectedUnit = MapDataForUnit(.X, .Y)
					
					'合体イベント
					HandleEvent("合体", SelectedUnit.MainPilot.ID, SelectedUnit.Name)
				End If
				
				'移動後の収納・合体イベントでステージが終了することがあるので
				If IsScenarioFinished Then
					IsScenarioFinished = False
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ClearUnitStatus()
					RedrawScreen()
					CommandState = "ユニット選択"
					UnlockGUI()
					Exit Sub
				End If
				
				'残り行動数を減少させる
				SelectedUnit.UseAction()
				
				'持続期間が「移動」のスペシャルパワー効果を削除
				SelectedUnit.RemoveSpecialPowerInEffect("移動")
				
				DisplayUnitStatus(SelectedUnit)
				RedrawScreen()
				CommandState = "ユニット選択"
				UnlockGUI()
				Exit Sub
			End If
			' ADD START MARGE
			If SelectedUnitMoveCost > 0 Then
				'行動数を減らす
				WaitCommand()
				Exit Sub
			End If
			
			SelectedUnitMoveCost = TotalMoveCost(.X, .Y)
			' ADD END MARGE
		End With
		
		CommandState = "移動後コマンド選択"
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'「テレポート」コマンドを開始
	' MOD START MARGE
	'Public Sub StartTeleportCommand()
	Private Sub StartTeleportCommand()
		' MOD END MARGE
		SelectedCommand = "テレポート"
		AreaInTeleport(SelectedUnit)
		If Not IsOptionDefined("大型マップ") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		End If
		CommandState = "ターゲット選択"
	End Sub
	
	'「テレポート」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishTeleportCommand()
	Private Sub FinishTeleportCommand()
		' MOD END MARGE
		Dim ret As Short
		
		LockGUI()
		
		With SelectedUnit
			PrevUnitX = .X
			PrevUnitY = .Y
			PrevUnitArea = .Area
			PrevUnitEN = .EN
			
			'テレポート後に着艦or合体する場合はプレイヤーに確認を取る
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("母艦") Then
					ret = MsgBox("着艦しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "着艦")
				Else
					ret = MsgBox("合体しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "合体")
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'メッセージを表示
			If .IsMessageDefined("テレポート(" & .FeatureName("テレポート") & ")") Then
				OpenMessageForm()
				.PilotMessage("テレポート(" & .FeatureName("テレポート") & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("テレポート") Then 
				OpenMessageForm()
				.PilotMessage("テレポート")
				CloseMessageForm()
			End If
			
			'アニメ表示
			If .IsAnimationDefined("テレポート", .FeatureName("テレポート")) Then
				.PlayAnimation("テレポート", .FeatureName("テレポート"))
			ElseIf .IsSpecialEffectDefined("テレポート", .FeatureName("テレポート")) Then 
				.SpecialEffect("テレポート", .FeatureName("テレポート"))
			ElseIf BattleAnimation Then 
				ShowAnimation("テレポート発動 Whiz.wav " & .FeatureName0("テレポート"))
			Else
				PlayWave("Whiz.wav")
			End If
			
			'ＥＮを消費
			If LLength(.FeatureData("テレポート")) = 2 Then
				.EN = PrevUnitEN - CShort(LIndex(.FeatureData("テレポート"), 2))
			Else
				.EN = PrevUnitEN - 40
			End If
			
			'ユニットを移動
			.Move(SelectedX, SelectedY, True, False, True)
			RedrawScreen()
			
			'移動後に着艦または合体した？
			If Not MapDataForUnit(SelectedX, SelectedY) Is SelectedUnit Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("母艦") And Not .IsFeatureAvailable("母艦") And .CountPilot > 0 Then
					'着艦メッセージ表示
					If .IsMessageDefined("着艦(" & .Name & ")") Then
						OpenMessageForm()
						.PilotMessage("着艦(" & .Name & ")")
						CloseMessageForm()
					ElseIf .IsMessageDefined("着艦") Then 
						OpenMessageForm()
						.PilotMessage("着艦")
						CloseMessageForm()
					End If
					.SpecialEffect("着艦", .Name)
					
					'収納イベント
					SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
					HandleEvent("収納", .MainPilot.ID)
				Else
					'合体後のユニットを選択
					SelectedUnit = MapDataForUnit(SelectedX, SelectedY)
					
					'合体イベント
					HandleEvent("合体", SelectedUnit.MainPilot.ID, SelectedUnit.Name)
				End If
				
				'移動後の収納・合体イベントでステージが終了することがあるので
				If IsScenarioFinished Then
					IsScenarioFinished = False
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ClearUnitStatus()
					RedrawScreen()
					CommandState = "ユニット選択"
					UnlockGUI()
					Exit Sub
				End If
				
				'残り行動数を減少させる
				SelectedUnit.UseAction()
				
				'持続期間が「移動」のスペシャルパワー効果を削除
				SelectedUnit.RemoveSpecialPowerInEffect("移動")
				
				DisplayUnitStatus(MapDataForUnit(SelectedX, SelectedY))
				RedrawScreen()
				CommandState = "ユニット選択"
				UnlockGUI()
				Exit Sub
			End If
			' ADD START MARGE
			SelectedUnitMoveCost = 100
			' ADD END MARGE
		End With
		
		CommandState = "移動後コマンド選択"
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'「ジャンプ」コマンドを開始
	' MOD START MARGE
	'Public Sub StartJumpCommand()
	Private Sub StartJumpCommand()
		' MOD END MARGE
		SelectedCommand = "ジャンプ"
		AreaInSpeed(SelectedUnit, True)
		If Not IsOptionDefined("大型マップ") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		End If
		CommandState = "ターゲット選択"
	End Sub
	
	'「ジャンプ」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishJumpCommand()
	Private Sub FinishJumpCommand()
		' MOD END MARGE
		Dim ret As Short
		
		LockGUI()
		
		With SelectedUnit
			PrevUnitX = .X
			PrevUnitY = .Y
			PrevUnitArea = .Area
			PrevUnitEN = .EN
			
			'ジャンプ後に着艦or合体する場合はプレイヤーに確認を取る
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("母艦") Then
					ret = MsgBox("着艦しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "着艦")
				Else
					ret = MsgBox("合体しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "合体")
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'メッセージを表示
			If .IsMessageDefined("ジャンプ(" & .FeatureName("ジャンプ") & ")") Then
				OpenMessageForm()
				.PilotMessage("ジャンプ(" & .FeatureName("ジャンプ") & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("ジャンプ") Then 
				OpenMessageForm()
				.PilotMessage("ジャンプ")
				CloseMessageForm()
			End If
			
			'アニメ表示
			If .IsAnimationDefined("ジャンプ", .FeatureName("ジャンプ")) Then
				.PlayAnimation("ジャンプ", .FeatureName("ジャンプ"))
			ElseIf .IsSpecialEffectDefined("ジャンプ", .FeatureName("ジャンプ")) Then 
				.SpecialEffect("ジャンプ", .FeatureName("ジャンプ"))
			Else
				PlayWave("Swing.wav")
			End If
			
			'ＥＮを消費
			If LLength(.FeatureData("ジャンプ")) = 2 Then
				.EN = PrevUnitEN - CShort(LIndex(.FeatureData("ジャンプ"), 2))
			End If
			
			'ユニットを移動
			.Move(SelectedX, SelectedY, True, False, True)
			RedrawScreen()
			
			'移動後に着艦または合体した？
			If Not MapDataForUnit(SelectedX, SelectedY) Is SelectedUnit Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("母艦") And Not .IsFeatureAvailable("母艦") And .CountPilot > 0 Then
					'着艦メッセージ表示
					If .IsMessageDefined("着艦(" & .Name & ")") Then
						OpenMessageForm()
						.PilotMessage("着艦(" & .Name & ")")
						CloseMessageForm()
					ElseIf .IsMessageDefined("着艦") Then 
						OpenMessageForm()
						.PilotMessage("着艦")
						CloseMessageForm()
					End If
					.SpecialEffect("着艦", .Name)
					
					'収納イベント
					SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
					HandleEvent("収納", .MainPilot.ID)
				Else
					'合体後のユニットを選択
					SelectedUnit = MapDataForUnit(SelectedX, SelectedY)
					
					'合体イベント
					HandleEvent("合体", SelectedUnit.MainPilot.ID, SelectedUnit.Name)
				End If
				
				'移動後の収納・合体イベントでステージが終了することがあるので
				If IsScenarioFinished Then
					IsScenarioFinished = False
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ClearUnitStatus()
					RedrawScreen()
					CommandState = "ユニット選択"
					UnlockGUI()
					Exit Sub
				End If
				
				'残り行動数を減少させる
				SelectedUnit.UseAction()
				
				'持続期間が「移動」のスペシャルパワー効果を削除
				SelectedUnit.RemoveSpecialPowerInEffect("移動")
				
				DisplayUnitStatus(MapDataForUnit(SelectedX, SelectedY))
				RedrawScreen()
				CommandState = "ユニット選択"
				UnlockGUI()
				Exit Sub
			End If
			' ADD START MARGE
			SelectedUnitMoveCost = 100
			' ADD END MARGE
		End With
		
		CommandState = "移動後コマンド選択"
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'「攻撃」コマンドを開始
	' MOD START MARGE
	'Public Sub StartAttackCommand()
	Private Sub StartAttackCommand()
		' MOD END MARGE
		Dim i, j As Short
		Dim t, u As Unit
		Dim min_range, max_range As Short
		Dim BGM As String
		
		LockGUI()
		
		Dim partners() As Unit
		With SelectedUnit
			'ＢＧＭの設定
			If .IsFeatureAvailable("ＢＧＭ") Then
				BGM = SearchMidiFile(.FeatureData("ＢＧＭ"))
			End If
			If Len(BGM) = 0 Then
				BGM = SearchMidiFile(.MainPilot.BGM)
			End If
			If Len(BGM) = 0 Then
				BGM = BGMName("default")
			End If
			
			'武器の選択
			UseSupportAttack = True
			If CommandState = "コマンド選択" Then
				SelectedWeapon = WeaponListBox(SelectedUnit, "武器選択", "移動前", BGM)
			Else
				SelectedWeapon = WeaponListBox(SelectedUnit, "武器選択", "移動後", BGM)
			End If
			
			'キャンセル
			If SelectedWeapon = 0 Then
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			
			'武器ＢＧＭの演奏
			If .IsFeatureAvailable("武器ＢＧＭ") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "武器ＢＧＭ" And LIndex(.FeatureData(i), 1) = .Weapon(SelectedWeapon).Name Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
						End If
						Exit For
					End If
				Next 
			End If
			
			'選択した武器の種類により、この後のコマンドの進行の仕方が異なる
			If .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ") Then
				SelectedCommand = "マップ攻撃"
			Else
				SelectedCommand = "攻撃"
			End If
			
			'武器の射程を求めておく
			min_range = .Weapon(SelectedWeapon).MinRange
			max_range = .WeaponMaxRange(SelectedWeapon)
			
			'攻撃範囲の表示
			If .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ直") Then
				AreaInCross(.X, .Y, min_range, max_range)
			ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ拡") Then 
				AreaInWideCross(.X, .Y, min_range, max_range)
			ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ扇") Then 
				AreaInSectorCross(.X, .Y, min_range, max_range, .WeaponLevel(SelectedWeapon, "Ｍ扇"))
			ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ全") Or .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ投") Or .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ線") Then 
				AreaInRange(.X, .Y, max_range, min_range, "すべて")
			ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ移") Then 
				AreaInMoveAction(SelectedUnit, max_range)
			Else
				AreaInRange(.X, .Y, max_range, min_range, "味方の敵")
			End If
			
			'射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
			If max_range = 1 And .IsWeaponClassifiedAs(SelectedWeapon, "合") And Not .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ") Then
				For i = 1 To 4
					'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					t = Nothing
					Select Case i
						Case 1
							If .X > 1 Then
								t = MapDataForUnit(.X - 1, .Y)
							End If
						Case 2
							If .X < MapWidth Then
								t = MapDataForUnit(.X + 1, .Y)
							End If
						Case 3
							If .Y > 1 Then
								t = MapDataForUnit(.X, .Y - 1)
							End If
						Case 4
							If .Y < MapHeight Then
								t = MapDataForUnit(.X, .Y + 1)
							End If
					End Select
					
					If Not t Is Nothing Then
						If .IsEnemy(t) Then
							.CombinationPartner("武装", SelectedWeapon, partners, t.X, t.Y)
							If UBound(partners) = 0 Then
								MaskData(t.X, t.Y) = True
							End If
						End If
					End If
				Next 
			End If
			
			'ユニットに対するマスクの設定
			If Not .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ投") And Not .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ線") Then
				For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
					For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
						If MaskData(i, j) Then
							GoTo NextLoop
						End If
						
						t = MapDataForUnit(i, j)
						
						If t Is Nothing Then
							GoTo NextLoop
						End If
						
						'武器の地形適応が有効？
						If .WeaponAdaption(SelectedWeapon, (t.Area)) = 0 Then
							MaskData(i, j) = True
							GoTo NextLoop
						End If
						
						'封印武器の対象属性外でない？
						If .IsWeaponClassifiedAs(SelectedWeapon, "封") Then
							If (.Weapon(SelectedWeapon).Power > 0 And .Damage(SelectedWeapon, t, True) = 0) Or .CriticalProbability(SelectedWeapon, t) = 0 Then
								MaskData(i, j) = True
								GoTo NextLoop
							End If
						End If
						
						'限定武器の対象属性外でない？
						If .IsWeaponClassifiedAs(SelectedWeapon, "限") Then
							If (.Weapon(SelectedWeapon).Power > 0 And .Damage(SelectedWeapon, t, True) = 0) Or (.Weapon(SelectedWeapon).Power = 0 And .CriticalProbability(SelectedWeapon, t) = 0) Then
								MaskData(i, j) = True
								GoTo NextLoop
							End If
						End If
						
						'識別攻撃の場合の処理
						If .IsWeaponClassifiedAs(SelectedWeapon, "識") Or .IsUnderSpecialPowerEffect("識別攻撃") Then
							If .IsAlly(t) Then
								MaskData(i, j) = True
								GoTo NextLoop
							End If
						End If
						
						'ステルス＆隠れ身チェック
						If Not .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ") Then
							If Not .IsTargetWithinRange(SelectedWeapon, t) Then
								MaskData(i, j) = True
								GoTo NextLoop
							End If
						End If
NextLoop: 
					Next 
				Next 
			End If
			MaskData(.X, .Y) = False
			If Not IsOptionDefined("大型マップ") Then
				Center(.X, .Y)
			End If
			MaskScreen()
		End With
		
		'ターゲット選択へ
		If CommandState = "コマンド選択" Then
			CommandState = "ターゲット選択"
		Else
			CommandState = "移動後ターゲット選択"
		End If
		
		'カーソル自動移動を行う？
		If Not AutoMoveCursor Then
			UnlockGUI()
			Exit Sub
		End If
		
		'ＨＰがもっとも低いターゲットを探す
		'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		t = Nothing
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" And (.Party = "敵" Or .Party = "中立") Then
					If MaskData(.X, .Y) = False Then
						If t Is Nothing Then
							t = u
						ElseIf .HP < t.HP Then 
							t = u
						ElseIf .HP = t.HP Then 
							If System.Math.Abs(SelectedUnit.X - .X) ^ 2 + System.Math.Abs(SelectedUnit.Y - .Y) ^ 2 < System.Math.Abs(SelectedUnit.X - t.X) ^ 2 + System.Math.Abs(SelectedUnit.Y - t.Y) ^ 2 Then
								t = u
							End If
						End If
					End If
				End If
			End With
		Next u
		
		'適当なターゲットが見つからなければ自分自身を選択
		If t Is Nothing Then
			t = SelectedUnit
		End If
		
		'カーソルを移動
		MoveCursorPos("ユニット選択", t)
		
		'ターゲットのステータスを表示
		If Not SelectedUnit Is t Then
			DisplayUnitStatus(t)
		End If
		
		UnlockGUI()
	End Sub
	
	'「攻撃」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishAttackCommand()
	Private Sub FinishAttackCommand()
		' MOD END MARGE
		Dim i As Short
		Dim earnings As Integer
		Dim def_mode As String
		Dim u As Unit
		Dim partners() As Unit
		Dim BGM As String
		Dim is_suiside As Boolean
		Dim wname, twname As String
		Dim tx, ty As Short
		Dim attack_target As Unit
		Dim attack_target_hp_ratio As Double
		Dim defense_target As Unit
		Dim defense_target_hp_ratio As Double
		Dim defense_target2 As Unit
		Dim defense_target2_hp_ratio As Double
		Dim support_attack_done As Boolean
		Dim w2 As Short
		' ADD START MARGE
		Dim is_p_weapon As Boolean
		' ADD END MARGE
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		wname = SelectedUnit.Weapon(SelectedWeapon).Name
		SelectedWeaponName = wname
		
		' ADD START MARGE
		'移動後使用後可能な武器か記録しておく
		is_p_weapon = SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "移動後攻撃可")
		' ADD END MARGE
		
		'合体技のパートナーを設定
		With SelectedUnit
			If .IsWeaponClassifiedAs(SelectedWeapon, "合") Then
				If .WeaponMaxRange(SelectedWeapon) = 1 Then
					.CombinationPartner("武装", SelectedWeapon, partners, SelectedTarget.X, SelectedTarget.Y)
				Else
					.CombinationPartner("武装", SelectedWeapon, partners)
				End If
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
		End With
		
		'敵の反撃手段を設定
		UseSupportGuard = True
		SelectedTWeapon = SelectWeapon(SelectedTarget, SelectedUnit, "反撃")
		If SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "間") Then
			SelectedTWeapon = 0
		End If
		If SelectedTWeapon > 0 Then
			twname = SelectedTarget.Weapon(SelectedTWeapon).Name
			SelectedTWeaponName = twname
		Else
			SelectedTWeaponName = ""
		End If
		
		'敵の防御行動を設定
		'UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		def_mode = SelectDefense(SelectedUnit, SelectedWeapon, SelectedTarget, SelectedTWeapon)
		If def_mode <> "" Then
			If SelectedTWeapon > 0 Then
				SelectedTWeapon = -1
			End If
		End If
		SelectedDefenseOption = def_mode
		
		'戦闘前に一旦クリア
		'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit2 = Nothing
		
		'攻撃側の武器使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, wname)
		If IsScenarioFinished Then
			UnlockGUI()
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			WaitCommand()
			Exit Sub
		End If
		
		'敵の武器使用イベント
		If SelectedTWeapon > 0 Then
			SaveSelections()
			SwapSelections()
			HandleEvent("使用", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Then
				IsScenarioFinished = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				ReDim SelectedPartners(0)
				WaitCommand()
				Exit Sub
			End If
		End If
		
		'攻撃イベント
		HandleEvent("攻撃", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			ReDim SelectedPartners(0)
			WaitCommand()
			Exit Sub
		End If
		
		'敵がＢＧＭ能力を持つ場合はＢＧＭを変更
		With SelectedTarget
			If .IsFeatureAvailable("ＢＧＭ") And InStr(.MainPilot.Name, "(ザコ)") = 0 Then
				BGM = SearchMidiFile(.FeatureData("ＢＧＭ"))
				If Len(BGM) > 0 Then
					BossBGM = False
					ChangeBGM(BGM)
					BossBGM = True
				End If
			End If
		End With
		
		'そうではなく、ボス用ＢＧＭが流れていれば味方のＢＧＭに切り替え
		If Len(BGM) = 0 And BossBGM Then
			BossBGM = False
			BGM = ""
			With SelectedUnit
				If .IsFeatureAvailable("武器ＢＧＭ") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "武器ＢＧＭ" And LIndex(.FeatureData(i), 1) = .Weapon(SelectedWeapon).Name Then
							BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
							Exit For
						End If
					Next 
				End If
				If Len(BGM) = 0 Then
					If .IsFeatureAvailable("ＢＧＭ") Then
						BGM = SearchMidiFile(.FeatureData("ＢＧＭ"))
					End If
				End If
				If Len(BGM) = 0 Then
					BGM = SearchMidiFile(.MainPilot.BGM)
				End If
				If Len(BGM) = 0 Then
					BGM = BGMName("default")
				End If
				ChangeBGM(BGM)
			End With
		End If
		
		With SelectedUnit
			'攻撃参加ユニット以外はマスク
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Then
						MaskData(.X, .Y) = True
					End If
				End With
			Next u
			
			'合体技パートナーのハイライト表示
			For i = 1 To UBound(partners)
				With partners(i)
					MaskData(.X, .Y) = False
				End With
			Next 
			
			MaskData(.X, .Y) = False
			MaskData(SelectedTarget.X, SelectedTarget.Y) = False
			If Not BattleAnimation Then
				MaskScreen()
			End If
		End With
		
		'イベント用に戦闘に参加するユニットの情報を記録しておく
		AttackUnit = SelectedUnit
		attack_target = SelectedUnit
		attack_target_hp_ratio = SelectedUnit.HP / SelectedUnit.MaxHP
		defense_target = SelectedTarget
		defense_target_hp_ratio = SelectedTarget.HP / SelectedTarget.MaxHP
		'UPGRADE_NOTE: オブジェクト defense_target2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		defense_target2 = Nothing
		'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit2 = Nothing
		
		'ターゲットの位置を記録
		tx = SelectedTarget.X
		ty = SelectedTarget.Y
		
		OpenMessageForm(SelectedTarget, SelectedUnit)
		
		'相手の先制攻撃？
		With SelectedTarget
			' MOD START MARGE
			'        If SelectedTWeapon > 0 And .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "移動前") Then
			'SelectedTWeapon > 0の判定は、IsWeaponAvailableで行うようにした
			If .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "移動前") Then
				' MOD END MARGE
				If Not .IsWeaponClassifiedAs(SelectedTWeapon, "後") Then
					If SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "後") Then
						def_mode = "先制攻撃"
						
						If .IsWeaponClassifiedAs(SelectedTWeapon, "自") Then
							is_suiside = True
						End If
						
						'先制攻撃攻撃を実施
						.Attack(SelectedTWeapon, SelectedUnit, "先制攻撃", "")
						SelectedTarget = .CurrentForm
					ElseIf .IsWeaponClassifiedAs(SelectedTWeapon, "先") Or .MainPilot.SkillLevel("先読み") >= Dice(16) Or .IsUnderSpecialPowerEffect("カウンター") Then 
						def_mode = "先制攻撃"
						
						If .IsWeaponClassifiedAs(SelectedTWeapon, "自") Then
							is_suiside = True
						End If
						
						'カウンター攻撃を実施
						.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "")
						SelectedTarget = .CurrentForm
					ElseIf .MaxCounterAttack > .UsedCounterAttack Then 
						def_mode = "先制攻撃"
						
						If .IsWeaponClassifiedAs(SelectedTWeapon, "自") Then
							is_suiside = True
						End If
						
						'カウンター攻撃の残り回数を減少
						.UsedCounterAttack = .UsedCounterAttack + 1
						
						'カウンター攻撃を実施
						.Attack(SelectedTWeapon, SelectedUnit, "カウンター", "")
						SelectedTarget = .CurrentForm
					End If
				End If
				
				'攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
				If Not SupportGuardUnit2 Is Nothing Then
					attack_target = SupportGuardUnit2
					attack_target_hp_ratio = SupportGuardUnitHPRatio2
				End If
			End If
		End With
		
		'サポートアタックのパートナーを探す
		With SelectedUnit
			If .Status_Renamed = "出撃" And SelectedTarget.Status_Renamed = "出撃" And UseSupportAttack Then
				SupportAttackUnit = .LookForSupportAttack(SelectedTarget)
				
				'合体技ではサポートアタック不能
				If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
					If .IsWeaponClassifiedAs(SelectedWeapon, "合") Then
						'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						SupportAttackUnit = Nothing
					End If
				End If
				
				'魅了された場合
				If .IsConditionSatisfied("魅了") And .Master Is SelectedTarget Then
					'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					SupportAttackUnit = Nothing
				End If
				
				'憑依された場合
				If .IsConditionSatisfied("憑依") Then
					If .Master.Party = SelectedTarget.Party Then
						'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						SupportAttackUnit = Nothing
					End If
				End If
				
				'踊らされた場合
				If .IsConditionSatisfied("踊り") Then
					'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					SupportAttackUnit = Nothing
				End If
			End If
		End With
		
		'攻撃の実施
		With SelectedUnit
			If .Status_Renamed = "出撃" And .MaxAction(True) > 0 And Not .IsConditionSatisfied("攻撃不能") And SelectedTarget.Status_Renamed = "出撃" Then
				'まだ武器は使用可能か？
				If SelectedWeapon > .CountWeapon Then
					SelectedWeapon = -1
				ElseIf wname <> .Weapon(SelectedWeapon).Name Then 
					SelectedWeapon = -1
				ElseIf CommandState = "移動後ターゲット選択" Then 
					If Not .IsWeaponAvailable(SelectedWeapon, "移動後") Then
						SelectedWeapon = -1
					End If
				Else
					If Not .IsWeaponAvailable(SelectedWeapon, "移動前") Then
						SelectedWeapon = -1
					End If
				End If
				If SelectedWeapon > 0 Then
					If Not .IsTargetWithinRange(SelectedWeapon, SelectedTarget) Then
						SelectedWeapon = 0
					End If
				End If
				
				'魅了された場合
				If .IsConditionSatisfied("魅了") And .Master Is SelectedTarget Then
					SelectedWeapon = -1
				End If
				
				'憑依された場合
				If .IsConditionSatisfied("憑依") Then
					If .Master.Party = SelectedTarget.Party0 Then
						SelectedWeapon = -1
					End If
				End If
				
				'踊らされた場合
				If .IsConditionSatisfied("踊り") Then
					SelectedWeapon = -1
				End If
				
				If SelectedWeapon > 0 Then
					If Not SupportAttackUnit Is Nothing And .MaxSyncAttack > .UsedSyncAttack Then
						'同時援護攻撃
						.Attack(SelectedWeapon, SelectedTarget, "統率", def_mode)
					Else
						'通常攻撃
						.Attack(SelectedWeapon, SelectedTarget, "", def_mode)
					End If
				ElseIf SelectedWeapon = 0 Then 
					If .IsAnimationDefined("射程外") Then
						.PlayAnimation("射程外")
					Else
						.SpecialEffect("射程外")
					End If
					.PilotMessage("射程外")
				End If
			Else
				SelectedWeapon = -1
			End If
			SelectedUnit = .CurrentForm
			
			'防御側のユニットがかばわれた場合は2番目の防御側ユニットとして記録
			If Not SupportGuardUnit Is Nothing Then
				defense_target2 = SupportGuardUnit
				defense_target2_hp_ratio = SupportGuardUnitHPRatio
			End If
		End With
		
		'同時攻撃
		If Not SupportAttackUnit Is Nothing Then
			If SupportAttackUnit.Status_Renamed <> "出撃" Or SelectedUnit.Status_Renamed <> "出撃" Or SelectedTarget.Status_Renamed <> "出撃" Then
				'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SupportAttackUnit = Nothing
			End If
		End If
		If Not SupportAttackUnit Is Nothing Then
			If SelectedUnit.MaxSyncAttack > SelectedUnit.UsedSyncAttack Then
				With SupportAttackUnit
					'サポートアタックに使う武器を決定
					w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "サポートアタック")
					
					If w2 > 0 Then
						'サポートアタックを実施
						MaskData(.X, .Y) = False
						If Not BattleAnimation Then
							MaskScreen()
						End If
						If .IsAnimationDefined("サポートアタック開始") Then
							.PlayAnimation("サポートアタック開始")
						End If
						UpdateMessageForm(SelectedTarget, SupportAttackUnit)
						.Attack(w2, SelectedTarget, "同時援護攻撃", def_mode)
					End If
				End With
				
				'後始末
				With SupportAttackUnit.CurrentForm
					If w2 > 0 Then
						If .IsAnimationDefined("サポートアタック終了") Then
							.PlayAnimation("サポートアタック終了")
						End If
						
						'サポートアタックの残り回数を減らす
						.UsedSupportAttack = .UsedSupportAttack + 1
						
						'同時援護攻撃の残り回数を減らす
						SelectedUnit.UsedSyncAttack = SelectedUnit.UsedSyncAttack + 1
					End If
				End With
				
				support_attack_done = True
				
				'防御側のユニットがかばわれた場合は本来の防御ユニットデータと
				'入れ替えて記録
				If Not SupportGuardUnit Is Nothing Then
					defense_target = SupportGuardUnit
					defense_target_hp_ratio = SupportGuardUnitHPRatio
				End If
			End If
		End If
		
		'反撃の実施
		With SelectedTarget
			If def_mode <> "先制攻撃" Then
				If .Status_Renamed = "出撃" And .Party <> "味方" And SelectedUnit.Status_Renamed = "出撃" Then
					'まだ武器は使用可能か？
					If SelectedTWeapon > 0 Then
						If SelectedTWeapon > .CountWeapon Then
							SelectedTWeapon = -1
						ElseIf twname <> .Weapon(SelectedTWeapon).Name Or Not .IsWeaponAvailable(SelectedTWeapon, "移動前") Then 
							SelectedTWeapon = -1
						End If
					End If
					If SelectedTWeapon > 0 Then
						If Not .IsTargetWithinRange(SelectedTWeapon, SelectedUnit) Then
							'敵が射程外に逃げていたら武器を再選択
							SelectedTWeapon = SelectWeapon(SelectedTarget, SelectedUnit, "反撃")
						End If
					End If
					
					'行動不能な場合
					If .MaxAction = 0 Then
						SelectedTWeapon = -1
					End If
					
					'魅了された場合
					If .IsConditionSatisfied("魅了") And .Master Is SelectedUnit Then
						SelectedTWeapon = -1
					End If
					
					'憑依された場合
					If .IsConditionSatisfied("憑依") Then
						If .Master.Party = SelectedUnit.Party Then
							SelectedWeapon = -1
						End If
					End If
					
					'踊らされた場合
					If .IsConditionSatisfied("踊り") Then
						SelectedTWeapon = -1
					End If
					
					If SelectedTWeapon > 0 And def_mode = "" Then
						'反撃を実施
						If .IsWeaponClassifiedAs(SelectedTWeapon, "自") Then
							is_suiside = True
						End If
						.Attack(SelectedTWeapon, SelectedUnit, "", "")
						
						'攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
						If Not SupportGuardUnit2 Is Nothing Then
							attack_target = SupportGuardUnit2
							attack_target_hp_ratio = SupportGuardUnitHPRatio2
						End If
					ElseIf SelectedTWeapon = 0 And .X = tx And .Y = ty Then 
						'反撃出来る武器がなかった場合は射程外メッセージを表示
						If .IsAnimationDefined("射程外") Then
							.PlayAnimation("射程外")
						Else
							.SpecialEffect("射程外")
						End If
						.PilotMessage("射程外")
					Else
						SelectedTWeapon = -1
					End If
				Else
					SelectedTWeapon = -1
				End If
			End If
		End With
		
		'サポートアタック
		If Not SupportAttackUnit Is Nothing Then
			If SupportAttackUnit.Status_Renamed <> "出撃" Or SelectedUnit.Status_Renamed <> "出撃" Or SelectedTarget.Status_Renamed <> "出撃" Or support_attack_done Then
				'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SupportAttackUnit = Nothing
			End If
		End If
		If Not SupportAttackUnit Is Nothing Then
			With SupportAttackUnit
				'サポートアタックに使う武器を決定
				w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "サポートアタック")
				
				If w2 > 0 Then
					'サポートアタックを実施
					MaskData(.X, .Y) = False
					If Not BattleAnimation Then
						MaskScreen()
					End If
					If .IsAnimationDefined("サポートアタック開始") Then
						.PlayAnimation("サポートアタック開始")
					End If
					UpdateMessageForm(SelectedTarget, SupportAttackUnit)
					.Attack(w2, SelectedTarget, "援護攻撃", def_mode)
				End If
			End With
			
			'後始末
			With SupportAttackUnit.CurrentForm
				If .IsAnimationDefined("サポートアタック終了") Then
					.PlayAnimation("サポートアタック終了")
				End If
				
				'サポートアタックの残り回数を減らす
				If w2 > 0 Then
					.UsedSupportAttack = .UsedSupportAttack + 1
				End If
			End With
			
			'防御側のユニットがかばわれた場合は本来の防御ユニットデータと
			'入れ替えて記録
			If Not SupportGuardUnit Is Nothing Then
				defense_target = SupportGuardUnit
				defense_target_hp_ratio = SupportGuardUnitHPRatio
			End If
		End If
		
		SelectedTarget = SelectedTarget.CurrentForm
		
		With SelectedUnit
			If .Status_Renamed = "出撃" Then
				'攻撃をかけたユニットがまだ生き残っていれば経験値＆資金を獲得
				
				If SelectedTarget.Status_Renamed = "破壊" And Not is_suiside Then
					'敵を破壊した場合
					
					'経験値を獲得
					.GetExp(SelectedTarget, "破壊")
					If Not IsOptionDefined("合体技パートナー経験値無効") Then
						For i = 1 To UBound(partners)
							partners(i).CurrentForm.GetExp(SelectedTarget, "破壊", "パートナー")
						Next 
					End If
					
					'獲得する資金を算出
					earnings = SelectedTarget.Value \ 2
					
					'スペシャルパワーによる獲得資金増加
					If .IsUnderSpecialPowerEffect("獲得資金増加") Then
						earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("獲得資金増加"))
					End If
					
					'パイロット能力による獲得資金増加
					If .IsSkillAvailable("資金獲得") Then
						If Not .IsUnderSpecialPowerEffect("獲得資金増加") Or IsOptionDefined("収得効果重複") Then
							earnings = MinDbl(earnings * ((10 + .SkillLevel("資金獲得", 5)) / 10), 999999999)
						End If
					End If
					
					'資金を獲得
					IncrMoney(earnings)
					
					If earnings > 0 Then
						DisplaySysMessage(VB6.Format(earnings) & "の" & Term("資金", SelectedUnit) & "を得た。")
					End If
					
					'スペシャルパワー効果「敵破壊時再行動」
					If .IsUnderSpecialPowerEffect("敵破壊時再行動") Then
						.UsedAction = .UsedAction - 1
					End If
				Else
					'相手を破壊できなかった場合
					
					'経験値を獲得
					.GetExp(SelectedTarget, "攻撃")
					If Not IsOptionDefined("合体技パートナー経験値無効") Then
						For i = 1 To UBound(partners)
							partners(i).CurrentForm.GetExp(SelectedTarget, "攻撃", "パートナー")
						Next 
					End If
				End If
				
				'スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
				.RemoveSpecialPowerInEffect("戦闘終了")
				If earnings > 0 Then
					.RemoveSpecialPowerInEffect("敵破壊")
				End If
			End If
		End With
		
		With SelectedTarget
			If .Status_Renamed = "出撃" Then
				'持続期間が「戦闘終了」のスペシャルパワー効果を削除
				.RemoveSpecialPowerInEffect("戦闘終了")
			End If
		End With
		
		CloseMessageForm()
		ClearUnitStatus()
		
		'状態＆データ更新
		With attack_target.CurrentForm
			.UpdateCondition()
			.Update()
		End With
		If Not SupportAttackUnit Is Nothing Then
			With SupportAttackUnit.CurrentForm
				.UpdateCondition()
				.Update()
			End With
		End If
		With defense_target.CurrentForm
			.UpdateCondition()
			.Update()
		End With
		If Not defense_target2 Is Nothing Then
			With defense_target2.CurrentForm
				.UpdateCondition()
				.Update()
			End With
		End If
		
		'破壊＆損傷率イベント発生
		
		If SelectedWeapon <= 0 Then
			SelectedWeaponName = ""
		End If
		If SelectedTWeapon <= 0 Then
			SelectedTWeaponName = ""
		End If
		
		'攻撃を受けた攻撃側ユニット
		With attack_target.CurrentForm
			If .Status_Renamed = "破壊" Then
				HandleEvent("破壊", .MainPilot.ID)
			ElseIf .Status_Renamed = "出撃" And .HP / .MaxHP < attack_target_hp_ratio Then 
				HandleEvent("損傷率", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
			End If
		End With
		If IsScenarioFinished Then
			UnlockGUI()
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			GoTo EndAttack
		End If
		
		SelectedUnit = SelectedUnit.CurrentForm
		
		'ターゲット側のイベント処理を行うためにユニットの入れ替えを行う
		SaveSelections()
		SwapSelections()
		
		'攻撃を受けた防御側ユニット
		With defense_target.CurrentForm
			If .CountPilot > 0 Then
				If .Status_Renamed = "破壊" Then
					HandleEvent("破壊", .MainPilot.ID)
				ElseIf .Status_Renamed = "出撃" And .HP / .MaxHP < defense_target_hp_ratio Then 
					HandleEvent("損傷率", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
				End If
			End If
		End With
		
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'攻撃を受けた防御側ユニットその2
		If Not defense_target2 Is Nothing Then
			If Not defense_target2.CurrentForm Is defense_target.CurrentForm Then
				With defense_target2.CurrentForm
					If .CountPilot > 0 Then
						If .Status_Renamed = "破壊" Then
							HandleEvent("破壊", .MainPilot.ID)
						ElseIf .Status_Renamed = "出撃" And .HP / .MaxHP < defense_target2_hp_ratio Then 
							HandleEvent("損傷率", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
						End If
					End If
				End With
			End If
		End If
		
		RestoreSelections()
		
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			ReDim SelectedPartners(0)
			UnlockGUI()
			Exit Sub
		End If
		
		'武器の使用後イベント
		If SelectedUnit.Status_Renamed = "出撃" And SelectedWeapon > 0 Then
			HandleEvent("使用後", SelectedUnit.MainPilot.ID, wname)
			If IsScenarioFinished Then
				IsScenarioFinished = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
		End If
		
		If SelectedTarget.Status_Renamed = "出撃" And SelectedTWeapon > 0 Then
			SaveSelections()
			SwapSelections()
			HandleEvent("使用後", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Then
				IsScenarioFinished = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
		End If
		
		'攻撃後イベント
		If SelectedUnit.Status_Renamed = "出撃" And SelectedTarget.Status_Renamed = "出撃" Then
			HandleEvent("攻撃後", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
			If IsScenarioFinished Then
				IsScenarioFinished = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
		End If
		
		'もし敵が移動していれば進入イベント
		With SelectedTarget
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
			If .Status_Renamed = "出撃" Then
				If .X <> tx Or .Y <> ty Then
					HandleEvent("進入", .MainPilot.ID, .X, .Y)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						ReDim SelectedPartners(0)
						UnlockGUI()
						Exit Sub
					End If
					If IsCanceled Then
						IsCanceled = False
						ReDim SelectedPartners(0)
						UnlockGUI()
						Exit Sub
					End If
				End If
			End If
		End With
		
EndAttack: 
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		'ハイパーモード＆ノーマルモードの自動発動をチェック
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
		
		' ADD START MARGE
		'再移動
		If is_p_weapon And SelectedUnit.Status_Renamed = "出撃" Then
			If SelectedUnit.MainPilot.IsSkillAvailable("遊撃") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
				'進入イベント
				If SelectedUnitMoveCost > 0 Then
					HandleEvent("進入", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						Exit Sub
					End If
				End If
				
				'ユニットが既に出撃していない？
				If SelectedUnit.Status_Renamed <> "出撃" Then
					RedrawScreen()
					ClearUnitStatus()
					Exit Sub
				End If
				
				SelectedCommand = "再移動"
				AreaInSpeed(SelectedUnit)
				If Not IsOptionDefined("大型マップ") Then
					Center(SelectedUnit.X, SelectedUnit.Y)
				End If
				MaskScreen()
				If NewGUIMode Then
					System.Windows.Forms.Application.DoEvents()
					ClearUnitStatus()
				Else
					DisplayUnitStatus(SelectedUnit)
				End If
				CommandState = "ターゲット選択"
				Exit Sub
			End If
		End If
		' ADD END MARGE
		
		'行動数を減らす
		WaitCommand()
	End Sub
	
	'マップ攻撃による「攻撃」コマンドを終了
	' MOD START MARGE
	'Public Sub MapAttackCommand()
	Private Sub MapAttackCommand()
		' MOD END MARGE
		Dim i As Short
		Dim partners() As Unit
		' ADD START MARGE
		Dim is_p_weapon As Boolean
		' ADD END MARGE
		
		With SelectedUnit
			' ADD START MARGE
			'移動後使用後可能な武器か記録しておく
			is_p_weapon = .IsWeaponClassifiedAs(SelectedWeapon, "移動後攻撃可")
			' ADD END MARGE
			'攻撃目標地点を選択して初めて攻撃範囲が分かるタイプのマップ攻撃
			'の場合は再度プレイヤーの選択を促す必要がある
			If CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択" Then
				If .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ投") Then
					If CommandState = "ターゲット選択" Then
						CommandState = "マップ攻撃使用"
					Else
						CommandState = "移動後マップ攻撃使用"
					End If
					
					'攻撃目標地点
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					'攻撃範囲を設定
					If .IsWeaponClassifiedAs(SelectedWeapon, "識") Or .IsUnderSpecialPowerEffect("識別攻撃") Then
						AreaInRange(SelectedX, SelectedY, .WeaponLevel(SelectedWeapon, "Ｍ投"), 1, "味方の敵")
					Else
						AreaInRange(SelectedX, SelectedY, .WeaponLevel(SelectedWeapon, "Ｍ投"), 1, "すべて")
					End If
					MaskScreen()
					Exit Sub
				ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ移") Then 
					'攻撃目標地点
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					'攻撃目標地点に他のユニットがいては駄目
					If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
						MaskScreen()
						Exit Sub
					End If
					
					If CommandState = "ターゲット選択" Then
						CommandState = "マップ攻撃使用"
					Else
						CommandState = "移動後マップ攻撃使用"
					End If
					
					'攻撃範囲を設定
					AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
					MaskScreen()
					Exit Sub
				ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Ｍ線") Then 
					If CommandState = "ターゲット選択" Then
						CommandState = "マップ攻撃使用"
					Else
						CommandState = "移動後マップ攻撃使用"
					End If
					
					'攻撃目標地点
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					'攻撃範囲を設定
					AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
					MaskScreen()
					Exit Sub
				End If
			End If
			
			'合体技パートナーの設定
			If .IsWeaponClassifiedAs(SelectedWeapon, "合") Then
				.CombinationPartner("武装", SelectedWeapon, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			If MainWidth <> 15 Then
				ClearUnitStatus()
			End If
			
			LockGUI()
			
			SelectedTWeapon = 0
			
			'マップ攻撃による攻撃を行う
			.MapAttack(SelectedWeapon, SelectedX, SelectedY)
			SelectedUnit = .CurrentForm
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
			
			If IsScenarioFinished Then
				IsScenarioFinished = False
				ReDim SelectedPartners(0)
				UnlockGUI()
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				ReDim SelectedPartners(0)
				WaitCommand()
				Exit Sub
			End If
		End With
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		' ADD START MARGE
		'再移動
		If is_p_weapon And SelectedUnit.Status_Renamed = "出撃" Then
			If SelectedUnit.MainPilot.IsSkillAvailable("遊撃") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
				'進入イベント
				If SelectedUnitMoveCost > 0 Then
					HandleEvent("進入", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						Exit Sub
					End If
				End If
				
				'ユニットが既に出撃していない？
				If SelectedUnit.Status_Renamed <> "出撃" Then
					RedrawScreen()
					ClearUnitStatus()
					Exit Sub
				End If
				
				SelectedCommand = "再移動"
				AreaInSpeed(SelectedUnit)
				If Not IsOptionDefined("大型マップ") Then
					Center(SelectedUnit.X, SelectedUnit.Y)
				End If
				MaskScreen()
				If NewGUIMode Then
					System.Windows.Forms.Application.DoEvents()
					ClearUnitStatus()
				Else
					DisplayUnitStatus(SelectedUnit)
				End If
				CommandState = "ターゲット選択"
				Exit Sub
			End If
		End If
		' ADD END MARGE
		
		'行動終了
		WaitCommand()
	End Sub
	
	
	'「アビリティ」コマンドを開始
	' is_item=True の場合は「アイテム」コマンドによる使い捨てアイテムのアビリティ
	' MOD STAR MARGE
	'Public Sub StartAbilityCommand(Optional ByVal is_item As Boolean)
	Private Sub StartAbilityCommand(Optional ByVal is_item As Boolean = False)
		' MOD END MARGE
		Dim i, j As Short
		Dim t, u As Unit
		Dim min_range, max_range As Short
		Dim cap As String
		
		LockGUI()
		
		'使用するアビリティを選択
		If is_item Then
			cap = "アイテム選択"
		Else
			cap = Term("アビリティ", SelectedUnit) & "選択"
		End If
		If CommandState = "コマンド選択" Then
			SelectedAbility = AbilityListBox(SelectedUnit, cap, "移動前", is_item)
		Else
			SelectedAbility = AbilityListBox(SelectedUnit, cap, "移動後", is_item)
		End If
		
		'キャンセル
		If SelectedAbility = 0 Then
			If AutoMoveCursor Then
				RestoreCursorPos()
			End If
			CancelCommand()
			UnlockGUI()
			Exit Sub
		End If
		
		'アビリティ専用ＢＧＭがあればそれを演奏
		Dim BGM As String
		With SelectedUnit
			If .IsFeatureAvailable("アビリティＢＧＭ") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "アビリティＢＧＭ" And LIndex(.FeatureData(i), 1) = .Ability(SelectedAbility).Name Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
						End If
						Exit For
					End If
				Next 
			End If
		End With
		
		'射程0のアビリティはその場で実行
		Dim is_transformation As Boolean
		If SelectedUnit.Ability(SelectedAbility).MaxRange = 0 Then
			
			SelectedTarget = SelectedUnit
			
			'変身アビリティであるか判定
			For i = 1 To SelectedUnit.Ability(SelectedAbility).CountEffect
				If SelectedUnit.Ability(SelectedAbility).EffectType(i) = "変身" Then
					is_transformation = True
					Exit For
				End If
			Next 
			
			SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name
			
			'使用イベント
			HandleEvent("使用", SelectedUnit.MainPilot.ID, SelectedAbilityName)
			If IsScenarioFinished Then
				IsScenarioFinished = False
				UnlockGUI()
				Exit Sub
			End If
			If IsCanceled Then
				IsCanceled = False
				WaitCommand()
				Exit Sub
			End If
			
			'アビリティを実行
			SelectedUnit.ExecuteAbility(SelectedAbility, SelectedUnit)
			SelectedUnit = SelectedUnit.CurrentForm
			CloseMessageForm()
			
			'破壊イベント
			With SelectedUnit
				If .Status_Renamed = "破壊" Then
					If .CountPilot > 0 Then
						HandleEvent("破壊", .MainPilot.ID)
						If IsScenarioFinished Then
							IsScenarioFinished = False
							UnlockGUI()
							Exit Sub
						End If
						If IsCanceled Then
							IsCanceled = False
							UnlockGUI()
							Exit Sub
						End If
					End If
					WaitCommand()
					Exit Sub
				End If
			End With
			
			'使用後イベント
			With SelectedUnit
				If .CountPilot > 0 Then
					HandleEvent("使用後", .MainPilot.ID, SelectedAbilityName)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						UnlockGUI()
						Exit Sub
					End If
					If IsCanceled Then
						IsCanceled = False
						UnlockGUI()
						Exit Sub
					End If
				End If
			End With
			
			'変身アビリティの場合は行動終了しない
			If Not is_transformation Or CommandState = "移動後コマンド選択" Then
				WaitCommand()
			Else
				If SelectedUnit.Status_Renamed = "出撃" Then
					'カーソル自動移動
					If AutoMoveCursor Then
						MoveCursorPos("ユニット選択", SelectedUnit)
					End If
					DisplayUnitStatus(SelectedUnit)
				Else
					ClearUnitStatus()
				End If
				CommandState = "ユニット選択"
				UnlockGUI()
			End If
			Exit Sub
		End If
		
		Dim partners() As Unit
		With SelectedUnit
			'マップ型アビリティかどうかで今後のコマンド処理の進行の仕方が異なる
			If is_item Then
				If .IsAbilityClassifiedAs(SelectedAbility, "Ｍ") Then
					SelectedCommand = "マップアイテム"
				Else
					SelectedCommand = "アイテム"
				End If
			Else
				If .IsAbilityClassifiedAs(SelectedAbility, "Ｍ") Then
					SelectedCommand = "マップアビリティ"
				Else
					SelectedCommand = "アビリティ"
				End If
			End If
			
			'アビリティの射程を求めておく
			min_range = .AbilityMinRange(SelectedAbility)
			max_range = .AbilityMaxRange(SelectedAbility)
			
			'アビリティの効果範囲を設定
			If .IsAbilityClassifiedAs(SelectedAbility, "Ｍ直") Then
				AreaInCross(.X, .Y, min_range, max_range)
			ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Ｍ拡") Then 
				AreaInWideCross(.X, .Y, min_range, max_range)
			ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Ｍ扇") Then 
				AreaInSectorCross(.X, .Y, min_range, max_range, .AbilityLevel(SelectedAbility, "Ｍ扇"))
			ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Ｍ移") Then 
				AreaInMoveAction(SelectedUnit, max_range)
			Else
				AreaInRange(.X, .Y, max_range, min_range, "すべて")
			End If
			
			'射程１の合体技はパートナーで相手を取り囲んでいないと使用できない
			If .IsAbilityClassifiedAs(SelectedAbility, "合") And Not .IsAbilityClassifiedAs(SelectedAbility, "Ｍ") And .Ability(SelectedAbility).MaxRange = 1 Then
				For i = 1 To 4
					'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					t = Nothing
					Select Case i
						Case 1
							If .X > 1 Then
								t = MapDataForUnit(.X - 1, .Y)
							End If
						Case 2
							If .X < MapWidth Then
								t = MapDataForUnit(.X + 1, .Y)
							End If
						Case 3
							If .Y > 1 Then
								t = MapDataForUnit(.X, .Y - 1)
							End If
						Case 4
							If .Y < MapHeight Then
								t = MapDataForUnit(.X, .Y + 1)
							End If
					End Select
					
					If Not t Is Nothing Then
						If .IsAlly(t) Then
							.CombinationPartner("アビリティ", SelectedAbility, partners, t.X, t.Y)
							If UBound(partners) = 0 Then
								MaskData(t.X, t.Y) = True
							End If
						End If
					End If
				Next 
			End If
			
			'ユニットがいるマスの処理
			If Not .IsAbilityClassifiedAs(SelectedAbility, "Ｍ投") And Not .IsAbilityClassifiedAs(SelectedAbility, "Ｍ線") And Not .IsAbilityClassifiedAs(SelectedAbility, "Ｍ移") Then
				For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
					For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
						If Not MaskData(i, j) Then
							t = MapDataForUnit(i, j)
							If Not t Is Nothing Then
								'有効？
								If .IsAbilityEffective(SelectedAbility, t) Then
									MaskData(i, j) = False
								Else
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			End If
			
			'支援専用アビリティは自分には使用できない
			If Not MaskData(.X, .Y) Then
				If .IsAbilityClassifiedAs(SelectedAbility, "援") Then
					MaskData(.X, .Y) = True
				End If
			End If
			
			If Not IsOptionDefined("大型マップ") Then
				Center(.X, .Y)
			End If
			MaskScreen()
		End With
		
		If CommandState = "コマンド選択" Then
			CommandState = "ターゲット選択"
		Else
			CommandState = "移動後ターゲット選択"
		End If
		
		'カーソル自動移動を行う？
		If Not AutoMoveCursor Then
			UnlockGUI()
			Exit Sub
		End If
		
		'自分から最も近い味方ユニットを探す
		'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		t = Nothing
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" And .Party = "味方" Then
					If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
						If t Is Nothing Then
							t = u
						Else
							If System.Math.Abs(SelectedUnit.X - .X) ^ 2 + System.Math.Abs(SelectedUnit.Y - .Y) ^ 2 < System.Math.Abs(SelectedUnit.X - t.X) ^ 2 + System.Math.Abs(SelectedUnit.Y - t.Y) ^ 2 Then
								t = u
							End If
						End If
					End If
				End If
			End With
		Next u
		
		'適当がユニットがなければ自分自身を選択
		If t Is Nothing Then
			t = SelectedUnit
		End If
		
		'カーソルを移動
		MoveCursorPos("ユニット選択", t)
		
		'ターゲットのステータスを表示
		If Not SelectedUnit Is t Then
			DisplayUnitStatus(t)
		End If
		
		UnlockGUI()
	End Sub
	
	'「アビリティ」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishAbilityCommand()
	Private Sub FinishAbilityCommand()
		' MOD END MARGE
		Dim i As Short
		Dim u As Unit
		Dim partners() As Unit
		Dim aname As String
		' ADD START MARGE
		Dim is_p_ability As Boolean
		' ADD END MARGE
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		'合体技のパートナーを設定
		With SelectedUnit
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				If .AbilityMaxRange(SelectedAbility) = 1 Then
					.CombinationPartner("アビリティ", SelectedAbility, partners, SelectedTarget.X, SelectedTarget.Y)
				Else
					.CombinationPartner("アビリティ", SelectedAbility, partners)
				End If
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
		End With
		
		aname = SelectedUnit.Ability(SelectedAbility).Name
		SelectedAbilityName = aname
		
		' ADD START MARGE
		'移動後使用後可能なアビリティか記録しておく
		is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "Ｐ") Or (SelectedUnit.AbilityMaxRange(SelectedAbility) = 1 And Not SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "Ｑ"))
		' ADD END MARGE
		
		'使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, aname)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			ReDim SelectedPartners(0)
			WaitCommand()
			Exit Sub
		End If
		
		With SelectedUnit
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Then
						MaskData(.X, .Y) = True
					End If
				End With
			Next u
			
			'合体技パートナーのハイライト表示
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				For i = 1 To UBound(partners)
					With partners(i)
						MaskData(.X, .Y) = False
					End With
				Next 
			End If
			
			MaskData(.X, .Y) = False
			MaskData(SelectedTarget.X, SelectedTarget.Y) = False
			If Not BattleAnimation Then
				MaskScreen()
			End If
			
			'アビリティを実行
			.ExecuteAbility(SelectedAbility, SelectedTarget)
			SelectedUnit = .CurrentForm
			
			CloseMessageForm()
			ClearUnitStatus()
		End With
		
		'破壊イベント
		With SelectedUnit
			If .Status_Renamed = "破壊" Then
				If .CountPilot > 0 Then
					HandleEvent("破壊", .MainPilot.ID)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						ReDim SelectedPartners(0)
						UnlockGUI()
						Exit Sub
					End If
					If IsCanceled Then
						IsCanceled = False
						ReDim SelectedPartners(0)
						UnlockGUI()
						Exit Sub
					End If
				End If
				WaitCommand()
				Exit Sub
			End If
		End With
		
		'使用後イベント
		With SelectedUnit
			If .CountPilot > 0 Then
				HandleEvent("使用後", .MainPilot.ID, aname)
				If IsScenarioFinished Then
					IsScenarioFinished = False
					ReDim SelectedPartners(0)
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ReDim SelectedPartners(0)
					UnlockGUI()
					Exit Sub
				End If
			End If
		End With
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		' ADD START MARGE
		'再移動
		If is_p_ability And SelectedUnit.Status_Renamed = "出撃" Then
			If SelectedUnit.MainPilot.IsSkillAvailable("遊撃") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
				'進入イベント
				If SelectedUnitMoveCost > 0 Then
					HandleEvent("進入", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						Exit Sub
					End If
				End If
				
				'ユニットが既に出撃していない？
				If SelectedUnit.Status_Renamed <> "出撃" Then
					RedrawScreen()
					ClearUnitStatus()
					Exit Sub
				End If
				
				SelectedCommand = "再移動"
				AreaInSpeed(SelectedUnit)
				If Not IsOptionDefined("大型マップ") Then
					Center(SelectedUnit.X, SelectedUnit.Y)
				End If
				MaskScreen()
				If NewGUIMode Then
					System.Windows.Forms.Application.DoEvents()
					ClearUnitStatus()
				Else
					DisplayUnitStatus(SelectedUnit)
				End If
				CommandState = "ターゲット選択"
				Exit Sub
			End If
		End If
		' ADD END MARGE
		
		'行動終了
		WaitCommand()
	End Sub
	
	'マップ型「アビリティ」コマンドを終了
	' MOD START MARGE
	'Public Sub MapAbilityCommand()
	Private Sub MapAbilityCommand()
		' MOD END MARGE
		Dim i As Short
		Dim partners() As Unit
		' ADD START MARGE
		Dim is_p_ability As Boolean
		' ADD END MARGE
		
		With SelectedUnit
			' ADD START MARGE
			'移動後使用後可能なアビリティか記録しておく
			is_p_ability = .IsAbilityClassifiedAs(SelectedAbility, "Ｐ") Or (.AbilityMaxRange(SelectedAbility) = 1 And Not .IsAbilityClassifiedAs(SelectedAbility, "Ｑ"))
			' ADD END MARGE
			
			'目標地点を選択して初めて効果範囲が分かるタイプのマップアビリティ
			'の場合は再度プレイヤーの選択を促す必要がある
			If CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択" Then
				If .IsAbilityClassifiedAs(SelectedAbility, "Ｍ投") Then
					If CommandState = "ターゲット選択" Then
						CommandState = "マップ攻撃使用"
					Else
						CommandState = "移動後マップ攻撃使用"
					End If
					
					'目標地点
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					'効果範囲を設定
					AreaInRange(SelectedX, SelectedY, .AbilityLevel(SelectedAbility, "Ｍ投"), 1, "味方")
					MaskScreen()
					Exit Sub
				ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Ｍ移") Then 
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
						MaskScreen()
						Exit Sub
					End If
					
					'目標地点
					If CommandState = "ターゲット選択" Then
						CommandState = "マップ攻撃使用"
					Else
						CommandState = "移動後マップ攻撃使用"
					End If
					
					'効果範囲を設定
					AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
					MaskScreen()
					Exit Sub
				ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Ｍ線") Then 
					If CommandState = "ターゲット選択" Then
						CommandState = "マップ攻撃使用"
					Else
						CommandState = "移動後マップ攻撃使用"
					End If
					
					'目標地点
					SelectedX = PixelToMapX(MouseX)
					SelectedY = PixelToMapY(MouseY)
					
					'効果範囲を設定
					AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
					MaskScreen()
					Exit Sub
				End If
			End If
			
			'合体技パートナーの設定
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				.CombinationPartner("アビリティ", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
		End With
		
		If MainWidth <> 15 Then
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		'アビリティを実行
		SelectedUnit.ExecuteMapAbility(SelectedAbility, SelectedX, SelectedY)
		SelectedUnit = SelectedUnit.CurrentForm
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			ReDim SelectedPartners(0)
			WaitCommand()
			Exit Sub
		End If
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		' ADD START MARGE
		'再移動
		If is_p_ability And SelectedUnit.Status_Renamed = "出撃" Then
			If SelectedUnit.MainPilot.IsSkillAvailable("遊撃") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
				'進入イベント
				If SelectedUnitMoveCost > 0 Then
					HandleEvent("進入", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						Exit Sub
					End If
				End If
				
				'ユニットが既に出撃していない？
				If SelectedUnit.Status_Renamed <> "出撃" Then
					RedrawScreen()
					ClearUnitStatus()
					Exit Sub
				End If
				
				SelectedCommand = "再移動"
				AreaInSpeed(SelectedUnit)
				If Not IsOptionDefined("大型マップ") Then
					Center(SelectedUnit.X, SelectedUnit.Y)
				End If
				MaskScreen()
				If NewGUIMode Then
					System.Windows.Forms.Application.DoEvents()
					ClearUnitStatus()
				Else
					DisplayUnitStatus(SelectedUnit)
				End If
				CommandState = "ターゲット選択"
				Exit Sub
			End If
		End If
		' ADD END MARGE
		
		'行動終了
		WaitCommand()
	End Sub
	
	
	'スペシャルパワーコマンドを開始
	' MOD START MARGE
	'Public Sub StartSpecialPowerCommand()
	Private Sub StartSpecialPowerCommand()
		' MOD END MARGE
		Dim n, i, j, ret As Short
		Dim pname_list() As String
		Dim pid_list() As String
		Dim sp_list() As String
		Dim list() As String
		Dim id_list() As String
		Dim sname As String
		Dim sd As SpecialPowerData
		Dim u As Unit
		Dim p As Pilot
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_str As String
		Dim buf As String
		Dim found As Boolean
		
		LockGUI()
		
		SelectedCommand = "スペシャルパワー"
		
		With SelectedUnit
			ReDim pname_list(0)
			ReDim pid_list(0)
			ReDim ListItemFlag(0)
			
			'スペシャルパワーを使用可能なパイロットの一覧を作成
			n = .CountPilot + .CountSupport
			If .IsFeatureAvailable("追加サポート") Then
				n = n + 1
			End If
			For i = 1 To n
				If i <= .CountPilot Then
					'メインパイロット＆サブパイロット
					p = .Pilot(i)
					
					If i = 1 Then
						'１番目のパイロットの場合はメインパイロットを使用
						p = .MainPilot
						
						'ただし２人乗り以上のユニットで、メインパイロットが
						'スペシャルパワーを持たない場合はそのまま１番目のパイロットを使用
						If .CountPilot > 1 Then
							If p.Data.SP <= 0 And .Pilot(1).Data.SP > 0 Then
								p = .Pilot(1)
							End If
						End If
						
						'サブパイロットがメインパイロットを勤めている場合も
						'１番目のパイロットを使用
						For j = 2 To .CountPilot
							If p Is .Pilot(j) Then
								p = .Pilot(1)
								Exit For
							End If
						Next 
					Else
						
					End If
					
					With p
						If .CountSpecialPower = 0 Then
							GoTo NextPilot
						End If
						
						ReDim Preserve pname_list(UBound(pname_list) + 1)
						ReDim Preserve pid_list(UBound(pname_list) + 1)
						ReDim Preserve ListItemFlag(UBound(pname_list))
						ListItemFlag(UBound(pname_list)) = False
						
						pid_list(UBound(pname_list)) = .ID
						pname_list(UBound(pname_list)) = RightPaddedString(.Nickname, 17) & RightPaddedString(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP), 8)
						For j = 1 To .CountSpecialPower
							sname = .SpecialPower(j)
							If .SP >= .SpecialPowerCost(sname) Then
								pname_list(UBound(pname_list)) = pname_list(UBound(pname_list)) & SPDList.Item(sname).ShortName
							End If
						Next 
					End With
				ElseIf i <= .CountPilot + .CountSupport Then 
					'サポートパイロット
					With .Support(i - .CountPilot)
						If .CountSpecialPower = 0 Then
							GoTo NextPilot
						End If
						
						ReDim Preserve pname_list(UBound(pname_list) + 1)
						ReDim Preserve pid_list(UBound(pname_list) + 1)
						ReDim Preserve ListItemFlag(UBound(pname_list))
						ListItemFlag(UBound(pname_list)) = False
						
						pid_list(UBound(pname_list)) = .ID
						pname_list(UBound(pname_list)) = RightPaddedString(.Nickname, 17) & RightPaddedString(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP), 8)
						For j = 1 To .CountSpecialPower
							sname = .SpecialPower(j)
							If .SP >= .SpecialPowerCost(sname) Then
								pname_list(UBound(pname_list)) = pname_list(UBound(pname_list)) & SPDList.Item(sname).ShortName
							End If
						Next 
					End With
				Else
					'追加サポートパイロット
					With .AdditionalSupport
						If .CountSpecialPower = 0 Then
							GoTo NextPilot
						End If
						
						ReDim Preserve pname_list(UBound(pname_list) + 1)
						ReDim Preserve pid_list(UBound(pname_list) + 1)
						ReDim Preserve ListItemFlag(UBound(pname_list))
						ListItemFlag(UBound(pname_list)) = False
						
						pid_list(UBound(pname_list)) = .ID
						pname_list(UBound(pname_list)) = RightPaddedString(.Nickname, 17) & RightPaddedString(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP), 8)
						For j = 1 To .CountSpecialPower
							sname = .SpecialPower(j)
							If .SP >= .SpecialPowerCost(sname) Then
								pname_list(UBound(pname_list)) = pname_list(UBound(pname_list)) & SPDList.Item(sname).ShortName
							End If
						Next 
					End With
				End If
NextPilot: 
			Next 
			TopItem = 1
			If UBound(pname_list) > 1 Then
				'どのパイロットを使うか選択
				If IsOptionDefined("等身大基準") Then
					i = ListBox("キャラクター選択", pname_list, "キャラクター     " & Term("SP", SelectedUnit, 2) & "/Max" & Term("SP", SelectedUnit, 2), "連続表示,カーソル移動")
				Else
					i = ListBox("パイロット選択", pname_list, "パイロット       " & Term("SP", SelectedUnit, 2) & "/Max" & Term("SP", SelectedUnit, 2), "連続表示,カーソル移動")
				End If
			Else
				'一人しかいないので選択の必要なし
				i = 1
			End If
			
			'誰もスペシャルパワーを使えなければキャンセル
			If i = 0 Then
				frmListBox.Hide()
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				UnlockGUI()
				CancelCommand()
				Exit Sub
			End If
			
			'スペシャルパワーを使うパイロットを設定
			SelectedPilot = PList.Item(pid_list(i))
			'そのパイロットのステータスを表示
			If UBound(pname_list) > 1 Then
				DisplayPilotStatus(SelectedPilot)
			End If
		End With
		
		With SelectedPilot
			'使用可能なスペシャルパワーの一覧を作成
			ReDim sp_list(.CountSpecialPower)
			ReDim ListItemFlag(.CountSpecialPower)
			For i = 1 To .CountSpecialPower
				sname = .SpecialPower(i)
				sp_list(i) = RightPaddedString(sname, 13) & LeftPaddedString(VB6.Format(.SpecialPowerCost(sname)), 3) & "  " & SPDList.Item(sname).Comment
				
				If .SP >= .SpecialPowerCost(sname) Then
					If Not .IsSpecialPowerUseful(sname) Then
						ListItemFlag(i) = True
					End If
				Else
					ListItemFlag(i) = True
				End If
			Next 
		End With
		
		'どのコマンドを使用するかを選択
		With SelectedPilot
			TopItem = 1
			i = ListBox(Term("スペシャルパワー", SelectedUnit) & "選択", sp_list, "名称         消費" & Term("SP", SelectedUnit) & "（" & .Nickname & " " & Term("SP", SelectedUnit) & "=" & VB6.Format(.SP) & "/" & VB6.Format(.MaxSP) & "）", "カーソル移動(行きのみ)")
		End With
		
		'キャンセル
		If i = 0 Then
			DisplayUnitStatus(SelectedUnit)
			'カーソル自動移動
			If AutoMoveCursor Then
				MoveCursorPos("ユニット選択", SelectedUnit)
			End If
			UnlockGUI()
			CancelCommand()
			Exit Sub
		End If
		
		'使用するスペシャルパワーを設定
		SelectedSpecialPower = SelectedPilot.SpecialPower(i)
		
		'味方スペシャルパワー実行の効果により他のパイロットが持っているスペシャルパワーを
		'使う場合は記録しておき、後で消費ＳＰを倍にする必要がある
		If SPDList.Item(SelectedSpecialPower).EffectType(1) = "味方スペシャルパワー実行" Then
			'スペシャルパワー一覧
			ReDim list(0)
			For i = 1 To SPDList.Count
				With SPDList.Item(i)
					If .EffectType(1) <> "味方スペシャルパワー実行" And .ShortName <> "非表示" Then
						ReDim Preserve list(UBound(list) + 1)
						ReDim Preserve strkey_list(UBound(list))
						list(UBound(list)) = .Name
						strkey_list(UBound(list)) = .KanaName
					End If
				End With
			Next 
			ReDim ListItemFlag(UBound(list))
			
			'ソート
			For i = 1 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = strkey_list(i)
					strkey_list(i) = max_str
					strkey_list(max_item) = buf
				End If
			Next 
			
			'スペシャルパワーを使用可能なパイロットがいるかどうかを判定
			For i = 1 To UBound(list)
				ListItemFlag(i) = True
				For	Each p In PList
					With p
						If .Party = "味方" Then
							If Not .Unit_Renamed Is Nothing Then
								If .Unit_Renamed.Status_Renamed = "出撃" And Not .Unit_Renamed.IsConditionSatisfied("憑依") Then
									'本当に乗っている？
									found = False
									With .Unit_Renamed
										If p Is .MainPilot Then
											found = True
										Else
											For j = 2 To .CountPilot
												If p Is .Pilot(j) Then
													found = True
													Exit For
												End If
											Next 
											For j = 1 To .CountSupport
												If p Is .Support(j) Then
													found = True
													Exit For
												End If
											Next 
											If p Is .AdditionalSupport Then
												found = True
											End If
										End If
									End With
									
									If found Then
										If .IsSpecialPowerAvailable(list(i)) Then
											ListItemFlag(i) = False
											Exit For
										End If
									End If
								End If
							End If
						End If
					End With
				Next p
			Next 
			
			'各スペシャルパワーが使用可能か判定
			With SelectedPilot
				For i = 1 To UBound(list)
					If Not ListItemFlag(i) And .SP >= 2 * .SpecialPowerCost(list(i)) Then
						If Not .IsSpecialPowerUseful(list(i)) Then
							ListItemFlag(i) = True
						End If
					Else
						ListItemFlag(i) = True
					End If
				Next 
			End With
			
			'スペシャルパワーの解説を設定
			ReDim ListItemComment(UBound(list))
			For i = 1 To UBound(list)
				ListItemComment(i) = SPDList.Item(list(i)).Comment
			Next 
			
			'検索するスペシャルパワーを選択
			TopItem = 1
			ret = MultiColumnListBox(Term("スペシャルパワー") & "検索", list, True)
			If ret = 0 Then
				SelectedSpecialPower = ""
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			
			'スペシャルパワー使用メッセージ
			If SelectedUnit.IsMessageDefined(SelectedSpecialPower) Then
				OpenMessageForm()
				SelectedUnit.PilotMessage(SelectedSpecialPower)
				CloseMessageForm()
			End If
			
			SelectedSpecialPower = list(ret)
			
			WithDoubleSPConsumption = True
		Else
			WithDoubleSPConsumption = False
		End If
		
		sd = SPDList.Item(SelectedSpecialPower)
		
		'ターゲットを選択する必要があるスペシャルパワーの場合
		Select Case sd.TargetType
			Case "味方", "敵", "任意"
				'マップ上のユニットからターゲットを選択する
				
				OpenMessageForm()
				DisplaySysMessage(SelectedPilot.Nickname & "は" & SelectedSpecialPower & "を使った。;" & "ターゲットを選んでください。")
				CloseMessageForm()
				
				'ターゲットのエリアを設定
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						MaskData(i, j) = True
						
						u = MapDataForUnit(i, j)
						
						If u Is Nothing Then
							GoTo NextLoop
						End If
						
						'陣営が合っている？
						Select Case sd.TargetType
							Case "味方"
								With u
									If .Party <> "味方" And .Party0 <> "味方" And .Party <> "ＮＰＣ" And .Party0 <> "ＮＰＣ" Then
										GoTo NextLoop
									End If
								End With
							Case "敵"
								With u
									If (.Party = "味方" And .Party0 = "味方") Or (.Party = "ＮＰＣ" And .Party0 = "ＮＰＣ") Then
										GoTo NextLoop
									End If
								End With
						End Select
						
						'スペシャルパワーを適用可能？
						If Not sd.Effective(SelectedPilot, u) Then
							GoTo NextLoop
						End If
						
						MaskData(i, j) = False
NextLoop: 
					Next 
				Next 
				MaskScreen()
				CommandState = "ターゲット選択"
				UnlockGUI()
				Exit Sub
				
			Case "破壊味方"
				'破壊された味方ユニットの中からターゲットを選択する
				
				OpenMessageForm()
				DisplaySysMessage(SelectedPilot.Nickname & "は" & SelectedSpecialPower & "を使った。;" & "復活させるユニットを選んでください。")
				CloseMessageForm()
				
				'破壊された味方ユニットのリストを作成
				ReDim list(0)
				ReDim id_list(0)
				ReDim ListItemFlag(0)
				For	Each u In UList
					With u
						If .Party0 = "味方" And .Status_Renamed = "破壊" And (.CountPilot > 0 Or .Data.PilotNum = 0) Then
							ReDim Preserve list(UBound(list) + 1)
							ReDim Preserve id_list(UBound(list))
							ReDim Preserve ListItemFlag(UBound(list))
							list(UBound(list)) = RightPaddedString(.Nickname, 28) & RightPaddedString(.MainPilot.Nickname, 18) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
							id_list(UBound(list)) = .ID
							ListItemFlag(UBound(list)) = False
						End If
					End With
				Next u
				
				TopItem = 1
				i = ListBox("ユニット選択", list, "ユニット名                  パイロット     レベル")
				If i = 0 Then
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				SelectedTarget = UList.Item(id_list(i))
		End Select
		
		'自爆を選択した場合は確認を取る
		'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(自爆) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If sd.IsEffectAvailable("自爆") Then
			ret = MsgBox("自爆させますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "自爆")
			If ret = MsgBoxResult.Cancel Then
				UnlockGUI()
				Exit Sub
			End If
		End If
		
		'使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			UnlockGUI()
			Exit Sub
		End If
		
		'スペシャルパワーを使用
		If WithDoubleSPConsumption Then
			SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2)
		Else
			SelectedPilot.UseSpecialPower(SelectedSpecialPower)
		End If
		SelectedUnit = SelectedUnit.CurrentForm
		
		'カーソル自動移動
		If AutoMoveCursor Then
			MoveCursorPos("ユニット選択", SelectedUnit)
		End If
		
		'ステータスウィンドウを更新
		DisplayUnitStatus(SelectedUnit)
		
		'使用後イベント
		HandleEvent("使用後", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Then
			IsScenarioFinished = False
		End If
		If IsCanceled Then
			IsCanceled = False
		End If
		
		SelectedSpecialPower = ""
		
		UnlockGUI()
		
		CommandState = "ユニット選択"
	End Sub
	
	'スペシャルパワーコマンドを終了
	' MOD START MARGE
	'Public Sub FinishSpecialPowerCommand()
	Private Sub FinishSpecialPowerCommand()
		' MOD END MARGE
		Dim i, ret As Short
		
		LockGUI()
		
		'自爆を選択した場合は確認を取る
		With SPDList.Item(SelectedSpecialPower)
			For i = 1 To .CountEffect
				If .EffectType(i) = "自爆" Then
					ret = MsgBox("自爆させますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "自爆")
					If ret = MsgBoxResult.Cancel Then
						CommandState = "ユニット選択"
						UnlockGUI()
						Exit Sub
					End If
					Exit For
				End If
			Next 
		End With
		
		'使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		
		'スペシャルパワーを使用
		If WithDoubleSPConsumption Then
			SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2)
		Else
			SelectedPilot.UseSpecialPower(SelectedSpecialPower)
		End If
		SelectedUnit = SelectedUnit.CurrentForm
		
		'ステータスウィンドウを更新
		If Not SelectedTarget Is Nothing Then
			If SelectedTarget.CurrentForm.Status_Renamed = "出撃" Then
				DisplayUnitStatus(SelectedTarget)
			End If
		End If
		
		'使用後イベント
		HandleEvent("使用後", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Then
			IsScenarioFinished = False
		End If
		If IsCanceled Then
			IsCanceled = False
		End If
		
		SelectedSpecialPower = ""
		
		UnlockGUI()
		
		CommandState = "ユニット選択"
	End Sub
	
	
	'「修理」コマンドを開始
	' MOD START MARGE
	'Public Sub StartFixCommand()
	Private Sub StartFixCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim t, u As Unit
		Dim fname As String
		
		SelectedCommand = "修理"
		
		'射程範囲？を表示
		With SelectedUnit
			AreaInRange(.X, .Y, 1, 1, "味方")
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If Not MaskData(i, j) And Not MapDataForUnit(i, j) Is Nothing Then
						With MapDataForUnit(i, j)
							If .HP = .MaxHP Or .IsConditionSatisfied("ゾンビ") Then
								MaskData(i, j) = True
							End If
							If .IsFeatureAvailable("修理不可") Then
								For k = 2 To CInt(.FeatureData("修理不可"))
									fname = LIndex(.FeatureData("修理不可"), k)
									If Left(fname, 1) = "!" Then
										fname = Mid(fname, 2)
										If fname <> SelectedUnit.FeatureName0("修理装置") Then
											MaskData(i, j) = True
											Exit For
										End If
									Else
										If fname = SelectedUnit.FeatureName0("修理装置") Then
											MaskData(i, j) = True
											Exit For
										End If
									End If
								Next 
							End If
						End With
					End If
				Next 
			Next 
			MaskData(.X, .Y) = False
		End With
		MaskScreen()
		
		'カーソル自動移動
		If AutoMoveCursor Then
			'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			t = Nothing
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" And .Party = "味方" Then
						If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
							If t Is Nothing Then
								t = u
							ElseIf u.MaxHP - u.HP > t.MaxHP - t.HP Then 
								t = u
							End If
						End If
					End If
				End With
			Next u
			If t Is Nothing Then
				t = SelectedUnit
			End If
			MoveCursorPos("ユニット選択", t)
			If Not SelectedUnit Is t Then
				DisplayUnitStatus(t)
			End If
		End If
		
		If CommandState = "コマンド選択" Then
			CommandState = "ターゲット選択"
		Else
			CommandState = "移動後ターゲット選択"
		End If
	End Sub
	
	'「修理」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishFixCommand()
	Private Sub FinishFixCommand()
		' MOD END MARGE
		Dim tmp As Integer
		
		LockGUI()
		
		OpenMessageForm(SelectedTarget, SelectedUnit)
		
		With SelectedUnit
			'選択内容を変更
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'修理メッセージ＆特殊効果
			If .IsMessageDefined("修理") Then
				.PilotMessage("修理")
			End If
			If .IsAnimationDefined("修理", .FeatureName("修理装置")) Then
				.PlayAnimation("修理", .FeatureName("修理装置"))
			Else
				.SpecialEffect("修理", .FeatureName("修理装置"))
			End If
			
			DisplaySysMessage(.Nickname & "は" & SelectedTarget.Nickname & "に" & .FeatureName("修理装置") & "を使った。")
			
			'修理を実行
			tmp = SelectedTarget.HP
			Select Case .FeatureLevel("修理装置")
				Case 1, -1
					SelectedTarget.RecoverHP(30 + 3 * SelectedUnit.MainPilot.SkillLevel("修理"))
				Case 2
					SelectedTarget.RecoverHP(50 + 5 * SelectedUnit.MainPilot.SkillLevel("修理"))
				Case 3
					SelectedTarget.RecoverHP(100)
			End Select
			If IsNumeric(LIndex(.FeatureData("修理装置"), 2)) Then
				.EN = .EN - CShort(LIndex(.FeatureData("修理装置"), 2))
			End If
			
			DrawSysString(SelectedTarget.X, SelectedTarget.Y, "+" & VB6.Format(SelectedTarget.HP - tmp))
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "の" & Term("ＨＰ", SelectedTarget) & "が" & VB6.Format(SelectedTarget.HP - tmp) & "回復した。")
			
			'経験値獲得
			.GetExp(SelectedTarget, "修理")
			
			If MessageWait < 10000 Then
				Sleep(MessageWait)
			End If
		End With
		
		CloseMessageForm()
		
		'形態変化のチェック
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		'行動終了
		WaitCommand()
	End Sub
	
	
	'「補給」コマンドを開始
	' MOD START MARGE
	'Public Sub StartSupplyCommand()
	Private Sub StartSupplyCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim t, u As Unit
		
		SelectedCommand = "補給"
		
		'射程範囲？を表示
		With SelectedUnit
			AreaInRange(.X, .Y, 1, 1, "味方")
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If Not MaskData(i, j) And Not MapDataForUnit(i, j) Is Nothing Then
						MaskData(i, j) = True
						With MapDataForUnit(i, j)
							If .EN < .MaxEN And Not .IsConditionSatisfied("ゾンビ") Then
								MaskData(i, j) = False
							Else
								For k = 1 To .CountWeapon
									If .Bullet(k) < .MaxBullet(k) Then
										MaskData(i, j) = False
										Exit For
									End If
								Next 
								For k = 1 To .CountAbility
									If .Stock(k) < .MaxStock(k) Then
										MaskData(i, j) = False
										Exit For
									End If
								Next 
							End If
						End With
					End If
				Next 
			Next 
			MaskData(.X, .Y) = False
		End With
		MaskScreen()
		
		'カーソル自動移動
		If AutoMoveCursor Then
			'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			t = Nothing
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" And .Party = "味方" Then
						If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
							t = u
							Exit For
						End If
					End If
				End With
			Next u
			If t Is Nothing Then
				t = SelectedUnit
			End If
			MoveCursorPos("ユニット選択", t)
			If Not SelectedUnit Is t Then
				DisplayUnitStatus(t)
			End If
		End If
		
		If CommandState = "コマンド選択" Then
			CommandState = "ターゲット選択"
		Else
			CommandState = "移動後ターゲット選択"
		End If
	End Sub
	
	'「補給」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishSupplyCommand()
	Private Sub FinishSupplyCommand()
		' MOD END MARGE
		
		LockGUI()
		
		OpenMessageForm(SelectedTarget, SelectedUnit)
		
		With SelectedUnit
			'選択内容を変更
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'補給メッセージ＆特殊効果
			If .IsMessageDefined("補給") Then
				.PilotMessage("補給")
			End If
			If .IsAnimationDefined("補給", .FeatureName("補給装置")) Then
				.PlayAnimation("補給", .FeatureName("補給装置"))
			Else
				.SpecialEffect("補給", .FeatureName("補給装置"))
			End If
			
			DisplaySysMessage(.Nickname & "は" & SelectedTarget.Nickname & "に" & .FeatureName("補給装置") & "を使った。")
			
			'補給を実施
			SelectedTarget.FullSupply()
			SelectedTarget.IncreaseMorale(-10)
			If IsNumeric(LIndex(.FeatureData("補給装置"), 2)) Then
				.EN = .EN - CShort(LIndex(.FeatureData("補給装置"), 2))
			End If
			
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "の弾数と" & Term("ＥＮ", SelectedTarget) & "が全快した。")
			
			'経験値を獲得
			.GetExp(SelectedTarget, "補給")
			
			If MessageWait < 10000 Then
				Sleep(MessageWait)
			End If
		End With
		
		'形態変化のチェック
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		CloseMessageForm()
		
		'行動終了
		WaitCommand()
	End Sub
	
	
	'「チャージ」コマンド
	' MOD START MARGE
	'Public Sub ChargeCommand()
	Private Sub ChargeCommand()
		' MOD END MARGE
		Dim ret As Short
		Dim partners() As Unit
		Dim i As Short
		
		LockGUI()
		
		ret = MsgBox("チャージを開始しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "チャージ開始")
		If ret = MsgBoxResult.Cancel Then
			CancelCommand()
			UnlockGUI()
			Exit Sub
		End If
		
		'使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, "チャージ")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		
		With SelectedUnit
			'チャージのメッセージを表示
			If .IsMessageDefined("チャージ") Then
				OpenMessageForm()
				.PilotMessage("チャージ")
				CloseMessageForm()
			End If
			
			'アニメ表示を行う
			If .IsAnimationDefined("チャージ") Then
				.PlayAnimation("チャージ")
			ElseIf .IsSpecialEffectDefined("チャージ") Then 
				.SpecialEffect("チャージ")
			Else
				PlayWave("Charge.wav")
			End If
			
			'チャージ攻撃のパートナーを探す
			ReDim partners(0)
			For i = 1 To .CountWeapon
				If .IsWeaponClassifiedAs(i, "Ｃ") And .IsWeaponClassifiedAs(i, "合") Then
					If .IsWeaponAvailable(i, "チャージ") Then
						.CombinationPartner("武装", i, partners)
						Exit For
					End If
				End If
			Next 
			If UBound(partners) = 0 Then
				For i = 1 To .CountAbility
					If .IsAbilityClassifiedAs(i, "Ｃ") And .IsAbilityClassifiedAs(i, "合") Then
						If .IsAbilityAvailable(i, "チャージ") Then
							.CombinationPartner("アビリティ", i, partners)
							Exit For
						End If
					End If
				Next 
			End If
			
			'ユニットの状態をチャージ中に
			.AddCondition("チャージ", 1)
			
			'チャージ攻撃のパートナーもチャージ中にする
			For i = 1 To UBound(partners)
				With partners(i)
					.AddCondition("チャージ", 1)
				End With
			Next 
		End With
		
		'使用後イベント
		HandleEvent("使用後", SelectedUnit.MainPilot.ID, "チャージ")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		
		UnlockGUI()
		
		'行動終了
		WaitCommand()
	End Sub
	
	'「会話」コマンドを開始
	' MOD START MARGE
	'Public Sub StartTalkCommand()
	Private Sub StartTalkCommand()
		' MOD END MARGE
		Dim i, j As Short
		Dim t As Unit
		
		SelectedCommand = "会話"
		
		'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		t = Nothing
		
		'会話可能なユニットを表示
		With SelectedUnit
			AreaInRange(.X, .Y, 1, 1, "")
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If Not MaskData(i, j) Then
						If Not MapDataForUnit(i, j) Is Nothing Then
							If Not IsEventDefined("会話 " & .MainPilot.ID & " " & MapDataForUnit(i, j).MainPilot.ID) Then
								MaskData(i, j) = True
								t = MapDataForUnit(i, j)
							End If
						End If
					End If
				Next 
			Next 
			MaskData(.X, .Y) = False
		End With
		MaskScreen()
		
		'カーソル自動移動
		If AutoMoveCursor Then
			If Not t Is Nothing Then
				MoveCursorPos("ユニット選択", t)
				DisplayUnitStatus(t)
			End If
		End If
		
		If CommandState = "コマンド選択" Then
			CommandState = "ターゲット選択"
		Else
			CommandState = "移動後ターゲット選択"
		End If
	End Sub
	
	'「会話」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishTalkCommand()
	Private Sub FinishTalkCommand()
		' MOD END MARGE
		Dim p As Pilot
		
		LockGUI()
		
		If SelectedUnit.CountPilot > 0 Then
			p = SelectedUnit.Pilot(1)
		Else
			'UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			p = Nothing
		End If
		
		'会話イベントを実施
		HandleEvent("会話", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
		
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		If Not p Is Nothing Then
			If Not p.Unit_Renamed Is Nothing Then
				SelectedUnit = p.Unit_Renamed
			End If
		End If
		
		UnlockGUI()
		
		'行動終了
		WaitCommand()
	End Sub
	
	'「変形」コマンド
	' MOD START MARGE
	'Public Sub TransformCommand()
	Private Sub TransformCommand()
		' MOD END MARGE
		Dim list() As String
		Dim list_id() As String
		Dim i As Short
		Dim ret As Short
		Dim uname, fdata As String
		Dim prev_uname As String
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		fdata = SelectedUnit.FeatureData("変形")
		
		If MapFileName = "" Then
			'ユニットステータスコマンドの場合
			With SelectedUnit
				ReDim list(0)
				ReDim list_id(0)
				'変形可能な形態一覧を作成
				For i = 2 To LLength(fdata)
					With .OtherForm(LIndex(fdata, i))
						If .IsAvailable Then
							ReDim Preserve list(UBound(list) + 1)
							ReDim Preserve list_id(UBound(list))
							list(UBound(list)) = .Nickname
							list_id(UBound(list)) = .Name
						End If
					End With
				Next 
				ReDim ListItemFlag(UBound(list))
				
				'変形する形態を選択
				If UBound(list) > 1 Then
					TopItem = 1
					ret = ListBox("変形", list, "名前", "カーソル移動")
					If ret = 0 Then
						CancelCommand()
						UnlockGUI()
						Exit Sub
					End If
				Else
					ret = 1
				End If
				
				'変形を実施
				.Transform((.OtherForm(list_id(ret)).Name))
				
				'ユニットリストの表示を更新
				MakeUnitList()
				
				'ステータスウィンドウの表示を更新
				DisplayUnitStatus(.CurrentForm)
				
				'コマンドを終了
				UnlockGUI()
				CommandState = "ユニット選択"
				Exit Sub
			End With
		End If
		
		'変形可能な形態の一覧を作成
		ReDim list(0)
		ReDim list_id(0)
		ReDim ListItemFlag(0)
		For i = 2 To LLength(fdata)
			With SelectedUnit.OtherForm(LIndex(fdata, i))
				If .IsAvailable Then
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve list_id(UBound(list))
					ReDim Preserve ListItemFlag(UBound(list))
					list(UBound(list)) = .Nickname0
					list_id(UBound(list)) = .Name
					If .IsAbleToEnter(SelectedUnit.X, SelectedUnit.Y) Or MapFileName = "" Then
						ListItemFlag(UBound(list)) = False
					Else
						ListItemFlag(UBound(list)) = True
					End If
				End If
			End With
		Next 
		
		'変形先の形態を選択
		If UBound(list) = 1 Then
			If ListItemFlag(1) Then
				MsgBox("この地形では" & LIndex(fdata, 1) & "できません")
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			ret = 1
		Else
			TopItem = 1
			If Not SelectedUnit.IsHero Then
				ret = ListBox("変形先", list, "名前", "カーソル移動")
			Else
				ret = ListBox("変身先", list, "名前", "カーソル移動")
			End If
			If ret = 0 Then
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
		End If
		
		uname = list_id(ret)
		
		Dim BGM As String
		With SelectedUnit
			'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
			With UDList.Item(uname)
				If .IsFeatureAvailable("追加パイロット") Then
					If Not PList.IsDefined(.FeatureData("追加パイロット")) Then
						If Not PDList.IsDefined(.FeatureData("追加パイロット")) Then
							ErrorMessage(uname & "の追加パイロット「" & .FeatureData("追加パイロット") & "」のデータが見つかりません")
							TerminateSRC()
						End If
						PList.Add(.FeatureData("追加パイロット"), SelectedUnit.MainPilot.Level, (SelectedUnit.Party0))
					End If
				End If
			End With
			
			'ＢＧＭの変更
			If .IsFeatureAvailable("変形ＢＧＭ") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "変形ＢＧＭ" And LIndex(.FeatureData(i), 1) = uname Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
							Sleep(500)
						End If
						Exit For
					End If
				Next 
			End If
			
			'メッセージを表示
			If .IsMessageDefined("変形(" & .Name & "=>" & uname & ")") Or .IsMessageDefined("変形(" & uname & ")") Or .IsMessageDefined("変形(" & .FeatureName("変形") & ")") Then
				Center(.X, .Y)
				RefreshScreen()
				
				OpenMessageForm()
				If .IsMessageDefined("変形(" & .Name & "=>" & uname & ")") Then
					.PilotMessage("変形(" & .Name & "=>" & uname & ")")
				ElseIf .IsMessageDefined("変形(" & uname & ")") Then 
					.PilotMessage("変形(" & uname & ")")
				ElseIf .IsMessageDefined("変形(" & .FeatureName("変形") & ")") Then 
					.PilotMessage("変形(" & .FeatureName("変形") & ")")
				End If
				CloseMessageForm()
			End If
			
			'アニメ表示
			If .IsAnimationDefined("変形(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("変形(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("変形(" & uname & ")") Then 
				.PlayAnimation("変形(" & uname & ")")
			ElseIf .IsAnimationDefined("変形(" & .FeatureName("変形") & ")") Then 
				.PlayAnimation("変形(" & .FeatureName("変形") & ")")
			ElseIf .IsSpecialEffectDefined("変形(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("変形(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("変形(" & uname & ")") Then 
				.SpecialEffect("変形(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("変形(" & .FeatureName("変形") & ")") Then 
				.SpecialEffect("変形(" & .FeatureName("変形") & ")")
			End If
		End With
		
		'変形
		prev_uname = SelectedUnit.Name
		SelectedUnit.Transform(uname)
		SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
		
		'変形をキャンセルする？
		If SelectedUnit.Action = 0 Then
			ret = MsgBox("この形態ではこれ以上の行動が出来ません。" & vbCr & vbLf & "それでも変形しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "変形")
			If ret = MsgBoxResult.Cancel Then
				SelectedUnit.Transform(prev_uname)
				SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
				If SelectedUnit.IsConditionSatisfied("消耗") Then
					SelectedUnit.DeleteCondition("消耗")
				End If
			End If
			RedrawScreen()
		End If
		
		'変形イベント
		With SelectedUnit.CurrentForm
			HandleEvent("変形", .MainPilot.ID, .Name)
		End With
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		IsCanceled = False
		
		'ハイパーモード・ノーマルモードの自動発動をチェック
		SelectedUnit.CurrentForm.CheckAutoHyperMode()
		SelectedUnit.CurrentForm.CheckAutoNormalMode()
		
		'カーソル自動移動
		If SelectedUnit.Status_Renamed = "出撃" Then
			If AutoMoveCursor Then
				MoveCursorPos("ユニット選択", SelectedUnit)
			End If
			DisplayUnitStatus(SelectedUnit)
		End If
		
		CommandState = "ユニット選択"
		
		UnlockGUI()
	End Sub
	
	'「ハイパーモード」コマンド
	' MOD START MARGE
	'Public Sub HyperModeCommand()
	Private Sub HyperModeCommand()
		' MOD END MARGE
		Dim uname, fname As String
		Dim i As Short
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		uname = LIndex(SelectedUnit.FeatureData("ハイパーモード"), 2)
		fname = SelectedUnit.FeatureName("ハイパーモード")
		
		If MapFileName = "" Then
			'ユニットステータスコマンドの場合
			With SelectedUnit
				If Not .IsFeatureAvailable("ハイパーモード") Then
					uname = LIndex(SelectedUnit.FeatureData("ノーマルモード"), 1)
				End If
				
				'ハイパーモードを発動
				.Transform(uname)
				
				'ユニットリストの表示を更新
				MakeUnitList()
				
				'ステータスウィンドウの表示を更新
				DisplayUnitStatus(.CurrentForm)
				
				'コマンドを終了
				UnlockGUI()
				CommandState = "ユニット選択"
				Exit Sub
			End With
		End If
		
		'ハイパーモードを発動可能かどうかチェック
		With SelectedUnit.OtherForm(uname)
			If Not .IsAbleToEnter(SelectedUnit.X, SelectedUnit.Y) And MapFileName <> "" Then
				MsgBox("この地形では変形できません")
				UnlockGUI()
				CancelCommand()
				Exit Sub
			End If
		End With
		
		'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
		With UDList.Item(uname)
			If .IsFeatureAvailable("追加パイロット") Then
				If Not PList.IsDefined(.FeatureData("追加パイロット")) Then
					If Not PDList.IsDefined(.FeatureData("追加パイロット")) Then
						ErrorMessage(uname & "の追加パイロット「" & .FeatureData("追加パイロット") & "」のデータが見つかりません")
						TerminateSRC()
					End If
					PList.Add(.FeatureData("追加パイロット"), SelectedUnit.MainPilot.Level, (SelectedUnit.Party0))
				End If
			End If
		End With
		
		Dim BGM As String
		With SelectedUnit
			'ＢＧＭを変更
			If .IsFeatureAvailable("ハイパーモードＢＧＭ") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "ハイパーモードＢＧＭ" And LIndex(.FeatureData(i), 1) = uname Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
							Sleep(500)
						End If
						Exit For
					End If
				Next 
			End If
			
			'メッセージを表示
			If .IsMessageDefined("ハイパーモード(" & .Name & "=>" & uname & ")") Or .IsMessageDefined("ハイパーモード(" & uname & ")") Or .IsMessageDefined("ハイパーモード(" & fname & ")") Or .IsMessageDefined("ハイパーモード") Then
				Center(.X, .Y)
				RefreshScreen()
				
				OpenMessageForm()
				If .IsMessageDefined("ハイパーモード(" & .Name & "=>" & uname & ")") Then
					.PilotMessage("ハイパーモード(" & .Name & "=>" & uname & ")")
				ElseIf .IsMessageDefined("ハイパーモード(" & uname & ")") Then 
					.PilotMessage("ハイパーモード(" & uname & ")")
				ElseIf .IsMessageDefined("ハイパーモード(" & fname & ")") Then 
					.PilotMessage("ハイパーモード(" & fname & ")")
				Else
					.PilotMessage("ハイパーモード")
				End If
				CloseMessageForm()
			End If
			
			'アニメ表示
			If .IsAnimationDefined("ハイパーモード(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("ハイパーモード(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("ハイパーモード(" & uname & ")") Then 
				.PlayAnimation("ハイパーモード(" & uname & ")")
			ElseIf .IsAnimationDefined("ハイパーモード(" & fname & ")") Then 
				.PlayAnimation("ハイパーモード(" & fname & ")")
			ElseIf .IsAnimationDefined("ハイパーモード") Then 
				.PlayAnimation("ハイパーモード")
			ElseIf .IsSpecialEffectDefined("ハイパーモード(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("ハイパーモード(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("ハイパーモード(" & uname & ")") Then 
				.SpecialEffect("ハイパーモード(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("ハイパーモード(" & fname & ")") Then 
				.SpecialEffect("ハイパーモード(" & fname & ")")
			Else
				.SpecialEffect("ハイパーモード")
			End If
		End With
		
		'ハイパーモード発動
		SelectedUnit.Transform(uname)
		
		'ハイパーモード・ノーマルモードの自動発動をチェック
		SelectedUnit.CurrentForm.CheckAutoHyperMode()
		SelectedUnit.CurrentForm.CheckAutoNormalMode()
		SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
		
		'変形イベント
		With SelectedUnit.CurrentForm
			HandleEvent("変形", .MainPilot.ID, .Name)
		End With
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		IsCanceled = False
		
		'カーソル自動移動
		If SelectedUnit.Status_Renamed = "出撃" Then
			If AutoMoveCursor Then
				MoveCursorPos("ユニット選択", SelectedUnit)
			End If
			DisplayUnitStatus(SelectedUnit)
		End If
		
		CommandState = "ユニット選択"
		
		UnlockGUI()
	End Sub
	
	'「変身解除」コマンド
	' MOD START MARGE
	'Public Sub CancelTransformationCommand()
	Private Sub CancelTransformationCommand()
		' MOD END MARGE
		Dim ret As Short
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		With SelectedUnit
			If MapFileName = "" Then
				'ユニットステータスコマンドの場合
				.Transform(LIndex(.FeatureData("ノーマルモード"), 1))
				MakeUnitList()
				DisplayUnitStatus(.CurrentForm)
				UnlockGUI()
				CommandState = "ユニット選択"
				Exit Sub
			End If
			
			If .IsHero Then
				ret = MsgBox("変身を解除しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "変身解除")
			Else
				ret = MsgBox("特殊モードを解除しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "特殊モード解除")
			End If
			If ret = MsgBoxResult.Cancel Then
				UnlockGUI()
				CancelCommand()
				Exit Sub
			End If
			
			.Transform(LIndex(.FeatureData("ノーマルモード"), 1))
			SelectedUnit = MapDataForUnit(.X, .Y)
		End With
		
		'カーソル自動移動
		If AutoMoveCursor Then
			MoveCursorPos("ユニット選択", SelectedUnit)
		End If
		DisplayUnitStatus(SelectedUnit)
		
		RedrawScreen()
		
		'変形イベント
		HandleEvent("変形", SelectedUnit.MainPilot.ID, SelectedUnit.Name)
		IsScenarioFinished = False
		IsCanceled = False
		
		CommandState = "ユニット選択"
		
		UnlockGUI()
	End Sub
	
	'「分離」コマンド
	' MOD START MARGE
	'Public Sub SplitCommand()
	Private Sub SplitCommand()
		' MOD END MARGE
		Dim uname, tname, fname As String
		Dim ret As Short
		Dim BGM As String
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		If MapFileName = "" Then
			'ユニットステータスコマンドの場合
			
			'分離を実施
			With SelectedUnit
				If .IsFeatureAvailable("パーツ分離") Then
					tname = LIndex(.FeatureData("パーツ分離"), 2)
					.Transform(tname)
				Else
					.Split_Renamed()
				End If
				UList.CheckAutoHyperMode()
				UList.CheckAutoNormalMode()
				DisplayUnitStatus(MapDataForUnit(.X, .Y))
			End With
			
			'ユニットリストの表示を更新
			MakeUnitList()
			
			'コマンドを終了
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		
		With SelectedUnit
			If .IsFeatureAvailable("パーツ分離") Then
				'パーツ分離を行う場合
				
				ret = MsgBox("パーツを分離しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "パーツ分離")
				If ret = MsgBoxResult.Cancel Then
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				tname = LIndex(.FeatureData("パーツ分離"), 2)
				
				If Not .OtherForm(tname).IsAbleToEnter(.X, .Y) Then
					MsgBox("この地形では分離できません")
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				'ＢＧＭ変更
				If .IsFeatureAvailable("分離ＢＧＭ") Then
					BGM = SearchMidiFile(.FeatureData("分離ＢＧＭ"))
					If Len(BGM) > 0 Then
						StartBGM(.FeatureData("分離ＢＧＭ"))
						Sleep(500)
					End If
				End If
				
				fname = .FeatureName("パーツ分離")
				
				'メッセージを表示
				If .IsMessageDefined("分離(" & .Name & ")") Or .IsMessageDefined("分離(" & fname & ")") Or .IsMessageDefined("分離") Then
					Center(.X, .Y)
					RefreshScreen()
					
					OpenMessageForm()
					If .IsMessageDefined("分離(" & .Name & ")") Then
						.PilotMessage("分離(" & .Name & ")")
					ElseIf .IsMessageDefined("分離(" & fname & ")") Then 
						.PilotMessage("分離(" & fname & ")")
					Else
						.PilotMessage("分離")
					End If
					CloseMessageForm()
				End If
				
				'アニメ表示
				If .IsAnimationDefined("分離(" & .Name & ")") Then
					.PlayAnimation("分離(" & .Name & ")")
				ElseIf .IsAnimationDefined("分離(" & fname & ")") Then 
					.PlayAnimation("分離(" & fname & ")")
				ElseIf .IsAnimationDefined("分離") Then 
					.PlayAnimation("分離")
				ElseIf .IsSpecialEffectDefined("分離(" & .Name & ")") Then 
					.SpecialEffect("分離(" & .Name & ")")
				ElseIf .IsSpecialEffectDefined("分離(" & fname & ")") Then 
					.SpecialEffect("分離(" & fname & ")")
				Else
					.SpecialEffect("分離")
				End If
				
				'パーツ分離
				uname = .Name
				.Transform(tname)
				SelectedUnit = MapDataForUnit(.X, .Y)
				DisplayUnitStatus(SelectedUnit)
			Else
				'通常の分離を行う場合
				
				ret = MsgBox("分離しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "分離")
				If ret = MsgBoxResult.Cancel Then
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				'ＢＧＭを変更
				If .IsFeatureAvailable("分離ＢＧＭ") Then
					BGM = SearchMidiFile(.FeatureData("分離ＢＧＭ"))
					If Len(BGM) > 0 Then
						StartBGM(.FeatureData("分離ＢＧＭ"))
						Sleep(500)
					End If
				End If
				
				fname = .FeatureName("分離")
				
				'メッセージを表示
				If .IsMessageDefined("分離(" & .Name & ")") Or .IsMessageDefined("分離(" & fname & ")") Or .IsMessageDefined("分離") Then
					Center(.X, .Y)
					RefreshScreen()
					
					OpenMessageForm()
					If .IsMessageDefined("分離(" & .Name & ")") Then
						.PilotMessage("分離(" & .Name & ")")
					ElseIf .IsMessageDefined("分離(" & fname & ")") Then 
						.PilotMessage("分離(" & fname & ")")
					Else
						.PilotMessage("分離")
					End If
					CloseMessageForm()
				End If
				
				'アニメ表示
				If .IsAnimationDefined("分離(" & .Name & ")") Then
					.PlayAnimation("分離(" & .Name & ")")
				ElseIf .IsAnimationDefined("分離(" & fname & ")") Then 
					.PlayAnimation("分離(" & fname & ")")
				ElseIf .IsAnimationDefined("分離") Then 
					.PlayAnimation("分離")
				ElseIf .IsSpecialEffectDefined("分離(" & .Name & ")") Then 
					.SpecialEffect("分離(" & .Name & ")")
				ElseIf .IsSpecialEffectDefined("分離(" & fname & ")") Then 
					.SpecialEffect("分離(" & fname & ")")
				Else
					.SpecialEffect("分離")
				End If
				
				'分離
				uname = .Name
				.Split_Renamed()
				
				'選択ユニットを再設定
				SelectedUnit = UList.Item(LIndex(.FeatureData("分離"), 2))
				
				DisplayUnitStatus(SelectedUnit)
				
			End If
		End With
		
		'分離イベント
		HandleEvent("分離", SelectedUnit.MainPilot.ID, uname)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		IsCanceled = False
		
		'カーソル自動移動
		If AutoMoveCursor Then
			MoveCursorPos("ユニット選択", SelectedUnit)
		End If
		
		'ハイパーモード＆ノーマルモードの自動発動チェック
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
		
		CommandState = "ユニット選択"
		
		UnlockGUI()
	End Sub
	
	'「合体」コマンド
	' MOD START MARGE
	'Public Sub CombineCommand()
	Private Sub CombineCommand()
		' MOD END MARGE
		Dim i, j As Short
		Dim list() As String
		Dim u As Unit
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		ReDim list(0)
		ReDim ListItemFlag(0)
		With SelectedUnit
			If MapFileName = "" Then
				'ユニットステータスコマンドの時
				'パーツ合体ならば……
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If MainForm.mnuUnitCommandItem.Item(CombineCmdID).Caption = "パーツ合体" And .IsFeatureAvailable("パーツ合体") Then
					'パーツ合体を実施
					.Transform(.FeatureData("パーツ合体"))
					DisplayUnitStatus(MapDataForUnit(.X, .Y))
					MapDataForUnit(.X, .Y).CheckAutoHyperMode()
					MapDataForUnit(.X, .Y).CheckAutoNormalMode()
					
					'ユニットリストの表示を更新
					MakeUnitList()
					
					'コマンドを終了
					CommandState = "ユニット選択"
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'選択可能な合体パターンのリストを作成
			For i = 1 To .CountFeature
				If .Feature(i) = "合体" And (LLength(.FeatureData(i)) > 3 Or MapFileName = "") Then
					If Not UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
						GoTo NextLoop
					End If
					
					For j = 3 To LLength(.FeatureData(i))
						u = UList.Item(LIndex(.FeatureData(i), j))
						If u Is Nothing Then
							GoTo NextLoop
						End If
						If Not u.IsOperational Then
							GoTo NextLoop
						End If
						If u.Status_Renamed <> "出撃" And u.CurrentForm.IsFeatureAvailable("合体制限") Then
							GoTo NextLoop
						End If
						If MapFileName <> "" Then
							If System.Math.Abs(.X - u.CurrentForm.X) > 2 Or System.Math.Abs(.Y - u.CurrentForm.Y) > 2 Then
								GoTo NextLoop
							End If
						End If
					Next 
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim ListItemFlag(UBound(list))
					list(UBound(list)) = LIndex(.FeatureData(i), 2)
					ListItemFlag(UBound(list)) = False
				End If
NextLoop: 
			Next 
			
			'どの合体を行うかを選択
			If UBound(list) = 1 Then
				i = 1
			Else
				TopItem = 1
				i = ListBox("合体後の形態", list, "名前")
				If i = 0 Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
		End With
		
		If MapFileName = "" Then
			'ユニットステータスコマンドの時
			SelectedUnit.Combine(list(i), True)
			
			'ハイパーモード・ノーマルモードの自動発動をチェック
			UList.CheckAutoHyperMode()
			UList.CheckAutoNormalMode()
			
			'ユニットリストの表示を更新
			MakeUnitList()
			
			'コマンドを終了
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		
		'合体！
		SelectedUnit.Combine(list(i))
		
		'ハイパーモード＆ノーマルモードの自動発動
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
		
		'合体後のユニットを選択しておく
		SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
		
		'行動数消費
		SelectedUnit.UseAction()
		
		'カーソル自動移動
		If AutoMoveCursor Then
			MoveCursorPos("ユニット選択", SelectedUnit)
		End If
		
		DisplayUnitStatus(SelectedUnit)
		
		'合体イベント
		HandleEvent("合体", SelectedUnit.MainPilot.ID, SelectedUnit.Name)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "ユニット選択"
			UnlockGUI()
			Exit Sub
		End If
		
		'行動終了
		WaitCommand(True)
	End Sub
	
	'「換装」コマンド
	' MOD START MARGE
	'Public Sub ExchangeFormCommand()
	Public Sub ExchangeFormCommand()
		' MOD END MARGE
		Dim list() As String
		Dim id_list() As String
		Dim j, i, k As Short
		Dim max_value As Integer
		Dim ret As Short
		Dim fdata As String
		Dim farray() As String
		
		LockGUI()
		
		With SelectedUnit
			fdata = .FeatureData("換装")
			
			'選択可能な換装先のリストを作成
			ReDim list(0)
			ReDim id_list(0)
			ReDim ListItemComment(0)
			For i = 1 To LLength(fdata)
				With .OtherForm(LIndex(fdata, i))
					If .IsAvailable Then
						ReDim Preserve list(UBound(list) + 1)
						ReDim Preserve id_list(UBound(list))
						ReDim Preserve ListItemComment(UBound(list))
						id_list(UBound(list)) = .Name
						
						'各形態の表示内容を作成
						If SelectedUnit.Nickname0 = .Nickname Then
							list(UBound(list)) = RightPaddedString(.Name, 27)
						Else
							list(UBound(list)) = RightPaddedString(.Nickname0, 27)
						End If
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5) & " " & .Data.Adaption
						
						'最大攻撃力
						max_value = 0
						For j = 1 To .CountWeapon
							If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
								If .WeaponPower(j, "") > max_value Then
									max_value = .WeaponPower(j, "")
								End If
							End If
						Next 
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(max_value), 7)
						
						'最大射程
						max_value = 0
						For j = 1 To .CountWeapon
							If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
								If .WeaponMaxRange(j) > max_value Then
									max_value = .WeaponMaxRange(j)
								End If
							End If
						Next 
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(max_value), 5)
						
						'換装先が持つ特殊能力一覧
						ReDim farray(0)
						For j = 1 To .CountFeature
							If .FeatureName(j) <> "" Then
								'重複する特殊能力は表示しないようチェック
								For k = 1 To UBound(farray)
									If .FeatureName(j) = farray(k) Then
										Exit For
									End If
								Next 
								If k > UBound(farray) Then
									ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .FeatureName(j) & " "
									ReDim Preserve farray(UBound(farray) + 1)
									farray(UBound(farray)) = .FeatureName(j)
								End If
							End If
						Next 
					End If
				End With
			Next 
			ReDim ListItemFlag(UBound(list))
			
			'どの形態に換装するかを選択
			TopItem = 1
			ret = ListBox("変更先選択", list, "ユニット                     " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動", Nothing, 4) & " " & "適応 攻撃力 射程", "カーソル移動,コメント")
			If ret = 0 Then
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			
			'換装を実施
			.Transform((.OtherForm(id_list(ret)).Name))
			
			'ユニットリストの再構築
			MakeUnitList()
			
			'カーソル自動移動
			If AutoMoveCursor Then
				MoveCursorPos("ユニット選択", .CurrentForm)
			End If
			DisplayUnitStatus(.CurrentForm)
		End With
		
		CommandState = "ユニット選択"
		
		UnlockGUI()
	End Sub
	
	
	'「発進」コマンドを開始
	' MOD START MARGE
	'Public Sub StartLaunchCommand()
	Private Sub StartLaunchCommand()
		' MOD END MARGE
		Dim i, ret As Short
		Dim list() As String
		
		With SelectedUnit
			ReDim list(.CountUnitOnBoard)
			ReDim ListItemID(.CountUnitOnBoard)
			ReDim ListItemFlag(.CountUnitOnBoard)
		End With
		
		'母艦に搭載しているユニットの一覧を作成
		For i = 1 To SelectedUnit.CountUnitOnBoard
			With SelectedUnit.UnitOnBoard(i)
				list(i) = RightPaddedString(.Nickname0, 25) & RightPaddedString(.MainPilot.Nickname, 17) & LeftPaddedString(VB6.Format(.MainPilot.Level), 2) & " " & RightPaddedString(VB6.Format(.HP) & "/" & VB6.Format(.MaxHP), 12) & RightPaddedString(VB6.Format(.EN) & "/" & VB6.Format(.MaxEN), 8)
				ListItemID(i) = .ID
				If .Action > 0 Then
					ListItemFlag(i) = False
				Else
					ListItemFlag(i) = True
				End If
			End With
		Next 
		
		'どのユニットを発進させるか選択
		TopItem = 1
		ret = ListBox("ユニット選択", list, "ユニット名               パイロット       Lv " & Term("ＨＰ", Nothing, 8) & Term("ＥＮ"), "カーソル移動")
		
		'キャンセルされた？
		If ret = 0 Then
			ReDim ListItemID(0)
			CancelCommand()
			Exit Sub
		End If
		
		SelectedCommand = "発進"
		
		'ユニットの発進処理
		SelectedTarget = UList.Item(ListItemID(ret))
		With SelectedTarget
			.X = SelectedUnit.X
			.Y = SelectedUnit.Y
			
			If .IsFeatureAvailable("テレポート") And (.Data.Speed = 0 Or LIndex(.FeatureData("テレポート"), 2) = "0") Then
				'テレポートによる発進
				AreaInTeleport(SelectedTarget)
			ElseIf .IsFeatureAvailable("ジャンプ") And (.Data.Speed = 0 Or LLength(.FeatureData("ジャンプ")) < 2 Or LIndex(.FeatureData("ジャンプ"), 2) = "0") Then 
				'ジャンプによる発進
				AreaInSpeed(SelectedTarget, True)
			Else
				'通常移動による発進
				AreaInSpeed(SelectedTarget)
			End If
			
			'母艦を中央表示
			Center(.X, .Y)
			
			'発進させるユニットを母艦の代わりに表示
			If .BitmapID = 0 Then
				With UList.Item(.Name)
					If SelectedTarget.Party0 = .Party0 And .BitmapID <> 0 And SelectedTarget.Bitmap = .Bitmap Then
						SelectedTarget.BitmapID = .BitmapID
					Else
						SelectedTarget.BitmapID = MakeUnitBitmap(SelectedTarget)
					End If
				End With
			End If
			
			MaskScreen()
		End With
		
		ReDim ListItemID(0)
		
		If CommandState = "コマンド選択" Then
			CommandState = "ターゲット選択"
		End If
	End Sub
	
	'「発進」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishLaunchCommand()
	Private Sub FinishLaunchCommand()
		' MOD END MARGE
		Dim ret As Short
		
		LockGUI()
		
		With SelectedTarget
			'発進コマンドの目的地にユニットがいた場合
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("母艦") Then
					ret = MsgBox("着艦しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "着艦")
				Else
					ret = MsgBox("合体しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "合体")
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'メッセージの表示
			If .IsMessageDefined("発進(" & .Name & ")") Then
				OpenMessageForm()
				.PilotMessage("発進(" & .Name & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("発進") Then 
				OpenMessageForm()
				.PilotMessage("発進")
				CloseMessageForm()
			End If
			
			.SpecialEffect("発進", .Name)
			
			PrevUnitArea = .Area
			PrevUnitEN = .EN
			.Status_Renamed = "出撃"
			
			'指定した位置に発進したユニットを移動
			.Move(SelectedX, SelectedY)
		End With
		
		'発進したユニットを母艦から降ろす
		With SelectedUnit
			PrevUnitX = .X
			PrevUnitY = .Y
			.UnloadUnit((SelectedTarget.ID))
			
			'母艦の位置には発進したユニットが表示されているので元に戻しておく
			MapDataForUnit(.X, .Y) = SelectedUnit
			PaintUnitBitmap(SelectedUnit)
		End With
		
		SelectedUnit = SelectedTarget
		With SelectedUnit
			If MapDataForUnit(.X, .Y).ID <> .ID Then
				RedrawScreen()
				CommandState = "ユニット選択"
				UnlockGUI()
				Exit Sub
			End If
		End With
		
		CommandState = "移動後コマンド選択"
		
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'「命令」コマンドを開始
	' MOD START MARGE
	'Public Sub StartOrderCommand()
	Private Sub StartOrderCommand()
		' MOD END MARGE
		Dim list() As String
		Dim i, ret, j As Short
		
		LockGUI()
		
		ReDim list(4)
		ReDim ListItemFlag(4)
		
		'可能な命令内容一覧を作成
		list(1) = "自由：自由に行動させる"
		list(2) = "移動：指定した位置に移動"
		list(3) = "攻撃：指定した敵を攻撃"
		list(4) = "護衛：指定したユニットを護衛"
		If Not SelectedUnit.Summoner Is Nothing Or Not SelectedUnit.Master Is Nothing Then
			ReDim Preserve list(5)
			ReDim Preserve ListItemFlag(5)
			If Not SelectedUnit.Master Is Nothing Then
				list(5) = "帰還：主人の所に戻る"
			Else
				list(5) = "帰還：召喚主の所に戻る"
			End If
		End If
		
		'命令する行動パターンを選択
		ret = ListBox("命令", list, "行動パターン")
		
		'選択された行動パターンに応じてターゲット領域を表示
		Select Case ret
			Case 0
				CancelCommand()
			Case 1 '自由
				SelectedUnit.Mode = "通常"
				CommandState = "ユニット選択"
				DisplayUnitStatus(SelectedUnit)
			Case 2 '移動
				SelectedCommand = "移動命令"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						MaskData(i, j) = False
					Next 
				Next 
				MaskScreen()
				CommandState = "ターゲット選択"
			Case 3 '攻撃
				SelectedCommand = "攻撃命令"
				AreaWithUnit("味方の敵")
				MaskData(SelectedUnit.X, SelectedUnit.Y) = True
				MaskScreen()
				CommandState = "ターゲット選択"
			Case 4 '護衛
				SelectedCommand = "護衛命令"
				AreaWithUnit("味方")
				MaskData(SelectedUnit.X, SelectedUnit.Y) = True
				MaskScreen()
				CommandState = "ターゲット選択"
			Case 5 '帰還
				If Not SelectedUnit.Master Is Nothing Then
					SelectedUnit.Mode = SelectedUnit.Master.MainPilot.ID
				Else
					SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot.ID
				End If
				CommandState = "ユニット選択"
				DisplayUnitStatus(SelectedUnit)
		End Select
		
		UnlockGUI()
	End Sub
	
	'「命令」コマンドを終了
	' MOD START MARGE
	'Public Sub FinishOrderCommand()
	Private Sub FinishOrderCommand()
		' MOD END MARGE
		Select Case SelectedCommand
			Case "移動命令"
				SelectedUnit.Mode = VB6.Format(SelectedX) & " " & VB6.Format(SelectedY)
			Case "攻撃命令", "護衛命令"
				SelectedUnit.Mode = SelectedTarget.MainPilot.ID
		End Select
		If DisplayedUnit Is SelectedUnit Then
			DisplayUnitStatus(SelectedUnit)
		End If
		
		RedrawScreen()
		
		CommandState = "ユニット選択"
	End Sub
	
	
	'「特殊能力一覧」コマンド
	' MOD START MARGE
	'Public Sub FeatureListCommand()
	Private Sub FeatureListCommand()
		' MOD END MARGE
		Dim list() As String
		Dim id_list() As String
		Dim is_unit_feature() As Boolean
		Dim i, j As Short
		Dim ret As Short
		Dim fname0, fname, ftype As String
		
		LockGUI()
		
		'表示する特殊能力名一覧の作成
		ReDim list(0)
		Dim id_ist(0) As Object
		ReDim is_unit_feature(0)
		
		'武器・防具クラス
		If IsOptionDefined("アイテム交換") Then
			With SelectedUnit
				If .IsFeatureAvailable("武器クラス") Or .IsFeatureAvailable("防具クラス") Then
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve is_unit_feature(UBound(list))
					list(UBound(list)) = "武器・防具クラス"
					id_list(UBound(list)) = "武器・防具クラス"
					is_unit_feature(UBound(list)) = True
				End If
			End With
		End If
		
		With SelectedUnit.MainPilot
			'パイロット特殊能力
			For i = 1 To .CountSkill
				Select Case .Skill(i)
					Case "得意技", "不得手"
						fname = .Skill(i)
					Case Else
						fname = .SkillName(i)
				End Select
				
				'非表示の能力は除く
				If InStr(fname, "非表示") > 0 Then
					GoTo NextSkill
				End If
				
				'既に表示されていればスキップ
				For j = 1 To UBound(list)
					If list(j) = fname Then
						GoTo NextSkill
					End If
				Next 
				
				'リストに追加
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				list(UBound(list)) = fname
				id_list(UBound(list)) = VB6.Format(i)
NextSkill: 
			Next 
		End With
		With SelectedUnit
			'付加・強化されたパイロット用特殊能力
			For i = 1 To .CountCondition
				'パイロット能力付加または強化？
				If Right(.Condition(i), 3) <> "付加２" And Right(.Condition(i), 3) <> "強化２" Then
					GoTo NextSkill2
				End If
				
				ftype = Left(.Condition(i), Len(.Condition(i)) - 3)
				
				'非表示の能力？
				Select Case LIndex(.ConditionData(i), 1)
					Case "非表示", "解説"
						GoTo NextSkill2
				End Select
				
				'有効時間が残っている？
				If .ConditionLifetime(i) = 0 Then
					GoTo NextSkill2
				End If
				
				'表示名称
				fname = .MainPilot.SkillName(ftype)
				If InStr(fname, "非表示") > 0 Then
					GoTo NextSkill2
				End If
				
				'既に表示していればスキップ
				For j = 1 To UBound(list)
					If list(j) = fname Then
						GoTo NextSkill2
					End If
				Next 
				
				'リストに追加
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				list(UBound(list)) = fname
				id_list(UBound(list)) = ftype
NextSkill2: 
			Next 
			ReDim Preserve is_unit_feature(UBound(list))
			
			'ユニット用特殊能力
			'付加された特殊能力より先に固有の特殊能力を表示
			If .CountAllFeature > .AdditionalFeaturesNum Then
				i = .AdditionalFeaturesNum + 1
			Else
				i = 1
			End If
			Do While i <= .CountAllFeature
				'非表示の特殊能力を排除
				If .AllFeatureName(i) = "" Then
					GoTo NextFeature
				End If
				
				'合体の場合は合体後の形態が作成されていなければならない
				If .AllFeature(i) = "合体" And Not UList.IsDefined(LIndex(.AllFeatureData(i), 2)) Then
					GoTo NextFeature
				End If
				
				'既に表示していればスキップ
				For j = 1 To UBound(list)
					If list(j) = .AllFeatureName(i) Then
						GoTo NextFeature
					End If
				Next 
				
				'リストに追加
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve is_unit_feature(UBound(list))
				list(UBound(list)) = .AllFeatureName(i)
				id_list(UBound(list)) = VB6.Format(i)
				is_unit_feature(UBound(list)) = True
NextFeature: 
				If i = .AdditionalFeaturesNum Then
					Exit Do
				ElseIf i = .CountFeature Then 
					'付加された特殊能力は後から表示
					If .AdditionalFeaturesNum > 0 Then
						i = 0
					End If
				End If
				i = i + 1
			Loop 
			
			'アビリティで付加・強化されたパイロット用特殊能力
			For i = 1 To .CountCondition
				'パイロット能力付加または強化？
				If Right(.Condition(i), 2) <> "付加" And Right(.Condition(i), 2) <> "強化" Then
					GoTo NextSkill3
				End If
				
				ftype = Left(.Condition(i), Len(.Condition(i)) - 2)
				
				'非表示の能力？
				If ftype = "メッセージ" Then
					GoTo NextSkill3
				End If
				Select Case LIndex(.ConditionData(i), 1)
					Case "非表示", "解説"
						GoTo NextSkill3
				End Select
				
				'有効時間が残っている？
				If .ConditionLifetime(i) = 0 Then
					GoTo NextSkill3
				End If
				
				'表示名称
				If .FeatureName0(ftype) = "" Then
					GoTo NextSkill3
				End If
				fname = .MainPilot.SkillName0(ftype)
				If InStr(fname, "非表示") > 0 Then
					GoTo NextSkill3
				End If
				
				'付加されたユニット用特殊能力として既に表示していればスキップ
				For j = 1 To UBound(list)
					If list(j) = fname Then
						GoTo NextSkill3
					End If
				Next 
				
				fname = .MainPilot.SkillName(ftype)
				If InStr(fname, "Lv") > 0 Then
					fname0 = Left(fname, InStr(fname, "Lv") - 1)
				Else
					fname0 = fname
				End If
				
				'パイロット用特殊能力として既に表示していればスキップ
				For j = 1 To UBound(list)
					If list(j) = fname Or list(j) = fname0 Then
						GoTo NextSkill3
					End If
				Next 
				
				'リストに追加
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve is_unit_feature(UBound(list))
				list(UBound(list)) = fname
				id_list(UBound(list)) = ftype
				is_unit_feature(UBound(list)) = False
NextSkill3: 
			Next 
		End With
		ReDim ListItemFlag(UBound(list))
		
		Select Case UBound(list)
			Case 0
			Case 1
				If AutoMoveCursor Then
					SaveCursorPos()
				End If
				If id_list(ret) = "武器・防具クラス" Then
					FeatureHelp(SelectedUnit, id_list(1), False)
				ElseIf is_unit_feature(1) Then 
					FeatureHelp(SelectedUnit, id_list(1), StrToLng(id_list(1)) <= SelectedUnit.AdditionalFeaturesNum)
				Else
					SkillHelp(SelectedUnit.MainPilot, id_list(1))
				End If
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
			Case Else
				TopItem = 1
				ret = ListBox("特殊能力一覧", list, "能力名", "表示のみ")
				If AutoMoveCursor Then
					MoveCursorPos("ダイアログ")
				End If
				Do While True
					ret = ListBox("特殊能力一覧", list, "能力名", "連続表示")
					'listが一定なので連続表示を流用
					frmListBox.Hide()
					If ret = 0 Then
						Exit Do
					End If
					If id_list(ret) = "武器・防具クラス" Then
						FeatureHelp(SelectedUnit, id_list(ret), False)
					ElseIf is_unit_feature(ret) Then 
						FeatureHelp(SelectedUnit, id_list(ret), CDbl(id_list(ret)) <= SelectedUnit.AdditionalFeaturesNum)
					Else
						SkillHelp(SelectedUnit.MainPilot, id_list(ret))
					End If
				Loop 
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
		End Select
		
		CommandState = "ユニット選択"
		
		UnlockGUI()
	End Sub
	
	'「武器一覧」コマンド
	' MOD START MARGE
	'Public Sub WeaponListCommand()
	Private Sub WeaponListCommand()
		' MOD END MARGE
		Dim list() As String
		Dim i As Short
		Dim buf As String
		Dim w As Short
		Dim wclass As String
		Dim atype, alevel As String
		Dim c As String
		
		LockGUI()
		
		Dim min_range, max_range As Short
		Do While True
			w = WeaponListBox(SelectedUnit, "武装一覧", "一覧")
			SelectedWeapon = w
			
			If SelectedWeapon <= 0 Then
				'キャンセル
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				frmListBox.Hide()
				UnlockGUI()
				CommandState = "ユニット選択"
				Exit Sub
			End If
			
			'指定された武器の属性一覧を作成
			ReDim list(0)
			i = 0
			With SelectedUnit
				wclass = .WeaponClass(w)
				
				Do While i <= Len(wclass)
					i = i + 1
					buf = GetClassBundle(wclass, i)
					atype = ""
					alevel = ""
					
					'非表示？
					If buf = "|" Then
						Exit Do
					End If
					
					'Ｍ属性
					If Mid(wclass, i, 1) = "Ｍ" Then
						i = i + 1
						buf = buf & Mid(wclass, i, 1)
					End If
					
					'レベル指定
					If Mid(wclass, i + 1, 1) = "L" Then
						i = i + 2
						c = Mid(wclass, i, 1)
						Do While IsNumeric(c) Or c = "." Or c = "-"
							alevel = alevel & c
							i = i + 1
							c = Mid(wclass, i, 1)
						Loop 
						i = i - 1
					End If
					
					'属性の名称
					atype = AttributeName(SelectedUnit, buf)
					If Len(atype) > 0 Then
						ReDim Preserve list(UBound(list) + 1)
						
						If Len(alevel) > 0 Then
							list(UBound(list)) = RightPaddedString(buf & "L" & alevel, 8) & atype & "レベル" & StrConv(alevel, VbStrConv.Wide)
						Else
							list(UBound(list)) = RightPaddedString(buf, 8) & atype
						End If
					End If
				Loop 
				
				If MapFileName <> "" Then
					ReDim Preserve list(UBound(list) + 1)
					list(UBound(list)) = "射程範囲"
				End If
				
				If UBound(list) > 0 Then
					TopItem = 1
					Do While True
						If UBound(list) = 1 And list(1) = "射程範囲" Then
							i = 1
						Else
							ReDim ListItemFlag(UBound(list))
							i = ListBox("武器属性一覧", list, "属性    効果", "連続表示")
						End If
						
						If i = 0 Then
							'キャンセル
							Exit Do
						ElseIf list(i) = "射程範囲" Then 
							
							frmListBox.Hide()
							
							'武器の射程を求めておく
							min_range = .Weapon(w).MinRange
							max_range = .WeaponMaxRange(w)
							
							'射程範囲表示
							If (max_range = 1 Or .IsWeaponClassifiedAs(w, "Ｐ")) And Not .IsWeaponClassifiedAs(w, "Ｑ") Then
								AreaInReachable(SelectedUnit, max_range, .Party & "の敵")
							ElseIf .IsWeaponClassifiedAs(w, "Ｍ直") Then 
								AreaInCross(.X, .Y, min_range, max_range)
							ElseIf .IsWeaponClassifiedAs(w, "Ｍ拡") Then 
								AreaInWideCross(.X, .Y, min_range, max_range)
							ElseIf .IsWeaponClassifiedAs(w, "Ｍ扇") Then 
								AreaInSectorCross(.X, .Y, min_range, max_range, .WeaponLevel(w, "Ｍ扇"))
							ElseIf .IsWeaponClassifiedAs(w, "Ｍ全") Or .IsWeaponClassifiedAs(w, "Ｍ線") Then 
								AreaInRange(.X, .Y, max_range, min_range, "すべて")
							ElseIf .IsWeaponClassifiedAs(w, "Ｍ投") Then 
								max_range = max_range + .WeaponLevel(w, "Ｍ投")
								min_range = min_range - .WeaponLevel(w, "Ｍ投")
								min_range = MaxLng(min_range, 1)
								AreaInRange(.X, .Y, max_range, min_range, "すべて")
							ElseIf .IsWeaponClassifiedAs(w, "Ｍ移") Then 
								AreaInMoveAction(SelectedUnit, max_range)
							Else
								AreaInRange(.X, .Y, max_range, min_range, .Party & "の敵")
							End If
							Center(.X, .Y)
							MaskScreen()
							
							'先行入力されていたクリックイベントを解消
							System.Windows.Forms.Application.DoEvents()
							WaitClickMode = True
							IsFormClicked = False
							
							'クリックされるまで待つ
							Do Until IsFormClicked
								Sleep(25)
								System.Windows.Forms.Application.DoEvents()
								
								If IsRButtonPressed(True) Then
									Exit Do
								End If
							Loop 
							
							RedrawScreen()
							
							If UBound(list) = 1 And list(i) = "射程範囲" Then
								Exit Do
							End If
						Else
							'指定された属性の解説を表示
							frmListBox.Hide()
							AttributeHelp(SelectedUnit, LIndex(list(i), 1), w)
						End If
					Loop 
				End If
			End With
		Loop 
	End Sub
	
	'「アビリティ一覧」コマンド
	' MOD START MARGE
	'Public Sub AbilityListCommand()
	Private Sub AbilityListCommand()
		' MOD END MARGE
		Dim list() As String
		Dim i As Short
		Dim buf As String
		Dim a As Short
		Dim alevel, atype, aclass As String
		Dim c As String
		
		LockGUI()
		
		Dim min_range, max_range As Short
		Do While True
			a = AbilityListBox(SelectedUnit, Term("アビリティ", SelectedUnit) & "一覧", "一覧")
			SelectedAbility = a
			
			If SelectedAbility <= 0 Then
				'キャンセル
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				frmListBox.Hide()
				UnlockGUI()
				CommandState = "ユニット選択"
				Exit Sub
			End If
			
			'指定されたアビリティの属性一覧を作成
			ReDim list(0)
			i = 0
			With SelectedUnit
				aclass = .Ability(a).Class_Renamed
				
				Do While i <= Len(aclass)
					i = i + 1
					buf = GetClassBundle(aclass, i)
					atype = ""
					alevel = ""
					
					'非表示？
					If buf = "|" Then
						Exit Do
					End If
					
					'Ｍ属性
					If Mid(aclass, i, 1) = "Ｍ" Then
						i = i + 1
						buf = buf & Mid(aclass, i, 1)
					End If
					
					'レベル指定
					If Mid(aclass, i + 1, 1) = "L" Then
						i = i + 2
						c = Mid(aclass, i, 1)
						Do While IsNumeric(c) Or c = "." Or c = "-"
							alevel = alevel & c
							i = i + 1
							c = Mid(aclass, i, 1)
						Loop 
						i = i - 1
					End If
					
					'属性の名称
					atype = AttributeName(SelectedUnit, buf, True)
					If Len(atype) > 0 Then
						ReDim Preserve list(UBound(list) + 1)
						
						If Len(alevel) > 0 Then
							list(UBound(list)) = RightPaddedString(buf & "L" & alevel, 8) & atype & "レベル" & StrConv(alevel, VbStrConv.Wide)
						Else
							list(UBound(list)) = RightPaddedString(buf, 8) & atype
						End If
					End If
				Loop 
				
				If MapFileName <> "" Then
					ReDim Preserve list(UBound(list) + 1)
					list(UBound(list)) = "射程範囲"
				End If
				
				If UBound(list) > 0 Then
					TopItem = 1
					Do While True
						If UBound(list) = 1 And list(1) = "射程範囲" Then
							i = 1
						Else
							ReDim ListItemFlag(UBound(list))
							i = ListBox("アビリティ属性一覧", list, "属性    効果", "連続表示")
						End If
						
						If i = 0 Then
							'キャンセル
							Exit Do
						ElseIf list(i) = "射程範囲" Then 
							
							frmListBox.Hide()
							
							'アビリティの射程を求めておく
							min_range = .AbilityMinRange(a)
							max_range = .AbilityMaxRange(a)
							
							'射程範囲表示
							If (max_range = 1 Or .IsAbilityClassifiedAs(a, "Ｐ")) And Not .IsAbilityClassifiedAs(a, "Ｑ") Then
								AreaInReachable(SelectedUnit, max_range, "すべて")
							ElseIf .IsAbilityClassifiedAs(a, "Ｍ直") Then 
								AreaInCross(.X, .Y, min_range, max_range)
							ElseIf .IsAbilityClassifiedAs(a, "Ｍ拡") Then 
								AreaInWideCross(.X, .Y, min_range, max_range)
							ElseIf .IsAbilityClassifiedAs(a, "Ｍ扇") Then 
								AreaInSectorCross(.X, .Y, min_range, max_range, .AbilityLevel(a, "Ｍ扇"))
							ElseIf .IsAbilityClassifiedAs(a, "Ｍ投") Then 
								max_range = max_range + .AbilityLevel(a, "Ｍ投")
								min_range = min_range - .AbilityLevel(a, "Ｍ投")
								min_range = MaxLng(min_range, 1)
								AreaInRange(.X, .Y, max_range, min_range, "すべて")
							ElseIf .IsAbilityClassifiedAs(a, "Ｍ移") Then 
								AreaInMoveAction(SelectedUnit, max_range)
							Else
								AreaInRange(.X, .Y, max_range, min_range, "すべて")
							End If
							Center(.X, .Y)
							MaskScreen()
							
							'先行入力されていたクリックイベントを解消
							System.Windows.Forms.Application.DoEvents()
							WaitClickMode = True
							IsFormClicked = False
							
							'クリックされるまで待つ
							Do Until IsFormClicked
								Sleep(25)
								System.Windows.Forms.Application.DoEvents()
								
								If IsRButtonPressed(True) Then
									Exit Do
								End If
							Loop 
							
							RedrawScreen()
							
							If UBound(list) = 1 And list(i) = "射程範囲" Then
								Exit Do
							End If
						Else
							'指定された属性の解説を表示
							frmListBox.Hide()
							AttributeHelp(SelectedUnit, LIndex(list(i), 1), a, True)
						End If
					Loop 
				End If
			End With
		Loop 
	End Sub
	
	'「移動範囲」コマンド
	' MOD START MARGE
	'Public Sub ShowAreaInSpeedCommand()
	Private Sub ShowAreaInSpeedCommand()
		' MOD END MARGE
		SelectedCommand = "移動範囲"
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		AreaInSpeed(SelectedUnit)
		Center(SelectedUnit.X, SelectedUnit.Y)
		MaskScreen()
		CommandState = "ターゲット選択"
	End Sub
	
	'「射程範囲」コマンド
	' MOD START MARGE
	'Public Sub ShowAreaInRangeCommand()
	Private Sub ShowAreaInRangeCommand()
		' MOD END MARGE
		Dim w, i, max_range As Short
		
		SelectedCommand = "射程範囲"
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		With SelectedUnit
			'最大の射程を持つ武器を探す
			w = 0
			max_range = 0
			For i = 1 To .CountWeapon
				If .IsWeaponAvailable(i, "ステータス") And Not .IsWeaponClassifiedAs(i, "Ｍ") Then
					If .WeaponMaxRange(i) > max_range Then
						w = i
						max_range = .WeaponMaxRange(i)
					End If
				End If
			Next 
			
			'見つかった最大の射程を持つ武器の射程範囲を選択
			AreaInRange(.X, .Y, max_range, 1, .Party & "の敵")
			
			'射程範囲を表示
			Center(.X, .Y)
			MaskScreen()
		End With
		
		CommandState = "ターゲット選択"
	End Sub
	
	'「待機」コマンド
	'他のコマンドの終了処理にも使われる
	' MOD START MARGE
	'Public Sub WaitCommand(Optional ByVal WithoutAction As Boolean)
	' 今後どうしてもPrivateじゃダメな処理が出たら戻してください
	Private Sub WaitCommand(Optional ByVal WithoutAction As Boolean = False)
		' MOD END MARGE
		Dim p As Pilot
		Dim i As Short
		
		'コマンド終了時はターゲットを解除
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		
		'ユニットにパイロットが乗っていない？
		If SelectedUnit.CountPilot = 0 Then
			CommandState = "ユニット選択"
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		If Not WithoutAction Then
			'残り行動数を減少させる
			SelectedUnit.UseAction()
			
			'持続期間が「移動」のスペシャルパワー効果を削除
			If InStr(CommandState, "移動後") > 0 Then
				SelectedUnit.RemoveSpecialPowerInEffect("移動")
			End If
		End If
		
		CommandState = "ユニット選択"
		
		'アップデート
		SelectedUnit.Update()
		PList.UpdateSupportMod(SelectedUnit)
		
		'ユニットが既に出撃していない？
		If SelectedUnit.Status_Renamed <> "出撃" Then
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		LockGUI()
		
		RedrawScreen()
		
		p = SelectedUnit.Pilot(1)
		
		'接触イベント
		For i = 1 To 4
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
			With SelectedUnit
				Select Case i
					Case 1
						If .X > 1 Then
							SelectedTarget = MapDataForUnit(.X - 1, .Y)
						End If
					Case 2
						If .X < MapWidth Then
							SelectedTarget = MapDataForUnit(.X + 1, .Y)
						End If
					Case 3
						If .Y > 1 Then
							SelectedTarget = MapDataForUnit(.X, .Y - 1)
						End If
					Case 4
						If .Y < MapHeight Then
							SelectedTarget = MapDataForUnit(.X, .Y + 1)
						End If
				End Select
			End With
			
			If Not SelectedTarget Is Nothing Then
				HandleEvent("接触", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
				'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SelectedTarget = Nothing
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
				If SelectedUnit.Status_Renamed <> "出撃" Then
					RedrawScreen()
					ClearUnitStatus()
					UnlockGUI()
					Exit Sub
				End If
			End If
		Next 
		
		'進入イベント
		HandleEvent("進入", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		If SelectedUnit.CountPilot = 0 Then
			RedrawScreen()
			ClearUnitStatus()
			UnlockGUI()
			Exit Sub
		End If
		
		'行動終了イベント
		HandleEvent("行動終了", SelectedUnit.MainPilot.ID)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		If SelectedUnit.CountPilot = 0 Then
			RedrawScreen()
			ClearUnitStatus()
			UnlockGUI()
			Exit Sub
		End If
		
		If Not p.Unit_Renamed Is Nothing Then
			SelectedUnit = p.Unit_Renamed
		End If
		
		If SelectedUnit.Action > 0 And SelectedUnit.CountPilot > 0 Then
			'カーソル自動移動
			If AutoMoveCursor Then
				MoveCursorPos("ユニット選択", SelectedUnit)
			End If
		End If
		
		'ハイパーモード・ノーマルモードの自動発動をチェック
		SelectedUnit.CurrentForm.CheckAutoHyperMode()
		SelectedUnit.CurrentForm.CheckAutoNormalMode()
		
		If IsPictureVisible Or IsCursorVisible Then
			RedrawScreen()
		End If
		
		UnlockGUI()
		
		'ステータスウィンドウの表示内容を更新
		If SelectedUnit.Status_Renamed = "出撃" And MainWidth = 15 Then
			DisplayUnitStatus(SelectedUnit)
		Else
			ClearUnitStatus()
		End If
	End Sub
	
	
	'マップコマンド実行
	Public Sub MapCommand(ByVal idx As Short)
		CommandState = "ユニット選択"
		
		Select Case idx
			Case EndTurnCmdID 'ターン終了
				If ViewMode Then
					ViewMode = False
					Exit Sub
				End If
				EndTurnCommand()
			Case DumpCmdID '中断
				DumpCommand()
			Case UnitListCmdID '部隊表
				UnitListCommand()
			Case SearchSpecialPowerCmdID 'スペシャルパワー検索
				SearchSpecialPowerCommand()
			Case GlobalMapCmdID '全体マップ
				GlobalMapCommand()
			Case OperationObjectCmdID '作戦目的
				LockGUI()
				HandleEvent("勝利条件")
				RedrawScreen()
				UnlockGUI()
			Case MapCommand1CmdID To MapCommand10CmdID 'マップコマンド
				LockGUI()
				HandleEvent(MapCommandLabelList(idx - MapCommand1CmdID + 1))
				UnlockGUI()
			Case AutoDefenseCmdID '自動反撃モード
				'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = Not MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked
				'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
					WriteIni("Option", "AutoDefense", "On")
				Else
					WriteIni("Option", "AutoDefense", "Off")
				End If
			Case ConfigurationCmdID '設定変更
				'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
				Load(frmConfiguration)
				frmConfiguration.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(frmConfiguration.Width)) / 2)
				frmConfiguration.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(frmConfiguration.Height)) / 3)
				frmConfiguration.ShowDialog()
				frmConfiguration.Close()
				'UPGRADE_NOTE: オブジェクト frmConfiguration をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				frmConfiguration = Nothing
			Case RestartCmdID 'リスタート
				RestartCommand()
			Case QuickLoadCmdID 'クイックロード
				QuickLoadCommand()
			Case QuickSaveCmdID 'クイックセーブ
				QuickSaveCommand()
		End Select
		IsScenarioFinished = False
	End Sub
	
	'「ターン終了」コマンド
	' MOD START MARGE
	'Public Sub EndTurnCommand()
	Private Sub EndTurnCommand()
		' MOD END MARGE
		Dim num As Short
		Dim ret As Short
		Dim u As Unit
		
		'行動していない味方ユニットの数を数える
		num = 0
		For	Each u In UList
			With u
				If .Party = "味方" And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") And .Action > 0 Then
					num = num + 1
				End If
			End With
		Next u
		
		'行動していないユニットがいれば警告
		If num > 0 Then
			ret = MsgBox("行動していないユニットが" & VB6.Format(num) & "体あります" & vbCr & "このターンを終了しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "終了")
		Else
			ret = 0
		End If
		
		Select Case ret
			Case 1
			Case 2
				Exit Sub
		End Select
		
		'行動終了していないユニットに対して行動終了イベントを実施
		For	Each SelectedUnit In UList
			With SelectedUnit
				If .Party = "味方" And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") And .Action > 0 Then
					HandleEvent("行動終了", .MainPilot.ID)
					If IsScenarioFinished Then
						IsScenarioFinished = False
						Exit Sub
					End If
				End If
			End With
		Next SelectedUnit
		
		'各陣営のフェイズに移行
		
		StartTurn("敵")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		StartTurn("中立")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		StartTurn("ＮＰＣ")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		'味方フェイズに戻る
		StartTurn("味方")
		IsScenarioFinished = False
	End Sub
	
	'ユニット一覧の表示
	' MOD START MARGE
	'Public Sub UnitListCommand()
	Private Sub UnitListCommand()
		' MOD END MARGE
		Dim list() As String
		Dim id_list() As String
		Dim j, i, ret As Short
		Dim uparty, sort_mode As String
		Dim mode_list() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim buf As String
		Dim u As Unit
		Dim pilot_status_mode As Boolean
		
		LockGUI()
		
		TopItem = 1
		EnlargeListBoxHeight()
		ReduceListBoxWidth()
		
		'デフォルトのソート方法
		uparty = "味方"
		sort_mode = "レベル"
		
Beginning: 
		
		'ユニット一覧のリストを作成
		ReDim list(1)
		ReDim id_list(1)
		list(1) = "▽陣営変更・並べ替え▽"
		For	Each u In UList
			With u
				If .Party0 = uparty And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") Then
					'未確認のユニットは表示しない
					If (IsOptionDefined("ユニット情報隠蔽") And (Not .IsConditionSatisfied("識別済み") And (.Party0 = "敵" Or .Party0 = "中立"))) Or .IsConditionSatisfied("ユニット情報隠蔽") Then
						GoTo NextUnit
					End If
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemFlag(UBound(list))
					
					If Not .IsFeatureAvailable("ダミーユニット") Then
						'通常のユニット表示
						If IsOptionDefined("等身大基準") Then
							'等身大基準を使った場合のユニット表示
							list(UBound(list)) = RightPaddedString(.Nickname0, 33) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3) & " "
						Else
							list(UBound(list)) = RightPaddedString(.Nickname0, 23)
							If .MainPilot.Nickname0 = "パイロット不在" Then
								'パイロットが乗っていない場合
								list(UBound(list)) = RightPaddedString(list(UBound(list)) & "", 34) & LeftPaddedString("", 2)
							Else
								list(UBound(list)) = RightPaddedString(list(UBound(list)) & .MainPilot.Nickname, 34) & LeftPaddedString(VB6.Format(.MainPilot.Level), 2)
							End If
							list(UBound(list)) = RightPaddedString(list(UBound(list)), 37)
						End If
						If .IsConditionSatisfied("データ不明") Then
							list(UBound(list)) = list(UBound(list)) & "?????/????? ???/???"
						Else
							list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.HP), 5) & "/" & LeftPaddedString(VB6.Format(.MaxHP), 5) & " " & LeftPaddedString(VB6.Format(.EN), 3) & "/" & LeftPaddedString(VB6.Format(.MaxEN), 3)
						End If
					Else
						'パイロットステータス表示時
						pilot_status_mode = True
						With .MainPilot
							list(UBound(list)) = RightPaddedString(.Nickname, 21) & LeftPaddedString(VB6.Format(.Level), 3) & LeftPaddedString(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP), 9) & "  "
							'使用可能なスペシャルパワー一覧
							For i = 1 To .CountSpecialPower
								If .SP >= .SpecialPowerCost(.SpecialPower(i)) Then
									list(UBound(list)) = list(UBound(list)) & SPDList.Item(.SpecialPower(i)).ShortName
								End If
							Next 
						End With
					End If
					
					If .Action = 0 Then
						list(UBound(list)) = list(UBound(list)) & "済"
					End If
					If .Status_Renamed = "格納" Then
						list(UBound(list)) = list(UBound(list)) & "格"
					End If
					
					id_list(UBound(list)) = .ID
					ListItemFlag(UBound(list)) = False
				End If
			End With
NextUnit: 
		Next u
		
SortList: 
		
		'ソート
		If InStr(sort_mode, "名称") = 0 Then
			'数値を使ったソート
			
			'まず並べ替えに使うキーのリストを作成
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "ＨＰ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).HP
						Next 
					Case "ＥＮ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).EN
						Next 
					Case "レベル", "パイロットレベル"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'キーを使って並べ換え
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'文字列を使ったソート
			
			'まず並べ替えに使うキーのリストを作成
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "名称", "ユニット名称"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "パイロット名称"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'キーを使って並べ換え
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		ReDim ListItemFlag(0)
		
		ReDim ListItemID(UBound(list))
		For i = 1 To UBound(list)
			ListItemID(i) = id_list(i)
		Next 
		
		'リストを表示
		If pilot_status_mode Then
			ret = ListBox(uparty & "パイロット一覧", list, "パイロット名       レベル    " & Term("ＳＰ", Nothing, 4) & "  " & Term("スペシャルパワー"), "連続表示")
		ElseIf IsOptionDefined("等身大基準") Then 
			ret = ListBox(uparty & "ユニット一覧", list, "ユニット名                        Lv     " & Term("ＨＰ", Nothing, 8) & Term("ＥＮ"), "連続表示")
		Else
			ret = ListBox(uparty & "ユニット一覧", list, "ユニット               パイロット Lv     " & Term("ＨＰ", Nothing, 8) & Term("ＥＮ"), "連続表示")
		End If
		
		Select Case ret
			Case 0
				'キャンセル
				frmListBox.Hide()
				ReduceListBoxHeight()
				EnlargeListBoxWidth()
				ReDim ListItemID(0)
				UnlockGUI()
				Exit Sub
			Case 1
				'表示する陣営
				ReDim mode_list(4)
				mode_list(1) = "味方一覧"
				mode_list(2) = "ＮＰＣ一覧"
				mode_list(3) = "敵一覧"
				mode_list(4) = "中立一覧"
				
				'ソート方法を選択
				If pilot_status_mode Then
					ReDim Preserve mode_list(7)
					mode_list(5) = "パイロット名称で並べ替え"
					mode_list(6) = "レベルで並べ替え"
					mode_list(7) = Term("ＳＰ") & "で並べ替え"
				ElseIf IsOptionDefined("等身大基準") Then 
					ReDim Preserve mode_list(8)
					mode_list(5) = "名称で並べ替え"
					mode_list(6) = "レベルで並べ替え"
					mode_list(7) = Term("ＨＰ") & "で並べ替え"
					mode_list(8) = Term("ＥＮ") & "で並べ替え"
				Else
					ReDim Preserve mode_list(9)
					mode_list(5) = Term("ＨＰ") & "で並べ替え"
					mode_list(6) = Term("ＥＮ") & "で並べ替え"
					mode_list(7) = "パイロットレベルで並べ替え"
					mode_list(8) = "ユニット名称で並べ替え"
					mode_list(9) = "パイロット名称で並べ替え"
				End If
				ReDim ListItemID(UBound(mode_list))
				ReDim ListItemFlag(UBound(mode_list))
				
				ret = ListBox("選択", mode_list, "一覧表示方法", "連続表示")
				
				'陣営を変更して再表示
				If ret > 0 Then
					If Right(mode_list(ret), 2) = "一覧" Then
						uparty = Left(mode_list(ret), Len(mode_list(ret)) - 2)
						GoTo Beginning
					ElseIf Right(mode_list(ret), 5) = "で並べ替え" Then 
						sort_mode = Left(mode_list(ret), Len(mode_list(ret)) - 5)
						GoTo SortList
					End If
				End If
				
				GoTo SortList
		End Select
		
		frmListBox.Hide()
		ReduceListBoxHeight()
		EnlargeListBoxWidth()
		
		'選択されたユニットを画面中央に表示
		u = UList.Item(ListItemID(ret))
		Center(u.X, u.Y)
		RefreshScreen()
		DisplayUnitStatus(u)
		
		'カーソル自動移動
		If AutoMoveCursor Then
			MoveCursorPos("ユニット選択", u)
		End If
		
		ReDim ListItemID(0)
		
		UnlockGUI()
	End Sub
	
	'スペシャルパワー検索コマンド
	' MOD START MARGE
	'Public Sub SearchSpecialPowerCommand()
	Private Sub SearchSpecialPowerCommand()
		' MOD END MARGE
		Dim j, i, ret As Short
		Dim list() As String
		Dim list2() As String
		Dim flist() As Boolean
		Dim pid() As String
		Dim buf As String
		Dim p As Pilot
		Dim id_list() As String
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_str As String
		Dim found As Boolean
		
		LockGUI()
		
		'イベント専用のコマンドを除いた全スペシャルパワーのリストを作成
		ReDim list(0)
		For i = 1 To SPDList.Count
			With SPDList.Item(i)
				If .ShortName <> "非表示" Then
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve strkey_list(UBound(list))
					list(UBound(list)) = .Name
					strkey_list(UBound(list)) = .KanaName
				End If
			End With
		Next 
		
		'ソート
		For i = 1 To UBound(strkey_list) - 1
			max_item = i
			max_str = strkey_list(i)
			For j = i + 1 To UBound(strkey_list)
				If StrComp(strkey_list(j), max_str, 1) = -1 Then
					max_item = j
					max_str = strkey_list(j)
				End If
			Next 
			If max_item <> i Then
				buf = list(i)
				list(i) = list(max_item)
				list(max_item) = buf
				
				buf = strkey_list(i)
				strkey_list(i) = max_str
				strkey_list(max_item) = buf
			End If
		Next 
		
		'個々のスペシャルパワーに対して、そのスペシャルパワーを使用可能なパイロットが
		'いるかどうか判定
		ReDim flist(UBound(list))
		For i = 1 To UBound(list)
			flist(i) = True
			For	Each p In PList
				With p
					If .Party = "味方" Then
						If Not .Unit_Renamed Is Nothing Then
							If .Unit_Renamed.Status_Renamed = "出撃" And Not .Unit_Renamed.IsConditionSatisfied("憑依") Then
								'本当に乗っている？
								found = False
								With .Unit_Renamed
									If p Is .MainPilot Then
										found = True
									Else
										For j = 2 To .CountPilot
											If p Is .Pilot(j) Then
												found = True
												Exit For
											End If
										Next 
										For j = 1 To .CountSupport
											If p Is .Support(j) Then
												found = True
												Exit For
											End If
										Next 
										If p Is .AdditionalSupport Then
											found = True
										End If
									End If
								End With
								
								If found Then
									If .IsSpecialPowerAvailable(list(i)) Then
										flist(i) = False
										Exit For
									End If
								End If
							End If
						End If
					End If
				End With
			Next p
		Next 
		
		Do While True
			ReDim ListItemFlag(UBound(list))
			ReDim ListItemComment(UBound(list))
			ReDim id_list(UBound(list))
			ReDim strkey_list(UBound(list))
			
			'選択出来ないスペシャルパワーをマスク
			For i = 1 To UBound(ListItemFlag)
				ListItemFlag(i) = flist(i)
			Next 
			
			'スペシャルパワーの解説を設定
			For i = 1 To UBound(ListItemComment)
				ListItemComment(i) = SPDList.Item(list(i)).Comment
			Next 
			
			'検索するスペシャルパワーを選択
			TopItem = 1
			ret = MultiColumnListBox(Term("スペシャルパワー") & "検索", list, True)
			If ret = 0 Then
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			SelectedSpecialPower = list(ret)
			
			'選択されたスペシャルパワーを使用できるパイロットの一覧を作成
			ReDim list2(0)
			ReDim ListItemFlag(0)
			ReDim id_list(0)
			ReDim pid(0)
			For	Each p In PList
				With p
					'選択したスペシャルパワーを使用できるパイロットかどうか判定
					If .Party <> "味方" Then
						GoTo NextLoop
					End If
					If .Unit_Renamed Is Nothing Then
						GoTo NextLoop
					End If
					If .Unit_Renamed.Status_Renamed <> "出撃" Then
						GoTo NextLoop
					End If
					If .Unit_Renamed.CountPilot > 0 Then
						If .ID = .Unit_Renamed.Pilot(1).ID And .ID <> .Unit_Renamed.MainPilot.ID Then
							'追加パイロットのため、使用されていない
							GoTo NextLoop
						End If
					End If
					If Not .IsSpecialPowerAvailable(SelectedSpecialPower) Then
						GoTo NextLoop
					End If
					
					'パイロットをリストに追加
					ReDim Preserve list2(UBound(list2) + 1)
					ReDim Preserve ListItemFlag(UBound(list2))
					ReDim Preserve id_list(UBound(list2))
					ReDim Preserve pid(UBound(list2))
					ListItemFlag(UBound(list2)) = False
					id_list(UBound(list2)) = .Unit_Renamed.ID
					pid(UBound(list2)) = .ID
					list2(UBound(list2)) = RightPaddedString(.Nickname, 19) & RightPaddedString(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP), 10)
					
					buf = ""
					For j = 1 To .CountSpecialPower
						buf = buf & SPDList.Item(.SpecialPower(j)).ShortName
					Next 
					list2(UBound(list2)) = list2(UBound(list2)) & RightPaddedString(buf, 12)
					
					If .SP < .SpecialPowerCost(SelectedSpecialPower) Then
						list2(UBound(list2)) = list2(UBound(list2)) & " " & Term("ＳＰ", p.Unit_Renamed) & "不足"
					End If
					If .Unit_Renamed.Action = 0 Then
						list2(UBound(list2)) = list2(UBound(list2)) & " 行動済"
					End If
				End With
NextLoop: 
			Next p
			
			SelectedSpecialPower = ""
			
			'検索をかけるパイロットの選択
			TopItem = 1
			EnlargeListBoxHeight()
			If IsOptionDefined("等身大基準") Then
				ret = ListBox("ユニット選択", list2, "ユニット           " & Term("SP", Nothing, 2) & "/Max" & Term("SP", Nothing, 2) & "  " & Term("スペシャルパワー"))
			Else
				ret = ListBox("パイロット選択", list2, "パイロット         " & Term("SP", Nothing, 2) & "/Max" & Term("SP", Nothing, 2) & "  " & Term("スペシャルパワー"))
			End If
			ReduceListBoxHeight()
			
			'パイロットの乗るユニットを画面中央に表示
			If ret > 0 Then
				With PList.Item(pid(ret))
					Center(.Unit_Renamed.X, .Unit_Renamed.Y)
					RefreshScreen()
					DisplayUnitStatus(.Unit_Renamed)
					
					'カーソル自動移動
					If AutoMoveCursor Then
						MoveCursorPos("ユニット選択", .Unit_Renamed)
					End If
				End With
				
				ReDim id_list(0)
				
				UnlockGUI()
				Exit Sub
			End If
		Loop 
	End Sub
	
	'リスタートコマンド
	' MOD START MARGE
	'Public Sub RestartCommand()
	Private Sub RestartCommand()
		' MOD END MARGE
		Dim ret As Short
		
		'リスタートを行うか確認
		ret = MsgBox("リスタートしますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "リスタート")
		If ret = MsgBoxResult.Cancel Then
			Exit Sub
		End If
		
		LockGUI()
		
		RestoreData(ScenarioPath & "_リスタート.src", True)
		
		UnlockGUI()
	End Sub
	
	'クイックロードコマンド
	' MOD START MARGE
	'Public Sub QuickLoadCommand()
	Private Sub QuickLoadCommand()
		' MOD END MARGE
		Dim ret As Short
		
		'ロードを行うか確認
		ret = MsgBox("データをロードしますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "クイックロード")
		If ret = MsgBoxResult.Cancel Then
			Exit Sub
		End If
		
		LockGUI()
		
		RestoreData(ScenarioPath & "_クイックセーブ.src", True)
		
		'画面を書き直してステータスを表示
		RedrawScreen()
		DisplayGlobalStatus()
		
		UnlockGUI()
	End Sub
	
	'クイックセーブコマンド
	' MOD START MARGE
	'Public Sub QuickSaveCommand()
	Private Sub QuickSaveCommand()
		' MOD END MARGE
		
		LockGUI()
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'中断データをセーブ
		DumpData(ScenarioPath & "_クイックセーブ.src")
		
		UnlockGUI()
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
	End Sub
	
	
	'プレイを中断し、中断用データをセーブする
	' MOD START MARGE
	'Public Sub DumpCommand()
	Private Sub DumpCommand()
		' MOD END MARGE
		Dim fname, save_path As String
		Dim ret, i As Short
		
		'プレイを中断するか確認
		ret = MsgBox("プレイを中断しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "中断")
		If ret = MsgBoxResult.Cancel Then
			Exit Sub
		End If
		
		'中断データをセーブするファイル名を決定
		For i = 1 To Len(ScenarioFileName)
			If Mid(ScenarioFileName, Len(ScenarioFileName) - i + 1, 1) = "\" Then
				Exit For
			End If
		Next 
		fname = Mid(ScenarioFileName, Len(ScenarioFileName) - i + 2, i - 5)
		fname = fname & "を中断.src"
		
		fname = SaveFileDialog("データセーブ", ScenarioPath, fname, 2, "ｾｰﾌﾞﾃﾞｰﾀ", "src")
		
		If fname = "" Then
			'キャンセル
			Exit Sub
		End If
		
		'セーブ先はシナリオフォルダ？
		If InStr(fname, "\") > 0 Then
			save_path = Left(fname, InStr2(fname, "\"))
		End If
		'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
		If Dir(save_path) <> Dir(ScenarioPath) Then
			If MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" & vbCr & vbLf & "このままセーブしますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question) <> 1 Then
				Exit Sub
			End If
		End If
		
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor 'マウスカーソルを砂時計に
		
		LockGUI()
		
		'中断データをセーブ
		DumpData(fname)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		MainForm.Hide()
		
		'ゲームを終了
		ExitGame()
	End Sub
	
	
	'全体マップの表示
	' MOD START MARGE
	'Public Sub GlobalMapCommand()
	Private Sub GlobalMapCommand()
		' MOD END MARGE
		Dim pic As System.Windows.Forms.PictureBox
		Dim xx, yy As Short
		Dim num, num2 As Short
		Dim mwidth, mheight As Integer
		Dim ret, smode As Integer
		Dim u As Unit
		Dim i, j As Short
		Dim prev_mode As Boolean
		
		LockGUI()
		
		With MainForm
			'見やすいように背景を設定
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0).BackColor = &HC0C0C0
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0) = System.Drawing.Image.FromFile("")
			
			'マップの縦横の比率を元に縮小マップの大きさを決める
			If MapWidth > MapHeight Then
				mwidth = 300
				mheight = 300 * MapHeight \ MapWidth
			Else
				mheight = 300
				mwidth = 300 * MapWidth \ MapHeight
			End If
			
			'マップの全体画像を作成
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picTmp
			With pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(MapPWidth)
				.Height = VB6.TwipsToPixelsY(MapPHeight)
			End With
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = GUI.BitBlt(pic.hDC, 0, 0, MapPWidth, MapPHeight, .picBack.hDC, 0, 0, SRCCOPY)
			For i = 1 To MapWidth
				xx = 32 * (i - 1)
				For j = 1 To MapHeight
					yy = 32 * (j - 1)
					u = MapDataForUnit(i, j)
					If Not u Is Nothing Then
						If u.BitmapID > 0 Then
							If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
								'ユニット
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
							Else
								'行動済のユニット
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
							End If
							
							'ユニットのいる場所に合わせて表示を変更
							Select Case u.Area
								Case "空中"
									'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									pic.Line (xx, yy + 28) - (xx + 31, yy + 28), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
								Case "水中"
									'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									pic.Line (xx, yy + 3) - (xx + 31, yy + 3), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
								Case "地中"
									'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									pic.Line (xx, yy + 28) - (xx + 31, yy + 28), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
									'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									pic.Line (xx, yy + 3) - (xx + 31, yy + 3), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
								Case "宇宙"
									If TerrainClass(i, j) = "月面" Then
										'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										pic.Line (xx, yy + 28) - (xx + 31, yy + 28), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
									End If
							End Select
						End If
					End If
				Next 
			Next 
			
			'マップ全体を縮小して描画
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			smode = GetStretchBltMode(.picMain(0).hDC)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = SetStretchBltMode(.picMain(0).hDC, STRETCH_DELETESCANS)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = StretchBlt(.picMain(0).hDC, (MainPWidth - mwidth) \ 2, (MainPHeight - mheight) \ 2, mwidth, mheight, pic.hDC, 0, 0, MapPWidth, MapPHeight, SRCCOPY)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = SetStretchBltMode(.picMain(0).hDC, smode)
			
			'マップ全体画像を破棄
			With pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(32)
				.Height = VB6.TwipsToPixelsY(32)
			End With
			
			'画面を更新
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0).Refresh()
		End With
		
		'味方ユニット数、敵ユニット数のカウント
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
					If .Party0 = "味方" Or .Party0 = "ＮＰＣ" Then
						num = num + 1
					Else
						num2 = num2 + 1
					End If
				End If
			End With
		Next u
		
		'各ユニット数の表示
		prev_mode = AutoMessageMode
		AutoMessageMode = False
		
		OpenMessageForm()
		DisplayMessage("システム", "味方ユニット： " & VB6.Format(num) & ";" & "敵ユニット  ： " & VB6.Format(num2))
		CloseMessageForm()
		
		AutoMessageMode = prev_mode
		
		'画面を元に戻す
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).BackColor = &HFFFFFF
		RefreshScreen()
		
		UnlockGUI()
	End Sub
	
	
	'現在の選択状況を記録
	Public Sub SaveSelections()
		'スタックのインデックスを増やす
		SelectionStackIndex = SelectionStackIndex + 1
		
		'スタック領域確保
		ReDim Preserve SavedSelectedUnit(SelectionStackIndex)
		ReDim Preserve SavedSelectedTarget(SelectionStackIndex)
		ReDim Preserve SavedSelectedUnitForEvent(SelectionStackIndex)
		ReDim Preserve SavedSelectedTargetForEvent(SelectionStackIndex)
		ReDim Preserve SavedSelectedWeapon(SelectionStackIndex)
		ReDim Preserve SavedSelectedWeaponName(SelectionStackIndex)
		ReDim Preserve SavedSelectedTWeapon(SelectionStackIndex)
		ReDim Preserve SavedSelectedTWeaponName(SelectionStackIndex)
		ReDim Preserve SavedSelectedDefenseOption(SelectionStackIndex)
		ReDim Preserve SavedSelectedAbility(SelectionStackIndex)
		ReDim Preserve SavedSelectedAbilityName(SelectionStackIndex)
		ReDim Preserve SavedSelectedX(SelectionStackIndex)
		ReDim Preserve SavedSelectedY(SelectionStackIndex)
		
		'確保した領域に選択状況を記録
		SavedSelectedUnit(SelectionStackIndex) = SelectedUnit
		SavedSelectedTarget(SelectionStackIndex) = SelectedTarget
		SavedSelectedUnitForEvent(SelectionStackIndex) = SelectedUnitForEvent
		SavedSelectedTargetForEvent(SelectionStackIndex) = SelectedTargetForEvent
		SavedSelectedWeapon(SelectionStackIndex) = SelectedWeapon
		SavedSelectedWeaponName(SelectionStackIndex) = SelectedWeaponName
		SavedSelectedTWeapon(SelectionStackIndex) = SelectedTWeapon
		SavedSelectedTWeaponName(SelectionStackIndex) = SelectedTWeaponName
		SavedSelectedDefenseOption(SelectionStackIndex) = SelectedDefenseOption
		SavedSelectedAbility(SelectionStackIndex) = SelectedAbility
		SavedSelectedAbilityName(SelectionStackIndex) = SelectedAbilityName
		SavedSelectedX(SelectionStackIndex) = SelectedX
		SavedSelectedY(SelectionStackIndex) = SelectedY
	End Sub
	
	'選択状況を復元
	Public Sub RestoreSelections()
		'スタックに積まれていない？
		If SelectionStackIndex = 0 Then
			Exit Sub
		End If
		
		'スタックトップから記録された選択状況を取り出す
		If Not SavedSelectedUnit(SelectionStackIndex) Is Nothing Then
			SelectedUnit = SavedSelectedUnit(SelectionStackIndex).CurrentForm
		Else
			'UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedUnit = Nothing
		End If
		If Not SavedSelectedTarget(SelectionStackIndex) Is Nothing Then
			SelectedTarget = SavedSelectedTarget(SelectionStackIndex).CurrentForm
		Else
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
		End If
		If Not SavedSelectedUnitForEvent(SelectionStackIndex) Is Nothing Then
			SelectedUnitForEvent = SavedSelectedUnitForEvent(SelectionStackIndex).CurrentForm
		Else
			'UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedUnitForEvent = Nothing
		End If
		If Not SavedSelectedTargetForEvent(SelectionStackIndex) Is Nothing Then
			SelectedTargetForEvent = SavedSelectedTargetForEvent(SelectionStackIndex).CurrentForm
		Else
			'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTargetForEvent = Nothing
		End If
		SelectedWeapon = SavedSelectedWeapon(SelectionStackIndex)
		SelectedWeaponName = SavedSelectedWeaponName(SelectionStackIndex)
		SelectedTWeapon = SavedSelectedTWeapon(SelectionStackIndex)
		SelectedTWeaponName = SavedSelectedTWeaponName(SelectionStackIndex)
		SelectedDefenseOption = SavedSelectedDefenseOption(SelectionStackIndex)
		SelectedAbility = SavedSelectedAbility(SelectionStackIndex)
		SelectedAbilityName = SavedSelectedAbilityName(SelectionStackIndex)
		SelectedX = SavedSelectedX(SelectionStackIndex)
		SelectedY = SavedSelectedY(SelectionStackIndex)
		
		'スタックのインデックスを１減らす
		SelectionStackIndex = SelectionStackIndex - 1
		
		'スタックの領域を開放
		ReDim Preserve SavedSelectedUnit(SelectionStackIndex)
		ReDim Preserve SavedSelectedTarget(SelectionStackIndex)
		ReDim Preserve SavedSelectedUnitForEvent(SelectionStackIndex)
		ReDim Preserve SavedSelectedTargetForEvent(SelectionStackIndex)
		ReDim Preserve SavedSelectedWeapon(SelectionStackIndex)
		ReDim Preserve SavedSelectedWeaponName(SelectionStackIndex)
		ReDim Preserve SavedSelectedTWeapon(SelectionStackIndex)
		ReDim Preserve SavedSelectedTWeaponName(SelectionStackIndex)
		ReDim Preserve SavedSelectedDefenseOption(SelectionStackIndex)
		ReDim Preserve SavedSelectedAbility(SelectionStackIndex)
		ReDim Preserve SavedSelectedAbilityName(SelectionStackIndex)
		ReDim Preserve SavedSelectedX(SelectionStackIndex)
		ReDim Preserve SavedSelectedY(SelectionStackIndex)
	End Sub
	
	'選択を入れ替える
	Public Sub SwapSelections()
		Dim u, t As Unit
		Dim w, tw As Short
		Dim wname, twname As String
		
		u = SelectedUnit
		t = SelectedTarget
		SelectedUnit = t
		SelectedTarget = u
		
		w = SelectedWeapon
		tw = SelectedTWeapon
		SelectedWeapon = tw
		SelectedTWeapon = w
		
		wname = SelectedWeaponName
		twname = SelectedTWeaponName
		SelectedWeaponName = twname
		SelectedTWeaponName = wname
	End Sub
End Module