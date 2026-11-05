using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SRCCore.Extensions;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Config.Tests
{
    /// <summary>
    /// シナリオのフォルダの設定ファイル（ScenarioConfig）と、それをアプリの設定に重ねた設定（ComposedConfig）のテスト
    /// </summary>
    [TestClass]
    public class ScenarioConfigTests
    {
        private string appDir;
        private string scenarioDir;
        private LocalFileSystem fs;

        [TestInitialize]
        public void Setup()
        {
            var root = Path.Combine(Path.GetTempPath(), "srcsc-" + Guid.NewGuid().ToString("N"));
            appDir = Path.Combine(root, "app");
            scenarioDir = Path.Combine(root, "scenario");
            Directory.CreateDirectory(appDir);
            Directory.CreateDirectory(scenarioDir);
            fs = new LocalFileSystem();
        }

        [TestCleanup]
        public void Cleanup()
        {
            fs.Dispose();
            Directory.Delete(Path.GetDirectoryName(appDir), true);
        }

        private LocalFileConfig NewBase()
        {
            return new LocalFileConfig
            {
                AppPath = appDir,
                BattleAnimation = true,
                ShowSquareLine = false,
                SpecialPowerAnimation = true,
                ImageBufferSize = 64,
                MaxImageBufferByteSize = 8 * 1024 * 1024,
            };
        }

        private void WriteSjisIni(string text)
        {
            File.WriteAllBytes(Path.Combine(scenarioDir, SrcIni.FileName), ZipNameEncoding.ShiftJis.GetBytes(text));
        }

        private string ReadSjisIni()
        {
            return ZipNameEncoding.ShiftJis.GetString(File.ReadAllBytes(Path.Combine(scenarioDir, SrcIni.FileName)));
        }

        private ComposedConfig Compose(LocalFileConfig baseConfig, Action<Exception> onError = null)
        {
            return new ComposedConfig(baseConfig, ScenarioConfig.Read(fs, scenarioDir), fs, onError);
        }

        [TestMethod]
        public void Read_ReturnsNullWithoutFiles()
        {
            Assert.IsNull(ScenarioConfig.Read(fs, scenarioDir));
        }

        [TestMethod]
        public void SrcIni_OverridesBaseConfig()
        {
            WriteSjisIni(";シナリオの設定\r\n[Option]\r\nBattleAnimation=Off\r\nSquare=on\r\nMindEffect=Off\r\nImageBufferNum=3\r\nMaxImageBufferSize=2\r\nMP3Volume=30\r\nAutoDeffence=On\r\nTurn=On\r\n[BGM]\r\nMap1=味方.mid\r\n");
            var baseConfig = NewBase();
            var config = Compose(baseConfig);

            Assert.IsFalse(config.BattleAnimation);
            Assert.IsTrue(config.ShowSquareLine);
            Assert.IsFalse(config.SpecialPowerAnimation);
            Assert.AreEqual(5, config.ImageBufferSize);
            Assert.AreEqual(2 * 1024 * 1024, config.MaxImageBufferByteSize);
            Assert.AreEqual(30, config.SoundVolume);
            Assert.IsTrue(config.AutoDefense);
            Assert.IsTrue(config.GetFlag("Option", "Turn"));
            Assert.AreEqual("味方.mid", config.GetItem("BGM", "map1"));
            // シナリオに無い項目はアプリの設定
            Assert.IsTrue(config.WeaponAnimation == baseConfig.WeaponAnimation);
            Assert.AreEqual("", config.GetItem("BGM", "Map2"));
            // アプリの設定は変わらない
            Assert.IsTrue(baseConfig.BattleAnimation);
            Assert.IsFalse(baseConfig.ShowSquareLine);
            Assert.AreEqual(50, baseConfig.SoundVolume);
        }

        [TestMethod]
        public void MessageWait_DefaultIsNormalSpeed()
        {
            // 元の SRC の既定値（普通）
            Assert.AreEqual(700, new LocalFileConfig().MessageWait);
            Assert.AreEqual(2, MessageSpeeds.IndexOf(700));
            Assert.AreEqual("普通", MessageSpeeds.Items[2].Name);
            // 選択肢に無い値は選ばない
            Assert.AreEqual(-1, MessageSpeeds.IndexOf(500));
        }

        [TestMethod]
        public void SrcIni_MessageWait_OverridesBaseConfigAndWritesBack()
        {
            WriteSjisIni("[Option]\r\nMessageWait=20000000\r\n");
            var baseConfig = NewBase();
            var config = Compose(baseConfig);

            // 元の SRC と同じく、上限（手動送り）を超えた値は上限にする
            Assert.AreEqual(MessageSpeeds.MaxWait, config.MessageWait);

            // シナリオの Src.ini にある項目は、設定変更で変えたら Src.ini に書く
            config.MessageWait = 200;
            config.Save();
            StringAssert.Contains(ReadSjisIni(), "MessageWait=200");
            Assert.AreEqual(700, baseConfig.MessageWait);
        }

        [TestMethod]
        public void MessageWait_WithoutScenarioItem_WritesBaseConfig()
        {
            var baseConfig = NewBase();
            var config = new ComposedConfig(baseConfig, null);
            Assert.AreEqual(700, config.MessageWait);
            config.MessageWait = 1000;
            Assert.AreEqual(1000, baseConfig.MessageWait);
        }

        [TestMethod]
        public void SrcsJson_OverridesSrcIni()
        {
            WriteSjisIni("[Option]\r\nBattleAnimation=Off\r\n[BGM]\r\nMap1=ini.mid\r\n");
            File.WriteAllText(Path.Combine(scenarioDir, ScenarioConfig.JsonFileName),
                "{ \"SRCCompatibilityMode\": false, \"BattleAnimation\": true, \"AppPath\": \"x\", \"Sections\": [ { \"Name\": \"BGM\", \"Items\": [ { \"Name\": \"Map1\", \"Value\": \"json.mid\" } ] } ] }");
            var baseConfig = NewBase();
            var config = Compose(baseConfig);

            Assert.AreEqual(SRCCompatibilityMode.None, config.SRCCompatibilityMode);
            Assert.IsTrue(config.BattleAnimation);
            Assert.AreEqual("json.mid", config.GetItem("BGM", "Map1"));
            // アプリの場所はシナリオでは変えない
            Assert.AreEqual(appDir, config.AppPath);
            Assert.AreEqual(SRCCompatibilityMode.ReadWrite, baseConfig.SRCCompatibilityMode);
        }

        [TestMethod]
        [DataRow("false", SRCCompatibilityMode.None)]
        [DataRow("true", SRCCompatibilityMode.ReadWrite)]
        [DataRow("1", SRCCompatibilityMode.Read)]
        [DataRow("\"Write\"", SRCCompatibilityMode.Write)]
        [DataRow("\"none\"", SRCCompatibilityMode.None)]
        public void SrcsJson_CompatibilityMode(string json, SRCCompatibilityMode expected)
        {
            File.WriteAllText(Path.Combine(scenarioDir, ScenarioConfig.JsonFileName), "{ \"SRCCompatibilityMode\": " + json + " }");
            Assert.AreEqual(expected, Compose(NewBase()).SRCCompatibilityMode);
        }

        [TestMethod]
        public void Set_ScenarioItem_WritesScenarioFileOnly()
        {
            WriteSjisIni(";シナリオの設定\r\n[Option]\r\nBattleAnimation=Off\r\nTurn=Off\r\n");
            var baseConfig = NewBase();
            var config = Compose(baseConfig);

            config.BattleAnimation = true;
            config.SetItem("Option", "Turn", "On");
            // シナリオに無い項目はアプリの設定に書く
            config.MoveAnimation = false;
            config.SetItem("Option", "DebugMode", "On");
            config.Save();

            Assert.AreEqual(";シナリオの設定\r\n[Option]\r\nBattleAnimation=On\r\nTurn=On\r\n", ReadSjisIni());
            Assert.IsTrue(config.BattleAnimation);
            Assert.IsTrue(config.GetFlag("Option", "Turn"));

            // アプリの設定（srcs.json）にはシナリオの項目を書かない
            Assert.IsTrue(baseConfig.BattleAnimation);
            Assert.AreEqual("", baseConfig.GetItem("Option", "Turn"));
            Assert.IsFalse(baseConfig.MoveAnimation);
            Assert.AreEqual("On", baseConfig.GetItem("Option", "DebugMode"));
            var saved = JObject.Parse(File.ReadAllText(Path.Combine(appDir, "srcs.json")));
            Assert.IsTrue((bool)saved["BattleAnimation"]);

            // 次に開いたときも変えた値になる（元の SRC の WriteIni と同じ）
            Assert.IsTrue(Compose(NewBase()).BattleAnimation);
        }

        [TestMethod]
        public void Set_SrcIniOptionItem_UpdatesProperty()
        {
            WriteSjisIni("[Option]\r\nSquare=Off\r\n");
            var config = Compose(NewBase());

            config.SetItem("Option", "Square", "On");

            Assert.IsTrue(config.ShowSquareLine);
            config.ShowSquareLine = false;
            Assert.AreEqual("Off", config.GetItem("Option", "Square"));
        }

        [TestMethod]
        public void Set_SrcsJsonValue_KeepsJsonType()
        {
            var json = Path.Combine(scenarioDir, ScenarioConfig.JsonFileName);
            File.WriteAllText(json, "{ \"KeepEnemyBGM\": false, \"SoundVolume\": 20 }");
            var config = Compose(NewBase());

            config.KeepEnemyBGM = true;
            config.SoundVolume = 70;
            config.Save();

            var saved = JObject.Parse(File.ReadAllText(json));
            Assert.AreEqual(JTokenType.Boolean, saved["KeepEnemyBGM"].Type);
            Assert.IsTrue((bool)saved["KeepEnemyBGM"]);
            Assert.AreEqual(70, (int)saved["SoundVolume"]);
        }

        [TestMethod]
        public void Save_ScenarioWriteFailure_KeepsValueForSession()
        {
            WriteSjisIni("[Option]\r\nBattleAnimation=Off\r\n");
            var errors = new List<Exception>();
            var config = new ComposedConfig(NewBase(), ScenarioConfig.Read(fs, scenarioDir), new ReadOnlyFileSystem(fs), errors.Add);

            config.BattleAnimation = true;
            config.Save();

            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(config.BattleAnimation);
            Assert.AreEqual("[Option]\r\nBattleAnimation=Off\r\n", ReadSjisIni());
        }

        [TestMethod]
        public void SrcIni_Utf8IsWrittenAsUtf8()
        {
            var ini = Path.Combine(scenarioDir, SrcIni.FileName);
            File.WriteAllText(ini, "; ＵＴＦ－８\n[Option]\nMoveAnimation=On\n", new UTF8Encoding(false));
            var config = Compose(NewBase());

            config.MoveAnimation = false;
            config.Save();

            Assert.AreEqual("; ＵＴＦ－８\n[Option]\nMoveAnimation=Off\n", File.ReadAllText(ini, new UTF8Encoding(false, true)));
        }

        [TestMethod]
        public void SRC_ConfigComposesSystemConfigAndScenarioConfig()
        {
            WriteSjisIni("[Option]\r\nBattleAnimation=Off\r\n");
            var src = new SRC { FileSystem = fs, ScenarioPath = scenarioDir };
            var baseConfig = NewBase();
            src.SystemConfig = baseConfig;
            Assert.IsTrue(src.BattleAnimation);

            src.LoadScenarioConfig();
            Assert.IsFalse(src.BattleAnimation);
            Assert.IsTrue(src.SystemConfig.BattleAnimation);

            // アプリの設定を差し替えても、シナリオの設定を重ねる
            var other = NewBase();
            other.ShowSquareLine = true;
            src.SystemConfig = other;
            Assert.IsFalse(src.BattleAnimation);
            Assert.IsTrue(src.ShowSquareLine);
        }

        [TestMethod]
        public void SRC_ScenarioInAppFolder_IsNotReadAsScenarioConfig()
        {
            File.WriteAllText(Path.Combine(appDir, ScenarioConfig.JsonFileName), "{ \"BattleAnimation\": false }");
            var src = new SRC { FileSystem = fs, ScenarioPath = appDir };
            src.SystemConfig = NewBase();

            src.LoadScenarioConfig();

            Assert.IsNull(src.ScenarioConfig);
            Assert.IsTrue(src.BattleAnimation);
        }

        [TestMethod]
        public void SRC_BattleAnimationUnavailable_DoesNotChangeConfig()
        {
            var src = new SRC { FileSystem = fs, ScenarioPath = scenarioDir };
            var baseConfig = NewBase();
            src.SystemConfig = baseConfig;

            src.BattleAnimationUnavailable = true;

            Assert.IsFalse(src.BattleAnimation);
            Assert.IsTrue(baseConfig.BattleAnimation);
            Assert.IsTrue(src.Config.BattleAnimation);
        }

        /// <summary>書き込めないファイルシステム（アーカイブの中のシナリオなど）</summary>
        private class ReadOnlyFileSystem : IFileSystem
        {
            private readonly IFileSystem inner;
            public ReadOnlyFileSystem(IFileSystem inner) { this.inner = inner; }
            public string PathCombine(params string[] paths) => inner.PathCombine(paths);
            public bool PathEquals(string a, string b) => inner.PathEquals(a, b);
            public bool FileExists(params string[] paths) => inner.FileExists(paths);
            public Stream Open(params string[] paths) => inner.Open(paths);
            public void AddPath(string basePath) => inner.AddPath(basePath);
            public void AddAchive(string basePath, string archivePath) => inner.AddAchive(basePath, archivePath);
            public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions) => inner.GetFileSystemEntries(path, searchPattern, enumerationOptions);
            public void AddSafePath(string basePath) => inner.AddSafePath(basePath);
            public Stream OpenSafe(SafeOpenMode mode, params string[] paths) => throw new UnauthorizedAccessException();
            public Stream Create(string path) => throw new UnauthorizedAccessException();
            public event Action<string> FileWritten { add { } remove { } }
            public bool Remove(params string[] paths) => false;
            public bool MkDir(params string[] paths) => false;
            public bool RmDir(params string[] paths) => false;
            public bool RelativePathEuqals(string scenarioPath, string a, string b) => inner.RelativePathEuqals(scenarioPath, a, b);
            public bool IsAbsolutePath(string path) => inner.IsAbsolutePath(path);
            public string ToAbsolutePath(string scenarioPath, string path) => inner.ToAbsolutePath(scenarioPath, path);
            public string ToRelativePath(string scenarioPath, string path) => inner.ToRelativePath(scenarioPath, path);
            public string NormalizePath(string path) => inner.NormalizePath(path);
        }
    }
}
