using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Units.cs（VB: Units.cls）のユニットリスト管理のテスト
    /// </summary>
    [TestClass]
    public class UnitCoreUnitsTests
    {
        // 特殊能力行を受け取ってユニットデータ1件分のテキストを作る
        private static string UnitBlock(string name, string cls = "汎用", int pilotNum = 1, int value = 1000, params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, {cls}, {pilotNum}, 2
陸, 5, M, {value}, 100
{feat}
3000, 100, 800, 90
AA-A, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        private static SRC CreateSrcWithUnits(params string[] blocks)
        {
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src, string.Join("\n", blocks));
            return src;
        }

        // ──────────────────────────────────────────────
        // Add / CreateID / Item
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_UndefinedUnitData_ReturnsNullAndAddsNothing()
        {
            // ユニットデータが定義されていなければ Nothing を返す
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            Assert.IsNull(src.UList.Add("未定義機", 0, "味方"));
            Assert.AreEqual(0, src.UList.Count());
        }

        [TestMethod]
        public void Add_SetsRankPartyAndSequentialIds()
        {
            // ID は「ユニット名:連番」で作られ、ランクと陣営が設定される
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            var u1 = src.UList.Add("テスト機", 3, "敵");
            var u2 = src.UList.Add("テスト機", 0, "味方");

            Assert.AreEqual("テスト機:1", u1.ID);
            Assert.AreEqual("テスト機:2", u2.ID);
            Assert.AreEqual(3, u1.Rank);
            Assert.AreEqual("敵", u1.Party);
            Assert.AreEqual(2, src.UList.Count());
            // FullRecover されているので HP は最大値
            Assert.AreEqual(u1.MaxHP, u1.HP);
        }

        [TestMethod]
        public void CreateID_SkipsIdAlreadyRegistered()
        {
            // 既に使われている ID は飛ばして次の番号を採番する
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            var u = src.UList.Add("テスト機", 0, "味方");
            Assert.AreEqual("テスト機:1", u.ID);
            src.UList.IDCount = 0;
            Assert.AreEqual("テスト機:2", src.UList.CreateID("テスト機"));
        }

        [TestMethod]
        public void Add_TransformFeature_CreatesOtherFormsLinkedToEachOther()
        {
            // 変形先の形態が「他形態」状態で作られ、互いに他形態として登録される
            var src = CreateSrcWithUnits(
                UnitBlock("人型", features: "変形=変形 飛行型 水中型"),
                UnitBlock("飛行型", features: "変形=変形 人型 水中型"),
                UnitBlock("水中型", features: "変形=変形 人型 飛行型"));

            var u = src.UList.Add("人型", 0, "味方");

            Assert.AreEqual(3, src.UList.Count());
            Assert.AreEqual("待機", u.Status);
            CollectionAssert.AreEquivalent(new[] { "飛行型", "水中型" }, u.OtherForms.Select(x => x.Name).ToList());
            var fly = u.OtherForm("飛行型");
            Assert.AreEqual("他形態", fly.Status);
            // 他形態からも本体ともう一方の形態が参照できる
            CollectionAssert.AreEquivalent(new[] { "人型", "水中型" }, fly.OtherForms.Select(x => x.Name).ToList());
            // 他形態の ID も本体のユニット名で採番される
            Assert.IsTrue(fly.ID.StartsWith("人型:"));
        }

        [TestMethod]
        public void Add_TransformTargetUndefined_ReturnsNull()
        {
            // 変形先のユニットデータが無ければエラーとして Nothing を返す
            var src = CreateSrcWithUnits(UnitBlock("人型", features: "変形=変形 未定義型"));
            Assert.IsNull(src.UList.Add("人型", 0, "味方"));
        }

        [TestMethod]
        public void Add_HyperModeTargetUndefined_ReturnsNull()
        {
            var src = CreateSrcWithUnits(UnitBlock("通常型", features: "ハイパーモード=覚醒 未定義型"));
            Assert.IsNull(src.UList.Add("通常型", 0, "味方"));
        }

        [TestMethod]
        public void Add_HyperAndNormalMode_CreatesBothDirections()
        {
            // ハイパーモード先(2番目の要素)とノーマルモード先(1番目の要素)を他形態として追加する
            var src = CreateSrcWithUnits(
                UnitBlock("通常型", features: "ハイパーモード=覚醒 覚醒型"),
                UnitBlock("覚醒型", features: "ノーマルモード=通常型"));

            var u = src.UList.Add("通常型", 0, "味方");
            Assert.IsTrue(u.IsOtherFormDefined("覚醒型"));
            Assert.IsTrue(u.OtherForm("覚醒型").IsOtherFormDefined("通常型"));
        }

        [TestMethod]
        public void Add_PartsSeparation_AddsSeparatedForm()
        {
            var src = CreateSrcWithUnits(
                UnitBlock("全装備型", features: "パーツ分離=パージ 軽装型"),
                UnitBlock("軽装型", features: "パーツ合体=全装備型"));

            var u = src.UList.Add("全装備型", 0, "味方");
            Assert.IsTrue(u.IsOtherFormDefined("軽装型"));
        }

        [TestMethod]
        public void Add_PartsSeparationTargetUndefined_ReturnsNull()
        {
            var src = CreateSrcWithUnits(UnitBlock("全装備型", features: "パーツ分離=パージ 未定義型"));
            Assert.IsNull(src.UList.Add("全装備型", 0, "味方"));
        }

        [TestMethod]
        public void Add_ExchangeFeature_CreatesAllListedForms()
        {
            // 換装=換装先１ 換装先２ … の全形態を他形態として作成する（VB は i = 1 から）
            var src = CreateSrcWithUnits(
                UnitBlock("陸戦装備", features: "換装=宇宙装備 空戦装備"),
                UnitBlock("宇宙装備", features: "換装=陸戦装備 空戦装備"),
                UnitBlock("空戦装備", features: "換装=陸戦装備 宇宙装備"));

            var u = src.UList.Add("陸戦装備", 0, "味方");
            Assert.IsTrue(u.IsOtherFormDefined("宇宙装備"));
            Assert.IsTrue(u.IsOtherFormDefined("空戦装備"));
        }

        [TestMethod]
        public void Add_OtherFormFeature_CreatesAllListedForms()
        {
            // 他形態=形態１ 形態２ … の全形態を他形態として作成する（VB は i = 1 から）
            var src = CreateSrcWithUnits(
                UnitBlock("本体", features: "他形態=形態Ａ 形態Ｂ"),
                UnitBlock("形態Ａ"),
                UnitBlock("形態Ｂ"));

            var u = src.UList.Add("本体", 0, "味方");
            Assert.IsTrue(u.IsOtherFormDefined("形態Ａ"));
            Assert.IsTrue(u.IsOtherFormDefined("形態Ｂ"));
        }

        [TestMethod]
        public void Add_CombinedUnitAlreadyExists_NewPartIsOtherForm()
        {
            // 既に合体先のユニットが作成されていれば、後から作った分離側は他形態になる
            var src = CreateSrcWithUnits(
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離 パーツＡ パーツＢ"),
                UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));

            var combined = src.UList.Add("合体後", 0, "味方");
            Assert.AreEqual("待機", combined.Status);
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            Assert.AreEqual("他形態", partA.Status);
        }

        [TestMethod]
        public void Add_PartUnitAlreadyExists_NewCombinedIsOtherForm()
        {
            // 既に分離先のユニットが作成されていれば、合体後のユニットは他形態になる
            var src = CreateSrcWithUnits(
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離 パーツＡ パーツＢ"),
                UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));

            src.UList.Add("パーツＢ", 0, "味方");
            var combined = src.UList.Add("合体後", 0, "味方");
            Assert.AreEqual("他形態", combined.Status);
        }

        [TestMethod]
        public void Item_FindsByIdThenByNameIgnoringDiscarded()
        {
            // ID で見つからなければ名前で検索するが、破棄されたユニットは対象外
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            var u1 = src.UList.Add("テスト機", 0, "味方");
            var u2 = src.UList.Add("テスト機", 0, "味方");

            Assert.AreSame(u2, src.UList.Item(u2.ID));
            Assert.AreSame(u1, src.UList.Item("テスト機"));

            u1.Status = "破棄";
            Assert.AreSame(u2, src.UList.Item("テスト機"));
            // Item2/IsDefined2 は ID のみで検索する
            Assert.IsNull(src.UList.Item2("テスト機"));
            Assert.IsFalse(src.UList.IsDefined2("テスト機"));
            Assert.IsTrue(src.UList.IsDefined("テスト機"));

            u2.Status = "破棄";
            Assert.IsFalse(src.UList.IsDefined("テスト機"));
            // ID 指定なら破棄されていても取得できる
            Assert.AreSame(u1, src.UList.Item(u1.ID));
        }

        [TestMethod]
        public void Clear_RemovesAllUnits()
        {
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            src.UList.Add("テスト機", 0, "味方");
            src.UList.Add("テスト機", 0, "敵");
            src.UList.Clear();
            Assert.AreEqual(0, src.UList.Count());
        }

        // ──────────────────────────────────────────────
        // Clean
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clean_RemovesNonPlayerIdleOrDestroyedUnitsAndKillsTheirPilots()
        {
            // 味方以外で待機・破壊状態のユニットは破棄され、搭乗パイロットも死亡扱い
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            UnitCoreFixture.LoadPilots(src, @"
敵兵(ザコ)
敵兵, てきへい, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid
");
            var ally = src.UList.Add("テスト機", 0, "味方");
            var enemyIdle = src.UList.Add("テスト機", 0, "敵");
            var enemyOut = src.UList.Add("テスト機", 0, "敵");
            enemyOut.Status = "出撃";
            var p = UnitCoreFixture.Board(src, enemyIdle, "敵兵(ザコ)");

            src.UList.Clean();

            CollectionAssert.AreEquivalent(new[] { ally, enemyOut }, src.UList.Items.ToList());
            Assert.AreEqual("破棄", enemyIdle.Status);
            Assert.IsFalse(p.Alive);
        }

        [TestMethod]
        public void Clean_RemovesDiscardedPlayerUnit()
        {
            // 味方でも破棄状態のユニットは削除される
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            var ally = src.UList.Add("テスト機", 0, "味方");
            var discarded = src.UList.Add("テスト機", 0, "味方");
            discarded.Status = "破棄";

            src.UList.Clean();

            CollectionAssert.AreEqual(new[] { ally }, src.UList.Items.ToList());
        }

        // ──────────────────────────────────────────────
        // Update
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Update_RepairsDestroyedPlayerUnitsAndChargesValue()
        {
            // 破壊された味方ユニットは修理費を支払って待機状態に戻る
            var src = CreateSrcWithUnits(UnitBlock("テスト機", value: 1000), UnitBlock("人間", cls: "(歩兵)", value: 500));
            var gui = (MockGUI)src.GUI;
            var messages = new List<string>();
            var opened = 0;
            var closed = 0;
            gui.OpenMessageFormHandler = (u1, u2) => opened++;
            gui.CloseMessageFormHandler = () => closed++;
            gui.DisplayMessageHandler = (pname, msg, mode) => messages.Add(pname + ":" + msg);
            src.Money = 10000;

            var robot = src.UList.Add("テスト機", 0, "味方");
            var hero = src.UList.Add("人間", 0, "味方");
            var safe = src.UList.Add("テスト機", 0, "味方");
            robot.Status = "破壊";
            hero.Status = "破壊";
            safe.Status = "出撃";

            src.UList.Update();

            Assert.AreEqual(10000 - 1000 - 500, src.Money);
            Assert.AreEqual("待機", robot.Status);
            Assert.AreEqual("待機", hero.Status);
            // 出撃中のユニットも待機に戻る
            Assert.AreEqual("待機", safe.Status);
            Assert.AreEqual(1, opened);
            Assert.AreEqual(1, closed);
            CollectionAssert.AreEqual(new[]
            {
                "システム:テスト機を修理した;修理費 = 1000",
                // 人間ユニットは「治療」
                "システム:人間を治療した;治療費 = 500",
                "システム:合計 = 1500",
            }, messages);
        }

        [TestMethod]
        public void Update_NoDestroyedPlayerUnit_DoesNotOpenMessageForm()
        {
            // 破壊された味方ユニットが無ければ修理メッセージは出さない（敵の破壊は対象外）
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            var gui = (MockGUI)src.GUI;
            var opened = 0;
            gui.OpenMessageFormHandler = (u1, u2) => opened++;
            src.Money = 5000;
            var ally = src.UList.Add("テスト機", 0, "味方");
            var enemy = src.UList.Add("テスト機", 0, "敵");
            enemy.Status = "破壊";

            src.UList.Update();

            Assert.AreEqual(0, opened);
            Assert.AreEqual(5000, src.Money);
            // 味方以外のユニットはリストから削除される
            CollectionAssert.AreEqual(new[] { ally }, src.UList.Items.ToList());
        }

        [TestMethod]
        public void Update_RepairsNpcSummonedByPlayer()
        {
            // 味方が召喚したＮＰＣユニットは修理対象、召喚者のないＮＰＣは対象外
            var src = CreateSrcWithUnits(UnitBlock("テスト機", value: 1000));
            var gui = (MockGUI)src.GUI;
            var messages = new List<string>();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => messages.Add(msg);
            src.Money = 10000;

            var summoner = src.UList.Add("テスト機", 0, "味方");
            var npc = src.UList.Add("テスト機", 0, "ＮＰＣ");
            npc.Summoner = summoner;
            npc.Status = "破壊";
            var npcAlone = src.UList.Add("テスト機", 0, "ＮＰＣ");
            npcAlone.Status = "破壊";

            src.UList.Update();

            Assert.AreEqual(9000, src.Money);
            CollectionAssert.AreEqual(new[] { "テスト機を修理した;修理費 = 1000", "合計 = 1000" }, messages);
        }

        [TestMethod]
        public void Update_DiscardsSummonedAndDummyUnits()
        {
            // 召喚ユニット・ダミーユニットは必ず破棄されてリストから消える
            var src = CreateSrcWithUnits(
                UnitBlock("テスト機"),
                UnitBlock("召喚獣", features: "召喚ユニット"),
                UnitBlock("ダミー", features: "ダミーユニット"));
            var ally = src.UList.Add("テスト機", 0, "味方");
            var summoned = src.UList.Add("召喚獣", 0, "味方");
            var dummy = src.UList.Add("ダミー", 0, "味方");

            src.UList.Update();

            CollectionAssert.AreEqual(new[] { ally }, src.UList.Items.ToList());
            Assert.AreEqual("破棄", summoned.Status);
            Assert.AreEqual("破棄", dummy.Status);
        }

        [TestMethod]
        public void Update_TransformsBackToFormDefinedFirst()
        {
            // 変形可能なユニットはデータ定義順で最初の形態（標準形態）に戻る
            var src = CreateSrcWithUnits(
                UnitBlock("人型", features: "変形=変形 飛行型"),
                UnitBlock("飛行型", features: "変形=変形 人型"));
            var u = src.UList.Add("人型", 0, "味方");
            u.Transform("飛行型");
            var fly = u.OtherForm("飛行型");
            Assert.AreEqual("待機", fly.Status);
            Assert.AreEqual("他形態", u.Status);

            src.UList.Update();

            Assert.AreEqual("待機", u.Status);
            Assert.AreEqual("他形態", fly.Status);
        }

        [TestMethod]
        public void Update_NormalModeReturnsFromHyperMode()
        {
            // ノーマルモードを持つ形態（変身後）はノーマルモード先へ戻る
            var src = CreateSrcWithUnits(
                UnitBlock("通常型", features: "ハイパーモード=覚醒 覚醒型"),
                UnitBlock("覚醒型", features: "ノーマルモード=通常型"));
            var u = src.UList.Add("通常型", 0, "味方");
            u.Transform("覚醒型");
            var hyper = u.OtherForm("覚醒型");
            Assert.AreEqual("待機", hyper.Status);

            src.UList.Update();

            Assert.AreEqual("待機", u.Status);
            Assert.AreEqual("他形態", hyper.Status);
        }

        private const string CrewPilots = @"
乗員Ａ
乗員Ａ, じょういんえー, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid

乗員Ｂ
乗員Ｂ, じょういんびー, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid
";

        [TestMethod]
        public void Update_SplitsCombinedUnitWhenPartsAreAvailable()
        {
            // 合体形態が主形態でなければ、Update 時に分離して分離先の形態に戻る
            var src = CreateSrcWithUnits(
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離 パーツＡ パーツＢ"),
                UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));
            UnitCoreFixture.LoadPilots(src, CrewPilots);
            var combined = src.UList.Add("合体後", 0, "味方");
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            var partB = src.UList.Add("パーツＢ", 0, "味方");
            UnitCoreFixture.Board(src, combined, "乗員Ａ");
            UnitCoreFixture.Board(src, combined, "乗員Ｂ");
            Assert.AreEqual("他形態", partA.Status);

            src.UList.Update();

            Assert.AreEqual("他形態", combined.Status);
            Assert.AreEqual("待機", partA.Status);
            Assert.AreEqual("待機", partB.Status);
            Assert.AreEqual(0, combined.CountPilot());
            Assert.AreEqual(1, partA.CountPilot());
            Assert.AreEqual(1, partB.CountPilot());
        }

        [TestMethod]
        public void Update_DoesNotSplitWhenPilotsAreInsufficient()
        {
            // 分離先に必要なパイロット数が足りなければ分離しない
            var src = CreateSrcWithUnits(
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離 パーツＡ パーツＢ"),
                UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));
            UnitCoreFixture.LoadPilots(src, CrewPilots);
            var combined = src.UList.Add("合体後", 0, "味方");
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            src.UList.Add("パーツＢ", 0, "味方");
            UnitCoreFixture.Board(src, combined, "乗員Ａ");

            src.UList.Update();

            Assert.AreEqual("待機", combined.Status);
            Assert.AreEqual("他形態", partA.Status);
        }

        [TestMethod]
        public void Update_CombinesPartsWhenCombinedFormIsMainForm()
        {
            // 合体後の形態が主形態なら、待機中のパーツは Update 時に合体する
            var src = CreateSrcWithUnits(
                UnitBlock("合体後", pilotNum: 2, features: new[] { "分離=分離 パーツＡ パーツＢ", "主形態" }),
                UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));
            UnitCoreFixture.LoadPilots(src, CrewPilots);
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            var partB = src.UList.Add("パーツＢ", 0, "味方");
            var combined = src.UList.Add("合体後", 0, "味方");
            UnitCoreFixture.Board(src, partA, "乗員Ａ");
            UnitCoreFixture.Board(src, partB, "乗員Ｂ");
            Assert.AreEqual("他形態", combined.Status);

            src.UList.Update();

            Assert.AreEqual("待機", combined.Status);
            // 合体を実施したユニットは旧主形態、パートナーは旧形態になる（VB: Combine）
            Assert.AreEqual("旧主形態", partA.Status);
            Assert.AreEqual("旧形態", partB.Status);
            Assert.AreEqual(2, combined.CountPilot());
        }

        [TestMethod]
        public void Update_DoesNotCombineWhenCombinedFormIsNotMainForm()
        {
            // 2体合体で合体後が主形態でなければ合体しない（VB: LLength = 3 かつ 主形態なし）
            var src = CreateSrcWithUnits(
                UnitBlock("合体後", pilotNum: 2, features: "分離=分離 パーツＡ パーツＢ"),
                UnitBlock("パーツＡ", features: "合体=合体 合体後 パーツＢ"),
                UnitBlock("パーツＢ", features: "合体=合体 合体後 パーツＡ"));
            UnitCoreFixture.LoadPilots(src, CrewPilots);
            var partA = src.UList.Add("パーツＡ", 0, "味方");
            var partB = src.UList.Add("パーツＢ", 0, "味方");
            var combined = src.UList.Add("合体後", 0, "味方");
            UnitCoreFixture.Board(src, partA, "乗員Ａ");
            UnitCoreFixture.Board(src, partB, "乗員Ｂ");

            src.UList.Update();

            Assert.AreEqual("他形態", combined.Status);
            Assert.AreEqual("待機", partA.Status);
            Assert.AreEqual("待機", partB.Status);
        }

        [TestMethod]
        public void Update_RemovesDummyPilot()
        {
            // 愛称が「パイロット不在」のダミーパイロットは降ろされる
            var src = CreateSrcWithUnits(UnitBlock("テスト機"));
            UnitCoreFixture.LoadPilots(src, @"
ダミー
パイロット不在, だみー, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid
");
            var u = src.UList.Add("テスト機", 0, "味方");
            UnitCoreFixture.Board(src, u, "ダミー");
            Assert.AreEqual(1, u.CountPilot());

            src.UList.Update();

            Assert.AreEqual(0, u.CountPilot());
        }

        [TestMethod]
        public void Update_DeletesBerserkPilot()
        {
            // 暴走時パイロットは Update でパイロットリストから削除される
            var src = CreateSrcWithUnits(UnitBlock("テスト機", features: "暴走時パイロット=暴走体"));
            UnitCoreFixture.LoadPilots(src, @"
暴走体
暴走体, ぼうそうたい, 男性, 汎用, AAAA, 100
特殊能力なし
100, 100, 100, 100, 100, 100, 普通
ＳＰなし
-.bmp, -.mid
");
            src.UList.Add("テスト機", 0, "味方");
            src.PList.Add("暴走体", 1, "味方", gid: "");
            Assert.IsTrue(src.PList.IsDefined("暴走体"));

            src.UList.Update();

            Assert.IsFalse(src.PList.IsDefined("暴走体"));
        }
    }
}
