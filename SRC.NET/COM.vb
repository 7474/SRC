Option Strict Off
Option Explicit On
Module COM
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�R���s���[�^�[�̎v�l���[�`���֘A���W���[��
	
	
	'�R���s���[�^�[�ɂ�郆�j�b�g����(�P�s��)
	Public Sub OperateUnit()
		Dim j, i, tmp As Short
		Dim w, tw As Short
		Dim wname, twname As String
		Dim u As Unit
		Dim tmp_w As Short
		Dim prob, dmg As Integer
		Dim tprob, tdmg As Integer
		Dim max_prob, max_dmg As Integer
		Dim max_range, min_range As Short
		Dim dst_x, dst_y As Short
		Dim new_x, new_y As Short
		Dim new_x0, new_y0 As Short
		Dim tx, ty As Short
		Dim new_locations_value As Short
		Dim distance As Short
		Dim def_mode As String
		Dim is_suiside As Boolean
		Dim moved, transfered As Boolean
		Dim mmode As String
		Dim searched_enemy, searched_nearest_enemy As Boolean
		Dim guard_unit_mode As Boolean
		Dim buf As String
		Dim partners() As Unit
		Dim prev_money, earnings As Integer
		Dim BGM As String
		Dim attack_target As Unit
		Dim attack_target_hp_ratio As Double
		Dim defense_target As Unit
		Dim defense_target_hp_ratio As Double
		Dim defense_target2 As Unit
		Dim defense_target2_hp_ratio As Double
		Dim support_attack_done As Boolean
		Dim w2 As Short
		Dim indirect_attack As Boolean
		Dim is_p_weapon As Boolean
		Dim took_action As Boolean
		
		SelectedUnitForEvent = SelectedUnit
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTarget = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTargetForEvent ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SelectedTargetForEvent = Nothing
		SelectedWeapon = 0
		SelectedTWeapon = 0
		SelectedAbility = 0
		SelectedUnitMoveCost = 0
		
		'�܂���Update
		SelectedUnit.Update()
		
		'�s���o���Ȃ���΂��̂܂܏I��
		If SelectedUnit.MaxAction = 0 Then
			Exit Sub
		End If
		
		'�x���Ă���H
		If SelectedUnit.IsConditionSatisfied("�x��") Then
			'�x��ɖZ�����c�c
			Exit Sub
		End If
		
		'�X�y�V�����p���[���g���H
		If IsOptionDefined("�G���j�b�g�X�y�V�����p���[�g�p") Or IsOptionDefined("�G���j�b�g���_�R�}���h�g�p") Then
			TrySpecialPower(SelectedUnit.MainPilot)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			For i = 2 To SelectedUnit.CountPilot
				TrySpecialPower(SelectedUnit.Pilot(i))
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			Next 
			For i = 1 To SelectedUnit.CountSupport
				TrySpecialPower(SelectedUnit.Support(i))
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			Next 
			If SelectedUnit.IsFeatureAvailable("�ǉ��T�|�[�g") Then
				TrySpecialPower(SelectedUnit.AdditionalSupport)
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			End If
		End If
		
		'�n�C�p�[���[�h���\�ł���΃n�C�p�[���[�h����
		TryHyperMode()
		
		'����Ȏv�l���[�h�̏ꍇ�̏���
		With SelectedUnit
			'�w�肳�ꂽ�n�_��ڎw���ꍇ
			If LLength(.Mode) = 2 Then
				dst_x = CShort(LIndex(.Mode, 1))
				dst_y = CShort(LIndex(.Mode, 2))
				If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
					GoTo Move
				End If
			End If
			
			'���S��������ꍇ
			If .Mode = "���S" Then
				GoTo Move
			End If
			
			'�v�l���[�h���u�p�C���b�g���v�̏ꍇ�̏���
			If Not PList.IsDefined(.Mode) Then
				GoTo TryBattleTransform
			End If
			If PList.Item(.Mode).Unit_Renamed Is Nothing Then
				GoTo TryBattleTransform
			End If
			If PList.Item(.Mode).Unit_Renamed.Status_Renamed <> "�o��" Then
				GoTo TryBattleTransform
			End If
			SelectedTarget = PList.Item(.Mode).Unit_Renamed
			AreaInSpeed(SelectedUnit)
			If Not .IsAlly(SelectedTarget) Then
				'���j�b�g���G�̏ꍇ�͂��̃��j�b�g��_��
				w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ��\")
				If w = 0 Then
					dst_x = SelectedTarget.X
					dst_y = SelectedTarget.Y
					GoTo Move
				End If
			Else
				'���j�b�g�������̏ꍇ�͂��̃��j�b�g����q
				w = 0
				distance = 1000
				dst_x = SelectedTarget.X
				dst_y = SelectedTarget.Y
				max_prob = 0
				max_dmg = 0
				
				'��q�Ώۂ��������Ă���ꍇ�͏C�����u���g��
				If TryFix(moved, SelectedTarget) Then
					GoTo EndOfOperation
				End If
				
				'��q�Ώۂ��������Ă���ꍇ�͉񕜃A�r���e�B���g��
				If TryHealing(moved, SelectedTarget) Then
					GoTo EndOfOperation
				End If
				
				'���̋Z�≇��h��������Ă���ꍇ�͂Ƃɂ�����q�Ώۂ�
				'�אڂ��邱�Ƃ�D�悷��
				If .MainPilot.IsSkillAvailable("����") Or .MainPilot.IsSkillAvailable("����h��") Then
					If System.Math.Abs(.X - dst_x) + System.Math.Abs(.Y - dst_y) > 1 Then
						GoTo Move
					End If
					guard_unit_mode = True
				End If
				If .IsFeatureAvailable("���̋Z") Then
					If System.Math.Abs(.X - dst_x) > 1 Or System.Math.Abs(.Y - dst_y) > 1 Then
						GoTo Move
					End If
					guard_unit_mode = True
				End If
				If guard_unit_mode Then
					'�����Ɨאڂ��Ă���̂Ŏ���̓G��r��
					'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					SelectedTarget = Nothing
					GoTo TryBattleTransform
				End If
				
				'��q���郆�j�b�g�����������j�b�g�����݂��邩�`�F�b�N
				For	Each u In UList
					With u
						If .Status_Renamed = "�o��" And SelectedUnit.IsEnemy(u) And System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) <= 5 Then
							tmp_w = SelectWeapon(SelectedUnit, u, "�ړ��\", prob, dmg)
						Else
							tmp_w = 0
						End If
						
						If tmp_w > 0 Then
							'���ЂƂȂ蓾�郆�j�b�g�ƔF��
							If distance > System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
								'�߂��ʒu�ɂ��郆�j�b�g��D��
								SelectedTarget = u
								w = tmp_w
								distance = System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y)
								max_prob = prob
								max_dmg = dmg
							ElseIf distance = System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then 
								'���܂łɌ����������j�b�g�ƈʒu���ς��Ȃ����
								'���댯�x���������j�b�g��D��
								If prob > max_prob And prob > 50 Then
									SelectedTarget = u
									w = tmp_w
									max_prob = prob
								ElseIf max_prob = 0 And dmg > max_dmg Then 
									SelectedTarget = u
									w = tmp_w
									max_dmg = dmg
								End If
							End If
						End If
					End With
				Next u
				
				If w = 0 Then
					'��q���郆�j�b�g�͈��S�B��q���郆�j�b�g�̋߂��ֈړ�����
					GoTo Move
				Else
					'��q���郆�j�b�g�����������j�b�g�ɍU��
					GoTo AttackEnemy
				End If
			End If
		End With
		
TryBattleTransform: 
		'�퓬�`�Ԃւ̕ό`���\�ł���Εό`
		If TryBattleTransform() Then
			transfered = True
			'���Ƀ^�[�Q�b�g��I�����Ă���ꍇ�͍U�����@���đI��
			If w > 0 Then
				w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ��\")
				If w = 0 Then
					'�ό`�̌��ʁA�U���ł��Ȃ��Ȃ��Ă��܂����c�c
					dst_x = SelectedTarget.X
					dst_y = SelectedTarget.Y
					GoTo Move
				End If
			End If
		End If
		
		'���s���Ԃ�����Ȃ��A�r���e�B������Ύg���Ă���
		TryInstantAbility()
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		With SelectedUnit
			If .HP = 0 Or .MaxAction = 0 Then
				GoTo EndOfOperation
			End If
		End With
		
		'���ɖڕW�����܂��Ă���΂��̖ڕW���U��
		If Not SelectedTarget Is Nothing Then
			GoTo AttackEnemy
		End If
		
		'�������\�ł���Ώ���
		If TrySummonning() Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
		'�C�����\�ł���ΏC�����u���g��
		If TryFix(moved) Then
			GoTo EndOfOperation
		End If
		
		'�}�b�v�^�񕜃A�r���e�B���g���H
		If TryMapHealing(moved) Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
		'�񕜃A�r���e�B���g���H
		If TryHealing(moved) Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
TryMapAttack: 
		'�}�b�v�U�����g���H
		If TryMapAttack(moved) Then
			GoTo EndOfOperation
		End If
		
SearchNearestEnemyWithinRange: 
		'�^�[�Q�b�g�ɂ��郆�j�b�g��T��
		With SelectedUnit
			AreaInSpeed(SelectedUnit)
			
			'��q���ׂ����j�b�g������ꍇ�͈ړ��͈͂�����
			If guard_unit_mode Then
				With PList.Item(.Mode).Unit_Renamed
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							If Not MaskData(i, j) Then
								If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > 1 Then
									MaskData(i, j) = True
								End If
							End If
						Next 
					Next 
				End With
			End If
			
			'�X�̃��j�b�g�ɑ΂��ă^�[�Q�b�g�ƂȂ蓾�邩����
			'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			SelectedTarget = Nothing
			w = 0
			max_prob = 0
			max_dmg = 0
			For	Each u In UList
				If u.Status_Renamed <> "�o��" Then
					GoTo NextLoop
				End If
				
				'�G���ǂ����𔻒�
				If .IsAlly(u) Then
					GoTo NextLoop
				End If
				
				'����̐w�c�݂̂�_���v�l���[�h�̏ꍇ
				Select Case .Mode
					Case "����"
						If u.Party <> "����" And u.Party <> "�m�o�b" Then
							GoTo NextLoop
						End If
					Case "�m�o�b", "�G", "����"
						If u.Party <> .Mode Then
							GoTo NextLoop
						End If
				End Select
				
				'�������g�ɂ͍U�����Ȃ�
				If SelectedUnit.CurrentForm Is u.CurrentForm Then
					GoTo NextLoop
				End If
				
				'�B��g��
				If u.IsUnderSpecialPowerEffect("�B��g") Then
					GoTo NextLoop
				End If
				
				'�X�e���X�̓G�͉���������͍U�����󂯂Ȃ�
				If u.IsFeatureAvailable("�X�e���X") And Not u.IsConditionSatisfied("�X�e���X����") And Not .IsFeatureAvailable("�X�e���X������") Then
					If u.IsFeatureLevelSpecified("�X�e���X") Then
						If System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) > u.FeatureLevel("�X�e���X") Then
							GoTo NextLoop
						End If
					Else
						If System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) > 3 Then
							GoTo NextLoop
						End If
					End If
				End If
				
				'�U���Ɏg�������I��
				If moved Then
					tmp_w = SelectWeapon(SelectedUnit, u, "�ړ���", prob, dmg)
				Else
					tmp_w = SelectWeapon(SelectedUnit, u, "�ړ��\", prob, dmg)
				End If
				If tmp_w <= 0 Then
					GoTo NextLoop
				End If
				
				'�T�|�[�g�K�[�h�����H
				If .MainPilot.TacticalTechnique >= 150 Then
					If Not u.LookForSupportGuard(SelectedUnit, tmp_w) Is Nothing Then
						'�����j�󂷂邱�Ƃ͏o���Ȃ�
						prob = 0
						'���z�I�Ƀ_���[�W�𔼌����Ĕ���
						dmg = dmg \ 2
					End If
				End If
				
				'�ԐڍU���H
				indirect_attack = .IsWeaponClassifiedAs(w, "��")
				
				'�������j�b�g�͎���������Ă��܂��悤�ȍU���͂����Ȃ�
				If .Party = "�m�o�b" And .IsFeatureAvailable("�������j�b�g") And Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("����m") And Not indirect_attack Then
					tw = SelectWeapon(u, SelectedUnit, "����", tprob, tdmg)
					If prob < 80 And tprob > prob Then
						GoTo NextLoop
					End If
				End If
				
				'�j��m����50%�ȏ�ł���Δj��m�����������j�b�g��D��
				'�����łȂ���΃_���[�W�̊��Ғl���������j�b�g��D��
				If prob > 50 Then
					'�d�v�ȃ��j�b�g�͗D�悵�ă^�[�Q�b�g�ɂ���
					If .MainPilot.TacticalTechnique >= 150 Then
						With u
							If .MainPilot.IsSkillAvailable("�w��") Or .MainPilot.IsSkillAvailable("�L��T�|�[�g") Or .IsFeatureAvailable("�C�����u") Then
								prob = 1.5 * prob
							Else
								'�񕜃A�r���e�B�������Ă���H
								For i = 1 To .CountAbility
									With .Ability(i)
										If .MaxRange > 0 Then
											If .CountEffect > 0 Then
												If .EffectType(1) = "��" Then
													prob = 1.5 * prob
													Exit For
												End If
											End If
										End If
									End With
								Next 
							End If
						End With
					End If
					
					If prob > max_prob Then
						SelectedTarget = u
						w = tmp_w
						max_prob = prob
					End If
				ElseIf max_prob = 0 Then 
					'����̔�����i���`�F�b�N
					tw = 0
					For i = 1 To u.CountWeapon
						If u.IsWeaponAvailable(i, "�ړ��O") And Not u.IsWeaponClassifiedAs(i, "�l") Then
							If Not moved And .Mode <> "�Œ�" And .IsWeaponClassifiedAs(tmp_w, "�ړ���U����") Then
								If u.WeaponMaxRange(i) >= .WeaponMaxRange(tmp_w) Then
									tw = i
									Exit For
								End If
							Else
								If u.WeaponMaxRange(i) >= System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) Then
									tw = i
									Exit For
								End If
							End If
						End If
					Next 
					
					'�ԐڍU���ɂ͔����s�\
					If indirect_attack Then
						tw = 0
					End If
					
					'�X�e�[�^�X�ُ�ɂ�蔽���s�\�H
					If u.MaxAction = 0 Or u.IsConditionSatisfied("�U���s�\") Then
						tw = 0
					End If
					
					'�������Ă��Ȃ��H
					If tw = 0 Then
						dmg = 1.5 * dmg
					End If
					
					'�d�v�ȃ��j�b�g�͗D�悵�ă^�[�Q�b�g�ɂ���
					If .MainPilot.TacticalTechnique >= 150 Then
						With u
							If .MainPilot.IsSkillAvailable("�w��") Or .MainPilot.IsSkillAvailable("�L��T�|�[�g") Or .IsFeatureAvailable("�C�����u") Then
								'���C���p�C���b�g���w����L��T�|�[�g��L���Ă�����
								'�C�����u�������Ă���Ώd�v���j�b�g�ƔF��
								dmg = 1.5 * dmg
							Else
								'�񕜃A�r���e�B�������Ă���ꍇ���d�v���j�b�g�ƔF��
								For i = 1 To .CountAbility
									With .Ability(i)
										If .MaxRange > 0 Then
											If .CountEffect > 0 Then
												If .EffectType(1) = "��" Then
													dmg = 1.5 * dmg
													Exit For
												End If
											End If
										End If
									End With
								Next 
							End If
						End With
					End If
					
					If dmg >= max_dmg Then
						'���݂̃��j�b�g���^�[�Q�b�g�ɐݒ�
						SelectedTarget = u
						w = tmp_w
						max_dmg = dmg
					End If
				End If
NextLoop: 
			Next u
			
			'�˒����ɓG�����Ȃ���Έړ��A�������͑ҋ@
			If SelectedTarget Is Nothing Then
				If .Mode = "�ҋ@" Or .Mode = "�Œ�" Or LLength(.Mode) = 2 Then
					GoTo EndOfOperation
				End If
				
				If moved Then
					'���Ɉړ��ς݂ł���΂����ŏI��
					GoTo EndOfOperation
				End If
				
				If searched_enemy Then
					'���ɍ��G�ς݂ł���΂����ŏI��
					GoTo EndOfOperation
				End If
				
				'��x���G���������Ƃ��L�^
				searched_enemy = True
				
				'��ԋ߂��G�̕��ֈړ�����
				GoTo SearchNearestEnemy
			End If
			searched_enemy = True
		End With
		
AttackEnemy: 
		'�G���U��
		
		'�G��Update
		SelectedTarget.Update()
		
		'�G�̈ʒu���L�^���Ă���
		tx = SelectedTarget.X
		ty = SelectedTarget.Y
		
		Dim list() As String
		Dim caption_msg As String
		Dim hit_prob, crit_prob As Short
		With SelectedUnit
			'�ړ���U���\�ȕ���̏ꍇ�͍U���O�Ɉړ����s��
			'���������̋Z�͈ړ���̈ʒu�ɂ���čU���ł��Ȃ��ꍇ������̂ŗ�O
			If .IsWeaponClassifiedAs(w, "�ړ���U����") And Not .IsWeaponClassifiedAs(w, "��") And Not moved And .Mode <> "�Œ�" Then
				'�ړ����Ȃ��Ă��U���o����ꍇ�͌��݈ʒu���f�t�H���g�̍U���ʒu�ɐݒ�
				If .IsTargetWithinRange(w, SelectedTarget) Then
					new_locations_value = TerrainEffectForHPRecover(.X, .Y) + TerrainEffectForENRecover(.X, .Y) + 100 * .LookForSupport(.X, .Y, True)
					If .Area <> "��" Then
						'�n�`�ɂ��h����ʂ͋󒆂ɂ���ꍇ�͎󂯂��Ȃ�
						new_locations_value = new_locations_value + TerrainEffectForHit(.X, .Y) + TerrainEffectForDamage(.X, .Y)
					End If
					new_x = .X
					new_y = .Y
				Else
					new_locations_value = -1000
					new_x = 0
					new_y = 0
				End If
				
				'�U������������ʒu�̂����A�����Ƃ��n�`���ʂ̍����ꏊ��T��
				'�n�`���ʂ������Ȃ�����Ƃ��߂��ꏊ��D��
				max_range = .WeaponMaxRange(w)
				min_range = .Weapon(w).MinRange
				For i = MaxLng(1, tx - max_range) To MinLng(tx + max_range, MapWidth)
					For j = MaxLng(1, ty - (max_range - System.Math.Abs(tx - i))) To MinLng(ty + (max_range - System.Math.Abs(tx - i)), MapHeight)
						If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) >= min_range Then
							tmp = TerrainEffectForHPRecover(i, j) + TerrainEffectForENRecover(i, j) + 100 * .LookForSupport(i, j, True)
							
							If .Area <> "��" Then
								'�n�`�ɂ��h����ʂ͋󒆂ɂ���ꍇ�͎󂯂��Ȃ�
								tmp = tmp + TerrainEffectForHit(i, j) + TerrainEffectForDamage(i, j)
								
								'�����͐����p���j�b�g�łȂ�����I�����Ȃ�
								If TerrainClass(i, j) = "��" Then
									If .IsTransAvailable("��") Then
										tmp = tmp + 100
									Else
										tmp = -1000
									End If
								End If
							End If
							
							'�����������ł���Β��������ŋ߂��ꏊ��I������
							tmp = tmp - System.Math.Sqrt(System.Math.Abs(.X - i) ^ 2 + System.Math.Abs(.Y - j) ^ 2)
							
							If new_locations_value < tmp Then
								new_locations_value = tmp
								new_x = i
								new_y = j
							End If
						End If
					Next 
				Next 
				
				If new_x = 0 And new_y = 0 Then
					'�U������������ʒu���Ȃ�
					If searched_nearest_enemy Then
						'���ɍ��G�ς݂ł���΂����ŏI��
						GoTo EndOfOperation
					End If
					GoTo SearchNearestEnemy
				End If
				
				'�������ʒu�Ɉړ�
				If new_x <> .X Or new_y <> .Y Then
					.Move(new_x, new_y)
					SelectedUnitMoveCost = TotalMoveCost(new_x, new_y)
					moved = True
					
					'�ړ��̂��߂d�m�؂�H
					If .EN = 0 Then
						If .MaxAction = 0 Then
							GoTo EndOfOperation
						End If
					End If
					
					'���̓}�b�v�U�����g����H
					If TryMapAttack(True) Then
						GoTo EndOfOperation
					End If
					
					'�ړ��̂��߂ɑI�����Ă������킪�g���Ȃ��Ȃ�����A���̋Z���g����
					'�悤�ɂȂ����肷�邱�Ƃ�����̂ŁA������ēx�I��
					w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ���")
					If w = 0 Then
						'�U���o���Ȃ��̂ōs���I��
						GoTo EndOfOperation
					End If
				End If
			End If
			
			'���j�b�g�𒆉��\��
			Center(.X, .Y)
			
			'�n�C���C�g�\�����s��
			If Not BattleAnimation Then
				'�˒��͈͂��n�C���C�g
				' MOD START �}�[�W
				'            AreaInRange .X, .Y, _
				''                .Weapon(w).MinRange, _
				''                .WeaponMaxRange(w), _
				''                "���"
				AreaInRange(.X, .Y, .WeaponMaxRange(w), .Weapon(w).MinRange, "���")
				' MOD END �}�[�W
			End If
			'���̋Z�̏ꍇ�̓p�[�g�i�[���n�C���C�g�\��
			If .IsWeaponClassifiedAs(w, "��") Then
				If .WeaponMaxRange(w) = 1 Then
					.CombinationPartner("����", w, partners, SelectedTarget.X, SelectedTarget.Y)
				Else
					.CombinationPartner("����", w, partners)
				End If
				If Not BattleAnimation Then
					For i = 1 To UBound(partners)
						With partners(i)
							MaskData(.X, .Y) = False
						End With
					Next 
				End If
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			If Not BattleAnimation Then
				'�������g�ƃ^�[�Q�b�g���n�C���C�g
				MaskData(.X, .Y) = False
				MaskData(SelectedTarget.X, SelectedTarget.Y) = False
				
				'�n�C���C�g�\�������{
				MaskScreen()
			Else
				'�퓬�A�j����\������ꍇ�̓n�C���C�g�\�����s��Ȃ�
				RefreshScreen()
			End If
			
			'�a�f�l��ύX
			If Not KeepEnemyBGM Then
				BGM = ""
				
				'�{�X�p�a�f�l�H
				If .IsFeatureAvailable("�a�f�l") And InStr(.MainPilot.Name, "(�U�R)") = 0 Then
					BGM = SearchMidiFile(.FeatureData("�a�f�l"))
				End If
				
				BossBGM = False
				If Len(BGM) > 0 Then
					'�{�X�p�a�f�l�����t����ꍇ
					ChangeBGM(BGM)
					BossBGM = True
				Else
					'�ʏ�̐퓬�a�f�l
					
					'�^�[�Q�b�g�͖����H
					If SelectedTarget.Party = "����" Or (SelectedTarget.Party = "�m�o�b" And .Party <> "�m�o�b") Then
						'�^�[�Q�b�g�������Ȃ̂Ń^�[�Q�b�g����D��
						If SelectedTarget.IsFeatureAvailable("�a�f�l") Then
							BGM = SearchMidiFile(SelectedTarget.FeatureData("�a�f�l"))
						End If
						If Len(BGM) = 0 Then
							BGM = SearchMidiFile(SelectedTarget.MainPilot.BGM)
						End If
					Else
						'�^�[�Q�b�g�������łȂ���΍U������D��
						If .IsFeatureAvailable("�a�f�l") Then
							BGM = SearchMidiFile(.FeatureData("�a�f�l"))
						End If
						If Len(BGM) = 0 Then
							BGM = SearchMidiFile(.MainPilot.BGM)
						End If
					End If
					If Len(BGM) = 0 Then
						BGM = BGMName("default")
					End If
					
					'�a�f�l��ύX
					ChangeBGM(BGM)
				End If
			End If
			
			'�ړ���U���\�H
			is_p_weapon = .IsWeaponClassifiedAs(w, "�ړ���U����")
			
			'�ԐڍU���H
			indirect_attack = .IsWeaponClassifiedAs(w, "��")
			
			'����̔�����i��ݒ�
			def_mode = ""
			UseSupportGuard = True
			If SelectedTarget.MaxAction = 0 Then
				'�s���s�\�̏ꍇ
				
				tw = -1
				'�`���[�W���܂��͏��Ղ��Ă���ꍇ�͎����I�ɖh��
				If SelectedTarget.Party = "����" And (SelectedTarget.IsFeatureAvailable("�`���[�W") Or SelectedTarget.IsFeatureAvailable("����")) Then
					def_mode = "�h��"
				End If
				
				'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ElseIf SelectedTarget.Party = "����" And Not MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then 
				'�������j�b�g�ɂ��蓮�������s���ꍇ
				
				'�퓬�A�j����\������ꍇ�ł��蓮�������ɂ̓n�C���C�g�\�����s��
				If BattleAnimation Then
					'�˒��͈͂��n�C���C�g
					' MOD START �}�[�W
					'                AreaInRange .X, .Y, _
					''                    .Weapon(w).MinRange, _
					''                    .WeaponMaxRange(w), _
					''                    "���"
					AreaInRange(.X, .Y, .WeaponMaxRange(w), .Weapon(w).MinRange, "���")
					' MOD END �}�[�W
					
					'���̋Z�̏ꍇ�̓p�[�g�i�[���n�C���C�g�\��
					If .IsWeaponClassifiedAs(w, "��") Then
						For i = 1 To UBound(partners)
							With partners(i)
								MaskData(.X, .Y) = False
							End With
						Next 
					End If
					
					'�������g�ƃ^�[�Q�b�g���n�C���C�g
					MaskData(.X, .Y) = False
					MaskData(SelectedTarget.X, SelectedTarget.Y) = False
					
					'�n�C���C�g�\�������{
					MaskScreen()
				End If
				
				
				hit_prob = .HitProbability(w, SelectedTarget, True)
				crit_prob = .CriticalProbability(w, SelectedTarget)
				caption_msg = "�����F" & .WeaponNickname(w) & " �U����=" & VB6.Format(.WeaponPower(w, ""))
				If Not IsOptionDefined("�\����������\��") Then
					caption_msg = caption_msg & " ������=" & VB6.Format(MinLng(hit_prob, 100)) & "���i" & crit_prob & "���j"
				End If
				
				ReDim list(3)
				
				If IsAbleToCounterAttack(SelectedTarget, SelectedUnit) And Not indirect_attack Then
					list(1) = "����"
				Else
					list(1) = "�����s�\"
				End If
				If Not IsOptionDefined("�\����������\��") Then
					list(2) = "�h��F��������" & VB6.Format(MinLng(hit_prob, 100)) & "���i" & .CriticalProbability(w, SelectedTarget, "�h��") & "���j"
					list(3) = "����F��������" & VB6.Format(MinLng(hit_prob \ 2, 100)) & "���i" & .CriticalProbability(w, SelectedTarget, "���") & "���j"
				Else
					list(2) = "�h��"
					list(3) = "���"
				End If
				
				'����h�䂪�󂯂���H
				SupportGuardUnit = SelectedTarget.LookForSupportGuard(SelectedUnit, w)
				If Not SupportGuardUnit Is Nothing Then
					ReDim Preserve list(4)
					If IsOptionDefined("���g��") Then
						list(4) = "����h��F�g�p���� (" & SupportGuardUnit.Nickname & ")"
					Else
						list(4) = "����h��F�g�p���� (" & SupportGuardUnit.Nickname & "/" & SupportGuardUnit.MainPilot.Nickname & ")"
					End If
					UseSupportGuard = True
				End If
				
				AddPartsToListBox()
				Do 
					'�U���ւ̑Ή���i��I��
					With SelectedTarget
						ReDim ListItemFlag(UBound(list))
						'�e�΍R��i���I���\������
						
						'�������I���\�H
						If list(1) = "����" Then
							ListItemFlag(1) = False
							tw = -1
						Else
							ListItemFlag(1) = True
							tw = 0
						End If
						
						'�h�䂪�I���\�H
						If .IsFeatureAvailable("�h��s��") Then
							ListItemFlag(2) = True
						Else
							ListItemFlag(2) = False
						End If
						
						'������I���\�H
						If .IsFeatureAvailable("���s��") Or .IsConditionSatisfied("�ړ��s�\") Then
							ListItemFlag(3) = True
						Else
							ListItemFlag(3) = False
						End If
						
						'�Ή���i��I��
						TopItem = 1
						i = ListBox(caption_msg, list, .Nickname0 & " " & .MainPilot.Nickname, "�A���\��,�J�[�\���ړ�")
					End With
					
					Select Case i
						Case 1
							'������I�������ꍇ�͔����Ɏg�������I��
							buf = "�����F" & .WeaponNickname(w) & " �U����=" & VB6.Format(.WeaponPower(w, ""))
							If Not IsOptionDefined("�\����������\��") Then
								buf = buf & " ������=" & VB6.Format(MinLng(hit_prob, 100)) & "���i" & crit_prob & "���j" & " �F "
							End If
							With SelectedTarget.MainPilot
								buf = buf & .Nickname & " " & Term("�i��", SelectedTarget) & VB6.Format(.Infight) & " "
								If .HasMana() Then
									buf = buf & Term("����", SelectedTarget) & VB6.Format(.Shooting)
								Else
									buf = buf & Term("�ˌ�", SelectedTarget) & VB6.Format(.Shooting)
								End If
							End With
							
							tw = WeaponListBox(SelectedTarget, buf, "����")
							
							If tw = 0 Then
								i = 0
							End If
						Case 2
							'�h���I������
							def_mode = "�h��"
						Case 3
							'�����I������
							def_mode = "���"
						Case 4
							'����h����g�p���邩�ǂ�����؂�ւ���
							UseSupportGuard = Not UseSupportGuard
							If UseSupportGuard Then
								list(4) = "����h��F�g�p���� ("
							Else
								list(4) = "����h��F�g�p���Ȃ� ("
							End If
							If IsOptionDefined("���g��") Then
								list(4) = list(4) & SupportGuardUnit.Nickname & ")"
							Else
								list(4) = list(4) & SupportGuardUnit.Nickname & "/" & SupportGuardUnit.MainPilot.Nickname & ")"
							End If
							i = 0
						Case Else
							'�����E�h��E����̑S�Ă��I���o���Ȃ��H
							If ListItemFlag(1) And ListItemFlag(2) And ListItemFlag(3) Then
								Exit Do
							End If
					End Select
				Loop While i = 0
				
				'������i�I���I��
				frmListBox.Hide()
				RemovePartsOnListBox()
				
				'�n�C���C�g�\��������
				If BattleAnimation Then
					RefreshScreen()
				End If
			Else
				'�R���s���[�^�[�����삷�郆�j�b�g�y�ю����������[�h�̏ꍇ
				
				'�����Ɏg�������I��
				tw = SelectWeapon(SelectedTarget, SelectedUnit, "����")
				If indirect_attack Then
					tw = 0
				End If
				
				'�h���I������H
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				def_mode = SelectDefense(SelectedUnit, w, SelectedTarget, tw)
				If def_mode <> "" Then
					tw = -1
				End If
			End If
		End With
		
		'�������j�b�g�̏ꍇ�͕���p�a�f�l�����t����
		If Not KeepEnemyBGM Then
			With SelectedTarget
				If .Party = "����" And tw > 0 And .IsFeatureAvailable("����a�f�l") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "����a�f�l" And LIndex(.FeatureData(i), 1) = .Weapon(tw).Name Then
							'����p�a�f�l���w�肳��Ă���
							BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
							If Len(BGM) > 0 Then
								'����p�a�f�l��MIDI�����������̂ła�f�l��ύX
								BossBGM = False
								ChangeBGM(BGM)
							End If
							Exit For
						End If
					Next 
				End If
			End With
		End If
		
		SelectedWeapon = w
		SelectedTWeapon = tw
		SelectedDefenseOption = def_mode
		
		wname = SelectedUnit.Weapon(w).Name
		SelectedWeaponName = wname
		If tw > 0 Then
			twname = SelectedTarget.Weapon(tw).Name
			SelectedTWeaponName = twname
		Else
			SelectedTWeaponName = ""
		End If
		
		' ADD START �}�[�W
		'�퓬�O�Ɉ�U�N���A
		'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SupportGuardUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: �I�u�W�F�N�g SupportGuardUnit2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		SupportGuardUnit2 = Nothing
		' ADD END �}�[�W
		
		'����̎g�p�C�x���g
		HandleEvent("�g�p", SelectedUnit.MainPilot.ID, wname)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		If tw > 0 Then
			twname = SelectedTarget.Weapon(tw).Name
			SaveSelections()
			SwapSelections()
			HandleEvent("�g�p", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
		End If
		
		'�U���C�x���g
		HandleEvent("�U��", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		'���b�Z�[�W�E�B���h�E���J��
		If Stage = "�m�o�b" Then
			OpenMessageForm(SelectedTarget, SelectedUnit)
		Else
			OpenMessageForm(SelectedUnit, SelectedTarget)
		End If
		
		'�C�x���g�p�ɐ퓬�ɎQ�����郆�j�b�g�̏����L�^���Ă���
		AttackUnit = SelectedUnit
		attack_target = SelectedUnit
		attack_target_hp_ratio = SelectedUnit.HP / SelectedUnit.MaxHP
		defense_target = SelectedTarget
		defense_target_hp_ratio = SelectedTarget.HP / SelectedTarget.MaxHP
		'UPGRADE_NOTE: �I�u�W�F�N�g defense_target2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		defense_target2 = Nothing
		' DEL START �}�[�W
		'    Set SupportAttackUnit = Nothing
		'    Set SupportGuardUnit = Nothing
		' DEL END �}�[�W
		
		'����̐搧�U���H
		With SelectedTarget
			' MOD START �}�[�W
			'        If tw > 0 And .MaxAction > 0 Then
			' tw > 0�̔����IsWeaponAvailable����
			If .MaxAction > 0 And .IsWeaponAvailable(tw, "�ړ��O") Then
				' MOD END �}�[�W
				If Not .IsWeaponClassifiedAs(tw, "��") Then
					If SelectedUnit.IsWeaponClassifiedAs(w, "��") Then
						def_mode = "�搧�U��"
						.Attack(tw, SelectedUnit, "�搧�U��", "")
						SelectedTarget = .CurrentForm
					ElseIf .IsWeaponClassifiedAs(tw, "��") Or .MainPilot.SkillLevel("��ǂ�") >= Dice(16) Or .IsUnderSpecialPowerEffect("�J�E���^�[") Then 
						def_mode = "�搧�U��"
						.Attack(tw, SelectedUnit, "�J�E���^�[", "")
						SelectedTarget = .CurrentForm
					ElseIf .MaxCounterAttack > .UsedCounterAttack Then 
						def_mode = "�搧�U��"
						.UsedCounterAttack = .UsedCounterAttack + 1
						.Attack(tw, SelectedUnit, "�J�E���^�[", "")
						SelectedTarget = .CurrentForm
					End If
					
					'�U�����̃��j�b�g�����΂�ꂽ�ꍇ�͍U�����̃^�[�Q�b�g���Đݒ�
					If Not SupportGuardUnit Is Nothing Then
						attack_target = SupportGuardUnit
						attack_target_hp_ratio = SupportGuardUnitHPRatio
					End If
				End If
			End If
		End With
		
		'�T�|�[�g�A�^�b�N�̃p�[�g�i�[��T��
		With SelectedUnit
			If .Status_Renamed = "�o��" And SelectedTarget.Status_Renamed = "�o��" Then
				SupportAttackUnit = .LookForSupportAttack(SelectedTarget)
				
				'���̋Z�ł̓T�|�[�g�A�^�b�N�s�\
				If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
					If .IsWeaponClassifiedAs(SelectedWeapon, "��") Then
						'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						SupportAttackUnit = Nothing
					End If
				End If
				
				'�������ꂽ�ꍇ
				If .IsConditionSatisfied("����") And .Master Is SelectedTarget Then
					'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					SupportAttackUnit = Nothing
				End If
				
				'�߈˂��ꂽ�ꍇ
				If .IsConditionSatisfied("�߈�") Then
					If .Master.Party = SelectedTarget.Party Then
						'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						SupportAttackUnit = Nothing
					End If
				End If
				
				'�x�炳�ꂽ�ꍇ
				If .IsConditionSatisfied("�x��") Then
					'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
					SupportAttackUnit = Nothing
				End If
			End If
		End With
		
		'�U���̎��{
		With SelectedUnit
			' MOD START �}�[�W
			'        If .Status = "�o��" _
			''            And .MaxAction(True) > 0 _
			''            And SelectedTarget.Status = "�o��" _
			''        Then
			If .Status_Renamed = "�o��" And .MaxAction(True) > 0 And Not .IsConditionSatisfied("�U���s�\") And SelectedTarget.Status_Renamed = "�o��" Then
				' MOD END �}�[�W
				'�܂�����͎g�p�\���H
				If w > .CountWeapon Then
					w = -1
				ElseIf wname <> .Weapon(w).Name Then 
					w = -1
				ElseIf moved Then 
					If Not .IsWeaponAvailable(w, "�ړ���") Then
						w = -1
					End If
				Else
					If Not .IsWeaponAvailable(w, "�ړ��O") Then
						w = -1
					End If
				End If
				If w > 0 Then
					If Not .IsTargetWithinRange(w, SelectedTarget) Then
						w = 0
					End If
				End If
				
				'�s���s�\�ȏꍇ
				If .MaxAction(True) = 0 Then
					w = -1
				End If
				
				'�������ꂽ�ꍇ
				If .IsConditionSatisfied("����") And .Master Is SelectedTarget Then
					w = -1
				End If
				
				'�߈˂��ꂽ�ꍇ
				If .IsConditionSatisfied("�߈�") Then
					If .Master.Party = SelectedTarget.Party Then
						w = -1
					End If
				End If
				
				'�x�炳�ꂽ�ꍇ
				If .IsConditionSatisfied("�x��") Then
					w = -1
				End If
				
				If w > 0 Then
					'�����U���H
					If .IsWeaponClassifiedAs(w, "��") Then
						is_suiside = True
					End If
					
					If Not SupportAttackUnit Is Nothing And .MaxSyncAttack > .UsedSyncAttack Then
						'��������U��
						.Attack(w, SelectedTarget, "����", def_mode)
					Else
						'�ʏ�U��
						.Attack(w, SelectedTarget, "", def_mode)
					End If
				ElseIf w = 0 Then 
					'�˒��O
					If .IsAnimationDefined("�˒��O") Then
						.PlayAnimation("�˒��O")
					Else
						.SpecialEffect("�˒��O")
					End If
					.PilotMessage("�˒��O")
				End If
			Else
				w = -1
			End If
			SelectedUnit = .CurrentForm
			
			'�h�䑤�̃��j�b�g�����΂�ꂽ�ꍇ��2�Ԗڂ̖h�䑤���j�b�g�Ƃ��ċL�^
			If Not SupportGuardUnit Is Nothing Then
				defense_target2 = SupportGuardUnit
				defense_target2_hp_ratio = SupportGuardUnitHPRatio
			End If
		End With
		
		'�����U��
		If Not SupportAttackUnit Is Nothing Then
			If SupportAttackUnit.Status_Renamed <> "�o��" Or SelectedUnit.Status_Renamed <> "�o��" Or SelectedTarget.Status_Renamed <> "�o��" Then
				'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				SupportAttackUnit = Nothing
			End If
		End If
		If Not SupportAttackUnit Is Nothing Then
			If SelectedUnit.MaxSyncAttack > SelectedUnit.UsedSyncAttack Then
				With SupportAttackUnit
					'�T�|�[�g�A�^�b�N�Ɏg�����������
					w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "�T�|�[�g�A�^�b�N")
					
					If w2 > 0 Then
						'�T�|�[�g�A�^�b�N�����{
						MaskData(.X, .Y) = False
						If Not BattleAnimation Then
							MaskScreen()
						End If
						If .IsAnimationDefined("�T�|�[�g�A�^�b�N�J�n") Then
							.PlayAnimation("�T�|�[�g�A�^�b�N�J�n")
						End If
						UpdateMessageForm(SelectedTarget, SupportAttackUnit)
						.Attack(w2, SelectedTarget, "��������U��", def_mode)
					End If
				End With
				
				'��n��
				With SupportAttackUnit.CurrentForm
					If w2 > 0 Then
						If .IsAnimationDefined("�T�|�[�g�A�^�b�N�I��") Then
							.PlayAnimation("�T�|�[�g�A�^�b�N�I��")
						End If
						
						'�T�|�[�g�A�^�b�N�̎c��񐔂����炷
						.UsedSupportAttack = .UsedSupportAttack + 1
						
						'��������U���̎c��񐔂����炷
						SelectedUnit.UsedSyncAttack = SelectedUnit.UsedSyncAttack + 1
					End If
				End With
				
				support_attack_done = True
				
				'�h�䑤�̃��j�b�g�����΂�ꂽ�ꍇ�͖{���̖h�䃆�j�b�g�f�[�^��
				'����ւ��ċL�^
				If Not SupportGuardUnit Is Nothing Then
					defense_target = SupportGuardUnit
					defense_target_hp_ratio = SupportGuardUnitHPRatio
				End If
			End If
		End If
		
		With SelectedTarget
			'�����̎��s
			If def_mode <> "�搧�U��" Then
				If .Status_Renamed = "�o��" And SelectedUnit.Status_Renamed = "�o��" Then
					'�܂�����͎g�p�\���H
					If tw > 0 Then
						If tw > .CountWeapon Then
							tw = -1
						ElseIf twname <> .Weapon(tw).Name Or Not .IsWeaponAvailable(tw, "�ړ��O") Then 
							tw = -1
						End If
					End If
					If tw > 0 Then
						If Not .IsTargetWithinRange(tw, SelectedUnit) Then
							'�G���˒��O�ɓ����Ă����畐����đI��
							tw = 0
						End If
					End If
					
					'�s���s�\�ȏꍇ
					If .MaxAction = 0 Then
						tw = -1
					End If
					
					'�������ꂽ�ꍇ
					If .IsConditionSatisfied("����") And .Master Is SelectedUnit Then
						tw = -1
					End If
					
					'�߈˂��ꂽ�ꍇ
					If .IsConditionSatisfied("�߈�") Then
						If .Master.Party = SelectedUnit.Party Then
							tw = -1
						End If
					End If
					
					'�x�炳�ꂽ�ꍇ
					If .IsConditionSatisfied("�x��") Then
						tw = -1
					End If
					
					If tw > 0 And def_mode = "" Then
						'���������{
						.Attack(tw, SelectedUnit, "", "")
						If .Status_Renamed = "���`��" Then
							SelectedTarget = .CurrentForm
						End If
						If SelectedUnit.Status_Renamed = "���`��" Then
							SelectedUnit = SelectedUnit.CurrentForm
						End If
						
						'�U�����̃��j�b�g�����΂�ꂽ�ꍇ�͍U�����̃^�[�Q�b�g���Đݒ�
						' MOD START �}�[�W
						'                    If Not SupportGuardUnit Is Nothing Then
						'                        Set attack_target = SupportGuardUnit
						'                        attack_target_hp_ratio = SupportGuardUnitHPRatio
						'                    End If
						If Not SupportGuardUnit2 Is Nothing Then
							attack_target = SupportGuardUnit2
							attack_target_hp_ratio = SupportGuardUnitHPRatio2
						End If
						' MOD END �}�[�W
					ElseIf tw = 0 And .X = tx And .Y = ty Then 
						'�����o���镐�킪�Ȃ������ꍇ�͎˒��O���b�Z�[�W��\��
						If .IsAnimationDefined("�˒��O") Then
							.PlayAnimation("�˒��O")
						Else
							.SpecialEffect("�˒��O")
						End If
						.PilotMessage("�˒��O")
					Else
						tw = -1
					End If
				Else
					tw = -1
				End If
			End If
		End With
		
		'�T�|�[�g�A�^�b�N
		If Not SupportAttackUnit Is Nothing Then
			If SupportAttackUnit.Status_Renamed <> "�o��" Or SelectedUnit.Status_Renamed <> "�o��" Or SelectedTarget.Status_Renamed <> "�o��" Or support_attack_done Then
				'UPGRADE_NOTE: �I�u�W�F�N�g SupportAttackUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				SupportAttackUnit = Nothing
			End If
		End If
		If Not SupportAttackUnit Is Nothing Then
			With SupportAttackUnit
				'�T�|�[�g�A�^�b�N�Ɏg�����������
				w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "�T�|�[�g�A�^�b�N")
				
				If w2 > 0 Then
					'�T�|�[�g�A�^�b�N�����{
					MaskData(.X, .Y) = False
					If Not BattleAnimation Then
						MaskScreen()
					End If
					If .IsAnimationDefined("�T�|�[�g�A�^�b�N�J�n") Then
						.PlayAnimation("�T�|�[�g�A�^�b�N�J�n")
					End If
					UpdateMessageForm(SelectedTarget, SupportAttackUnit)
					.Attack(w2, SelectedTarget, "����U��", def_mode)
				End If
			End With
			
			'��n��
			With SupportAttackUnit.CurrentForm
				If .IsAnimationDefined("�T�|�[�g�A�^�b�N�I��") Then
					.PlayAnimation("�T�|�[�g�A�^�b�N�I��")
				End If
				
				'�T�|�[�g�A�^�b�N�̎c��񐔂����炷
				If w2 > 0 Then
					.UsedSupportAttack = .UsedSupportAttack + 1
				End If
			End With
			
			'�h�䑤�̃��j�b�g�����΂�ꂽ�ꍇ�͖{���̖h�䃆�j�b�g�f�[�^��
			'����ւ��ċL�^
			If Not SupportGuardUnit Is Nothing Then
				defense_target = SupportGuardUnit
				defense_target_hp_ratio = SupportGuardUnitHPRatio
			End If
		End If
		
		'�W�I�������̏ꍇ�̌o���l�Ǝ����l������
		'(�W�I���������Ăяo�����������j�b�g�̏ꍇ��)
		SelectedUnit = SelectedUnit.CurrentForm
		Dim get_reward As Boolean
		With SelectedTarget
			
			'�o���l���������l���ł��邩����
			If .Party = "����" And .Status_Renamed = "�o��" Then
				get_reward = True
			ElseIf Not .Summoner Is Nothing Then 
				If .Summoner.Party = "����" And .Party0 = "�m�o�b" And .Status_Renamed = "�o��" And .IsFeatureAvailable("�������j�b�g") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�\��") Then
					get_reward = True
				End If
			End If
			
			If get_reward Then
				If SelectedUnit.Status_Renamed = "�j��" And Not is_suiside Then
					'�o���l���l��
					.GetExp(SelectedUnit, "�j��")
					
					'���݂̎������L�^
					prev_money = Money
					
					'�l�����鎑�����Z�o
					earnings = SelectedUnit.Value \ 2
					
					'�X�y�V�����p���[�ɂ��l����������
					If .IsUnderSpecialPowerEffect("�l����������") Then
						earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("�l����������"))
					End If
					
					'�p�C���b�g�\�͂ɂ��l����������
					If .IsSkillAvailable("�����l��") Then
						If Not .IsUnderSpecialPowerEffect("�l����������") Or IsOptionDefined("�������ʏd��") Then
							earnings = MinDbl(earnings * ((10 + .SkillLevel("�����l��", 5)) / 10), 999999999)
						End If
					End If
					
					'�������l��
					IncrMoney(earnings)
					
					If Money > prev_money Then
						DisplaySysMessage(VB6.Format(Money - prev_money) & "��" & Term("����", SelectedUnit) & "�𓾂��B")
					End If
				Else
					.GetExp(SelectedUnit, "�U��")
				End If
			End If
			
			'�X�y�V�����p���[�u�l�����������v�u�l���o���l�����v�̌��ʂ͂����ō폜����
			.RemoveSpecialPowerInEffect("�퓬�I��")
			If earnings > 0 Then
				.RemoveSpecialPowerInEffect("�G�j��")
			End If
		End With
		
		'�������Ăяo�����������j�b�g�̏ꍇ�͂m�o�b�ł��o���l�Ǝ������l��
		SelectedUnit = SelectedUnit.CurrentForm
		With SelectedUnit
			If Not .Summoner Is Nothing Then
				If .Summoner.Party = "����" And .Party0 = "�m�o�b" And .Status_Renamed = "�o��" And .IsFeatureAvailable("�������j�b�g") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�\��") Then
					If SelectedTarget.Status_Renamed = "�j��" Then
						'�^�[�Q�b�g��j�󂵂��ꍇ
						
						'�o���l���l��
						.GetExp(SelectedTarget, "�j��")
						
						'�l�����鎑�����Z�o
						earnings = SelectedTarget.Value \ 2
						
						'�X�y�V�����p���[�ɂ��l����������
						If .IsUnderSpecialPowerEffect("�l����������") Then
							earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("�l����������"))
						End If
						
						'�p�C���b�g�\�͂ɂ��l����������
						If .IsSkillAvailable("�����l��") Then
							If Not .IsUnderSpecialPowerEffect("�l����������") Or IsOptionDefined("�������ʏd��") Then
								earnings = earnings * (10 + .SkillLevel("�����l��", 5)) \ 10
							End If
						End If
						
						'�������l��
						IncrMoney(earnings)
						If earnings > 0 Then
							DisplaySysMessage(VB6.Format(earnings) & "��" & Term("����", SelectedTarget) & "�𓾂��B")
						End If
					Else
						'�^�[�Q�b�g��j��o���Ȃ������ꍇ
						
						'�o���l���l��
						.GetExp(SelectedTarget, "�U��")
					End If
				End If
			End If
			
			If .Status_Renamed = "�o��" Then
				'�X�y�V�����p���[���ʁu�G�j�󎞍čs���v
				If .IsUnderSpecialPowerEffect("�G�j�󎞍čs��") Then
					If SelectedTarget.Status_Renamed = "�j��" Then
						.UsedAction = .UsedAction - 1
					End If
				End If
				
				'�������Ԃ��u�퓬�I���v�̃X�y�V�����p���[���ʂ��폜
				.RemoveSpecialPowerInEffect("�퓬�I��")
				If earnings > 0 Then
					.RemoveSpecialPowerInEffect("�G�j��")
				End If
			End If
		End With
		
		CloseMessageForm()
		
		RedrawScreen()
		
		'��ԁ��f�[�^�X�V
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
		
		If SelectedWeapon <= 0 Then
			SelectedWeaponName = ""
		End If
		If SelectedTWeapon <= 0 Then
			SelectedTWeaponName = ""
		End If
		
		'�j�󁕑������C�x���g����
		
		'�U�����󂯂��U�������j�b�g
		With attack_target.CurrentForm
			If .CountPilot > 0 Then
				If .Status_Renamed = "�j��" Then
					HandleEvent("�j��", .MainPilot.ID)
				ElseIf .Status_Renamed = "�o��" And .HP / .MaxHP < attack_target_hp_ratio Then 
					HandleEvent("������", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
				End If
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			End If
		End With
		
		'�^�[�Q�b�g���̃C�x���g�������s�����߂Ƀ��j�b�g�̓���ւ����s��
		SaveSelections()
		SwapSelections()
		
		'�U�����󂯂��h�䑤���j�b�g
		With defense_target.CurrentForm
			If .CountPilot > 0 Then
				If .Status_Renamed = "�j��" Then
					HandleEvent("�j��", .MainPilot.ID)
				ElseIf .Status_Renamed = "�o��" And .HP / .MaxHP < defense_target_hp_ratio Then 
					HandleEvent("������", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
				End If
			End If
		End With
		
		If IsScenarioFinished Then
			RestoreSelections()
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'�U�����󂯂��h�䑤���j�b�g����2
		If Not defense_target2 Is Nothing Then
			If Not defense_target2.CurrentForm Is defense_target.CurrentForm Then
				With defense_target2.CurrentForm
					If .CountPilot > 0 Then
						If .Status_Renamed = "�j��" Then
							HandleEvent("�j��", .MainPilot.ID)
						ElseIf .Status_Renamed = "�o��" And .HP / .MaxHP < defense_target2_hp_ratio Then 
							HandleEvent("������", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
						End If
					End If
				End With
			End If
		End If
		
		'���ɖ߂�
		RestoreSelections()
		
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'����̎g�p��C�x���g
		If SelectedUnit.Status_Renamed = "�o��" And w > 0 Then
			HandleEvent("�g�p��", SelectedUnit.MainPilot.ID, wname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		If SelectedTarget.Status_Renamed = "�o��" And tw > 0 Then
			SaveSelections()
			SwapSelections()
			HandleEvent("�g�p��", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		'�U����C�x���g
		If SelectedUnit.Status_Renamed = "�o��" And SelectedTarget.Status_Renamed = "�o��" Then
			HandleEvent("�U����", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		'�����G���ړ����Ă���ΐi���C�x���g
		With SelectedTarget
			If .Status_Renamed = "�o��" Then
				If .X <> tx Or .Y <> ty Then
					HandleEvent("�i��", .MainPilot.ID, .X, .Y)
					If IsScenarioFinished Or IsCanceled Then
						ReDim SelectedPartners(0)
						Exit Sub
					End If
				End If
			End If
		End With
		
		'���̋Z�̃p�[�g�i�[�̍s���������炷
		If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		
		'�Ĉړ�
		If is_p_weapon And SelectedUnit.Status_Renamed = "�o��" Then
			If SelectedUnit.MainPilot.IsSkillAvailable("�V��") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
				'�i���C�x���g
				If SelectedUnitMoveCost > 0 Then
					HandleEvent("�i��", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
					If IsScenarioFinished Then
						Exit Sub
					End If
				End If
				
				'���j�b�g�����ɏo�����Ă��Ȃ��H
				If SelectedUnit.Status_Renamed <> "�o��" Then
					Exit Sub
				End If
				
				took_action = True
				
				AreaInSpeed(SelectedUnit)
				
				'�ڕW�n�_���ݒ肳��Ă���H
				If LLength((SelectedUnit.Mode)) = 2 Then
					dst_x = CShort(LIndex((SelectedUnit.Mode), 1))
					dst_y = CShort(LIndex((SelectedUnit.Mode), 2))
					If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
						GoTo Move
					End If
				End If
				
				'�����łȂ���Έ��S�ȏꏊ��
				SafetyPoint(SelectedUnit, dst_x, dst_y)
				
				GoTo Move
			End If
		End If
		
		'�s���I��
		GoTo EndOfOperation
		
SearchNearestEnemy: 
		
		'�����Ƃ��߂��ɂ���G��T��
		searched_nearest_enemy = True
		SelectedTarget = SearchNearestEnemy(SelectedUnit)
		
		'�^�[�Q�b�g��������Ȃ���΂�����߂ďI��
		If SelectedTarget Is Nothing Then
			GoTo EndOfOperation
		End If
		
		'����������^�[�Q�b�g�̈ʒu��ڕW�n�_�ɂ��Ĉړ�
		dst_x = SelectedTarget.X
		dst_y = SelectedTarget.Y
		
Move: 
		
		'�ڕW�n�_
		SelectedX = dst_x
		SelectedY = dst_y
		
		'�ړ��`�Ԃւ̕ό`���\�ł���Εό`
		If Not transfered Then
			If TryMoveTransform() Then
				transfered = True
			End If
		End If
		
		With SelectedUnit
			'�ړ��\�͈͂�ݒ�
			
			'�e���|�[�g�\�͂��g����ꍇ�͗D��I�Ɏg�p
			If LLength(.FeatureData("�e���|�[�g")) = 2 Then
				tmp = CShort(LIndex(.FeatureData("�e���|�[�g"), 2))
			Else
				tmp = 40
			End If
			If .IsFeatureAvailable("�e���|�[�g") And (.EN > 10 * tmp Or .EN - tmp > .MaxEN \ 2) And SelectedUnitMoveCost = 0 Then
				mmode = "�e���|�[�g"
				.EN = .EN - tmp
				AreaInTeleport(SelectedUnit)
				GoTo MoveAreaSelected
			End If
			
			'�W�����v�\�͂��g���H
			If LLength(.FeatureData("�W�����v")) = 2 Then
				tmp = CShort(LIndex(.FeatureData("�W�����v"), 2))
			Else
				tmp = 0
			End If
			If .IsFeatureAvailable("�W�����v") And .Area <> "��" And .Area <> "�F��" And (.EN > 10 * tmp Or .EN - tmp > .MaxEN \ 2) And SelectedUnitMoveCost = 0 Then
				mmode = "�W�����v"
				.EN = .EN - tmp
				AreaInSpeed(SelectedUnit, True)
				GoTo MoveAreaSelected
			End If
			
			'�ʏ�ړ�
			mmode = ""
			AreaInSpeed(SelectedUnit)
			
MoveAreaSelected: 
			
			'��q���ׂ����j�b�g������ꍇ�͓�����͈͂�����
			If guard_unit_mode Then
				With PList.Item(.Mode).Unit_Renamed
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							If Not MaskData(i, j) Then
								If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > 1 Then
									MaskData(i, j) = True
								End If
							End If
						Next 
					Next 
				End With
			End If
			
			If .Mode = "���S" Then
				'�ړ��\�͈͓��œG����ł������ꏊ������
				SafetyPoint(SelectedUnit, dst_x, dst_y)
				new_x = dst_x
				new_y = dst_y
			ElseIf .IsConditionSatisfied("����") Then 
				'�ړ��\�͈͓����烉���_���ɍs�����I��
				dst_x = .X + Dice(.Speed + 1) - Dice(.Speed + 1)
				dst_y = .Y + Dice(.Speed + 1) - Dice(.Speed + 1)
				NearestPoint(SelectedUnit, dst_x, dst_y, new_x, new_y)
			Else
				'�ړ��\�͈͓��ňړ��ړI�n�ɍł��߂��ꏊ������
				NearestPoint(SelectedUnit, dst_x, dst_y, new_x0, new_y0)
				
				'�ړ��悪�댯�n�悩�ǂ������肷��
				tmp = System.Math.Abs(dst_x - new_x0) + System.Math.Abs(dst_y - new_y0)
				If tmp <= 5 Then
					If Not MapDataForUnit(dst_x, dst_y) Is Nothing Then
						If Not .IsEnemy(MapDataForUnit(dst_x, dst_y)) Then
							tmp = 1000
						End If
					Else
						tmp = 1000
					End If
				End If
				
				new_x = new_x0
				new_y = new_y0
				new_locations_value = -1
				If tmp <= 5 Then
					'�ړ���͊댯�n��B���삵�����郆�j�b�g�Ɨאڂ��邩�A
					'�L���Ȓn�`���ʂ�������ꏊ��T���B
					For i = 0 To 12
						Select Case i
							Case 0
								tx = new_x0
								ty = new_y0
							Case 1
								tx = new_x0 + 1
								ty = new_y0
							Case 2
								tx = new_x0 - 1
								ty = new_y0
							Case 3
								tx = new_x0
								ty = new_y0 + 1
							Case 4
								tx = new_x0
								ty = new_y0 - 1
							Case 5
								tx = new_x0 + 1
								ty = new_y0 + 1
							Case 6
								tx = new_x0 - 1
								ty = new_y0 + 1
							Case 7
								tx = new_x0 + 1
								ty = new_y0 - 1
							Case 8
								tx = new_x0 - 1
								ty = new_y0 - 1
							Case 9
								tx = new_x0 + 2
								ty = new_y0
							Case 10
								tx = new_x0 - 2
								ty = new_y0
							Case 11
								tx = new_x0
								ty = new_y0 + 2
							Case 12
								tx = new_x0
								ty = new_y0 - 2
						End Select
						
						
						If 1 <= tx And tx <= MapWidth And 1 <= ty And ty <= MapHeight Then
							If Not MaskData(tx, ty) And System.Math.Abs(dst_x - tx) + System.Math.Abs(dst_y - ty) < System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
								tmp = TerrainEffectForHPRecover(tx, ty) + TerrainEffectForENRecover(tx, ty) + 100 * .LookForSupport(tx, ty)
								
								'�n�`�ɂ��h����ʂ͋󒆂ɂ���ꍇ�ɂ̂ݓK�p
								If .Area <> "��" Then
									tmp = tmp + TerrainEffectForHit(tx, ty) + TerrainEffectForDamage(tx, ty)
									'�����p���j�b�g�̏ꍇ�͐�����D��
									If TerrainClass(tx, ty) = "��" Then
										If .IsTransAvailable("��") Then
											tmp = tmp + 100
										End If
									End If
								End If
								
								If tmp > new_locations_value Then
									new_x = tx
									new_y = ty
									new_locations_value = tmp
								End If
							End If
						End If
					Next 
				Else
					'�ړ���͊댯�n��ł͂Ȃ��B
					'���삵�����郆�j�b�g������Ηאڂ���B
					For i = 0 To 4
						Select Case i
							Case 0
								tx = new_x0
								ty = new_y0
							Case 1
								tx = new_x0 + 1
								ty = new_y0
							Case 2
								tx = new_x0 - 1
								ty = new_y0
							Case 3
								tx = new_x0
								ty = new_y0 + 1
							Case 4
								tx = new_x0
								ty = new_y0 - 1
						End Select
						
						If 1 <= tx And tx <= MapWidth And 1 <= ty And ty <= MapHeight Then
							If Not MaskData(tx, ty) And System.Math.Abs(dst_x - tx) + System.Math.Abs(dst_y - ty) < System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
								tmp = .LookForSupport(tx, ty)
								If tmp > new_locations_value Then
									new_x = tx
									new_y = ty
									new_locations_value = tmp
								End If
							End If
						End If
					Next 
				End If
			End If
			
			If new_x < 1 Or MapWidth < new_x Or new_y < 1 Or MapHeight < new_y Then
				'�ړ��ł���ꏊ���Ȃ��c�c
				GoTo EndOfOperation
			End If
			
			'���������ꏊ�����܂���ꏊ�łȂ���΂����ֈړ�
			If .X <> new_x Or .Y <> new_y Then
				Select Case mmode
					Case "�e���|�[�g"
						If .IsMessageDefined("�e���|�[�g") Then
							OpenMessageForm()
							.PilotMessage("�e���|�[�g")
							CloseMessageForm()
						End If
						If .IsAnimationDefined("�e���|�[�g", .FeatureName("�e���|�[�g")) Then
							.PlayAnimation("�e���|�[�g", .FeatureName("�e���|�[�g"))
						ElseIf .IsSpecialEffectDefined("�e���|�[�g", .FeatureName("�e���|�[�g")) Then 
							.SpecialEffect("�e���|�[�g", .FeatureName("�e���|�[�g"))
						ElseIf BattleAnimation Then 
							ShowAnimation("�e���|�[�g���� Whiz.wav " & .FeatureName0("�e���|�[�g"))
						Else
							PlayWave("Whiz.wav")
						End If
						.Move(new_x, new_y, True, False, True)
						SelectedUnitMoveCost = 1000
						RedrawScreen()
					Case "�W�����v"
						If .IsMessageDefined("�W�����v") Then
							OpenMessageForm()
							.PilotMessage("�W�����v")
							CloseMessageForm()
						End If
						If .IsAnimationDefined("�W�����v", .FeatureName("�W�����v")) Then
							.PlayAnimation("�W�����v", .FeatureName("�W�����v"))
						ElseIf .IsSpecialEffectDefined("�W�����v", .FeatureName("�W�����v")) Then 
							.SpecialEffect("�W�����v", .FeatureName("�W�����v"))
						Else
							PlayWave("Swing.wav")
						End If
						.Move(new_x, new_y, True, False, True)
						SelectedUnitMoveCost = 1000
						RedrawScreen()
					Case Else
						'�ʏ�ړ�
						.Move(new_x, new_y)
						SelectedUnitMoveCost = TotalMoveCost(new_x, new_y)
				End Select
				moved = True
				
				'�v�l���[�h���u(X,Y)�Ɉړ��v�ŖړI�n�ɂ����ꍇ
				If LLength(.Mode) = 2 Then
					If .X = dst_x And .Y = dst_y Then
						.Mode = "�ҋ@"
					End If
				End If
			End If
			
			'�����łd�m�؂�H
			If .EN = 0 Then
				If .MaxAction = 0 Then
					GoTo EndOfOperation
				End If
			End If
			
			'��������Ă���ꍇ
			If .IsConditionSatisfied("����") Then
				GoTo EndOfOperation
			End If
			
			'�����Ă���ꍇ
			If .Mode = "���S" Then
				GoTo EndOfOperation
			End If
			
			'�v�l���[�h������̃^�[�Q�b�g��_���悤�ɐݒ肳��Ă���ꍇ
			If PList.IsDefined(.Mode) Then
				If PList.Item(.Mode).Unit_Renamed Is SelectedTarget Then
					If .IsEnemy(SelectedTarget) Then
						If moved Then
							w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ���")
						Else
							w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ��\")
						End If
						If w > 0 Then
							'�ړ��̌��ʁA�^�[�Q�b�g���˒����ɓ�����
							GoTo AttackEnemy
						End If
					Else
						'��q���郆�j�b�g�̂��Ƃ𗣂��ׂ��炸
						moved = True
					End If
				End If
			End If
			
			'����̒n�_�Ɉړ���
			If LLength(.Mode) = 2 Then
				If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
					If Not MapDataForUnit(dst_x, dst_y) Is Nothing Then
						SelectedTarget = MapDataForUnit(dst_x, dst_y)
						If .IsEnemy(SelectedTarget) Then
							'�ړ���̏ꏊ�ɂ���G��D�悵�Ĕr��
							If moved Then
								w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ���")
							Else
								w = SelectWeapon(SelectedUnit, SelectedTarget, "�ړ��\")
							End If
							If w > 0 Then
								GoTo AttackEnemy
							End If
						End If
						'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
						SelectedTarget = Nothing
					End If
				End If
			End If
			
			'���߂čU���̃V�[�P���X�Ɉڍs
			If Not took_action Then
				GoTo TryMapAttack
			End If
		End With
		
EndOfOperation: 
		
		'�s���I��
		
		ReDim SelectedPartners(0)
		
		If moved Then
			'�������Ԃ��u�ړ��v�̃X�y�V�����p���[���ʂ��폜
			SelectedUnit.RemoveSpecialPowerInEffect("�ړ�")
		End If
	End Sub
	
	'�n�C�p�[���[�h���\�ł���΃n�C�p�[���[�h����
	Private Sub TryHyperMode()
		Dim uname As String
		Dim u As Unit
		Dim fname, fdata As String
		Dim flevel As Double
		
		With SelectedUnit
			'�n�C�p�[���[�h�������Ă���H
			If Not .IsFeatureAvailable("�n�C�p�[���[�h") Then
				Exit Sub
			End If
			
			fname = .FeatureName("�n�C�p�[���[�h")
			flevel = .FeatureLevel("�n�C�p�[���[�h")
			fdata = .FeatureData("�n�C�p�[���[�h")
			
			'���������𖞂����H
			If .MainPilot.Morale < 100 + CShort(10# * flevel) And (InStr(fdata, "�C�͔���") > 0 Or .HP > .MaxHP \ 4) Then
				Exit Sub
			End If
			
			'�n�C�p�[���[�h���֎~����Ă���H
			If .IsConditionSatisfied("�`�ԌŒ�") Then
				Exit Sub
			End If
			If .IsConditionSatisfied("�@�̌Œ�") Then
				Exit Sub
			End If
			
			'�ϐg���E�\�̓R�s�[���̓n�C�p�[���[�h���g�p�ł��Ȃ�
			If .IsConditionSatisfied("�m�[�}�����[�h�t��") Then
				Exit Sub
			End If
			
			'�n�C�p�[���[�h��̌`�Ԃ𒲂ׂ�
			uname = LIndex(fdata, 2)
			u = .OtherForm(uname)
			
			'�n�C�p�[���[�h��̌`�Ԃ͎g�p�\�H
			If u.IsConditionSatisfied("�s���s�\") Or Not u.IsAbleToEnter(.X, .Y) Then
				Exit Sub
			End If
			
			'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
			If u.IsFeatureAvailable("�ǉ��p�C���b�g") Then
				If Not PList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
					PList.Add(u.FeatureData("�ǉ��p�C���b�g"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'�n�C�p�[���[�h���b�Z�[�W
			If .IsMessageDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�n�C�p�[���[�h(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("�n�C�p�[���[�h(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�n�C�p�[���[�h(" & fname & ")") Then 
				OpenMessageForm()
				.PilotMessage("�n�C�p�[���[�h(" & fname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�n�C�p�[���[�h") Then 
				OpenMessageForm()
				.PilotMessage("�n�C�p�[���[�h")
				CloseMessageForm()
			End If
			
			'�A�j���\��
			If .IsAnimationDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("�n�C�p�[���[�h(" & uname & ")") Then 
				.PlayAnimation("�n�C�p�[���[�h(" & uname & ")")
			ElseIf .IsAnimationDefined("�n�C�p�[���[�h(" & fname & ")") Then 
				.PlayAnimation("�n�C�p�[���[�h(" & fname & ")")
			ElseIf .IsAnimationDefined("�n�C�p�[���[�h") Then 
				.PlayAnimation("�n�C�p�[���[�h")
			ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h(" & uname & ")") Then 
				.SpecialEffect("�n�C�p�[���[�h(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h(" & fname & ")") Then 
				.SpecialEffect("�n�C�p�[���[�h(" & fname & ")")
			ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h") Then 
				.SpecialEffect("�n�C�p�[���[�h")
			End If
			
			'�n�C�p�[���[�h����
			.Transform(uname)
		End With
		
		'�n�C�p�[���[�h�C�x���g
		With u.CurrentForm
			HandleEvent("�n�C�p�[���[�h", .MainPilot.ID, .Name)
		End With
		
		'�n�C�p�[���[�h���m�[�}�����[�h�̎�������
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
	End Sub
	
	'�퓬�`�Ԃւ̕ό`���\�ł���Εό`����
	Public Function TryBattleTransform() As Boolean
		Dim uname As String
		Dim u As Unit
		Dim flag As Boolean
		Dim xx, yy As Short
		Dim i, j As Short
		
		With SelectedUnit
			'�ό`���\�H
			If Not .IsFeatureAvailable("�ό`") Or .IsConditionSatisfied("�`�ԌŒ�") Or .IsConditionSatisfied("�@�̌Œ�") Then
				Exit Function
			End If
			
			'�T�}�X�ȓ��ɓG�����邩�`�F�b�N
			If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
				'����ɓG�͂��Ȃ�
				Exit Function
			End If
			
			'�ł��^�����������`�Ԃɕό`
			u = SelectedUnit
			xx = .X
			yy = .Y
			For i = 2 To LLength(.FeatureData("�ό`"))
				uname = LIndex(.FeatureData("�ό`"), i)
				With .OtherForm(uname)
					'���̌`�Ԃɕό`�\�H
					If .IsConditionSatisfied("�s���s�\") Or Not .IsAbleToEnter(xx, yy) Then
						GoTo NextForm
					End If
					
					'�ʏ�`�Ԃ͎ア�`�Ԃł���Ƃ�������Ɋ�Â��A���̌`�Ԃ�
					'�m�[�}�����[�h�Ŏw�肳��Ă���ꍇ�Ζ�������
					If uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1) Then
						GoTo NextForm
					End If
					
					'�C�ł͐����������͋󒆓K���������j�b�g��D��
					Select Case TerrainClass(xx, yy)
						Case "��", "�[�C"
							'�����K���������j�b�g���ŗD��
							If InStr(.Data.Transportation, "��") > 0 Then
								If InStr(u.Data.Transportation, "��") = 0 Then
									u = .OtherForm(uname)
									GoTo NextForm
								End If
							End If
							If InStr(u.Data.Transportation, "��") > 0 Then
								If InStr(.Data.Transportation, "��") = 0 Then
									GoTo NextForm
								End If
							End If
							
							'���_�ŋ󒆓K�����j�b�g
							If InStr(.Data.Transportation, "��") > 0 Then
								If InStr(u.Data.Transportation, "��") = 0 Then
									u = .OtherForm(uname)
									GoTo NextForm
								End If
							End If
							If InStr(u.Data.Transportation, "��") > 0 Then
								If InStr(.Data.Transportation, "��") = 0 Then
									GoTo NextForm
								End If
							End If
					End Select
					
					'�^�������������̂�D��
					If .Data.Mobility < u.Data.Mobility Then
						GoTo NextForm
					ElseIf .Data.Mobility = u.Data.Mobility Then 
						'�^�����������Ȃ�U���͂��������̂�D��
						If .Data.CountWeapon = 0 Then
							'���̌`�Ԃ͕���������Ă��Ȃ�
							GoTo NextForm
						ElseIf u.Data.CountWeapon > 0 Then 
							If .Data.Weapon(.Data.CountWeapon).Power < u.Data.Weapon(u.Data.CountWeapon).Power Then
								GoTo NextForm
							ElseIf .Data.Weapon(.Data.CountWeapon).Power = u.Data.Weapon(u.Data.CountWeapon).Power Then 
								'�U���͂������Ȃ瑕�b���������̂�D��
								If .Data.Armor <= u.Data.Armor Then
									GoTo NextForm
								End If
							End If
						End If
					End If
				End With
				u = .OtherForm(uname)
NextForm: 
			Next 
			
			'���݂̌`�Ԃ��ł��퓬�ɓK���Ă���H
			If u Is SelectedUnit Then
				Exit Function
			End If
			
			'�`��u�ɕό`����
			uname = u.Name
			
			'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
			If u.IsFeatureAvailable("�ǉ��p�C���b�g") Then
				If Not PList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
					If Not PDList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
						ErrorMessage(uname & "�̒ǉ��p�C���b�g�u" & u.FeatureData("�ǉ��p�C���b�g") & "�v�̃f�[�^��������܂���")
						TerminateSRC()
					End If
					PList.Add(u.FeatureData("�ǉ��p�C���b�g"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'�ό`���b�Z�[�W
			If .IsMessageDefined("�ό`(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("�ό`(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�ό`(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("�ό`(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�ό`(" & .FeatureName("�ό`") & ")") Then 
				OpenMessageForm()
				.PilotMessage("�ό`(" & .FeatureName("�ό`") & ")")
				CloseMessageForm()
			End If
			
			'�A�j���\��
			If .IsAnimationDefined("�ό`(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("�ό`(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("�ό`(" & uname & ")") Then 
				.PlayAnimation("�ό`(" & uname & ")")
			ElseIf .IsAnimationDefined("�ό`(" & .FeatureName("�ό`") & ")") Then 
				.PlayAnimation("�ό`(" & .FeatureName("�ό`") & ")")
			ElseIf .IsSpecialEffectDefined("�ό`(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("�ό`(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("�ό`(" & uname & ")") Then 
				.SpecialEffect("�ό`(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("�ό`(" & .FeatureName("�ό`") & ")") Then 
				.SpecialEffect("�ό`(" & .FeatureName("�ό`") & ")")
			End If
			
			'�ό`
			.Transform(uname)
		End With
		
		'�ό`�C�x���g
		With u.CurrentForm
			HandleEvent("�ό`", .MainPilot.ID, .Name)
		End With
		
		'�n�C�p�[���[�h���m�[�}�����[�h�̎�������
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
		
		TryBattleTransform = True
	End Function
	
	'�ړ��`�Ԃւ̕ό`���\�ł���Εό`����
	Private Function TryMoveTransform() As Boolean
		Dim uname As String
		Dim u As Unit
		Dim xx, yy As Short
		Dim tx, ty As Short
		Dim speed1, speed2 As Short
		Dim i As Short
		
		With SelectedUnit
			'�ό`���\�H
			If Not .IsFeatureAvailable("�ό`") Or .IsConditionSatisfied("�`�ԌŒ�") Or .IsConditionSatisfied("�@�̌Œ�") Then
				Exit Function
			End If
			
			xx = .X
			yy = .Y
			
			'�n�`�Ɏז�����Ĉړ��ł��Ȃ��Ȃ�Ȃ������ׂ邽�߁A�ړI�n�̕����ɂ���
			'�אڂ���}�X�̍��W�𒲂ׂ�
			If System.Math.Abs(SelectedX - xx) > System.Math.Abs(SelectedY - yy) Then
				If SelectedX > xx Then
					tx = xx + 1
				Else
					tx = xx - 1
				End If
				ty = yy
			Else
				tx = xx
				If SelectedY > ty Then
					ty = yy + 1
				Else
					ty = yy - 1
				End If
			End If
			
			'�ł��ړ��͂������`�Ԃɕό`
			u = SelectedUnit
			For i = 2 To LLength(.FeatureData("�ό`"))
				uname = LIndex(.FeatureData("�ό`"), i)
				With .OtherForm(uname)
					'���̌`�Ԃɕό`�\�H
					If .IsConditionSatisfied("�s���s�\") Or Not .IsAbleToEnter(xx, yy) Then
						GoTo NextForm
					End If
					
					'�ړI�n���ʂɈړ��\�H
					If u.IsAbleToEnter(tx, ty) And Not .IsAbleToEnter(tx, ty) Then
						GoTo NextForm
					End If
					
					'�ړ��͂���������D��
					speed1 = .Data.Speed
					If .Data.IsFeatureAvailable("�e���|�[�g") Then
						speed1 = speed1 + .Data.FeatureLevel("�e���|�[�g") + 1
					End If
					If .Data.IsFeatureAvailable("�W�����v") Then
						speed1 = speed1 + .Data.FeatureLevel("�W�����v") + 1
					End If
					'�ړ��\�Ȓn�`�^�C�v���l��
					Select Case TerrainClass(xx, yy)
						Case "��", "�[�C"
							If InStr(.Data.Transportation, "��") > 0 Or InStr(.Data.Transportation, "��") > 0 Then
								speed1 = speed1 + 1
							End If
						Case "�F��", "����"
							'�F���≮���ł͍����o�Ȃ�
						Case Else
							If InStr(.Data.Transportation, "��") > 0 Then
								speed1 = speed1 + 1
							End If
					End Select
					
					speed2 = u.Data.Speed
					If u.Data.IsFeatureAvailable("�e���|�[�g") Then
						speed2 = speed2 + u.Data.FeatureLevel("�e���|�[�g") + 1
					End If
					If u.Data.IsFeatureAvailable("�W�����v") Then
						speed2 = speed2 + u.Data.FeatureLevel("�W�����v") + 1
					End If
					'�ړ��\�Ȓn�`�^�C�v���l��
					Select Case TerrainClass(xx, yy)
						Case "��", "�[�C"
							If InStr(u.Data.Transportation, "��") > 0 Or InStr(u.Data.Transportation, "��") > 0 Then
								speed2 = speed2 + 1
							End If
						Case "�F��", "����"
							'�F���≮���ł͍����o�Ȃ�
						Case Else
							If InStr(u.Data.Transportation, "��") > 0 Then
								speed2 = speed2 + 1
							End If
					End Select
					
					If speed2 > speed1 Then
						GoTo NextForm
					ElseIf speed2 = speed1 Then 
						'�ړ��͂������Ȃ瑕�b����������D��
						If u.Data.Armor >= .Data.Armor Then
							GoTo NextForm
						End If
					End If
				End With
				u = .OtherForm(uname)
NextForm: 
			Next 
			
			'���݂̌`�Ԃ��ł��ړ��ɓK���Ă���H
			If SelectedUnit Is u Then
				Exit Function
			End If
			
			'�`��u�ɕό`����
			uname = u.Name
			
			'�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
			If u.IsFeatureAvailable("�ǉ��p�C���b�g") Then
				If Not PList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
					If Not PDList.IsDefined(u.FeatureData("�ǉ��p�C���b�g")) Then
						ErrorMessage(uname & "�̒ǉ��p�C���b�g�u" & u.FeatureData("�ǉ��p�C���b�g") & "�v�̃f�[�^��������܂���")
						TerminateSRC()
					End If
					PList.Add(u.FeatureData("�ǉ��p�C���b�g"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'�ό`���b�Z�[�W
			If .IsMessageDefined("�ό`(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("�ό`(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�ό`(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("�ό`(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("�ό`(" & .FeatureName("�ό`") & ")") Then 
				OpenMessageForm()
				.PilotMessage("�ό`(" & .FeatureName("�ό`") & ")")
				CloseMessageForm()
			End If
			
			'�A�j���\��
			If .IsAnimationDefined("�ό`(" & .Name & "=>" & uname & ")") Then
				.PlayAnimation("�ό`(" & .Name & "=>" & uname & ")")
			ElseIf .IsAnimationDefined("�ό`(" & uname & ")") Then 
				.PlayAnimation("�ό`(" & uname & ")")
			ElseIf .IsAnimationDefined("�ό`(" & .FeatureName("�ό`") & ")") Then 
				.PlayAnimation("�ό`(" & .FeatureName("�ό`") & ")")
			ElseIf .IsSpecialEffectDefined("�ό`(" & .Name & "=>" & uname & ")") Then 
				.SpecialEffect("�ό`(" & .Name & "=>" & uname & ")")
			ElseIf .IsSpecialEffectDefined("�ό`(" & uname & ")") Then 
				.SpecialEffect("�ό`(" & uname & ")")
			ElseIf .IsSpecialEffectDefined("�ό`(" & .FeatureName("�ό`") & ")") Then 
				.SpecialEffect("�ό`(" & .FeatureName("�ό`") & ")")
			End If
			
			'�ό`
			.Transform(uname)
		End With
		
		'�ό`�C�x���g
		With u.CurrentForm
			HandleEvent("�ό`", .MainPilot.ID, .Name)
		End With
		
		'�n�C�p�[���[�h���m�[�}�����[�h�̎�������
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
		
		TryMoveTransform = True
	End Function
	
	'���s���Ԃ�K�v�Ƃ��Ȃ��A�r���e�B������Ύg���Ă���
	Public Sub TryInstantAbility()
		Dim i, j As Short
		Dim aname As String
		Dim partners() As Unit
		
		'�T�}�X�ȓ��ɓG�����邩�`�F�b�N
		If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
			'����ɓG�͂��Ȃ��̂ŃA�r���e�B�͎g��Ȃ�
			Exit Sub
		End If
		
		With SelectedUnit
			'���s���Ԃ�K�v�Ƃ��Ȃ��A�r���e�B��T��
			For i = 1 To .CountAbility
				'�g�p�\�����ʂ���H
				If Not .IsAbilityUseful(i, "�ړ��O") Then
					GoTo NextAbility
				End If
				
				'�d�m����������Ȃ��H
				If .AbilityENConsumption(i) > 0 Then
					If .AbilityENConsumption(i) >= .EN \ 2 Then
						GoTo NextAbility
					End If
				End If
				
				With .Ability(i)
					'���ȋ����̃A�r���e�B�݂̂��Ώ�
					If .MaxRange <> 0 Then
						GoTo NextAbility
					End If
					
					'���s���Ԃ�K�v�Ƃ��Ȃ��H
					For j = 1 To .CountEffect
						If .EffectType(j) = "�čs��" Then
							Exit For
						End If
					Next 
					If j > .CountEffect Then
						GoTo NextAbility
					End If
					
					'�����p�A�r���e�B�H
					For j = 1 To .CountEffect
						If .EffectType(j) = "���" Or .EffectType(j) = "�t��" Or .EffectType(j) = "����" Then
							'�����p�A�r���e�B����������
							SelectedAbility = i
							GoTo UseInstantAbility
						End If
					Next 
				End With
NextAbility: 
			Next 
			
			'�����ɗ��鎞�͎g�p�ł���A�r���e�B���Ȃ������ꍇ
			Exit Sub
			
UseInstantAbility: 
			
			'���̋Z�p�[�g�i�[�̐ݒ�
			If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
				.CombinationPartner("�A�r���e�B", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'�A�r���e�B�̎g�p�C�x���g
			HandleEvent("�g�p", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			
			'�A�r���e�B���g�p
			OpenMessageForm(SelectedUnit)
			.ExecuteAbility(SelectedAbility, SelectedUnit)
			CloseMessageForm()
			SelectedUnit = .CurrentForm
		End With
		
		'�A�r���e�B�̎g�p��C�x���g
		HandleEvent("�g�p��", SelectedUnit.MainPilot.ID, aname)
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'�����A�r���e�B�̔j��C�x���g
		If SelectedUnit.Status_Renamed = "�j��" Then
			HandleEvent("�j��", SelectedUnit.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		'�s����������Ă���
		SelectedUnit.UseAction()
		
		'���̋Z�̃p�[�g�i�[�̍s���������炷
		If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Sub
	
	'�������\�ł���Ώ�������
	Public Function TrySummonning() As Boolean
		Dim i, j As Short
		Dim aname As String
		Dim partners() As Unit
		
		With SelectedUnit
			'�����A�r���e�B������
			For i = 1 To .CountAbility
				If .IsAbilityAvailable(i, "�ړ��O") Then
					For j = 1 To .Ability(i).CountEffect
						If .Ability(i).EffectType(j) = "����" Then
							SelectedAbility = i
							GoTo UseSummonning
						End If
					Next 
				End If
			Next 
			
			'�g�p�\�ȏ����A�r���e�B�������Ă��Ȃ�����
			Exit Function
			
UseSummonning: 
			
			TrySummonning = True
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'�����A�r���e�B�̎g�p�C�x���g
			HandleEvent("�g�p", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Function
			End If
			
			'���̋Z�p�[�g�i�[�̐ݒ�
			If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
				.CombinationPartner("�A�r���e�B", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			'�����A�r���e�B���g�p
			OpenMessageForm(SelectedUnit)
			.ExecuteAbility(SelectedAbility, SelectedUnit)
			CloseMessageForm()
			SelectedUnit = .CurrentForm
		End With
		
		'�����A�r���e�B�̎g�p��C�x���g
		HandleEvent("�g�p��", SelectedUnit.MainPilot.ID, aname)
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Function
		End If
		
		'�����A�r���e�B�̔j��C�x���g
		If SelectedUnit.Status_Renamed = "�j��" Then
			HandleEvent("�j��", SelectedUnit.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'���̋Z�̃p�[�g�i�[�̍s���������炷
		If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Function
	
	'�}�b�v�^�񕜃A�r���e�B�g�p�Ɋւ��鏈��
	Public Function TryMapHealing(ByRef moved As Boolean) As Boolean
		Dim a As Short
		Dim apower As Integer
		Dim max_range, min_range As Short
		Dim xx, tx, ty, yy As Short
		Dim y1, x1, x2, y2 As Short
		Dim i, j As Short
		Dim num As Short
		Dim score, max_score As Integer
		Dim p As Pilot
		Dim t As Unit
		Dim partners() As Unit
		
		Dim tmp_num, tmp_score As Short
		Dim mlv As Short
		With SelectedUnit
			SelectedAbility = 0
			
			'����m��Ԃ̍ۂ͉񕜃A�r���e�B���g��Ȃ�
			If .IsConditionSatisfied("����m") Then
				Exit Function
			End If
			
			p = .MainPilot
			
			a = .CountAbility()
			Do While a > 0
				'�}�b�v�A�r���e�B���ǂ���
				If Not .IsAbilityClassifiedAs(a, "�l") Then
					GoTo NextAbility
				End If
				
				'�A�r���e�B�̎g�p�ۂ𔻒�
				If moved Then
					If Not .IsAbilityAvailable(a, "�ړ���") Then
						GoTo NextAbility
					End If
				Else
					If Not .IsAbilityAvailable(a, "�ړ��O") Then
						GoTo NextAbility
					End If
				End If
				
				'�񕜃A�r���e�B���ǂ���
				For i = 1 To .Ability(a).CountEffect
					If .Ability(a).EffectType(i) = "��" Then
						'�񕜗ʂ��Z�o���Ă���
						If .IsSpellAbility(a) Then
							apower = 5 * .Ability(a).EffectLevel(i) * p.Shooting
						Else
							apower = 500 * .Ability(a).EffectLevel(i)
						End If
						Exit For
					End If
				Next 
				If i > .Ability(a).CountEffect Then
					'�񕜃A�r���e�B�ł͂Ȃ�����
					GoTo NextAbility
				End If
				
				max_range = .AbilityMaxRange(a)
				min_range = .AbilityMinRange(a)
				
				x1 = MaxLng(.X - max_range, 1)
				x2 = MinLng(.X + max_range, MapWidth)
				y1 = MaxLng(.Y - max_range, 1)
				y2 = MinLng(.Y + max_range, MapHeight)
				
				'�A�r���e�B�̌��ʔ͈͂ɉ����ăA�r���e�B���L�����ǂ������f����
				num = 0
				score = 0
				If .IsAbilityClassifiedAs(a, "�l�S") Then
					' MOD START �}�[�W
					'                AreaInRange .X, .Y, min_range, max_range, .Party
					AreaInRange(.X, .Y, max_range, min_range, .Party)
					' MOD END �}�[�W
					
					'�x����p�A�r���e�B�̏ꍇ�͎����ɂ͌��ʂ��Ȃ�
					If .IsAbilityClassifiedAs(a, "��") Then
						MaskData(.X, .Y) = True
					End If
					
					'���ʔ͈͓��ɂ���^�[�Q�b�g���J�E���g
					For i = x1 To x2
						For j = y1 To y2
							If MaskData(i, j) Then
								GoTo NextUnit1
							End If
							
							t = MapDataForUnit(i, j)
							If t Is Nothing Then
								GoTo NextUnit1
							End If
							
							'�A�r���e�B���K�p�\�H
							If Not .IsAbilityApplicable(a, t) Then
								GoTo NextUnit1
							End If
							
							With t
								'�]���r�H
								If .IsConditionSatisfied("�]���r") Then
									GoTo NextUnit1
								End If
								
								If 100 * .HP \ .MaxHP < 90 Then
									num = num + 1
								End If
								score = score + 100 * MinLng(.MaxHP - .HP, apower) \ .MaxHP
							End With
NextUnit1: 
						Next 
					Next 
					
					'�s�v�H
					tx = .X
					ty = .Y
				ElseIf .IsAbilityClassifiedAs(a, "�l��") Then 
					
					mlv = .AbilityLevel(a, "�l��")
					
					'�����ʒu��ς��Ȃ��玎���Ă݂�
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) > max_range Or System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) < min_range Then
								GoTo NextPoint
							End If
							
							' MOD START �}�[�W
							AreaInRange(xx, yy, 1, mlv, .Party)
							AreaInRange(xx, yy, mlv, 1, .Party)
							' MOD END �}�[�W
							
							'�x����p�A�r���e�B�̏ꍇ�͎����ɂ͌��ʂ��Ȃ�
							If .IsAbilityClassifiedAs(a, "��") Then
								MaskData(.X, .Y) = True
							End If
							
							'���ʔ͈͓��ɂ���^�[�Q�b�g���J�E���g
							tmp_num = 0
							tmp_score = 0
							For i = MaxLng(xx - mlv, 1) To MinLng(xx + mlv, MapWidth)
								For j = MaxLng(yy - mlv, 1) To MinLng(yy + mlv, MapHeight)
									If MaskData(i, j) Then
										GoTo NextUnit2
									End If
									
									t = MapDataForUnit(i, j)
									If t Is Nothing Then
										GoTo NextUnit2
									End If
									
									'�A�r���e�B���K�p�\�H
									If Not .IsAbilityApplicable(a, t) Then
										GoTo NextUnit2
									End If
									
									With t
										'�]���r�H
										If .IsConditionSatisfied("�]���r") Then
											GoTo NextUnit2
										End If
										
										If 100 * .HP \ .MaxHP < 90 Then
											tmp_num = tmp_num + 1
										End If
										tmp_score = tmp_score + 100 * MinLng(.MaxHP - .HP, apower) \ .MaxHP
									End With
NextUnit2: 
								Next 
							Next 
							If tmp_num > 2 And tmp_score > score Then
								num = tmp_num
								score = tmp_score
								tx = xx
								ty = yy
							End If
NextPoint: 
						Next 
					Next 
				End If
				
				If num > 1 And score > max_score Then
					SelectedAbility = a
					max_score = score
				End If
				
NextAbility: 
				a = a - 1
			Loop 
			
			If SelectedAbility = 0 Then
				'�L���ȃ}�b�v�A�r���e�B���Ȃ�����
				Exit Function
			End If
			
			'���̋Z�p�[�g�i�[�̐ݒ�
			If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
				.CombinationPartner("�A�r���e�B", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			SelectedAbilityName = .Ability(SelectedAbility).Name
			
			'�A�r���e�B���g�p
			.ExecuteMapAbility(SelectedAbility, tx, ty)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
			
			'���̋Z�̃p�[�g�i�[�̍s���������炷
			If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.UseAction()
				Next 
			End If
			ReDim SelectedPartners(0)
		End With
		
		TryMapHealing = True
	End Function
	
	'�\�ł���Ή񕜃A�r���e�B���g��
	Public Function TryHealing(ByRef moved As Boolean, Optional ByRef t As Unit = Nothing) As Boolean
		Dim i, a, j As Short
		Dim aname As String
		Dim apower As Integer
		Dim max_power As Integer
		Dim max_range As Short
		Dim dmg, max_dmg As Integer
		Dim new_x, new_y As Short
		Dim distance As Integer
		Dim is_able_to_move, dont_move, sa_is_able_to_move As Boolean
		Dim u As Unit
		Dim partners() As Unit
		
		With SelectedUnit
			'����m��Ԃ̍ۂ͉񕜃A�r���e�B���g��Ȃ�
			If .IsConditionSatisfied("����m") Then
				Exit Function
			End If
			
			'������
			'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			SelectedTarget = Nothing
			max_dmg = 80
			SelectedAbility = 0
			max_power = 0
			
			'�ړ��\�H
			dont_move = moved Or .Mode = "�Œ�"
			
			'�ړ��\�ł���ꍇ�͈ړ��͈͂�ݒ肵�Ă���
			If Not dont_move Then
				AreaInSpeed(SelectedUnit)
			End If
			
			For a = 1 To .CountAbility
				'�A�r���e�B���g�p�\�H
				If moved Then
					If Not .IsAbilityAvailable(a, "�ړ���") Then
						GoTo NextHealingSkill
					End If
				Else
					If Not .IsAbilityAvailable(a, "�ړ��O") Then
						GoTo NextHealingSkill
					End If
				End If
				
				'�}�b�v�A�r���e�B�͕ʊ֐��Œ��ׂ�
				If .IsAbilityClassifiedAs(a, "�l") Then
					GoTo NextHealingSkill
				End If
				
				'����͉񕜃A�r���e�B�H
				For i = 1 To .Ability(a).CountEffect
					If .Ability(a).EffectType(i) = "��" Then
						Exit For
					End If
				Next 
				If i > .Ability(a).CountEffect Then
					GoTo NextHealingSkill
				End If
				
				'�񕜗ʂ��Z�o
				If .IsSpellAbility(a) Then
					apower = CInt(5 * .Ability(a).EffectLevel(i) * .MainPilot.Shooting)
				Else
					apower = 500 * .Ability(a).EffectLevel(i)
				End If
				
				'�𗧂����H
				If apower <= 0 Then
					GoTo NextHealingSkill
				End If
				
				'���݂̉񕜃A�r���e�B���g���ĉ񕜂�������^�[�Q�b�g�����邩����
				For	Each u In UList
					If u.Status_Renamed <> "�o��" Then
						GoTo NextHealingTarget
					End If
					
					'�������ǂ����𔻒�
					If Not .IsAlly(u) Then
						GoTo NextHealingTarget
					End If
					
					'�f�t�H���g�̃^�[�Q�b�g���w�肳��Ă���ꍇ�͂��̃��j�b�g�ȊO��
					'�^�[�Q�b�g�ɂ͂��Ȃ�
					If Not t Is Nothing Then
						If Not u Is t Then
							GoTo NextHealingTarget
						End If
					End If
					
					'�����x�́H
					dmg = 100 * u.HP \ u.MaxHP
					
					'�d�v�ȃ��j�b�g��D��
					If Not u Is SelectedUnit Then
						If u.BossRank >= 0 Then
							dmg = 100 - 2 * (100 - dmg)
						End If
					End If
					
					'���݂̃^�[�Q�b�g��葹���x���Ђǂ��Ȃ��Ȃ疳��
					If dmg > max_dmg Then
						GoTo NextHealingTarget
					End If
					
					'�ړ��\���H
					If .AbilityMaxRange(a) = 1 Or .IsAbilityClassifiedAs(a, "�o") Then
						is_able_to_move = True
					Else
						is_able_to_move = False
					End If
					If .IsAbilityClassifiedAs(a, "�p") Then
						is_able_to_move = False
					End If
					If dont_move Then
						is_able_to_move = False
					End If
					Select Case .Area
						Case "��", "�F��"
							If .EN - .AbilityENConsumption(a) < 5 Then
								is_able_to_move = False
							End If
					End Select
					
					'�˒����ɂ��邩�H
					If is_able_to_move Then
						If Not .IsTargetReachableForAbility(a, u) Then
							GoTo NextHealingTarget
						End If
					Else
						If Not .IsTargetWithinAbilityRange(a, u) Then
							GoTo NextHealingTarget
						End If
					End If
					
					'�A�r���e�B���K�p�\�H
					If Not .IsAbilityApplicable(a, u) Then
						GoTo NextHealingTarget
					End If
					
					'�]���r�H
					If u.IsConditionSatisfied("�]���r") Then
						GoTo NextHealingTarget
					End If
					
					'�V�K�^�[�Q�b�g�H
					If Not u Is SelectedTarget Then
						'�^�[�Q�b�g�ݒ�
						SelectedTarget = u
						max_dmg = dmg
						
						'�V�K�^�[�Q�b�g��D�悷�邽�߁A���ݑI������Ă���A�r���e�B�͔j��
						SelectedAbility = 0
						max_power = 0
					End If
					
					'���ݑI������Ă���񕜃A�r���e�B�ƃ`�F�b�N���̃A�r���e�B�̂ǂ��炪
					'�D��Ă��邩�𔻒�
					If max_power < u.MaxHP - u.HP Then
						'���ݑI�����Ă���񕜃A�r���e�B�ł͑S�_���[�W���񕜂�����Ȃ��ꍇ
						If apower < max_power Then
							'�񕜗ʂ������ق���D��
							GoTo NextHealingTarget
						ElseIf apower = max_power Then 
							'�񕜗ʂ������Ȃ�R�X�g���Ⴂ����D��
							If .Ability(a).ENConsumption > .Ability(SelectedAbility).ENConsumption Then
								GoTo NextHealingTarget
							End If
							If .Ability(a).Stock < .Ability(SelectedAbility).Stock Then
								GoTo NextHealingTarget
							End If
						End If
					ElseIf SelectedAbility > 0 Then 
						'���ݑI�����Ă���񕜃A�r���e�B�őS������ꍇ
						'�S�����邱�Ƃ��K�v����
						If apower >= u.MaxHP - u.HP Then
							GoTo NextHealingTarget
						End If
						'�R�X�g���Ⴂ����D��
						If .Ability(a).ENConsumption > .Ability(SelectedAbility).ENConsumption Then
							GoTo NextHealingTarget
						End If
						If .Ability(a).Stock < .Ability(SelectedAbility).Stock Then
							GoTo NextHealingTarget
						End If
					End If
					
					SelectedAbility = a
					max_power = apower
					sa_is_able_to_move = is_able_to_move
NextHealingTarget: 
				Next u
NextHealingSkill: 
			Next 
			
			'�L�p�ȃA�r���e�B���^�[�Q�b�g�����������H
			If SelectedAbility = 0 Then
				Exit Function
			End If
			If SelectedTarget Is Nothing Then
				Exit Function
			End If
			
			'�񕜃A�r���e�B���g�p���邱�Ƃ��m��
			TryHealing = True
			
			'�K�؂Ȉʒu�Ɉړ�
			If Not SelectedTarget Is SelectedUnit And sa_is_able_to_move Then
				new_x = .X
				new_y = .Y
				max_range = .AbilityMaxRange(SelectedAbility)
				With SelectedTarget
					'���݈ʒu����񕜂��\�ł���Ό��݈ʒu��D��
					If System.Math.Abs(.X - new_x) + System.Math.Abs(.Y - new_y) <= max_range Then
						distance = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
					Else
						distance = 10000
					End If
					
					'�K�؂Ȉʒu��T��
					For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
						For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
							If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) <= max_range Then
								With SelectedUnit
									If System.Math.Abs(.X - i) ^ 2 + System.Math.Abs(.Y - j) ^ 2 < distance Then
										new_x = i
										new_y = j
										distance = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
									End If
								End With
							End If
						Next 
					Next 
				End With
				
				If new_x <> .X Or new_y <> .Y Then
					'�K�؂ȏꏊ�����������̂ňړ�
					.Move(new_x, new_y)
					moved = True
				End If
			End If
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'���̋Z�p�[�g�i�[�̐ݒ�
			If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
				.CombinationPartner("�A�r���e�B", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			'�g�p�C�x���g
			HandleEvent("�g�p", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
			
			If SelectedTarget Is SelectedUnit Then
				OpenMessageForm(SelectedUnit)
			Else
				OpenMessageForm(SelectedTarget, SelectedUnit)
			End If
			
			'�񕜃A�r���e�B�����s
			.ExecuteAbility(SelectedAbility, SelectedTarget)
			SelectedUnit = .CurrentForm
		End With
		
		CloseMessageForm()
		
		'���������ꍇ�̔j��C�x���g
		If SelectedUnit.Status_Renamed = "�j��" Then
			If SelectedUnit.CountPilot > 0 Then
				HandleEvent("�j��", SelectedUnit.MainPilot.ID)
			End If
			ReDim SelectedPartners(0)
			Exit Function
		End If
		
		'�g�p��C�x���g
		If SelectedUnit.CountPilot > 0 Then
			HandleEvent("�g�p��", SelectedUnit.MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'�����A�r���e�B�̔j��C�x���g
		If SelectedUnit.Status_Renamed = "�j��" Then
			HandleEvent("�j��", SelectedUnit.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'���̋Z�̃p�[�g�i�[�̍s���������炷
		If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Function
	
	'�C�����\�ł���ΏC�����u���g��
	Public Function TryFix(ByRef moved As Boolean, Optional ByRef t As Unit = Nothing) As Boolean
		Dim TmpMaskData() As Boolean
		Dim j, i, k As Short
		Dim new_x, new_y As Short
		Dim max_dmg As Integer
		Dim tmp As Integer
		Dim u As Unit
		Dim fname As String
		
		With SelectedUnit
			'�C�����u���g�p�\�H
			If Not .IsFeatureAvailable("�C�����u") Or .Area = "�n��" Then
				Exit Function
			End If
			
			'����m��Ԃ̍ۂ͏C�����u���g��Ȃ�
			If .IsConditionSatisfied("����m") Then
				Exit Function
			End If
			
			'�C�����u���g�p�\�ȗ̈��ݒ�
			If moved Or .Mode = "�Œ�" Then
				'�ړ��łȂ��ꍇ
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						MaskData(i, j) = True
					Next 
				Next 
				If .X > 1 Then
					MaskData(.X - 1, .Y) = False
				End If
				If .X < MapWidth Then
					MaskData(.X + 1, .Y) = False
				End If
				If .Y > 1 Then
					MaskData(.X, .Y - 1) = False
				End If
				If .Y < MapHeight Then
					MaskData(.X, .Y + 1) = False
				End If
			Else
				'�ړ��\�ȏꍇ
				ReDim TmpMaskData(MapWidth + 1, MapHeight + 1)
				AreaInSpeed(SelectedUnit)
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						TmpMaskData(i, j) = MaskData(i, j)
					Next 
				Next 
				For i = 0 To MapWidth
					TmpMaskData(i, 0) = True
					TmpMaskData(i, MapHeight + 1) = True
				Next 
				For i = 0 To MapHeight
					TmpMaskData(0, i) = True
					TmpMaskData(MapWidth + 1, i) = True
				Next 
				For i = MaxLng(.X - (.Speed + 1), 1) To MinLng(.X + (.Speed + 1), MapWidth)
					For j = MaxLng(.Y - (.Speed + 1), 1) To MinLng(.Y + (.Speed + 1), MapHeight)
						MaskData(i, j) = TmpMaskData(i, j) And TmpMaskData(i - 1, j) And TmpMaskData(i + 1, j) And TmpMaskData(i, j - 1) And TmpMaskData(i, j + 1)
					Next 
				Next 
				MaskData(.X, .Y) = True
			End If
			
			'�^�[�Q�b�g��T��
			'UPGRADE_NOTE: �I�u�W�F�N�g SelectedTarget ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			SelectedTarget = Nothing
			max_dmg = 90
			For i = MaxLng(.X - (.Speed + 1), 1) To MinLng(.X + (.Speed + 1), MapWidth)
				For j = MaxLng(.Y - (.Speed + 1), 1) To MinLng(.Y + (.Speed + 1), MapHeight)
					If MaskData(i, j) Then
						GoTo NextFixTarget
					End If
					
					u = MapDataForUnit(i, j)
					If u Is Nothing Then
						GoTo NextFixTarget
					End If
					
					'�f�t�H���g�̃^�[�Q�b�g���w�肳��Ă���ꍇ�͂��̃��j�b�g�ȊO��
					'�^�[�Q�b�g�ɂ͂��Ȃ�
					If Not t Is Nothing Then
						If Not u Is t Then
							GoTo NextFixTarget
						End If
					End If
					
					'���݂̑I�����Ă���^�[�Q�b�g���_���[�W�����Ȃ���ΑI�����Ȃ�
					If 100 * u.HP \ u.MaxHP > max_dmg Then
						GoTo NextFixTarget
					End If
					
					'�������ǂ�������
					If Not .IsAlly(u) Then
						GoTo NextFixTarget
					End If
					
					'�]���r�H
					If u.IsConditionSatisfied("�]���r") Then
						GoTo NextFixTarget
					End If
					
					'�C���s�H
					If u.IsFeatureAvailable("�C���s��") Then
						For k = 2 To CInt(u.FeatureData("�C���s��"))
							fname = LIndex(u.FeatureData("�C���s��"), k)
							If Left(fname, 1) = "!" Then
								fname = Mid(fname, 2)
								If fname <> .FeatureName0("�C�����u") Then
									GoTo NextFixTarget
								End If
							Else
								If fname = .FeatureName0("�C�����u") Then
									GoTo NextFixTarget
								End If
							End If
						Next 
					End If
					
					SelectedTarget = u
					max_dmg = 100 * u.HP \ u.MaxHP
NextFixTarget: 
				Next 
			Next 
			
			'�^�[�Q�b�g��������Ȃ�
			If SelectedTarget Is Nothing Then
				Exit Function
			End If
			
			'�^�[�Q�b�g�ɗאڂ���悤�Ɉړ�
			If Not moved And .Mode <> "�Œ�" Then
				new_x = .X
				new_y = .Y
				With SelectedTarget
					'���݈ʒu����C�����\�ł���Ό��݈ʒu��D��
					If System.Math.Abs(.X - new_x) + System.Math.Abs(.Y - new_y) = 1 Then
						tmp = 1
					Else
						tmp = 10000
					End If
					
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							MaskData(i, j) = TmpMaskData(i, j)
						Next 
					Next 
					
					'�K�؂ȏꏊ��T��
					For i = MaxLng(.X - 1, 1) To MinLng(.X + 1, MapWidth)
						For j = MaxLng(.Y - 1, 1) To MinLng(.Y + 1, MapHeight)
							If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) = 1 Then
								With SelectedUnit
									If System.Math.Abs(.X - i) ^ 2 + System.Math.Abs(.Y - j) ^ 2 < tmp Then
										new_x = i
										new_y = j
										tmp = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
									End If
								End With
							End If
						Next 
					Next 
				End With
				
				If new_x <> .X Or new_y <> .Y Then
					'�K�؂ȏꏊ�����������̂ňړ�
					.Move(new_x, new_y)
					moved = True
				End If
			End If
			
			'�I����e��ύX
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'���b�Z�[�W�\��
			OpenMessageForm(SelectedTarget, SelectedUnit)
			If .IsMessageDefined("�C��") Then
				.PilotMessage("�C��")
			End If
			
			'�A�j���\��
			If .IsAnimationDefined("�C��", .FeatureName("�C��")) Then
				.PlayAnimation("�C��", .FeatureName("�C��"))
			Else
				.SpecialEffect("�C��", .FeatureName("�C��"))
			End If
			
			DisplaySysMessage(.Nickname & "��[" & SelectedTarget.Nickname & "]��[" & .FeatureName0("�C�����u") & "]���g�����B")
			
			'�C�����s
			tmp = SelectedTarget.HP
			Select Case .FeatureLevel("�C�����u")
				Case 1, -1
					SelectedTarget.RecoverHP(30 + 3 * .MainPilot.SkillLevel("�C���Z�\"))
				Case 2
					SelectedTarget.RecoverHP(50 + 5 * .MainPilot.SkillLevel("�C���Z�\"))
				Case 3
					SelectedTarget.RecoverHP(100)
			End Select
			DrawSysString(SelectedTarget.X, SelectedTarget.Y, "+" & VB6.Format(SelectedTarget.HP - tmp))
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "�̂g�o��[" & VB6.Format(SelectedTarget.HP - tmp) & "]�񕜂����B")
		End With
		
		'�o���l�l��
		SelectedUnit.GetExp(SelectedTarget, "�C��")
		
		If MessageWait < 10000 Then
			Sleep(MessageWait)
		End If
		
		CloseMessageForm()
		
		'�`�ԕω��̃`�F�b�N
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		TryFix = True
	End Function
	
	'�}�b�v�U���g�p�Ɋւ��鏈��
	Public Function TryMapAttack(ByRef moved As Boolean) As Boolean
		Dim w As Short
		Dim xx, tx, ty, yy As Short
		Dim y1, x1, x2, y2 As Short
		Dim i, j As Short
		Dim enemy_num As Short
		Dim score, score_limit As Short
		Dim t As Unit
		Dim direction As String
		Dim min_range, max_range, lv As Short
		Dim partners() As Unit
		
		With SelectedUnit
			SaveSelections()
			
			'�}�b�v�U�����g�p����^�[�Q�b�g���̉�����ݒ肷��
			score_limit = 1
			For i = 1 To .CountWeapon
				'�ʏ�U���������Ă���ꍇ�͒P�Ƃ̓G�ւ̍U���̍ۂɒʏ�U����D�悷��
				If Not .IsWeaponClassifiedAs(i, "�l") Then
					' MOD START �}�[�W
					'                score_limit = 2
					'                Exit For
					If .IsWeaponAvailable(i, "�ړ��O") Then
						score_limit = 2
						Exit For
					End If
					' MOD END �}�[�W
				End If
			Next 
			
			'�З͂̍��������D�悵�đI��
			w = .CountWeapon
			Do While w > 0
				SelectedWeapon = w
				SelectedTWeapon = 0
				
				'�}�b�v�U�����ǂ���
				If Not .IsWeaponClassifiedAs(w, "�l") Then
					GoTo NextWeapon
				End If
				
				'����̎g�p�ۂ𔻒�
				If moved Then
					If Not .IsWeaponAvailable(w, "�ړ���") Then
						GoTo NextWeapon
					End If
				Else
					If Not .IsWeaponAvailable(w, "�ړ��O") Then
						GoTo NextWeapon
					End If
				End If
				
				'�{�X���j�b�g���������S�d�m����U�������g���͔̂�펞�̂�
				If .BossRank >= 0 Then
					If .IsWeaponClassifiedAs(w, "��") Or .IsWeaponClassifiedAs(w, "�s") Or .IsWeaponClassifiedAs(w, "��") Then
						If .HP > .MaxHP \ 4 Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				max_range = .WeaponMaxRange(w)
				min_range = .Weapon(w).MinRange
				
				x1 = MaxLng(.X - max_range, 1)
				y1 = MaxLng(.Y - max_range, 1)
				x2 = MinLng(.X + max_range, MapWidth)
				y2 = MinLng(.Y + max_range, MapHeight)
				
				'�}�b�v�U���̎�ނɂ��������Č��ʔ͈͓��ɂ���G���J�E���g
				If .IsWeaponClassifiedAs(w, "�l��") Then
					For i = 1 To 4
						Select Case i
							Case 1
								direction = "N"
							Case 2
								direction = "S"
							Case 3
								direction = "W"
							Case 4
								direction = "E"
						End Select
						
						'���ʔ͈͂�ݒ�
						AreaInLine(.X, .Y, min_range, max_range, direction)
						MaskData(.X, .Y) = True
						
						'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
						If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
							Select Case direction
								Case "N"
									tx = .X
									ty = MaxLng(.Y - max_range, 1)
								Case "S"
									tx = .X
									ty = MinLng(.Y + max_range, MapHeight)
								Case "W"
									tx = MaxLng(.X - max_range, 1)
									ty = .Y
								Case "E"
									tx = MinLng(.X + max_range, MapWidth)
									ty = .Y
							End Select
							GoTo FoundWeapon
						End If
					Next 
					
				ElseIf .IsWeaponClassifiedAs(w, "�l�g") Then 
					For i = 1 To 4
						Select Case i
							Case 1
								direction = "N"
							Case 2
								direction = "S"
							Case 3
								direction = "W"
							Case 4
								direction = "E"
						End Select
						
						'���ʔ͈͂�ݒ�
						AreaInCone(.X, .Y, min_range, max_range, direction)
						MaskData(.X, .Y) = True
						
						'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
						If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
							Select Case direction
								Case "N"
									tx = .X
									ty = .Y - 1
								Case "S"
									tx = .X
									ty = .Y + 1
								Case "W"
									tx = .X - 1
									ty = .Y
								Case "E"
									tx = .X + 1
									ty = .Y
							End Select
							GoTo FoundWeapon
						End If
					Next 
					
				ElseIf .IsWeaponClassifiedAs(w, "�l��") Then 
					For i = 1 To 4
						Select Case i
							Case 1
								direction = "N"
							Case 2
								direction = "S"
							Case 3
								direction = "W"
							Case 4
								direction = "E"
						End Select
						
						'���ʔ͈͂�ݒ�
						AreaInSector(.X, .Y, min_range, max_range, direction, .WeaponLevel(w, "�l��"))
						MaskData(.X, .Y) = True
						
						'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
						If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
							Select Case direction
								Case "N"
									tx = .X
									ty = .Y - 1
								Case "S"
									tx = .X
									ty = .Y + 1
								Case "W"
									tx = .X - 1
									ty = .Y
								Case "E"
									tx = .X + 1
									ty = .Y
							End Select
							GoTo FoundWeapon
						End If
					Next 
					
				ElseIf .IsWeaponClassifiedAs(w, "�l�S") Then 
					'���ʔ͈͂�ݒ�
					' MOD START �}�[�W
					'                AreaInRange .X, .Y, min_range, max_range, "���ׂ�"
					AreaInRange(.X, .Y, max_range, min_range, "���ׂ�")
					' MOD END �}�[�W
					MaskData(.X, .Y) = True
					
					'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
					enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
					
					'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
					If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
						tx = .X
						ty = .Y
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "�l��") Then 
					lv = .WeaponLevel(w, "�l��")
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range Then
								'���ʔ͈͂�ݒ�
								If lv > 0 Then
									' MOD START �}�[�W
									'                                AreaInRange xx, yy, 1, lv, "���ׂ�"
									AreaInRange(xx, yy, lv, 1, "���ׂ�")
									' MOD END �}�[�W
								Else
									For i = 1 To MapWidth
										For j = 1 To MapHeight
											MaskData(i, j) = True
										Next 
									Next 
									MaskData(xx, yy) = False
								End If
								MaskData(.X, .Y) = True
								
								'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
								enemy_num = CountTargetInRange(w, xx - lv, yy - lv, xx + lv, yy + lv)
								
								If enemy_num > score Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						Next 
					Next 
					
					'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
					'�܂��A�l��L0�̏ꍇ�͍ő�ł��P�̂̓G�����_���Ȃ�
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Or (score = 1 And lv = 0) Then
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "�l��") Then 
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range Then
								'���ʔ͈͂�ݒ�
								AreaInPointToPoint(.X, .Y, xx, yy)
								MaskData(.X, .Y) = True
								
								'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
								enemy_num = CountTargetInRange(w, MinLng(.X, xx), MinLng(.Y, yy), MaxLng(.X, xx), MaxLng(.Y, yy))
								
								If enemy_num > score Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						Next 
					Next 
					
					'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Then
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "�l��") Then 
					'���̏�𓮂��Ȃ��ꍇ�͈ړ��^�}�b�v�U���͑I�l�O
					If .Mode = "�Œ�" Then
						GoTo NextWeapon
					End If
					
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range And MapDataForUnit(xx, yy) Is Nothing And .IsAbleToEnter(xx, yy) Then
								'���ʔ͈͂�ݒ�
								AreaInPointToPoint(.X, .Y, xx, yy)
								MaskData(.X, .Y) = True
								
								'���ʔ͈͓��ɂ��郆�j�b�g���J�E���g
								enemy_num = CountTargetInRange(w, MinLng(.X, xx), MinLng(.Y, yy), MaxLng(.X, xx), MaxLng(.Y, yy))
								
								If enemy_num > score Then
									'�ŏI�`�F�b�N �ڕW�n�_�ɂ��ǂ蒅���邩�H
									AreaInMoveAction(SelectedUnit, max_range)
									If Not MaskData(xx, yy) Then
										score = enemy_num
										tx = xx
										ty = yy
									End If
								End If
							End If
						Next 
					Next 
					
					'�}�b�v�U�����ŋ�����ł���΃^�[�Q�b�g���P�̂ł����Ă��g�p
					'�܂��A�˒����Q�̏ꍇ�͍ő�ł��P�̂̓G�����_���Ȃ�
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Or (score = 1 And max_range = 2) Then
						GoTo FoundWeapon
					End If
				End If
NextWeapon: 
				w = w - 1
			Loop 
			
			'�L���ȃ}�b�v�U����������Ȃ�����
			
			RestoreSelections()
			TryMapAttack = False
			
			Exit Function
			
FoundWeapon: 
			
			'�L���ȃ}�b�v�U�������������ꍇ
			
			'���̋Z�p�[�g�i�[�̐ݒ�
			If .IsWeaponClassifiedAs(w, "��") Then
				.CombinationPartner("����", w, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			'�}�b�v�U���ɂ��U�������s
			.MapAttack(w, tx, ty)
			
			'���̋Z�̃p�[�g�i�[�̍s���������炷
			If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.UseAction()
				Next 
			End If
			ReDim SelectedPartners(0)
			
			RestoreSelections()
			TryMapAttack = True
		End With
	End Function
	
	'���ʔ͈͓��ɂ���^�[�Q�b�g���J�E���g
	Private Function CountTargetInRange(ByVal w As Short, ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short) As Short
		Dim i, j As Short
		Dim t As Unit
		Dim is_ally_involved As Boolean
		
		With SelectedUnit
			'���ʔ͈͓��̃^�[�Q�b�g������
			For i = MaxLng(x1, 1) To MinLng(x2, MapWidth)
				For j = MaxLng(y1, 1) To MinLng(y2, MapHeight)
					'���ʔ͈͓��H
					If MaskData(i, j) Then
						GoTo NextPoint
					End If
					
					t = MapDataForUnit(i, j)
					
					'���j�b�g�����݂���H
					If t Is Nothing Then
						GoTo NextPoint
					End If
					
					'�_���[�W��^������H
					If .HitProbability(w, t, False) = 0 Then
						GoTo NextPoint
					ElseIf .ExpDamage(w, t, False) <= 10 Then 
						If .IsNormalWeapon(w) Then
							GoTo NextPoint
						ElseIf .CriticalProbability(w, t) <= 1 And .WeaponLevel(w, "�j") = 0 And .WeaponLevel(w, "��") = 0 Then 
							GoTo NextPoint
						End If
					End If
					
					'�^�[�Q�b�g�͓G�H
					If .IsAlly(t) Then
						'�����̏ꍇ�͓��m�����̉\��������̂Ń`�F�b�N���Ă���
						is_ally_involved = True
						GoTo NextPoint
					End If
					
					'����̐w�c�݂̂��U������ꍇ
					Select Case .Mode
						Case "����", "�m�o�b"
							If t.Party <> "����" And t.Party <> "�m�o�b" Then
								GoTo NextPoint
							End If
						Case "�G"
							If t.Party <> "�G" Then
								GoTo NextPoint
							End If
						Case "����"
							If t.Party <> "����" Then
								GoTo NextPoint
							End If
					End Select
					
					'�^�[�Q�b�g��������H
					If t.IsUnderSpecialPowerEffect("�B��g") Then
						GoTo NextPoint
					End If
					If t.IsFeatureAvailable("�X�e���X") Then
						If Not t.IsConditionSatisfied("�X�e���X����") And Not .IsFeatureAvailable("�X�e���X������") Then
							If t.IsFeatureLevelSpecified("�X�e���X") Then
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > t.FeatureLevel("�X�e���X") Then
									GoTo NextPoint
								End If
							Else
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > 3 Then
									GoTo NextPoint
								End If
							End If
						End If
					End If
					
					'�^�[�Q�b�g�Ɋ܂߂�
					CountTargetInRange = CountTargetInRange + 1
NextPoint: 
				Next 
			Next 
			
			'��������������ł��܂��ꍇ�͍U�����~�߂�
			If is_ally_involved And Not .IsWeaponClassifiedAs(w, "��") And Not .IsUnderSpecialPowerEffect("���ʍU��") Then
				CountTargetInRange = 0
			End If
		End With
	End Function
	
	'�X�y�V�����p���[���g�p����
	Public Sub TrySpecialPower(ByRef p As Pilot)
		Dim slist As String
		Dim sd As SpecialPowerData
		Dim i, tnum As Short
		
		SelectedPilot = p
		
		'�U�R�p�C���b�g�̓X�y�V�����p���[���g��Ȃ�
		If InStr(p.Name, "(�U�R)") > 0 Then
			Exit Sub
		End If
		
		'�Z�ʂ������قǃX�y�V�����p���[�̔����m��������
		If Dice(100) > p.TacticalTechnique0 - 100 Then
			Exit Sub
		End If
		
		With SelectedUnit
			'����Ȕ��f�͂�����H
			If .IsConditionSatisfied("����") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("�߈�") Or .IsConditionSatisfied("���|") Or .IsConditionSatisfied("����m") Then
				Exit Sub
			End If
			
			'�X�y�V�����p���[�g�p�s�\
			If .IsConditionSatisfied("�X�y�V�����p���[�g�p�s�\") Then
				Exit Sub
			End If
		End With
		
		'�g�p����\���̂���X�y�V�����p���[�̈ꗗ���쐬
		slist = ""
		For i = 1 To p.CountSpecialPower
			SelectedSpecialPower = p.SpecialPower(i)
			sd = SPDList.Item(SelectedSpecialPower)
			
			'�r�o������Ă���H
			If p.SP < p.SpecialPowerCost(SelectedSpecialPower) Then
				GoTo NextSpecialPower
			End If
			
			'���Ɏ��s�ς݁H
			If SelectedUnit.IsSpecialPowerInEffect(SelectedSpecialPower) Then
				GoTo NextSpecialPower
			End If
			
			sd = SPDList.Item(SelectedSpecialPower)
			
			With sd
				'�^�[�Q�b�g��I������K�v�̂���X�y�V�����p���[�͔��f������̂�
				'�g�p���Ȃ�
				Select Case .TargetType
					Case "����", "�G", "�C��"
						GoTo NextSpecialPower
				End Select
				
				'�^�[�Q�b�g�����Ȃ���Ύg�p���Ȃ�
				tnum = .CountTarget(p)
				If tnum = 0 Then
					GoTo NextSpecialPower
				End If
				
				'�����̃��j�b�g���^�[�Q�b�g�ɂ���X�y�V�����p���[�̓^�[�Q�b�g��
				'���Ȃ��ꍇ�͎g�p���Ȃ�
				Select Case .TargetType
					Case "�S����", "�S�G"
						If tnum < 3 Then
							GoTo NextSpecialPower
						End If
				End Select
				
				'�g�p�ɓK�����󋵉��ɂ���H
				
				'UPGRADE_WARNING: �I�u�W�F�N�g sd.IsEffectAvailable(�g�o��) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .IsEffectAvailable("�g�o��") Then
					If .TargetType = "����" Then
						If SelectedUnit.HP < 0.7 * SelectedUnit.MaxHP Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "�S����" Then 
						If Turn >= 3 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
				
				'UPGRADE_WARNING: �I�u�W�F�N�g sd.IsEffectAvailable(�d�m��) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .IsEffectAvailable("�d�m��") Then
					If .TargetType = "����" Then
						If SelectedUnit.EN < 0.3 * SelectedUnit.MaxEN Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "�S����" Then 
						If Turn >= 4 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
				
				'UPGRADE_WARNING: �I�u�W�F�N�g sd.IsEffectAvailable(�C�͑���) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .IsEffectAvailable("�C�͑���") Then
					If .TargetType = "����" Then
						If p.Morale < p.MaxMorale Then
							If p.CountSpecialPower = 1 Or p.SP > p.MaxSP \ 2 Then
								GoTo AddSpecialPower
							End If
						End If
					ElseIf .TargetType = "�S����" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				'UPGRADE_WARNING: �I�u�W�F�N�g sd.IsEffectAvailable(�s��������) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .IsEffectAvailable("�s��������") Then
					If .TargetType = "����" Then
						If DistanceFromNearestEnemy(SelectedUnit) <= 5 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
				
				'UPGRADE_WARNING: �I�u�W�F�N�g sd.IsEffectAvailable(����) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If .IsEffectAvailable("����") Then
					If .TargetType = "����" Then
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "��Ζ���") Or IsSPEffectUseful(sd, "�_���[�W����") Or IsSPEffectUseful(sd, "�N���e�B�J��������") Or IsSPEffectUseful(sd, "��������") Or IsSPEffectUseful(sd, "�ђʍU��") Or IsSPEffectUseful(sd, "�čU��") Or IsSPEffectUseful(sd, "�B��g") Then
					If .TargetType = "����" Then
						If DistanceFromNearestEnemy(SelectedUnit) <= 5 Or .Duration = "�U��" Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "�S����" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "��Ή��") Or IsSPEffectUseful(sd, "��_���[�W�ቺ") Or IsSPEffectUseful(sd, "���b����") Or IsSPEffectUseful(sd, "�������") Then
					If .TargetType = "����" Then
						If DistanceFromNearestEnemy(SelectedUnit) <= 5 Or .Duration = "�h��" Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "�S����" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "�ړ��͋���") Then
					If .TargetType = "����" Then
						If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "�S����" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "�˒�����") Then
					If .TargetType = "����" Then
						Select Case DistanceFromNearestEnemy(SelectedUnit)
							Case 5, 6
								GoTo AddSpecialPower
						End Select
					ElseIf .TargetType = "�S����" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If .IsEffectAvailable("�C�͒ቺ") Or .IsEffectAvailable("�����_���_���[�W") Or .IsEffectAvailable("�g�o����") Or .IsEffectAvailable("�d�m����") Or .IsEffectAvailable("����") Then
					If .TargetType = "�S�G" Then
						GoTo AddSpecialPower
					End If
				End If
				
				If .IsEffectAvailable("�_���[�W�ቺ") Or .IsEffectAvailable("��_���[�W����") Or .IsEffectAvailable("�����ቺ") Or .IsEffectAvailable("���ቺ") Or .IsEffectAvailable("�������ቺ") Or .IsEffectAvailable("�ړ��͒ቺ") Or .IsEffectAvailable("�T�|�[�g�K�[�h�s�\") Then
					If .TargetType = "�S�G" Then
						If Turn >= 3 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
			End With
			
			'�L�p�Ȍ��ʂ�������Ȃ�����
			GoTo NextSpecialPower
			
AddSpecialPower: 
			
			'�X�y�V�����p���[����⃊�X�g�ɒǉ�
			slist = slist & " " & SelectedSpecialPower
			
NextSpecialPower: 
		Next 
		
		'�g�p�\�ȃX�y�V�����p���[�����L���Ă��Ȃ�
		If slist = "" Then
			SelectedSpecialPower = ""
			Exit Sub
		End If
		
		'�g�p����X�y�V�����p���[�������_���ɑI��
		SelectedSpecialPower = LIndex(slist, Dice(LLength(slist)))
		
		'�g�p�C�x���g
		HandleEvent("�g�p", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		'�I�������X�y�V�����p���[�����s����
		p.UseSpecialPower(SelectedSpecialPower)
		SelectedUnit = SelectedUnit.CurrentForm
		
		'�X�e�[�^�X�E�B���h�E�X�V
		If Not IsRButtonPressed Then
			DisplayUnitStatus(SelectedUnit)
		End If
		
		'�g�p��C�x���g
		HandleEvent("�g�p��", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		
		SelectedSpecialPower = ""
	End Sub
	
	Private Function IsSPEffectUseful(ByRef sd As SpecialPowerData, ByRef ename As String) As Boolean
		With sd
			'UPGRADE_WARNING: �I�u�W�F�N�g sd.IsEffectAvailable(ename) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If .IsEffectAvailable(ename) Then
				If .TargetType = "����" Then
					'�������g���^�[�Q�b�g�ł���ꍇ�A���ɓ������ʂ����X�y�V����
					'�p���[���g�p���Ă���ꍇ�͎g�p���Ȃ��B
					If Not SelectedUnit.IsSpecialPowerInEffect(ename) Then
						IsSPEffectUseful = True
					End If
				Else
					IsSPEffectUseful = True
				End If
			End If
		End With
	End Function
	
	'���j�b�g u ���^�[�Q�b�g t ���U�����邽�߂̕����I��
	'amode:�U���̎��
	'max_prob:�G��j��ł���m��
	'max_dmg:�_���[�W���Ғl
	Public Function SelectWeapon(ByRef u As Unit, ByRef t As Unit, Optional ByRef amode As String = "", Optional ByRef max_prob As Integer = 0, Optional ByRef max_dmg As Integer = 0) As Short
		Dim smode As String
		Dim use_true_value, is_move_attack As Boolean
		Dim prob, destroy_prob As Integer
		Dim ct_prob As Short
		Dim sp_prob As Double
		Dim dmg, exp_dmg As Integer
		Dim dmg_mod As Double
		Dim su As Unit
		Dim support_dmg, support_prob, support_exp_dmg As Integer
		Dim w As Short
		Dim wclass, wattr As String
		Dim max_destroy_prob, max_exp_dmg As Integer
		Dim i, j As Short
		Dim checku As Unit
		Dim checkwc As String
		
		Dim flag As Boolean
		Dim parry_prob As Short
		Dim fdata As String
		With u
			'���l���܂ɂ͂����炦�܂���
			If .IsConditionSatisfied("����") Then
				If .Master Is t Then
					SelectWeapon = -1
					Exit Function
				End If
			End If
			
			'�x��ɖZ�����c�c
			If .IsConditionSatisfied("�x��") Then
				SelectWeapon = -1
				Exit Function
			End If
			
			'�X�y�V�����p���[���̉e�����l���ĕ����I�����邩�𔻒�
			If .Party = "����" Then
				use_true_value = True
			End If
			
			'���j�b�g���ړ��O���ǂ����𔻒�
			If amode = "�ړ���" Then
				smode = "�ړ���"
			Else
				smode = "�ړ��O"
			End If
			
			'�T�|�[�g�A�^�b�N�����Ă���郆�j�b�g�����邩�ǂ���
			If InStr(amode, "����") = 0 And InStr(amode, "�T�|�[�g") = 0 Then
				su = .LookForSupportAttack(t)
				If Not su Is Nothing Then
					w = SelectWeapon(su, t, "�T�|�[�g�A�^�b�N", support_prob, support_exp_dmg)
					If w > 0 Then
						With su
							support_prob = MinLng(.HitProbability(w, t, use_true_value), 100)
							
							dmg_mod = 1
							
							'�T�|�[�g�A�^�b�N�_���[�W�ቺ
							If IsOptionDefined("�T�|�[�g�A�^�b�N�_���[�W�ቺ") Then
								dmg_mod = 0.7
							End If
							
							'��������U���H
							If .MainPilot.IsSkillAvailable("����") And .IsNormalWeapon(w) Then
								If IsOptionDefined("�_���[�W�{���ቺ") Then
									dmg_mod = 1.2 * dmg_mod
								Else
									dmg_mod = 1.5 * dmg_mod
								End If
							End If
							
							support_dmg = .ExpDamage(w, t, use_true_value, dmg_mod)
						End With
					End If
				End If
			End If
			
			SelectWeapon = 0
			max_destroy_prob = 0
			max_exp_dmg = -1
			
			'�e������g���Ď��s
			For w = 1 To .CountWeapon
				'���킪�g�p�\�H
				If Not .IsWeaponAvailable(w, smode) Then
					GoTo NextWeapon
				End If
				
				'�}�b�v�U���͕���I��O
				If .IsWeaponClassifiedAs(w, "�l") Then
					GoTo NextWeapon
				End If
				
				'���̋Z�͎�������U����������ꍇ�ɂ̂ݎg�p
				If .IsWeaponClassifiedAs(w, "��") Then
					If InStr(amode, "����") > 0 Or InStr(amode, "�T�|�[�g") > 0 Then
						GoTo NextWeapon
					End If
				End If
				
				'�˒��͈͓��H
				If .IsWeaponClassifiedAs(w, "�ړ���U����") And amode = "�ړ��\" And .Mode <> "�Œ�" Then
					'���̋Z�͈ړ���U���\�ł��ړ���O��ɂ��Ȃ�
					'(�ړ���̈ʒu�ł͎g���Ȃ��댯�������邽��)
					If .IsWeaponClassifiedAs(w, "��") And .IsWeaponClassifiedAs(w, "�o") Then
						'�ړ����čU���͏o���Ȃ�
						If Not .IsTargetWithinRange(w, t) Then
							GoTo NextWeapon
						End If
						is_move_attack = False
					Else
						'�ړ����čU���\
						If Not .IsTargetReachable(w, t) Then
							GoTo NextWeapon
						End If
						is_move_attack = True
					End If
				Else
					'�ړ����čU���͏o���Ȃ�
					If Not .IsTargetWithinRange(w, t) Then
						GoTo NextWeapon
					End If
					is_move_attack = False
				End If
				
				'�������j�b�g�̏ꍇ�A�Ō�̈ꔭ�͎g�p���Ȃ�
				If .Party = "����" And .Party0 = "����" And InStr(amode, "�C�x���g") = 0 Then
					'�����U���͕�����蓮�I������ꍇ�ɂ̂ݎg�p
					If .IsWeaponClassifiedAs(w, "��") Then
						GoTo NextWeapon
					End If
					
					'�蓮�������̃T�|�[�g�A�^�b�N�ȊO�͎c�e�������Ȃ�������g�p���Ȃ�
					'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
					If amode <> "�T�|�[�g�A�^�b�N" Or MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
						If Not .IsWeaponClassifiedAs(w, "�i") Then
							If .Bullet(w) = 1 Or .MaxBullet(w) = 2 Or .MaxBullet(w) = 3 Then
								GoTo NextWeapon
							End If
						End If
						If .WeaponENConsumption(w) > 0 Then
							If .WeaponENConsumption(w) >= .EN \ 2 Or .WeaponENConsumption(w) >= .MaxEN \ 4 Then
								GoTo NextWeapon
							End If
						End If
						If .IsWeaponClassifiedAs(w, "�s") Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				'�{�X���j�b�g���������S�d�m����U���g���͔̂�펞�̂�
				If .BossRank >= 0 And InStr(amode, "�C�x���g") = 0 Then
					If .IsWeaponClassifiedAs(w, "��") Or .IsWeaponClassifiedAs(w, "�s") Then
						If .HP > .MaxHP \ 4 Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				'����̃��j�b�g���^�[�Q�b�g�ɂ��Ă���ꍇ�A�����U���͂��̃^�[�Q�b�g�ɂ����g��Ȃ�
				If .IsWeaponClassifiedAs(w, "��") Then
					If PList.IsDefined(.Mode) Then
						If Not PList.Item(.Mode).Unit_Renamed Is Nothing Then
							If .IsEnemy((PList.Item(.Mode).Unit_Renamed)) Then
								If Not PList.Item(.Mode).Unit_Renamed Is t Then
									GoTo NextWeapon
								End If
							End If
						End If
					End If
				End If
				
				'�_���[�W�C����
				dmg_mod = 1
				
				'�T�|�[�g�A�^�b�N�_���[�W�ቺ
				If InStr(amode, "�T�|�[�g") > 0 Then
					If IsOptionDefined("�T�|�[�g�A�^�b�N�_���[�W�ቺ") Then
						dmg_mod = 0.7
					End If
				End If
				
				'�_���[�W�Z�o
				dmg = .ExpDamage(w, t, use_true_value, dmg_mod)
				
				'�U���̉۔�����s���ꍇ�̓_���[�W��^�����镐�킪����΂悢
				If InStr(amode, "�۔���") > 0 Then
					If dmg > 0 Then
						SelectWeapon = w
						Exit Function
					ElseIf Not .IsNormalWeapon(w) Then 
						If .CriticalProbability(w, t) > 0 Then
							SelectWeapon = w
							Exit Function
						End If
					End If
					GoTo NextWeapon
				End If
				
				If dmg = 0 Then
					'���E�U���͈ꌂ�œ|����ꍇ�łȂ��ƌ��ʂ�����
					If .IsWeaponClassifiedAs(w, "�E") Then
						GoTo NextWeapon
					End If
					
					'�_���[�W�����̃X�y�V�����p���[���g�p���Ă���ꍇ�̓_���[�W��^�����Ȃ�
					'�����I�����Ȃ�
					If .IsUnderSpecialPowerEffect("�_���[�W����") Then
						GoTo NextWeapon
					End If
				End If
				
				'����̂g�o��10�ȉ��̏ꍇ�̓_���[�W�������グ
				If t.HP <= 10 Then
					If 0 < dmg And dmg < 20 Then
						If .Weapon(w).Power > 0 Then
							dmg = 20
						End If
					End If
				End If
				
				'�čU�����\�ȏꍇ
				If InStr(amode, "�T�|�[�g") = 0 Then
					If .IsUnderSpecialPowerEffect("�čU��") Then
						'�čU������c�e���d�m������H
						If .Weapon(w).Bullet > 0 Then
							If .Bullet(w) < 2 Then
								GoTo NextWeapon
							End If
						End If
						If .Weapon(w).ENConsumption > 0 Then
							If .EN < 2 * .WeaponENConsumption(w) Then
								GoTo NextWeapon
							End If
						End If
						dmg = 2 * dmg
					ElseIf .IsWeaponClassifiedAs(w, "��") Then 
						dmg = dmg + dmg * .WeaponLevel(w, "��") \ 16
					End If
				End If
				
				'�������Z�o
				prob = .HitProbability(w, t, use_true_value)
				
				'����\�͂ɂ������F������H
				If (.MainPilot.TacticalTechnique >= 150 Or .Party = "����") And Not .IsUnderSpecialPowerEffect("��Ζ���") Then
					'�؂蕥���\�ȏꍇ�͖�������ቺ
					If .IsWeaponClassifiedAs(w, "��") Or .IsWeaponClassifiedAs(w, "��") Or .IsWeaponClassifiedAs(w, "��") Then
						
						'�؂蕥���\�H
						flag = False
						If t.IsFeatureAvailable("�i������") Then
							flag = True
						Else
							For i = 1 To t.CountWeapon
								If t.IsWeaponClassifiedAs(i, "��") And t.IsWeaponMastered(i) And t.MainPilot.Morale >= t.Weapon(i).NecessaryMorale And Not t.IsDisabled((t.Weapon(i).Name)) Then
									flag = True
									Exit For
								End If
							Next 
						End If
						If Not t.MainPilot.IsSkillAvailable("�؂蕥��") Then
							flag = False
						End If
						
						'�؂蕥���o����ꍇ�͖�������ቺ
						If flag Then
							
							parry_prob = 2 * t.MainPilot.SkillLevel("�؂蕥��")
							If .IsWeaponClassifiedAs(w, "��") Then
								If .IsWeaponClassifiedAs(w, "�T") Then
									parry_prob = parry_prob - .MainPilot.SkillLevel("�����o") - .MainPilot.SkillLevel("�m�o����")
									With t.MainPilot
										parry_prob = parry_prob + .SkillLevel("�����o") + .SkillLevel("�m�o����")
									End With
								End If
							Else
								parry_prob = parry_prob - .MainPilot.SkillLevel("�؂蕥��")
							End If
							
							If parry_prob > 0 Then
								prob = prob * (32 - parry_prob) \ 32
							End If
						End If
					End If
					
					'���g�\�ȏꍇ�͖�������ቺ
					If t.IsFeatureAvailable("���g") Then
						If t.MainPilot.Morale >= 130 Then
							prob = prob \ 2
						End If
					End If
					If t.MainPilot.SkillLevel("���g") > 0 Then
						prob = prob * t.MainPilot.SkillLevel("���g") \ 16
					End If
					
					'������\�ȏꍇ�͖�������ቺ
					If t.IsFeatureAvailable("�����") Then
						fdata = t.FeatureData("�����")
						If StrToLng(LIndex(fdata, 2)) > t.EN And StrToLng(LIndex(fdata, 3)) > t.MainPilot.Morale Then
							prob = prob * t.FeatureLevel("�����") \ 10
						End If
					End If
				End If
				
				'�b�s���Z�o
				ct_prob = .CriticalProbability(w, t)
				
				'������ʂ�^����m�����v�Z
				sp_prob = 0
				wclass = .WeaponClass(w)
				With t
					For i = 1 To Len(wclass)
						wattr = GetClassBundle(wclass, i)
						
						'������ʖ������ɂ���Ė����������H
						If .SpecialEffectImmune(wattr) Then
							GoTo NextAttribute
						End If
						
						Select Case wattr
							Case "��"
								If Not .IsConditionSatisfied("�s���s�\") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "�r"
								If Not .IsConditionSatisfied("�s���s�\") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "��"
								If Not .IsConditionSatisfied("����") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "�"
								If Not .IsConditionSatisfied("���") Then
									sp_prob = sp_prob + 0.7
								End If
							Case "�s"
								If Not .IsConditionSatisfied("�U���s�\") And .CountWeapon > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "�~"
								If Not .IsConditionSatisfied("�ړ��s�\") And .Speed > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "��"
								If Not .IsConditionSatisfied("�Ή�") And .BossRank < 0 Then
									sp_prob = sp_prob + 1
								End If
							Case "��"
								If Not .IsConditionSatisfied("����") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "��"
								If Not .IsConditionSatisfied("����") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "�h"
								If Not .IsConditionSatisfied("�h��") And .CountWeapon > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "��"
								If Not .IsConditionSatisfied("���|") Then
									sp_prob = sp_prob + 0.4
								End If
							Case "��"
								If Not .IsConditionSatisfied("����") Then
									sp_prob = sp_prob + 0.6
								End If
							Case "��"
								If .BossRank < 0 Then
									sp_prob = sp_prob + 1
								End If
							Case "��"
								If Not .IsConditionSatisfied("����") Then
									For j = 1 To .CountWeapon
										If .IsSpellWeapon(j) Or .IsWeaponClassifiedAs(j, "��") Then
											sp_prob = sp_prob + 0.3
											Exit For
										End If
									Next 
									If j > .CountWeapon Then
										For j = 1 To .CountAbility
											If .IsSpellAbility(j) Or .IsAbilityClassifiedAs(j, "��") Then
												sp_prob = sp_prob + 0.3
												Exit For
											End If
										Next 
									End If
								End If
							Case "��"
								If Not .IsConditionSatisfied("�Ӗ�") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "��"
								If Not .IsConditionSatisfied("��") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "�x"
								If Not .IsConditionSatisfied("�x��") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "��"
								If Not .IsConditionSatisfied("����m") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "�]"
								If Not .IsConditionSatisfied("�]���r") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "�Q"
								If Not .IsConditionSatisfied("�񕜕s�\") Then
									If .IsFeatureAvailable("�g�o��") Or .IsFeatureAvailable("�d�m��") Then
										sp_prob = sp_prob + 0.4
									End If
								End If
							Case "��"
								If Not .IsConditionSatisfied("���b��") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "��"
								If Not .IsConditionSatisfied("�o���A������") Then
									If .IsFeatureAvailable("�o���A") And InStr(t.FeatureData("�o���A"), "�o���A����������") = 0 Then
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("�L��o���A") Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("�o���A�V�[���h") And InStr(t.FeatureData("�o���A�V�[���h"), "�o���A����������") = 0 Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("�t�B�[���h") And InStr(t.FeatureData("�t�B�[���h"), "�o���A����������") = 0 Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("�L��t�B�[���h") Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("�A�N�e�B�u�t�B�[���h") And InStr(t.FeatureData("�A�N�e�B�u�t�B�[���h"), "�o���A����������") = 0 Then 
										sp_prob = sp_prob + 0.3
									End If
								End If
							Case "��"
								For j = 1 To .CountCondition
									If (InStr(.Condition(j), "�t��") > 0 Or InStr(.Condition(j), "����") > 0 Or InStr(.Condition(j), "�t�o") > 0) And .ConditionLifetime(j) > 0 Then
										sp_prob = sp_prob + 0.3
										Exit For
									End If
								Next 
							Case "��"
								If .BossRank < 0 Then
									sp_prob = sp_prob + 1
								End If
							Case "��"
								If .BossRank < 0 Then
									sp_prob = sp_prob + 0.4
								End If
							Case "�E"
								If .MainPilot.Personality <> "�@�B" Then
									sp_prob = sp_prob + 0.2
								End If
							Case "�c"
								If .MainPilot.Personality <> "�@�B" Then
									sp_prob = sp_prob + 0.25
								End If
							Case "��U"
								If Not .IsConditionSatisfied("�U���͂c�n�v�m") And .CountWeapon() > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "��h"
								If Not .IsConditionSatisfied("�h��͂c�n�v�m") Then
									sp_prob = sp_prob + 0.2
								End If
							Case "��^"
								If Not .IsConditionSatisfied("�^�����c�n�v�m") Then
									sp_prob = sp_prob + 0.1
								End If
							Case "���"
								If Not .IsConditionSatisfied("�ړ��͂c�n�v�m") And .Speed > 0 Then
									sp_prob = sp_prob + 0.1
								End If
							Case "��"
								If Not .IsConditionSatisfied("������҂�") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "��"
								If .BossRank >= 0 Or u.IsFeatureAvailable("�m�[�}�����[�h") Then
									GoTo NextAttribute
								End If
								Select Case u.Size
									Case "SS"
										Select Case .Size
											Case "M", "L", "LL", "XL"
												GoTo NextAttribute
										End Select
									Case "S"
										Select Case .Size
											Case "L", "LL", "XL"
												GoTo NextAttribute
										End Select
									Case "M"
										Select Case .Size
											Case "SS", "LL", "XL"
												GoTo NextAttribute
										End Select
									Case "L"
										Select Case .Size
											Case "SS", "S", "XL"
												GoTo NextAttribute
										End Select
									Case "LL"
										Select Case .Size
											Case "SS", "S", "M"
												GoTo NextAttribute
										End Select
									Case "XL"
										Select Case .Size
											Case "SS", "S", "M", "L"
												GoTo NextAttribute
										End Select
								End Select
								sp_prob = sp_prob + 1
							Case "��"
								If .BossRank < 0 And Not u.IsFeatureAvailable("�m�[�}�����[�h") Then
									sp_prob = sp_prob + 1
								End If
							Case "��"
								If .BossRank >= 0 Then
									Select Case CShort(u.WeaponLevel(w, "��"))
										Case 1
											sp_prob = sp_prob + 1 / 8
										Case 2
											sp_prob = sp_prob + 1 / 4
										Case 3
											sp_prob = sp_prob + 1 / 2
									End Select
								Else
									Select Case CShort(u.WeaponLevel(w, "��"))
										Case 1
											sp_prob = sp_prob + 1 / 4
										Case 2
											sp_prob = sp_prob + 1 / 2
										Case 3
											sp_prob = sp_prob + 3 / 4
									End Select
								End If
							Case "��"
								If .BossRank >= 0 Then
									Select Case CShort(u.WeaponLevel(w, "��"))
										Case 1
											sp_prob = sp_prob + 1 / 16
										Case 2
											sp_prob = sp_prob + 1 / 8
										Case 3
											sp_prob = sp_prob + 1 / 4
									End Select
								Else
									Select Case CShort(u.WeaponLevel(w, "��"))
										Case 1
											sp_prob = sp_prob + 1 / 8
										Case 2
											sp_prob = sp_prob + 1 / 4
										Case 3
											sp_prob = sp_prob + 1 / 2
									End Select
								End If
							Case Else
								'�㑮��
								If Left(wattr, 1) = "��" Then
									'�����S�����������āA���ݑΏۂɍU���\�ȃ��j�b�g��
									'�t��������_�ɑ΂��鑮���U�������ꍇ�B
									'������ʔ������͂Ƃ肠������h(0.2)�Ƃ��낦�Ă݂�
									checkwc = Mid(wattr, 2)
									If Not .Weakness(checkwc) Then
										For	Each checku In UList
											With checku
												If .Party = .Party And .Status_Renamed = "�o��" Then
													For j = 1 To .CountWeapon
														If .IsWeaponClassifiedAs(j, checkwc) And .IsWeaponAvailable(j, "�ړ��O") Then
															'�˒��͈͓��H
															If .IsWeaponClassifiedAs(j, "�ړ���U����") And .Mode <> "�Œ�" Then
																'���̋Z�͈ړ���U���\�ł��ړ���O��ɂ��Ȃ�
																'(�ړ���̈ʒu�ł͎g���Ȃ��댯�������邽��)
																If .IsWeaponClassifiedAs(j, "��") And .IsWeaponClassifiedAs(j, "�o") Then
																	'�ړ����čU���͏o���Ȃ�
																	If .IsTargetWithinRange(j, t) Then
																		sp_prob = sp_prob + 0.2
																		GoTo NextAttribute
																	End If
																Else
																	'�ړ����čU���\
																	If .IsTargetReachable(j, t) Then
																		sp_prob = sp_prob + 0.2
																		GoTo NextAttribute
																	End If
																End If
															Else
																'�ړ����čU���͏o���Ȃ�
																If .IsTargetWithinRange(j, t) Then
																	sp_prob = sp_prob + 0.2
																	GoTo NextAttribute
																End If
															End If
														End If
													Next 
												End If
											End With
										Next checku
									End If
									'������
								ElseIf Left(wattr, 1) = "��" Then 
									'�����S�����������āA���ݑΏۂɍU���\�ȃ��j�b�g��
									'�t�������L���ɑ΂��镕��A����U�������ꍇ�B
									'������ʔ�������0.1�Ƃ��Ă݂�
									checkwc = Mid(wattr, 2)
									If Not .Weakness(checkwc) And Not .Effective(checkwc) Then
										For	Each checku In UList
											If checku.Party = .Party And checku.Status_Renamed = "�o��" Then
												For j = 1 To checku.CountWeapon
													With checku
														If .IsWeaponClassifiedAs(j, checkwc) And .IsWeaponAvailable(j, "�ړ��O") Then
															'�t������L���ɑΉ����镕��A���蕐�킪����
															If InStrNotNest(.WeaponClass(j), "��") > 0 Or InStrNotNest(.WeaponClass(j), "��") > 0 Then
																If InStrNotNest(.WeaponClass(j), checkwc) > InStrNotNest(checku.WeaponClass(j), "��") Then
																	'�˒��͈͓��H
																	If .IsWeaponClassifiedAs(j, "�ړ���U����") And .Mode <> "�Œ�" Then
																		'���̋Z�͈ړ���U���\�ł��ړ���O��ɂ��Ȃ�
																		'(�ړ���̈ʒu�ł͎g���Ȃ��댯�������邽��)
																		If .IsWeaponClassifiedAs(j, "��") And .IsWeaponClassifiedAs(j, "�o") Then
																			'�ړ����čU���͏o���Ȃ�
																			If .IsTargetWithinRange(j, t) Then
																				sp_prob = sp_prob + 0.1
																				GoTo NextAttribute
																			End If
																		Else
																			'�ړ����čU���\
																			If .IsTargetReachable(j, t) Then
																				sp_prob = sp_prob + 0.1
																				GoTo NextAttribute
																			End If
																		End If
																	Else
																		'�ړ����čU���͏o���Ȃ�
																		If .IsTargetWithinRange(j, t) Then
																			sp_prob = sp_prob + 0.1
																			GoTo NextAttribute
																		End If
																	End If
																End If
															End If
														End If
													End With
												Next 
											End If
										Next checku
									End If
									'������
								ElseIf Left(wattr, 1) = "��" Then 
									'������ʔ������ّ͖���������0.3
									checkwc = Mid(wattr, 2)
									Select Case checkwc
										Case "�I"
											If Not .IsConditionSatisfied("�I�[���g�p�s�\") Then
												If .IsSkillAvailable("�I�[��") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "��"
											If Not .IsConditionSatisfied("���\�͎g�p�s�\") Then
												If .IsSkillAvailable("���\��") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "�V"
											If Not .IsConditionSatisfied("�������g�p�s�\") Then
												If .IsSkillAvailable("������") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "�T"
											If Not .IsConditionSatisfied("�����o�g�p�s�\") And Not .IsConditionSatisfied("�m�o�����g�p�s�\") Then
												If .IsSkillAvailable("�����o") Or .IsSkillAvailable("�m�o����") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "��"
											If Not .IsConditionSatisfied("��͎g�p�s�\") Then
												If .IsSkillAvailable("���") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "�p"
											'�p�͎ˌ��𖂗͂ƕ\�����邽�߂����ɕt���Ă���ꍇ�����邽��
											'1���x���ȉ��̏ꍇ�͕���A�A�r���e�B���m�F
											If Not .IsConditionSatisfied("�p�g�p�s�\") Then
												If .SkillLevel("�p") > 1 Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "�Z"
											If Not .IsConditionSatisfied("�Z�g�p�s�\") Then
												If .IsSkillAvailable("�Z") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
									End Select
									
									If Not .IsConditionSatisfied(checkwc & "�����g�p�s�\") Then
										For j = 1 To .CountWeapon
											If .IsWeaponClassifiedAs(j, checkwc) Then
												sp_prob = sp_prob + 0.3
												Exit For
											End If
										Next 
										If j > .CountWeapon Then
											For j = 1 To .CountAbility
												If .IsAbilityClassifiedAs(j, checkwc) Then
													sp_prob = sp_prob + 0.3
													Exit For
												End If
											Next 
										End If
									End If
								End If
						End Select
NextAttribute: 
					Next 
				End With
				If sp_prob > 1 Then
					sp_prob = System.Math.Sqrt(sp_prob)
				End If
				sp_prob = sp_prob * ct_prob
				
				'�o���A���ōU�����h����Ă��܂��ꍇ�͓�����ʂ͔������Ȃ�
				If .WeaponPower(w, "") > 0 And dmg = 0 And Not .IsWeaponClassifiedAs(w, "��") Then
					sp_prob = 0
				End If
				
				'�K���������������ʂ��l��
				If .IsWeaponClassifiedAs(w, "�z") Then
					If .HP < .MaxHP Then
						sp_prob = sp_prob + 25 * dmg \ t.MaxHP
					End If
				End If
				If .IsWeaponClassifiedAs(w, "��") Then
					sp_prob = sp_prob + 50 * dmg \ t.MaxHP
				End If
				If .IsWeaponClassifiedAs(w, "�D") Then
					sp_prob = sp_prob + 50 * dmg \ t.MaxHP
				End If
				
				'�搧�U���̏ꍇ�͓�����ʂ�L���ɔ���
				If InStr(amode, "����") > 0 Then
					If .IsWeaponClassifiedAs(w, "��") Or .UsedCounterAttack < .MaxCounterAttack Then
						sp_prob = 1.5 * sp_prob
					End If
				End If
				If sp_prob > 100 Then
					sp_prob = 100
				End If
				
				'�b�s�����Ⴂ�ꍇ�͓�����ʂ݂̂̍U�����d�����Ȃ�
				If dmg = 0 And ct_prob < 30 Then
					sp_prob = sp_prob / 5
				End If
				
				'�_���[�W���^�����Ȃ�����͎g�p���Ȃ�
				If dmg = 0 And sp_prob = 0 Then
					GoTo NextWeapon
				End If
				
				If prob > 0 Then
					If sp_prob > 0 Then
						'������ʂ̉e�����������ă_���[�W�̊��Ғl���v�Z
						exp_dmg = dmg + MaxLng(t.HP - dmg, 0) * sp_prob \ 100
					Else
						'�N���e�B�J���̉e�����������ă_���[�W�̊��Ғl���v�Z
						If IsOptionDefined("�_���[�W�{���ቺ") Then
							If .IsWeaponClassifiedAs(w, "��") Then
								exp_dmg = dmg + 0.1 * .WeaponLevel(w, "��") * dmg * ct_prob \ 100
							Else
								exp_dmg = dmg + 0.2 * dmg * ct_prob \ 100
							End If
						Else
							If .IsWeaponClassifiedAs(w, "��") Then
								exp_dmg = dmg + 0.25 * .WeaponLevel(w, "��") * dmg * ct_prob \ 100
							Else
								exp_dmg = dmg + 0.5 * dmg * ct_prob \ 100
							End If
						End If
					End If
					exp_dmg = exp_dmg * 0.01 * MinLng(prob, 100)
				Else
					'������������Ȃ��ꍇ�͊��Ғl���v���؂艺����
					prob = 1
					exp_dmg = (dmg \ 10 + MaxLng(t.HP - dmg \ 10, 0) * sp_prob \ 100) \ 10
				End If
				
				'�T�|�[�g�ɂ��_���[�W�����Ғl�ɒǉ�
				If Not is_move_attack Then
					exp_dmg = exp_dmg + support_exp_dmg
				End If
				
				'�G�̔j��m�����v�Z
				destroy_prob = 0
				With t
					If .Party = "����" And Not .IsFeatureAvailable("�h��s��") Then
						If dmg >= 2 * .HP Then
							destroy_prob = MinLng(prob, 100)
						End If
						'�T�|�[�g�ɂ��j��m��
						If Not is_move_attack Then
							If support_dmg >= 2 * .HP Then
								destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob \ 100
							ElseIf dmg + support_dmg >= 2 * .HP Then 
								destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob \ 10000
							End If
						End If
					Else
						If dmg >= .HP Then
							destroy_prob = MinLng(prob, 100)
						End If
						'�T�|�[�g�ɂ��j��m��
						If Not is_move_attack Then
							If support_dmg >= .HP Then
								destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob \ 100
							ElseIf dmg + support_dmg >= .HP Then 
								destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob \ 10000
							End If
						End If
					End If
				End With
				
				'�搧�U���̏ꍇ�͓G��j��o����U����L���ɔ���
				If InStr(amode, "����") > 0 Then
					If .IsWeaponClassifiedAs(w, "��") Or .UsedCounterAttack < .MaxCounterAttack Then
						destroy_prob = 1.5 * destroy_prob
					End If
				End If
				
				'�d�m���ՍU���̎g�p�͐T�d��
				If .IsWeaponClassifiedAs(w, "��") Then
					If .Party = "����" Then
						'�����������[�h���ǂ���
						'UPGRADE_ISSUE: Control mnuMapCommandItem �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
							GoTo NextWeapon
						End If
					Else
						'�G���j�b�g�͑����|����Ƃ��ɂ����d�m���ՍU�����g��Ȃ�
						If destroy_prob = 0 And .BossRank < 0 Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				If destroy_prob >= 100 Then
					'�j��m����100%�̏ꍇ�̓R�X�g�̒Ⴓ��D��
					'(�m���������ꍇ�͔ԍ����Ⴂ������g�p)
					If .Party = "����" Or .Party = "�m�o�b" Then
						If destroy_prob > max_destroy_prob Then
							SelectWeapon = w
							max_destroy_prob = destroy_prob
							max_exp_dmg = exp_dmg
						End If
					Else
						'�G�̏ꍇ�̓R�X�g����
						If destroy_prob > max_destroy_prob Then
							SelectWeapon = w
							max_destroy_prob = destroy_prob
							max_exp_dmg = exp_dmg
						Else
							If exp_dmg > max_exp_dmg Then
								SelectWeapon = w
								max_destroy_prob = destroy_prob
								max_exp_dmg = exp_dmg
							End If
						End If
					End If
				ElseIf destroy_prob > 50 Then 
					'�j��m����50%��荂���ꍇ�͔j��m���̍�����D��
					If destroy_prob > max_destroy_prob Then
						SelectWeapon = w
						max_destroy_prob = destroy_prob
						max_exp_dmg = exp_dmg
					ElseIf destroy_prob = max_destroy_prob Then 
						If exp_dmg > max_exp_dmg Then
							SelectWeapon = w
							max_destroy_prob = destroy_prob
							max_exp_dmg = exp_dmg
						End If
					End If
				Else
					'�j��m����50%�ȉ��̏ꍇ�̓_���[�W�̊��Ғl�̍�����D��
					If max_destroy_prob <= 50 Then
						If exp_dmg > max_exp_dmg Then
							SelectWeapon = w
							max_destroy_prob = destroy_prob
							max_exp_dmg = exp_dmg
						End If
					End If
				End If
NextWeapon: 
			Next 
			
			'�_���[�W��^�����Ȃ����킪�I�����ꂽ�ꍇ�̓L�����Z��
			If SelectWeapon > 0 Then
				If .WeaponAdaption(SelectWeapon, (t.Area)) = 0 Then
					SelectWeapon = 0
				End If
			End If
			
			'�U�����ʂ̊��Ғl�̏�������
			If max_destroy_prob > 50 Then
				max_prob = max_destroy_prob
			Else
				max_prob = 0
			End If
			max_dmg = 100 * (max_exp_dmg / .HP)
		End With
	End Function
	
	'���j�b�g u ������ w �ōU�����������ۂɃ^�[�Q�b�g t ���I������h��s����Ԃ�
	Public Function SelectDefense(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, ByRef tw As Short) As Object
		Dim prob, dmg As Integer
		Dim tprob, tdmg As Integer
		Dim is_target_inferior As Boolean
		
		'�}�b�v�U���ɑ΂��Ă͖h��s�������Ȃ�
		If u.IsWeaponClassifiedAs(w, "�l") Then
			Exit Function
		End If
		
		With t
			'�x���Ă���ꍇ�͉������
			If .IsConditionSatisfied("�x��") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				SelectDefense = "���"
				Exit Function
			End If
			
			'����m��Ԃ̍ۂ͖h��s�������Ȃ�
			If .IsConditionSatisfied("����m") Then
				Exit Function
			End If
			
			'���h����Ԃ̃��j�b�g�͖h��s�������Ȃ�
			If .IsUnderSpecialPowerEffect("���h��") Then
				Exit Function
			End If
			
			If .Party <> "����" Then
				'�u�G���j�b�g�h��g�p�v�I�v�V������I�����Ă���ꍇ�ɂ̂ݓG���j�b�g��
				'�h��s�����s��
				If Not IsOptionDefined("�G���j�b�g�h��g�p") Then
					Exit Function
				End If
				
				'�h��s�����g���Ă���̂͋Z�ʂ�160�ȏ�̃U�R�łȂ��p�C���b�g�̂�
				With .MainPilot
					If InStr(.Name, "(�U�R)") > 0 Or .TacticalTechnique < 160 Then
						Exit Function
					End If
				End With
			End If
			
			'�s���s�\�H
			If .MaxAction = 0 Then
				'�`���[�W���A���Ւ��͏�ɖh��A����ȊO�̏ꍇ�͖h��s�������Ȃ�
				If .IsConditionSatisfied("�`���[�W") Or .IsConditionSatisfied("����") Then
					'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					SelectDefense = "�h��"
				End If
				Exit Function
			End If
			
			'����̍U���̃_���[�W�E���������Z�o
			dmg = u.ExpDamage(w, t, True)
			prob = MinLng(u.HitProbability(w, t, True), 100)
			
			'�_�~�[�������Ă���ꍇ�A����̍U���͖���
			If .IsFeatureAvailable("�_�~�[") And .ConditionLevel("�_�~�[�j��") < .FeatureLevel("�_�~�[") Then
				prob = 0
			End If
			
			'�T�|�[�g�K�[�h�����ꍇ������̍U���͖���
			If Not .LookForSupportGuard(u, w) Is Nothing Then
				prob = 0
			End If
			
			'�����̃_���[�W�E���������Z�o
			If tw > 0 Then
				tdmg = .ExpDamage(tw, u, True)
				tprob = MinLng(.HitProbability(tw, u, True), 100)
				
				'�_�~�[�������Ă���ꍇ�͔����͖���
				If u.IsFeatureAvailable("�_�~�[") And u.ConditionLevel("�_�~�[�j��") < u.FeatureLevel("�_�~�[") Then
					prob = 0
				End If
			End If
			
			'����̍U���̌��ʂƂ�����̔����̌��ʂ��r
			If .Party = "����" Then
				'�������j�b�g�̏ꍇ�A����̍U���ɂ��_���[�W�̕��������ꍇ�͖h��
				If dmg * prob > tdmg * tprob And tdmg < u.HP Then
					is_target_inferior = True
				End If
				
				'�C���̈ꌂ�͖h���D�悵�A���߂���
				If u.IsUnderSpecialPowerEffect("�_���[�W����") Then
					If 2 * dmg * prob > tdmg * tprob And tdmg < u.HP Then
						is_target_inferior = True
					End If
				End If
			Else
				'�G���j�b�g�̏ꍇ�ł�����̍U���ɂ��_���[�W�̕����Q�{�ȏ㑽���ꍇ�͖h��
				If dmg * prob \ 2 > tdmg * tprob And tdmg < u.HP Then
					is_target_inferior = True
				End If
				
				'�C���̈ꌂ�͖h���D�悵�A���߂���
				If u.IsUnderSpecialPowerEffect("�_���[�W����") Then
					If dmg * prob > tdmg * tprob And tdmg < u.HP Then
						is_target_inferior = True
					End If
				End If
			End If
			
			'���ƈꌂ�Ŕj�󂳂�Ă��܂��ꍇ�͕K���h��
			'(���������Ⴂ�ꍇ������)
			If dmg >= .HP And prob > 10 Then
				is_target_inferior = True
			End If
			
			If tw > 0 Then
				'�搧�U���\�H
				If Not .IsWeaponClassifiedAs(tw, "��") Then
					If .IsWeaponClassifiedAs(tw, "��") Or u.IsWeaponClassifiedAs(w, "��") Or .MaxCounterAttack > .UsedCounterAttack Then
						If tdmg >= u.HP And tprob > 70 Then
							'�搧�U���œ|����ꍇ�͖��킸����
							is_target_inferior = False
						End If
					End If
				End If
			Else
				'�����ł��Ȃ��ꍇ�͖h��
				is_target_inferior = True
			End If
			
			If Not is_target_inferior Then
				'������I������
				Exit Function
			End If
			
			'�h�䑤���򐨂Ȃ̂Ŕ����͍s�킸�A�h��s����I��
			
			'��������Έꌂ���ŁA�h�䂷��Δj����܂ʂ����U���͕K���h��
			If dmg > .HP And dmg \ 2 < .HP And Not .IsFeatureAvailable("�h��s��") And Not u.IsWeaponClassifiedAs(w, "�E") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				SelectDefense = "�h��"
				Exit Function
			End If
			
			'����̖��������Ⴂ�ꍇ�͉��
			If prob < 50 And Not .IsFeatureAvailable("���s��") And Not .IsConditionSatisfied("�ړ��s�\") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				SelectDefense = "���"
				Exit Function
			End If
			
			'�h�䂷��Έꌂ�����܂ʂ����ꍇ�͖h��
			If dmg \ 2 < .HP And Not .IsFeatureAvailable("�h��s��") And Not u.IsWeaponClassifiedAs(w, "�E") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				SelectDefense = "�h��"
				Exit Function
			End If
			
			'�ǂ����悤���Ȃ��̂łƂ肠�������
			If Not .IsFeatureAvailable("���s��") And Not .IsConditionSatisfied("�ړ��s�\") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				SelectDefense = "���"
				Exit Function
			End If
			
			'������o���Ȃ��̂Ŗh��c�c
			If Not .IsFeatureAvailable("�h��s��") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				SelectDefense = "�h��"
			End If
		End With
	End Function
	
	'���j�b�g u ���^�[�Q�b�g t �ɔ����\���H
	Public Function IsAbleToCounterAttack(ByRef u As Unit, ByRef t As Unit) As Boolean
		Dim i, w, idx As Short
		Dim buf, wclass, ch As String
		
		With u
			For w = 1 To .CountWeapon
				'���킪�g�p�\�H
				If Not .IsWeaponAvailable(w, "�ړ��O") Then
					GoTo NextWeapon
				End If
				
				'�}�b�v�U���͕���I��O
				If .IsWeaponClassifiedAs(w, "�l") Then
					GoTo NextWeapon
				End If
				
				'���̋Z�͎�������U����������ꍇ�ɂ̂ݎg�p
				If .IsWeaponClassifiedAs(w, "��") Then
					GoTo NextWeapon
				End If
				
				'�˒��͈͓��H
				If Not .IsTargetWithinRange(w, t) Then
					GoTo NextWeapon
				End If
				
				' ADD START �}�[�W
				'�_���[�W��^������H
				If .Damage(w, t, True) > 0 Then
					IsAbleToCounterAttack = True
					Exit Function
				End If
				
				'������ʂ�^������H
				If Not .IsNormalWeapon(w) Then
					If .CriticalProbability(w, t) > 0 Then
						IsAbleToCounterAttack = True
						Exit Function
					End If
				End If
				' ADD END �}�[�W
				
				' DEL START �}�[�W
				'            '�n�`�K���́H
				'            If .WeaponAdaption(w, t.Area) = 0 Then
				'                GoTo NextWeapon
				'            End If
				'
				'            '����U���͎�_�A�L���������j�b�g�ȊO�ɂ͌����Ȃ�
				'            If .IsWeaponClassifiedAs(w, "��") Then
				'                wclass = .WeaponClass(w)
				'                buf = t.strWeakness & t.strEffective
				'                For i = 1 To Len(buf)
				'                    ch = GetClassBundle(buf, i)
				'                    If ch <> "��" And ch <> "��" Then
				'                        If InStrNotNest(wclass, ch) > 0 Then
				'                            Exit For
				'                        End If
				'                    End If
				'                Next
				'                If i > Len(buf) Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'            '����U���͎w�葮������_�A�L���������j�b�g�ȊO�ɂ͌����Ȃ�
				'            idx = InStrNotNest(.WeaponClass(w), "��")
				'            If idx > 0 Then
				'                wclass = .WeaponClass(w)
				'                buf = t.strWeakness & t.strEffective
				'                For i = 1 To Len(buf)
				'                    ch = GetClassBundle(buf, i)
				'                    If ch <> "��" And ch <> "��" Then
				'                        If InStrNotNest(wclass, ch) > idx Then
				'                            Exit For
				'                        End If
				'                    End If
				'                Next
				'                If i > Len(buf) Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'            '���背�x������U��
				'            If .IsWeaponClassifiedAs(w, "��") Then
				'                If t.MainPilot.Level Mod .WeaponLevel(w, "��") <> 0 Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'            '�����Ɏg�p�ł��镐�킪��������
				'            IsAbleToCounterAttack = True
				'            Exit Function
				' DEL END �}�[�W
NextWeapon: 
			Next 
		End With
		
		'�����Ɏg�p�ł��镐�킪�Ȃ�����
		IsAbleToCounterAttack = False
	End Function
	
	'�ł��߂��G���j�b�g��T��
	Public Function SearchNearestEnemy(ByRef u As Unit) As Unit
		Dim distance As Short
		Dim i, j As Short
		Dim t As Unit
		
		distance = 1000
		
		With u
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					t = MapDataForUnit(i, j)
					
					If t Is Nothing Then
						GoTo NexLoop
					End If
					
					'�����Ƌ߂��ɂ���G�𔭌��ς݁H
					If distance <= System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) Then
						GoTo NexLoop
					End If
					
					'�G�H
					If .IsAlly(t) Then
						GoTo NexLoop
					End If
					
					'����̐w�c�݂̂�_���v�l���[�h�̏ꍇ
					If .Mode = "����" Or .Mode = "�m�o�b" Or .Mode = "�G" Or .Mode = "����" Then
						If t.Party <> .Mode Then
							GoTo NexLoop
						End If
					End If
					
					'�ڎ��s�\�H
					If t.IsUnderSpecialPowerEffect("�B��g") Or t.Area = "�n��" Then
						GoTo NexLoop
					End If
					
					'�X�e���X��Ԃɂ���Ή�������͔����ł��Ȃ�
					If t.IsFeatureAvailable("�X�e���X") And Not t.IsConditionSatisfied("�X�e���X����") And Not .IsFeatureAvailable("�X�e���X������") Then
						If t.IsFeatureLevelSpecified("�X�e���X") Then
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > t.FeatureLevel("�X�e���X") Then
								GoTo NexLoop
							End If
						Else
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > 3 Then
								GoTo NexLoop
							End If
						End If
					End If
					
					'�^�[�Q�b�g�𔭌�
					SearchNearestEnemy = t
					distance = System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y)
					
NexLoop: 
				Next 
			Next 
		End With
	End Function
	
	'�ł��߂��G���j�b�g�ւ̋�����Ԃ�
	Private Function DistanceFromNearestEnemy(ByRef u As Unit) As Short
		Dim t As Unit
		
		t = SearchNearestEnemy(u)
		
		If Not t Is Nothing Then
			DistanceFromNearestEnemy = System.Math.Abs(u.X - t.X) + System.Math.Abs(u.Y - t.Y)
		Else
			DistanceFromNearestEnemy = 1000
		End If
	End Function
End Module