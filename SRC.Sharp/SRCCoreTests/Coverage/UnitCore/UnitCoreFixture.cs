using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.IO;
using System.Text;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// UnitCore 系テストの共通準備。データはテキストからストリームで読み込む（ファイルは使わない）。
    /// </summary>
    internal static class UnitCoreFixture
    {
        public static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        public static void LoadUnits(SRC src, string text)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            src.UDList.Load("unit.txt", stream);
        }

        public static void LoadPilots(SRC src, string text)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            src.PDList.Load("pilot.txt", stream);
        }

        // パイロットを作成してユニットに搭乗させる
        public static Pilot Board(SRC src, Unit u, string pname, int level = 1)
        {
            var p = src.PList.Add(pname, level, u.Party0, gid: "");
            p.Ride(u);
            return p;
        }
    }
}
