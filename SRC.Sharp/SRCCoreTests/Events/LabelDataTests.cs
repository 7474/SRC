using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// LabelData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class LabelDataTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            return src;
        }

        private LabelData Create(string data)
        {
            var src = CreateSrc();
            var ld = new LabelData(src)
            {
                Data = data
            };
            return ld;
        }

        // ──────────────────────────────────────────────
        // LabelType - 通常ラベル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_NormalLabel_IsNormalLabel()
        {
            var ld = Create("myLabel");
            Assert.AreEqual(LabelType.NormalLabel, ld.Name);
        }

        [TestMethod]
        public void Data_PrologueLabel_IsPrologueEventLabel()
        {
            var ld = Create("プロローグ");
            Assert.AreEqual(LabelType.PrologueEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_StartLabel_IsStartEventLabel()
        {
            var ld = Create("スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_EpilogueLabel_IsEpilogueEventLabel()
        {
            var ld = Create("エピローグ");
            Assert.AreEqual(LabelType.EpilogueEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_TurnLabel_IsTurnEventLabel()
        {
            var ld = Create("ターン");
            Assert.AreEqual(LabelType.TurnEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_DamageLabel_IsDamageEventLabel()
        {
            var ld = Create("損傷率");
            Assert.AreEqual(LabelType.DamageEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_DestructionLabel_IsDestructionEventLabel()
        {
            var ld = Create("破壊");
            Assert.AreEqual(LabelType.DestructionEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_TotalDestructionLabel_IsTotalDestructionEventLabel()
        {
            var ld = Create("全滅");
            Assert.AreEqual(LabelType.TotalDestructionEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_AttackLabel_IsAttackEventLabel()
        {
            var ld = Create("攻撃");
            Assert.AreEqual(LabelType.AttackEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_AfterAttackLabel_IsAfterAttackEventLabel()
        {
            var ld = Create("攻撃後");
            Assert.AreEqual(LabelType.AfterAttackEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_TalkLabel_IsTalkEventLabel()
        {
            var ld = Create("会話");
            Assert.AreEqual(LabelType.TalkEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_ContactLabel_IsContactEventLabel()
        {
            var ld = Create("接触");
            Assert.AreEqual(LabelType.ContactEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_EnterLabel_IsEnterEventLabel()
        {
            var ld = Create("進入");
            Assert.AreEqual(LabelType.EnterEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_EscapeLabel_IsEscapeEventLabel()
        {
            var ld = Create("脱出");
            Assert.AreEqual(LabelType.EscapeEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_LandLabel_IsLandEventLabel()
        {
            var ld = Create("収納");
            Assert.AreEqual(LabelType.LandEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_UseLabel_IsUseEventLabel()
        {
            var ld = Create("使用");
            Assert.AreEqual(LabelType.UseEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_AfterUseLabel_IsAfterUseEventLabel()
        {
            var ld = Create("使用後");
            Assert.AreEqual(LabelType.AfterUseEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_TransformLabel_IsTransformEventLabel()
        {
            var ld = Create("変形");
            Assert.AreEqual(LabelType.TransformEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_CombineLabel_IsCombineEventLabel()
        {
            var ld = Create("合体");
            Assert.AreEqual(LabelType.CombineEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_SplitLabel_IsSplitEventLabel()
        {
            var ld = Create("分離");
            Assert.AreEqual(LabelType.SplitEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_FinishLabel_IsFinishEventLabel()
        {
            var ld = Create("行動終了");
            Assert.AreEqual(LabelType.FinishEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_LevelUpLabel_IsLevelUpEventLabel()
        {
            var ld = Create("レベルアップ");
            Assert.AreEqual(LabelType.LevelUpEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_RequirementLabel_IsRequirementEventLabel()
        {
            var ld = Create("勝利条件");
            Assert.AreEqual(LabelType.RequirementEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_ResumeLabel_IsResumeEventLabel()
        {
            var ld = Create("再開");
            Assert.AreEqual(LabelType.ResumeEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_MapCommandLabel_IsMapCommandEventLabel()
        {
            var ld = Create("マップコマンド");
            Assert.AreEqual(LabelType.MapCommandEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_UnitCommandLabel_IsUnitCommandEventLabel()
        {
            var ld = Create("ユニットコマンド");
            Assert.AreEqual(LabelType.UnitCommandEventLabel, ld.Name);
        }

        [TestMethod]
        public void Data_EffectLabel_IsEffectEventLabel()
        {
            var ld = Create("特殊効果");
            Assert.AreEqual(LabelType.EffectEventLabel, ld.Name);
        }

        // ──────────────────────────────────────────────
        // アスタリスク・ハイフンプレフィックスの処理
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_SingleAsteriskPrefix_IsAsterNum2()
        {
            var ld = Create("*スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
            Assert.AreEqual(2, ld.AsterNum);
        }

        [TestMethod]
        public void Data_DoubleAsteriskPrefix_IsAsterNum3()
        {
            var ld = Create("**スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
            Assert.AreEqual(3, ld.AsterNum);
        }

        [TestMethod]
        public void Data_SingleHyphenPrefix_AsterNumIs0()
        {
            var ld = Create("-スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
            Assert.AreEqual(0, ld.AsterNum);
        }

        [TestMethod]
        public void Data_HyphenAsteriskPrefix_AsterNumIs1()
        {
            var ld = Create("-*スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
            Assert.AreEqual(1, ld.AsterNum);
        }

        // ──────────────────────────────────────────────
        // パラメータ数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountPara_NoParams_ReturnsOne()
        {
            var ld = Create("スタート");
            Assert.AreEqual(1, ld.CountPara());
        }

        [TestMethod]
        public void CountPara_WithParams_ReturnsCorrectCount()
        {
            var ld = Create("ターン 3 5");
            Assert.AreEqual(3, ld.CountPara());
        }

        // ──────────────────────────────────────────────
        // Para メソッド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Para_WithNumericParam_ReturnsValue()
        {
            var ld = Create("ターン 3 5");
            // Para(1) はラベル名, Para(2)=3, Para(3)=5
            Assert.AreEqual("3", ld.Para(2));
        }

        [TestMethod]
        public void Para_WithStringParam_ReturnsValue()
        {
            var ld = Create("破壊 味方");
            Assert.AreEqual("味方", ld.Para(2));
        }

        [TestMethod]
        public void Para_OutOfRange_ReturnsNull()
        {
            var ld = Create("スタート");
            Assert.IsNull(ld.Para(99));
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsEventDataId()
        {
            var ld = new LabelData(CreateSrc())
            {
                EventDataId = 42,
                Data = "スタート"
            };
            var str = ld.ToString();
            Assert.IsTrue(str.Contains("42"));
        }

        [TestMethod]
        public void ToString_ContainsData()
        {
            var ld = new LabelData(CreateSrc())
            {
                EventDataId = 1,
                Data = "myLabel"
            };
            var str = ld.ToString();
            Assert.IsTrue(str.Contains("myLabel"));
        }

        // ──────────────────────────────────────────────
        // Enable フラグ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Enable_DefaultIsFalse()
        {
            var src = CreateSrc();
            var ld = new LabelData(src);
            Assert.IsFalse(ld.Enable);
        }

        [TestMethod]
        public void Enable_CanBeSet()
        {
            var src = CreateSrc();
            var ld = new LabelData(src)
            {
                Enable = true,
                Data = "スタート"
            };
            Assert.IsTrue(ld.Enable);
        }
    }
}
