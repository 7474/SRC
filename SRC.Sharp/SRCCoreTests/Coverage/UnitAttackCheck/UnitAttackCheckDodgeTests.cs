using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.CheckDodgeFeature（VB: Unit.cls CheckDodgeFeature）のテスト。
    /// 発動率が 100% か 0% になる条件だけを使い、乱数の影響を受けないようにしている。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckDodgeTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

回避者
回避者, かいひしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

分身使い
分身使い, ぶんしんつかい, 男性, 汎用, AAAA, 100
特殊能力
分身Lv16, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

阻止者
阻止者, そししゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

分身見習い
分身見習い, ぶんしんみならい, 男性, 汎用, AAAA, 100
特殊能力
分身Lv8, 1
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
ビーム, 1500, 1, 3, +0, -, -, -, AAAA, +0, Ｂ
無属性砲, 1500, 1, 3, +0, -, -, -, AAAA, +0, 無

残像機
残像機, ざんぞうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
超回避Lv10=残像 30 0
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

手動残像機
手動残像機, しゅどうざんぞうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
超回避Lv10=残像 0 0 手動
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

高気力残像機
高気力残像機, こうきりょくざんぞうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
超回避Lv10=残像 0 130
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

分身機
分身機, ぶんしんき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
分身
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

阻止機
阻止機, そしき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
広域阻止Lv1=阻止フィールド 2 全 100 20
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

弱阻止機
弱阻止機, じゃくそしき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
広域阻止Lv2=阻止フィールド 2 全 100 20
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

