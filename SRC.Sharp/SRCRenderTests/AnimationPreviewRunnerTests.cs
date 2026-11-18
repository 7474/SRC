using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SRCRender.Tests
{
    [TestClass]
    public class AnimationPreviewRunnerTests
    {
        [TestMethod]
        public void AnimationPreviewRunner_PlaysAnimationEffectAndSubroutineWithoutMutatingConfig()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                workspace.Write(Path.Combine("Lib", "汎用戦闘アニメ", "include.eve"),
                    "戦闘アニメ_テストビーム:",
                    "Return");

                var customSubEve = workspace.Write("custom_anim.eve",
                    "戦闘アニメ_単体定義:",
                    "Return");

                workspace.WriteMap("preview.map", 16, 11, (x, y) => 0);

                var previewEve = workspace.Write("preview.eve",
                    "スタート:",
                    "ChangeMap preview.map",
                    "Exit");

                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    runner.Start(previewEve, workspace.AppPath);
                    var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                    while (!runner.IsFinished && DateTime.UtcNow < deadline)
                    {
                        var s = runner.Session.SRC;
                        if (s.Turn >= 1 && s.Stage == "味方" && !runner.GUI.IsGUILocked && s.Commands.CommandState == "ユニット選択")
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(50);
                    }

                    T RunOnGame<T>(Func<T> fn)
                    {
                        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
                        runner.Session.Game.Post(() =>
                        {
                            try { tcs.SetResult(fn()); }
                            catch (Exception ex) { tcs.SetException(ex); }
                        });
                        Assert.IsTrue(tcs.Task.Wait(TimeSpan.FromSeconds(15)));
                        return tcs.Task.Result;
                    }

                    var src = runner.Session.SRC;
                    src.Config.BattleAnimation = false;

                    // 1. 戦闘アニメ（Config.BattleAnimation が false でも一時上書きで再生し、設定自体は汚さない）
                    var animRes = RunOnGame(() => AnimationPreviewRunner.Play(src, new AnimationPreviewRequest
                    {
                        Kind = "animation",
                        Target = "試作機",
                        Situation = "攻撃",
                        Spec = "テストビーム",
                    }));
                    Assert.IsTrue(animRes.Played, animRes.Message);
                    Assert.IsFalse(src.Config.BattleAnimation, "Config.BattleAnimation は書き換えない");
                    Assert.IsNull(src.BattleAnimationOverride);

                    // 2. 特殊効果（effect.txt は自動置換を抑えて再生）
                    var effRes = RunOnGame(() => AnimationPreviewRunner.Play(src, new AnimationPreviewRequest
                    {
                        Kind = "effect",
                        Target = "試作機",
                        Situation = "攻撃",
                        Spec = "dummy.wav",
                    }));
                    Assert.IsTrue(effRes.Played, effRes.Message);
                    Assert.IsNull(src.BattleAnimationOverride);

                    // 3. 汎用戦闘アニメサブルーチン（include 外の単体 .eve でも LoadDynamic で読める）
                    var subRes = RunOnGame(() => AnimationPreviewRunner.Play(src, new AnimationPreviewRequest
                    {
                        Kind = "subroutine",
                        File = customSubEve,
                        Target = "汎用戦闘アニメ",
                        Situation = "戦闘アニメ_単体定義",
                        Spec = "戦闘アニメ_単体定義",
                    }));
                    Assert.IsTrue(subRes.Played, subRes.Message);

                    runner.Session.GUI.Terminate();
                }
            }
        }
    }
}
