Option Strict Off
Option Explicit On
Module Commands
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public CommandState As String
	
	'Invalid_string_refer_to_original_code
	Public WaitClickMode As Boolean
	'Invalid_string_refer_to_original_code
	Public ViewMode As Boolean
	
	'Invalid_string_refer_to_original_code
	Private MapCommandLabelList(10) As String
	'Invalid_string_refer_to_original_code
	Private UnitCommandLabelList(10) As String
	
	'Invalid_string_refer_to_original_code
	Public SelectedUnit As Unit 'Invalid_string_refer_to_original_code
	Public SelectedCommand As String 'Invalid_string_refer_to_original_code
	Public SelectedTarget As Unit 'Invalid_string_refer_to_original_code
	Public SelectedX As Short 'Invalid_string_refer_to_original_code
	Public SelectedY As Short 'Invalid_string_refer_to_original_code
	Public SelectedWeapon As Short '豁ｦ蝎ｨ
	Public SelectedWeaponName As String
	Public SelectedTWeapon As Short '蜿肴茶豁ｦ蝎ｨ
	Public SelectedTWeaponName As String
	Public SelectedDefenseOption As String 'Invalid_string_refer_to_original_code
	Public SelectedAbility As Short 'Invalid_string_refer_to_original_code
	Public SelectedAbilityName As String
	Public SelectedPilot As Pilot 'Invalid_string_refer_to_original_code
	Public SelectedItem As Short 'Invalid_string_refer_to_original_code
	Public SelectedSpecialPower As String '繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ
	Public SelectedPartners() As Unit 'Invalid_string_refer_to_original_code
	' ADD START MARGE
	Public SelectedUnitMoveCost As Short 'Invalid_string_refer_to_original_code
	' ADD END MARGE
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public UseSupportAttack As Boolean
	Public UseSupportGuard As Boolean
	
	'Invalid_string_refer_to_original_code
	Private WithDoubleSPConsumption As Boolean
	
	'Invalid_string_refer_to_original_code
	Public AttackUnit As Unit
	'Invalid_string_refer_to_original_code
	Public SupportAttackUnit As Unit
	'Invalid_string_refer_to_original_code
	Public SupportGuardUnit As Unit
	'Invalid_string_refer_to_original_code
	Public SupportGuardUnitHPRatio As Double
	'Invalid_string_refer_to_original_code
	Public SupportGuardUnit2 As Unit
	'Invalid_string_refer_to_original_code
	Public SupportGuardUnitHPRatio2 As Double
	
	'Invalid_string_refer_to_original_code
	Private PrevUnitX As Short
	Private PrevUnitY As Short
	Private PrevUnitArea As String
	Private PrevUnitEN As Short
	Private PrevCommand As String
	
	'Invalid_string_refer_to_original_code
	Public MovedUnit As Unit
	Public MovedUnitSpeed As Short
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub ProceedCommand(Optional ByVal by_cancel As Boolean = False)
		Dim j, i, n As Short
		Dim u As Unit
		Dim uname As String
		Dim p As Pilot
		Dim buf As String
		Dim lab As LabelData
		
		'Invalid_string_refer_to_original_code
		If ViewMode Then
			If by_cancel Then
				ViewMode = False
			End If
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		IsGUILocked = True
		
		'Invalid_string_refer_to_original_code
		IsScenarioFinished = False
		IsCanceled = False
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		Call GetAsyncKeyState(RButtonID)
		
		Select Case CommandState
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
						'Invalid_string_refer_to_original_code
						
						DisplayGlobalStatus()
						
						'Invalid_string_refer_to_original_code
						If ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "Invalid_string_refer_to_original_code"
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "Invalid_string_refer_to_original_code"
						End If
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(EndTurnCmdID).Visible = True
						
						'荳ｭ譁ｭ
						If IsOptionDefined("Invalid_string_refer_to_original_code") Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(DumpCmdID).Visible = True
						Else
							If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(DumpCmdID).Visible = True
							Else
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(DumpCmdID).Visible = False
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(GlobalMapCmdID).Visible = True
						
						'Invalid_string_refer_to_original_code
						If IsEventDefined("蜍晏茜譚｡莉ｶ") Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = True
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = False
						End If
						
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(AutoDefenseCmdID).Visible = True
						
						'險ｭ螳壼､画峩
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuMapCommandItem(ConfigurationCmdID).Visible = True
						
						'Invalid_string_refer_to_original_code
						If IsRestartSaveDataAvailable And Not ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(RestartCmdID).Visible = True
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(RestartCmdID).Visible = False
						End If
						
						'Invalid_string_refer_to_original_code
						If IsQuickSaveDataAvailable And Not ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = True
						Else
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = False
						End If
						
						'Invalid_string_refer_to_original_code
						If ViewMode Then
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = False
						ElseIf IsOptionDefined("Invalid_string_refer_to_original_code") Then 
							'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = True
						Else
							If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = True
							Else
								'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = False
							End If
						End If
					Else
						'Invalid_string_refer_to_original_code
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
					
					'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ讀懃ｴ｢
					'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = False
					'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Caption = Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ") & "讀懃ｴ｢"
					For	Each p In PList
						With p
							If .Party = "蜻ｳ譁ｹ" Then
								If .CountSpecialPower > 0 Then
									'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = True
									Exit For
								End If
							End If
						End With
					Next p
					
					'Invalid_string_refer_to_original_code
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
					
					CommandState = "Invalid_string_refer_to_original_code"
					
					IsGUILocked = False
					' ADD START 240a
					'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					With SelectedUnit
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code_
						'Or .IsConditionSatisfied("繝ｦ繝九ャ繝域ュ蝣ｱ髫阡ｽ") _
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						IsGUILocked = False
						Exit Sub
					End With
				End If
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End With
				IsGUILocked = False
				Exit Sub
				'End If
				
				CommandState = "Invalid_string_refer_to_original_code"
				ProceedCommand(by_cancel)
				
			Case "Invalid_string_refer_to_original_code"
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
				
				'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					'蜈医↓蛻､螳壹＠縺ｦ縺翫￥
					
					'Invalid_string_refer_to_original_code
					For i = 1 To .CountAllFeature
						If .AllFeatureName(i) <> "" Then
							Select Case .AllFeature(i)
								Case "Invalid_string_refer_to_original_code"
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
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							If InStr(.AllFeatureData(i), "髱櫁｡ｨ遉ｺ") = 0 Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
								Exit For
							End If
						ElseIf .AllFeature(i) = "豁ｦ蝎ｨ繧ｯ繝ｩ繧ｹ" Or .AllFeature(i) = "髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ" Then 
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
							Exit For
						End If
						'End If
					Next 
					With .MainPilot
						For i = 1 To .CountSkill
							If .SkillName0(i) <> "髱櫁｡ｨ遉ｺ" And .SkillName0(i) <> "" Then
								Select Case .Skill(i)
									Case "Invalid_string_refer_to_original_code"
										If Not IsOptionDefined("Invalid_string_refer_to_original_code") And Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
											'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
											MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
											Exit For
										End If
									Case "霑ｽ蜉繝ｬ繝吶Ν", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
									Case Else
										'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
										Exit For
								End Select
							End If
						Next 
					End With
					
					'Invalid_string_refer_to_original_code
					For i = 1 To .CountAbility
						'Invalid_string_refer_to_original_code_
						'Or .IsCombinationAbilityAvailable(i, True)) _
						'And Not .Ability(i).IsItem _
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AbilityListCmdID).Caption = Term("Invalid_string_refer_to_original_code", SelectedUnit) & "荳隕ｧ"
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = True
						Exit For
						'End If
					Next 
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'Or ViewMode _
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'And Not .IsConditionSatisfied("豺ｷ荵ｱ") _
					'Invalid_string_refer_to_original_code_
					'And Not .IsConditionSatisfied("證ｴ襍ｰ") _
					'And Not .IsConditionSatisfied("迢よ姶螢ｫ") _
					'And Not ViewMode _
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If Not .Summoner Is Nothing Then
						If .Summoner.Party = "蜻ｳ譁ｹ" Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "蜻ｽ莉､"
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
						End If
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'And Not .IsConditionSatisfied("豺ｷ荵ｱ") _
					'Invalid_string_refer_to_original_code_
					'And Not .IsConditionSatisfied("證ｴ襍ｰ") _
					'And Not .IsConditionSatisfied("迢よ姶螢ｫ") _
					'And Not ViewMode _
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If Not .Master Is Nothing Then
						If .Master.Party = "蜻ｳ譁ｹ" Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "蜻ｽ莉､"
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
						End If
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible Then
						UnitCommand(FeatureListCmdID)
					Else
						CommandState = "Invalid_string_refer_to_original_code"
					End If
					
					IsGUILocked = False
					Exit Sub
					'End If
					
					If MapFileName <> "" Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True
						
						For i = 1 To .CountWeapon
							If .IsWeaponAvailable(i, "") And Not .IsWeaponClassifiedAs(i, "Invalid_string_refer_to_original_code") Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "Invalid_string_refer_to_original_code"
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
							End If
						Next 
					End If
					
					'Invalid_string_refer_to_original_code
					If MapFileName = "" Then
						'Invalid_string_refer_to_original_code
						If .IsFeatureAvailable("螟牙ｽ｢") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(TransformCmdID).Caption = .FeatureName("螟牙ｽ｢")
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							If MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "" Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "螟牙ｽ｢"
							End If
							
							For i = 2 To LLength(.FeatureData("螟牙ｽ｢"))
								uname = LIndex(.FeatureData("螟牙ｽ｢"), i)
								If .OtherForm(uname).IsAvailable Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(TransformCmdID).Visible = True
									Exit For
								End If
							Next 
						End If
						
						'Invalid_string_refer_to_original_code
						If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(SplitCmdID).Caption = .FeatureName("Invalid_string_refer_to_original_code")
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							If MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "" Then
								'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "Invalid_string_refer_to_original_code"
							End If
							
							buf = .FeatureData("Invalid_string_refer_to_original_code")
							
							'Invalid_string_refer_to_original_code
							For i = 2 To LLength(buf)
								If Not UList.IsDefined(LIndex(buf, i)) Then
									'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
									Exit For
								End If
							Next 
							
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							If MainForm.mnuUnitCommandItem(SplitCmdID).Visible Then
								n = 0
								For i = 2 To LLength(buf)
									With UList.Item(LIndex(buf, i)).Data
										'Invalid_string_refer_to_original_code
										'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
										n = n + System.Math.Abs(.PilotNum)
									End With
								Next 
							End If
						End If
					End If
				End With
				'Next
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				'End If
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "Invalid_string_refer_to_original_code"
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
				'End If
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'螟芽ｺｫ隗｣髯､
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "Invalid_string_refer_to_original_code"
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Next
				
				'Invalid_string_refer_to_original_code
				With ALDList
					For i = 1 To .Count
						With .Item(i)
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.mnuUnitCommandItem(OrderCmdID).Caption = .Name
							Exit For
							'End If
						End With
					Next 
				End With
				'End If
				'End If
				
				'Invalid_string_refer_to_original_code
				If Not ViewMode Then
					i = UnitCommand1CmdID
					For	Each lab In colEventLabelList
						With lab
							If .Name = Event_Renamed.LabelType.UnitCommandEventLabel And .Enable Then
								buf = GetValueAsString(.Para(3))
								If (SelectedUnit.Party = "蜻ｳ譁ｹ" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "蜈ｨ" Then
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
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Or .IsConditionSatisfied("繝ｦ繝九ャ繝域ュ蝣ｱ髫阡ｽ") _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
					'Invalid_string_refer_to_original_code
					CommandState = "Invalid_string_refer_to_original_code"
					IsGUILocked = False
					Exit Sub
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
				'End If
				
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				i = UnitCommand1CmdID
				For	Each lab In colEventLabelList
					With lab
						If .Name = Event_Renamed.LabelType.UnitCommandEventLabel And (.AsterNum = 1 Or .AsterNum = 3) Then
							If .Enable Then
								buf = .Para(3)
								If (SelectedUnit.Party = "蜻ｳ譁ｹ" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "蜈ｨ" Then
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
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = "Invalid_string_refer_to_original_code"
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = True
				'End If
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
				'End If
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(JumpCmdID).Caption = "Invalid_string_refer_to_original_code"
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(JumpCmdID).Visible = True
				'End If
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
				'End If
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(JumpCmdID).Visible = True
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
				'End If
				'End If
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'Invalid_string_refer_to_original_code
				For i = 1 To 4
					'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					u = Nothing
					Select Case i
						Case 1
							'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
						Case 2
							'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
						Case 3
							'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
						Case 4
							'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
					End Select
					
					If Not u Is Nothing Then
						'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
					End If
				Next 
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "Invalid_string_refer_to_original_code"
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Next
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Next
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'Invalid_string_refer_to_original_code
				n = 0
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Term("Invalid_string_refer_to_original_code", SelectedUnit)
				If n = 1 Then
					'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Caption = Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ", SelectedUnit)
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = False
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				For i = 2 To LLength(buf)
					If Not UList.IsDefined(LIndex(buf, i)) Then
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
						Exit For
					End If
				Next 
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If MainForm.mnuUnitCommandItem(SplitCmdID).Visible Then
					n = 0
					For i = 2 To LLength(buf)
						With UList.Item(LIndex(buf, i)).Data
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							n = n + System.Math.Abs(.PilotNum)
						End With
					Next 
				End If
				'End With
				'Next
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				'End If
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'螟芽ｺｫ隗｣髯､
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'End If
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "Invalid_string_refer_to_original_code"
				'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuUnitCommandItem(GroundCmdID).Visible = True
				'End If
				'End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		End Select
		'End With
		'Next
		'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		i = UnitCommand1CmdID
		For	Each lab In colEventLabelList
			With lab
				If .Name = Event_Renamed.LabelType.UnitCommandEventLabel Then
					If .Enable Then
						buf = .Para(3)
						If (SelectedUnit.Party = "蜻ｳ譁ｹ" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "蜈ｨ" Then
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
		'End With
		
		If Not SelectedUnit Is DisplayedUnit Then
			'MOD START 240a
			'                DisplayUnitStatus SelectedUnit
			'Invalid_string_refer_to_original_code
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
		
		'UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'Case "Invalid_string_refer_to_original_code"
			'SelectedUnitForEvent = SelectedUnit
			'
			'With SelectedUnit
				'Invalid_string_refer_to_original_code
				' MOD START MARGE
				'                If MainWidth = 15 Then
				'If Not NewGUIMode Then
					' MOD END MARGE
					'If PrevUnitEN <> .EN Then
						'DisplayUnitStatus(SelectedUnit)
					'End If
				'End If
				'
				'With MainForm
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(WaitCmdID).Visible = True
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(MoveCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(TeleportCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(JumpCmdID).Visible = False
				'End With
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(TalkCmdID).Visible = False
				'For 'i = 1 To 4
					''UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					'u = Nothing
					'Select Case i
						'Case 1
							'If .X > 1 Then
								'u = MapDataForUnit(.X - 1, .Y)
							'End If
						'Case 2
							'If .X < MapWidth Then
								'u = MapDataForUnit(.X + 1, .Y)
							'End If
						'Case 3
							'If .Y > 1 Then
								'u = MapDataForUnit(.X, .Y - 1)
							'End If
						'Case 4
							'If .Y < MapHeight Then
								'u = MapDataForUnit(.X, .Y + 1)
							'End If
					'End Select
					'
					'If Not u Is Nothing Then
						'If IsEventDefined("莨夊ｩｱ " & .MainPilot.ID & " " & u.MainPilot.ID) Then
							''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'MainForm.mnuUnitCommandItem(TalkCmdID).Visible = True
							'Exit For
						'End If
					'End If
				'Next 
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "Invalid_string_refer_to_original_code"
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
				'For 'i = 1 To .CountWeapon
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
					'Exit For
					'End If
				'Next 
				'If .Area = "蝨ｰ荳ｭ" Then
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
				'End If
				'If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
				'End If
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
				'If .IsFeatureAvailable("Invalid_string_refer_to_original_code") And .Area <> "蝨ｰ荳ｭ" Then
					'For 'i = 1 To 4
						''UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						'u = Nothing
						'Select Case i
							'Case 1
								'If .X > 1 Then
									'u = MapDataForUnit(.X - 1, .Y)
								'End If
							'Case 2
								'If .X < MapWidth Then
									'u = MapDataForUnit(.X + 1, .Y)
								'End If
							'Case 3
								'If .Y > 1 Then
									'u = MapDataForUnit(.X, .Y - 1)
								'End If
							'Case 4
								'If .Y < MapHeight Then
									'u = MapDataForUnit(.X, .Y + 1)
								'End If
						'End Select
						'
						'If Not u Is Nothing Then
							'With u
								'If (.Party = "蜻ｳ譁ｹ" Or .Party = "Invalid_string_refer_to_original_code") And .HP < .MaxHP Then
									''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'MainForm.mnuUnitCommandItem(FixCmdID).Visible = True
									'Exit For
								'End If
							'End With
						'End If
					'Next 
					'
					'If Len(.FeatureData("Invalid_string_refer_to_original_code")) > 0 Then
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(FixCmdID).Caption = LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 1)
						'If IsNumeric(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)) Then
							'If .EN < CShort(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)) Then
								''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
							'End If
						'End If
					'Else
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(FixCmdID).Caption = "Invalid_string_refer_to_original_code"
					'End If
				'End If
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
				'If .IsFeatureAvailable("Invalid_string_refer_to_original_code") And .Area <> "蝨ｰ荳ｭ" Then
					'For 'i = 1 To 4
						''UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						'u = Nothing
						'Select Case i
							'Case 1
								'If .X > 1 Then
									'u = MapDataForUnit(.X - 1, .Y)
								'End If
							'Case 2
								'If .X < MapWidth Then
									'u = MapDataForUnit(.X + 1, .Y)
								'End If
							'Case 3
								'If .Y > 1 Then
									'u = MapDataForUnit(.X, .Y - 1)
								'End If
							'Case 4
								'If .Y < MapHeight Then
									'u = MapDataForUnit(.X, .Y + 1)
								'End If
						'End Select
						'
						'If Not u Is Nothing Then
							'With u
								'If .Party = "蜻ｳ譁ｹ" Or .Party = "Invalid_string_refer_to_original_code" Then
									'If .EN < .MaxEN Then
										''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										'MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
									'Else
										'For 'j = 1 To .CountWeapon
											'If .Bullet(j) < .MaxBullet(j) Then
												''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												'MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
												'Exit For
											'End If
										'Next 
										'For 'j = 1 To .CountAbility
											'If .Stock(j) < .MaxStock(j) Then
												''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
												'MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
												'Exit For
											'End If
										'Next 
									'End If
								'End If
							'End With
						'End If
					'Next 
					'
					'If Len(.FeatureData("Invalid_string_refer_to_original_code")) > 0 Then
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 1)
						'If IsNumeric(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)) Then
							'If .EN < CShort(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)) Or .MainPilot.Morale < 100 Then
								''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
							'End If
						'End If
					'Else
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "Invalid_string_refer_to_original_code"
					'End If
					'
					'If IsOptionDefined("遘ｻ蜍募ｾ瑚｣懃ｵｦ荳榊庄") And Not SelectedUnit.MainPilot.IsSkillAvailable("陬懃ｵｦ") Then
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
					'End If
				'End If
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
				'n = 0
				'For 'i = 1 To .CountAbility
					'If Not .Ability(i).IsItem Then
						'n = n + 1
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = True
					'End If
					'End If
				'Next 
				'If .Area = "蝨ｰ荳ｭ" Then
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
				'End If
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = Term("Invalid_string_refer_to_original_code", SelectedUnit)
				'If n = 1 Then
					'For 'i = 1 To .CountAbility
						'If Not .Ability(i).IsItem Then
							''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = .AbilityNickname(i)
							'Exit For
						'End If
					'Next 
				'End If
				'
				'With MainForm
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(ChargeCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(SpecialPowerCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(TransformCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(SplitCmdID).Visible = False
				'End With
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(CombineCmdID).Visible = False
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'For 'i = 1 To .CountFeature
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'And .FeatureName(i) <> "" _
					'And LLength(.FeatureData(i)) > 3 _
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'n = 0
					'For 'j = 3 To LLength(.FeatureData(i))
						'u = UList.Item(LIndex(.FeatureData(i), j))
						'If u Is Nothing Then
							'Exit For
						'End If
						'If Not u.IsOperational Then
							'Exit For
						'End If
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						'Exit For
						'End If
						'If System.Math.Abs(.X - u.CurrentForm.X) + System.Math.Abs(.Y - u.CurrentForm.Y) > 2 Then
							'Exit For
						'End If
						'n = n + 1
					'Next 
					'
					'uname = LIndex(.FeatureData(i), 2)
					'u = UList.Item(uname)
					'If u Is Nothing Then
						'n = 0
					'ElseIf u.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then 
						'n = 0
					'End If
					'
					'If n = LLength(.FeatureData(i)) - 2 Then
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
						''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'MainForm.mnuUnitCommandItem(CombineCmdID).Caption = LIndex(.FeatureData(i), 1)
						'Exit For
					'End If
					'End If
				'Next 
				'End If
				'
				'With MainForm
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(HyperModeCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(GroundCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(SkyCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UndergroundCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(WaterCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(LaunchCmdID).Visible = False
				'End With
				'
				'Invalid_string_refer_to_original_code
				''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
				'For 'i = 1 To .CountAbility
					'Invalid_string_refer_to_original_code_
					'And .Ability(i).IsItem _
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'MainForm.mnuUnitCommandItem(ItemCmdID).Visible = True
					'Exit For
					'End If
				'Next 
				'If .Area = "蝨ｰ荳ｭ" Then
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
				'End If
				'
				'With MainForm
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(DismissCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(OrderCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(FeatureListCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(WeaponListCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(AbilityListCmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand1CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand2CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand3CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand4CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand5CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand6CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand7CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand8CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand9CmdID).Visible = False
					''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'.mnuUnitCommandItem(UnitCommand10CmdID).Visible = False
				'End With
				'
				'Invalid_string_refer_to_original_code
				'i = UnitCommand1CmdID
				'For	Each lab In colEventLabelList
					'With lab
						'If .Name = Event_Renamed.LabelType.UnitCommandEventLabel And .AsterNum >= 2 Then
							'If .Enable Then
								'buf = .Para(3)
								'If (SelectedUnit.Party = "蜻ｳ譁ｹ" And (buf = SelectedUnit.MainPilot.Name Or buf = SelectedUnit.MainPilot.Nickname Or buf = SelectedUnit.Name)) Or buf = SelectedUnit.Party Or buf = "蜈ｨ" Then
									'If .CountPara <= 3 Then
										''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										'MainForm.mnuUnitCommandItem(i).Visible = True
									'ElseIf StrToLng(.Para(4)) <> 0 Then 
										''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
										'MainForm.mnuUnitCommandItem(i).Visible = True
									'End If
								'End If
							'End If
							'
							''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'If MainForm.mnuUnitCommandItem(i).Visible Then
								''UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
								'UnitCommandLabelList(i - UnitCommand1CmdID + 1) = CStr(.LineNum)
								'i = i + 1
								'If i > UnitCommand10CmdID Then
									'Exit For
								'End If
							'End If
						'End If
					'End With
				'Next lab
			'End With
			'
			'IsGUILocked = False
			'If by_cancel Then
				''UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				''UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5)
			'Else
				''UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				''UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6)
				'System.Windows.Forms.Application.DoEvents()
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				''UPGRADE_ISSUE: Control mnuUnitCommand は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				''UPGRADE_ISSUE: Form メソッド MainForm.PopupMenu はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'MainForm.PopupMenu(MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6)
				'System.Windows.Forms.Application.DoEvents()
				'Loop
			'End If
			'
			''UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
			'Case CInt("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'If Not MaskData(PixelToMapX(MouseX), PixelToMapY(MouseY)) Then
					'SelectedX = PixelToMapX(MouseX)
					'SelectedY = PixelToMapY(MouseY)
					'
					'Invalid_string_refer_to_original_code
					'If SelectedUnit.X = SelectedX And SelectedUnit.Y = SelectedY Then
						'If SelectedCommand = "繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ" Then
							'Invalid_string_refer_to_original_code
						'ElseIf SelectedCommand = "Invalid_string_refer_to_original_code" Or SelectedCommand = "Invalid_string_refer_to_original_code" Or SelectedCommand = "Invalid_string_refer_to_original_code" Or SelectedCommand = "Invalid_string_refer_to_original_code" Then 
							'If SelectedUnit.AbilityMinRange(SelectedAbility) > 0 Then
								'Invalid_string_refer_to_original_code
								'IsGUILocked = False
								'Exit Sub
							'End If
						'ElseIf SelectedCommand = "遘ｻ蜍募多莉､" Then 
							'Invalid_string_refer_to_original_code
						'Else
							'Invalid_string_refer_to_original_code
							'IsGUILocked = False
							'Exit Sub
						'End If
					'End If
					'
					'Invalid_string_refer_to_original_code
					'Select Case SelectedCommand
						' MOD START MARGE
						'Invalid_string_refer_to_original_code
						'Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							' MOD END MARGE
							'FinishMoveCommand()
							'IsGUILocked = False
							'Exit Sub
						'Case "Invalid_string_refer_to_original_code"
							'FinishTeleportCommand()
							'IsGUILocked = False
							'Exit Sub
						'Case "Invalid_string_refer_to_original_code"
							'FinishJumpCommand()
							'IsGUILocked = False
							'Exit Sub
						'Case "Invalid_string_refer_to_original_code"
							'MapAttackCommand()
							'IsGUILocked = False
							'Exit Sub
						'Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
							'MapAbilityCommand()
							'IsGUILocked = False
							'Exit Sub
						'Case "逋ｺ騾ｲ"
							'FinishLaunchCommand()
							'IsGUILocked = False
							'Exit Sub
						'Case "遘ｻ蜍募多莉､"
							'FinishOrderCommand()
							'IsGUILocked = False
							'Exit Sub
					'End Select
					'
					'Invalid_string_refer_to_original_code
					'
					'Invalid_string_refer_to_original_code
					'If MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
						'IsGUILocked = False
						'Exit Sub
					'End If
					'
					'Invalid_string_refer_to_original_code
					'SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
					'
					'Select Case SelectedCommand
						'Case "Invalid_string_refer_to_original_code"
							'FinishAttackCommand()
						'Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
							'FinishAbilityCommand()
						'Case "莨夊ｩｱ"
							'FinishTalkCommand()
						'Case "Invalid_string_refer_to_original_code"
							'FinishFixCommand()
						'Case "陬懃ｵｦ"
							'FinishSupplyCommand()
						'Case "繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ"
							'FinishSpecialPowerCommand()
						'Case "Invalid_string_refer_to_original_code", "隴ｷ陦帛多莉､"
							'FinishOrderCommand()
					'End Select
				'End If
				'
				''UPGRADE_WARNING: ProceedCommand に変換されていないステートメントがあります。ソース コードを確認してください。
				'Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					'If 1 <= PixelToMapX(MouseX) And PixelToMapX(MouseX) <= MapWidth Then
						'If 1 <= PixelToMapY(MouseY) And PixelToMapY(MouseY) <= MapHeight Then
							'If Not MaskData(PixelToMapX(MouseX), PixelToMapY(MouseY)) Then
								'Invalid_string_refer_to_original_code
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'MapAttackCommand()
							'Else
								'MapAbilityCommand()
							'End If
						'End If
					'End If
					'End If
					'End Select
					'
					'IsGUILocked = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CancelCommand()
		Dim tmp_x, tmp_y As Short
		
		With SelectedUnit
			Select Case CommandState
				Case "Invalid_string_refer_to_original_code"
				Case "Invalid_string_refer_to_original_code"
					CommandState = "Invalid_string_refer_to_original_code"
					'ADD START
					'Invalid_string_refer_to_original_code
					SelectedCommand = ""
					' MOD START MARGE
					'                If MainWidth <> 15 Then
					If NewGUIMode Then
						' MOD  END  MARGE
						ClearUnitStatus()
					End If
					
				Case "Invalid_string_refer_to_original_code"
					' ADD START MARGE
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					WaitCommand()
					Exit Sub
					'End If
					' ADD END MARGE
					CommandState = "Invalid_string_refer_to_original_code"
					DisplayUnitStatus(SelectedUnit)
					RedrawScreen()
					ProceedCommand(True)
					
				Case "Invalid_string_refer_to_original_code"
					CommandState = "Invalid_string_refer_to_original_code"
					.Area = PrevUnitArea
					.Move(PrevUnitX, PrevUnitY, True, True)
					.EN = PrevUnitEN
					If Not SelectedUnit Is MapDataForUnit(PrevUnitX, PrevUnitY) Then
						'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					SelectedUnitMoveCost = 0
					' MOD END MARGE
					Select Case SelectedCommand
						Case "Invalid_string_refer_to_original_code"
							StartMoveCommand()
						Case "Invalid_string_refer_to_original_code"
							StartTeleportCommand()
						Case "Invalid_string_refer_to_original_code"
							StartJumpCommand()
						Case "逋ｺ騾ｲ"
							PaintUnitBitmap(SelectedTarget)
					End Select
					
				Case "Invalid_string_refer_to_original_code"
					CommandState = "Invalid_string_refer_to_original_code"
					DisplayUnitStatus(SelectedUnit)
					
					tmp_x = .X
					tmp_y = .Y
					.X = PrevUnitX
					.Y = PrevUnitY
					Select Case PrevCommand
						Case "Invalid_string_refer_to_original_code"
							AreaInSpeed(SelectedUnit)
						Case "Invalid_string_refer_to_original_code"
							AreaInTeleport(SelectedUnit)
						Case "Invalid_string_refer_to_original_code"
							AreaInSpeed(SelectedUnit, True)
						Case "逋ｺ騾ｲ"
							With SelectedTarget
								'Invalid_string_refer_to_original_code_
								'And (.Data.Speed = 0 _
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								AreaInTeleport(SelectedTarget)
								'Invalid_string_refer_to_original_code_
								'And (.Data.Speed = 0 _
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								AreaInSpeed(SelectedTarget, True)
								AreaInSpeed(SelectedTarget)
								'End If
							End With
					End Select
					.X = tmp_x
					.Y = tmp_y
					SelectedCommand = PrevCommand
					
					MaskData(tmp_x, tmp_y) = False
					MaskScreen()
					ProceedCommand(True)
					
				Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					If CommandState = "Invalid_string_refer_to_original_code" Then
						CommandState = "Invalid_string_refer_to_original_code"
					Else
						CommandState = "Invalid_string_refer_to_original_code"
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then
						AreaInCross(.X, .Y, .WeaponMaxRange(SelectedWeapon), (.Weapon(SelectedWeapon).MinRange))
					ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then 
						AreaInMoveAction(SelectedUnit, .WeaponMaxRange(SelectedWeapon))
					Else
						AreaInRange(.X, .Y, .WeaponMaxRange(SelectedWeapon), .Weapon(SelectedWeapon).MinRange, "縺吶∋縺ｦ")
					End If
					If .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then
						AreaInCross(.X, .Y, .AbilityMaxRange(SelectedAbility), .AbilityMinRange(SelectedAbility))
					ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then 
						AreaInMoveAction(SelectedUnit, .AbilityMaxRange(SelectedAbility))
					Else
						AreaInRange(.X, .Y, .AbilityMaxRange(SelectedAbility), .AbilityMinRange(SelectedAbility), "縺吶∋縺ｦ")
					End If
					'End If
					MaskScreen()
			End Select
		End With
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub UnitCommand(ByVal idx As Short)
		Dim prev_used_action As Short
		
		PrevCommand = SelectedCommand
		
		With SelectedUnit
			prev_used_action = .UsedAction
			Select Case idx
				Case MoveCmdID 'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					System.Windows.Forms.Application.DoEvents()
					Exit Sub
					'End If
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					StartMoveCommand()
					ShowAreaInSpeedCommand()
					'End If
					
				Case TeleportCmdID 'Invalid_string_refer_to_original_code
					StartTeleportCommand()
					
				Case JumpCmdID 'Invalid_string_refer_to_original_code
					StartJumpCommand()
					
				Case TalkCmdID '莨夊ｩｱ
					StartTalkCommand()
					
				Case AttackCmdID 'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					StartAttackCommand()
					ShowAreaInRangeCommand()
					'End If
					
				Case FixCmdID 'Invalid_string_refer_to_original_code
					StartFixCommand()
					
				Case SupplyCmdID '陬懃ｵｦ
					StartSupplyCommand()
					
				Case AbilityCmdID 'Invalid_string_refer_to_original_code
					StartAbilityCommand()
					
				Case ChargeCmdID '繝√Ε繝ｼ繧ｸ
					ChargeCommand()
					
				Case SpecialPowerCmdID 'Invalid_string_refer_to_original_code
					StartSpecialPowerCommand()
					
				Case TransformCmdID '螟牙ｽ｢
					TransformCommand()
					
				Case SplitCmdID 'Invalid_string_refer_to_original_code
					SplitCommand()
					
				Case CombineCmdID 'Invalid_string_refer_to_original_code
					CombineCommand()
					
				Case HyperModeCmdID 'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					CancelTransformationCommand()
					HyperModeCommand()
					'End If
					
				Case GroundCmdID 'Invalid_string_refer_to_original_code
					LockGUI()
					If TerrainClass(.X, .Y) = "豌ｴ" Or TerrainClass(.X, .Y) = "豺ｱ豌ｴ" Then
						.Area = "Invalid_string_refer_to_original_code"
					Else
						.Area = "Invalid_string_refer_to_original_code"
					End If
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					
				Case SkyCmdID '遨ｺ荳ｭ
					LockGUI()
					If TerrainClass(.X, .Y) = "譛磯擇" Then
						.Area = "Invalid_string_refer_to_original_code"
					Else
						.Area = "遨ｺ荳ｭ"
					End If
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					
				Case UndergroundCmdID '蝨ｰ荳ｭ
					LockGUI()
					.Area = "蝨ｰ荳ｭ"
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					
				Case WaterCmdID '豌ｴ荳ｭ
					LockGUI()
					.Area = "豌ｴ荳ｭ"
					.Update()
					If .IsMessageDefined(.Area) Then
						OpenMessageForm()
						.PilotMessage(.Area)
						CloseMessageForm()
					End If
					PaintUnitBitmap(SelectedUnit)
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					
				Case LaunchCmdID '逋ｺ騾ｲ
					StartLaunchCommand()
					
				Case ItemCmdID 'Invalid_string_refer_to_original_code
					StartAbilityCommand(True)
					
				Case DismissCmdID '蜿ｬ蝟夊ｧ｣髯､
					LockGUI()
					
					'Invalid_string_refer_to_original_code
					HandleEvent("菴ｿ逕ｨ", .MainPilot.ID, "蜿ｬ蝟夊ｧ｣髯､")
					If IsScenarioFinished Then
						IsScenarioFinished = False
						UnlockGUI()
						Exit Sub
					End If
					If IsCanceled Then
						IsCanceled = False
						Exit Sub
					End If
					
					'蜿ｬ蝟壹Θ繝九ャ繝医ｒ隗｣謾ｾ
					.DismissServant()
					
					'Invalid_string_refer_to_original_code
					HandleEvent("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If IsScenarioFinished Then
						IsScenarioFinished = False
					End If
					If IsCanceled Then
						IsCanceled = False
					End If
					
					UnlockGUI()
					
				Case OrderCmdID 'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If MainForm.mnuUnitCommandItem.Item(OrderCmdID).Caption = "蜻ｽ莉､" Then
						StartOrderCommand()
					Else
						ExchangeFormCommand()
					End If
					
				Case FeatureListCmdID 'Invalid_string_refer_to_original_code
					FeatureListCommand()
					
				Case WeaponListCmdID '豁ｦ蝎ｨ荳隕ｧ
					WeaponListCommand()
					
				Case AbilityListCmdID 'Invalid_string_refer_to_original_code
					AbilityListCommand()
					
				Case UnitCommand1CmdID To UnitCommand10CmdID 'Invalid_string_refer_to_original_code
					LockGUI()
					
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					HandleEvent("菴ｿ逕ｨ", .MainPilot.ID, MainForm.mnuUnitCommandItem.Item(idx).Caption)
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
					
					'Invalid_string_refer_to_original_code
					HandleEvent(UnitCommandLabelList(idx - UnitCommand1CmdID + 1))
					
					If IsCanceled Then
						IsCanceled = False
						CancelCommand()
						UnlockGUI()
						Exit Sub
					End If
					
					'Invalid_string_refer_to_original_code
					If .CurrentForm.CountPilot > 0 Then
						HandleEvent("Invalid_string_refer_to_original_code")
						'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						MainForm.mnuUnitCommandItem.Item(idx).Caption()
						If IsScenarioFinished Then
							IsScenarioFinished = False
							UnlockGUI()
							Exit Sub
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If .CurrentForm.CountPilot > 0 Then
						DisplayUnitStatus(.CurrentForm)
					End If
					
					'Invalid_string_refer_to_original_code
					If .CurrentForm.UsedAction <= prev_used_action Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						WaitCommand()
					Else
						CommandState = "Invalid_string_refer_to_original_code"
						UnlockGUI()
					End If
					'UPGRADE_WARNING: UnitCommand に変換されていないステートメントがあります。ソース コードを確認してください。
					IsCanceled = False
					WaitCommand(True)
					'End If
					
				Case WaitCmdID 'Invalid_string_refer_to_original_code
					WaitCommand()
					
				Case Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					System.Windows.Forms.Application.DoEvents()
					Exit Sub
					'End If
			End Select
		End With
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartMoveCommand()
	Private Sub StartMoveCommand()
		' MOD END MARGE
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInSpeed(SelectedUnit)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
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
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("豈崎襖") And Not .IsFeatureAvailable("豈崎襖") Then
					'Invalid_string_refer_to_original_code_
					'vbOKCancel + vbQuestion, "逹濶ｦ")
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Else
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			.Move(SelectedX, SelectedY)
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(.X, .Y) Is SelectedUnit Then
				If MapDataForUnit(.X, .Y).IsFeatureAvailable("豈崎襖") And Not .IsFeatureAvailable("豈崎襖") And .CountPilot > 0 Then
					'Invalid_string_refer_to_original_code
					If .IsMessageDefined("逹濶ｦ(" & .Name & ")") Then
						OpenMessageForm()
						.PilotMessage("逹濶ｦ(" & .Name & ")")
						CloseMessageForm()
					ElseIf .IsMessageDefined("逹濶ｦ") Then 
						OpenMessageForm()
						.PilotMessage("逹濶ｦ")
						CloseMessageForm()
					End If
					.SpecialEffect("逹濶ｦ", .Name)
					
					'Invalid_string_refer_to_original_code
					SelectedTarget = MapDataForUnit(.X, .Y)
					HandleEvent("Invalid_string_refer_to_original_code")
				Else
					'Invalid_string_refer_to_original_code
					SelectedUnit = MapDataForUnit(.X, .Y)
					
					'Invalid_string_refer_to_original_code
					HandleEvent("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				If IsScenarioFinished Then
					IsScenarioFinished = False
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ClearUnitStatus()
					RedrawScreen()
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					Exit Sub
				End If
				
				'谿九ｊ陦悟虚謨ｰ繧呈ｸ帛ｰ代＆縺帙ｋ
				SelectedUnit.UseAction()
				
				'Invalid_string_refer_to_original_code
				SelectedUnit.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
				
				DisplayUnitStatus(SelectedUnit)
				RedrawScreen()
				CommandState = "Invalid_string_refer_to_original_code"
				UnlockGUI()
				Exit Sub
			End If
			' ADD START MARGE
			If SelectedUnitMoveCost > 0 Then
				'Invalid_string_refer_to_original_code
				WaitCommand()
				Exit Sub
			End If
			
			SelectedUnitMoveCost = TotalMoveCost(.X, .Y)
			' ADD END MARGE
		End With
		
		CommandState = "Invalid_string_refer_to_original_code"
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartTeleportCommand()
	Private Sub StartTeleportCommand()
		' MOD END MARGE
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInTeleport(SelectedUnit)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
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
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("豈崎襖") Then
					'Invalid_string_refer_to_original_code_
					'vbOKCancel + vbQuestion, "逹濶ｦ")
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Else
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			OpenMessageForm()
			.PilotMessage("Invalid_string_refer_to_original_code" & .FeatureName("Invalid_string_refer_to_original_code"))
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CloseMessageForm()
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			OpenMessageForm()
			.PilotMessage("Invalid_string_refer_to_original_code")
			CloseMessageForm()
			'End If
			
			'繧｢繝九Γ陦ｨ遉ｺ
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.SpecialEffect("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'UPGRADE_WARNING: FinishTeleportCommand に変換されていないステートメントがあります。ソース コードを確認してください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			PlayWave("Whiz.wav")
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.EN = PrevUnitEN - 40
			'End If
			
			'Invalid_string_refer_to_original_code
			.Move(SelectedX, SelectedY, True, False, True)
			RedrawScreen()
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is SelectedUnit Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("豈崎襖") And Not .IsFeatureAvailable("豈崎襖") And .CountPilot > 0 Then
					'Invalid_string_refer_to_original_code
					If .IsMessageDefined("逹濶ｦ(" & .Name & ")") Then
						OpenMessageForm()
						.PilotMessage("逹濶ｦ(" & .Name & ")")
						CloseMessageForm()
					ElseIf .IsMessageDefined("逹濶ｦ") Then 
						OpenMessageForm()
						.PilotMessage("逹濶ｦ")
						CloseMessageForm()
					End If
					.SpecialEffect("逹濶ｦ", .Name)
					
					'Invalid_string_refer_to_original_code
					SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
					HandleEvent("Invalid_string_refer_to_original_code")
				Else
					'Invalid_string_refer_to_original_code
					SelectedUnit = MapDataForUnit(SelectedX, SelectedY)
					
					'Invalid_string_refer_to_original_code
					HandleEvent("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				If IsScenarioFinished Then
					IsScenarioFinished = False
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ClearUnitStatus()
					RedrawScreen()
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					Exit Sub
				End If
				
				'谿九ｊ陦悟虚謨ｰ繧呈ｸ帛ｰ代＆縺帙ｋ
				SelectedUnit.UseAction()
				
				'Invalid_string_refer_to_original_code
				SelectedUnit.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
				
				DisplayUnitStatus(MapDataForUnit(SelectedX, SelectedY))
				RedrawScreen()
				CommandState = "Invalid_string_refer_to_original_code"
				UnlockGUI()
				Exit Sub
			End If
			' ADD START MARGE
			SelectedUnitMoveCost = 100
			' ADD END MARGE
		End With
		
		CommandState = "Invalid_string_refer_to_original_code"
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartJumpCommand()
	Private Sub StartJumpCommand()
		' MOD END MARGE
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInSpeed(SelectedUnit, True)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
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
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("豈崎襖") Then
					'Invalid_string_refer_to_original_code_
					'vbOKCancel + vbQuestion, "逹濶ｦ")
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Else
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			OpenMessageForm()
			.PilotMessage("Invalid_string_refer_to_original_code" & .FeatureName("Invalid_string_refer_to_original_code"))
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CloseMessageForm()
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			OpenMessageForm()
			.PilotMessage("Invalid_string_refer_to_original_code")
			CloseMessageForm()
			'End If
			
			'繧｢繝九Γ陦ｨ遉ｺ
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.SpecialEffect("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			PlayWave("Swing.wav")
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			
			'Invalid_string_refer_to_original_code
			.Move(SelectedX, SelectedY, True, False, True)
			RedrawScreen()
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is SelectedUnit Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("豈崎襖") And Not .IsFeatureAvailable("豈崎襖") And .CountPilot > 0 Then
					'Invalid_string_refer_to_original_code
					If .IsMessageDefined("逹濶ｦ(" & .Name & ")") Then
						OpenMessageForm()
						.PilotMessage("逹濶ｦ(" & .Name & ")")
						CloseMessageForm()
					ElseIf .IsMessageDefined("逹濶ｦ") Then 
						OpenMessageForm()
						.PilotMessage("逹濶ｦ")
						CloseMessageForm()
					End If
					.SpecialEffect("逹濶ｦ", .Name)
					
					'Invalid_string_refer_to_original_code
					SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
					HandleEvent("Invalid_string_refer_to_original_code")
				Else
					'Invalid_string_refer_to_original_code
					SelectedUnit = MapDataForUnit(SelectedX, SelectedY)
					
					'Invalid_string_refer_to_original_code
					HandleEvent("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				If IsScenarioFinished Then
					IsScenarioFinished = False
					UnlockGUI()
					Exit Sub
				End If
				If IsCanceled Then
					IsCanceled = False
					ClearUnitStatus()
					RedrawScreen()
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					Exit Sub
				End If
				
				'谿九ｊ陦悟虚謨ｰ繧呈ｸ帛ｰ代＆縺帙ｋ
				SelectedUnit.UseAction()
				
				'Invalid_string_refer_to_original_code
				SelectedUnit.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
				
				DisplayUnitStatus(MapDataForUnit(SelectedX, SelectedY))
				RedrawScreen()
				CommandState = "Invalid_string_refer_to_original_code"
				UnlockGUI()
				Exit Sub
			End If
			' ADD START MARGE
			SelectedUnitMoveCost = 100
			' ADD END MARGE
		End With
		
		CommandState = "Invalid_string_refer_to_original_code"
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
			End If
			If Len(BGM) = 0 Then
				BGM = SearchMidiFile(.MainPilot.BGM)
			End If
			If Len(BGM) = 0 Then
				BGM = BGMName("default")
			End If
			
			'Invalid_string_refer_to_original_code
			UseSupportAttack = True
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			
			'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
			If SelectedWeapon = 0 Then
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "Invalid_string_refer_to_original_code" And LIndex(.FeatureData(i), 1) = .Weapon(SelectedWeapon).Name Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
						End If
						Exit For
					End If
				Next 
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then
				SelectedCommand = "Invalid_string_refer_to_original_code"
			Else
				SelectedCommand = "Invalid_string_refer_to_original_code"
			End If
			
			'Invalid_string_refer_to_original_code
			min_range = .Weapon(SelectedWeapon).MinRange
			max_range = .WeaponMaxRange(SelectedWeapon)
			
			'Invalid_string_refer_to_original_code
			If .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then
				AreaInCross(.X, .Y, min_range, max_range)
			ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then 
				AreaInWideCross(.X, .Y, min_range, max_range)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				AreaInRange(.X, .Y, max_range, min_range, "縺吶∋縺ｦ")
			ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then 
				AreaInMoveAction(SelectedUnit, max_range)
			Else
				AreaInRange(.X, .Y, max_range, min_range, "蜻ｳ譁ｹ縺ｮ謨ｵ")
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
						.CombinationPartner("Invalid_string_refer_to_original_code")
						If UBound(partners) = 0 Then
							MaskData(t.X, t.Y) = True
						End If
					End If
				End If
			Next 
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					If MaskData(i, j) Then
						GoTo NextLoop
					End If
					
					t = MapDataForUnit(i, j)
					
					If t Is Nothing Then
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If .WeaponAdaption(SelectedWeapon, (t.Area)) = 0 Then
						MaskData(i, j) = True
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If (.Weapon(SelectedWeapon).Power > 0 And .Damage(SelectedWeapon, t, True) = 0) Or .CriticalProbability(SelectedWeapon, t) = 0 Then
						MaskData(i, j) = True
						GoTo NextLoop
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If (.Weapon(SelectedWeapon).Power > 0 And .Damage(SelectedWeapon, t, True) = 0) Or (.Weapon(SelectedWeapon).Power = 0 And .CriticalProbability(SelectedWeapon, t) = 0) Then
						MaskData(i, j) = True
						GoTo NextLoop
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If .IsAlly(t) Then
						MaskData(i, j) = True
						GoTo NextLoop
					End If
					'End If
					
					'Invalid_string_refer_to_original_code
					If Not .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code") Then
						If Not .IsTargetWithinRange(SelectedWeapon, t) Then
							MaskData(i, j) = True
							GoTo NextLoop
						End If
					End If
NextLoop: 
				Next 
			Next 
			'End If
			MaskData(.X, .Y) = False
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				Center(.X, .Y)
			End If
			MaskScreen()
		End With
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		CommandState = "Invalid_string_refer_to_original_code"
		CommandState = "Invalid_string_refer_to_original_code"
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not AutoMoveCursor Then
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		t = Nothing
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		If t Is Nothing Then
			t = SelectedUnit
		End If
		
		'Invalid_string_refer_to_original_code
		MoveCursorPos("Invalid_string_refer_to_original_code")
		
		'Invalid_string_refer_to_original_code
		If Not SelectedUnit Is t Then
			DisplayUnitStatus(t)
		End If
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		'遘ｻ蜍募ｾ御ｽｿ逕ｨ蠕悟庄閭ｽ縺ｪ豁ｦ蝎ｨ縺玖ｨ倬鹸縺励※縺翫￥
		is_p_weapon = SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code")
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .WeaponMaxRange(SelectedWeapon) = 1 Then
				.CombinationPartner("Invalid_string_refer_to_original_code")
				SelectedTarget.X( , SelectedTarget.Y)
			Else
				.CombinationPartner("Invalid_string_refer_to_original_code")
			End If
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		UseSupportGuard = True
		SelectedTWeapon = SelectWeapon(SelectedTarget, SelectedUnit, "蜿肴茶")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		SelectedTWeapon = 0
		'End If
		If SelectedTWeapon > 0 Then
			twname = SelectedTarget.Weapon(SelectedTWeapon).Name
			SelectedTWeaponName = twname
		Else
			SelectedTWeaponName = ""
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		def_mode = SelectDefense(SelectedUnit, SelectedWeapon, SelectedTarget, SelectedTWeapon)
		If def_mode <> "" Then
			If SelectedTWeapon > 0 Then
				SelectedTWeapon = -1
			End If
		End If
		SelectedDefenseOption = def_mode
		
		'謌ｦ髣伜燕縺ｫ荳譌ｦ繧ｯ繝ｪ繧｢
		'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit2 = Nothing
		
		'Invalid_string_refer_to_original_code
		HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, wname)
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
		
		'Invalid_string_refer_to_original_code
		If SelectedTWeapon > 0 Then
			SaveSelections()
			SwapSelections()
			HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, twname)
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
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
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
		
		'Invalid_string_refer_to_original_code
		With SelectedTarget
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") And InStr(.MainPilot.Name, "(繧ｶ繧ｳ)") = 0 Then
				BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
				If Len(BGM) > 0 Then
					BossBGM = False
					ChangeBGM(BGM)
					BossBGM = True
				End If
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Len(BGM) = 0 And BossBGM Then
			BossBGM = False
			BGM = ""
			With SelectedUnit
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "Invalid_string_refer_to_original_code" And LIndex(.FeatureData(i), 1) = .Weapon(SelectedWeapon).Name Then
							BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
							Exit For
						End If
					Next 
				End If
				If Len(BGM) = 0 Then
					If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
						BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
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
			'Invalid_string_refer_to_original_code
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					MaskData(.X, .Y) = True
					'End If
				End With
			Next u
			
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		tx = SelectedTarget.X
		ty = SelectedTarget.Y
		
		OpenMessageForm(SelectedTarget, SelectedUnit)
		
		'Invalid_string_refer_to_original_code
		With SelectedTarget
			' MOD START MARGE
			'        If SelectedTWeapon > 0 And .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "遘ｻ蜍募燕") Then
			'Invalid_string_refer_to_original_code
			If .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "遘ｻ蜍募燕") Then
				' MOD END MARGE
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				def_mode = "Invalid_string_refer_to_original_code"
				
				If .IsWeaponClassifiedAs(SelectedTWeapon, "閾ｪ") Then
					is_suiside = True
				End If
				
				'Invalid_string_refer_to_original_code
				.Attack(SelectedTWeapon, SelectedUnit, "Invalid_string_refer_to_original_code")
				SelectedTarget = .CurrentForm
				'Invalid_string_refer_to_original_code_
				'Or .MainPilot.SkillLevel("蜈郁ｪｭ縺ｿ") >= Dice(16) _
				'Or .IsUnderSpecialPowerEffect("繧ｫ繧ｦ繝ｳ繧ｿ繝ｼ") _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				def_mode = "Invalid_string_refer_to_original_code"
				
				If .IsWeaponClassifiedAs(SelectedTWeapon, "閾ｪ") Then
					is_suiside = True
				End If
				
				'Invalid_string_refer_to_original_code
				.Attack(SelectedTWeapon, SelectedUnit, "繧ｫ繧ｦ繝ｳ繧ｿ繝ｼ", "")
				SelectedTarget = .CurrentForm
			ElseIf .MaxCounterAttack > .UsedCounterAttack Then 
				def_mode = "Invalid_string_refer_to_original_code"
				
				If .IsWeaponClassifiedAs(SelectedTWeapon, "閾ｪ") Then
					is_suiside = True
				End If
				
				'Invalid_string_refer_to_original_code
				.UsedCounterAttack = .UsedCounterAttack + 1
				
				'Invalid_string_refer_to_original_code
				.Attack(SelectedTWeapon, SelectedUnit, "繧ｫ繧ｦ繝ｳ繧ｿ繝ｼ", "")
				SelectedTarget = .CurrentForm
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If Not SupportGuardUnit2 Is Nothing Then
				attack_target = SupportGuardUnit2
				attack_target_hp_ratio = SupportGuardUnitHPRatio2
			End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code_
			'And UseSupportAttack _
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			SupportAttackUnit = .LookForSupportAttack(SelectedTarget)
			
			'Invalid_string_refer_to_original_code
			If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SupportAttackUnit = Nothing
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SupportAttackUnit = Nothing
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Master.Party = SelectedTarget.Party Then
				'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SupportAttackUnit = Nothing
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("雕翫ｊ") Then
				'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SupportAttackUnit = Nothing
			End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code_
			'And .MaxAction(True) > 0 _
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			If SelectedWeapon > .CountWeapon Then
				SelectedWeapon = -1
			ElseIf wname <> .Weapon(SelectedWeapon).Name Then 
				SelectedWeapon = -1
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				SelectedWeapon = -1
			End If
			If Not .IsWeaponAvailable(SelectedWeapon, "遘ｻ蜍募燕") Then
				SelectedWeapon = -1
			End If
			'End If
			If SelectedWeapon > 0 Then
				If Not .IsTargetWithinRange(SelectedWeapon, SelectedTarget) Then
					SelectedWeapon = 0
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			SelectedWeapon = -1
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Master.Party = SelectedTarget.Party0 Then
				SelectedWeapon = -1
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("雕翫ｊ") Then
				SelectedWeapon = -1
			End If
			
			If SelectedWeapon > 0 Then
				If Not SupportAttackUnit Is Nothing And .MaxSyncAttack > .UsedSyncAttack Then
					'Invalid_string_refer_to_original_code
					.Attack(SelectedWeapon, SelectedTarget, "Invalid_string_refer_to_original_code")
				Else
					'Invalid_string_refer_to_original_code
					.Attack(SelectedWeapon, SelectedTarget, "", def_mode)
				End If
			ElseIf SelectedWeapon = 0 Then 
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				.PlayAnimation("Invalid_string_refer_to_original_code")
			Else
				.SpecialEffect("Invalid_string_refer_to_original_code")
			End If
			.PilotMessage("Invalid_string_refer_to_original_code")
			'End If
			SelectedWeapon = -1
			'End If
			SelectedUnit = .CurrentForm
			
			'Invalid_string_refer_to_original_code
			If Not SupportGuardUnit Is Nothing Then
				defense_target2 = SupportGuardUnit
				defense_target2_hp_ratio = SupportGuardUnitHPRatio
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not SupportAttackUnit Is Nothing Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SupportAttackUnit = Nothing
		End If
		'End If
		If Not SupportAttackUnit Is Nothing Then
			If SelectedUnit.MaxSyncAttack > SelectedUnit.UsedSyncAttack Then
				With SupportAttackUnit
					'Invalid_string_refer_to_original_code
					w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "Invalid_string_refer_to_original_code")
					
					If w2 > 0 Then
						'Invalid_string_refer_to_original_code
						MaskData(.X, .Y) = False
						If Not BattleAnimation Then
							MaskScreen()
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						.PlayAnimation("Invalid_string_refer_to_original_code")
					End If
					UpdateMessageForm(SelectedTarget, SupportAttackUnit)
					.Attack(w2, SelectedTarget, "Invalid_string_refer_to_original_code")
				End With
			End If
			'End With
			
			'蠕悟ｧ区忰
			With SupportAttackUnit.CurrentForm
				If w2 > 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					.PlayAnimation("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				.UsedSupportAttack = .UsedSupportAttack + 1
				
				'Invalid_string_refer_to_original_code
				SelectedUnit.UsedSyncAttack = SelectedUnit.UsedSyncAttack + 1
			End With
		End If
		'End With
		
		support_attack_done = True
		
		'Invalid_string_refer_to_original_code
		'蜈･繧梧崛縺医※險倬鹸
		If Not SupportGuardUnit Is Nothing Then
			defense_target = SupportGuardUnit
			defense_target_hp_ratio = SupportGuardUnitHPRatio
		End If
		'End If
		'End If
		
		'蜿肴茶縺ｮ螳滓命
		With SelectedTarget
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'And .Party <> "蜻ｳ譁ｹ" _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			If SelectedTWeapon > 0 Then
				If SelectedTWeapon > .CountWeapon Then
					SelectedTWeapon = -1
				ElseIf twname <> .Weapon(SelectedTWeapon).Name Or Not .IsWeaponAvailable(SelectedTWeapon, "遘ｻ蜍募燕") Then 
					SelectedTWeapon = -1
				End If
			End If
			If SelectedTWeapon > 0 Then
				If Not .IsTargetWithinRange(SelectedTWeapon, SelectedUnit) Then
					'Invalid_string_refer_to_original_code
					SelectedTWeapon = SelectWeapon(SelectedTarget, SelectedUnit, "蜿肴茶")
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .MaxAction = 0 Then
				SelectedTWeapon = -1
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			SelectedTWeapon = -1
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Master.Party = SelectedUnit.Party Then
				SelectedWeapon = -1
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			If .IsConditionSatisfied("雕翫ｊ") Then
				SelectedTWeapon = -1
			End If
			
			If SelectedTWeapon > 0 And def_mode = "" Then
				'蜿肴茶繧貞ｮ滓命
				If .IsWeaponClassifiedAs(SelectedTWeapon, "閾ｪ") Then
					is_suiside = True
				End If
				.Attack(SelectedTWeapon, SelectedUnit, "", "")
				
				'Invalid_string_refer_to_original_code
				If Not SupportGuardUnit2 Is Nothing Then
					attack_target = SupportGuardUnit2
					attack_target_hp_ratio = SupportGuardUnitHPRatio2
				End If
			ElseIf SelectedTWeapon = 0 And .X = tx And .Y = ty Then 
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				.PlayAnimation("Invalid_string_refer_to_original_code")
			Else
				.SpecialEffect("Invalid_string_refer_to_original_code")
			End If
			.PilotMessage("Invalid_string_refer_to_original_code")
			SelectedTWeapon = -1
			'End If
			SelectedTWeapon = -1
			'End If
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		If Not SupportAttackUnit Is Nothing Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or support_attack_done _
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SupportAttackUnit = Nothing
		End If
		'End If
		If Not SupportAttackUnit Is Nothing Then
			With SupportAttackUnit
				'Invalid_string_refer_to_original_code
				w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "Invalid_string_refer_to_original_code")
				
				If w2 > 0 Then
					'Invalid_string_refer_to_original_code
					MaskData(.X, .Y) = False
					If Not BattleAnimation Then
						MaskScreen()
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					.PlayAnimation("Invalid_string_refer_to_original_code")
				End If
				UpdateMessageForm(SelectedTarget, SupportAttackUnit)
				.Attack(w2, SelectedTarget, "Invalid_string_refer_to_original_code")
			End With
		End If
		'End With
		
		'蠕悟ｧ区忰
		With SupportAttackUnit.CurrentForm
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'End If
			
			'Invalid_string_refer_to_original_code
			If w2 > 0 Then
				.UsedSupportAttack = .UsedSupportAttack + 1
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		'蜈･繧梧崛縺医※險倬鹸
		If Not SupportGuardUnit Is Nothing Then
			defense_target = SupportGuardUnit
			defense_target_hp_ratio = SupportGuardUnitHPRatio
		End If
		'End If
		
		SelectedTarget = SelectedTarget.CurrentForm
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Next 
			End If
			
			'Invalid_string_refer_to_original_code
			earnings = SelectedTarget.Value \ 2
			
			'Invalid_string_refer_to_original_code
			If .IsUnderSpecialPowerEffect("Invalid_string_refer_to_original_code") Then
				earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("Invalid_string_refer_to_original_code"))
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			IncrMoney(earnings)
			
			If earnings > 0 Then
				DisplaySysMessage(VB6.Format(earnings) & "縺ｮ" & Term("Invalid_string_refer_to_original_code", SelectedUnit) & "Invalid_string_refer_to_original_code")
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsUnderSpecialPowerEffect("謨ｵ遐ｴ螢頑凾蜀崎｡悟虚") Then
				.UsedAction = .UsedAction - 1
			End If
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Next 
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			If earnings > 0 Then
				.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			End If
			'End If
		End With
		
		With SelectedTarget
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
			'End If
		End With
		
		CloseMessageForm()
		ClearUnitStatus()
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		
		If SelectedWeapon <= 0 Then
			SelectedWeaponName = ""
		End If
		If SelectedTWeapon <= 0 Then
			SelectedTWeaponName = ""
		End If
		
		'Invalid_string_refer_to_original_code
		With attack_target.CurrentForm
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			HandleEvent("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			HandleEvent("Invalid_string_refer_to_original_code")
			'End If
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
		
		'Invalid_string_refer_to_original_code
		SaveSelections()
		SwapSelections()
		
		'Invalid_string_refer_to_original_code
		With defense_target.CurrentForm
			If .CountPilot > 0 Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				HandleEvent("Invalid_string_refer_to_original_code")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				HandleEvent("Invalid_string_refer_to_original_code")
			End If
			'End If
		End With
		
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If Not defense_target2 Is Nothing Then
			If Not defense_target2.CurrentForm Is defense_target.CurrentForm Then
				With defense_target2.CurrentForm
					If .CountPilot > 0 Then
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						HandleEvent("Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						HandleEvent("Invalid_string_refer_to_original_code")
					End If
				End With
			End If
			'End With
		End If
		'End If
		
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
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		HandleEvent("Invalid_string_refer_to_original_code")
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
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		SaveSelections()
		SwapSelections()
		HandleEvent("Invalid_string_refer_to_original_code")
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
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		HandleEvent("Invalid_string_refer_to_original_code")
		SelectedTarget.MainPilot.ID()
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
		'End If
		
		'Invalid_string_refer_to_original_code
		With SelectedTarget
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .X <> tx Or .Y <> ty Then
				HandleEvent("騾ｲ蜈･", .MainPilot.ID, .X, .Y)
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
			'End If
		End With
		
EndAttack: 
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		'Invalid_string_refer_to_original_code
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
		
		' ADD START MARGE
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If SelectedUnit.MainPilot.IsSkillAvailable("驕頑茶") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
			'Invalid_string_refer_to_original_code
			If SelectedUnitMoveCost > 0 Then
				HandleEvent("騾ｲ蜈･", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInSpeed(SelectedUnit)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		If NewGUIMode Then
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		Else
			DisplayUnitStatus(SelectedUnit)
		End If
		CommandState = "Invalid_string_refer_to_original_code"
		Exit Sub
		'End If
		'End If
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			'遘ｻ蜍募ｾ御ｽｿ逕ｨ蠕悟庄閭ｽ縺ｪ豁ｦ蝎ｨ縺玖ｨ倬鹸縺励※縺翫￥
			is_p_weapon = .IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code")
			' ADD END MARGE
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CommandState = "Invalid_string_refer_to_original_code"
			CommandState = "Invalid_string_refer_to_original_code"
			'End If
			
			'Invalid_string_refer_to_original_code
			SelectedX = PixelToMapX(MouseX)
			SelectedY = PixelToMapY(MouseY)
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'1, "蜻ｳ譁ｹ縺ｮ謨ｵ"
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'1, "縺吶∋縺ｦ"
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			MaskScreen()
			Exit Sub
			.IsWeaponClassifiedAs(SelectedWeapon, "Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			SelectedX = PixelToMapX(MouseX)
			SelectedY = PixelToMapY(MouseY)
			
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				MaskScreen()
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CommandState = "Invalid_string_refer_to_original_code"
			CommandState = "Invalid_string_refer_to_original_code"
			'End If
			
			'Invalid_string_refer_to_original_code
			AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
			MaskScreen()
			Exit Sub
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CommandState = "Invalid_string_refer_to_original_code"
			CommandState = "Invalid_string_refer_to_original_code"
			'End If
			
			'Invalid_string_refer_to_original_code
			SelectedX = PixelToMapX(MouseX)
			SelectedY = PixelToMapY(MouseY)
			
			'Invalid_string_refer_to_original_code
			AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
			MaskScreen()
			Exit Sub
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.CombinationPartner("Invalid_string_refer_to_original_code")
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
			
			If MainWidth <> 15 Then
				ClearUnitStatus()
			End If
			
			LockGUI()
			
			SelectedTWeapon = 0
			
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		' ADD START MARGE
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If SelectedUnit.MainPilot.IsSkillAvailable("驕頑茶") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
			'Invalid_string_refer_to_original_code
			If SelectedUnitMoveCost > 0 Then
				HandleEvent("騾ｲ蜈･", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInSpeed(SelectedUnit)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		If NewGUIMode Then
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		Else
			DisplayUnitStatus(SelectedUnit)
		End If
		CommandState = "Invalid_string_refer_to_original_code"
		Exit Sub
		'End If
		'End If
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	' MOD STAR MARGE
	'Public Sub StartAbilityCommand(Optional ByVal is_item As Boolean)
	Private Sub StartAbilityCommand(Optional ByVal is_item As Boolean = False)
		' MOD END MARGE
		Dim i, j As Short
		Dim t, u As Unit
		Dim min_range, max_range As Short
		Dim cap As String
		
		LockGUI()
		
		'Invalid_string_refer_to_original_code
		If is_item Then
			cap = "Invalid_string_refer_to_original_code"
		Else
			cap = Term("Invalid_string_refer_to_original_code", SelectedUnit) & "Invalid_string_refer_to_original_code"
		End If
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		SelectedAbility = AbilityListBox(SelectedUnit, cap, "遘ｻ蜍募燕", is_item)
		Dim BGM As String
		Dim is_transformation As Boolean
		Dim partners() As Unit
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'End If
		
		'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
		If SelectedAbility = 0 Then
			If AutoMoveCursor Then
				RestoreCursorPos()
			End If
			CancelCommand()
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "Invalid_string_refer_to_original_code" And LIndex(.FeatureData(i), 1) = .Ability(SelectedAbility).Name Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
						End If
						Exit For
					End If
				Next 
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.Ability(SelectedAbility).MaxRange = 0 Then
			
			SelectedTarget = SelectedUnit
			
			'Invalid_string_refer_to_original_code
			For i = 1 To SelectedUnit.Ability(SelectedAbility).CountEffect
				If SelectedUnit.Ability(SelectedAbility).EffectType(i) = "螟芽ｺｫ" Then
					is_transformation = True
					Exit For
				End If
			Next 
			
			SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name
			
			'Invalid_string_refer_to_original_code
			HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, SelectedAbilityName)
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
			
			'Invalid_string_refer_to_original_code
			SelectedUnit.ExecuteAbility(SelectedAbility, SelectedUnit)
			SelectedUnit = SelectedUnit.CurrentForm
			CloseMessageForm()
			
			'Invalid_string_refer_to_original_code
			With SelectedUnit
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .CountPilot > 0 Then
					HandleEvent("Invalid_string_refer_to_original_code")
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
			End With
		End If
		'End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			If .CountPilot > 0 Then
				HandleEvent("Invalid_string_refer_to_original_code")
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
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		WaitCommand()
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		DisplayUnitStatus(SelectedUnit)
		ClearUnitStatus()
		'End If
		CommandState = "Invalid_string_refer_to_original_code"
		UnlockGUI()
		'End If
		Exit Sub
		'End If
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			If is_item Then
				If .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then
					SelectedCommand = "Invalid_string_refer_to_original_code"
				Else
					SelectedCommand = "Invalid_string_refer_to_original_code"
				End If
			Else
				If .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then
					SelectedCommand = "Invalid_string_refer_to_original_code"
				Else
					SelectedCommand = "Invalid_string_refer_to_original_code"
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			min_range = .AbilityMinRange(SelectedAbility)
			max_range = .AbilityMaxRange(SelectedAbility)
			
			'Invalid_string_refer_to_original_code
			If .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then
				AreaInCross(.X, .Y, min_range, max_range)
			ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then 
				AreaInWideCross(.X, .Y, min_range, max_range)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ElseIf .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Then 
				AreaInMoveAction(SelectedUnit, max_range)
			Else
				AreaInRange(.X, .Y, max_range, min_range, "縺吶∋縺ｦ")
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'And .Ability(SelectedAbility).MaxRange = 1 _
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
						.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners, t.X, t.Y)
						If UBound(partners) = 0 Then
							MaskData(t.X, t.Y) = True
						End If
					End If
				End If
			Next 
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					If Not MaskData(i, j) Then
						t = MapDataForUnit(i, j)
						If Not t Is Nothing Then
							'Invalid_string_refer_to_original_code
							If .IsAbilityEffective(SelectedAbility, t) Then
								MaskData(i, j) = False
							Else
								MaskData(i, j) = True
							End If
						End If
					End If
				Next 
			Next 
			'End If
			
			'Invalid_string_refer_to_original_code
			If Not MaskData(.X, .Y) Then
				If .IsAbilityClassifiedAs(SelectedAbility, "謠ｴ") Then
					MaskData(.X, .Y) = True
				End If
			End If
			
			If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
				Center(.X, .Y)
			End If
			MaskScreen()
		End With
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		CommandState = "Invalid_string_refer_to_original_code"
		CommandState = "Invalid_string_refer_to_original_code"
		'End If
		
		'Invalid_string_refer_to_original_code
		If Not AutoMoveCursor Then
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		t = Nothing
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
					If t Is Nothing Then
						t = u
					Else
						If System.Math.Abs(SelectedUnit.X - .X) ^ 2 + System.Math.Abs(SelectedUnit.Y - .Y) ^ 2 < System.Math.Abs(SelectedUnit.X - t.X) ^ 2 + System.Math.Abs(SelectedUnit.Y - t.Y) ^ 2 Then
							t = u
						End If
					End If
				End If
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		If t Is Nothing Then
			t = SelectedUnit
		End If
		
		'Invalid_string_refer_to_original_code
		MoveCursorPos("Invalid_string_refer_to_original_code")
		
		'Invalid_string_refer_to_original_code
		If Not SelectedUnit Is t Then
			DisplayUnitStatus(t)
		End If
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .AbilityMaxRange(SelectedAbility) = 1 Then
				.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners, SelectedTarget.X, SelectedTarget.Y)
			Else
				.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners)
			End If
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
		End With
		
		aname = SelectedUnit.Ability(SelectedAbility).Name
		SelectedAbilityName = aname
		
		' ADD START MARGE
		'Invalid_string_refer_to_original_code
		is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Or (SelectedUnit.AbilityMaxRange(SelectedAbility) = 1 And Not SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code"))
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, aname)
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
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					MaskData(.X, .Y) = True
					'End If
				End With
			Next u
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To UBound(partners)
				With partners(i)
					MaskData(.X, .Y) = False
				End With
			Next 
			'End If
			
			MaskData(.X, .Y) = False
			MaskData(SelectedTarget.X, SelectedTarget.Y) = False
			If Not BattleAnimation Then
				MaskScreen()
			End If
			
			'Invalid_string_refer_to_original_code
			.ExecuteAbility(SelectedAbility, SelectedTarget)
			SelectedUnit = .CurrentForm
			
			CloseMessageForm()
			ClearUnitStatus()
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .CountPilot > 0 Then
				HandleEvent("Invalid_string_refer_to_original_code")
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
			'End If
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			If .CountPilot > 0 Then
				HandleEvent("Invalid_string_refer_to_original_code")
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
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		' ADD START MARGE
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If SelectedUnit.MainPilot.IsSkillAvailable("驕頑茶") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
			'Invalid_string_refer_to_original_code
			If SelectedUnitMoveCost > 0 Then
				HandleEvent("騾ｲ蜈･", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInSpeed(SelectedUnit)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		If NewGUIMode Then
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		Else
			DisplayUnitStatus(SelectedUnit)
		End If
		CommandState = "Invalid_string_refer_to_original_code"
		Exit Sub
		'End If
		'End If
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			is_p_ability = .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code") Or (.AbilityMaxRange(SelectedAbility) = 1 And Not .IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code"))
			' ADD END MARGE
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CommandState = "Invalid_string_refer_to_original_code"
			CommandState = "Invalid_string_refer_to_original_code"
			'End If
			
			'逶ｮ讓吝慍轤ｹ
			SelectedX = PixelToMapX(MouseX)
			SelectedY = PixelToMapY(MouseY)
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'1, "蜻ｳ譁ｹ"
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			MaskScreen()
			Exit Sub
			.IsAbilityClassifiedAs(SelectedAbility, "Invalid_string_refer_to_original_code")
			SelectedX = PixelToMapX(MouseX)
			SelectedY = PixelToMapY(MouseY)
			
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				MaskScreen()
				Exit Sub
			End If
			
			'逶ｮ讓吝慍轤ｹ
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CommandState = "Invalid_string_refer_to_original_code"
			CommandState = "Invalid_string_refer_to_original_code"
			'End If
			
			'Invalid_string_refer_to_original_code
			AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
			MaskScreen()
			Exit Sub
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			CommandState = "Invalid_string_refer_to_original_code"
			CommandState = "Invalid_string_refer_to_original_code"
			'End If
			
			'逶ｮ讓吝慍轤ｹ
			SelectedX = PixelToMapX(MouseX)
			SelectedY = PixelToMapY(MouseY)
			
			'Invalid_string_refer_to_original_code
			AreaInPointToPoint(.X, .Y, SelectedX, SelectedY)
			MaskScreen()
			Exit Sub
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.CombinationPartner("Invalid_string_refer_to_original_code", SelectedAbility, partners)
			ReDim SelectedPartners(0)
			ReDim partners(0)
			'End If
		End With
		
		If MainWidth <> 15 Then
			ClearUnitStatus()
		End If
		
		LockGUI()
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
		
		' ADD START MARGE
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If SelectedUnit.MainPilot.IsSkillAvailable("驕頑茶") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
			'Invalid_string_refer_to_original_code
			If SelectedUnitMoveCost > 0 Then
				HandleEvent("騾ｲ蜈･", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		SelectedCommand = "Invalid_string_refer_to_original_code"
		AreaInSpeed(SelectedUnit)
		If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
			Center(SelectedUnit.X, SelectedUnit.Y)
		End If
		MaskScreen()
		If NewGUIMode Then
			System.Windows.Forms.Application.DoEvents()
			ClearUnitStatus()
		Else
			DisplayUnitStatus(SelectedUnit)
		End If
		CommandState = "Invalid_string_refer_to_original_code"
		Exit Sub
		'End If
		'End If
		' ADD END MARGE
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
		
		SelectedCommand = "繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ"
		
		With SelectedUnit
			ReDim pname_list(0)
			ReDim pid_list(0)
			ReDim ListItemFlag(0)
			
			'Invalid_string_refer_to_original_code
			n = .CountPilot + .CountSupport
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			n = n + 1
			'End If
			For i = 1 To n
				If i <= .CountPilot Then
					'Invalid_string_refer_to_original_code
					p = .Pilot(i)
					
					If i = 1 Then
						'Invalid_string_refer_to_original_code
						p = .MainPilot
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						If .CountPilot > 1 Then
							If p.Data.SP <= 0 And .Pilot(1).Data.SP > 0 Then
								p = .Pilot(1)
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code_
				'"繧ｭ繝｣繝ｩ繧ｯ繧ｿ繝ｼ     " & Term("SP", SelectedUnit, 2) _
				'& "/Max" & Term("SP", SelectedUnit, 2), _
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'& "/Max" & Term("SP", SelectedUnit, 2), _
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			End If
			'Invalid_string_refer_to_original_code
			i = 1
			'End If
			
			'隱ｰ繧ゅせ繝壹す繝｣繝ｫ繝代Ρ繝ｼ繧剃ｽｿ縺医↑縺代ｌ縺ｰ繧ｭ繝｣繝ｳ繧ｻ繝ｫ
			If i = 0 Then
				frmListBox.Hide()
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				UnlockGUI()
				CancelCommand()
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			SelectedPilot = PList.Item(pid_list(i))
			'Invalid_string_refer_to_original_code
			If UBound(pname_list) > 1 Then
				DisplayPilotStatus(SelectedPilot)
			End If
		End With
		
		With SelectedPilot
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		With SelectedPilot
			TopItem = 1
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& .Nickname & " " & Term("SP", SelectedUnit) & "=" _
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		End With
		
		'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
		If i = 0 Then
			DisplayUnitStatus(SelectedUnit)
			'Invalid_string_refer_to_original_code
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			UnlockGUI()
			CancelCommand()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		SelectedSpecialPower = SelectedPilot.SpecialPower(i)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ荳隕ｧ
		ReDim list(0)
		For i = 1 To SPDList.Count
			With SPDList.Item(i)
				'Invalid_string_refer_to_original_code_
				'And .ShortName <> "髱櫁｡ｨ遉ｺ" _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve strkey_list(UBound(list))
				list(UBound(list)) = .Name
				strkey_list(UBound(list)) = .KanaName
				'End If
			End With
		Next 
		ReDim ListItemFlag(UBound(list))
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		For i = 1 To UBound(list)
			ListItemFlag(i) = True
			For	Each p In PList
				With p
					If .Party = "蜻ｳ譁ｹ" Then
						If Not .Unit_Renamed Is Nothing Then
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'Invalid_string_refer_to_original_code
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
					'End If
				End With
			Next p
		Next 
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		ReDim ListItemComment(UBound(list))
		For i = 1 To UBound(list)
			ListItemComment(i) = SPDList.Item(list(i)).Comment
		Next 
		
		'Invalid_string_refer_to_original_code
		TopItem = 1
		ret = MultiColumnListBox(Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ") & "讀懃ｴ｢", list, True)
		If ret = 0 Then
			SelectedSpecialPower = ""
			CancelCommand()
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.IsMessageDefined(SelectedSpecialPower) Then
			OpenMessageForm()
			SelectedUnit.PilotMessage(SelectedSpecialPower)
			CloseMessageForm()
		End If
		
		SelectedSpecialPower = list(ret)
		
		WithDoubleSPConsumption = True
		WithDoubleSPConsumption = False
		'End If
		
		sd = SPDList.Item(SelectedSpecialPower)
		
		'Invalid_string_refer_to_original_code
		Select Case sd.TargetType
			Case "蜻ｳ譁ｹ", "謨ｵ", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				
				OpenMessageForm()
				DisplaySysMessage(SelectedPilot.Nickname & "縺ｯ" & SelectedSpecialPower & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code")
				CloseMessageForm()
				
				'Invalid_string_refer_to_original_code
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						MaskData(i, j) = True
						
						u = MapDataForUnit(i, j)
						
						If u Is Nothing Then
							GoTo NextLoop
						End If
						
						'Invalid_string_refer_to_original_code
						Select Case sd.TargetType
							Case "蜻ｳ譁ｹ"
								With u
									If .Party <> "蜻ｳ譁ｹ" And .Party0 <> "蜻ｳ譁ｹ" And .Party <> "Invalid_string_refer_to_original_code" And .Party0 <> "Invalid_string_refer_to_original_code" Then
										GoTo NextLoop
									End If
								End With
							Case "謨ｵ"
								With u
									If (.Party = "蜻ｳ譁ｹ" And .Party0 = "蜻ｳ譁ｹ") Or (.Party = "Invalid_string_refer_to_original_code" And .Party0 = "Invalid_string_refer_to_original_code") Then
										GoTo NextLoop
									End If
								End With
						End Select
						
						'Invalid_string_refer_to_original_code
						If Not sd.Effective(SelectedPilot, u) Then
							GoTo NextLoop
						End If
						
						MaskData(i, j) = False
NextLoop: 
					Next 
				Next 
				MaskScreen()
				CommandState = "Invalid_string_refer_to_original_code"
				UnlockGUI()
				Exit Sub
				
			Case "遐ｴ螢雁袖譁ｹ"
				'Invalid_string_refer_to_original_code
				
				OpenMessageForm()
				DisplaySysMessage(SelectedPilot.Nickname & "縺ｯ" & SelectedSpecialPower & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code")
				CloseMessageForm()
				
				'Invalid_string_refer_to_original_code
				ReDim list(0)
				ReDim id_list(0)
				ReDim ListItemFlag(0)
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code_
						'And (.CountPilot > 0 Or .Data.PilotNum = 0) _
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						ReDim Preserve list(UBound(list) + 1)
						ReDim Preserve id_list(UBound(list))
						ReDim Preserve ListItemFlag(UBound(list))
						list(UBound(list)) = RightPaddedString(.Nickname, 28) & RightPaddedString(.MainPilot.Nickname, 18) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
						id_list(UBound(list)) = .ID
						ListItemFlag(UBound(list)) = False
						'End If
					End With
				Next u
				
				TopItem = 1
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If i = 0 Then
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				SelectedTarget = UList.Item(id_list(i))
		End Select
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If ret = MsgBoxResult.Cancel Then
			UnlockGUI()
			Exit Sub
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
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
		
		'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ繧剃ｽｿ逕ｨ
		If WithDoubleSPConsumption Then
			SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2)
		Else
			SelectedPilot.UseSpecialPower(SelectedSpecialPower)
		End If
		SelectedUnit = SelectedUnit.CurrentForm
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		
		'Invalid_string_refer_to_original_code
		DisplayUnitStatus(SelectedUnit)
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
		End If
		If IsCanceled Then
			IsCanceled = False
		End If
		
		SelectedSpecialPower = ""
		
		UnlockGUI()
		
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub FinishSpecialPowerCommand()
	Private Sub FinishSpecialPowerCommand()
		' MOD END MARGE
		Dim i, ret As Short
		
		LockGUI()
		
		'Invalid_string_refer_to_original_code
		With SPDList.Item(SelectedSpecialPower)
			For i = 1 To .CountEffect
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If ret = MsgBoxResult.Cancel Then
					CommandState = "Invalid_string_refer_to_original_code"
					UnlockGUI()
					Exit Sub
				End If
				Exit For
				'End If
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		
		'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ繧剃ｽｿ逕ｨ
		If WithDoubleSPConsumption Then
			SelectedPilot.UseSpecialPower(SelectedSpecialPower, 2)
		Else
			SelectedPilot.UseSpecialPower(SelectedSpecialPower)
		End If
		SelectedUnit = SelectedUnit.CurrentForm
		
		'Invalid_string_refer_to_original_code
		If Not SelectedTarget Is Nothing Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			DisplayUnitStatus(SelectedTarget)
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
		End If
		If IsCanceled Then
			IsCanceled = False
		End If
		
		SelectedSpecialPower = ""
		
		UnlockGUI()
		
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartFixCommand()
	Private Sub StartFixCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim t, u As Unit
		Dim fname As String
		
		SelectedCommand = "Invalid_string_refer_to_original_code"
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			AreaInRange(.X, .Y, 1, 1, "蜻ｳ譁ｹ")
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If Not MaskData(i, j) And Not MapDataForUnit(i, j) Is Nothing Then
						With MapDataForUnit(i, j)
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							MaskData(i, j) = True
						End With
					End If
					If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
						For k = 2 To CInt(.FeatureData("Invalid_string_refer_to_original_code"))
							fname = LIndex(.FeatureData("Invalid_string_refer_to_original_code"), k)
							If Left(fname, 1) = "!" Then
								fname = Mid(fname, 2)
								If fname <> SelectedUnit.FeatureName0("Invalid_string_refer_to_original_code") Then
									MaskData(i, j) = True
									Exit For
								End If
							Else
								If fname = SelectedUnit.FeatureName0("Invalid_string_refer_to_original_code") Then
									MaskData(i, j) = True
									Exit For
								End If
							End If
						Next 
					End If
				Next 
			Next 
		End With
		'End If
		'Next
		'Next
		'UPGRADE_WARNING: StartFixCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		MaskScreen()
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			t = Nothing
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
						If t Is Nothing Then
							t = u
						ElseIf u.MaxHP - u.HP > t.MaxHP - t.HP Then 
							t = u
						End If
					End If
				End With
			Next u
		End If
		'End With
		'Next
		If t Is Nothing Then
			t = SelectedUnit
		End If
		MoveCursorPos("Invalid_string_refer_to_original_code")
		If Not SelectedUnit Is t Then
			DisplayUnitStatus(t)
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		CommandState = "Invalid_string_refer_to_original_code"
		CommandState = "Invalid_string_refer_to_original_code"
		'End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub FinishFixCommand()
	Private Sub FinishFixCommand()
		' MOD END MARGE
		Dim tmp As Integer
		
		LockGUI()
		
		OpenMessageForm(SelectedTarget, SelectedUnit)
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.PilotMessage("Invalid_string_refer_to_original_code")
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.SpecialEffect("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			
			DisplaySysMessage(.Nickname & "縺ｯ" & SelectedTarget.Nickname & "縺ｫ" & .FeatureName("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			tmp = SelectedTarget.HP
			Select Case .FeatureLevel("Invalid_string_refer_to_original_code")
				Case 1, -1
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Case 2
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Case 3
					SelectedTarget.RecoverHP(100)
			End Select
			If IsNumeric(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)) Then
				.EN = .EN - CShort(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2))
			End If
			
			DrawSysString(SelectedTarget.X, SelectedTarget.Y, "+" & VB6.Format(SelectedTarget.HP - tmp))
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "縺ｮ" & Term("Invalid_string_refer_to_original_code", SelectedTarget) & "Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			
			'Invalid_string_refer_to_original_code
			.GetExp(SelectedTarget, "Invalid_string_refer_to_original_code")
			
			If MessageWait < 10000 Then
				Sleep(MessageWait)
			End If
		End With
		
		CloseMessageForm()
		
		'Invalid_string_refer_to_original_code
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartSupplyCommand()
	Private Sub StartSupplyCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim t, u As Unit
		
		SelectedCommand = "陬懃ｵｦ"
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			AreaInRange(.X, .Y, 1, 1, "蜻ｳ譁ｹ")
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If Not MaskData(i, j) And Not MapDataForUnit(i, j) Is Nothing Then
						MaskData(i, j) = True
						With MapDataForUnit(i, j)
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							MaskData(i, j) = False
						End With
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
				Next 
			Next 
		End With
		'End If
		'Next
		'Next
		'UPGRADE_WARNING: StartSupplyCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		MaskScreen()
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			t = Nothing
			For	Each u In UList
				With u
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
						t = u
						Exit For
					End If
				End With
			Next u
		End If
		'End With
		'Next
		If t Is Nothing Then
			t = SelectedUnit
		End If
		MoveCursorPos("Invalid_string_refer_to_original_code")
		If Not SelectedUnit Is t Then
			DisplayUnitStatus(t)
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		CommandState = "Invalid_string_refer_to_original_code"
		CommandState = "Invalid_string_refer_to_original_code"
		'End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub FinishSupplyCommand()
	Private Sub FinishSupplyCommand()
		' MOD END MARGE
		
		LockGUI()
		
		OpenMessageForm(SelectedTarget, SelectedUnit)
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("陬懃ｵｦ") Then
				.PilotMessage("陬懃ｵｦ")
			End If
			If .IsAnimationDefined("陬懃ｵｦ", .FeatureName("Invalid_string_refer_to_original_code")) Then
				.PlayAnimation("陬懃ｵｦ", .FeatureName("Invalid_string_refer_to_original_code"))
			Else
				.SpecialEffect("陬懃ｵｦ", .FeatureName("Invalid_string_refer_to_original_code"))
			End If
			
			DisplaySysMessage(.Nickname & "縺ｯ" & SelectedTarget.Nickname & "縺ｫ" & .FeatureName("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code")
			
			'陬懃ｵｦ繧貞ｮ滓命
			SelectedTarget.FullSupply()
			SelectedTarget.IncreaseMorale(-10)
			If IsNumeric(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)) Then
				.EN = .EN - CShort(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2))
			End If
			
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "縺ｮ蠑ｾ謨ｰ縺ｨ" & Term("Invalid_string_refer_to_original_code", SelectedTarget) & "Invalid_string_refer_to_original_code")
			
			'Invalid_string_refer_to_original_code
			.GetExp(SelectedTarget, "陬懃ｵｦ")
			
			If MessageWait < 10000 Then
				Sleep(MessageWait)
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		CloseMessageForm()
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub ChargeCommand()
	Private Sub ChargeCommand()
		' MOD END MARGE
		Dim ret As Short
		Dim partners() As Unit
		Dim i As Short
		
		LockGUI()
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If ret = MsgBoxResult.Cancel Then
			CancelCommand()
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		HandleEvent("菴ｿ逕ｨ", SelectedUnit.MainPilot.ID, "繝√Ε繝ｼ繧ｸ")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("繝√Ε繝ｼ繧ｸ") Then
				OpenMessageForm()
				.PilotMessage("繝√Ε繝ｼ繧ｸ")
				CloseMessageForm()
			End If
			
			'繧｢繝九Γ陦ｨ遉ｺ繧定｡後≧
			If .IsAnimationDefined("繝√Ε繝ｼ繧ｸ") Then
				.PlayAnimation("繝√Ε繝ｼ繧ｸ")
			ElseIf .IsSpecialEffectDefined("繝√Ε繝ｼ繧ｸ") Then 
				.SpecialEffect("繝√Ε繝ｼ繧ｸ")
			Else
				PlayWave("Charge.wav")
			End If
			
			'Invalid_string_refer_to_original_code
			ReDim partners(0)
			For i = 1 To .CountWeapon
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .IsWeaponAvailable(i, "繝√Ε繝ｼ繧ｸ") Then
					.CombinationPartner("Invalid_string_refer_to_original_code")
					Exit For
				End If
				'End If
			Next 
			If UBound(partners) = 0 Then
				For i = 1 To .CountAbility
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If .IsAbilityAvailable(i, "繝√Ε繝ｼ繧ｸ") Then
						.CombinationPartner("Invalid_string_refer_to_original_code", i, partners)
						Exit For
					End If
				Next 
			End If
			'Next
			'End If
			
			'Invalid_string_refer_to_original_code
			.AddCondition("繝√Ε繝ｼ繧ｸ", 1)
			
			'Invalid_string_refer_to_original_code
			For i = 1 To UBound(partners)
				With partners(i)
					.AddCondition("繝√Ε繝ｼ繧ｸ", 1)
				End With
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If IsScenarioFinished Then
			IsScenarioFinished = False
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		
		UnlockGUI()
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartTalkCommand()
	Private Sub StartTalkCommand()
		' MOD END MARGE
		Dim i, j As Short
		Dim t As Unit
		
		SelectedCommand = "莨夊ｩｱ"
		
		'UPGRADE_NOTE: オブジェクト t をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		t = Nothing
		
		'莨夊ｩｱ蜿ｯ閭ｽ縺ｪ繝ｦ繝九ャ繝医ｒ陦ｨ遉ｺ
		With SelectedUnit
			AreaInRange(.X, .Y, 1, 1, "")
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If Not MaskData(i, j) Then
						If Not MapDataForUnit(i, j) Is Nothing Then
							If Not IsEventDefined("莨夊ｩｱ " & .MainPilot.ID & " " & MapDataForUnit(i, j).MainPilot.ID) Then
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
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			If Not t Is Nothing Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
				DisplayUnitStatus(t)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		CommandState = "Invalid_string_refer_to_original_code"
		CommandState = "Invalid_string_refer_to_original_code"
		'End If
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		'莨夊ｩｱ繧､繝吶Φ繝医ｒ螳滓命
		HandleEvent("莨夊ｩｱ", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
		
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
		
		'Invalid_string_refer_to_original_code
		WaitCommand()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		fdata = SelectedUnit.FeatureData("螟牙ｽ｢")
		
		If MapFileName = "" Then
			'Invalid_string_refer_to_original_code
			With SelectedUnit
				ReDim list(0)
				ReDim list_id(0)
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				If UBound(list) > 1 Then
					TopItem = 1
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If ret = 0 Then
						CancelCommand()
						UnlockGUI()
						Exit Sub
					End If
				Else
					ret = 1
				End If
				
				'螟牙ｽ｢繧貞ｮ滓命
				.Transform((.OtherForm(list_id(ret)).Name))
				
				'Invalid_string_refer_to_original_code
				MakeUnitList()
				
				'Invalid_string_refer_to_original_code
				DisplayUnitStatus(.CurrentForm)
				
				'Invalid_string_refer_to_original_code
				UnlockGUI()
				CommandState = "Invalid_string_refer_to_original_code"
				Exit Sub
			End With
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If UBound(list) = 1 Then
			If ListItemFlag(1) Then
				MsgBox("Invalid_string_refer_to_original_code" & LIndex(fdata, 1) & "縺ｧ縺阪∪縺帙ｓ")
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			ret = 1
		Else
			TopItem = 1
			If Not SelectedUnit.IsHero Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
			'Invalid_string_refer_to_original_code
			With UDList.Item(uname)
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					If Not PList.IsDefined(.FeatureData("Invalid_string_refer_to_original_code")) Then
						If Not PDList.IsDefined(.FeatureData("Invalid_string_refer_to_original_code")) Then
							ErrorMessage(uname & "Invalid_string_refer_to_original_code")
							.FeatureData(("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code")
							TerminateSRC()
						End If
						PList.Add(.FeatureData("Invalid_string_refer_to_original_code"), SelectedUnit.MainPilot.Level, (SelectedUnit.Party0))
					End If
				End If
			End With
			
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "Invalid_string_refer_to_original_code" And LIndex(.FeatureData(i), 1) = uname Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
							Sleep(500)
						End If
						Exit For
					End If
				Next 
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("螟牙ｽ｢(" & .Name & "=>" & uname & ")") Or .IsMessageDefined("螟牙ｽ｢(" & uname & ")") Or .IsMessageDefined("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")") Then
				Center(.X, .Y)
				RefreshScreen()
				
				OpenMessageForm()
				If .IsMessageDefined("螟牙ｽ｢(" & .Name & "=>" & uname & ")") Then
					.PilotMessage("螟牙ｽ｢(" & .Name & "=>" & uname & ")")
				ElseIf .IsMessageDefined("螟牙ｽ｢(" & uname & ")") Then 
					.PilotMessage("螟牙ｽ｢(" & uname & ")")
				ElseIf .IsMessageDefined("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")") Then 
					.PilotMessage("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")")
				End If
				CloseMessageForm()
			End If
			
			'繧｢繝九Γ陦ｨ遉ｺ
			If .IsAnimationDefined("螟牙ｽ｢(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("螟牙ｽ｢(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("螟牙ｽ｢(" & uname & ")") Then 
				.PlayAnimation("螟牙ｽ｢(" & uname & ")")
			ElseIf .IsAnimationDefined("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")") Then 
				.PlayAnimation("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")")
			ElseIf .IsSpecialEffectDefined("螟牙ｽ｢(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("螟牙ｽ｢(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("螟牙ｽ｢(" & uname & ")") Then 
				.SpecialEffect("螟牙ｽ｢(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")") Then 
				.SpecialEffect("螟牙ｽ｢(" & .FeatureName("螟牙ｽ｢") & ")")
			End If
		End With
		
		'螟牙ｽ｢
		prev_uname = SelectedUnit.Name
		SelectedUnit.Transform(uname)
		SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.Action = 0 Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'vbOKCancel + vbQuestion, "螟牙ｽ｢")
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If ret = MsgBoxResult.Cancel Then
				SelectedUnit.Transform(prev_uname)
				SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				SelectedUnit.DeleteCondition("Invalid_string_refer_to_original_code")
			End If
		End If
		RedrawScreen()
		'End If
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit.CurrentForm
			HandleEvent("螟牙ｽ｢", .MainPilot.ID, .Name)
		End With
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		IsCanceled = False
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.CurrentForm.CheckAutoHyperMode()
		SelectedUnit.CurrentForm.CheckAutoNormalMode()
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		DisplayUnitStatus(SelectedUnit)
		'End If
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		If MapFileName = "" Then
			'Invalid_string_refer_to_original_code
			With SelectedUnit
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: HyperModeCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		MakeUnitList()
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: HyperModeCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		UnlockGUI()
		CommandState = "Invalid_string_refer_to_original_code"
		Exit Sub
		'End With
		'End If
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit.OtherForm(uname)
			If Not .IsAbleToEnter(SelectedUnit.X, SelectedUnit.Y) And MapFileName <> "" Then
				MsgBox("Invalid_string_refer_to_original_code")
				UnlockGUI()
				CancelCommand()
				Exit Sub
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		With UDList.Item(uname)
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				If Not PList.IsDefined(.FeatureData("Invalid_string_refer_to_original_code")) Then
					If Not PDList.IsDefined(.FeatureData("Invalid_string_refer_to_original_code")) Then
						ErrorMessage(uname & "Invalid_string_refer_to_original_code")
						.FeatureData(("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code")
						TerminateSRC()
					End If
					PList.Add(.FeatureData("Invalid_string_refer_to_original_code"), SelectedUnit.MainPilot.Level, (SelectedUnit.Party0))
				End If
			End If
		End With
		
		Dim BGM As String
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "Invalid_string_refer_to_original_code" And LIndex(.FeatureData(i), 1) = uname Then
						BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
						If Len(BGM) > 0 Then
							ChangeBGM(BGM)
							Sleep(500)
						End If
						Exit For
					End If
				Next 
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Center(.X, .Y)
			RefreshScreen()
			
			OpenMessageForm()
			If .IsMessageDefined("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")") Then
				.PilotMessage("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")")
			ElseIf .IsMessageDefined("Invalid_string_refer_to_original_code" & uname & ")") Then 
				.PilotMessage("Invalid_string_refer_to_original_code" & uname & ")")
			ElseIf .IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
				.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
			Else
				.PilotMessage("Invalid_string_refer_to_original_code")
			End If
			CloseMessageForm()
			'End If
			
			'繧｢繝九Γ陦ｨ遉ｺ
			If .IsAnimationDefined("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code" & uname & ")") Then 
				.PlayAnimation("Invalid_string_refer_to_original_code" & uname & ")")
			ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
				.PlayAnimation("Invalid_string_refer_to_original_code" & fname & ")")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				.PlayAnimation("Invalid_string_refer_to_original_code")
			ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("Invalid_string_refer_to_original_code" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & uname & ")") Then 
				.SpecialEffect("Invalid_string_refer_to_original_code" & uname & ")")
			ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
				.SpecialEffect("Invalid_string_refer_to_original_code" & fname & ")")
			Else
				.SpecialEffect("Invalid_string_refer_to_original_code")
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.Transform(uname)
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.CurrentForm.CheckAutoHyperMode()
		SelectedUnit.CurrentForm.CheckAutoNormalMode()
		SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit.CurrentForm
			HandleEvent("螟牙ｽ｢", .MainPilot.ID, .Name)
		End With
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		IsCanceled = False
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		DisplayUnitStatus(SelectedUnit)
		'End If
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				MakeUnitList()
				DisplayUnitStatus(.CurrentForm)
				UnlockGUI()
				CommandState = "Invalid_string_refer_to_original_code"
				Exit Sub
			End If
			
			If .IsHero Then
				'Invalid_string_refer_to_original_code_
				'vbOKCancel + vbQuestion, "螟芽ｺｫ隗｣髯､")
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				'Invalid_string_refer_to_original_code_
				'vbOKCancel + vbQuestion, "迚ｹ谿翫Δ繝ｼ繝芽ｧ｣髯､")
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			End If
			If ret = MsgBoxResult.Cancel Then
				UnlockGUI()
				CancelCommand()
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			SelectedUnit = MapDataForUnit(.X, .Y)
		End With
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		DisplayUnitStatus(SelectedUnit)
		
		RedrawScreen()
		
		'Invalid_string_refer_to_original_code
		HandleEvent("螟牙ｽ｢", SelectedUnit.MainPilot.ID, SelectedUnit.Name)
		IsScenarioFinished = False
		IsCanceled = False
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			With SelectedUnit
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					tname = LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)
					.Transform(tname)
				Else
					.Split_Renamed()
				End If
				UList.CheckAutoHyperMode()
				UList.CheckAutoNormalMode()
				DisplayUnitStatus(MapDataForUnit(.X, .Y))
			End With
			
			'Invalid_string_refer_to_original_code
			MakeUnitList()
			
			'Invalid_string_refer_to_original_code
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		
		With SelectedUnit
			If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If ret = MsgBoxResult.Cancel Then
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				tname = LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)
				
				If Not .OtherForm(tname).IsAbleToEnter(.X, .Y) Then
					MsgBox("Invalid_string_refer_to_original_code")
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				'Invalid_string_refer_to_original_code
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
					If Len(BGM) > 0 Then
						StartBGM(.FeatureData("Invalid_string_refer_to_original_code"))
						Sleep(500)
					End If
				End If
				
				fname = .FeatureName("Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				If .IsMessageDefined("Invalid_string_refer_to_original_code" & .Name & ")") Or .IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Or .IsMessageDefined("Invalid_string_refer_to_original_code") Then
					Center(.X, .Y)
					RefreshScreen()
					
					OpenMessageForm()
					If .IsMessageDefined("Invalid_string_refer_to_original_code" & .Name & ")") Then
						.PilotMessage("Invalid_string_refer_to_original_code" & .Name & ")")
					ElseIf .IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
						.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
					Else
						.PilotMessage("Invalid_string_refer_to_original_code")
					End If
					CloseMessageForm()
				End If
				
				'繧｢繝九Γ陦ｨ遉ｺ
				If .IsAnimationDefined("Invalid_string_refer_to_original_code" & .Name & ")") Then
					.PlayAnimation("Invalid_string_refer_to_original_code" & .Name & ")")
				ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
					.PlayAnimation("Invalid_string_refer_to_original_code" & fname & ")")
				ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code") Then 
					.PlayAnimation("Invalid_string_refer_to_original_code")
				ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & .Name & ")") Then 
					.SpecialEffect("Invalid_string_refer_to_original_code" & .Name & ")")
				ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
					.SpecialEffect("Invalid_string_refer_to_original_code" & fname & ")")
				Else
					.SpecialEffect("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				uname = .Name
				.Transform(tname)
				SelectedUnit = MapDataForUnit(.X, .Y)
				DisplayUnitStatus(SelectedUnit)
			Else
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If ret = MsgBoxResult.Cancel Then
					UnlockGUI()
					CancelCommand()
					Exit Sub
				End If
				
				'Invalid_string_refer_to_original_code
				If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Then
					BGM = SearchMidiFile(.FeatureData("Invalid_string_refer_to_original_code"))
					If Len(BGM) > 0 Then
						StartBGM(.FeatureData("Invalid_string_refer_to_original_code"))
						Sleep(500)
					End If
				End If
				
				fname = .FeatureName("Invalid_string_refer_to_original_code")
				
				'Invalid_string_refer_to_original_code
				If .IsMessageDefined("Invalid_string_refer_to_original_code" & .Name & ")") Or .IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Or .IsMessageDefined("Invalid_string_refer_to_original_code") Then
					Center(.X, .Y)
					RefreshScreen()
					
					OpenMessageForm()
					If .IsMessageDefined("Invalid_string_refer_to_original_code" & .Name & ")") Then
						.PilotMessage("Invalid_string_refer_to_original_code" & .Name & ")")
					ElseIf .IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
						.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
					Else
						.PilotMessage("Invalid_string_refer_to_original_code")
					End If
					CloseMessageForm()
				End If
				
				'繧｢繝九Γ陦ｨ遉ｺ
				If .IsAnimationDefined("Invalid_string_refer_to_original_code" & .Name & ")") Then
					.PlayAnimation("Invalid_string_refer_to_original_code" & .Name & ")")
				ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
					.PlayAnimation("Invalid_string_refer_to_original_code" & fname & ")")
				ElseIf .IsAnimationDefined("Invalid_string_refer_to_original_code") Then 
					.PlayAnimation("Invalid_string_refer_to_original_code")
				ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & .Name & ")") Then 
					.SpecialEffect("Invalid_string_refer_to_original_code" & .Name & ")")
				ElseIf .IsSpecialEffectDefined("Invalid_string_refer_to_original_code" & fname & ")") Then 
					.SpecialEffect("Invalid_string_refer_to_original_code" & fname & ")")
				Else
					.SpecialEffect("Invalid_string_refer_to_original_code")
				End If
				
				'Invalid_string_refer_to_original_code
				uname = .Name
				.Split_Renamed()
				
				'Invalid_string_refer_to_original_code
				SelectedUnit = UList.Item(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2))
				
				DisplayUnitStatus(SelectedUnit)
				
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code", SelectedUnit.MainPilot.ID, uname)
		If IsScenarioFinished Then
			IsScenarioFinished = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		IsCanceled = False
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		
		'Invalid_string_refer_to_original_code
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				DisplayUnitStatus(MapDataForUnit(.X, .Y))
				MapDataForUnit(.X, .Y).CheckAutoHyperMode()
				MapDataForUnit(.X, .Y).CheckAutoNormalMode()
				
				'Invalid_string_refer_to_original_code
				MakeUnitList()
				
				'Invalid_string_refer_to_original_code
				CommandState = "Invalid_string_refer_to_original_code"
				UnlockGUI()
				Exit Sub
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountFeature
				'Invalid_string_refer_to_original_code_
				'And (LLength(.FeatureData(i)) > 3 Or MapFileName = "") _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					GoTo NextLoop
					'End If
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
				'End If
NextLoop: 
			Next 
			
			'Invalid_string_refer_to_original_code
			If UBound(list) = 1 Then
				i = 1
			Else
				TopItem = 1
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If i = 0 Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
		End With
		
		If MapFileName = "" Then
			'Invalid_string_refer_to_original_code
			SelectedUnit.Combine(list(i), True)
			
			'Invalid_string_refer_to_original_code
			UList.CheckAutoHyperMode()
			UList.CheckAutoNormalMode()
			
			'Invalid_string_refer_to_original_code
			MakeUnitList()
			
			'Invalid_string_refer_to_original_code
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.Combine(list(i))
		
		'Invalid_string_refer_to_original_code
		UList.CheckAutoHyperMode()
		UList.CheckAutoNormalMode()
		
		'Invalid_string_refer_to_original_code
		SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
		
		'陦悟虚謨ｰ豸郁ｲｻ
		SelectedUnit.UseAction()
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		
		DisplayUnitStatus(SelectedUnit)
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			UnlockGUI()
			Exit Sub
		End If
		If IsCanceled Then
			IsCanceled = False
			ClearUnitStatus()
			RedrawScreen()
			CommandState = "Invalid_string_refer_to_original_code"
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		WaitCommand(True)
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			
			'Invalid_string_refer_to_original_code
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
						
						'Invalid_string_refer_to_original_code
						If SelectedUnit.Nickname0 = .Nickname Then
							list(UBound(list)) = RightPaddedString(.Name, 27)
						Else
							list(UBound(list)) = RightPaddedString(.Nickname0, 27)
						End If
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5) & " " & .Data.Adaption
						
						'Invalid_string_refer_to_original_code
						max_value = 0
						For j = 1 To .CountWeapon
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							If .WeaponPower(j, "") > max_value Then
								max_value = .WeaponPower(j, "")
							End If
						Next 
					End If
				End With
			Next 
			list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(max_value), 7)
			
			'Invalid_string_refer_to_original_code
			max_value = 0
			For j = 1 To .CountWeapon
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .WeaponMaxRange(j) > max_value Then
					max_value = .WeaponMaxRange(j)
				End If
				'End If
			Next 
			list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(max_value), 5)
			
			'Invalid_string_refer_to_original_code
			ReDim farray(0)
			For j = 1 To .CountFeature
				If .FeatureName(j) <> "" Then
					'Invalid_string_refer_to_original_code
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
			'End If
		End With
		'Next
		ReDim ListItemFlag(UBound(list))
		
		'Invalid_string_refer_to_original_code
		TopItem = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If ret = 0 Then
			CancelCommand()
			UnlockGUI()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		
		'Invalid_string_refer_to_original_code
		MakeUnitList()
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		'UPGRADE_WARNING: ExchangeFormCommand に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		TopItem = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		'Invalid_string_refer_to_original_code
		If ret = 0 Then
			ReDim ListItemID(0)
			CancelCommand()
			Exit Sub
		End If
		
		SelectedCommand = "逋ｺ騾ｲ"
		
		'Invalid_string_refer_to_original_code
		SelectedTarget = UList.Item(ListItemID(ret))
		With SelectedTarget
			.X = SelectedUnit.X
			.Y = SelectedUnit.Y
			
			'Invalid_string_refer_to_original_code_
			'And (.Data.Speed = 0 _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			AreaInTeleport(SelectedTarget)
			'Invalid_string_refer_to_original_code_
			'And (.Data.Speed = 0 _
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'繧ｸ繝｣繝ｳ繝励↓繧医ｋ逋ｺ騾ｲ
			AreaInSpeed(SelectedTarget, True)
			'騾壼ｸｸ遘ｻ蜍輔↓繧医ｋ逋ｺ騾ｲ
			AreaInSpeed(SelectedTarget)
			'End If
			
			'豈崎襖繧剃ｸｭ螟ｮ陦ｨ遉ｺ
			Center(.X, .Y)
			
			'逋ｺ騾ｲ縺輔○繧九Θ繝九ャ繝医ｒ豈崎襖縺ｮ莉｣繧上ｊ縺ｫ陦ｨ遉ｺ
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
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		CommandState = "Invalid_string_refer_to_original_code"
		'End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub FinishLaunchCommand()
	Private Sub FinishLaunchCommand()
		' MOD END MARGE
		Dim ret As Short
		
		LockGUI()
		
		With SelectedTarget
			'Invalid_string_refer_to_original_code
			If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
				If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("豈崎襖") Then
					'Invalid_string_refer_to_original_code_
					'vbOKCancel + vbQuestion, "逹濶ｦ")
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Else
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				End If
				If ret = MsgBoxResult.Cancel Then
					CancelCommand()
					UnlockGUI()
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .IsMessageDefined("逋ｺ騾ｲ(" & .Name & ")") Then
				OpenMessageForm()
				.PilotMessage("逋ｺ騾ｲ(" & .Name & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("逋ｺ騾ｲ") Then 
				OpenMessageForm()
				.PilotMessage("逋ｺ騾ｲ")
				CloseMessageForm()
			End If
			
			.SpecialEffect("逋ｺ騾ｲ", .Name)
			
			PrevUnitArea = .Area
			PrevUnitEN = .EN
			.Status_Renamed = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			.Move(SelectedX, SelectedY)
		End With
		
		'Invalid_string_refer_to_original_code
		With SelectedUnit
			PrevUnitX = .X
			PrevUnitY = .Y
			.UnloadUnit((SelectedTarget.ID))
			
			'Invalid_string_refer_to_original_code
			MapDataForUnit(.X, .Y) = SelectedUnit
			PaintUnitBitmap(SelectedUnit)
		End With
		
		SelectedUnit = SelectedTarget
		With SelectedUnit
			If MapDataForUnit(.X, .Y).ID <> .ID Then
				RedrawScreen()
				CommandState = "Invalid_string_refer_to_original_code"
				UnlockGUI()
				Exit Sub
			End If
		End With
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
		ProceedCommand()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub StartOrderCommand()
	Private Sub StartOrderCommand()
		' MOD END MARGE
		Dim list() As String
		Dim i, ret, j As Short
		
		LockGUI()
		
		ReDim list(4)
		ReDim ListItemFlag(4)
		
		'Invalid_string_refer_to_original_code
		list(1) = "Invalid_string_refer_to_original_code"
		list(2) = "Invalid_string_refer_to_original_code"
		list(3) = "Invalid_string_refer_to_original_code"
		list(4) = "Invalid_string_refer_to_original_code"
		If Not SelectedUnit.Summoner Is Nothing Or Not SelectedUnit.Master Is Nothing Then
			ReDim Preserve list(5)
			ReDim Preserve ListItemFlag(5)
			If Not SelectedUnit.Master Is Nothing Then
				list(5) = "Invalid_string_refer_to_original_code"
			Else
				list(5) = "Invalid_string_refer_to_original_code"
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		ret = ListBox("蜻ｽ莉､", list, "陦悟虚繝代ち繝ｼ繝ｳ")
		
		'Invalid_string_refer_to_original_code
		Select Case ret
			Case 0
				CancelCommand()
			Case 1 '閾ｪ逕ｱ
				SelectedUnit.Mode = "騾壼ｸｸ"
				CommandState = "Invalid_string_refer_to_original_code"
				DisplayUnitStatus(SelectedUnit)
			Case 2 'Invalid_string_refer_to_original_code
				SelectedCommand = "遘ｻ蜍募多莉､"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						MaskData(i, j) = False
					Next 
				Next 
				MaskScreen()
				CommandState = "Invalid_string_refer_to_original_code"
			Case 3 'Invalid_string_refer_to_original_code
				SelectedCommand = "Invalid_string_refer_to_original_code"
				AreaWithUnit("蜻ｳ譁ｹ縺ｮ謨ｵ")
				MaskData(SelectedUnit.X, SelectedUnit.Y) = True
				MaskScreen()
				CommandState = "Invalid_string_refer_to_original_code"
			Case 4 'Invalid_string_refer_to_original_code
				SelectedCommand = "隴ｷ陦帛多莉､"
				AreaWithUnit("蜻ｳ譁ｹ")
				MaskData(SelectedUnit.X, SelectedUnit.Y) = True
				MaskScreen()
				CommandState = "Invalid_string_refer_to_original_code"
			Case 5 'Invalid_string_refer_to_original_code
				If Not SelectedUnit.Master Is Nothing Then
					SelectedUnit.Mode = SelectedUnit.Master.MainPilot.ID
				Else
					SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot.ID
				End If
				CommandState = "Invalid_string_refer_to_original_code"
				DisplayUnitStatus(SelectedUnit)
		End Select
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub FinishOrderCommand()
	Private Sub FinishOrderCommand()
		' MOD END MARGE
		Select Case SelectedCommand
			Case "遘ｻ蜍募多莉､"
				SelectedUnit.Mode = VB6.Format(SelectedX) & " " & VB6.Format(SelectedY)
			Case "Invalid_string_refer_to_original_code", "隴ｷ陦帛多莉､"
				SelectedUnit.Mode = SelectedTarget.MainPilot.ID
		End Select
		If DisplayedUnit Is SelectedUnit Then
			DisplayUnitStatus(SelectedUnit)
		End If
		
		RedrawScreen()
		
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		ReDim list(0)
		Dim id_ist(0) As Object
		ReDim is_unit_feature(0)
		
		'豁ｦ蝎ｨ繝ｻ髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		With SelectedUnit
			If .IsFeatureAvailable("豁ｦ蝎ｨ繧ｯ繝ｩ繧ｹ") Or .IsFeatureAvailable("髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ") Then
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve is_unit_feature(UBound(list))
				list(UBound(list)) = "豁ｦ蝎ｨ繝ｻ髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ"
				id_list(UBound(list)) = "豁ｦ蝎ｨ繝ｻ髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ"
				is_unit_feature(UBound(list)) = True
			End If
		End With
		'End If
		
		With SelectedUnit.MainPilot
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountSkill
				Select Case .Skill(i)
					Case "蠕玲э謚", "荳榊ｾ玲焔"
						fname = .Skill(i)
					Case Else
						fname = .SkillName(i)
				End Select
				
				'Invalid_string_refer_to_original_code
				If InStr(fname, "髱櫁｡ｨ遉ｺ") > 0 Then
					GoTo NextSkill
				End If
				
				'Invalid_string_refer_to_original_code
				For j = 1 To UBound(list)
					If list(j) = fname Then
						GoTo NextSkill
					End If
				Next 
				
				'繝ｪ繧ｹ繝医↓霑ｽ蜉
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				list(UBound(list)) = fname
				id_list(UBound(list)) = VB6.Format(i)
NextSkill: 
			Next 
		End With
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountCondition
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextSkill2
				'End If
				
				ftype = Left(.Condition(i), Len(.Condition(i)) - 3)
				
				'Invalid_string_refer_to_original_code
				Select Case LIndex(.ConditionData(i), 1)
					Case "髱櫁｡ｨ遉ｺ", "隗｣隱ｬ"
						GoTo NextSkill2
				End Select
				
				'Invalid_string_refer_to_original_code
				If .ConditionLifetime(i) = 0 Then
					GoTo NextSkill2
				End If
				
				'陦ｨ遉ｺ蜷咲ｧｰ
				fname = .MainPilot.SkillName(ftype)
				If InStr(fname, "髱櫁｡ｨ遉ｺ") > 0 Then
					GoTo NextSkill2
				End If
				
				'Invalid_string_refer_to_original_code
				For j = 1 To UBound(list)
					If list(j) = fname Then
						GoTo NextSkill2
					End If
				Next 
				
				'繝ｪ繧ｹ繝医↓霑ｽ蜉
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				list(UBound(list)) = fname
				id_list(UBound(list)) = ftype
NextSkill2: 
			Next 
			ReDim Preserve is_unit_feature(UBound(list))
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If .CountAllFeature > .AdditionalFeaturesNum Then
				i = .AdditionalFeaturesNum + 1
			Else
				i = 1
			End If
			Do While i <= .CountAllFeature
				'Invalid_string_refer_to_original_code
				If .AllFeatureName(i) = "" Then
					GoTo NextFeature
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'And Not UList.IsDefined(LIndex(.AllFeatureData(i), 2)) _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextFeature
				'End If
				
				'Invalid_string_refer_to_original_code
				For j = 1 To UBound(list)
					If list(j) = .AllFeatureName(i) Then
						GoTo NextFeature
					End If
				Next 
				
				'繝ｪ繧ｹ繝医↓霑ｽ蜉
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
					'Invalid_string_refer_to_original_code
					If .AdditionalFeaturesNum > 0 Then
						i = 0
					End If
				End If
				i = i + 1
			Loop 
			
			'Invalid_string_refer_to_original_code
			For i = 1 To .CountCondition
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextSkill3
				'End If
				
				ftype = Left(.Condition(i), Len(.Condition(i)) - 2)
				
				'Invalid_string_refer_to_original_code
				If ftype = "Invalid_string_refer_to_original_code" Then
					GoTo NextSkill3
				End If
				Select Case LIndex(.ConditionData(i), 1)
					Case "髱櫁｡ｨ遉ｺ", "隗｣隱ｬ"
						GoTo NextSkill3
				End Select
				
				'Invalid_string_refer_to_original_code
				If .ConditionLifetime(i) = 0 Then
					GoTo NextSkill3
				End If
				
				'陦ｨ遉ｺ蜷咲ｧｰ
				If .FeatureName0(ftype) = "" Then
					GoTo NextSkill3
				End If
				fname = .MainPilot.SkillName0(ftype)
				If InStr(fname, "髱櫁｡ｨ遉ｺ") > 0 Then
					GoTo NextSkill3
				End If
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				For j = 1 To UBound(list)
					If list(j) = fname Or list(j) = fname0 Then
						GoTo NextSkill3
					End If
				Next 
				
				'繝ｪ繧ｹ繝医↓霑ｽ蜉
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
				If id_list(ret) = "豁ｦ蝎ｨ繝ｻ髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ" Then
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
				ret = ListBox("Invalid_string_refer_to_original_code", list, "閭ｽ蜉帛錐", "陦ｨ遉ｺ縺ｮ縺ｿ")
				If AutoMoveCursor Then
					MoveCursorPos("繝繧､繧｢繝ｭ繧ｰ")
				End If
				Do While True
					ret = ListBox("Invalid_string_refer_to_original_code", list, "閭ｽ蜉帛錐", "騾｣邯夊｡ｨ遉ｺ")
					'list縺御ｸ螳壹↑縺ｮ縺ｧ騾｣邯夊｡ｨ遉ｺ繧呈ｵ∫畑
					frmListBox.Hide()
					If ret = 0 Then
						Exit Do
					End If
					If id_list(ret) = "豁ｦ蝎ｨ繝ｻ髦ｲ蜈ｷ繧ｯ繝ｩ繧ｹ" Then
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
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			w = WeaponListBox(SelectedUnit, "Invalid_string_refer_to_original_code", "荳隕ｧ")
			SelectedWeapon = w
			
			If SelectedWeapon <= 0 Then
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				frmListBox.Hide()
				UnlockGUI()
				CommandState = "Invalid_string_refer_to_original_code"
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			ReDim list(0)
			i = 0
			With SelectedUnit
				wclass = .WeaponClass(w)
				
				Do While i <= Len(wclass)
					i = i + 1
					buf = GetClassBundle(wclass, i)
					atype = ""
					alevel = ""
					
					'Invalid_string_refer_to_original_code
					If buf = "|" Then
						Exit Do
					End If
					
					'Invalid_string_refer_to_original_code
					If Mid(wclass, i, 1) = "Invalid_string_refer_to_original_code" Then
						i = i + 1
						buf = buf & Mid(wclass, i, 1)
					End If
					
					'Invalid_string_refer_to_original_code
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
					
					'螻樊ｧ縺ｮ蜷咲ｧｰ
					atype = AttributeName(SelectedUnit, buf)
					If Len(atype) > 0 Then
						ReDim Preserve list(UBound(list) + 1)
						
						If Len(alevel) > 0 Then
							list(UBound(list)) = RightPaddedString(buf & "L" & alevel, 8) & atype & "繝ｬ繝吶Ν" & StrConv(alevel, VbStrConv.Wide)
						Else
							list(UBound(list)) = RightPaddedString(buf, 8) & atype
						End If
					End If
				Loop 
				
				If MapFileName <> "" Then
					ReDim Preserve list(UBound(list) + 1)
					list(UBound(list)) = "Invalid_string_refer_to_original_code"
				End If
				
				If UBound(list) > 0 Then
					TopItem = 1
					Do While True
						If UBound(list) = 1 And list(1) = "Invalid_string_refer_to_original_code" Then
							i = 1
						Else
							ReDim ListItemFlag(UBound(list))
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						End If
						
						If i = 0 Then
							'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
							Exit Do
						ElseIf list(i) = "Invalid_string_refer_to_original_code" Then 
							
							frmListBox.Hide()
							
							'Invalid_string_refer_to_original_code
							min_range = .Weapon(w).MinRange
							max_range = .WeaponMaxRange(w)
							
							'Invalid_string_refer_to_original_code
							If (max_range = 1 Or .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code")) And Not .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then
								AreaInReachable(SelectedUnit, max_range, .Party & "縺ｮ謨ｵ")
							ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
								AreaInCross(.X, .Y, min_range, max_range)
							ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
								AreaInWideCross(.X, .Y, min_range, max_range)
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								AreaInRange(.X, .Y, max_range, min_range, "縺吶∋縺ｦ")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								min_range = MaxLng(min_range, 1)
								AreaInRange(.X, .Y, max_range, min_range, "縺吶∋縺ｦ")
							ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
								AreaInMoveAction(SelectedUnit, max_range)
							Else
								AreaInRange(.X, .Y, max_range, min_range, .Party & "縺ｮ謨ｵ")
							End If
							Center(.X, .Y)
							MaskScreen()
							
							'Invalid_string_refer_to_original_code
							System.Windows.Forms.Application.DoEvents()
							WaitClickMode = True
							IsFormClicked = False
							
							'Invalid_string_refer_to_original_code
							Do Until IsFormClicked
								Sleep(25)
								System.Windows.Forms.Application.DoEvents()
								
								If IsRButtonPressed(True) Then
									Exit Do
								End If
							Loop 
							
							RedrawScreen()
							
							If UBound(list) = 1 And list(i) = "Invalid_string_refer_to_original_code" Then
								Exit Do
							End If
						Else
							'Invalid_string_refer_to_original_code
							frmListBox.Hide()
							AttributeHelp(SelectedUnit, LIndex(list(i), 1), w)
						End If
					Loop 
				End If
			End With
		Loop 
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			a = AbilityListBox(SelectedUnit, Term("Invalid_string_refer_to_original_code", SelectedUnit) & "荳隕ｧ", "荳隕ｧ")
			SelectedAbility = a
			
			If SelectedAbility <= 0 Then
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				If AutoMoveCursor Then
					RestoreCursorPos()
				End If
				frmListBox.Hide()
				UnlockGUI()
				CommandState = "Invalid_string_refer_to_original_code"
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			ReDim list(0)
			i = 0
			With SelectedUnit
				aclass = .Ability(a).Class_Renamed
				
				Do While i <= Len(aclass)
					i = i + 1
					buf = GetClassBundle(aclass, i)
					atype = ""
					alevel = ""
					
					'Invalid_string_refer_to_original_code
					If buf = "|" Then
						Exit Do
					End If
					
					'Invalid_string_refer_to_original_code
					If Mid(aclass, i, 1) = "Invalid_string_refer_to_original_code" Then
						i = i + 1
						buf = buf & Mid(aclass, i, 1)
					End If
					
					'Invalid_string_refer_to_original_code
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
					
					'螻樊ｧ縺ｮ蜷咲ｧｰ
					atype = AttributeName(SelectedUnit, buf, True)
					If Len(atype) > 0 Then
						ReDim Preserve list(UBound(list) + 1)
						
						If Len(alevel) > 0 Then
							list(UBound(list)) = RightPaddedString(buf & "L" & alevel, 8) & atype & "繝ｬ繝吶Ν" & StrConv(alevel, VbStrConv.Wide)
						Else
							list(UBound(list)) = RightPaddedString(buf, 8) & atype
						End If
					End If
				Loop 
				
				If MapFileName <> "" Then
					ReDim Preserve list(UBound(list) + 1)
					list(UBound(list)) = "Invalid_string_refer_to_original_code"
				End If
				
				If UBound(list) > 0 Then
					TopItem = 1
					Do While True
						If UBound(list) = 1 And list(1) = "Invalid_string_refer_to_original_code" Then
							i = 1
						Else
							ReDim ListItemFlag(UBound(list))
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						End If
						
						If i = 0 Then
							'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
							Exit Do
						ElseIf list(i) = "Invalid_string_refer_to_original_code" Then 
							
							frmListBox.Hide()
							
							'Invalid_string_refer_to_original_code
							min_range = .AbilityMinRange(a)
							max_range = .AbilityMaxRange(a)
							
							'Invalid_string_refer_to_original_code
							If (max_range = 1 Or .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code")) And Not .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then
								AreaInReachable(SelectedUnit, max_range, "縺吶∋縺ｦ")
							ElseIf .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then 
								AreaInCross(.X, .Y, min_range, max_range)
							ElseIf .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then 
								AreaInWideCross(.X, .Y, min_range, max_range)
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								min_range = MaxLng(min_range, 1)
								AreaInRange(.X, .Y, max_range, min_range, "縺吶∋縺ｦ")
							ElseIf .IsAbilityClassifiedAs(a, "Invalid_string_refer_to_original_code") Then 
								AreaInMoveAction(SelectedUnit, max_range)
							Else
								AreaInRange(.X, .Y, max_range, min_range, "縺吶∋縺ｦ")
							End If
							Center(.X, .Y)
							MaskScreen()
							
							'Invalid_string_refer_to_original_code
							System.Windows.Forms.Application.DoEvents()
							WaitClickMode = True
							IsFormClicked = False
							
							'Invalid_string_refer_to_original_code
							Do Until IsFormClicked
								Sleep(25)
								System.Windows.Forms.Application.DoEvents()
								
								If IsRButtonPressed(True) Then
									Exit Do
								End If
							Loop 
							
							RedrawScreen()
							
							If UBound(list) = 1 And list(i) = "Invalid_string_refer_to_original_code" Then
								Exit Do
							End If
						Else
							'Invalid_string_refer_to_original_code
							frmListBox.Hide()
							AttributeHelp(SelectedUnit, LIndex(list(i), 1), a, True)
						End If
					Loop 
				End If
			End With
		Loop 
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub ShowAreaInSpeedCommand()
	Private Sub ShowAreaInSpeedCommand()
		' MOD END MARGE
		SelectedCommand = "Invalid_string_refer_to_original_code"
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		AreaInSpeed(SelectedUnit)
		Center(SelectedUnit.X, SelectedUnit.Y)
		MaskScreen()
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub ShowAreaInRangeCommand()
	Private Sub ShowAreaInRangeCommand()
		' MOD END MARGE
		Dim w, i, max_range As Short
		
		SelectedCommand = "Invalid_string_refer_to_original_code"
		
		' MOD START MARGE
		'    If MainWidth <> 15 Then
		If NewGUIMode Then
			' MOD END MARGE
			ClearUnitStatus()
		End If
		
		With SelectedUnit
			'Invalid_string_refer_to_original_code
			w = 0
			max_range = 0
			For i = 1 To .CountWeapon
				If .IsWeaponAvailable(i, "Invalid_string_refer_to_original_code") And Not .IsWeaponClassifiedAs(i, "Invalid_string_refer_to_original_code") Then
					If .WeaponMaxRange(i) > max_range Then
						w = i
						max_range = .WeaponMaxRange(i)
					End If
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			AreaInRange(.X, .Y, max_range, 1, .Party & "縺ｮ謨ｵ")
			
			'Invalid_string_refer_to_original_code
			Center(.X, .Y)
			MaskScreen()
		End With
		
		CommandState = "Invalid_string_refer_to_original_code"
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub WaitCommand(Optional ByVal WithoutAction As Boolean)
	'Invalid_string_refer_to_original_code
	Private Sub WaitCommand(Optional ByVal WithoutAction As Boolean = False)
		' MOD END MARGE
		Dim p As Pilot
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		
		'Invalid_string_refer_to_original_code
		If SelectedUnit.CountPilot = 0 Then
			CommandState = "Invalid_string_refer_to_original_code"
			RedrawScreen()
			ClearUnitStatus()
			Exit Sub
		End If
		
		If Not WithoutAction Then
			'谿九ｊ陦悟虚謨ｰ繧呈ｸ帛ｰ代＆縺帙ｋ
			SelectedUnit.UseAction()
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			SelectedUnit.RemoveSpecialPowerInEffect("Invalid_string_refer_to_original_code")
		End If
		'End If
		
		CommandState = "Invalid_string_refer_to_original_code"
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.Update()
		PList.UpdateSupportMod(SelectedUnit)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		RedrawScreen()
		ClearUnitStatus()
		Exit Sub
		'End If
		
		LockGUI()
		
		RedrawScreen()
		
		p = SelectedUnit.Pilot(1)
		
		'Invalid_string_refer_to_original_code
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
				HandleEvent("謗･隗ｦ", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
				'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				SelectedTarget = Nothing
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				RedrawScreen()
				ClearUnitStatus()
				UnlockGUI()
				Exit Sub
			End If
			'End If
		Next 
		
		'Invalid_string_refer_to_original_code
		HandleEvent("騾ｲ蜈･", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
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
		
		'Invalid_string_refer_to_original_code
		HandleEvent("Invalid_string_refer_to_original_code")
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
			'Invalid_string_refer_to_original_code
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		SelectedUnit.CurrentForm.CheckAutoHyperMode()
		SelectedUnit.CurrentForm.CheckAutoNormalMode()
		
		If IsPictureVisible Or IsCursorVisible Then
			RedrawScreen()
		End If
		
		UnlockGUI()
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		DisplayUnitStatus(SelectedUnit)
		ClearUnitStatus()
		'End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub MapCommand(ByVal idx As Short)
		CommandState = "Invalid_string_refer_to_original_code"
		
		Select Case idx
			Case EndTurnCmdID 'Invalid_string_refer_to_original_code
				If ViewMode Then
					ViewMode = False
					Exit Sub
				End If
				EndTurnCommand()
			Case DumpCmdID '荳ｭ譁ｭ
				DumpCommand()
			Case UnitListCmdID '驛ｨ髫願｡ｨ
				UnitListCommand()
			Case SearchSpecialPowerCmdID '繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ讀懃ｴ｢
				SearchSpecialPowerCommand()
			Case GlobalMapCmdID 'Invalid_string_refer_to_original_code
				GlobalMapCommand()
			Case OperationObjectCmdID 'Invalid_string_refer_to_original_code
				LockGUI()
				HandleEvent("蜍晏茜譚｡莉ｶ")
				RedrawScreen()
				UnlockGUI()
			Case MapCommand1CmdID To MapCommand10CmdID 'Invalid_string_refer_to_original_code
				LockGUI()
				HandleEvent(MapCommandLabelList(idx - MapCommand1CmdID + 1))
				UnlockGUI()
			Case AutoDefenseCmdID 'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = Not MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked
				'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
					WriteIni("Option", "AutoDefense", "On")
				Else
					WriteIni("Option", "AutoDefense", "Off")
				End If
			Case ConfigurationCmdID '險ｭ螳壼､画峩
				'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
				Load(frmConfiguration)
				frmConfiguration.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(frmConfiguration.Width)) / 2)
				frmConfiguration.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(frmConfiguration.Height)) / 3)
				frmConfiguration.ShowDialog()
				frmConfiguration.Close()
				'UPGRADE_NOTE: オブジェクト frmConfiguration をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				frmConfiguration = Nothing
			Case RestartCmdID 'Invalid_string_refer_to_original_code
				RestartCommand()
			Case QuickLoadCmdID 'Invalid_string_refer_to_original_code
				QuickLoadCommand()
			Case QuickSaveCmdID 'Invalid_string_refer_to_original_code
				QuickSaveCommand()
		End Select
		IsScenarioFinished = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub EndTurnCommand()
	Private Sub EndTurnCommand()
		' MOD END MARGE
		Dim num As Short
		Dim ret As Short
		Dim u As Unit
		
		'Invalid_string_refer_to_original_code
		num = 0
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code_
				'And .Action > 0 _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				num = num + 1
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		If num > 0 Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		Else
			ret = 0
		End If
		
		Select Case ret
			Case 1
			Case 2
				Exit Sub
		End Select
		
		'Invalid_string_refer_to_original_code
		For	Each SelectedUnit In UList
			With SelectedUnit
				'Invalid_string_refer_to_original_code_
				'And .Action > 0 _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				HandleEvent("Invalid_string_refer_to_original_code")
				If IsScenarioFinished Then
					IsScenarioFinished = False
					Exit Sub
				End If
				'End If
			End With
		Next SelectedUnit
		
		'Invalid_string_refer_to_original_code
		
		StartTurn("謨ｵ")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		StartTurn("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		StartTurn("Invalid_string_refer_to_original_code")
		If IsScenarioFinished Then
			IsScenarioFinished = False
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		StartTurn("蜻ｳ譁ｹ")
		IsScenarioFinished = False
	End Sub
	
	'繝ｦ繝九ャ繝井ｸ隕ｧ縺ｮ陦ｨ遉ｺ
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
		
		'Invalid_string_refer_to_original_code
		uparty = "蜻ｳ譁ｹ"
		sort_mode = "繝ｬ繝吶Ν"
		
Beginning: 
		
		'Invalid_string_refer_to_original_code
		ReDim list(1)
		ReDim id_list(1)
		list(1) = "笆ｽ髯｣蝟ｶ螟画峩繝ｻ荳ｦ縺ｹ譖ｿ縺遺命"
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code_
				'Or .IsConditionSatisfied("繝ｦ繝九ャ繝域ュ蝣ｱ髫阡ｽ") _
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextUnit
				'End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemFlag(UBound(list))
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'騾壼ｸｸ縺ｮ繝ｦ繝九ャ繝郁｡ｨ遉ｺ
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				list(UBound(list)) = RightPaddedString(.Nickname0, 33) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3) & " "
				list(UBound(list)) = RightPaddedString(.Nickname0, 23)
				If .MainPilot.Nickname0 = "Invalid_string_refer_to_original_code" Then
					'Invalid_string_refer_to_original_code
					list(UBound(list)) = RightPaddedString(list(UBound(list)) & "", 34) & LeftPaddedString("", 2)
				Else
					list(UBound(list)) = RightPaddedString(list(UBound(list)) & .MainPilot.Nickname, 34) & LeftPaddedString(VB6.Format(.MainPilot.Level), 2)
				End If
				list(UBound(list)) = RightPaddedString(list(UBound(list)), 37)
				'End If
				If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
					list(UBound(list)) = list(UBound(list)) & "?????/????? ???/???"
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.HP), 5) & "/" & LeftPaddedString(VB6.Format(.MaxHP), 5) & " " & LeftPaddedString(VB6.Format(.EN), 3) & "/" & LeftPaddedString(VB6.Format(.MaxEN), 3)
				End If
				'Invalid_string_refer_to_original_code
				pilot_status_mode = True
				With .MainPilot
					list(UBound(list)) = RightPaddedString(.Nickname, 21) & LeftPaddedString(VB6.Format(.Level), 3) & LeftPaddedString(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP), 9) & "  "
					'菴ｿ逕ｨ蜿ｯ閭ｽ縺ｪ繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ荳隕ｧ
					For i = 1 To .CountSpecialPower
						If .SP >= .SpecialPowerCost(.SpecialPower(i)) Then
							list(UBound(list)) = list(UBound(list)) & SPDList.Item(.SpecialPower(i)).ShortName
						End If
					Next 
				End With
				'End If
				
				If .Action = 0 Then
					list(UBound(list)) = list(UBound(list)) & "Invalid_string_refer_to_original_code"
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				list(UBound(list)) = list(UBound(list)) & "譬ｼ"
				'End If
				
				id_list(UBound(list)) = .ID
				ListItemFlag(UBound(list)) = False
				'End If
			End With
NextUnit: 
		Next u
		
SortList: 
		
		'Invalid_string_refer_to_original_code
		If InStr(sort_mode, "蜷咲ｧｰ") = 0 Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).HP
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).EN
						Next 
					Case "繝ｬ繝吶Ν", "Invalid_string_refer_to_original_code"
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
			
			'繧ｭ繝ｼ繧剃ｽｿ縺｣縺ｦ荳ｦ縺ｹ謠帙∴
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
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "蜷咲ｧｰ", "繝ｦ繝九ャ繝亥錐遘ｰ"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "Invalid_string_refer_to_original_code"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'繧ｭ繝ｼ繧剃ｽｿ縺｣縺ｦ荳ｦ縺ｹ謠帙∴
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
		
		'繝ｪ繧ｹ繝医ｒ陦ｨ遉ｺ
		If pilot_status_mode Then
			ret = ListBox(uparty & "Invalid_string_refer_to_original_code", list, "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", Nothing, 4) & "  " & Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ"), "騾｣邯夊｡ｨ遉ｺ")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			ret = ListBox(uparty & "繝ｦ繝九ャ繝井ｸ隕ｧ", list, "繝ｦ繝九ャ繝亥錐                        Lv     " & Term("Invalid_string_refer_to_original_code", Nothing, 8) & Term("Invalid_string_refer_to_original_code"), "騾｣邯夊｡ｨ遉ｺ")
		Else
			ret = ListBox(uparty & "繝ｦ繝九ャ繝井ｸ隕ｧ", list, "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", Nothing, 8) & Term("Invalid_string_refer_to_original_code"), "騾｣邯夊｡ｨ遉ｺ")
		End If
		
		Select Case ret
			Case 0
				'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
				frmListBox.Hide()
				ReduceListBoxHeight()
				EnlargeListBoxWidth()
				ReDim ListItemID(0)
				UnlockGUI()
				Exit Sub
			Case 1
				'陦ｨ遉ｺ縺吶ｋ髯｣蝟ｶ
				ReDim mode_list(4)
				mode_list(1) = "蜻ｳ譁ｹ荳隕ｧ"
				mode_list(2) = "Invalid_string_refer_to_original_code"
				mode_list(3) = "謨ｵ荳隕ｧ"
				mode_list(4) = "荳ｭ遶倶ｸ隕ｧ"
				
				'Invalid_string_refer_to_original_code
				If pilot_status_mode Then
					ReDim Preserve mode_list(7)
					mode_list(5) = "Invalid_string_refer_to_original_code"
					mode_list(6) = "Invalid_string_refer_to_original_code"
					mode_list(7) = Term("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					ReDim Preserve mode_list(8)
					mode_list(5) = "Invalid_string_refer_to_original_code"
					mode_list(6) = "Invalid_string_refer_to_original_code"
					mode_list(7) = Term("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code"
					mode_list(8) = Term("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code"
				Else
					ReDim Preserve mode_list(9)
					mode_list(5) = Term("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code"
					mode_list(6) = Term("Invalid_string_refer_to_original_code") & "Invalid_string_refer_to_original_code"
					mode_list(7) = "Invalid_string_refer_to_original_code"
					mode_list(8) = "Invalid_string_refer_to_original_code"
					mode_list(9) = "Invalid_string_refer_to_original_code"
				End If
				ReDim ListItemID(UBound(mode_list))
				ReDim ListItemFlag(UBound(mode_list))
				
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				
				'髯｣蝟ｶ繧貞､画峩縺励※蜀崎｡ｨ遉ｺ
				If ret > 0 Then
					If Right(mode_list(ret), 2) = "荳隕ｧ" Then
						uparty = Left(mode_list(ret), Len(mode_list(ret)) - 2)
						GoTo Beginning
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						sort_mode = Left(mode_list(ret), Len(mode_list(ret)) - 5)
						GoTo SortList
					End If
				End If
				
				GoTo SortList
		End Select
		
		frmListBox.Hide()
		ReduceListBoxHeight()
		EnlargeListBoxWidth()
		
		'驕ｸ謚槭＆繧後◆繝ｦ繝九ャ繝医ｒ逕ｻ髱｢荳ｭ螟ｮ縺ｫ陦ｨ遉ｺ
		u = UList.Item(ListItemID(ret))
		Center(u.X, u.Y)
		RefreshScreen()
		DisplayUnitStatus(u)
		
		'Invalid_string_refer_to_original_code
		If AutoMoveCursor Then
			MoveCursorPos("Invalid_string_refer_to_original_code")
		End If
		
		ReDim ListItemID(0)
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		ReDim list(0)
		For i = 1 To SPDList.Count
			With SPDList.Item(i)
				If .ShortName <> "髱櫁｡ｨ遉ｺ" Then
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve strkey_list(UBound(list))
					list(UBound(list)) = .Name
					strkey_list(UBound(list)) = .KanaName
				End If
			End With
		Next 
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		ReDim flist(UBound(list))
		For i = 1 To UBound(list)
			flist(i) = True
			For	Each p In PList
				With p
					If .Party = "蜻ｳ譁ｹ" Then
						If Not .Unit_Renamed Is Nothing Then
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'Invalid_string_refer_to_original_code
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
					'End If
				End With
			Next p
		Next 
		
		Do While True
			ReDim ListItemFlag(UBound(list))
			ReDim ListItemComment(UBound(list))
			ReDim id_list(UBound(list))
			ReDim strkey_list(UBound(list))
			
			'Invalid_string_refer_to_original_code
			For i = 1 To UBound(ListItemFlag)
				ListItemFlag(i) = flist(i)
			Next 
			
			'Invalid_string_refer_to_original_code
			For i = 1 To UBound(ListItemComment)
				ListItemComment(i) = SPDList.Item(list(i)).Comment
			Next 
			
			'Invalid_string_refer_to_original_code
			TopItem = 1
			ret = MultiColumnListBox(Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ") & "讀懃ｴ｢", list, True)
			If ret = 0 Then
				CancelCommand()
				UnlockGUI()
				Exit Sub
			End If
			SelectedSpecialPower = list(ret)
			
			'Invalid_string_refer_to_original_code
			ReDim list2(0)
			ReDim ListItemFlag(0)
			ReDim id_list(0)
			ReDim pid(0)
			For	Each p In PList
				With p
					'Invalid_string_refer_to_original_code
					If .Party <> "蜻ｳ譁ｹ" Then
						GoTo NextLoop
					End If
					If .Unit_Renamed Is Nothing Then
						GoTo NextLoop
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					GoTo NextLoop
					'End If
					If .Unit_Renamed.CountPilot > 0 Then
						If .ID = .Unit_Renamed.Pilot(1).ID And .ID <> .Unit_Renamed.MainPilot.ID Then
							'Invalid_string_refer_to_original_code
							GoTo NextLoop
						End If
					End If
					If Not .IsSpecialPowerAvailable(SelectedSpecialPower) Then
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
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
						list2(UBound(list2)) = list2(UBound(list2)) & " " & Term("Invalid_string_refer_to_original_code", p.Unit_Renamed) & "荳崎ｶｳ"
					End If
					If .Unit_Renamed.Action = 0 Then
						list2(UBound(list2)) = list2(UBound(list2)) & "Invalid_string_refer_to_original_code"
					End If
				End With
NextLoop: 
			Next p
			
			SelectedSpecialPower = ""
			
			'Invalid_string_refer_to_original_code
			TopItem = 1
			EnlargeListBoxHeight()
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& "  " & Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ"))
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'& "  " & Term("繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ"))
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			ReduceListBoxHeight()
			
			'Invalid_string_refer_to_original_code
			If ret > 0 Then
				With PList.Item(pid(ret))
					Center(.Unit_Renamed.X, .Unit_Renamed.Y)
					RefreshScreen()
					DisplayUnitStatus(.Unit_Renamed)
					
					'Invalid_string_refer_to_original_code
					If AutoMoveCursor Then
						MoveCursorPos("Invalid_string_refer_to_original_code")
					End If
				End With
				
				ReDim id_list(0)
				
				UnlockGUI()
				Exit Sub
			End If
		Loop 
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub RestartCommand()
	Private Sub RestartCommand()
		' MOD END MARGE
		Dim ret As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If ret = MsgBoxResult.Cancel Then
			Exit Sub
		End If
		
		LockGUI()
		
		RestoreData(ScenarioPath & "Invalid_string_refer_to_original_code", True)
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub QuickLoadCommand()
	Private Sub QuickLoadCommand()
		' MOD END MARGE
		Dim ret As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If ret = MsgBoxResult.Cancel Then
			Exit Sub
		End If
		
		LockGUI()
		
		RestoreData(ScenarioPath & "Invalid_string_refer_to_original_code", True)
		
		'Invalid_string_refer_to_original_code
		RedrawScreen()
		DisplayGlobalStatus()
		
		UnlockGUI()
	End Sub
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub QuickSaveCommand()
	Private Sub QuickSaveCommand()
		' MOD END MARGE
		
		LockGUI()
		
		'繝槭え繧ｹ繧ｫ繝ｼ繧ｽ繝ｫ繧堤よ凾險医↓
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'Invalid_string_refer_to_original_code
		DumpData(ScenarioPath & "Invalid_string_refer_to_original_code")
		
		UnlockGUI()
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	' MOD START MARGE
	'Public Sub DumpCommand()
	Private Sub DumpCommand()
		' MOD END MARGE
		Dim fname, save_path As String
		Dim ret, i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'vbOKCancel + vbQuestion, "荳ｭ譁ｭ")
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If ret = MsgBoxResult.Cancel Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		For i = 1 To Len(ScenarioFileName)
			If Mid(ScenarioFileName, Len(ScenarioFileName) - i + 1, 1) = "\" Then
				Exit For
			End If
		Next 
		fname = Mid(ScenarioFileName, Len(ScenarioFileName) - i + 2, i - 5)
		fname = fname & "繧剃ｸｭ譁ｭ.src"
		
		'Invalid_string_refer_to_original_code_
		'ScenarioPath, fname, _
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		If fname = "" Then
			'繧ｭ繝｣繝ｳ繧ｻ繝ｫ
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If InStr(fname, "\") > 0 Then
			save_path = Left(fname, InStr2(fname, "\"))
		End If
		'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
		If Dir(save_path) <> Dir(ScenarioPath) Then
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Exit Sub
		End If
		'End If
		
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor '繝槭え繧ｹ繧ｫ繝ｼ繧ｽ繝ｫ繧堤よ凾險医↓
		
		LockGUI()
		
		'Invalid_string_refer_to_original_code
		DumpData(fname)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		MainForm.Hide()
		
		'Invalid_string_refer_to_original_code
		ExitGame()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0).BackColor = &HC0C0C0
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0) = System.Drawing.Image.FromFile("")
			
			'Invalid_string_refer_to_original_code
			If MapWidth > MapHeight Then
				mwidth = 300
				mheight = 300 * MapHeight \ MapWidth
			Else
				mheight = 300
				mwidth = 300 * MapWidth \ MapHeight
			End If
			
			'Invalid_string_refer_to_original_code
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
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
						Else
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
						End If
						
						'Invalid_string_refer_to_original_code
						Select Case u.Area
							Case "遨ｺ荳ｭ"
								'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								pic.Line (xx, yy + 28) - (xx + 31, yy + 28), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
							Case "豌ｴ荳ｭ"
								'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								pic.Line (xx, yy + 3) - (xx + 31, yy + 3), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
							Case "蝨ｰ荳ｭ"
								'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								pic.Line (xx, yy + 28) - (xx + 31, yy + 28), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
								'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								pic.Line (xx, yy + 3) - (xx + 31, yy + 3), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
							Case "Invalid_string_refer_to_original_code"
								If TerrainClass(i, j) = "譛磯擇" Then
									'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									pic.Line (xx, yy + 28) - (xx + 31, yy + 28), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
								End If
						End Select
					End If
					'End If
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			smode = GetStretchBltMode(.picMain(0).hDC)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = SetStretchBltMode(.picMain(0).hDC, STRETCH_DELETESCANS)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = StretchBlt(.picMain(0).hDC, (MainPWidth - mwidth) \ 2, (MainPHeight - mheight) \ 2, mwidth, mheight, pic.hDC, 0, 0, MapPWidth, MapPHeight, SRCCOPY)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = SetStretchBltMode(.picMain(0).hDC, smode)
			
			'Invalid_string_refer_to_original_code
			With pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(32)
				.Height = VB6.TwipsToPixelsY(32)
			End With
			
			'逕ｻ髱｢繧呈峩譁ｰ
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0).Refresh()
		End With
		
		'Invalid_string_refer_to_original_code
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If .Party0 = "蜻ｳ譁ｹ" Or .Party0 = "Invalid_string_refer_to_original_code" Then
					num = num + 1
				Else
					num2 = num2 + 1
				End If
				'End If
			End With
		Next u
		
		'Invalid_string_refer_to_original_code
		prev_mode = AutoMessageMode
		AutoMessageMode = False
		
		OpenMessageForm()
		DisplayMessage("Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code" & VB6.Format(num) & ";" & "Invalid_string_refer_to_original_code" & VB6.Format(num2))
		CloseMessageForm()
		
		AutoMessageMode = prev_mode
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).BackColor = &HFFFFFF
		RefreshScreen()
		
		UnlockGUI()
	End Sub
	
	
	'迴ｾ蝨ｨ縺ｮ驕ｸ謚樒憾豕√ｒ險倬鹸
	Public Sub SaveSelections()
		'Invalid_string_refer_to_original_code
		SelectionStackIndex = SelectionStackIndex + 1
		
		'Invalid_string_refer_to_original_code
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
		
		'遒ｺ菫昴＠縺滄伜沺縺ｫ驕ｸ謚樒憾豕√ｒ險倬鹸
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
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreSelections()
		'Invalid_string_refer_to_original_code
		If SelectionStackIndex = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		SelectionStackIndex = SelectionStackIndex - 1
		
		'Invalid_string_refer_to_original_code
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
	
	'驕ｸ謚槭ｒ蜈･繧梧崛縺医ｋ
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