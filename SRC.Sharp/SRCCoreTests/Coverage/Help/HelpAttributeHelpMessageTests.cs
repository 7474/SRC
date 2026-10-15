using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help.AttributeHelpMessage（VB: Help.bas AttributeHelpMessage）のテスト。
    /// 武器・アビリティ属性の解説文を、属性レベルや武器構成から組み立てる規則を確認する。
    /// </summary>
    [TestClass]
    public class HelpAttributeHelpMessageTests
    {
        // 属性 cls を持つ武器1つのユニットで、属性 atr の解説を得る
        private static string WeaponAttr(string atr, string cls = null)
        {
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", cls ?? atr, bullet: 5);
            var u = fx.Build();
            return fx.Src.Help.AttributeHelpMessage(u, atr, 1, false);
        }

        [TestMethod]
        public void BasicAttackTypes()
        {
            Assert.AreEqual("パイロットの格闘を使って攻撃力を算出。", WeaponAttr("格"));
            Assert.AreEqual("パイロットの射撃を使って攻撃力を算出。", WeaponAttr("射"));
            Assert.AreEqual("格闘と射撃の両方を使った攻撃。パイロットの格闘と射撃の平均値を使って攻撃力を算出する。", WeaponAttr("複"));
        }

        [TestMethod]
        public void Kai_DefaultVsLevel()
        {
            // 改: レベル指定なしは半分、指定ありは 10×Lv％
            Assert.AreEqual("ユニットランクによる攻撃力上昇が通常の半分。", WeaponAttr("改"));
            Assert.AreEqual("ユニットランクによる攻撃力上昇が50％になる。", WeaponAttr("改L5"));
        }

        [TestMethod]
        public void R_AlwaysHalf()
        {
            // VB でもレベルに関わらず最後に「通常の半分」で上書きされる
            Assert.AreEqual("ユニットランクや特殊能力による攻撃力上昇が通常の半分。", WeaponAttr("ＲL5"));
        }

        [TestMethod]
        public void Penetrate_DefaultVsLevel()
        {
            Assert.AreEqual("相手の装甲を半分とみなしてダメージ計算を行う。", WeaponAttr("貫"));
            Assert.AreEqual("相手の装甲を本来の70％の値とみなしてダメージ計算を行う。", WeaponAttr("貫L3"));
        }

        [TestMethod]
        public void Petrify_DefaultVsTurns()
        {
            Assert.AreEqual("クリティカル発生時に相手を石化させる。", WeaponAttr("石"));
            Assert.AreEqual("クリティカル発生時に相手を２ターン石化させる。", WeaponAttr("石L2"));
        }

        [TestMethod]
        public void Paralyze_DefaultIsThreeTurns()
        {
            Assert.AreEqual("クリティカル発生時に相手を３ターン麻痺させる。", WeaponAttr("痺"));
        }

        [TestMethod]
        public void Knockback_DefaultVsLevel()
        {
            Assert.AreEqual("クリティカル発生時に相手ユニットを１マス吹き飛ばす。", WeaponAttr("吹"));
            Assert.AreEqual("相手ユニットを２マス吹き飛ばす。;クリティカル発生時は吹き飛ばし距離＋１。", WeaponAttr("吹L2"));
        }

        [TestMethod]
        public void MultiHit_UsesLevel()
        {
            Assert.AreEqual(
                "3回連続して攻撃を行う。;攻撃によって与えるダメージは下記の式で計算される。;  通常のダメージ量 × 命中回数 ／ 攻撃回数",
                WeaponAttr("連L3"));
        }

        [TestMethod]
        public void Decay_LevelSelectsFraction()
        {
            Assert.AreEqual("クリティカル発生時に敵のＨＰを現在値の 1/2 まで減少させる。", WeaponAttr("衰L2"));
            Assert.AreEqual("クリティカル発生時に敵のＥＮを現在値の 1/4 まで減少させる。", WeaponAttr("滅L3"));
        }

        [TestMethod]
        public void LevelBasedCosts()
        {
            // 再: 100×Lv\16 %、難: 10×Lv %、気: 5×Lv
            Assert.AreEqual("25%の確率で再攻撃。", WeaponAttr("再L4"));
            Assert.AreEqual("30%の確率で失敗する。", WeaponAttr("難L3"));
            Assert.AreEqual("使用時に気力10を消費。", WeaponAttr("気L2"));
        }

        [TestMethod]
        public void HpLoss_UsesMaxHp()
        {
            // 失: Lv×最大ＨＰ\10（最大ＨＰ 3000）
            Assert.AreEqual("使用時に600のＨＰを失う。", WeaponAttr("失L2"));
        }

        [TestMethod]
        public void LevelTarget_WideDigit()
        {
            Assert.AreEqual("相手のメインパイロットのレベルが５の倍数の場合にのみ有効な攻撃。", WeaponAttr("対L5"));
        }

        [TestMethod]
        public void MapAttackShapes()
        {
            Assert.AreEqual("上下左右の一方向に対する直線状の効果範囲を持つ。", WeaponAttr("Ｍ直"));
            Assert.AreEqual("上下左右の一方向に対する扇状の効果範囲を持つ。;扇の広がり方の度合いはレベルによって異なる。", WeaponAttr("Ｍ扇"));
            Assert.AreEqual("指定した地点とユニットを結ぶ直線が効果範囲になる。", WeaponAttr("Ｍ線"));
        }

        [TestMethod]
        public void Charge_WithoutLinkedWeapons()
        {
            Assert.AreEqual("使用すると2ターン後に再チャージが完了するまで使用不能。", WeaponAttr("ＡL2"));
        }

        [TestMethod]
        public void Charge_SameNameWeapon_IsLinked()
        {
            // 同名の武器があれば連動して使用不能になる旨を付記
            var fx = new HelpTestFixture();
            fx.AddWeapon("ビーム", "ＡL2", bullet: 5);
            fx.AddWeapon("ビーム", "射", bullet: 5);
            var u = fx.Build();
            Assert.AreEqual(
                "使用すると2ターン後に再チャージが完了するまで使用不能。同名の武器も連動して使用不能になる。",
                fx.Src.Help.AttributeHelpMessage(u, "ＡL2", 1, false));
        }

        [TestMethod]
        public void Charge_SharedAmmoWithoutBullet_IsLinked()
        {
            // 共属性で弾数0の武器は同レベルの弾薬共有武器と連動
            var fx = new HelpTestFixture();
            fx.AddWeapon("ミサイル", "ＡL2共L1", bullet: 0);
            var u = fx.Build();
            Assert.AreEqual(
                "使用すると2ターン後に再チャージが完了するまで使用不能。同レベルの弾薬共有武器も連動して使用不能になる。",
                fx.Src.Help.AttributeHelpMessage(u, "ＡL2", 1, false));
        }

        [TestMethod]
        public void Charge_Ability_UsesAbilityWording()
        {
            var fx = new HelpTestFixture();
            fx.AddAbility("回復", "ＡL3", stock: 1);
            fx.AddAbility("回復", "", stock: 1);
            var u = fx.Build();
            Assert.AreEqual(
                "使用すると3ターン後に再チャージが完了するまで使用不能。同名のアビリティも連動して使用不能になる。",
                fx.Src.Help.AttributeHelpMessage(u, "ＡL3", 1, true));
        }

        [TestMethod]
        public void SharedAmmo_WeaponVsAbility()
        {
            Assert.AreEqual("複数の武器で弾薬を共有していることを示す。;同レベルの弾薬共有武器間で弾薬を共有している。", WeaponAttr("共L1"));

            var fx = new HelpTestFixture();
            fx.AddAbility("回復", "共L1", stock: 2);
            var u = fx.Build();
            Assert.AreEqual(
                "複数のアビリティで使用回数を共有していることを示す。;同レベルの使用回数共有アビリティ間で使用回数を共有している。",
                fx.Src.Help.AttributeHelpMessage(u, "共L1", 1, true));
        }

        [TestMethod]
        public void WeaknessEffectiveAndSeal_Prefixes()
        {
            // 弱・効・剋 は先頭文字で判定し、レベル省略時は３ターン
            Assert.AreEqual("クリティカル発生時に相手に火属性に対する弱点を３ターン付加する。", WeaponAttr("弱火"));
            Assert.AreEqual("クリティカル発生時に相手に光属性に対する有効を２ターン付加する。", WeaponAttr("効光L2"));
            Assert.AreEqual("クリティカル発生時に相手のオーラを３ターン使用不能にする。", WeaponAttr("剋オ"));
            Assert.AreEqual("クリティカル発生時に相手の火属性の武器、アビリティをその戦闘中のみ使用不能にする。", WeaponAttr("剋火L0"));
        }

        [TestMethod]
        public void Explanation_DefinedByFeature_OverridesText()
        {
            // 属性名と同名の「解説」能力があれば、第3要素以降で置き換える
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("格=解説 格闘技 \"拳で 殴る\"");
            fx.AddWeapon("パンチ", "格");
            var u = fx.Build();
            Assert.AreEqual("拳で 殴る", fx.Src.Help.AttributeHelpMessage(u, "格", 1, false));
        }

        [TestMethod]
        public void LifeSizeOption_ReplacesPilotWords()
        {
            // 等身大基準では「メインパイロット」「パイロット」を「ユニット」に
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", "格対L5");
            var u = fx.Build();
            fx.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            Assert.AreEqual("ユニットの格闘を使って攻撃力を算出。", fx.Src.Help.AttributeHelpMessage(u, "格", 1, false));
            Assert.AreEqual("相手ユニットのレベルが５の倍数の場合にのみ有効な攻撃。", fx.Src.Help.AttributeHelpMessage(u, "対L5", 1, false));
        }

        [TestMethod]
        public void DistanceOption_AddsNoteToSolidAmmo()
        {
            // 距離修正オプションで実弾属性の説明が追加される
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", "実");
            var u = fx.Build();
            fx.Src.Expression.DefineGlobalVariable("Option(距離修正)");
            Assert.AreEqual("切り払いと迎撃の対象になる。長距離の敵を攻撃する際もダメージが低下しない。", fx.Src.Help.AttributeHelpMessage(u, "実", 1, false));
        }
    }
}
