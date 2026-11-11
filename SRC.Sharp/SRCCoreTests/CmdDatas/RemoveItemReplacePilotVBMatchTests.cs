using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdData.cls の ExecRemoveItemCmd・ExecRemovePilotCmd・ExecReplacePilotCmd・ExecRenameFileCmd・ExecSaveDataCmd を、
    /// 元の SRC の動作を期待値にして確かめる（7474/SRC#1505）。
    /// </summary>
    [TestClass]
    public class RemoveItemReplacePilotVBMatchTests
    {
        private static (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui);
        }

        private static CmdData CreateCmd(SRC src, string cmdText)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        private static Units.Unit CreateUnit(SRC src, string name, string party = "味方")
        {
            if (!src.UDList.IsDefined(name))
            {
                var ud = src.UDList.Add(name);
                ud.HP = 1000;
                ud.EN = 100;
                ud.PilotNum = 1;
            }
            var u = src.UList.Add(name, 0, party);
            u.Update();
            return u;
        }

        private static Items.Item CreateItem(SRC src, string name, params string[] features)
        {
            if (!src.IDList.IsDefined(name))
            {
                var id = src.IDList.Add(name);
                foreach (var f in features)
                {
                    id.AddFeature(f);
                }
            }
            return src.IList.Add(name);
        }

        private static Pilots.Pilot CreatePilot(SRC src, string name)
        {
            if (!src.PDList.IsDefined(name))
            {
                src.PDList.Add(name).SP = 100;
            }
            return src.PList.Add(name, 1, "味方");
        }

        // 元の SRC: Set u = UList.Item(pname).CurrentForm（他形態の名前で指定しても、今の形態のアイテムを外す）
        [TestMethod]
        public void RemoveItem_UnitName_UsesCurrentForm()
        {
            var (src, _) = CreateSrc();
            var form1 = CreateUnit(src, "形態Ａ");
            var form2 = CreateUnit(src, "形態Ｂ");
            form1.AddOtherForm(form2);
            form2.AddOtherForm(form1);
            form1.Status = "他形態";
            form2.Status = "待機";
            var itm = CreateItem(src, "装備品");
            form2.AddItem(itm);

            Assert.AreEqual(1, CreateCmd(src, "RemoveItem 形態Ａ").Exec());

            Assert.AreEqual(0, form2.CountItem());
            Assert.IsNull(itm.Unit);
        }

        // 元の SRC: Case 3 も UList.Item(pname).CurrentForm のアイテムを外す
        [TestMethod]
        public void RemoveItem_UnitAndItemName_UsesCurrentForm()
        {
            var (src, _) = CreateSrc();
            var form1 = CreateUnit(src, "形態Ａ");
            var form2 = CreateUnit(src, "形態Ｂ");
            form1.AddOtherForm(form2);
            form2.AddOtherForm(form1);
            form1.Status = "他形態";
            form2.Status = "待機";
            var itm = CreateItem(src, "装備品");
            form2.AddItem(itm);

            Assert.AreEqual(1, CreateCmd(src, "RemoveItem 形態Ａ 装備品").Exec());

            Assert.AreEqual(0, form2.CountItem());
            Assert.IsFalse(itm.Exist);
        }

        // 元の SRC: ユニット画像を持つアイテムを外したときだけ、アイテムを外したユニットを描き直す
        [TestMethod]
        public void RemoveItem_ItemWithUnitImage_RepaintsTheUnit()
        {
            var (src, gui) = CreateSrc();
            src.Map.MapFileName = "test.map";
            var u = CreateUnit(src, "テスト機");
            var other = CreateUnit(src, "別の機体");
            u.Status = "出撃";
            src.Event.SelectedUnitForEvent = other;
            var itm = CreateItem(src, "画像装備", "ユニット画像=test.bmp");
            var painted = new List<Units.Unit>();
            gui.PaintUnitBitmapHandler = (pu, _) => painted.Add(pu);
            // 装備したときも Unit.Update が画像の変わったユニットを描き直す
            u.AddItem(itm);
            painted.Clear();

            Assert.AreEqual(1, CreateCmd(src, "RemoveItem テスト機 画像装備").Exec());

            // 元の SRC は DeleteItem から呼ぶ Unit.Update でも、画像が変わった出撃中のユニットを描き直す（Unit.cls 1684〜1698 行）
            CollectionAssert.AreEqual(new[] { u, u }, painted);
        }

        [TestMethod]
        public void RemoveItem_ItemWithoutUnitImage_DoesNotRepaint()
        {
            var (src, gui) = CreateSrc();
            src.Map.MapFileName = "test.map";
            var u = CreateUnit(src, "テスト機");
            u.Status = "出撃";
            var itm = CreateItem(src, "普通の装備");
            u.AddItem(itm);
            var painted = 0;
            gui.PaintUnitBitmapHandler = (_, _) => painted++;

            Assert.AreEqual(1, CreateCmd(src, "RemoveItem テスト機").Exec());

            Assert.AreEqual(0, u.CountItem());
            Assert.AreEqual(0, painted);
        }

        // 元の SRC: RemovePilot でメインでないパイロットを指定すると、サポートから外して Unit を Nothing にする
        [TestMethod]
        public void RemovePilot_Support_IsRemovedFromSupports()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            var main = CreatePilot(src, "メイン");
            var sup = CreatePilot(src, "サポート");
            main.Ride(u);
            u.AddSupport(sup);
            sup.Unit = u;

            Assert.AreEqual(1, CreateCmd(src, "RemovePilot サポート").Exec());

            Assert.AreEqual(0, u.CountSupport());
            Assert.IsNull(sup.Unit);
            Assert.IsFalse(sup.Alive);
            Assert.AreSame(u, main.Unit);
            Assert.AreNotEqual("破棄", u.Status);
        }

        // 元の SRC: PList.UpdateSupportMod p1.Unit の p1.Unit は乗せ換えで Nothing になっているので、全パイロットを更新する
        [TestMethod]
        public void ReplacePilot_UpdatesSupportModOfAllPilots()
        {
            var (src, _) = CreateSrc();
            src.Map.MapFileName = "test.map";
            var u = CreateUnit(src, "テスト機");
            var p1 = CreatePilot(src, "マルタ");
            p1.Ride(u);
            var bystander = CreatePilot(src, "ほかの人");
            bystander.Infight = 999;
            src.PDList.Add("ソラ").SP = 100;

            Assert.AreEqual(1, CreateCmd(src, "ReplacePilot マルタ ソラ").Exec());

            Assert.AreEqual(bystander.InfightBase + bystander.InfightMod, bystander.Infight);
        }

        // 元の SRC: 見つからない・既にあるときのメッセージは、ScenarioPath を付けたファイル名を出す
        [TestMethod]
        public void RenameFile_MissingSource_MessageHasScenarioPath()
        {
            var (src, _) = CreateSrc();
            var dir = Path.Combine(Path.GetTempPath(), "SRCRenameFileTest_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                src.FileSystem = new SRCCore.Filesystem.LocalFileSystem();
                src.ScenarioPath = dir;
                Assert.AreEqual(-1, CreateCmd(src, "RenameFile none.txt dest.txt").Exec());
                var expected = src.FileSystem.PathCombine(dir, "none.txt");
                CollectionAssert.AreEqual(new[] { "元のファイル「" + expected + "」が見つかりません" }, src.DataErrors.Select(x => x.msg).ToArray());
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        // 元の SRC: SaveData に空のファイル名を渡すとセーブしない（If fname <> "" Then）
        [TestMethod]
        public void SaveData_EmptyFileName_DoesNothing()
        {
            var (src, _) = CreateSrc();

            Assert.AreEqual(1, CreateCmd(src, "SaveData \"\"").Exec());
        }
    }
}
