using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// データの定義のあるファイルと行（DataLocation）の記録（7474/SRC#1609）。
    /// 作成支援ツールの規則が診断の場所を示すのに使う。読み込みの結果は変えない
    /// </summary>
    [TestClass]
    public class DataLocationTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI(), FileSystem = new LocalFileSystem() };
        }

        private static Stream ToStream(string text)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(text));
        }

        [TestMethod]
        public void UnitDataRecordsNameFieldAndWeaponLines()
        {
            var src = CreateSrc();
            src.UDList.Load("robot.txt", ToStream(
                "# コメント\n"                       // 1
                + "\n"                               // 2
                + "テスト機\n"                        // 3
                + "テスト機, てすとき, 試験機, 1, 1\n" // 4
                + "陸, 4, M, 1000, 100\n"             // 5
                + "特殊能力なし\n"                    // 6
                + "3000, 100, 600, 80\n"              // 7
                + "AAAA, a.bmp\n"                     // 8
                + "パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突\n" // 9
                + "キック, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突\n" // 10
                + "===\n"                             // 11
                + "修理, 修理装置, 1, -, -, -, 1, -, -\n" // 12
                + "\n"));

            var ud = src.UDList.Item("テスト機");
            Assert.AreEqual("robot.txt", ud.Location.FileName);
            Assert.AreEqual(3, ud.Location.Line);
            Assert.AreEqual(5, ud.Location.FieldLine("移動可能地形"));
            Assert.AreEqual(7, ud.Location.FieldLine("最大ＨＰ"));
            Assert.AreEqual(8, ud.Location.FieldLine("地形適応"));
            // 記録の無い欄は名前の行
            Assert.AreEqual(3, ud.Location.FieldLine("存在しない欄"));
            CollectionAssert.AreEqual(new[] { 9, 10 }, ud.Weapons.Select(x => x.Location.Line).ToArray());
            Assert.AreEqual(12, ud.Abilities.Single().Location.Line);
            Assert.AreEqual("robot.txt", ud.Weapons.First().Location.FileName);
        }

        [TestMethod]
        public void PilotDataRecordsNameStatsAndSpecialPowerLines()
        {
            var src = CreateSrc();
            src.PDList.Load("pilot.txt", ToStream(
                "テスト\n"                                // 1
                + "テスト,てすと,男性,汎用,AAAA,100\n"   // 2
                + "特殊能力なし\n"                        // 3
                + "100,100,100,100,100,100,普通\n"        // 4
                + "ＳＰなし\n"                            // 5
                + "test.bmp,-\n"));                       // 6

            var pd = src.PDList.Item("テスト");
            Assert.AreEqual("pilot.txt", pd.Location.FileName);
            Assert.AreEqual(1, pd.Location.Line);
            Assert.AreEqual(4, pd.Location.FieldLine("格闘"));
            Assert.AreEqual(5, pd.Location.FieldLine("スペシャルパワー"));
        }

        [TestMethod]
        public void MessageDataRecordsEachSituationLine()
        {
            var src = CreateSrc();
            src.MDList.Load("pilot_message.txt", false, ToStream(
                "テスト\n"            // 1
                + "回避,よけた\n"     // 2
                + "ダメージ小,痛い\n" // 3
                + "\n"));

            var md = src.MDList.Item("テスト");
            Assert.AreEqual(1, md.Location.Line);
            CollectionAssert.AreEqual(new[] { 2, 3 }, md.Items.Select(x => x.Location.Line).ToArray());
        }

        [TestMethod]
        public void RedefinedUnitPointsToLastDefinition()
        {
            var src = CreateSrc();
            var unit = "テスト機\n"
                + "テスト機, てすとき, 試験機, 1, 1\n"
                + "陸, 4, M, 1000, 100\n"
                + "特殊能力なし\n"
                + "3000, 100, 600, 80\n"
                + "AAAA, a.bmp\n"
                + "\n";
            src.UDList.Load("a.txt", ToStream(unit));
            src.UDList.Load("b.txt", ToStream("\n\n" + unit));

            var ud = src.UDList.Item("テスト機");
            Assert.AreEqual("b.txt", ud.Location.FileName);
            Assert.AreEqual(3, ud.Location.Line);
        }
    }
}
