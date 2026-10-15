using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using System.Drawing;
using static SRCCore.Coverage.Tests.ExprInfo.ExprInfoFixture;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数の「マップ」「オプション」に関するテスト
    /// 仕様: Info関数.md / VB の EvalInfoFunc（Expression.bas）
    /// </summary>
    [TestClass]
    public class ExprInfoMapOptionTests
    {
        // 2x1 のマップを用意する（SetMapSize は GUI のマップ実装を要するため、フィールドに直接置く）
        private static SRC CreateSrcWithMap()
        {
            var src = CreateSrc();
            var plain = new TerrainData { ID = 1, Name = "平地", Class = "陸", MoveCost = 2, HitMod = 0, DamageMod = 0 };
            var fort = new TerrainData { ID = 2, Name = "基地", Class = "屋内", MoveCost = 3, HitMod = 25, DamageMod = 10 };
            fort.AddFeature("ＨＰ回復Lv1");
            fort.AddFeature("ＥＮ回復Lv2");
            src.Map.MapWidth = 2;
            src.Map.MapHeight = 1;
            src.Map.MapData = new Src2DArray<MapCell>(2, 1);
            src.Map.MapData[1, 1] = new MapCell { X = 1, Y = 1, UnderTerrain = plain };
            src.Map.MapData[2, 1] = new MapCell { X = 2, Y = 1, UnderTerrain = fort };
            src.Map.MapDataForUnit = new Src2DArray<Units.Unit>(2, 1);
            return src;
        }

        [TestMethod]
        public void Map_Size()
        {
            var src = CreateSrcWithMap();
            Assert.AreEqual("2", Eval(src, "Info(マップ,幅)"));
            Assert.AreEqual("1", Eval(src, "Info(マップ,高さ)"));
        }

        [TestMethod]
        public void Map_FileName_IsRelativeToScenarioPath()
        {
            // シナリオフォルダ配下のマップはシナリオフォルダからの相対パスを返す
            var src = CreateSrcWithMap();
            src.ScenarioPath = "/scenario/";
            src.Map.MapFileName = "/scenario/map/test.map";
            Assert.AreEqual("map/test.map", Eval(src, "Info(マップ,ファイル名)"));

            // シナリオフォルダ外のマップはそのまま
            src.Map.MapFileName = "/other/test.map";
            Assert.AreEqual("/other/test.map", Eval(src, "Info(マップ,ファイル名)"));
        }

        [TestMethod]
        public void Map_TimeOfDay()
        {
            // 描画モード未設定なら「昼」、設定されていればそのモード名、マップ限定なら「 マップ限定」を付ける
            var src = CreateSrcWithMap();
            src.Map.MapDrawMode = "";
            Assert.AreEqual("昼", Eval(src, "Info(マップ,時間帯)"));

            src.Map.MapDrawMode = "夜";
            Assert.AreEqual("夜", Eval(src, "Info(マップ,時間帯)"));

            src.Map.MapDrawMode = "セピア";
            src.Map.MapDrawIsMapOnly = true;
            Assert.AreEqual("セピア マップ限定", Eval(src, "Info(マップ,時間帯)"));
        }

        [TestMethod]
        [Ignore("移植の不具合: フィルタ時の時間帯に不透明度（\"nn%\"）が付かない")]
        public void Map_TimeOfDay_Filter_IncludesTransparency()
        {
            // VB: buf = "#RRGGBB" & " " & CStr(MapDrawFilterTransPercent * 100) & "%"
            var src = CreateSrcWithMap();
            src.Map.MapDrawMode = "フィルタ";
            src.Map.MapDrawFilterColor = Color.FromArgb(0x12, 0x34, 0x56);
            src.Map.MapDrawFilterTransPercent = 0.5d;
            src.Map.MapDrawIsMapOnly = true;
            Assert.AreEqual("#123456 50% マップ限定", Eval(src, "Info(マップ,時間帯)"));
        }

        [TestMethod]
        public void Map_Terrain_Properties()
        {
            var src = CreateSrcWithMap();
            Assert.AreEqual("平地", Eval(src, "Info(マップ,1,1,地形名)"));
            Assert.AreEqual("陸", Eval(src, "Info(マップ,1,1,地形タイプ)"));
            Assert.AreEqual("基地", Eval(src, "Info(マップ,2,1,地形名)"));
            Assert.AreEqual("屋内", Eval(src, "Info(マップ,2,1,地形クラス)"));
            // 移動コストは２倍の値で記録されているので半分にして返す
            Assert.AreEqual("1", Eval(src, "Info(マップ,1,1,移動コスト)"));
            Assert.AreEqual("1.5", Eval(src, "Info(マップ,2,1,移動コスト)"));
            Assert.AreEqual("25", Eval(src, "Info(マップ,2,1,回避修正)"));
            Assert.AreEqual("10", Eval(src, "Info(マップ,2,1,ダメージ修正)"));
            // 回復量は特殊能力レベル×10(%)
            Assert.AreEqual("10", Eval(src, "Info(マップ,2,1,ＨＰ回復量)"));
            Assert.AreEqual("20", Eval(src, "Info(マップ,2,1,ＥＮ回復量)"));
            Assert.AreEqual("0", Eval(src, "Info(マップ,1,1,ＨＰ回復量)"));
        }

        [TestMethod]
        public void Map_Terrain_OutOfRange_ReturnsEmpty()
        {
            // マップ外の座標は空文字列
            var src = CreateSrcWithMap();
            Assert.AreEqual("", Eval(src, "Info(マップ,3,1,地形名)"));
            Assert.AreEqual("", Eval(src, "Info(マップ,1,2,地形名)"));
            Assert.AreEqual("", Eval(src, "Info(マップ,0,1,地形名)"));
        }

        [TestMethod]
        public void Map_UnitId()
        {
            // 指定地点にいるユニットのIDを返し、いなければ空文字列
            var src = CreateSrcWithMap();
            var u = CreatePilotedUnit(src);
            src.Map.MapDataForUnit[2, 1] = u;
            Assert.AreEqual(u.ID, Eval(src, "Info(マップ,2,1,ユニットＩＤ)"));
            Assert.AreEqual("", Eval(src, "Info(マップ,1,1,ユニットＩＤ)"));
        }

        [TestMethod]
        public void Option_AnimationFlags_ReturnOnOff()
        {
            var src = CreateSrc();
            src.SystemConfig.BattleAnimation = true;
            src.SystemConfig.SpecialPowerAnimation = false;
            src.SystemConfig.AutoDefense = true;
            Assert.AreEqual("On", Eval(src, "Info(オプション,BattleAnimation)"));
            Assert.AreEqual("Off", Eval(src, "Info(オプション,SpecialPowerAnimation)"));
            Assert.AreEqual("On", Eval(src, "Info(オプション,AutoDeffence)"));

            src.SystemConfig.BattleAnimation = false;
            Assert.AreEqual("Off", Eval(src, "Info(オプション,BattleAnimation)"));
        }

        [TestMethod]
        public void Option_MessageWait()
        {
            var src = CreateSrc();
            src.GUI.MessageWait = 1200;
            Assert.AreEqual("1200", Eval(src, "Info(オプション,MessageWait)"));
        }

        [TestMethod]
        public void Option_OptionCommandFlags()
        {
            // それ以外の名前は Option コマンドで設定されたオプションかどうかを On/Off で返す
            var src = CreateSrc();
            Assert.AreEqual("Off", Eval(src, "Info(オプション,テスト用オプション)"));
            src.Expression.DefineGlobalVariable("Option(テスト用オプション)");
            Assert.AreEqual("On", Eval(src, "Info(オプション,テスト用オプション)"));
        }

        [TestMethod]
        public void EmptyFirstParam_ReturnsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", Eval(src, "Info(\"\",名称)"));
        }
    }
}
