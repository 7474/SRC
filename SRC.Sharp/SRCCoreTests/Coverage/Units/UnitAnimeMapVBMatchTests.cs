using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.cls の SpecialEffect・AnimationData・PlayAnimation・StandBy・Move・Jump・Land を
    /// 元の SRC と突き合わせたテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class UnitAnimeMapVBMatchTests
    {
        // ──────────────────────────────────────────────
        // PlayAnimation・AnimationData・SpecialEffect
        // ──────────────────────────────────────────────

        private static string LoadEvents(UnitMessageAnimeFixture f, params string[] lines)
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcs-anime-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, "test.eve");
            File.WriteAllText(file, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
            f.Config.AppPath = dir;
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            f.Src.FileSystem = fs;
            f.Src.ScenarioPath = dir;
            f.Gui.OpenNowLoadingFormHandler = () => { };
            f.Gui.CloseNowLoadingFormHandler = () => { };
            f.Gui.DisplayLoadingProgressHandler = () => { };
            f.Gui.SetLoadImageSizeHandler = _ => { };
            f.Src.Event.InitEventData();
            f.Src.Event.LoadEventData(file);
            f.Src.BattleAnimationUnavailable = false;
            f.Config.BattleAnimation = true;
            f.Src.Expression.DefineGlobalVariable("記録");
            f.Src.Expression.SetVariableAsString("記録", "");
            return dir;
        }

        [TestMethod]
        public void PlayAnimation_SubroutineWithoutArguments_PassesNoArgument()
        {
            // VB: For j = 2 To ListLength(anime) で引数を足すので、引数の無い指定では「,」を付けずに Call する。
            // 一括指定のときは「一括指定」が 1 番目の引数になる
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            var dir = LoadEvents(f,
                "Exit",
                "戦闘アニメ_テスト発動:",
                "Set 記録 (記録 & \"[\" & Argnum & \":\" & Args(1) & \"]\")",
                "Return",
                "戦闘アニメ_テスト命中:",
                "Set 記録 (記録 & \"[\" & Argnum & \":\" & Args(1) & \"]\")",
                "Return");
            try
            {
                var md = f.Src.ADList.Add("テスト機");
                md.AddMessage("チャージ", "テスト");
                md.AddMessage("ビーム", "テスト");

                u.PlayAnimation("チャージ");
                u.PlayAnimation("ビーム(命中)");

                Assert.AreEqual("[0:][1:一括指定]", f.Src.Expression.GetValueAsString("記録"));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void PlayAnimation_BattleAnimationDisabled_DoesNothing()
        {
            // VB の AnimationData は戦闘アニメが無効なら空文字列を返し、PlayAnimation は何もしない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            f.Src.ADList.Add("汎用").AddMessage("チャージ", "エネルギー充填");
            f.Config.BattleAnimation = false;

            Assert.AreEqual("", u.AnimationData("チャージ", ""));
            u.PlayAnimation("チャージ(命中)");
            u.PlayAnimation("チャージ");

            Assert.AreEqual(0, f.Messages.Count);
        }

        [TestMethod]
        public void SpecialEffect_TrimsOnlyTabSpaceAndFullWidthSpace()
        {
            // VB の TrimString はタブ・空白・全角空白だけを除く。改行は残るので「-」の取り消しにならない
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit();
            var md = f.Src.EDList.Add("汎用");
            md.AddMessage("A", "　-\t");
            md.AddMessage("B", "-\n");

            u.SpecialEffect("A");
            Assert.AreEqual(0, f.Messages.Count);

            u.SpecialEffect("B");
            Assert.AreEqual(1, f.Messages.Count);
        }

        // ──────────────────────────────────────────────
        // StandBy の登場時アニメ
        // ──────────────────────────────────────────────

        private static string PrepareUnitOnBitmaps(UnitMapActionFixture f, List<string> calls)
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcs-standby-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(dir, "Bitmap", "Event"));
            File.WriteAllBytes(Path.Combine(dir, "Bitmap", "Event", "AUnitOn01.bmp"), new byte[0]);
            ((LocalFileConfig)f.Src.SystemConfig).AppPath = dir;
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            f.Src.FileSystem = fs;
            f.GUI.MainFormVisible = true;
            f.GUI.IsRButtonPressedHandler = _ => false;
            f.GUI.DoEventsHandler = () => { };
            f.GUI.MapToPixelXHandler = x => x * 32;
            f.GUI.MapToPixelYHandler = y => y * 32;
            f.GUI.DrawPictureHandler = (fname, dx, dy, dw, dh, sx, sy, sw, sh, opt) => { calls.Add("DrawPicture"); return true; };
            f.GUI.UpdateScreenHandler = () => calls.Add("UpdateScreen");
            f.GUI.ClearPictureHandler = () => calls.Add("ClearPicture");
            return dir;
        }

        [TestMethod]
        public void StandBy_Sortie_PlaysAppearanceAnimationAndRefreshesEachFrame()
        {
            // VB: 画像（BitmapID > 0）があれば 4 コマを描き、コマごとに picMain(0).Refresh してから消す。
            // 画像ファイル名が空でも MakeUnitBitmap は正の番号を返すのでアニメを表示する
            var f = new UnitMapActionFixture();
            var calls = new List<string>();
            var dir = PrepareUnitOnBitmaps(f, calls);
            try
            {
                f.AddUnitData("機体");
                var u = f.CreateUnit("機体");

                u.StandBy(3, 3, "出撃");

                CollectionAssert.AreEqual(
                    Enumerable.Repeat(new[] { "DrawPicture", "UpdateScreen", "ClearPicture" }, 4).SelectMany(x => x).ToList(),
                    calls);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void StandBy_Sortie_HiddenUnit_DoesNotPlayAppearanceAnimation()
        {
            // VB: 「非表示」のユニットは MakeUnitBitmap が -1 を返すので登場時アニメを表示しない
            var f = new UnitMapActionFixture();
            var calls = new List<string>();
            var dir = PrepareUnitOnBitmaps(f, calls);
            try
            {
                f.AddUnitData("機体", features: "非表示").Bitmap = "unit.bmp";
                var u = f.CreateUnit("機体");

                u.StandBy(3, 3, "出撃");

                Assert.AreEqual(0, calls.Count);
                Assert.AreEqual("出撃", u.Status);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        // ──────────────────────────────────────────────
        // Move の合体
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Move_OntoPartnerWithCombineFeature_Combines()
        {
            // VB: 移動先のユニットの合体能力の相手（3 番目）の現在の形態が自分なら Combine する
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機", features: "分離=分離 パーツA パーツB").PilotNum = 2;
            f.AddUnitData("パーツA", features: "合体=合体 合体機 パーツB");
            f.AddUnitData("パーツB", features: "合体=合体 合体機 パーツA");
            var combined = f.CreateUnit("合体機");
            combined.Status = "他形態";
            var pa = f.Deploy(f.CreateUnit("パーツA", "味方", f.CreatePilot("パイロットA")), 1, 1);
            var pb = f.Deploy(f.CreateUnit("パーツB", "味方", f.CreatePilot("パイロットB")), 2, 1);

            pa.Move(2, 1);

            Assert.AreEqual("出撃", combined.Status);
            Assert.AreSame(combined, f.Map.MapDataForUnit[2, 1]);
        }

        [TestMethod]
        public void Move_OntoUnitWithoutCombineFeature_ShowsErrorAndDoesNotCombine()
        {
            // VB は移動先のユニットの合体能力だけを見る。移動先に合体能力も母艦も無ければ
            // 「合体元ユニット…」のエラーで終わり、自分の合体能力では合体しない
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機", features: "分離=分離 パーツA パーツB").PilotNum = 2;
            f.AddUnitData("パーツA", features: "合体=合体 合体機 パーツB");
            f.AddUnitData("パーツB");
            var combined = f.CreateUnit("合体機");
            combined.Status = "他形態";
            var pa = f.Deploy(f.CreateUnit("パーツA", "味方", f.CreatePilot("パイロットA")), 1, 1);
            var pb = f.Deploy(f.CreateUnit("パーツB", "味方", f.CreatePilot("パイロットB")), 2, 1);

            pa.Move(2, 1);

            Assert.AreEqual("他形態", combined.Status);
            Assert.AreEqual("出撃", pa.Status);
            Assert.AreSame(pb, f.Map.MapDataForUnit[2, 1]);
            Assert.IsNull(f.Map.MapDataForUnit[1, 1]);
        }

        // ──────────────────────────────────────────────
        // Jump
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Jump_NoFreeCell_StaysAtOriginalPosition()
        {
            // VB は空き位置が見つからなければ x・y を変えないので、元の位置に戻る
            // （ジャンプ先の距離 10 以内がすべて埋まっているか進入不能）
            var f = new UnitMapActionFixture(1, 15);
            f.AddUnitData("機体");
            var blocker = f.Deploy(f.CreateUnit("機体"), 1, 1);
            var u = f.Deploy(f.CreateUnit("機体"), 1, 15);
            for (var y = 2; y <= 11; y++)
            {
                f.SetTerrain(1, y, "陸", moveCost: 1000);
            }

            u.Jump(1, 1);

            Assert.AreEqual((1, 15), (u.x, u.y));
            Assert.AreSame(u, f.Map.MapDataForUnit[1, 15]);
            Assert.AreSame(blocker, f.Map.MapDataForUnit[1, 1]);
        }

        // ──────────────────────────────────────────────
        // Land の気力低下
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Land_MainPilotIsAdditionalPilot_FirstPilotAlsoLosesMorale()
        {
            // VB: For i = 1 To CountPilot で、メインパイロット以外の全員（1 人目を含む）の気力を 5 下げる。
            // 追加パイロットがメインのときは 1 人目のパイロットも下がる
            var f = new UnitMapActionFixture();
            f.AddPilotData("追加");
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機", features: "追加パイロット=追加");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var first = f.CreatePilot("パイロット");
            var u = f.Deploy(f.CreateUnit("艦載機", "味方", first), 1, 1);
            var main = u.MainPilot();
            Assert.AreNotSame(first, main);
            first.Morale = 110;
            main.Morale = 110;

            u.Escape();
            u.Land(carrier);

            Assert.AreEqual(105, first.Morale);
            Assert.AreEqual(105, main.Morale);
        }
    }
}
