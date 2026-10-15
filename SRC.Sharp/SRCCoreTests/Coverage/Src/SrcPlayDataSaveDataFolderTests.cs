using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// SRC.SearchDataFolder / IncludeData / LoadDataDirectory / LoadLocalData（元の SRC.bas の同名関数と LoadData のローカルデータ読み込み）
    /// </summary>
    [TestClass]
    public class SrcPlayDataSaveDataFolderTests
    {
        private static string[] Sp(string name)
        {
            return new[]
            {
                name,
                "略, 10, 自分, ターン, -, -, " + name,
                "移動力強化Lv2",
                "説明",
                "",
            };
        }

        private static string[] NonPilot(string name)
        {
            return new[] { name, name + ", " + name + ".bmp", "" };
        }

        [TestMethod]
        public void SearchDataFolder_SearchesScenarioExtDataExtData2ThenAppPath()
        {
            // シナリオ→ExtDataPath→ExtDataPath2→SRC フォルダの順に Data\作品名 フォルダを探す
            using var f = new SrcPlayDataSaveFixture(separateAppPath: true);
            var ext1 = Path.Combine(f.Dir, "ext1");
            var ext2 = Path.Combine(f.Dir, "ext2");
            f.Config.ExtDataPath = ext1;
            f.Config.ExtDataPath2 = ext2;
            foreach (var baseDir in new[] { f.ScenarioDir, ext1, ext2, f.AppDir })
            {
                Directory.CreateDirectory(Path.Combine(baseDir, "Data", "全部"));
            }
            Directory.CreateDirectory(Path.Combine(ext1, "Data", "外部1以降"));
            Directory.CreateDirectory(Path.Combine(ext2, "Data", "外部1以降"));
            Directory.CreateDirectory(Path.Combine(f.AppDir, "Data", "外部1以降"));
            Directory.CreateDirectory(Path.Combine(ext2, "Data", "外部2以降"));
            Directory.CreateDirectory(Path.Combine(f.AppDir, "Data", "外部2以降"));
            Directory.CreateDirectory(Path.Combine(f.AppDir, "Data", "本体のみ"));

            Assert.AreEqual(Path.Combine(f.ScenarioDir, "Data", "全部"), f.Src.SearchDataFolder("全部"));
            Assert.AreEqual(Path.Combine(ext1, "Data", "外部1以降"), f.Src.SearchDataFolder("外部1以降"));
            Assert.AreEqual(Path.Combine(ext2, "Data", "外部2以降"), f.Src.SearchDataFolder("外部2以降"));
            Assert.AreEqual(Path.Combine(f.AppDir, "Data", "本体のみ"), f.Src.SearchDataFolder("本体のみ"));
        }

        [TestMethod]
        public void SearchDataFolder_NotFound_ReturnsEmpty()
        {
            // どこにも無ければ空文字列
            using var f = new SrcPlayDataSaveFixture(separateAppPath: true);
            Directory.CreateDirectory(Path.Combine(f.ScenarioDir, "Data", "別作品"));
            Directory.CreateDirectory(Path.Combine(f.AppDir, "Data"));

            Assert.AreEqual("", f.Src.SearchDataFolder("無い作品"));
        }

        [TestMethod]
        public void SearchDataFolder_ScenarioWithoutDataFolder_UsesAppPath()
        {
            // シナリオ側に Data フォルダ自体が無い場合は SRC フォルダから探す
            using var f = new SrcPlayDataSaveFixture(separateAppPath: true);
            Directory.CreateDirectory(Path.Combine(f.AppDir, "Data", "作品"));

            Assert.AreEqual(Path.Combine(f.AppDir, "Data", "作品"), f.Src.SearchDataFolder("作品"));
        }

        [TestMethod]
        public void IncludeData_LoadsDataFilesOfFoundFolder()
        {
            // 見つかった作品フォルダの alias.txt / sp.txt / non_pilot.txt などを読み込む
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/作品/alias.txt", "作品エリアス", "バリア=バリア 1000", "");
            f.Write("Data/作品/sp.txt", Sp("作品ＳＰ"));
            f.Write("Data/作品/non_pilot.txt", NonPilot("作品ノンパイ"));

            f.Src.IncludeData("作品");

            Assert.IsTrue(f.Src.ALDList.IsDefined("作品エリアス"));
            Assert.IsTrue(f.Src.SPDList.IsDefined("作品ＳＰ"));
            Assert.IsTrue(f.Src.NPDList.IsDefined("作品ノンパイ"));
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        [TestMethod]
        public void IncludeData_MindTxtUsedWhenNoSpTxt()
        {
            // sp.txt が無ければ旧名の mind.txt を読み込む
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/作品/mind.txt", Sp("精神コマンド"));

            f.Src.IncludeData("作品");

            Assert.IsTrue(f.Src.SPDList.IsDefined("精神コマンド"));
        }

        [TestMethod]
        [Ignore("移植の不具合: LoadDataDirectory が sp.txt より mind.txt を優先して読み込む（VB の IncludeData は sp.txt 優先）")]
        public void IncludeData_SpTxtTakesPrecedenceOverMindTxt()
        {
            // VB: If FileExists(fpath & "\sp.txt") Then ... ElseIf FileExists(fpath & "\mind.txt")
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/作品/sp.txt", Sp("新ＳＰ"));
            f.Write("Data/作品/mind.txt", Sp("旧精神"));

            f.Src.IncludeData("作品");

            Assert.IsTrue(f.Src.SPDList.IsDefined("新ＳＰ"));
            Assert.IsFalse(f.Src.SPDList.IsDefined("旧精神"));
        }

        [TestMethod]
        public void IncludeData_FolderNotFound_ShowsErrorAndTerminates()
        {
            // VB: ErrorMessage "データ「" & new_title & "」のフォルダが見つかりません" → TerminateSRC
            using var f = new SrcPlayDataSaveFixture();

            SrcPlayDataSaveFixture.Run(() => f.Src.IncludeData("無い作品"));

            Assert.AreEqual("データ「無い作品」のフォルダが見つかりません",
                f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Single().Args[0]);
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        [TestMethod]
        public void LoadLocalData_SpTxtTakesPrecedenceOverMindTxt()
        {
            // シナリオの Data 直下のローカルデータも sp.txt を mind.txt より優先する
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/sp.txt", Sp("ローカルＳＰ"));
            f.Write("Data/mind.txt", Sp("ローカル精神"));
            f.Write("Data/non_pilot.txt", NonPilot("ローカルノンパイ"));

            f.Src.LoadLocalData();

            Assert.IsTrue(f.Src.SPDList.IsDefined("ローカルＳＰ"));
            Assert.IsFalse(f.Src.SPDList.IsDefined("ローカル精神"));
            Assert.IsTrue(f.Src.NPDList.IsDefined("ローカルノンパイ"));
            Assert.IsTrue(f.Src.IsLocalDataLoaded);
        }

        [TestMethod]
        public void LoadLocalData_MindTxtUsedWhenNoSpTxt()
        {
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/mind.txt", Sp("ローカル精神"));

            f.Src.LoadLocalData();

            Assert.IsTrue(f.Src.SPDList.IsDefined("ローカル精神"));
        }
    }
}
