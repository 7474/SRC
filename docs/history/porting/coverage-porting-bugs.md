# カバレッジ強化で見つかった移植の不具合

> 記録として残している文書です（2026-10-15 時点）。現在の状態は [移植の現状](../../porting.md) を参照してください。

## この文書について

SRCCore のテストカバレッジを強化する作業で、VB 版 `SRC/SRC_20121125` の動作を仕様としてテストを書いた。その過程で、C# 版が VB 版と違う動きをする箇所が見つかったので、ここにまとめる。各項目はレビュー担当が VB の元コードと照合して確認済み。

- 不具合のある箇所には、期待する（VB と同じ）動作を確かめるテストを書き、`[Ignore("移植の不具合: ...")]` を付けてある。修正したら `[Ignore]` を外し、テストが通ることを確認する。
- 該当テストは `grep -rn '移植の不具合' SRC.Sharp/SRCCoreTests/Coverage` で一覧できる（すべて修正済みのため、いまは 0 件）。
- 表の書き方
  - C# の箇所は `SRC.Sharp/SRCCore/` からの相対パス、VB の箇所は `SRC/SRC_20121125/` からの相対パス。
  - テスト名は `SRC.Sharp/SRCCoreTests/Coverage/` からの相対パスと `::メソッド名`。
  - 行番号は 2026-10 時点のもの。C# の行番号は sed / grep で確認し、ずれていたものは直した。
- 「テストなし」の項目は、テストを書くコストが高い、または今は表に出ないため、テストを書いていないもの。

## 修正の状況

- **121 件すべてを修正済み**（表の概要に「✅ 修正済み」）。各項目のテストは `[Ignore]` を外して通している（7474/SRC#1215 で 57 件、続く PR で残り 64 件）
- 修正の途中で見つかった同じ種類の違い（丸め・ラベルの有無の判定・合体の距離・武器の引き継ぎ・装填など）も、テストを足して直した
- 以降に見つかった不具合は、同じく VB どおりの期待値のテストを書いて `[Ignore("移植の不具合: ...")]` にし、この一覧に加える

## 件数

**報告 132 件 → 重複をまとめて 121 件**

25 の対象領域で別々に報告されたもののうち、同じコードの同じ原因を指しているもの（例: Unit.Update で耐性・弱点などの文字列が作られない件が 4 領域から報告されている）を 1 件にまとめた。原因が同じ種類でも、直す箇所が違うもの（例: 丸めの違いが別々の関数にある）は別の件として数え、同じ分類にまとめている。

| 分類 | 件数（まとめた後） | 元の報告数 |
|---|---:|---:|
| 1. 数値の丸め（VB の Integer/Long 代入は銀行丸め、C# は切り捨て） | 13 | 13 |
| 2. 属性・特殊能力の組み立て（Update）の欠落 | 8 | 11 |
| 3. 条件の反転・取り違え | 16 | 17 |
| 4. ループの抜け方の違い（Exit Do / Exit For と break） | 11 | 12 |
| 5. 0 始まり・「見つからない」値の違い | 6 | 10 |
| 6. 特殊能力データではなく名前の文字列を読んでいる | 4 | 4 |
| 7. DEFAULT_LEVEL（-1000）の扱い | 3 | 3 |
| 8. 例外で止まる | 6 | 6 |
| 9. その他: 戦闘・移動・アビリティの処理 | 10 | 11 |
| 10. その他: コマンド・一覧・メッセージ表示 | 29 | 30 |
| 11. その他: データ読み込み・イベント・セーブ | 15 | 15 |
| 合計 | 121 | 132 |

## 優先して直すもの

戦闘の結果（ダメージ、命中・回避、弾数、EN、気力、経験値）が変わるもの。上から順に影響が大きい。

1. **2-1 Unit.Update で耐性・弱点・無効化・吸収・有効・特殊効果無効化の文字列が作られない**。データで定義した 耐性=火 などがダメージにも状態異常にもアビリティの判定にも効かない。影響範囲がいちばん広い。
2. **7-2 UpdateSupportMod の cmd_level が -1000 から始まる**。範囲内に指揮官がいないザコパイロットの命中・回避が -5000 になる。
3. **7-1 レベルのないスペシャルパワー効果のレベルが -1000 になる**。レベルのない ＨＰ回復 で RecoverHP(-10000) が呼ばれる。
4. **2-6 Unit.Update でアビリティの使用回数が 0 になる**。アビリティを誰かに使うと、対象のアビリティがすべて使えなくなる。
5. **4-1 WeaponLevel、4-2 AbilityLevel が次の属性で止まらない**。`実L3突` のようにレベルの後に属性が続くと、レベルが 0 になる。
6. **2-2 ＣＴ率強化・特殊効果発動率強化が反映されない**、**2-3 / 2-4 攻撃属性の付け方の誤り**、**2-5 無効にしたアイテムの武器が残る**。
7. 弾数・EN: **9-1 斉射で他の武器の弾数が回復する**、**4-9 合体技で 2 人目以降のパートナーが弾数・EN を消費しない**、**6-2 / 6-3 テレポート・ジャンプで EN を消費しない**、**1-2〜1-6、1-11 の丸め**。
8. **3-1 盾が無防備状態でも使われる**、**4-6 隣接ユニットＺＯＣ無効化が味方の ZOC も消す**、**9-5 進入不可・移動制限が効かない**、**6-1 先手必勝の気力条件が読まれない**。
9. 例外で止まるもの（分類 8 と 4-10）。発生した時点でゲームが止まる。

表示だけが変わるもの（後回しでよい）: 分類 10 の大半（一覧の空白、表題、並べ替えメニュー、一覧の列、10-1 の Info の文字列）、11-8・11-9 のウィンドウ表題やスクロール位置など。ただし 10-21・10-22（インターミッションの換装・合体）、10-12〜10-14（使えないコマンドが出る）は操作に影響するので、表示系の中では先に直す。

---

## 1. 数値の丸め（VB の Integer/Long 代入は銀行丸め、C# は切り捨て）

