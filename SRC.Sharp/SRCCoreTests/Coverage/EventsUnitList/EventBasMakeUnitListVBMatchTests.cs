using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.EventsUnitList
{
    /// <summary>
    /// Event.MakeUnitList の並べ替えを、元の SRC（VB: Event.bas MakeUnitList）の動作を期待値にして確かめる（#1498）。
    /// </summary>
    [TestClass]
    public class EventBasMakeUnitListVBMatchTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC();
            new HeadlessGUI().Install(src);
            src.GUI.MainWidth = 20;
            src.GUI.MainHeight = 45;
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private static Unit AddUnit(SRC src, string name, int hp, string kana = null, bool withPilot = true)
        {
            var ud = src.UDList.Add(name);
            ud.Nickname = name;
            ud.KanaName = kana ?? name;
            ud.HP = hp;
            ud.EN = 100;
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            var u = src.UList.Add(name, 0, "味方");
            u.Status = "待機";
            if (withPilot)
            {
                var pd = src.PDList.Add(name + "の乗り手");
                pd.Nickname = pd.Name;
                pd.KanaName = pd.Name;
                pd.SP = 10;
                pd.Adaption = "AAAA";
                src.PList.Add(pd.Name, 1, "味方").Ride(u);
            }
            return u;
        }

        // VB は選択ソート（大きいものを前と入れ替える）なので、同じ値の並びは入れ替えの影響を受ける。
        // [A=1000, B=1000, C=2000] → C と A を入れ替えて [C, B, A]
        [TestMethod]
        public void MakeUnitList_TiesFollowVBSelectionSort()
        {
            var src = CreateSrc();
            var a = AddUnit(src, "機体A", 1000);
            var b = AddUnit(src, "機体B", 1000);
            var c = AddUnit(src, "機体C", 2000);

            src.Event.MakeUnitList("ＨＰ");

            Assert.AreEqual(1, c.x);
            Assert.AreEqual(6, b.x);
            Assert.AreEqual(11, a.x);
        }

        // 「名称」も選択ソート（StrComp(…, vbTextCompare) = -1 で小さいものを前と入れ替える）。
        // 読み仮名の比較は大文字・小文字、ひらがな・カタカナを区別しない
        [TestMethod]
        public void MakeUnitList_NameSortUsesTextCompareSelectionSort()
        {
            var src = CreateSrc();
            var x = AddUnit(src, "機体X", 1000, kana: "い");
            var y = AddUnit(src, "機体Y", 1000, kana: "イ");
            var z = AddUnit(src, "機体Z", 1000, kana: "あ");

            src.Event.MakeUnitList("名称");

            // [い, イ, あ] → あ と い を入れ替えて [あ, イ, い]（イ と い は同じとみなすので入れ替えない）
            Assert.AreEqual(1, z.x);
            Assert.AreEqual(6, y.x);
            Assert.AreEqual(11, x.x);
        }

        // VB は「名称」以外ではメインパイロットの読み仮名を見ない。
        // 等身大基準でパイロットの乗っていないユニットがあっても、ＨＰの一覧は作れる（ダミーパイロットを乗せる）
        [TestMethod]
        public void MakeUnitList_HPSortWithLifeSizeOption_DoesNotNeedPilot()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(等身大基準)");
            var empty = AddUnit(src, "無人機", 1000, withPilot: false);

            src.Event.MakeUnitList("ＨＰ");

            Assert.AreEqual("出撃", empty.Status);
            Assert.AreEqual(1, empty.CountPilot());
        }
    }
}
