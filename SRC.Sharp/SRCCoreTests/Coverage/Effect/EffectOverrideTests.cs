using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.Effects
{
    /// <summary>
    /// Effect のうち、特殊効果データによる個別指定が自動選択より優先される規則と、
    /// 回避・切り払い・シールド・攻撃無効化・破壊/爆発アニメの残りの分岐のテスト。
    /// VB Effect.bas の DodgeEffect / ParryEffect / ShieldEffect / NegateEffect / DieAnimation / ExplodeAnimation に対応。
    /// </summary>
    [TestClass]
    public class EffectOverrideTests
    {
        private const int NormalWait = 700;

        // ユニット名で特殊効果データ（effect.txt 相当）を定義する
        private static void DefineEffect(EffectTestEnv env, Unit u, string situation, string data)
        {
            var ed = env.Src.EDList.IsDefined(u.Name) ? env.Src.EDList.Item(u.Name) : env.Src.EDList.Add(u.Name);
            ed.AddMessage(situation, data);
        }

        private static List<string> SysMessages(EffectTestEnv env)
            => env.Gui.CallsOf("DisplaySysMessage").Select(x => (string)x.Args[0]).ToList();

        private static void AssertList(IEnumerable<string> actual, params string[] expected)
        {
            var list = actual.ToList();
            CollectionAssert.AreEqual(expected, list, "実際: " + string.Join(" / ", list));
        }

        // ──────────────────────────────────────────────
        // DodgeEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DodgeEffect_WeaponDodgeEffectDefined_UsesItInsteadOfSwing()
        {
            // VB DodgeEffect: 「武器名(回避)」の特殊効果が定義されていれば、自動選択の風切り音の代わりにそれを使う
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ロングソード", "武");
                DefineEffect(env, w.Unit, "ロングソード(回避)", "Whip.wav");
                env.Src.Effect.DodgeEffect(w.Unit, w);
                AssertList(env.PlayedWaves(), "Whip.wav");
            }
        }

        [TestMethod]
        public void DodgeEffect_WeaponDodgeEffectDefined_UsedEvenWithAnimationOn()
        {
            // 個別指定は戦闘アニメオンでも使われる（自動選択の場合はアニメオンなら何もしない）
            using (var env = new EffectTestEnv(true))
            {
                var w = env.CreateWeapon("ロングソード", "武");
                DefineEffect(env, w.Unit, "ロングソード(回避)", "Whip.wav");
                env.Src.Effect.DodgeEffect(w.Unit, w);
                AssertList(env.PlayedWaves(), "Whip.wav");
            }
        }

        [TestMethod]
        public void DodgeEffect_AttackEffectIsSwingOnly_NoExtraSwing()
        {
            // 攻撃時の特殊効果が風切り音だけなら、回避時に風切り音を重ねない
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ロングソード", "武");
                DefineEffect(env, w.Unit, "ロングソード", "Swing.wav");
                env.Src.Effect.DodgeEffect(w.Unit, w);
                AssertList(env.PlayedWaves());
            }
        }

        // ──────────────────────────────────────────────
        // ParryEffect / ShieldEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ParryEffect_AnimationOn_StillPlaysSounds()
        {
            // VB ParryEffect には戦闘アニメの分岐が無く、アニメオンでも切り払い音を鳴らす
            using (var env = new EffectTestEnv(true, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.ParryEffect(w.Unit, w, t.Unit);
                AssertList(env.PlayedWaves(), "Saber.wav", "Explode(Small).wav");
                Assert.IsFalse(env.Src.Sound.IsWavePlayed);
            }
        }

        [TestMethod]
        public void ParryEffect_RightButtonPressed_Skips()
        {
            // 右クリック中は効果音をスキップ
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                env.Gui.Handle("IsRButtonPressed", _ => true);
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.ParryEffect(w.Unit, w, t.Unit);
                AssertList(env.PlayedWaves());
            }
        }

        [TestMethod]
        public void ShieldEffect_NonAutoSelectOption_IgnoresShieldType()
        {
            // 戦闘アニメ非自動選択ならシールドの種類によらず「シールド防御発動」
            using (var env = new EffectTestEnv(true, new[] { "シールド防御発動" }, options: new[] { "戦闘アニメ非自動選択" }))
            {
                var w = env.CreateWeapon("ダミー", "", feature: "大型シールド");
                env.Src.Effect.ShieldEffect(w.Unit);
                AssertList(env.Animations(), "シールド防御発動");
            }
        }

        [TestMethod]
        public void ShieldEffect_AnimationOff_ShowsNothing()
        {
            // ShieldEffect はアニメ表示のみで、戦闘アニメオフなら何も表示せず効果音も鳴らさない
            using (var env = new EffectTestEnv(false, new[] { "シールド防御発動" }))
            {
                var w = env.CreateWeapon("ダミー", "", feature: "シールド");
                env.Src.Effect.ShieldEffect(w.Unit);
                AssertList(env.Animations());
                AssertList(env.PlayedWaves());
            }
        }

        // ──────────────────────────────────────────────
        // NegateEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NegateEffect_BeamAbsorbed_AnimationOn_ShowsParticleAndNamedMessage()
        {
            // ビーム吸収（dmg < 0）は戦闘アニメオンなら粒子集中発動。能力名付きの吸収メッセージ
            using (var env = new EffectTestEnv(true, new[] { "粒子集中発動", "ビームコート発動" }))
            {
                var w = env.CreateWeapon("炎のビーム", "Ｂ");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, -100, "ビーム吸収", "Ｂ", 0, "", true);
                AssertList(env.Animations(), "粒子集中発動 赤");
                AssertList(SysMessages(env), "守備機の[ビーム吸収]が攻撃を吸収した。");
            }
        }

        [TestMethod]
        public void NegateEffect_Absorbed_NotBeam_NoHitEffect()
        {
            // 攻撃無効化側の吸収は吸収の特殊効果のみで、命中時の特殊効果は表示しない。
            // 能力名が無ければ「～は攻撃を吸収した。」（ビーム側の「～が」と異なる）
            using (var env = new EffectTestEnv(true, new[] { "粒子集中発動", "爆発命中", "バリア発動" }, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("炎の矢", "実");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, -100, "", "1000", 0, "", true);
                AssertList(env.Animations(), "粒子集中発動 赤");
                AssertList(SysMessages(env), "守備機は攻撃を吸収した。");
            }
        }

        [TestMethod]
        public void NegateEffect_TargetEffectDefined_TakesPriority()
        {
            // 防御側に「攻撃無効化(能力名)」の特殊効果があれば、バリア発動アニメの代わりにそれを使う
            using (var env = new EffectTestEnv(true, new[] { "バリア発動", "爆発命中" }, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                DefineEffect(env, t.Unit, "攻撃無効化(バリア)", "Bubble.wav");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "バリア", "1000", 10, "", true);
                AssertList(env.PlayedWaves(), "Bubble.wav");
                AssertList(env.Animations());
                AssertList(SysMessages(env), "守備機の[バリア]が攻撃を防いだ。");
            }
        }

        [TestMethod]
        public void NegateEffect_AttackerWeaponNegateEffect_SuppressesHitEffect()
        {
            // 攻撃側の「武器名(攻撃無効化)」の特殊効果が定義されていれば、
            // 他に特殊効果が無くても命中時の特殊効果（効果音）は出さない
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                DefineEffect(env, w.Unit, "ミサイル(攻撃無効化)", "Bubble.wav");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "", "1000", 0, "", true);
                AssertList(env.PlayedWaves(), "Bubble.wav");
            }
            // 定義が無ければ通常の命中音（ミサイル→小爆発）になる
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "", "1000", 0, "", true);
                AssertList(env.PlayedWaves(), "Explode(Small).wav");
            }
        }

        [TestMethod]
        public void NegateEffect_BeamCoatSound_NotRepeatedIfWavePlayed()
        {
            // 戦闘アニメオフでビーム無効化時、既に効果音が鳴っていればビームコート音を重ねない
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ビームライフル", "Ｂ");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Sound.IsWavePlayed = true;
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "Ｉフィールド", "Ｂ", 0, "", true);
                AssertList(env.PlayedWaves());
                AssertList(SysMessages(env), "守備機の[Ｉフィールド]が攻撃を防いだ。");
            }
        }

        // ──────────────────────────────────────────────
        // DieAnimation / ExplodeAnimation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DieAndExplodeAnimation_DelegateToGuiWithArguments()
        {
            // 破壊・爆発アニメの描画は GUI 側に委ねる。対象ユニット・サイズ・座標をそのまま渡す
            using (var env = new EffectTestEnv(true))
            {
                var w = env.CreateWeapon("ダミー", "");
                env.Src.Effect.DieAnimation(w.Unit);
                env.Src.Effect.ExplodeAnimation("LL", 3, 7);

                var die = env.Gui.CallsOf("DieAnimation").Single();
                Assert.AreSame(w.Unit, die.Args[0]);
                var explode = env.Gui.CallsOf("ExplodeAnimation").Single();
                CollectionAssert.AreEqual(new object[] { "LL", 3, 7 }, explode.Args);
            }
        }
    }
}
