using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using System.Linq;
using static SRCCore.Coverage.Tests.CommandsCoverage.CommandAttackProcessFixture;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.process.cs のメニュー構築（ProceedCommand の「ユニット選択」「コマンド選択」
    /// 「移動後コマンド選択」）のテスト。
    /// VB 版 Command.bas の ProceedCommand でメニュー項目の Visible を切り替えている部分に対応する。
    /// </summary>
    [TestClass]
    public class CommandAttackProcessMenuTests
    {
        private static CommandAttackProcessFixture CreateWithAlly(out Units.Unit ally, int speed = 4, params string[] features)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機", speed, features);
            f.AddWeapon(ud, "ビームサーベル");
            ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            return f;
        }

        // ══════════════════════════════════════════════════════
        // マップコマンド（ユニットのいない地点を選択）
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void UnitSelect_EmptyCell_ShowsMapCommandMenuAndEntersMapCommandState()
        {
            // ユニットのいない地点ではマップコマンドメニューを表示し「マップコマンド」状態になる
            var f = new CommandAttackProcessFixture();
            f.Commands.CommandState = "ユニット選択";

            // ユニットのいない地点での右クリックはマップコマンド（VB: Main.frm picMain_MouseDown）
            f.Commands.ProceedInput(GuiButton.Right, f.SRC.Map.MapData[3, 3], null);

            Assert.AreEqual("マップコマンド", f.Commands.CommandState);
            var menu = f.LastMapCommandMenu();
            Assert.IsNotNull(menu);
            Assert.AreEqual("ターン終了", Label(menu, Command.EndTurnCmdID));
            // クイックセーブ不可でもデバッグでもなければ中断とクイックセーブが使える
            CollectionAssert.IsSubsetOf(
                new[] { Command.DumpCmdID, Command.UnitListCmdID, Command.GlobalMapCmdID, Command.AutoDefenseCmdID, Command.ConfigurationCmdID, Command.QuickSaveCmdID },
                Ids(menu));
            // 勝利条件イベントやリスタート・クイックロード用データが無ければ表示しない
            CollectionAssert.DoesNotContain(Ids(menu), Command.OperationObjectCmdID);
            CollectionAssert.DoesNotContain(Ids(menu), Command.RestartCmdID);
            CollectionAssert.DoesNotContain(Ids(menu), Command.QuickLoadCmdID);
            // 味方にスペシャルパワーを持つパイロットがいなければ検索は表示しない
            CollectionAssert.DoesNotContain(Ids(menu), Command.SearchSpecialPowerCmdID);
            Assert.IsFalse(f.SRC.GUI.IsGUILocked);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_QuickSaveDisabledOption_HidesDumpAndQuickSave()
        {
            // Option「クイックセーブ不可」では中断・クイックセーブを表示しない
            var f = new CommandAttackProcessFixture();
            f.SRC.Expression.DefineGlobalVariable("Option(クイックセーブ不可)");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            var ids = Ids(f.LastMapCommandMenu());
            CollectionAssert.DoesNotContain(ids, Command.DumpCmdID);
            CollectionAssert.DoesNotContain(ids, Command.QuickSaveCmdID);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_DebugOptionOverridesQuickSaveDisabled()
        {
            // Option「デバッグ」があれば「クイックセーブ不可」でも中断・クイックセーブを表示する
            var f = new CommandAttackProcessFixture();
            f.SRC.Expression.DefineGlobalVariable("Option(クイックセーブ不可)");
            f.SRC.Expression.DefineGlobalVariable("Option(デバッグ)");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            var ids = Ids(f.LastMapCommandMenu());
            CollectionAssert.Contains(ids, Command.DumpCmdID);
            CollectionAssert.Contains(ids, Command.QuickSaveCmdID);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_RestartAndQuickLoadShownWhenDataAvailable()
        {
            // リスタート・クイックロード用のデータがあれば表示する
            var f = new CommandAttackProcessFixture();
            f.SRC.IsRestartSaveDataAvailable = true;
            f.SRC.IsQuickSaveDataAvailable = true;
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            var ids = Ids(f.LastMapCommandMenu());
            CollectionAssert.Contains(ids, Command.RestartCmdID);
            CollectionAssert.Contains(ids, Command.QuickLoadCmdID);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_VictoryConditionEventShowsOperationObject()
        {
            // 「勝利条件」イベントが定義されていれば作戦目的コマンドを表示する
            var f = new CommandAttackProcessFixture();
            f.AddEventLabel("勝利条件", 1);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            Assert.AreEqual("勝利条件", Label(f.LastMapCommandMenu(), Command.OperationObjectCmdID));
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_MapCommandEventLabels_AddedByCondition()
        {
            // マップコマンドイベント: 条件省略なら常に、条件式が 0 以外なら表示し、0 なら表示しない
            var f = new CommandAttackProcessFixture();
            f.AddEventLabel("マップコマンド 作戦会議:", 1);
            f.AddEventLabel("マップコマンド 秘密の作戦 0:", 2);
            f.AddEventLabel("マップコマンド 撤退 1:", 3);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            var labels = f.LastMapCommandMenu().Where(x => x.Id == Command.MapCommandCmdID).Select(x => x.Label).ToList();
            CollectionAssert.AreEqual(new[] { "作戦会議", "撤退" }, labels);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_DisabledMapCommandLabel_NotShown()
        {
            // ClearEvent 等で無効化されたラベルは表示しない
            var f = new CommandAttackProcessFixture();
            f.AddEventLabel("マップコマンド 作戦会議:", 1);
            f.SRC.Event.colEventLabelList.List.First().Enable = false;
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            CollectionAssert.DoesNotContain(Ids(f.LastMapCommandMenu()), Command.MapCommandCmdID);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_StatusView_ShowsOnlyCommonCommands()
        {
            // ステータス表示用の空マップ（MapFileName なし）ではターン終了などの通常ステージ用コマンドを出さない
            var f = new CommandAttackProcessFixture(mapFileName: "");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            var ids = Ids(f.LastMapCommandMenu());
            CollectionAssert.DoesNotContain(ids, Command.EndTurnCmdID);
            CollectionAssert.DoesNotContain(ids, Command.DumpCmdID);
            CollectionAssert.DoesNotContain(ids, Command.GlobalMapCmdID);
        }

        [TestMethod]
        public void UnitSelect_EmptyCell_AllyPilotWithSpecialPower_ShowsSearchCommand()
        {
            // 味方パイロットにスペシャルパワー持ちがいれば「スペシャルパワー検索」を表示する
            var f = new CommandAttackProcessFixture();
            f.SRC.SPDList.Add("集中");
            var pd = f.AddPilotData("SP持ち");
            pd.SP = 50;
            pd.AddSpecialPower("集中", 1, 10);
            f.SRC.PList.Add("SP持ち", 1, "味方");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            Assert.AreEqual("スペシャルパワー検索", Label(f.LastMapCommandMenu(), Command.SearchSpecialPowerCmdID));
        }

        // ══════════════════════════════════════════════════════
        // ユニット選択 → コマンド選択
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void UnitSelect_AllyUnit_ShowsUnitCommandMenuWithMoveAndAttack()
        {
            // 味方ユニットを選ぶと「コマンド選択」に進み、移動・攻撃・武装一覧・待機以外の移動前コマンドを表示する
            var f = CreateWithAlly(out var ally);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            Assert.AreEqual("コマンド選択", f.Commands.CommandState);
            Assert.AreSame(ally, f.Commands.SelectedUnit);
            Assert.AreSame(ally, f.SRC.Event.SelectedUnitForEvent);
            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("移動", Label(menu, Command.MoveCmdID));
            Assert.AreEqual("武装一覧", Label(menu, Command.WeaponListCmdID));
            // 射程内に敵がいなければ「攻撃」は表示しない
            CollectionAssert.DoesNotContain(Ids(menu), Command.AttackCmdID);
            // コマンドは ID 順に並ぶ
            CollectionAssert.AreEqual(Ids(menu).OrderBy(x => x).ToList(), Ids(menu));
            Assert.IsFalse(f.SRC.GUI.IsGUILocked);
        }

        [TestMethod]
        public void CommandSelect_AdjacentEnemy_ShowsAttack()
        {
            // 射程内に敵がいれば「攻撃」を表示する
            var f = CreateWithAlly(out var ally);
            var eud = f.AddUnitData("敵機");
            f.AddWeapon(eud, "敵の武器");
            f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            Assert.AreEqual("攻撃", Label(f.LastUnitCommandMenu(), Command.AttackCmdID));
        }

        [TestMethod]
        public void CommandSelect_AttackDisabledCondition_HidesAttack()
        {
            // 攻撃不能状態では「攻撃」を表示しない
            var f = CreateWithAlly(out var ally);
            f.AddUnitData("敵機");
            f.Deploy("敵機", "敵兵", "敵", 6, 5);
            // 同じ配置で攻撃不能でなければ「攻撃」が表示されることを先に確認する
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);
            CollectionAssert.Contains(Ids(f.LastUnitCommandMenu()), Command.AttackCmdID);

            ally.AddCondition("攻撃不能", 1, cdata: "");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.AttackCmdID);
        }

        [TestMethod]
        public void CommandSelect_SpeedZero_ShowsWaitInsteadOfMove()
        {
            // 移動力 0 のユニットは「移動」の代わりに「待機」を表示する
            var f = CreateWithAlly(out var ally, speed: 0);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var ids = Ids(f.LastUnitCommandMenu());
            CollectionAssert.Contains(ids, Command.WaitCmdID);
            CollectionAssert.DoesNotContain(ids, Command.MoveCmdID);
        }

        [TestMethod]
        public void CommandSelect_Teleport_ShowsTeleportWhenEnoughEN()
        {
            // テレポート能力: EN がコスト以上なら名称付きで表示する（データ "名称 ENコスト"）
            var f = CreateWithAlly(out var ally, 4, "テレポート=瞬間移動 30");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("瞬間移動", Label(menu, Command.TeleportCmdID));
            // EN コストが 0 でなく移動力もあるので通常移動も残る
            CollectionAssert.Contains(Ids(menu), Command.MoveCmdID);
        }

        [TestMethod]
        public void CommandSelect_Teleport_NotEnoughEN_HidesTeleport()
        {
            // EN がテレポートのコストに満たなければ表示しない
            var f = CreateWithAlly(out var ally, 4, "テレポート=瞬間移動 30");
            ally.EN = 10;
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.TeleportCmdID);
        }

        [TestMethod]
        public void CommandSelect_TeleportWithZeroCost_ReplacesNormalMove()
        {
            // EN コスト 0 のテレポートは通常移動がテレポートになるため「移動」を表示しない
            var f = CreateWithAlly(out var ally, 4, "テレポート=瞬間移動 0");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var ids = Ids(f.LastUnitCommandMenu());
            CollectionAssert.Contains(ids, Command.TeleportCmdID);
            CollectionAssert.DoesNotContain(ids, Command.MoveCmdID);
        }

        [TestMethod]
        public void CommandSelect_JumpWithoutCost_ReplacesNormalMove()
        {
            // コスト指定の無いジャンプは常に使え、通常移動がジャンプになる
            var f = CreateWithAlly(out var ally, 4, "ジャンプ");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("ジャンプ", Label(menu, Command.JumpCmdID));
            CollectionAssert.DoesNotContain(Ids(menu), Command.MoveCmdID);
        }

        [TestMethod]
        public void CommandSelect_CannotMoveCondition_HidesTeleportAndJump()
        {
            // 移動不能状態ではテレポート・ジャンプを表示しない（VB: Command.bas 873, 907）
            var f = CreateWithAlly(out var ally, 4, "テレポート=瞬間移動 30", "ジャンプ=跳躍 10");
            ally.AddCondition("移動不能", 1, cdata: "");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var ids = Ids(f.LastUnitCommandMenu());
            CollectionAssert.DoesNotContain(ids, Command.TeleportCmdID);
            CollectionAssert.DoesNotContain(ids, Command.JumpCmdID);
            // 移動不能では移動力が 0 になる（VB: Unit.cls Speed）ため「移動」ではなく「待機」を表示する
            CollectionAssert.DoesNotContain(ids, Command.MoveCmdID);
            CollectionAssert.Contains(ids, Command.WaitCmdID);
        }

        [TestMethod]
        public void CommandSelect_TeleportWithoutCost_RequiresDefaultEN40()
        {
            // コスト指定の無いテレポートは EN 40 で使える（VB: Command.bas 866-869）
            var f = CreateWithAlly(out var ally, 4, "テレポート");
            ally.EN = 39;
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);
            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.TeleportCmdID);

            ally.EN = 40;
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("テレポート", Label(menu, Command.TeleportCmdID));
            // EN コストがあるので通常移動も残る
            CollectionAssert.Contains(Ids(menu), Command.MoveCmdID);
        }

        [TestMethod]
        public void CommandSelect_CarrierWithUnitOnBoard_ShowsLaunch_HiddenUnderground()
        {
            // 格納ユニットのいる母艦は「発進」を表示するが、地中にいる場合は表示しない
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("母艦", 4, "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy("母艦", "艦長", "味方", 5, 5);
            var fighter = f.Deploy("艦載機", "艦載機乗り", "味方", 1, 1);
            fighter.Escape();
            carrier.LoadUnit(fighter);
            fighter.Status = "格納";
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], carrier);
            Assert.AreEqual("発進", Label(f.LastUnitCommandMenu(), Command.LaunchCmdID));

            carrier.Area = "地中";
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], carrier);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.LaunchCmdID);
        }

        [TestMethod]
        public void CommandSelect_TalkEventWithAdjacentUnit_ShowsTalk()
        {
            // 隣接ユニットとの「会話」イベントが定義されていれば会話コマンドを表示する
            var f = CreateWithAlly(out var ally);
            var oud = f.AddUnitData("僚機");
            var other = f.Deploy("僚機", "相棒", "味方", 5, 6);
            f.AddEventLabel("会話 主人公 相棒", 1);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            Assert.AreEqual("会話", Label(f.LastUnitCommandMenu(), Command.TalkCmdID));
        }

        [TestMethod]
        public void CommandSelect_TalkEventWithNonAdjacentUnit_HidesTalk()
        {
            // 会話相手が隣接していなければ表示しない
            var f = CreateWithAlly(out var ally);
            f.AddUnitData("僚機");
            f.Deploy("僚機", "相棒", "味方", 5, 8);
            f.AddEventLabel("会話 主人公 相棒", 1);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.TalkCmdID);
        }

        [TestMethod]
        public void CommandSelect_SkyTransport_ShowsSkyCommandOnLand()
        {
            // 空を移動できるユニットが陸地にいれば「空中」コマンドを表示する
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("飛行機");
            ud.Transportation = "陸空";
            var u = f.Deploy("飛行機", "パイロット", "味方", 5, 5);
            u.Area = "地上";
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("空中", Label(menu, Command.SkyCmdID));
            // すでに地上にいるので「地上」は表示しない
            CollectionAssert.DoesNotContain(Ids(menu), Command.GroundCmdID);
        }

        [TestMethod]
        public void CommandSelect_FlyingUnitOverLand_ShowsGroundCommand()
        {
            // 空中にいる陸移動可能ユニットが陸地上にいれば「地上」コマンドを表示する
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("飛行機");
            ud.Transportation = "陸空";
            var u = f.Deploy("飛行機", "パイロット", "味方", 5, 5);
            u.Area = "空中";
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("地上", Label(menu, Command.GroundCmdID));
            CollectionAssert.DoesNotContain(Ids(menu), Command.SkyCmdID);
        }

        [TestMethod]
        public void CommandSelect_OnWater_ShowsUnderwaterCommand()
        {
            // 水地形で水中適応が "-" でなければ「水中」コマンドを表示する
            var f = new CommandAttackProcessFixture();
            f.SetTerrain(5, 5, Water());
            var ud = f.AddUnitData("水陸両用");
            ud.Transportation = "陸水";
            var u = f.Deploy("水陸両用", "パイロット", "味方", 5, 5);
            u.Area = "水上";
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            Assert.AreEqual("水中", Label(f.LastUnitCommandMenu(), Command.WaterCmdID));
        }

        [TestMethod]
        public void CommandSelect_UnitCommandEventLabels_FilteredByTargetAndCondition()
        {
            // ユニットコマンドイベント: 対象（パイロット名・陣営・全）と条件式で表示を判定する
            var f = CreateWithAlly(out var ally);
            f.AddEventLabel("ユニットコマンド 名指し 主人公:", 1);
            f.AddEventLabel("ユニットコマンド 陣営指定 味方:", 2);
            f.AddEventLabel("ユニットコマンド 全員 全:", 3);
            f.AddEventLabel("ユニットコマンド 他人用 別人:", 4);
            f.AddEventLabel("ユニットコマンド 条件偽 全 0:", 5);
            f.AddEventLabel("ユニットコマンド 条件真 全 1:", 6);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var labels = f.LastUnitCommandMenu().Where(x => x.Id == Command.UnitCommandCmdID).Select(x => x.Label).ToList();
            CollectionAssert.AreEqual(new[] { "名指し", "陣営指定", "全員", "条件真" }, labels);
        }

        [TestMethod]
        public void CommandSelect_UnitCommandByPilotName_NotForEnemy()
        {
            // パイロット名での指定は味方ユニットにのみ有効（ヘルプ: 対象が味方の場合のみ使用可能）
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("敵機");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            f.AddEventLabel("ユニットコマンド 名指し 敵兵:", 1);
            f.AddEventLabel("ユニットコマンド 陣営指定 敵:", 2);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], enemy);

            var labels = f.LastUnitCommandMenu().Where(x => x.Id == Command.UnitCommandCmdID).Select(x => x.Label).ToList();
            CollectionAssert.AreEqual(new[] { "陣営指定" }, labels);
        }

        private static CommandAttackProcessFixture CreateSplitUnit(out Units.Unit u, int partAPilots, int partBPilots)
        {
            // 2 人乗りのユニットが 分離 で 2 機に分かれる
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("合体機", 4, "分離=分離 分離機Ａ 分離機Ｂ");
            ud.PilotNum = 2;
            f.AddUnitData("分離機Ａ").PilotNum = partAPilots;
            f.AddUnitData("分離機Ｂ").PilotNum = partBPilots;
            f.SRC.UList.Add("分離機Ａ", 0, "味方");
            f.SRC.UList.Add("分離機Ｂ", 0, "味方");
            u = f.Deploy("合体機", "メイン", "味方", 5, 5);
            f.AddPilotData("サブ");
            f.SRC.PList.Add("サブ", 1, "味方").Ride(u);
            f.Commands.CommandState = "ユニット選択";
            return f;
        }

        [TestMethod]
        public void CommandSelect_Split_EnoughPilots_ShowsSplit()
        {
            // 分離先のユニットが作成済みで、分離先に必要なパイロット数を満たしていれば分離コマンドを表示する
            var f = CreateSplitUnit(out var u, 1, 1);

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            Assert.AreEqual("分離", Label(f.LastUnitCommandMenu(), Command.SplitCmdID));
        }

        [TestMethod]
        [Ignore("移植の不具合: 分離に必要なパイロット数を分離先のユニット数で数えている（VB は PilotNum の合計）")]
        public void CommandSelect_Split_NotEnoughPilots_HidesSplit()
        {
            // VB: 分離先の PilotNum の合計が搭乗パイロット数を超える場合は分離できない（2 人乗りで 2+1 人必要）
            var f = CreateSplitUnit(out var u, 2, 1);

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.SplitCmdID);
        }

        // ══════════════════════════════════════════════════════
        // 味方以外・行動済みのユニット
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void CommandSelect_EnemyUnit_ShowsRangeCommandsOnly()
        {
            // 敵ユニットでは「移動範囲」「射程範囲」と一覧系コマンドのみ表示する
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("敵機");
            f.AddWeapon(ud, "敵の武器");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], enemy);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("移動範囲", Label(menu, Command.MoveCmdID));
            Assert.AreEqual("射程範囲", Label(menu, Command.AttackCmdID));
            CollectionAssert.DoesNotContain(Ids(menu), Command.WaitCmdID);
        }

        [TestMethod]
        public void CommandSelect_EnemyWithOnlyMapWeapon_HidesRangeCommand()
        {
            // マップ兵器しか持たない場合「射程範囲」は表示しない
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("敵機");
            f.AddWeapon(ud, "マップ兵器", wclass: "Ｍ全", maxRange: 3);
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], enemy);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.AttackCmdID);
        }

        [TestMethod]
        public void CommandSelect_ActedAllyCarrierWithUnitOnBoard_ShowsOnlyLaunch()
        {
            // 行動終了した母艦でも格納ユニットがいれば「発進」は使える
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("母艦", 4, "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy("母艦", "艦長", "味方", 5, 5);
            var fighter = f.Deploy("艦載機", "艦載機乗り", "味方", 1, 1);
            fighter.Escape();
            carrier.LoadUnit(fighter);
            fighter.Status = "格納";
            carrier.UseAction();
            Assert.AreEqual(0, carrier.Action);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], carrier);

            var ids = Ids(f.LastUnitCommandMenu());
            CollectionAssert.Contains(ids, Command.LaunchCmdID);
            CollectionAssert.DoesNotContain(ids, Command.MoveCmdID);
            CollectionAssert.DoesNotContain(ids, Command.WaitCmdID);
        }

        [TestMethod]
        [Ignore("移植の不具合: 行動終了後のユニットコマンドは「-*」「**」付きラベルのみ表示すべきだが全ラベルを表示する")]
        public void CommandSelect_ActedAlly_OnlyAfterActionUnitCommandsShown()
        {
            // VB: 行動終了している場合は AsterNum = 1 (-*) または 3 (**) のユニットコマンドのみ表示
            var f = CreateWithAlly(out var ally);
            f.AddEventLabel("ユニットコマンド 通常 全:", 1);
            f.AddEventLabel("-*ユニットコマンド 行動後 全:", 2);
            ally.UseAction();
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var labels = f.LastUnitCommandMenu().Where(x => x.Id == Command.UnitCommandCmdID).Select(x => x.Label).ToList();
            CollectionAssert.AreEqual(new[] { "行動後" }, labels);
        }

        [TestMethod]
        public void CommandSelect_SummonedNpcOfAlly_ShowsOrder()
        {
            // 味方が召喚した NPC には「命令」コマンドを使える
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("召喚主");
            f.AddUnitData("召喚獣", 4, "召喚ユニット");
            var summoner = f.Deploy("召喚主", "術師", "味方", 3, 3);
            var servant = f.Deploy("召喚獣", "使い魔", "ＮＰＣ", 5, 5);
            summoner.AddServant(servant);
            servant.Summoner = summoner;
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], servant);

            Assert.AreEqual("命令", Label(f.LastUnitCommandMenu(), Command.OrderCmdID));
        }

        [TestMethod]
        public void CommandSelect_CharmedNpcOfAlly_ShowsOrder()
        {
            // 味方が魅了した NPC には「命令」コマンドを使える
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("術者機");
            f.AddUnitData("被魅了機");
            var master = f.Deploy("術者機", "術師", "味方", 3, 3);
            var charmed = f.Deploy("被魅了機", "操られた者", "ＮＰＣ", 5, 5);
            charmed.AddCondition("魅了", 3, cdata: "");
            charmed.Master = master;
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], charmed);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("命令", Label(menu, Command.OrderCmdID));
            Assert.AreEqual(1, Ids(menu).Count(x => x == Command.OrderCmdID));
        }

        [TestMethod]
        public void CommandSelect_CharmedNpcOfEnemy_NoOrder()
        {
            // 魅了した側が味方でなければ「命令」は使えない
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("術者機");
            f.AddUnitData("被魅了機");
            var master = f.Deploy("術者機", "術師", "敵", 3, 3);
            var charmed = f.Deploy("被魅了機", "操られた者", "ＮＰＣ", 5, 5);
            charmed.AddCondition("魅了", 3, cdata: "");
            charmed.Master = master;
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], charmed);

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.OrderCmdID);
        }

        [TestMethod]
        public void CommandSelect_HiddenInfoOptionButIdentifiedEnemy_ShowsRangeCommands()
        {
            // ユニット情報隠蔽中でも識別済みの敵は情報を隠さない
            var f = new CommandAttackProcessFixture();
            f.SRC.Expression.DefineGlobalVariable("Option(ユニット情報隠蔽)");
            var ud = f.AddUnitData("敵機");
            f.AddWeapon(ud, "敵の武器");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            enemy.AddCondition("識別済み", -1, cdata: "");
            f.AddEventLabel("ユニットコマンド 調べる 敵:", 1);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], enemy);

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("移動範囲", Label(menu, Command.MoveCmdID));
            Assert.AreEqual("射程範囲", Label(menu, Command.AttackCmdID));
            Assert.AreEqual("調べる", Label(menu, Command.UnitCommandCmdID));
        }

        [TestMethod]
        [Ignore("移植の不具合: ダミーユニットで特殊能力一覧の有無の判定が逆になっている")]
        public void CommandSelect_DummyUnitWithoutFeatureList_ReturnsToUnitSelect()
        {
            // VB: ダミーユニットは特殊能力一覧を表示できなければ「ユニット選択」に戻るだけ
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("ダミー", 4, "ダミーユニット=非表示");
            var dummy = f.Deploy("ダミー", "ダミー乗り", "敵", 5, 5);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], dummy);

            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.IsNull(f.LastUnitCommandMenu());
        }

        [TestMethod]
        [Ignore("移植の不具合: ユニット情報隠蔽時に隠すべき一覧系コマンドだけを残し、他のコマンドを消している")]
        public void CommandSelect_HiddenInfoEnemy_HidesListCommandsAndKeepsUnitCommands()
        {
            // VB: 情報隠蔽中の敵は移動範囲・射程範囲・特殊能力一覧・武装一覧・アビリティ一覧を隠し、
            // ユニットコマンドイベントなど他のコマンドは表示する
            var f = new CommandAttackProcessFixture();
            f.SRC.Expression.DefineGlobalVariable("Option(ユニット情報隠蔽)");
            var ud = f.AddUnitData("敵機");
            f.AddWeapon(ud, "敵の武器");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            f.AddEventLabel("ユニットコマンド 調べる 敵:", 1);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], enemy);

            var ids = Ids(f.LastUnitCommandMenu());
            CollectionAssert.AreEqual(new[] { Command.UnitCommandCmdID }, ids);
        }

        // ══════════════════════════════════════════════════════
        // 右クリック（キャンセル）でのユニット選択
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void UnitSelect_RightClickOnUnitWithWeapons_OpensWeaponList()
        {
            // ユニット上でキャンセルすると武装一覧を表示する（コマンド選択には進まない）
            var f = CreateWithAlly(out var ally);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Right, f.SRC.Map.MapData[5, 5], ally);

            var call = f.Headless.CallsOf("WeaponListBox").Single();
            Assert.AreSame(ally, call.Args[0]);
            Assert.AreEqual("一覧", call.Args[3]);
            Assert.IsNull(f.LastUnitCommandMenu());
            Assert.AreEqual(0, f.Commands.SelectedWeapon);
        }

        [TestMethod]
        public void UnitSelect_RightClickOnHiddenEnemy_ShowsNothing()
        {
            // 情報隠蔽された未識別の敵ではキャンセルしても一覧を表示しない
            var f = new CommandAttackProcessFixture();
            f.SRC.Expression.DefineGlobalVariable("Option(ユニット情報隠蔽)");
            var ud = f.AddUnitData("敵機");
            f.AddWeapon(ud, "敵の武器");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Right, f.SRC.Map.MapData[5, 5], enemy);

            Assert.AreEqual(0, f.Headless.CallsOf("WeaponListBox").Count());
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ══════════════════════════════════════════════════════
        // ステータス表示用マップ（MapFileName なし）
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void CommandSelect_StatusView_TransformAvailable_ShowsTransform()
        {
            // ステータス表示では変形先が利用可能なら変形コマンドを表示する
            var f = new CommandAttackProcessFixture(mapFileName: "");
            f.AddUnitData("形態A", 4, "変形=変形 形態B");
            f.AddUnitData("形態B", 4, "変形=変形 形態A");
            var u = f.Deploy("形態A", "パイロット", "味方", 5, 5);
            u.Status = "待機";
            f.Commands.CommandState = "コマンド選択";
            f.Commands.ViewMode = false;
            f.Commands.SelectedUnit = u;
            // 味方でも閲覧時と同様のコマンド一覧を出すため非操作状態にする
            u.AddCondition("非操作", -1, cdata: "");

            f.Commands.ProceedCommand();

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("変形", Label(menu, Command.TransformCmdID));
            // マップが無いので移動範囲・射程範囲は表示しない
            CollectionAssert.DoesNotContain(Ids(menu), Command.MoveCmdID);
        }

        private static CommandAttackProcessFixture CreateStatusViewSplitUnit(out Units.Unit u, int pilots, int partAPilots, int partBPilots)
        {
            // ステータス表示用マップ（MapFileName なし）で 分離 能力を持つユニットを表示する
            var f = new CommandAttackProcessFixture(mapFileName: "");
            f.AddUnitData("合体機", 4, "分離=分離 分離機Ａ 分離機Ｂ").PilotNum = pilots;
            f.AddUnitData("分離機Ａ").PilotNum = partAPilots;
            f.AddUnitData("分離機Ｂ").PilotNum = partBPilots;
            f.SRC.UList.Add("分離機Ａ", 0, "味方");
            f.SRC.UList.Add("分離機Ｂ", 0, "味方");
            u = f.Deploy("合体機", "メイン", "味方", 5, 5);
            for (var i = 2; i <= pilots; i++)
            {
                f.AddPilotData("サブ" + i);
                f.SRC.PList.Add("サブ" + i, 1, "味方").Ride(u);
            }
            u.Status = "待機";
            u.AddCondition("非操作", -1, cdata: "");
            f.Commands.ViewMode = false;
            f.Commands.SelectedUnit = u;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        [TestMethod]
        public void CommandSelect_StatusView_Split_PilotsMatchParts_ShowsSplit()
        {
            // ステータス表示: 搭乗パイロット数が分離先の PilotNum の合計と等しければ分離を表示する
            var f = CreateStatusViewSplitUnit(out var u, 2, 1, 1);
            Assert.AreEqual(2, u.CountPilot());

            f.Commands.ProceedCommand();

            Assert.AreEqual("分離", Label(f.LastUnitCommandMenu(), Command.SplitCmdID));
        }

        [TestMethod]
        [Ignore("移植の不具合: ステータス表示の分離判定でパイロット数の比較が逆（PilotNum 合計 >= CountPilot で表示している）")]
        public void CommandSelect_StatusView_Split_MorePilotsThanParts_ShowsSplit()
        {
            // VB: Command.bas 590-600 は CountPilot < Σ|PilotNum| の時だけ隠す。
            // 3 人乗りで分離先が 1+1 人なら分離できる
            var f = CreateStatusViewSplitUnit(out var u, 3, 1, 1);
            Assert.AreEqual(3, u.CountPilot());

            f.Commands.ProceedCommand();

            Assert.AreEqual("分離", Label(f.LastUnitCommandMenu(), Command.SplitCmdID));
        }

        [TestMethod]
        [Ignore("移植の不具合: ステータス表示の分離判定でパイロット数の比較が逆（PilotNum 合計 >= CountPilot で表示している）")]
        public void CommandSelect_StatusView_Split_NotEnoughPilots_HidesSplit()
        {
            // VB: 2 人乗りで分離先が 2+1 人必要なら分離は表示しない
            var f = CreateStatusViewSplitUnit(out var u, 2, 2, 1);

            f.Commands.ProceedCommand();

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.SplitCmdID);
        }

        // ══════════════════════════════════════════════════════
        // 移動後コマンド選択
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void AfterMoveCommandSelect_ShowsWaitAndAfterMoveAttackOnly()
        {
            // 移動後は待機と移動後に使える武器による攻撃のみ
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            // 射程 2 以上の武器はＰ属性が無ければ移動後に使えない
            f.AddWeapon(ud, "移動後不可の武器", minRange: 1, maxRange: 2);
            f.AddWeapon(ud, "移動後可の武器", minRange: 1, maxRange: 2, wclass: "Ｐ");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            var eud = f.AddUnitData("敵機");
            f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.ProceedCommand();

            var menu = f.LastUnitCommandMenu();
            Assert.AreEqual("待機", Label(menu, Command.WaitCmdID));
            Assert.AreEqual("攻撃", Label(menu, Command.AttackCmdID));
            CollectionAssert.DoesNotContain(Ids(menu), Command.MoveCmdID);
            CollectionAssert.DoesNotContain(Ids(menu), Command.WeaponListCmdID);
        }

        [TestMethod]
        public void AfterMoveCommandSelect_NoAfterMoveWeapon_HidesAttack()
        {
            // 移動後に使える武器が無ければ「攻撃」は表示しない
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            f.AddWeapon(ud, "移動後不可の武器", minRange: 1, maxRange: 2);
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.AddUnitData("敵機");
            f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.ProceedCommand();

            CollectionAssert.AreEqual(new[] { Command.WaitCmdID }, Ids(f.LastUnitCommandMenu()));
        }

        [TestMethod]
        [Ignore("移植の不具合: 移動後のユニットコマンドは「*」付きラベルのみ表示すべきだが全ラベルを表示する")]
        public void AfterMoveCommandSelect_OnlyAfterMoveUnitCommandsShown()
        {
            // VB: 移動後コマンド選択では AsterNum >= 2（*, *-, **）のユニットコマンドのみ
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("自機");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.AddEventLabel("ユニットコマンド 通常 全:", 1);
            f.AddEventLabel("*ユニットコマンド 移動後 全:", 2);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.ProceedCommand();

            var labels = f.LastUnitCommandMenu().Where(x => x.Id == Command.UnitCommandCmdID).Select(x => x.Label).ToList();
            CollectionAssert.AreEqual(new[] { "移動後" }, labels);
        }
    }
}
