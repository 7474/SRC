using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// alias.txt で定義したエリアスによる特殊能力の展開
    /// （元の SRC の UnitData.AddFeature / PilotData.AddFeature / ItemData.AddFeature）
    /// </summary>
    [TestClass]
    public class FeatureAliasExpansionTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };

            // レベルの加算
            src.ALDList.Add("強化バリア").AddAlias("バリアLv+1=バリア 1000 10");
            // レベルの乗算（データ中の Lv1 も置き換える）
            src.ALDList.Add("超回復").AddAlias("ＥＮ回復Lv*2=回復 Lv1");
            // データの連結
            src.ALDList.Add("剣").AddAlias("格闘武器=ソード 格闘 強");
            // 特殊能力解説と複数の展開
            var ad = src.ALDList.Add("超能力");
            ad.AddAlias("超能力=解説 説明文");
            ad.AddAlias("バリアLv1=サイコバリア 500");
            return src;
        }

        private static void AssertFeature(FeatureData fd, string name, double level, string data)
        {
            Assert.IsNotNull(fd, name);
            Assert.AreEqual(name, fd.Name);
            Assert.AreEqual(level, fd.Level);
            Assert.AreEqual(data, fd.Data);
        }

        [TestMethod]
        public void UnitData_PlusLevel()
        {
            var src = CreateSrc();
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("強化バリアLv2");

            Assert.AreEqual(1, ud.Features.Count);
            AssertFeature(ud.Features.Single(), "バリア", 3d, "バリア 1000 10");
            Assert.IsFalse(ud.IsFeatureAvailable("強化バリア"));
        }

        [TestMethod]
        public void UnitData_PlusLevel_DefaultLevelIsOne()
        {
            var src = CreateSrc();
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("強化バリア");

            AssertFeature(ud.Features.Single(), "バリア", 2d, "バリア 1000 10");
        }

        [TestMethod]
        public void UnitData_MultLevel_ReplacesLv1InData()
        {
            var src = CreateSrc();
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("超回復Lv3");

            AssertFeature(ud.Features.Single(), "ＥＮ回復", 6d, "回復 Lv3");
        }

        [TestMethod]
        public void UnitData_ConcatenatesData()
        {
            var src = CreateSrc();
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("剣=ナイフ");

            AssertFeature(ud.Features.Single(), "格闘武器", Constants.DEFAULT_LEVEL, "ナイフ 格闘 強");
        }

        [TestMethod]
        public void UnitData_DescriptionRenamesAndExpands()
        {
            var src = CreateSrc();
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("超能力=サイキック");

            Assert.AreEqual(2, ud.Features.Count);
            Assert.AreEqual("サイキック", ud.Features[0].Name);
            Assert.AreEqual("解説 説明文", ud.Features[0].Data);
            AssertFeature(ud.Features[1], "バリア", 1d, "サイキック 500");
            Assert.IsFalse(ud.Features.Any(x => x.Name == "超能力"));
        }

        [TestMethod]
        public void UnitData_DescriptionDataIsNotExpanded()
        {
            // 「解説」で始まるデータはエリアスを展開せずそのまま登録する
            var src = CreateSrc();
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("超能力=解説 独自の説明");

            AssertFeature(ud.Features.Single(), "超能力", Constants.DEFAULT_LEVEL, "解説 独自の説明");
        }

        [TestMethod]
        public void UnitData_InheritsNecessarySkillFromAlias()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.ALDList.Add("条件付きバリア").AddAlias("バリア=バリア 1000 (超能力)");
            var ud = src.UDList.Add("テスト機体");
            ud.AddFeature("条件付きバリア");
            ud.AddFeature("条件付きバリア (念動力)");

            Assert.AreEqual("超能力", ud.Features[0].NecessarySkill);
            Assert.AreEqual("念動力", ud.Features[1].NecessarySkill);
        }

        [TestMethod]
        public void PilotData_ExpandsAlias()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("テストパイロット");
            pd.AddFeature("強化バリアLv2");
            pd.AddFeature("剣=ナイフ");

            Assert.AreEqual(2, pd.Features.Count);
            AssertFeature(pd.Features[0], "バリア", 3d, "バリア 1000 10");
            AssertFeature(pd.Features[1], "格闘武器", Constants.DEFAULT_LEVEL, "ナイフ 格闘 強");
            Assert.IsFalse(pd.IsFeatureAvailable("強化バリア"));
        }

        [TestMethod]
        public void ItemData_ExpandsAlias()
        {
            var src = CreateSrc();
            var itd = src.IDList.Add("テストアイテム");
            itd.AddFeature("超回復Lv3");
            itd.AddFeature("超能力=サイキック");

            Assert.AreEqual(3, itd.Features.Count);
            AssertFeature(itd.Features[0], "ＥＮ回復", 6d, "回復 Lv3");
            Assert.AreEqual("サイキック", itd.Features[1].Name);
            AssertFeature(itd.Features[2], "バリア", 1d, "サイキック 500");
            Assert.IsTrue(itd.IsFeatureAvailable("ＥＮ回復"));
            Assert.IsFalse(itd.IsFeatureAvailable("超回復"));
        }
    }
}
