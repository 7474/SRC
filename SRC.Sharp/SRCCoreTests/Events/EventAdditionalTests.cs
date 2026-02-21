using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// Events クラス群の追加ユニットテスト（既存テストで未カバーの項目）
    /// </summary>

    // ══════════════════════════════════════════════════════
    // LabelData 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class LabelDataAdditionalTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private LabelData Create(string data)
        {
            var ld = new LabelData(CreateSrc()) { Data = data };
            return ld;
        }

        // ──────────────────────────────────────────────
        // デフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultEventDataId_IsZero()
        {
            var ld = new LabelData(CreateSrc());
            Assert.AreEqual(0, ld.EventDataId);
        }

        [TestMethod]
        public void DefaultAsterNum_IsZero()
        {
            var ld = new LabelData(CreateSrc());
            Assert.AreEqual(0, ld.AsterNum);
        }

        // ──────────────────────────────────────────────
        // Data getter は設定した値をそのまま返す
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DataGetter_ReturnsOriginalString()
        {
            var ld = Create("スタート");
            Assert.AreEqual("スタート", ld.Data);
        }

        [TestMethod]
        public void DataGetter_WithPrefixAndParams_ReturnsOriginalString()
        {
            var ld = Create("*ターン 3 5");
            Assert.AreEqual("*ターン 3 5", ld.Data);
        }

        // ──────────────────────────────────────────────
        // プレフィックス: *- → AsterNum=2
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_AsteriskHyphenPrefix_IsAsterNum2AndStartLabel()
        {
            var ld = Create("*-スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
            Assert.AreEqual(2, ld.AsterNum);
        }

        // ──────────────────────────────────────────────
        // プレフィックス: -- → AsterNum=0 (通常)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_DoubleHyphenPrefix_IsAsterNum0AndStartLabel()
        {
            var ld = Create("--スタート");
            Assert.AreEqual(LabelType.StartEventLabel, ld.Name);
            Assert.AreEqual(0, ld.AsterNum);
        }

        // ──────────────────────────────────────────────
        // パラメータの種類 (定数文字列パラメータ)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Para_NpcParam_ReturnsNpc()
        {
            var ld = Create("破壊 ＮＰＣ");
            Assert.AreEqual("ＮＰＣ", ld.Para(2));
        }

        [TestMethod]
        public void Para_EnemyParam_ReturnsEnemy()
        {
            var ld = Create("破壊 敵");
            Assert.AreEqual("敵", ld.Para(2));
        }

        [TestMethod]
        public void Para_AllParam_ReturnsAll()
        {
            var ld = Create("全滅 全");
            Assert.AreEqual("全", ld.Para(2));
        }

        // ──────────────────────────────────────────────
        // パラメータ数のバリエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountPara_ThreeParams_ReturnsThree()
        {
            var ld = Create("ターン 1 10");
            Assert.AreEqual(3, ld.CountPara());
        }

        [TestMethod]
        public void CountPara_TwoParams_ReturnsTwo()
        {
            var ld = Create("攻撃 味方");
            Assert.AreEqual(2, ld.CountPara());
        }

        // ──────────────────────────────────────────────
        // ToString フォーマット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_DefaultEventDataId_ContainsZero()
        {
            var ld = Create("スタート");
            // EventDataId のデフォルトは 0
            var str = ld.ToString();
            Assert.IsTrue(str.Contains("0"));
            Assert.IsTrue(str.Contains("スタート"));
        }
    }

    // ══════════════════════════════════════════════════════
    // EventDataLine 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class EventDataLineAdditionalTests
    {
        // ──────────────────────────────────────────────
        // Empty インスタンスの全プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_FileIsDash()
        {
            Assert.AreEqual("-", EventDataLine.Empty.File);
        }

        [TestMethod]
        public void Empty_LineNumIsMinusOne()
        {
            Assert.AreEqual(-1, EventDataLine.Empty.LineNum);
        }

        [TestMethod]
        public void Empty_SourceIsUnknown()
        {
            Assert.AreEqual(EventDataSource.Unknown, EventDataLine.Empty.Source);
        }

        // ──────────────────────────────────────────────
        // IsAlwaysEventLabel の追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAlwaysEventLabel_EmptyData_ReturnsFalse()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "f.eve", 0, "");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        [TestMethod]
        public void IsAlwaysEventLabel_DataStartsWithHyphen_ReturnsFalse()
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "f.eve", 0, "-ラベル");
            Assert.IsFalse(line.IsAlwaysEventLabel);
        }

        // ──────────────────────────────────────────────
        // NextID の追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextID_ForNegativeOne_ReturnsZero()
        {
            var line = EventDataLine.Empty; // ID = -1
            Assert.AreEqual(0, line.NextID);
        }

        [TestMethod]
        public void NextID_ForLargeId_ReturnsLargePlusOne()
        {
            var line = new EventDataLine(10000, EventDataSource.Scenario, "f.eve", 0, "cmd");
            Assert.AreEqual(10001, line.NextID);
        }

        // ──────────────────────────────────────────────
        // IsSystemData のバリエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSystemData_Empty_ReturnsFalse()
        {
            // Empty は Source=Unknown
            Assert.IsFalse(EventDataLine.Empty.IsSystemData);
        }

        // ──────────────────────────────────────────────
        // ToString フォーマット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_Format_IsIdColonData()
        {
            var line = new EventDataLine(7, EventDataSource.Scenario, "x.eve", 1, "MyCmd");
            Assert.AreEqual("7: MyCmd", line.ToString());
        }
    }

    // ══════════════════════════════════════════════════════
    // HotPoint 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class HotPointAdditionalTests
    {
        // ──────────────────────────────────────────────
        // 大きな座標値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_LargeCoordinates_ReturnsFormattedString()
        {
            var hp = new HotPoint
            {
                Name = "大ボタン",
                Left = 99999,
                Top = 88888,
                Width = 1920,
                Height = 1080,
                Caption = "large"
            };
            var result = hp.ToString();
            Assert.AreEqual("大ボタン(99999,88888,1920,1080): large", result);
        }

        // ──────────────────────────────────────────────
        // キャプションに空白を含む場合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Caption_WithSpaces_PreservesValue()
        {
            var hp = new HotPoint { Name = "btn", Caption = "OK Cancel" };
            Assert.AreEqual("OK Cancel", hp.Caption);
        }

        // ──────────────────────────────────────────────
        // 構造体は値コピー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Struct_ValueCopy_IndependentAfterAssignment()
        {
            var hp1 = new HotPoint { Name = "A", Left = 10 };
            var hp2 = hp1; // 値コピー
            hp2.Left = 99;
            Assert.AreEqual(10, hp1.Left);
            Assert.AreEqual(99, hp2.Left);
        }
    }

    // ══════════════════════════════════════════════════════
    // BCVariable 追加テスト
    // ══════════════════════════════════════════════════════
    [TestClass]
    public class BCVariableAdditionalTests
    {
        // ──────────────────────────────────────────────
        // Armor のデフォルト値とリセット挙動
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Armor_DefaultIsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.Armor);
        }

        [TestMethod]
        public void DataReset_DoesNotClearArmor()
        {
            var bcv = new BCVariable();
            bcv.Armor = 800;
            bcv.DataReset();
            // Armor は DataReset でリセットされない
            Assert.AreEqual(800, bcv.Armor);
        }

        // ──────────────────────────────────────────────
        // DataReset を複数回呼んでも結果は同じ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DataReset_CalledTwice_ValuesStillDefault()
        {
            var bcv = new BCVariable();
            bcv.AttackExp = 100;
            bcv.TerrainAdaption = 2.0;
            bcv.DataReset();
            bcv.DataReset();
            Assert.AreEqual(0, bcv.AttackExp);
            Assert.AreEqual(1d, bcv.TerrainAdaption);
        }

        // ──────────────────────────────────────────────
        // WeaponNumber は DataReset の影響を受けない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponNumber_Zero_ByDefault()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.WeaponNumber);
        }

        [TestMethod]
        public void MeUnit_AtkUnit_DefUnit_NullByDefault()
        {
            var bcv = new BCVariable();
            Assert.IsNull(bcv.MeUnit);
            Assert.IsNull(bcv.AtkUnit);
            Assert.IsNull(bcv.DefUnit);
        }
    }
}
