using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.Effects
{
    /// <summary>
    /// Effect の戦闘アニメ自動選択（戦闘アニメオン時）のテスト。
    /// VB Effect.bas の ShowAnimation / PrepareWeaponAnimation / AttackAnimation / HitAnimation /
    /// ShieldEffect / AbsorbEffect / CriticalEffect / NegateEffect と、
    /// 武器の種類・色を決める CheckWeaponType / IsBeamWeapon / SpellColor の規則を確認する。
    /// 呼ばれたアニメは「戦闘アニメ_xxx」サブルーチンに渡された名前と引数で確認する。
    /// </summary>
    [TestClass]
    public class EffectAnimationTests
    {
        private const int NormalWait = 700;

        private static void AssertAnimations(EffectTestEnv env, params string[] expected)
        {
            var actual = env.Animations();
            CollectionAssert.AreEqual(expected, actual, "実際: " + string.Join(" / ", actual));
        }

        /// <summary>
        /// 武器準備・攻撃・命中のいずれかのアニメを、期待するアニメのラベルを定義した環境で実行する。
        /// </summary>
        private static List<string> Run(string wname, string wclass, Action<EffectTestEnv, Units.UnitWeapon> act,
            string[] labels, string[] options = null, string unitClass = "汎用", string transportation = "陸")
        {
            using (var env = new EffectTestEnv(true, labels, messageWait: NormalWait, options: options))
            {
                var w = env.CreateWeapon(wname, wclass, unitClass: unitClass, transportation: transportation);
                act(env, w);
                return env.Animations();
            }
        }

        private static List<string> Prepare(string wname, string wclass, params string[] labels)
            => Run(wname, wclass, (env, w) => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w), labels);

        private static List<string> Attack(string wname, string wclass, params string[] labels)
            => Run(wname, wclass, (env, w) => env.Src.Effect.AttackAnimation(w.Unit, w), labels);

        private static List<string> Hit(string wname, string wclass, int hitCount, params string[] labels)
            => Run(wname, wclass, (env, w) =>
            {
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.HitAnimation(w.Unit, w, t.Unit, hitCount);
            }, labels);

        private static void AssertList(List<string> actual, params string[] expected)
        {
            CollectionAssert.AreEqual(expected, actual, "実際: " + string.Join(" / ", actual));
        }

        // ──────────────────────────────────────────────
        // ShowAnimation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowAnimation_CallsSubroutineWithArguments()
        {
            // 先頭の要素に「戦闘アニメ_」を付けたサブルーチンを、残りの要素を引数として呼ぶ
            using (var env = new EffectTestEnv(true, new[] { "爆発命中" }))
            {
                env.Src.Effect.ShowAnimation("爆発命中 赤 Explode.wav");
                AssertAnimations(env, "爆発命中 赤 Explode.wav");
            }
        }

        [TestMethod]
        public void ShowAnimation_PrefixAlreadyGiven_IsNotDoubled()
        {
            // 既に「戦闘アニメ_」で始まる名前はそのまま使う
            using (var env = new EffectTestEnv(true, new[] { "爆発命中" }))
            {
                env.Src.Effect.ShowAnimation("戦闘アニメ_爆発命中 青");
                AssertAnimations(env, "爆発命中 青");
            }
        }

        [TestMethod]
        public void ShowAnimation_AnimationOff_DoesNothing()
        {
            // 戦闘アニメオフの場合は何もしない
            using (var env = new EffectTestEnv(false, new[] { "爆発命中" }))
            {
                env.Src.Effect.ShowAnimation("爆発命中");
                AssertAnimations(env);
            }
        }

        [TestMethod]
        public void ShowAnimation_RightButtonPressed_Skips()
        {
            // 右クリック中は特殊効果をスキップ
            using (var env = new EffectTestEnv(true, new[] { "爆発命中" }))
            {
                env.Gui.Handle("IsRButtonPressed", _ => true);
                env.Src.Effect.ShowAnimation("爆発命中");
                AssertAnimations(env);
            }
        }

        [TestMethod]
        public void ShowAnimation_PictureDrawn_ClearedUnlessKeep()
        {
            // 戦闘アニメ中に画像が描画された場合、戻り値が keep でなければ画像を消去する
            using (var env = new EffectTestEnv(true, new[] { "爆発命中", "残像" }, keepAnimations: new[] { "残像" }))
            {
                env.Gui.Handle("get_IsPictureDrawn", _ => true);

                env.Src.Effect.ShowAnimation("残像");
                Assert.AreEqual(0, env.Gui.CallsOf("ClearPicture").Count());

                env.Src.Effect.ShowAnimation("爆発命中");
                // VB: ClearPicture の後に picMain(0).Refresh で画面を更新する
                var members = env.Gui.Calls.Select(x => x.Member).ToList();
                var clear = members.IndexOf("ClearPicture");
                Assert.IsTrue(clear >= 0, "ClearPicture が呼ばれていない");
                Assert.AreEqual(1, members.Count(x => x == "ClearPicture"));
                Assert.IsTrue(members.Skip(clear + 1).Contains("UpdateScreen"), "消去後に画面が更新されていない");
            }
        }

        [TestMethod]
        public void ShowAnimation_UndefinedSubroutine_ShowsErrorMessage()
        {
            // VB: FindNormalLabel(expr) = 0 なら「サブルーチン「…」が見つかりません」を表示して戻る
            using (var env = new EffectTestEnv(true))
            {
                env.Src.Effect.ShowAnimation("存在しないアニメ 赤");
                CollectionAssert.AreEqual(
                    new[] { "サブルーチン「戦闘アニメ_存在しないアニメ」が見つかりません" },
                    env.Gui.CallsOf("ErrorMessage").Select(x => (string)x.Args[0]).ToList());
            }
        }

        // ──────────────────────────────────────────────
        // PrepareWeaponAnimation（CheckWeaponType）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Prepare_BeamSaber_ColorByName()
        {
            // ビームサーベル系の準備アニメは武器名から刃の色を決める
            AssertList(Prepare("ビームサーベル", "武", "ビームサーベル準備"), "ビームサーベル準備 ピンク");
            AssertList(Prepare("ライトセイバー", "武", "ライトセイバー準備"), "ライトセイバー準備 イエロー");
            AssertList(Prepare("ハイパービームサーベル", "武", "ハイパービームサーベル準備"), "ハイパービームサーベル準備 ピンク");
        }

        [TestMethod]
        public void Prepare_DoubleWeapon_AddsTwoSwordStyle()
        {
            // ダブル・ツイン等は二刀流
            AssertList(Prepare("ダブルビームサーベル", "武", "ビームサーベル準備"), "ビームサーベル準備 ピンク 二刀流");
        }

        [TestMethod]
        public void Prepare_HighFrequencyBlade_AddsHumSound()
        {
            // 高周波・電磁の武器は「ブーン」という効果音を付ける
            AssertList(Prepare("高周波ナイフ", "武", "ナイフ準備"), "ナイフ準備 BeamSaber.wav");
        }

        [TestMethod]
        public void Prepare_MeleeWeaponTypes()
        {
            // 武器名から武器の種類を判定する
            AssertList(Prepare("ロングソード", "武", "ソード準備"), "ソード準備");
            AssertList(Prepare("ショートソード", "武", "ショートソード準備"), "ショートソード準備");
            AssertList(Prepare("黒の剣", "武", "黒剣準備"), "黒剣準備");
            // 投げる短剣（実属性）は投げナイフ
            AssertList(Prepare("投げナイフ", "実", "投げナイフ準備"), "投げナイフ準備");
            // 種類の分からない武器は準備アニメなし
            AssertList(Prepare("ブラックソード", "武", "ソード準備"));
        }

        [TestMethod]
        public void Prepare_Shuriken_IsNotPrepared()
        {
            // 手裏剣は構えずにいきなり投げる
            AssertList(Prepare("手裏剣", "実", "手裏剣準備"));
        }

        [TestMethod]
        public void Prepare_HandheldBeamWeapons()
        {
            // 手持ちのビーム兵器
            AssertList(Prepare("ビームライフル", "Ｂ", "ビームライフル準備"), "ビームライフル準備");
            // メガ～ライフルはバスタービームライフル
            AssertList(Prepare("メガビームライフル", "Ｂ", "バスタービームライフル準備"), "バスタービームライフル準備");
            // マップ攻撃
            AssertList(Prepare("ビームライフル", "ＢＭ", "ＭＡＰバスタービームライフル準備"), "ＭＡＰバスタービームライフル準備");
            // 小型のものはレーザーガン
            AssertList(Prepare("ミニビームガン", "Ｂ", "レーザーガン準備"), "レーザーガン準備");
        }

        [TestMethod]
        public void Prepare_HandheldGuns()
        {
            AssertList(Prepare("ヘビーマシンガン", "実", "ヘビーマシンガン準備"), "ヘビーマシンガン準備");
            AssertList(Prepare("バズーカ", "実", "バズーカ準備"), "バズーカ準備");
            AssertList(Prepare("ショットガン", "実", "ショットガン準備"), "ショットガン準備");
        }

        [TestMethod]
        public void Prepare_SkippedConditions()
        {
            // 空中移動専用形態は武器を手で構えない
            AssertList(Run("ロングソード", "武", (env, w) => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w),
                new[] { "ソード準備" }, transportation: "空"));
            // 等身大基準では非人間ユニットは内蔵武器を優先する
            AssertList(Run("ロングソード", "武", (env, w) => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w),
                new[] { "ソード準備" }, options: new[] { "等身大基準" }));
            // 人間ユニットなら等身大基準でも構える
            AssertList(Run("ロングソード", "武", (env, w) => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w),
                new[] { "ソード準備" }, options: new[] { "等身大基準" }, unitClass: "(人間)"), "ソード準備");
            // 武器準備アニメ非表示・戦闘アニメ非自動選択オプション
            AssertList(Run("ロングソード", "武", (env, w) => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w),
                new[] { "ソード準備" }, options: new[] { "武器準備アニメ非表示" }));
            AssertList(Run("ロングソード", "武", (env, w) => env.Src.Effect.PrepareWeaponAnimation(w.Unit, w),
                new[] { "ソード準備" }, options: new[] { "戦闘アニメ非自動選択" }));
        }

        [TestMethod]
        public void PrepareWeaponEffect_AnimationOn_UsesAnimation()
        {
            // 戦闘アニメオンなら PrepareWeaponEffect はアニメ側を使い、効果音は直接鳴らさない
            using (var env = new EffectTestEnv(true, new[] { "ビームサーベル準備" }))
            {
                var w = env.CreateWeapon("ビームサーベル", "武");
                env.Src.Effect.PrepareWeaponEffect(w.Unit, w);
                AssertAnimations(env, "ビームサーベル準備 ピンク");
                Assert.AreEqual(0, env.PlayedWaves().Count);
            }
        }

        // ──────────────────────────────────────────────
        // AttackAnimation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Attack_NonAutoSelectOption_UsesDefault()
        {
            // 戦闘アニメ非自動選択ならデフォルト攻撃
            AssertList(Run("ビームライフル", "Ｂ", (env, w) => env.Src.Effect.AttackAnimation(w.Unit, w),
                new[] { "デフォルト攻撃" }, options: new[] { "戦闘アニメ非自動選択" }), "デフォルト攻撃");
        }

        [TestMethod]
        public void Attack_Missiles_WithShotCount()
        {
            // ミサイルランチャーは小型ミサイル６発、大型ミサイルは１発
            AssertList(Attack("ミサイルランチャー", "実", "小型ミサイル攻撃"), "小型ミサイル攻撃 6");
            AssertList(Attack("大型ミサイル", "実", "スーパーミサイル攻撃"), "スーパーミサイル攻撃 1");
            // 名前の連数を発射数とする
            AssertList(Attack("３連ミサイル", "実", "ミサイル攻撃"), "ミサイル攻撃 3");
        }

        [TestMethod]
        public void Attack_BeamWeapons()
        {
            // Ｂ属性の射撃。色名を含む場合は色を指定
            AssertList(Attack("赤いビーム", "Ｂ", "小ビーム攻撃"), "小ビーム攻撃 赤");
            AssertList(Attack("拡散ビーム砲", "Ｂ", "拡散ビーム攻撃"), "拡散ビーム攻撃");
            // メガ～ライフルはビームランチャー（バスターライフルの画像を使用）
            AssertList(Attack("メガビームライフル", "Ｂ", "ビームランチャー攻撃"),
                @"ビームランチャー攻撃 Weapon\EFFECT_BusterRifle01.bmp");
        }

        [TestMethod]
        public void Attack_ElementalAttacks()
        {
            // ブレスは火炎放射。色は SpellColor（炎→赤）で決まる
            AssertList(Attack("炎のブレス", "", "火炎放射攻撃"), "火炎放射攻撃 赤 Breath.wav");
            // 毒ガスは緑色の煙
            AssertList(Attack("毒ガス", "", "煙攻撃"), "煙攻撃 緑");
            // 衝撃波は白い波動放射
            AssertList(Attack("衝撃波", "", "波動放射攻撃"), "波動放射攻撃 白 Bazooka.wav");
            // 魔属性はデフォルト攻撃に魔法音
            AssertList(Attack("ファイヤーボール", "魔", "デフォルト攻撃"), "デフォルト攻撃 Whiz.wav");
        }

        [TestMethod]
        public void Attack_MeleeWeapons()
        {
            AssertList(Attack("パンチ", "突", "打突攻撃"), "打突攻撃");
            // 連属性の打撃は連打
            AssertList(Attack("連続パンチ", "突連", "連打攻撃"), "連打攻撃");
            // 色名（ブラック）は黒
            AssertList(Attack("ブラックソード", "武", "白兵武器攻撃"), "白兵武器攻撃 黒");
        }

        [TestMethod]
        public void Attack_Charge_UsesWeaponInHand()
        {
            // 突撃系は構えている武器（武器準備アニメで記録）を使った突撃になる
            using (var env = new EffectTestEnv(true, new[] { "ソード準備", "ソード突撃攻撃" }, messageWait: NormalWait))
            {
                var sword = env.CreateWeapon("ロングソード", "武");
                env.Src.Effect.PrepareWeaponAnimation(sword.Unit, sword);
                var charge = env.CreateWeapon("突撃", "武");
                env.Src.Effect.AttackAnimation(charge.Unit, charge);
                AssertAnimations(env, "ソード準備", "ソード突撃攻撃");
            }
        }

        [TestMethod]
        public void Attack_BeamColorFromIsBeamWeapon()
        {
            // VB: IsBeamWeapon(wname, wclass, cname) の cname は参照渡しで、熱線はレッド、プラズマはオレンジ
            AssertList(Attack("熱線砲", "", "中ビーム攻撃"), "中ビーム攻撃 レッド");
            AssertList(Attack("プラズマキャノン", "", "中ビーム攻撃"), "中ビーム攻撃 オレンジ");
            AssertList(Hit("熱線砲", "", 0, "中ビーム命中"), "中ビーム命中 レッド");
        }

        // ──────────────────────────────────────────────
        // HitAnimation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Hit_Explosions_WithHitCount()
        {
            // 複数発のミサイルは連続爆発、１発なら回数を付けない
            AssertList(Hit("ミサイルランチャー", "実", 0, "連続爆発命中"), "連続爆発命中 4");
            AssertList(Hit("大型ミサイル", "実", 0, "大爆発命中"), "大爆発命中");
            // 連属性の武器は命中数を爆発数とする
            AssertList(Hit("ミサイル", "実連L3", 2, "連続爆発命中"), "連続爆発命中 2");
            AssertList(Hit("ミサイル", "実連L3", 1, "爆発命中"), "爆発命中");
        }

        [TestMethod]
        public void Hit_BeamAndMelee()
        {
            AssertList(Hit("赤いビーム", "Ｂ", 0, "小ビーム命中"), "小ビーム命中 赤");
            AssertList(Hit("ロングソード", "武", 0, "斬撃命中"), "斬撃命中");
            AssertList(Hit("黒の剣", "武", 0, "黒斬撃命中"), "黒斬撃命中 黒");
            AssertList(Hit("キック", "突", 0, "強打命中"), "強打命中");
        }

        [TestMethod]
        public void Hit_KnockBack_DowngradesStrongBlow()
        {
            // 吹き飛ばし時はアニメの不整合を防ぐため強打を打撃に抑える
            AssertList(Hit("キック", "突吹", 0, "打撃命中"), "打撃命中");
        }

        [TestMethod]
        public void HitEffect_AnimationOn_UsesAnimation()
        {
            // 戦闘アニメオンなら HitEffect / AttackEffect はアニメ側を使う
            using (var env = new EffectTestEnv(true, new[] { "スーパーミサイル攻撃", "大爆発命中" }, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("大型ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.AttackEffect(w.Unit, w);
                env.Src.Effect.HitEffect(w.Unit, w, t.Unit);
                AssertAnimations(env, "スーパーミサイル攻撃 1", "大爆発命中");
            }
        }

        // ──────────────────────────────────────────────
        // ShieldEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShieldEffect_ByShieldFeature()
        {
            // シールドの種類によってアニメ（とシールドの大きさ）を変える
            var cases = new[]
            {
                ("エネルギーシールド", "ビームシールド発動"),
                ("小型シールド", "シールド防御発動 28"),
                ("大型シールド", "シールド防御発動 40"),
                ("シールド", "シールド防御発動"),
            };
            foreach (var (feature, expected) in cases)
            {
                using (var env = new EffectTestEnv(true, new[] { "ビームシールド発動", "シールド防御発動" }))
                {
                    var w = env.CreateWeapon("ダミー", "", feature: feature);
                    env.Src.Effect.ShieldEffect(w.Unit);
                    AssertAnimations(env, expected);
                }
            }
        }

        // ──────────────────────────────────────────────
        // AbsorbEffect（SpellColor）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbsorbEffect_ColorBySpellColor()
        {
            // 吸収時は粒子集中アニメ。色は武器名・属性の漢字から決める
            var cases = new[]
            {
                ("炎の矢", "", "赤"),
                ("アクアショット", "", "青"),
                ("ポイズンニードル", "", "緑"),
                ("ふしぎな弾", "闇", "黒"),
                ("ストーンブラスト", "", "黄"),
                ("聖なる光", "", "白"),
                ("サンライト", "", "橙"),
            };
            foreach (var (wname, wclass, color) in cases)
            {
                using (var env = new EffectTestEnv(true, new[] { "粒子集中発動" }))
                {
                    var w = env.CreateWeapon(wname, wclass);
                    var t = env.CreateWeapon("ダミー", "");
                    env.Src.Effect.AbsorbEffect(w.Unit, w, t.Unit);
                    AssertAnimations(env, "粒子集中発動 " + color);
                }
            }
        }

        [TestMethod]
        public void AbsorbEffect_ColorFallsBackToBeamColor()
        {
            // VB: SpellColor で色が決まらなければ IsBeamWeapon で決めた色（プラズマ→オレンジ）を使う
            using (var env = new EffectTestEnv(true, new[] { "粒子集中発動" }))
            {
                var w = env.CreateWeapon("プラズマキャノン", "");
                var t = env.CreateWeapon("ダミー", "");
                env.Src.Effect.AbsorbEffect(w.Unit, w, t.Unit);
                AssertAnimations(env, "粒子集中発動 オレンジ");
            }
        }

        // ──────────────────────────────────────────────
        // CriticalEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CriticalEffect_EmptyType_ShowsDefault()
        {
            using (var env = new EffectTestEnv(true, new[] { "デフォルトクリティカル" }))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                env.Src.Effect.CriticalEffect("", w, false);
                AssertAnimations(env, "デフォルトクリティカル");
            }
        }

        [TestMethod]
        public void CriticalEffect_EachConditionInOrder_IgnoringDeathWhenRequested()
        {
            // 状態ごとに「状態名クリティカル」を表示する。ignore_death なら即死は表示しない
            using (var env = new EffectTestEnv(true, new[] { "毒クリティカル", "即死クリティカル", "麻痺クリティカル" }))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                env.Src.Effect.CriticalEffect("毒 即死 麻痺", w, true);
                AssertAnimations(env, "毒クリティカル", "麻痺クリティカル");
            }
            using (var env = new EffectTestEnv(true, new[] { "毒クリティカル", "即死クリティカル" }))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                env.Src.Effect.CriticalEffect("即死", w, false);
                AssertAnimations(env, "即死クリティカル");
            }
        }

        [TestMethod]
        public void CriticalEffect_ShockByColdWeapon_MutesSound()
        {
            // 冷気による攻撃で行動不能になった場合はショックの効果音をオフにする
            using (var env = new EffectTestEnv(true, new[] { "ショッククリティカル" }))
            {
                var cold = env.CreateWeapon("フリーズビーム", "Ｂ冷");
                env.Src.Effect.CriticalEffect("ショック", cold, false);
                var normal = env.CreateWeapon("スタンガン", "実");
                env.Src.Effect.CriticalEffect("ショック", normal, false);
                AssertAnimations(env, "ショッククリティカル -.wav", "ショッククリティカル");
            }
        }

        [TestMethod]
        public void CriticalEffect_UndefinedAnimation_IsSkipped()
        {
            // VB: FindNormalLabel("戦闘アニメ_" & aname) = 0 なら表示しない
            using (var env = new EffectTestEnv(true, new[] { "毒クリティカル" }))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                env.Src.Effect.CriticalEffect("毒 麻痺", w, false);
                AssertAnimations(env, "毒クリティカル");
                Assert.AreEqual(0, env.Gui.CallsOf("ErrorMessage").Count());
            }
        }

        // ──────────────────────────────────────────────
        // NegateEffect
        // ──────────────────────────────────────────────

        private static List<string> SysMessages(EffectTestEnv env)
            => env.Gui.CallsOf("DisplaySysMessage").Select(x => (string)x.Args[0]).ToList();

        [TestMethod]
        public void NegateEffect_BeamNegated_ShowsBeamCoatAndMessage()
        {
            // ビーム無効化（Ｂ指定）: ビームコート発動アニメと「～の[能力]が攻撃を防いだ。」
            using (var env = new EffectTestEnv(true, new[] { "ビームコート発動" }))
            {
                var w = env.CreateWeapon("ビームライフル", "Ｂ");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "Ｉフィールド", "Ｂ", 0, "", true);
                AssertAnimations(env, "ビームコート発動 - Ｉフィールド");
                CollectionAssert.AreEqual(new[] { "守備機の[Ｉフィールド]が攻撃を防いだ。" }, SysMessages(env));
            }
        }

        [TestMethod]
        public void NegateEffect_BeamAbsorbed_AnimationOff_PlaysChargeAndMessage()
        {
            // ダメージが負（吸収）の場合は吸収の特殊効果。能力名が無ければ「～が攻撃を吸収した。」
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ビームライフル", "Ｂ");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, -100, "", "Ｂ", 0, "", true);
                CollectionAssert.AreEqual(new[] { "Charge.wav" }, env.PlayedWaves());
                CollectionAssert.AreEqual(new[] { "守備機が攻撃を吸収した。" }, SysMessages(env));
            }
        }

        [TestMethod]
        public void NegateEffect_BeamNegated_AnimationOff_PlaysBeamCoat()
        {
            using (var env = new EffectTestEnv(false))
            {
                var w = env.CreateWeapon("ビームライフル", "Ｂ");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "", "Ｂ", 0, "メッセージ;", true);
                CollectionAssert.AreEqual(new[] { "BeamCoat.wav" }, env.PlayedWaves());
                CollectionAssert.AreEqual(new[] { "メッセージ;守備機が攻撃を防いだ。" }, SysMessages(env));
            }
        }

        [TestMethod]
        public void NegateEffect_Barrier_ShowsBarrierAnimation()
        {
            // 攻撃無効化（バリア）: 能力名に応じたバリア発動アニメ
            using (var env = new EffectTestEnv(true, new[] { "バリア発動" }))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "バリア", "1000", 10, "", true);
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "", "1000", 10, "", true);
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "フィールド", "1000", 10, "", true);
                AssertAnimations(env, "バリア発動", "バリア発動 - 攻撃無効化", "バリア発動 - フィールド");
                CollectionAssert.AreEqual(new[]
                {
                    "守備機の[バリア]が攻撃を防いだ。",
                    "守備機は攻撃を防いだ。",
                    "守備機の[フィールド]が攻撃を防いだ。",
                }, SysMessages(env));
            }
        }

        [TestMethod]
        public void NegateEffect_BarrierWithoutAnimation_FallsBackToHitEffect()
        {
            // バリア無効化無効で EN 消費が無い場合はバリアのアニメを出さず、通常の命中時の特殊効果を表示する
            using (var env = new EffectTestEnv(true, new[] { "バリア発動", "爆発命中" }, messageWait: NormalWait))
            {
                var w = env.CreateWeapon("ミサイル", "実");
                var t = env.CreateWeapon("ダミー", "", nickname: "守備機");
                env.Src.Effect.NegateEffect(w.Unit, t.Unit, w, w.Name, 0, "フィールド", "1000 0 バリア無効化無効", 0, "", true);
                AssertAnimations(env, "爆発命中");
            }
        }
    }
}
