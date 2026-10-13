using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SRCCore.Expressions;
using SRCCore.TestLib;

namespace SRCCore.Tests
{
    /// <summary>
    /// セーブデータのグローバル変数（SRC.SaveData / LoadData と同じく Newtonsoft.Json で書いて読み戻す）
    /// </summary>
    [TestClass]
    public class SRCSaveLoadRoundTripTests
    {
        [TestMethod]
        public void GlobalVariables_AreRestored()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            src.Expression.DefineGlobalVariable("クリア数");
            src.Expression.SetVariableAsDouble("クリア数", 3d);

            var json = JsonConvert.SerializeObject(new SRCSaveData { GlobalVariableList = src.Event.GlobalVariableList });
            var data = JsonConvert.DeserializeObject<SRCSaveData>(json);

            var loaded = new SRC { GUI = new MockGUI() };
            loaded.Event.GlobalVariableList = data.GlobalVariableList;
            Assert.AreEqual("next.eve", loaded.Expression.GetValueAsString("次ステージ"));
            Assert.AreEqual(3d, loaded.Expression.GetValueAsDouble("クリア数"));
        }

        [TestMethod]
        public void VarData_KeepsNameTypeAndValues()
        {
            var json = JsonConvert.SerializeObject(new VarData("名前", ValueType.StringType, "テスト", 0d));
            var restored = JsonConvert.DeserializeObject<VarData>(json);
            Assert.AreEqual("名前", restored.Name);
            Assert.AreEqual(ValueType.StringType, restored.VariableType);
            Assert.AreEqual("テスト", restored.StringValue);
        }
    }
}
