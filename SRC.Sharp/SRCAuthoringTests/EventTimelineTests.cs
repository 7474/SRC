using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Maps;
using System.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>人形劇の編集（7474/SRC#1694）。VB 版の Unit.Jump・MoveUnitBitmap と同じ規則で、一手ずつの状態を作る</summary>
    [TestClass]
    public class EventTimelineTests
    {
        private const string Scene = @"# 人形劇
スタート:
ChangeMap 第1話.map
Create 味方 灯機ホタル 0 ソラ 10 3 4
Create 敵 灯機ホタル 0 リオ 8 10 4
Talk ソラ
来たな、リオ！
Talk リオ
今日こそ決着をつける。
End
Move リオ 4 4 アニメ表示
Talk リオ
覚悟しろ！
Talk ミウ
待って！
End
Create 味方 灯機ホタル 0 ミウ 10 3 6
Escape リオ
Exit

ターン 1 敵:
Move リオ 1 1
";

        [TestMethod]
        public void Blocks_ListLabelsWithMovesAndTalks()
        {
            var blocks = EventTimeline.Blocks(Scene);
            Assert.AreEqual(2, blocks.Count);
            Assert.AreEqual("スタート", blocks[0].Label);
            Assert.AreEqual(2, blocks[0].Line);
            Assert.AreEqual(5, blocks[0].Moves); // Create×3・Move・Escape
            Assert.AreEqual(4, blocks[0].Talks);
            Assert.AreEqual("ターン 1 敵", blocks[1].Label);
        }

        [TestMethod]
        public void Build_StepsThroughMovesAndTalkLines()
        {
            var steps = EventTimeline.Build(Scene, 2);
            CollectionAssert.AreEqual(
                new[] { "changemap", "create", "create", "talk", "talk", "move", "talk", "talk", "create", "escape" },
                steps.Select(s => s.Kind).ToArray());

            // 会話の行には、その時点でマップにいる人と話す人が付く
            var first = steps[3];
            Assert.AreEqual("ソラ", first.Speaker);
            Assert.AreEqual("来たな、リオ！", first.Message);
            CollectionAssert.AreEquivalent(new[] { "ソラ", "リオ" }, first.Units.Select(u => u.Key).ToArray());

            // Move: (4,4) は空いているのでそのまま。アニメ表示なので L 字の通り道（長い方の軸 x を先に）
            var move = steps[5];
            Assert.AreEqual("リオ", move.Target);
            Assert.AreEqual((10, 4), (move.FromX.Value, move.FromY.Value));
            Assert.AreEqual((4, 4), (move.ToX.Value, move.ToY.Value));
            Assert.AreEqual(7, move.Path.Count);
            CollectionAssert.AreEqual(new[] { 4, 4 }, move.Path.Last());
            Assert.AreEqual(0, move.Warnings.Count);

            // まだ出ていないミウが話すと知らせる（ミウは後で Create される）
            var miu = steps[7];
            Assert.AreEqual("ミウ", miu.Speaker);
            Assert.AreEqual(1, miu.Warnings.Count, string.Join("\n", miu.Warnings));
            Assert.IsTrue(steps[6].Warnings.Count == 0);

            // Escape で退く
            var escape = steps[9];
            CollectionAssert.AreEquivalent(new[] { "ソラ", "ミウ" }, escape.Units.Select(u => u.Key).ToArray());

            // Exit で止まり、次の塊（ターン 1 敵:）には進まない
            Assert.IsFalse(steps.Any(s => s.Line > 18));
        }

        [TestMethod]
        public void Build_RelocatesLikeVbJumpWhenTheCellIsTaken()
        {
            var text = @"スタート:
Create 味方 機体A 0 甲 10 5 5
Create 敵 機体B 0 乙 10 8 8
Move 乙 5 5
Move 丙 1 1
";
            var map = new TimelineMap { Width = 10, Height = 10 };
            var steps = EventTimeline.Build(text, 1, map);
            var move = steps[2];
            // (5,5) は甲がいるので、距離 1 のマスを x の小さい順・y の小さい順に探す（VB 版の Unit.Jump の順）: (4,5)
            Assert.AreEqual((4, 5), (move.ToX.Value, move.ToY.Value));
            Assert.AreEqual((5, 5), (move.RequestedX.Value, move.RequestedY.Value));
            Assert.AreEqual(1, move.Warnings.Count);
            Assert.IsNull(move.Path, "アニメ表示でなければ通り道は無い（一瞬で移る）");

            // 出ていないユニットを動かすと知らせる
            Assert.AreEqual(1, steps[3].Warnings.Count);
            StringAssert.Contains(steps[3].Warnings[0], "丙");
        }

        [TestMethod]
        public void Build_SkipsTerrainTheUnitCannotEnter()
        {
            var text = @"スタート:
Create 味方 機体A 0 甲 10 2 2
Move 甲 3 2
";
            // (3,2) は入れない地形（移動コスト 1000）なので、距離 1 の (2,2)…は自分が抜けた後で空いているので (2,2)
            var map = new TimelineMap { Width = 5, Height = 5, Terrain = (x, y) => x == 3 ? ("陸", 1000d) : ("陸", 1d) };
            var steps = EventTimeline.Build(text, 1, map);
            Assert.AreEqual((2, 2), (steps[1].ToX.Value, steps[1].ToY.Value));
        }

        [TestMethod]
        public void Build_FollowsUnitIdVariables()
        {
            var text = @"スタート:
Create 敵 機体B 0 煤鴉兵(ザコ) 6 18 7
先陣 = UnitID(煤鴉兵(ザコ))
Set 次陣 UnitID(煤鴉兵(ザコ))
Move 先陣 8 4 アニメ表示
Move 次陣 9 4
";
            var steps = EventTimeline.Build(text, 1);
            Assert.AreEqual(3, steps.Count);
            Assert.AreEqual("煤鴉兵(ザコ)", steps[1].Target);
            Assert.AreEqual(0, steps[1].Warnings.Count, string.Join("\n", steps[1].Warnings));
            Assert.AreEqual((9, 4), (steps[2].ToX.Value, steps[2].ToY.Value));
        }

        [TestMethod]
        public void SceneScript_PlaysOnlyTheChosenBlock()
        {
            var text = "@作品\r\nプロローグ:\r\nOption 乗り換え\r\nGlobal 名前\r\nTalk ナレーター\r\n長い前置き\r\nEnd\r\nInput 名前 名前は？ ソラ\r\nIf 名前 = \"\" Then\r\n  Set 名前 ソラ\r\nEndIf\r\nExit\r\n"
                + "スタート:\r\nCreate 味方 機体A 0 ソラ 10 3 4\r\nTalk ソラ\r\n出撃！\r\nEnd\r\n先陣 = UnitID(ソラ)\r\nExit\r\n"
                + "ターン 2 味方:\r\nMove ソラ 5 5\r\nTalk ソラ\r\n進むぞ\r\nEnd\r\nExit\r\n"
                + "章題:\r\nTelop 第一話\r\nReturn\r\n";
            var scene = EventTimeline.SceneScript(text, 20, "stage/第1話.map");
            var lines = scene.Split("\r\n");
            // プロローグは設定だけ。Input は既定の値の Set にする。会話・条件の中は飛ばす
            CollectionAssert.AreEqual(new[] { "プロローグ:", "Option 乗り換え", "Global 名前", "Set 名前 ソラ", "Exit" },
                lines.SkipWhile(l => l != "プロローグ:").Take(5).ToArray());
            // スタートで場面のマップに移り、スタートの配置（会話は飛ばす）をしてから、場面を流す。Exit は終わりに飛ぶ
            CollectionAssert.AreEqual(new[]
            {
                "スタート:", "ChangeMap stage\\第1話.map", "Create 味方 機体A 0 ソラ 10 3 4", "先陣 = UnitID(ソラ)",
                "Move ソラ 5 5", "Talk ソラ", "進むぞ", "End", "Goto 場面の再生の終わり",
                "Goto 場面の再生の終わり", "場面の再生の終わり:", "Telop 場面の再生はここまで", "GameClear",
            }, lines.SkipWhile(l => l != "スタート:").Take(13).ToArray());
            // ほかの塊（Call の先）は残し、元のプロローグ・スタート・場面は二重にしない
            StringAssert.Contains(scene, "章題:\r\nTelop 第一話\r\nReturn");
            Assert.AreEqual(1, lines.Count(l => l == "スタート:"));
            Assert.AreEqual(1, lines.Count(l => l == "プロローグ:"));
            Assert.IsFalse(lines.Contains("ターン 2 味方:"));
            Assert.AreEqual("@作品", lines[0]);

            // 場面がスタートなら、配置を重ねない。場面が ChangeMap していれば足さない
            var own = EventTimeline.SceneScript("スタート:\nChangeMap 別.map\nCreate 味方 機体A 0 ソラ 10 3 4\nExit\n", 1, "第1話.map");
            StringAssert.Contains(own, "スタート:\nChangeMap 別.map\nCreate");
            Assert.IsFalse(own.Contains("第1話.map"));
        }

        [TestMethod]
        public void LPath_LongerAxisFirst()
        {
            CollectionAssert.AreEqual(new[] { 1, 1, 1, 2, 1, 3, 2, 3 }, EventTimeline.LPath(1, 1, 2, 3).SelectMany(p => p).ToArray());
            CollectionAssert.AreEqual(new[] { 1, 1, 2, 1, 3, 1, 3, 2 }, EventTimeline.LPath(1, 1, 3, 2).SelectMany(p => p).ToArray());
        }

        [TestMethod]
        public void InsertMove_AfterCommandOrSplittingTheTalk()
        {
            var text = "スタート:\r\n  Create 味方 機体A 0 甲 10 2 2\r\n  Talk 甲\r\n  一行目\r\n  二行目\r\n  End\r\n";

            // 命令の後ならそのまま差し込む（字下げをそろえる）
            var (a, lineA) = EventTimeline.InsertMove(text, 2, "甲", 5, 5);
            Assert.AreEqual(3, lineA);
            StringAssert.Contains(a, "  Create 味方 機体A 0 甲 10 2 2\r\n  Move 甲 5 5\r\n  Talk 甲\r\n");

            // 会話の途中なら会話を切り、End・Move・Talk（同じ話す人）を差し込む
            var (b, lineB) = EventTimeline.InsertMove(text, 4, "甲", 5, 5);
            Assert.AreEqual(6, lineB);
            StringAssert.Contains(b, "  一行目\r\n  End\r\n  Move 甲 5 5\r\n  Talk 甲\r\n  二行目\r\n");

            // 会話の最後の行の後なら、End の後に差し込む
            var (c, lineC) = EventTimeline.InsertMove(text, 5, "甲", 5, 5);
            Assert.AreEqual(7, lineC);
            StringAssert.Contains(c, "  二行目\r\n  End\r\n  Move 甲 5 5\r\n");

            // 名前に空白があれば "…" で囲む
            StringAssert.Contains(EventTimeline.InsertMove("スタート:\n", 1, "灯機 改", 1, 2).text, "Move \"灯機 改\" 1 2");
        }
    }
}
