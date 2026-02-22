using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRC クラスの基本プロパティのユニットテスト
    /// </summary>
    [TestClass]
    public class SRCPropsTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // Turn プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Turn_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Turn);
        }

        [TestMethod]
        public void Turn_CanBeSet()
        {
            var src = CreateSrc();
            src.Turn = 5;
            Assert.AreEqual(5, src.Turn);
        }

        [TestMethod]
        public void Turn_CanBeIncremented()
        {
            var src = CreateSrc();
            src.Turn = 3;
            src.Turn++;
            Assert.AreEqual(4, src.Turn);
        }

        // ──────────────────────────────────────────────
        // TotalTurn プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TotalTurn_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.TotalTurn);
        }

        [TestMethod]
        public void TotalTurn_CanBeSet()
        {
            var src = CreateSrc();
            src.TotalTurn = 100;
            Assert.AreEqual(100, src.TotalTurn);
        }

        // ──────────────────────────────────────────────
        // Money プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Money_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void Money_CanBeSet()
        {
            var src = CreateSrc();
            src.Money = 5000;
            Assert.AreEqual(5000, src.Money);
        }

        // ──────────────────────────────────────────────
        // Stage プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Stage_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Stage);
        }

        [TestMethod]
        public void Stage_CanBeSet()
        {
            var src = CreateSrc();
            src.Stage = "マップ";
            Assert.AreEqual("マップ", src.Stage);
        }

        [TestMethod]
        public void Stage_CanBeSetToEpilogue()
        {
            var src = CreateSrc();
            src.Stage = "エピローグ";
            Assert.AreEqual("エピローグ", src.Stage);
        }

        // ──────────────────────────────────────────────
        // ScenarioFileName プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScenarioFileName_DefaultIsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.ScenarioFileName);
        }

        [TestMethod]
        public void ScenarioFileName_CanBeSet()
        {
            var src = CreateSrc();
            src.ScenarioFileName = "test.eve";
            Assert.AreEqual("test.eve", src.ScenarioFileName);
        }

        // ──────────────────────────────────────────────
        // ScenarioPath プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ScenarioPath_DefaultIsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.ScenarioPath);
        }

        // ──────────────────────────────────────────────
        // IsScenarioFinished プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsScenarioFinished_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsScenarioFinished);
        }

        [TestMethod]
        public void IsScenarioFinished_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.IsScenarioFinished = true;
            Assert.IsTrue(src.IsScenarioFinished);
        }

        // ──────────────────────────────────────────────
        // IsSubStage プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSubStage_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsSubStage);
        }

        [TestMethod]
        public void IsSubStage_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.IsSubStage = true;
            Assert.IsTrue(src.IsSubStage);
        }

        // ──────────────────────────────────────────────
        // IsCanceled プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsCanceled_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsCanceled);
        }

        [TestMethod]
        public void IsCanceled_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.IsCanceled = true;
            Assert.IsTrue(src.IsCanceled);
        }

        // ──────────────────────────────────────────────
        // SaveDataFileNumber / SaveDataVersion
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SaveDataFileNumber_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.SaveDataFileNumber);
        }

        [TestMethod]
        public void SaveDataFileNumber_CanBeSet()
        {
            var src = CreateSrc();
            src.SaveDataFileNumber = 3;
            Assert.AreEqual(3, src.SaveDataFileNumber);
        }

        [TestMethod]
        public void SaveDataVersion_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.SaveDataVersion);
        }

        // ──────────────────────────────────────────────
        // IsLocalDataLoaded プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsLocalDataLoaded_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsLocalDataLoaded);
        }

        [TestMethod]
        public void IsLocalDataLoaded_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.IsLocalDataLoaded = true;
            Assert.IsTrue(src.IsLocalDataLoaded);
        }

        // ──────────────────────────────────────────────
        // Titles プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Titles_DefaultIsNull()
        {
            // Titles は Event.data.cs の ClearEventData() で初期化される
            var src = CreateSrc();
            Assert.IsNull(src.Titles);
        }

        [TestMethod]
        public void Titles_AfterAssignment_CanAddItem()
        {
            var src = CreateSrc();
            src.Titles = new System.Collections.Generic.List<string>();
            src.Titles.Add("テストタイトル");
            Assert.AreEqual(1, src.Titles.Count);
            Assert.AreEqual("テストタイトル", src.Titles[0]);
        }

        // ──────────────────────────────────────────────
        // IsRestartSaveDataAvailable / IsQuickSaveDataAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsRestartSaveDataAvailable_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsRestartSaveDataAvailable);
        }

        [TestMethod]
        public void IsRestartSaveDataAvailable_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.IsRestartSaveDataAvailable = true;
            Assert.IsTrue(src.IsRestartSaveDataAvailable);
        }

        [TestMethod]
        public void IsQuickSaveDataAvailable_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsQuickSaveDataAvailable);
        }

        [TestMethod]
        public void IsQuickSaveDataAvailable_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.IsQuickSaveDataAvailable = true;
            Assert.IsTrue(src.IsQuickSaveDataAvailable);
        }

        // ──────────────────────────────────────────────
        // SRC サブオブジェクト (Expression, Event, Map, Commands, Sound)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Expression_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Expression);
        }

        [TestMethod]
        public void Event_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Event);
        }

        [TestMethod]
        public void Map_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Map);
        }

        [TestMethod]
        public void Commands_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Commands);
        }

        [TestMethod]
        public void Sound_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Sound);
        }

        [TestMethod]
        public void Help_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Help);
        }

        // ──────────────────────────────────────────────
        // Data lists
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.PDList);
        }

        [TestMethod]
        public void NPDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.NPDList);
        }

        [TestMethod]
        public void UDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.UDList);
        }

        [TestMethod]
        public void IDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.IDList);
        }

        [TestMethod]
        public void MDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.MDList);
        }

        [TestMethod]
        public void TDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.TDList);
        }

        [TestMethod]
        public void PList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.PList);
        }

        [TestMethod]
        public void UList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.UList);
        }

        [TestMethod]
        public void IList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.IList);
        }
    }
}
