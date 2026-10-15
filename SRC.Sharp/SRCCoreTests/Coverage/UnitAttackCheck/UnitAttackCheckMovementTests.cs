using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// 強制移動・変身系の判定（VB: Unit.cls CheckBlowAttack / CheckDrawAttack /
    /// CheckTeleportAwayAttack / CheckMetamorphAttack）と IsSameCategory のテスト。
    /// 攻撃側を「特殊効果発動」効果下に置いて発生判定を100%に固定している。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckMovementTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

標的
標的, ひょうてき, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

障害
障害, しょうがい, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid
";

        private const string Units = @"
攻撃機
攻撃機, こうげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
吹き飛ばし拳, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突吹L2
引き寄せ鎖, 1500, 1, 5, +0, -, -, -, AAAA, +0, 実引
転移光線, 1500, 1, 5, +0, -, -, -, AAAA, +0, Ｂ転L1
体当たり, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突Ｋ
コピー光線, 1500, 1, 5, +0, -, -, -, AAAA, +0, Ｂ写

小型攻撃機
小型攻撃機, こがたこうげきき, 汎用, 1, 2
陸, 4, SS, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
体当たり, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突Ｋ
コピー光線, 1500, 1, 5, +0, -, -, -, AAAA, +0, Ｂ写

標的機
標的機, ひょうてきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

巨大機
巨大機, きょだいき, 汎用, 1, 2
陸, 4, XL, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

