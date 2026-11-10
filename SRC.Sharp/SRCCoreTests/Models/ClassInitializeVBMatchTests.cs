using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;
using SRCCore.Units;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// クラスモジュール（*.cls）の Class_Initialize で入れる初期値が VB と同じかのテスト（7474/SRC#1498 のモジュールの宣言）
    /// </summary>
    [TestClass]
    public class ClassInitializeVBMatchTests
    {
        [TestMethod]
        public void Unit_Status_は待機_BossRank_は_マイナス1()
        {
            // VB: Unit.cls の Class_Initialize（Status = "待機"、intBossRank = -1、Summoner・Master は Nothing）
            var u = new Unit(new SRC());
            Assert.AreEqual("待機", u.Status);
            Assert.AreEqual(-1, u.BossRank);
            Assert.IsNull(u.Summoner);
            Assert.IsNull(u.Master);
            Assert.AreEqual(0, u.UsedAction);
        }

        [TestMethod]
        public void Item_Exist_と_Activated_は_True()
        {
            // VB: Item.cls の Class_Initialize（Exist = True、Activated = True、Data・Unit は Nothing）
            var itm = new Item(new SRC());
            Assert.IsTrue(itm.Exist);
            Assert.IsTrue(itm.Activated);
            Assert.IsNull(itm.ItemName);
            Assert.IsNull(itm.UnitId);
        }

        [TestMethod]
        public void TerrainData_ID_は_マイナス1()
        {
            // VB: TerrainData.cls の Class_Initialize（ID = -1）
            Assert.AreEqual(-1, new TerrainData().ID);
        }

        [TestMethod]
        public void BattleConfigData_ConfigCalc_は空文字列()
        {
            // VB: BattleConfigData.cls の Class_Initialize（ConfigCalc = ""）
            Assert.AreEqual("", new BattleConfigData(new SRC()).ConfigCalc);
        }

        [TestMethod]
        public void Units_IDCount_は0()
        {
            // VB: Units.cls の Class_Initialize（IDCount = 0）
            Assert.AreEqual(0, new Units.Units(new SRC()).IDCount);
        }

        [TestMethod]
        public void データリストはダミーのデータを1つ持つ()
        {
            // VB: PilotDataList・UnitDataList・NonPilotDataList の Class_Initialize で入れるダミーのデータ
            var src = new SRC();

            Assert.AreEqual(1, src.PDList.Count());
            var pd = src.PDList.Item("ステータス表示用ダミーパイロット(ザコ)");
            Assert.AreEqual("パイロット不在", pd.Nickname);
            Assert.AreEqual("AAAA", pd.Adaption);
            Assert.AreEqual(".bmp", pd.Bitmap0);

            Assert.AreEqual(1, src.UDList.Count());
            var ud = src.UDList.Item("ステータス表示用ダミーユニット");
            Assert.AreEqual("ユニット無し", ud.Nickname);
            Assert.AreEqual(1, ud.PilotNum);
            Assert.AreEqual("陸", ud.Transportation);
            Assert.AreEqual("AAAA", ud.Adaption);
            Assert.AreEqual(".bmp", ud.Bitmap0);
            Assert.IsTrue(ud.IsFeatureAvailable("ダミーユニット"));
            // ダミーは UnitDataList.Add を通らないので ID は 0
            Assert.AreEqual(0, ud.ID);

            Assert.AreEqual(1, src.NPDList.Count());
            var npd = src.NPDList.Item("ナレーター");
            Assert.AreEqual("ナレーター", npd.Nickname0);
            Assert.AreEqual(".bmp", npd.Bitmap0);
        }
    }
}
