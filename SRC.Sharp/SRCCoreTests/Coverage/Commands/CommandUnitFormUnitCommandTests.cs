using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Events;
using SRCCore.Models;
using System.Linq;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.unitcommand.cs（UnitCommand の地形移動系・ユーザー定義コマンド・召喚解除・待機）と
    /// Command.unitxxx.cs（チャージ・会話・命令）のテスト。
    /// 元の SRC の Command.bas UnitCommand / WaitCommand / ChargeCommand / StartTalkCommand /
    /// StartOrderCommand / FinishOrderCommand に対応する。
    /// </summary>
    [TestClass]
    public class CommandUnitFormUnitCommandTests
    {
        private static void Run(CommandUnitFormFixture f, int cmdId, string label)
        {
            f.Src.Commands.UnitCommand(new UiCommand(cmdId, label));
        }

        // ──────────────────────────────────────────────
        // 地上・空中・地中・水中
        // ──────────────────────────────────────────────

        [TestMethod]
        [DataRow("陸", "地上")]
        [DataRow("水", "水上")]
        [DataRow("深水", "水上")]
        public void Ground_SetsAreaByTerrainClass(string terrainClass, string expectedArea)
        {
            // 「地上」コマンドは水・深水地形なら水上、それ以外なら地上に移る
            var f = new CommandUnitFormFixture(onMap: true);
            var ud = f.AddUnitData("機体");
            ud.Transportation = "陸空水";
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.SetTerrain(3, 3, new TerrainData { ID = 1, Name = "地形", Class = terrainClass, MoveCost = 2 });
            u.Area = "空中";
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.GroundCmdID, "地上");

            Assert.AreEqual(expectedArea, u.Area);
            Assert.AreSame(u, f.Gui.CallsOf("PaintUnitBitmap").Last().Args[0]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        [DataRow("陸", "空中")]
        [DataRow("月面", "宇宙")]
        public void Sky_SetsAreaByTerrainClass(string terrainClass, string expectedArea)
        {
            // 「空中」コマンドは月面なら宇宙、それ以外なら空中に移る
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体").Transportation = "陸空";
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.SetTerrain(3, 3, new TerrainData { ID = 1, Name = "地形", Class = terrainClass, MoveCost = 2 });
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.SkyCmdID, "空中");

            Assert.AreEqual(expectedArea, u.Area);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void UndergroundAndWater_SetFixedArea()
        {
            // 「地中」「水中」コマンドは地形に関係なくそのエリアに移る
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体").Transportation = "陸水";
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.UndergroundCmdID, "地中");
            Assert.AreEqual("地中", u.Area);

            Run(f, Command.WaterCmdID, "水中");
            Assert.AreEqual("水中", u.Area);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 待機
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wait_ConsumesActionAndClearsTarget()
        {
            // 待機は行動数を消費し、ターゲットを解除してユニット選択に戻る
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            var other = f.CreateUnit("機体", "相手", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedTarget = other;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.WaitCmdID, "待機");

            Assert.AreEqual(1, u.UsedAction);
            Assert.AreEqual(0, u.Action);
            Assert.IsNull(f.Src.Commands.SelectedTarget);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Wait_UnitWithoutPilot_DoesNotConsumeAction()
        {
            // パイロットが乗っていないユニットは行動数を消費せずステータス表示を消すだけ
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("無人機");
            var u = f.Src.UList.Add("無人機", 0, "味方");
            u.StandBy(2, 2);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.WaitCmdID, "待機");

            Assert.AreEqual(0, u.UsedAction);
            Assert.IsTrue(f.CountCalls("ClearUnitStatus") > 0);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Wait_RunsContactEnterAndActionEndEvents()
        {
            // 待機時に隣接ユニットとの接触イベント、進入イベント、行動終了イベントがこの順で発生する
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "接触 パイロット 相手:",
                "Set 記録 (記録 & \"接触\")",
                "Exit",
                "進入 パイロット 3 3:",
                "Set 記録 (記録 & \"進入\")",
                "Exit",
                "行動終了 パイロット:",
                "Set 記録 (記録 & \"終了\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.CreateUnit("機体", "相手", x: 4, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.WaitCmdID, "待機");

            Assert.AreEqual("接触進入終了", f.Record);
            Assert.AreEqual(1, u.UsedAction);
        }

        // ──────────────────────────────────────────────
        // ユーザー定義のユニットコマンド
        // ──────────────────────────────────────────────

        private static UiCommand FindUnitCommand(CommandUnitFormFixture f, string name)
        {
            var lab = f.Src.Event.colEventLabelList.Values
                .First(x => x.Name == LabelType.UnitCommandEventLabel && x.Para(2) == name);
            return new UiCommand(Command.UnitCommandCmdID, name, lab);
        }

        [TestMethod]
        public void UserUnitCommand_RunsUseEventCommandAndAfterUseEvent_WithoutConsumingAction()
        {
            // ユニットコマンドは「使用」→ コマンド本体 →「使用後」の順にイベントを実行し、
            // 行動数が変化しなければ行動終了にならない
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "使用 パイロット 必殺:",
                "Set 記録 (記録 & \"使用\")",
                "Exit",
                "ユニットコマンド 必殺 パイロット:",
                "Set 記録 (記録 & \"本体\")",
                "Exit",
                "使用後 パイロット 必殺:",
                "Set 記録 (記録 & \"使用後\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            f.Src.Commands.UnitCommand(FindUnitCommand(f, "必殺"));

            Assert.AreEqual("使用本体使用後", f.Record);
            Assert.AreEqual(0, u.UsedAction);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void UserUnitCommand_StackedConditionalLabels_ShownOnceAndRunFromSatisfiedLabel()
        {
            // 同名の条件付きユニットコマンドのラベルを重ねて1つの本体を共有する場合（#1331）
            // VB: Command.bas ProceedCommand は条件を満たしたラベルだけを表示し、
            // 選択時は UnitCommandLabelList に控えたそのラベルの行からイベントを実行する
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "ユニットコマンド 回復 全 (段階 = 1):",
                "ユニットコマンド 回復 全 (段階 = 2):",
                "ユニットコマンド 回復 全 (段階 = 3):",
                "Set 記録 (記録 & \"本体\")",
                "Exit");
            f.Src.Expression.DefineGlobalVariable("段階");
            f.Src.Expression.SetVariableAsLong("段階", 2);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.CommandState = "ユニット選択";

            f.Src.Commands.ProceedInput(GuiButton.Left, f.Src.Map.MapData[3, 3], u);

            var menu = (System.Collections.Generic.IList<UiCommand>)f.Gui.CallsOf("ShowUnitCommandMenu").Last().Args[0];
            var commands = menu.Where(x => x.Id == Command.UnitCommandCmdID).ToList();
            Assert.AreEqual(1, commands.Count);
            Assert.AreEqual("回復", commands[0].Label);
            Assert.AreEqual("ユニットコマンド 回復 全 (段階 = 2)", commands[0].LabelData.Data);

            f.Src.Commands.UnitCommand(commands[0]);

            Assert.AreEqual("本体", f.Record);
        }

        [TestMethod]
        public void UserUnitCommand_AfterMove_EndsAction()
        {
            // 移動後に使ったユニットコマンドは行動数が変化しなくても待機扱いで行動を終える
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "ユニットコマンド 必殺 パイロット:",
                "Set 記録 (記録 & \"本体\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "移動後コマンド選択";

            f.Src.Commands.UnitCommand(FindUnitCommand(f, "必殺"));

            Assert.AreEqual("本体", f.Record);
            Assert.AreEqual(1, u.UsedAction);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void UserUnitCommand_CancelInUseEvent_EndsActionWithoutRunningBody()
        {
            // 使用イベントで Cancel されるとコマンド本体は実行せずに待機（行動終了）する
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "使用 パイロット 必殺:",
                "Cancel",
                "Exit",
                "ユニットコマンド 必殺 パイロット:",
                "Set 記録 (記録 & \"本体\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            f.Src.Commands.UnitCommand(FindUnitCommand(f, "必殺"));

            Assert.AreEqual("", f.Record);
            Assert.AreEqual(1, u.UsedAction);
            Assert.IsFalse(f.Src.IsCanceled);
        }

        [TestMethod]
        public void UserUnitCommand_CancelInBody_CancelsCommand()
        {
            // コマンド本体で Cancel された場合はコマンドの取り消し扱い（行動数は消費しない）
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "ユニットコマンド 必殺 パイロット:",
                "Set 記録 (記録 & \"本体\")",
                "Cancel",
                "Exit",
                "使用後 パイロット 必殺:",
                "Set 記録 (記録 & \"使用後\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            f.Src.Commands.UnitCommand(FindUnitCommand(f, "必殺"));

            Assert.AreEqual("本体", f.Record);
            Assert.AreEqual(0, u.UsedAction);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
            Assert.IsFalse(f.Src.IsCanceled);
        }

        // ──────────────────────────────────────────────
        // 召喚解除
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dismiss_ReleasesServants()
        {
            // 召喚解除は召喚したユニットを撤退させて破棄する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("召喚師");
            f.AddUnitData("召喚獣", "召喚ユニット");
            var master = f.CreateUnit("召喚師", "パイロット", x: 2, y: 2);
            var servant = f.CreateUnit("召喚獣", "召喚獣パイロット", x: 3, y: 2);
            master.AddServant(servant);
            servant.Summoner = master;
            f.Src.Commands.SelectedUnit = master;

            Run(f, Command.DismissCmdID, "召喚解除");

            Assert.AreEqual("破棄", servant.Status);
            Assert.IsNull(f.Src.Map.MapDataForUnit[3, 2]);
            Assert.AreEqual(0, master.CountServant());
            // 召喚解除自体は行動を消費しない
            Assert.AreEqual(0, master.UsedAction);
        }

        // ──────────────────────────────────────────────
        // チャージ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Charge_ConfirmOk_AddsChargeConditionAndEndsAction()
        {
            // チャージを開始するとユニットがチャージ状態になり、行動を終える
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.ChargeCmdID, "チャージ");

            Assert.AreEqual(("チャージを開始しますか？", "チャージ開始"), f.Confirms.Single());
            Assert.IsTrue(u.IsConditionSatisfied("チャージ"));
            Assert.AreEqual(1, u.ConditionLifetime("チャージ"));
            // チャージ中は行動できない
            Assert.AreEqual(0, u.Action);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Charge_CanceledByUseEvent_DoesNotCharge()
        {
            // 使用イベントで Cancel されるとチャージしない
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "使用 パイロット チャージ:",
                "Cancel",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.ChargeCmdID, "チャージ");

            Assert.IsFalse(u.IsConditionSatisfied("チャージ"));
            Assert.AreEqual(0, u.UsedAction);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
            Assert.IsFalse(f.Src.IsCanceled);
        }

        // ──────────────────────────────────────────────
        // 会話
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Talk_OnlyAdjacentUnitsWithTalkEventAreSelectable()
        {
            // 隣接ユニットのうち「会話 自分 相手」イベントが定義されている相手だけを選択可能にする
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "会話 パイロット 相手甲:",
                "Set 記録 (記録 & \"会話\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.CreateUnit("機体", "相手甲", x: 4, y: 3);
            f.CreateUnit("機体", "相手乙", x: 3, y: 4);
            f.CreateUnit("機体", "相手丙", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.TalkCmdID, "会話");

            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[4, 3], "会話イベントのある隣接ユニットは選択可能");
            Assert.IsTrue(mask[3, 4], "会話イベントのない隣接ユニットは選択不可");
            Assert.IsTrue(mask[6, 6], "隣接していないユニットは選択不可");
            Assert.IsFalse(mask[3, 3]);
            Assert.AreEqual("会話", f.Src.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Talk_TalkEventOnFirstLineOfScenario_IsSelectable()
        {
            // 元の SRC の IsEventDefined は 1 始まりの EventData を SearchLabel(lname, 1) から探すので
            // シナリオの先頭行のラベルも見つかる
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEventsWithoutLeadingLabel(
                "会話 パイロット 相手甲:",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.CreateUnit("機体", "相手甲", x: 4, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.TalkCmdID, "会話");

            Assert.IsFalse(f.Src.Map.MaskData[4, 3]);
        }

        [TestMethod]
        public void Talk_AfterMove_StateIsAfterMoveTargetSelect()
        {
            // 移動後に会話を選んだ場合は「移動後ターゲット選択」になる
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "移動後コマンド選択";

            Run(f, Command.TalkCmdID, "会話");

            Assert.AreEqual("移動後ターゲット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Talk_FinishRunsTalkEventAndEndsAction()
        {
            // 会話相手を選ぶと会話イベントを実行して行動を終える
            using var f = new CommandUnitFormFixture(onMap: true);
            f.LoadEvents(
                "会話 パイロット 相手甲:",
                "Set 記録 (記録 & \"会話\")",
                "Exit");
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            var t = f.CreateUnit("機体", "相手甲", x: 4, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            Run(f, Command.TalkCmdID, "会話");

            f.Src.Commands.ProceedCommand(cell: f.Src.Map.MapData[4, 3], unit: t);

            Assert.AreEqual("会話", f.Record);
            Assert.AreEqual(1, u.UsedAction);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 命令
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Order_Move_SelectsAnyPointAndSetsModeToCoordinates()
        {
            // 「移動」命令は全マスを選択可能にし、選んだ座標を "X Y" として行動モードに設定する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 2;

            Run(f, Command.OrderCmdID, "命令");

            var lb = f.ListBoxes.Single();
            Assert.AreEqual("命令", lb.lb_caption);
            Assert.AreEqual("移動命令", f.Src.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Src.Commands.CommandState);
            for (var x = 1; x <= f.Src.Map.MapWidth; x++)
            {
                for (var y = 1; y <= f.Src.Map.MapHeight; y++)
                {
                    Assert.IsFalse(f.Src.Map.MaskData[x, y]);
                }
            }

            f.Src.Commands.ProceedCommand(cell: f.Src.Map.MapData[7, 2]);

            Assert.AreEqual("7 2", u.Mode);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Order_Attack_OnlyEnemiesSelectableAndSetsModeToTargetPilot()
        {
            // 「攻撃」命令は敵ユニットのみ選択可能にし、選んだユニットのパイロットIDを行動モードにする
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            var ally = f.CreateUnit("機体", "味方パイロット", x: 5, y: 3);
            var enemy = f.CreateUnit("機体", "敵パイロット", "敵", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 3;

            Run(f, Command.OrderCmdID, "命令");

            Assert.AreEqual("攻撃命令", f.Src.Commands.SelectedCommand);
            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[6, 6]);
            Assert.IsTrue(mask[5, 3]);
            Assert.IsTrue(mask[3, 3]);
            Assert.IsTrue(mask[1, 1]);

            f.Src.Commands.ProceedCommand(cell: f.Src.Map.MapData[6, 6], unit: enemy);

            Assert.AreEqual(enemy.MainPilot().ID, u.Mode);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Order_Escort_OnlyAlliesSelectableExceptSelf()
        {
            // 「護衛」命令は味方ユニット（自分以外）を選択可能にし、選んだユニットのパイロットIDを行動モードにする
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            var ally = f.CreateUnit("機体", "味方パイロット", x: 5, y: 3);
            f.CreateUnit("機体", "敵パイロット", "敵", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 4;

            Run(f, Command.OrderCmdID, "命令");

            Assert.AreEqual("護衛命令", f.Src.Commands.SelectedCommand);
            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[5, 3]);
            Assert.IsTrue(mask[3, 3]);
            Assert.IsTrue(mask[6, 6]);

            f.Src.Commands.ProceedCommand(cell: f.Src.Map.MapData[5, 3], unit: ally);

            Assert.AreEqual(ally.MainPilot().ID, u.Mode);
        }

        [TestMethod]
        public void Order_Free_WithoutMasterOrSummoner_SetsModeToNormal()
        {
            // 主人も召喚主もいなければ「帰還」は選択肢に出ない。「自由」を選ぶと行動モードを「通常」に戻す
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            u.Mode = "固定";
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 1;

            Run(f, Command.OrderCmdID, "命令");

            var lb = f.ListBoxes.Single();
            Assert.AreEqual("命令", lb.lb_caption);
            Assert.AreEqual(4, lb.Items.Count);
            Assert.IsFalse(lb.Items.Any(x => x.Text.StartsWith("帰還")));
            Assert.AreEqual("通常", u.Mode);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Order_Cancel_KeepsMode()
        {
            // 命令リストをキャンセルすれば行動モードは変わらない
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            u.Mode = "固定";
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.OrderCmdID, "命令");

            Assert.AreEqual("固定", u.Mode);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Order_ReturnToMaster_SetsModeToMasterPilot()
        {
            // 主人がいるユニットの「帰還：主人の所に戻る」は主人のパイロットIDを行動モードにする
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var master = f.CreateUnit("機体", "主人", x: 1, y: 1);
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            u.Master = master;
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 5;

            Run(f, Command.OrderCmdID, "命令");

            Assert.AreEqual("帰還：主人の所に戻る", f.ListBoxes.Single().Items[4].Text);
            Assert.AreEqual(master.MainPilot().ID, u.Mode);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Order_ReturnToSummoner_SetsModeToSummonerPilot()
        {
            // 召喚ユニットの「帰還：召喚主の所に戻る」は召喚主のパイロットIDを行動モードにする
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体");
            var summoner = f.CreateUnit("機体", "召喚主", x: 1, y: 1);
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            u.Summoner = summoner;
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 5;

            Run(f, Command.OrderCmdID, "命令");

            Assert.AreEqual("帰還：召喚主の所に戻る", f.ListBoxes.Single().Items[4].Text);
            Assert.AreEqual(summoner.MainPilot().ID, u.Mode);
        }
    }
}
