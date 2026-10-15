using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Models;
using System.Linq;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.unitform.cs の「変形」「ハイパーモード」「変身解除」コマンドのテスト。
    /// 元の SRC の Command.bas TransformCommand / HyperModeCommand / CancelTransformationCommand に対応する。
    /// </summary>
    [TestClass]
    public class CommandUnitFormTransformTests
    {
        private static void Run(CommandUnitFormFixture f, int cmdId, string label)
        {
            f.Src.Commands.UnitCommand(new UiCommand(cmdId, label));
        }

        // ──────────────────────────────────────────────
        // 変形（マップ上）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Transform_OnMap_SingleForm_TransformsWithoutListBox()
        {
            // 変形先が１つなら選択リストを出さずにそのまま変形し、
            // 選択ユニットは変形後の形態（マップ上の同じ位置のユニット）になる
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var ua = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = ua;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual(0, f.ListBoxes.Count);
            var selected = f.Src.Commands.SelectedUnit;
            Assert.AreEqual("形態B", selected.Name);
            Assert.AreSame(selected, f.Src.Map.MapDataForUnit[3, 3]);
            Assert.AreEqual("出撃", selected.Status);
            Assert.AreEqual("他形態", ua.Status);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Transform_OnMap_MultipleForms_ListBoxSelectsDestination()
        {
            // 変形先が複数なら「変形先」リストで選ばせ、選択した形態に変形する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("一号機", "変形=変形 二号機 三号機");
            f.AddUnitData("二号機", "変形=変形 一号機 三号機");
            f.AddUnitData("三号機", "変形=変形 一号機 二号機");
            var u = f.CreateUnit("一号機", "パイロット", x: 2, y: 2);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ListBoxAnswer = _ => 2;

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual(1, f.ListBoxes.Count);
            var lb = f.ListBoxes[0];
            Assert.AreEqual("変形先", lb.lb_caption);
            Assert.IsTrue(lb.HasFlag);
            CollectionAssert.AreEqual(new[] { "二号機", "三号機" }, lb.Items.Select(x => x.ListItemID).ToArray());
            Assert.AreEqual("三号機", f.Src.Commands.SelectedUnit.Name);
            Assert.AreSame(f.Src.Commands.SelectedUnit, f.Src.Map.MapDataForUnit[2, 2]);
        }

        [TestMethod]
        public void Transform_OnMap_HeroUsesTransformCaptionForHero()
        {
            // ユニットクラスが "(" で始まる変身ヒーローは「変身先」と表示する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("ヒーロー", "変形=変身 フォーム1 フォーム2").Class = "(格闘)";
            f.AddUnitData("フォーム1").Class = "(格闘)";
            f.AddUnitData("フォーム2").Class = "(格闘)";
            var u = f.CreateUnit("ヒーロー", "パイロット", x: 2, y: 2);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.TransformCmdID, "変身");

            Assert.AreEqual("変身先", f.ListBoxes.Single().lb_caption);
            // キャンセルしたので変形しない
            Assert.AreEqual("ヒーロー", f.Src.Commands.SelectedUnit.Name);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Transform_OnMap_ListBoxCancel_KeepsForm()
        {
            // 変形先リストをキャンセルすると変形せずにコマンドを取り消す
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("一号機", "変形=変形 二号機 三号機");
            f.AddUnitData("二号機");
            f.AddUnitData("三号機");
            var u = f.CreateUnit("一号機", "パイロット", x: 2, y: 2);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual("出撃", u.Status);
            Assert.AreSame(u, f.Src.Map.MapDataForUnit[2, 2]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Transform_OnMap_SingleFormCannotEnterTerrain_ShowsMessageAndCancels()
        {
            // 変形先が１つでその形態が現在地形に進入できない場合は
            // 「この地形では<変形の名称>できません」と表示して取り消す
            var f = new CommandUnitFormFixture(onMap: true);
            f.SetTerrain(4, 4, new TerrainData { ID = 1, Name = "海", Class = "水", MoveCost = 2 });
            var ua = f.AddUnitData("形態A", "変形=分身 形態B");
            ua.Transportation = "陸空";
            ua.Adaption = "AAAA";
            var ub = f.AddUnitData("形態B");
            ub.Adaption = "AA-A"; // 水適応なし
            var u = f.CreateUnit("形態A", "パイロット", x: 4, y: 4);
            Assert.AreSame(u, f.Src.Map.MapDataForUnit[4, 4]);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.TransformCmdID, "分身");

            Assert.AreEqual("この地形では分身できません", f.Confirms.Single().message);
            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
            Assert.AreEqual("出撃", u.Status);
        }

        [TestMethod]
        public void Transform_OnMap_NoActionLeft_CancelRevertsToPreviousForm()
        {
            // 変形後に行動できなくなる場合は確認し、キャンセルなら元の形態に戻す
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var u = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            u.UsedAction = 1; // 既に行動済み → 変形後の Action が 0
            f.Src.Commands.SelectedUnit = u;
            f.ConfirmAnswer = _ => GuiDialogResult.Cancel;

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual(1, f.Confirms.Count);
            StringAssert.StartsWith(f.Confirms[0].message, "この形態ではこれ以上の行動が出来ません。");
            Assert.AreEqual("変形", f.Confirms[0].title);
            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
            Assert.AreSame(u, f.Src.Map.MapDataForUnit[3, 3]);
        }

        [TestMethod]
        public void Transform_OnMap_NoActionLeft_CancelRemovesExhaustion()
        {
            // 変形をキャンセルして元の形態に戻したときは「消耗」状態を解除する（TransformCommand）
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var u = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            u.UsedAction = 1;
            u.AddCondition("消耗", 1, cdata: "");
            f.Src.Commands.SelectedUnit = u;
            f.ConfirmAnswer = _ => GuiDialogResult.Cancel;

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
            Assert.IsFalse(f.Src.Commands.SelectedUnit.IsConditionSatisfied("消耗"));
        }

        [TestMethod]
        public void Transform_OnMap_NoActionLeft_OkKeepsNewForm()
        {
            // 確認で OK なら行動できなくても変形後の形態のまま
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var u = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            u.UsedAction = 1;
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual(1, f.Confirms.Count);
            Assert.AreEqual("形態B", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void Transform_OnMap_CreatesAdditionalPilotOfDestination()
        {
            // 変形先に追加パイロットがある場合、メッセージ表示のため事前にパイロットを作成する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "追加パイロット=サブパイロット");
            f.AddPilotData("サブパイロット");
            var u = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            Assert.IsFalse(f.Src.PList.IsDefined("サブパイロット"));

            Run(f, Command.TransformCmdID, "変形");

            Assert.IsTrue(f.Src.PList.IsDefined("サブパイロット"));
            Assert.AreEqual("味方", f.Src.PList.Item("サブパイロット").Party);
        }

        // ──────────────────────────────────────────────
        // 変形（ユニットステータス表示＝インターミッション）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Transform_StatusView_SingleForm_TransformsAndRebuildsUnitList()
        {
            // ユニットステータスコマンドでは確認なしで変形し、ユニットリストを作り直して
            // 変形後の形態のステータスを表示する
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var u = f.CreateUnit("形態A", "パイロット");
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual("形態B", u.CurrentForm().Name);
            Assert.AreEqual("他形態", u.Status);
            Assert.AreEqual(0, f.Confirms.Count);
            // MakeUnitList はマップを「ステータス」背景で作り直す
            Assert.IsTrue(f.Gui.CallsOf("SetupBackground").Any(c => (string)c.Args[1] == "ステータス"));
            var displayed = f.Gui.CallsOf("DisplayUnitStatus").Last();
            Assert.AreSame(u.CurrentForm(), displayed.Args[0]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void Transform_StatusView_ListBoxCancel_KeepsForm()
        {
            // ユニットステータスコマンドでも複数候補のときはリストで選び、キャンセルなら変形しない。
            // リストの項目は変形先の愛称、モードは「カーソル移動」（TransformCommand のステータス表示分岐）
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("一号機", "変形=変形 二号機 三号機");
            f.AddUnitData("二号機").Nickname = "ニゴウ";
            f.AddUnitData("三号機").Nickname = "サンゴウ";
            var u = f.CreateUnit("一号機", "パイロット");
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.TransformCmdID, "変形");

            var lb = f.ListBoxes.Single();
            Assert.AreEqual("ニゴウ|サンゴウ", string.Join("|", lb.Items.Select(x => x.Text)));
            Assert.AreEqual("二号機|三号機", string.Join("|", lb.Items.Select(x => x.ListItemID)));
            Assert.AreEqual("カーソル移動", lb.lb_mode);
            Assert.AreEqual("一号機", u.CurrentForm().Name);
        }

        [TestMethod]
        [Ignore("移植の不具合: ユニットステータスコマンドの変形先リストの見出しが「変形」ではなく「変形先」になっている")]
        public void Transform_StatusView_ListCaptionIsTransform()
        {
            // 元の SRC はステータス表示中の変形先選択を ListBox("変形", list, "名前", "カーソル移動") で行う
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("一号機", "変形=変形 二号機 三号機");
            f.AddUnitData("二号機");
            f.AddUnitData("三号機");
            var u = f.CreateUnit("一号機", "パイロット");
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.TransformCmdID, "変形");

            Assert.AreEqual("変形", f.ListBoxes.Single().lb_caption);
        }

        // ──────────────────────────────────────────────
        // ハイパーモード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HyperMode_OnMap_TransformsToHyperForm()
        {
            // ハイパーモード=名称 形態 の第２要素の形態へ変形する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("通常形態", "ハイパーモード=スーパーモード ハイパー形態");
            f.AddUnitData("ハイパー形態", "ノーマルモード=通常形態");
            var u = f.CreateUnit("通常形態", "パイロット", x: 5, y: 5);
            f.Src.Commands.CommandState = "コマンド選択";
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "スーパーモード");

            var selected = f.Src.Commands.SelectedUnit;
            Assert.AreEqual("ハイパー形態", selected.Name);
            Assert.AreSame(selected, f.Src.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
            Assert.AreEqual(0, f.Confirms.Count);
        }

        [TestMethod]
        public void HyperMode_OnMap_CannotEnterTerrain_ShowsMessageAndCancels()
        {
            // ハイパーモードの形態が現在地形に進入できない場合は発動しない
            var f = new CommandUnitFormFixture(onMap: true);
            f.SetTerrain(5, 5, new TerrainData { ID = 1, Name = "海", Class = "水", MoveCost = 2 });
            var normal = f.AddUnitData("通常形態", "ハイパーモード=スーパーモード ハイパー形態");
            normal.Transportation = "陸空";
            f.AddUnitData("ハイパー形態", "ノーマルモード=通常形態").Adaption = "AA-A"; // 水適応なし
            var u = f.CreateUnit("通常形態", "パイロット", x: 5, y: 5);
            f.Src.Commands.CommandState = "コマンド選択";
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "スーパーモード");

            Assert.AreEqual("この地形では変形できません", f.Confirms.Single().message);
            Assert.AreEqual("通常形態", f.Src.Commands.SelectedUnit.Name);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void HyperMode_StatusView_FromHyperForm_ReturnsToNormalForm()
        {
            // ユニットステータスコマンドでは、ハイパーモードを持たない形態（ハイパー形態側）なら
            // ノーマルモードの形態に戻す
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("通常形態", "ハイパーモード=スーパーモード ハイパー形態");
            f.AddUnitData("ハイパー形態", "ノーマルモード=通常形態");
            var u = f.CreateUnit("ハイパー形態", "パイロット");
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "ノーマルモード");

            Assert.AreEqual("通常形態", u.CurrentForm().Name);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void HyperMode_StatusView_FromNormalForm_TransformsToHyperForm()
        {
            // ユニットステータスコマンドで通常形態ならハイパーモード形態へ
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("通常形態", "ハイパーモード=スーパーモード ハイパー形態");
            f.AddUnitData("ハイパー形態", "ノーマルモード=通常形態");
            var u = f.CreateUnit("通常形態", "パイロット");
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "スーパーモード");

            Assert.AreEqual("ハイパー形態", u.CurrentForm().Name);
        }

        // ──────────────────────────────────────────────
        // 変身解除（ノーマルモードに「手動解除」があるとき）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelTransformation_OnMap_ConfirmCancel_KeepsForm()
        {
            // ノーマルモードに手動解除がある場合 HyperModeCmdID は解除コマンドになる。
            // 通常ユニットは「特殊モードを解除しますか？」を確認し、キャンセルなら解除しない
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("通常形態");
            f.AddUnitData("特殊形態", "ノーマルモード=通常形態 手動解除");
            var u = f.CreateUnit("特殊形態", "パイロット", x: 2, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";
            f.ConfirmAnswer = _ => GuiDialogResult.Cancel;

            Run(f, Command.HyperModeCmdID, "特殊モード解除");

            Assert.AreEqual(("特殊モードを解除しますか？", "特殊モード解除"), f.Confirms.Single());
            Assert.AreEqual("特殊形態", f.Src.Commands.SelectedUnit.Name);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelTransformation_OnMap_HeroConfirmOk_ReturnsToNormalForm()
        {
            // 変身ヒーローは「変身を解除しますか？」を確認し、OK ならノーマルモードの形態に戻る
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("素顔").Class = "(格闘)";
            f.AddUnitData("変身形態", "ノーマルモード=素顔 手動解除").Class = "(格闘)";
            var u = f.CreateUnit("変身形態", "パイロット", x: 2, y: 3);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "変身解除");

            Assert.AreEqual(("変身を解除しますか？", "変身解除"), f.Confirms.Single());
            var selected = f.Src.Commands.SelectedUnit;
            Assert.AreEqual("素顔", selected.Name);
            Assert.AreSame(selected, f.Src.Map.MapDataForUnit[2, 3]);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelTransformation_StatusView_ReturnsWithoutConfirm()
        {
            // ユニットステータスコマンドでは確認なしで解除する
            var f = new CommandUnitFormFixture(onMap: false);
            f.AddUnitData("通常形態");
            f.AddUnitData("特殊形態", "ノーマルモード=通常形態 手動解除");
            var u = f.CreateUnit("特殊形態", "パイロット");
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.HyperModeCmdID, "特殊モード解除");

            Assert.AreEqual(0, f.Confirms.Count);
            Assert.AreEqual("通常形態", u.CurrentForm().Name);
        }
    }
}
