using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// BattleConfigData, DialogData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class BattleConfigAndDialogTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // BattleConfigData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BattleConfigData_Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src)
            {
                Name = "ダメージ計算式",
                ConfigCalc = "攻撃力 * 2 - 防御力"
            };

            Assert.AreEqual("ダメージ計算式", cd.Name);
            Assert.AreEqual("攻撃力 * 2 - 防御力", cd.ConfigCalc);
        }

        [TestMethod]
        public void BattleConfigData_Calculate_SimpleExpression()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "10 + 5" };
            Assert.AreEqual(15d, cd.Calculate());
        }

        [TestMethod]
        public void BattleConfigData_Calculate_SetsIsConfigTrue()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src)
            {
                ConfigCalc = "1"
            };
            // Calculate実行中にIsConfigがtrueになるが、終了後はfalseに戻る
            cd.Calculate();
            Assert.IsFalse(src.Event.BCVariable.IsConfig);
        }

        // ──────────────────────────────────────────────
        // DialogData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DialogData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var dd = new DialogData(src) { Name = "アムロ" };
            Assert.AreEqual("アムロ", dd.Name);
        }

        [TestMethod]
        public void DialogData_AddDialog_IncreasesCount()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            Assert.AreEqual(0, dd.CountDialog());
            dd.AddDialog("攻撃");
            Assert.AreEqual(1, dd.CountDialog());
        }

        [TestMethod]
        public void DialogData_AddDialog_MultipleDialogs()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("攻撃");
            dd.AddDialog("防御");
            dd.AddDialog("回避");
            Assert.AreEqual(3, dd.CountDialog());
        }

        [TestMethod]
        public void DialogData_AddDialog_ReturnedDialog_HasCorrectSituation()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var dialog = dd.AddDialog("撃墜");
            Assert.AreEqual("撃墜", dialog.Situation);
        }

        // ──────────────────────────────────────────────
        // Dialog
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dialog_AddMessage_IncreasesItemCount()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var dialog = dd.AddDialog("攻撃");
            dialog.AddMessage("アムロ", "行くぞ！");
            Assert.AreEqual(1, dialog.Items.Count);
        }

        [TestMethod]
        public void Dialog_AddMessage_StoresCorrectNameAndMessage()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var dialog = dd.AddDialog("撃墜");
            dialog.AddMessage("シャア", "認めたくないものだな");
            Assert.AreEqual("シャア", dialog.Items[0].strName);
            Assert.AreEqual("認めたくないものだな", dialog.Items[0].strMessage);
        }

        [TestMethod]
        public void Dialog_AddMessage_Multiple_AddsAll()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var dialog = dd.AddDialog("会話");
            dialog.AddMessage("話者A", "メッセージA");
            dialog.AddMessage("話者B", "メッセージB");
            dialog.AddMessage("話者C", "メッセージC");
            Assert.AreEqual(3, dialog.Items.Count);
        }

        [TestMethod]
        public void DialogData_Items_InitiallyEmpty()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            Assert.AreEqual(0, dd.Items.Count);
        }

        [TestMethod]
        public void DialogData_Items_AfterAddDialog_CountIncreases()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            dd.AddDialog("攻撃");
            dd.AddDialog("防御");
            Assert.AreEqual(2, dd.Items.Count);
        }

        [TestMethod]
        public void BattleConfigData_Name_CanBeChanged()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { Name = "初期名" };
            cd.Name = "変更後";
            Assert.AreEqual("変更後", cd.Name);
        }

        [TestMethod]
        public void BattleConfigData_Calculate_MultiplyExpression()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "3 * 4" };
            Assert.AreEqual(12d, cd.Calculate());
        }

        [TestMethod]
        public void BattleConfigData_Calculate_SubtractExpression()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "20 - 7" };
            Assert.AreEqual(13d, cd.Calculate());
        }

        [TestMethod]
        public void DialogData_AddDialog_ReturnsSameAsItems()
        {
            var src = CreateSRC();
            var dd = new DialogData(src);
            var d1 = dd.AddDialog("撃墜");
            Assert.AreSame(d1, dd.Items[0]);
        }
    }
}
