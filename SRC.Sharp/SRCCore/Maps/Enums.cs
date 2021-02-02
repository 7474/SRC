using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Maps
{
    public enum MapDataIndex
    {
        TerrainType = 0,
        BitmapNo = 1,
        LayerType = 2,
        LayerBitmapNo = 3,
        BoxType = 4
    }

    public enum BoxTypes
    {
        Under = 1,
        Upper = 2,
        UpperDataOnly = 3,
        UpperBmpOnly = 4
    }

    // マップの画像ファイルの格納形式
    public enum MapImageFileType
    {
        OldMapImageFileType, // 旧形式 (plain0.bmp)
        FourFiguresMapImageFileType, // ４桁の数値 (plain0000.bmp)
        SeparateDirMapImageFileType // ディレクトリ分割 (plain\plain0000.bmp)
    }
}
