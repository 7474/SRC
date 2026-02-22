using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Pilots.Tests
{
    [TestClass]
    public class PilotsTests
    {
        private SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        private Pilot AddPilot(SRC src, string name, int level = 1, string party = "味方", string gid = null)
        {
            if (!src.PDList.IsDefined(name))
            {
                var pd = src.PDList.Add(name);
                pd.SP = 10;
                pd.Adaption = "AAAA";
            }
            return src.PList.Add(name, level, party, gid);
        }

        // ──────────────────────────────────────────────
        // Constructor
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_CountIsZero()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.PList.Count());
        }

        [TestMethod]
        public void Constructor_ItemsIsEmpty()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.PList.Items.Count);
        }

        // ──────────────────────────────────────────────
        // Add
        // ──────────────────────────────────────────────

        [TestMethod]
        [ExpectedException(typeof(TerminateException))]
        public void Add_UndefinedPilotData_ThrowsTerminateException()
        {
            var src = CreateSRC();
            src.PList.Add("未定義パイロット", 1, "味方");
        }

        [TestMethod]
        public void Add_RegisteredPilot_ReturnsNonNull()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "テストA");
            Assert.IsNotNull(p);
        }

        [TestMethod]
        public void Add_RegisteredPilot_IncreasesCount()
        {
            var src = CreateSRC();
            AddPilot(src, "テストB");
            Assert.AreEqual(1, src.PList.Count());
        }

        [TestMethod]
        public void Add_MultiplePilots_CountIsCorrect()
        {
            var src = CreateSRC();
            AddPilot(src, "テストC1");
            AddPilot(src, "テストC2");
            AddPilot(src, "テストC3");
            Assert.AreEqual(3, src.PList.Count());
        }

        [TestMethod]
        public void Add_SetsLevelCorrectly()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "テストD", level: 7);
            Assert.AreEqual(7, p.Level);
        }

        [TestMethod]
        public void Add_SetsPartyCorrectly()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "テストE", party: "敵");
            Assert.AreEqual("敵", p.Party);
        }

        [TestMethod]
        public void Add_SetsAliveTrue()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "テストF");
            Assert.IsTrue(p.Alive);
        }

        [TestMethod]
        public void Add_SetsUnitNull()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "テストG");
            Assert.IsNull(p.Unit);
        }

        [TestMethod]
        public void Add_WithGroupId_SetsIDToGroupId()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "テストH", gid: "grp1");
            Assert.AreEqual("grp1", p.ID);
        }

        // ──────────────────────────────────────────────
        // Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_AfterAddAndDelete_ReturnsCorrectValue()
        {
            var src = CreateSRC();
            var p1 = AddPilot(src, "カウントA");
            AddPilot(src, "カウントB");
            Assert.AreEqual(2, src.PList.Count());
            src.PList.Delete(p1.ID);
            Assert.AreEqual(1, src.PList.Count());
        }

        // ──────────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Delete_RemovesPilotByID()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "削除対象");
            var id = p.ID;
            src.PList.Delete(id);
            Assert.IsFalse(src.PList.IsDefined2(id));
        }

        [TestMethod]
        public void Delete_DecreasesCount()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "削除カウント");
            src.PList.Delete(p.ID);
            Assert.AreEqual(0, src.PList.Count());
        }

        // ──────────────────────────────────────────────
        // Item (ID / 名称 / 愛称 検索)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_ByID_ReturnsCorrectPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "検索ID");
            var found = src.PList.Item(p.ID);
            Assert.IsNotNull(found);
            Assert.AreEqual(p.ID, found.ID);
        }

        [TestMethod]
        public void Item_ByName_ReturnsCorrectPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "名前検索テスト");
            var found = src.PList.Item("名前検索テスト");
            Assert.IsNotNull(found);
            Assert.AreEqual(p.ID, found.ID);
        }

        [TestMethod]
        public void Item_ByNickname_ReturnsCorrectPilot()
        {
            var src = CreateSRC();
            var pd = src.PDList.Add("愛称検索パイロット");
            pd.SP = 10;
            pd.Adaption = "AAAA";
            pd.Nickname = "テスト愛称";
            var p = src.PList.Add("愛称検索パイロット", 1, "味方");
            var found = src.PList.Item("テスト愛称");
            Assert.IsNotNull(found);
            Assert.AreEqual(p.ID, found.ID);
        }

        [TestMethod]
        public void Item_NonExistent_ReturnsNull()
        {
            var src = CreateSRC();
            Assert.IsNull(src.PList.Item("存在しないパイロット"));
        }

        [TestMethod]
        public void Item_DeadPilot_ReturnsNull()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "死亡パイロット");
            p.Alive = false;
            Assert.IsNull(src.PList.Item(p.ID));
        }

        // ──────────────────────────────────────────────
        // IsDefined / IsDefined2
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ExistingPilot_ReturnsTrue()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "定義済み");
            Assert.IsTrue(src.PList.IsDefined(p.ID));
        }

        [TestMethod]
        public void IsDefined_NonExistent_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.PList.IsDefined("未登録ID"));
        }

        [TestMethod]
        public void IsDefined_DeadPilot_ReturnsFalse()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "死亡定義チェック");
            p.Alive = false;
            Assert.IsFalse(src.PList.IsDefined(p.ID));
        }

        [TestMethod]
        public void IsDefined2_ExistingPilot_ReturnsTrue()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "定義済み2");
            Assert.IsTrue(src.PList.IsDefined2(p.ID));
        }

        [TestMethod]
        public void IsDefined2_NonExistent_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.PList.IsDefined2("未登録ID2"));
        }

        [TestMethod]
        public void IsDefined2_DeadPilot_StillReturnsTrue()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "死亡ID検索");
            p.Alive = false;
            // Item2 はAliveに関係なくIDで検索する
            Assert.IsTrue(src.PList.IsDefined2(p.ID));
        }

        // ──────────────────────────────────────────────
        // Item2 (IDのみで検索)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item2_ByID_ReturnsPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "ID検索");
            var found = src.PList.Item2(p.ID);
            Assert.IsNotNull(found);
            Assert.AreEqual(p.ID, found.ID);
        }

        [TestMethod]
        public void Item2_NonExistent_ReturnsNull()
        {
            var src = CreateSRC();
            Assert.IsNull(src.PList.Item2("存在しないID"));
        }

        [TestMethod]
        public void Item2_DeadPilot_StillReturnsPilot()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "死亡Item2");
            p.Alive = false;
            var found = src.PList.Item2(p.ID);
            Assert.IsNotNull(found);
        }

        // ──────────────────────────────────────────────
        // AlivePilots
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AlivePilots_ExcludesDeadPilots()
        {
            var src = CreateSRC();
            var p1 = AddPilot(src, "生存者A");
            var p2 = AddPilot(src, "死亡者A");
            p2.Alive = false;
            var alive = src.PList.AlivePilots.ToList();
            Assert.AreEqual(1, alive.Count);
            Assert.AreEqual(p1.ID, alive[0].ID);
        }

        [TestMethod]
        public void AlivePilots_AllAlive_ReturnsAll()
        {
            var src = CreateSRC();
            AddPilot(src, "全員生存A");
            AddPilot(src, "全員生存B");
            Assert.AreEqual(2, src.PList.AlivePilots.Count());
        }

        [TestMethod]
        public void AlivePilots_NoneAlive_ReturnsEmpty()
        {
            var src = CreateSRC();
            var p = AddPilot(src, "全員死亡");
            p.Alive = false;
            Assert.AreEqual(0, src.PList.AlivePilots.Count());
        }

        // ──────────────────────────────────────────────
        // ItemsByGroupId
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemsByGroupId_WithoutFirst_ExcludesFirstPilot()
        {
            var src = CreateSRC();
            var p1 = AddPilot(src, "グループ先頭(ザコ)", gid: "gtest");
            var p2 = AddPilot(src, "グループ2番(ザコ)", gid: "gtest");
            var result = src.PList.ItemsByGroupId("gtest", without_first: true).ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(p2.ID, result[0].ID);
        }

        [TestMethod]
        public void ItemsByGroupId_IncludingFirst_ReturnsAll()
        {
            var src = CreateSRC();
            AddPilot(src, "グループ含先頭(ザコ)", gid: "ginc");
            AddPilot(src, "グループ含2番(ザコ)", gid: "ginc");
            var result = src.PList.ItemsByGroupId("ginc", without_first: false).ToList();
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ItemsByGroupId_NonExistentGroup_ReturnsEmpty()
        {
            var src = CreateSRC();
            var result = src.PList.ItemsByGroupId("nogroup", without_first: false).ToList();
            Assert.AreEqual(0, result.Count);
        }

        // ──────────────────────────────────────────────
        // Clean
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clean_RemovesDeadPilots()
        {
            var src = CreateSRC();
            var alive = AddPilot(src, "生存クリーン");
            var dead = AddPilot(src, "死亡クリーン");
            dead.Alive = false;
            src.PList.Clean();
            Assert.AreEqual(1, src.PList.Count());
            Assert.IsTrue(src.PList.IsDefined2(alive.ID));
            Assert.IsFalse(src.PList.IsDefined2(dead.ID));
        }

        [TestMethod]
        public void Clean_AllAlive_RemovesNothing()
        {
            var src = CreateSRC();
            AddPilot(src, "クリーン生存A");
            AddPilot(src, "クリーン生存B");
            src.PList.Clean();
            Assert.AreEqual(2, src.PList.Count());
        }

        [TestMethod]
        public void Clean_AllDead_RemovesAll()
        {
            var src = CreateSRC();
            var p1 = AddPilot(src, "クリーン死亡A");
            var p2 = AddPilot(src, "クリーン死亡B");
            p1.Alive = false;
            p2.Alive = false;
            src.PList.Clean();
            Assert.AreEqual(0, src.PList.Count());
        }
    }
}
