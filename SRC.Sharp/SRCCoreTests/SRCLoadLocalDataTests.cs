using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Tests
{
    /// <summary>
    /// セーブデータのロード時にシナリオの Data フォルダ直下のローカルデータを読み込む（元の SRC の LoadData）
    /// </summary>
    [TestClass]
    public class SRCLoadLocalDataTests
    {
        private string dir;

        [TestInitialize]
        public void Initialize()
        {
            dir = Path.Combine(Path.GetTempPath(), "srclocaldata-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(dir, "Data"));
            File.WriteAllText(Path.Combine(dir, "Data", "alias.txt"),
                "ローカルエリアス\r\nバリア=バリア 1000\r\n\r\n", new UTF8Encoding(false));
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                Directory.Delete(dir, true);
            }
            catch
            {
                // ignore
            }
        }

        private SRC CreateSrc()
        {
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            return new SRC
            {
                GUI = new MockGUI
                {
                    DisplayLoadingProgressHandler = () => { },
                    SetLoadImageSizeHandler = _ => { },
                },
                FileSystem = fs,
                ScenarioPath = dir,
                Titles = new List<string>(),
            };
        }

        [TestMethod]
        public void LoadLocalData_LoadsAliasFromScenarioDataFolder()
        {
            var src = CreateSrc();

            src.LoadLocalData();

            Assert.IsTrue(src.ALDList.IsDefined("ローカルエリアス"));
            Assert.IsTrue(src.IsLocalDataLoaded);
        }

        [TestMethod]
        public void LoadData_LoadsLocalData()
        {
            var saver = CreateSrc();
            using var stream = new MemoryStream();
            saver.SaveData(stream);

            var loaded = CreateSrc();
            Assert.IsFalse(loaded.ALDList.IsDefined("ローカルエリアス"));
            stream.Position = 0;
            loaded.LoadData(stream);

            Assert.IsTrue(loaded.ALDList.IsDefined("ローカルエリアス"));
            Assert.IsTrue(loaded.IsLocalDataLoaded);
        }
    }
}
