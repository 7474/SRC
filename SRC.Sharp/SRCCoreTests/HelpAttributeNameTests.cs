using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Tests
{
    /// <summary>
    /// Help.AttributeName メソッドのユニットテスト。
    /// 武器属性コードを日本語の属性名に変換する純粋なマッピングロジック。
    /// </summary>
    [TestClass]
    public class HelpAttributeNameTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Unit CreateUnit(SRC src)
        {
            return new Unit(src);
        }

        // ──────────────────────────────────────────────
        // 基本攻撃タイプ属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_全_Returns全ての攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("全ての攻撃", src.Help.AttributeName(unit, "全"));
        }

        [TestMethod]
        public void AttributeName_格_Returns格闘系攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("格闘系攻撃", src.Help.AttributeName(unit, "格"));
        }

        [TestMethod]
        public void AttributeName_射_Returns射撃系攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("射撃系攻撃", src.Help.AttributeName(unit, "射"));
        }

        [TestMethod]
        public void AttributeName_複_Returns複合技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("複合技", src.Help.AttributeName(unit, "複"));
        }

        // ──────────────────────────────────────────────
        // 移動関連属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Ｐ_Returns移動後使用可能攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("移動後使用可能攻撃", src.Help.AttributeName(unit, "Ｐ"));
        }

        [TestMethod]
        public void AttributeName_Ｑ_Returns移動後使用不能攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("移動後使用不能攻撃", src.Help.AttributeName(unit, "Ｑ"));
        }

        // ──────────────────────────────────────────────
        // 武器カテゴリ属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Ｂ_Returnsビーム攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("ビーム攻撃", src.Help.AttributeName(unit, "Ｂ"));
        }

        [TestMethod]
        public void AttributeName_実_Returns実弾攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("実弾攻撃", src.Help.AttributeName(unit, "実"));
        }

        [TestMethod]
        public void AttributeName_オ_Returnsオーラ技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("オーラ技", src.Help.AttributeName(unit, "オ"));
        }

        [TestMethod]
        public void AttributeName_超_Returnsサイキック攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("サイキック攻撃", src.Help.AttributeName(unit, "超"));
        }

        [TestMethod]
        public void AttributeName_サ_Returns思念誘導攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("思念誘導攻撃", src.Help.AttributeName(unit, "サ"));
        }

        // ──────────────────────────────────────────────
        // 特殊効果属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_貫_Returns貫通攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("貫通攻撃", src.Help.AttributeName(unit, "貫"));
        }

        [TestMethod]
        public void AttributeName_無_Returnsバリア無効化攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("バリア無効化攻撃", src.Help.AttributeName(unit, "無"));
        }

        [TestMethod]
        public void AttributeName_浄_Returns浄化技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("浄化技", src.Help.AttributeName(unit, "浄"));
        }

        [TestMethod]
        public void AttributeName_封_Returns封印技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("封印技", src.Help.AttributeName(unit, "封"));
        }

        [TestMethod]
        public void AttributeName_殺_Returns抹殺攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("抹殺攻撃", src.Help.AttributeName(unit, "殺"));
        }

        [TestMethod]
        public void AttributeName_破_Returnsシールド貫通攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("シールド貫通攻撃", src.Help.AttributeName(unit, "破"));
        }

        // ──────────────────────────────────────────────
        // 状態異常攻撃属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_石_Returns石化攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("石化攻撃", src.Help.AttributeName(unit, "石"));
        }

        [TestMethod]
        public void AttributeName_凍_Returns凍結攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("凍結攻撃", src.Help.AttributeName(unit, "凍"));
        }

        [TestMethod]
        public void AttributeName_痺_Returns麻痺攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("麻痺攻撃", src.Help.AttributeName(unit, "痺"));
        }

        [TestMethod]
        public void AttributeName_眠_Returns催眠攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("催眠攻撃", src.Help.AttributeName(unit, "眠"));
        }

        [TestMethod]
        public void AttributeName_乱_Returns混乱攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("混乱攻撃", src.Help.AttributeName(unit, "乱"));
        }

        [TestMethod]
        public void AttributeName_魅_Returns魅了攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("魅了攻撃", src.Help.AttributeName(unit, "魅"));
        }

        // ──────────────────────────────────────────────
        // マップ攻撃属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Ｍ直_Returns直線型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("直線型マップ攻撃", src.Help.AttributeName(unit, "Ｍ直"));
        }

        [TestMethod]
        public void AttributeName_Ｍ拡_Returns拡散型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("拡散型マップ攻撃", src.Help.AttributeName(unit, "Ｍ拡"));
        }

        [TestMethod]
        public void AttributeName_Ｍ扇_Returns扇型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("扇型マップ攻撃", src.Help.AttributeName(unit, "Ｍ扇"));
        }

        [TestMethod]
        public void AttributeName_Ｍ全_Returns全方位型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("全方位型マップ攻撃", src.Help.AttributeName(unit, "Ｍ全"));
        }

        [TestMethod]
        public void AttributeName_Ｍ投_Returns投下型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("投下型マップ攻撃", src.Help.AttributeName(unit, "Ｍ投"));
        }

        [TestMethod]
        public void AttributeName_Ｍ移_Returns移動型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("移動型マップ攻撃", src.Help.AttributeName(unit, "Ｍ移"));
        }

        [TestMethod]
        public void AttributeName_Ｍ線_Returns線状マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("線状マップ攻撃", src.Help.AttributeName(unit, "Ｍ線"));
        }

        // ──────────────────────────────────────────────
        // 戦闘モード属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_攻_Returns攻撃専用()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("攻撃専用", src.Help.AttributeName(unit, "攻"));
        }

        [TestMethod]
        public void AttributeName_反_Returns反撃専用()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("反撃専用", src.Help.AttributeName(unit, "反"));
        }

        [TestMethod]
        public void AttributeName_武_Returns格闘武器()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("格闘武器", src.Help.AttributeName(unit, "武"));
        }

        [TestMethod]
        public void AttributeName_突_Returns突進技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("突進技", src.Help.AttributeName(unit, "突"));
        }

        [TestMethod]
        public void AttributeName_接_Returns接近戦攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("接近戦攻撃", src.Help.AttributeName(unit, "接"));
        }

        [TestMethod]
        public void AttributeName_Ｊ_Returnsジャンプ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("ジャンプ攻撃", src.Help.AttributeName(unit, "Ｊ"));
        }

        // ──────────────────────────────────────────────
        // 弾薬・補給関連属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Ａ_Returns自動充填式攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("自動充填式攻撃", src.Help.AttributeName(unit, "Ａ"));
        }

        [TestMethod]
        public void AttributeName_Ｃ_Returnsチャージ式攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("チャージ式攻撃", src.Help.AttributeName(unit, "Ｃ"));
        }

        [TestMethod]
        public void AttributeName_合_Returns合体技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("合体技", src.Help.AttributeName(unit, "合"));
        }

        [TestMethod]
        public void AttributeName_斉_Returns一斉発射()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("一斉発射", src.Help.AttributeName(unit, "斉"));
        }

        [TestMethod]
        public void AttributeName_永_Returns永続武器()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("永続武器", src.Help.AttributeName(unit, "永"));
        }

        // ──────────────────────────────────────────────
        // コスト消費系属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_自_Returns自爆攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("自爆攻撃", src.Help.AttributeName(unit, "自"));
        }

        [TestMethod]
        public void AttributeName_消_Returns消耗技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("消耗技", src.Help.AttributeName(unit, "消"));
        }

        [TestMethod]
        public void AttributeName_変_Returns変形技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("変形技", src.Help.AttributeName(unit, "変"));
        }

        // ──────────────────────────────────────────────
        // 改造・低改造属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Ｒ_Returns低改造武器()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("低改造武器", src.Help.AttributeName(unit, "Ｒ"));
        }

        [TestMethod]
        public void AttributeName_改_Returns低改造武器()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("低改造武器", src.Help.AttributeName(unit, "改"));
        }

        // ──────────────────────────────────────────────
        // is_ability パラメータによる分岐
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_共_NotAbility_Returns弾薬共有武器()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("弾薬共有武器", src.Help.AttributeName(unit, "共", is_ability: false));
        }

        [TestMethod]
        public void AttributeName_共_IsAbility_Returns使用回数共有アビリティ()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            // デフォルトの用語変換では "アビリティ"
            var result = src.Help.AttributeName(unit, "共", is_ability: true);
            Assert.IsTrue(result.Contains("使用回数共有"), $"Expected '使用回数共有' in '{result}'");
        }

        [TestMethod]
        public void AttributeName_音_NotAbility_Returns音波攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("音波攻撃", src.Help.AttributeName(unit, "音", is_ability: false));
        }

        [TestMethod]
        public void AttributeName_音_IsAbility_Returns音波アビリティ()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            var result = src.Help.AttributeName(unit, "音", is_ability: true);
            Assert.IsTrue(result.StartsWith("音波"), $"Expected result to start with '音波' but got '{result}'");
        }

        // ──────────────────────────────────────────────
        // その他の属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_体_Returns生命力換算攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("生命力換算攻撃", src.Help.AttributeName(unit, "体"));
        }

        [TestMethod]
        public void AttributeName_間_Returns間接攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("間接攻撃", src.Help.AttributeName(unit, "間"));
        }

        [TestMethod]
        public void AttributeName_識_Returns識別型マップ攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("識別型マップ攻撃", src.Help.AttributeName(unit, "識"));
        }

        [TestMethod]
        public void AttributeName_縛_Returns捕縛攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("捕縛攻撃", src.Help.AttributeName(unit, "縛"));
        }

        [TestMethod]
        public void AttributeName_Ｓ_Returnsショック攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("ショック攻撃", src.Help.AttributeName(unit, "Ｓ"));
        }

        [TestMethod]
        public void AttributeName_劣_Returns装甲劣化攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("装甲劣化攻撃", src.Help.AttributeName(unit, "劣"));
        }

        [TestMethod]
        public void AttributeName_中_Returnsバリア中和攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("バリア中和攻撃", src.Help.AttributeName(unit, "中"));
        }

        [TestMethod]
        public void AttributeName_限_Returns限定技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("限定技", src.Help.AttributeName(unit, "限"));
        }

        [TestMethod]
        public void AttributeName_シ_Returns同調率対象攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("同調率対象攻撃", src.Help.AttributeName(unit, "シ"));
        }

        [TestMethod]
        public void AttributeName_術_Returns術()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("術", src.Help.AttributeName(unit, "術"));
        }

        [TestMethod]
        public void AttributeName_技_Returns技()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("技", src.Help.AttributeName(unit, "技"));
        }

        [TestMethod]
        public void AttributeName_視_Returns視覚攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("視覚攻撃", src.Help.AttributeName(unit, "視"));
        }

        [TestMethod]
        public void AttributeName_浸_Returns浸蝕攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("浸蝕攻撃", src.Help.AttributeName(unit, "浸"));
        }

        // ──────────────────────────────────────────────
        // 性別属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Male_Returns対男性用攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("対男性用攻撃", src.Help.AttributeName(unit, "♂"));
        }

        [TestMethod]
        public void AttributeName_Female_Returns対女性用攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("対女性用攻撃", src.Help.AttributeName(unit, "♀"));
        }

        // ──────────────────────────────────────────────
        // 霊力・プラーナ消費属性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_霊_Returns霊力消費攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("霊力消費攻撃", src.Help.AttributeName(unit, "霊"));
        }

        [TestMethod]
        public void AttributeName_プ_Returns霊力消費攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            // 霊 と プ は同じ結果
            Assert.AreEqual("霊力消費攻撃", src.Help.AttributeName(unit, "プ"));
        }

        [TestMethod]
        public void AttributeName_憑_Returns憑依攻撃()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            Assert.AreEqual("憑依攻撃", src.Help.AttributeName(unit, "憑"));
        }
    }
}
