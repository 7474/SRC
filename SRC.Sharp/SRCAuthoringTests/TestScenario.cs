using System;
using System.IO;
using System.Text;

namespace SRCAuthoring.Tests
{
    /// <summary>テスト用のシナリオのフォルダ（実行に必要な最小限の素材を含む）</summary>
    public sealed class TestScenario : IDisposable
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        public TestScenario()
        {
            Root = Path.Combine(Path.GetTempPath(), "srcs-authoring-" + Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(Root, "Data"));
            Directory.CreateDirectory(Path.Combine(Root, "Bitmap", "Event"));
            Directory.CreateDirectory(Path.Combine(Root, "Sound"));
            Write(Path.Combine("Data", "System", "terrain.txt"), "0", "平地, plain", "陸, 1, 0, 0", "", "");
        }

        public string Root { get; }

        public string Write(string relativePath, params string[] lines)
        {
            var path = Path.Combine(Root, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Join("\n", lines), Utf8);
            return path;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Root, true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
