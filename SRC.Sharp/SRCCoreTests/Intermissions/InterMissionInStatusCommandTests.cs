using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission.InStatusCommand() のユニットテスト (Intermission.cs)
    ///
    /// InStatusCommand は以下の条件を判定する:
    /// - Map.IsStatusView が true (MapFileName が null/空) → false を返す
    /// - Map.IsStatusView が false かつ:
    ///   - ScenarioFileName の basename が "ユニットステータス表示.eve" → true
    ///   - ScenarioFileName の basename が "パイロットステータス表示.eve" → true
    ///   - SRC.IsSubStage が true → true
    ///   - それ以外 → false
    /// </summary>
    [TestClass]
    public class InterMissionInStatusCommandTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // IsStatusView == true (MapFileName が空) の場合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStatusCommand_MapIsStatusViewTrue_ReturnsFalse()
        {
            // Map.IsStatusView は MapFileName が null/空 のとき true
            // → InStatusCommand は即 false を返す
            var src = CreateSrc();
            src.Map.MapFileName = null; // IsStatusView = true
            src.ScenarioFileName = "ユニットステータス表示.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsFalse(result, "IsStatusView=true のとき false を返すこと");
        }

        [TestMethod]
        public void InStatusCommand_MapFileNameIsEmpty_ReturnsFalse()
        {
            var src = CreateSrc();
            src.Map.MapFileName = ""; // IsStatusView = true
            src.IsSubStage = true;
            var result = src.InterMission.InStatusCommand();
            Assert.IsFalse(result);
        }

        // ──────────────────────────────────────────────
        // IsStatusView == false (MapFileName が設定済み) の場合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStatusCommand_IsSubStageTrue_ReturnsTrue()
        {
            // IsSubStage が true なら InStatusCommand は true を返す
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map"; // IsStatusView = false
            src.IsSubStage = true;
            var result = src.InterMission.InStatusCommand();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InStatusCommand_IsSubStageFalse_OtherFile_ReturnsFalse()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "普通のシナリオ.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void InStatusCommand_UnitStatusEveFilename_ReturnsTrue()
        {
            // ScenarioFileName が "ユニットステータス表示.eve" のとき true
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "ユニットステータス表示.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InStatusCommand_PilotStatusEveFilename_ReturnsTrue()
        {
            // ScenarioFileName が "パイロットステータス表示.eve" のとき true
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "パイロットステータス表示.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InStatusCommand_UnitStatusEveFilenameUpperCase_ReturnsTrue()
        {
            // .ToLower() によって大文字小文字を区別しない
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "ユニットステータス表示.EVE"; // 大文字拡張子
            var result = src.InterMission.InStatusCommand();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InStatusCommand_UnitStatusEveFilenameWithPath_ReturnsTrue()
        {
            // Path.GetFileName を使用しているため、パス付きでも basename で判定される
            // Linux では '/' 区切りを使う ('\' は Linux では区切り文字ではない)
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "/home/user/SRC/ユニットステータス表示.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InStatusCommand_PilotStatusEveFilenameWithPath_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "/home/user/SRC/パイロットステータス表示.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void InStatusCommand_EmptyScenarioFileName_ReturnsFalse()
        {
            // ScenarioFileName が空文字なら basename も空 → false
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "";
            var result = src.InterMission.InStatusCommand();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void InStatusCommand_SimilarButNotExactFilename_ReturnsFalse()
        {
            // 名前が似ているが一致しない場合は false
            var src = CreateSrc();
            src.Map.MapFileName = "stage01.map";
            src.IsSubStage = false;
            src.ScenarioFileName = "ユニットステータス表示2.eve";
            var result = src.InterMission.InStatusCommand();
            Assert.IsFalse(result);
        }
    }
}
