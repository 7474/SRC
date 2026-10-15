using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.UnitItem.UnitItemTestHelper;

namespace SRCCore.Coverage.Tests.UnitItem
{
    /// <summary>
    /// Item クラス（元の SRC の Item.cls）の判定ロジックのテスト
    /// </summary>
    [TestClass]
    public class UnitItemItemClassTests
    {
        // ──────────────────────────────────────────────
        // Class0: 専用指定の除去
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Class0_RemovesExclusiveSpecifier()
        {
            // 「剣(勇者専用)」の専用指定を取り除いて「剣」を返す（Item.cls Class0）
            var src = CreateSRC();
            var itm = CreateItem(src, "勇者の剣", "片手", "剣(勇者専用)");
            Assert.AreEqual("剣", itm.Class0());
            Assert.AreEqual("剣(勇者専用)", itm.Class());
        }

        [TestMethod]
        public void Class0_NestedParenthesesInsideSpecifier()
        {
            // 専用指定の中の括弧は対応を取って読み飛ばす
            var src = CreateSRC();
            var itm = CreateItem(src, "改造剣", "片手", "剣(ガンダム(改)専用)");
            Assert.AreEqual("剣", itm.Class0());
        }

        [TestMethod]
        public void Class0_WithoutSpecifier_ReturnsClassAsIs()
        {
            var src = CreateSRC();
            var itm = CreateItem(src, "普通の剣", "片手", "剣(改)");
            Assert.AreEqual("剣(改)", itm.Class0());
        }

        [TestMethod]
        public void Class0_KeepsParenthesesBeforeSpecifier()
        {
            // VB は専用指定の開き括弧を見つけた時点で Exit Do する。
            // クラス名自体に括弧がある「剣(改)(勇者専用)」は「剣(改)」になる。
            var src = CreateSRC();
            var itm = CreateItem(src, "勇者の改造剣", "片手", "剣(改)(勇者専用)");
            Assert.AreEqual("剣(改)", itm.Class0());
        }

        // ──────────────────────────────────────────────
        // IsFix / IsVisible
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFix_ByFixCommandVariable_ClassFixed_OrCurse()
        {
            // Fix コマンド（Fix(アイテム名) 変数）、クラス「固定」、特殊能力「呪い」のいずれかで外せない
            var src = CreateSRC();
            var normal = CreateItem(src, "普通の盾", "盾");
            var fixedClass = CreateItem(src, "固定の盾", "盾", "固定");
            var cursed = CreateItem(src, "呪いの盾", "盾", "汎用", "呪い");
            Assert.IsFalse(normal.IsFix);
            Assert.IsTrue(fixedClass.IsFix);
            Assert.IsTrue(cursed.IsFix);

            src.Expression.DefineGlobalVariable("Fix(普通の盾)");
            Assert.IsTrue(normal.IsFix);
        }

        [TestMethod]
        public void IsVisible_HiddenPartOrFixedHiddenItem()
        {
            // 装備個所「非表示」か、固定クラスで「非表示」能力を持つアイテムは表示されない
            var src = CreateSRC();
            var normal = CreateItem(src, "表示アイテム", "アイテム");
            var hiddenPart = CreateItem(src, "非表示個所", "非表示");
            var fixedHidden = CreateItem(src, "固定非表示", "アイテム", "固定", "非表示");
            var hiddenOnlyFeature = CreateItem(src, "汎用非表示", "アイテム", "汎用", "非表示");
            Assert.IsTrue(normal.IsVisible);
            Assert.IsTrue(hiddenPart.IsHidden);
            Assert.IsTrue(fixedHidden.IsHidden);
            // 固定でなければ「非表示」能力だけでは隠れない
            Assert.IsTrue(hiddenOnlyFeature.IsVisible);
        }

        // ──────────────────────────────────────────────
        // Nickname / KanaName: 式置換とデフォルトユニットの退避
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_ReplacesSubExpression_AndRestoresSelectedUnit()
        {
            // 愛称内の式は置換され、置換後は SelectedUnitForEvent が元に戻る（Item.cls Nickname）
            var src = CreateSRC();
            var u = CreateUnit(src, "機体Ａ");
            var other = CreateUnit(src, "機体Ｂ");
            var itm = CreateItem(src, "数式アイテム", "アイテム");
            itm.Data.Nickname = "強化$(1 + 2)型";
            itm.Data.KanaName = "きょうか$(2 * 3)";
            u.AddItem(itm);
            src.Event.SelectedUnitForEvent = other;

            Assert.AreEqual("強化3型", itm.Nickname());
            Assert.AreEqual("きょうか6", itm.KanaName());
            Assert.AreSame(other, src.Event.SelectedUnitForEvent);
        }

