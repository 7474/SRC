using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using System.Linq;
using static SRCCore.Coverage.Tests.CommandsCoverage.CommandAttackProcessFixture;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.bas の ProceedCommand・CancelCommand と突き合わせて見つけた違いのテスト（7474/SRC#1498）。
    /// 期待値は元の SRC（VB 版）の動作。
    /// </summary>
    [TestClass]
    public class CommandProceedCommandVbTests
    {
        private static CommandAttackProcessFixture CreateWithAlly(out Units.Unit ally, params string[] features)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機", 4, features);
            f.AddWeapon(ud, "ビームサーベル");
            ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            return f;
        }

        // ── マップコマンド ──

        [TestMethod]
        public void MapCommand_NormalStage_OrderFollowsVbMenuIndex()
        {
            // VB: Main.frm の mnuMapCommandItem は番号の順に並ぶ。
            // スペシャルパワー検索は部隊表の次、イベントのマップコマンドは作戦目的の次（自動反撃モードの前）
            var f = new CommandAttackProcessFixture();
            f.SRC.SPDList.Add("集中");
            var pd = f.AddPilotData("SP持ち");
            pd.SP = 50;
            pd.AddSpecialPower("集中", 1, 10);
            f.SRC.PList.Add("SP持ち", 1, "味方");
            f.AddEventLabel("勝利条件", 1);
            f.AddEventLabel("マップコマンド 補給要請:", 2);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            CollectionAssert.AreEqual(new[]
            {
                Command.EndTurnCmdID,
                Command.DumpCmdID,
                Command.UnitListCmdID,
                Command.SearchSpecialPowerCmdID,
                Command.GlobalMapCmdID,
                Command.OperationObjectCmdID,
                Command.MapCommandCmdID,
                Command.AutoDefenseCmdID,
                Command.AutoPlayCmdID,
                Command.ConfigurationCmdID,
                Command.QuickSaveCmdID,
            }, Ids(f.LastMapCommandMenu()));
        }

        [TestMethod]
        public void MapCommand_StatusViewStage_ShowsUnitList()
        {
            // VB: パイロットステータス・ユニットステータスのステージ（MapFileName = ""）では
            // 部隊表の Visible を変えないので、部隊表は表示する
            var f = new CommandAttackProcessFixture(mapFileName: "");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[3, 3], null);

            CollectionAssert.AreEqual(new[] { Command.UnitListCmdID }, Ids(f.LastMapCommandMenu()));
        }

        // ── ユニットコマンド（移動前） ──

        [TestMethod]
        public void CommandSelect_DisplaysUnitStatus()
        {
            // VB: 新ＧＵＩでなければコマンド選択のときに選んだユニットのステータスを表示する
            var f = CreateWithAlly(out var ally);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            var call = f.Headless.CallsOf("DisplayUnitStatus").LastOrDefault();
            Assert.IsNotNull(call);
            Assert.AreSame(ally, call.Args[0]);
        }

        [TestMethod]
        public void CommandSelect_Charge_CaptionIsNotRenamedByTerm()
        {
            // VB: チャージの表示名は Main.frm の「チャージ」のまま（Term は使わない）
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機", 4);
            f.AddWeapon(ud, "溜め撃ち", wclass: "Ｃ");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.SRC.Expression.DefineGlobalVariable("Term(チャージ)");
            f.SRC.Expression.SetVariableAsString("Term(チャージ)", "力溜め");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            Assert.AreEqual("チャージ", Label(f.LastUnitCommandMenu(), Command.ChargeCmdID));
        }

        [TestMethod]
        public void CommandSelect_HyperMode_DisabledHyperForm_Hidden()
        {
            // VB: Command.bas 1278 は変身後の形態の行動不能を見る（.OtherForm(uname).IsConditionSatisfied("行動不能")）。
            // 変身後の形態の IsAbleToEnter（IsAvailable）も行動不能を見るので、どちらでも表示しない
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("通常機", 4, "ハイパーモード=スーパーモード 超形態");
            f.AddWeapon(ud, "ビームサーベル");
            var hud = f.AddUnitData("超形態", 4, "ノーマルモード=通常機");
            f.AddWeapon(hud, "ビームサーベル");
            var ally = f.Deploy("通常機", "主人公", "味方", 5, 5);
            ally.MainPilot().Morale = 150;
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);
            Assert.AreEqual("スーパーモード", Label(f.LastUnitCommandMenu(), Command.HyperModeCmdID));

            ally.OtherForm("超形態").AddCondition("行動不能", -1, cdata: "");
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);
            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.HyperModeCmdID);
        }

        [TestMethod]
        public void CommandSelect_SplitAndPartsSplit_ShowsOneItemNamedPartsSplit()
        {
            // VB: パーツ分離は分離と同じメニュー項目の表示名を置き換える（項目は 1 つ）
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("合体機", 4, "分離=分離 分離機Ａ 分離機Ｂ", "パーツ分離=パージ 軽装機");
            ud.PilotNum = 2;
            f.AddWeapon(ud, "ビームサーベル");
            f.AddUnitData("分離機Ａ");
            f.AddUnitData("分離機Ｂ");
            f.AddUnitData("軽装機");
            f.SRC.UList.Add("分離機Ａ", 0, "味方");
            f.SRC.UList.Add("分離機Ｂ", 0, "味方");
            var u = f.Deploy("合体機", "メイン", "味方", 5, 5);
            f.AddPilotData("サブ");
            f.SRC.PList.Add("サブ", 1, "味方").Ride(u);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            var splits = f.LastUnitCommandMenu().Where(x => x.Id == Command.SplitCmdID).ToList();
            Assert.AreEqual(1, splits.Count);
            Assert.AreEqual("パージ", splits[0].Label);
        }

        private static CommandAttackProcessFixture CreateTwoCombineUnit(out Units.Unit u)
        {
            // 3 体合体の能力を 2 つ持ち、どちらも合体できるユニット
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("Ａ機", 4, "合体=合体Ｘ 合体機Ｘ Ｂ機 Ｃ機", "合体=合体Ｙ 合体機Ｙ Ｂ機 Ｃ機");
            f.AddWeapon(ud, "ビームサーベル");
            f.AddUnitData("Ｂ機");
            f.AddUnitData("Ｃ機");
            f.AddUnitData("合体機Ｘ");
            f.AddUnitData("合体機Ｙ");
            f.SRC.UList.Add("合体機Ｘ", 0, "味方");
            f.SRC.UList.Add("合体機Ｙ", 0, "味方");
            u = f.Deploy("Ａ機", "パイロットＡ", "味方", 5, 5);
            f.Deploy("Ｂ機", "パイロットＢ", "味方", 5, 6);
            f.Deploy("Ｃ機", "パイロットＣ", "味方", 6, 5);
            return f;
        }

        [TestMethod]
        public void CommandSelect_TwoCombineFeatures_ShowsFirstOnly()
        {
            // VB: Command.bas 1257-1260 は条件を満たす最初の合体能力で Exit For する（項目は 1 つ）
            var f = CreateTwoCombineUnit(out var u);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            var combines = f.LastUnitCommandMenu().Where(x => x.Id == Command.CombineCmdID).ToList();
            Assert.AreEqual(1, combines.Count);
            Assert.AreEqual("合体Ｘ", combines[0].Label);
        }

        [TestMethod]
        public void CommandSelect_FirstCombineHidden_SecondNamedCombineShown()
        {
            // VB: Command.bas 1226 は能力ごとに .FeatureName(i) を見る。
            // 最初の合体能力の名称が非表示でも、2 つ目の名称のある合体能力はメニューに出す
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("Ａ機", 4, "合体=非表示 合体機Ｘ Ｂ機 Ｃ機", "合体=合体Ｙ 合体機Ｙ Ｂ機 Ｃ機");
            f.AddWeapon(ud, "ビームサーベル");
            f.AddUnitData("Ｂ機");
            f.AddUnitData("Ｃ機");
            f.AddUnitData("合体機Ｘ");
            f.AddUnitData("合体機Ｙ");
            f.SRC.UList.Add("合体機Ｘ", 0, "味方");
            f.SRC.UList.Add("合体機Ｙ", 0, "味方");
            var u = f.Deploy("Ａ機", "パイロットＡ", "味方", 5, 5);
            f.Deploy("Ｂ機", "パイロットＢ", "味方", 5, 6);
            f.Deploy("Ｃ機", "パイロットＣ", "味方", 6, 5);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], u);

            Assert.AreEqual("合体Ｙ", Label(f.LastUnitCommandMenu(), Command.CombineCmdID));
        }

        [TestMethod]
        public void AfterMoveCommandSelect_TwoCombineFeatures_ShowsFirstOnly()
        {
            // VB: Command.bas 1732-1735 も同じく最初の合体能力だけ
            var f = CreateTwoCombineUnit(out var u);
            f.Commands.SelectedUnit = u;
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.ProceedCommand();

            var combines = f.LastUnitCommandMenu().Where(x => x.Id == Command.CombineCmdID).ToList();
            Assert.AreEqual(1, combines.Count);
            Assert.AreEqual("合体Ｘ", combines[0].Label);
        }

        private static CommandAttackProcessFixture CreateRepairUnit(out Units.Unit fixer, out Units.Unit damaged, params string[] damagedFeatures)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("修理機", 4, "修理装置");
            f.AddWeapon(ud, "ビームサーベル");
            f.AddUnitData("損傷機", 4, damagedFeatures);
            fixer = f.Deploy("修理機", "整備士", "味方", 5, 5);
            damaged = f.Deploy("損傷機", "僚機", "味方", 5, 6);
            damaged.HP = damaged.MaxHP / 2;
            return f;
        }

        [TestMethod]
        public void Fix_AdjacentZombie_HiddenBeforeMove_ShownAfterMove()
        {
            // VB: 移動前のメニューはゾンビの隣接ユニットを除く（Command.bas 979）が、
            // 移動後のメニューはゾンビを見ない（Command.bas 1563）
            var f = CreateRepairUnit(out var fixer, out var damaged);
            damaged.AddCondition("ゾンビ", -1, cdata: "");

            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], fixer);
            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.FixCmdID);

            f.Commands.SelectedUnit = fixer;
            f.Commands.CommandState = "移動後コマンド選択";
            f.Commands.ProceedCommand();
            Assert.AreEqual("修理装置", Label(f.LastUnitCommandMenu(), Command.FixCmdID));
        }

        [TestMethod]
        public void Fix_AdjacentRepairImmune_ShownInMenu()
        {
            // VB: メニューは隣接ユニットの HP だけを見て、修理不可は見ない（修理の対象を選ぶときに見る）
            var f = CreateRepairUnit(out var fixer, out _, "修理不可=修理不可 修理装置");
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], fixer);

            Assert.AreEqual("修理装置", Label(f.LastUnitCommandMenu(), Command.FixCmdID));
        }

        [TestMethod]
        public void Supply_AdjacentZombieWithLowEN_HiddenBeforeMove_ShownAfterMove()
        {
            // VB: 移動前はゾンビの EN を見ない（Command.bas 1040）が、移動後は見る（Command.bas 1620）
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("補給機", 4, "補給装置");
            f.AddWeapon(ud, "ビームサーベル");
            f.AddUnitData("消耗機");
            var supplier = f.Deploy("補給機", "補給兵", "味方", 5, 5);
            var tired = f.Deploy("消耗機", "僚機", "味方", 5, 6);
            tired.EN = 10;
            tired.AddCondition("ゾンビ", -1, cdata: "");

            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], supplier);
            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.SupplyCmdID);

            f.Commands.SelectedUnit = supplier;
            f.Commands.CommandState = "移動後コマンド選択";
            f.Commands.ProceedCommand();
            Assert.AreEqual("補給装置", Label(f.LastUnitCommandMenu(), Command.SupplyCmdID));
        }

        [TestMethod]
        public void Dismiss_ServantCombinedIntoDeployedUnit_Hidden()
        {
            // VB: 旧形態の召喚ユニットは、合体能力の合体後のユニット（2 体合体を含む）の今の形態が出撃中なら召喚解除できない
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("召喚主", 4);
            f.AddWeapon(ud, "ビームサーベル");
            f.AddUnitData("召喚獣", 4, "召喚ユニット", "合体=合体 合体獣 相棒");
            f.AddUnitData("合体獣");
            f.AddUnitData("相棒");
            var summoner = f.Deploy("召喚主", "術師", "味方", 5, 5);
            var servant = f.SRC.UList.Add("召喚獣", 0, "ＮＰＣ");
            summoner.AddServant(servant);
            servant.Summoner = summoner;
            var combined = f.Deploy("合体獣", "合体獣パイロット", "味方", 7, 7);

            servant.Status = "旧形態";
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], summoner);
            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.DismissCmdID);

            // 合体後のユニットが出撃していなければ召喚解除できる
            combined.Status = "待機";
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], summoner);
            Assert.AreEqual("召喚解除", Label(f.LastUnitCommandMenu(), Command.DismissCmdID));
        }

        // ── 味方以外のユニット ──

        [TestMethod]
        public void CommandSelect_Enemy_UnitCommandTargetEvaluatedAsExpression()
        {
            // VB: Command.bas 735 は味方以外のメニューで buf = GetValueAsString(.Para(3)) とさらに式として評価する
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("敵機", 4);
            f.AddWeapon(ud, "ビームサーベル");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 5, 5);
            f.SRC.Expression.DefineGlobalVariable("対象");
            f.SRC.Expression.SetVariableAsString("対象", "陣営名");
            f.SRC.Expression.DefineGlobalVariable("陣営名");
            f.SRC.Expression.SetVariableAsString("陣営名", "敵");
            f.AddEventLabel("ユニットコマンド 調べる 対象:", 1);
            f.Commands.CommandState = "ユニット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], enemy);

            Assert.AreEqual("調べる", Label(f.LastUnitCommandMenu(), Command.UnitCommandCmdID));
        }

        // ── ユニットステータスのステージ ──

        private static CommandAttackProcessFixture CreateStatusViewUnit(out Units.Unit u, string[] otherUnits, params string[] features)
        {
            var f = new CommandAttackProcessFixture(mapFileName: "");
            // 形態のユニットは本体より先に定義しておく
            foreach (var other in otherUnits)
            {
                f.AddUnitData(other, 4, "変形=非表示 表示機");
            }
            var ud = f.AddUnitData("表示機", 4, features);
            f.AddWeapon(ud, "ビームサーベル");
            u = f.Deploy("表示機", "メイン", "味方", 5, 5);
            u.Status = "待機";
            u.AddCondition("非操作", -1, cdata: "");
            f.Commands.ViewMode = false;
            f.Commands.SelectedUnit = u;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        [TestMethod]
        public void StatusView_HiddenTransformName_CaptionFallsBackToTransform()
        {
            // VB: 表示名が空（非表示）なら「変形」と表示する
            var f = CreateStatusViewUnit(out _, new[] { "変形後" }, "変形=非表示 変形後");

            f.Commands.ProceedCommand();

            Assert.AreEqual("変形", Label(f.LastUnitCommandMenu(), Command.TransformCmdID));
        }

        [TestMethod]
        public void StatusView_PartsSplit_HiddenName_ShowsOneItemNamedPartsSplit()
        {
            // VB: パーツ分離の表示名が空なら「パーツ分離」。分離と同じ項目なので 1 つだけ
            var f = CreateStatusViewUnit(out _, new[] { "軽装機" }, "パーツ分離=非表示 軽装機");

            f.Commands.ProceedCommand();

            var splits = f.LastUnitCommandMenu().Where(x => x.Id == Command.SplitCmdID).ToList();
            Assert.AreEqual(1, splits.Count);
            Assert.AreEqual("パーツ分離", splits[0].Label);
        }

        [TestMethod]
        public void StatusView_CombinePartnerNotCreated_HiddenWithoutError()
        {
            // VB: パートナーのユニットが作られていなければ（UList.Item が Nothing）合体は表示しない
            var f = CreateStatusViewUnit(out _, new string[0], "合体=合体 合体後 未作成機");
            f.AddUnitData("合体後");
            f.SRC.UList.Add("合体後", 0, "味方");

            f.Commands.ProceedCommand();

            CollectionAssert.DoesNotContain(Ids(f.LastUnitCommandMenu()), Command.CombineCmdID);
        }

        // ── CancelCommand ──

        [TestMethod]
        public void Cancel_AfterMoveCommandSelect_DisplaysUnitStatus()
        {
            // VB: 移動後コマンド選択のキャンセルで、発進でなければ（新ＧＵＩでなければ）ユニットステータスを表示する
            var f = CreateWithAlly(out var ally);
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 7], null);
            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
            var before = f.Headless.CallsOf("DisplayUnitStatus").Count();

            f.Commands.CancelCommand();

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual(5, ally.y);
            var calls = f.Headless.CallsOf("DisplayUnitStatus").ToList();
            Assert.IsTrue(calls.Count > before);
            Assert.AreSame(ally, calls.Last().Args[0]);
        }
    }
}
