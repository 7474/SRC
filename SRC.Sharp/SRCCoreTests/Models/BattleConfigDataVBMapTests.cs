using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.IO;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// BattleConfigData・BattleConfigDataList を元の SRC と突き合わせたテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class BattleConfigDataVBMapTests
    {
        [TestMethod]
        public void Load_NameWithoutExpression_ConfigCalcIsEmptyAndCalculatesZero()
        {
            // VB の Class_Initialize は ConfigCalc = ""。式の行が無いデータも Calculate は 0 になる
            var src = new SRC { GUI = new MockGUI() };
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("ダメージ\n\n")))
            {
                src.BCList.Load("battle.txt", stream);
            }

            var cd = src.BCList.Item("ダメージ");
            Assert.AreEqual("", cd.ConfigCalc);
            Assert.AreEqual(0d, cd.Calculate());
            Assert.IsFalse(src.Event.BCVariable.IsConfig);
        }
    }
}
