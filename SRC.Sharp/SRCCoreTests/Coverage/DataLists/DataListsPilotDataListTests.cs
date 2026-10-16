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
    /// PilotDataList.Load (pilot.txt / パイロットデータ) の読み込み仕様のテスト。
    /// VB 版 PilotDataList.cls の Load を仕様として確認する。
    /// </summary>
    [TestClass]
    public class DataListsPilotDataListTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            // スペシャルパワーは定義済みのものだけ登録できる
            src.SPDList.Add("集中");
            src.SPDList.Add("ひらめき");
            src.SPDList.Add("熱血");
            return src;
        }

        private static PilotDataList Load(SRC src, string text, string fname = "pilot.txt")
        {
            src.PDList.Load(fname, new MemoryStream(Encoding.UTF8.GetBytes(text)));
            return src.PDList;
        }

        private static string[] Errors(SRC src) => src.DataErrors.Select(x => x.msg).ToArray();

        private const string Amuro =
            "アムロ\n" +
            "アムロ・レイ,あむろれい,男性,ニュータイプ,AABA,200\n" +
            "特殊能力\n" +
            "ニュータイプLv1,1,ニュータイプLv2,10\n" +
            "切り払いLv1,5\n" +
            "150,140,160,170,180,190,強気\n" +
            "精神,40,集中,1,ひらめき,5,熱血=20,10\n" +
            "amuro.bmp,amuro.mid\n";

        // 各行を差し替えられるパイロットデータ
        private static string PilotWith(string line2 = "テスト,てすと,男性,汎用,AAAA,100", string feature = "特殊能力なし",
            string status = "100,100,100,100,100,100,普通", string sp = "ＳＰなし", string bitmap = "test.bmp,-", string rest = "")
        {
            return "テスト\n" + line2 + "\n" + feature + "\n" + status + "\n" + sp + "\n" + bitmap + "\n" + rest;
        }

        [TestMethod]
        public void Load_PngBitmap_Accepted()
        {
            // 元の SRC は .bmp だけだが、SRC# は .png・.jpg・.gif も受け付ける（#1212）
            var src = CreateSrc();
            var pd = Load(src, PilotWith(bitmap: "test.png,-")).Item("テスト");
            Assert.AreEqual("test.png", pd.Bitmap);
            Assert.IsFalse(pd.IsBitmapMissing);
        }

        [TestMethod]
        public void Load_StandardPilot_ParsesBasicFields()
        {
            var src = CreateSrc();
            var pd = Load(src, Amuro).Item("アムロ");

            Assert.AreEqual("アムロ・レイ", pd.Nickname);
            Assert.AreEqual("あむろれい", pd.KanaName);
            Assert.AreEqual("男性", pd.Sex);
            Assert.AreEqual("ニュータイプ", pd.Class);
            Assert.AreEqual("AABA", pd.Adaption);
            Assert.AreEqual(200, pd.ExpValue);
            Assert.AreEqual(150, pd.Infight);
            Assert.AreEqual(140, pd.Shooting);
            Assert.AreEqual(160, pd.Hit);
            Assert.AreEqual(170, pd.Dodge);
            Assert.AreEqual(180, pd.Technique);
            Assert.AreEqual(190, pd.Intuition);
            Assert.AreEqual("強気", pd.Personality);
            Assert.AreEqual(40, pd.SP);
            Assert.AreEqual("amuro.bmp", pd.Bitmap);
            Assert.AreEqual("amuro.mid", pd.BGM);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_StandardPilot_ParsesSkillsWithAcquireLevel()
        {
            // 新形式: 「能力名Lvn,修得レベル」の組を並べる。数値で始まる行で終了する
            var src = CreateSrc();
            var pd = Load(src, Amuro).Item("アムロ");

            var skills = pd.Skills.Select(x => (x.Name, x.Level, x.NecessaryLevel)).ToArray();
            CollectionAssert.AreEqual(new[]
            {
                ("ニュータイプ", 1d, 1),
                ("ニュータイプ", 2d, 10),
                ("切り払い", 1d, 5),
            }, skills);
            Assert.AreEqual(2d, pd.SkillLevel(10, "ニュータイプ"));
            Assert.AreEqual(1d, pd.SkillLevel(9, "ニュータイプ"));
        }

        [TestMethod]
        public void Load_StandardPilot_ParsesSpecialPowers()
        {
            // 「名称=消費量」で個別のＳＰ消費量を指定できる
            var src = CreateSrc();
            var pd = Load(src, Amuro).Item("アムロ");

            var sps = pd.DefinedSpecialPowers().Select(x => (x.Name, x.NecessaryLevel, x.SPConsumption)).ToArray();
            CollectionAssert.AreEqual(new[]
            {
                ("集中", 1, 0),
                ("ひらめき", 5, 0),
                ("熱血", 10, 20),
            }, sps);
        }

        [TestMethod]
        public void Load_FourCommas_ThirdFieldIsSexOrKana()
        {
            // コンマ4個: 3番目が性別(男性/女性/-)なら読み仮名は愛称から、そうでなければ読み仮名
            var src = CreateSrc();
            var list = Load(src,
                PilotWith(line2: "セイラ,女性,汎用,AAAA,100") + "\n" +
                PilotWith(line2: "テスト,てすと,汎用,AAAA,100").Replace("テスト\n", "テスト2\n"));

            var a = list.Item("テスト");
            Assert.AreEqual("女性", a.Sex);
            Assert.AreEqual("せいら", a.KanaName);
            var b = list.Item("テスト2");
            Assert.AreEqual("てすと", b.KanaName);
            // VB は .Sex を設定しない (既定値 "")。性別が未設定であることを確認する
            Assert.IsTrue(string.IsNullOrEmpty(b.Sex));
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_ThreeCommas_DerivesKanaFromNickname()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(line2: "カミーユ,汎用,AAAA,100")).Item("テスト");

            Assert.AreEqual("かみーゆ", pd.KanaName);
            Assert.AreEqual("汎用", pd.Class);
        }

        [TestMethod]
        public void Load_FiveCommas_InvalidKanaAndSex_ReportErrors()
        {
            var src = CreateSrc();
            Load(src, PilotWith(line2: "テスト,男性,不明,汎用,AAAA,100"));

            CollectionAssert.IsSubsetOf(new[]
            {
                "読み仮名の設定が抜けています。",
                "性別の設定が間違っています。",
            }, Errors(src));
            Assert.AreEqual("てすと", src.PDList.Item("テスト").KanaName);
        }

        [TestMethod]
        public void Load_InvalidValues_ReportErrorsAndFallBack()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(
                line2: "テスト,てすと,男性,1,AB,X",
                status: "X,X,X,X,X,X,1",
                sp: "精神,X",
                bitmap: "test.txt,bgm.txt")).Item("テスト");

            Assert.AreEqual("AAAA", pd.Adaption);
            Assert.AreEqual(1, pd.SP);
            Assert.IsTrue(pd.IsBitmapMissing);
            CollectionAssert.IsSubsetOf(new[]
            {
                "クラスの設定が間違っています。",
                "地形適応の設定が間違っています。",
                "経験値の設定が間違っています。",
                "格闘攻撃力の設定が間違っています。",
                "射撃攻撃力の設定が間違っています。",
                "命中の設定が間違っています。",
                "回避の設定が間違っています。",
                "技量の設定が間違っています。",
                "反応の設定が間違っています。",
                "性格の設定が間違っています。",
                "ＳＰの設定が間違っています。",
                "ビットマップの設定が間違っています。",
                "ＭＩＤＩの設定が間違っています。",
            }, Errors(src));
            Assert.IsTrue(src.DataErrors.All(x => x.dname == "テスト" && x.fname == "pilot.txt"));
        }

        [TestMethod]
        public void Load_ValuesOverLimit_AreClamped()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(
                line2: "テスト,てすと,男性,汎用,AAAA,99999",
                status: "99999,99999,99999,99999,99999,99999,普通",
                sp: "ＳＰ,99999")).Item("テスト");

            Assert.AreEqual(9999, pd.ExpValue);
            Assert.AreEqual(9999, pd.Infight);
            Assert.AreEqual(9999, pd.Shooting);
            Assert.AreEqual(9999, pd.Hit);
            Assert.AreEqual(9999, pd.Dodge);
            Assert.AreEqual(9999, pd.Technique);
            Assert.AreEqual(9999, pd.Intuition);
            Assert.AreEqual(9999, pd.SP);
        }

        [TestMethod]
        public void Load_PersonalityWithTrailingComma_ReportsErrorAndTrims()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(status: "100,100,100,100,100,100,普通,")).Item("テスト");

            Assert.AreEqual("普通", pd.Personality);
            CollectionAssert.AreEqual(new[] { "行末に余分なコンマが付けられています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_NoSpecialPower_LeavesSpZero()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(sp: "精神なし")).Item("テスト");

            Assert.AreEqual(0, pd.SP);
            Assert.AreEqual(0, pd.DefinedSpecialPowers().Count);
        }

        [TestMethod]
        public void Load_UndefinedSpecialPower_ReportsErrorAndSkips()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(sp: "ＳＰ,30,未定義,1,集中,3")).Item("テスト");

            CollectionAssert.AreEqual(new[] { "集中" }, pd.DefinedSpecialPowers().Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "未定義というスペシャルパワーは存在しません。" }, Errors(src));
        }

        [TestMethod]
        public void Load_SpecialPowerInvalidLevel_RegistersAtLevelOne()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(sp: "ＳＰ,30,集中,X")).Item("テスト");

            Assert.AreEqual(1, pd.DefinedSpecialPowers().Single().NecessaryLevel);
            CollectionAssert.AreEqual(new[] { "スペシャルパワー「集中」の獲得レベルが間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_SpecialPowerMissingLevel_ReportsError()
        {
            // 末尾にレベルの無いスペシャルパワー名が残った場合 (VB と同じく直前の名前でメッセージを出す)
            var src = CreateSrc();
            var pd = Load(src, PilotWith(sp: "ＳＰ,30,集中,1,ひらめき")).Item("テスト");

            CollectionAssert.AreEqual(new[] { "集中" }, pd.DefinedSpecialPowers().Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "スペシャルパワー「集中」の獲得レベル指定が抜けています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_EmptySpecialPowerName_ReportsError()
        {
            var src = CreateSrc();
            Load(src, PilotWith(sp: "ＳＰ,30,,1"));
            CollectionAssert.AreEqual(new[] { "スペシャルパワーの指定が抜けています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_HyphenBgm_IsAccepted()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(bitmap: "test.bmp,-")).Item("テスト");
            Assert.AreEqual("-", pd.BGM);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_BgmWithTrailingParenthesis_IsAccepted()
        {
            // 末尾の「)」を除いて拡張子を判定する
            var src = CreateSrc();
            var pd = Load(src, PilotWith(bitmap: "test.bmp,(theme.mp3)")).Item("テスト");
            Assert.AreEqual("(theme.mp3)", pd.BGM);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_EmptyBgm_ReportsMissing()
        {
            var src = CreateSrc();
            Load(src, PilotWith(bitmap: "test.bmp,"));
            CollectionAssert.AreEqual(new[] { "ＭＩＤＩの設定が抜けています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_OldFormatSkills_LevelOnlyContinuesPreviousSkill()
        {
            // 旧形式: 「Lvn」だけの指定は直前の特殊能力のレベル追加
            var src = CreateSrc();
            var pd = Load(src, PilotWith(feature: "特殊能力,ニュータイプLv1,1,Lv2,10,底力,20")).Item("テスト");

            var skills = pd.Skills.Select(x => (x.Name, x.Level, x.NecessaryLevel)).ToArray();
            CollectionAssert.AreEqual(new[]
            {
                ("ニュータイプ", 1d, 1),
                ("ニュータイプ", 2d, 10),
                ("底力", (double)Constants.DEFAULT_LEVEL, 20),
            }, skills);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_OldFormatSkillWithoutAcquireLevel_ReportsError()
        {
            var src = CreateSrc();
            Load(src, PilotWith(feature: "特殊能力,底力Lv2"));
            CollectionAssert.AreEqual(new[] { "特殊能力「底力Lv2」の修得レベルが間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_OldFormatSkillWithSpace_Throws()
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力,底力 1,1")));
            Assert.AreEqual("行頭から1番目の特殊能力の指定の後に「,」が抜けています。", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Load_NewFormatSkillWithData_KeepsData()
        {
            // 「能力名=データ」の別名・データ指定
            var src = CreateSrc();
            var pd = Load(src, PilotWith(feature: "特殊能力\n得意技=格闘,1")).Item("テスト");

            var sd = pd.Skills.Single();
            Assert.AreEqual("得意技", sd.Name);
            Assert.AreEqual("格闘", sd.StrData);
        }

        [TestMethod]
        public void Load_NewFormatInvalidAcquireLevel_ReportsErrorAndUsesOne()
        {
            var src = CreateSrc();
            var pd = Load(src, PilotWith(feature: "特殊能力\n底力,X")).Item("テスト");

            Assert.AreEqual(1, pd.Skills.Single().NecessaryLevel);
            CollectionAssert.AreEqual(new[] { "特殊能力「底力」の修得レベルが間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_NewFormatMissingAcquireLevel_ReportsError()
        {
            var src = CreateSrc();
            Load(src, PilotWith(feature: "特殊能力\n底力Lv3"));
            CollectionAssert.AreEqual(new[] { "特殊能力「底力Lv3」の修得レベルが間違っています。" }, Errors(src));
        }

        [TestMethod]
        public void Load_NewFormatSkillWithSpace_Throws()
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力\n切り払い 1,5")));
            Assert.AreEqual("行頭から1番目の特殊能力「切り払い」の指定の後に「,」が抜けています。", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Load_NewFormatTrailingComma_Throws()
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力\n底力,1,")));
            Assert.AreEqual("行末の「,」の後に特殊能力レベル指定が抜けています。", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Load_NewFormatNumericSkillName_ReportsError()
        {
            // 2組目以降の能力名が数値の場合はエラー
            var src = CreateSrc();
            Load(src, PilotWith(feature: "特殊能力\n底力,1,5,3"));
            CollectionAssert.Contains(Errors(src), "行頭から2番目の特殊能力名の設定が間違っています。");
        }

        [TestMethod]
        public void Load_ExtendedSections_ParseFeaturesWeaponsAndAbilities()
        {
            // ビットマップ行の後の「===」以降: 特殊能力 → 武器 → アビリティ
            var src = CreateSrc();
            var pd = Load(src, PilotWith(rest:
                "===\n" +
                "格闘武器=拳\n" +
                "シールド\n" +
                "===\n" +
                "必殺拳,1800,1,1,+20,-,20,110,AAAA,+20,武\n" +
                "===\n" +
                "応援,気力増加Lv1,1,1,-,-,-\n")).Item("テスト");

            CollectionAssert.AreEqual(new[] { "格闘武器", "シールド" }, pd.Features.Select(x => x.Name).ToArray());
            Assert.AreEqual("拳", pd.FeatureData("格闘武器"));
            Assert.AreEqual(1800, pd.Weapons.Single().Power);
            Assert.AreEqual(20, pd.Weapons.Single().ENConsumption);
            Assert.AreEqual("応援", pd.Abilities.Single().Name);
            Assert.AreEqual(1, pd.Abilities.Single().Stock);
        }

        [TestMethod]
        public void Load_NonSeparatorAfterBitmap_StartsNextPilot()
        {
            var src = CreateSrc();
            var list = Load(src, PilotWith() + "\n" + Amuro);
            Assert.IsTrue(list.IsDefined("テスト"));
            Assert.IsTrue(list.IsDefined("アムロ"));
        }

        [TestMethod]
        [DataRow("テスト,てすと\nテスト,汎用,AAAA,100\n", "名称の設定が抜けています。")]
        [DataRow("テスト 1\nテスト,汎用,AAAA,100\n", "名称に半角スペースは使用出来ません。")]
        [DataRow("テスト（改）\nテスト,汎用,AAAA,100\n", "名称に全角括弧は使用出来ません")]
        [DataRow("テスト\"\nテスト,汎用,AAAA,100\n", "名称に\"は使用出来ません。")]
        [DataRow("テスト\nテスト,汎用,AAAA\n", "設定に抜けがあります。")]
        [DataRow("テスト\nテスト,て,男性,汎用,AAAA,100,1\n", "余分な「,」があります。")]
        [DataRow("テスト\n,て,男性,汎用,AAAA,100\n", "愛称の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n100,100,100,100,100,100,普通\n", "特殊能力の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n\n", "格闘攻撃力の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100\n", "射撃攻撃力の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100\n", "命中の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100\n", "回避の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100\n", "技量の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100\n", "反応の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100,100,\n", "性格の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100,100,普通\n\n", "スペシャルパワーの設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100,100,普通\n精神\n", "ＳＰ値の設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100,100,普通\n気力,40\n", "スペシャルパワーの設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100,100,普通\nＳＰなし\n\n", "ビットマップの設定が抜けています。")]
        [DataRow("テスト\nテスト,汎用,AAAA,100\n特殊能力なし\n100,100,100,100,100,100,普通\nＳＰなし\ntest.bmp\n", "ＭＩＤＩの設定が抜けています。")]
        public void Load_MalformedPilot_ThrowsWithMessage(string text, string expected)
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, text));
            Assert.AreEqual(expected, ex.InvalidDataList.Single().msg);
            Assert.AreEqual("pilot.txt", ex.InvalidDataList.Single().fname);
        }

        [TestMethod]
        public void Item_FindsPilotByNickname_ButIsDefined2DoesNot()
        {
            // Item/IsDefined は名称で見つからなければ愛称で検索する。IsDefined2 は愛称を見ない
            var src = CreateSrc();
            var list = Load(src, Amuro);

            Assert.AreSame(list.Item("アムロ"), list.Item("アムロ・レイ"));
            Assert.IsTrue(list.IsDefined("アムロ・レイ"));
            Assert.IsFalse(list.IsDefined2("アムロ・レイ"));
            Assert.IsTrue(list.IsDefined2("アムロ"));
            Assert.IsNull(list.Item("シャア"));
        }

        [TestMethod]
        public void Load_RedefinedPilot_ReplacesSameInstance()
        {
            var src = CreateSrc();
            var list = Load(src, Amuro);
            var before = list.Item("アムロ");
            var count = list.Count();

            Load(src, "アムロ\nアムロ・レイ,汎用,AAAA,1\n特殊能力なし\n1,1,1,1,1,1,普通\nＳＰなし\namuro.bmp,-\n");

            var after = list.Item("アムロ");
            Assert.AreSame(before, after);
            Assert.AreEqual(count, list.Count());
            Assert.AreEqual(0, after.Skills.Count);
            Assert.AreEqual(0, after.DefinedSpecialPowers().Count);
            Assert.AreEqual(1, after.Infight);
        }

        [TestMethod]
        public void Constructor_RegistersStatusDummyPilot()
        {
            var src = CreateSrc();
            var pd = src.PDList.Item("ステータス表示用ダミーパイロット(ザコ)");
            Assert.IsNotNull(pd);
            Assert.AreEqual("パイロット不在", pd.Nickname);
        }
    
        [TestMethod]
        public void Load_OldFormatLevelWithoutPrecedingSkill_Throws()
        {
            // 旧形式で先頭が「Lvn」だけの場合は能力名が無いので中断する
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, PilotWith(feature: "特殊能力,Lv2,1")));
            Assert.AreEqual("行頭から1番目の特殊能力の名前「Lv2」が不正です", ex.InvalidDataList.Single().msg);
        }

        [TestMethod]
        public void Load_ExtendedFeatureSection_EmptyItemReportsError()
        {
            // 「===」以降の特殊能力で空の項目は「行頭からn番目の特殊能力の設定が間違っています。」
            var src = CreateSrc();
            var pd = Load(src, PilotWith(rest:
                "===\n" +
                ",シールド\n" +
                "===\n" +
                "必殺拳,1800,1,1,+20,-,20,110,AAAA,+20,武\n")).Item("テスト");

            CollectionAssert.AreEqual(new[] { "シールド" }, pd.Features.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "行頭から1番目の特殊能力の設定が間違っています。" }, Errors(src));
            Assert.AreEqual("必殺拳", pd.Weapons.Single().Name);
        }

        [TestMethod]
        public void Load_ExtendedFeatureSection_NumericItemReportsError()
        {
            // VB PilotDataList.cls: If buf2 = "" Or IsNumeric(buf2) Then DataErrorMessage ... (読み込みは継続)
            var src = CreateSrc();
            var pd = Load(src, PilotWith(rest:
                "===\n" +
                "100,シールド\n" +
                "===\n" +
                "必殺拳,1800,1,1,+20,-,20,110,AAAA,+20,武\n")).Item("テスト");

            CollectionAssert.AreEqual(new[] { "シールド" }, pd.Features.Select(x => x.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "行頭から1番目の特殊能力の設定が間違っています。" }, Errors(src));
            Assert.AreEqual("必殺拳", pd.Weapons.Single().Name);
        }

        [TestMethod]
        public void Load_ExtendedWeaponSectionEndingAtEof_HasNoAbilities()
        {
            // 武器データの後にアビリティ区切りが無くファイルが終わってもよい
            var src = CreateSrc();
            var pd = Load(src, PilotWith(rest:
                "===\n" +
                "シールド\n" +
                "===\n" +
                "必殺拳,1800,1,1,+20,-,20,110,AAAA,+20,武")).Item("テスト");

            Assert.AreEqual("必殺拳", pd.Weapons.Single().Name);
            Assert.AreEqual("武", pd.Weapons.Single().Class);
            Assert.AreEqual(0, pd.Abilities.Count);
            Assert.AreEqual(0, src.DataErrors.Count);
        }
    }
}
