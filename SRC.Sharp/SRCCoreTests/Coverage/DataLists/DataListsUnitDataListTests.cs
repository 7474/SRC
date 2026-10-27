using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// UnitDataList.Load (robot.txt / ユニットデータ) の読み込み仕様のテスト。
    /// VB 版 UnitDataList.cls の Load を仕様として、各項目の解釈・既定値・上限・エラー報告を確認する。
    /// </summary>
    [TestClass]
    public class DataListsUnitDataListTests
    {
        private static SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static UnitDataList Load(SRC src, string text, string fname = "robot.txt")
        {
            src.UDList.Load(fname, new MemoryStream(Encoding.UTF8.GetBytes(text)));
            return src.UDList;
        }

        private static string[] Errors(SRC src) => src.DataErrors.Select(x => x.msg).ToArray();

        // 標準的なユニットデータ（武器・アビリティ付き）
        private const string Gundam =
            "ガンダム\n" +
            "ガンダム,がんだむ,ＭＳ,1,2\n" +
            "陸,4,M,4000,150\n" +
            "特殊能力\n" +
            "剣装備,シールド\n" +
            "分身Lv2\n" +
            "3500,180,1200,75\n" +
            "BBAA,gundam.bmp\n" +
            "ビームサーベル,1400,1,1,+20,-,-,-,AABA,+10,武\n" +
            "ビームライフル,1500,2,5,+10,8,10,110,AABA,+0,B\n" +
            "===\n" +
            "修理装置,回復Lv2,1,3,40,-,-\n";

        // 基本項目の後ろに任意の行を付けたユニットデータを作る
        private static string UnitWith(string line2 = "テスト機,ＭＳ,1,2", string line3 = "陸,4,M,4000,150",
            string feature = "特殊能力なし", string status = "3500,180,1200,75", string line5 = "BBAA,test.bmp",
            string rest = "")
        {
            return "テスト機\n" + line2 + "\n" + line3 + "\n" + feature + "\n" + status + "\n" + line5 + "\n" + rest;
        }

        [TestMethod]
        public void Load_PngJpgGifBitmap_Accepted()
        {
            // 元の SRC は .bmp だけだが、SRC# は .png・.jpg・.gif も受け付ける（#1212）
            foreach (var name in new[] { "test.png", "test.JPG", "test.gif" })
            {
                var src = CreateSrc();
                var ud = Load(src, UnitWith(line5: "BBAA," + name)).Item("テスト機");
                Assert.AreEqual(name, ud.Bitmap);
                Assert.IsFalse(ud.IsBitmapMissing, name);
            }
        }

        [TestMethod]
        public void Load_StandardUnit_ParsesAllBasicFields()
        {
            var src = CreateSrc();
            var ud = Load(src, Gundam).Item("ガンダム");

            Assert.IsNotNull(ud);
            Assert.AreEqual("ガンダム", ud.Nickname);
            Assert.AreEqual("がんだむ", ud.KanaName);
            Assert.AreEqual("ＭＳ", ud.Class);
            Assert.AreEqual(1, ud.PilotNum);
            Assert.AreEqual(2, ud.ItemNum);
            Assert.AreEqual("陸", ud.Transportation);
            Assert.AreEqual(4, ud.Speed);
            Assert.AreEqual("M", ud.Size);
            Assert.AreEqual(4000, ud.Value);
            Assert.AreEqual(150, ud.ExpValue);
            Assert.AreEqual(3500, ud.HP);
            Assert.AreEqual(180, ud.EN);
            Assert.AreEqual(1200, ud.Armor);
            Assert.AreEqual(75, ud.Mobility);
            Assert.AreEqual("BBAA", ud.Adaption);
            Assert.AreEqual("gundam.bmp", ud.Bitmap);
            Assert.IsFalse(ud.IsBitmapMissing);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_StandardUnit_ParsesNewFormatFeaturesAcrossLines()
        {
            // 新形式の特殊能力は数値のみの行（最大ＨＰ等）が現れるまで複数行にわたって読む
            var src = CreateSrc();
            var ud = Load(src, Gundam).Item("ガンダム");

            CollectionAssert.AreEqual(new[] { "剣装備", "シールド", "分身" }, ud.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual(2d, ud.FeatureLevel("分身"));
        }

        [TestMethod]
        public void Load_StandardUnit_ParsesWeapons()
        {
            var src = CreateSrc();
            var ud = Load(src, Gundam).Item("ガンダム");

            Assert.AreEqual(2, ud.Weapons.Count);
            var saber = ud.Weapons[0];
            Assert.AreEqual("ビームサーベル", saber.Name);
            Assert.AreEqual(1400, saber.Power);
            Assert.AreEqual(1, saber.MinRange);
            Assert.AreEqual(1, saber.MaxRange);
            Assert.AreEqual(20, saber.Precision);
            // 「-」指定の弾数・消費ＥＮ・必要気力は 0 (制限なし)
            Assert.AreEqual(0, saber.Bullet);
            Assert.AreEqual(0, saber.ENConsumption);
            Assert.AreEqual(0, saber.NecessaryMorale);
            Assert.AreEqual("AABA", saber.Adaption);
            Assert.AreEqual(10, saber.Critical);
            Assert.AreEqual("武", saber.Class);

            var rifle = ud.Weapons[1];
            Assert.AreEqual(2, rifle.MinRange);
            Assert.AreEqual(5, rifle.MaxRange);
            Assert.AreEqual(8, rifle.Bullet);
            Assert.AreEqual(10, rifle.ENConsumption);
            Assert.AreEqual(110, rifle.NecessaryMorale);
            Assert.AreEqual(0, rifle.Critical);
            Assert.AreEqual("B", rifle.Class);
        }

        [TestMethod]
        public void Load_StandardUnit_ParsesAbilitiesAfterSeparator()
        {
            // 「===」の後はアビリティデータ: 名称,効果,射程,回数,消費ＥＮ,必要気力,属性
            var src = CreateSrc();
            var ud = Load(src, Gundam).Item("ガンダム");

            Assert.AreEqual(1, ud.Abilities.Count);
            var ab = ud.Abilities[0];
            Assert.AreEqual("修理装置", ab.Name);
            Assert.AreEqual("回復", ab.Effects.Single().Name);
            Assert.AreEqual(2d, ab.Effects.Single().Level);
            Assert.AreEqual(0, ab.MinRange);
            Assert.AreEqual(1, ab.MaxRange);
            Assert.AreEqual(3, ab.Stock);
            Assert.AreEqual(40, ab.ENConsumption);
            Assert.AreEqual(0, ab.NecessaryMorale);
            Assert.AreEqual("", ab.Class);
        }

        [TestMethod]
        public void Load_KanaOmitted_DerivesKanaFromNickname()
        {
            // 2行目のコンマが3個の場合、読み仮名は愛称をひらがな化したもの
            var src = CreateSrc();
            var ud = Load(src, UnitWith(line2: "ザク,ＭＳ,1,2")).Item("テスト機");

            Assert.AreEqual("ザク", ud.Nickname);
            Assert.AreEqual("ざく", ud.KanaName);
        }

        [TestMethod]
        public void Load_ParenthesizedPilotNum_IsStoredAsNegative()
        {
            // パイロット数を括弧で囲んだ場合は負数で保持する (VB: .PilotNum = -.PilotNum)
            var src = CreateSrc();
            var ud = Load(src, UnitWith(line2: "テスト機,ＭＳ,(2),2")).Item("テスト機");

            Assert.AreEqual(-2, ud.PilotNum);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_ParenthesizedPilotNumWithoutClosing_Throws()
        {
            var src = CreateSrc();
            // 閉じ括弧が無い場合は読み込みを中断する (VB: Error 0)。
            // VB は err_msg を設定せずに中断するためメッセージ文言は C# 独自であり検証しない
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, UnitWith(line2: "テスト機,ＭＳ,(2,2")));
            var err = ex.InvalidDataList.Single();
            Assert.AreEqual("robot.txt", err.fname);
            Assert.AreEqual(2, err.line_num);
        }

        [TestMethod]
        public void Load_ZeroPilotNum_ReportsErrorAndUsesOne()
        {
            var src = CreateSrc();
            var ud = Load(src, UnitWith(line2: "テスト機,ＭＳ,0,2")).Item("テスト機");

            Assert.AreEqual(1, ud.PilotNum);
            CollectionAssert.Contains(Errors(src), "パイロット数の設定が間違っています。");
        }

        [TestMethod]
        public void Load_InvalidValues_ReportErrorsAndFallBackToDefaults()
        {
            // 不正値は DataErrorMessage を出しつつ既定値で読み込みを継続する
            var src = CreateSrc();
            var ud = Load(src, UnitWith(
                line2: "テスト機,1,X,Y",
                line3: "2,X,XXL,X,X",
                status: "X,X,X,X",
                line5: "ABC,test.txt")).Item("テスト機");

            Assert.AreEqual("汎用", ud.Class);
            Assert.AreEqual(1, ud.PilotNum);
            Assert.AreEqual(4, ud.ItemNum);
            Assert.AreEqual("陸", ud.Transportation);
            Assert.AreEqual("M", ud.Size);
            Assert.AreEqual(1000, ud.HP);
            Assert.AreEqual(100, ud.EN);
            Assert.AreEqual("AAAA", ud.Adaption);
            Assert.IsTrue(ud.IsBitmapMissing);

            var errors = Errors(src);
            CollectionAssert.IsSubsetOf(new[]
            {
                "ユニットクラスの設定が間違っています。",
                "パイロット数の設定が間違っています。",
                "アイテム数の設定が間違っています。",
                "移動可能地形の設定が間違っています。",
                "移動力の設定が間違っています。",
                "サイズの設定が間違っています。",
                "修理費の設定が間違っています。",
                "経験値の設定が間違っています。",
                "最大ＨＰの設定が間違っています。",
                "最大ＥＮの設定が間違っています。",
                "装甲の設定が間違っています。",
                "運動性の設定が間違っています。",
                "地形適応の設定が間違っています。",
                "ビットマップの設定が間違っています。",
            }, errors);
            // エラーにはユニット名とファイル名が記録される
            Assert.IsTrue(src.DataErrors.All(x => x.dname == "テスト機" && x.fname == "robot.txt"));
        }

        [TestMethod]
        public void Load_DataError_RecordsLineNumberAndLine()
        {
            var src = CreateSrc();
            Load(src, UnitWith(line5: "ABC,test.bmp"));

            var err = src.DataErrors.Single();
            Assert.AreEqual("地形適応の設定が間違っています。", err.msg);
            Assert.AreEqual(6, err.line_num);
            Assert.AreEqual("ABC,test.bmp", err.line_buf);
        }

        [TestMethod]
        public void Load_ValuesOverLimit_AreClamped()
        {
            // VB: MinLng による上限 (移動力99, 修理費9999999, 経験値9999, HP9999999, EN9999, 装甲99999, 運動性9999, パイロット数/アイテム数99)
            var src = CreateSrc();
            var ud = Load(src, UnitWith(
                line2: "テスト機,ＭＳ,150,150",
                line3: "陸,150,LL,99999999,99999",
                status: "99999999,99999,999999,99999")).Item("テスト機");

            Assert.AreEqual(99, ud.PilotNum);
            Assert.AreEqual(99, ud.ItemNum);
            Assert.AreEqual(99, ud.Speed);
            Assert.AreEqual("LL", ud.Size);
            Assert.AreEqual(9999999, ud.Value);
            Assert.AreEqual(9999, ud.ExpValue);
            Assert.AreEqual(9999999, ud.HP);
            Assert.AreEqual(9999, ud.EN);
            Assert.AreEqual(99999, ud.Armor);
            Assert.AreEqual(9999, ud.Mobility);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        [DataRow("テスト 機\nテスト機,ＭＳ,1,2\n", "名称に半角スペースは使用出来ません。")]
        [DataRow("テスト（改）\nテスト機,ＭＳ,1,2\n", "名称に全角括弧は使用出来ません。")]
        [DataRow("テスト機,てすと,き\nテスト機,ＭＳ,1,2\n", "読み仮名の後に余分なデータが指定されています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1\n", "設定に抜けがあります。")]
        [DataRow("テスト機\nテスト機,てすと,ＭＳ,1,2,3\n", "余分な「,」があります。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,\n", "アイテム数の設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸\n", "移動力の設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4\n", "サイズの設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4,M,4000,\n", "経験値の設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4,M,4000,150\n3500,180,1200,75\n", "特殊能力の設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n3500\n", "最大ＥＮの設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n3500,180,1200,\n", "運動性の設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n3500,180,1200,75\nBBAA\n", "ビットマップの設定が抜けています。")]
        [DataRow("テスト機\nテスト機,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n3500,180,1200,75\nBBAA,\n", "ビットマップの設定が抜けています。")]
        public void Load_MalformedUnit_ThrowsWithMessage(string text, string expected)
        {
            // 致命的な書式エラーは InvalidSrcDataException で読み込みを中断する (VB: Error 0 → ErrorHandler)
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, text));
            Assert.AreEqual(expected, ex.InvalidDataList.Single().msg);
            Assert.AreEqual("robot.txt", ex.InvalidDataList.Single().fname);
        }

        [TestMethod]
        public void Load_NoFeatures_LeavesFeatureListEmpty()
        {
            var src = CreateSrc();
            var ud = Load(src, UnitWith()).Item("テスト機");
            Assert.AreEqual(0, ud.CountFeature());
            Assert.AreEqual(3500, ud.HP);
        }

        [TestMethod]
        public void Load_OldFormatFeatures_RespectsQuotedComma()
        {
            // 旧形式「特殊能力,…」: 「"」で囲まれた中のコンマは区切りとみなさない
            var src = CreateSrc();
            var ud = Load(src, UnitWith(feature: "特殊能力,解説=\"a,b\",シールド")).Item("テスト機");

            CollectionAssert.AreEqual(new[] { "解説", "シールド" }, ud.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual("a,b", ud.Feature("解説").StrData);
        }

        [TestMethod]
        public void Load_OldFormatThreeFeatures_SplitsEachFeature()
        {
            // VB: 最初のコンマで Exit For し、以降も1つずつ特殊能力として登録する
            var src = CreateSrc();
            var ud = Load(src, UnitWith(feature: "特殊能力,剣装備,シールド,超回避")).Item("テスト機");

            CollectionAssert.AreEqual(new[] { "剣装備", "シールド", "超回避" }, ud.Features.Select(x => x.Name).ToArray());
        }

        [TestMethod]
        public void Load_NewFormatThreeFeaturesOnOneLine_SplitsEachFeature()
        {
            var src = CreateSrc();
            var ud = Load(src, UnitWith(feature: "特殊能力\n剣装備,シールド,超回避")).Item("テスト機");

            CollectionAssert.AreEqual(new[] { "剣装備", "シールド", "超回避" }, ud.Features.Select(x => x.Name).ToArray());
        }

        [TestMethod]
        public void Load_OldFormatEmptyFeature_ReportsError()
        {
            // 旧形式で末尾の特殊能力が空なら「n番目の特殊能力の設定が間違っています。」
            var src = CreateSrc();
            var ud = Load(src, UnitWith(feature: "特殊能力,シールド,")).Item("テスト機");

            CollectionAssert.AreEqual(new[] { "シールド" }, ud.Features.Select(x => x.Name).ToArray());
            CollectionAssert.Contains(Errors(src), "2番目の特殊能力の設定が間違っています。");
        }

        [TestMethod]
        public void Load_WeaponValueClamps()
        {
            // 命中率・クリティカル率は ±999、必要気力は 0〜1000、最大射程は 99、攻撃力は 99999 に制限
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest:
                "A,999999,1,150,+1500,150,9999,2000,AAAA,-1500,B\n" +
                "B,100,1,1,-1500,-,-,-5,AAAA,+1500,B\n")).Item("テスト機");

            var a = ud.Weapons[0];
            Assert.AreEqual(99999, a.Power);
            Assert.AreEqual(99, a.MaxRange);
            Assert.AreEqual(999, a.Precision);
            Assert.AreEqual(99, a.Bullet);
            Assert.AreEqual(999, a.ENConsumption);
            Assert.AreEqual(1000, a.NecessaryMorale);
            Assert.AreEqual(-999, a.Critical);

            var b = ud.Weapons[1];
            Assert.AreEqual(-999, b.Precision);
            Assert.AreEqual(0, b.NecessaryMorale);
            Assert.AreEqual(999, b.Critical);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_WeaponClassWithNecessarySkill()
        {
            // 属性の末尾「(技能)」は必要技能として切り出す
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "ファンネル,1600,1,6,+20,6,-,120,AAAA,+10,Ｐ (ニュータイプ)\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual("Ｐ", w.Class);
            Assert.AreEqual("ニュータイプ", w.NecessarySkill);
        }

        [TestMethod]
        public void Load_WeaponClassWithNecessaryCondition()
        {
            // 属性の末尾「<条件>」は必要条件として切り出す
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "必殺,3000,1,1,+0,1,-,-,AAAA,+0,武 <合体技>\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual("武", w.Class);
            Assert.AreEqual("合体技", w.NecessaryCondition);
        }

        [TestMethod]
        public void Load_WeaponClassWithConditionAndSkill()
        {
            // 「<条件> (技能)」の両方が付いた場合は両方を切り出す
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "必殺,3000,1,1,+0,1,-,-,AAAA,+0,武 <合体技> (ニュータイプ)\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual("武", w.Class);
            Assert.AreEqual("合体技", w.NecessaryCondition);
            Assert.AreEqual("ニュータイプ", w.NecessarySkill);
        }

        [TestMethod]
        public void Load_WeaponClassHyphen_IsEmptyClass()
        {
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "パンチ,800,1,1,+0,-,-,-,AAAA,+0,-\n")).Item("テスト機");
            Assert.AreEqual("", ud.Weapons.Single().Class);
        }

        [TestMethod]
        public void Load_WeaponClassWithLv_ReportsError()
        {
            var src = CreateSrc();
            Load(src, UnitWith(rest: "パンチ,800,1,1,+0,-,-,-,AAAA,+0,格Lv2\n"));
            CollectionAssert.Contains(Errors(src), "パンチの属性のレベル指定が間違っています。");
        }

        [TestMethod]
        public void Load_WeaponFullWidthSpaceAroundNumbers_ReadsValuesLikeVb()
        {
            // 桁揃えの全角の空白（7474/SRC#1323）。VB は全角を半角にして読むので、エラーにせず値をそのまま読む
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "ビーム,　1400,　1,5　,+10　,　8,　10,　110,AABA,+0　,B\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual(1400, w.Power);
            Assert.AreEqual(1, w.MinRange);
            Assert.AreEqual(5, w.MaxRange);
            Assert.AreEqual(10, w.Precision);
            Assert.AreEqual(8, w.Bullet);
            Assert.AreEqual(10, w.ENConsumption);
            Assert.AreEqual(110, w.NecessaryMorale);
            Assert.AreEqual(0, w.Critical);
            CollectionAssert.AreEqual(new string[0], Errors(src));
        }

        [TestMethod]
        public void Load_WeaponFullWidthDigits_ReadsValuesLikeVb()
        {
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "ビーム,１４００,1,5,+10,8,10,110,AABA,+0,B\n")).Item("テスト機");

            Assert.AreEqual(1400, ud.Weapons.Single().Power);
            CollectionAssert.AreEqual(new string[0], Errors(src));
        }

        [TestMethod]
        public void Load_WeaponFullWidthSpaceInsideNumber_ReportsErrorWithoutSplitting()
        {
            // 「1400　1」は VB でも数値でない。VB の LLength は全角の空白で分けないので、半角の空白のような復旧はしない
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "ビーム,1400　1,5,+10,8,10,110,AABA,+0,B,-\n")).Item("テスト機");

            Assert.AreEqual(0, ud.Weapons.Single().Power);
            CollectionAssert.Contains(Errors(src), "ビームの攻撃力の設定が間違っています。");
        }

        [TestMethod]
        public void Load_WeaponMissingComma_RecoversFromSpaceSeparatedValue()
        {
            // 「1400 1」のようにコンマが抜けていた場合、エラーを出しつつ空白で分割して後続の項目に回す
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "ビーム,1400 1,5,+10,8,10,110,AABA,+0,B\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual(1400, w.Power);
            Assert.AreEqual(1, w.MinRange);
            Assert.AreEqual(5, w.MaxRange);
            Assert.AreEqual(10, w.Precision);
            Assert.AreEqual(8, w.Bullet);
            Assert.AreEqual("AABA", w.Adaption);
            Assert.AreEqual("B", w.Class);
            CollectionAssert.AreEqual(new[] { "ビームの攻撃力の設定が間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_WeaponInvalidAdaption_ReportsErrorAndUsesHyphens()
        {
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "ビーム,1400,1,5,+10,8,10,110,AB,+0,B\n")).Item("テスト機");

            Assert.AreEqual("----", ud.Weapons.Single().Adaption);
            CollectionAssert.Contains(Errors(src), "ビームの地形適応の設定が間違っています。");
        }

        [TestMethod]
        [DataRow("ビーム\n", "武器データの終りには空行を置いてください。")]
        [DataRow(",1400,1,5,+10,8,10,110,AABA,+0,B\n", "武器名の設定が間違っています。")]
        [DataRow("ビーム,1400\n", "ビームの最小射程が抜けています。")]
        [DataRow("ビーム,1400,1\n", "ビームの最大射程の設定が抜けています。")]
        [DataRow("ビーム,1400,1,5\n", "ビームの命中率の設定が抜けています。")]
        [DataRow("ビーム,1400,1,5,+10\n", "ビームの弾数の設定が抜けています。")]
        [DataRow("ビーム,1400,1,5,+10,8\n", "ビームの消費ＥＮの設定が抜けています。")]
        [DataRow("ビーム,1400,1,5,+10,8,10\n", "ビームの必要気力が抜けています。")]
        [DataRow("ビーム,1400,1,5,+10,8,10,110\n", "ビームの地形適応が抜けています。")]
        [DataRow("ビーム,1400,1,5,+10,8,10,110,AABA\n", "ビームのクリティカル率が抜けています。")]
        [DataRow("ビーム,1400,1,5,+10,8,10,110,AABA,+0\n", "ビームの武器属性が抜けています。")]
        public void Load_MalformedWeapon_ThrowsWithMessage(string weaponLine, string expected)
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, UnitWith(rest: weaponLine)));
            Assert.AreEqual(expected, ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Load_AbilityWithSkillAndHyphenRange()
        {
            // アビリティの射程「-」は 0 (自分のみ)、属性末尾の「(技能)」は必要技能
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest:
                "パンチ,800,1,1,+0,-,-,-,AAAA,+0,-\n===\n" +
                "変形,変形=テスト機改,-,2,10,120,Ｍ (変形技能)\n")).Item("テスト機");

            var ab = ud.Abilities.Single();
            Assert.AreEqual(0, ab.MaxRange);
            Assert.AreEqual(2, ab.Stock);
            Assert.AreEqual(10, ab.ENConsumption);
            Assert.AreEqual(120, ab.NecessaryMorale);
            Assert.AreEqual("Ｍ", ab.Class);
            Assert.AreEqual("変形技能", ab.NecessarySkill);
        }

        [TestMethod]
        public void Load_AbilityInvalidValues_ReportErrors()
        {
            var src = CreateSrc();
            Load(src, UnitWith(rest:
                "パンチ,800,1,1,+0,-,-,-,AAAA,+0,-\n===\n" +
                "回復,回復Lv1,X,X,X,X,回Lv1\n"));

            CollectionAssert.IsSubsetOf(new[]
            {
                "回復の射程の設定が間違っています。",
                "回復の回数の設定が間違っています。",
                "回復の消費ＥＮの設定が間違っています。",
                "回復の必要気力の設定が間違っています。",
                "回復の属性のレベル指定が間違っています。",
            }, Errors(src));
        }

        [TestMethod]
        [DataRow("回復\n", "アビリティデータの終りに空行を置いてください。")]
        [DataRow(",回復Lv1,1,1,1,-,-\n", "アビリティ名の設定が間違っています。")]
        [DataRow("回復,回復Lv1\n", "回復の射程の設定が抜けています。")]
        [DataRow("回復,回復Lv1,1\n", "回復の回数の設定が抜けています。")]
        [DataRow("回復,回復Lv1,1,1\n", "回復の消費ＥＮの設定が抜けています。")]
        [DataRow("回復,回復Lv1,1,1,1\n", "回復の必要気力の設定が抜けています。")]
        [DataRow("回復,回復Lv1,1,1,1,-\n", "回復のアビリティ属性の設定が抜けています。")]
        public void Load_MalformedAbility_ThrowsWithMessage(string abilityLine, string expected)
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() =>
                Load(src, UnitWith(rest: "パンチ,800,1,1,+0,-,-,-,AAAA,+0,-\n===\n" + abilityLine)));
            Assert.AreEqual(expected, ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Load_MultipleUnits_SeparatedByBlankLine()
        {
            var src = CreateSrc();
            var list = Load(src, Gundam + "\n" + UnitWith(line2: "ザク,ＭＳ,1,1"));

            Assert.IsTrue(list.IsDefined("ガンダム"));
            Assert.IsTrue(list.IsDefined("テスト機"));
            Assert.AreEqual("ザク", list.Item("テスト機").Nickname);
            // ダミーユニット + 2 機
            Assert.AreEqual(3, list.Count());
        }

        [TestMethod]
        public void Load_RedefinedUnit_ReplacesDataAndKeepsId()
        {
            // 既に定義済みのユニットは Clear して上書きする (VB: ud.Clear)
            var src = CreateSrc();
            var list = Load(src, Gundam);
            var id = list.Item("ガンダム").ID;
            var count = list.Count();

            Load(src, "ガンダム\nガンダム,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n5000,180,1200,75\nBBAA,gundam.bmp\n");

            var ud = list.Item("ガンダム");
            Assert.AreEqual(count, list.Count());
            Assert.AreEqual(id, ud.ID);
            Assert.AreEqual(5000, ud.HP);
            Assert.AreEqual(0, ud.CountFeature());
            Assert.AreEqual(0, ud.Weapons.Count);
            Assert.AreEqual(0, ud.Abilities.Count);
        }

        [TestMethod]
        public void Load_Comments_AreCollectedAsDataComment()
        {
            // 先頭のコメントはリスト全体の、ユニットの後ろのコメントはそのユニットの DataComment になる
            var src = CreateSrc();
            var list = Load(src, "# 先頭コメント\n" + Gundam + "\n# ガンダムの後\n" + UnitWith());

            Assert.AreEqual("# 先頭コメント", list.DataComment);
            Assert.AreEqual("# ガンダムの後", list.Item("ガンダム").DataComment);
            StringAssert.StartsWith(list.Item("ガンダム").Raw, "ガンダム");
            StringAssert.Contains(list.Raw, "# ガンダムの後");
        }

        [TestMethod]
        public void Constructor_RegistersStatusDummyUnit()
        {
            // ステータス表示用のダミーユニットが常に登録されている
            var src = CreateSrc();
            var ud = src.UDList.Item("ステータス表示用ダミーユニット");
            Assert.IsNotNull(ud);
            Assert.AreEqual("ユニット無し", ud.Nickname);
            Assert.IsTrue(ud.IsFeatureAvailable("ダミーユニット"));
            Assert.IsNull(src.UDList.Item("未定義"));
        }
    
        [TestMethod]
        public void Load_KanaOnNameLine_IsOverriddenByNicknameKana()
        {
            // 1行目「名称,読み仮名」で指定した読み仮名は、2行目で読み仮名を省略すると愛称のひらがな化で上書きされる
            // (VB: .KanaName = buf の後、comma_num = 3 なら .KanaName = StrToHiragana(.Nickname))
            var src = CreateSrc();
            var ud = Load(src, "テスト機,てすときかい\nザク,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n3500,180,1200,75\nBBAA,test.bmp\n").Item("テスト機");
            Assert.AreEqual("ざく", ud.KanaName);
        }

        [TestMethod]
        public void Load_KanaOnBothLines_SecondLineWins()
        {
            // 2行目にも読み仮名があれば2行目の指定が優先される
            var src = CreateSrc();
            var ud = Load(src, "テスト機,てすときかい\nザク,ざくつー,ＭＳ,1,2\n陸,4,M,4000,150\n特殊能力なし\n3500,180,1200,75\nBBAA,test.bmp\n").Item("テスト機");
            Assert.AreEqual("ざくつー", ud.KanaName);
        }

        [TestMethod]
        public void Load_NewFormatEmptyLeadingFeature_ReportsErrorAndKeepsOthers()
        {
            // 新形式で空の特殊能力は「行頭からn番目の特殊能力の設定が間違っています。」とし、残りは登録する
            var src = CreateSrc();
            var ud = Load(src, UnitWith(feature: "特殊能力\n,シールド")).Item("テスト機");

            CollectionAssert.AreEqual(new[] { "シールド" }, ud.Features.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "行頭から1番目の特殊能力の設定が間違っています。" }, Errors(src));
            Assert.AreEqual(3500, ud.HP);
        }

        [TestMethod]
        public void Load_NewFormatEmptyMiddleFeature_ReportsErrorAndKeepsOthers()
        {
            // VB: 「剣装備,,シールド」は 2 番目が空としてエラーになり、剣装備・シールドは登録される
            var src = CreateSrc();
            var ud = Load(src, UnitWith(feature: "特殊能力\n剣装備,,シールド")).Item("テスト機");

            CollectionAssert.AreEqual(new[] { "剣装備", "シールド" }, ud.Features.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "行頭から2番目の特殊能力の設定が間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_WeaponInvalidNumericFields_ReportErrorsWithoutShiftingFollowingFields()
        {
            // 命中率・弾数・必要気力・クリティカル率の不正値はエラーを出して読み飛ばし、後続の項目はずれない
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "A,100,1,1,X,X,10,X,AAAA,X,B\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual(100, w.Power);
            Assert.AreEqual(10, w.ENConsumption);
            Assert.AreEqual("AAAA", w.Adaption);
            Assert.AreEqual("B", w.Class);
            CollectionAssert.AreEqual(new[]
            {
                "Aの命中率の設定が間違っています。",
                "Aの弾数の設定が間違っています。",
                "Aの必要気力の設定が間違っています。",
                "Aのクリティカル率の設定が間違っています。",
            }, Errors(src));
        }

        [TestMethod]
        public void Load_WeaponInvalidENConsumption_ReportsErrorWithoutShiftingFollowingFields()
        {
            // VB: 消費ＥＮが不正でも空白区切りでなければ後続の項目はずれない
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "A,100,1,1,+0,-,X,-,AAAA,+0,B\n")).Item("テスト機");

            var w = ud.Weapons.Single();
            Assert.AreEqual("AAAA", w.Adaption);
            Assert.AreEqual("B", w.Class);
            CollectionAssert.AreEqual(new[] { "Aの消費ＥＮの設定が間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_WeaponEmptyClass_ReportsError()
        {
            // 属性欄が空なら「武器属性の設定が間違っています。」(読み込みは継続)
            var src = CreateSrc();
            var ud = Load(src, UnitWith(rest: "A,100,1,1,+0,-,-,-,AAAA,+0,\n")).Item("テスト機");

            Assert.AreEqual("", ud.Weapons.Single().Class);
            CollectionAssert.AreEqual(new[] { "Aの武器属性の設定が間違っています。" }, Errors(src));
        }
    }
}