砦
砦, とりで, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
地形ユニット
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        private static UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.FillPlainTerrain();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            return f;
        }

        private static UnitWeapon WeaponOf(Unit u, string name)
        {
            return u.Weapons.First(w => w.Name == name);
        }

        private static Unit Attacker(UnitAttackCheckFixture f, int x, int y, string uname = "攻撃機")
        {
            var a = f.Place(uname, "攻撃者", "味方", x, y);
            f.ApplySpecialPower(a, "発動テスト", "特殊効果発動");
            return a;
        }

        // ──────────────────────────────────────────────
        // 吹き飛ばし
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Blow_CriticalAddsOneSquareAndPushesAway()
        {
            // 吹L2 + クリティカルで3マス、攻撃側から遠ざかる方向に吹き飛ばす
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("標的機", "標的", "敵", 3, 5);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            var result = a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct);

            Assert.IsTrue(result);
            Assert.AreEqual(6, t.x);
            Assert.AreEqual(5, t.y);
            Assert.AreSame(t, f.Src.Map.MapDataForUnit[6, 5]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[3, 5]);
            Assert.AreEqual(1500, dmg);
            Assert.AreEqual("クリティカル！ 標的機を吹き飛ばした。;", msg);
            Assert.AreEqual(" 吹き飛ばし", ct);
        }

        [TestMethod]
        public void Blow_VerticalDirection()
        {
            // 縦方向に離れていれば縦に吹き飛ばす
            var f = CreateFixture();
            var a = Attacker(f, 5, 5);
            var t = f.Place("標的機", "標的", "敵", 5, 4);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            Assert.IsTrue(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(5, t.x);
            Assert.AreEqual(1, t.y);
        }

        [TestMethod]
        public void Blow_StopsAtMapEdgeWithoutCrash()
        {
            // マップ端で止まる（激突扱いにはならない）
            var f = CreateFixture();
            var a = Attacker(f, 8, 5);
            var t = f.Place("標的機", "標的", "敵", 9, 5);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            Assert.IsTrue(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(10, t.x);
            Assert.AreEqual(1500, dmg);
        }

        [TestMethod]
        public void Blow_CrashIntoUnit_AddsDamageAndHurtsBystander()
        {
            // ユニットに激突: 目標に (dmg − 装甲×気力/100)/2 の追加ダメージ、巻き添えにも同式のダメージ
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("標的機", "標的", "敵", 3, 5);
            var b = f.Place("標的機", "障害", "敵", 5, 5);
            var bhp = b.HP;
            var dmg = 1500;
            var msg = "";
            var ct = "";
            var tArmorTerm = t.get_Armor("") * t.MainPilot().Morale / 100;
            var bArmorTerm = b.get_Armor("") * b.MainPilot().Morale / 100;

            Assert.IsTrue(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(4, t.x);
            Assert.AreEqual(1500 + System.Math.Max((1500 - tArmorTerm) / 2, 0), dmg);
            var dmg2 = System.Math.Max((1500 - bArmorTerm) / 2, 10);
            Assert.AreEqual(bhp - dmg2, b.HP);
            Assert.IsTrue(f.Gui.CallsOf(nameof(IGUI.DrawSysString)).Any(c => (string)c.Args[2] == dmg2.ToString()));
        }

        [TestMethod]
        public void Blow_CrashMinimumDamage()
        {
            // 激突時の最低ダメージは10（防御時は5）
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("標的機", "標的", "敵", 3, 5);
            f.Place("標的機", "障害", "敵", 4, 5);
            var dmg = 0;
            var msg = "";
            var ct = "";
            Assert.IsTrue(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(3, t.x);
            Assert.AreEqual(10, dmg);

            dmg = 0;
            Assert.IsTrue(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "防御", ref ct));
            Assert.AreEqual(5, dmg);
        }

        [TestMethod]
        public void Blow_XLTarget_NotMoved()
        {
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("巨大機", "標的", "敵", 3, 5);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(3, t.x);
            Assert.AreEqual("", ct);
        }

        [TestMethod]
        public void Blow_TerrainUnit_NotMoved()
        {
            // 地形ユニットは動かせない
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("砦", "標的", "敵", 3, 5);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CheckBlowAttack(WeaponOf(a, "吹き飛ばし拳"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(3, t.x);
        }

        [TestMethod]
        public void Blow_KnockbackBySmallAttacker_LimitedBySize()
        {
            // Ｋ属性: SS サイズの攻撃側は SS/S の相手しか弾き飛ばせない
            var f = CreateFixture();
            var a = Attacker(f, 2, 5, "小型攻撃機");
            var t = f.Place("標的機", "標的", "敵", 3, 5);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CheckBlowAttack(WeaponOf(a, "体当たり"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(3, t.x);
        }

        [TestMethod]
        public void Blow_KnockbackWithoutLevel_MovesOneSquareOnCritical()
        {
            // Ｋ（レベルなし）は通常 0 マス、クリティカル時に1マス弾き飛ばす
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("標的機", "標的", "敵", 3, 5);
            var dmg = 1500;
            var msg = "";
            var ct = "";

            Assert.IsTrue(a.CheckBlowAttack(WeaponOf(a, "体当たり"), t, ref dmg, ref msg, "", "", ref ct));
            Assert.AreEqual(4, t.x);
        }

        // ──────────────────────────────────────────────
        // 引き寄せ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Draw_PullsTargetNextToAttacker()
        {
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("標的機", "標的", "敵", 6, 5);
            var msg = "";
            var ct = "";

            Assert.IsTrue(a.CheckDrawAttack(WeaponOf(a, "引き寄せ鎖"), t, ref msg, "", ref ct));
            Assert.AreEqual(3, t.x);
            Assert.AreEqual(5, t.y);
            Assert.AreEqual("標的機を引き寄せた。;", msg);
            Assert.AreEqual(" 引き寄せ", ct);
        }

        [TestMethod]
        public void Draw_AlreadyAdjacent_DoesNothing()
        {
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("標的機", "標的", "敵", 3, 5);
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CheckDrawAttack(WeaponOf(a, "引き寄せ鎖"), t, ref msg, "", ref ct));
            Assert.AreEqual(3, t.x);
        }

        [TestMethod]
        public void Draw_XLTarget_NotMoved()
        {
            var f = CreateFixture();
            var a = Attacker(f, 2, 5);
            var t = f.Place("巨大機", "標的", "敵", 6, 5);
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CheckDrawAttack(WeaponOf(a, "引き寄せ鎖"), t, ref msg, "", ref ct));
            Assert.AreEqual(6, t.x);
        }

        // ──────────────────────────────────────────────
        // 強制転移
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TeleportAway_Level1_MovesExactlyOneSquare()
        {
            // 転L1 は x 方向に距離1だけ転移させる（方向は乱数）
            var f = CreateFixture();
            var a = Attacker(f, 1, 1);
            var t = f.Place("標的機", "標的", "敵", 5, 5);
            var msg = "";
            var ct = "";

            Assert.IsTrue(a.CheckTeleportAwayAttack(WeaponOf(a, "転移光線"), t, ref msg, "", ref ct));
            Assert.AreEqual(5, t.y);
            Assert.AreEqual(1, System.Math.Abs(t.x - 5));
            Assert.AreEqual("標的機をテレポートさせた。;", msg);
            Assert.AreEqual(" 強制転移", ct);
        }

        [TestMethod]
        public void TeleportAway_TerrainUnit_NotMoved()
        {
            var f = CreateFixture();
            var a = Attacker(f, 1, 1);
            var t = f.Place("砦", "標的", "敵", 5, 5);
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CheckTeleportAwayAttack(WeaponOf(a, "転移光線"), t, ref msg, "", ref ct));
            Assert.AreEqual(5, t.x);
        }

        // ──────────────────────────────────────────────
        // 能力コピー（早期判定）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Metamorph_AlreadyCopied_Fails()
        {
            // 既にノーマルモードを持つ（変身中の）ユニットはコピーできない
            var f = CreateFixture();
            var a = Attacker(f, 1, 1);
            a.AddCondition("ノーマルモード付加", -1, 1d, "攻撃機 手動解除可");
            a.Update();
            Assert.IsTrue(a.IsFeatureAvailable("ノーマルモード"));
            var t = f.Place("標的機", "標的", "敵", 2, 1);

            Assert.IsFalse(a.CheckMetamorphAttack(WeaponOf(a, "コピー光線"), t, ""));
        }

        [TestMethod]
        public void Metamorph_BossTarget_Fails()
        {
            var f = CreateFixture();
            var a = Attacker(f, 1, 1);
            var t = f.Place("標的機", "標的", "敵", 2, 1);
            t.BossRank = 0;

            Assert.IsFalse(a.CheckMetamorphAttack(WeaponOf(a, "コピー光線"), t, ""));
        }

        [TestMethod]
        public void Metamorph_SizeTooDifferent_Fails()
        {
            // 写属性: SS サイズは M 以上の相手をコピーできない
            var f = CreateFixture();
            var a = Attacker(f, 1, 1, "小型攻撃機");
            var t = f.Place("標的機", "標的", "敵", 2, 1);

            Assert.IsFalse(a.CheckMetamorphAttack(WeaponOf(a, "コピー光線"), t, ""));
            Assert.AreEqual(0, f.SysMessages().Length);
        }

        // ──────────────────────────────────────────────
        // IsSameCategory
        // ──────────────────────────────────────────────

        [DataTestMethod]
        [DataRow("バリア 全 10", "バリア 全 5", true)]
        [DataRow("バリアLv2 全 10", "バリアLv5", true)]
        [DataRow("バリア", "Ｉフィールド", false)]
        [DataRow("", "", true)]
        public void IsSameCategory_ComparesFirstElementIgnoringLevel(string fdata1, string fdata2, bool expected)
        {
            // 「中和」「相殺」用: 先頭要素（Lv指定を除く）が同じなら同種
            var f = CreateFixture();
            var u = f.Place("標的機", "標的", "敵", 1, 1);

            Assert.AreEqual(expected, u.IsSameCategory(fdata1, fdata2));
        }
    }
}
