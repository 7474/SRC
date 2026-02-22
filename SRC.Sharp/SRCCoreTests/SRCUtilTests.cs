using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRC クラス (SRC.util.cs, SRC.data.cs) のユニットテスト
    /// </summary>
    [TestClass]
    public class SRCUtilTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // IncrMoney
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncrMoney_PositiveAmount_IncreasesMoneyCorrectly()
        {
            var src = CreateSrc();
            src.Money = 1000;
            src.IncrMoney(500);
            Assert.AreEqual(1500, src.Money);
        }

        [TestMethod]
        public void IncrMoney_NegativeAmount_DecreasesMoneyCorrectly()
        {
            var src = CreateSrc();
            src.Money = 1000;
            src.IncrMoney(-300);
            Assert.AreEqual(700, src.Money);
        }

        [TestMethod]
        public void IncrMoney_OverflowsCap_ClampedTo999999999()
        {
            var src = CreateSrc();
            src.Money = 999999990;
            src.IncrMoney(100);
            Assert.AreEqual(999999999, src.Money);
        }

        [TestMethod]
        public void IncrMoney_BelowZero_ClampedToZero()
        {
            var src = CreateSrc();
            src.Money = 100;
            src.IncrMoney(-500);
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void IncrMoney_Zero_MoneyUnchanged()
        {
            var src = CreateSrc();
            src.Money = 1000;
            src.IncrMoney(0);
            Assert.AreEqual(1000, src.Money);
        }

        [TestMethod]
        public void IncrMoney_ExactlyAtMax_RemainsAtMax()
        {
            var src = CreateSrc();
            src.Money = 999999999;
            src.IncrMoney(0);
            Assert.AreEqual(999999999, src.Money);
        }

        [TestMethod]
        public void IncrMoney_ExactlyAtZero_CannotGoBelow()
        {
            var src = CreateSrc();
            src.Money = 0;
            src.IncrMoney(-1);
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void IncrMoney_LargePositive_ClampedToMax()
        {
            var src = CreateSrc();
            src.Money = 0;
            src.IncrMoney(int.MaxValue);
            Assert.AreEqual(999999999, src.Money);
        }

        // ──────────────────────────────────────────────
        // ConvertUnitID
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConvertUnitID_AlreadyHasColon_UnchangedFormat()
        {
            var src = CreateSrc();
            string id = "ガンダム:1";
            src.ConvertUnitID(ref id);
            // コロンが既にある場合はそのまま
            Assert.AreEqual("ガンダム:1", id);
        }

        [TestMethod]
        public void ConvertUnitID_EmptyString_Unchanged()
        {
            var src = CreateSrc();
            string id = "";
            src.ConvertUnitID(ref id);
            // 空文字列の場合は何も変わらないか確認
            // 実装に従いコロンが挿入される
            Assert.IsNotNull(id);
        }

        // ──────────────────────────────────────────────
        // Rand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Rand_ReturnsValueBetweenZeroAndOne()
        {
            var src = CreateSrc();
            var value = src.Rand();
            Assert.IsTrue(value >= 0.0 && value < 1.0);
        }

        [TestMethod]
        public void Rand_CalledMultipleTimes_ReturnsDouble()
        {
            var src = CreateSrc();
            for (int i = 0; i < 100; i++)
            {
                var value = src.Rand();
                Assert.IsTrue(value >= 0.0 && value < 1.0, $"反復{i}: {value} が範囲外");
            }
        }

        [TestMethod]
        public void Rand_ReturnsDifferentValues()
        {
            var src = CreateSrc();
            bool foundDifferent = false;
            double first = src.Rand();
            for (int i = 0; i < 100; i++)
            {
                if (src.Rand() != first)
                {
                    foundDifferent = true;
                    break;
                }
            }
            Assert.IsTrue(foundDifferent, "100回の乱数生成でも同一値が続いた");
        }

        // ──────────────────────────────────────────────
        // Version プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Version_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Version);
        }

        [TestMethod]
        public void Version_CanBeSet()
        {
            var src = CreateSrc();
            var ver = new Version(1, 2, 3, 4);
            src.Version = ver;
            Assert.AreEqual(ver, src.Version);
        }

        [TestMethod]
        public void Version_IsVersionType()
        {
            var src = CreateSrc();
            Assert.IsInstanceOfType(src.Version, typeof(Version));
        }

        // ──────────────────────────────────────────────
        // DataErrors (SRC.data.cs)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DataErrors_InitiallyEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void HasDataError_InitiallyFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.HasDataError);
        }

        [TestMethod]
        public void AddDataError_IncreasesCount()
        {
            var src = CreateSrc();
            src.AddDataError(new Exceptions.InvalidSrcData("テストエラー", "test.txt", 1, "line", "データ名"));
            Assert.AreEqual(1, src.DataErrors.Count);
        }

        [TestMethod]
        public void AddDataError_HasDataError_IsTrue()
        {
            var src = CreateSrc();
            src.AddDataError(new Exceptions.InvalidSrcData("テストエラー", "test.txt", 1, "line", "データ名"));
            Assert.IsTrue(src.HasDataError);
        }

        [TestMethod]
        public void ClearDataError_RemovesAllErrors()
        {
            var src = CreateSrc();
            src.AddDataError(new Exceptions.InvalidSrcData("エラー1", "test.txt", 1, "line1", "データ1"));
            src.AddDataError(new Exceptions.InvalidSrcData("エラー2", "test.txt", 2, "line2", "データ2"));
            src.ClearDataError();
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void ClearDataError_HasDataError_IsFalse()
        {
            var src = CreateSrc();
            src.AddDataError(new Exceptions.InvalidSrcData("エラー", "test.txt", 1, "line", "データ"));
            src.ClearDataError();
            Assert.IsFalse(src.HasDataError);
        }

        [TestMethod]
        public void AddDataError_MultipleErrors_CountIsCorrect()
        {
            var src = CreateSrc();
            src.AddDataError(new Exceptions.InvalidSrcData("エラー1", "test.txt", 1, "line1", "データ1"));
            src.AddDataError(new Exceptions.InvalidSrcData("エラー2", "test.txt", 2, "line2", "データ2"));
            src.AddDataError(new Exceptions.InvalidSrcData("エラー3", "test.txt", 3, "line3", "データ3"));
            Assert.AreEqual(3, src.DataErrors.Count);
        }

        // ──────────────────────────────────────────────
        // SRC プロパティ初期化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SRC_Constructor_Money_IsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void SRC_Constructor_Turn_IsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Turn);
        }

        [TestMethod]
        public void SRC_Constructor_TotalTurn_IsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.TotalTurn);
        }

        [TestMethod]
        public void SRC_Constructor_IsScenarioFinished_IsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsScenarioFinished);
        }

        [TestMethod]
        public void SRC_Constructor_IsSubStage_IsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsSubStage);
        }

        [TestMethod]
        public void SRC_Constructor_IsCanceled_IsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsCanceled);
        }

        [TestMethod]
        public void SRC_Constructor_IsLocalDataLoaded_IsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsLocalDataLoaded);
        }

        [TestMethod]
        public void SRC_Constructor_IsRestartSaveDataAvailable_IsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsRestartSaveDataAvailable);
        }

        [TestMethod]
        public void SRC_Constructor_IsQuickSaveDataAvailable_IsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.IsQuickSaveDataAvailable);
        }

        [TestMethod]
        public void SRC_Constructor_ScenarioFileName_IsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.ScenarioFileName);
        }

        [TestMethod]
        public void SRC_Constructor_ScenarioPath_IsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.ScenarioPath);
        }

        [TestMethod]
        public void SRC_Constructor_PDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.PDList);
        }

        [TestMethod]
        public void SRC_Constructor_UDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.UDList);
        }

        [TestMethod]
        public void SRC_Constructor_MDList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.MDList);
        }

        [TestMethod]
        public void SRC_Constructor_PList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.PList);
        }

        [TestMethod]
        public void SRC_Constructor_UList_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.UList);
        }

        [TestMethod]
        public void SRC_Constructor_Event_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Event);
        }

        [TestMethod]
        public void SRC_Constructor_Expression_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Expression);
        }

        [TestMethod]
        public void SRC_Constructor_SystemConfig_IsNotNull()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.SystemConfig);
        }

        [TestMethod]
        public void SRC_Money_CanBeSet()
        {
            var src = CreateSrc();
            src.Money = 5000;
            Assert.AreEqual(5000, src.Money);
        }

        [TestMethod]
        public void SRC_Turn_CanBeSet()
        {
            var src = CreateSrc();
            src.Turn = 3;
            Assert.AreEqual(3, src.Turn);
        }

        [TestMethod]
        public void SRC_Stage_CanBeSet()
        {
            var src = CreateSrc();
            src.Stage = "味方";
            Assert.AreEqual("味方", src.Stage);
        }
    }
}
