using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainData・TerrainDataList を元の SRC（TerrainData.cls・TerrainDataList.cls）と突き合わせたテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class TerrainDataListVBMapTests
    {
        private static SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static void Load(SRC src, string data)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                src.TDList.Load("terrain.txt", stream);
            }
        }

        [TestMethod]
        public void Load_ThreeFeaturesOnOneLine_SplitsAtFirstComma()
        {
            // VB は引用符の外の最初の「,」で Exit For する。3 つ以上の地形効果を 1 行に書いても 1 つずつ登録する
            var src = CreateSRC();
            Load(src, "1\n草原, plain\n陸, 1, 0, 0\nＨＰ回復Lv1, ＥＮ回復Lv2, 移動停止\n");

            var td = src.TDList.Item(1);
            Assert.AreEqual(3, td.CountFeature());
            Assert.AreEqual(1d, td.FeatureLevel("ＨＰ回復"));
            Assert.AreEqual(2d, td.FeatureLevel("ＥＮ回復"));
            Assert.IsTrue(td.IsFeatureAvailable("移動停止"));
        }

        [TestMethod]
        public void ItemByName_SameName_ReturnsFirstRegistered()
        {
            // VB の ChangeTerrain などは OrderedID（登録の順）で名前を探す
            var src = CreateSRC();
            Load(src, "5\n草原, plain\n陸, 1, 0, 0\n\n2\n草原, plain2\n陸, 2, 0, 0\n");

            Assert.AreEqual(2, src.TDList.Count);
            Assert.AreEqual(5, src.TDList.OrderedID(1));
            Assert.AreEqual(2, src.TDList.OrderedID(2));
            Assert.AreEqual(5, src.TDList.ItemByName("草原").ID);
        }

        [TestMethod]
        public void Load_Redefine_KeepsOrderAndClearsFeatures()
        {
            // 定義済みの番号は .Clear して上書きする。登録の順は変わらない
            var src = CreateSRC();
            Load(src, "5\n草原, plain\n陸, 1, 0, 0\n移動停止\n\n3\n森, forest\n陸, 2, 0, 0\n");
            var td = src.TDList.Item(5);
            Load(src, "5\n平原, plain\n陸, 1, 0, 0\n");

            Assert.AreSame(td, src.TDList.Item(5));
            Assert.AreEqual(2, src.TDList.Count);
            Assert.AreEqual(5, src.TDList.OrderedID(1));
            Assert.AreEqual("平原", td.Name);
            Assert.AreEqual(0, td.CountFeature());
        }

        [TestMethod]
        public void IsDefined_OutOfRange_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.TDList.IsDefined(-1));
            Assert.IsFalse(src.TDList.IsDefined(5000));
            Assert.IsFalse(src.TDList.IsDefined(0));
        }

        [TestMethod]
        public void AddFeature_WithoutData_StrDataIsEmpty()
        {
            // VB の String の既定値は ""
            var td = new TerrainData();
            td.AddFeature("移動停止");
            Assert.AreEqual("", td.colFeature["移動停止"].StrData);
        }

        [TestMethod]
        public void Load_MissingBitmap_DataNameIsEmptyLikeVB()
        {
            // VB は名称を切り出す前のエラーでは data_name が空のまま DataErrorMessage を呼ぶ
            var src = CreateSRC();
            var ex = Assert.ThrowsExactly<InvalidSrcDataException>(() => Load(src, "1\n草原\n"));
            Assert.AreEqual("", ex.InvalidDataList.First().dname);
        }
    }
}
