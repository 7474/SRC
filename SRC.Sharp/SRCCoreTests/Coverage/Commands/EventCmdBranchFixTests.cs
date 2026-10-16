using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// イベントコマンドの引数の解釈を VB 版（CmdData.cls）に合わせた修正のテスト。
    /// E2E コーパス cmd-branch-* を作る中で見つかったもの。
    /// </summary>
    [TestClass]
    public class EventCmdBranchFixTests
    {
        private static CmdData Parse(SRC src, string text)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, text);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // UseAbility（引数３個）
        // VB: デフォルトユニットのアビリティ名なら「アビリティ 対象」、そうでなければ「ユニット アビリティ」
        // ──────────────────────────────────────────────

        private static CommandUnitOpsFixture CreateAbilityField(out Units.Unit a, out Units.Unit b)
        {
            var f = new CommandUnitOpsFixture();
            var ud = f.DefineUnit("支援機");
            var ad = ud.AddAbility("激励");
            ad.Class = "";
            ad.MinRange = 1;
            ad.MaxRange = 3;
            ad.SetEffect("気力増加Lv1");
            a = f.Place("支援機", "Ａ", "味方", 5, 5);
            b = f.Place("支援機", "Ｂ", "味方", 6, 5);
            return f;
        }

        [TestMethod]
        public void UseAbility_ThreeArgs_AbilityAndTarget_DefaultUnitUsesOnTarget()
        {
            var f = CreateAbilityField(out var a, out var b);
            f.Src.Event.SelectedUnitForEvent = a;
            var morale = b.MainPilot().Morale;

            var ret = Parse(f.Src, "UseAbility 激励 Ｂ").Exec();

            Assert.AreNotEqual(-1, ret);
            Assert.AreEqual(morale + 10, b.MainPilot().Morale);
        }

        [TestMethod]
        public void UseAbility_ThreeArgs_UnitAndAbility_UsesOnItself()
        {
            var f = CreateAbilityField(out var a, out var b);
            f.Src.Event.SelectedUnitForEvent = a;
            var moraleA = a.MainPilot().Morale;
            var moraleB = b.MainPilot().Morale;

            var ret = Parse(f.Src, "UseAbility Ｂ 激励").Exec();

            Assert.AreNotEqual(-1, ret);
            Assert.AreEqual(moraleB + 10, b.MainPilot().Morale);
            Assert.AreEqual(moraleA, a.MainPilot().Morale);
        }

        // ──────────────────────────────────────────────
        // SetSkill（エリアスのレベル乗算）
        // VB: AliasLevelIsPlusMod Or AliasLevelIsMultMod のとき別名に「Lv<指定レベル>」を付ける
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetSkill_AliasWithMultMod_AddsLevelToAliasData()
        {
            var f = new CommandUnitOpsFixture();
            f.Src.ALDList.Add("豪腕").AddAlias("底力Lv*2");
            f.DefineUnit("機体");
            var u = f.Place("機体", "Ａ", "味方", 5, 5);
            var pid = u.MainPilot().ID;

            var ret = Parse(f.Src, "SetSkill Ａ 豪腕 3").Exec();

            Assert.AreNotEqual(-1, ret);
            Assert.AreEqual("6 Lv3", f.Src.Expression.GetValueAsString("Ability(" + pid + ",底力)", true).Trim());
            Assert.AreEqual(6d, u.MainPilot().SkillLevel("底力"));
        }

        [TestMethod]
        public void SetSkill_AliasWithPlusMod_AddsLevelToAliasData()
        {
            var f = new CommandUnitOpsFixture();
            f.Src.ALDList.Add("剣豪").AddAlias("切り払いLv+1=剣豪 剣技");
            f.DefineUnit("機体");
            var u = f.Place("機体", "Ａ", "味方", 5, 5);
            var pid = u.MainPilot().ID;

            Parse(f.Src, "SetSkill Ａ 剣豪 2").Exec();

            Assert.AreEqual("3 剣豪Lv2 剣技", f.Src.Expression.GetValueAsString("Ability(" + pid + ",切り払い)", true));
            Assert.AreEqual(3d, u.MainPilot().SkillLevel("切り払い"));
        }

        // ──────────────────────────────────────────────
        // PaintString
        // VB: 構文解析時に引数を整理していた（引数２個は文字列、４個以上は座標指定の有無を判定）
        // ──────────────────────────────────────────────

        private static (SRC src, List<(string msg, int x, int y, bool wcr)> calls) CreatePaintSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            var calls = new List<(string, int, int, bool)>();
            ((MockGUI)src.GUI).DrawStringHandler = (msg, x, y, wcr) => calls.Add((msg, x, y, wcr));
            return (src, calls);
        }

        [TestMethod]
        public void PaintString_TwoWords_DrawsBothAsString()
        {
            var (src, calls) = CreatePaintSrc();

            Parse(src, "PaintString 二語の 文字列").Exec();

            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual("二語の 文字列", calls[0].msg);
            Assert.AreEqual(Constants.DEFAULT_LEVEL, calls[0].x);
        }

        [TestMethod]
        public void PaintString_CoordsAndSeveralWords_DrawsRestAtPosition()
        {
            var (src, calls) = CreatePaintSrc();

            Parse(src, "PaintString 30 40 三語の 長い 文字列").Exec();

            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual("三語の 長い 文字列", calls[0].msg);
            Assert.AreEqual(30, calls[0].x);
            Assert.AreEqual(40, calls[0].y);
        }

        [TestMethod]
        public void PaintString_ThreeWordsWithoutCoords_DrawsAllAsString()
        {
            var (src, calls) = CreatePaintSrc();

            Parse(src, "PaintString 座標 でない 並び").Exec();

            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual("座標 でない 並び", calls[0].msg);
            Assert.AreEqual(-1, calls[0].x);
            Assert.AreEqual(-1, calls[0].y);
        }

        [TestMethod]
        public void PaintString_SeveralWordsWithSubExpression_ReplacesIt()
        {
            var (src, calls) = CreatePaintSrc();
            src.Expression.SetVariableAsLong("pv", 42);

            Parse(src, "PaintString 10 20 値は $(pv) です").Exec();

            Assert.AreEqual("値は 42 です", calls[0].msg);
        }

        [TestMethod]
        public void PaintString_SingleTermWithSubExpression_ReplacesIt()
        {
            var (src, calls) = CreatePaintSrc();
            src.Expression.SetVariableAsLong("pv", 42);

            Parse(src, "PaintString 値は$(pv)です").Exec();

            Assert.AreEqual("値は42です", calls[0].msg);
        }

        [TestMethod]
        public void PaintString_Semicolon_DrawsStringWithoutSemicolon()
        {
            // 末尾の「;」で解析し直した引数だけを使う（CmdData.ParseArgs が前の引数を残していた）
            var (src, calls) = CreatePaintSrc();

            Parse(src, "PaintString 続けて;").Exec();

            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual("続けて", calls[0].msg);
            Assert.IsTrue(calls[0].wcr);
        }

        [TestMethod]
        public void PaintString_CoordsWithEmptyStringAndSemicolon_DrawsEmptyWithoutCr()
        {
            var (src, calls) = CreatePaintSrc();

            Parse(src, "PaintString 10 20 ;").Exec();

            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual("", calls[0].msg);
            Assert.IsTrue(calls[0].wcr);
        }
    }
}
