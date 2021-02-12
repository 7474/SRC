using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Maps
{
    public enum MapBoxType
    {
        Upper = 1,
        Under = 2,
        UpperOnly = 3,
        UpperViewOnly = 4,
    }

    public class MapCell
    {
        // XXX Enumにするとか
        // 地形の種類
        public int TerrainType { get; set; }
        // ビットマップの番号
        public int BitmapNo { get; set; }
        // マップ上層レイヤーデータ
        // 地形の種類。未設定はNO_LAYER_NUM
        public int LayerType { get; set; }
        // ビットマップの番号。未設定はNO_LAYER_NUM
        public int LayerBitmapNo { get; set; }
        // マスのデータタイプ。1:下層 2:上層 3:上層データのみ 4:上層見た目のみ
        public MapBoxType BoxType { get; set; }
    }
}
