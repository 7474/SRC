using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Expressions.Functions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using System.Drawing;
using System.IO;
using static SRCCore.Coverage.Tests.ExprInfo.ExprInfoFixture;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数の後半（VB の Expression.bas の EvalInfoFunc の「武器数」以降）を VB に合わせたことのテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class ExprInfoLatterHalfVBMatchTests
    {
        private static ValueType Invoke(SRC src, ValueType etype, out string str_result, out double num_result, params string[] args)
        {
            var @params = new string[args.Length + 1];
            var is_term = new bool[args.Length + 1];
            for (var i = 0; i < args.Length; i++)
            {
                @params[i + 1] = args[i];
                is_term[i + 1] = true;
            }
            return new Info().Invoke(src, etype, @params, args.Length, is_term, out str_result, out num_result);
        }

        [TestMethod]
        public void ユニットのアビリティの効果_範囲外の番号は空文字列()
        {
            // VB のユニットの分岐は「j <= 0 And .CountEffect < j」と書き誤っていて Collection の範囲外で実行時エラーになる。
            // ユニットデータなどの分岐（Or。範囲外は Exit Function）に合わせて空文字列を返す
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            Assert.AreEqual("回復", Eval(src, $"Info({u.ID},アビリティ,1,効果タイプ,1)"));
            Assert.AreEqual("2", Eval(src, $"Info({u.ID},アビリティ,1,効果レベル,1)"));
            Assert.AreEqual("", Eval(src, $"Info({u.ID},アビリティ,1,効果タイプ,2)"));
            Assert.AreEqual("", Eval(src, $"Info({u.ID},アビリティ,1,効果レベル,0)"));
            Assert.AreEqual("", Eval(src, $"Info({u.ID},アビリティ,1,効果データ,2)"));
        }

        [TestMethod]
        public void ユニットデータのアビリティの効果_範囲外の番号は空文字列()
        {
            // VB: If j <= 0 Or .CountEffect < j Then Exit Function
            var src = CreateSrc();
            Assert.AreEqual("回復", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,効果タイプ,1)"));
            Assert.AreEqual("", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,効果タイプ,2)"));
            Assert.AreEqual("", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,効果レベル,0)"));
            Assert.AreEqual("", Eval(src, "Info(ユニットデータ,テスト機,アビリティ,1,効果データ,5)"));
        }

        [TestMethod]
        public void 武器が見つからないとき_NumericType_で呼ぶと数値の0()
        {
            // VB: Exit Function で "" を返し、呼び出し元（CallFunction の "info"）が StrToDbl("") = 0 の NumericType にする
            var src = CreateSrc();
            var u = CreatePilotedUnit(src);
            var res = Invoke(src, ValueType.NumericType, out _, out var num, u.ID, "武器", "9", "攻撃力");
            Assert.AreEqual(ValueType.NumericType, res);
            Assert.AreEqual(0d, num);

            res = Invoke(src, ValueType.NumericType, out _, out num, u.ID, "アビリティ", "無いアビリティ", "射程");
            Assert.AreEqual(ValueType.NumericType, res);
            Assert.AreEqual(0d, num);

            res = Invoke(src, ValueType.StringType, out var str, out _, u.ID, "武器", "9", "攻撃力");
            Assert.AreEqual(ValueType.StringType, res);
            Assert.AreEqual("", str);
        }

        private static SRC CreateSrcWithMap()
        {
            var src = CreateSrc();
            var plain = new TerrainData { ID = 1, Name = "平地", Class = "陸", Bitmap = "plain" };
            var wood = new TerrainData { ID = 2, Name = "森", Class = "陸", Bitmap = "wood" };
            src.Map.MapWidth = 2;
            src.Map.MapHeight = 1;
            src.Map.MapData = new Src2DArray<MapCell>(2, 1);
            src.Map.MapData[1, 1] = new MapCell { X = 1, Y = 1, UnderTerrain = plain, BitmapNo = 3, BoxType = BoxTypes.Under };
            src.Map.MapData[2, 1] = new MapCell { X = 2, Y = 1, UnderTerrain = plain, BitmapNo = 3, UpperTerrain = wood, LayerBitmapNo = 12, BoxType = BoxTypes.Upper };
            src.Map.MapUnderImageFilePath = new Src2DArray<string>(2, 1);
            src.Map.MapUpperImageFilePath = new Src2DArray<string>(2, 1);
            src.Map.MapDataForUnit = new Src2DArray<Units.Unit>(2, 1);
            return src;
        }

        [TestMethod]
        public void マップのビットマップ名_見つけたファイルの格納形式で相対のファイル名を返す()
        {
            var src = CreateSrcWithMap();
            var map = Path.Combine("scenario", "Bitmap", "Map");

            // まだ画像を探していない地点は旧形式（VB の MapImageFileTypeData の既定値 0）
            Assert.AreEqual("plain3.bmp", Eval(src, "Info(マップ,1,1,ビットマップ名)"));

            // ディレクトリ分割（plain\plain0003.bmp）
            src.Map.MapUnderImageFilePath[1, 1] = Path.Combine(map, "plain", "plain0003.bmp");
            Assert.AreEqual(@"plain\plain0003.bmp", Eval(src, "Info(マップ,1,1,ビットマップ名)"));

            // ４桁の数値（plain0003.bmp）
            src.Map.MapUnderImageFilePath[1, 1] = Path.Combine(map, "plain0003.bmp");
            Assert.AreEqual("plain0003.bmp", Eval(src, "Info(マップ,1,1,ビットマップ名)"));

            // 旧形式（plain3.bmp）
            src.Map.MapUnderImageFilePath[1, 1] = Path.Combine(map, "plain3.bmp");
            Assert.AreEqual("plain3.bmp", Eval(src, "Info(マップ,1,1,ビットマップ名)"));
        }

        [TestMethod]
        public void マップのレイヤービットマップ名_地点の最後に見つけた形式を下層にも使う()
        {
            // VB は地点ごとに一つの MapImageFileTypeData を下層・上層の順に探して上書きするので、
            // 上層が見つかっていれば上層の形式で下層の名前も作る
            var src = CreateSrcWithMap();
            var map = Path.Combine("scenario", "Bitmap", "Map");
            src.Map.MapUnderImageFilePath[2, 1] = Path.Combine(map, "plain3.bmp");
            src.Map.MapUpperImageFilePath[2, 1] = Path.Combine(map, "wood", "wood0012.bmp");
            Assert.AreEqual(@"wood\wood0012.bmp", Eval(src, "Info(マップ,2,1,レイヤービットマップ名)"));
            Assert.AreEqual(@"plain\plain0003.bmp", Eval(src, "Info(マップ,2,1,ビットマップ名)"));
        }

        [TestMethod]
        public void マップの座標が範囲外_NumericType_で呼ぶと数値の0()
        {
            var src = CreateSrcWithMap();
            var res = Invoke(src, ValueType.NumericType, out _, out var num, "マップ", "3", "1", "回避修正");
            Assert.AreEqual(ValueType.NumericType, res);
            Assert.AreEqual(0d, num);
        }

        [TestMethod]
        public void マップの時間帯_フィルタの透過率は_CStr_と同じく15桁で書く()
        {
            // VB: CStr(MapDrawFilterTransPercent * 100)。0.07 * 100 = 7.000000000000001 は「7」になる
            var src = CreateSrcWithMap();
            src.Map.MapDrawMode = "フィルタ";
            src.Map.MapDrawFilterColor = Color.FromArgb(0xFF, 0x00, 0x00);
            src.Map.MapDrawFilterTransPercent = 7d / 100;
            Assert.AreEqual("#FF0000 7%", Eval(src, "Info(マップ,時間帯)"));

            src.Map.MapDrawFilterTransPercent = 0.333d;
            Assert.AreEqual("#FF0000 33.3%", Eval(src, "Info(マップ,時間帯)"));
        }
    }
}
