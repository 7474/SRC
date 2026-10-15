using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.Effects
{
    /// <summary>
    /// Effect の効果音選択（戦闘アニメオフ時）のテスト。
    /// VB Effect.bas の AttackSound / HitSound / PrepareWeaponSound / DodgeEffect / ParryEffect /
    /// AbsorbEffect と、効果音の再生回数を決める CountAttack / CountAttack0 の規則を確認する。
    /// </summary>
    [TestClass]
    public class EffectSoundTests
    {
        // メッセージスピードが「超高速」(200 以下) でない値。再生回数が武器名どおりになる
        private const int NormalWait = 700;

        private static List<string> AttackSound(string wname, string wclass, int messageWait = NormalWait)
        {
            using (var env = new EffectTestEnv(false, messageWait: messageWait))
            {
                var w = env.CreateWeapon(wname, wclass);
                env.Src.Effect.AttackSound(w.Unit, w);
                return env.PlayedWaves();
            }
        }

        private static List<string> HitSound(string wname, string wclass, bool targetIsHero, int messageWait = NormalWait)
        {
            using (var env = new EffectTestEnv(false, messageWait: messageWait))
            {
                var w = env.CreateWeapon(wname, wclass);
                var t = env.CreateWeapon("ダミー", "", unitClass: targetIsHero ? "(人間)" : "汎用");
                env.Src.Effect.HitSound(w.Unit, w, t.Unit, 0);
                return env.PlayedWaves();
            }
        }

        private static void AssertWaves(IEnumerable<string> actual, params string[] expected)
        {
            CollectionAssert.AreEqual(expected, actual.ToList(), "実際: " + string.Join(",", actual));
        }

        // ──────────────────────────────────────────────
        // AttackSound
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttackSound_InfightWeapon_PlaysNothing()
        {
            // 武・突・接属性の攻撃は攻撃時の効果音が不要（命中時に鳴らす）
            AssertWaves(AttackSound("ロングソード", "武"));
            AssertWaves(AttackSound("ショルダータックル", "突"));
            AssertWaves(AttackSound("ガンパンチ", "接"));
        }

        [TestMethod]
        public void AttackSound_MainGun_BeamOrCannonByClass()
        {
            // 主砲・副砲は Ｂ属性ならビーム音、それ以外は砲撃音
            AssertWaves(AttackSound("主砲", "Ｂ"), "Beam.wav");
            AssertWaves(AttackSound("副砲", "実"), "Cannon.wav");
        }

        [TestMethod]
        public void AttackSound_LaserAndParticleNames()
        {
            // 光線系の名前はレーザー音、粒子・プラズマ系はビーム音
            AssertWaves(AttackSound("熱線砲", ""), "LaserGun.wav");
            AssertWaves(AttackSound("プラズマキャノン", ""), "Beam.wav");
            // ビームは Ｂ属性ならビーム音、そうでなければレーザー音
            AssertWaves(AttackSound("ビームガン", "Ｂ"), "Beam.wav");
            AssertWaves(AttackSound("ビームガン", ""), "LaserGun.wav");
        }

        [TestMethod]
        public void AttackSound_MachineGun_PlaysOnceEvenIfRapidFire()
        {
            // マシンガン系は連射音の効果音を１回だけ鳴らす（CountAttack0 は 4 だが num = 1 に上書き）
            AssertWaves(AttackSound("マシンガン", "実"), "MachineGun.wav");
            AssertWaves(AttackSound("マシンガン", "Ｂ"), "LaserGun.wav");
            AssertWaves(AttackSound("バルカン", "実"), "GunPod.wav");
            AssertWaves(AttackSound("機関砲", "実"), "MachineCannon.wav");
        }

        [TestMethod]
        public void AttackSound_MissileLauncher_RepeatsUpToFourTimes()
        {
            // ミサイルランチャーは CountAttack0 = 4。効果音ごとに 130ms のウェイトが入る
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイルランチャー", "実");
                env.Src.Effect.AttackSound(w.Unit, w);
                AssertWaves(env.PlayedWaves(), "Missile.wav", "Missile.wav", "Missile.wav", "Missile.wav");
                CollectionAssert.AreEqual(new[] { 130, 130, 130, 130 }, env.Sleeps());
                // 再生後は効果音再生フラグがクリアされる
                Assert.IsFalse(env.Src.Sound.IsWavePlayed);
            }
        }

        [TestMethod]
        public void AttackSound_FastMessageSpeed_PlaysOnlyOnce()
        {
            // メッセージスピードが「超高速」(MessageWait <= 200) なら繰り返し数は１
            AssertWaves(AttackSound("ミサイルランチャー", "実", messageWait: 200), "Missile.wav");
        }

        [TestMethod]
        public void AttackSound_CountByNameDigits()
        {
            // 武器名の「Ｎ連」から回数を決め、最大４回に抑える
            AssertWaves(AttackSound("３連ミサイル", "実"), "Missile.wav", "Missile.wav", "Missile.wav");
            AssertWaves(AttackSound("２連ミサイル", "実"), "Missile.wav", "Missile.wav");
            AssertWaves(AttackSound("２４連ミサイル", "実"), "Missile.wav", "Missile.wav", "Missile.wav", "Missile.wav");
            // 「連」を含むが数の指定が無い場合は２回
            AssertWaves(AttackSound("連装ミサイル", "実"), "Missile.wav", "Missile.wav");
            // ツイン・ダブル等は２回
            AssertWaves(AttackSound("ツインミサイル", "実"), "Missile.wav", "Missile.wav");
        }

        [TestMethod]
        public void AttackSound_CountByContinuousAttackLevel()
        {
            // 連属性の武器は連属性のレベルを攻撃回数とする
            AssertWaves(AttackSound("ミサイル", "実連L3"), "Missile.wav", "Missile.wav", "Missile.wav");
        }

        [TestMethod]
        public void AttackSound_Railgun_PlaysThunderThenCannons()
        {
            // レールガン系は放電音の後に砲撃音を回数分鳴らす
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ツインレールガン", "実");
                env.Src.Effect.AttackSound(w.Unit, w);
                AssertWaves(env.PlayedWaves(), "Thunder.wav", "Cannon.wav", "Cannon.wav");
                CollectionAssert.AreEqual(new[] { 300, 130 }, env.Sleeps());
            }
        }

        [TestMethod]
        public void AttackSound_Boomerang_FiveSwingsWithExtraWait()
        {
            // ブーメランは風切り音５回。風切り音のときは 130ms に加え 150ms 待つ
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ブーメラン", "実");
                env.Src.Effect.AttackSound(w.Unit, w);
                AssertWaves(env.PlayedWaves(), Enumerable.Repeat("Swing.wav", 5).ToArray());
                CollectionAssert.AreEqual(Enumerable.Range(0, 5).SelectMany(_ => new[] { 130, 150 }).ToList(), env.Sleeps());
            }
        }

        [TestMethod]
        public void AttackSound_BreathByElement()
        {
            // 「～ブレス」は属性で効果音が変わる
            AssertWaves(AttackSound("ドラゴンブレス", "火"), "AntiShipMissile.wav");
            AssertWaves(AttackSound("ドラゴンブレス", "冷"), "Storm.wav");
            AssertWaves(AttackSound("ドラゴンブレス", "闇"), "GunPod.wav");
            AssertWaves(AttackSound("ドラゴンブレス", "水"), "Hide.wav");
            AssertWaves(AttackSound("ドラゴンブレス", ""), "AntiShipMissile.wav");
        }

        [TestMethod]
        public void AttackSound_ElementAndMagicClasses()
        {
            // 雷属性は放電音１回、魔属性は魔法音
            AssertWaves(AttackSound("ぴりぴり", "雷"), "Thunder.wav");
            AssertWaves(AttackSound("マジックミサイル", "魔"), "Missile.wav");
            AssertWaves(AttackSound("ふしぎな力", "魔"), "Whiz.wav");
        }

        [TestMethod]
        public void AttackSound_UnknownName_FallsBackToClass()
        {
            // 名前で判定できない場合は Ｂ属性ならビーム音、銃属性なら銃声、どちらでもなければ無音
            AssertWaves(AttackSound("ふしぎな力", "Ｂ"), "Beam.wav");
            AssertWaves(AttackSound("ふしぎな力", "銃"), "Gun.wav");
            AssertWaves(AttackSound("ふしぎな力", ""));
        }

        [TestMethod]
        public void AttackSound_RightButtonPressed_Skips()
        {
            // 右クリック中は効果音をスキップ
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                env.Gui.Handle("IsRButtonPressed", _ => true);
                var w = env.CreateWeapon("ミサイル", "実");
                env.Src.Effect.AttackSound(w.Unit, w);
                AssertWaves(env.PlayedWaves());
            }
        }

        // ──────────────────────────────────────────────
        // HitSound
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HitSound_Slash_DependsOnTargetBeingHero()
        {
            // 斬撃は相手が人間（等身大）なら風切り音＋斬撃音、メカなら金属音
            AssertWaves(HitSound("ロングソード", "武", targetIsHero: true), "Swing.wav", "Slash.wav");
            AssertWaves(HitSound("ロングソード", "武", targetIsHero: false), "Saber.wav");
            // ビーム系の刃は相手によらず金属音
            AssertWaves(HitSound("ビームサーベル", "武", targetIsHero: true), "Saber.wav");
        }

        [TestMethod]
        public void HitSound_Slash_RepeatsByCount()
        {
            // ダブル～は２回。２回目以降は 350ms 待って斬撃音
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ダブルソード", "武");
                var t = env.CreateWeapon("ダミー", "", unitClass: "(人間)");
                env.Src.Effect.HitSound(w.Unit, w, t.Unit, 0);
                AssertWaves(env.PlayedWaves(), "Swing.wav", "Slash.wav", "Slash.wav");
                CollectionAssert.AreEqual(new[] { 190, 350 }, env.Sleeps());
            }
        }

        [TestMethod]
        public void HitSound_Stab_DependsOnTargetBeingHero()
        {
            // 槍などの刺突は人間相手なら風切り音＋刺突音
            AssertWaves(HitSound("スピア", "武", targetIsHero: true), "Swing.wav", "Stab.wav");
            AssertWaves(HitSound("スピア", "武", targetIsHero: false), "Saber.wav");
        }

        [TestMethod]
        public void HitSound_BlowAndThrow()
        {
            // 打撃は打撃音、投げ技は風切り音の後に地響き
            AssertWaves(HitSound("パンチ", "突", targetIsHero: true), "Punch.wav");
            AssertWaves(HitSound("ダブルパンチ", "突", targetIsHero: true), "Punch.wav", "Punch.wav");
            AssertWaves(HitSound("体当たり", "突", targetIsHero: true), "Crash.wav");
            AssertWaves(HitSound("背負い投げ", "突", targetIsHero: true), "Swing.wav", "Shock(Low).wav");
            AssertWaves(HitSound("関節技", "突", targetIsHero: true), "Swing.wav", "BreakOff.wav");
        }

        [TestMethod]
        public void HitSound_PhysicalShots()
        {
            // 実弾のミサイルは回数分の爆発音、核は核爆発音
            AssertWaves(HitSound("ミサイルランチャー", "実", targetIsHero: false),
                "Explode(Small).wav", "Explode(Small).wav", "Explode(Small).wav", "Explode(Small).wav");
            AssertWaves(HitSound("反応弾", "実", targetIsHero: false), "Explode(Nuclear).wav");
            // 正体不明の実弾は、相手が人間なら無音、メカなら爆発音
            AssertWaves(HitSound("ふしぎな弾", "実", targetIsHero: true));
            AssertWaves(HitSound("ふしぎな弾", "実", targetIsHero: false), "Explode(Small).wav");
        }

        [TestMethod]
        public void HitSound_NonPhysicalAttacks()
        {
            // 武・突・接・実のいずれでもない攻撃
            AssertWaves(HitSound("ハリケーン", "", targetIsHero: false));
            AssertWaves(HitSound("溶解液", "", targetIsHero: false), "Inori.wav");
            AssertWaves(HitSound("発火能力", "", targetIsHero: false), "Fire.wav");
            AssertWaves(HitSound("テレキネシス", "", targetIsHero: false), "Crash.wav");
            AssertWaves(HitSound("エナジー吸収", "", targetIsHero: false), "Charge.wav");
            AssertWaves(HitSound("ビームライフル", "Ｂ", targetIsHero: false), "Explode(Small).wav");
            AssertWaves(HitSound("ビームライフル", "Ｂ", targetIsHero: true));
        }

        // ──────────────────────────────────────────────
        // PrepareWeaponSound / PrepareWeaponEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PrepareWeaponSound_BeamBlade_PlaysSaberHum()
        {
            // ビーム系・高周波系の格闘武器を構えると「ブーン」という音
            foreach (var name in new[] { "ビームサーベル", "高周波ブレード", "セイバー" })
            {
                using (var env = new EffectTestEnv(false))
                {
                    var w = env.CreateWeapon(name, "武");
                    env.Src.Effect.PrepareWeaponSound(w.Unit, w);
                    AssertWaves(env.PlayedWaves(), "BeamSaber.wav");
                    Assert.IsFalse(env.Src.Sound.IsWavePlayed);
                }
            }
        }

        [TestMethod]
        public void PrepareWeaponSound_OtherWeapons_PlayNothing()
        {
            // 実体剣や射撃武器では鳴らさない
            using (var env = new EffectTestEnv(false))
            {
                var sword = env.CreateWeapon("ロングソード", "武");
                env.Src.Effect.PrepareWeaponSound(sword.Unit, sword);
                var beam = env.CreateWeapon("ビームライフル", "Ｂ");
                env.Src.Effect.PrepareWeaponSound(beam.Unit, beam);
                AssertWaves(env.PlayedWaves());
            }
        }

        [TestMethod]
        public void PrepareWeaponEffect_AnimationOff_UsesSound()
        {
            // 戦闘アニメオフなら PrepareWeaponEffect は効果音側を使う
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ビームサーベル", "武");
                env.Src.Effect.PrepareWeaponEffect(w.Unit, w);
                AssertWaves(env.PlayedWaves(), "BeamSaber.wav");
            }
        }

        [TestMethod]
        public void AttackAndHitEffect_AnimationOff_UseSound()
        {
            // 戦闘アニメオフなら AttackEffect / HitEffect は効果音側を使う
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.AttackEffect(w.Unit, w);
                env.Src.Effect.HitEffect(w.Unit, w, t.Unit);
                AssertWaves(env.PlayedWaves(), "Missile.wav", "Explode(Small).wav");
            }
        }

        // ──────────────────────────────────────────────
        // DodgeEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DodgeEffect_InfightWeapon_PlaysSwing()
        {
            // 格闘武器を回避されたら風切り音
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ロングソード", "武");
                env.Src.Effect.DodgeEffect(w.Unit, w);
                AssertWaves(env.PlayedWaves(), "Swing.wav");
            }
        }

        [TestMethod]
        public void DodgeEffect_PhysicalWhip_PlaysSwing_ButMissileDoesNot()
        {
            // 実弾の中でも鞭・触手など振り回すものだけ風切り音
            using (var env = new EffectTestEnv(false))
            {
                var whip = env.CreateWeapon("テイルウイップ", "実");
                env.Src.Effect.DodgeEffect(whip.Unit, whip);
                var missile = env.CreateWeapon("ミサイル", "実");
                env.Src.Effect.DodgeEffect(missile.Unit, missile);
                AssertWaves(env.PlayedWaves(), "Swing.wav");
            }
        }

        [TestMethod]
        public void DodgeEffect_AnimationOn_PlaysNothing()
        {
            // 戦闘アニメオンの場合は回避時の風切り音を鳴らさない
            using (var env = new EffectTestEnv(true))
            {
                var w = env.CreateWeapon("ロングソード", "武");
                env.Src.Effect.DodgeEffect(w.Unit, w);
                AssertWaves(env.PlayedWaves());
            }
        }

        // ──────────────────────────────────────────────
        // ParryEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ParryEffect_Beam_SaberThenBeamCoat()
        {
            // 切り払い音の後に、Ｂ属性ならビームコート音
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ビームライフル", "Ｂ");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.ParryEffect(w.Unit, w, t.Unit);
                AssertWaves(env.PlayedWaves(), "Saber.wav", "BeamCoat.wav");
                CollectionAssert.AreEqual(new[] { 100 }, env.Sleeps());
            }
        }

        [TestMethod]
        public void ParryEffect_MachineGun_RepeatsFourTimes()
        {
            // マシンガン類は４回切り払う。実弾は小爆発音
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("マシンガン", "実");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.ParryEffect(w.Unit, w, t.Unit);
                var pair = new[] { "Saber.wav", "Explode(Small).wav" };
                AssertWaves(env.PlayedWaves(), Enumerable.Repeat(pair, 4).SelectMany(x => x).ToArray());
                CollectionAssert.AreEqual(new[] { 100, 130, 100, 130, 100, 130, 100 }, env.Sleeps());
            }
        }

        [TestMethod]
        public void ParryEffect_ArrowAndBlade_PlaySword()
        {
            // 弓矢や刀剣を切り払った場合は剣戟音
            using (var env = new EffectTestEnv(false, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ロングボウ", "実");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.ParryEffect(w.Unit, w, t.Unit);
                AssertWaves(env.PlayedWaves(), "Saber.wav", "Sword.wav");
            }
        }

        // ──────────────────────────────────────────────
        // AbsorbEffect（戦闘アニメオフ）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbsorbEffect_AnimationOff_PlaysCharge()
        {
            // 戦闘アニメオフの場合、吸収時は効果音のみ
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ビームライフル", "Ｂ");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.AbsorbEffect(w.Unit, w, t.Unit);
                AssertWaves(env.PlayedWaves(), "Charge.wav");
            }
        }
    }
}
