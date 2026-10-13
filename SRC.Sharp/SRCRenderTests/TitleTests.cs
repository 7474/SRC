using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 起動時のタイトル（元の SRC の frmTitle）。バージョンはここにだけ出す（docs/versioning.md）。
    /// </summary>
    [TestClass]
    public class TitleTests
    {
        [TestMethod]
        public void OpenTitleForm_ShowsVersion()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Session.SRC.Version = new Version(3, 5, 0, 0);
                runner.GUI.OpenTitleForm();
                Assert.AreEqual("3.5.0", runner.State.Title.Version);
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "title-01-version");

                runner.GUI.CloseTitleForm();
                Assert.IsNull(runner.State.Title);
            }
        }
    }
}
