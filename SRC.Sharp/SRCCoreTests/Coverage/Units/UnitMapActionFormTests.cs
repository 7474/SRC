using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.map.cs（Transform / Combine / Split）のテスト。
    /// 元の SRC の Unit.cls の Transform / Combine / Split を仕様とする。
    /// </summary>
    [TestClass]
    public class UnitMapActionFormTests
    {
        // ──────────────────────────────────────────────
        // Transform
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Transform_HyperModeRelease_OnBattleMap_AddsExhaustionAndActionLimit()
        {
            // ハイパーモードが解ける（ノーマルモードの形態へ戻る）と消耗状態になる。
            // 「回数制限」指定ならハイパーモードの形態は以後行動不能
            var f = new UnitMapActionFixture();
            f.SetBattleMap();
            f.AddUnitData("通常形態", features: "ハイパーモード=ハイパーモード 超形態");
            f.AddUnitData("超形態", features: "ノーマルモード=通常形態 回数制限");
            var normal = f.CreateUnit("通常形態", "味方", f.CreatePilot("パイロット"));
            var hyper = normal.OtherForm("超形態");
            normal.Transform("超形態");
            Assert.AreEqual("待機", hyper.Status);

            hyper.Transform("通常形態");

            Assert.AreEqual("待機", normal.Status);
            Assert.AreEqual("他形態", hyper.Status);
            Assert.IsTrue(normal.IsConditionSatisfied("消耗"));
            Assert.AreEqual(1, normal.ConditionLifetime("消耗"));
            Assert.IsTrue(hyper.IsConditionSatisfied("行動不能"));
        }

        [TestMethod]
        public void Transform_HyperModeRelease_NoExhaustionOption()
        {
            // 「消耗なし」指定なら消耗状態にならない
            var f = new UnitMapActionFixture();
            f.SetBattleMap();
            f.AddUnitData("通常形態", features: "ハイパーモード=ハイパーモード 超形態");
            f.AddUnitData("超形態", features: "ノーマルモード=通常形態 消耗なし");
            var normal = f.CreateUnit("通常形態", "味方", f.CreatePilot("パイロット"));
            var hyper = normal.OtherForm("超形態");
            normal.Transform("超形態");

            hyper.Transform("通常形態");

            Assert.IsFalse(normal.IsConditionSatisfied("消耗"));
            Assert.IsFalse(hyper.IsConditionSatisfied("行動不能"));
        }

        [TestMethod]
        public void Transform_HyperModeRelease_InStatusView_HasNoPenalty()
        {
            // ユニットステータス表示（マップなし）での形態切り替えでは消耗も行動不能も付かない
            var f = new UnitMapActionFixture();
            f.AddUnitData("通常形態", features: "ハイパーモード=ハイパーモード 超形態");
            f.AddUnitData("超形態", features: "ノーマルモード=通常形態 回数制限");
            var normal = f.CreateUnit("通常形態", "味方", f.CreatePilot("パイロット"));
            var hyper = normal.OtherForm("超形態");
            normal.Transform("超形態");

            hyper.Transform("通常形態");

            Assert.IsFalse(normal.IsConditionSatisfied("消耗"));
            Assert.IsFalse(hyper.IsConditionSatisfied("行動不能"));
        }

        [TestMethod]
        public void Transform_ToNormalModeForm_AddsRemainingTime_AndRemovesItOnRelease()
        {
            // ノーマルモードの第2引数が数値なら残り時間を付加し、元に戻るときに解除する
            var f = new UnitMapActionFixture();
            f.AddUnitData("通常形態", features: "ハイパーモード=ハイパーモード 超形態");
            f.AddUnitData("超形態", features: "ノーマルモード=通常形態 3");
            var normal = f.CreateUnit("通常形態", "味方", f.CreatePilot("パイロット"));
            var hyper = normal.OtherForm("超形態");

            normal.Transform("超形態");
            Assert.IsTrue(hyper.IsConditionSatisfied("残り時間"));
            Assert.AreEqual(3, hyper.ConditionLifetime("残り時間"));

            hyper.Transform("通常形態");
            Assert.IsFalse(normal.IsConditionSatisfied("残り時間"));
        }

        [TestMethod]
        public void Transform_ToTimeLimitedForm_AddsRemainingTime()
        {
            // 制限時間を持つ形態へ変形すると残り時間が付く
            var f = new UnitMapActionFixture();
            f.AddUnitData("形態A", features: "変形=変形 形態B");
            f.AddUnitData("形態B", features: new[] { "変形=変形 形態A", "制限時間=2" });
            var a = f.CreateUnit("形態A", "味方", f.CreatePilot("パイロット"));

            a.Transform("形態B");

            var b = a.OtherForm("形態B");
            Assert.AreEqual(2, b.ConditionLifetime("残り時間"));
        }

        [TestMethod]
        public void Transform_TransformationRelease_LowersMorale_AndClearsCopyConditions()
        {
            // ノーマルモード付加（変身）が解ける場合、マップ上なら「気力低下」で気力が10下がる。
            // 能力コピーによる状態（パイロット画像・メッセージ）も解除される
            var f = new UnitMapActionFixture();
            f.SetBattleMap();
            f.AddUnitData("通常形態", features: "変形=変形 変身形態");
            f.AddUnitData("変身形態", features: "ノーマルモード=通常形態 気力低下");
            var pilot = f.CreatePilot("パイロット");
            pilot.Morale = 120;
            var normal = f.CreateUnit("通常形態", "味方", pilot);
            normal.Transform("変身形態");
            var transformed = normal.OtherForm("変身形態");
            transformed.AddCondition("ノーマルモード付加", -1, cdata: "");
            transformed.AddCondition("能力コピー", -1, cdata: "");
            transformed.AddCondition("パイロット画像", -1, cdata: "");
            transformed.AddCondition("メッセージ", -1, cdata: "");

            transformed.Transform("通常形態");

            Assert.AreEqual(110, pilot.Morale);
            Assert.AreSame(normal, pilot.Unit);
            Assert.IsFalse(normal.IsConditionSatisfied("ノーマルモード付加"));
            Assert.IsFalse(normal.IsConditionSatisfied("能力コピー"));
            Assert.IsFalse(normal.IsConditionSatisfied("パイロット画像"));
            Assert.IsFalse(normal.IsConditionSatisfied("メッセージ"));
            // 変身の解除では消耗しない（「消耗あり」指定時のみ）
            Assert.IsFalse(normal.IsConditionSatisfied("消耗"));
        }

        [TestMethod]
        public void Transform_CarriesBulletRatioWithIntegerDivision()
        {
            // 同名武器の弾数は「残弾 × 変形後の最大弾数 \ 変形前の最大弾数」で受け継ぐ（切り捨て）
            var f = new UnitMapActionFixture();
            var ua = f.AddUnitData("形態A", features: "変形=変形 形態B");
            ua.AddWeapon("ミサイル").Bullet = 10;
            var ub = f.AddUnitData("形態B", features: "変形=変形 形態A");
            ub.AddWeapon("ミサイル").Bullet = 3;
            ub.AddWeapon("ビーム").Bullet = 4;
            var a = f.CreateUnit("形態A", "味方", f.CreatePilot("パイロット"));
            a.Weapons.First().SetBullet(5);

            a.Transform("形態B");

            var b = a.OtherForm("形態B");
            Assert.AreEqual(1, b.Weapons.Single(w => w.Name == "ミサイル").Bullet());
            // 変形前に無い武器は満タンのまま
            Assert.AreEqual(4, b.Weapons.Single(w => w.Name == "ビーム").Bullet());
        }

        [TestMethod]
        public void Transform_Deployed_PlacesNewFormAtSamePosition()
        {
            // 出撃中に変形すると変形後のユニットが同じ位置に出撃し、行動回数を受け継ぐ
            var f = new UnitMapActionFixture();
            f.AddUnitData("形態A", features: "変形=変形 形態B");
            f.AddUnitData("形態B", features: "変形=変形 形態A");
            var pilot = f.CreatePilot("パイロット");
            var a = f.Deploy(f.CreateUnit("形態A", "味方", pilot), 3, 4);
            a.UsedAction = 1;
            a.UsedSupportAttack = 2;
            a.Rank = 3;

            a.Transform("形態B");

            var b = a.OtherForm("形態B");
            Assert.AreEqual("出撃", b.Status);
            Assert.AreEqual("他形態", a.Status);
            Assert.AreEqual((3, 4), (b.x, b.y));
            Assert.AreSame(b, f.Map.MapDataForUnit[3, 4]);
            Assert.AreEqual(1, b.UsedAction);
            Assert.AreEqual(2, b.UsedSupportAttack);
            Assert.AreEqual(3, b.Rank);
            Assert.AreSame(b, pilot.Unit);
            Assert.AreEqual(0, a.CountPilot());
        }

        [TestMethod]
        public void Transform_Stored_SwapsUnitInCarrier()
        {
            // 格納中に変形すると母艦の格納ユニットが変形後のユニットに入れ替わる
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("形態A", features: "変形=変形 形態B");
            f.AddUnitData("形態B", features: "変形=変形 形態A");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var a = f.CreateUnit("形態A", "味方", f.CreatePilot("パイロット"));
            a.Status = "格納";
            carrier.LoadUnit(a);

            a.Transform("形態B");

            var b = a.OtherForm("形態B");
            Assert.AreEqual("格納", b.Status);
            Assert.AreSame(b, carrier.UnitOnBoards.Single());
        }

        [TestMethod]
        public void Transform_PilotOrderChangesWhenPilotNumIsNegative()
        {
            // パイロット数が負（変形で主パイロットが入れ替わる）の場合、変形先に対応するパイロットが主パイロットになる
            var f = new UnitMapActionFixture();
            f.AddUnitData("形態A", features: "変形=変形 形態B").PilotNum = -2;
            f.AddUnitData("形態B", features: "変形=変形 形態A").PilotNum = -2;
            var p1 = f.CreatePilot("パイロット1");
            var p2 = f.CreatePilot("パイロット2");
            var a = f.CreateUnit("形態A", "味方", p1, p2);

            a.Transform("形態B");

            var b = a.OtherForm("形態B");
            CollectionAssert.AreEqual(new[] { p2, p1 }, b.Pilots.ToArray());
        }

        // ──────────────────────────────────────────────
        // Combine
        // ──────────────────────────────────────────────

        private static (UnitMapActionFixture f, Unit combined, Unit pa, Unit pb) CreateCombineSet(
            Pilot pilotA = null, Pilot pilotB = null)
        {
            var f = new UnitMapActionFixture();
            var uc = f.AddUnitData("合体機", hp: 3000, en: 200, features: "分離=分離 パーツA パーツB");
            uc.PilotNum = 2;
            uc.AddWeapon("ミサイル").Bullet = 10;
            var ua = f.AddUnitData("パーツA", hp: 1000, en: 100, features: "合体=合体 合体機 パーツB");
            ua.AddWeapon("ミサイル").Bullet = 4;
            var ub = f.AddUnitData("パーツB", hp: 1000, en: 100, features: "合体=合体 合体機 パーツA");
            ub.AddWeapon("ミサイル").Bullet = 4;
            var combined = f.CreateUnit("合体機");
            combined.Status = "他形態";
            var pa = f.CreateUnit("パーツA", "味方", pilotA ?? f.CreatePilot("パイロットA"));
            var pb = f.CreateUnit("パーツB", "味方", pilotB ?? f.CreatePilot("パイロットB"));
            return (f, combined, pa, pb);
        }

        [TestMethod]
        public void Combine_ByMovingOntoPartner_DeploysCombinedUnitAndTransfersPilots()
        {
            // 合体相手のいる地点への移動で２体合体し、合体後のユニットがその地点に出撃する
            var (f, combined, pa, pb) = CreateCombineSet();
            f.Deploy(pa, 2, 2);
            f.Deploy(pb, 5, 5);
            var pilotA = pa.MainPilot();
            var pilotB = pb.MainPilot();

            pa.Move(5, 5);

            Assert.AreEqual("出撃", combined.Status);
            Assert.AreEqual((5, 5), (combined.x, combined.y));
            Assert.AreSame(combined, f.Map.MapDataForUnit[5, 5]);
            Assert.IsNull(f.Map.MapDataForUnit[2, 2]);
            Assert.AreEqual("旧主形態", pa.Status);
            Assert.AreEqual("旧形態", pb.Status);
            CollectionAssert.AreEqual(new[] { pilotA, pilotB }, combined.Pilots.ToArray());
            // 分離ユニットの座標は合体後のユニットに合わせる
            Assert.AreEqual((5, 5), (pa.x, pa.y));
            Assert.AreEqual((5, 5), (pb.x, pb.y));
        }

        [TestMethod]
        public void Combine_SumsBulletsAndTakesMaxRank()
        {
            // 合体後の同名武器の弾数は各パーツの残弾の合計。ランクは高い方を受け継ぐ
            var (f, combined, pa, pb) = CreateCombineSet();
            pa.Status = "待機";
            pb.Status = "待機";
            pa.Weapons.Single().SetBullet(1);
            pb.Weapons.Single().SetBullet(3);
            pb.Rank = 4;

            pa.Combine("合体機", true);

            Assert.AreEqual(4, combined.Weapons.Single().Bullet());
            Assert.AreEqual(4, combined.Rank);
            Assert.AreEqual("待機", combined.Status);
        }

        [TestMethod]
        public void Combine_CombinedUnitWithoutSplitData_DoesNothing()
        {
            // 合体後のユニットに分離指定がなければ合体処理を行わない
            var f = new UnitMapActionFixture();
            f.AddUnitData("合体機");
            f.AddUnitData("パーツA", features: "合体=合体 合体機 パーツB");
            f.AddUnitData("パーツB", features: "合体=合体 合体機 パーツA");
            var combined = f.CreateUnit("合体機");
            combined.Status = "他形態";
            var pa = f.CreateUnit("パーツA", "味方", f.CreatePilot("パイロットA"));
            var pb = f.CreateUnit("パーツB", "味方", f.CreatePilot("パイロットB"));
            pa.Status = "待機";
            pb.Status = "待機";

            pa.Combine("合体機", true);

            Assert.AreEqual("他形態", combined.Status);
            Assert.AreEqual("待機", pa.Status);
            Assert.AreEqual(1, pa.CountPilot());
            Assert.AreEqual(0, combined.CountPilot());
        }

        [TestMethod]
        [Ignore("移植の不具合: 合体メッセージが未定義でも OpenMessageForm を呼んでしまう")]
        public void Combine_NoCombineMessage_DoesNotOpenMessageForm()
        {
            // 元の SRC では合体メッセージが定義されている場合だけメッセージウィンドウを開く
            var (f, combined, pa, pb) = CreateCombineSet();
            f.Deploy(pa, 2, 2);
            f.Deploy(pb, 5, 5);

            pa.Move(5, 5);

            Assert.AreEqual("出撃", combined.Status);
            Assert.AreEqual(0, f.OpenMessageFormCount);
        }

        // ──────────────────────────────────────────────
        // Split
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Split_Deployed_DistributesPilotsHpAndPositions()
        {
            // 分離すると各パーツに順にパイロットが乗り、HP/EN の割合を受け継いで周囲に出撃する
            var (f, combined, pa, pb) = CreateCombineSet();
            pa.Status = "待機";
            pb.Status = "待機";
            pa.Combine("合体機", true);
            var pilotA = combined.Pilots.ElementAt(0);
            var pilotB = combined.Pilots.ElementAt(1);
            f.Deploy(combined, 5, 5);
            combined.HP = 1500;
            combined.EN = 50;
            combined.UsedAction = 1;
            pb.UsedSupportAttack = 1;

            combined.Split();

            Assert.AreEqual("他形態", combined.Status);
            Assert.AreEqual(0, combined.CountPilot());
            Assert.AreEqual("出撃", pa.Status);
            Assert.AreEqual("出撃", pb.Status);
            Assert.AreEqual((5, 5), (pa.x, pa.y));
            Assert.AreEqual((4, 5), (pb.x, pb.y));
            Assert.AreSame(pilotA, pa.MainPilot());
            Assert.AreSame(pilotB, pb.MainPilot());
            Assert.AreEqual(500, pa.HP);
            Assert.AreEqual(500, pb.HP);
            Assert.AreEqual(25, pa.EN);
            Assert.AreEqual(25, pb.EN);
            // 行動済みは各パーツに引き継がれ、２番機以降の援護回数はリセット
            Assert.AreEqual(1, pa.UsedAction);
            Assert.AreEqual(1, pb.UsedAction);
            Assert.AreEqual(0, pb.UsedSupportAttack);
        }

        [TestMethod]
        public void Split_SyncsBulletRatioToParts()
        {
            // 分離後の弾数は「パーツの最大弾数 × 残弾 \ 合体時の最大弾数」
            var (f, combined, pa, pb) = CreateCombineSet();
            pa.Status = "待機";
            pb.Status = "待機";
            pa.Combine("合体機", true);
            combined.Weapons.Single().SetBullet(5);

            combined.Split();

            // 4 * 5 \ 10 = 2
            Assert.AreEqual(2, pa.Weapons.Single().Bullet());
            Assert.AreEqual(2, pb.Weapons.Single().Bullet());
        }

        [TestMethod]
        public void Split_TimeLimitedCombination_OnBattleMap_BecomesUnusable()
        {
            // 制限時間付きの合体形態は（ステータス表示以外で）分離すると二度と使えない
            var f = new UnitMapActionFixture();
            f.SetBattleMap();
            f.AddUnitData("合体機", features: new[] { "分離=分離 パーツA パーツB", "制限時間=3" }).PilotNum = 2;
            f.AddUnitData("パーツA", features: "合体=合体 合体機 パーツB");
            f.AddUnitData("パーツB", features: "合体=合体 合体機 パーツA");
            var combined = f.CreateUnit("合体機", "味方", f.CreatePilot("パイロットA"), f.CreatePilot("パイロットB"));
            var pa = f.CreateUnit("パーツA");
            var pb = f.CreateUnit("パーツB");
            pa.Status = "旧主形態";
            pb.Status = "旧形態";
            combined.Status = "待機";

            combined.Split();

            Assert.IsTrue(combined.IsConditionSatisfied("行動不能"));
            Assert.AreEqual("待機", pa.Status);
            Assert.AreEqual("待機", pb.Status);
            Assert.AreEqual(1, pa.CountPilot());
            Assert.AreEqual(1, pb.CountPilot());
        }
    }
}
