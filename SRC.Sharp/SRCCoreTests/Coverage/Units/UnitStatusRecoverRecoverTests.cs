using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit の回復処理（元の SRC の Unit.FullRecover / FullSupply / RecoverHP / RecoverEN / Rest / Reset / SyncBullet）
    /// と修理可否（CanFix）
    /// </summary>
    [TestClass]
    public class UnitStatusRecoverRecoverTests
    {
        private List<string> sysMessages;

        // ユニットの退却（Escape）でステータス表示を参照するため
        private class NullGUIStatus : IGUIStatus
        {
            public Unit DisplayedUnit { get; set; }
            public Pilots.Pilot DisplayedPilot { get; set; }
            public void DisplayGlobalStatus() { }
            public void DisplayUnitStatus(Unit u, Pilots.Pilot p = null) { }
            public void DisplayPilotStatus(Pilots.Pilot p) { }
            public void InstantUnitStatusDisplay(int X, int Y) { }
            public void ClearUnitStatus() { }
        }

        private SRC CreateSrc()
        {
            sysMessages = new List<string>();
            var gui = new MockGUI
            {
                CenterHandler = (x, y) => { },
                RedrawScreenHandler = (late) => { },
                RefreshScreenHandler = (a, b) => { },
                EraseUnitBitmapHandler = (x, y, r) => { },
                PaintUnitBitmapHandler = (u, m) => { },
                OpenMessageFormHandler = (u1, u2) => { },
                CloseMessageFormHandler = () => { },
                DisplaySysMessageHandler = (m, b) => sysMessages.Add(m),
                UnlockGUIHandler = () => { },
                LockGUIHandler = () => { },
                DisplayTelopHandler = (m) => { },
                SleepHandler = (ms, e) => { },
                ResetDrawStringHandler = () => { },
                UpdateScreenHandler = () => { },
                MainFormShowHandler = () => { },
            };
            var src = new SRC { GUI = gui, GUIStatus = new NullGUIStatus() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            src.Titles = new List<string>();
            // 味方ステージの1ターン目は回復しないため2ターン目とする
            src.Stage = "味方";
            src.Turn = 2;
            return src;
        }

        private static UnitData AddUnitData(SRC src, string name = "テスト機体")
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            return ud;
        }

        private static Unit CreateUnit(SRC src, UnitData ud, int pilotLevel = 1, int pilotSP = 0, params string[] skills)
        {
            var pd = src.PDList.Add("テストパイロット");
            pd.SP = pilotSP;
            pd.Adaption = "AAAA";
            foreach (var s in skills)
            {
                pd.AddSkill(s, 1d, "", 1);
            }
            var unit = src.UList.Add(ud.Name, 0, "味方");
            var pilot = src.PList.Add("テストパイロット", pilotLevel, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        private static TerrainData CreateMap(SRC src)
        {
            var td = new TerrainData
            {
                ID = 1,
                Name = "平地",
                Class = "陸",
                MoveCost = 2,
            };
            var map = src.Map;
            map.MapFileName = "test.map";
            map.MapWidth = 3;
            map.MapHeight = 3;
            map.MapData = new Src2DArray<MapCell>(3, 3);
            foreach (var x in map.MapData.XRange)
            {
                foreach (var y in map.MapData.YRange)
                {
                    map.MapData[x, y] = new MapCell
                    {
                        X = x,
                        Y = y,
                        UnderTerrain = td,
                        BoxType = BoxTypes.Under,
                    };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(3, 3);
            map.MaskData = new Src2DArray<bool>(3, 3);
            return td;
        }

        private static WeaponData AddWeapon(UnitData ud, string name, string wclass, int bullet)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = 1000;
            wd.Class = wclass;
            wd.MinRange = 1;
            wd.MaxRange = 3;
            wd.Bullet = bullet;
            return wd;
        }

        private static AbilityData AddAbility(UnitData ud, string name, string aclass, int stock)
        {
            var ad = ud.AddAbility(name);
            ad.Class = aclass;
            ad.Stock = stock;
            return ad;
        }

        private static UnitWeapon Weapon(Unit unit, string name) => unit.Weapons.First(w => w.Name == name);
        private static UnitAbility Ability(Unit unit, string name) => unit.Abilities.First(a => a.Data.Name == name);

        // ──────────────────────────────────────────────
        // CanFix
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CanFix_RejectsFixerNamedInRepairBlock()
        {
            // 修理不可で指定された修理装置では修理できない（修理装置指定なしなら CanHPRecovery に従う）
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("修理不可=修理不可 修理装置");
            var unit = CreateUnit(src, ud);
            var fixerData = AddUnitData(src, "修理機");
            fixerData.AddFeature("修理装置");
            var fixer = src.UList.Add("修理機", 0, "味方");
            fixer.Update();
            unit.HP = 500;

            Assert.IsFalse(unit.CanFix(fixer));
            Assert.IsTrue(unit.CanFix(null));
        }

        [TestMethod]
        public void CanFix_NegatedRepairBlock_AllowsOnlyNamedFixer()
        {
            // 「!名称」指定は指定された修理装置以外では修理できない
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("修理不可=修理不可 !修理装置");
            var unit = CreateUnit(src, ud);
            var fixerData = AddUnitData(src, "修理機");
            fixerData.AddFeature("修理装置");
            var fixer = src.UList.Add("修理機", 0, "味方");
            fixer.Update();
            var otherData = AddUnitData(src, "応急修理機");
            otherData.AddFeature("修理装置=応急修理");
            var other = src.UList.Add("応急修理機", 0, "味方");
            other.Update();
            unit.HP = 500;

            Assert.IsTrue(unit.CanFix(fixer));
            Assert.IsFalse(unit.CanFix(other));
        }

        [TestMethod]
        public void CanFix_FullHP_IsFalse()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));

            Assert.IsFalse(unit.CanFix(null));
        }

        // ──────────────────────────────────────────────
        // FullRecover / FullSupply / RecoverHP / RecoverEN / Reset
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FullRecover_RestoresStatusAndKeepsGrantedConditions()
        {
            // 全回復はＨＰ・ＥＮ・弾数を最大にし、ステータス異常のみ消去する（付加・強化・ＵＰ・残り時間・非操作は残す）
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイル", "射", 6);
            AddAbility(ud, "修理", "", 2);
            var unit = CreateUnit(src, ud);
            unit.HP = 10;
            unit.EN = 3;
            Weapon(unit, "ミサイル").SetBullet(1);
            Ability(unit, "修理").SetStock(0);
            unit.AddCondition("毒", 3);
            unit.AddCondition("麻痺", 2);
            unit.AddCondition("残り時間", 3);
            unit.AddCondition("非操作", -1);
            unit.AddCondition("Ｖ－ＵＰ", -1, 0d, "全");
            unit.AddCondition("運動性強化", 2);
            unit.UsedSupportAttack = 2;
            unit.UsedSupportGuard = 1;
            unit.UsedSyncAttack = 1;
            unit.UsedCounterAttack = 1;
            unit.Mode = "固定";

            unit.FullRecover();

            Assert.AreEqual(unit.MaxHP, unit.HP);
            Assert.AreEqual(unit.MaxEN, unit.EN);
            Assert.AreEqual(6, Weapon(unit, "ミサイル").Bullet());
            Assert.AreEqual(2, Ability(unit, "修理").Stock());
            Assert.IsFalse(unit.IsConditionSatisfied("毒"));
            Assert.IsFalse(unit.IsConditionSatisfied("麻痺"));
            Assert.IsTrue(unit.IsConditionSatisfied("残り時間"));
            Assert.IsTrue(unit.IsConditionSatisfied("非操作"));
            Assert.IsTrue(unit.IsConditionSatisfied("Ｖ－ＵＰ"));
            Assert.IsTrue(unit.IsConditionSatisfied("運動性強化"));
            Assert.AreEqual(0, unit.UsedSupportAttack);
            Assert.AreEqual(0, unit.UsedSupportGuard);
            Assert.AreEqual(0, unit.UsedSyncAttack);
            Assert.AreEqual(0, unit.UsedCounterAttack);
            Assert.AreEqual("通常", unit.Mode);
        }

        [TestMethod]
        public void FullRecover_RestoresPilotSP()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src), pilotSP: 40);
            var pilot = unit.MainPilot();
            pilot.SP = 0;

            unit.FullRecover();

            Assert.AreEqual(pilot.MaxSP, pilot.SP);
            Assert.IsTrue(pilot.SP > 0);
        }

        [TestMethod]
        public void BulletSupply_And_StockSupply_RestoreSeparately()
        {
            // BulletSupply は弾数のみ、StockSupply は使用回数のみ、FullSupply はＥＮも含めて回復
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイル", "射", 6);
            AddAbility(ud, "修理", "", 2);
            var unit = CreateUnit(src, ud);
            unit.EN = 10;
            Weapon(unit, "ミサイル").SetBullet(1);
            Ability(unit, "修理").SetStock(0);

            unit.BulletSupply();
            Assert.AreEqual(6, Weapon(unit, "ミサイル").Bullet());
            Assert.AreEqual(0, Ability(unit, "修理").Stock());
            Assert.AreEqual(10, unit.EN);

            unit.StockSupply();
            Assert.AreEqual(2, Ability(unit, "修理").Stock());
            Assert.AreEqual(10, unit.EN);

            Weapon(unit, "ミサイル").SetBullet(0);
            Ability(unit, "修理").SetStock(0);
            unit.FullSupply();
            Assert.AreEqual(100, unit.EN);
            Assert.AreEqual(6, Weapon(unit, "ミサイル").Bullet());
            Assert.AreEqual(2, Ability(unit, "修理").Stock());
        }

        [TestMethod]
        public void CanSupply_DetectsBulletAndStockShortage()
        {
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイル", "射", 6);
            AddAbility(ud, "修理", "", 2);
            var unit = CreateUnit(src, ud);
            Assert.IsFalse(unit.CanSupply);

            Weapon(unit, "ミサイル").SetBullet(5);
            Assert.IsTrue(unit.CanBulletRecovery);
            Assert.IsTrue(unit.CanSupply);

            unit.BulletSupply();
            Ability(unit, "修理").SetStock(1);
            Assert.IsFalse(unit.CanBulletRecovery);
            Assert.IsTrue(unit.CanStockRecovery);
            Assert.IsTrue(unit.CanSupply);
        }

        [TestMethod]
        public void RecoverHP_NegativePercent_LeavesAtLeastOne()
        {
            // ＨＰ減少でも 0 にはならず 1 残る
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.HP = 300;

            unit.RecoverHP(-50);

            Assert.AreEqual(1, unit.HP);
        }

        [TestMethod]
        public void RecoverHP_Unstable_GoesBerserkAtQuarterHP()
        {
            // 不安定を持つユニットはＨＰが最大の 1/4 以下になると暴走する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("不安定");
            var unit = CreateUnit(src, ud);
            unit.HP = 400;

            unit.RecoverHP(-10);
            Assert.AreEqual(300, unit.HP);
            Assert.IsFalse(unit.IsConditionSatisfied("暴走"));

            unit.RecoverHP(-5);
            Assert.AreEqual(250, unit.HP);
            Assert.IsTrue(unit.IsConditionSatisfied("暴走"));
            Assert.AreEqual(-1, unit.ConditionLifetime("暴走"));
        }

        [TestMethod]
        public void RecoverEN_ClampsBetweenZeroAndMax()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.EN = 50;

            unit.RecoverEN(30);
            Assert.AreEqual(80, unit.EN);

            unit.RecoverEN(-200);
            Assert.AreEqual(0, unit.EN);

            unit.RecoverEN(500);
            Assert.AreEqual(100, unit.EN);
        }

        [TestMethod]
        public void Reset_ClearsAllConditionsAndRestores()
        {
            // Reset は全状態（付加状態を含む）を消去し、ＨＰ・ＥＮ・弾数を最大に戻す
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイル", "射", 6);
            var unit = CreateUnit(src, ud);
            unit.AddCondition("ＨＰ強化付加", 3, 2d, "");
            unit.Update();
            unit.AddCondition("毒", 3);
            unit.HP = 10;
            unit.EN = 0;
            Weapon(unit, "ミサイル").SetBullet(0);
            unit.Mode = "固定";

            unit.Reset();

            Assert.AreEqual(0, unit.CountCondition());
            Assert.AreEqual(1000, unit.MaxHP);
            Assert.AreEqual(1000, unit.HP);
            Assert.AreEqual(100, unit.EN);
            Assert.AreEqual(6, Weapon(unit, "ミサイル").Bullet());
            Assert.AreEqual("通常", unit.Mode);
        }

        // ──────────────────────────────────────────────
        // SyncBullet
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SyncBullet_SharedWeapons_FollowRemainingRatio()
        {
            // 共属性で同じレベルの武器は残弾の割合を合わせる（多い方は切り捨て計算）
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイルA", "射共", 4);
            AddWeapon(ud, "ミサイルB", "射共", 8);
            AddWeapon(ud, "別系統", "射共L2", 8);
            var unit = CreateUnit(src, ud);
            Weapon(unit, "ミサイルA").SetBullet(1);

            unit.SyncBullet();

            Assert.AreEqual(1, Weapon(unit, "ミサイルA").Bullet());
            Assert.AreEqual(2, Weapon(unit, "ミサイルB").Bullet());
            Assert.AreEqual(8, Weapon(unit, "別系統").Bullet());
        }

        [TestMethod]
        public void SyncBullet_SharedWeaponAndAbility_ShareCount()
        {
            // 共属性の武器とアビリティは弾数・使用回数を共有する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイル", "射共", 4);
            AddAbility(ud, "補給", "共", 4);
            var unit = CreateUnit(src, ud);
            Weapon(unit, "ミサイル").SetBullet(2);

            unit.SyncBullet();
            Assert.AreEqual(2, Ability(unit, "補給").Stock());

            Ability(unit, "補給").SetStock(0);
            unit.SyncBullet();
            Assert.AreEqual(0, Weapon(unit, "ミサイル").Bullet());
        }

        [TestMethod]
        public void SyncBullet_VolleyWeapon_LimitedByOtherWeapons()
        {
            // 斉属性の武器の弾数は他の武器の残弾割合以下になる
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "一斉射撃", "射斉", 4);
            AddWeapon(ud, "ミサイル", "射", 4);
            var unit = CreateUnit(src, ud);
            Weapon(unit, "ミサイル").SetBullet(2);

            unit.SyncBullet();

            Assert.AreEqual(2, Weapon(unit, "一斉射撃").Bullet());
        }

        [TestMethod]
        [Ignore("移植の不具合: SyncBullet の共属性の端数処理が CInt(x + 0.49999)（四捨五入）でなく切り捨てになっている")]
        public void SyncBullet_SharedWeapons_RoundsWhenFewerMaxBullets()
        {
            // 元の SRC: 最大弾数が少ない側は CInt(最大弾数×残弾÷相手の最大弾数 + 0.49999) で合わせる
            // 最大2発中1発 → 最大3発の武器は CInt(1.5 + 0.49999) = 2 発
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイルA", "射共", 2);
            AddWeapon(ud, "ミサイルB", "射共", 3);
            var unit = CreateUnit(src, ud);
            Weapon(unit, "ミサイルA").SetBullet(1);

            unit.SyncBullet();

            Assert.AreEqual(2, Weapon(unit, "ミサイルB").Bullet());
        }

        // ──────────────────────────────────────────────
        // Rest
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Rest_SPRecoverySkill_AddsLevelOver8Plus5()
        {
            // ＳＰ回復: 毎ターン レベル÷8＋5 回復
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src), pilotLevel: 16, pilotSP: 50, "ＳＰ回復");
            var pilot = unit.MainPilot();
            pilot.SP = 0;

            unit.Rest();

            Assert.AreEqual(7, pilot.SP);
        }

        [TestMethod]
        public void Rest_Concentration_RecoversOnlyWhenSPLow()
        {
            // 精神統一: ＳＰが最大の 1/5 未満なら最大の 1/10 回復
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src), pilotSP: 50, skills: "精神統一");
            var pilot = unit.MainPilot();
            var max = pilot.MaxSP;
            pilot.SP = max / 5 - 1;

            unit.Rest();
            Assert.AreEqual(max / 5 - 1 + max / 10, pilot.SP);

            pilot.SP = max / 5;
            unit.Rest();
            Assert.AreEqual(max / 5, pilot.SP);
        }

        [TestMethod]
        public void Rest_Revival_FullyRecoversWhenAllLow()
        {
            // 起死回生: ＳＰ・ＨＰ・ＥＮがすべて最大の 1/5 以下なら全回復
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src), pilotSP: 50, skills: "起死回生");
            var pilot = unit.MainPilot();
            pilot.SP = 0;
            unit.HP = 200;
            unit.EN = 20;

            unit.Rest();

            Assert.AreEqual(pilot.MaxSP, pilot.SP);
            Assert.AreEqual(1000, unit.HP);
            Assert.AreEqual(100, unit.EN);
        }

        [TestMethod]
        public void Rest_Revival_NotTriggeredIfHPHigh()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src), pilotSP: 50, skills: "起死回生");
            var pilot = unit.MainPilot();
            pilot.SP = 0;
            unit.HP = 201;
            unit.EN = 20;

            unit.Rest();

            Assert.AreEqual(0, pilot.SP);
            Assert.AreEqual(201, unit.HP);
        }

        [TestMethod]
        public void Rest_Poison_ModifiedByResistanceAndWeakness()
        {
            // 毒のＨＰ減少は基本 10%、耐性で半減、弱点で倍、無効化で 0
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));

            unit.strResist = "毒";
            unit.HP = 1000;
            unit.AddCondition("毒", 5);
            unit.Rest();
            Assert.AreEqual(950, unit.HP);

            unit.strResist = "";
            unit.strWeakness = "毒";
            unit.HP = 1000;
            unit.Rest();
            Assert.AreEqual(800, unit.HP);

            unit.strWeakness = "";
            unit.strImmune = "毒";
            unit.HP = 1000;
            unit.Rest();
            Assert.AreEqual(1000, unit.HP);
        }

        [TestMethod]
        public void Rest_StrongPoisonOption_Uses25Percent()
        {
            // Option(毒効果大) ではボスランクの無いユニットの毒は 25%
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(毒効果大)");
            src.Expression.SetVariableAsLong("Option(毒効果大)", 1);
            var unit = CreateUnit(src, AddUnitData(src));
            unit.AddCondition("毒", 5);

            unit.Rest();

            Assert.AreEqual(750, unit.HP);
        }

        [TestMethod]
        public void Rest_TerrainDecrease_ModifiedByAttributes()
        {
            // 地形のＨＰ減少=属性 は 10%/Lv、耐性で半減、吸収で回復、無効化で 0
            var src = CreateSrc();
            var td = CreateMap(src);
            td.AddFeature("ＨＰ減少Lv1=火");
            var unit = CreateUnit(src, AddUnitData(src));
            unit.StandBy(2, 2);

            unit.HP = 500;
            unit.Rest();
            Assert.AreEqual(400, unit.HP);

            unit.strResist = "火";
            unit.HP = 500;
            unit.Rest();
            Assert.AreEqual(450, unit.HP);

            unit.strResist = "";
            unit.strAbsorb = "火";
            unit.HP = 500;
            unit.Rest();
            Assert.AreEqual(600, unit.HP);

            unit.strAbsorb = "";
            unit.strImmune = "火";
            unit.HP = 500;
            unit.Rest();
            Assert.AreEqual(500, unit.HP);
        }

        [TestMethod]
        public void Rest_TerrainEffectImmunity_BlocksDecreaseButNotRecovery()
        {
            // 地形効果無効化は減少・状態付加のみ無効化し、回復は受ける
            var src = CreateSrc();
            var td = CreateMap(src);
            td.AddFeature("ＨＰ減少Lv1=火");
            td.AddFeature("ＥＮ増加Lv2");
            td.AddFeature("状態付加=毒");
            var ud = AddUnitData(src);
            ud.AddFeature("地形効果無効化");
            var unit = CreateUnit(src, ud);
            unit.StandBy(2, 2);
            unit.HP = 500;
            unit.EN = 10;

            unit.Rest();

            Assert.AreEqual(500, unit.HP);
            // 自然回復 5 + ＥＮ増加 20
            Assert.AreEqual(35, unit.EN);
            Assert.IsFalse(unit.IsConditionSatisfied("毒"));
        }

        [TestMethod]
        public void Rest_TerrainEffectImmunity_ForOtherTerrainOnly()
        {
            // 地形効果無効化=名称 地形名 は指定地形にのみ有効
            var src = CreateSrc();
            var td = CreateMap(src);
            td.AddFeature("ＨＰ減少Lv1=火");
            var ud = AddUnitData(src);
            ud.AddFeature("地形効果無効化=地形効果無効化 溶岩");
            var unit = CreateUnit(src, ud);
            unit.StandBy(2, 2);
            unit.HP = 500;

            unit.Rest();

            Assert.AreEqual(400, unit.HP);
        }

        [TestMethod]
        public void Rest_TerrainHPUp_AddsFixedAmount()
        {
            // ＨＰ増加は 1000/Lv の固定値回復、ＥＮ低下=属性 は 10/Lv の固定値減少
            var src = CreateSrc();
            var td = CreateMap(src);
            td.AddFeature("ＨＰ増加Lv0.1");
            td.AddFeature("ＥＮ低下Lv1=冷");
            var unit = CreateUnit(src, AddUnitData(src));
            unit.StandBy(2, 2);
            unit.HP = 500;
            unit.EN = 50;

            unit.Rest();

            Assert.AreEqual(600, unit.HP);
            Assert.AreEqual(45, unit.EN);
        }

        [TestMethod]
        public void Rest_TerrainEffectRange_ExcludesOtherAreas()
        {
            // 効果範囲で指定された場所にいない場合、地形効果は適用されない
            var src = CreateSrc();
            var td = CreateMap(src);
            td.AddFeature("ＨＰ回復Lv2");
            td.AddFeature("効果範囲=空中");
            var unit = CreateUnit(src, AddUnitData(src));
            unit.StandBy(2, 2);
            unit.HP = 500;

            unit.Rest();

            Assert.AreEqual("地上", unit.Area);
            Assert.AreEqual(500, unit.HP);
        }

        [TestMethod]
        public void Rest_TerrainCondition_ImmuneUnitIsNotAffected()
        {
            // 状態付加は特殊効果無効化を持つユニットには付加されない。Lv指定が無ければ 10000 ターン
            var src = CreateSrc();
            var td = CreateMap(src);
            td.AddFeature("状態付加=毒");
            var unit = CreateUnit(src, AddUnitData(src));
            unit.StandBy(2, 2);
            unit.strSpecialEffectImmune = "毒";

            unit.Rest();
            Assert.IsFalse(unit.IsConditionSatisfied("毒"));

            unit.strSpecialEffectImmune = "";
            unit.Rest();
            Assert.AreEqual(10000, unit.ConditionLifetime("毒"));
        }

        [TestMethod]
        public void Rest_RecoveryBlocked_StillAppliesConsumption()
        {
            // 回復不能状態でもＨＰ消費は適用される
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("ＨＰ回復Lv2");
            ud.AddFeature("ＨＰ消費Lv1");
            var unit = CreateUnit(src, ud);
            unit.HP = 500;

            unit.Rest();
            Assert.AreEqual(600, unit.HP);

            unit.AddCondition("回復不能", 3);
            unit.Rest();
            Assert.AreEqual(500, unit.HP);
        }

        [TestMethod]
        public void Rest_Unstable_GoesBerserkWhenHPFallsToQuarter()
        {
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("不安定");
            ud.AddFeature("ＨＰ消費Lv1");
            var unit = CreateUnit(src, ud);
            unit.HP = 350;

            unit.Rest();

            Assert.AreEqual(250, unit.HP);
            Assert.IsTrue(unit.IsConditionSatisfied("暴走"));
        }

        [TestMethod]
        public void Rest_ENExhaustedWithNormalMode_ReturnsToNormalForm()
        {
            // ＥＮ消費でＥＮが尽きたハイパーモード形態はノーマルモードに戻る
            var src = CreateSrc();
            var hyper = AddUnitData(src, "テスト機体改");
            hyper.AddFeature("ノーマルモード=テスト機体");
            hyper.AddFeature("ＥＮ消費Lv5");
            var normal = AddUnitData(src);
            normal.AddFeature("ハイパーモード=ハイパーモード テスト機体改");
            var unit = CreateUnit(src, normal);
            CreateMap(src);
            unit.StandBy(2, 2);
            unit.Transform("テスト機体改");
            var cf = unit.CurrentForm();
            Assert.AreEqual("テスト機体改", cf.Name);
            cf.EN = 10;

            cf.Rest();

            Assert.AreEqual("テスト機体", unit.CurrentForm().Name);
            Assert.AreEqual("出撃", unit.CurrentForm().Status);
        }

        [TestMethod]
        public void Rest_TimeLimitWithoutNormalMode_Escapes()
        {
            // 残り時間が切れたユニットはノーマルモード・分離が無ければ退却する
            var src = CreateSrc();
            CreateMap(src);
            var unit = CreateUnit(src, AddUnitData(src));
            unit.StandBy(2, 2);
            unit.AddCondition("残り時間", 1);

            unit.Rest();

            Assert.AreEqual("待機", unit.Status);
            Assert.IsNull(src.Map.MapDataForUnit[2, 2]);
            CollectionAssert.Contains(sysMessages, unit.Nickname + "は制限時間切れのため退却します。");
        }

        [TestMethod]
        public void Rest_LostFlightOverUnenterableTerrain_ForcesEscape()
        {
            // 付加された空中移動が切れ、その場所に留まれない場合は強制退却
            var src = CreateSrc();
            var td = CreateMap(src);
            td.Class = "空";
            var unit = CreateUnit(src, AddUnitData(src));
            unit.AddCondition("空中移動付加", 1, 0d, "");
            unit.Update();
            unit.StandBy(2, 2);
            Assert.AreEqual("空中", unit.Area);

            unit.Rest();

            Assert.AreEqual("待機", unit.Status);
            CollectionAssert.Contains(sysMessages, unit.Nickname + "は強制的に退却させられた。");
        }
    }
}
