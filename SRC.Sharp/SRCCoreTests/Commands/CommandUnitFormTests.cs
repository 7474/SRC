using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitform.cs のユニットテスト
    /// TransformCommand (変形コマンド)
    /// </summary>
    [TestClass]
    public class CommandUnitFormTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
                GUIMap = new MockGUIMap(),
                GUIStatus = new MockGUIStatus(),
            };
            // IsStatusView = false にするため MapFileName を設定
            src.Map.MapFileName = "test.map";
            // IsHero() が Data.Class を参照するため、UDList+UList 経由でユニットを作成
            if (!src.UDList.IsDefined("変形テスト機体"))
            {
                src.UDList.Add("変形テスト機体");
            }
            var unit = src.UList.Add("変形テスト機体", 0, "味方");
            src.Commands.SelectedUnit = unit;
            return src;
        }

        // ──────────────────────────────────────────────
        // TransformCommand: 変形可能形態なし + ListBox キャンセル (→ 0)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TransformCommand_NoForms_ListBoxCancel_CommandStateResetToUnitSelect()
        {
            // 変形データなしのユニット → forms 空 → ListBox が呼ばれる → 0 でキャンセル
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void TransformCommand_NoForms_ListBoxCancel_SelectedCommandCleared()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "変形";

            src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void TransformCommand_NoForms_ListBoxCalledWithEmptyForms()
        {
            // 変形フォームなしのとき ListBox が呼ばれることを確認
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            bool listBoxCalled = false;
            ((MockGUI)src.GUI).ListBoxHandler = (_) => { listBoxCalled = true; return 0; };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));

            Assert.IsTrue(listBoxCalled, "TransformCommand は変形フォームがないとき ListBox を呼ぶべき");
        }

        [TestMethod]
        public void TransformCommand_IsTriggeredByTransformCmdID()
        {
            // TransformCmdID (10) で TransformCommand が呼ばれる
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;
            src.Commands.CommandState = "コマンド選択";

            // キャンセル後の CommandState が "ユニット選択" であることで正しくディスパッチされたことを確認
            src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
