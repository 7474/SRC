using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Models;
using SRCCore.VB;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// SpecialPowerDataList.Load (sp.txt の読み込み) と SpecialPowerData.SetEffect の解析。
    /// 元の SRC の SpecialPowerDataList.Load / SpecialPowerData.SetEffect に対応。
    /// </summary>
    [TestClass]
    public class SpecialPowerDataListLoadTests
    {
        private static SpecialPowerDataList Load(SRC src, string text)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                src.SPDList.Load("sp.txt", stream);
            }
            return src.SPDList;
        }

        [TestMethod]
        public void Load_ParsesAllFields()
        {
            // 名称, 読み仮名 / 短縮形, 消費ＳＰ, 対象, 効果時間, 適用条件, 使用条件, アニメ / 効果 / 解説
            var src = new SRC();
            var list = Load(src,
                "熱血, ねっけつ\n" +
                "熱, 40, 自分, 攻撃, -, -, 熱血アニメ\n" +
                "ダメージ増加Lv1\n" +
                "次の攻撃のダメージが2倍\n");

            Assert.AreEqual(1, list.Count());
            var sd = list.Item("熱血");
            Assert.AreEqual("熱血", sd.Name);
            Assert.AreEqual("ねっけつ", sd.KanaName);
            Assert.AreEqual("熱", sd.ShortName);
            Assert.AreEqual(40, sd.SPConsumption);
            Assert.AreEqual("自分", sd.TargetType);
            Assert.AreEqual("攻撃", sd.Duration);
            Assert.AreEqual("-", sd.NecessaryCondition);
            Assert.AreEqual("熱血アニメ", sd.Animation);
            Assert.AreEqual(1, sd.CountEffect());
            Assert.AreEqual("ダメージ増加", sd.Effects[0].strEffectType);
            Assert.AreEqual(1d, sd.Effects[0].dblEffectLevel);
            Assert.AreEqual("次の攻撃のダメージが2倍", sd.Comment);
        }

        [TestMethod]
        public void Load_OmittedKanaAndAnimation_UseDefaults()
        {
            // 読み仮名省略時は名称のひらがな化、アニメ省略時は名称がアニメ名になる
            var src = new SRC();
            var list = Load(src,
                "ガッツ\n" +
                "ガ, 20, 自分, 即効, -, -\n" +
                "ＨＰ回復Lv3\n" +
                "ＨＰを30%回復\n");

            var sd = list.Item("ガッツ");
            Assert.AreEqual("がっつ", sd.KanaName);
            Assert.AreEqual("ガッツ", sd.Animation);
            Assert.AreEqual("即効", sd.Duration);
            Assert.AreEqual(3d, sd.Effects[0].dblEffectLevel);
        }

        [TestMethod]
        public void Load_MultipleEntries_SkipsBlankLinesAndComments()
        {
            var src = new SRC();
            var list = Load(src,
                "# コメント行\n" +
                "加速\n" +
                "加, 10, 自分, ターン, -, -\n" +
                "移動力強化Lv2\n" +
                "移動力+2\n" +
                "\n" +
                "\n" +
                "再動\n" +
                "再, 70, 味方, 即効, 支援, -\n" +
                "行動数回復\n" +
                "行動済みユニットを再行動\n");

            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.IsDefined("加速"));
            Assert.IsTrue(list.IsDefined("再動"));
            Assert.AreEqual("支援", list.Item("再動").NecessaryCondition);
            Assert.AreEqual("味方", list.Item("再動").TargetType);
        }

        [TestMethod]
        public void Load_Redefinition_ReplacesPreviousData()
        {
            // 同名のデータが再定義された場合は後の定義で置き換える
            var src = new SRC();
            var list = Load(src,
                "必中\n" +
                "必, 15, 自分, ターン, -, -\n" +
                "絶対命中\n" +
                "旧\n" +
                "必中\n" +
                "必, 25, 自分, ターン, -, -\n" +
                "絶対命中\n" +
                "新\n");

            Assert.AreEqual(1, list.Count());
            Assert.AreEqual(25, list.Item("必中").SPConsumption);
            Assert.AreEqual("新", list.Item("必中").Comment);
        }

        [TestMethod]
        public void Load_NonNumericCost_IsReportedAsDataErrorAndContinues()
        {
            // 消費ＳＰが数値でない場合はデータエラーとして記録し、読み込みは継続する
            var src = new SRC();
            var list = Load(src,
                "閃き\n" +
                "閃, たくさん, 自分, 回避, -, -\n" +
                "絶対回避\n" +
                "次の攻撃を回避\n");

            Assert.IsTrue(list.IsDefined("閃き"));
            Assert.AreEqual(0, list.Item("閃き").SPConsumption);
            Assert.AreEqual(1, src.DataErrors.Count);
            Assert.IsTrue(src.DataErrors[0].msg.Contains("消費ＳＰ"));
        }

        [TestMethod]
        public void Load_InvalidName_Throws()
        {
            // 名称に半角スペース・全角括弧・" は使用できない
            foreach (var name in new[] { "熱 血", "熱血（改）", "熱\"血" })
            {
                var src = new SRC();
                var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src,
                    name + "\n" +
                    "熱, 40, 自分, 攻撃, -, -\n" +
                    "ダメージ増加Lv1\n" +
                    "解説\n"), name);
                Assert.IsTrue(ex.InvalidDataList[0].msg.StartsWith("名称に"), name);
            }
        }

        [TestMethod]
        public void Load_MissingFields_Throws()
        {
            // 必須項目の欠落はエラー
            AssertLoadError("熱血\n熱\n効果\n解説\n", "消費ＳＰが抜けています。");
            AssertLoadError("熱血\n熱, 40\n効果\n解説\n", "対象が抜けています。");
            AssertLoadError("熱血\n熱, 40, 自分, 攻撃\n効果\n解説\n", "効果時間が抜けています。");
            AssertLoadError("熱血\n熱, 40, 1, 攻撃, -, -\n効果\n解説\n", "対象が間違っています。");
            AssertLoadError("熱血\n熱, 40, 自分, 1, -\n効果\n解説\n", "効果時間が間違っています。");
            AssertLoadError("熱血, ねっけつ, 余分\n熱, 40, 自分, 攻撃, -, -\n効果\n解説\n", "読み仮名の後に余分なデータが指定されています。");
            AssertLoadError("熱血\n熱, 40, 自分, 攻撃, -\n効果\n解説\n", "使用条件が抜けています。");
            AssertLoadError("熱血\n熱, 40, 自分, 攻撃, -, -\n効果\n", "解説が指定されていません。");
        }

        private static void AssertLoadError(string text, string expectedMsg)
        {
            var src = new SRC();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, text), expectedMsg);
            Assert.AreEqual(expectedMsg, ex.InvalidDataList[0].msg);
        }

        // ──────────────────────────────────────────────
        // SetEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetEffect_ParsesLevelDataAndPlainEffects()
        {
            // 「効果名」「効果名Lvn」「効果名=データ」を空白区切りで複数指定できる
            var sd = new SpecialPowerData(new SRC()) { Name = "奇襲" };
            sd.SetEffect("ＨＰ回復Lv10 絶対命中 スペシャルパワー=加速 ＥＮ回復Lv-2.5");

            Assert.AreEqual(4, sd.CountEffect());
            Assert.AreEqual("ＨＰ回復", sd.Effects[0].strEffectType);
            Assert.AreEqual(10d, sd.Effects[0].dblEffectLevel);
            Assert.IsNull(sd.Effects[0].strEffectData);

            Assert.AreEqual("絶対命中", sd.Effects[1].strEffectType);
            Assert.AreEqual(Constants.DEFAULT_LEVEL, sd.Effects[1].dblEffectLevel);

            Assert.AreEqual("スペシャルパワー", sd.Effects[2].strEffectType);
            Assert.AreEqual("加速", sd.Effects[2].strEffectData);
            Assert.AreEqual(Constants.DEFAULT_LEVEL, sd.Effects[2].dblEffectLevel);

            Assert.AreEqual("ＥＮ回復", sd.Effects[3].strEffectType);
            Assert.AreEqual(-2.5d, sd.Effects[3].dblEffectLevel);
        }

        [TestMethod]
        public void SetEffect_LevelWithQuotedData_ParsesInnerLevelAndData()
        {
            // 「効果Lvn="種類Lvm=データ"」形式: 外側のレベルと、データ部分は「種類 m データ」に正規化
            var sd = new SpecialPowerData(new SRC()) { Name = "強化" };
            sd.SetEffect("付加Lv2=\"特殊能力Lv3=追加データ\"");

            var e = sd.Effects.Single();
            Assert.AreEqual("付加", e.strEffectType);
            Assert.AreEqual(2d, e.dblEffectLevel);
            Assert.AreEqual("特殊能力 3 追加データ", e.strEffectData);
        }

        [TestMethod]
        public void SetEffect_DataWithInnerData_JoinsTypeAndData()
        {
            // 「効果=種類=データ」はデータ部分が「種類 データ」となる
            var sd = new SpecialPowerData(new SRC()) { Name = "テスト" };
            sd.SetEffect("イベント=サブ=引数 状態=攻撃不能Lv2");

            Assert.AreEqual("イベント", sd.Effects[0].strEffectType);
            Assert.AreEqual("サブ  引数", sd.Effects[0].strEffectData);
            Assert.AreEqual("状態", sd.Effects[1].strEffectType);
            Assert.AreEqual("攻撃不能 2", sd.Effects[1].strEffectData);
        }

        [TestMethod]
        public void SetEffect_SPNamedFuka_DefaultsInnerLevel()
        {
            // スペシャルパワー名が「付加」でデータ中のレベルが省略された場合は DEFAULT_LEVEL を補う
            var sd = new SpecialPowerData(new SRC()) { Name = "付加" };
            sd.SetEffect("付加=特殊能力");

            Assert.AreEqual("特殊能力 " + SrcFormatter.Format(Constants.DEFAULT_LEVEL), sd.Effects[0].strEffectData);
        }

        // ──────────────────────────────────────────────
        // IsEffectAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsEffectAvailable_FindsDirectAndNestedEffects()
        {
            // 「スペシャルパワー=X」効果は X の効果も持っているとみなす
            var src = new SRC();
            var kasoku = src.SPDList.Add("加速");
            kasoku.SetEffect("移動力強化Lv2");
            var kishuu = src.SPDList.Add("奇襲");
            kishuu.SetEffect("絶対命中 スペシャルパワー=加速");

            Assert.IsTrue(kishuu.IsEffectAvailable("絶対命中"));
            Assert.IsTrue(kishuu.IsEffectAvailable("移動力強化"));
            Assert.IsFalse(kishuu.IsEffectAvailable("復活"));
            Assert.IsFalse(kasoku.IsEffectAvailable("絶対命中"));
        }

        [TestMethod]
        public void List_AddDeleteItem()
        {
            var src = new SRC();
            src.SPDList.Add("熱血");
            src.SPDList.Add("必中");
            Assert.AreEqual(2, src.SPDList.Count());
            Assert.AreSame(src.SPDList.Items[1], src.SPDList.Item("必中"));

            src.SPDList.Delete("熱血");
            Assert.AreEqual(1, src.SPDList.Count());
            Assert.IsFalse(src.SPDList.IsDefined("熱血"));
            Assert.IsNull(src.SPDList.Item("熱血"));
        }
    }
}
