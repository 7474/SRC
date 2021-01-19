Option Strict Off
Option Explicit On
Module COM
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'コンピューターの思考ルーチン関連モジュール
	
	
	'コンピューターによるユニット操作(１行動)
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
		'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTarget = Nothing
		'UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SelectedTargetForEvent = Nothing
		SelectedWeapon = 0
		SelectedTWeapon = 0
		SelectedAbility = 0
		SelectedUnitMoveCost = 0
		
		'まずはUpdate
		SelectedUnit.Update()
		
		'行動出来なければそのまま終了
		If SelectedUnit.MaxAction = 0 Then
			Exit Sub
		End If
		
		'踊っている？
		If SelectedUnit.IsConditionSatisfied("踊り") Then
			'踊りに忙しい……
			Exit Sub
		End If
		
		'スペシャルパワーを使う？
		If IsOptionDefined("敵ユニットスペシャルパワー使用") Or IsOptionDefined("敵ユニット精神コマンド使用") Then
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
			If SelectedUnit.IsFeatureAvailable("追加サポート") Then
				TrySpecialPower(SelectedUnit.AdditionalSupport)
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			End If
		End If
		
		'ハイパーモードが可能であればハイパーモード発動
		TryHyperMode()
		
		'特殊な思考モードの場合の処理
		With SelectedUnit
			'指定された地点を目指す場合
			If LLength(.Mode) = 2 Then
				dst_x = CShort(LIndex(.Mode, 1))
				dst_y = CShort(LIndex(.Mode, 2))
				If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
					GoTo Move
				End If
			End If
			
			'逃亡し続ける場合
			If .Mode = "逃亡" Then
				GoTo Move
			End If
			
			'思考モードが「パイロット名」の場合の処理
			If Not PList.IsDefined(.Mode) Then
				GoTo TryBattleTransform
			End If
			If PList.Item(.Mode).Unit_Renamed Is Nothing Then
				GoTo TryBattleTransform
			End If
			If PList.Item(.Mode).Unit_Renamed.Status_Renamed <> "出撃" Then
				GoTo TryBattleTransform
			End If
			SelectedTarget = PList.Item(.Mode).Unit_Renamed
			AreaInSpeed(SelectedUnit)
			If Not .IsAlly(SelectedTarget) Then
				'ユニットが敵の場合はそのユニットを狙う
				w = SelectWeapon(SelectedUnit, SelectedTarget, "移動可能")
				If w = 0 Then
					dst_x = SelectedTarget.X
					dst_y = SelectedTarget.Y
					GoTo Move
				End If
			Else
				'ユニットが味方の場合はそのユニットを護衛
				w = 0
				distance = 1000
				dst_x = SelectedTarget.X
				dst_y = SelectedTarget.Y
				max_prob = 0
				max_dmg = 0
				
				'護衛対象が損傷している場合は修理装置を使う
				If TryFix(moved, SelectedTarget) Then
					GoTo EndOfOperation
				End If
				
				'護衛対象が損傷している場合は回復アビリティを使う
				If TryHealing(moved, SelectedTarget) Then
					GoTo EndOfOperation
				End If
				
				'合体技や援護防御を持っている場合はとにかく護衛対象に
				'隣接することを優先する
				If .MainPilot.IsSkillAvailable("援護") Or .MainPilot.IsSkillAvailable("援護防御") Then
					If System.Math.Abs(.X - dst_x) + System.Math.Abs(.Y - dst_y) > 1 Then
						GoTo Move
					End If
					guard_unit_mode = True
				End If
				If .IsFeatureAvailable("合体技") Then
					If System.Math.Abs(.X - dst_x) > 1 Or System.Math.Abs(.Y - dst_y) > 1 Then
						GoTo Move
					End If
					guard_unit_mode = True
				End If
				If guard_unit_mode Then
					'ちゃんと隣接しているので周りの敵を排除
					'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					SelectedTarget = Nothing
					GoTo TryBattleTransform
				End If
				
				'護衛するユニットを脅かすユニットが存在するかチェック
				For	Each u In UList
					With u
						If .Status_Renamed = "出撃" And SelectedUnit.IsEnemy(u) And System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) <= 5 Then
							tmp_w = SelectWeapon(SelectedUnit, u, "移動可能", prob, dmg)
						Else
							tmp_w = 0
						End If
						
						If tmp_w > 0 Then
							'脅威となり得るユニットと認定
							If distance > System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then
								'近い位置にいるユニットを優先
								SelectedTarget = u
								w = tmp_w
								distance = System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y)
								max_prob = prob
								max_dmg = dmg
							ElseIf distance = System.Math.Abs(dst_x - .X) + System.Math.Abs(dst_y - .Y) Then 
								'今までに見つかったユニットと位置が変わらなければ
								'より危険度が高いユニットを優先
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
					'護衛するユニットは安全。護衛するユニットの近くへ移動する
					GoTo Move
				Else
					'護衛するユニットを脅かすユニットに攻撃
					GoTo AttackEnemy
				End If
			End If
		End With
		
TryBattleTransform: 
		'戦闘形態への変形が可能であれば変形
		If TryBattleTransform() Then
			transfered = True
			'既にターゲットを選択している場合は攻撃方法を再選択
			If w > 0 Then
				w = SelectWeapon(SelectedUnit, SelectedTarget, "移動可能")
				If w = 0 Then
					'変形の結果、攻撃できなくなってしまった……
					dst_x = SelectedTarget.X
					dst_y = SelectedTarget.Y
					GoTo Move
				End If
			End If
		End If
		
		'実行時間を消費しないアビリティがあれば使っておく
		TryInstantAbility()
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		With SelectedUnit
			If .HP = 0 Or .MaxAction = 0 Then
				GoTo EndOfOperation
			End If
		End With
		
		'既に目標が決まっていればその目標を攻撃
		If Not SelectedTarget Is Nothing Then
			GoTo AttackEnemy
		End If
		
		'召喚が可能であれば召喚
		If TrySummonning() Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
		'修理が可能であれば修理装置を使う
		If TryFix(moved) Then
			GoTo EndOfOperation
		End If
		
		'マップ型回復アビリティを使う？
		If TryMapHealing(moved) Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
		'回復アビリティを使う？
		If TryHealing(moved) Then
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			GoTo EndOfOperation
		End If
		
TryMapAttack: 
		'マップ攻撃を使う？
		If TryMapAttack(moved) Then
			GoTo EndOfOperation
		End If
		
SearchNearestEnemyWithinRange: 
		'ターゲットにするユニットを探す
		With SelectedUnit
			AreaInSpeed(SelectedUnit)
			
			'護衛すべきユニットがいる場合は移動範囲を限定
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
			
			'個々のユニットに対してターゲットとなり得るか判定
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
			w = 0
			max_prob = 0
			max_dmg = 0
			For	Each u In UList
				If u.Status_Renamed <> "出撃" Then
					GoTo NextLoop
				End If
				
				'敵かどうかを判定
				If .IsAlly(u) Then
					GoTo NextLoop
				End If
				
				'特定の陣営のみを狙う思考モードの場合
				Select Case .Mode
					Case "味方"
						If u.Party <> "味方" And u.Party <> "ＮＰＣ" Then
							GoTo NextLoop
						End If
					Case "ＮＰＣ", "敵", "中立"
						If u.Party <> .Mode Then
							GoTo NextLoop
						End If
				End Select
				
				'自分自身には攻撃しない
				If SelectedUnit.CurrentForm Is u.CurrentForm Then
					GoTo NextLoop
				End If
				
				'隠れ身中
				If u.IsUnderSpecialPowerEffect("隠れ身") Then
					GoTo NextLoop
				End If
				
				'ステルスの敵は遠距離からは攻撃を受けない
				If u.IsFeatureAvailable("ステルス") And Not u.IsConditionSatisfied("ステルス無効") And Not .IsFeatureAvailable("ステルス無効化") Then
					If u.IsFeatureLevelSpecified("ステルス") Then
						If System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) > u.FeatureLevel("ステルス") Then
							GoTo NextLoop
						End If
					Else
						If System.Math.Abs(.X - u.X) + System.Math.Abs(.Y - u.Y) > 3 Then
							GoTo NextLoop
						End If
					End If
				End If
				
				'攻撃に使う武器を選択
				If moved Then
					tmp_w = SelectWeapon(SelectedUnit, u, "移動後", prob, dmg)
				Else
					tmp_w = SelectWeapon(SelectedUnit, u, "移動可能", prob, dmg)
				End If
				If tmp_w <= 0 Then
					GoTo NextLoop
				End If
				
				'サポートガードされる？
				If .MainPilot.TacticalTechnique >= 150 Then
					If Not u.LookForSupportGuard(SelectedUnit, tmp_w) Is Nothing Then
						'相手を破壊することは出来ない
						prob = 0
						'仮想的にダメージを半減して判定
						dmg = dmg \ 2
					End If
				End If
				
				'間接攻撃？
				indirect_attack = .IsWeaponClassifiedAs(w, "間")
				
				'召喚ユニットは自分がやられてしまうような攻撃はかけない
				If .Party = "ＮＰＣ" And .IsFeatureAvailable("召喚ユニット") And Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("狂戦士") And Not indirect_attack Then
					tw = SelectWeapon(u, SelectedUnit, "反撃", tprob, tdmg)
					If prob < 80 And tprob > prob Then
						GoTo NextLoop
					End If
				End If
				
				'破壊確率が50%以上であれば破壊確率が高いユニットを優先
				'そうでなければダメージの期待値が高いユニットを優先
				If prob > 50 Then
					'重要なユニットは優先してターゲットにする
					If .MainPilot.TacticalTechnique >= 150 Then
						With u
							If .MainPilot.IsSkillAvailable("指揮") Or .MainPilot.IsSkillAvailable("広域サポート") Or .IsFeatureAvailable("修理装置") Then
								prob = 1.5 * prob
							Else
								'回復アビリティを持っている？
								For i = 1 To .CountAbility
									With .Ability(i)
										If .MaxRange > 0 Then
											If .CountEffect > 0 Then
												If .EffectType(1) = "回復" Then
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
					'相手の反撃手段もチェック
					tw = 0
					For i = 1 To u.CountWeapon
						If u.IsWeaponAvailable(i, "移動前") And Not u.IsWeaponClassifiedAs(i, "Ｍ") Then
							If Not moved And .Mode <> "固定" And .IsWeaponClassifiedAs(tmp_w, "移動後攻撃可") Then
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
					
					'間接攻撃には反撃不能
					If indirect_attack Then
						tw = 0
					End If
					
					'ステータス異常により反撃不能？
					If u.MaxAction = 0 Or u.IsConditionSatisfied("攻撃不能") Then
						tw = 0
					End If
					
					'反撃してこない？
					If tw = 0 Then
						dmg = 1.5 * dmg
					End If
					
					'重要なユニットは優先してターゲットにする
					If .MainPilot.TacticalTechnique >= 150 Then
						With u
							If .MainPilot.IsSkillAvailable("指揮") Or .MainPilot.IsSkillAvailable("広域サポート") Or .IsFeatureAvailable("修理装置") Then
								'メインパイロットが指揮や広域サポートを有していたり
								'修理装置を持っていれば重要ユニットと認定
								dmg = 1.5 * dmg
							Else
								'回復アビリティを持っている場合も重要ユニットと認定
								For i = 1 To .CountAbility
									With .Ability(i)
										If .MaxRange > 0 Then
											If .CountEffect > 0 Then
												If .EffectType(1) = "回復" Then
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
						'現在のユニットをターゲットに設定
						SelectedTarget = u
						w = tmp_w
						max_dmg = dmg
					End If
				End If
NextLoop: 
			Next u
			
			'射程内に敵がいなければ移動、もしくは待機
			If SelectedTarget Is Nothing Then
				If .Mode = "待機" Or .Mode = "固定" Or LLength(.Mode) = 2 Then
					GoTo EndOfOperation
				End If
				
				If moved Then
					'既に移動済みであればここで終了
					GoTo EndOfOperation
				End If
				
				If searched_enemy Then
					'既に索敵済みであればここで終了
					GoTo EndOfOperation
				End If
				
				'一度索敵をしたことを記録
				searched_enemy = True
				
				'一番近い敵の方へ移動する
				GoTo SearchNearestEnemy
			End If
			searched_enemy = True
		End With
		
