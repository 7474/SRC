using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help.bas（SRC/SRC_20121125）と Help.cs を突き合わせて見つけた違いを、VB の動作を期待値にして確かめる（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class HelpBasVbParityTests
    {
        [TestMethod]
        public void AttributeName_UnknownAttribute_ReturnsEmptyString()
        {
            // VB の AttributeName は、どの Case にも当たらなければ値を代入しないので "" を返す（Help.bas 3565〜3592 行）
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", "Ｘ");
            var u = fx.Build();
            Assert.AreEqual("", fx.Src.Help.AttributeName(u, "Ｘ"));
        }

        [TestMethod]
        public void AttributeHelpMessage_UnknownAttribute_ReturnsEmptyString()
        {
            // 解説の無い属性は msg が "" のまま返る
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", "Ｘ");
            var u = fx.Build();
            Assert.AreEqual("", fx.Src.Help.AttributeHelpMessage(u, "Ｘ", 1, false));
        }

        [TestMethod]
        public void AttributeHelpMessage_UnknownAttribute_WithLifeSizeOption_ReturnsEmptyString()
        {
            // 等身大基準では msg に ReplaceString をかける（Help.bas 4538〜4542 行）。msg が "" でもエラーにならない
            var fx = new HelpTestFixture();
            fx.AddWeapon("テスト武器", "Ｘ");
            var u = fx.Build();
            fx.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            Assert.AreEqual("", fx.Src.Help.AttributeHelpMessage(u, "Ｘ", 1, false));
        }

        [TestMethod]
        public void FeatureHelpMessage_NoDescription_ReturnsEmptyString()
        {
            // データも解説も無いダミー能力は Exit Function で "" を返す（Help.bas 3260〜3264 行）
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("謎能力");
            var u = fx.Build();
            Assert.AreEqual("", fx.Src.Help.FeatureHelpMessage(u, 1, false));
        }

        [TestMethod]
        public void TransformAttack_FindsMatchingFeatureAmongSeveral()
        {
            // 変: すべての変形技から、データの 1 番目が武器名のものを探す（Help.bas 3910〜3919 行）
            var fx = new HelpTestFixture();
            fx.AddWeapon("変形攻撃Ａ", "変");
            fx.AddWeapon("変形攻撃Ｂ", "変");
            fx.UnitData.AddFeature("変形技=変形攻撃Ａ フォームＡ");
            fx.UnitData.AddFeature("変形技=変形攻撃Ｂ フォームＢ");
            AddForm(fx, "フォームＡ", "形態Ａ");
            AddForm(fx, "フォームＢ", "形態Ｂ");
            var u = fx.Build();
            // 変形先の愛称が自分と違うので、愛称を出す
            Assert.AreEqual("使用後に形態Ａへ変化する。", fx.Src.Help.AttributeHelpMessage(u, "変", 1, false));
            Assert.AreEqual("使用後に形態Ｂへ変化する。", fx.Src.Help.AttributeHelpMessage(u, "変", 2, false));
        }

        // 変形先のユニットデータを定義する
        private static void AddForm(HelpTestFixture fx, string name, string nickname)
        {
            var ud = fx.Src.UDList.Add(name);
            ud.Nickname = nickname;
            ud.HP = 3000;
            ud.EN = 200;
            ud.Armor = 1000;
            ud.Mobility = 100;
            ud.Speed = 5;
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Size = "M";
        }

        [TestMethod]
        public void CombinationAttack_SkipsFeatureWithEmptyData()
        {
            // 合: データの無い合体技は LIndex が "" なので飛ばし、次の合体技を見る（Help.bas 3803〜3809 行）
            var fx = new HelpTestFixture();
            fx.AddWeapon("合体攻撃", "合");
            fx.UnitData.AddFeature("合体技");
            fx.UnitData.AddFeature("合体技=合体攻撃 僚機");
            var u = fx.Build();
            Assert.AreEqual("僚機と協力して行う技。", fx.Src.Help.AttributeHelpMessage(u, "合", 1, false));
        }

        [TestMethod]
        public void Ecm_UsesVbText()
        {
            // ＥＣＭ: 「同時に相手の…能力の効果を無効化。」（Help.bas 2926 行）
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("ＥＣＭLv2");
            var u = fx.Build();
            Assert.AreEqual(
                "半径３マス以内の味方ユニットに対する攻撃の命中率を元の90%に減少させる。同時に相手のＥＣＭ能力の効果を無効化。;思念誘導攻撃や近接攻撃には無効。",
                fx.Src.Help.FeatureHelpMessage(u, 1, false));
        }
    }
}
