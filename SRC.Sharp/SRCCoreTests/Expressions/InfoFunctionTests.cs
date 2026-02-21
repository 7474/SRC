using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す Info 関数のユニットテスト
    /// </summary>
    [TestClass]
    public class InfoFunctionTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Models.PilotData CreatePilotData(SRC src, string name)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return pd;
        }

        private Pilots.Pilot CreatePilot(SRC src, string name, int level = 1)
        {
            CreatePilotData(src, name);
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // パイロットデータ: 名称
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_PilotData_Name_ReturnsName()
        {
            var src = CreateSrc();
            CreatePilotData(src, "アムロ");
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"アムロ\",\"名称\")");
            Assert.AreEqual("アムロ", result);
        }

        [TestMethod]
        public void Info_PilotData_Name_NonExistent_ReturnsEmpty()
        {
            var src = CreateSrc();
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"存在しない\",\"名称\")");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // パイロットデータ: 経験値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_PilotData_ExpValue_ReturnsDefault()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "テスト");
            pd.ExpValue = 0;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"テスト\",\"経験値\")");
            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void Info_PilotData_ExpValue_ReturnsSetValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "強敵");
            pd.ExpValue = 200;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"強敵\",\"経験値\")");
            Assert.AreEqual("200", result);
        }

        // ──────────────────────────────────────────────
        // パイロットデータ: 性別
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_PilotData_Sex_ReturnsSex()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "花子");
            pd.Sex = "女";
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"花子\",\"性別\")");
            Assert.AreEqual("女", result);
        }

        [TestMethod]
        public void Info_PilotData_Sex_EmptySex_ReturnsEmpty()
        {
            var src = CreateSrc();
            CreatePilotData(src, "無性別");
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"無性別\",\"性別\")");
            // Sex未設定の場合は空文字またはnull
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        // ──────────────────────────────────────────────
        // パイロット: 名称
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_Pilot_Name_ReturnsPilotName()
        {
            var src = CreateSrc();
            CreatePilot(src, "カミーユ");
            var result = src.Expression.GetValueAsString("Info(\"パイロット\",\"カミーユ\",\"名称\")");
            Assert.AreEqual("カミーユ", result);
        }

        [TestMethod]
        public void Info_Pilot_Name_NonExistent_ReturnsEmpty()
        {
            var src = CreateSrc();
            var result = src.Expression.GetValueAsString("Info(\"パイロット\",\"存在しないパイロット\",\"名称\")");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // パイロット: 格闘・射撃
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_Pilot_Infight_ReturnsValue()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "格闘家");
            // パイロットの格闘値はData.Infight(=0)を元にUpdate()で計算される
            // レベル1時: InfightBase = Data.Infight + lv * ... の最低値は1以上
            var result = src.Expression.GetValueAsDouble("Info(\"パイロット\",\"格闘家\",\"格闘\")");
            Assert.IsTrue(result >= 0d);
        }

        [TestMethod]
        public void Info_PilotData_Infight_ReturnsValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "格闘データ");
            pd.Infight = 150;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"格闘データ\",\"格闘\")");
            Assert.AreEqual("150", result);
        }

        [TestMethod]
        public void Info_PilotData_Shooting_ReturnsValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "射撃データ");
            pd.Shooting = 180;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"射撃データ\",\"射撃\")");
            Assert.AreEqual("180", result);
        }

        // ──────────────────────────────────────────────
        // パイロットデータ: 命中・回避
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_PilotData_Hit_ReturnsValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "命中テスト");
            pd.Hit = 120;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"命中テスト\",\"命中\")");
            Assert.AreEqual("120", result);
        }

        [TestMethod]
        public void Info_PilotData_Dodge_ReturnsValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "回避テスト");
            pd.Dodge = 130;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"回避テスト\",\"回避\")");
            Assert.AreEqual("130", result);
        }

        // ──────────────────────────────────────────────
        // 先頭引数が空の場合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_EmptyFirstParam_ReturnsEmpty()
        {
            var src = CreateSrc();
            var result = src.Expression.GetValueAsString("Info(\"\",\"アムロ\",\"名称\")");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // パイロットデータ: 技量・反応
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Info_PilotData_Technique_ReturnsValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "技量テスト");
            pd.Technique = 140;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"技量テスト\",\"技量\")");
            Assert.AreEqual("140", result);
        }

        [TestMethod]
        public void Info_PilotData_Intuition_ReturnsValue()
        {
            var src = CreateSrc();
            var pd = CreatePilotData(src, "反応テスト");
            pd.Intuition = 110;
            var result = src.Expression.GetValueAsString("Info(\"パイロットデータ\",\"反応テスト\",\"反応\")");
            Assert.AreEqual("110", result);
        }
    }
}
