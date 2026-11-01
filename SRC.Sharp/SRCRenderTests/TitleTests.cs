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
                runner.Session.SRC.PreRelease = "";
                runner.GUI.OpenTitleForm();
                Assert.AreEqual("3.5.0", runner.State.Title.Version);
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "title-01-version");

                runner.GUI.CloseTitleForm();
                Assert.IsNull(runner.State.Title);
            }
        }

        [TestMethod]
        public void PreReleaseOf_ReadsOnlyDevLabel()
        {
            // 開発版の印だけを読み、正式版やほかのプレリリースは空にする
            Assert.AreEqual("", SRCCore.SRC.PreReleaseOf(typeof(TitleTests).Assembly));
            Assert.AreEqual("", SRCCore.SRC.PreReleaseOf(null));
        }

        [TestMethod]
        public void OpenTitleForm_DevBuild_ShowsPreRelease()
        {
            // 開発版（master へのマージごとの dev）はプレリリースの印も出す
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Session.SRC.Version = new Version(3, 5, 6, 0);
                runner.Session.SRC.PreRelease = "dev.123";
                runner.GUI.OpenTitleForm();
                Assert.AreEqual("3.5.6-dev.123", runner.State.Title.Version);
            }
        }
    }
}
