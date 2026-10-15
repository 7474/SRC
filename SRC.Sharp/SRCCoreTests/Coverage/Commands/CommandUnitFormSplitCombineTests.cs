using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.unitform.cs の「分離」「合体」「換装」コマンドのテスト。
    /// 元の SRC の Command.bas SplitCommand / CombineCommand / ExchangeFormCommand に対応する。
    /// </summary>
    [TestClass]
    public class CommandUnitFormSplitCombineTests
    {
        private static void Run(CommandUnitFormFixture f, int cmdId, string label)
        {
            f.Src.Commands.UnitCommand(new UiCommand(cmdId, label));
        }

        /// <summary>
        /// ２体合体（合体形態 = 子1 + 子2）のデータを用意し、合体形態にパイロット２人を乗せて返す。
        /// </summary>
        private static Unit CreateCombinedUnit(CommandUnitFormFixture f, int x, int y)
        {
            f.AddUnitData("合体形態", "分離=分離 子1 子2").PilotNum = 2;
            f.AddUnitData("子1", "合体=合体 合体形態 子2").PilotNum = 1;
            f.AddUnitData("子2", "合体=合体 合体形態 子1").PilotNum = 1;
            var c1 = f.Src.UList.Add("子1", 0, "味方");
            var c2 = f.Src.UList.Add("子2", 0, "味方");
            c1.Status = "旧主形態";
            c2.Status = "旧形態";
            var u = f.CreateUnit("合体形態", "パイロット1", x: x, y: y);
            f.AddPilotData("パイロット2");
            var p2 = f.Src.PList.Add("パイロット2", 1, "味方");
            p2.Ride(u);
            return u;
        }

        // ──────────────────────────────────────────────
        // 分離
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Split_OnMap_ConfirmOk_SplitsAndSelectsFirstUnit()
        {
            // 「分離しますか？」に OK すると分離し、分離=… の第２要素のユニットを選択する
            var f = new CommandUnitFormFixture(onMap: true);
            var u = CreateCombinedUnit(f, 4, 4);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.SplitCmdID, "分離");

            Assert.AreEqual(("分離しますか？", "分離"), f.Confirms.Single());
            var c1 = f.Src.UList.Item("子1");
            var c2 = f.Src.UList.Item("子2");
            Assert.AreSame(c1, f.Src.Commands.SelectedUnit);
            Assert.AreEqual("出撃", c1.Status);
            Assert.AreEqual("出撃", c2.Status);
            Assert.AreSame(c1, f.Src.Map.MapDataForUnit[4, 4]);
            Assert.AreEqual("パイロット1", c1.MainPilot().Name);
            Assert.AreEqual("パイロット2", c2.MainPilot().Name);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Split_OnMap_ConfirmCancel_DoesNotSplit()
        {
            // 確認でキャンセルすると分離しない
            var f = new CommandUnitFormFixture(onMap: true);
            var u = CreateCombinedUnit(f, 4, 4);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ConfirmAnswer = _ => GuiDialogResult.Cancel;

            Run(f, Command.SplitCmdID, "分離");

            Assert.AreSame(u, f.Src.Map.MapDataForUnit[4, 4]);
            Assert.AreEqual("出撃", u.Status);
            Assert.AreEqual("旧主形態", f.Src.UList.Item("子1").Status);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Split_StatusView_SplitsWithoutConfirm()
        {
            // ユニットステータスコマンドでは確認なしで分離する
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateCombinedUnit(f, 0, 0);
            u.Status = "待機";
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.SplitCmdID, "分離");

            Assert.AreEqual(0, f.Confirms.Count);
            Assert.AreEqual("パイロット1", f.Src.UList.Item("子1").MainPilot().Name);
            Assert.AreEqual("パイロット2", f.Src.UList.Item("子2").MainPilot().Name);
            Assert.AreEqual(0, u.CountPilot());
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void PartsSplit_OnMap_ConfirmOk_TransformsToPartsForm()
        {
            // パーツ分離=名称 形態 は「パーツを分離しますか？」の確認後、第２要素の形態に変形する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("装甲形態", "パーツ分離=装甲分離 本体");
            f.AddUnitData("本体", "パーツ合体=装甲形態");
            var u = f.CreateUnit("装甲形態", "パイロット", x: 2, y: 5);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.SplitCmdID, "装甲分離");

            Assert.AreEqual(("パーツを分離しますか？", "パーツ分離"), f.Confirms.Single());
            var selected = f.Src.Commands.SelectedUnit;
            Assert.AreEqual("本体", selected.Name);
            Assert.AreSame(selected, f.Src.Map.MapDataForUnit[2, 5]);
        }

        [TestMethod]
        public void PartsSplit_OnMap_CannotEnterTerrain_ShowsMessageAndCancels()
        {
            // パーツ分離後の形態が地形に進入できないなら「この地形では分離できません」
            var f = new CommandUnitFormFixture(onMap: true);
            f.SetTerrain(2, 5, new TerrainData { ID = 1, Name = "海", Class = "水", MoveCost = 2 });
            f.AddUnitData("装甲形態", "パーツ分離=装甲分離 本体").Transportation = "陸空";
            f.AddUnitData("本体").Adaption = "AA-A"; // 水適応なし
            var u = f.CreateUnit("装甲形態", "パイロット", x: 2, y: 5);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.SplitCmdID, "装甲分離");

            CollectionAssert.AreEqual(
                new[] { ("パーツを分離しますか？", "パーツ分離"), ("この地形では分離できません", "分離") },
                f.Confirms.ToArray());
            Assert.AreSame(u, f.Src.Map.MapDataForUnit[2, 5]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void PartsSplit_StatusView_TransformsWithoutConfirm()
        {
            // ユニットステータスコマンドではパーツ分離も確認なし
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("装甲形態", "パーツ分離=装甲分離 本体");
            f.AddUnitData("本体", "パーツ合体=装甲形態");
            var u = f.CreateUnit("装甲形態", "パイロット");
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.SplitCmdID, "装甲分離");

            Assert.AreEqual(0, f.Confirms.Count);
            Assert.AreEqual("本体", u.CurrentForm().Name);
        }

        // ──────────────────────────────────────────────
        // 合体
        // ──────────────────────────────────────────────

        /// <summary>
        /// ３体合体（合体形態 = A + B + C）のデータを用意する。
        /// </summary>
        private static (Unit a, Unit b, Unit c) CreateTripleParts(CommandUnitFormFixture f,
            (int x, int y) pa, (int x, int y) pb, (int x, int y) pc)
        {
            f.AddUnitData("合体形態", "分離=分離 A機 B機 C機").PilotNum = 3;
            f.AddUnitData("A機", "合体=合体 合体形態 B機 C機").PilotNum = 1;
            f.AddUnitData("B機", "合体=合体 合体形態 A機 C機").PilotNum = 1;
            f.AddUnitData("C機", "合体=合体 合体形態 A機 B機").PilotNum = 1;
            var combined = f.Src.UList.Add("合体形態", 0, "味方");
            combined.Status = "他形態";
            var a = f.CreateUnit("A機", "パイロットA", x: pa.x, y: pa.y);
            var b = f.CreateUnit("B機", "パイロットB", x: pb.x, y: pb.y);
            var c = f.CreateUnit("C機", "パイロットC", x: pc.x, y: pc.y);
            return (a, b, c);
        }

        [TestMethod]
        public void Combine_OnMap_TripleCombine_CombinesAndConsumesAction()
        {
            // ３体合体でパートナーが２マス以内にいれば合体し、合体後のユニットを選択して行動を消費する
            var f = new CommandUnitFormFixture(onMap: true);
            var (a, b, c) = CreateTripleParts(f, (3, 3), (4, 3), (3, 4));
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            // 合体パターンが１つなのでリストは出さない
            Assert.AreEqual(0, f.ListBoxes.Count);
            var combined = f.Src.UList.Item("合体形態");
            Assert.AreSame(combined, f.Src.Commands.SelectedUnit);
            Assert.AreEqual("出撃", combined.Status);
            Assert.AreSame(combined, f.Src.Map.MapDataForUnit[3, 3]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[4, 3]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[3, 4]);
            Assert.AreEqual(3, combined.CountPilot());
            Assert.AreEqual(1, combined.UsedAction);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Combine_OnMap_PartnerTooFar_IsNotCombined()
        {
            // パートナーが２マスより離れていれば合体候補にならない
            var f = new CommandUnitFormFixture(onMap: true);
            var (a, b, c) = CreateTripleParts(f, (1, 1), (2, 1), (6, 6));
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            // 候補なしでリスト（空）をキャンセル → 合体しない
            Assert.AreEqual(0, f.ListBoxes.Single().Items.Count);
            Assert.AreEqual("他形態", f.Src.UList.Item("合体形態").Status);
            Assert.AreSame(a, f.Src.Map.MapDataForUnit[1, 1]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Combine_OnMap_RunsCombineEventThenEndsAction()
        {
            // 合体後に「合体」イベントを実行し、その後は行動終了（待機）として「行動終了」イベントも起きる
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "合体 全 合体形態:",
                "Set 記録 (記録 & \"合体\")",
                "Exit",
                "行動終了 パイロットA:",
                "Set 記録 (記録 & \"終了\")",
                "Exit");
            var (a, b, c) = CreateTripleParts(f, (3, 3), (4, 3), (3, 4));
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            Assert.AreEqual("合体終了", f.Record);
            Assert.AreEqual("出撃", f.Src.UList.Item("合体形態").Status);
        }

        [TestMethod]
        public void Combine_OnMap_CancelInCombineEvent_SkipsActionEnd()
        {
            // 合体イベントで Cancel された場合は合体はそのままで、行動終了の処理（行動終了イベント）は行わない
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "合体 全 合体形態:",
                "Set 記録 (記録 & \"合体\")",
                "Cancel",
                "Exit",
                "行動終了 パイロットA:",
                "Set 記録 (記録 & \"終了\")",
                "Exit");
            var (a, b, c) = CreateTripleParts(f, (3, 3), (4, 3), (3, 4));
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            Assert.AreEqual("合体", f.Record);
            Assert.AreEqual("出撃", f.Src.UList.Item("合体形態").Status);
            Assert.IsFalse(f.Src.IsCanceled);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Combine_OnMap_PartnerDiagonalWithinTwo_IsCombined()
        {
            // 元の SRC は Abs(dx) > 2 Or Abs(dy) > 2 のときだけ除外する（縦横それぞれ２マス以内の正方形）。
            // (3,3) から見て (5,4) は dx=2, dy=1 なので合体できる
            var f = new CommandUnitFormFixture(onMap: true);
            var (a, b, c) = CreateTripleParts(f, (3, 3), (5, 4), (3, 4));
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            Assert.AreEqual(0, f.ListBoxes.Count);
            Assert.AreEqual("出撃", f.Src.UList.Item("合体形態").Status);
        }

        [TestMethod]
        public void Combine_StatusView_TwoUnitCombine_IsCombined()
        {
            // 元の SRC はマップがない（ユニットステータスコマンド）場合、２体合体も合体候補に含め、
            // 距離も判定しない
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("合体形態", "分離=分離 子1 子2").PilotNum = 2;
            f.AddUnitData("子1", "合体=合体 合体形態 子2").PilotNum = 1;
            f.AddUnitData("子2", "合体=合体 合体形態 子1").PilotNum = 1;
            var combined = f.Src.UList.Add("合体形態", 0, "味方");
            combined.Status = "他形態";
            var c1 = f.CreateUnit("子1", "パイロット1", x: 1, y: 1);
            f.CreateUnit("子2", "パイロット2", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = c1;

            Run(f, Command.CombineCmdID, "合体");

            Assert.AreEqual(0, f.ListBoxes.Count);
            Assert.AreEqual(2, f.Src.UList.Item("合体形態").CountPilot());
        }

        [TestMethod]
        public void Combine_OnMap_MultiplePatterns_ListShowsCombinedFormNames()
        {
            // 元の SRC は list に LIndex(合体データ, 2)（合体後の形態名）を入れて「合体後の形態」リストに表示する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("合体形態1", "分離=分離 A機 B機 C機").PilotNum = 3;
            f.AddUnitData("合体形態2", "分離=分離 A機 B機 C機").PilotNum = 3;
            f.AddUnitData("A機", "合体=合体 合体形態1 B機 C機", "合体=合体 合体形態2 B機 C機").PilotNum = 1;
            f.AddUnitData("B機").PilotNum = 1;
            f.AddUnitData("C機").PilotNum = 1;
            f.Src.UList.Add("合体形態1", 0, "味方").Status = "他形態";
            f.Src.UList.Add("合体形態2", 0, "味方").Status = "他形態";
            var a = f.CreateUnit("A機", "パイロットA", x: 3, y: 3);
            f.CreateUnit("B機", "パイロットB", x: 4, y: 3);
            f.CreateUnit("C機", "パイロットC", x: 3, y: 4);
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.CombineCmdID, "合体");

            var lb = f.ListBoxes.Single();
            Assert.AreEqual("合体後の形態", lb.lb_caption);
            Assert.AreEqual("合体形態1|合体形態2", string.Join("|", lb.Items.Select(x => x.Text)));
        }

        [TestMethod]
        public void Combine_OnMap_MultiplePatterns_SelectsChosenCombination()
        {
            // 合体パターンが複数あれば「合体後の形態」リストで選択したパターンで合体する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("合体形態1", "分離=分離 A機 B機 C機").PilotNum = 3;
            f.AddUnitData("合体形態2", "分離=分離 A機 B機 C機").PilotNum = 3;
            f.AddUnitData("A機", "合体=合体 合体形態1 B機 C機", "合体=合体 合体形態2 B機 C機").PilotNum = 1;
            f.AddUnitData("B機").PilotNum = 1;
            f.AddUnitData("C機").PilotNum = 1;
            f.Src.UList.Add("合体形態1", 0, "味方").Status = "他形態";
            f.Src.UList.Add("合体形態2", 0, "味方").Status = "他形態";
            var a = f.CreateUnit("A機", "パイロットA", x: 3, y: 3);
            f.CreateUnit("B機", "パイロットB", x: 4, y: 3);
            f.CreateUnit("C機", "パイロットC", x: 3, y: 4);
            f.Src.Commands.SelectedUnit = a;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 2;

            Run(f, Command.CombineCmdID, "合体");

            Assert.AreEqual(2, f.ListBoxes.Single().Items.Count);
            Assert.AreEqual("合体後の形態", f.ListBoxes[0].lb_caption);
            Assert.AreEqual("他形態", f.Src.UList.Item("合体形態1").Status);
            Assert.AreEqual("出撃", f.Src.UList.Item("合体形態2").Status);
            Assert.AreSame(f.Src.UList.Item("合体形態2"), f.Src.Commands.SelectedUnit);
        }

        [TestMethod]
        public void PartsCombine_StatusView_TransformsToCombinedForm()
        {
            // ユニットステータスコマンドで「パーツ合体」を選ぶとパーツ合体=形態 へ変形する
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("装甲形態", "パーツ分離=装甲分離 本体");
            f.AddUnitData("本体", "パーツ合体=装甲形態");
            var u = f.CreateUnit("本体", "パイロット");
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.CombineCmdID, "パーツ合体");

            Assert.AreEqual("装甲形態", u.CurrentForm().Name);
            Assert.AreEqual(0, f.ListBoxes.Count);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 換装
        // ──────────────────────────────────────────────

        private static Unit CreateExchangeUnit(CommandUnitFormFixture f, bool withWeapons = true)
        {
            var land = f.AddUnitData("陸戦型", "換装=空戦型 宇宙型");
            var air = f.AddUnitData("空戦型", "換装=陸戦型 宇宙型", "ホバー移動");
            var space = f.AddUnitData("宇宙型", "換装=陸戦型 空戦型");
            air.Adaption = "SBCD";
            space.HP = 1200;
            space.EN = 150;
            space.Armor = 900;
            space.Mobility = 80;
            space.Adaption = "BCDS";
            if (withWeapons)
            {
                foreach (var ud in new[] { land, air, space })
                {
                    var w = ud.AddWeapon("ビームライフル");
                    w.Power = 1500;
                    w.MinRange = 1;
                    w.MaxRange = 4;
                    w.Class = "Ｂ";
                    w.Adaption = "AAAA";
                }
                var w2 = space.AddWeapon("メガ粒子砲");
                w2.Power = 2800;
                w2.MinRange = 2;
                w2.MaxRange = 6;
                w2.Class = "Ｂ";
                w2.Adaption = "AAAA";
            }
            var u = f.CreateUnit("陸戦型", "パイロット");
            // 換装はインターミッションのユニットステータスで使う
            f.MakeStatusViewUnitList();
            return u;
        }

        [TestMethod]
        public void ExchangeForm_ListsAvailableFormsAndTransformsToSelected()
        {
            // 換装=… の各形態を「変更先選択」リストに並べ、選択した形態に変形する
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateExchangeUnit(f);
            f.Src.Commands.SelectedUnit = u;
            f.ListBoxAnswer = _ => 2;

            Run(f, Command.ExchangeFormCmdID, "換装");

            var lb = f.ListBoxes.Single();
            Assert.AreEqual("変更先選択", lb.lb_caption);
            Assert.AreEqual(2, lb.Items.Count);
            StringAssert.StartsWith(lb.Items[0].Text, "空戦型");
            StringAssert.StartsWith(lb.Items[1].Text, "宇宙型");
            // 宇宙型の最大ＨＰ・最大攻撃力・最大射程が表示される
            StringAssert.Contains(lb.Items[1].Text, "1200");
            StringAssert.Contains(lb.Items[1].Text, "2800");
            Assert.IsTrue(lb.Items[1].Text.TrimEnd().EndsWith("6"));
            // 特殊能力一覧はコメントとして表示される
            StringAssert.Contains(lb.Items[0].ListItemComment, "ホバー移動");

            Assert.AreEqual("宇宙型", u.CurrentForm().Name);
            Assert.AreSame(u.CurrentForm(), f.Gui.CallsOf("DisplayUnitStatus").Last().Args[0]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void ExchangeForm_Cancel_KeepsForm()
        {
            // リストをキャンセルすると換装しない
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateExchangeUnit(f);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.ExchangeFormCmdID, "換装");

            Assert.AreEqual("陸戦型", u.CurrentForm().Name);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void ExchangeForm_ListBoxIsClosedAfterSelection()
        {
            // 元の SRC は ListBox(…, "カーソル移動,コメント") で選択時にリストを閉じる。
            // 「連続表示」で開いた場合は呼び出し側で閉じる必要がある
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateExchangeUnit(f);
            f.Src.Commands.SelectedUnit = u;
            f.ListBoxAnswer = _ => 1;

            Run(f, Command.ExchangeFormCmdID, "換装");

            var lb = f.ListBoxes.Single();
            var listBoxIndex = f.Gui.Calls.ToList().FindIndex(c => c.Member == "ListBox");
            var closedAfter = f.Gui.Calls.Skip(listBoxIndex + 1).Any(c => c.Member == "CloseListBox");
            Assert.IsTrue(!lb.lb_mode.Contains("連続表示") || closedAfter);
        }

        [TestMethod]
        public void ExchangeForm_ListItemContainsAdaption()
        {
            // 元の SRC は運動性の後に " " & .Data.Adaption を出力する（見出しの「適応」列）
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateExchangeUnit(f);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.ExchangeFormCmdID, "換装");

            var lb = f.ListBoxes.Single();
            StringAssert.Contains(lb.Items[0].Text, " SBCD");
            StringAssert.Contains(lb.Items[1].Text, " BCDS");
        }

        [TestMethod]
        public void ExchangeForm_FormsWithoutWeapons_ShowsZeroPowerAndRange()
        {
            // 元の SRC は max_value = 0 から求めるので、武器がなければ 0 と表示して換装できる
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateExchangeUnit(f, withWeapons: false);
            f.Src.Commands.SelectedUnit = u;
            f.ListBoxAnswer = _ => 1;

            Run(f, Command.ExchangeFormCmdID, "換装");

            Assert.AreEqual(2, f.ListBoxes.Single().Items.Count);
            Assert.AreEqual("空戦型", u.CurrentForm().Name);
        }
    }
}
