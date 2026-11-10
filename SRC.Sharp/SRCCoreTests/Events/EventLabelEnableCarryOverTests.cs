using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SRCCore.Coverage.Tests;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// ClearEvent で無効にしたイベントラベルの有効・無効の持ち越し（#1529）。
    /// </summary>
    [TestClass]
    public class EventLabelEnableCarryOverTests
    {
        private static readonly string[] SystemLib =
        {
            "ターン 1 味方:",
            "Exit",
            "ターン 1 敵:",
            "Exit",
        };

        private static readonly string[] Scenario =
        {
            // 先頭行のラベルは IsEventDefined が見落とすため、ダミーのラベルを先頭に置く
            "準備:",
            "Exit",
            "",
            "ターン 1 味方:",
            "Exit",
            "",
            "ターン 2 味方:",
            "Exit",
        };

        private static string PrepareScenario(SrcPlayDataSaveFixture f)
        {
            // シナリオの Lib\include.eve はシステム側のイベントデータとして読む
            f.Write("Lib/include.eve", SystemLib);
            var path = f.Write("test.eve", Scenario);
            f.Src.ScenarioFileName = path;
            f.Src.Event.LoadEventData(path, "");
            f.Src.Map.SetMapSize(3, 3);
            f.Src.Map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            return path;
        }

        private static bool[] Enables(SRC src, string data)
        {
            return src.Event.colEventLabelList.List.Where(x => x.Data == data).Select(x => x.Enable).ToArray();
        }

        private static byte[] Dump(SrcPlayDataSaveFixture f)
        {
            var stream = new MemoryStream();
            f.Src.DumpData(stream, SRCSaveKind.Suspend);
            return stream.ToArray();
        }

        [TestMethod]
        public void RestoreData_RestoresEachLabelEnable_ForSameLabelText()
        {
            // VB: Event.bas DumpEventData は colEventLabelList の順に lab.Enable を書き、
            // RestoreEventData は順番に戻す。同じ文字列のラベルの片方だけを無効にしたら、片方だけが無効のまま
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            CollectionAssert.AreEqual(new[] { true, true }, Enables(f.Src, "ターン 1 味方"));
            // シナリオ側（後に登録した方）だけを無効にする
            f.Src.Event.colEventLabelList.List.Last(x => x.Data == "ターン 1 味方").Enable = false;
            var bytes = Dump(f);

            f.NewSession();
            f.Src.RestoreData(new MemoryStream(bytes), SRCSaveKind.Suspend);

            CollectionAssert.AreEqual(new[] { true, false }, Enables(f.Src, "ターン 1 味方"));
            CollectionAssert.AreEqual(new[] { true }, Enables(f.Src, "ターン 2 味方"));
        }

        [TestMethod]
        public void RestoreData_LabelsMoreThanSaved_AreEnabled()
        {
            // VB: RestoreEventData は保存したものより多いラベルを True にする
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var json = JObject.Parse(Encoding.UTF8.GetString(Dump(f)));
            var enables = (JArray)json[nameof(SRCSuspendData.EventLabelEnabled)];
            Assert.IsNotNull(enables);
            Assert.AreEqual(f.Src.Event.colEventLabelList.Count, enables.Count);
            // 後ろの 2 つ（シナリオ側）を持たず、持っているものはすべて無効のセーブにする
            json[nameof(SRCSuspendData.EventLabelEnabled)] = new JArray(enables.Take(enables.Count - 2).Select(x => false));

            f.NewSession();
            f.Src.RestoreData(new MemoryStream(Encoding.UTF8.GetBytes(json.ToString())), SRCSaveKind.Suspend);

            var labels = f.Src.Event.colEventLabelList.List;
            CollectionAssert.AreEqual(
                Enumerable.Repeat(false, enables.Count - 2).Concat(new[] { true, true }).ToArray(),
                labels.Select(x => x.Enable).ToArray());
            CollectionAssert.AreEqual(new[] { false, true }, Enables(f.Src, "ターン 1 味方"));
        }

        [TestMethod]
        public void RestoreData_OldSaveWithDisableEventLabelsOnly_DisablesByText()
        {
            // 古いセーブ（DisableEventLabels だけのもの）は、これまでどおりラベルの文字列で戻す
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var json = JObject.Parse(Encoding.UTF8.GetString(Dump(f)));
            json.Remove(nameof(SRCSuspendData.EventLabelEnabled));
            json[nameof(SRCSuspendData.DisableEventLabels)] = new JArray("ターン 2 味方");

            f.NewSession();
            f.Src.RestoreData(new MemoryStream(Encoding.UTF8.GetBytes(json.ToString(Formatting.None))), SRCSaveKind.Suspend);

            CollectionAssert.AreEqual(new[] { true, true }, Enables(f.Src, "ターン 1 味方"));
            CollectionAssert.AreEqual(new[] { false }, Enables(f.Src, "ターン 2 味方"));
        }

        [TestMethod]
        public void LoadEventData_KeepsSystemEventLabelEnable_AcrossStages()
        {
            // VB: Event.bas LoadEventData はシステム側のイベントラベル（LineNum <= SysEventDataSize）を消さずに残すので、
            // ClearEvent で無効にしたシステム側のイベントラベルは次のステージでも無効のまま
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            f.Src.Event.colEventLabelList.List.First(x => x.Data == "ターン 1 敵").Enable = false;

            var stage2 = f.Write("stage2.eve", "準備:", "Exit", "ターン 1 敵:", "Exit");
            f.Src.Event.ClearEventData();
            f.Src.Event.LoadEventData(stage2, "");

            // システム側は無効のまま、シナリオ側の同じ文字列のラベルは有効
            CollectionAssert.AreEqual(new[] { false, true }, Enables(f.Src, "ターン 1 敵"));
            CollectionAssert.AreEqual(new[] { true }, Enables(f.Src, "ターン 1 味方"));
        }

        [TestMethod]
        public void LoadEventData_ScenarioLibReload_ResetsSystemEventLabelEnable()
        {
            // VB: 最初のシナリオのロードでシナリオの Lib を読み直すときは、イベントラベルをすべて消して登録し直す
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Lib/汎用戦闘アニメ/include.eve", SystemLib);
            f.Src.Event.LoadEventData("", "システム");
            f.Src.Event.colEventLabelList.List.First(x => x.Data == "ターン 1 敵").Enable = false;

            var path = f.Write("test.eve", Scenario);
            f.Src.Event.LoadEventData(path, "");

            CollectionAssert.AreEqual(new[] { true }, Enables(f.Src, "ターン 1 敵"));
        }
    }
}
