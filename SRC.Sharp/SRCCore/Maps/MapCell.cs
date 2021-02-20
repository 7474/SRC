using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Maps
{
    public class MapCell
    {
        // レイヤー無しの固定値
        public const int NO_LAYER_NUM = 10000;

        public int X { get; set; }
        public int Y { get; set; }

        // 地形の種類
        public int TerrainType { get; set; }
        public TerrainData Terrain { get; set; }
        // ビットマップの番号
        public int BitmapNo { get; set; }
        // マップ上層レイヤーデータ
        // 地形の種類。未設定はNO_LAYER_NUM
        public int LayerType { get; set; }
        // ビットマップの番号。未設定はNO_LAYER_NUM
        public int LayerBitmapNo { get; set; }
        // マスのデータタイプ。1:下層 2:上層 3:上層データのみ 4:上層見た目のみ
        public BoxTypes BoxType { get; set; }

        public string TerrainClass
        {
            get
            {
                return Terrain.Class;
                // TODO レイヤ対応
                //switch (BoxType)
                //{
                //    case BoxTypes.Under:
                //    case BoxTypes.UpperBmpOnly:
                //        // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
                //        {
                //            TerrainClassRet = SRC.TDList.Class_Renamed(MapData[X, Y, MapDataIndex.TerrainType]);
                //            break;
                //        }

                //    default:
                //        {
                //            // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
                //            TerrainClassRet = SRC.TDList.Class_Renamed(MapData[X, Y, MapDataIndex.LayerType]);
                //            break;
                //        }
                //}
            }
        }

        public MapCell()
        {
            TerrainType = 0;
            BitmapNo = 0;
            LayerType = NO_LAYER_NUM;
            LayerBitmapNo = NO_LAYER_NUM;
            BoxType = BoxTypes.Under;
        }
    }
}
