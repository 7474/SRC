using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Intermissions;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// インターミッションの換装コマンド（VB: InterMission.bas ExchangeFormCommand）を通した
    /// Units.Add の換装形態作成のテスト。ListBox の呼び出し内容で一覧を検証する。
    /// </summary>
    [TestClass]
    public class UnitCoreIntermissionExchangeTests
    {
        private static string UnitBlock(string name, params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, 汎用, 1, 2
陸, 5, M, 1000, 100
{feat}
3000, 100, 800, 90
AAAA, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        // ListBox の呼び出しごとに表示項目の名前を記録し、responder の戻り値で選択する
        private static (SRC src, List<ListBoxArgs> calls) CreateSrc(string units, System.Func<int, ListBoxArgs, int> responder)
        {
            var calls = new List<ListBoxArgs>();
            var gui = new MockGUI
            {
                ListBoxHandler = args =>
                {
                    calls.Add(args);
                    return responder(calls.Count, args);
                },
            };
            var src = new SRC { GUI = gui };
            UnitCoreFixture.LoadUnits(src, units);
            return (src, calls);
        }

        [TestMethod]
        public void ExchangeFormCommand_SelectSecondForm_TransformsUnit()
        {
            // 換装可能なユニットを選び、換装先を選ぶとその形態に換装される
            var units = UnitBlock("陸戦装備", "換装=宇宙装備 空戦装備")
                + UnitBlock("宇宙装備", "換装=陸戦装備 空戦装備")
                + UnitBlock("空戦装備", "換装=陸戦装備 宇宙装備");
            var (src, calls) = CreateSrc(units, (n, args) =>
            {
                switch (n)
                {
                    case 1:
                        return 1;
                    case 2:
                        // 換装先一覧から空戦装備を選ぶ
                        return args.Items.ToList().FindIndex(x => x.Text.StartsWith("空戦装備")) + 1;
                    default:
                        return 0;
                }
            });
            var u = src.UList.Add("陸戦装備", 0, "味方");

            new InterMission(src).ExchangeFormCommand();

            Assert.AreEqual("ユニット選択", calls[0].lb_caption);
            Assert.AreEqual(u.ID, calls[0].Items.Single().ListItemID);
            Assert.AreEqual("変更先選択", calls[1].lb_caption);
            Assert.AreEqual("空戦装備", u.CurrentForm().Name);
            Assert.AreEqual("待機", u.CurrentForm().Status);
            Assert.AreEqual("他形態", u.Status);
            // 換装後は一覧に戻り、キャンセルで終了する
            Assert.AreEqual(3, calls.Count);
        }

        [TestMethod]
        public void ExchangeFormCommand_ListsAllExchangeForms()
        {
            // VB Units.cls: 換装=宇宙装備 空戦装備 の全形態を作成するので、換装先一覧には両方が並ぶ
            var units = UnitBlock("陸戦装備", "換装=宇宙装備 空戦装備")
                + UnitBlock("宇宙装備", "換装=陸戦装備 空戦装備")
                + UnitBlock("空戦装備", "換装=陸戦装備 宇宙装備");
            var (src, calls) = CreateSrc(units, (n, args) => n == 1 ? 1 : 0);
            src.UList.Add("陸戦装備", 0, "味方");

            new InterMission(src).ExchangeFormCommand();

            var forms = calls[1].Items.Select(x => x.Text.Split(' ')[0]).ToList();
            CollectionAssert.AreEqual(new[] { "宇宙装備", "空戦装備" }, forms);
        }

        [TestMethod]
        public void ExchangeFormCommand_SingleExchangeForm_UnitIsListed()
        {
            // 換装先が1つだけのユニットも換装可能ユニットとして一覧に出る
            var units = UnitBlock("陸戦装備", "換装=宇宙装備")
                + UnitBlock("宇宙装備", "換装=陸戦装備");
            var (src, calls) = CreateSrc(units, (n, args) => 0);
            var u = src.UList.Add("陸戦装備", 0, "味方");

            new InterMission(src).ExchangeFormCommand();

            Assert.AreEqual(u.ID, calls[0].Items.Single().ListItemID);
        }
    }
}
