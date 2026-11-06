using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests;
using SRCCore.Lib;
using SRCCore.TestLib;
using System.IO;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// Pilots を元の SRC（Pilots.cls）の動作と突き合わせるテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class PilotsVBMatchTests
    {
        private static SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        private static Pilot AddPilot(SRC src, string name, string nickname = null, string gid = null)
        {
            if (!src.PDList.IsDefined2(name))
            {
                var pd = src.PDList.Add(name);
                pd.Nickname = nickname ?? name;
                pd.Adaption = "AAAA";
            }
            return src.PList.Add(name, 1, "味方", gid);
        }

        [TestMethod]
        public void Item_DeadPilotById_PrefersAlivePilotWithSameName()
        {
            // ＩＤで見つかったパイロットが破棄されていれば、名称で破棄されていないパイロットを探す
            var src = CreateSRC();
            var dead = AddPilot(src, "兵士", gid: "部隊");
            dead.Alive = false;
            var alive = AddPilot(src, "兵士");

            Assert.AreSame(alive, src.PList.Item("兵士"));
            // 名称・愛称で見つからなければ、ＩＤで見つかった破棄されたパイロットを返す
            Assert.AreSame(dead, src.PList.Item("部隊"));
            // IsDefined は破棄されたパイロットを数えない
            Assert.IsFalse(src.PList.IsDefined("部隊"));
            Assert.IsTrue(src.PList.IsDefined("兵士"));
        }

        [TestMethod]
        public void Item_ByNickname_FindsAlivePilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "名前", nickname: "愛称");

            Assert.AreSame(p, src.PList.Item("愛称"));
            Assert.IsTrue(src.PList.IsDefined("愛称"));
            p.Alive = false;
            Assert.IsNull(src.PList.Item("愛称"));
            Assert.IsFalse(src.PList.IsDefined("愛称"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_RecalculatesSupportMod()
        {
            // 支援効果による修正値はセーブしない。元の SRC は RestoreData の最後に PList.UpdateSupportMod で計算し直す
            using var f = new SrcPlayDataSaveFixture();
            var path = f.Write("test.eve", "準備:", "Exit", "");
            f.Src.ScenarioFileName = path;
            f.Src.Event.LoadEventData(path, "");
            f.Src.Map.SetMapSize(3, 3);
            var plain = new Models.TerrainData { ID = 1, Name = "平地", Class = "陸", MoveCost = 2 };
            for (var x = 1; x <= 3; x++)
            {
                for (var y = 1; y <= 3; y++)
                {
                    var cell = f.Src.Map.MapData[x, y];
                    cell.UnderTerrain = plain;
                    cell.UpperTerrain = plain;
                    cell.TerrainType = plain.ID;
                    cell.BoxType = Maps.BoxTypes.Under;
                }
            }
            f.Src.Map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            f.Src.Turn = 3;

            var ud = f.Src.UDList.Add("機体");
            ud.Nickname = "機体";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 1000;
            ud.EN = 100;
            ud.Size = "M";
            var supporterData = f.Src.PDList.Add("支援役");
            supporterData.Nickname = "支援役";
            supporterData.Adaption = "AAAA";
            supporterData.AddSkill("広域サポート", 1, "", 1);
            var targetData = f.Src.PDList.Add("受ける役");
            targetData.Nickname = "受ける役";
            targetData.Adaption = "AAAA";

            var target = f.Src.PList.Add("受ける役", 1, "味方");
            var tu = f.Src.UList.Add("機体", 0, "味方");
            target.Ride(tu);
            tu.StandBy(1, 1);
            var supporter = f.Src.PList.Add("支援役", 1, "味方");
            var su = f.Src.UList.Add("機体", 0, "味方");
            supporter.Ride(su);
            su.StandBy(2, 1);
            f.Src.PList.UpdateSupportMod();
            Assert.AreEqual(5, target.HitMod2);

            var stream = new MemoryStream();
            f.Src.DumpData(stream, SRCSaveKind.Quik);
            f.Src.RestoreData(new MemoryStream(stream.ToArray()), SRCSaveKind.Quik);

            var restored = f.Src.PList.Item("受ける役");
            Assert.AreNotSame(target, restored);
            Assert.AreEqual(5, restored.HitMod2);
            Assert.AreEqual(5, restored.DodgeMod2);
        }
    }
}
