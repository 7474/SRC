using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// UnitAbility のレビュー指摘で追加した、霊力・一斉使用・無効化・不得手・補給・装填・
    /// 霊力回復/ＳＰ回復/気力増加・合体技・IsAbilityApplicable/IsAbilityUseful のテスト。
    /// 元の SRC の Unit.cls（IsAbilityAvailable / UseAbility / ExecuteAbility /
    /// IsAbilityApplicable / IsAbilityUseful）に対応する。
    /// </summary>
    [TestClass]
    public class UnitAbilityCoverageReviewTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        // 使用者ユニット（アビリティ1つ）と対象ユニットを作る
        private (Unit user, Unit target, UnitAbility ability) Create(
            string effect, string aclass = "", int maxRange = 1, int stock = 0)
        {
            var ud = fx.AddUnitData("使用機");
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.MinRange = maxRange == 0 ? 0 : 1;
            ad.MaxRange = maxRange;
            ad.Stock = stock;
            if (!fx.Src.UDList.IsDefined("対象機"))
            {
                fx.AddUnitData("対象機");
            }
            var user = fx.CreateUnit("使用機", "使用者");
            var target = fx.CreateUnit("対象機", "対象者");
            return (user, target, user.Ability(1));
        }

        // ステータス表示用の空マップ（マップファイルなし）を用意する
        private void SetupEmptyMap()
        {
            var map = fx.Src.Map;
            map.MapFileName = "";
            map.MapWidth = 10;
            map.MapHeight = 10;
            map.MapData = new Src2DArray<MapCell>(map.MapWidth, map.MapHeight);
            for (var x = 1; x <= map.MapWidth; x++)
            {
                for (var y = 1; y <= map.MapHeight; y++)
                {
                    map.MapData[x, y] = MapCell.EmptyCell;
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(map.MapWidth, map.MapHeight);
        }

        private void Place(Unit u, int x, int y)
        {
            u.x = x;
            u.y = y;
            fx.Src.Map.MapDataForUnit[x, y] = u;
        }

        // ===== 霊/プ 属性 =====

        [TestMethod]
        public void PlanaClass_RequiresPlana5xLevel_AndUseConsumesIt()
        {
            // 霊LN は使用に霊力 5×N が必要で、使用時に 5×N 消費する（HP・EN は割合を保つ）
            fx.AddPilotData("使用者").AddSkill("霊力", 20d, "", 1);
            var (user, _, a) = Create("回復Lv1", aclass: "霊L2");
            var p = user.MainPilot();
            Assert.AreEqual(21, p.MaxPlana());

            p.Plana = 9;
            Assert.IsFalse(a.IsAbilityAvailable("移動前"));
            p.Plana = 10;
            Assert.IsTrue(a.IsAbilityAvailable("移動前"));

            user.HP = 500;
            user.EN = 40;
            p.Plana = 21;
            a.UseAbility();
            Assert.AreEqual(11, p.Plana);
            Assert.AreEqual(500, user.HP);
            Assert.AreEqual(40, user.EN);
        }

        [TestMethod]
        public void PranaClass_RequiresPlanaLikeSpiritClass()
        {
            // プ属性も霊属性と同じく 5×N の霊力を要求・消費する
            fx.AddPilotData("使用者").AddSkill("霊力", 20d, "", 1);
            var (user, _, a) = Create("回復Lv1", aclass: "プL1");
            var p = user.MainPilot();
            p.Plana = 4;
            Assert.IsFalse(a.IsAbilityAvailable("移動前"));
            p.Plana = 5;
            Assert.IsTrue(a.IsAbilityAvailable("移動前"));
            a.UseAbility();
            Assert.AreEqual(0, p.Plana);
        }

        // ===== 斉 属性の逆方向 =====

        [TestMethod]
        public void Use_NormalAbility_LowersSimultaneousAbilityStock()
        {
            // 通常アビリティを使うと斉属性アビリティの使用回数も
            // MinLng(CInt(MaxStock(斉) * Stock(a) \ MaxStock(a) + 0.49999), Stock(斉)) に下がる
            var ud = fx.AddUnitData("使用機");
            var ad1 = ud.AddAbility("通常");
            ad1.SetEffect("補給Lv1");
            ad1.Stock = 4;
            var ad2 = ud.AddAbility("一斉");
            ad2.SetEffect("回復Lv1");
            ad2.Class = "斉";
            ad2.Stock = 2;
            var u = fx.CreateUnit("使用機", "使用者");

            u.Ability(1).UseAbility();

            Assert.AreEqual(3, u.Ability(1).Stock());
            // 2 * 3 \ 4 = 1
            Assert.AreEqual(1, u.Ability(2).Stock());
        }

        // ===== ExecuteAbility: 無効化・不得手 =====

        [TestMethod]
        public void Execute_TargetImmuneToClass_HasNoEffect()
        {
            // 対象がアビリティの属性を無効化するなら効果なし（Unit.cls: If t.Immune(aclass) Then GoTo Finish）
            fx.AddUnitData("対象機").AddFeature("無効化=魔");
            var (user, target, a) = Create("回復Lv1", aclass: "魔脱");
            target.HP = 300;
            var morale = target.MainPilot().Morale;

            var useful = user.ExecuteAbility(a, target);

            Assert.IsFalse(useful);
            Assert.AreEqual(300, target.HP);
            Assert.AreEqual(morale, target.MainPilot().Morale, "脱属性の気力低下も行われない");
        }

        [TestMethod]
        public void Execute_WeakTechnique_MultipliesLevelBy0_8()
        {
            // 不得手に該当する属性のアビリティは効果レベルが 0.8 倍: 500×0.8 = 400
            fx.AddPilotData("使用者").AddSkill("不得手", 1d, "魔", 1);
            var (user, target, a) = Create("回復Lv1", aclass: "魔");
            target.HP = 100;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(500, target.HP);
        }

        [TestMethod]
        public void Execute_WeakTechnique_ShortensAttachDurationBy0_6()
        {
            // 付加の持続ターンは elevel2（不得手で 0.6 倍）: 5×0.6 = 3
            fx.AddPilotData("使用者").AddSkill("不得手", 1d, "魔", 1);
            var (user, target, a) = Create("付加Lv5=耐性Lv1=火", aclass: "魔");

            user.ExecuteAbility(a, target);

            Assert.AreEqual(3, target.ConditionLifetime("耐性付加"));
        }

        // ===== ExecuteAbility: 補給 =====

        [TestMethod]
        public void Supply_SupplySkillIncreasesAmount()
        {
            // 補給技能 LvN で補給量は (10+N)/10 倍: 50×12/10 = 60
            fx.AddPilotData("使用者").AddSkill("補給", 2d, "", 1);
            var (user, target, a) = Create("補給Lv1");
            target.EN = 10;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(70, target.EN);
        }

        [TestMethod]
        public void Supply_OnZombieTarget_IsNotUseful()
        {
            var (user, target, a) = Create("補給Lv1");
            target.EN = 10;
            target.AddCondition("ゾンビ", 3);

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.AreEqual(10, target.EN);
        }

        [TestMethod]
        public void SpellSupply_AmountIsLevelxShootingHalf()
        {
            // 術の補給量は レベル×射撃 \ 2: 2×40\2 = 40
            fx.AddPilotData("使用者").Shooting = 39;
            var (user, target, a) = Create("補給Lv2", aclass: "術");
            target.EN = 20;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(60, target.EN);
        }

        // ===== ExecuteAbility: 装填（データ指定なし） =====

        [TestMethod]
        public void Reload_WithoutData_RefillsAllTargetWeapons()
        {
            // 装填（データなし）は対象の全武器の弾数を回復する（使用者の武器は変わらない）
            var uud = fx.AddUnitData("使用機");
            var uw = uud.AddWeapon("バルカン");
            uw.Bullet = 5;
            uw.MaxRange = 2;
            var tud = fx.AddUnitData("対象機");
            var w1 = tud.AddWeapon("ライフル");
            w1.Bullet = 6;
            w1.MaxRange = 3;
            var w2 = tud.AddWeapon("ミサイル");
            w2.Bullet = 4;
            w2.MaxRange = 3;
            var ad = uud.AddAbility("テストアビリティ");
            ad.SetEffect("装填");
            ad.MinRange = 1;
            ad.MaxRange = 1;
            var user = fx.CreateUnit("使用機", "使用者");
            var target = fx.CreateUnit("対象機", "対象者");
            user.Weapon(1).SetBullet(0);
            target.Weapon(1).SetBullet(0);
            target.Weapon(2).SetBullet(1);

            var useful = user.ExecuteAbility(user.Ability(1), target);

            Assert.IsTrue(useful);
            Assert.AreEqual(6, target.Weapon(1).Bullet());
            Assert.AreEqual(4, target.Weapon(2).Bullet());
            Assert.AreEqual(0, user.Weapon(1).Bullet());
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機の武装の使用回数が回復した。");
        }

        [TestMethod]
        public void Reload_WithoutData_RefillsSharedAbilityStock()
        {
            // 装填（データなし）は共属性の武器と同じ共レベルのアビリティの使用回数も最大にする
            // （Unit.cls ExecuteAbility 装填: .SetStock j, .MaxStock(j)）
            var tud = fx.AddUnitData("対象機");
            var w1 = tud.AddWeapon("ライフル");
            w1.Bullet = 6;
            w1.MaxRange = 3;
            w1.Class = "共L1";
            var tad = tud.AddAbility("弾薬補充");
            tad.SetEffect("補給Lv1");
            tad.Class = "共L1";
            tad.Stock = 3;
            var (user, target, a) = Create("装填");
            target.Weapon(1).SetBullet(0);
            target.Ability(1).SetStock(0);

            user.ExecuteAbility(a, target);

            Assert.AreEqual(6, target.Weapon(1).Bullet());
            Assert.AreEqual(3, target.Ability(1).Stock());
        }

        [TestMethod]
        public void Reload_WithoutData_AllFull_IsNotUseful()
        {
            var tud = fx.AddUnitData("対象機");
            var w1 = tud.AddWeapon("ライフル");
            w1.Bullet = 6;
            w1.MaxRange = 3;
            var (user, target, a) = Create("装填");

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.IsFalse(fx.SysMessages.Any(m => m.Contains("使用回数が回復した")));
        }

        // ===== ExecuteAbility: 霊力回復・ＳＰ回復・気力増加 =====

        [TestMethod]
        public void PlanaRecovery_Restores10PerLevel_AndNegativeReduces()
        {
            fx.AddPilotData("対象者").AddSkill("霊力", 20d, "", 1);
            var (user, target, a) = Create("霊力回復Lv1");
            var p = target.MainPilot();
            p.Plana = 5;

            Assert.IsTrue(user.ExecuteAbility(a, target));
            Assert.AreEqual(15, p.Plana);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象者の[霊力]が[10]回復した。");

            // 霊力が最大値なら回復処理は行われない
            p.Plana = p.MaxPlana();
            Assert.IsFalse(user.ExecuteAbility(a, target));
        }

        [TestMethod]
        public void PlanaRecovery_NegativeLevel_ReducesPlana_AndSkipsAtZero()
        {
            fx.AddPilotData("対象者").AddSkill("霊力", 20d, "", 1);
            var (user, target, a) = Create("霊力回復Lv-1");
            var p = target.MainPilot();
            p.Plana = 15;

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.AreEqual(5, p.Plana);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象者の[霊力]が[10]減少した。");

            p.Plana = 0;
            var before = fx.SysMessages.Count();
            user.ExecuteAbility(a, target);
            Assert.AreEqual(0, p.Plana);
            Assert.IsFalse(fx.SysMessages.Skip(before).Any(m => m.Contains("減少した")));
        }

        [TestMethod]
        public void SPRecovery_SinglePilot_Restores10PerLevel()
        {
            // パイロット1名なら 10×Lv を回復（最大ＳＰ = 2×レベル + 基本値 = 52）
            fx.AddPilotData("対象者").SP = 50;
            var (user, target, a) = Create("ＳＰ回復Lv1");
            var p = target.MainPilot();
            Assert.AreEqual(52, p.MaxSP);
            p.SP = 10;

            Assert.IsTrue(user.ExecuteAbility(a, target));
            Assert.AreEqual(20, p.SP);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象者のＳＰが10回復した。");

            // 最大値を超えて回復した分は有効扱いにならない
            p.SP = p.MaxSP;
            Assert.IsFalse(user.ExecuteAbility(a, target));
        }

        [TestMethod]
        public void SPRecovery_NegativeLevel_ReducesSP()
        {
            fx.AddPilotData("対象者").SP = 50;
            var (user, target, a) = Create("ＳＰ回復Lv-2");
            var p = target.MainPilot();
            p.SP = 30;

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.AreEqual(10, p.SP);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象者のＳＰが20減少した。");
        }

        [TestMethod]
        public void MoraleIncrease_NegativeLevel_ReducesMorale_NotUseful()
        {
            var (user, target, a) = Create("気力増加Lv-1");
            var p = target.MainPilot();
            var prev = p.Morale;

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.AreEqual(prev - 10, p.Morale);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象者の気力が10減少した。");
        }

        // ===== 合体技 =====

        [TestMethod]
        public void CombinationAbility_WithoutPartner_IsUnavailable_ButShownInStatus()
        {
            // 合体技はパートナーが見つからなければ使用不可。ステータス表示では必要技能のみで判定
            var (user, _, a) = Create("回復Lv1", aclass: "合");

            Assert.IsFalse(a.IsCombinationAbilityAvailable());
            Assert.IsFalse(a.IsAbilityAvailable("移動前"));
            Assert.IsTrue(a.IsAbilityAvailable("インターミッション"));
        }

        // ===== IsAbilityApplicable =====

        [TestMethod]
        public void Applicable_SupportOnlyClass_NotOnSelf()
        {
            // 援属性（支援専用）は自分には使えないが他ユニットには使える
            var (user, target, a) = Create("回復Lv1", aclass: "援");
            Assert.IsFalse(a.IsAbilityApplicable(user));
            Assert.IsTrue(a.IsAbilityApplicable(target));
        }

        [TestMethod]
        public void Applicable_SealedClass_RequiresWeaknessOrEffective()
        {
            // 封属性は対象が弱点か有効を持つ場合のみ使用可能
            var (user, target, a) = Create("回復Lv1", aclass: "封魔");
            Assert.IsFalse(a.IsAbilityApplicable(target));

            var fx2 = new UnitAbilityCoverageFixture();
            fx = fx2;
            fx.AddUnitData("対象機").AddFeature("弱点=魔");
            var (_, target2, a2) = Create("回復Lv1", aclass: "封魔");
            Assert.IsTrue(a2.IsAbilityApplicable(target2));
        }

        [TestMethod]
        public void Applicable_ImmuneTarget_IsFalse()
        {
            fx.AddUnitData("対象機").AddFeature("無効化=魔");
            var (user, target, a) = Create("回復Lv1", aclass: "魔");
            Assert.IsFalse(a.IsAbilityApplicable(target));
            // 自分自身には無効化の判定をしない
            Assert.IsTrue(a.IsAbilityApplicable(user));
        }

        [TestMethod]
        public void Applicable_SexRestriction()
        {
            // ♂ 属性は男性パイロットにのみ使用可能
            fx.AddPilotData("対象者").Sex = "女性";
            var (_, target, a) = Create("回復Lv1", aclass: "♂");
            Assert.IsFalse(a.IsAbilityApplicable(target));
            fx.Src.PDList.Item("対象者").Sex = "男性";
            Assert.IsTrue(a.IsAbilityApplicable(target));
        }

        [TestMethod]
        public void Applicable_BlindTargetForVisualClass_IsFalse()
        {
            var (_, target, a) = Create("回復Lv1", aclass: "視");
            Assert.IsTrue(a.IsAbilityApplicable(target));
            target.AddCondition("盲目", 2);
            Assert.IsFalse(a.IsAbilityApplicable(target));
        }

        [TestMethod]
        public void Update_KeepsAbilityStock()
        {
            // 元の SRC の Unit.Update は同じアビリティデータの使用回数(dblStock)を引き継ぎ、新規分は 1（満タン）にする
            var (user, _, a) = Create("回復Lv1", stock: 3);
            a.SetStock(1);

            user.Update();

            Assert.AreEqual(1, user.Ability(1).Stock());
        }

        // ===== IsAbilityUseful =====

        [TestMethod]
        public void Useful_RequiresEffectiveAllyWithinRange()
        {
            // 射程内に効果のある味方がいるときだけ有用
            SetupEmptyMap();
            var (user, target, a) = Create("回復Lv1");
            Place(user, 5, 5);
            Place(target, 5, 6);

            // 対象のＨＰが満タンなら有用でない（使用者自身も満タン）
            Assert.IsFalse(a.IsAbilityUseful("移動前"));

            target.HP = 500;
            Assert.IsTrue(a.IsAbilityUseful("移動前"));

            // 射程外に離すと有用でない
            fx.Src.Map.MapDataForUnit[5, 6] = null;
            Place(target, 5, 8);
            Assert.IsFalse(a.IsAbilityUseful("移動前"));
        }

        [TestMethod]
        public void Useful_EnemyTarget_IsIgnored()
        {
            // 味方のアビリティは敵には使用しない
            SetupEmptyMap();
            var (user, _, a) = Create("回復Lv1");
            var enemy = fx.CreateUnit("対象機", "敵パイロット", "敵");
            Place(user, 5, 5);
            Place(enemy, 5, 6);
            enemy.HP = 500;

            Assert.IsFalse(a.IsAbilityUseful("移動前"));
        }

        [TestMethod]
        public void Useful_SummonIsAlwaysUseful_WhenAvailable()
        {
            // 召喚アビリティは使用可能なら常に有用（対象検索をしない）
            SetupEmptyMap();
            // 元の SRC と同じく召喚ユニットと追加パイロットのデータを定義しておく
            fx.AddUnitData("召喚獣").AddFeature("追加パイロット=召喚獣パイロット(汎用)");
            fx.AddPilotData("召喚獣パイロット(汎用)");
            var (user, _, a) = Create("召喚=召喚獣", maxRange: 0);
            Place(user, 5, 5);

            Assert.IsTrue(a.IsAbilityUseful("移動前"));
        }
    }
}