        // ──────────────────────────────────────────────
        // IsAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAvailable_DisabledByDisableCommand()
        {
            // Disable コマンドで使用不可にされたアイテムは使えない
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var itm = CreateItem(src, "封印アイテム", "アイテム");
            Assert.IsTrue(itm.IsAvailable(u));
            src.Expression.DefineGlobalVariable("Disable(封印アイテム)");
            Assert.IsFalse(itm.IsAvailable(u));
        }

        [TestMethod]
        public void IsAvailable_RequiresMatchingEquipPart()
        {
            // 片手/両手/盾は「腕」、肩/両肩は「肩」、体・頭はそれぞれの装備個所が必要
            var src = CreateSRC();
            var armOnly = CreateUnit(src, "腕だけ", "汎用", 4, "味方", "装備個所=腕");
            var full = CreateUnit(src, "全部", "汎用", 4, "味方", "装備個所=腕 肩 体 頭");
            var sword = CreateItem(src, "剣", "片手");
            var shoulder = CreateItem(src, "肩当て", "肩");
            var armor = CreateItem(src, "鎧", "体");
            var helm = CreateItem(src, "兜", "頭");

            Assert.IsTrue(sword.IsAvailable(armOnly));
            Assert.IsFalse(shoulder.IsAvailable(armOnly));
            Assert.IsFalse(armor.IsAvailable(armOnly));
            Assert.IsFalse(helm.IsAvailable(armOnly));
            Assert.IsTrue(shoulder.IsAvailable(full));
            Assert.IsTrue(armor.IsAvailable(full));
            Assert.IsTrue(helm.IsAvailable(full));
        }

        [TestMethod]
        public void IsAvailable_WeaponAndArmorClassProficiency()
        {
            // 武器は武器クラス、防具は防具クラスに Class0 が含まれる必要がある。固定・汎用は常に可
            var src = CreateSRC();
            var u = CreateUnit(src, "戦士", "汎用", 4, "味方", "装備個所=腕 体", "武器クラス=剣 槍", "防具クラス=鎧");
            Assert.IsTrue(CreateItem(src, "鋼の剣", "片手", "剣").IsAvailable(u));
            // 専用指定は Class0 で取り除いて比較する
            Assert.IsTrue(CreateItem(src, "勇者の剣", "両手", "剣(勇者専用)").IsAvailable(u));
            Assert.IsFalse(CreateItem(src, "斧", "片手", "斧").IsAvailable(u));
            Assert.IsTrue(CreateItem(src, "鉄の鎧", "体", "鎧").IsAvailable(u));
            Assert.IsFalse(CreateItem(src, "ローブ", "体", "服").IsAvailable(u));
            Assert.IsTrue(CreateItem(src, "棒", "武器", "汎用").IsAvailable(u));
            Assert.IsTrue(CreateItem(src, "呪いの鎧", "体", "固定").IsAvailable(u));
            // その他の装備個所はクラスに関係なく使用可能
            Assert.IsTrue(CreateItem(src, "指輪", "アイテム", "指輪").IsAvailable(u));
        }

        [TestMethod]
        public void IsAvailable_NecessarySkill_NotEquipped()
        {
            // 必要技能を満たさない場合、まだ装備していなければ使用できない
            var src = CreateSRC();
            var robot = CreateUnit(src, "ロボ", "汎用");
            var human = CreateUnit(src, "人間", "(人間)");
            var itm = CreateItem(src, "生身用装備", "アイテム", "汎用", "必要技能=生身");
            Assert.IsFalse(itm.IsAvailable(robot));
            Assert.IsTrue(itm.IsAvailable(human));
        }

        [TestMethod]
        public void IsAvailable_NecessarySkill_EquippedButUnsatisfied()
        {
            // 装備中でも必要技能を満たさず、自分自身で解決できない場合は使用できない
            var src = CreateSRC();
            var robot = CreateUnit(src, "ロボ", "汎用");
            var itm = CreateItem(src, "生身用装備", "アイテム", "汎用", "必要技能=生身");
            robot.AddItem0(itm);
            Assert.IsFalse(itm.IsAvailable(robot));
        }