VB では Double を Integer / Long の変数・戻り値に代入すると銀行丸め（偶数丸め）になる。`\` 演算子も被演算子を先に丸める。C# の移植では `(int)` キャストを使っているため切り捨てになり、x.5 以上の端数で 1 小さくなる。直すときは `VBMath.CLng` など既存の丸め関数に揃える。

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 1-1 | ✅ 修正済み。気力吸収（Ｄ）で攻撃側が得る気力 | Units/Unit.attackcheck.cs:6234 | Unit.cls:15374（IncreaseMorale の引数は Integer） | Lv3 で 7.5 → VB 8、C# 7。気力が 1 少ない | UnitAttackCheck/UnitAttackCheckCauseEffectTests.cs::MoraleAbsorb_Level3_RoundsLikeVB |
| 1-2 | ✅ 修正済み。Bullet()（残弾 = 比率 × 最大弾数） | Units/UnitWeapon.cs:5084 | Unit.cls Bullet（Integer に代入） | (1/49)×49 が 0 になる。弾があるのに 0 発表示・使用不可になりうる | UnitWeapon/UnitWeaponCovUseTests.cs::Bullet_RoundsRatioTimesMax |
| 1-3 | ✅ 修正済み。武器の消費 EN | Units/UnitWeapon.cs:839-977 | Unit.cls:5347-5412 | 11×0.9 が VB 10、C# 9。EN 消費が少ない | UnitWeapon/UnitWeaponCovClassTests.cs::WeaponENConsumption_RoundsLikeVbIntegerAssignment |
| 1-4 | ✅ 修正済み。斉射武器の残弾（他の武器を使った後）`CInt(x + 0.49999)` | Units/UnitWeapon.cs:5010 | Unit.cls:17544 | 3×0.5+0.49999 が VB 2、C# 1。弾数が減る | UnitWeapon/UnitWeaponCovUseTests.cs::UseWeapon_OtherWeapon_RoundsFullBurstAmmoUp |
| 1-5 | ✅ 修正済み。部隊ユニットの攻撃力低下（`\ 100`） | Units/UnitWeapon.cs:205 | Unit.cls WeaponPower 部隊ユニット分岐 | 5599.8 が VB 56、C# 55。攻撃力が 1 低い | UnitWeapon/UnitWeaponCovAdaptionPowerTests.cs::WeaponPower_SquadUnit_RoundsBeforeIntegerDivision |
| 1-6 | ✅ 修正済み。SetMaxBulletRate（ボスランクなどによる最大弾数の増加） | Units/UnitWeapon.cs:5096 | Unit.cls:3306 | 弾数 3 × 1.2 が VB 4、C# 3 | UnitWeapon/UnitWeaponCovExtraTests.cs::SetMaxBulletRate_RoundsLikeVbIntegerAssignment |
| 1-7 | ✅ 修正済み。AreaInPointToPoint / IsLineBlocked の途中座標 | Maps/Map.cs:1292, 1311, 3102, 3152 | Map.bas:1543, 1554, 3139, 3170 | (1,1)→(4,2) で VB は (3,2) を通るが C# は通らない。直線範囲と壁による遮断判定が変わる | Map/MapAreaShapeCoverageTests.cs::AreaInPointToPoint_RoundsIntermediateCoordinatesLikeVB、Map/MapAreaShapeCoverageTests.cs::IsLineBlocked_RoundsIntermediateCoordinatesLikeVB |
| 1-8 | ✅ 修正済み。Pilot.MaxSP の ＳＰ高成長 ×1.5 | Pilots/Pilot.sp.cs:59 | Pilot.cls:1300 | Lv41 で 121.5 → VB 122、C# 121 | SpecialPower/SpecialPowerPilotTests.cs::MaxSP_HighGrowth_RoundsLikeVBLongAssignment |
| 1-9 | ✅ 修正済み。SpecialPowerCost の 集中力・超能力 ×0.8、知覚強化 ×1.2 | Pilots/Pilot.sp.cs:261, 266 | Pilot.cls:2639, 2642 | 消費 7 + 集中力 が VB 6、C# 5。13 + 知覚強化 が VB 16、C# 15 | SpecialPower/SpecialPowerPilotTests.cs::SpecialPowerCost_ConcentrationRoundsLikeVBIntegerAssignment |
| 1-10 | ✅ 修正済み。GetExp の倍率計算 | Units/Unit.action2.cs:95, 103, 126, 130, 150-156 | Unit.cls:26633（xp As Long）, 26696, 26701, 26721, 26730-26746 | 1.5×5 = 7.5 が VB 8、C# 7。獲得経験値が少ない | Units/UnitMapActionAction2Tests.cs::GetExp_FractionalMultiplier_RoundsLikeVbLongAssignment |
| 1-11 | ✅ 修正済み。SyncBullet（共 属性の弾数同期）`CInt(x + 0.49999)` | Units/Unit.recover.cs:1288, 1304, 1330, 1346, 1365 | Unit.cls:17646-17651 | 最大 2・満タンの A と最大 3・残 2 の B で、VB は A=2、C# は A=1 | Units/UnitStatusRecoverRecoverTests.cs::SyncBullet_SharedWeapons_RoundsWhenFewerMaxBullets |
| 1-12 | ✅ 修正済み。COM SelectWeapon の max_dmg / exp_dmg | COM.cs:6578（exp_dmg は 6386-6415 付近） | COM.bas SelectWeapon | 期待値 33 が 32。AI の武器選択が変わりうる | COM/COMSelectWeaponTests.cs::SelectWeapon_MaxDmgIsRoundedLikeVBLongAssignment |
| 1-13 | ✅ 修正済み。Unit.Value（ユニットの価値。ボスランク・ランク補正） | Units/Unit.props.cs:1099, 1103 | Unit.cls:907-920 | 1030×1.05 = 1081.5 が VB 1082、C# 1081。撃墜時の資金などに響く | UnitCore/UnitCorePropsTests.cs::Value_FractionIsRoundedLikeVbLong |

## 2. 属性・特殊能力の組み立て（Update）の欠落

Unit.Update / Pilot.Update で、VB では特殊能力から組み立てていた値が、C# ではコメントアウトされているか一部しか移植されていない。

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 2-1 | ✅ 修正済み。**Unit.Update が strAbsorb / strImmune / strResist / strWeakness / strEffective / strSpecialEffectImmune を作らない**（UnitAttackCheck、UnitWeapon、UnitAbility、UnitStatusRecover の 4 領域から報告。1 件にまとめた） | Units/Unit.status.cs:1070-1215 付近（ブロック全体がコメントアウト）。代入は Unit.ref.cs にしかなく、このファイルはビルド対象外。読み出しは Units/Unit.attribute.cs:369 など | Unit.cls:2036-2121 | 特殊能力で与えた 吸収・無効化・耐性・弱点・有効・特殊効果無効化 がすべて効かない。ダメージ計算、CauseEffect（状態異常）、ExecuteAbility の `t.Immune(aclass)`、IsAbilityApplicable の 封/限/無効化、Rest の毒・地形による HP 減少の補正がすべて変わる | UnitAttackCheck/UnitAttackCheckCauseEffectTests.cs::SpecialEffectImmune_PreventsEffect、UnitWeapon/UnitWeaponCovCombatTests.cs::Damage_ResistanceAttribute_Halves、Units/UnitAbilityCoverageReviewTests.cs::Execute_TargetImmuneToClass_HasNoEffect、Units/UnitAbilityCoverageReviewTests.cs::Applicable_SealedClass_RequiresWeaknessOrEffective、Units/UnitAbilityCoverageReviewTests.cs::Applicable_ImmuneTarget_IsFalse、Units/UnitStatusRecoverUpdateTests.cs::DefenseFeatures_UpdateAttributeResistance、Units/UnitStatusRecoverRecoverTests.cs::Rest_Poison_ModifiedByResistanceAndWeakness、Units/UnitStatusRecoverRecoverTests.cs::Rest_TerrainDecrease_ModifiedByAttributes、Units/UnitStatusRecoverRecoverTests.cs::Rest_TerrainCondition_ImmuneUnitIsNotAffected |
| 2-2 | ✅ 修正済み。ＣＴ率強化 / 特殊効果発動率強化 の補正値を計算するが武器に設定しない | Units/Unit.status.cs:1712-1752 | Unit.cls:3002-3093, 3188 | クリティカル率・特殊効果発動率が 5×レベル 分低い | Units/UnitStatusRecoverUpdateTests.cs::CriticalEnhance_Adds5PerLevel、Units/UnitStatusRecoverUpdateTests.cs::SpecialEffectRateEnhance_Adds5PerLevelToEffectWeapons |
| 2-3 | ✅ 修正済み。攻撃属性 で隠し属性（`|` 以降）を切り取り後の文字列から取り、隠し属性がなくても `|` を付ける | Units/Unit.status.cs:1243-1244, 1333 | Unit.cls:2196-2200, 2348-2350 | `格|毒` + 攻撃属性=火 が `格火|格` になり毒が消える。`格` は `格火|` になる | Units/UnitStatusRecoverUpdateTests.cs::AttackAttribute_KeepsHiddenAttribute、Units/UnitStatusRecoverUpdateTests.cs::AttackAttribute_NoHiddenAttribute_NoTrailingBar |
| 2-4 | ✅ 修正済み。攻撃属性 の武器指定リストの先頭を読み飛ばす（skip=2） | Units/Unit.status.cs:1272-1278 | Unit.cls:2228-2232 付近 | `攻撃属性=火 パンチ` で対象リストが空になり、全武器に火が付く | Units/UnitStatusRecoverUpdateTests.cs::AttackAttribute_WithTarget_AppliesOnlyToMatchingWeapons |
| 2-5 | ✅ 修正済み。無効化（Disable）されたアイテムの武器もユニットに追加する | Units/Unit.status.cs:1229 | Unit.cls:2167-2176（If .Activated） | 使えないはずのアイテム武器で攻撃できる | Units/UnitStatusRecoverUpdateTests.cs::ItemWeapons_OnlyFromActivatedItems |
| 2-6 | ✅ 修正済み。Unit.Update がアビリティを作り直すとき使用回数（dblStock）を引き継がない | Units/Unit.status.cs:1812-1815（引き継ぎ処理は 1816-1922 付近でコメントアウト）。ExecuteAbility が毎回 Update を呼ぶ（Units/Unit.ability.cs:1890） | Unit.cls:3336-3339, 3394-3399 | 対象にアビリティを使うと、対象のアビリティの使用回数がすべて 0 になる | Units/UnitAbilityCoverageReviewTests.cs::Update_KeepsAbilityStock、Units/UnitAbilityCoverageReviewTests.cs::Reload_WithoutData_RefillsSharedAbilityStock |
| 2-7 | ✅ 修正済み。Pilot.Update で パイロット地形適応変更 が未移植、最後の SetMorale がない | Pilots/Pilot.status.cs:398-538（どちらもコメントアウト。SetMorale は 536） | Pilot.cls:761-818, 824 | 地形適応が変わらない。気力上限が下がっても気力が上限に収まらない（150 のまま、期待 120） | Pilots/PilotSkillStatusUpdateTests.cs::Update_パイロット地形適応変更で地形適応が変わる、Pilots/PilotSkillStatusReviewTests.cs::Update_気力上限が下がると気力が上限に収められる |
| 2-8 | ✅ 修正済み。Pilot.Update の ハンター / ＳＰ消費減少 / スペシャルパワー自動発動 の重複判定が `x.StrData == x.StrData`（自分同士の比較） | Pilots/Pilot.status.cs:37 | Pilot.cls:447-452 | データの違う同名能力が 1 つにまとめられる | Pilots/PilotSkillStatusUpdateTests.cs::Update_ハンターはデータが異なれば複数持てる |

## 3. 条件の反転・取り違え

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 3-1 | ✅ 修正済み。CheckShieldFeature の行動可能判定が `無防備` ではなく `行動不能1` を見ている | Units/Unit.attackcheck.cs:2279 | Unit.cls:12520 | 無防備状態でもシールド防御が発動する | UnitAttackCheck/UnitAttackCheckDefenseTests.cs::ShieldDefense_Defenseless_NotUsed |
| 3-2 | ✅ 修正済み。IdDisplayFor の合体技の条件が反転 | Units/UnitAbility.cs:1579 | GUI.bas:5476-5480、Command.bas:472-475 | 使える合体アビリティが隠れ、使えないものが表示される | Units/UnitAbilityCoveragePortingBugTests.cs::IdDisplayFor_HidesUnavailableCombinationAbility |
| 3-3 | ✅ 修正済み。IsTargetWithinAbilityRange の射程 1 合体技のパートナー判定が反転 | Units/UnitAbility.cs:1363-1369 | Unit.cls:18838-18846 | パートナーがいないのに射程内と判定される | Units/UnitAbilityCoveragePortingBugTests.cs::TargetWithinRange_CombinationWithoutPartner_IsFalse |
| 3-4 | ✅ 修正済み。**Pilot.SkillType の 付加 判定が反転**（`IsNullOrEmpty(conditionType)` で return）（UnitLookup と PilotSkillStatus から報告。1 件にまとめた） | Pilots/Pilot.skill.cs:1205 | Pilot.cls:2511-2535 | 付加で与えた別名の種類が返らない。ユニットが何か状態を持つと、IsNecessarySkillSatisfied2 のユニット名・@地形・装備・隣接・状態 の分岐が飛ばされ、「～状態」の必要技能が満たされない | Units/UnitLookupSkillTests.cs::IsNecessarySkillSatisfied2_Condition_WithStateSuffix、Pilots/PilotSkillStatusSkillTests.cs::SkillType_付加で与えられた別名から能力の種類を返す、Pilots/PilotSkillStatusSkillTests.cs::SkillType_付加に該当しない未修得能力は名称そのまま |
| 3-5 | ✅ 修正済み。Intermission.InStatusCommand のマップ判定が反転（`!Map.IsStatusView`） | Intermissions/Intermission.cs:2198 | Intermission.bas:3067 | ステータス表示中に `+` 付き必要技能が満たされない。サブステージの戦闘中には満たされてしまう | Units/UnitLookupSkillTests.cs::IsNecessarySkillSatisfied2_PlusPrefix_SatisfiedInStatusCommand |
| 3-6 | ✅ 修正済み。SearchMoveRoute の MovedUnitSpeed の判定が反転し、1 ずれている | Maps/Map.cs:3594-3603 | Map.bas:3800-3806 | 直進 3 マスが VB 3、C# 1。現在 MovedUnitSpeed を読む箇所がなく、表には出ない | Map/MapMovementCoverageTests.cs::SearchMoveRoute_StraightRoute_CountsStraightDistance |
| 3-7 | ✅ 修正済み。AnimationData が `main(sub)` を探さない（`==` と `!=` の取り違え） | Units/Unit.anime.cs:64 | Unit.cls:24277-24287 | `修理(修理装置)` などの戦闘アニメが使われない。Unit.se.cs:17 は正しく `!=` | Units/UnitMessageAnimeEffectTests.cs::AnimationData_SubSituation_PrefersCombinedSituation |
| 3-8 | ✅ 修正済み。PilotMessage の会話ループが 追加パイロット なしでもメインパイロットの項目で止まる（`i == pnames.Count - 1`） | Units/Unit.message.cs:458-465 | Unit.cls:22553-22579（MDList の確認は i = 4 のときだけ） | 複数乗りユニットで、メインパイロット単独をキーにした会話データが使われない | Units/UnitMessageAnimePilotMessageTests.cs::PilotMessage_MultiPilotUnit_UsesMainPilotDialogEvenIfMessageDataExists |
| 3-9 | ✅ 修正済み。ステータス表示での 分離 の表示判定でパイロット数の比較が逆 | Commands/Command.process.cs:538-540 | Command.bas:590-600 | 3 人乗りでパーツ 1+1 は VB で表示、C# で非表示。2 人乗りでパーツ 2+1 は逆 | Commands/CommandAttackProcessMenuTests.cs::CommandSelect_StatusView_Split_MorePilotsThanParts_ShowsSplit、Commands/CommandAttackProcessMenuTests.cs::CommandSelect_StatusView_Split_NotEnoughPilots_HidesSplit |
| 3-10 | ✅ 修正済み。ダミーユニットの 特殊能力一覧 の判定が逆 | Commands/Command.process.cs:494-500 | Command.bas:526-531 | FeatureList がないときに `First()` を呼んで InvalidOperationException。あるときはユニット選択に戻る | Commands/CommandAttackProcessMenuTests.cs::CommandSelect_DummyUnitWithoutFeatureList_ReturnsToUnitSelect |
| 3-11 | ✅ 修正済み。ユニット情報隠蔽時に残すコマンドと消すコマンドが逆 | Commands/Command.process.cs:643-650 | Command.bas:753-776 | 隠すべき 移動範囲・射程範囲・各一覧 だけが残り、他のコマンドが消える | Commands/CommandAttackProcessMenuTests.cs::CommandSelect_HiddenInfoEnemy_HidesListCommandsAndKeepsUnitCommands |
| 3-12 | ✅ 修正済み。スペシャルパワー 自爆 の確認で OK と Cancel の扱いが逆 | Commands/Command.unitsp.cs:348-353 | Command.bas:5177-5182 | OK で中止、Cancel で自爆する | Commands/CommandUnitOpsSpecialPowerTests.cs::Start_SelfDestruct_ConfirmCancel_DoesNotUse |
| 3-13 | ✅ 修正済み。部隊表のパイロット一覧で `SP > cost`（VB は `>=`） | Commands/Command.mapcommend.cs:232 | Command.bas:8025 | 残り SP と消費 SP が同じスペシャルパワーが表示されない | Commands/CommandUnitOpsMapCommandTests.cs::UnitList_DummyUnit_ShowsSpecialPowerWhenSPEqualsCost |
| 3-14 | ✅ 修正済み。合体パートナーの距離判定がマンハッタン距離（VB は dx・dy それぞれ 2 以内） | Extensions/UnitExtension.cs:86 | Command.bas:6522-6524 | dx=2, dy=1 のパートナーと合体できない | Commands/CommandUnitFormSplitCombineTests.cs::Combine_OnMap_PartnerDiagonalWithinTwo_IsCombined |
| 3-15 | ✅ 修正済み。インターミッションの装備解除で最後のスロットが「全て外す」扱い（`removeRet < equipList.Count`） | Intermissions/Intermission.cs:1812 | Intermission.bas:2275, 2297 | 最後のスロットを選ぶと全アイテムが外れる | Intermission/IntermissionCoverageTests.cs::RemoveLastSlotItem_RemovesOnlyThatItem |
| 3-16 | ✅ 修正済み。乗り換えの対象判定で PilotNum = -1（括弧付き 1 人乗り）を除外しない | Intermissions/Intermission.cs:1121-1125 | Intermission.bas:1101-1104 | `(1)` 指定のユニットのパイロットが乗り換え一覧に出る | Intermission/IntermissionReviewCoverageTests.cs::ExchangeUnit_ExcludesPilotOfParenthesizedSingleSeatUnit |

## 4. ループの抜け方の違い（Exit Do / Exit For と break）

VB の `Exit Do` / `Exit For` を、C# で switch の中の `break` に置き換えたため、switch しか抜けずにループが最後まで回るものが多い。逆に、内側のループだけ抜けるべきところで外側まで抜けているものもある。

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 4-1 | ✅ 修正済み。**WeaponLevel が次の属性で止まらない**（UnitWeapon と ExprInfo から報告。1 件にまとめた） | Units/UnitWeapon.cs:1147-1172 | Unit.cls:5520-5536（Case Else: Exit Do） | `体L2格`、`実L3突` のようにレベルの後に属性が続くとレベル 0（エラー表示あり）。レベル付き属性の効果と Info(…,属性レベル,…) が 0 になる | UnitWeapon/UnitWeaponCovClassTests.cs::WeaponLevel_StopsAtNextAttribute、ExprInfo/ExprInfoExtraTests.cs::Unit_Weapon_AttributeLevel_FollowedByOtherAttribute |
| 4-2 | ✅ 修正済み。AbilityLevel が数字以外で止まらない | Units/UnitAbility.cs:291-303（catch は 309-313） | Unit.cls:17918-17922 | `難L3魔` のレベルが 0 になりエラー表示 | Units/UnitAbilityCoveragePortingBugTests.cs::AbilityLevel_StopsAtNonNumericCharacter |
| 4-3 | ✅ 修正済み。Item.Class0 が専用指定の `(` を見つけた後も走査を続ける | Items/Item.cs:150-156 | Item.cls Class0（Exit Do） | `剣(改)(勇者専用)` が `剣` になる | UnitItem/UnitItemItemClassTests.cs::Class0_KeepsParenthesesBeforeSpecifier |
| 4-4 | ✅ 修正済み。Unit.Class0 も同じ | Units/Unit.props.cs:298-301 | Unit.cls:436-440 | `汎用(改)(アムロ専用)` が `汎用` になる | UnitCore/UnitCorePropsTests.cs::Class0_KeepsEarlierParenthesesBeforeExclusiveSpec |
| 4-5 | ✅ 修正済み。特殊能力行のカンマ走査が最初のカンマで止まらず、最後のカンマで分割する | Models/UnitDataList.cs LoadFeature（680-700 付近）と LoadFeatureOuter 旧形式（560-575 付近）。ItemDataList.cs:217、PilotDataList.cs:1073 も同じ処理を使う | UnitDataList.cls:371-374, 421-423 | `剣装備,シールド,超回避` などが正しく分割されない。`剣装備,` が能力として登録され、空項目のエラーも出ない | DataLists/DataListsUnitDataListTests.cs::Load_OldFormatThreeFeatures_SplitsEachFeature、DataLists/DataListsUnitDataListTests.cs::Load_NewFormatThreeFeaturesOnOneLine_SplitsEachFeature、DataLists/DataListsUnitDataListTests.cs::Load_NewFormatEmptyMiddleFeature_ReportsErrorAndKeepsOthers |
| 4-6 | ✅ 修正済み。隣接ユニットＺＯＣ無効化 で同じ陣営の隣接ユニットを読み飛ばさない | Maps/Map.cs:1466-1511 | Map.bas:2114-2127 | ZOC ユニットの隣にいる 隣接ユニットＺＯＣ無効化 持ちの敵が、自軍側の ZOC を消してしまう | Map/MapMovementCoverageTests.cs::AreaInSpeed_AdjacentZocNullification_IgnoresSameSideUnits |
| 4-7 | ✅ 修正済み。戦闘アニメ・特殊効果の `keep` が switch だけ抜ける。ループ後の判定も文字列全体と比較している | Units/Unit.se.cs:279-283（後判定 332）、Units/Unit.anime.cs:439（後判定 605, 629） | Unit.cls:24161-24163, 24611 | SpecialEffect が `keep` をメッセージとして表示し、残りの要素も再生する。PlayAnimation は `戦闘アニメ_keep発動` を Call しようとする | Units/UnitMessageAnimeReviewTests.cs::SpecialEffect_Keep_StopsProcessingRemainingParts |
| 4-8 | ✅ 修正済み。イベント読み込みの Question のブロック判定が、最初の非オプション引数で止まらない | Events/Event.data.cs:693-736 | Event.bas:966-986 | 非オプション引数が 4 つ以上の Question も常にブロックとして push する。説明書の構文はブロック形式だけなので影響は小さい | テストなし |
| 4-9 | ✅ 修正済み。Attack の合体技で、武器を持つ最初のパートナーを処理した `break` が partners のループまで抜ける | Units/Unit.attack.cs:2432-2507 | Unit.cls:10584-10637（Exit For は内側の武器ループだけ抜ける） | 2 人目以降のパートナーの弾数・EN が減らない | UnitAttack/UnitAttackAttackTests.cs::Attack_Combination_TwoPartners_ConsumesEveryPartner |
| 4-10 | ✅ 修正済み。AddItem で既存アイテムを外す処理が、入れ子の switch 内の break で foreach を抜けず、列挙中にコレクションを変更する | Units/Unit.item.cs:218, 233, 257（片手）, 282, 296（盾）, 320（両肩、break なし）, 342, 356（肩）, 404-409（ハードポイント） | Unit.cls AddItem（21488-21600 付近。DeleteItem の後に Exit For） | 装備の入れ替えで InvalidOperationException（Collection was modified） | UnitItem/UnitItemEquipTests.cs::AddItem_Shield_ReplacesShield_KeepsOneHandWeapon、::AddItem_OneHanded_Ambidextrous_KeepsOneOtherHandItem、::AddItem_OneHanded_ReplacesOneHandedWeapon_KeepsShield、::AddItem_BothShoulders_RemovesAllShoulderItems、::AddItem_Shoulder_ThirdReplacesSecond、::AddItem_Shoulder_ReplacesBothShoulders、::AddItem_Hardpoint_LargeItemFreesMultipleSlots |
| 4-11 | ✅ 修正済み。AddItem の Case Else（体・頭・独自部位）で同じ部位の既存アイテムを外さない。VB の For ループ後の i（CountFeature+1）を C# の foreach が再現していない | Units/Unit.item.cs:377-435 | Unit.cls AddItem Case Else | 革の鎧 → 兜 → 鉄の鎧 と装備すると鎧が 2 つ残る | UnitItem/UnitItemEquipTests.cs::AddItem_SamePart_ReplacesExisting |

## 5. 0 始まり・「見つからない」値の違い

VB はイベント行番号が 1 始まりで、ラベルが見つからないと 0 を返す。C# は行 ID が 0 始まりで、見つからないと -1 を返す。比較を移植しきれていない箇所がある。コレクションの添字の数え方の違いによる Skip(1) の誤りもここに入れた。

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 5-1 | ✅ 修正済み。**FindNormalLabel の「見つからない」判定が `== 0` のまま**（UnitMessageAnime と Effect から報告。1 件にまとめた） | Effect.cs:62（ShowAnimation）、Effect.cs:5571（CriticalEffect）、Units/Unit.anime.cs:511（PlayAnimation）、Models/SpecialPowerData.cs:2400。戻り値は Events/Event.label.cs:622-645 | Event.bas:2226-2257、Effect.bas:38, 4190、Unit.cls:24685-24722 | 未定義のアニメでエラーが出ない。未定義のクリティカルアニメを Call する。サブルーチンのない文字列がメッセージとして表示されない | Units/UnitMessageAnimeEffectTests.cs::PlayAnimation_TextWithoutSubroutine_DisplaysAsMessage、Units/UnitMessageAnimeEffectTests.cs::PlayAnimation_BulkPreparationWithoutSubroutine_IsSkipped、Effect/EffectAnimationTests.cs::ShowAnimation_UndefinedSubroutine_ShowsErrorMessage、Effect/EffectAnimationTests.cs::CriticalEffect_UndefinedAnimation_IsSkipped |
| 5-2 | ✅ 修正済み。SearchLabel の攻撃・接触ラベル入れ替え判定が `FindLabel(..) == 0` | Events/Event.label.cs:225 | Event.bas:1985 | 攻撃側・防御側を入れ替えたラベルに一致しない | EventsUnitList/EventsUnitListEventLoadTests.cs::SearchLabel_ReversedAttackLabel_MatchesAndSwapsSelectedUnits |
| 5-3 | ✅ 修正済み。**IsEventDefined が先頭行（ID 0）のラベルを見つけない**（CommandUnitForm、SrcPlayDataSave、EventsUnitList から報告。1 件にまとめた） | Events/Event.label.cs:419-422（`SearchLabel(lname, i + 1)`、i = 0 から）、SearchLabel の開始判定 :199 | Event.bas:2023 | 最初のイベントファイルの 1 行目にある プロローグ や 会話 ラベルが実行・選択されない | Commands/CommandUnitFormUnitCommandTests.cs::Talk_TalkEventOnFirstLineOfScenario_IsSelectable、Src/SrcPlayDataSaveStartScenarioTests.cs::GameOver_PrologueOnFirstLine_IsExecuted、EventsUnitList/EventsUnitListEventLoadTests.cs::IsEventDefined_LabelOnFirstLine_IsFound |
| 5-4 | ✅ 修正済み。**PilotsHaveSpecialPower が `Supports.Skip(1)` で最初のサポートを落とす。サブパイロットがメインのときの Pilot(1) への切り替えもない**（CommandUnitOps と UnitCore から報告。1 件にまとめた） | Units/Unit.pilot.cs:892-906 | Command.bas:4767-4774, 4799-4802 | 最初のサポートパイロットがスペシャルパワーを使えない。サブパイロットがメインのとき、同じパイロットが 2 回出て Pilot(1) が出ない | Commands/CommandUnitOpsSpecialPowerTests.cs::Start_SupportPilot_IsSelectable、UnitCore/UnitCorePilotTests.cs::PilotsHaveSpecialPower_IncludesAllSupports、UnitCore/UnitCoreMainPilotTests.cs::PilotsHaveSpecialPower_SubPilotAsMainPilot_UsesFirstPilot |
| 5-5 | ✅ 修正済み。Units.Add が 換装 の最初の形態を作らない（`fd.DataL.Skip(1)`。DataL にはデータ部分しか入っていない） | Units/Units.cs:180 | Units.cls:195-197 | 最初の換装先が作られない。換装先が 1 つのユニットはインターミッションのユニット選択に出ない | UnitCore/UnitCoreUnitsTests.cs::Add_ExchangeFeature_CreatesAllListedForms、UnitCore/UnitCoreUnitsExtraTests.cs::Add_ExchangeFirstTargetUndefined_ReturnsNull、UnitCore/UnitCoreIntermissionExchangeTests.cs::ExchangeFormCommand_ListsAllExchangeForms、UnitCore/UnitCoreIntermissionExchangeTests.cs::ExchangeFormCommand_SingleExchangeForm_UnitIsListed |
| 5-6 | ✅ 修正済み。Units.Add が 他形態 の最初の形態を作らない（同じ Skip(1)） | Units/Units.cs:197 | Units.cls:209-210 | 最初の他形態が作られない | UnitCore/UnitCoreUnitsTests.cs::Add_OtherFormFeature_CreatesAllListedForms |

Units/Units.cs には 74, 90, 126, 161, 243, 252 行目にも `fd.DataL.Skip(1)` がある。今回テストしていないが、同じ誤りでないか確認したほうがよい。

## 6. 特殊能力データではなく名前の文字列を読んでいる

VB の `LIndex(.FeatureData("X"), n)` を `LIndex("X", n)` と移植しており、常に空文字列か名前そのものになる。

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 6-1 | ✅ 修正済み。MaxCounterAttack の 先手必勝 の必要気力 | Units/Unit.lookup.cs:236 | Unit.cls:29915 | 指定した気力が読まれない（常に ""） | Units/UnitLookupSkillTests.cs::MaxCounterAttack_SenteHisshou_UsesSpecifiedMorale |
| 6-2 | ✅ 修正済み。COM のテレポートの消費 EN | COM.cs:2139 | COM.bas:1755 | AI のテレポートで EN が減らない（期待 180、実際 200） | COM/COMOperateUnitMoveTests.cs::OperateUnit_Teleport_ConsumesENFromFeatureData |
| 6-3 | ✅ 修正済み。COM のジャンプの消費 EN | COM.cs:2157 | COM.bas:1771 | AI のジャンプで EN が減らない（期待 170、実際 200） | COM/COMOperateUnitMoveTests.cs::OperateUnit_Jump_ConsumesENFromFeatureData |
| 6-4 | ✅ 修正済み。COM TryBattleTransform の ノーマルモード 判定 | COM.cs:2745 | COM.bas:2239 | ノーマルモードに指定された形態が候補から外れない | COM/COMTryBattleTransformTests.cs::TryBattleTransform_FormDesignatedAsNormalMode_IsIgnored |

## 7. DEFAULT_LEVEL（-1000）の扱い

C# ではレベル未指定を `Constants.DEFAULT_LEVEL`（-1000）で表すが、VB では 0 や 1 として扱っていた箇所にそのまま使っている。

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 7-1 | ✅ 修正済み。レベルのないスペシャルパワー効果の dblEffectLevel が -1000 | Models/SpecialPowerData.cs:66 | SpecialPowerData.cls:61-62, 115-159（0 のまま） | Info の効果レベルが -1000。レベルなしの ＨＰ回復 が RecoverHP(-10000) になり、ＥＮ回復・行動数増加 も負のレベルになる | SpecialPower/SpecialPowerDataListLoadTests.cs::SetEffect_EffectWithoutLevel_HasLevelZero |
| 7-2 | ✅ 修正済み。UpdateSupportMod の cmd_level の初期値 | Pilots/Pilot.status.cs:557 | Pilot.cls:841（Double の既定値 0） | 範囲内に指揮官がいないザコパイロットの命中・回避に 5×(-1000) が加わる | Pilots/PilotSkillStatusSupportModTests.cs::UpdateSupportMod_指揮官が範囲外ならザコの命中回避は変化しない |
| 7-3 | ✅ 修正済み。FeatureHelpMessage がレベルなしの能力で fd.Level（-1000）を使う（fd.FeatureLevel を使うべき） | Help.cs:1403 | Help.bas:854（AllFeatureLevel は 1 を返す） | バリア・修理装置・テレポートなどの解説の数値がおかしい | Help/HelpFeatureHelpMessageTests.cs::Barrier_WithoutLevel_TreatsLevelAsOne、Help/HelpFeatureHelpMessageTests.cs::RepairDevice_WithoutLevel_ThirtyPercent、Help/HelpFeatureHelpMessageTests.cs::Teleport_WithoutLevel_SpeedPlusOne |

## 8. 例外で止まる

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 8-1 | ✅ 修正済み。合体技のパートナー（同名武器なし）が自爆したときのパーツ分離で LIndex の 1 番目（分離名）を形態名として使う | Units/Unit.attack.cs:2558-2559 | Unit.cls（`If j > .CountWeapon` 分岐）`LIndex(.FeatureData("パーツ分離"), 2)` | OtherForm が null を返し NullReferenceException | UnitAttack/UnitAttackR2AttackTests.cs::Attack_Combination_PartnerWithoutWeapon_SelfDestructSeparatesParts |
| 8-2 | ✅ 修正済み。スペシャルパワー 復活 で ノーマルモード のデータ全体を Transform に渡す | Models/SpecialPowerData.cs:2103 | SpecialPowerData.cls:1619 | `ノーマルモード=X 3 回数制限` のようにオプションがあると NullReferenceException（Units/Unit.map.cs:912-913） | SpecialPower/SpecialPowerReviveTests.cs::Revive_HyperModeUnit_ReturnsToNormalFormEvenWithOptions |
| 8-3 | ✅ 修正済み。換装コマンド（マップ・ステータス表示）で武器のない形態に Enumerable.Max | Commands/Command.unitform.cs:785, 788 | Command.bas:6652, 6667 | InvalidOperationException | Commands/CommandUnitFormSplitCombineTests.cs::ExchangeForm_FormsWithoutWeapons_ShowsZeroPowerAndRange |
| 8-4 | ✅ 修正済み。インターミッションの換装で同じ問題 | Intermissions/Intermission.cs:2153, 2156 | Intermission.bas:2994-3015 | InvalidOperationException | Intermission/IntermissionCoverageTests.cs::FormWithoutWeapons_ShowsZeroPowerAndRange |
| 8-5 | ✅ 修正済み。FeatureHelpMessage(u, "武器・防具クラス") で AllFeature が null | Help.cs:1375-1377（参照外しは 1400）。Expressions/Functions/Info.cs:1371 から呼ばれる | Help.bas:846-848, 3101-3112 | NullReferenceException。期待値は `武器【剣】;防具【-】` | Help/HelpReviewFollowupTests.cs::FeatureHelpMessage_WeaponArmorClass_ByString |
| 8-6 | ✅ 修正済み。レベル付き・データなしの未知の能力の解説で、表示名（例 `謎能力Lv2`）で AllFeature を引く | Help.cs:5118 | Help.bas:3262（エラー時は "" を返す） | NullReferenceException。VB は空の解説を返す | Help/HelpFeatureHelpMessageTests.cs::UnknownFeature_WithLevelWithoutData_ReturnsEmpty |

ほかに例外になるもの: 3-10（ダミーユニットの特殊能力一覧）、4-10（AddItem）、10-15（MakePilotList の未知のソートキー）。

## 9. その他: 戦闘・移動・アビリティの処理

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 9-1 | ✅ 修正済み。斉射（斉）を使うと他の武器の弾数を比率で上書きする（VB は現在値との小さい方） | Units/UnitWeapon.cs:4996-5001 | Unit.cls:17539 | 2/10 のミサイルが 6/10 に回復する | UnitWeapon/UnitWeaponCovUseTests.cs::UseWeapon_FullBurst_DoesNotRefillOtherWeapons |
| 9-2 | ✅ 修正済み。IsAbilityAvailable の召喚ユニットのデータ確認が、召喚済みユニットのループの中にある | Units/UnitAbility.cs:710-773 | Unit.cls:18256-18278 | 召喚ユニットが未定義でも、召喚済みがいなければ使用可能になる | Units/UnitAbilityCoveragePortingBugTests.cs::Available_SummonWithUndefinedUnit_IsFalse |
| 9-3 | ✅ 修正済み。装填=武器名 が対象ではなく使用者の武器を回復する（ラムダの引数を使わず this.Weapons を参照） | Units/Unit.ability.cs:878 | Unit.cls:19597-19620 | 対象の弾数が回復しない | Units/UnitAbilityCoveragePortingBugTests.cs::Reload_SpecificWeapon_RefillsTargetWeapon |
| 9-4 | ✅ 修正済み。**AreaWithUnit がどのマスのマスクも外さない**（Map と CommandUnitForm から報告。1 件にまとめた） | Maps/Map.cs:679-683 | Map.bas:1090-1140 | どのマスも選べない。命令 の 攻撃・護衛 の対象を選べない | Map/MapAreaShapeCoverageTests.cs::AreaWithUnit_SelectsOnlyCellsWithUnitsOfParty、Commands/CommandUnitFormUnitCommandTests.cs::Order_Attack_OnlyEnemiesSelectableAndSetsModeToTargetPilot、Commands/CommandUnitFormUnitCommandTests.cs::Order_Escort_OnlyAlliesSelectableExceptSelf |
| 9-5 | ✅ 修正済み。進入不可 / 移動制限 を MaskData に OR しているが、AreaInSpeed が後で上書きする | Maps/Map.cs:2554-2568（上書きは 1744-1773） | Map.bas:1963-2011（ジャンプ以外は move_cost = 1000000） | 進入不可の地形に入れる。移動制限で許されていない地形も通れる | Map/MapMovementCoverageTests.cs::AreaInSpeed_ProhibitedTerrain_CannotBeEntered、Map/MapReviewFollowupCoverageTests.cs::AreaInSpeed_AllowedTerrains_CannotEnterOtherTerrain |
| 9-6 | ✅ 修正済み。LookForSupportAttack の隣接ユニットの順序が VB と違う（C# は右・左・下・上、VB は左・右・上・下） | Units/Unit.lookup.cs:272、Maps/Map.cs:164-169 | Unit.cls:29949-29967 | 同条件のとき援護攻撃するユニットが変わる | Units/UnitLookupSupportTests.cs::LookForSupportAttack_WithoutTarget_PrefersLeftNeighbourLikeVb |
| 9-7 | ✅ 修正済み。COM TryBattleTransform が最後の武器ではなく最強の武器の攻撃力で比べる | COM.cs:2811, 2815 | COM.bas:2282-2287 | AI の変形先の選び方が変わる | COM/COMTryBattleTransformTests.cs::TryBattleTransform_SameMobility_ComparesLastWeaponPower |
| 9-8 | ✅ 修正済み。MapAbilityCommand で使用後イベントが Cancel したとき行動終了しない | Commands/Command.unitability.cs:672-678 | Command.bas:4659-4664 | ユニットがもう一度行動できる | Commands/CommandUnitOpsAbilityTests.cs::MapAbility_CanceledByAfterUseEvent_EndsAction |
| 9-9 | ✅ 修正済み。DeleteItem で 両手利き を失ったとき 2 本目の片手武器を外さない（Update の後に判定している） | Units/Unit.item.cs:475, 486 | Unit.cls:21782, 21785 | 両手に片手武器を持ったまま | UnitItem/UnitItemEquipTests.cs::DeleteItem_LosingAmbidexterity_RemovesSecondOneHandedWeapon |
| 9-10 | ✅ 修正済み。DeleteItem が独自部位のアイテムや装備個所の外のアイテムも外す | Units/Unit.item.cs:488-497、Items/ItemSlot.cs:140-203 | Unit.cls:21787-21900 | 関係ないアイテムを外すとバックパックなども外れる | UnitItem/UnitItemEquipTests.cs::DeleteItem_KeepsCustomPartItem |

## 10. その他: コマンド・一覧・メッセージ表示

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 10-1 | ✅ 修正済み。Info(マップ,時間帯) のフィルタ時に透過率が入らない | Expressions/Functions/Info.cs:2485-2488 | Expression.bas:5987-5993 | `#123456 50% マップ限定` が `#123456 マップ限定` になる | ExprInfo/ExprInfoMapOptionTests.cs::Map_TimeOfDay_Filter_IncludesTransparency |
| 10-2 | ✅ 修正済み。Combine で合体メッセージがなくてもメッセージウィンドウを開く | Units/Unit.map.cs:1397-1430 | Unit.cls:26006-26037 | 空のメッセージウィンドウが開閉する | Units/UnitMapActionFormTests.cs::Combine_NoCombineMessage_DoesNotOpenMessageForm |
| 10-3 | ✅ 修正済み。SysMessage / IsSysMessageDefined が戦闘アニメデータ（ADList）ではなく MDList を探す | Units/Unit.message.cs:1053-1054 | Unit.cls:23171-23318 | 戦闘アニメデータの (解説) が使われない | Units/UnitMessageAnimeSysMessageTests.cs::SysMessage_BattleAnimationDataIsSearched |
| 10-4 | ✅ 修正済み。PilotMessage が抽選されたサブパイロットではなく 1 人目のメッセージデータを使う | Units/Unit.message.cs:568-571 | Unit.cls:22663-22668 | 2 人目だけにデータがあると何も表示されない | Units/UnitMessageAnimePilotMessageTests.cs::PilotMessage_MultiPilotUnit_UsesMessageDataOfChosenPilot |
| 10-5 | ✅ 修正済み。SpecialEffect に BattleAnimation 有効時の自動選択（swing.wav、(命中)、デフォルト攻撃 / ダメージ命中）がない | Units/Unit.se.cs（該当処理なし） | Unit.cls:23948-24030, 24189-24197 | 戦闘アニメ有効時に既定のアニメが出ない | テストなし |
| 10-6 | ✅ 修正済み。IsBeamWeapon の cname が値渡し（VB は ByRef） | Effect.cs:5832（呼び出し 261, 1953, 3538, 4249, 5544） | Effect.bas:4384, 4167 | 熱線のレッド、プラズマのオレンジなどビームの色が付かない | Effect/EffectAnimationTests.cs::Attack_BeamColorFromIsBeamWeapon、Effect/EffectAnimationTests.cs::AbsorbEffect_ColorFallsBackToBeamColor |
| 10-7 | ✅ 修正済み。**SkillName0 が所有能力の別名を返さない**（PilotSkillStatus と Help から報告。1 件にまとめた） | Pilots/Pilot.skill.cs:953-967 | Pilot.cls:2138-2146 | 別名ではなく元の名前（または ""）が返る。別名で定義した 解説 が使われない | Pilots/PilotSkillStatusSkillTests.cs::SkillName0_所有能力の別名を返す、Help/HelpSkillHelpMessageTests.cs::Explanation_DefinedByAliasSkill_OverridesText |
| 10-8 | ✅ 修正済み。SkillName が所有能力ですぐ return し、VB の後処理（強化の表示、階級の Lv 除去、括弧の除去）を飛ばす | Pilots/Pilot.skill.cs SkillName(string) | Pilot.cls:1842-1885, 2083-2105 | 能力名の表示が違う | Pilots/PilotSkillStatusSkillTests.cs::SkillName_所有能力でも強化分が表示される、Pilots/PilotSkillStatusSkillTests.cs::SkillName_所有する階級はレベル表示を除く、Pilots/PilotSkillStatusSkillTests.cs::SkillName_所有能力のレベル非表示用括弧は取り除かれる |
| 10-9 | ✅ 修正済み。Pilot.Nickname が パイロット愛称 をステータス表示用ダミーユニットでしか適用しない | Pilots/Pilot.props.cs:58-98 | Pilot.cls:150-180 | 愛称が変わらない | Pilots/PilotSkillStatusPropsTests.cs::Nickname_パイロット愛称でメインパイロットの愛称が変わる |
| 10-10 | ✅ 修正済み。Pilot.KanaName がダミーユニットで 搭乗ユニット[ID] 未設定のとき null | Pilots/Pilot.props.cs:166-169 | Pilot.cls:234-236 | 読み仮名が null（並べ替えなどに影響） | Pilots/PilotSkillStatusPropsTests.cs::KanaName_ダミーユニットで搭乗ユニット未設定ならデータの読み仮名 |
| 10-11 | ✅ 修正済み。Unit.Nickname が 等身大基準 で null を返す | Units/Unit.props.cs:83-84 | Unit.cls:308-321 | メインパイロットの愛称ではなく null | UnitCore/UnitCorePropsTests.cs::Nickname_HeroScale_UsesMainPilotNickname |
| 10-12 | ✅ 修正済み。移動前メニューの 分離 で、必要パイロット数を分離先のユニット数で数える | Commands/Command.process.cs:855-858 | Command.bas:1190-1203 | パイロットが足りなくても 分離 が出る | Commands/CommandAttackProcessMenuTests.cs::CommandSelect_Split_NotEnoughPilots_HidesSplit |
| 10-13 | ✅ 修正済み。行動終了後のユニットコマンドで `-*` / `**` ラベルに絞り込まない | Commands/Command.process.cs:691（AddUserUnitCommand 1130-1157） | Command.bas:799-801 | 行動後に使えないはずのユニットコマンドが出る | Commands/CommandAttackProcessMenuTests.cs::CommandSelect_ActedAlly_OnlyAfterActionUnitCommandsShown |
| 10-14 | ✅ 修正済み。移動後のユニットコマンドで `*` ラベルに絞り込まない | Commands/Command.process.cs:1250 | Command.bas:1785-1787 | 移動後に使えないはずのユニットコマンドが出る | Commands/CommandAttackProcessMenuTests.cs::AfterMoveCommandSelect_OnlyAfterMoveUnitCommandsShown |
| 10-15 | ✅ 修正済み。MakePilotList が未知のソートキーでエラーにする | CmdDatas/Commands/Other/MakePilotListCmd.cs:79-80 | CmdData.cls:7708-7744 | VB は登録順で一覧を出すが、C# はコマンドエラーで止まる | EventsUnitList/EventsUnitListMakeListTests.cs::MakePilotList_UnknownSortKey_ListsInRegistrationOrderWithoutStatusLine |
| 10-16 | ✅ 修正済み。部隊表の ＨＰ / ＥＮ 順の並べ替えが最大値を使う | Commands/Command.mapcommend.cs:263-264 | Command.bas:8057-8064 | 現在値の順に並ばない | Commands/CommandUnitOpsMapCommandTests.cs::UnitList_SortByHP_UsesCurrentHP |
| 10-17 | ✅ 修正済み。部隊表の並べ替えメニューに パイロット名称で並べ替え がなく ユニット名称で並べ替え が 2 つある | Commands/Command.mapcommend.cs:361-362 | Command.bas:8201-8202 | パイロット名で並べ替えられない | Commands/CommandUnitOpsMapCommandTests.cs::UnitList_SortMenu_HasPilotNameSort |
| 10-18 | ✅ 修正済み。等身大基準の並べ替えメニューの ＥＮで並べ替え が スペシャルパワーで並べ替え になっている | Commands/Command.mapcommend.cs:352 | Command.bas:8195 | 選んでも並ばない | Commands/CommandUnitOpsMapCommandTests.cs::UnitList_PersonScale_SortMenu_HasENSort |
| 10-19 | ✅ 修正済み。部隊表のパイロット一覧で SP 欄の後の空白 2 桁がない | Commands/Command.mapcommend.cs:229-233 | Command.bas:8021 | 表示の桁ずれ | Commands/CommandUnitOpsMapCommandTests.cs::UnitList_DummyUnit_SeparatesSPAndSpecialPowersWithTwoSpaces |
| 10-20 | ✅ 修正済み。スペシャルパワー一覧で消費 SP と解説の間の空白が 1 桁 | Commands/Command.unitsp.cs:112 | Command.bas:4899-4901 | 表示の桁ずれ | Commands/CommandUnitOpsSpecialPowerTests.cs::Start_SpecialPowerList_SeparatesCommentWithTwoSpaces |
| 10-21 | ✅ 修正済み。ステータス表示の 換装 の一覧が選択後も閉じない（`連続表示`） | Commands/Command.unitform.cs:812 | Command.bas:6697-6702 | 換装後も一覧ウィンドウが残る。インターミッションのユニット一覧の不具合報告に関係する可能性がある | Commands/CommandUnitFormSplitCombineTests.cs::ExchangeForm_ListBoxIsClosedAfterSelection |
| 10-22 | ✅ 修正済み。ステータス表示で 2 体合体が出ない | Extensions/UnitExtension.cs:64, 86 | Command.bas:6501-6503, 6522 | インターミッションで 合体 を選ぶと空の一覧が出て合体できない | Commands/CommandUnitFormSplitCombineTests.cs::Combine_StatusView_TwoUnitCombine_IsCombined |
| 10-23 | ✅ 修正済み。合体後の形態の一覧に合体先の形態名ではなく合体名を出す | Commands/Command.unitform.cs:687 | Command.bas:6534 | 一覧の表示名が違う | Commands/CommandUnitFormSplitCombineTests.cs::Combine_OnMap_MultiplePatterns_ListShowsCombinedFormNames |
| 10-24 | ✅ 修正済み。換装の一覧の行に地形適応の列がない | Commands/Command.unitform.cs:778-788 | Command.bas:6644-6649 | 表示の欠け | Commands/CommandUnitFormSplitCombineTests.cs::ExchangeForm_ListItemContainsAdaption |
| 10-25 | ✅ 修正済み。武器一覧の属性ダイアログの表題が アビリティ属性一覧 | Commands/Command.preview.cs:423 | Command.bas:7365 | 表題の誤り | Commands/CommandUnitFormPreviewTests.cs::WeaponList_AttributeListCaptionIsWeaponAttributeList |
| 10-26 | ✅ 修正済み。武器一覧をキャンセルしても SelectedWeapon が 0 に戻らない | Commands/Command.preview.cs:334-347 | Command.bas:7290-7291 | 前に選んだ武器番号が残る | Commands/CommandUnitFormPreviewTests.cs::WeaponList_Cancel_ResetsSelectedWeapon |
| 10-27 | ✅ 修正済み。ステータス表示の 変形 一覧の表題が 変形先 / 変身先 | Commands/Command.unitform.cs:48 | Command.bas:5823 | 表題の違い（軽微） | Commands/CommandUnitFormTransformTests.cs::Transform_StatusView_ListCaptionIsTransform |
| 10-28 | ✅ 修正済み。インターミッションの乗り換え先ユニットの並べ替えが sort_mode2 ではなく sort_mode を見る | Intermissions/Intermission.cs:1329, 1336, 1354 | Intermission.bas:1477-1501, 1540-1545 | ユニットの読み仮名順に並ばない | Intermission/IntermissionCoverageTests.cs::DestinationList_DefaultSortIsUnitKanaName |
| 10-29 | ✅ 修正済み。インターミッションのコマンド一覧の表題に総ターン数・資金が出ない（作った文字列を使っていない） | Intermissions/Intermission.cs:168-178 | Intermission.bas:177-179 | 総ターン数と資金が表示されない | Intermission/IntermissionReviewCoverageTests.cs::Menu_CaptionShowsTotalTurnsAndMoney |

## 11. その他: データ読み込み・イベント・セーブ

| # | 概要 | C# の箇所 | VB の箇所 | 影響 | テスト |
|---|---|---|---|---|---|
| 11-1 | ✅ 修正済み。ClearMap が MapFileName を空にしない | Maps/Map.cs:405-418 | Map.bas:589 | ClearMap 後も IsStatusView が false のまま | Map/MapReviewFollowupCoverageTests.cs::ClearMap_ResetsMapFileName |
| 11-2 | ✅ 修正済み。SetVariable がサブルーチンローカル変数を大文字小文字を無視して探し、名前を書き換える | Events/Event.variable.cs:28（Variable.cs:228-230, 478-483 から到達） | Expression.bas:7214, 7484, 6265, 7761, 7792 | `Set ABC` がサブローカルの `abc` を上書きし名前を ABC に変える。VB はローカル変数 ABC を新しく作る | Variable/VariableSetUndefineCoverageTests.cs::SetVariable_SubLocalNameWithDifferentCase_DoesNotOverwriteSubLocal |
| 11-3 | ✅ 修正済み。Information.IsNumeric が途中の空白を取り除いてから判定する | VB/Information.cs:15（届かない回復処理は Models/UnitDataList.cs:789-795） | UnitDataList.cls:585-595 付近 | `ビーム,1400 1,5,...` のようにカンマが抜けた武器データでエラーが出ず、値の回復もされない | DataLists/DataListsUnitDataListTests.cs::Load_WeaponMissingComma_RecoversFromSpaceSeparatedValue |
| 11-4 | ✅ 修正済み。武器の消費ＥＮのエラー処理に `If LLength(buf2) > 1` がない | Models/UnitDataList.cs:921-925 | UnitDataList.cls:696-701 | 以降の項目が 1 つずれ、必要気力・地形適応・クリティカル率のエラーが続けて出る。アイテム・パイロットの武器も同じ | DataLists/DataListsUnitDataListTests.cs::Load_WeaponInvalidENConsumption_ReportsErrorWithoutShiftingFollowingFields |
| 11-5 | ✅ 修正済み。パイロットの `===` 特殊能力節で数値だけの項目があると、エラーを出さずに節を終える | Models/PilotDataList.cs:1073、Models/UnitDataList.cs:719-722 | PilotDataList.cls:885-888 | そのパイロットの残りのデータが読まれない | DataLists/DataListsPilotDataListTests.cs::Load_ExtendedFeatureSection_NumericItemReportsError |
| 11-6 | ✅ 修正済み。GameOver が ScenarioFileName に null を入れる（GameOver.eve のパスを入れるべき） | SRC.play.cs:775, 803, 806 | SRC.bas:1763 | シナリオ名が null。エラーメッセージのファイル名も空 | Src/SrcPlayDataSaveStartScenarioTests.cs::GameOver_SetsScenarioFileNameToGameOverEve |
| 11-7 | ✅ 修正済み。IncludeData のデータフォルダ読み込みで mind.txt を sp.txt より先に見る | SRC.data.cs:133-140 | SRC.bas:2519-2523 | 両方あるときに読むファイルが違う（LoadLocalData は正しい順） | Src/SrcPlayDataSaveDataFolderTests.cs::IncludeData_SpTxtTakesPrecedenceOverMindTxt |
| 11-8 | ✅ 修正済み。RestoreData のウィンドウ表題が、保存されたシナリオではなく直前のシナリオ名になる | SRC.save.cs:258（置き換えは 268） | SRC.bas RestoreData | 表題の誤り | Src/SrcPlayDataSaveSuspendTests.cs::RestoreData_DifferentScenario_SetsTitleFromSavedScenario |
| 11-9 | ✅ 修正済み。RestoreData で保存したマップの表示位置（MapX/MapY）が失われる | SRC.save.cs:271-272（Map.Restore は 300。SetMapSize がリセット: Maps/Map.cs:58, 399-400） | Map.bas RestoreMapData | 再開時に画面中央が表示される | Src/SrcPlayDataSaveSuspendTests.cs::RestoreData_RestoresMapDisplayPosition |
| 11-10 | ✅ 修正済み。ステータス表示ライブラリの判定がパス区切り `\` 固定 | SRC.play.cs:148-149 | SRC.bas StartScenario | Linux / Web でステータス表示用ファイルにも _リスタート.srcq を書き出す | Src/SrcPlayDataSaveStartScenarioTests.cs::StartScenario_StatusViewLibrary_DoesNotDumpRestartData |
| 11-11 | ✅ 修正済み。Organize の並べ替えキーが `Level + Exp`（VB は `500 * Level + Exp`） | CmdDatas/Commands/Unit/OrganizeCmd.cs:213-219 | CmdData.cls:8656 | Lv4・Exp400 が Lv5・Exp0 より上に並ぶ | EventsUnitList/EventsUnitListOrganizeCmdTests.cs::Organize_SortKeyWeighsLevelOverExperience |
| 11-12 | ✅ 修正済み。イベントの構文チェックで制御スタックが空のとき照合しない | Events/Event.data.cs:647 ほか cmdStack.Any() で囲まれた ElseIf / Case / Next / End / Suspend の判定 | Event.bas:687, 736-745, 793-808, 841-849, 925-931, 991-997, 1068-1091 | トップレベルの End のない Talk、対応のない ElseIf / Next / End がエラーにならない | EventsUnitList/EventsUnitListEventLoadTests.cs::LoadEventData_TopLevelTalkWithoutEnd_IsError、EventsUnitList/EventsUnitListEventLoadTests.cs::LoadEventData_TopLevelEndIfLikeCommandsWithoutOpener_AreErrors |
| 11-13 | ✅ 修正済み。ラベル末尾が全角コロンのときエラーを出すが終了しない（error_found を立てない） | Events/Event.data.cs:335-338 | Event.bas:664-666, 1093-1096 | TerminateSRC が呼ばれず続行する | EventsUnitList/EventsUnitListEventLoadTests.cs::LoadEventData_LabelEndingWithFullWidthColon_Terminates |
| 11-14 | ✅ 修正済み。MakePilotList の 名称 モード以外でも追加サポートを除外する | CmdDatas/Commands/Other/MakePilotListCmd.cs:52 | CmdData.cls:7763-7768 | 一覧から追加サポートが漏れる（軽微） | テストなし |
| 11-15 | ✅ 修正済み。未装備の呪いのアイテムが装備候補から外れる（IsFix に 呪い が含まれる） | Intermissions/Intermission.cs:1850、Items/Item.cs:60-62 | Intermission.bas:2505-2523, 2652-2654 | 呪いのアイテムを装備できず、「呪われていた！」も出ない | Intermission/IntermissionCoverageTests.cs::CursedItem_ShowsCurseMessageWhenEquipped |

---

## まとめた重複の一覧

| まとめた項目 | 報告元の対象領域 |
|---|---|
| 2-1 Unit.Update の耐性などの文字列 | UnitAttackCheck、UnitWeapon、UnitAbility、UnitStatusRecover |
| 3-4 Pilot.SkillType の条件反転 | UnitLookup、PilotSkillStatus |
| 4-1 WeaponLevel が止まらない | UnitWeapon、ExprInfo |
| 5-1 FindNormalLabel の `== 0` | UnitMessageAnime、Effect |
| 5-3 IsEventDefined の先頭行 | CommandUnitForm、SrcPlayDataSave、EventsUnitList |
| 5-4 PilotsHaveSpecialPower の Skip(1) | CommandUnitOps、UnitCore |
| 9-4 AreaWithUnit | Map、CommandUnitForm |
| 10-7 SkillName0 の別名 | PilotSkillStatus、Help |
