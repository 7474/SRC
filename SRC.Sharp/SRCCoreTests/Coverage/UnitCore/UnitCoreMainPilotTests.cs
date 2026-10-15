using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Unit.MainPilot / PilotsHaveSpecialPower（VB: Unit.cls MainPilot、Command.bas スペシャルパワーコマンド）のテスト
    /// </summary>
    [TestClass]
    public class UnitCoreMainPilotTests
    {
        private static string UnitBlock(string name, int pilotNum = 1, params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, 汎用, {pilotNum}, 2
陸, 5, M, 1000, 100
{feat}
3000, 100, 800, 90
AAAA, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        private static string PilotBlock(string name, int sp = 0)
        {
            var spLine = sp > 0 ? $"ＳＰ, {sp}" : "ＳＰなし";
            return $@"
{name}
{name}, ぱいろっと, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
{spLine}
-.bmp, -.mid
";
        }

        private static SRC CreateSrc(string units, params string[] pilots)
        {
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src, units);
            UnitCoreFixture.LoadPilots(src, string.Join("\n", pilots));
            return src;
        }

        // ──────────────────────────────────────────────
        // MainPilot
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MainPilot_NoPilotAndNoAdditionalPilot_Terminates()
        {
            // パイロットが乗っておらず追加パイロットも無い場合は致命的エラー（VB: ErrorMessage → TerminateSRC）
            var src = CreateSrc(UnitBlock("テスト機"), PilotBlock("本体"));
            var u = src.UList.Add("テスト機", 0, "味方");
            Assert.Throws<TerminateException>(() => u.MainPilot());
        }

        [TestMethod]
        public void MainPilot_NoPilotWithAdditionalPilot_CreatesLevelOnePilot()
        {
            // パイロット不在でも追加パイロットがあれば、レベル1で作成してメインパイロットにする
            var src = CreateSrc(UnitBlock("無人機", features: "追加パイロット=制御ＡＩ"), PilotBlock("制御ＡＩ"));
            var u = src.UList.Add("無人機", 0, "味方");

            var p = u.MainPilot();

            Assert.AreEqual("制御ＡＩ", p.Name);
            Assert.AreEqual(1, p.Level);
            Assert.IsTrue(p.IsAdditionalPilot);
            Assert.AreSame(u, p.Unit);
            Assert.IsTrue(src.PList.IsDefined("制御ＡＩ"));
        }

        [TestMethod]
        public void MainPilot_BerserkWithoutBerserkPilotFeature_UsesFirstPilot()
        {
            // 暴走時パイロットを持たないユニットは暴走中でも第１パイロット
            var src = CreateSrc(UnitBlock("テスト機"), PilotBlock("本体"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "本体");
            u.AddCondition("暴走", -1);

            Assert.AreSame(main, u.MainPilot());
        }

        [TestMethod]
        public void MainPilot_SubPilotNamedAsAdditionalPilot_TakesOverWithoutAdditionalFlag()
        {
            // 追加パイロットに搭乗中のサブパイロットが指定されていれば、単なるメインパイロットの交代として扱う
            var src = CreateSrc(UnitBlock("複座機", pilotNum: 2, features: "追加パイロット=副"), PilotBlock("主"), PilotBlock("副"));
            var u = src.UList.Add("複座機", 0, "味方");
            // Ride だと搭乗時の Update で追加パイロットとして先に作成されるため、直接搭乗させる
            var main = src.PList.Add("主", 1, "味方", gid: "");
            var sub = src.PList.Add("副", 1, "味方", gid: "");
            u.AddPilot(main);
            main.Unit = u;
            u.AddPilot(sub);
            sub.Unit = u;

            Assert.AreSame(sub, u.MainPilot());
            Assert.IsFalse(sub.IsAdditionalPilot);
            // 新たなパイロットは作成されない
            Assert.AreEqual(2, src.PList.Items.Count());
        }

        [TestMethod]
        public void MainPilot_CopyCondition_IgnoresAdditionalPilot()
        {
            // 能力コピー中は追加パイロットを使わず第１パイロットを返す
            var src = CreateSrc(UnitBlock("テスト機", features: "追加パイロット=代役"), PilotBlock("本体"), PilotBlock("代役"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "本体");
            u.AddCondition("能力コピー", -1);

            Assert.AreSame(main, u.MainPilot());
        }

        [TestMethod]
        public void MainPilot_DiscardedUnit_UsesFirstPilot()
        {
            // 破棄されたユニットではメインパイロットの変更を行わない
            var src = CreateSrc(UnitBlock("テスト機", features: "追加パイロット=代役"), PilotBlock("本体"), PilotBlock("代役"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "本体");
            u.Status = "破棄";

            Assert.AreSame(main, u.MainPilot());
        }

        // ──────────────────────────────────────────────
        // PilotsHaveSpecialPower
        // ──────────────────────────────────────────────

        private static SRC CreateSpSrc(string unitBlock)
        {
            var src = CreateSrc(unitBlock, PilotBlock("主", sp: 50), PilotBlock("副", sp: 40), PilotBlock("妖精", sp: 30), PilotBlock("無能"));
            foreach (var pname in new[] { "主", "副", "妖精" })
            {
                src.PDList.Item(pname).AddSpecialPower("集中", 1, 10);
            }
            return src;
        }

        [TestMethod]
        public void PilotsHaveSpecialPower_IncludesAdditionalSupport()
        {
            // 追加サポートパイロットもスペシャルパワーの使用者に含まれる（VB: n = CountPilot + CountSupport + 1）
            var src = CreateSpSrc(UnitBlock("テスト機", features: "追加サポート=妖精"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "主");

            var list = u.PilotsHaveSpecialPower();

            Assert.AreEqual(2, list.Count);
            Assert.AreSame(main, list[0]);
            Assert.AreSame(u.AdditionalSupport(), list[1]);
            Assert.AreEqual("妖精", list[1].Name);
        }

        [TestMethod]
        public void PilotsHaveSpecialPower_AdditionalSupportWithoutSp_IsExcluded()
        {
            // スペシャルパワーを持たない追加サポートは含めない
            var src = CreateSpSrc(UnitBlock("テスト機", features: "追加サポート=無能"));
            var u = src.UList.Add("テスト機", 0, "味方");
            var main = UnitCoreFixture.Board(src, u, "主");

            CollectionAssert.AreEqual(new Pilot[] { main }, u.PilotsHaveSpecialPower().ToList());
        }

        [TestMethod]
        public void PilotsHaveSpecialPower_SubPilotAsMainPilot_UsesFirstPilot()
        {
            // VB Command.bas: サブパイロットがメインパイロットを勤めている場合も１番目のパイロットを使用
            var src = CreateSpSrc(UnitBlock("複座機", pilotNum: 2, features: "追加パイロット=副"));
            var u = src.UList.Add("複座機", 0, "味方");
            var first = src.PList.Add("主", 1, "味方", gid: "");
            var second = src.PList.Add("副", 1, "味方", gid: "");
            u.AddPilot(first);
            first.Unit = u;
            u.AddPilot(second);
            second.Unit = u;
            Assert.AreSame(second, u.MainPilot());

            CollectionAssert.AreEqual(new Pilot[] { first, second }, u.PilotsHaveSpecialPower().ToList());
        }
    }
}