AttackEnemy: 
		'敵を攻撃
		
		'敵をUpdate
		SelectedTarget.Update()
		
		'敵の位置を記録しておく
		tx = SelectedTarget.X
		ty = SelectedTarget.Y
		
		Dim list() As String
		Dim caption_msg As String
		Dim hit_prob, crit_prob As Short
		With SelectedUnit
			'移動後攻撃可能な武器の場合は攻撃前に移動を行う
			'ただし合体技は移動後の位置によって攻撃できない場合があるので例外
			If .IsWeaponClassifiedAs(w, "移動後攻撃可") And Not .IsWeaponClassifiedAs(w, "合") And Not moved And .Mode <> "固定" Then
				'移動しなくても攻撃出来る場合は現在位置をデフォルトの攻撃位置に設定
				If .IsTargetWithinRange(w, SelectedTarget) Then
					new_locations_value = TerrainEffectForHPRecover(.X, .Y) + TerrainEffectForENRecover(.X, .Y) + 100 * .LookForSupport(.X, .Y, True)
					If .Area <> "空中" Then
						'地形による防御効果は空中にいる場合は受けられない
						new_locations_value = new_locations_value + TerrainEffectForHit(.X, .Y) + TerrainEffectForDamage(.X, .Y)
					End If
					new_x = .X
					new_y = .Y
				Else
					new_locations_value = -1000
					new_x = 0
					new_y = 0
				End If
				
				'攻撃をかけられる位置のうち、もっとも地形効果の高い場所を探す
				'地形効果が同等ならもっとも近い場所を優先
				max_range = .WeaponMaxRange(w)
				min_range = .Weapon(w).MinRange
				For i = MaxLng(1, tx - max_range) To MinLng(tx + max_range, MapWidth)
					For j = MaxLng(1, ty - (max_range - System.Math.Abs(tx - i))) To MinLng(ty + (max_range - System.Math.Abs(tx - i)), MapHeight)
						If Not MaskData(i, j) And MapDataForUnit(i, j) Is Nothing And System.Math.Abs(tx - i) + System.Math.Abs(ty - j) >= min_range Then
							tmp = TerrainEffectForHPRecover(i, j) + TerrainEffectForENRecover(i, j) + 100 * .LookForSupport(i, j, True)
							
							If .Area <> "空中" Then
								'地形による防御効果は空中にいる場合は受けられない
								tmp = tmp + TerrainEffectForHit(i, j) + TerrainEffectForDamage(i, j)
								
								'水中は水中用ユニットでない限り選択しない
								If TerrainClass(i, j) = "水" Then
									If .IsTransAvailable("水") Then
										tmp = tmp + 100
									Else
										tmp = -1000
									End If
								End If
							End If
							
							'条件が同じであれば直線距離で近い場所を選択する
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
					'攻撃をかけられる位置がない
					If searched_nearest_enemy Then
						'既に索敵済みであればここで終了
						GoTo EndOfOperation
					End If
					GoTo SearchNearestEnemy
				End If
				
				'見つけた位置に移動
				If new_x <> .X Or new_y <> .Y Then
					.Move(new_x, new_y)
					SelectedUnitMoveCost = TotalMoveCost(new_x, new_y)
					moved = True
					
					'移動のためＥＮ切れ？
					If .EN = 0 Then
						If .MaxAction = 0 Then
							GoTo EndOfOperation
						End If
					End If
					
					'実はマップ攻撃が使える？
					If TryMapAttack(True) Then
						GoTo EndOfOperation
					End If
					
					'移動のために選択していた武器が使えなくなったり、合体技が使える
					'ようになったりすることがあるので、武器を再度選択
					w = SelectWeapon(SelectedUnit, SelectedTarget, "移動後")
					If w = 0 Then
						'攻撃出来ないので行動終了
						GoTo EndOfOperation
					End If
				End If
			End If
			
			'ユニットを中央表示
			Center(.X, .Y)
			
			'ハイライト表示を行う
			If Not BattleAnimation Then
				'射程範囲をハイライト
				' MOD START マージ
				'            AreaInRange .X, .Y, _
				''                .Weapon(w).MinRange, _
				''                .WeaponMaxRange(w), _
				''                "空間"
				AreaInRange(.X, .Y, .WeaponMaxRange(w), .Weapon(w).MinRange, "空間")
				' MOD END マージ
			End If
			'合体技の場合はパートナーもハイライト表示
			If .IsWeaponClassifiedAs(w, "合") Then
				If .WeaponMaxRange(w) = 1 Then
					.CombinationPartner("武装", w, partners, SelectedTarget.X, SelectedTarget.Y)
				Else
					.CombinationPartner("武装", w, partners)
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
				'自分自身とターゲットもハイライト
				MaskData(.X, .Y) = False
				MaskData(SelectedTarget.X, SelectedTarget.Y) = False
				
				'ハイライト表示を実施
				MaskScreen()
			Else
				'戦闘アニメを表示する場合はハイライト表示を行わない
				RefreshScreen()
			End If
			
			'ＢＧＭを変更
			If Not KeepEnemyBGM Then
				BGM = ""
				
				'ボス用ＢＧＭ？
				If .IsFeatureAvailable("ＢＧＭ") And InStr(.MainPilot.Name, "(ザコ)") = 0 Then
					BGM = SearchMidiFile(.FeatureData("ＢＧＭ"))
				End If
				
				BossBGM = False
				If Len(BGM) > 0 Then
					'ボス用ＢＧＭを演奏する場合
					ChangeBGM(BGM)
					BossBGM = True
				Else
					'通常の戦闘ＢＧＭ
					
					'ターゲットは味方？
					If SelectedTarget.Party = "味方" Or (SelectedTarget.Party = "ＮＰＣ" And .Party <> "ＮＰＣ") Then
						'ターゲットが味方なのでターゲット側を優先
						If SelectedTarget.IsFeatureAvailable("ＢＧＭ") Then
							BGM = SearchMidiFile(SelectedTarget.FeatureData("ＢＧＭ"))
						End If
						If Len(BGM) = 0 Then
							BGM = SearchMidiFile(SelectedTarget.MainPilot.BGM)
						End If
					Else
						'ターゲットが味方でなければ攻撃側を優先
						If .IsFeatureAvailable("ＢＧＭ") Then
							BGM = SearchMidiFile(.FeatureData("ＢＧＭ"))
						End If
						If Len(BGM) = 0 Then
							BGM = SearchMidiFile(.MainPilot.BGM)
						End If
					End If
					If Len(BGM) = 0 Then
						BGM = BGMName("default")
					End If
					
					'ＢＧＭを変更
					ChangeBGM(BGM)
				End If
			End If
			
			'移動後攻撃可能？
			is_p_weapon = .IsWeaponClassifiedAs(w, "移動後攻撃可")
			
			'間接攻撃？
			indirect_attack = .IsWeaponClassifiedAs(w, "間")
			
			'相手の反撃手段を設定
			def_mode = ""
			UseSupportGuard = True
			If SelectedTarget.MaxAction = 0 Then
				'行動不能の場合
				
				tw = -1
				'チャージ中または消耗している場合は自動的に防御
				If SelectedTarget.Party = "味方" And (SelectedTarget.IsFeatureAvailable("チャージ") Or SelectedTarget.IsFeatureAvailable("消耗")) Then
					def_mode = "防御"
				End If
				
				'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ElseIf SelectedTarget.Party = "味方" And Not MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then 
				'味方ユニットによる手動反撃を行う場合
				
				'戦闘アニメを表示する場合でも手動反撃時にはハイライト表示を行う
				If BattleAnimation Then
					'射程範囲をハイライト
					' MOD START マージ
					'                AreaInRange .X, .Y, _
					''                    .Weapon(w).MinRange, _
					''                    .WeaponMaxRange(w), _
					''                    "空間"
					AreaInRange(.X, .Y, .WeaponMaxRange(w), .Weapon(w).MinRange, "空間")
					' MOD END マージ
					
					'合体技の場合はパートナーもハイライト表示
					If .IsWeaponClassifiedAs(w, "合") Then
						For i = 1 To UBound(partners)
							With partners(i)
								MaskData(.X, .Y) = False
							End With
						Next 
					End If
					
					'自分自身とターゲットもハイライト
					MaskData(.X, .Y) = False
					MaskData(SelectedTarget.X, SelectedTarget.Y) = False
					
					'ハイライト表示を実施
					MaskScreen()
				End If
				
				
				hit_prob = .HitProbability(w, SelectedTarget, True)
				crit_prob = .CriticalProbability(w, SelectedTarget)
				caption_msg = "反撃：" & .WeaponNickname(w) & " 攻撃力=" & VB6.Format(.WeaponPower(w, ""))
				If Not IsOptionDefined("予測命中率非表示") Then
					caption_msg = caption_msg & " 命中率=" & VB6.Format(MinLng(hit_prob, 100)) & "％（" & crit_prob & "％）"
				End If
				
				ReDim list(3)
				
				If IsAbleToCounterAttack(SelectedTarget, SelectedUnit) And Not indirect_attack Then
					list(1) = "反撃"
				Else
					list(1) = "反撃不能"
				End If
				If Not IsOptionDefined("予測命中率非表示") Then
					list(2) = "防御：命中率＝" & VB6.Format(MinLng(hit_prob, 100)) & "％（" & .CriticalProbability(w, SelectedTarget, "防御") & "％）"
					list(3) = "回避：命中率＝" & VB6.Format(MinLng(hit_prob \ 2, 100)) & "％（" & .CriticalProbability(w, SelectedTarget, "回避") & "％）"
				Else
					list(2) = "防御"
					list(3) = "回避"
				End If
				
				'援護防御が受けられる？
				SupportGuardUnit = SelectedTarget.LookForSupportGuard(SelectedUnit, w)
				If Not SupportGuardUnit Is Nothing Then
					ReDim Preserve list(4)
					If IsOptionDefined("等身大基準") Then
						list(4) = "援護防御：使用する (" & SupportGuardUnit.Nickname & ")"
					Else
						list(4) = "援護防御：使用する (" & SupportGuardUnit.Nickname & "/" & SupportGuardUnit.MainPilot.Nickname & ")"
					End If
					UseSupportGuard = True
				End If
				
				AddPartsToListBox()
				Do 
					'攻撃への対応手段を選択
					With SelectedTarget
						ReDim ListItemFlag(UBound(list))
						'各対抗手段が選択可能か判定
						
						'反撃が選択可能？
						If list(1) = "反撃" Then
							ListItemFlag(1) = False
							tw = -1
						Else
							ListItemFlag(1) = True
							tw = 0
						End If
						
						'防御が選択可能？
						If .IsFeatureAvailable("防御不可") Then
							ListItemFlag(2) = True
						Else
							ListItemFlag(2) = False
						End If
						
						'回避が選択可能？
						If .IsFeatureAvailable("回避不可") Or .IsConditionSatisfied("移動不能") Then
							ListItemFlag(3) = True
						Else
							ListItemFlag(3) = False
						End If
						
						'対応手段を選択
						TopItem = 1
						i = ListBox(caption_msg, list, .Nickname0 & " " & .MainPilot.Nickname, "連続表示,カーソル移動")
					End With
					
					Select Case i
						Case 1
							'反撃を選択した場合は反撃に使う武器を選択
							buf = "反撃：" & .WeaponNickname(w) & " 攻撃力=" & VB6.Format(.WeaponPower(w, ""))
							If Not IsOptionDefined("予測命中率非表示") Then
								buf = buf & " 命中率=" & VB6.Format(MinLng(hit_prob, 100)) & "％（" & crit_prob & "％）" & " ： "
							End If
							With SelectedTarget.MainPilot
								buf = buf & .Nickname & " " & Term("格闘", SelectedTarget) & VB6.Format(.Infight) & " "
								If .HasMana() Then
									buf = buf & Term("魔力", SelectedTarget) & VB6.Format(.Shooting)
								Else
									buf = buf & Term("射撃", SelectedTarget) & VB6.Format(.Shooting)
								End If
							End With
							
							tw = WeaponListBox(SelectedTarget, buf, "反撃")
							
							If tw = 0 Then
								i = 0
							End If
						Case 2
							'防御を選択した
							def_mode = "防御"
						Case 3
							'回避を選択した
							def_mode = "回避"
						Case 4
							'援護防御を使用するかどうかを切り替えた
							UseSupportGuard = Not UseSupportGuard
							If UseSupportGuard Then
								list(4) = "援護防御：使用する ("
							Else
								list(4) = "援護防御：使用しない ("
							End If
							If IsOptionDefined("等身大基準") Then
								list(4) = list(4) & SupportGuardUnit.Nickname & ")"
							Else
								list(4) = list(4) & SupportGuardUnit.Nickname & "/" & SupportGuardUnit.MainPilot.Nickname & ")"
							End If
							i = 0
						Case Else
							'反撃・防御・回避の全てが選択出来ない？
							If ListItemFlag(1) And ListItemFlag(2) And ListItemFlag(3) Then
								Exit Do
							End If
					End Select
				Loop While i = 0
				
				'反撃手段選択終了
				frmListBox.Hide()
				RemovePartsOnListBox()
				
				'ハイライト表示を消去
				If BattleAnimation Then
					RefreshScreen()
				End If
			Else
				'コンピューターが操作するユニット及び自動反撃モードの場合
				
				'反撃に使う武器を選択
				tw = SelectWeapon(SelectedTarget, SelectedUnit, "反撃")
				If indirect_attack Then
					tw = 0
				End If
				
				'防御を選択する？
				'UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				def_mode = SelectDefense(SelectedUnit, w, SelectedTarget, tw)
				If def_mode <> "" Then
					tw = -1
				End If
			End If
		End With
		
		'味方ユニットの場合は武器用ＢＧＭを演奏する
		If Not KeepEnemyBGM Then
			With SelectedTarget
				If .Party = "味方" And tw > 0 And .IsFeatureAvailable("武器ＢＧＭ") Then
					For i = 1 To .CountFeature
						If .Feature(i) = "武器ＢＧＭ" And LIndex(.FeatureData(i), 1) = .Weapon(tw).Name Then
							'武器用ＢＧＭが指定されていた
							BGM = SearchMidiFile(Mid(.FeatureData(i), InStr(.FeatureData(i), " ") + 1))
							If Len(BGM) > 0 Then
								'武器用ＢＧＭのMIDIが見つかったのでＢＧＭを変更
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
		
		' ADD START マージ
		'戦闘前に一旦クリア
		'UPGRADE_NOTE: オブジェクト SupportAttackUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportAttackUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit = Nothing
		'UPGRADE_NOTE: オブジェクト SupportGuardUnit2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		SupportGuardUnit2 = Nothing
		' ADD END マージ
		
		'武器の使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, wname)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		If tw > 0 Then
			twname = SelectedTarget.Weapon(tw).Name
			SaveSelections()
			SwapSelections()
			HandleEvent("使用", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
		End If
		
		'攻撃イベント
		HandleEvent("攻撃", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		'メッセージウィンドウを開く
		If Stage = "ＮＰＣ" Then
			OpenMessageForm(SelectedTarget, SelectedUnit)
		Else
			OpenMessageForm(SelectedUnit, SelectedTarget)
		End If
		
		'イベント用に戦闘に参加するユニットの情報を記録しておく
		AttackUnit = SelectedUnit
		attack_target = SelectedUnit
		attack_target_hp_ratio = SelectedUnit.HP / SelectedUnit.MaxHP
		defense_target = SelectedTarget
		defense_target_hp_ratio = SelectedTarget.HP / SelectedTarget.MaxHP
		'UPGRADE_NOTE: オブジェクト defense_target2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		defense_target2 = Nothing
		' DEL START マージ
		'    Set SupportAttackUnit = Nothing
		'    Set SupportGuardUnit = Nothing
		' DEL END マージ
		
		'相手の先制攻撃？
		With SelectedTarget
			' MOD START マージ
			'        If tw > 0 And .MaxAction > 0 Then
			' tw > 0の判定はIsWeaponAvailable内に
			If .MaxAction > 0 And .IsWeaponAvailable(tw, "移動前") Then
				' MOD END マージ
				If Not .IsWeaponClassifiedAs(tw, "後") Then
					If SelectedUnit.IsWeaponClassifiedAs(w, "後") Then
						def_mode = "先制攻撃"
						.Attack(tw, SelectedUnit, "先制攻撃", "")
						SelectedTarget = .CurrentForm
					ElseIf .IsWeaponClassifiedAs(tw, "先") Or .MainPilot.SkillLevel("先読み") >= Dice(16) Or .IsUnderSpecialPowerEffect("カウンター") Then 
						def_mode = "先制攻撃"
						.Attack(tw, SelectedUnit, "カウンター", "")
						SelectedTarget = .CurrentForm
					ElseIf .MaxCounterAttack > .UsedCounterAttack Then 
						def_mode = "先制攻撃"
						.UsedCounterAttack = .UsedCounterAttack + 1
						.Attack(tw, SelectedUnit, "カウンター", "")
						SelectedTarget = .CurrentForm
					End If
					
					'攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
					If Not SupportGuardUnit Is Nothing Then
						attack_target = SupportGuardUnit
						attack_target_hp_ratio = SupportGuardUnitHPRatio
					End If
				End If
			End If
		End With
		
		'サポートアタックのパートナーを探す
		With SelectedUnit
			If .Status_Renamed = "出撃" And SelectedTarget.Status_Renamed = "出撃" Then
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
			' MOD START マージ
			'        If .Status = "出撃" _
			''            And .MaxAction(True) > 0 _
			''            And SelectedTarget.Status = "出撃" _
			''        Then
			If .Status_Renamed = "出撃" And .MaxAction(True) > 0 And Not .IsConditionSatisfied("攻撃不能") And SelectedTarget.Status_Renamed = "出撃" Then
				' MOD END マージ
				'まだ武器は使用可能か？
				If w > .CountWeapon Then
					w = -1
				ElseIf wname <> .Weapon(w).Name Then 
					w = -1
				ElseIf moved Then 
					If Not .IsWeaponAvailable(w, "移動後") Then
						w = -1
					End If
				Else
					If Not .IsWeaponAvailable(w, "移動前") Then
						w = -1
					End If
				End If
				If w > 0 Then
					If Not .IsTargetWithinRange(w, SelectedTarget) Then
						w = 0
					End If
				End If
				
				'行動不能な場合
				If .MaxAction(True) = 0 Then
					w = -1
				End If
				
				'魅了された場合
				If .IsConditionSatisfied("魅了") And .Master Is SelectedTarget Then
					w = -1
				End If
				
				'憑依された場合
				If .IsConditionSatisfied("憑依") Then
					If .Master.Party = SelectedTarget.Party Then
						w = -1
					End If
				End If
				
				'踊らされた場合
				If .IsConditionSatisfied("踊り") Then
					w = -1
				End If
				
				If w > 0 Then
					'自爆攻撃？
					If .IsWeaponClassifiedAs(w, "自") Then
						is_suiside = True
					End If
					
					If Not SupportAttackUnit Is Nothing And .MaxSyncAttack > .UsedSyncAttack Then
						'同時援護攻撃
						.Attack(w, SelectedTarget, "統率", def_mode)
					Else
						'通常攻撃
						.Attack(w, SelectedTarget, "", def_mode)
					End If
				ElseIf w = 0 Then 
					'射程外
					If .IsAnimationDefined("射程外") Then
						.PlayAnimation("射程外")
					Else
						.SpecialEffect("射程外")
					End If
					.PilotMessage("射程外")
				End If
			Else
				w = -1
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
		
		With SelectedTarget
			'反撃の実行
			If def_mode <> "先制攻撃" Then
				If .Status_Renamed = "出撃" And SelectedUnit.Status_Renamed = "出撃" Then
					'まだ武器は使用可能か？
					If tw > 0 Then
						If tw > .CountWeapon Then
							tw = -1
						ElseIf twname <> .Weapon(tw).Name Or Not .IsWeaponAvailable(tw, "移動前") Then 
							tw = -1
						End If
					End If
					If tw > 0 Then
						If Not .IsTargetWithinRange(tw, SelectedUnit) Then
							'敵が射程外に逃げていたら武器を再選択
							tw = 0
						End If
					End If
					
					'行動不能な場合
					If .MaxAction = 0 Then
						tw = -1
					End If
					
					'魅了された場合
					If .IsConditionSatisfied("魅了") And .Master Is SelectedUnit Then
						tw = -1
					End If
					
					'憑依された場合
					If .IsConditionSatisfied("憑依") Then
						If .Master.Party = SelectedUnit.Party Then
							tw = -1
						End If
					End If
					
					'踊らされた場合
					If .IsConditionSatisfied("踊り") Then
						tw = -1
					End If
					
					If tw > 0 And def_mode = "" Then
						'反撃を実施
						.Attack(tw, SelectedUnit, "", "")
						If .Status_Renamed = "他形態" Then
							SelectedTarget = .CurrentForm
						End If
						If SelectedUnit.Status_Renamed = "他形態" Then
							SelectedUnit = SelectedUnit.CurrentForm
						End If
						
						'攻撃側のユニットがかばわれた場合は攻撃側のターゲットを再設定
						' MOD START マージ
						'                    If Not SupportGuardUnit Is Nothing Then
						'                        Set attack_target = SupportGuardUnit
						'                        attack_target_hp_ratio = SupportGuardUnitHPRatio
						'                    End If
						If Not SupportGuardUnit2 Is Nothing Then
							attack_target = SupportGuardUnit2
							attack_target_hp_ratio = SupportGuardUnitHPRatio2
						End If
						' MOD END マージ
					ElseIf tw = 0 And .X = tx And .Y = ty Then 
						'反撃出来る武器がなかった場合は射程外メッセージを表示
						If .IsAnimationDefined("射程外") Then
							.PlayAnimation("射程外")
						Else
							.SpecialEffect("射程外")
						End If
						.PilotMessage("射程外")
					Else
						tw = -1
					End If
				Else
					tw = -1
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
		
		'標的が味方の場合の経験値と資金獲得処理
		'(標的が味方が呼び出した召喚ユニットの場合も)
		SelectedUnit = SelectedUnit.CurrentForm
		Dim get_reward As Boolean
		With SelectedTarget
			
			'経験値＆資金が獲得できるか判定
			If .Party = "味方" And .Status_Renamed = "出撃" Then
				get_reward = True
			ElseIf Not .Summoner Is Nothing Then 
				If .Summoner.Party = "味方" And .Party0 = "ＮＰＣ" And .Status_Renamed = "出撃" And .IsFeatureAvailable("召喚ユニット") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("暴走") Then
					get_reward = True
				End If
			End If
			
			If get_reward Then
				If SelectedUnit.Status_Renamed = "破壊" And Not is_suiside Then
					'経験値を獲得
					.GetExp(SelectedUnit, "破壊")
					
					'現在の資金を記録
					prev_money = Money
					
					'獲得する資金を算出
					earnings = SelectedUnit.Value \ 2
					
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
					
					If Money > prev_money Then
						DisplaySysMessage(VB6.Format(Money - prev_money) & "の" & Term("資金", SelectedUnit) & "を得た。")
					End If
				Else
					.GetExp(SelectedUnit, "攻撃")
				End If
			End If
			
			'スペシャルパワー「獲得資金増加」「獲得経験値増加」の効果はここで削除する
			.RemoveSpecialPowerInEffect("戦闘終了")
			If earnings > 0 Then
				.RemoveSpecialPowerInEffect("敵破壊")
			End If
		End With
		
		'味方が呼び出した召喚ユニットの場合はＮＰＣでも経験値と資金を獲得
		SelectedUnit = SelectedUnit.CurrentForm
		With SelectedUnit
			If Not .Summoner Is Nothing Then
				If .Summoner.Party = "味方" And .Party0 = "ＮＰＣ" And .Status_Renamed = "出撃" And .IsFeatureAvailable("召喚ユニット") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("暴走") Then
					If SelectedTarget.Status_Renamed = "破壊" Then
						'ターゲットを破壊した場合
						
						'経験値を獲得
						.GetExp(SelectedTarget, "破壊")
						
						'獲得する資金を算出
						earnings = SelectedTarget.Value \ 2
						
						'スペシャルパワーによる獲得資金増加
						If .IsUnderSpecialPowerEffect("獲得資金増加") Then
							earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("獲得資金増加"))
						End If
						
						'パイロット能力による獲得資金増加
						If .IsSkillAvailable("資金獲得") Then
							If Not .IsUnderSpecialPowerEffect("獲得資金増加") Or IsOptionDefined("収得効果重複") Then
								earnings = earnings * (10 + .SkillLevel("資金獲得", 5)) \ 10
							End If
						End If
						
						'資金を獲得
						IncrMoney(earnings)
						If earnings > 0 Then
							DisplaySysMessage(VB6.Format(earnings) & "の" & Term("資金", SelectedTarget) & "を得た。")
						End If
					Else
						'ターゲットを破壊出来なかった場合
						
						'経験値を獲得
						.GetExp(SelectedTarget, "攻撃")
					End If
				End If
			End If
			
			If .Status_Renamed = "出撃" Then
				'スペシャルパワー効果「敵破壊時再行動」
				If .IsUnderSpecialPowerEffect("敵破壊時再行動") Then
					If SelectedTarget.Status_Renamed = "破壊" Then
						.UsedAction = .UsedAction - 1
					End If
				End If
				
				'持続期間が「戦闘終了」のスペシャルパワー効果を削除
				.RemoveSpecialPowerInEffect("戦闘終了")
				If earnings > 0 Then
					.RemoveSpecialPowerInEffect("敵破壊")
				End If
			End If
		End With
		
		CloseMessageForm()
		
		RedrawScreen()
		
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
		
		If SelectedWeapon <= 0 Then
			SelectedWeaponName = ""
		End If
		If SelectedTWeapon <= 0 Then
			SelectedTWeaponName = ""
		End If
		
		'破壊＆損傷率イベント発生
		
		'攻撃を受けた攻撃側ユニット
		With attack_target.CurrentForm
			If .CountPilot > 0 Then
				If .Status_Renamed = "破壊" Then
					HandleEvent("破壊", .MainPilot.ID)
				ElseIf .Status_Renamed = "出撃" And .HP / .MaxHP < attack_target_hp_ratio Then 
					HandleEvent("損傷率", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP)
				End If
				If IsScenarioFinished Or IsCanceled Then
					Exit Sub
				End If
			End If
		End With
		
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
			RestoreSelections()
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
		
		'元に戻す
		RestoreSelections()
		
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'武器の使用後イベント
		If SelectedUnit.Status_Renamed = "出撃" And w > 0 Then
			HandleEvent("使用後", SelectedUnit.MainPilot.ID, wname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		If SelectedTarget.Status_Renamed = "出撃" And tw > 0 Then
			SaveSelections()
			SwapSelections()
			HandleEvent("使用後", SelectedUnit.MainPilot.ID, twname)
			RestoreSelections()
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		'攻撃後イベント
		If SelectedUnit.Status_Renamed = "出撃" And SelectedTarget.Status_Renamed = "出撃" Then
			HandleEvent("攻撃後", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		'もし敵が移動していれば進入イベント
		With SelectedTarget
			If .Status_Renamed = "出撃" Then
				If .X <> tx Or .Y <> ty Then
					HandleEvent("進入", .MainPilot.ID, .X, .Y)
					If IsScenarioFinished Or IsCanceled Then
						ReDim SelectedPartners(0)
						Exit Sub
					End If
				End If
			End If
		End With
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		
		'再移動
		If is_p_weapon And SelectedUnit.Status_Renamed = "出撃" Then
			If SelectedUnit.MainPilot.IsSkillAvailable("遊撃") And SelectedUnit.Speed * 2 > SelectedUnitMoveCost Then
				'進入イベント
				If SelectedUnitMoveCost > 0 Then
					HandleEvent("進入", SelectedUnit.MainPilot.ID, SelectedUnit.X, SelectedUnit.Y)
					If IsScenarioFinished Then
						Exit Sub
					End If
				End If
				
				'ユニットが既に出撃していない？
				If SelectedUnit.Status_Renamed <> "出撃" Then
					Exit Sub
				End If
				
				took_action = True
				
				AreaInSpeed(SelectedUnit)
				
				'目標地点が設定されている？
				If LLength((SelectedUnit.Mode)) = 2 Then
					dst_x = CShort(LIndex((SelectedUnit.Mode), 1))
					dst_y = CShort(LIndex((SelectedUnit.Mode), 2))
					If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
						GoTo Move
					End If
				End If
				
				'そうでなければ安全な場所へ
				SafetyPoint(SelectedUnit, dst_x, dst_y)
				
				GoTo Move
			End If
		End If
		
		'行動終了
		GoTo EndOfOperation
		
SearchNearestEnemy: 
		
		'もっとも近くにいる敵を探す
		searched_nearest_enemy = True
		SelectedTarget = SearchNearestEnemy(SelectedUnit)
		
		'ターゲットが見つからなければあきらめて終了
		If SelectedTarget Is Nothing Then
			GoTo EndOfOperation
		End If
		
		'見つかったらターゲットの位置を目標地点にして移動
		dst_x = SelectedTarget.X
		dst_y = SelectedTarget.Y
		
Move: 
		
		'目標地点
		SelectedX = dst_x
		SelectedY = dst_y
		
		'移動形態への変形が可能であれば変形
		If Not transfered Then
			If TryMoveTransform() Then
				transfered = True
			End If
		End If
		
		With SelectedUnit
			'移動可能範囲を設定
			
			'テレポート能力を使える場合は優先的に使用
			If LLength(.FeatureData("テレポート")) = 2 Then
				tmp = CShort(LIndex(.FeatureData("テレポート"), 2))
			Else
				tmp = 40
			End If
			If .IsFeatureAvailable("テレポート") And (.EN > 10 * tmp Or .EN - tmp > .MaxEN \ 2) And SelectedUnitMoveCost = 0 Then
				mmode = "テレポート"
				.EN = .EN - tmp
				AreaInTeleport(SelectedUnit)
				GoTo MoveAreaSelected
			End If
			
			'ジャンプ能力を使う？
			If LLength(.FeatureData("ジャンプ")) = 2 Then
				tmp = CShort(LIndex(.FeatureData("ジャンプ"), 2))
			Else
				tmp = 0
			End If
			If .IsFeatureAvailable("ジャンプ") And .Area <> "空中" And .Area <> "宇宙" And (.EN > 10 * tmp Or .EN - tmp > .MaxEN \ 2) And SelectedUnitMoveCost = 0 Then
				mmode = "ジャンプ"
				.EN = .EN - tmp
				AreaInSpeed(SelectedUnit, True)
				GoTo MoveAreaSelected
			End If
			
			'通常移動
			mmode = ""
			AreaInSpeed(SelectedUnit)
			
MoveAreaSelected: 
			
			'護衛すべきユニットがいる場合は動ける範囲を限定
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
			
			If .Mode = "逃亡" Then
				'移動可能範囲内で敵から最も遠い場所を検索
				SafetyPoint(SelectedUnit, dst_x, dst_y)
				new_x = dst_x
				new_y = dst_y
			ElseIf .IsConditionSatisfied("混乱") Then 
				'移動可能範囲内からランダムに行き先を選択
				dst_x = .X + Dice(.Speed + 1) - Dice(.Speed + 1)
				dst_y = .Y + Dice(.Speed + 1) - Dice(.Speed + 1)
				NearestPoint(SelectedUnit, dst_x, dst_y, new_x, new_y)
			Else
				'移動可能範囲内で移動目的地に最も近い場所を検索
				NearestPoint(SelectedUnit, dst_x, dst_y, new_x0, new_y0)
				
				'移動先が危険地域かどうか判定する
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
					'移動先は危険地域。援護し合えるユニットと隣接するか、
					'有効な地形効果が得られる場所を探す。
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
								
								'地形による防御効果は空中にいる場合にのみ適用
								If .Area <> "空中" Then
									tmp = tmp + TerrainEffectForHit(tx, ty) + TerrainEffectForDamage(tx, ty)
									'水中用ユニットの場合は水中を優先
									If TerrainClass(tx, ty) = "水" Then
										If .IsTransAvailable("水") Then
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
					'移動先は危険地域ではない。
					'援護し合えるユニットがいれば隣接する。
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
				'移動できる場所がない……
				GoTo EndOfOperation
			End If
			
			'見つかった場所がいまいる場所でなければそこへ移動
			If .X <> new_x Or .Y <> new_y Then
				Select Case mmode
					Case "テレポート"
						If .IsMessageDefined("テレポート") Then
							OpenMessageForm()
							.PilotMessage("テレポート")
							CloseMessageForm()
						End If
						If .IsAnimationDefined("テレポート", .FeatureName("テレポート")) Then
							.PlayAnimation("テレポート", .FeatureName("テレポート"))
						ElseIf .IsSpecialEffectDefined("テレポート", .FeatureName("テレポート")) Then 
							.SpecialEffect("テレポート", .FeatureName("テレポート"))
						ElseIf BattleAnimation Then 
							ShowAnimation("テレポート発動 Whiz.wav " & .FeatureName0("テレポート"))
						Else
							PlayWave("Whiz.wav")
						End If
						.Move(new_x, new_y, True, False, True)
						SelectedUnitMoveCost = 1000
						RedrawScreen()
					Case "ジャンプ"
						If .IsMessageDefined("ジャンプ") Then
							OpenMessageForm()
							.PilotMessage("ジャンプ")
							CloseMessageForm()
						End If
						If .IsAnimationDefined("ジャンプ", .FeatureName("ジャンプ")) Then
							.PlayAnimation("ジャンプ", .FeatureName("ジャンプ"))
						ElseIf .IsSpecialEffectDefined("ジャンプ", .FeatureName("ジャンプ")) Then 
							.SpecialEffect("ジャンプ", .FeatureName("ジャンプ"))
						Else
							PlayWave("Swing.wav")
						End If
						.Move(new_x, new_y, True, False, True)
						SelectedUnitMoveCost = 1000
						RedrawScreen()
					Case Else
						'通常移動
						.Move(new_x, new_y)
						SelectedUnitMoveCost = TotalMoveCost(new_x, new_y)
				End Select
				moved = True
				
				'思考モードが「(X,Y)に移動」で目的地についた場合
				If LLength(.Mode) = 2 Then
					If .X = dst_x And .Y = dst_y Then
						.Mode = "待機"
					End If
				End If
			End If
			
			'ここでＥＮ切れ？
			If .EN = 0 Then
				If .MaxAction = 0 Then
					GoTo EndOfOperation
				End If
			End If
			
			'魅了されている場合
			If .IsConditionSatisfied("魅了") Then
				GoTo EndOfOperation
			End If
			
			'逃げている場合
			If .Mode = "逃亡" Then
				GoTo EndOfOperation
			End If
			
			'思考モードが特定のターゲットを狙うように設定されている場合
			If PList.IsDefined(.Mode) Then
				If PList.Item(.Mode).Unit_Renamed Is SelectedTarget Then
					If .IsEnemy(SelectedTarget) Then
						If moved Then
							w = SelectWeapon(SelectedUnit, SelectedTarget, "移動後")
						Else
							w = SelectWeapon(SelectedUnit, SelectedTarget, "移動可能")
						End If
						If w > 0 Then
							'移動の結果、ターゲットが射程内に入った
							GoTo AttackEnemy
						End If
					Else
						'護衛するユニットのもとを離れるべからず
						moved = True
					End If
				End If
			End If
			
			'特定の地点に移動中
			If LLength(.Mode) = 2 Then
				If 1 <= dst_x And dst_x <= MapWidth And 1 <= dst_y And dst_y <= MapHeight Then
					If Not MapDataForUnit(dst_x, dst_y) Is Nothing Then
						SelectedTarget = MapDataForUnit(dst_x, dst_y)
						If .IsEnemy(SelectedTarget) Then
							'移動先の場所にいる敵を優先して排除
							If moved Then
								w = SelectWeapon(SelectedUnit, SelectedTarget, "移動後")
							Else
								w = SelectWeapon(SelectedUnit, SelectedTarget, "移動可能")
							End If
							If w > 0 Then
								GoTo AttackEnemy
							End If
						End If
						'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						SelectedTarget = Nothing
					End If
				End If
			End If
			
			'改めて攻撃のシーケンスに移行
			If Not took_action Then
				GoTo TryMapAttack
			End If
		End With
		
EndOfOperation: 
		
		'行動終了
		
		ReDim SelectedPartners(0)
		
		If moved Then
			'持続期間が「移動」のスペシャルパワー効果を削除
			SelectedUnit.RemoveSpecialPowerInEffect("移動")
		End If
	End Sub
	
	'ハイパーモードが可能であればハイパーモード発動
	Private Sub TryHyperMode()
		Dim uname As String
		Dim u As Unit
		Dim fname, fdata As String
		Dim flevel As Double
		
		With SelectedUnit
			'ハイパーモードを持っている？
			If Not .IsFeatureAvailable("ハイパーモード") Then
				Exit Sub
			End If
			
			fname = .FeatureName("ハイパーモード")
			flevel = .FeatureLevel("ハイパーモード")
			fdata = .FeatureData("ハイパーモード")
			
			'発動条件を満たす？
			If .MainPilot.Morale < 100 + CShort(10# * flevel) And (InStr(fdata, "気力発動") > 0 Or .HP > .MaxHP \ 4) Then
				Exit Sub
			End If
			
			'ハイパーモードが禁止されている？
			If .IsConditionSatisfied("形態固定") Then
				Exit Sub
			End If
			If .IsConditionSatisfied("機体固定") Then
				Exit Sub
			End If
			
			'変身中・能力コピー中はハイパーモードを使用できない
			If .IsConditionSatisfied("ノーマルモード付加") Then
				Exit Sub
			End If
			
			'ハイパーモード先の形態を調べる
			uname = LIndex(fdata, 2)
			u = .OtherForm(uname)
			
			'ハイパーモード先の形態は使用可能？
			If u.IsConditionSatisfied("行動不能") Or Not u.IsAbleToEnter(.X, .Y) Then
				Exit Sub
			End If
			
			'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
			If u.IsFeatureAvailable("追加パイロット") Then
				If Not PList.IsDefined(u.FeatureData("追加パイロット")) Then
					PList.Add(u.FeatureData("追加パイロット"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'ハイパーモードメッセージ
			If .IsMessageDefined("ハイパーモード(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("ハイパーモード(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("ハイパーモード(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("ハイパーモード(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("ハイパーモード(" & fname & ")") Then 
				OpenMessageForm()
				.PilotMessage("ハイパーモード(" & fname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("ハイパーモード") Then 
				OpenMessageForm()
				.PilotMessage("ハイパーモード")
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
			ElseIf .IsSpecialEffectDefined("ハイパーモード") Then 
				.SpecialEffect("ハイパーモード")
			End If
			
			'ハイパーモード発動
			.Transform(uname)
		End With
		
		'ハイパーモードイベント
		With u.CurrentForm
			HandleEvent("ハイパーモード", .MainPilot.ID, .Name)
		End With
		
		'ハイパーモード＆ノーマルモードの自動発動
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
	End Sub
	
	'戦闘形態への変形が可能であれば変形する
	Public Function TryBattleTransform() As Boolean
		Dim uname As String
		Dim u As Unit
		Dim flag As Boolean
		Dim xx, yy As Short
		Dim i, j As Short
		
		With SelectedUnit
			'変形が可能？
			If Not .IsFeatureAvailable("変形") Or .IsConditionSatisfied("形態固定") Or .IsConditionSatisfied("機体固定") Then
				Exit Function
			End If
			
			'５マス以内に敵がいるかチェック
			If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
				'周りに敵はいない
				Exit Function
			End If
			
			'最も運動性が高い形態に変形
			u = SelectedUnit
			xx = .X
			yy = .Y
			For i = 2 To LLength(.FeatureData("変形"))
				uname = LIndex(.FeatureData("変形"), i)
				With .OtherForm(uname)
					'その形態に変形可能？
					If .IsConditionSatisfied("行動不能") Or Not .IsAbleToEnter(xx, yy) Then
						GoTo NextForm
					End If
					
					'通常形態は弱い形態であるという仮定に基づき、その形態が
					'ノーマルモードで指定されている場合ば無視する
					If uname = LIndex(.FeatureData("ノーマルモード"), 1) Then
						GoTo NextForm
					End If
					
					'海では水中もしくは空中適応を持つユニットを優先
					Select Case TerrainClass(xx, yy)
						Case "水", "深海"
							'水中適応を持つユニットを最優先
							If InStr(.Data.Transportation, "水") > 0 Then
								If InStr(u.Data.Transportation, "水") = 0 Then
									u = .OtherForm(uname)
									GoTo NextForm
								End If
							End If
							If InStr(u.Data.Transportation, "水") > 0 Then
								If InStr(.Data.Transportation, "水") = 0 Then
									GoTo NextForm
								End If
							End If
							
							'次点で空中適応ユニット
							If InStr(.Data.Transportation, "空") > 0 Then
								If InStr(u.Data.Transportation, "空") = 0 Then
									u = .OtherForm(uname)
									GoTo NextForm
								End If
							End If
							If InStr(u.Data.Transportation, "空") > 0 Then
								If InStr(.Data.Transportation, "空") = 0 Then
									GoTo NextForm
								End If
							End If
					End Select
					
					'運動性が高いものを優先
					If .Data.Mobility < u.Data.Mobility Then
						GoTo NextForm
					ElseIf .Data.Mobility = u.Data.Mobility Then 
						'運動性が同じなら攻撃力が高いものを優先
						If .Data.CountWeapon = 0 Then
							'この形態は武器を持っていない
							GoTo NextForm
						ElseIf u.Data.CountWeapon > 0 Then 
							If .Data.Weapon(.Data.CountWeapon).Power < u.Data.Weapon(u.Data.CountWeapon).Power Then
								GoTo NextForm
							ElseIf .Data.Weapon(.Data.CountWeapon).Power = u.Data.Weapon(u.Data.CountWeapon).Power Then 
								'攻撃力も同じなら装甲が高いものを優先
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
			
			'現在の形態が最も戦闘に適している？
			If u Is SelectedUnit Then
				Exit Function
			End If
			
			'形態uに変形決定
			uname = u.Name
			
			'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
			If u.IsFeatureAvailable("追加パイロット") Then
				If Not PList.IsDefined(u.FeatureData("追加パイロット")) Then
					If Not PDList.IsDefined(u.FeatureData("追加パイロット")) Then
						ErrorMessage(uname & "の追加パイロット「" & u.FeatureData("追加パイロット") & "」のデータが見つかりません")
						TerminateSRC()
					End If
					PList.Add(u.FeatureData("追加パイロット"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'変形メッセージ
			If .IsMessageDefined("変形(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("変形(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("変形(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("変形(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("変形(" & .FeatureName("変形") & ")") Then 
				OpenMessageForm()
				.PilotMessage("変形(" & .FeatureName("変形") & ")")
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
			
			'変形
			.Transform(uname)
		End With
		
		'変形イベント
		With u.CurrentForm
			HandleEvent("変形", .MainPilot.ID, .Name)
		End With
		
		'ハイパーモード＆ノーマルモードの自動発動
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
		
		TryBattleTransform = True
	End Function
	
	'移動形態への変形が可能であれば変形する
	Private Function TryMoveTransform() As Boolean
		Dim uname As String
		Dim u As Unit
		Dim xx, yy As Short
		Dim tx, ty As Short
		Dim speed1, speed2 As Short
		Dim i As Short
		
		With SelectedUnit
			'変形が可能？
			If Not .IsFeatureAvailable("変形") Or .IsConditionSatisfied("形態固定") Or .IsConditionSatisfied("機体固定") Then
				Exit Function
			End If
			
			xx = .X
			yy = .Y
			
			'地形に邪魔されて移動できなくならないか調べるため、目的地の方向にある
			'隣接するマスの座標を調べる
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
			
			'最も移動力が高い形態に変形
			u = SelectedUnit
			For i = 2 To LLength(.FeatureData("変形"))
				uname = LIndex(.FeatureData("変形"), i)
				With .OtherForm(uname)
					'その形態に変形可能？
					If .IsConditionSatisfied("行動不能") Or Not .IsAbleToEnter(xx, yy) Then
						GoTo NextForm
					End If
					
					'目的地方面に移動可能？
					If u.IsAbleToEnter(tx, ty) And Not .IsAbleToEnter(tx, ty) Then
						GoTo NextForm
					End If
					
					'移動力が高い方を優先
					speed1 = .Data.Speed
					If .Data.IsFeatureAvailable("テレポート") Then
						speed1 = speed1 + .Data.FeatureLevel("テレポート") + 1
					End If
					If .Data.IsFeatureAvailable("ジャンプ") Then
						speed1 = speed1 + .Data.FeatureLevel("ジャンプ") + 1
					End If
					'移動可能な地形タイプも考慮
					Select Case TerrainClass(xx, yy)
						Case "水", "深海"
							If InStr(.Data.Transportation, "水") > 0 Or InStr(.Data.Transportation, "空") > 0 Then
								speed1 = speed1 + 1
							End If
						Case "宇宙", "屋内"
							'宇宙や屋内では差が出ない
						Case Else
							If InStr(.Data.Transportation, "空") > 0 Then
								speed1 = speed1 + 1
							End If
					End Select
					
					speed2 = u.Data.Speed
					If u.Data.IsFeatureAvailable("テレポート") Then
						speed2 = speed2 + u.Data.FeatureLevel("テレポート") + 1
					End If
					If u.Data.IsFeatureAvailable("ジャンプ") Then
						speed2 = speed2 + u.Data.FeatureLevel("ジャンプ") + 1
					End If
					'移動可能な地形タイプも考慮
					Select Case TerrainClass(xx, yy)
						Case "水", "深海"
							If InStr(u.Data.Transportation, "水") > 0 Or InStr(u.Data.Transportation, "空") > 0 Then
								speed2 = speed2 + 1
							End If
						Case "宇宙", "屋内"
							'宇宙や屋内では差が出ない
						Case Else
							If InStr(u.Data.Transportation, "空") > 0 Then
								speed2 = speed2 + 1
							End If
					End Select
					
					If speed2 > speed1 Then
						GoTo NextForm
					ElseIf speed2 = speed1 Then 
						'移動力が同じなら装甲が高い方を優先
						If u.Data.Armor >= .Data.Armor Then
							GoTo NextForm
						End If
					End If
				End With
				u = .OtherForm(uname)
NextForm: 
			Next 
			
			'現在の形態が最も移動に適している？
			If SelectedUnit Is u Then
				Exit Function
			End If
			
			'形態uに変形決定
			uname = u.Name
			
			'ダイアログでメッセージを表示させるため追加パイロットをあらかじめ作成
			If u.IsFeatureAvailable("追加パイロット") Then
				If Not PList.IsDefined(u.FeatureData("追加パイロット")) Then
					If Not PDList.IsDefined(u.FeatureData("追加パイロット")) Then
						ErrorMessage(uname & "の追加パイロット「" & u.FeatureData("追加パイロット") & "」のデータが見つかりません")
						TerminateSRC()
					End If
					PList.Add(u.FeatureData("追加パイロット"), .MainPilot.Level, .Party0)
				End If
			End If
			
			'変形メッセージ
			If .IsMessageDefined("変形(" & .Name & "=>" & uname & ")") Then
				OpenMessageForm()
				.PilotMessage("変形(" & .Name & "=>" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("変形(" & uname & ")") Then 
				OpenMessageForm()
				.PilotMessage("変形(" & uname & ")")
				CloseMessageForm()
			ElseIf .IsMessageDefined("変形(" & .FeatureName("変形") & ")") Then 
				OpenMessageForm()
				.PilotMessage("変形(" & .FeatureName("変形") & ")")
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
			
			'変形
			.Transform(uname)
		End With
		
		'変形イベント
		With u.CurrentForm
			HandleEvent("変形", .MainPilot.ID, .Name)
		End With
		
		'ハイパーモード＆ノーマルモードの自動発動
		u.CurrentForm.CheckAutoHyperMode()
		u.CurrentForm.CheckAutoNormalMode()
		
		SelectedUnit = u.CurrentForm
		DisplayUnitStatus(SelectedUnit)
		
		TryMoveTransform = True
	End Function
	
	'実行時間を必要としないアビリティがあれば使っておく
	Public Sub TryInstantAbility()
		Dim i, j As Short
		Dim aname As String
		Dim partners() As Unit
		
		'５マス以内に敵がいるかチェック
		If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
			'周りに敵はいないのでアビリティは使わない
			Exit Sub
		End If
		
		With SelectedUnit
			'実行時間を必要としないアビリティを探す
			For i = 1 To .CountAbility
				'使用可能＆効果あり？
				If Not .IsAbilityUseful(i, "移動前") Then
					GoTo NextAbility
				End If
				
				'ＥＮ消費が多すぎない？
				If .AbilityENConsumption(i) > 0 Then
					If .AbilityENConsumption(i) >= .EN \ 2 Then
						GoTo NextAbility
					End If
				End If
				
				With .Ability(i)
					'自己強化のアビリティのみが対象
					If .MaxRange <> 0 Then
						GoTo NextAbility
					End If
					
					'実行時間を必要としない？
					For j = 1 To .CountEffect
						If .EffectType(j) = "再行動" Then
							Exit For
						End If
					Next 
					If j > .CountEffect Then
						GoTo NextAbility
					End If
					
					'強化用アビリティ？
					For j = 1 To .CountEffect
						If .EffectType(j) = "状態" Or .EffectType(j) = "付加" Or .EffectType(j) = "強化" Then
							'強化用アビリティが見つかった
							SelectedAbility = i
							GoTo UseInstantAbility
						End If
					Next 
				End With
NextAbility: 
			Next 
			
			'ここに来る時は使用できるアビリティがなかった場合
			Exit Sub
			
UseInstantAbility: 
			
			'合体技パートナーの設定
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				.CombinationPartner("アビリティ", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'アビリティの使用イベント
			HandleEvent("使用", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Sub
			End If
			
			'アビリティを使用
			OpenMessageForm(SelectedUnit)
			.ExecuteAbility(SelectedAbility, SelectedUnit)
			CloseMessageForm()
			SelectedUnit = .CurrentForm
		End With
		
		'アビリティの使用後イベント
		HandleEvent("使用後", SelectedUnit.MainPilot.ID, aname)
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Sub
		End If
		
		'自爆アビリティの破壊イベント
		If SelectedUnit.Status_Renamed = "破壊" Then
			HandleEvent("破壊", SelectedUnit.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Sub
			End If
		End If
		
		'行動数を消費しておく
		SelectedUnit.UseAction()
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Sub
	
	'召喚が可能であれば召喚する
	Public Function TrySummonning() As Boolean
		Dim i, j As Short
		Dim aname As String
		Dim partners() As Unit
		
		With SelectedUnit
			'召喚アビリティを検索
			For i = 1 To .CountAbility
				If .IsAbilityAvailable(i, "移動前") Then
					For j = 1 To .Ability(i).CountEffect
						If .Ability(i).EffectType(j) = "召喚" Then
							SelectedAbility = i
							GoTo UseSummonning
						End If
					Next 
				End If
			Next 
			
			'使用可能な召喚アビリティを持っていなかった
			Exit Function
			
UseSummonning: 
			
			TrySummonning = True
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'召喚アビリティの使用イベント
			HandleEvent("使用", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				Exit Function
			End If
			
			'合体技パートナーの設定
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				.CombinationPartner("アビリティ", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			'召喚アビリティを使用
			OpenMessageForm(SelectedUnit)
			.ExecuteAbility(SelectedAbility, SelectedUnit)
			CloseMessageForm()
			SelectedUnit = .CurrentForm
		End With
		
		'召喚アビリティの使用後イベント
		HandleEvent("使用後", SelectedUnit.MainPilot.ID, aname)
		If IsScenarioFinished Or IsCanceled Then
			ReDim SelectedPartners(0)
			Exit Function
		End If
		
		'自爆アビリティの破壊イベント
		If SelectedUnit.Status_Renamed = "破壊" Then
			HandleEvent("破壊", SelectedUnit.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Function
	
	'マップ型回復アビリティ使用に関する処理
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
			
			'狂戦士状態の際は回復アビリティを使わない
			If .IsConditionSatisfied("狂戦士") Then
				Exit Function
			End If
			
			p = .MainPilot
			
			a = .CountAbility()
			Do While a > 0
				'マップアビリティかどうか
				If Not .IsAbilityClassifiedAs(a, "Ｍ") Then
					GoTo NextAbility
				End If
				
				'アビリティの使用可否を判定
				If moved Then
					If Not .IsAbilityAvailable(a, "移動後") Then
						GoTo NextAbility
					End If
				Else
					If Not .IsAbilityAvailable(a, "移動前") Then
						GoTo NextAbility
					End If
				End If
				
				'回復アビリティかどうか
				For i = 1 To .Ability(a).CountEffect
					If .Ability(a).EffectType(i) = "回復" Then
						'回復量を算出しておく
						If .IsSpellAbility(a) Then
							apower = 5 * .Ability(a).EffectLevel(i) * p.Shooting
						Else
							apower = 500 * .Ability(a).EffectLevel(i)
						End If
						Exit For
					End If
				Next 
				If i > .Ability(a).CountEffect Then
					'回復アビリティではなかった
					GoTo NextAbility
				End If
				
				max_range = .AbilityMaxRange(a)
				min_range = .AbilityMinRange(a)
				
				x1 = MaxLng(.X - max_range, 1)
				x2 = MinLng(.X + max_range, MapWidth)
				y1 = MaxLng(.Y - max_range, 1)
				y2 = MinLng(.Y + max_range, MapHeight)
				
				'アビリティの効果範囲に応じてアビリティが有効かどうか判断する
				num = 0
				score = 0
				If .IsAbilityClassifiedAs(a, "Ｍ全") Then
					' MOD START マージ
					'                AreaInRange .X, .Y, min_range, max_range, .Party
					AreaInRange(.X, .Y, max_range, min_range, .Party)
					' MOD END マージ
					
					'支援専用アビリティの場合は自分には効果がない
					If .IsAbilityClassifiedAs(a, "援") Then
						MaskData(.X, .Y) = True
					End If
					
					'効果範囲内にいるターゲットをカウント
					For i = x1 To x2
						For j = y1 To y2
							If MaskData(i, j) Then
								GoTo NextUnit1
							End If
							
							t = MapDataForUnit(i, j)
							If t Is Nothing Then
								GoTo NextUnit1
							End If
							
							'アビリティが適用可能？
							If Not .IsAbilityApplicable(a, t) Then
								GoTo NextUnit1
							End If
							
							With t
								'ゾンビ？
								If .IsConditionSatisfied("ゾンビ") Then
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
					
					'不要？
					tx = .X
					ty = .Y
				ElseIf .IsAbilityClassifiedAs(a, "Ｍ投") Then 
					
					mlv = .AbilityLevel(a, "Ｍ投")
					
					'投下位置を変えながら試してみる
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) > max_range Or System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) < min_range Then
								GoTo NextPoint
							End If
							
							' MOD START マージ
							AreaInRange(xx, yy, 1, mlv, .Party)
							AreaInRange(xx, yy, mlv, 1, .Party)
							' MOD END マージ
							
							'支援専用アビリティの場合は自分には効果がない
							If .IsAbilityClassifiedAs(a, "援") Then
								MaskData(.X, .Y) = True
							End If
							
							'効果範囲内にいるターゲットをカウント
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
									
									'アビリティが適用可能？
									If Not .IsAbilityApplicable(a, t) Then
										GoTo NextUnit2
									End If
									
									With t
										'ゾンビ？
										If .IsConditionSatisfied("ゾンビ") Then
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
				'有効なマップアビリティがなかった
				Exit Function
			End If
			
			'合体技パートナーの設定
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				.CombinationPartner("アビリティ", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			SelectedAbilityName = .Ability(SelectedAbility).Name
			
			'アビリティを使用
			.ExecuteMapAbility(SelectedAbility, tx, ty)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
			
			'合体技のパートナーの行動数を減らす
			If Not IsOptionDefined("合体技パートナー行動数無消費") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.UseAction()
				Next 
			End If
			ReDim SelectedPartners(0)
		End With
		
		TryMapHealing = True
	End Function
	
	'可能であれば回復アビリティを使う
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
			'狂戦士状態の際は回復アビリティを使わない
			If .IsConditionSatisfied("狂戦士") Then
				Exit Function
			End If
			
			'初期化
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			SelectedTarget = Nothing
			max_dmg = 80
			SelectedAbility = 0
			max_power = 0
			
			'移動可能？
			dont_move = moved Or .Mode = "固定"
			
			'移動可能である場合は移動範囲を設定しておく
			If Not dont_move Then
				AreaInSpeed(SelectedUnit)
			End If
			
			For a = 1 To .CountAbility
				'アビリティが使用可能？
				If moved Then
					If Not .IsAbilityAvailable(a, "移動後") Then
						GoTo NextHealingSkill
					End If
				Else
					If Not .IsAbilityAvailable(a, "移動前") Then
						GoTo NextHealingSkill
					End If
				End If
				
				'マップアビリティは別関数で調べる
				If .IsAbilityClassifiedAs(a, "Ｍ") Then
					GoTo NextHealingSkill
				End If
				
				'これは回復アビリティ？
				For i = 1 To .Ability(a).CountEffect
					If .Ability(a).EffectType(i) = "回復" Then
						Exit For
					End If
				Next 
				If i > .Ability(a).CountEffect Then
					GoTo NextHealingSkill
				End If
				
				'回復量を算出
				If .IsSpellAbility(a) Then
					apower = CInt(5 * .Ability(a).EffectLevel(i) * .MainPilot.Shooting)
				Else
					apower = 500 * .Ability(a).EffectLevel(i)
				End If
				
				'役立たず？
				If apower <= 0 Then
					GoTo NextHealingSkill
				End If
				
				'現在の回復アビリティを使って回復させられるターゲットがいるか検索
				For	Each u In UList
					If u.Status_Renamed <> "出撃" Then
						GoTo NextHealingTarget
					End If
					
					'味方かどうかを判定
					If Not .IsAlly(u) Then
						GoTo NextHealingTarget
					End If
					
					'デフォルトのターゲットが指定されている場合はそのユニット以外を
					'ターゲットにはしない
					If Not t Is Nothing Then
						If Not u Is t Then
							GoTo NextHealingTarget
						End If
					End If
					
					'損傷度は？
					dmg = 100 * u.HP \ u.MaxHP
					
					'重要なユニットを優先
					If Not u Is SelectedUnit Then
						If u.BossRank >= 0 Then
							dmg = 100 - 2 * (100 - dmg)
						End If
					End If
					
					'現在のターゲットより損傷度がひどくないなら無視
					If dmg > max_dmg Then
						GoTo NextHealingTarget
					End If
					
					'移動可能か？
					If .AbilityMaxRange(a) = 1 Or .IsAbilityClassifiedAs(a, "Ｐ") Then
						is_able_to_move = True
					Else
						is_able_to_move = False
					End If
					If .IsAbilityClassifiedAs(a, "Ｑ") Then
						is_able_to_move = False
					End If
					If dont_move Then
						is_able_to_move = False
					End If
					Select Case .Area
						Case "空中", "宇宙"
							If .EN - .AbilityENConsumption(a) < 5 Then
								is_able_to_move = False
							End If
					End Select
					
					'射程内にいるか？
					If is_able_to_move Then
						If Not .IsTargetReachableForAbility(a, u) Then
							GoTo NextHealingTarget
						End If
					Else
						If Not .IsTargetWithinAbilityRange(a, u) Then
							GoTo NextHealingTarget
						End If
					End If
					
					'アビリティが適用可能？
					If Not .IsAbilityApplicable(a, u) Then
						GoTo NextHealingTarget
					End If
					
					'ゾンビ？
					If u.IsConditionSatisfied("ゾンビ") Then
						GoTo NextHealingTarget
					End If
					
					'新規ターゲット？
					If Not u Is SelectedTarget Then
						'ターゲット設定
						SelectedTarget = u
						max_dmg = dmg
						
						'新規ターゲットを優先するため、現在選択されているアビリティは破棄
						SelectedAbility = 0
						max_power = 0
					End If
					
					'現在選択されている回復アビリティとチェック中のアビリティのどちらが
					'優れているかを判定
					If max_power < u.MaxHP - u.HP Then
						'現在選択している回復アビリティでは全ダメージを回復しきれない場合
						If apower < max_power Then
							'回復量が多いほうを優先
							GoTo NextHealingTarget
						ElseIf apower = max_power Then 
							'回復量が同じならコストが低い方を優先
							If .Ability(a).ENConsumption > .Ability(SelectedAbility).ENConsumption Then
								GoTo NextHealingTarget
							End If
							If .Ability(a).Stock < .Ability(SelectedAbility).Stock Then
								GoTo NextHealingTarget
							End If
						End If
					ElseIf SelectedAbility > 0 Then 
						'現在選択している回復アビリティで全快する場合
						'全快することが必要条件
						If apower >= u.MaxHP - u.HP Then
							GoTo NextHealingTarget
						End If
						'コストが低い方を優先
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
			
			'有用なアビリティ＆ターゲットが見つかった？
			If SelectedAbility = 0 Then
				Exit Function
			End If
			If SelectedTarget Is Nothing Then
				Exit Function
			End If
			
			'回復アビリティを使用することが確定
			TryHealing = True
			
			'適切な位置に移動
			If Not SelectedTarget Is SelectedUnit And sa_is_able_to_move Then
				new_x = .X
				new_y = .Y
				max_range = .AbilityMaxRange(SelectedAbility)
				With SelectedTarget
					'現在位置から回復が可能であれば現在位置を優先
					If System.Math.Abs(.X - new_x) + System.Math.Abs(.Y - new_y) <= max_range Then
						distance = System.Math.Abs(.X - new_x) ^ 2 + System.Math.Abs(.Y - new_y) ^ 2
					Else
						distance = 10000
					End If
					
					'適切な位置を探す
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
					'適切な場所が見つかったので移動
					.Move(new_x, new_y)
					moved = True
				End If
			End If
			
			aname = .Ability(SelectedAbility).Name
			SelectedAbilityName = aname
			
			'合体技パートナーの設定
			If .IsAbilityClassifiedAs(SelectedAbility, "合") Then
				.CombinationPartner("アビリティ", SelectedAbility, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			'使用イベント
			HandleEvent("使用", .MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
			
			If SelectedTarget Is SelectedUnit Then
				OpenMessageForm(SelectedUnit)
			Else
				OpenMessageForm(SelectedTarget, SelectedUnit)
			End If
			
			'回復アビリティを実行
			.ExecuteAbility(SelectedAbility, SelectedTarget)
			SelectedUnit = .CurrentForm
		End With
		
		CloseMessageForm()
		
		'自爆した場合の破壊イベント
		If SelectedUnit.Status_Renamed = "破壊" Then
			If SelectedUnit.CountPilot > 0 Then
				HandleEvent("破壊", SelectedUnit.MainPilot.ID)
			End If
			ReDim SelectedPartners(0)
			Exit Function
		End If
		
		'使用後イベント
		If SelectedUnit.CountPilot > 0 Then
			HandleEvent("使用後", SelectedUnit.MainPilot.ID, aname)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'自爆アビリティの破壊イベント
		If SelectedUnit.Status_Renamed = "破壊" Then
			HandleEvent("破壊", SelectedUnit.MainPilot.ID)
			If IsScenarioFinished Or IsCanceled Then
				ReDim SelectedPartners(0)
				Exit Function
			End If
		End If
		
		'合体技のパートナーの行動数を減らす
		If Not IsOptionDefined("合体技パートナー行動数無消費") Then
			For i = 1 To UBound(partners)
				partners(i).CurrentForm.UseAction()
			Next 
		End If
		ReDim SelectedPartners(0)
	End Function
	
	'修理が可能であれば修理装置を使う
	Public Function TryFix(ByRef moved As Boolean, Optional ByRef t As Unit = Nothing) As Boolean
		Dim TmpMaskData() As Boolean
		Dim j, i, k As Short
		Dim new_x, new_y As Short
		Dim max_dmg As Integer
		Dim tmp As Integer
		Dim u As Unit
		Dim fname As String
		
		With SelectedUnit
			'修理装置を使用可能？
			If Not .IsFeatureAvailable("修理装置") Or .Area = "地中" Then
				Exit Function
			End If
			
			'狂戦士状態の際は修理装置を使わない
			If .IsConditionSatisfied("狂戦士") Then
				Exit Function
			End If
			
			'修理装置を使用可能な領域を設定
			If moved Or .Mode = "固定" Then
				'移動でない場合
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
				'移動可能な場合
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
			
			'ターゲットを探す
			'UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
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
					
					'デフォルトのターゲットが指定されている場合はそのユニット以外を
					'ターゲットにはしない
					If Not t Is Nothing Then
						If Not u Is t Then
							GoTo NextFixTarget
						End If
					End If
					
					'現在の選択しているターゲットよりダメージが少なければ選択しない
					If 100 * u.HP \ u.MaxHP > max_dmg Then
						GoTo NextFixTarget
					End If
					
					'味方かどうか判定
					If Not .IsAlly(u) Then
						GoTo NextFixTarget
					End If
					
					'ゾンビ？
					If u.IsConditionSatisfied("ゾンビ") Then
						GoTo NextFixTarget
					End If
					
					'修理不可？
					If u.IsFeatureAvailable("修理不可") Then
						For k = 2 To CInt(u.FeatureData("修理不可"))
							fname = LIndex(u.FeatureData("修理不可"), k)
							If Left(fname, 1) = "!" Then
								fname = Mid(fname, 2)
								If fname <> .FeatureName0("修理装置") Then
									GoTo NextFixTarget
								End If
							Else
								If fname = .FeatureName0("修理装置") Then
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
			
			'ターゲットが見つからない
			If SelectedTarget Is Nothing Then
				Exit Function
			End If
			
			'ターゲットに隣接するように移動
			If Not moved And .Mode <> "固定" Then
				new_x = .X
				new_y = .Y
				With SelectedTarget
					'現在位置から修理が可能であれば現在位置を優先
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
					
					'適切な場所を探す
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
					'適切な場所が見つかったので移動
					.Move(new_x, new_y)
					moved = True
				End If
			End If
			
			'選択内容を変更
			SelectedUnitForEvent = SelectedUnit
			SelectedTargetForEvent = SelectedTarget
			
			'メッセージ表示
			OpenMessageForm(SelectedTarget, SelectedUnit)
			If .IsMessageDefined("修理") Then
				.PilotMessage("修理")
			End If
			
			'アニメ表示
			If .IsAnimationDefined("修理", .FeatureName("修理")) Then
				.PlayAnimation("修理", .FeatureName("修理"))
			Else
				.SpecialEffect("修理", .FeatureName("修理"))
			End If
			
			DisplaySysMessage(.Nickname & "は[" & SelectedTarget.Nickname & "]に[" & .FeatureName0("修理装置") & "]を使った。")
			
			'修理実行
			tmp = SelectedTarget.HP
			Select Case .FeatureLevel("修理装置")
				Case 1, -1
					SelectedTarget.RecoverHP(30 + 3 * .MainPilot.SkillLevel("修理技能"))
				Case 2
					SelectedTarget.RecoverHP(50 + 5 * .MainPilot.SkillLevel("修理技能"))
				Case 3
					SelectedTarget.RecoverHP(100)
			End Select
			DrawSysString(SelectedTarget.X, SelectedTarget.Y, "+" & VB6.Format(SelectedTarget.HP - tmp))
			UpdateMessageForm(SelectedTarget, SelectedUnit)
			DisplaySysMessage(SelectedTarget.Nickname & "のＨＰが[" & VB6.Format(SelectedTarget.HP - tmp) & "]回復した。")
		End With
		
		'経験値獲得
		SelectedUnit.GetExp(SelectedTarget, "修理")
		
		If MessageWait < 10000 Then
			Sleep(MessageWait)
		End If
		
		CloseMessageForm()
		
		'形態変化のチェック
		SelectedTarget.Update()
		SelectedTarget.CurrentForm.CheckAutoHyperMode()
		SelectedTarget.CurrentForm.CheckAutoNormalMode()
		
		TryFix = True
	End Function
	
	'マップ攻撃使用に関する処理
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
			
			'マップ攻撃を使用するターゲット数の下限を設定する
			score_limit = 1
			For i = 1 To .CountWeapon
				'通常攻撃を持っている場合は単独の敵への攻撃の際に通常攻撃を優先する
				If Not .IsWeaponClassifiedAs(i, "Ｍ") Then
					' MOD START マージ
					'                score_limit = 2
					'                Exit For
					If .IsWeaponAvailable(i, "移動前") Then
						score_limit = 2
						Exit For
					End If
					' MOD END マージ
				End If
			Next 
			
			'威力の高い武器を優先して選択
			w = .CountWeapon
			Do While w > 0
				SelectedWeapon = w
				SelectedTWeapon = 0
				
				'マップ攻撃かどうか
				If Not .IsWeaponClassifiedAs(w, "Ｍ") Then
					GoTo NextWeapon
				End If
				
				'武器の使用可否を判定
				If moved Then
					If Not .IsWeaponAvailable(w, "移動後") Then
						GoTo NextWeapon
					End If
				Else
					If Not .IsWeaponAvailable(w, "移動前") Then
						GoTo NextWeapon
					End If
				End If
				
				'ボスユニットが自爆＆全ＥＮ消費攻撃等を使うのは非常時のみ
				If .BossRank >= 0 Then
					If .IsWeaponClassifiedAs(w, "自") Or .IsWeaponClassifiedAs(w, "尽") Or .IsWeaponClassifiedAs(w, "消") Then
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
				
				'マップ攻撃の種類にしたがって効果範囲内にいる敵をカウント
				If .IsWeaponClassifiedAs(w, "Ｍ直") Then
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
						
						'効果範囲を設定
						AreaInLine(.X, .Y, min_range, max_range, direction)
						MaskData(.X, .Y) = True
						
						'効果範囲内にいるユニットをカウント
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'マップ攻撃が最強武器であればターゲットが１体であっても使用
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
					
				ElseIf .IsWeaponClassifiedAs(w, "Ｍ拡") Then 
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
						
						'効果範囲を設定
						AreaInCone(.X, .Y, min_range, max_range, direction)
						MaskData(.X, .Y) = True
						
						'効果範囲内にいるユニットをカウント
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'マップ攻撃が最強武器であればターゲットが１体であっても使用
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
					
				ElseIf .IsWeaponClassifiedAs(w, "Ｍ扇") Then 
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
						
						'効果範囲を設定
						AreaInSector(.X, .Y, min_range, max_range, direction, .WeaponLevel(w, "Ｍ扇"))
						MaskData(.X, .Y) = True
						
						'効果範囲内にいるユニットをカウント
						enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
						
						'マップ攻撃が最強武器であればターゲットが１体であっても使用
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
					
				ElseIf .IsWeaponClassifiedAs(w, "Ｍ全") Then 
					'効果範囲を設定
					' MOD START マージ
					'                AreaInRange .X, .Y, min_range, max_range, "すべて"
					AreaInRange(.X, .Y, max_range, min_range, "すべて")
					' MOD END マージ
					MaskData(.X, .Y) = True
					
					'効果範囲内にいるユニットをカウント
					enemy_num = CountTargetInRange(w, x1, y1, x2, y2)
					
					'マップ攻撃が最強武器であればターゲットが１体であっても使用
					If enemy_num >= score_limit Or (enemy_num = 1 And w = .CountWeapon) Then
						tx = .X
						ty = .Y
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "Ｍ投") Then 
					lv = .WeaponLevel(w, "Ｍ投")
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range Then
								'効果範囲を設定
								If lv > 0 Then
									' MOD START マージ
									'                                AreaInRange xx, yy, 1, lv, "すべて"
									AreaInRange(xx, yy, lv, 1, "すべて")
									' MOD END マージ
								Else
									For i = 1 To MapWidth
										For j = 1 To MapHeight
											MaskData(i, j) = True
										Next 
									Next 
									MaskData(xx, yy) = False
								End If
								MaskData(.X, .Y) = True
								
								'効果範囲内にいるユニットをカウント
								enemy_num = CountTargetInRange(w, xx - lv, yy - lv, xx + lv, yy + lv)
								
								If enemy_num > score Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						Next 
					Next 
					
					'マップ攻撃が最強武器であればターゲットが１体であっても使用
					'また、Ｍ投L0の場合は最大でも１体の敵しか狙えない
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Or (score = 1 And lv = 0) Then
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "Ｍ線") Then 
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range Then
								'効果範囲を設定
								AreaInPointToPoint(.X, .Y, xx, yy)
								MaskData(.X, .Y) = True
								
								'効果範囲内にいるユニットをカウント
								enemy_num = CountTargetInRange(w, MinLng(.X, xx), MinLng(.Y, yy), MaxLng(.X, xx), MaxLng(.Y, yy))
								
								If enemy_num > score Then
									score = enemy_num
									tx = xx
									ty = yy
								End If
							End If
						Next 
					Next 
					
					'マップ攻撃が最強武器であればターゲットが１体であっても使用
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Then
						GoTo FoundWeapon
					End If
					
				ElseIf .IsWeaponClassifiedAs(w, "Ｍ移") Then 
					'その場を動かない場合は移動型マップ攻撃は選考外
					If .Mode = "固定" Then
						GoTo NextWeapon
					End If
					
					score = 0
					For xx = x1 To x2
						For yy = y1 To y2
							If System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) <= max_range And System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy) >= min_range And MapDataForUnit(xx, yy) Is Nothing And .IsAbleToEnter(xx, yy) Then
								'効果範囲を設定
								AreaInPointToPoint(.X, .Y, xx, yy)
								MaskData(.X, .Y) = True
								
								'効果範囲内にいるユニットをカウント
								enemy_num = CountTargetInRange(w, MinLng(.X, xx), MinLng(.Y, yy), MaxLng(.X, xx), MaxLng(.Y, yy))
								
								If enemy_num > score Then
									'最終チェック 目標地点にたどり着けるか？
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
					
					'マップ攻撃が最強武器であればターゲットが１体であっても使用
					'また、射程が２の場合は最大でも１体の敵しか狙えない
					If score >= score_limit Or (score = 1 And w = .CountWeapon) Or (score = 1 And max_range = 2) Then
						GoTo FoundWeapon
					End If
				End If
NextWeapon: 
				w = w - 1
			Loop 
			
			'有効なマップ攻撃が見つからなかった
			
			RestoreSelections()
			TryMapAttack = False
			
			Exit Function
			
FoundWeapon: 
			
			'有効なマップ攻撃が見つかった場合
			
			'合体技パートナーの設定
			If .IsWeaponClassifiedAs(w, "合") Then
				.CombinationPartner("武装", w, partners)
			Else
				ReDim SelectedPartners(0)
				ReDim partners(0)
			End If
			
			'マップ攻撃による攻撃を実行
			.MapAttack(w, tx, ty)
			
			'合体技のパートナーの行動数を減らす
			If Not IsOptionDefined("合体技パートナー行動数無消費") Then
				For i = 1 To UBound(partners)
					partners(i).CurrentForm.UseAction()
				Next 
			End If
			ReDim SelectedPartners(0)
			
			RestoreSelections()
			TryMapAttack = True
		End With
	End Function
	
	'効果範囲内にいるターゲットをカウント
	Private Function CountTargetInRange(ByVal w As Short, ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short) As Short
		Dim i, j As Short
		Dim t As Unit
		Dim is_ally_involved As Boolean
		
		With SelectedUnit
			'効果範囲内のターゲットを検索
			For i = MaxLng(x1, 1) To MinLng(x2, MapWidth)
				For j = MaxLng(y1, 1) To MinLng(y2, MapHeight)
					'効果範囲内？
					If MaskData(i, j) Then
						GoTo NextPoint
					End If
					
					t = MapDataForUnit(i, j)
					
					'ユニットが存在する？
					If t Is Nothing Then
						GoTo NextPoint
					End If
					
					'ダメージを与えられる？
					If .HitProbability(w, t, False) = 0 Then
						GoTo NextPoint
					ElseIf .ExpDamage(w, t, False) <= 10 Then 
						If .IsNormalWeapon(w) Then
							GoTo NextPoint
						ElseIf .CriticalProbability(w, t) <= 1 And .WeaponLevel(w, "Ｋ") = 0 And .WeaponLevel(w, "吹") = 0 Then 
							GoTo NextPoint
						End If
					End If
					
					'ターゲットは敵？
					If .IsAlly(t) Then
						'味方の場合は同士討ちの可能性があるのでチェックしておく
						is_ally_involved = True
						GoTo NextPoint
					End If
					
					'特定の陣営のみを攻撃する場合
					Select Case .Mode
						Case "味方", "ＮＰＣ"
							If t.Party <> "味方" And t.Party <> "ＮＰＣ" Then
								GoTo NextPoint
							End If
						Case "敵"
							If t.Party <> "敵" Then
								GoTo NextPoint
							End If
						Case "中立"
							If t.Party <> "中立" Then
								GoTo NextPoint
							End If
					End Select
					
					'ターゲットが見える？
					If t.IsUnderSpecialPowerEffect("隠れ身") Then
						GoTo NextPoint
					End If
					If t.IsFeatureAvailable("ステルス") Then
						If Not t.IsConditionSatisfied("ステルス無効") And Not .IsFeatureAvailable("ステルス無効化") Then
							If t.IsFeatureLevelSpecified("ステルス") Then
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > t.FeatureLevel("ステルス") Then
									GoTo NextPoint
								End If
							Else
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > 3 Then
									GoTo NextPoint
								End If
							End If
						End If
					End If
					
					'ターゲットに含める
					CountTargetInRange = CountTargetInRange + 1
NextPoint: 
				Next 
			Next 
			
			'味方を巻き込んでしまう場合は攻撃を止める
			If is_ally_involved And Not .IsWeaponClassifiedAs(w, "識") And Not .IsUnderSpecialPowerEffect("識別攻撃") Then
				CountTargetInRange = 0
			End If
		End With
	End Function
	
	'スペシャルパワーを使用する
	Public Sub TrySpecialPower(ByRef p As Pilot)
		Dim slist As String
		Dim sd As SpecialPowerData
		Dim i, tnum As Short
		
		SelectedPilot = p
		
		'ザコパイロットはスペシャルパワーを使わない
		If InStr(p.Name, "(ザコ)") > 0 Then
			Exit Sub
		End If
		
		'技量が高いほどスペシャルパワーの発動確率が高い
		If Dice(100) > p.TacticalTechnique0 - 100 Then
			Exit Sub
		End If
		
		With SelectedUnit
			'正常な判断力がある？
			If .IsConditionSatisfied("混乱") Or .IsConditionSatisfied("魅了") Or .IsConditionSatisfied("憑依") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("狂戦士") Then
				Exit Sub
			End If
			
			'スペシャルパワー使用不能
			If .IsConditionSatisfied("スペシャルパワー使用不能") Then
				Exit Sub
			End If
		End With
		
		'使用する可能性のあるスペシャルパワーの一覧を作成
		slist = ""
		For i = 1 To p.CountSpecialPower
			SelectedSpecialPower = p.SpecialPower(i)
			sd = SPDList.Item(SelectedSpecialPower)
			
			'ＳＰが足りている？
			If p.SP < p.SpecialPowerCost(SelectedSpecialPower) Then
				GoTo NextSpecialPower
			End If
			
			'既に実行済み？
			If SelectedUnit.IsSpecialPowerInEffect(SelectedSpecialPower) Then
				GoTo NextSpecialPower
			End If
			
			sd = SPDList.Item(SelectedSpecialPower)
			
			With sd
				'ターゲットを選択する必要のあるスペシャルパワーは判断が難しいので
				'使用しない
				Select Case .TargetType
					Case "味方", "敵", "任意"
						GoTo NextSpecialPower
				End Select
				
				'ターゲットがいなければ使用しない
				tnum = .CountTarget(p)
				If tnum = 0 Then
					GoTo NextSpecialPower
				End If
				
				'複数のユニットをターゲットにするスペシャルパワーはターゲットが
				'少ない場合は使用しない
				Select Case .TargetType
					Case "全味方", "全敵"
						If tnum < 3 Then
							GoTo NextSpecialPower
						End If
				End Select
				
				'使用に適した状況下にある？
				
				'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ＨＰ回復) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If .IsEffectAvailable("ＨＰ回復") Then
					If .TargetType = "自分" Then
						If SelectedUnit.HP < 0.7 * SelectedUnit.MaxHP Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "全味方" Then 
						If Turn >= 3 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
				
				'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ＥＮ回復) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If .IsEffectAvailable("ＥＮ回復") Then
					If .TargetType = "自分" Then
						If SelectedUnit.EN < 0.3 * SelectedUnit.MaxEN Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "全味方" Then 
						If Turn >= 4 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
				
				'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(気力増加) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If .IsEffectAvailable("気力増加") Then
					If .TargetType = "自分" Then
						If p.Morale < p.MaxMorale Then
							If p.CountSpecialPower = 1 Or p.SP > p.MaxSP \ 2 Then
								GoTo AddSpecialPower
							End If
						End If
					ElseIf .TargetType = "全味方" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(行動数増加) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If .IsEffectAvailable("行動数増加") Then
					If .TargetType = "自分" Then
						If DistanceFromNearestEnemy(SelectedUnit) <= 5 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
				
				'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(復活) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				If .IsEffectAvailable("復活") Then
					If .TargetType = "自分" Then
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "絶対命中") Or IsSPEffectUseful(sd, "ダメージ増加") Or IsSPEffectUseful(sd, "クリティカル率増加") Or IsSPEffectUseful(sd, "命中強化") Or IsSPEffectUseful(sd, "貫通攻撃") Or IsSPEffectUseful(sd, "再攻撃") Or IsSPEffectUseful(sd, "隠れ身") Then
					If .TargetType = "自分" Then
						If DistanceFromNearestEnemy(SelectedUnit) <= 5 Or .Duration = "攻撃" Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "全味方" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "絶対回避") Or IsSPEffectUseful(sd, "被ダメージ低下") Or IsSPEffectUseful(sd, "装甲強化") Or IsSPEffectUseful(sd, "回避強化") Then
					If .TargetType = "自分" Then
						If DistanceFromNearestEnemy(SelectedUnit) <= 5 Or .Duration = "防御" Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "全味方" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "移動力強化") Then
					If .TargetType = "自分" Then
						If DistanceFromNearestEnemy(SelectedUnit) > 5 Then
							GoTo AddSpecialPower
						End If
					ElseIf .TargetType = "全味方" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If IsSPEffectUseful(sd, "射程延長") Then
					If .TargetType = "自分" Then
						Select Case DistanceFromNearestEnemy(SelectedUnit)
							Case 5, 6
								GoTo AddSpecialPower
						End Select
					ElseIf .TargetType = "全味方" Then 
						GoTo AddSpecialPower
					End If
				End If
				
				If .IsEffectAvailable("気力低下") Or .IsEffectAvailable("ランダムダメージ") Or .IsEffectAvailable("ＨＰ減少") Or .IsEffectAvailable("ＥＮ減少") Or .IsEffectAvailable("挑発") Then
					If .TargetType = "全敵" Then
						GoTo AddSpecialPower
					End If
				End If
				
				If .IsEffectAvailable("ダメージ低下") Or .IsEffectAvailable("被ダメージ増加") Or .IsEffectAvailable("命中低下") Or .IsEffectAvailable("回避低下") Or .IsEffectAvailable("命中率低下") Or .IsEffectAvailable("移動力低下") Or .IsEffectAvailable("サポートガード不能") Then
					If .TargetType = "全敵" Then
						If Turn >= 3 Then
							GoTo AddSpecialPower
						End If
					End If
				End If
			End With
			
			'有用な効果が見つからなかった
			GoTo NextSpecialPower
			
AddSpecialPower: 
			
			'スペシャルパワーを候補リストに追加
			slist = slist & " " & SelectedSpecialPower
			
NextSpecialPower: 
		Next 
		
		'使用可能なスペシャルパワーを所有していない
		If slist = "" Then
			SelectedSpecialPower = ""
			Exit Sub
		End If
		
		'使用するスペシャルパワーをランダムに選択
		SelectedSpecialPower = LIndex(slist, Dice(LLength(slist)))
		
		'使用イベント
		HandleEvent("使用", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		If IsScenarioFinished Or IsCanceled Then
			Exit Sub
		End If
		
		'選択したスペシャルパワーを実行する
		p.UseSpecialPower(SelectedSpecialPower)
		SelectedUnit = SelectedUnit.CurrentForm
		
		'ステータスウィンドウ更新
		If Not IsRButtonPressed Then
			DisplayUnitStatus(SelectedUnit)
		End If
		
		'使用後イベント
		HandleEvent("使用後", SelectedUnit.MainPilot.ID, SelectedSpecialPower)
		
		SelectedSpecialPower = ""
	End Sub
	
	Private Function IsSPEffectUseful(ByRef sd As SpecialPowerData, ByRef ename As String) As Boolean
		With sd
			'UPGRADE_WARNING: オブジェクト sd.IsEffectAvailable(ename) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If .IsEffectAvailable(ename) Then
				If .TargetType = "自分" Then
					'自分自身がターゲットである場合、既に同じ効果を持つスペシャル
					'パワーを使用している場合は使用しない。
					If Not SelectedUnit.IsSpecialPowerInEffect(ename) Then
						IsSPEffectUseful = True
					End If
				Else
					IsSPEffectUseful = True
				End If
			End If
		End With
	End Function
	
	'ユニット u がターゲット t を攻撃するための武器を選択
	'amode:攻撃の種類
	'max_prob:敵を破壊できる確率
	'max_dmg:ダメージ期待値
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
			'御主人さまにはさからえません
			If .IsConditionSatisfied("魅了") Then
				If .Master Is t Then
					SelectWeapon = -1
					Exit Function
				End If
			End If
			
			'踊りに忙しい……
			If .IsConditionSatisfied("踊り") Then
				SelectWeapon = -1
				Exit Function
			End If
			
			'スペシャルパワー等の影響を考えて武器を選択するかを判定
			If .Party = "味方" Then
				use_true_value = True
			End If
			
			'ユニットが移動前かどうかを判定
			If amode = "移動後" Then
				smode = "移動後"
			Else
				smode = "移動前"
			End If
			
			'サポートアタックをしてくれるユニットがいるかどうか
			If InStr(amode, "反撃") = 0 And InStr(amode, "サポート") = 0 Then
				su = .LookForSupportAttack(t)
				If Not su Is Nothing Then
					w = SelectWeapon(su, t, "サポートアタック", support_prob, support_exp_dmg)
					If w > 0 Then
						With su
							support_prob = MinLng(.HitProbability(w, t, use_true_value), 100)
							
							dmg_mod = 1
							
							'サポートアタックダメージ低下
							If IsOptionDefined("サポートアタックダメージ低下") Then
								dmg_mod = 0.7
							End If
							
							'同時援護攻撃？
							If .MainPilot.IsSkillAvailable("統率") And .IsNormalWeapon(w) Then
								If IsOptionDefined("ダメージ倍率低下") Then
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
			
			'各武器を使って試行
			For w = 1 To .CountWeapon
				'武器が使用可能？
				If Not .IsWeaponAvailable(w, smode) Then
					GoTo NextWeapon
				End If
				
				'マップ攻撃は武器選定外
				If .IsWeaponClassifiedAs(w, "Ｍ") Then
					GoTo NextWeapon
				End If
				
				'合体技は自分から攻撃をかける場合にのみ使用
				If .IsWeaponClassifiedAs(w, "合") Then
					If InStr(amode, "反撃") > 0 Or InStr(amode, "サポート") > 0 Then
						GoTo NextWeapon
					End If
				End If
				
				'射程範囲内？
				If .IsWeaponClassifiedAs(w, "移動後攻撃可") And amode = "移動可能" And .Mode <> "固定" Then
					'合体技は移動後攻撃可能でも移動を前提にしない
					'(移動後の位置では使えない危険性があるため)
					If .IsWeaponClassifiedAs(w, "合") And .IsWeaponClassifiedAs(w, "Ｐ") Then
						'移動して攻撃は出来ない
						If Not .IsTargetWithinRange(w, t) Then
							GoTo NextWeapon
						End If
						is_move_attack = False
					Else
						'移動して攻撃可能
						If Not .IsTargetReachable(w, t) Then
							GoTo NextWeapon
						End If
						is_move_attack = True
					End If
				Else
					'移動して攻撃は出来ない
					If Not .IsTargetWithinRange(w, t) Then
						GoTo NextWeapon
					End If
					is_move_attack = False
				End If
				
				'味方ユニットの場合、最後の一発は使用しない
				If .Party = "味方" And .Party0 = "味方" And InStr(amode, "イベント") = 0 Then
					'自爆攻撃は武器を手動選択する場合にのみ使用
					If .IsWeaponClassifiedAs(w, "自") Then
						GoTo NextWeapon
					End If
					
					'手動反撃時のサポートアタック以外は残弾数が少ない武器を使用しない
					'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If amode <> "サポートアタック" Or MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
						If Not .IsWeaponClassifiedAs(w, "永") Then
							If .Bullet(w) = 1 Or .MaxBullet(w) = 2 Or .MaxBullet(w) = 3 Then
								GoTo NextWeapon
							End If
						End If
						If .WeaponENConsumption(w) > 0 Then
							If .WeaponENConsumption(w) >= .EN \ 2 Or .WeaponENConsumption(w) >= .MaxEN \ 4 Then
								GoTo NextWeapon
							End If
						End If
						If .IsWeaponClassifiedAs(w, "尽") Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				'ボスユニットが自爆＆全ＥＮ消費攻撃使うのは非常時のみ
				If .BossRank >= 0 And InStr(amode, "イベント") = 0 Then
					If .IsWeaponClassifiedAs(w, "自") Or .IsWeaponClassifiedAs(w, "尽") Then
						If .HP > .MaxHP \ 4 Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				'特定のユニットをターゲットにしている場合、自爆攻撃はそのターゲットにしか使わない
				If .IsWeaponClassifiedAs(w, "自") Then
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
				
				'ダメージ修正率
				dmg_mod = 1
				
				'サポートアタックダメージ低下
				If InStr(amode, "サポート") > 0 Then
					If IsOptionDefined("サポートアタックダメージ低下") Then
						dmg_mod = 0.7
					End If
				End If
				
				'ダメージ算出
				dmg = .ExpDamage(w, t, use_true_value, dmg_mod)
				
				'攻撃の可否判定を行う場合はダメージを与えられる武器があればよい
				If InStr(amode, "可否判定") > 0 Then
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
					'抹殺攻撃は一撃で倒せる場合でないと効果が無い
					If .IsWeaponClassifiedAs(w, "殺") Then
						GoTo NextWeapon
					End If
					
					'ダメージ増加のスペシャルパワーを使用している場合はダメージを与えられない
					'武器を選択しない
					If .IsUnderSpecialPowerEffect("ダメージ増加") Then
						GoTo NextWeapon
					End If
				End If
				
				'相手のＨＰが10以下の場合はダメージをかさ上げ
				If t.HP <= 10 Then
					If 0 < dmg And dmg < 20 Then
						If .Weapon(w).Power > 0 Then
							dmg = 20
						End If
					End If
				End If
				
				'再攻撃が可能な場合
				If InStr(amode, "サポート") = 0 Then
					If .IsUnderSpecialPowerEffect("再攻撃") Then
						'再攻撃する残弾＆ＥＮがある？
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
					ElseIf .IsWeaponClassifiedAs(w, "再") Then 
						dmg = dmg + dmg * .WeaponLevel(w, "再") \ 16
					End If
				End If
				
				'命中率算出
				prob = .HitProbability(w, t, use_true_value)
				
				'特殊能力による回避を認識する？
				If (.MainPilot.TacticalTechnique >= 150 Or .Party = "味方") And Not .IsUnderSpecialPowerEffect("絶対命中") Then
					'切り払い可能な場合は命中率を低下
					If .IsWeaponClassifiedAs(w, "武") Or .IsWeaponClassifiedAs(w, "突") Or .IsWeaponClassifiedAs(w, "実") Then
						
						'切り払い可能？
						flag = False
						If t.IsFeatureAvailable("格闘武器") Then
							flag = True
						Else
							For i = 1 To t.CountWeapon
								If t.IsWeaponClassifiedAs(i, "武") And t.IsWeaponMastered(i) And t.MainPilot.Morale >= t.Weapon(i).NecessaryMorale And Not t.IsDisabled((t.Weapon(i).Name)) Then
									flag = True
									Exit For
								End If
							Next 
						End If
						If Not t.MainPilot.IsSkillAvailable("切り払い") Then
							flag = False
						End If
						
						'切り払い出来る場合は命中率を低下
						If flag Then
							
							parry_prob = 2 * t.MainPilot.SkillLevel("切り払い")
							If .IsWeaponClassifiedAs(w, "実") Then
								If .IsWeaponClassifiedAs(w, "サ") Then
									parry_prob = parry_prob - .MainPilot.SkillLevel("超感覚") - .MainPilot.SkillLevel("知覚強化")
									With t.MainPilot
										parry_prob = parry_prob + .SkillLevel("超感覚") + .SkillLevel("知覚強化")
									End With
								End If
							Else
								parry_prob = parry_prob - .MainPilot.SkillLevel("切り払い")
							End If
							
							If parry_prob > 0 Then
								prob = prob * (32 - parry_prob) \ 32
							End If
						End If
					End If
					
					'分身可能な場合は命中率を低下
					If t.IsFeatureAvailable("分身") Then
						If t.MainPilot.Morale >= 130 Then
							prob = prob \ 2
						End If
					End If
					If t.MainPilot.SkillLevel("分身") > 0 Then
						prob = prob * t.MainPilot.SkillLevel("分身") \ 16
					End If
					
					'超回避可能な場合は命中率を低下
					If t.IsFeatureAvailable("超回避") Then
						fdata = t.FeatureData("超回避")
						If StrToLng(LIndex(fdata, 2)) > t.EN And StrToLng(LIndex(fdata, 3)) > t.MainPilot.Morale Then
							prob = prob * t.FeatureLevel("超回避") \ 10
						End If
					End If
				End If
				
				'ＣＴ率算出
				ct_prob = .CriticalProbability(w, t)
				
				'特殊効果を与える確率を計算
				sp_prob = 0
				wclass = .WeaponClass(w)
				With t
					For i = 1 To Len(wclass)
						wattr = GetClassBundle(wclass, i)
						
						'特殊効果無効化によって無効化される？
						If .SpecialEffectImmune(wattr) Then
							GoTo NextAttribute
						End If
						
						Select Case wattr
							Case "縛"
								If Not .IsConditionSatisfied("行動不能") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "Ｓ"
								If Not .IsConditionSatisfied("行動不能") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "眠"
								If Not .IsConditionSatisfied("睡眠") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "痺"
								If Not .IsConditionSatisfied("麻痺") Then
									sp_prob = sp_prob + 0.7
								End If
							Case "不"
								If Not .IsConditionSatisfied("攻撃不能") And .CountWeapon > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "止"
								If Not .IsConditionSatisfied("移動不能") And .Speed > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "石"
								If Not .IsConditionSatisfied("石化") And .BossRank < 0 Then
									sp_prob = sp_prob + 1
								End If
							Case "凍"
								If Not .IsConditionSatisfied("凍結") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "乱"
								If Not .IsConditionSatisfied("混乱") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "撹"
								If Not .IsConditionSatisfied("撹乱") And .CountWeapon > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "恐"
								If Not .IsConditionSatisfied("恐怖") Then
									sp_prob = sp_prob + 0.4
								End If
							Case "魅"
								If Not .IsConditionSatisfied("魅了") Then
									sp_prob = sp_prob + 0.6
								End If
							Case "憑"
								If .BossRank < 0 Then
									sp_prob = sp_prob + 1
								End If
							Case "黙"
								If Not .IsConditionSatisfied("沈黙") Then
									For j = 1 To .CountWeapon
										If .IsSpellWeapon(j) Or .IsWeaponClassifiedAs(j, "音") Then
											sp_prob = sp_prob + 0.3
											Exit For
										End If
									Next 
									If j > .CountWeapon Then
										For j = 1 To .CountAbility
											If .IsSpellAbility(j) Or .IsAbilityClassifiedAs(j, "音") Then
												sp_prob = sp_prob + 0.3
												Exit For
											End If
										Next 
									End If
								End If
							Case "盲"
								If Not .IsConditionSatisfied("盲目") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "毒"
								If Not .IsConditionSatisfied("毒") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "踊"
								If Not .IsConditionSatisfied("踊り") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "狂"
								If Not .IsConditionSatisfied("狂戦士") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "ゾ"
								If Not .IsConditionSatisfied("ゾンビ") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "害"
								If Not .IsConditionSatisfied("回復不能") Then
									If .IsFeatureAvailable("ＨＰ回復") Or .IsFeatureAvailable("ＥＮ回復") Then
										sp_prob = sp_prob + 0.4
									End If
								End If
							Case "劣"
								If Not .IsConditionSatisfied("装甲劣化") Then
									sp_prob = sp_prob + 0.3
								End If
							Case "中"
								If Not .IsConditionSatisfied("バリア無効化") Then
									If .IsFeatureAvailable("バリア") And InStr(t.FeatureData("バリア"), "バリア無効化無効") = 0 Then
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("広域バリア") Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("バリアシールド") And InStr(t.FeatureData("バリアシールド"), "バリア無効化無効") = 0 Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("フィールド") And InStr(t.FeatureData("フィールド"), "バリア無効化無効") = 0 Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("広域フィールド") Then 
										sp_prob = sp_prob + 0.3
									ElseIf .IsFeatureAvailable("アクティブフィールド") And InStr(t.FeatureData("アクティブフィールド"), "バリア無効化無効") = 0 Then 
										sp_prob = sp_prob + 0.3
									End If
								End If
							Case "除"
								For j = 1 To .CountCondition
									If (InStr(.Condition(j), "付加") > 0 Or InStr(.Condition(j), "強化") > 0 Or InStr(.Condition(j), "ＵＰ") > 0) And .ConditionLifetime(j) > 0 Then
										sp_prob = sp_prob + 0.3
										Exit For
									End If
								Next 
							Case "即"
								If .BossRank < 0 Then
									sp_prob = sp_prob + 1
								End If
							Case "告"
								If .BossRank < 0 Then
									sp_prob = sp_prob + 0.4
								End If
							Case "脱"
								If .MainPilot.Personality <> "機械" Then
									sp_prob = sp_prob + 0.2
								End If
							Case "Ｄ"
								If .MainPilot.Personality <> "機械" Then
									sp_prob = sp_prob + 0.25
								End If
							Case "低攻"
								If Not .IsConditionSatisfied("攻撃力ＤＯＷＮ") And .CountWeapon() > 0 Then
									sp_prob = sp_prob + 0.2
								End If
							Case "低防"
								If Not .IsConditionSatisfied("防御力ＤＯＷＮ") Then
									sp_prob = sp_prob + 0.2
								End If
							Case "低運"
								If Not .IsConditionSatisfied("運動性ＤＯＷＮ") Then
									sp_prob = sp_prob + 0.1
								End If
							Case "低移"
								If Not .IsConditionSatisfied("移動力ＤＯＷＮ") And .Speed > 0 Then
									sp_prob = sp_prob + 0.1
								End If
							Case "盗"
								If Not .IsConditionSatisfied("すかんぴん") Then
									sp_prob = sp_prob + 0.5
								End If
							Case "写"
								If .BossRank >= 0 Or u.IsFeatureAvailable("ノーマルモード") Then
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
							Case "化"
								If .BossRank < 0 And Not u.IsFeatureAvailable("ノーマルモード") Then
									sp_prob = sp_prob + 1
								End If
							Case "衰"
								If .BossRank >= 0 Then
									Select Case CShort(u.WeaponLevel(w, "衰"))
										Case 1
											sp_prob = sp_prob + 1 / 8
										Case 2
											sp_prob = sp_prob + 1 / 4
										Case 3
											sp_prob = sp_prob + 1 / 2
									End Select
								Else
									Select Case CShort(u.WeaponLevel(w, "衰"))
										Case 1
											sp_prob = sp_prob + 1 / 4
										Case 2
											sp_prob = sp_prob + 1 / 2
										Case 3
											sp_prob = sp_prob + 3 / 4
									End Select
								End If
							Case "滅"
								If .BossRank >= 0 Then
									Select Case CShort(u.WeaponLevel(w, "滅"))
										Case 1
											sp_prob = sp_prob + 1 / 16
										Case 2
											sp_prob = sp_prob + 1 / 8
										Case 3
											sp_prob = sp_prob + 1 / 4
									End Select
								Else
									Select Case CShort(u.WeaponLevel(w, "滅"))
										Case 1
											sp_prob = sp_prob + 1 / 8
										Case 2
											sp_prob = sp_prob + 1 / 4
										Case 3
											sp_prob = sp_prob + 1 / 2
									End Select
								End If
							Case Else
								'弱属性
								If Left(wattr, 1) = "弱" Then
									'味方全員を検索して、現在対象に攻撃可能なユニットが
									'付加した弱点に対する属性攻撃を持つ場合。
									'特殊効果発動率はとりあえず低防(0.2)とそろえてみた
									checkwc = Mid(wattr, 2)
									If Not .Weakness(checkwc) Then
										For	Each checku In UList
											With checku
												If .Party = .Party And .Status_Renamed = "出撃" Then
													For j = 1 To .CountWeapon
														If .IsWeaponClassifiedAs(j, checkwc) And .IsWeaponAvailable(j, "移動前") Then
															'射程範囲内？
															If .IsWeaponClassifiedAs(j, "移動後攻撃可") And .Mode <> "固定" Then
																'合体技は移動後攻撃可能でも移動を前提にしない
																'(移動後の位置では使えない危険性があるため)
																If .IsWeaponClassifiedAs(j, "合") And .IsWeaponClassifiedAs(j, "Ｐ") Then
																	'移動して攻撃は出来ない
																	If .IsTargetWithinRange(j, t) Then
																		sp_prob = sp_prob + 0.2
																		GoTo NextAttribute
																	End If
																Else
																	'移動して攻撃可能
																	If .IsTargetReachable(j, t) Then
																		sp_prob = sp_prob + 0.2
																		GoTo NextAttribute
																	End If
																End If
															Else
																'移動して攻撃は出来ない
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
									'効属性
								ElseIf Left(wattr, 1) = "効" Then 
									'味方全員を検索して、現在対象に攻撃可能なユニットが
									'付加した有効に対する封印、限定攻撃を持つ場合。
									'特殊効果発動率は0.1としてみた
									checkwc = Mid(wattr, 2)
									If Not .Weakness(checkwc) And Not .Effective(checkwc) Then
										For	Each checku In UList
											If checku.Party = .Party And checku.Status_Renamed = "出撃" Then
												For j = 1 To checku.CountWeapon
													With checku
														If .IsWeaponClassifiedAs(j, checkwc) And .IsWeaponAvailable(j, "移動前") Then
															'付加する有効に対応する封印、限定武器がある
															If InStrNotNest(.WeaponClass(j), "封") > 0 Or InStrNotNest(.WeaponClass(j), "限") > 0 Then
																If InStrNotNest(.WeaponClass(j), checkwc) > InStrNotNest(checku.WeaponClass(j), "限") Then
																	'射程範囲内？
																	If .IsWeaponClassifiedAs(j, "移動後攻撃可") And .Mode <> "固定" Then
																		'合体技は移動後攻撃可能でも移動を前提にしない
																		'(移動後の位置では使えない危険性があるため)
																		If .IsWeaponClassifiedAs(j, "合") And .IsWeaponClassifiedAs(j, "Ｐ") Then
																			'移動して攻撃は出来ない
																			If .IsTargetWithinRange(j, t) Then
																				sp_prob = sp_prob + 0.1
																				GoTo NextAttribute
																			End If
																		Else
																			'移動して攻撃可能
																			If .IsTargetReachable(j, t) Then
																				sp_prob = sp_prob + 0.1
																				GoTo NextAttribute
																			End If
																		End If
																	Else
																		'移動して攻撃は出来ない
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
									'剋属性
								ElseIf Left(wattr, 1) = "剋" Then 
									'特殊効果発動率は黙属性揃えで0.3
									checkwc = Mid(wattr, 2)
									Select Case checkwc
										Case "オ"
											If Not .IsConditionSatisfied("オーラ使用不能") Then
												If .IsSkillAvailable("オーラ") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "超"
											If Not .IsConditionSatisfied("超能力使用不能") Then
												If .IsSkillAvailable("超能力") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "シ"
											If Not .IsConditionSatisfied("同調率使用不能") Then
												If .IsSkillAvailable("同調率") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "サ"
											If Not .IsConditionSatisfied("超感覚使用不能") And Not .IsConditionSatisfied("知覚強化使用不能") Then
												If .IsSkillAvailable("超感覚") Or .IsSkillAvailable("知覚強化") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "霊"
											If Not .IsConditionSatisfied("霊力使用不能") Then
												If .IsSkillAvailable("霊力") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "術"
											'術は射撃を魔力と表示するためだけに付いている場合があるため
											'1レベル以下の場合は武器、アビリティを確認
											If Not .IsConditionSatisfied("術使用不能") Then
												If .SkillLevel("術") > 1 Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
										Case "技"
											If Not .IsConditionSatisfied("技使用不能") Then
												If .IsSkillAvailable("技") Then
													sp_prob = sp_prob + 0.3
												End If
											Else
												GoTo NextAttribute
											End If
									End Select
									
									If Not .IsConditionSatisfied(checkwc & "属性使用不能") Then
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
				
				'バリア等で攻撃が防がれてしまう場合は特殊効果は発動しない
				If .WeaponPower(w, "") > 0 And dmg = 0 And Not .IsWeaponClassifiedAs(w, "無") Then
					sp_prob = 0
				End If
				
				'必ず発動する特殊効果を考慮
				If .IsWeaponClassifiedAs(w, "吸") Then
					If .HP < .MaxHP Then
						sp_prob = sp_prob + 25 * dmg \ t.MaxHP
					End If
				End If
				If .IsWeaponClassifiedAs(w, "減") Then
					sp_prob = sp_prob + 50 * dmg \ t.MaxHP
				End If
				If .IsWeaponClassifiedAs(w, "奪") Then
					sp_prob = sp_prob + 50 * dmg \ t.MaxHP
				End If
				
				'先制攻撃の場合は特殊効果を有利に判定
				If InStr(amode, "反撃") > 0 Then
					If .IsWeaponClassifiedAs(w, "先") Or .UsedCounterAttack < .MaxCounterAttack Then
						sp_prob = 1.5 * sp_prob
					End If
				End If
				If sp_prob > 100 Then
					sp_prob = 100
				End If
				
				'ＣＴ率が低い場合は特殊効果のみの攻撃を重視しない
				If dmg = 0 And ct_prob < 30 Then
					sp_prob = sp_prob / 5
				End If
				
				'ダメージが与えられない武器は使用しない
				If dmg = 0 And sp_prob = 0 Then
					GoTo NextWeapon
				End If
				
				If prob > 0 Then
					If sp_prob > 0 Then
						'特殊効果の影響を加味してダメージの期待値を計算
						exp_dmg = dmg + MaxLng(t.HP - dmg, 0) * sp_prob \ 100
					Else
						'クリティカルの影響を加味してダメージの期待値を計算
						If IsOptionDefined("ダメージ倍率低下") Then
							If .IsWeaponClassifiedAs(w, "痛") Then
								exp_dmg = dmg + 0.1 * .WeaponLevel(w, "痛") * dmg * ct_prob \ 100
							Else
								exp_dmg = dmg + 0.2 * dmg * ct_prob \ 100
							End If
						Else
							If .IsWeaponClassifiedAs(w, "痛") Then
								exp_dmg = dmg + 0.25 * .WeaponLevel(w, "痛") * dmg * ct_prob \ 100
							Else
								exp_dmg = dmg + 0.5 * dmg * ct_prob \ 100
							End If
						End If
					End If
					exp_dmg = exp_dmg * 0.01 * MinLng(prob, 100)
				Else
					'命中が当たらない場合は期待値を思い切り下げる
					prob = 1
					exp_dmg = (dmg \ 10 + MaxLng(t.HP - dmg \ 10, 0) * sp_prob \ 100) \ 10
				End If
				
				'サポートによるダメージを期待値に追加
				If Not is_move_attack Then
					exp_dmg = exp_dmg + support_exp_dmg
				End If
				
				'敵の破壊確率を計算
				destroy_prob = 0
				With t
					If .Party = "味方" And Not .IsFeatureAvailable("防御不可") Then
						If dmg >= 2 * .HP Then
							destroy_prob = MinLng(prob, 100)
						End If
						'サポートによる破壊確率
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
						'サポートによる破壊確率
						If Not is_move_attack Then
							If support_dmg >= .HP Then
								destroy_prob = destroy_prob + (100 - destroy_prob) * support_prob \ 100
							ElseIf dmg + support_dmg >= .HP Then 
								destroy_prob = destroy_prob + (100 - destroy_prob) * prob * support_prob \ 10000
							End If
						End If
					End If
				End With
				
				'先制攻撃の場合は敵を破壊出来る攻撃を有利に判定
				If InStr(amode, "反撃") > 0 Then
					If .IsWeaponClassifiedAs(w, "先") Or .UsedCounterAttack < .MaxCounterAttack Then
						destroy_prob = 1.5 * destroy_prob
					End If
				End If
				
				'ＥＮ消耗攻撃の使用は慎重に
				If .IsWeaponClassifiedAs(w, "消") Then
					If .Party = "味方" Then
						'自動反撃モードかどうか
						'UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
							GoTo NextWeapon
						End If
					Else
						'敵ユニットは相手を倒せるときにしかＥＮ消耗攻撃を使わない
						If destroy_prob = 0 And .BossRank < 0 Then
							GoTo NextWeapon
						End If
					End If
				End If
				
				If destroy_prob >= 100 Then
					'破壊確率が100%の場合はコストの低さを優先
					'(確率が同じ場合は番号が低い武器を使用)
					If .Party = "味方" Or .Party = "ＮＰＣ" Then
						If destroy_prob > max_destroy_prob Then
							SelectWeapon = w
							max_destroy_prob = destroy_prob
							max_exp_dmg = exp_dmg
						End If
					Else
						'敵の場合はコスト無視
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
					'破壊確率が50%より高い場合は破壊確率の高さを優先
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
					'破壊確率が50%以下の場合はダメージの期待値の高さを優先
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
			
			'ダメージを与えられない武器が選択された場合はキャンセル
			If SelectWeapon > 0 Then
				If .WeaponAdaption(SelectWeapon, (t.Area)) = 0 Then
					SelectWeapon = 0
				End If
			End If
			
			'攻撃結果の期待値の書き込み
			If max_destroy_prob > 50 Then
				max_prob = max_destroy_prob
			Else
				max_prob = 0
			End If
			max_dmg = 100 * (max_exp_dmg / .HP)
		End With
	End Function
	
	'ユニット u が武器 w で攻撃をかけた際にターゲット t が選択する防御行動を返す
	Public Function SelectDefense(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, ByRef tw As Short) As Object
		Dim prob, dmg As Integer
		Dim tprob, tdmg As Integer
		Dim is_target_inferior As Boolean
		
		'マップ攻撃に対しては防御行動が取れない
		If u.IsWeaponClassifiedAs(w, "Ｍ") Then
			Exit Function
		End If
		
		With t
			'踊っている場合は回避扱い
			If .IsConditionSatisfied("踊り") Then
				'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SelectDefense = "回避"
				Exit Function
			End If
			
			'狂戦士状態の際は防御行動を取らない
			If .IsConditionSatisfied("狂戦士") Then
				Exit Function
			End If
			
			'無防備状態のユニットは防御行動が取れない
			If .IsUnderSpecialPowerEffect("無防備") Then
				Exit Function
			End If
			
			If .Party <> "味方" Then
				'「敵ユニット防御使用」オプションを選択している場合にのみ敵ユニットは
				'防御行動を行う
				If Not IsOptionDefined("敵ユニット防御使用") Then
					Exit Function
				End If
				
				'防御行動を使ってくるのは技量が160以上のザコでないパイロットのみ
				With .MainPilot
					If InStr(.Name, "(ザコ)") > 0 Or .TacticalTechnique < 160 Then
						Exit Function
					End If
				End With
			End If
			
			'行動不能？
			If .MaxAction = 0 Then
				'チャージ中、消耗中は常に防御、それ以外の場合は防御行動が取れない
				If .IsConditionSatisfied("チャージ") Or .IsConditionSatisfied("消耗") Then
					'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
					SelectDefense = "防御"
				End If
				Exit Function
			End If
			
			'相手の攻撃のダメージ・命中率を算出
			dmg = u.ExpDamage(w, t, True)
			prob = MinLng(u.HitProbability(w, t, True), 100)
			
			'ダミーを持っている場合、相手の攻撃は無効
			If .IsFeatureAvailable("ダミー") And .ConditionLevel("ダミー破壊") < .FeatureLevel("ダミー") Then
				prob = 0
			End If
			
			'サポートガードされる場合も相手の攻撃は無効
			If Not .LookForSupportGuard(u, w) Is Nothing Then
				prob = 0
			End If
			
			'反撃のダメージ・命中率を算出
			If tw > 0 Then
				tdmg = .ExpDamage(tw, u, True)
				tprob = MinLng(.HitProbability(tw, u, True), 100)
				
				'ダミーを持っている場合は反撃は無効
				If u.IsFeatureAvailable("ダミー") And u.ConditionLevel("ダミー破壊") < u.FeatureLevel("ダミー") Then
					prob = 0
				End If
			End If
			
			'相手の攻撃の効果とこちらの反撃の効果を比較
			If .Party = "味方" Then
				'味方ユニットの場合、相手の攻撃によるダメージの方が多い場合は防御
				If dmg * prob > tdmg * tprob And tdmg < u.HP Then
					is_target_inferior = True
				End If
				
				'気合の一撃は防御を優先し、やり過ごす
				If u.IsUnderSpecialPowerEffect("ダメージ増加") Then
					If 2 * dmg * prob > tdmg * tprob And tdmg < u.HP Then
						is_target_inferior = True
					End If
				End If
			Else
				'敵ユニットの場合でも相手の攻撃によるダメージの方が２倍以上多い場合は防御
				If dmg * prob \ 2 > tdmg * tprob And tdmg < u.HP Then
					is_target_inferior = True
				End If
				
				'気合の一撃は防御を優先し、やり過ごす
				If u.IsUnderSpecialPowerEffect("ダメージ増加") Then
					If dmg * prob > tdmg * tprob And tdmg < u.HP Then
						is_target_inferior = True
					End If
				End If
			End If
			
			'あと一撃で破壊されてしまう場合は必ず防御
			'(命中率が低い場合を除く)
			If dmg >= .HP And prob > 10 Then
				is_target_inferior = True
			End If
			
			If tw > 0 Then
				'先制攻撃可能？
				If Not .IsWeaponClassifiedAs(tw, "後") Then
					If .IsWeaponClassifiedAs(tw, "先") Or u.IsWeaponClassifiedAs(w, "後") Or .MaxCounterAttack > .UsedCounterAttack Then
						If tdmg >= u.HP And tprob > 70 Then
							'先制攻撃で倒せる場合は迷わず反撃
							is_target_inferior = False
						End If
					End If
				End If
			Else
				'反撃できない場合は防御
				is_target_inferior = True
			End If
			
			If Not is_target_inferior Then
				'反撃を選択した
				Exit Function
			End If
			
			'防御側が劣勢なので反撃は行わず、防御行動を選択
			
			'命中すれば一撃死で、防御すれば破壊をまぬがれる攻撃は必ず防御
			If dmg > .HP And dmg \ 2 < .HP And Not .IsFeatureAvailable("防御不可") And Not u.IsWeaponClassifiedAs(w, "殺") Then
				'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SelectDefense = "防御"
				Exit Function
			End If
			
			'相手の命中率が低い場合は回避
			If prob < 50 And Not .IsFeatureAvailable("回避不可") And Not .IsConditionSatisfied("移動不能") Then
				'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SelectDefense = "回避"
				Exit Function
			End If
			
			'防御すれば一撃死をまぬがれる場合は防御
			If dmg \ 2 < .HP And Not .IsFeatureAvailable("防御不可") And Not u.IsWeaponClassifiedAs(w, "殺") Then
				'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SelectDefense = "防御"
				Exit Function
			End If
			
			'どうしようもないのでとりあえず回避
			If Not .IsFeatureAvailable("回避不可") And Not .IsConditionSatisfied("移動不能") Then
				'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SelectDefense = "回避"
				Exit Function
			End If
			
			'回避も出来ないので防御……
			If Not .IsFeatureAvailable("防御不可") Then
				'UPGRADE_WARNING: オブジェクト SelectDefense の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SelectDefense = "防御"
			End If
		End With
	End Function
	
	'ユニット u がターゲット t に反撃可能か？
	Public Function IsAbleToCounterAttack(ByRef u As Unit, ByRef t As Unit) As Boolean
		Dim i, w, idx As Short
		Dim buf, wclass, ch As String
		
		With u
			For w = 1 To .CountWeapon
				'武器が使用可能？
				If Not .IsWeaponAvailable(w, "移動前") Then
					GoTo NextWeapon
				End If
				
				'マップ攻撃は武器選定外
				If .IsWeaponClassifiedAs(w, "Ｍ") Then
					GoTo NextWeapon
				End If
				
				'合体技は自分から攻撃をかける場合にのみ使用
				If .IsWeaponClassifiedAs(w, "合") Then
					GoTo NextWeapon
				End If
				
				'射程範囲内？
				If Not .IsTargetWithinRange(w, t) Then
					GoTo NextWeapon
				End If
				
				' ADD START マージ
				'ダメージを与えられる？
				If .Damage(w, t, True) > 0 Then
					IsAbleToCounterAttack = True
					Exit Function
				End If
				
				'特殊効果を与えられる？
				If Not .IsNormalWeapon(w) Then
					If .CriticalProbability(w, t) > 0 Then
						IsAbleToCounterAttack = True
						Exit Function
					End If
				End If
				' ADD END マージ
				
				' DEL START マージ
				'            '地形適応は？
				'            If .WeaponAdaption(w, t.Area) = 0 Then
				'                GoTo NextWeapon
				'            End If
				'
				'            '封印攻撃は弱点、有効を持つユニット以外には効かない
				'            If .IsWeaponClassifiedAs(w, "封") Then
				'                wclass = .WeaponClass(w)
				'                buf = t.strWeakness & t.strEffective
				'                For i = 1 To Len(buf)
				'                    ch = GetClassBundle(buf, i)
				'                    If ch <> "物" And ch <> "魔" Then
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
				'            '限定攻撃は指定属性を弱点、有効を持つユニット以外には効かない
				'            idx = InStrNotNest(.WeaponClass(w), "限")
				'            If idx > 0 Then
				'                wclass = .WeaponClass(w)
				'                buf = t.strWeakness & t.strEffective
				'                For i = 1 To Len(buf)
				'                    ch = GetClassBundle(buf, i)
				'                    If ch <> "物" And ch <> "魔" Then
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
				'            '特定レベル限定攻撃
				'            If .IsWeaponClassifiedAs(w, "対") Then
				'                If t.MainPilot.Level Mod .WeaponLevel(w, "対") <> 0 Then
				'                    GoTo NextWeapon
				'                End If
				'            End If
				'
				'            '反撃に使用できる武器が見つかった
				'            IsAbleToCounterAttack = True
				'            Exit Function
				' DEL END マージ
NextWeapon: 
			Next 
		End With
		
		'反撃に使用できる武器がなかった
		IsAbleToCounterAttack = False
	End Function
	
	'最も近い敵ユニットを探す
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
					
					'もっと近くにいる敵を発見済み？
					If distance <= System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) Then
						GoTo NexLoop
					End If
					
					'敵？
					If .IsAlly(t) Then
						GoTo NexLoop
					End If
					
					'特定の陣営のみを狙う思考モードの場合
					If .Mode = "味方" Or .Mode = "ＮＰＣ" Or .Mode = "敵" Or .Mode = "中立" Then
						If t.Party <> .Mode Then
							GoTo NexLoop
						End If
					End If
					
					'目視不能？
					If t.IsUnderSpecialPowerEffect("隠れ身") Or t.Area = "地中" Then
						GoTo NexLoop
					End If
					
					'ステルス状態にあれば遠くからは発見できない
					If t.IsFeatureAvailable("ステルス") And Not t.IsConditionSatisfied("ステルス無効") And Not .IsFeatureAvailable("ステルス無効化") Then
						If t.IsFeatureLevelSpecified("ステルス") Then
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > t.FeatureLevel("ステルス") Then
								GoTo NexLoop
							End If
						Else
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > 3 Then
								GoTo NexLoop
							End If
						End If
					End If
					
					'ターゲットを発見
					SearchNearestEnemy = t
					distance = System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y)
					
NexLoop: 
				Next 
			Next 
		End With
	End Function
	
	'最も近い敵ユニットへの距離を返す
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