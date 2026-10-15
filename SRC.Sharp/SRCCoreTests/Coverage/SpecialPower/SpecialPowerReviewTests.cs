using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// 追加パイロットのＳＰ共有、ＳＰ消費減少付加、効果別の Effective 判定の補足、
    /// UseSpecialPower の使用メッセージ表示。
    /// 元の SRC の Pilot.SP / MaxSP / SpecialPowerCost / UseSpecialPower、SpecialPowerData.Effective に対応。
    /// </summary>
    [TestClass]
    public class SpecialPowerReviewTests
    {
        // ──────────────────────────────────────────────
        // 追加パイロットのＳＰ
        // ──────────────────────────────────────────────

        /// <summary>追加パイロットを持つユニットを作り、(第１パイロット, 追加パイロット) を返す</summary>
        private static (Pilots.Pilot pilot1, Pilots.Pilot additional) CreateWithAdditionalPilot(
            SpecialPowerTestFixture f, int additionalSP)
        {
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1", cost: 40);
            var pd1 = f.DefinePilot("本体", sp: 50);
            pd1.AddSpecialPower("熱血", 1, 0);
            pd1.AddSkill("集中力", 1, "", 1);
            f.DefinePilot("追加", sp: additionalSP);
            var ud = f.Src.UDList.Add("機体");
            ud.AddFeature("追加パイロット=追加");

            var u = f.AddUnit("機体", "本体", "味方", 1, 1, level: 10);
            var pilot1 = u.Pilots.First();
            var additional = u.MainPilot();
            Assert.AreEqual("追加", additional.Name);
            return (pilot1, additional);
        }

        [TestMethod]
        public void AdditionalPilotWithoutSP_SharesPilot1SPAndSpecialPowers()
        {
            // ＳＰを持たない追加パイロットは第１パイロットの最大ＳＰ・ＳＰ値・スペシャルパワー・消費ＳＰをそのまま使う
            var f = new SpecialPowerTestFixture();
            var (pilot1, add) = CreateWithAdditionalPilot(f, additionalSP: 0);

            Assert.IsTrue(add.IsActiveAdditionalPilot());
            Assert.IsFalse(pilot1.IsActiveAdditionalPilot());
            Assert.AreEqual(70, pilot1.MaxSP);
            Assert.AreEqual(70, add.MaxSP);

            pilot1.SP0 = 40;
            Assert.AreEqual(40, add.SP, "第１パイロットのＳＰ値を読む");

            add.SP = 25;
            Assert.AreEqual(25, pilot1.SP0, "第１パイロットのＳＰ値に書き込む");
            add.SP = 1000;
            Assert.AreEqual(70, pilot1.SP0, "第１パイロットの最大ＳＰで頭打ち");
            add.SP = -3;
            Assert.AreEqual(0, pilot1.SP0);

            // スペシャルパワーと消費ＳＰは第１パイロットのもの (集中力で 40×0.8)
            CollectionAssert.AreEqual(new[] { "熱血" }, add.SpecialPowerNames.ToArray());
            Assert.IsTrue(add.IsSpecialPowerAvailable("熱血"));
            Assert.AreEqual(32, add.SpecialPowerCost("熱血"));
        }

        [TestMethod]
        public void AdditionalPilotWithSP_KeepsSameConsumptionRatioAsPilot1()
        {
            // ＳＰを持つ追加パイロットは自分の最大ＳＰを持ち、第１パイロットと消費率を一致させる
            var f = new SpecialPowerTestFixture();
            var (pilot1, add) = CreateWithAdditionalPilot(f, additionalSP: 30);

            Assert.AreEqual(70, pilot1.MaxSP);
            Assert.AreEqual(30 + 20, add.MaxSP);

            // 読み出し: 50 × 35 \ 70 = 25
            pilot1.SP0 = 35;
            Assert.AreEqual(25, add.SP);

            // 書き込み: 第１パイロットは 70 × 10 \ 50 = 14、追加パイロットは 50 × 14 \ 70 = 10
            add.SP = 10;
            Assert.AreEqual(14, pilot1.SP0);
            Assert.AreEqual(10, add.SP0);
        }

        // ──────────────────────────────────────────────
        // ＳＰ消費減少付加
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerCost_SPCostReductionGrantedByCondition()
        {
            // ユニットに「ＳＰ消費減少付加」状態があればそのレベル・データで消費ＳＰを減らす
            var f = new SpecialPowerTestFixture();
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1", cost: 40);
            f.DefineSP("必中", "自分", "ターン", "絶対命中", cost: 15);
            var pd = f.DefinePilot("パイロット");
            pd.AddSpecialPower("熱血", 1, 0);
            pd.AddSpecialPower("必中", 1, 0);
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10);
            var p = u.MainPilot();
            Assert.AreEqual(40, p.SpecialPowerCost("熱血"));

            u.AddCondition("ＳＰ消費減少付加", -1, 3, "熱い心 熱血");

            Assert.AreEqual((10 - 3) * 40 / 10, p.SpecialPowerCost("熱血"));
            Assert.AreEqual(15, p.SpecialPowerCost("必中"), "データに含まれないものは減らない");
        }

        // ──────────────────────────────────────────────
        // Load: 適用条件
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Load_NumericNecessaryCondition_Throws()
        {
            // 適用条件に数値は指定できない
            var src = new SRC();
            var ex = Assert.Throws<InvalidSrcDataException>(() =>
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(
                    "熱血\n熱, 40, 自分, 攻撃, 1, -\nダメージ増加Lv1\n解説\n")))
                {
                    src.SPDList.Load("sp.txt", stream);
                }
            });
            Assert.AreEqual("適用条件が間違っています。", ex.InvalidDataList[0].msg);
        }

        // ──────────────────────────────────────────────
        // Effective: 行動数増加・装填・霊力
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ActionIncrease_EffectiveOnlyBelowThreeActions_UnlessNegative()
        {
            // 行動数増加は行動数が３未満で行動可能な場合のみ有効。負のレベルは常に有効
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("覚醒", "自分", "即効", "行動数増加Lv1");
            var minus = f.DefineSP("鈍足", "自分", "即効", "行動数増加Lv-1");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();

            u.UsedAction = -1;
            Assert.AreEqual(2, u.Action);
            Assert.IsTrue(sd.Effective(p, u));

            u.UsedAction = -2;
            Assert.AreEqual(3, u.Action);
            Assert.IsFalse(sd.Effective(p, u), "行動数３で頭打ち");
            Assert.IsTrue(minus.Effective(p, u), "負のレベルは常に有効");

            u.UsedAction = 0;
            u.AddCondition("麻痺", 2);
            Assert.AreEqual(0, u.MaxAction());
            Assert.IsFalse(sd.Effective(p, u), "行動不能なユニットには無効");
        }

        [TestMethod]
        public void Reload_EffectiveOnlyWhenSomeWeaponIsNotFull()
        {
            // 装填は弾数が最大でない武器があるときのみ有効
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("補給", "自分", "即効", "装填");
            var ud = f.Src.UDList.Add("機体");
            var wd = ud.AddWeapon("ミサイル");
            wd.Power = 1000;
            wd.MinRange = 1;
            wd.MaxRange = 3;
            wd.Bullet = 4;
            wd.Class = "";
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            var w = u.Weapons.First();
            Assert.AreEqual(4, w.Bullet());
            Assert.IsFalse(sd.Effective(p, u), "全弾補充済みなら効果なし");

            w.SetBullet(1);
            Assert.IsTrue(sd.Effective(p, u));
            sd.Apply(p, u, is_event: true);
            Assert.AreEqual(4, w.Bullet());
        }

        [TestMethod]
        public void PlanaRecoveryAndIncrease()
        {
            // 霊力回復Lvn は最大霊力の n×10%、霊力増加Lvn は 10×n 霊力を回復。満タンなら効果なし
            var f = new SpecialPowerTestFixture();
            var recover = f.DefineSP("祈り", "自分", "即効", "霊力回復Lv2");
            var increase = f.DefineSP("念", "自分", "即効", "霊力増加Lv3");
            f.DefinePilot("パイロット").AddSkill("霊力", 50, "", 1);
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10);
            var p = u.MainPilot();
            // 最大霊力 = 霊力Lv + 1.5×Lv = 50 + 15
            Assert.AreEqual(65, p.MaxPlana());
            p.Plana = p.MaxPlana();
            Assert.IsFalse(recover.Effective(p, u));
            Assert.IsFalse(increase.Effective(p, u));

            p.Plana = 10;
            Assert.IsTrue(recover.Effective(p, u));
            recover.Apply(p, u, is_event: true);
            Assert.AreEqual(10 + 65 * 2 / 10, p.Plana);

            increase.Apply(p, u, is_event: true);
            Assert.AreEqual(23 + 30, p.Plana);
        }

        // ──────────────────────────────────────────────
        // Effective: 無効化・気力
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SeishinCommandNullification_BlocksEnemyTargetedOnly()
        {
            // 「精神コマンド無効化」も敵・全敵・任意・全を対象とするものを無効にする
            var f = new SpecialPowerTestFixture();
            var enemySp = f.DefineSP("脱力", "敵", "即効", "気力低下Lv1");
            var anySp = f.DefineSP("干渉", "任意", "即効", "気力低下Lv1");
            var allySp = f.DefineSP("激励", "味方", "即効", "気力増加Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5);
            Assert.IsTrue(enemySp.Effective(me.MainPilot(), enemy));

            enemy.AddCondition("精神コマンド無効化", 3);

            Assert.IsFalse(enemySp.Effective(me.MainPilot(), enemy));
            Assert.IsFalse(anySp.Effective(me.MainPilot(), enemy));
            Assert.IsTrue(allySp.Effective(me.MainPilot(), enemy));
        }

        [TestMethod]
        public void MoraleIncrease_EffectiveWhenOnlySubPilotCanGain()
        {
            // メインパイロットが気力上限でも、２人目以降のパイロットが上げられれば有効 (機械は除く)
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("激励", "味方", "即効", "気力増加Lv1");
            var u = f.AddUnit("機体", "メイン", "味方", 1, 1);
            var main = u.MainPilot();
            f.DefinePilot("サブ");
            var sub = f.Src.PList.Add("サブ", 1, "味方");
            u.AddPilot(sub);
            sub.Unit = u;

            main.Morale = main.MaxMorale;
            sub.Morale = 100;
            Assert.IsTrue(sd.Effective(main, u));

            sub.Morale = main.MaxMorale;
            Assert.IsFalse(sd.Effective(main, u));
        }

        [TestMethod]
        public void MoraleIncrease_MachineSubPilotIsIgnored()
        {
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("激励", "味方", "即効", "気力増加Lv1");
            var u = f.AddUnit("機体", "メイン", "味方", 1, 1);
            var main = u.MainPilot();
            f.DefinePilot("ロボ").Personality = "機械";
            var sub = f.Src.PList.Add("ロボ", 1, "味方");
            u.AddPilot(sub);
            sub.Unit = u;

            main.Morale = main.MaxMorale;
            sub.Morale = 100;
            Assert.IsFalse(sd.Effective(main, u));
        }

        [TestMethod]
        public void MoraleDecrease_MachineMainPilot_NotEffective()
        {
            // 気力低下は性格が機械のメインパイロットには効かない
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("脱力", "敵", "即効", "気力低下Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            f.DefinePilot("ロボ").Personality = "機械";
            var robo = f.AddUnit("敵機", "ロボ", "敵", 5, 5);
            var human = f.AddUnit("敵機２", "敵パイロット", "敵", 6, 6);
            Assert.IsTrue(robo.MainPilot().Morale > robo.MainPilot().MinMorale);

            Assert.IsFalse(sd.Effective(me.MainPilot(), robo));
            Assert.IsTrue(sd.Effective(me.MainPilot(), human));
        }

        // ──────────────────────────────────────────────
        // UseSpecialPower の使用メッセージ
        // ──────────────────────────────────────────────

        private static (Unit u, List<string> log) SetupMessage(SpecialPowerTestFixture f, string sname, string effects)
        {
            f.DefineSP(sname, "自分", "即効", effects, cost: 10);
            var pd = f.DefinePilot("パイロット");
            pd.AddSpecialPower(sname, 1, 0);
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10);
            var p = u.MainPilot();

            var log = new List<string>();
            f.GUI.OpenMessageFormHandler = (u1, u2) => { log.Add("Open"); f.GUI.MessageFormVisible = true; };
            f.GUI.CloseMessageFormHandler = () => { log.Add("Close"); f.GUI.MessageFormVisible = false; };
            f.GUI.DisplayBattleMessageHandler = (pname, msg, mode) => { log.Add("Msg:" + pname + ":" + msg); };

            // SetMessage 相当でメッセージを定義する
            var vname = "Message(" + p.ID + "," + sname + ")";
            f.Src.Expression.DefineLocalVariable(vname);
            f.Src.Expression.SetVariableAsString(vname, "いくぞ！");
            return (u, log);
        }

        [TestMethod]
        public void UseSpecialPower_DefinedMessage_ShownInOwnMessageFormBeforeEffect()
        {
            // 使用メッセージが定義されていればメッセージウィンドウを開いて表示し、閉じてからアニメ (ここでは使用表示) に進む
            var f = new SpecialPowerTestFixture();
            var (u, log) = SetupMessage(f, "気合", "気力増加Lv1");
            var p = u.MainPilot();

            p.UseSpecialPower("気合");

            // 最初にメッセージ用ウィンドウを開いて台詞を表示し、閉じてから使用表示のウィンドウを開く
            CollectionAssert.AreEqual(new[] { "Open", "Msg:" + p.ID + ":いくぞ！", "Close", "Open" }, log.Take(4).ToArray(), string.Join(",", log));
            Assert.AreEqual(1, log.Count(x => x.StartsWith("Msg:")));
            Assert.AreEqual("Close", log.Last(), "最後にメッセージウィンドウを閉じる");
            Assert.AreEqual(110, p.Morale);
        }

        [TestMethod]
        public void UseSpecialPower_MessageSuppressed_ForSPMod2AndReviveOrSelfDestruct()
        {
            // sp_mod = 2 の場合、復活・自爆の効果を持つ場合は使用メッセージを表示しない
            {
                var f = new SpecialPowerTestFixture();
                var (u, log) = SetupMessage(f, "気合", "気力増加Lv1");
                u.MainPilot().UseSpecialPower("気合", 2d);
                Assert.IsFalse(log.Any(x => x.StartsWith("Msg:")), "sp_mod = 2");
            }
            {
                var f = new SpecialPowerTestFixture();
                var (u, log) = SetupMessage(f, "根性", "ＨＰ回復Lv1 復活");
                u.HP = 100;
                u.MainPilot().UseSpecialPower("根性");
                Assert.IsFalse(log.Any(x => x.StartsWith("Msg:")), "復活");
            }
        }
    }
}
