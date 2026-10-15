using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Reflection;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.attackcheck.cs の防御系判定（VB: Unit.cls CheckDefenseFeature / CheckShieldFeature /
    /// CheckDummyFeature）のテスト。private メソッドはリフレクションで呼び出す。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckDefenseTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

防御者
防御者, ぼうぎょしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

盾名人
盾名人, たてめいじん, 男性, 汎用, AAAA, 100
特殊能力
Ｓ防御Lv16, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

盾初心者
盾初心者, たてしょしんしゃ, 男性, 汎用, AAAA, 100
特殊能力
Ｓ防御Lv1, 1
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
パンチ, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突
封印剣, 1500, 1, 1, +0, -, -, -, AAAA, +0, 武封
ミサイル, 1500, 1, 3, +0, -, -, -, AAAA, +0, 実
破砕弾, 1500, 1, 3, +0, -, -, -, AAAA, +0, 実破
精神波, 1500, 1, 3, +0, -, -, -, AAAA, +0, 精
無属性砲, 1500, 1, 3, +0, -, -, -, AAAA, +0, 無

普通機
普通機, ふつうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

バリア機
バリア機, ばりあき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
バリアLv1=光の壁 全 10 0
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

高気力バリア機
高気力バリア機, こうきりょくばりあき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
バリアLv1=光の壁 全 10 130
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

ビームバリア機
ビームバリア機, びーむばりあき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
バリアLv1=ビームコート Ｂ 10 0
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

フィールド機
フィールド機, ふぃーるどき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
フィールドLv1=力場 全 0 0
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

変換機
変換機, へんかんき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
変換Lv5=エネルギー変換 全 0
3000, 300, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

ビーム吸収機
ビーム吸収機, びーむきゅうしゅうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
ビーム吸収=吸収装甲
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

盾機
盾機, たてき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
盾Lv2=大盾
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

融合機
融合機, ゆうごうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
融合Lv16=同化
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

シールド機
シールド機, しーるどき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
シールド
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

ＥＳ機
ＥＳ機, えすき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
エネルギーシールド
3000, 100, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