普通機
普通機, ふつうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        private UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            return f;
        }

        // ──────────────────────────────────────────────
        // 超回避
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SuperDodge_Level10_AlwaysDodgesAndConsumesEN()
        {
            // 超回避Lv10 は発動率 10/10。ＥＮ 30 を消費して回避し、メッセージを出す
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("残像機", "回避者", "敵", 3, 1);
            var en = t.EN;

            var result = a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false);

            Assert.IsTrue(result);
            Assert.AreEqual(en - 30, t.EN);
            CollectionAssert.Contains(f.SysMessages(), "残像機は[残像]を使って攻撃をかわした。");
        }

        [TestMethod]
        public void SuperDodge_NotEnoughEN_DoesNotDodge()
        {
            // 消費ＥＮが足りなければ発動しない（ＥＮも減らない）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("残像機", "回避者", "敵", 3, 1);
            t.EN = 29;

            var result = a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false);

            Assert.IsFalse(result);
            Assert.AreEqual(29, t.EN);
            Assert.AreEqual(0, f.SysMessages().Length);
        }

        [TestMethod]
        public void SuperDodge_ManualOption_OnlyWhenDefModeIsDodge()
        {
            // 「手動」指定時は def_mode が「回避」の場合のみ発動する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("手動残像機", "回避者", "敵", 3, 1);

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "防御", 1500, false));
            Assert.IsTrue(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "回避", 1500, false));
        }

        [TestMethod]
        public void SuperDodge_MoraleRequirement()
        {
            // 必要気力 130 に満たなければ発動しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("高気力残像機", "回避者", "敵", 3, 1);
            t.MainPilot().Morale = 129;
            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));

            t.MainPilot().Morale = 130;
            Assert.IsTrue(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
        }

        [TestMethod]
        public void AttackerUnderSureHit_DisablesDodge()
        {
            // 攻撃側が「絶対命中」効果下なら回避能力は無効
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("残像機", "回避者", "敵", 3, 1);
            f.ApplySpecialPower(a, "必中テスト", "絶対命中");
            var en = t.EN;

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        public void AttackerUnderSureHit_TargetWithSpecialDefense_StillDodges()
        {
            // 防御側が「特殊防御発動」効果下なら絶対命中でも回避判定を行う
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("残像機", "回避者", "敵", 3, 1);
            f.ApplySpecialPower(a, "必中テスト", "絶対命中");
            f.ApplySpecialPower(t, "見切りテスト", "特殊防御発動");

            Assert.IsTrue(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
        }

        [TestMethod]
        public void TargetDefenseless_CannotDodge()
        {
            // 「無防備」状態では能動防御は発動しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("残像機", "回避者", "敵", 3, 1);
            f.ApplySpecialPower(t, "無防備テスト", "無防備");

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
        }

        // ──────────────────────────────────────────────
        // 分身（ユニット用特殊能力）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitDoubleImage_RequiresMorale130()
        {
            // ユニット用「分身」は気力130以上が必要（特殊防御発動下なら必ず発動）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("分身機", "回避者", "敵", 3, 1);
            f.ApplySpecialPower(t, "見切りテスト", "特殊防御発動");

            t.MainPilot().Morale = 129;
            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));

            t.MainPilot().Morale = 130;
            Assert.IsTrue(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
            CollectionAssert.Contains(f.SysMessages(), "分身機は分身して攻撃をかわした。");
        }

        // ──────────────────────────────────────────────
        // 分身（パイロット用特殊能力）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotDoubleImage_Level16_AgainstNoSkill_AlwaysDodges()
        {
            // 発動率 = 2 × 防御側Lv − 攻撃側Lv = 32 / 32
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "分身使い", "敵", 3, 1);

            Assert.IsTrue(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
            CollectionAssert.Contains(f.SysMessages(), "普通機は分身して攻撃をかわした。");
        }

        [TestMethod]
        public void PilotDoubleImage_CancelledByAttackerSkill()
        {
            // 防御側Lv8・攻撃側Lv16 なら 2×8−16 = 0 で発動しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "分身使い", "味方", 1, 1);
            var t = f.Place("普通機", "分身見習い", "敵", 3, 1);

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
        }

        // ──────────────────────────────────────────────
        // 広域阻止
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AreaBlock_AllyWithinRange_BlocksAttackAndConsumesEN()
        {
            // 目標の味方が「広域阻止」を持ち、目標が有効範囲内・攻撃側が範囲外なら阻止する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "回避者", "敵", 4, 1);
            var blocker = f.Place("阻止機", "阻止者", "敵", 5, 1);
            var en = blocker.EN;

            var result = a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false);

            Assert.IsTrue(result);
            Assert.AreEqual(en - 20, blocker.EN);
            CollectionAssert.Contains(f.SysMessages(), "阻止機は[阻止フィールド]で[ビーム]を防いだ。");
        }

        [TestMethod]
        public void AreaBlock_AttackerInsideRange_DoesNotBlock()
        {
            // 攻撃側が阻止ユニットの有効範囲内にいる場合は阻止できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 3, 1);
            var t = f.Place("普通機", "回避者", "敵", 4, 1);
            var blocker = f.Place("阻止機", "阻止者", "敵", 5, 1);
            var en = blocker.EN;

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
            Assert.AreEqual(en, blocker.EN);
        }

        [TestMethod]
        public void AreaBlock_NoAttributeWeapon_IsNotBlocked()
        {
            // 無属性武器には阻止が効かない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "回避者", "敵", 4, 1);
            f.Place("阻止機", "阻止者", "敵", 5, 1);

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(2), t, t.x, t.y, "", "", 1500, false));
        }

        [TestMethod]
        public void AreaBlock_Level2_DamageLimitIs1000()
        {
            // レベル指定時は 500×Lv を超えるダメージは阻止できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "回避者", "敵", 4, 1);
            f.Place("弱阻止機", "阻止者", "敵", 5, 1);

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1001, false));
            Assert.IsTrue(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1000, false));
        }

        [TestMethod]
        public void AreaBlock_EnemyOfTarget_DoesNotBlock()
        {
            // 目標の敵ユニット（攻撃側の味方）は阻止してくれない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "回避者", "敵", 4, 1);
            f.Place("阻止機", "阻止者", "味方", 5, 1);

            Assert.IsFalse(a.CheckDodgeFeature(a.Weapon(1), t, t.x, t.y, "", "", 1500, false));
            Assert.IsFalse(f.SysMessages().Any(m => m.Contains("防いだ")));
        }
    }
}
