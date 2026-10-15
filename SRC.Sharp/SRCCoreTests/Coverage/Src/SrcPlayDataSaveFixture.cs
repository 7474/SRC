using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using SRCCore.CmdDatas;
using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// SRC.play / SRC.data / SRC.save のテスト用に、一時フォルダをシナリオフォルダとした
    /// SRC を HeadlessGUI 付きで用意する。
    /// separateAppPath を指定すると SRC 本体のフォルダ（AppPath）を別フォルダにする。
    /// </summary>
    internal sealed class SrcPlayDataSaveFixture : IDisposable
    {
        public string Dir { get; }
        public string AppDir { get; }
        public SRC Src { get; private set; }
        public HeadlessGUI Gui { get; private set; }
        public LocalFileConfig Config { get; }
        public string ScenarioDir { get; }

        public SrcPlayDataSaveFixture(bool separateAppPath = false)
        {
            Dir = Path.Combine(Path.GetTempPath(), "srcs-pds-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Dir);
            AppDir = separateAppPath ? Path.Combine(Dir, "app") : Dir;
            ScenarioDir = separateAppPath ? Path.Combine(Dir, "scenario") : Dir;
            Directory.CreateDirectory(AppDir);
            Directory.CreateDirectory(ScenarioDir);

            Config = new LocalFileConfig { AppPath = AppDir };
            NewSession();
        }

        /// <summary>
        /// 同じフォルダ構成で SRC と HeadlessGUI を作り直す（別の起動でロードする場合の模擬）。
        /// </summary>
        public void NewSession()
        {
            Src = new SRC
            {
                SystemConfig = Config,
                ScenarioPath = ScenarioDir,
            };
            var fs = new LocalFileSystem();
            fs.AddPath(Dir);
            Src.FileSystem = fs;

            Gui = new HeadlessGUI();
            Gui.Install(Src);

            // Execute を経由しないため、イベントデータの入れ物を初期化しておく
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();
            Src.Titles = new List<string>();
        }

        /// <summary>
        /// シナリオフォルダからの相対パス（/ 区切り）にテキストファイルを書く。
        /// </summary>
        public string Write(string relativePath, params string[] lines)
        {
            return WriteTo(ScenarioDir, relativePath, lines);
        }

        public static string WriteTo(string baseDir, string relativePath, params string[] lines)
        {
            var path = Path.Combine(new[] { baseDir }.Concat(relativePath.Split('/')).ToArray());
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
            return path;
        }

        /// <summary>
        /// 処理を実行し、送出された例外を返す（TerminateSRC 後は HeadlessGUI が例外で処理を打ち切るため）。
        /// </summary>
        public static Exception Run(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public bool Called(string member)
        {
            return Gui.CallsOf(member).Any();
        }

        /// <summary>
        /// mark（Gui.Calls.Count）以降に member が呼ばれたか。
        /// </summary>
        public bool CalledSince(int mark, string member)
        {
            return Gui.Calls.Skip(mark).Any(x => x.Member == member);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Dir, true);
            }
            catch
            {
                // ignore
            }
        }
    }
}
