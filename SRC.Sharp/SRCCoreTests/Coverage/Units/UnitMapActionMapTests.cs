using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.Units
{
    /// <summary>
    /// Unit.map.cs（StandBy / Move / Jump / Escape / Land）のテスト。
    /// 元の SRC の Unit.cls の同名メソッドを仕様とする。
    /// </summary>
    [TestClass]
    public class UnitMapActionMapTests
    {
        // ──────────────────────────────────────────────
        // StandBy
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StandBy_TargetOccupied_PlacesAtNearestFreeCellInScanOrder()
        {
            // 指定地点が埋まっていれば距離1の地点を x 昇順・y 昇順に調べて最初の空きに置く
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体");
            var blocker = f.Deploy(f.CreateUnit("機体"), 5, 5);
            var u = f.CreateUnit("機体");

            u.StandBy(5, 5);

            Assert.AreEqual(5, blocker.x);
            Assert.AreEqual((4, 5), (u.x, u.y));
            Assert.AreEqual("出撃", u.Status);
            Assert.AreSame(u, f.Map.MapDataForUnit[4, 5]);
            Assert.AreSame(blocker, f.Map.MapDataForUnit[5, 5]);
        }

        [TestMethod]
        public void StandBy_TroopPlacement_SkipsOddDistances()
        {
            // 「部隊配置」ではユニット同士を隣接させないため奇数距離を飛ばす
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体");
            f.Deploy(f.CreateUnit("機体"), 5, 5);
            var u = f.CreateUnit("機体");

            u.StandBy(5, 5, "部隊配置");

            Assert.AreEqual((3, 5), (u.x, u.y));
            Assert.IsNull(f.Map.MapDataForUnit[4, 5]);
        }

        [TestMethod]
        public void StandBy_SkipsImpassableAndUnenterableTerrain()
        {
            // 移動コスト100超・空（空を飛べない）・深水（水上/空/水移動不可）の地点は配置先にならない
            var f = new UnitMapActionFixture();
            f.AddUnitData("陸戦機", transportation: "陸", adaption: "-AAA");
            f.SetTerrain(5, 5, "陸", moveCost: 1000);
            f.SetTerrain(4, 5, "深水");
            f.SetTerrain(5, 4, "空");
            var u = f.CreateUnit("陸戦機");

            u.StandBy(5, 5);

            Assert.AreEqual((5, 6), (u.x, u.y));
            Assert.AreEqual("地上", u.Area);
        }

        [TestMethod]
        public void StandBy_ShallowWater_EnterableWithWaterAdaption()
        {
            // 水地形は水上移動・空中移動がなくても水の地形適応があれば配置でき、水中に置かれる
            var f = new UnitMapActionFixture();
            f.AddUnitData("陸戦機", transportation: "陸", adaption: "-AAA");
            f.SetTerrain(5, 5, "水");
            var u = f.CreateUnit("陸戦機");

            u.StandBy(5, 5);

            Assert.AreEqual((5, 5), (u.x, u.y));
            Assert.AreEqual("水中", u.Area);
        }

        [TestMethod]
        public void StandBy_ShallowWater_NoWaterAdaption_IsSkipped()
        {
            // 水の地形適応が「-」（0）だと水地形には配置されない
            var f = new UnitMapActionFixture();
            f.AddUnitData("陸戦機", transportation: "陸", adaption: "-A-A");
            f.SetTerrain(5, 5, "水");
            var u = f.CreateUnit("陸戦機");

            u.StandBy(5, 5);

            Assert.AreEqual((4, 5), (u.x, u.y));
        }

        [DataTestMethod]
        [DataRow("陸", "空陸", "AAAA", "空中")]   // 空の適応が陸以上なら空中
        [DataRow("陸", "空陸", "BAAA", "地上")]   // 空の適応が陸より低ければ地上
        [DataRow("屋内", "空陸", "BAAA", "地上")]
        [DataRow("月面", "陸", "-A-A", "地上")]   // 空も宇宙も移動できなければ地上
        [DataRow("月面", "宇宙陸", "-A-A", "宇宙")]
        [DataRow("水", "水上陸", "-AAA", "水上")]
        [DataRow("宇宙", "陸", "-A-A", "宇宙")]
        [DataRow("空", "空", "A---", "空中")]
        public void StandBy_SetsAreaByTerrainClass(string terrainClass, string trans, string adaption, string expectedArea)
        {
            // 配置先の地形クラスと移動タイプ・地形適応からユニットの位置（Area）が決まる
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体", transportation: trans, adaption: adaption);
            f.SetTerrain(3, 3, terrainClass);
            var u = f.CreateUnit("機体");

            u.StandBy(3, 3);

            Assert.AreEqual((3, 3), (u.x, u.y));
            Assert.AreEqual(expectedArea, u.Area);
        }

        [TestMethod]
        public void StandBy_NoFreeCell_BecomesStandbyWithoutPlacement()
        {
            // 空き地点がなければ出撃せず「待機」になる
            var f = new UnitMapActionFixture(1, 1);
            f.AddUnitData("機体");
            var blocker = f.Deploy(f.CreateUnit("機体"), 1, 1);
            var u = f.CreateUnit("機体");

            u.StandBy(1, 1);

            Assert.AreEqual("待機", u.Status);
            Assert.AreEqual((0, 0), (u.x, u.y));
            Assert.AreSame(blocker, f.Map.MapDataForUnit[1, 1]);
        }

        [TestMethod]
        public void StandBy_UncontrollableFeature_AddsBerserkCondition()
        {
            // 制御不可の特殊能力を持つユニットは出撃時に暴走状態になる
            var f = new UnitMapActionFixture();
            f.AddUnitData("暴走機", features: "制御不可");
            var u = f.CreateUnit("暴走機");

            u.StandBy(2, 2);

            Assert.IsTrue(u.IsConditionSatisfied("暴走"));
            Assert.AreEqual(-1, u.ConditionLifetime("暴走"));
        }

        [TestMethod]
        public void StandBy_SyncsOtherFormsAndUnitsOnBoard()
        {
            // 他形態は座標を合わせて「他形態」に、搭載ユニットは座標だけを合わせる
            var f = new UnitMapActionFixture();
            f.AddUnitData("形態A", features: new[] { "変形=変形 形態B", "母艦" });
            f.AddUnitData("形態B", features: "変形=変形 形態A");
            f.AddUnitData("搭載機");
            var u = f.CreateUnit("形態A");
            var other = u.OtherForm("形態B");
            var cargo = f.CreateUnit("搭載機");
            cargo.Status = "格納";
            u.LoadUnit(cargo);

            u.StandBy(6, 7);

            Assert.AreEqual((6, 7), (other.x, other.y));
            Assert.AreEqual("他形態", other.Status);
            Assert.AreEqual((6, 7), (cargo.x, cargo.y));
            Assert.AreEqual("格納", cargo.Status);
        }

        [TestMethod]
        public void StandBy_OnBattleMap_PaintsUnitBitmap_AsyncModeWithoutRefresh()
        {
            // マップ表示中は配置後にユニット画像を描画する。「非同期」ならリフレッシュ無しで描く
            var f = new UnitMapActionFixture();
            f.SetBattleMap();
            f.AddUnitData("機体");
            var u1 = f.CreateUnit("機体");
            var u2 = f.CreateUnit("機体");

            u1.StandBy(2, 2);
            u2.StandBy(4, 4, "非同期");

            Assert.AreEqual(2, f.Painted.Count);
            Assert.AreSame(u1, f.Painted[0].u);
            Assert.AreEqual("", f.Painted[0].mode ?? "");
            Assert.AreSame(u2, f.Painted[1].u);
            Assert.AreEqual("リフレッシュ無し", f.Painted[1].mode);
        }

        // ──────────────────────────────────────────────
        // Move
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Move_ToEmptyCell_UpdatesMapAndConsumesEnInAir()
        {
            // 空中での移動はＥＮを5消費し、マップの登録位置が移動先に変わる
            var f = new UnitMapActionFixture();
            f.AddUnitData("飛行機", transportation: "空陸", adaption: "AAAA");
            f.SetTerrain(2, 2, "陸");
            var u = f.Deploy(f.CreateUnit("飛行機"), 2, 2);
            Assert.AreEqual("空中", u.Area);
            u.EN = 100;

            u.Move(4, 3);

            Assert.AreEqual((4, 3), (u.x, u.y));
            Assert.IsNull(f.Map.MapDataForUnit[2, 2]);
            Assert.AreSame(u, f.Map.MapDataForUnit[4, 3]);
            Assert.AreEqual(95, u.EN);
            // 移動元の画像を消してから移動先に描画する
            Assert.AreEqual((2, 2, false), f.Erased.Single());
            Assert.AreSame(u, f.Painted.Last().u);
        }

        [DataTestMethod]
        [DataRow("陸", "AAAA", new string[0], "地上", 100)]           // 地上の通常移動は消費なし
        [DataRow("陸", "AAAA", new[] { "ホバー移動" }, "地上", 95)]    // ホバー移動は地上でも5消費
        [DataRow("陸地中", "AAAA", new string[0], "地中", 90)]        // 地中は10消費
        public void Move_EnConsumptionDependsOnArea(string trans, string adaption, string[] features, string area, int expectedEn)
        {
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体", transportation: trans, adaption: adaption, features: features);
            var u = f.Deploy(f.CreateUnit("機体"), 2, 2);
            u.Area = area;
            u.EN = 100;

            u.Move(3, 2);

            Assert.AreEqual(area, u.Area);
            Assert.AreEqual(expectedEn, u.EN);
        }

        [TestMethod]
        public void Move_WithoutEnConsumption_KeepsEn()
        {
            // without_en_consumption 指定時は空中でもＥＮを消費しない
            var f = new UnitMapActionFixture();
            f.AddUnitData("飛行機", transportation: "空陸", adaption: "AAAA");
            var u = f.Deploy(f.CreateUnit("飛行機"), 2, 2);
            u.EN = 100;

            u.Move(3, 3, without_en_consumption: true);

            Assert.AreEqual(100, u.EN);
        }

        [TestMethod]
        public void Move_WithMoveAnimation_UsesMoveBitmapUnlessCanceled()
        {
            // 移動アニメ有効時は MoveUnitBitmap2 で描画する。キャンセルによる移動では通常描画
            var f = new UnitMapActionFixture();
            f.Src.SystemConfig.MoveAnimation = true;
            f.AddUnitData("機体");
            var u = f.Deploy(f.CreateUnit("機体"), 2, 2);

            u.Move(3, 2);
            Assert.AreSame(u, f.MovedBitmaps.Single());
            Assert.AreEqual(0, f.Painted.Count);

            u.Move(2, 2, by_cancel: true);
            Assert.AreEqual(1, f.MovedBitmaps.Count);
            Assert.AreSame(u, f.Painted.Single().u);
        }

        [DataTestMethod]
        [DataRow("水", "陸", "地上", "水中")]       // 地上から水へ入ると水中
        [DataRow("水", "水上陸", "地上", "水上")]   // 水上移動できれば水上
        [DataRow("深水", "陸", "宇宙", "水中")]     // 宇宙から水へは水中
        [DataRow("陸", "陸", "水中", "地上")]       // 水中から陸へ上がると地上
        [DataRow("陸", "陸", "宇宙", "地上")]       // 宇宙から陸へは空を飛べなければ地上
        [DataRow("陸", "空陸", "宇宙", "空中")]     // 空を飛べて空の適応が陸以上なら空中
        [DataRow("陸", "陸", "空中", "空中")]       // 空中から陸へ移っても空中のまま
        [DataRow("月面", "陸", "地上", "地上")]     // 月面で地上なら変化なし
        [DataRow("月面", "宇宙陸", "空中", "宇宙")] // 月面で浮いていれば宇宙
        [DataRow("宇宙", "陸", "地上", "宇宙")]
        [DataRow("空", "空陸", "地上", "空中")]
        public void Move_ResolvesAreaByDestinationTerrain(string terrainClass, string trans, string fromArea, string expectedArea)
        {
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体", transportation: trans, adaption: "AAAA");
            var u = f.Deploy(f.CreateUnit("機体"), 2, 2);
            u.Area = fromArea;
            f.SetTerrain(3, 2, terrainClass);

            u.Move(3, 2, without_en_consumption: true);

            Assert.AreEqual(expectedArea, u.Area);
        }

        [TestMethod]
        public void Move_OntoCarrier_LandsAndLowersMorale()
        {
            // 母艦のいる地点へ移動すると着艦し、パイロットの気力が5下がる
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy(f.CreateUnit("母艦", "味方", f.CreatePilot("艦長")), 5, 5);
            var pilot = f.CreatePilot("パイロット");
            pilot.Morale = 110;
            var u = f.Deploy(f.CreateUnit("艦載機", "味方", pilot), 3, 5);

            u.Move(5, 5);

            Assert.AreEqual("格納", u.Status);
            Assert.AreEqual(1, carrier.CountUnitOnBoard());
            Assert.AreSame(carrier, f.Map.MapDataForUnit[5, 5]);
            Assert.IsNull(f.Map.MapDataForUnit[3, 5]);
            Assert.AreEqual((5, 5), (u.x, u.y));
            Assert.AreEqual(105, pilot.Morale);
        }

        [TestMethod]
        public void Move_OntoCarrierByCancel_DoesNotLowerMorale()
        {
            // 移動キャンセルによる着艦では気力は下がらない
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            f.Deploy(f.CreateUnit("母艦", "味方", f.CreatePilot("艦長")), 5, 5);
            var pilot = f.CreatePilot("パイロット");
            pilot.Morale = 110;
            var u = f.Deploy(f.CreateUnit("艦載機", "味方", pilot), 3, 5);

            u.Move(5, 5, by_cancel: true);

            Assert.AreEqual("格納", u.Status);
            Assert.AreEqual(110, pilot.Morale);
        }

        [TestMethod]
        public void Move_OntoNonCarrierUnit_DoesNotLandOrRegister()
        {
            // 母艦でも合体相手でもないユニットのいる地点へは着艦しない（エラー扱い）
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体");
            var other = f.Deploy(f.CreateUnit("機体"), 5, 5);
            var u = f.Deploy(f.CreateUnit("機体"), 3, 5);

            u.Move(5, 5);

            Assert.AreEqual("出撃", u.Status);
            Assert.AreSame(other, f.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual(0, other.CountUnitOnBoard());
        }

        // ──────────────────────────────────────────────
        // Jump
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Jump_TargetOccupied_PlacesAtNearestFreeCellAndResolvesArea()
        {
            // ジャンプ先が埋まっていれば近くの空き地点へ。移動先の地形で位置が決まる
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体", transportation: "陸", adaption: "AAAA");
            f.Deploy(f.CreateUnit("機体"), 6, 6);
            var u = f.Deploy(f.CreateUnit("機体"), 1, 1);
            f.SetTerrain(5, 6, "宇宙");

            u.Jump(6, 6);

            Assert.AreEqual((5, 6), (u.x, u.y));
            Assert.AreEqual("宇宙", u.Area);
            Assert.IsNull(f.Map.MapDataForUnit[1, 1]);
            Assert.AreSame(u, f.Map.MapDataForUnit[5, 6]);
            Assert.AreEqual((1, 1, true), f.Erased.Single());
            Assert.AreSame(u, f.Painted.Single().u);
        }

        [TestMethod]
        public void Jump_WithoutRefresh_DoesNotPaint()
        {
            // do_refresh=false なら消去はリフレッシュ無し、描画も行わない
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体");
            var u = f.Deploy(f.CreateUnit("機体"), 1, 1);

            u.Jump(4, 4, do_refresh: false);

            Assert.AreEqual((4, 4), (u.x, u.y));
            Assert.AreEqual((1, 1, false), f.Erased.Single());
            Assert.AreEqual(0, f.Painted.Count);
        }

        [TestMethod]
        public void Jump_SkipsWaterWithoutWaterAbility()
        {
            // ジャンプでも水地形（浅い水・深水とも）は水上/空/水の適応がなければ避ける
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体", transportation: "陸", adaption: "-A-A");
            var u = f.Deploy(f.CreateUnit("機体"), 1, 1);
            f.SetTerrain(5, 5, "深水");
            f.SetTerrain(4, 5, "水");

            u.Jump(5, 5);

            Assert.AreEqual((5, 4), (u.x, u.y));
        }

        // ──────────────────────────────────────────────
        // Escape
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Escape_DeployedUnit_RemovesFromMapAndUnloadsCargo()
        {
            // 出撃中のユニットはマップから消えて待機になり、搭載ユニットも待機で降ろされる
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 3, 3);
            var cargo = f.CreateUnit("艦載機");
            cargo.Status = "格納";
            carrier.LoadUnit(cargo);
            carrier.AddCondition("破壊キャンセル", 1, cdata: "");
            f.Status.DisplayedUnit = carrier;

            carrier.Escape();

            Assert.AreEqual("待機", carrier.Status);
            Assert.IsNull(f.Map.MapDataForUnit[3, 3]);
            Assert.AreEqual((3, 3, false), f.Erased.Single());
            Assert.IsFalse(carrier.IsConditionSatisfied("破壊キャンセル"));
            Assert.AreEqual(0, carrier.CountUnitOnBoard());
            Assert.AreEqual("待機", cargo.Status);
            // ステータス表示中のユニットなら表示を解除する
            Assert.AreEqual(1, f.Status.ClearCount);
        }

        [TestMethod]
        public void Escape_OnBattleMap_ErasesWithRefreshUnlessAsync()
        {
            // マップ表示中はリフレッシュ付きで消す。「非同期」ならリフレッシュ無し
            var f = new UnitMapActionFixture();
            f.SetBattleMap();
            f.AddUnitData("機体");
            var u1 = f.Deploy(f.CreateUnit("機体"), 2, 2);
            var u2 = f.Deploy(f.CreateUnit("機体"), 4, 4);

            u1.Escape();
            u2.Escape("非同期");

            CollectionAssert.AreEqual(new[] { (2, 2, true), (4, 4, false) }, f.Erased);
            Assert.AreEqual(0, f.Status.ClearCount);
        }

        [TestMethod]
        public void Escape_StoredUnit_IsUnloadedFromCarrier()
        {
            // 格納中のユニットは母艦から降ろされて待機になる
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 3, 3);
            var cargo = f.CreateUnit("艦載機");
            cargo.Status = "格納";
            carrier.LoadUnit(cargo);

            cargo.Escape();

            Assert.AreEqual("待機", cargo.Status);
            Assert.AreEqual(0, carrier.CountUnitOnBoard());
            Assert.AreSame(carrier, f.Map.MapDataForUnit[3, 3]);
            Assert.AreEqual(0, f.Erased.Count);
        }

        [TestMethod]
        public void Escape_ReleasesServantsAndSlaves()
        {
            // 撤退時には召喚ユニットと魅了・憑依したユニットを解放する
            var f = new UnitMapActionFixture();
            f.AddUnitData("主機");
            f.AddUnitData("召喚機");
            f.AddUnitData("隷属機");
            var master = f.Deploy(f.CreateUnit("主機"), 2, 2);
            var servant = f.Deploy(f.CreateUnit("召喚機"), 3, 3);
            var slave = f.Deploy(f.CreateUnit("隷属機", "敵"), 4, 4);
            master.AddServant(servant);
            servant.Summoner = master;
            master.AddSlave(slave);
            slave.Master = master;
            slave.AddCondition("魅了", 3, cdata: "");

            master.Escape();

            Assert.AreEqual(0, master.CountServant());
            Assert.AreEqual(0, master.CountSlave());
            // 出撃中の召喚ユニットは撤退して破棄される（Unit.cls DismissServant）
            Assert.AreEqual("破棄", servant.Status);
            Assert.IsNull(f.Map.MapDataForUnit[3, 3]);
            // 魅了は解け、支配者の参照も外れる
            Assert.IsFalse(slave.IsConditionSatisfied("魅了"));
            Assert.IsNull(slave.Master);
        }

        // ──────────────────────────────────────────────
        // Land
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Land_MoraleLossOption_DoesNotDropBelow100()
        {
            // オプション「母艦収納時気力低下小」では気力は100未満にならず、100未満からは下がらない
            var f = new UnitMapActionFixture();
            f.Src.Expression.DefineGlobalVariable("Option(母艦収納時気力低下小)");
            f.Src.Expression.SetVariableAsLong("Option(母艦収納時気力低下小)", 1);
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var p1 = f.CreatePilot("パイロットA");
            var p2 = f.CreatePilot("パイロットB");
            var u1 = f.Deploy(f.CreateUnit("艦載機", "味方", p1), 1, 1);
            var u2 = f.Deploy(f.CreateUnit("艦載機", "味方", p2), 2, 1);
            p1.Morale = 103;
            p2.Morale = 99;

            u1.Escape();
            u1.Land(carrier);
            u2.Escape();
            u2.Land(carrier);

            Assert.AreEqual(100, p1.Morale);
            Assert.AreEqual(99, p2.Morale);
            Assert.AreEqual(2, carrier.CountUnitOnBoard());
        }

        [TestMethod]
        public void Land_MachinePersonality_KeepsMorale_ButSubPilotsAndSupportsLose()
        {
            // 機械の性格は気力が下がらない。サブパイロット・サポートはそれぞれ5下がる
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("複座機").PilotNum = 2;
            f.AddPilotData("ロボット", personality: "機械");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var main = f.CreatePilot("ロボット");
            var sub = f.CreatePilot("副操縦士");
            var support = f.CreatePilot("サポート");
            var u = f.CreateUnit("複座機", "味方", main, sub);
            u.AddSupport(support);
            support.Unit = u;
            main.Morale = 110;
            sub.Morale = 110;
            support.Morale = 110;

            u.Land(carrier);

            Assert.AreEqual("格納", u.Status);
            Assert.AreEqual(110, main.Morale);
            Assert.AreEqual(105, sub.Morale);
            Assert.AreEqual(105, support.Morale);
        }

        [TestMethod]
        public void Land_Event_FromStandby_PreparesAreaAndResetsActionCounters()
        {
            // Land コマンドでの着艦（待機中のユニット）は母艦の地形から位置を決め、行動回数等を回復する
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", transportation: "宇宙", adaption: "---A", features: "母艦");
            f.AddUnitData("艦載機", transportation: "宇宙陸", adaption: "AAAA", features: "制御不可");
            f.SetTerrain(5, 5, "宇宙");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var u = f.CreateUnit("艦載機", "味方", f.CreatePilot("パイロット"));
            u.Status = "待機";
            u.Area = "地上";
            u.UsedAction = 1;
            u.UsedSupportAttack = 1;
            u.UsedSupportGuard = 1;
            u.UsedSyncAttack = 1;
            u.UsedCounterAttack = 1;

            u.Land(carrier, is_event: true);

            Assert.AreEqual("格納", u.Status);
            Assert.AreEqual("宇宙", u.Area);
            Assert.AreEqual((5, 5), (u.x, u.y));
            Assert.AreEqual(0, u.UsedAction);
            Assert.AreEqual(0, u.UsedSupportAttack);
            Assert.AreEqual(0, u.UsedSupportGuard);
            Assert.AreEqual(0, u.UsedSyncAttack);
            Assert.AreEqual(0, u.UsedCounterAttack);
            Assert.IsTrue(u.IsConditionSatisfied("暴走"));
        }

        [TestMethod]
        public void Land_Event_FromDeployed_EscapesFirst()
        {
            // Land コマンドで出撃中のユニットを着艦させるとマップから撤退してから格納される
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var u = f.Deploy(f.CreateUnit("艦載機", "味方", f.CreatePilot("パイロット")), 2, 2);
            u.UsedAction = 1;

            u.Land(carrier, is_event: true);

            Assert.AreEqual("格納", u.Status);
            Assert.IsNull(f.Map.MapDataForUnit[2, 2]);
            Assert.AreEqual((5, 5), (u.x, u.y));
            // 出撃中からの着艦では行動回数は回復しない
            Assert.AreEqual(1, u.UsedAction);
        }
    }
}
