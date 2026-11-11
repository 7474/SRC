using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// UnitDataList.Load・ItemDataList.Load を元の SRC（UnitDataList.cls・ItemDataList.cls の Load）の動作と
    /// 突き合わせるテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class UnitItemDataLoadVBMatchTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static Stream ToStream(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

        private static void LoadUnit(SRC src, string text) => src.UDList.Load("robot.txt", ToStream(text));
        private static void LoadItem(SRC src, string text) => src.IDList.Load("item.txt", ToStream(text));
        private static void LoadPilot(SRC src, string text) => src.PDList.Load("pilot.txt", ToStream(text));

        private static string[] Errors(SRC src) => src.DataErrors.Select(x => x.msg).ToArray();

        private static string UnitWith(string name = "テスト", string feature = "特殊能力なし", string pilotNum = "1", string rest = "")
        {
            return name + "\n"
                + "テスト,てすと,汎用," + pilotNum + ",4\n"
                + "陸,4,M,1000,100\n"
                + feature + "\n"
                + "3000,100,1000,100\n"
                + "AAAA,test.bmp\n"
                + rest;
        }

        private static string ItemWith(string name = "テストアイテム", string feature = "特殊能力なし", string rest = "")
        {
            return name + "\n"
                + "テストアイテム,てすとあいてむ,汎用,片手\n"
                + feature + "\n"
                + "100,10,0,0,0\n"
                + rest;
        }

        private static string PilotWith(string name = "テスト", string rest = "")
        {
            return name + "\n"
                + "テスト,てすと,男性,汎用,AAAA,100\n"
                + "特殊能力なし\n"
                + "100,100,100,100,100,100,普通\n"
                + "ＳＰなし\n"
                + "test.bmp,-\n"
                + rest;
        }

        private static void AssertStopsWithoutMessage(System.Action load, string dname)
        {
            var ex = Assert.Throws<InvalidSrcDataException>(load);
            Assert.AreEqual("", ex.InvalidDataList.Single().msg);
            Assert.AreEqual(dname, ex.InvalidDataList.Single().dname);
        }

        // 7474/SRC#1526: 「Lv」の後が数値でないと、AddFeature の CDbl が実行時エラー 13 になり、
        // Load の ErrorHandler が原因を書かないデータエラーとして読み込みを打ち切る

        [TestMethod]
        public void Unit_NewFormat_NonNumericFeatureLevel_StopsLoading()
        {
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadUnit(src, UnitWith(feature: "特殊能力\nバリアLvabc")), "テスト");
        }

        [TestMethod]
        public void Unit_NewFormat_NonNumericFeatureLevelWithData_StopsLoading()
        {
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadUnit(src, UnitWith(feature: "特殊能力\nバリアLvx=バリア 1000")), "テスト");
        }

        [TestMethod]
        public void Unit_OldFormat_NonNumericFeatureLevel_StopsLoading()
        {
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadUnit(src, UnitWith(feature: "特殊能力,ＥＮ回復,バリアLv")), "テスト");
        }

        [TestMethod]
        public void Item_NewFormat_NonNumericFeatureLevel_StopsLoading()
        {
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadItem(src, ItemWith(feature: "特殊能力\n格闘強化Lvx")), "テストアイテム");
        }

        [TestMethod]
        public void Item_OldFormat_NonNumericFeatureLevel_StopsLoading()
        {
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadItem(src, ItemWith(feature: "特殊能力,格闘強化Lvx")), "テストアイテム");
        }

        [TestMethod]
        public void Pilot_FeatureSection_NonNumericFeatureLevel_StopsLoading()
        {
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadPilot(src, PilotWith(rest: "===\nバリアLvx\n")), "テスト");
        }

        [TestMethod]
        public void Unit_NumericFeatureLevel_Loads()
        {
            // 必要技能・必要条件を外してから「Lv」の後を読む
            var src = CreateSrc();
            LoadUnit(src, UnitWith(feature: "特殊能力\nバリアLv2 (底力),ＥＮ回復Lv1.5 <気力130>"));
            var ud = src.UDList.Item("テスト");

            Assert.AreEqual(2d, ud.FeatureLevel("バリア"));
            Assert.AreEqual(1.5d, ud.FeatureLevel("ＥＮ回復"));
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Unit_AbilityNonNumericEffectLevel_StopsLoading()
        {
            // AbilityData.SetEffect の CDbl も同じく実行時エラーになる
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadUnit(src, UnitWith(rest: "===\n修理,回復Lvx,1,1,-,-,-\n")), "テスト");
        }

        [TestMethod]
        public void Unit_AbilityEmptyEffectData_StopsLoading()
        {
            // 「回復=」のようにデータが空だと、SetEffect の Asc("") が実行時エラー 5 になる
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadUnit(src, UnitWith(rest: "===\n修理,回復=,1,1,-,-,-\n")), "テスト");
        }

        // 特殊能力の節（新形式）

        [TestMethod]
        public void Unit_NewFormat_BlankLine_ReportsAndReadsNextLine()
        {
            // 元の SRC は空行を空の特殊能力として「行頭から1番目の特殊能力の設定が間違っています。」を出し、次の行を読み続ける
            var src = CreateSrc();
            LoadUnit(src, UnitWith(feature: "特殊能力\nバリア\n\nＥＮ回復"));
            var ud = src.UDList.Item("テスト");

            CollectionAssert.AreEqual(new[] { "行頭から1番目の特殊能力の設定が間違っています。" }, Errors(src));
            CollectionAssert.AreEqual(new[] { "バリア", "ＥＮ回復" }, ud.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual(3000, ud.HP);
        }

        [TestMethod]
        public void Unit_NewFormat_NumericOnlyInIsNumeric_EndsSection()
        {
            // 数値（IsNumeric）の項目で節を終える。「1-2」は数値ではないので特殊能力として読む
            var src = CreateSrc();
            LoadUnit(src, UnitWith(feature: "特殊能力\nバリア,1-2"));
            var ud = src.UDList.Item("テスト");

            CollectionAssert.AreEqual(new[] { "バリア", "1-2" }, ud.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual(3000, ud.HP);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Item_NewFormat_NumericAfterFirstItem_IsFeature()
        {
            // アイテムは行頭の項目が数値のときだけ節を終える。2 番目以降の数値は特殊能力として読む
            var src = CreateSrc();
            LoadItem(src, ItemWith(feature: "特殊能力\nバリア,100"));
            var id = src.IDList.Item("テストアイテム");

            CollectionAssert.AreEqual(new[] { "バリア", "100" }, id.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual(100, id.HP);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Item_NewFormat_BlankLine_ReportsAndReadsNextLine()
        {
            var src = CreateSrc();
            LoadItem(src, ItemWith(feature: "特殊能力\nバリア\n\nＥＮ回復"));
            var id = src.IDList.Item("テストアイテム");

            CollectionAssert.AreEqual(new[] { "行頭から1番目の特殊能力の設定が間違っています。" }, Errors(src));
            CollectionAssert.AreEqual(new[] { "バリア", "ＥＮ回復" }, id.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual(100, id.HP);
        }

        // 武器・アビリティ

        [TestMethod]
        public void Unit_WeaponStartingWithAsterisk_IsWeapon()
        {
            // 「*」で始まる行で武器・アビリティを終えるのはアイテム（解説）だけ
            var src = CreateSrc();
            LoadUnit(src, UnitWith(rest: "*ビーム,1500,1,4,+0,-,10,-,AAAA,+0,B\n===\n*修理,回復Lv1,1,1,-,-,-\n"));
            var ud = src.UDList.Item("テスト");

            CollectionAssert.AreEqual(new[] { "*ビーム" }, ud.Weapons.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "*修理" }, ud.Abilities.Select(x => x.Name).ToArray());
            Assert.AreEqual(2, src.UDList.Count());
        }

        [TestMethod]
        public void Item_WeaponMinRange_IsCappedAt99()
        {
            // アイテムの最小射程は MinLng(CLng(...), 99)。ユニットは CInt のまま
            var src = CreateSrc();
            LoadItem(src, ItemWith(rest: "剣,1000,150,150,+0,-,-,-,AAAA,+0,武\n"));
            LoadUnit(src, UnitWith(rest: "剣,1000,150,150,+0,-,-,-,AAAA,+0,武\n"));

            Assert.AreEqual(99, src.IDList.Item("テストアイテム").Weapons.Single().MinRange);
            Assert.AreEqual(150, src.UDList.Item("テスト").Weapons.Single().MinRange);
        }

        [TestMethod]
        public void Item_AbilityWithoutComma_UsesItemMessage()
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => LoadItem(src, ItemWith(rest: "===\n修理\n")));

            Assert.AreEqual("アビリティデータの終りには空行を置いてください。", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Unit_AbilityWithoutComma_UsesUnitMessage()
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => LoadUnit(src, UnitWith(rest: "===\n修理\n")));

            Assert.AreEqual("アビリティデータの終りに空行を置いてください。", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Unit_PilotNumWithoutClosingParen_StopsWithoutMessage()
        {
            // 元の SRC は err_msg を設定せずに Error 0 を呼ぶ
            var src = CreateSrc();
            AssertStopsWithoutMessage(() => LoadUnit(src, UnitWith(pilotNum: "(2")), "テスト");
        }
    }

    /// <summary>
    /// UnitData・ItemData・AbilityData を元の SRC（UnitData.cls・ItemData.cls・AbilityData.cls）の動作と
    /// 突き合わせるテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class UnitItemAbilityDataVBMatchTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        public void ItemData_FeatureName_AddsLvWhenLevelSpecified()
        {
            // ItemData.cls は Level <> DEFAULT_LEVEL で「Lv」を付ける（UnitData.cls は Level > 0）
            var src = CreateSrc();
            var id = new ItemData(src);
            id.AddFeature("格闘強化Lv0");
            id.AddFeature("射撃強化Lv-1");
            id.AddFeature("バリア");

            Assert.AreEqual("格闘強化Lv0", id.FeatureName(1));
            Assert.AreEqual("射撃強化Lv-1", id.FeatureName("射撃強化"));
            Assert.AreEqual("バリア", id.FeatureName(3));

            var ud = new UnitData(src);
            ud.AddFeature("格闘強化Lv0");
            Assert.AreEqual("格闘強化", ud.FeatureName(1));
        }

        [TestMethod]
        public void FeatureLevelAndData_OutOfRangeIndex_ReturnsDefault()
        {
            // On Error GoTo ErrorHandler で 0・"" を返す
            var src = CreateSrc();
            var id = new ItemData(src);
            id.AddFeature("バリア=バリア 1000");
            var ud = new UnitData(src);
            ud.AddFeature("バリア=バリア 1000");

            Assert.AreEqual(0d, id.FeatureLevel(5));
            Assert.AreEqual("", id.FeatureData(5));
            Assert.AreEqual(0d, ud.FeatureLevel(5));
            Assert.AreEqual("", ud.FeatureData(5));
        }

        [TestMethod]
        public void ItemData_AddAbility_KeyUsesItemName()
        {
            // ItemData.cls はアイテムの名前と番号をキーにするので、アビリティの名前と番号が重ねても追加できる
            var src = CreateSrc();
            var id = new ItemData(src) { Name = "テストアイテム" };
            id.AddAbility("A1");
            for (var i = 0; i < 9; i++)
            {
                id.AddAbility("B");
            }
            id.AddAbility("A");

            Assert.AreEqual(11, id.CountAbility());
        }

        [TestMethod]
        public void AbilityData_EffectName_TurnsAreRoundedByMaxLng()
        {
            // 「最低でも１ターン持続」の MaxLng は Long の引数なので、ターン数は整数に丸めてから比べる
            var src = CreateSrc();
            var unit = src.UDList.Add("召喚ユニット");
            unit.Nickname = "召喚体";
            var ad = new AbilityData(src);
            ad.SetEffect("状態Lv10.4=攻撃不能 召喚Lv1.4=召喚ユニット");

            CollectionAssert.AreEqual(new[]
            {
                "攻撃不能(10ターン)",
                "召喚体を召喚",
            }, ad.Effects.Select(x => ad.EffectName(x)).ToArray());
        }
    }
}