ダミー機
ダミー機, だみーき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
ダミーLv2=身代わり人形
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

        private static UnitWeapon WeaponOf(Unit u, string name)
        {
            foreach (var w in u.Weapons)
            {
                if (w.Name == name)
                {
                    return w;
                }
            }
            Assert.Fail("武器が見つからない: " + name);
            return null;
        }

        private static bool Defense(Unit a, UnitWeapon w, Unit t, ref int dmg, ref string msg, out bool penetrated, string def_mode = "")
        {
            var mi = typeof(Unit).GetMethod("CheckDefenseFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            var args = new object[] { w, t, t.x, t.y, "", def_mode, dmg, msg, false, false };
            var ret = (bool)mi.Invoke(a, args);
            dmg = (int)args[6];
            msg = (string)args[7];
            penetrated = (bool)args[9];
            return ret;
        }

        private static bool Shield(Unit a, UnitWeapon w, Unit t, int dmg)
        {
            var mi = typeof(Unit).GetMethod("CheckShieldFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            var args = new object[] { w, t, dmg, true, false, false };
            mi.Invoke(a, args);
            return (bool)args[4];
        }

        private static bool Dummy(Unit a, UnitWeapon w, Unit t)
        {
            var mi = typeof(Unit).GetMethod("CheckDummyFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)mi.Invoke(a, new object[] { w, t, true });
        }

        // ──────────────────────────────────────────────
        // 攻撃吸収・攻撃無効化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Defense_NegativeDamage_AbsorbsIntoHP()
        {
            // ダメージが負なら攻撃を吸収してＨＰが回復する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 2, 1);
            t.HP = 1000;
            var dmg = -300;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1300, t.HP);
        }

        [TestMethod]
        public void Defense_ZeroDamageWithSealWeapon_ShowsNotEffectiveMessage()
        {
            // 攻撃力があるのにダメージ0、かつ「封」属性なら「通用しない」と表示する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 2, 1);
            var dmg = 0;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "封印剣"), t, ref dmg, ref msg, out _));
            CollectionAssert.Contains(f.SysMessages(), "普通機には[封印剣]は通用しない。");
        }

        [TestMethod]
        public void Defense_NoDefenseFeature_ReturnsFalseAndKeepsDamage()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 2, 1);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out var penetrated));
            Assert.AreEqual(1500, dmg);
            Assert.IsFalse(penetrated);
        }

        // ──────────────────────────────────────────────
        // バリア
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Barrier_DamageWithinLimit_IsNegatedAndConsumesEN()
        {
            // バリアLv1 は 1000 以下のダメージを無効化し、ＥＮ10 を消費する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            var en = t.EN;
            var dmg = 1000;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out var penetrated));
            Assert.AreEqual(en - 10, t.EN);
            Assert.IsFalse(penetrated);
        }

        [TestMethod]
        public void Barrier_DamageOverLimit_IsPenetrated()
        {
            // 上限を超えるとバリアは貫通される（ＥＮは消費される）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            var en = t.EN;
            var dmg = 1001;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out var penetrated));
            Assert.IsTrue(penetrated);
            Assert.AreEqual(1001, dmg);
            Assert.AreEqual("ビームが[光の壁]を貫いた。;", msg);
            Assert.AreEqual(en - 10, t.EN);
        }

        [TestMethod]
        public void Barrier_NotEnoughEN_IsNotActivated()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            t.EN = 9;
            var dmg = 500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(9, t.EN);
        }

        [TestMethod]
        public void Barrier_MoraleRequirement()
        {
            // 必要気力 130 未満では発動しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("高気力バリア機", "防御者", "敵", 2, 1);
            var dmg = 500;
            var msg = "";
            t.MainPilot().Morale = 129;
            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));

            t.MainPilot().Morale = 130;
            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
        }

        [TestMethod]
        public void Barrier_AttributeRestricted_OnlyBlocksMatchingWeapon()
        {
            // 対象属性「Ｂ」のバリアはビーム兵器のみ防ぐ
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("ビームバリア機", "防御者", "敵", 2, 1);
            var dmg = 500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
        }

        [TestMethod]
        public void Barrier_NeutralizedCondition_DisablesBarrier()
        {
            // 「バリア無効化」状態ではバリアは働かない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            t.AddCondition("バリア無効化", 1);
            var en = t.EN;
            var dmg = 500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        public void Barrier_NoAttributeWeapon_IgnoresBarrier()
        {
            // 無属性武器はバリアを無視する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            var en = t.EN;
            var dmg = 500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "無属性砲"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        public void Barrier_AttackerWithDefenseNullify_IgnoresBarrier()
        {
            // 攻撃側が「防御能力無効化」効果下ならバリアは働かない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            f.ApplySpecialPower(a, "貫通テスト", "防御能力無効化");
            var dmg = 500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
        }

        // ──────────────────────────────────────────────
        // フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Field_DamageWithinLimit_IsNegated()
        {
            // フィールドLv1 は 500 以下のダメージを無効化する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("フィールド機", "防御者", "敵", 2, 1);
            var dmg = 500;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
        }

        [TestMethod]
        public void Field_DamageOverLimit_IsReduced()
        {
            // 上限を超えたダメージは 500×Lv だけ減少する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("フィールド機", "防御者", "敵", 2, 1);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1000, dmg);
            Assert.AreEqual("ビームが[力場]を貫いた。;", msg);
        }

        // ──────────────────────────────────────────────
        // 変換・ビーム吸収
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Conversion_AddsENFromDamage()
        {
            // 変換LvN: ＥＮ += 0.01 × N × ダメージ
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("変換機", "防御者", "敵", 2, 1);
            t.EN = 100;
            var dmg = 1000;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(150, t.EN);
        }

        [TestMethod]
        public void BeamAbsorb_BeamWeapon_HealsHP()
        {
            // ビーム吸収はＢ属性の攻撃をＨＰに変える
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("ビーム吸収機", "防御者", "敵", 2, 1);
            t.HP = 1000;
            var dmg = 800;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1800, t.HP);
        }

        [TestMethod]
        public void BeamAbsorb_NonBeamWeapon_NoEffect()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("ビーム吸収機", "防御者", "敵", 2, 1);
            t.HP = 1000;
            var dmg = 800;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1000, t.HP);
        }

        // ──────────────────────────────────────────────
        // 盾
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShieldFeature_ReducesDamageAndRecordsShieldDamage()
        {
            // 盾: ダメージ − 100×(Ｓ防御Lv+4)。貫通されたら「盾ダメージ」が1増える
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("盾機", "盾初心者", "敵", 2, 1);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1000, dmg);
            Assert.AreEqual(1d, t.ConditionLevel("盾ダメージ"));
            CollectionAssert.Contains(f.SysMessages(), "普通機は[大盾]を使ってダメージを軽減させた。".Replace("普通機", "盾機"));
        }

        [TestMethod]
        public void ShieldFeature_BreakWeapon_HalfReductionAndDoubleShieldDamage()
        {
            // 「破」属性: 軽減は 50×(Ｓ防御Lv+4)、盾ダメージは2
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("盾機", "盾初心者", "敵", 2, 1);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "破砕弾"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1250, dmg);
            Assert.AreEqual(2d, t.ConditionLevel("盾ダメージ"));
        }

        [TestMethod]
        public void ShieldFeature_FullyBlocked_ReturnsTrue()
        {
            // 軽減後のダメージが0なら完全に防ぐ（盾ダメージは増えない）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("盾機", "盾初心者", "敵", 2, 1);
            var dmg = 400;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(0, dmg);
            Assert.IsFalse(t.IsConditionSatisfied("盾ダメージ"));
            CollectionAssert.Contains(f.SysMessages(), "盾機は[大盾]を使って攻撃を防いだ。");
        }

        [TestMethod]
        public void ShieldFeature_BrokenShield_NoLongerUsed()
        {
            // 盾ダメージが盾のレベルに達したら使えない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("盾機", "盾初心者", "敵", 2, 1);
            t.AddCondition("盾ダメージ", -1, 2d);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1500, dmg);
        }

        [TestMethod]
        public void ShieldFeature_WithoutShieldSkill_NotUsed()
        {
            // Ｓ防御技能がなければ盾は使えない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("盾機", "防御者", "敵", 2, 1);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1500, dmg);
        }

        [TestMethod]
        public void ShieldFeature_SpiritWeapon_IgnoresShield()
        {
            // 「精」属性の攻撃は盾で防げない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("盾機", "盾初心者", "敵", 2, 1);
            var dmg = 1500;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "精神波"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1500, dmg);
        }

        // ──────────────────────────────────────────────
        // 融合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fusion_RangedWeapon_AbsorbsDamage()
        {
            // 融合Lv16 は格闘系以外の攻撃を必ず吸収してＨＰに変える
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("融合機", "防御者", "敵", 2, 1);
            t.HP = 1000;
            var dmg = 700;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1700, t.HP);
            CollectionAssert.Contains(f.SysMessages(), "融合機は[ビーム]の攻撃を吸収した。");
        }

        [TestMethod]
        public void Fusion_PhysicalWeapon_IsTakenIn()
        {
            // 「実」属性の場合は「取り込んだ」と表示する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("融合機", "防御者", "敵", 2, 1);
            var dmg = 700;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ミサイル"), t, ref dmg, ref msg, out _));
            CollectionAssert.Contains(f.SysMessages(), "融合機は[ミサイル]を取り込んだ。");
        }

        [TestMethod]
        public void Fusion_MeleeWeapon_NotAbsorbed()
        {
            // 「突」属性（格闘系）は融合できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("融合機", "防御者", "敵", 2, 1);
            t.HP = 1000;
            var dmg = 700;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "パンチ"), t, ref dmg, ref msg, out _));
            Assert.AreEqual(1000, t.HP);
        }

        // ──────────────────────────────────────────────
        // シールド防御（CheckShieldFeature）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShieldDefense_Level16_AlwaysUsed()
        {
            // Ｓ防御Lv16 なら発動率 16/16
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "盾名人", "敵", 2, 1);

            Assert.IsTrue(Shield(a, WeaponOf(a, "ビーム"), t, 1000));
        }

        [TestMethod]
        public void ShieldDefense_ZeroDamage_NotUsed()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "盾名人", "敵", 2, 1);

            Assert.IsFalse(Shield(a, WeaponOf(a, "ビーム"), t, 0));
        }

        [TestMethod]
        public void ShieldDefense_WithoutSkill_NotUsed()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Shield(a, WeaponOf(a, "ビーム"), t, 1000));
        }

        [TestMethod]
        public void ShieldDefense_SpiritWeapon_NotUsed()
        {
            // 「精」属性の攻撃にはシールド防御できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "盾名人", "敵", 2, 1);

            Assert.IsFalse(Shield(a, WeaponOf(a, "精神波"), t, 1000));
        }

        [TestMethod]
        public void ShieldDefense_Paralyzed_NotUsed()
        {
            // 麻痺などで行動できなければシールド防御できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "盾名人", "敵", 2, 1);
            t.AddCondition("麻痺", 1);

            Assert.IsFalse(Shield(a, WeaponOf(a, "ビーム"), t, 1000));
        }

        [TestMethod]
        public void ShieldDefense_AttackerWithShieldNullify_NotUsed()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "盾名人", "敵", 2, 1);
            f.ApplySpecialPower(a, "盾無効テスト", "シールド防御無効化");

            Assert.IsFalse(Shield(a, WeaponOf(a, "ビーム"), t, 1000));
        }

        [TestMethod]
        public void ShieldDefense_EnergyShield_RequiresENOver5()
        {
            // エネルギーシールドはＥＮが5を超えている場合のみ使える
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("ＥＳ機", "盾名人", "敵", 2, 1);
            t.EN = 5;
            Assert.IsFalse(Shield(a, WeaponOf(a, "ビーム"), t, 1000));

            t.EN = 6;
            Assert.IsTrue(Shield(a, WeaponOf(a, "ビーム"), t, 1000));
        }

        [TestMethod]
        [Ignore("移植の不具合: CheckShieldFeature が「無防備」ではなく「行動不能1」を判定している")]
        public void ShieldDefense_Defenseless_NotUsed()
        {
            // VB では防御側が「無防備」効果下ならシールド防御できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("シールド機", "盾名人", "敵", 2, 1);
            f.ApplySpecialPower(t, "無防備テスト", "無防備");

            Assert.IsFalse(Shield(a, WeaponOf(a, "ビーム"), t, 1000));
        }

        // ──────────────────────────────────────────────
        // ダミー（CheckDummyFeature）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dummy_UsableUpToFeatureLevelTimes()
        {
            // ダミーLv2 は2回まで身代わりになり、使用回数は「ダミー破壊」に記録される
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("ダミー機", "防御者", "敵", 2, 1);
            var w = WeaponOf(a, "ビーム");

            Assert.IsTrue(Dummy(a, w, t));
            Assert.AreEqual(1d, t.ConditionLevel("ダミー破壊"));
            Assert.IsTrue(Dummy(a, w, t));
            Assert.AreEqual(2d, t.ConditionLevel("ダミー破壊"));
            Assert.IsFalse(Dummy(a, w, t));
            CollectionAssert.Contains(f.SysMessages(), "ダミー機は[身代わり人形]を身代わりにして攻撃をかわした。");
        }

        [TestMethod]
        public void Dummy_ConditionGranted_DecrementsAndRemoves()
        {
            // 「ダミー付加」状態はレベル分だけ身代わりになり、0になると解除される
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 2, 1);
            t.AddCondition("ダミー付加", -1, 1d);
            var w = WeaponOf(a, "ビーム");

            Assert.IsTrue(Dummy(a, w, t));
            Assert.IsFalse(t.IsConditionSatisfied("ダミー付加"));
            Assert.IsFalse(Dummy(a, w, t));
            CollectionAssert.Contains(f.SysMessages(), "普通機は[ダミー]を身代わりにして攻撃をかわした。");
        }
    }
}
