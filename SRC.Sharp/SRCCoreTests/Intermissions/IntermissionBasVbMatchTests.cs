using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests;
using SRCCore.Items;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// 元の SRC の Intermission.bas と分岐ごとに突き合わせて見つかった違い（7474/SRC#1498）。
    /// 期待値は元の SRC（SRC/SRC_20121125/Intermission.bas）の動作から取る。
    /// </summary>
    [TestClass]
    public class IntermissionBasVbMatchTests
    {
        private sealed class RecordingPlayer : IPlaySound
        {
            public List<string> Calls { get; } = new List<string>();
            public BGMStatus BGMStatus => BGMStatus.Playing;
            public float SoundVolume { get; set; }
            public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode) => Calls.Add("Play");
            public void Stop(int channel) => Calls.Add("Stop");
            public SoundType ResolveSoundType(string path) => SoundType.Midi;
            public void Dispose() { }
        }

        private static string W(int n) => Strings.StrConv(SrcFormatter.Format(n), VbStrConv.Wide);

        // ──────────────────────────────────────────────
        // InterMissionCommand
        // ──────────────────────────────────────────────

        private static List<string> StopCallsBeforeMenu(string playing, string intermissionBgm)
        {
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            // ＢＧＭのファイルは見つからない（StartBGM は探すだけで鳴らさない）
            src.FileSystem = new SRCCore.Filesystem.LocalFileSystem();
            src.ScenarioPath = Path.Combine(Path.GetTempPath(), "srcsharp-intermission-bgm-" + Guid.NewGuid().ToString("N"));
            src.Expression.SetVariableAsString("BGM(Intermission)", intermissionBgm);
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            var player = new RecordingPlayer();
            src.Sound.Player = player;
            src.Sound.BGMFileName = playing;
            List<string> calls = null;
            gui.ListBoxHandler = _ =>
            {
                calls ??= player.Calls.ToList();
                return 1; // 次のステージへ
            };

            src.InterMission.InterMissionCommand(skip_update: true);
            return calls;
        }

        [TestMethod]
        public void Bgm_NumberedVariantOfIntermissionBgm_IsRestarted()
        {
            // Intermission.bas:24 InStr(BGMFileName, "\" & BGMName("Intermission")) = 0 なら StopBGM・StartBGM。
            // ChangeBGM と違い、番号違い（(2) など）のファイルを同じ曲とはみなさない
            var calls = StopCallsBeforeMenu(@"C:\SRC\Midi\Intermission(2).mid", "Intermission.mid");
            CollectionAssert.Contains(calls, "Stop");
        }

        [TestMethod]
        public void Bgm_SameIntermissionBgm_KeepsPlaying_EvenWithSlashPath()
        {
            // 同じＢＧＭを演奏中なら止めない（パスの区切りが / でも同じ）
            var calls = StopCallsBeforeMenu("/home/user/SRC/Midi/Intermission.mid", "Intermission.mid");
            CollectionAssert.DoesNotContain(calls, "Stop");
        }

        [TestMethod]
        public void Bgm_SameIntermissionBgm_WithoutFolder_KeepsPlaying()
        {
            // 7474/SRC#1534: SRC# は読み込み対象のフォルダの直下で見つけた曲を、フォルダの付かない名前で BGMFileName に入れる。
            // 元の SRC の BGMFileName には必ずフォルダが付くので、同じ曲なら InStr(BGMFileName, "\" & BGMName("Intermission")) > 0 で止めない
            var calls = StopCallsBeforeMenu("Intermission.mid", "Intermission.mid");
            CollectionAssert.DoesNotContain(calls, "Stop");
        }

        [TestMethod]
        public void Bgm_SameBodyWithOtherExtension_IsRestarted()
        {
            // Intermission.bas:24 は拡張子まで含めた BGMName("Intermission") で比べる（ChangeBGM の本体部分の比較とは違う）
            var calls = StopCallsBeforeMenu(@"C:\SRC\Midi\Intermission.mp3", "Intermission.mid");
            CollectionAssert.Contains(calls, "Stop");
        }

        [TestMethod]
        public void UserCommand_LabelOfBuiltinCommand_RunsBuiltinCommand()
        {
            // Intermission.bas:183 Select Case cmd_list(ret) はコマンドの表示名で分ける。
            // ユーザー定義のコマンドの名前が「機体改造」なら機体改造を行う
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            IntermissionCoverageFixture.SetOption(src, "改造不可");
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            src.Expression.DefineGlobalVariable("コマンド名");
            src.Expression.SetVariableAsString("コマンド名", "機体改造");
            src.Expression.DefineGlobalVariable("IntermissionCommand(コマンド名)");
            src.Expression.SetVariableAsString("IntermissionCommand(コマンド名)", "user.eve");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return args.Items.Select(x => x.Text).ToList().IndexOf("機体改造") + 1;
                    case 2: return 0; // 機体改造のユニット選択をキャンセル
                    default: return 1; // 次のステージへ
                }
            };

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual(3, lists.Count);
            StringAssert.StartsWith(lists[1].lb_caption, "ユニット選択：");
        }

        // ──────────────────────────────────────────────
        // RankUpCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUp_RowAndHeader_MatchVbLayout()
        {
            // Intermission.bas:393-435・608-613: ランクは 10 未満なら全角、ＥＮ・運動性は 4 桁、装甲は 6 桁
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Money = 1000000;
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", rank: 3);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.RankUpCommand();

            var expected = GeneralLib.RightPaddedString("試作機", 33) + W(3)
                + GeneralLib.LeftPaddedString("40000", 7)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 4)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 6)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 4);
            Assert.AreEqual(expected, captured.Items[1].Text);
            var e = src.Expression;
            Assert.AreEqual("ユニット                     " + e.Term("ランク", null, 6) + "   費用  "
                + e.Term("ＨＰ", null, 4) + " " + e.Term("ＥＮ", null, 4) + " "
                + e.Term("装甲", null, 4) + " " + e.Term("運動", null, 4) + " パイロット", captured.lb_info);
        }

        [TestMethod]
        public void RankUp_SortWithTies_UsesVbSelectionSort()
        {
            // Intermission.bas:499-527 の選択ソート（同じ値は入れ替えない・入れ替えで順が崩れる）。
            // ＨＰが 1000, 1000, 2000 の順なら、1 番目と 3 番目を入れ替えて C, B, A になる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var a = IntermissionCoverageFixture.CreateUnit(src, "Ａ機", hp: 1000);
            var b = IntermissionCoverageFixture.CreateUnit(src, "Ｂ機", hp: 1000);
            var c = IntermissionCoverageFixture.CreateUnit(src, "Ｃ機", hp: 2000);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.RankUpCommand();

            CollectionAssert.AreEqual(new[] { c.ID, b.ID, a.ID }, captured.Items.Skip(1).Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void RankUp_ResortStartsFromPreviousOrder()
        {
            // 元の SRC は並べ替えた一覧をそのまま次の並べ替えに使う。
            // ＨＰ順（C, B, A）のあと、全て同じ値のランクで並べ替えても順は変わらない
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var a = IntermissionCoverageFixture.CreateUnit(src, "Ａ機", hp: 1000);
            var b = IntermissionCoverageFixture.CreateUnit(src, "Ｂ機", hp: 1000);
            var c = IntermissionCoverageFixture.CreateUnit(src, "Ｃ機", hp: 2000);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 1;
                    case 2: return args.Items.Select(x => x.ListItemID).ToList().IndexOf("ユニットランク") + 1;
                    default: return 0;
                }
            };

            src.InterMission.RankUpCommand();

            CollectionAssert.AreEqual(new[] { c.ID, b.ID, a.ID }, lists[2].Items.Skip(1).Select(x => x.ListItemID).ToList());
        }

        // ──────────────────────────────────────────────
        // ExchangeUnitCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeUnit_PilotList_CaptionLevelAndSort()
        {
            // Intermission.bas:1159-1161・1236・1312-1313: 題は「パイロット選択」、レベルは全角、
            // 並べ替えのキーは 500 * Level + Exp
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var p1 = IntermissionCoverageFixture.CreatePilot(src, "新人", level: 5);
            var p2 = IntermissionCoverageFixture.CreatePilot(src, "古参", level: 5);
            p2.Exp = 100;
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeUnitCommand();

            Assert.AreEqual("パイロット選択", captured.lb_caption);
            Assert.AreEqual(" パイロット            レベル  ユニット", captured.lb_info);
            CollectionAssert.AreEqual(new[] { p2.ID, p1.ID }, captured.Items.Skip(1).Select(x => x.ListItemID).ToList());
            Assert.AreEqual(GeneralLib.RightPaddedString(" 古参", 25) + GeneralLib.LeftPaddedString(W(5), 4), captured.Items[1].Text);
        }

        [TestMethod]
        public void ExchangeUnit_UnitList_CaptionAndRank()
        {
            // Intermission.bas:1408-1417・1586-1597: 題は「乗り換え先選択 ： …」、ランクは全角
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var target = IntermissionCoverageFixture.CreateUnit(src, "新型機", rank: 2);
            var current = IntermissionCoverageFixture.CreatePilot(src, "前任者", level: 1);
            current.Ride(target);
            var p = IntermissionCoverageFixture.CreatePilot(src, "後任者", level: 10);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count == 1 ? args.Items.Select(x => x.ListItemID).ToList().IndexOf(p.ID) + 1 : 0;
            };

            src.InterMission.ExchangeUnitCommand();

            Assert.AreEqual("乗り換え先選択 ： 後任者", lists[1].lb_caption);
            StringAssert.StartsWith(lists[1].lb_info, " ユニット                           パイロット         ");
            Assert.AreEqual(" " + GeneralLib.RightPaddedString("新型機", 35) + GeneralLib.RightPaddedString("前任者", 21) + " " + W(2),
                lists[1].Items[1].Text);
        }

        // ──────────────────────────────────────────────
        // ExchangeItemCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeItem_UnitList_CountsFixedItemsAndShowsRank()
        {
            // Intermission.bas:1759-1797: 「固定」でも「非表示」が無ければ数えてコメントに並べる。ランクは全角
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 3, rank: 1);
            u.AddItem(IntermissionCoverageFixture.CreateItem(src, "固定装甲", iclass: "固定"));
            u.AddItem(IntermissionCoverageFixture.CreateItem(src, "ブースター"));
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeItemCommand();

            var row = captured.Items[1];
            StringAssert.StartsWith(row.Text, GeneralLib.RightPaddedString("試作機", 31) + "2/3      " + W(1));
            Assert.AreEqual("固定装甲 ブースター ", row.ListItemComment);
        }

        [TestMethod]
        public void ExchangeItem_EquipList_ShowsFixedItemAndPutsShieldInLeftHand()
        {
            // Intermission.bas:2137-2161・2247-2251: 「固定」のアイテムも並べて選べない印を付ける。
            // 盾はいつも左手に割り当てる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var ud = IntermissionCoverageFixture.DefineUnit(src, "騎士", uclass: "(戦士)", itemNum: 1);
            ud.AddFeature("装備個所=腕");
            var u = IntermissionCoverageFixture.CreateUnit(src, "騎士", uclass: "(戦士)", itemNum: 1);
            var shield = IntermissionCoverageFixture.CreateItem(src, "木の盾", part: "盾");
            var ring = IntermissionCoverageFixture.CreateItem(src, "呪縛の輪", part: "アイテム", iclass: "固定");
            u.AddItem0(shield);
            u.AddItem0(ring);
            ListBoxArgs captured = null;

            gui.ListBoxHandler = args => { captured = args; return 0; };
            src.InterMission.ExchangeItemCommand(u, "");

            var texts = captured.Items.Select(x => x.Text).ToList();
            Assert.AreEqual(GeneralLib.RightPaddedString("----", 23) + "右手", texts[0]);
            Assert.AreEqual(GeneralLib.RightPaddedString("木の盾", 22) + " 左手", texts[1]);
            Assert.AreEqual(GeneralLib.RightPaddedString("呪縛の輪", 22) + " アイテム", texts[2]);
            Assert.IsTrue(captured.Items[2].ListItemFlag);
        }

        [TestMethod]
        public void ExchangeItem_EquipList_ItemWithoutSlot_IsAppended()
        {
            // Intermission.bas:2211-2216: 合う装備個所が無いアイテムは、その装備個所の行を足して並べる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            var armor = IntermissionCoverageFixture.CreateItem(src, "鎧", part: "体");
            u.AddItem0(armor);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeItemCommand(u, "");

            CollectionAssert.AreEqual(new[]
            {
                GeneralLib.RightPaddedString("----", 23) + "強化パーツ",
                GeneralLib.RightPaddedString("鎧", 22) + " 体",
                "▽装備解除▽",
            }, captured.Items.Select(x => x.Text).ToList());
        }

        [TestMethod]
        public void ExchangeItem_ItemChoices_ShowUnusedPerUsableCountAndComment()
        {
            // Intermission.bas:2563-2591: 数量は「未装備の数/使える数」、コメントはアイテムの解説
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            var other = IntermissionCoverageFixture.CreateUnit(src, "僚機", itemNum: 1);
            IntermissionCoverageFixture.CreateItem(src, "ブースター");
            other.AddItem(IntermissionCoverageFixture.CreateItem(src, "ブースター"));
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count == 1 ? 1 : 0;
            };

            src.InterMission.ExchangeItemCommand(u, "");

            var choice = lists[1].Items.Single();
            Assert.AreEqual(GeneralLib.RightPaddedString("ブースター", 22) + " "
                + GeneralLib.RightPaddedString("強化パーツ", 15) + " 1/ 2", choice.Text);
            Assert.AreEqual("ブースターの説明", choice.ListItemComment);
        }

        [TestMethod]
        public void ExchangeItem_CancelSourceUnit_ReturnsToItemChoices()
        {
            // Intermission.bas:2751-2775: 入手先の選択をキャンセルすると、装備するアイテムの選択に戻る
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            var other = IntermissionCoverageFixture.CreateUnit(src, "僚機", itemNum: 1);
            other.AddItem(IntermissionCoverageFixture.CreateItem(src, "ブースター"));
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 1; // スロット
                    case 2: return 1; // ブースター
                    default: return 0; // 入手先をキャンセル → …
                }
            };

            src.InterMission.ExchangeItemCommand(u, "");

            StringAssert.StartsWith(lists[2].lb_caption, "ブースターの入手先を選択");
            StringAssert.StartsWith(lists[3].lb_caption, "装備するアイテムを選択");
        }

        // ──────────────────────────────────────────────
        // ExchangeFormCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeForm_RowsHeaderAndBossRank()
        {
            // Intermission.bas:2848-2882・2938-2950・2974-2991: ランクは全角、パイロットはレベルではなく名前、
            // 見出しに費用は無い。換装先は BossRank も合わせ、適応を表示する
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var heavy = IntermissionCoverageFixture.DefineUnit(src, "砲撃装備", hp: 2000);
            var basic = IntermissionCoverageFixture.DefineUnit(src, "標準装備", hp: 1500);
            basic.AddFeature("換装=砲撃装備");
            heavy.AddFeature("換装=標準装備");
            var u = IntermissionCoverageFixture.CreateUnit(src, "標準装備", hp: 1500, rank: 3);
            IntermissionCoverageFixture.CreatePilot(src, "操縦者", level: 7).Ride(u);
            u.BossRank = 1;
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count == 1 ? 1 : 0;
            };

            src.InterMission.ExchangeFormCommand();

            var e = src.Expression;
            Assert.AreEqual(GeneralLib.RightPaddedString("標準装備", 33) + W(3)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5)
                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5)
                + "  操縦者", lists[0].Items[0].Text);
            Assert.AreEqual("ユニット                     " + e.Term("ランク", null, 6) + "   "
                + e.Term("ＨＰ", null, 4) + " " + e.Term("ＥＮ", null, 4) + " "
                + e.Term("装甲", null, 4) + " " + e.Term("運動", null, 4) + " パイロット", lists[0].lb_info);
            var form = u.OtherForm("砲撃装備");
            Assert.AreEqual(1, form.BossRank);
            StringAssert.Contains(lists[1].Items[0].Text, " AAAA");
            Assert.AreEqual("砲撃装備", lists[1].Items[0].ListItemID);
        }

        // ──────────────────────────────────────────────
        // InStatusCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStatusCommand_BackslashPath_IsStatusCommand()
        {
            // Intermission.bas:3068 InStr(ScenarioFileName, "\ユニットステータス表示.eve")。区切りが \ のパスでも判定する
            var (src, _) = IntermissionCoverageFixture.CreateSrc();
            src.ScenarioFileName = @"C:\SRC\Lib\ユニットステータス表示.eve";
            Assert.IsTrue(src.InterMission.InStatusCommand());
        }
    }
}
