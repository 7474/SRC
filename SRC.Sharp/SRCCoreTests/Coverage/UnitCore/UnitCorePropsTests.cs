using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Unit.props.cs（VB: Unit.cls の各種基本ステータス）のテスト
    /// </summary>
    [TestClass]
    public class UnitCorePropsTests
    {
        private static string UnitBlock(string name, string nick = null, string kana = "てすと", string cls = "汎用",
            string trans = "陸", int speed = 5, int value = 1000, int exp = 100,
            int hp = 3000, int en = 100, int armor = 800, int mobility = 90, string adaption = "AA-A",
            params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{nick ?? name}, {kana}, {cls}, 1, 2
{trans}, {speed}, M, {value}, {exp}
{feat}
{hp}, {en}, {armor}, {mobility}
{adaption}, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        private static string PilotBlock(string name, string nick, string kana = "ぱいろっと", string adaption = "AAAA", params string[] skills)
        {
            var sk = skills.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", skills);
            return $@"
{name}
{nick}, {kana}, 男性, 汎用, {adaption}, 100
{sk}
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid
";
        }

        private static (SRC src, Unit u) Create(string unitBlock, string pilotBlocks = null, string party = "味方")
        {
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src, unitBlock);
            if (pilotBlocks != null)
            {
                UnitCoreFixture.LoadPilots(src, pilotBlocks);
            }
            var name = unitBlock.Trim().Split('\n')[0].Trim();
            return (src, src.UList.Add(name, 0, party));
        }

        private static void DefineOption(SRC src, string oname)
        {
            src.Expression.DefineGlobalVariable("Option(" + oname + ")");
        }

        // ──────────────────────────────────────────────
        // 愛称・読み仮名
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname0_RenameFeature_ReplacesUnitAndPilotNickname()
        {
            // 愛称変更の $(愛称) は元の愛称、$(パイロット愛称) はメインパイロットの愛称に置換
            var (src, u) = Create(
                UnitBlock("テスト機", nick: "テスト", features: "愛称変更=$(パイロット愛称)専用$(愛称)"),
                PilotBlock("アムロ", "アムロ"));
            // パイロット不在時は「○○」
            Assert.AreEqual("○○専用テスト", u.Nickname0);

            UnitCoreFixture.Board(src, u, "アムロ");
            Assert.AreEqual("アムロ専用テスト", u.Nickname0);
        }

        [TestMethod]
        public void Nickname0_WithoutRenameFeature_ReturnsDataNickname()
        {
            var (_, u) = Create(UnitBlock("テスト機", nick: "テスト"));
            Assert.AreEqual("テスト", u.Nickname0);
            // 等身大基準でなければ Nickname も同じ
            Assert.AreEqual("テスト", u.Nickname);
        }

        [TestMethod]
        public void Nickname_HeroScale_GenericPilotUsesUnitNickname()
        {
            // 等身大基準でもザコ・汎用パイロットの場合はユニットの愛称を使う
            var (src, u) = Create(UnitBlock("テスト機", nick: "テスト"), PilotBlock("兵士(ザコ)", "兵士"));
            DefineOption(src, "等身大基準");
            UnitCoreFixture.Board(src, u, "兵士(ザコ)");
            Assert.AreEqual("テスト", u.Nickname);
        }

        [TestMethod]
        [Ignore("移植の不具合: 等身大基準でパイロット愛称を代入した後 return default で null を返している")]
        public void Nickname_HeroScale_UsesMainPilotNickname()
        {
            // 等身大基準ではメッセージ中の愛称にメインパイロットの愛称を使う（VB: Nickname = MainPilot.Nickname）
            var (src, u) = Create(UnitBlock("テスト機", nick: "テスト"), PilotBlock("アムロ", "アムロ・レイ"));
            DefineOption(src, "等身大基準");
            UnitCoreFixture.Board(src, u, "アムロ");
            Assert.AreEqual("アムロ・レイ", u.Nickname);
        }

        [TestMethod]
        public void KanaName_KanaRenameFeature_TakesPrecedenceOverRename()
        {
            // 読み仮名変更があれば愛称変更より優先され、ひらがな化はされない
            var (_, u) = Create(UnitBlock("テスト機", kana: "てすと", features: new[] { "読み仮名変更=カスタムきー", "愛称変更=別名" }));
            Assert.AreEqual("カスタムきー", u.KanaName);
        }

        [TestMethod]
        public void KanaName_RenameFeature_ReplacesPilotNickname()
        {
            // 愛称変更の $(パイロット愛称) はメインパイロットの愛称で置換され、ひらがな化される
            var (src, u) = Create(UnitBlock("テスト機", features: "愛称変更=$(パイロット愛称)カスタム"), PilotBlock("アムロ", "アムロ"));
            Assert.AreEqual("○○かすたむ", u.KanaName);
            UnitCoreFixture.Board(src, u, "アムロ");
            Assert.AreEqual("あむろかすたむ", u.KanaName);
        }

        [TestMethod]
        public void KanaName_RenameFeatureOnly_ConvertsRenamedNicknameToHiragana()
        {
            // 読み仮名変更が無く愛称変更がある場合は、変更後の愛称をひらがな化して読み仮名とする
            var (_, u) = Create(UnitBlock("テスト機", nick: "テスト", features: "愛称変更=カスタム$(愛称)"));
            Assert.AreEqual("かすたむてすと", u.KanaName);
        }

        // ──────────────────────────────────────────────
        // ランク
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Rank_IsCappedAt999()
        {
            var (_, u) = Create(UnitBlock("テスト機"));
            u.Rank = 1500;
            Assert.AreEqual(999, u.Rank);
        }

        [TestMethod]
        public void RankUp_RaisesOtherFormsAndAppliesRatios()
        {
            // ランクアップ時は他形態のランクも上げ、HP/EN を指定比率で設定する
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src,
                UnitBlock("人型", hp: 3000, en: 100, features: "変形=変形 飛行型")
                + UnitBlock("飛行型", hp: 2000, en: 200, features: "変形=変形 人型"));
            var u = src.UList.Add("人型", 0, "味方");

            u.RankUp(2, 0.5, 1d);

            Assert.AreEqual(2, u.Rank);
            Assert.AreEqual(u.MaxHP / 2, u.HP);
            Assert.AreEqual(u.MaxEN, u.EN);
            var fly = u.OtherForm("飛行型");
            Assert.AreEqual(2, fly.Rank);
            Assert.AreEqual(fly.MaxHP / 2, fly.HP);
        }

        [TestMethod]
        public void RankUp_CombinedUnit_RaisesSeparatedParts()
        {
            // 合体ユニットのランクを上げると分離先のユニットのランクも合わせて上がる
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src,
                UnitBlock("合体後", features: "分離=分離 パーツＡ パーツＢ")
                + UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ")
                + UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));
            var combined = src.UList.Add("合体後", 0, "味方");
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            var partB = src.UList.Add("パーツＢ", 3, "味方");

            combined.RankUp(2);

            Assert.AreEqual(2, combined.Rank);
            Assert.AreEqual(2, partA.Rank);
            // 既に高いランクのパーツは下げない
            Assert.AreEqual(3, partB.Rank);
        }

        [TestMethod]
        public void RankUp_PartOfMainFormCombination_RaisesOtherParts()
        {
            // 合体形態が主形態の分離形態が改造された場合は他の分離形態のランクも上げる
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src,
                UnitBlock("合体後", features: new[] { "分離=分離 パーツＡ パーツＢ", "主形態" })
                + UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ")
                + UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            var partB = src.UList.Add("パーツＢ", 0, "味方");

            partA.RankUp(1);

            Assert.AreEqual(1, partA.Rank);
            Assert.AreEqual(1, partB.Rank);
        }

        // ──────────────────────────────────────────────
        // クラス・サイズ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Class0_RemovesHeroParenthesesAndExclusiveSpec()
        {
            // 人間ユニット指定の括弧を外し、「(～専用)」指定を取り除く
            var (_, hero) = Create(UnitBlock("歩兵", cls: "(歩兵)"));
            Assert.AreEqual("歩兵", hero.Class0);
            var (_, mecha) = Create(UnitBlock("専用機", cls: "汎用(アムロ専用)"));
            Assert.AreEqual("汎用", mecha.Class0);
            var (_, heroEx) = Create(UnitBlock("専用歩兵", cls: "(戦士(勇者専用))"));
            Assert.AreEqual("戦士", heroEx.Class0);
        }

        [TestMethod]
        [Ignore("移植の不具合: Class0 の「(」検出時の break が switch しか抜けず、より前の括弧まで削ってしまう")]
        public void Class0_KeepsEarlierParenthesesBeforeExclusiveSpec()
        {
            // VB は対応する「(」で Exit Do するため、それより前の括弧付き部分は残る
            var (_, u) = Create(UnitBlock("専用機", cls: "汎用(改)(アムロ専用)"));
            Assert.AreEqual("汎用(改)", u.Class0);
        }

        [TestMethod]
        public void Size_SizeChangeFeatureOverridesData()
        {
            var (_, normal) = Create(UnitBlock("テスト機"));
            Assert.AreEqual("M", normal.Size);
            var (_, changed) = Create(UnitBlock("巨大機", features: "サイズ変更=LL"));
            Assert.AreEqual("LL", changed.Size);
        }

        // ──────────────────────────────────────────────
        // ＨＰ・ＥＮ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HpEn_AreClampedBetweenZeroAndMax()
        {
            var (_, u) = Create(UnitBlock("テスト機", hp: 3000, en: 100));
            u.HP = 99999;
            Assert.AreEqual(3000, u.HP);
            u.HP = -5;
            Assert.AreEqual(0, u.HP);
            u.EN = 500;
            Assert.AreEqual(100, u.EN);
            u.EN = -1;
            Assert.AreEqual(0, u.EN);
        }

        [TestMethod]
        public void MaxHpEn_HaveMinimumValues()
        {
            // 最大ＨＰは最低 1、最大ＥＮは最低 5
            var (_, u) = Create(UnitBlock("テスト機", hp: 0, en: 0));
            Assert.AreEqual(1, u.MaxHP);
            Assert.AreEqual(5, u.MaxEN);
        }

        // ──────────────────────────────────────────────
        // 移動力・移動形態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Speed_ZeroBaseStaysZeroUnlessTeleportOrJump()
        {
            // 元々移動力が 0 の場合は移動力ＵＰ状態でも 0 のまま
            var (_, fixedUnit) = Create(UnitBlock("砲台", speed: 0));
            fixedUnit.AddCondition("移動力ＵＰ", 1);
            Assert.AreEqual(0, fixedUnit.Speed);

            // テレポートを持てば修正が適用される
            var (_, tele) = Create(UnitBlock("転移機", speed: 0, features: "テレポート=テレポート 0"));
            tele.AddCondition("移動力ＵＰ", 1);
            Assert.AreEqual(1, tele.Speed);
        }

        [TestMethod]
        public void Speed_ConditionModifiers()
        {
            var (src, u) = Create(UnitBlock("テスト機", speed: 5));
            Assert.AreEqual(5, u.Speed);

            // 移動力ＵＰは +1（大型マップでは +2）
            u.AddCondition("移動力ＵＰ", 1);
            Assert.AreEqual(6, u.Speed);
            DefineOption(src, "大型マップ");
            Assert.AreEqual(7, u.Speed);
            u.DeleteCondition("移動力ＵＰ");

            // 移動力ＤＯＷＮは半減（最低 1）
            u.AddCondition("移動力ＤＯＷＮ", 1);
            Assert.AreEqual(2, u.Speed);
            u.DeleteCondition("移動力ＤＯＷＮ");

            // 移動不能なら 0
            u.AddCondition("移動不能", 1);
            Assert.AreEqual(0, u.Speed);
        }

        [TestMethod]
        public void Speed_OutOfEnergyInAirOrUnderground_IsZero()
        {
            // 出撃中、空中・宇宙で EN 5 未満、地中で EN 10 未満なら移動できない
            var (_, u) = Create(UnitBlock("テスト機", speed: 5));
            u.Status = "出撃";
            u.Area = "空中";
            u.EN = 4;
            Assert.AreEqual(0, u.Speed);
            u.EN = 5;
            Assert.AreEqual(5, u.Speed);
            u.Area = "地中";
            u.EN = 9;
            Assert.AreEqual(0, u.Speed);
            u.Area = "地上";
            Assert.AreEqual(5, u.Speed);
        }

        [TestMethod]
        public void Transportation_FeaturesAddMovementTypes()
        {
            // 空中移動・陸上移動・水中移動は先頭に、地中移動は末尾に追加。宇宙移動は空の場合のみ
            var (_, u) = Create(UnitBlock("テスト機", trans: "陸", features: new[] { "空中移動", "陸上移動", "水中移動", "地中移動" }));
            Assert.AreEqual("水空陸地中", u.Transportation);

            var (_, space) = Create(UnitBlock("宇宙機", trans: "陸", features: "宇宙移動"));
            Assert.AreEqual("陸", space.Transportation);
        }

        // ──────────────────────────────────────────────
        // 地形適応
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Adaption_UsesLowerOfUnitAndPilot()
        {
            // ユニットとパイロットの地形適応の低い方を使う。「-」は 0
            var (src, u) = Create(UnitBlock("テスト機", adaption: "SB-A"), PilotBlock("パイロット", "パイロット", adaption: "ASAD"));
            // パイロット不在時はパイロット側を A(4) とみなす
            Assert.AreEqual(4, u.get_Adaption(1));
            Assert.AreEqual(0, u.get_Adaption(3));

            UnitCoreFixture.Board(src, u, "パイロット");
            Assert.AreEqual(4, u.get_Adaption(1)); // S と A → A
            Assert.AreEqual(3, u.get_Adaption(2)); // B と S → B
            Assert.AreEqual(1, u.get_Adaption(4)); // A と D → D
        }

        [TestMethod]
        public void Adaption_SumOption_UsesSumTable()
        {
            // 地形適応総和計算: 総和 9-10→S, 7-8→A, 5-6→B, 3-4→C, 1-2→D
            var (src, u) = Create(UnitBlock("テスト機", adaption: "SBDA"), PilotBlock("パイロット", "パイロット", adaption: "ADDC"));
            DefineOption(src, "地形適応総和計算");
            UnitCoreFixture.Board(src, u, "パイロット");
            Assert.AreEqual(5, u.get_Adaption(1)); // 5+4=9
            Assert.AreEqual(2, u.get_Adaption(2)); // 3+1=4
            Assert.AreEqual(1, u.get_Adaption(3)); // 1+1=2
            Assert.AreEqual(3, u.get_Adaption(4)); // 4+2=6
        }

        [TestMethod]
        public void Adaption_PilotDash_IsZero()
        {
            var (src, u) = Create(UnitBlock("テスト機", adaption: "AAAA"), PilotBlock("パイロット", "パイロット", adaption: "A-AA"));
            UnitCoreFixture.Board(src, u, "パイロット");
            Assert.AreEqual(0, u.get_Adaption(2));
        }

        [TestMethod]
        public void AdaptionMod_DefaultTableAndCap()
        {
            // 標準: S=1.4, A=1.2, B=1.0, C=0.8, D=0.6, -=0。S でない限り補正で S にはならない
            var (src, u) = Create(UnitBlock("テスト機", adaption: "SBCD"), PilotBlock("パイロット", "パイロット", adaption: "SSSS"));
            UnitCoreFixture.Board(src, u, "パイロット");
            Assert.AreEqual(1.4d, u.get_AdaptionMod(1, 0));
            Assert.AreEqual(1d, u.get_AdaptionMod(2, 0));
            Assert.AreEqual(0.8d, u.get_AdaptionMod(3, 0));
            Assert.AreEqual(0.6d, u.get_AdaptionMod(4, 0));
            Assert.AreEqual(1.2d, u.get_AdaptionMod(2, 5));
        }

        [TestMethod]
        public void AdaptionMod_OptionTables()
        {
            var (src, u) = Create(UnitBlock("テスト機", adaption: "SABD"), PilotBlock("パイロット", "パイロット", adaption: "SSSS"));
            UnitCoreFixture.Board(src, u, "パイロット");
            // 地形適応修正繰り下げ: S=1.2, A=1.0, B=0.8, D=0.4
            DefineOption(src, "地形適応修正繰り下げ");
            Assert.AreEqual(1.2d, u.get_AdaptionMod(1, 0));
            Assert.AreEqual(1d, u.get_AdaptionMod(2, 0));
            Assert.AreEqual(0.8d, u.get_AdaptionMod(3, 0));
            Assert.AreEqual(0.4d, u.get_AdaptionMod(4, 0));
            // 緩和＋繰り下げ: S=1.1, A=1.0, B=0.9, D=0.7
            DefineOption(src, "地形適応修正緩和");
            Assert.AreEqual(1.1d, u.get_AdaptionMod(1, 0));
            Assert.AreEqual(1d, u.get_AdaptionMod(2, 0));
            Assert.AreEqual(0.9d, u.get_AdaptionMod(3, 0));
            Assert.AreEqual(0.7d, u.get_AdaptionMod(4, 0));
        }

        [TestMethod]
        public void AdaptionMod_RelaxedOption()
        {
            // 地形適応修正緩和のみ: S=1.2, A=1.1, B=1.0, C=0.9, D=0.8
            var (src, u) = Create(UnitBlock("テスト機", adaption: "SACD"), PilotBlock("パイロット", "パイロット", adaption: "SSSS"));
            UnitCoreFixture.Board(src, u, "パイロット");
            DefineOption(src, "地形適応修正緩和");
            Assert.AreEqual(1.2d, u.get_AdaptionMod(1, 0));
            Assert.AreEqual(1.1d, u.get_AdaptionMod(2, 0));
            Assert.AreEqual(0.9d, u.get_AdaptionMod(3, 0));
            Assert.AreEqual(0.8d, u.get_AdaptionMod(4, 0));
        }

        // ──────────────────────────────────────────────
        // 装甲・運動性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Armor_ConditionModifiers()
        {
            // 装甲劣化・凍結で半減、石化で倍
            var (_, u) = Create(UnitBlock("テスト機", armor: 800));
            Assert.AreEqual(800, u.get_Armor(""));
            u.AddCondition("装甲劣化", 1);
            Assert.AreEqual(400, u.get_Armor(""));
            Assert.AreEqual(400, u.get_Armor("基本値"));
            u.AddCondition("石化", 1);
            Assert.AreEqual(800, u.get_Armor(""));
            u.AddCondition("凍結", 1);
            Assert.AreEqual(400, u.get_Armor("基本値"));
            // 修正値はパイロット由来の修正のみ（ここでは 0）
            Assert.AreEqual(0, u.get_Armor("修正値"));
        }

        [TestMethod]
        public void ArmorMobility_AuraConverterUsesPilotAura()
        {
            // オーラ変換器: 装甲 +50×オーラ、運動性 +2×オーラ、最大HP +100×、最大EN +10×（レベルで上限）
            var (src, u) = Create(
                UnitBlock("オーラ機", hp: 3000, en: 100, armor: 800, mobility: 90, features: "オーラ変換器Lv2"),
                PilotBlock("聖戦士", "聖戦士", skills: "オーラLv3, 1"));
            UnitCoreFixture.Board(src, u, "聖戦士");

            Assert.AreEqual(2d, u.AuraLevel());
            Assert.AreEqual(3d, u.AuraLevel(no_limit: true));
            Assert.AreEqual(900, u.get_Armor(""));
            Assert.AreEqual(800, u.get_Armor("基本値"));
            Assert.AreEqual(100, u.get_Armor("修正値"));
            Assert.AreEqual(94, u.get_Mobility(""));
            Assert.AreEqual(90, u.get_Mobility("基本値"));
            Assert.AreEqual(4, u.get_Mobility("修正値"));
            Assert.AreEqual(3200, u.MaxHP);
            Assert.AreEqual(120, u.MaxEN);
        }

        [TestMethod]
        public void ArmorMobility_PsychicDriveUsesPsychicLevel()
        {
            // サイキックドライブ: 装甲 +100×超能力、運動性 +5×超能力
            var (src, u) = Create(
                UnitBlock("サイキック機", armor: 800, mobility: 90, features: "サイキックドライブ"),
                PilotBlock("超能力者", "超能力者", skills: "超能力Lv2, 1"));
            UnitCoreFixture.Board(src, u, "超能力者");

            Assert.AreEqual(2d, u.PsychicLevel());
            Assert.AreEqual(1000, u.get_Armor(""));
            Assert.AreEqual(100, u.get_Mobility(""));
        }

        // ──────────────────────────────────────────────
        // 画像
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bitmap_FeatureAndConditionOverride()
        {
            // ユニット画像状態 > ユニット画像能力 > データの画像
            var (_, u) = Create(UnitBlock("テスト機"));
            Assert.AreEqual("test.bmp", u.get_Bitmap(false));
            Assert.AreEqual("test.bmp", u.CurrentBitmap());

            var (_, f) = Create(UnitBlock("画像機", features: "ユニット画像=other.bmp"));
            Assert.AreEqual("other.bmp", f.get_Bitmap(false));

            f.AddCondition("ユニット画像", -1, cdata: "ユニット画像 cond.bmp");
            Assert.AreEqual("cond.bmp", f.get_Bitmap(false));

            // 非表示付加ではダミー画像
            f.AddCondition("非表示付加", -1);
            Assert.AreEqual("-.bmp", f.CurrentBitmap());
        }

        // ──────────────────────────────────────────────
        // 修理費・経験値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Value_RankAndBossRankAndFeature()
        {
            // 修理費 = (基本値 + 1000×修理費修正Lv) × (1 + 0.5×ボスランク + 0.05×ランク)
            var (_, u) = Create(UnitBlock("テスト機", value: 2000));
            Assert.AreEqual(2000, u.Value);
            u.Rank = 2;
            Assert.AreEqual(2200, u.Value);
            u.BossRank = 2;
            Assert.AreEqual(4200, u.Value);

            var (_, f) = Create(UnitBlock("割増機", value: 2000, features: "修理費修正Lv-3"));
            // 修正後の基本値は 0 未満にならない
            Assert.AreEqual(0, f.Value);
        }

        [TestMethod]
        [Ignore("移植の不具合: 修理費の端数を切り捨てている（VB は Long 代入で丸める）")]
        public void Value_FractionIsRoundedLikeVbLong()
        {
            // VB: Value = Value * (1 + 0.05 * Rank) を Long に代入するので四捨五入（偶数丸め）される
            // 1030 × 1.05 = 1081.5 → 1082
            var (_, v) = Create(UnitBlock("テスト機２", value: 1030));
            v.Rank = 1;
            Assert.AreEqual(1082, v.Value);
        }

        [TestMethod]
        public void ExpValue_FeatureAndBossRank()
        {
            // 経験値 = 基本値 + 10×経験値修正Lv（0 未満にならない）+ 20×ボスランク
            var (_, u) = Create(UnitBlock("テスト機", exp: 100, features: "経験値修正Lv3"));
            Assert.AreEqual(130, u.ExpValue);
            u.BossRank = 2;
            Assert.AreEqual(170, u.ExpValue);

            var (_, n) = Create(UnitBlock("低経験値機", exp: 20, features: "経験値修正Lv-5"));
            Assert.AreEqual(0, n.ExpValue);
        }
    }
}
