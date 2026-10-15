using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// 合体できるパートナーの距離の判定。元の SRC はコマンドメニュー（Command.bas の CommandSelect）では
    /// 縦横の和（Abs(dx) + Abs(dy) &gt; 2 で除外）、合体コマンドの実行（CombineCommand）では縦・横それぞれ
    /// （Abs(dx) &gt; 2 Or Abs(dy) &gt; 2 で除外）で測る。
    /// </summary>
    [TestClass]
    public class CombineMenuDistanceTests
    {
        private static Unit CreateTripleParts(CommandUnitFormFixture f, (int x, int y) pa, (int x, int y) pb, (int x, int y) pc)
        {
            f.AddUnitData("合体形態", "分離=分離 A機 B機 C機").PilotNum = 3;
            f.AddUnitData("A機", "合体=合体 合体形態 B機 C機").PilotNum = 1;
            f.AddUnitData("B機", "合体=合体 合体形態 A機 C機").PilotNum = 1;
            f.AddUnitData("C機", "合体=合体 合体形態 A機 B機").PilotNum = 1;
            var combined = f.Src.UList.Add("合体形態", 0, "味方");
            combined.Status = "他形態";
            var a = f.CreateUnit("A機", "パイロットA", x: pa.x, y: pa.y);
            f.CreateUnit("B機", "パイロットB", x: pb.x, y: pb.y);
            f.CreateUnit("C機", "パイロットC", x: pc.x, y: pc.y);
            return a;
        }

        [TestMethod]
        public void Diagonal_dx2dy1_IsCombinableButNotShownInMenu()
        {
            // (3,3) から見て (5,4) は dx=2, dy=1。実行時は合体できるが、メニューは和が 3 なので出さない
            var f = new CommandUnitFormFixture(onMap: true);
            var a = CreateTripleParts(f, (3, 3), (5, 4), (3, 4));

            Assert.AreEqual(1, a.CombineFeatures(f.Src).Count);
            Assert.AreEqual(0, a.CombineFeatures(f.Src, forMenu: true).Count);
        }

        [TestMethod]
        public void WithinTwoBySum_IsShownInMenu()
        {
            // (3,3) から見て (4,4)・(3,5) はどちらも和が 2 以内なのでメニューに出す
            var f = new CommandUnitFormFixture(onMap: true);
            var a = CreateTripleParts(f, (3, 3), (4, 4), (3, 5));

            Assert.AreEqual("合体", a.CombineFeatures(f.Src, forMenu: true).Single().CombineName);
        }
    }
}
