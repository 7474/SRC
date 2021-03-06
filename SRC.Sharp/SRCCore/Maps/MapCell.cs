using Newtonsoft.Json;
using SRCCore.Models;

namespace SRCCore.Maps
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MapCell
    {
        // レイヤー無しの固定値
        public const int NO_LAYER_NUM = 10000;

        public static readonly MapCell EmptyCell = new MapCell
        {
            TerrainType = Map.MAX_TERRAIN_DATA_NUM,
            BitmapNo = 0,
            LayerType = NO_LAYER_NUM,
            LayerBitmapNo = 0,
            BoxType = BoxTypes.Under,
            UnderTerrain = TerrainData.EmptyTerrain,
            UpperTerrain = TerrainData.EmptyTerrain,
        };

        [JsonProperty]
        public int X { get; set; }
        [JsonProperty]
        public int Y { get; set; }

        // 地形の種類
        [JsonProperty]
        public int TerrainType { get; set; }
        public TerrainData UnderTerrain { get; set; }
        // ビットマップの番号
        [JsonProperty]
        public int BitmapNo { get; set; }
        // マップ上層レイヤーデータ
        // 地形の種類。未設定はNO_LAYER_NUM
        [JsonProperty]
        public int LayerType { get; set; }
        public TerrainData UpperTerrain { get; set; }
        // ビットマップの番号。未設定はNO_LAYER_NUM
        [JsonProperty]
        public int LayerBitmapNo { get; set; }
        // マスのデータタイプ。1:下層 2:上層 3:上層データのみ 4:上層見た目のみ
        [JsonProperty]
        public BoxTypes BoxType { get; set; }

        public TerrainData Terrain
        {
            get
            {
                switch (BoxType)
                {
                    case BoxTypes.Under:
                    case BoxTypes.UpperBmpOnly:
                        // 上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
                        return UnderTerrain;

                    default:
                        // 上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
                        return UpperTerrain;
                }
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
        public void Restore(SRC src)
        {
            UnderTerrain = src.TDList.Item(TerrainType);
            UpperTerrain = src.TDList.Item(LayerType);
        }
    }
}
