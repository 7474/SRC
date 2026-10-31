using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SRCCore.Golden.Tests
{
    /// <summary>
    /// 武器の名前と属性から選ばれる戦闘アニメと効果音を、武器ごとの表にして固定する（#1400）。
    /// 武器は Golden/Effect/weapons.txt に並べ、いまの結果を Golden/Effect/expected.txt に置く。
    /// 戦闘アニメは、準備（PrepareWeaponAnimation）・攻撃（AttackAnimation）・命中（HitAnimation）の順に呼び、
    /// 呼ばれた「戦闘アニメ_xxx」の名前と引数を記録する。効果音は、戦闘アニメをオフにして
    /// 準備（PrepareWeaponSound）・攻撃（AttackSound）・命中（HitSound）で鳴らしたファイルを記録する。
    /// 正しさの確かめは EffectAnimationTests・EffectSoundTests（VB 版を期待値にしたもの）が受け持ち、
    /// ここは「変わっていないこと」を見る。意図して変えたときは SRC_GOLDEN_UPDATE=1 で表を作り直す（docs/testing.md）。
    /// </summary>
    [TestClass]
    public class EffectGoldenTests
    {
        // 演出の待ち時間（EffectAnimationTests と同じ）
        private const int MessageWait = 700;

        [TestMethod]
        public void EffectSelection_MatchesGolden()
        {
            var actual = Render();
            var expectedPath = Path.Combine(GoldenDir(), "expected.txt");
            if (Environment.GetEnvironmentVariable("SRC_GOLDEN_UPDATE") == "1")
            {
                File.WriteAllText(expectedPath, actual, new UTF8Encoding(false));
                Assert.Inconclusive("expected.txt を作り直した（SRC_GOLDEN_UPDATE=1）");
            }
            Assert.IsTrue(File.Exists(expectedPath), "expected.txt が無い。SRC_GOLDEN_UPDATE=1 dotnet test --filter EffectGoldenTests で作る");
            var expected = File.ReadAllText(expectedPath).Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }
            // どの武器が変わったかを先頭から 20 件だけ出す
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');
            var diffs = new List<string>();
            for (var i = 0; i < Math.Max(expectedLines.Length, actualLines.Length) && diffs.Count < 20; i++)
            {
                var e = i < expectedLines.Length ? expectedLines[i] : "(無し)";
                var a = i < actualLines.Length ? actualLines[i] : "(無し)";
                if (e != a)
                {
                    diffs.Add($"- {e}\n+ {a}");
                }
            }
            Assert.Fail("演出の選び方が変わった。意図した変更なら SRC_GOLDEN_UPDATE=1 で作り直して差分を PR に載せる\n" + string.Join("\n", diffs));
        }

        private static string Render()
        {
            var weapons = LoadWeapons();

            // 1 回目: ラベルを定義せずに流し、呼ばれた戦闘アニメの名前を集める（未定義のものは名前だけが分かる）
            var labels = new SortedSet<string>(StringComparer.Ordinal);
            foreach (var chunk in Chunks(weapons))
            {
                using (var env = new EffectTestEnv(true, messageWait: MessageWait))
                {
                    foreach (var (name, wclass) in chunk)
                    {
                        foreach (var a in Animations(env, name, wclass).SelectMany(x => x))
                        {
                            if (a.StartsWith("未定義:"))
                            {
                                labels.Add(a.Substring("未定義:".Length));
                            }
                        }
                    }
                }
            }

            // 2 回目: 集めた名前のラベルを定義して流し、引数まで記録する。効果音は戦闘アニメをオフにして記録する
            var sb = new StringBuilder();
            sb.Append("# 武器ごとに選ばれる戦闘アニメと効果音（EffectGoldenTests が作る。手で直さない）\n");
            sb.Append("# 名前, 属性 => 準備 | 攻撃 | 命中 | 準備の音 | 攻撃の音 | 命中の音（無ければ -。複数は / で区切る）\n");
            foreach (var chunk in Chunks(weapons))
            {
                using (var animEnv = new EffectTestEnv(true, labels, messageWait: MessageWait))
                using (var soundEnv = new EffectTestEnv(false, messageWait: MessageWait))
                {
                    foreach (var (name, wclass) in chunk)
                    {
                        var anim = Animations(animEnv, name, wclass);
                        var sound = Sounds(soundEnv, name, wclass);
                        sb.Append($"{name}, {wclass} => ")
                            .Append(string.Join(" | ", anim.Concat(sound).Select(Join)))
                            .Append('\n');
                    }
                }
            }
            return sb.ToString();
        }

        // ユニットが増えると作るたびに遅くなるので、環境は 50 件ごとに作り直す
        private static IEnumerable<List<(string Name, string Class)>> Chunks(List<(string Name, string Class)> weapons)
        {
            for (var i = 0; i < weapons.Count; i += 50)
            {
                yield return weapons.Skip(i).Take(50).ToList();
            }
        }

        private static string Join(List<string> items) => items.Count == 0 ? "-" : string.Join(" / ", items);

        // 準備・攻撃・命中のそれぞれで呼ばれた戦闘アニメ。
        // 準備のアニメが構えた武器（WeaponInHand）を決め直すので、同じ環境で続けて呼んでも前の武器の影響は残らない
        private static List<List<string>> Animations(EffectTestEnv env, string name, string wclass)
        {
            var w = env.CreateWeapon(name, wclass);
            var t = env.CreateWeapon("標的", "");
            return new List<List<string>>
            {
                Take(env, () => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w)),
                Take(env, () => env.Src.Effect.AttackAnimation(w.Unit, w)),
                Take(env, () => env.Src.Effect.HitAnimation(w.Unit, w, t.Unit, 0)),
            };
        }

        private static List<string> Take(EffectTestEnv env, Action act)
        {
            env.Src.Expression.SetVariableAsString("記録", "");
            act();
            return env.Animations();
        }

        // 準備・攻撃・命中のそれぞれで鳴らした効果音
        private static List<List<string>> Sounds(EffectTestEnv env, string name, string wclass)
        {
            var w = env.CreateWeapon(name, wclass);
            var t = env.CreateWeapon("標的", "");
            var result = new List<List<string>>();
            var played = env.PlayedWaves().Count;
            foreach (var act in new Action[]
            {
                () => env.Src.Effect.PrepareWeaponSound(w.Unit, w),
                () => env.Src.Effect.AttackSound(w.Unit, w),
                () => env.Src.Effect.HitSound(w.Unit, w, t.Unit, 0),
            })
            {
                act();
                var waves = env.PlayedWaves();
                result.Add(waves.Skip(played).ToList());
                played = waves.Count;
            }
            return result;
        }

        private static List<(string Name, string Class)> LoadWeapons()
        {
            return File.ReadAllLines(Path.Combine(GoldenDir(), "weapons.txt"))
                .Where(x => x.Trim().Length > 0 && !x.StartsWith("#"))
                .Select(x =>
                {
                    var i = x.IndexOf(',');
                    return (x.Substring(0, i).Trim(), x.Substring(i + 1).Trim());
                })
                .ToList();
        }

        private static string GoldenDir([CallerFilePath] string path = "")
        {
            return Path.Combine(Path.GetDirectoryName(path), "Effect");
        }
    }
}
