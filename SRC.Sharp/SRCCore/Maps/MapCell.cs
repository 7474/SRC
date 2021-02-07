using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Maps
{
    public class MapCell
    {
        // XXX Enumにするとか
        public int TerrainType { get; set; }
        public int BitmapNo { get; set; }
        public int LayerType { get; set; }
        public int LayerBitmapNo { get; set; }
        public int BoxType { get; set; }
    }
}
