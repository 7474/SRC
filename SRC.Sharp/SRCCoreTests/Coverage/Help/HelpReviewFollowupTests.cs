using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help（VB: Help.bas）のレビュー指摘で不足していた挙動のテスト。
    /// 空の解説の扱い、ダミー技能の解説の参照先、負レベル・吸収の文言、アビリティ用の属性名など。
    /// </summary>
    [TestClass]
    public class HelpReviewFollowupTests
    {
        private static List<string> Capture(HelpTestFixture fx, List<string> shown)
        {
            var calls = new List<string>();
            fx.Gui.OpenMessageFormHandler = (u1, u2) => calls.Add("open");
            fx.Gui.DisplayMessageHandler = (p, m, mode) => shown.Add(m);
            fx.Gui.CloseMessageFormHandler = () => calls.Add("close");
            return calls;
        }

        private static string FeatureMessage(string fdef)
        {
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature(fdef);
            var u = fx.Build();
            return fx.Src.Help.FeatureHelpMessage(u, 1, false);
        }

        // ──────────────────────────────────────────────
        // 解説が空の場合（VB: FeatureHelp / AttributeHelp の If Len(msg) > 0）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureHelp_EmptyMessage_OpensNothing()
        {
            // 解説を持たない未知の能力ではメッセージウィンドウを開かない
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("謎能力");
            var u = fx.Build();
            var shown = new List<string>();
            var calls = Capture(fx, shown);

            fx.Src.Help.FeatureHelp(u, 1, false);

            Assert.AreEqual(0, shown.Count);
            Assert.AreEqual(0, calls.Count);
        }

        [TestMethod]
        public void AttributeHelp_EmptyMessage_OpensNothing()
        {
            // 解説のない属性ではメッセージウィンドウを開かない
            var fx = new HelpTestFixture();
            fx.AddWeapon("謎武器", "謎");
            var u = fx.Build();
            var shown = new List<string>();
            var calls = Capture(fx, shown);

            fx.Src.Help.AttributeHelp(u, "謎", 1);

            Assert.AreEqual(0, shown.Count);
            Assert.AreEqual(0, calls.Count);
        }

        // ──────────────────────────────────────────────
        // SkillHelpMessage: ダミー技能の解説をユニット側の能力から取る
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DummySkill_ExplanationFromUnitFeature()
        {
            // パイロット側に解説がなければ、乗っているユニットの同名能力の「解説」を使う
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("謎の技能", 1, "", 1);
            fx.UnitData.AddFeature("謎の技能=解説 \"ユニットの 解説\"");
            var u = fx.Build();
            Assert.AreEqual("ユニットの 解説", fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1"));
        }

        // ステータス表示用ダミーユニット（UDList に既定で登録済み）に乗ったパイロットと、本来のユニット（テスト機）を作る
        private static (HelpTestFixture fx, SRCCore.Pilots.Pilot p) RideDummy(string realFeature, string dummyFeature, bool defineVar = true)
        {
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("謎の技能", 1, "", 1);
            if (realFeature != null)
            {
                fx.UnitData.AddFeature(realFeature);
            }
            if (dummyFeature != null)
            {
                fx.Src.UDList.Item("ステータス表示用ダミーユニット").AddFeature(dummyFeature);
            }
            var real = fx.Src.UList.Add(fx.UnitData.Name, 0, "味方");
            real.Update();
            var dummy = fx.Src.UList.Add("ステータス表示用ダミーユニット", 0, "味方");
            var p = fx.Src.PList.Add(fx.PilotData.Name, 1, "味方");
            p.Ride(dummy);
            dummy.Update();

            if (defineVar)
            {
                var vname = "搭乗ユニット[" + p.ID + "]";
                fx.Src.Expression.DefineLocalVariable(vname);
                fx.Src.Expression.SetVariableAsString(vname, real.ID);
            }
            return (fx, p);
        }

        [TestMethod]
        public void DummyUnit_ResolvesRealUnitFromLocalVariable()
        {
            // ダミーユニット搭乗時は ローカル変数 搭乗ユニット[ID] のユニットの能力から解説を探す
            var (fx, p) = RideDummy("謎の技能=解説 \"本来の 解説\"", null);
            Assert.AreEqual("本来の 解説", fx.Src.Help.SkillHelpMessage(p, "1"));
        }

        [TestMethod]
        public void DummyUnit_AlsoSearchesDummyUnitFeatures()
        {
            // 本来のユニットに解説がなくても、ダミーユニット側(u2)の能力も探す
            var (fx, p) = RideDummy(null, "謎の技能=解説 \"ダミーの 解説\"");
            Assert.AreEqual("ダミーの 解説", fx.Src.Help.SkillHelpMessage(p, "1"));
        }

        [TestMethod]
        public void DummyUnit_WithoutLocalVariable_DoesNotLookAtRealUnit()
        {
            // 搭乗ユニット[ID] が未定義ならダミーユニット自身しか見ないので、本来のユニットの解説は出ない
            var (fx, p) = RideDummy("謎の技能=解説 \"本来の 解説\"", null, defineVar: false);
            Assert.AreEqual("", fx.Src.Help.SkillHelpMessage(p, "1") ?? "");
        }

        // ──────────────────────────────────────────────
        // FeatureHelpMessage: 負レベル・吸収
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Field_NegativeLevel_IncreasesDamage()
        {
            // レベルが負ならダメージを 500×|Lv| 増加させる
            Assert.AreEqual("攻撃のダメージを500増加させる。", FeatureMessage("フィールドLv-1=フィールド"));
        }

        [TestMethod]
        public void Armor_NegativeLevel_DecreasesArmor()
        {
            // レベルが負なら装甲を 100×|Lv| 減少させる
            Assert.AreEqual("攻撃に対して装甲を200減少させる。", FeatureMessage("アーマーLv-2"));
        }

        [TestMethod]
        public void Resist_NegativeLevel_IncreasesDamage()
        {
            Assert.AreEqual("「火」属性を持つ攻撃に対してダメージを30%増加させる。", FeatureMessage("レジストLv-3=レジスト 火"));
        }

        [TestMethod]
        public void Protection_LevelOverTen_Absorbs()
        {
            // プロテクション: Lv>10 なら (10×Lv-100)% を吸収（VB: Help.bas 1456行）
            var msg = FeatureMessage("プロテクションLv12=プロテクション 全 10");
            StringAssert.StartsWith(msg, "攻撃のダメージを20%吸収する。");
        }

        [TestMethod]
        public void Protection_NegativeLevel_IncreasesDamage()
        {
            var msg = FeatureMessage("プロテクションLv-2=プロテクション 全 10");
            StringAssert.StartsWith(msg, "攻撃のダメージを20%増加させる。");
        }

        // ──────────────────────────────────────────────
        // AttributeName / AttributeHelp の見出し
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttributeName_Ability_ReplacesAttackWordWithAbility()
        {
            // アビリティでは「～攻撃」「～武器」を「～アビリティ」に言い換える
            var fx = new HelpTestFixture();
            var u = fx.Build();
            var help = fx.Src.Help;
            Assert.AreEqual("移動後使用可能攻撃", help.AttributeName(u, "Ｐ", false));
            Assert.AreEqual("移動後使用可能アビリティ", help.AttributeName(u, "Ｐ", true));
            Assert.AreEqual("低改造アビリティ", help.AttributeName(u, "改", true));
            // 個別に言い換えを持つ属性
            Assert.AreEqual("音波アビリティ", help.AttributeName(u, "音", true));
            Assert.AreEqual("使用回数共有アビリティ", help.AttributeName(u, "共", true));
            // 「攻撃」「武器」で終わらない名称はそのまま
            Assert.AreEqual("合体技", help.AttributeName(u, "合", true));
        }

        [TestMethod]
        public void AttributeName_ExplanationFeature_OverridesName()
        {
            // 属性名と同名の「解説」能力があれば第2要素を名称にする（アビリティでも言い換えない）
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("格=解説 拳法攻撃 \"拳で 殴る\"");
            var u = fx.Build();
            Assert.AreEqual("拳法攻撃", fx.Src.Help.AttributeName(u, "格", true));
        }

        [TestMethod]
        public void AttributeHelp_LeveledAttribute_OtherThanPetrify()
        {
            // 「痺L2」は「麻痺攻撃レベル２」の見出しになる
            var fx = new HelpTestFixture();
            fx.AddWeapon("スタンガン", "痺L2");
            var u = fx.Build();
            var shown = new List<string>();
            Capture(fx, shown);

            fx.Src.Help.AttributeHelp(u, "痺L2", 1);

            Assert.AreEqual(1, shown.Count);
            StringAssert.StartsWith(shown[0], "<b>麻痺攻撃レベル２</b>;");
        }

        [TestMethod]
        public void AttributeHelp_Ability_HeadingUsesAbilityWording()
        {
            var fx = new HelpTestFixture();
            fx.AddAbility("歌", "Ｐ", stock: 1);
            var u = fx.Build();
            var shown = new List<string>();
            Capture(fx, shown);

            fx.Src.Help.AttributeHelp(u, "Ｐ", 1, true);

            Assert.AreEqual(1, shown.Count);
            StringAssert.StartsWith(shown[0], "<b>移動後使用可能アビリティ</b>;");
        }

        // ──────────────────────────────────────────────
        // FeatureHelpMessage の文字列指定（VB: findex = "武器・防具クラス" の分岐）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureHelpMessage_WeaponArmorClass_ByString()
        {
            // VB では FeatureHelpMessage 自体が「武器・防具クラス」を扱い、Info 関数からも参照できる
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("武器クラス=剣");
            var u = fx.Build();
            Assert.AreEqual("武器【剣】;防具【-】", fx.Src.Help.FeatureHelpMessage(u, "武器・防具クラス", false));
        }

        [TestMethod]
        public void FeatureHelpMessage_ByFeatureName()
        {
            // 文字列指定では能力名で引ける
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("ＨＰ回復Lv2");
            fx.UnitData.AddFeature("ＥＮ回復Lv3");
            var u = fx.Build();
            Assert.AreEqual("毎ターン最大ＥＮの30%分のＥＮを回復。", fx.Src.Help.FeatureHelpMessage(u, "ＥＮ回復", false));
        }
    }
}
