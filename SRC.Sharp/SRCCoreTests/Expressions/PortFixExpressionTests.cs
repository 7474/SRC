using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// 移植時の差異を修正した関数の動作を元の SRC (Expression.bas) と比較するテスト
    /// </summary>
    [TestClass]
    public class PortFixExpressionTests
    {
        private static (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            return (src, gui);
        }

        private static Units.Unit CreateUnit(SRC src, string name)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            u.FullRecover();
            return u;
        }

        private static Pilots.Pilot CreatePilot(SRC src, string name, Units.Unit u = null, bool support = false)
        {
            src.PDList.Add(name);
            var p = src.PList.Add(name, 1, "味方");
            if (u != null)
            {
                if (support)
                {
                    u.AddSupport(p);
                }
                else
                {
                    u.AddPilot(p);
                }
                p.Unit = u;
            }
            return p;
        }

        // ──────────────────────────────────────────────
        // 関数の登録
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WindowWidthHeight_AreRegistered()
        {
            var (src, gui) = CreateSrc();
            gui.MainPWidth = 640;
            gui.MainPHeight = 480;
            Assert.AreEqual(640d, src.Expression.GetValueAsDouble("WindowWidth()"));
            Assert.AreEqual("480", src.Expression.GetValueAsString("WindowHeight()"));
        }

        [TestMethod]
        public void FileDialogs_AreRegistered_ReturnEmptyWhenCanceled()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual("", src.Expression.GetValueAsString("LoadFileDialog(\"テキスト\", \"txt\")"));
            Assert.AreEqual("", src.Expression.GetValueAsString("SaveFileDialog(\"テキスト\", \"txt\")"));
        }

        [TestMethod]
        public void Random_One_ReturnsOne()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("Random(1)"));
        }

        // ──────────────────────────────────────────────
        // Pilot / PilotID
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Pilot_NoArgs_ReturnsSelectedUnitMainPilot()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var p = CreatePilot(src, "パイロットＡ", u);
            src.Event.SelectedUnitForEvent = u;
            Assert.AreEqual("パイロットＡ", src.Expression.GetValueAsString("Pilot()"));
            Assert.AreEqual(p.ID, src.Expression.GetValueAsString("PilotID()"));
        }

        [TestMethod]
        public void Pilot_NumericArg_ReturnsNthPilotOfSelectedUnit()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            CreatePilot(src, "パイロットＡ", u);
            CreatePilot(src, "パイロットＢ", u);
            var s = CreatePilot(src, "サポートＣ", u, support: true);
            src.Event.SelectedUnitForEvent = u;
            Assert.AreEqual("パイロットＢ", src.Expression.GetValueAsString("Pilot(2)"));
            Assert.AreEqual("サポートＣ", src.Expression.GetValueAsString("Pilot(3)"));
            Assert.AreEqual(s.ID, src.Expression.GetValueAsString("PilotID(3)"));
            Assert.AreEqual("", src.Expression.GetValueAsString("Pilot(4)"));
        }

        [TestMethod]
        public void Pilot_UnitName_ReturnsMainPilot()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            CreatePilot(src, "パイロットＡ", u);
            Assert.AreEqual("パイロットＡ", src.Expression.GetValueAsString("Pilot(テスト機)"));
            Assert.AreEqual("パイロットＡ", src.Expression.GetValueAsString("Pilot(テスト機, 1)"));
        }

        // ──────────────────────────────────────────────
        // Action / X / Y
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Action_UnitName_NotFilteredByStatus()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            CreatePilot(src, "パイロットＡ", u);
            u.Status = "待機";
            Assert.IsTrue(u.Action > 0);
            Assert.AreEqual((double)u.Action, src.Expression.GetValueAsDouble("Action(" + u.ID + ")"));
            // パイロット名で指定した場合は出撃中のみ
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Action(パイロットＡ)"));
        }

        [TestMethod]
        public void XY_PilotNotOnUnit_ReturnsZero()
        {
            var (src, _) = CreateSrc();
            CreatePilot(src, "パイロットＡ");
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("X(パイロットＡ)"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Y(パイロットＡ)"));
        }

        // ──────────────────────────────────────────────
        // 数値の丸め
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_RoundsToEven()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(2, src.Expression.GetValueAsLong("2.5"));
            Assert.AreEqual(4, src.Expression.GetValueAsLong("3.5"));
            Assert.AreEqual(-2, src.Expression.GetValueAsLong("-2.5"));
            Assert.AreEqual(int.MaxValue, src.Expression.GetValueAsLong("10 ^ 20"));
        }

        [TestMethod]
        public void Round_FollowsVbSemantics()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(1200d, src.Expression.GetValueAsDouble("Round(1234, -2)"));
            Assert.AreEqual(1300d, src.Expression.GetValueAsDouble("Round(1250, -2)"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("Round(2.5)"));
            Assert.AreEqual(-2d, src.Expression.GetValueAsDouble("Round(-2.5, 0)"));
            Assert.AreEqual(1300d, src.Expression.GetValueAsDouble("RoundUp(1201, -2)"));
            Assert.AreEqual(1200d, src.Expression.GetValueAsDouble("RoundDown(1299, -2)"));
        }

        [TestMethod]
        public void RoundUpDown_LargeValues_DoNotOverflow()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(1e300, src.Expression.GetValueAsDouble("RoundUp(10 ^ 300, 2)"));
            Assert.AreEqual(1e300, src.Expression.GetValueAsDouble("RoundDown(10 ^ 300, 2)"));
            Assert.AreEqual(1e30, src.Expression.GetValueAsDouble("RoundDown(10 ^ 30, 0)"));
        }

        // ──────────────────────────────────────────────
        // 文字列関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AscChr_UseShiftJisCodes()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(-32096d, src.Expression.GetValueAsDouble("Asc(\"あ\")"));
            Assert.AreEqual(177d, src.Expression.GetValueAsDouble("Asc(\"ｱ\")"));
            Assert.AreEqual("ｱ", src.Expression.GetValueAsString("Chr(177)"));
            Assert.AreEqual("あ", src.Expression.GetValueAsString("Chr(-32096)"));
            Assert.AreEqual("あ", src.Expression.GetValueAsString("Chr(33440)"));
            Assert.AreEqual("A", src.Expression.GetValueAsString("Chr(65)"));
        }

        [TestMethod]
        public void StrComp_IsBinaryComparison()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("StrComp(\"a\", \"B\")"));
            Assert.AreEqual(-1d, src.Expression.GetValueAsDouble("StrComp(\"A\", \"b\")"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("StrComp(\"abc\", \"abc\")"));
            Assert.AreEqual(-1d, src.Expression.GetValueAsDouble("StrComp(\"a\", \"abc\")"));
        }

        [TestMethod]
        [Timeout(10000)]
        public void InStrRev_StartLessThanOne_ReturnsZero()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("InStrRev(\"abcabc\", \"bc\", 0)"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("InStrRev(\"abcabc\", \"bc\", -5)"));
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("InStrRev(\"abcabc\", \"bc\", 4)"));
            Assert.AreEqual(5d, src.Expression.GetValueAsDouble("InStrRev(\"abcabc\", \"bc\")"));
        }

        [TestMethod]
        public void Trim_RemovesHalfAndFullWidthSpaces()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual("a", src.Expression.GetValueAsString("Trim(\" 　a　 \")"));
            Assert.AreEqual("a　b", src.Expression.GetValueAsString("Trim(\"　a　b　\")"));
        }

        [TestMethod]
        public void RGB_UppercaseAndClamped()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual("#0102AB", src.Expression.GetValueAsString("RGB(1, 2, 171)"));
            Assert.AreEqual("#FF00FF", src.Expression.GetValueAsString("RGB(300, 0, 1000)"));
        }

        // ──────────────────────────────────────────────
        // 括弧・クォートの対応が取れていない式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnbalancedExpression_ReturnedAsIs()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual("(1 + 2", src.Expression.GetValueAsString("(1 + 2"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("(1 + 2"));
            Assert.AreEqual("1 + 2)", src.Expression.GetValueAsString("1 + 2)"));
            Assert.AreEqual("\"a\" & \"b", src.Expression.GetValueAsString("\"a\" & \"b"));
        }

        [TestMethod]
        public void ListSplitStrict_DetectsUnclosed()
        {
            Assert.AreEqual(-1, GeneralLib.ListSplitStrict("a (b c", out _));
            Assert.AreEqual(-1, GeneralLib.ListSplitStrict("a \"b c", out _));
            Assert.AreEqual(-1, GeneralLib.ListSplitStrict("a `b c", out _));
            Assert.AreEqual(-1, GeneralLib.ListSplitStrict("a b)", out _));
            Assert.AreEqual(2, GeneralLib.ListSplitStrict("a (b c)", out _));
            // 従来の ListSplit は末尾の閉じ忘れを検出しない
            Assert.AreEqual(2, GeneralLib.ListSplit("a (b c", out _));
        }

        // ──────────────────────────────────────────────
        // Format / Replace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_StringPlaceholders()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual("  abc", src.Expression.GetValueAsString("Format(\"abc\", \"@@@@@\")"));
            Assert.AreEqual("abc  ", src.Expression.GetValueAsString("Format(\"abc\", \"!@@@@@\")"));
            Assert.AreEqual("abc", src.Expression.GetValueAsString("Format(\"abc\", \"&&&&&\")"));
            Assert.AreEqual("12-34", src.Expression.GetValueAsString("Format(\"1234\", \"@@-@@\")"));
            Assert.AreEqual("ABC", src.Expression.GetValueAsString("Format(\"abc\", \">\")"));
            Assert.AreEqual("abcdef", src.Expression.GetValueAsString("Format(\"abcdef\", \"@@@\")"));
            Assert.AreEqual("   12", src.Expression.GetValueAsString("Format(12, \"@@@@@\")"));
        }

        [TestMethod]
        public void Format_NonNumericWithNumericFormat_ReturnsAsIs()
        {
            var (src, _) = CreateSrc();
            Assert.AreEqual("abc", src.Expression.GetValueAsString("Format(\"abc\", \"0.00\")"));
            Assert.AreEqual("3.14", src.Expression.GetValueAsString("Format(3.14159, \"0.00\")"));
        }

        [TestMethod]
        public void Replace_SameLength_RescansFromStart()
        {
            var (src, _) = CreateSrc();
            // VB版の ReplaceString は置換後の文字列を先頭から再検索する
            Assert.AreEqual("baa", src.Expression.GetValueAsString("Replace(\"aab\", \"ab\", \"ba\")"));
            // 長さが異なる場合は左から順に置換する
            Assert.AreEqual("xcxc", src.Expression.GetValueAsString("Replace(\"abcabc\", \"ab\", \"x\")"));
            Assert.AreEqual("aaa", src.Expression.GetValueAsString("Replace(\"aaa\", \"a\", \"a\")"));
        }
    }
}