        [TestMethod]
        public void IsAvailable_NecessarySkill_SelfEquipReference()
        {
            // 必要技能「<自分の名前>装備」は、自身が非活性で判定が満たされなくても装備中なら使用可能
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            RidePilot(src, u, "パイロット", "男性");
            var itm = CreateItem(src, "光の珠", "アイテム", "汎用", "必要技能=光の珠装備");
            var other = CreateItem(src, "闇の珠", "アイテム", "汎用", "必要技能=光の珠装備");
            u.AddItem0(itm);
            u.AddItem0(other);
            // 「光の珠」が効力を発揮していれば、それを必要技能とする他のアイテムも使用可能
            itm.Activated = true;
            Assert.IsTrue(other.IsAvailable(u));

            itm.Activated = false;
            Assert.IsTrue(itm.IsAvailable(u));
            // 他のアイテムは「光の珠」が活性でないので使用できない
            Assert.IsFalse(other.IsAvailable(u));
        }

        [TestMethod]
        public void IsAvailable_NecessarySkill_SealedByOwnAbility()
        {
            // アイテム自身のパイロット能力付加が必要技能と同名なら満たしているとみなす
            var src = CreateSRC();
            var robot = CreateUnit(src, "ロボ", "汎用");
            var itm = CreateItem(src, "変身ベルト", "アイテム", "汎用", "必要技能=生身", "パイロット能力付加=\"生身=解説\"");
            robot.AddItem0(itm);
            Assert.IsTrue(itm.IsAvailable(robot));
        }

        [TestMethod]
        public void IsAvailable_UnnecessarySkill()
        {
            // 不必要技能を満たしている場合は（装備前なら）使用できない
            var src = CreateSRC();
            var robot = CreateUnit(src, "ロボ", "汎用");
            var human = CreateUnit(src, "人間", "(人間)");
            var itm = CreateItem(src, "機械専用", "アイテム", "汎用", "不必要技能=生身");
            Assert.IsTrue(itm.IsAvailable(robot));
            Assert.IsFalse(itm.IsAvailable(human));

            // 装備中でも自分で解決できなければ使用できない
            var itm2 = CreateItem(src, "機械専用２", "アイテム", "汎用", "不必要技能=生身");
            human.AddItem0(itm2);
            Assert.IsFalse(itm2.IsAvailable(human));
        }

        [TestMethod]
        public void IsAvailable_UnnecessarySkill_SatisfiedByOwnAbility()
        {
            // アイテム自身が付加する能力によって不必要技能が満たされている場合は無視する
            var src = CreateSRC();
            var human = CreateUnit(src, "人間", "(人間)");
            var itm = CreateItem(src, "人化の指輪", "アイテム", "汎用", "不必要技能=生身", "パイロット能力付加=生身");
            human.AddItem0(itm);
            Assert.IsTrue(itm.IsAvailable(human));
        }

        // ──────────────────────────────────────────────
        // データ委譲
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Size_AndFeatures_FromItemData()
        {
            // サイズは 1+大型アイテムLv（Item.cls Size）、特殊能力はアイテムデータから取得する
            var src = CreateSRC();
            var big = CreateItem(src, "大型ブースター", "強化パーツ", "汎用", "大型アイテムLv2", "ハードポイント=強化パーツ");
            var normal = CreateItem(src, "小型ブースター", "強化パーツ");
            Assert.AreEqual(3, big.Size());
            Assert.AreEqual(1, normal.Size());
            Assert.AreEqual(2, big.CountFeature());
            Assert.AreEqual(2d, big.FeatureLevel("大型アイテム"));
            Assert.AreEqual("強化パーツ", big.FeatureData("ハードポイント"));
        }

        [TestMethod]
        public void Parameters_AreAppliedToEquippedUnit()
        {
            // アイテムの HP/EN/装甲/運動性/移動力の修正値は装備したユニットの能力値に加算される（Unit.cls Update）
            var src = CreateSRC();
            var u = CreateUnit(src, "機体");
            var itm = CreateItem(src, "強化キット", "アイテム");
            itm.Data.HP = 300;
            itm.Data.EN = 20;
            itm.Data.Armor = 50;
            itm.Data.Mobility = 5;
            itm.Data.Speed = 1;
            u.AddItem(itm);
            Assert.AreEqual(1300, u.MaxHP);
            Assert.AreEqual(120, u.MaxEN);
            Assert.AreEqual(150, u.get_Armor(""));
            Assert.AreEqual(55, u.get_Mobility(""));
            Assert.AreEqual(5, u.Speed);

            u.DeleteItem(itm);
            Assert.AreEqual(1000, u.MaxHP);
            Assert.AreEqual(100, u.MaxEN);
            Assert.AreEqual(50, u.get_Mobility(""));
            Assert.AreEqual(4, u.Speed);
        }
    }
}
