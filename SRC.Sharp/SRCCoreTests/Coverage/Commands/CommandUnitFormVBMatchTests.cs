using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using System.Linq;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// 「ハイパーモード」「合体」コマンドを、元の SRC（Command.bas の HyperModeCommand・CombineCommand）の動作を
    /// 期待値にして確かめるテスト。
    /// </summary>
    [TestClass]
    public class CommandUnitFormVBMatchTests
    {
        private static void Run(CommandUnitFormFixture f, int cmdId, string label)
        {
            f.Src.Commands.UnitCommand(new UiCommand(cmdId, label));
        }

        [TestMethod]
        public void HyperMode_PlainHyperModeMessage_IsDisplayed()
        {
            // VB は「ハイパーモード(元=>先)」「ハイパーモード(先)」「ハイパーモード(能力名)」が無ければ
            // 「ハイパーモード」のメッセージを表示する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("通常形態", "ハイパーモード=スーパーモード ハイパー形態");
            f.AddUnitData("ハイパー形態", "ノーマルモード=通常形態");
            var u = f.CreateUnit("通常形態", "パイロット", x: 5, y: 5);
            f.Src.MDList.Add("パイロット").AddMessage("ハイパーモード", "本気を出すぞ！");
            f.Src.Commands.CommandState = "コマンド選択";
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "スーパーモード");

            Assert.IsTrue(f.Gui.CallsOf("DisplayBattleMessage").Any(x => x.Args.OfType<string>().Any(a => a.Contains("本気を出すぞ！"))));
            Assert.AreEqual("ハイパー形態", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void Combine_OnMap_ListsPatternWhoseTargetCannotAct()
        {
            // VB の CombineCommand は合体後のユニットが作成されているか（UList.IsDefined）だけを見て、
            // 行動不能かは調べない（行動不能を除くのはメニューの判定だけ）
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("合体形態").PilotNum = 3;
            f.AddUnitData("別形態").PilotNum = 3;
            f.AddUnitData("A機", "合体=合体 合体形態 B機 C機", "合体=合体 別形態 B機 C機").PilotNum = 1;
            f.AddUnitData("B機").PilotNum = 1;
            f.AddUnitData("C機").PilotNum = 1;
            f.Src.UList.Add("合体形態", 0, "味方").Status = "他形態";
            var other = f.Src.UList.Add("別形態", 0, "味方");
            other.Status = "他形態";
            other.AddCondition("行動不能", -1);
            var a = f.CreateUnit("A機", "パイロットA", x: 3, y: 3);
            f.CreateUnit("B機", "パイロットB", x: 4, y: 3);
            f.CreateUnit("C機", "パイロットC", x: 3, y: 4);
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            var lb = f.ListBoxes.Single();
            Assert.AreEqual("合体後の形態", lb.lb_caption);
            CollectionAssert.AreEqual(new[] { "合体形態", "別形態" }, lb.Items.Select(x => x.Text).ToList());
            Assert.AreEqual("他形態", f.Src.UList.Item("合体形態").Status, "リストをキャンセルしたので合体しない");
        }

        [TestMethod]
        public void Combine_StatusView_HiddenCombinePattern_IsCombined()
        {
            // VB のステータス表示の合体メニューは名称が「非表示」の合体能力でも出し（見出しは「合体」）、
            // CombineCommand も能力の名称（FeatureName）を見ずに合体する
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("合体形態", "分離=分離 A機 B機").PilotNum = 2;
            f.AddUnitData("A機", "合体=非表示 合体形態 B機").PilotNum = 1;
            f.AddUnitData("B機", "合体=非表示 合体形態 A機").PilotNum = 1;
            f.Src.UList.Add("合体形態", 0, "味方").Status = "他形態";
            var a = f.CreateUnit("A機", "パイロットA", x: 1, y: 1);
            f.CreateUnit("B機", "パイロットB", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = a;

            Run(f, Command.CombineCmdID, "合体");
            Assert.AreEqual(0, f.ListBoxes.Count);
            Assert.AreEqual(2, f.Src.UList.Item("合体形態").CountPilot());
        }
    }
}
