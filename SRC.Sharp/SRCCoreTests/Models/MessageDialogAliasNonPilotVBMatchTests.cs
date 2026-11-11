using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// エリアス・ダイアログ・メッセージ・ノンパイロットのデータを、元の SRC（AliasData*.cls・DialogData*.cls・Dialog.cls・
    /// MessageData*.cls・NonPilotData*.cls）の動作に合わせたテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class MessageDialogAliasNonPilotVBMatchTests
    {
        private static SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static Stream ToStream(string content) => new MemoryStream(Encoding.UTF8.GetBytes(content));

        private static Units.Unit CreatePilotedUnit(SRC src, string name, string party)
        {
            var pd = src.PDList.Add(name + "のパイロット");
            pd.SP = 100;
            var p = src.PList.Add(name + "のパイロット", 1, party);
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            var u = src.UList.Add(name, 0, party);
            p.Ride(u);
            u.Update();
            u.Status = "出撃";
            u.x = 1;
            u.y = 1;
            return u;
        }

        private static void SetUpMap(SRC src)
        {
            src.Map.MapWidth = 1;
            src.Map.MapHeight = 1;
            src.Map.MapData = new Src2DArray<Maps.MapCell>(1, 1);
            src.Map.MapData[1, 1] = new Maps.MapCell { X = 1, Y = 1, UnderTerrain = new TerrainData { Name = "平地" } };
            src.Map.MapDataForUnit = new Src2DArray<Units.Unit>(1, 1);
        }

        private static List<MessageDataItem> Items(params (string situation, string message)[] entries)
        {
            var list = new List<MessageDataItem>();
            foreach (var (s, m) in entries)
            {
                list.Add(new MessageDataItem(s, m));
            }
            return list;
        }

        // ──────────────────────────────────────────────
        // AliasDataList.Load・AliasData.AddAlias
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AliasLoad_NameAtEndOfFile_IsNotError()
        {
            // 元の SRC はファイルの終わりではエリアスの数を調べずに読み込みを終える
            var src = CreateSrc();
            src.ALDList.Load("alias.txt", ToStream("攻撃系\n格闘\n\n最後"));
            Assert.IsTrue(src.ALDList.IsDefined("最後"));
            Assert.AreEqual(0, src.ALDList.Item("最後").Count);
        }

        [TestMethod]
        public void AliasLoad_NonNumericLevel_StopsLoading()
        {
            // 元の SRC はレベルの CDbl が実行時エラーになり、原因を書かないデータエラーで読み込みを打ち切る
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => src.ALDList.Load("alias.txt", ToStream("攻撃系\n格闘Lvx\n\n")));
            Assert.AreEqual("", ex.InvalidDataList[0].msg);
            Assert.AreEqual("攻撃系", ex.InvalidDataList[0].dname);
        }

        // ──────────────────────────────────────────────
        // Dialog.IsAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DialogIsAvailable_FacePattern_RemovesWholeBracket()
        {
            // 「テスト(笑)」がノンパイロットとして定義され、「テスト」がパイロットなら、表情パターンとみなして「テスト」を調べる。
            // 「テスト」のパイロットが作られていないので使えない
            var src = CreateSrc();
            SetUpMap(src);
            var u = CreatePilotedUnit(src, "自機", "味方");
            src.PDList.Add("テスト");
            src.NPDList.Add("テスト(笑)");
            var d = new Dialog(src);
            d.AddMessage("テスト(笑)", "こんにちは");
            Assert.IsFalse(d.IsAvailable(u, false));
        }

        // ──────────────────────────────────────────────
        // SelectMessage の乱数の呼ばれ方
        // ──────────────────────────────────────────────

        private static (SRC src, Units.Unit u) SetUpTarget()
        {
            var src = CreateSrc();
            SetUpMap(src);
            var u = CreatePilotedUnit(src, "自機", "味方");
            var t = CreatePilotedUnit(src, "敵機", "敵");
            src.Commands.SelectedUnit = t;
            src.Commands.SelectedTarget = u;
            return (src, u);
        }

        [TestMethod]
        public void SelectMessage_MustUseTarget_StillCallsDice2()
        {
            // 元の SRC は Dice(list_num) の後、「挑発」などでも Dice(2) を呼ぶ（Or は短絡しない）
            var (src, u) = SetUpTarget();
            var items = Items(("挑発(対敵機のパイロット)", "相手限定"), ("挑発", "汎用"));
            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var before = GeneralLib.RndIndex;
            var result = items.SelectMessage(src, "挑発", u);
            Assert.AreEqual("相手限定", result.Message);
            Assert.AreEqual(1, GeneralLib.RndIndex - before);
        }

        [TestMethod]
        public void SelectMessage_TargetOnly_CallsDice2AndReturnsTarget()
        {
            // 汎用メッセージが無くても Dice(2) を呼び、外れても選んだ相手限定メッセージを返す
            var (src, u) = SetUpTarget();
            var items = Items(("回避(対敵機のパイロット)", "相手限定"));
            for (var seed = 1; seed <= 20; seed++)
            {
                GeneralLib.RndSeed = seed;
                GeneralLib.RndReset();
                var before = GeneralLib.RndIndex;
                var result = items.SelectMessage(src, "回避", u);
                Assert.AreEqual("相手限定", result.Message);
                Assert.AreEqual(1, GeneralLib.RndIndex - before);
            }
        }

        [TestMethod]
        public void SelectMessage_TargetWeaponClassBundle_UsesWholeBundle()
        {
            // 元の SRC（MessageData）は相手の武器の属性を GetClassBundle で取り出すので、「弱火」で「(対弱火属性)」になる
            var (src, u) = SetUpTarget();
            var t = src.Commands.SelectedUnit;
            var w = t.Data.AddWeapon("火炎");
            w.Class = "弱火";
            w.Power = 1000;
            w.MaxRange = 1;
            w.MinRange = 1;
            w.Precision = 0;
            t.Update();
            src.Commands.SelectedWeapon = 1;
            var items = Items(("回避(対弱火属性)", "相手限定"));
            var result = items.SelectMessage(src, "回避", u);
            Assert.IsNotNull(result);
            Assert.AreEqual("相手限定", result.Message);
        }

        // ──────────────────────────────────────────────
        // DialogDataList.Load
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DialogLoad_SituationAtEndOfFile_AddsEmptyDialog()
        {
            // 元の SRC はシチュエーションを読んだ後にファイルの終わりを調べるので、メッセージの無いダイアログも登録する
            var src = CreateSrc();
            src.DDList.Load("pilot_dialog.txt", ToStream("テスト\n攻撃"));
            Assert.AreEqual(1, src.DDList.Item("テスト").CountDialog());
        }

        [TestMethod]
        public void DialogLoad_NextSituationAtEndOfFile_AddsDialog()
        {
            var src = CreateSrc();
            src.DDList.Load("pilot_dialog.txt", ToStream("テスト\n攻撃\nシステム,やあ\n回避"));
            Assert.AreEqual(2, src.DDList.Item("テスト").CountDialog());
        }

        [TestMethod]
        public void DialogLoad_TrailingBlankLines_AddNoData()
        {
            var src = CreateSrc();
            src.DDList.Load("pilot_dialog.txt", ToStream("テスト\n攻撃\nシステム,やあ\n\n\n\n"));
            Assert.AreEqual(1, src.DDList.Count());
        }

        // ──────────────────────────────────────────────
        // MessageDataList.Load
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageLoad_TrailingBlankLines_AddNoData()
        {
            var src = CreateSrc();
            var list = new MessageDataList(src);
            list.Load("pilot_message.txt", false, ToStream("テスト\n攻撃,やあ\n\n\n\n"));
            Assert.AreEqual(1, list.Count());
        }

        [TestMethod]
        public void MessageLoad_AnimationFile_AppendsDuplicates()
        {
            // 元の SRC はファイル名に animation.txt を含めば特殊効果データとして読み、同じ名前のデータに追加する
            var src = CreateSrc();
            var list = new MessageDataList(src);
            list.Load(@"Data\ext_animation.txt", false, ToStream("汎用\n攻撃,a\n\n汎用\n回避,b\n"));
            Assert.AreEqual(2, list.Item("汎用").Items.Count);
        }

        // ──────────────────────────────────────────────
        // NonPilotData.Nickname・NonPilotDataList.Load
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NonPilotNickname_Hero_UsesVariable()
        {
            var src = CreateSrc();
            var npd = src.NPDList.Add("主人公キャラ");
            npd.Nickname = "主人公";
            src.Expression.SetVariableAsString("主人公愛称", "タロウ");
            Assert.AreEqual("タロウ", npd.Nickname);
            Assert.AreEqual("主人公", npd.Nickname0);
        }

        [TestMethod]
        public void NonPilotNickname_FacePattern_AppliesPilotNicknameFeature()
        {
            // 表情パターンのノンパイロットは、そのパイロットのユニットの「パイロット愛称」を当てる
            var src = CreateSrc();
            SetUpMap(src);
            var u = CreatePilotedUnit(src, "自機", "味方");
            u.Data.AddFeature("パイロット愛称=隊長$(愛称)");
            u.Update();
            var npd = src.NPDList.Add("自機のパイロット(笑)");
            npd.Nickname = "ジキ";
            Assert.AreEqual("隊長ジキ", npd.Nickname);
        }

        [TestMethod]
        public void NonPilotLoad_NameDiffersOnlyInCase_StopsLoading()
        {
            // 元の SRC は Collection に同じキーを追加しようとして実行時エラーになる
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() =>
                src.NPDList.Load("non_pilot.txt", ToStream("abc\nえー,a.bmp\n\nABC\nびー,b.bmp\n")));
            Assert.AreEqual("", ex.InvalidDataList[0].msg);
        }
    }
}
