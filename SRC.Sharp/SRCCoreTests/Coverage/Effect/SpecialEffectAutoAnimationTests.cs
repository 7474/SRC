using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.Effects
{
    /// <summary>
    /// Unit.SpecialEffect の戦闘アニメ自動選択のテスト。
    /// VB Unit.cls の SpecialEffect は戦闘アニメ有効時、特殊効果データの効果音に合わせて
    /// 戦闘アニメ（刺突攻撃・白兵武器攻撃・各種命中）を選び、何も描画されなかった場合は
    /// デフォルト攻撃 / ダメージ命中 を表示する（Unit.cls:23948-24030, 24189-24197）。
    /// </summary>
    [TestClass]
    public class SpecialEffectAutoAnimationTests
    {
        private static readonly string[] Labels =
        {
            "刺突攻撃", "白兵武器攻撃", "斬撃命中", "打撃命中", "デフォルト攻撃", "ダメージ命中",
        };

        private static (List<string> animations, List<string> waves) Run(
            string wname, string wclass, string situation, string effect,
            bool battleAnimation = true, string[] options = null)
        {
            using (var env = new EffectTestEnv(battleAnimation, Labels, options: options))
            {
                var w = env.CreateWeapon(wname, wclass);
                var u = w.Unit;
                env.Src.EDList.Add(u.Name).AddMessage(situation, effect);

                u.SpecialEffect(situation);

                return (env.Animations(), env.PlayedWaves());
            }
        }

        [TestMethod]
        public void SpecialEffect_SwingOfSpear_ShowsThrustAnimation()
        {
            // 槍・スピア・ランス・ジャベリンの Swing.wav は「刺突攻撃」に置き換える
            var (animations, waves) = Run("ロングスピア", "武", "ロングスピア", "Swing.wav");
            CollectionAssert.AreEqual(new[] { "刺突攻撃" }, animations);
            Assert.AreEqual(0, waves.Count);
        }

        [TestMethod]
        public void SpecialEffect_SwingOfMeleeWeapon_ShowsMeleeWeaponAnimation()
        {
            // 武・実 属性の武器の Swing.wav は「白兵武器攻撃」に置き換える
            var (animations, waves) = Run("ソード", "武", "ソード", "Swing.wav");
            CollectionAssert.AreEqual(new[] { "白兵武器攻撃" }, animations);
            Assert.AreEqual(0, waves.Count);
        }

        [TestMethod]
        public void SpecialEffect_HitSound_ShowsHitAnimation()
        {
            // (命中) の効果音は対応する命中アニメに置き換える
            var (animations, waves) = Run("ソード", "武", "ソード(命中)", "Saber.wav");
            CollectionAssert.AreEqual(new[] { "斬撃命中 Saber.wav" }, animations);
            Assert.AreEqual(0, waves.Count);

            (animations, _) = Run("ソード", "武", "ソード(命中)", "Punch.wav");
            CollectionAssert.AreEqual(new[] { "打撃命中" }, animations);
        }

        [TestMethod]
        public void SpecialEffect_WeaponWithoutPicture_ShowsDefaultAttack()
        {
            // 置き換え対象でない効果音は再生し、画像が描画されていなければ「デフォルト攻撃」を表示する
            var (animations, waves) = Run("ソード", "武", "ソード", "Beam.wav");
            CollectionAssert.AreEqual(new[] { "デフォルト攻撃" }, animations);
            CollectionAssert.AreEqual(new[] { "Beam.wav" }, waves);
        }

        [TestMethod]
        public void SpecialEffect_HitWithoutPicture_ShowsDamageHit()
        {
            // (命中) で画像が描画されていなければ「ダメージ命中 -.wav」を表示する
            var (animations, waves) = Run("ソード", "武", "ソード(命中)", "Beam.wav");
            CollectionAssert.AreEqual(new[] { "ダメージ命中 -.wav" }, animations);
            CollectionAssert.AreEqual(new[] { "Beam.wav" }, waves);
        }

        [TestMethod]
        public void SpecialEffect_NoAutoSelectOption_PlaysSoundOnly()
        {
            // 「戦闘アニメ非自動選択」オプションがあれば自動選択しない
            var (animations, waves) = Run("ソード", "武", "ソード", "Swing.wav", options: new[] { "戦闘アニメ非自動選択" });
            Assert.AreEqual(0, animations.Count);
            CollectionAssert.AreEqual(new[] { "Swing.wav" }, waves);
        }

        [TestMethod]
        public void SpecialEffect_BattleAnimationOff_PlaysSoundOnly()
        {
            // 戦闘アニメが無効なら効果音をそのまま再生する
            var (animations, waves) = Run("ソード", "武", "ソード", "Swing.wav", battleAnimation: false);
            Assert.AreEqual(0, animations.Count);
            CollectionAssert.AreEqual(new[] { "Swing.wav" }, waves);
        }
    }
}
