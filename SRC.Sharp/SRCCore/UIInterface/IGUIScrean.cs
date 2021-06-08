// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;
using SRCCore.VB;
using System.Drawing;

namespace SRCCore
{
    // 画面描画のGUIインタフェース
    public interface IGUIScrean
    {
        void ArcCmd(ScreanDrawOption option, int x1, int y1, int rad, float start_angle, float end_angle);
        void CircleCmd(ScreanDrawOption option, int x1, int y1, int rad);
    }

    public class ScreanDrawOption
    {
        public ScreanDrawMode DrawOption { get; set; }
        public Color ForeColor { get; set; }
        public int DrawWidth { get; set; }
        public Color FillColor { get; set; }
        public FillStyle FillStyle { get; set; }

        public ScreanDrawOption() { }
        public ScreanDrawOption(Events.Event e, Color foreColor)
        {
            DrawOption = GUIScreanExtension.ScreanDrawModeFrom(e.ObjDrawOption);
            ForeColor = foreColor;
            DrawWidth = e.ObjDrawWidth;
            FillColor = e.ObjFillColor;
            FillStyle = e.ObjFillStyle;
        }
    }

    public enum ScreanDrawMode
    {
        Front,
        Preserve,
        Background,
    }

    public static class GUIScreanExtension
    {
        public static ScreanDrawMode ScreanDrawModeFrom(string mode)
        {
            switch (mode)
            {
                case "背景": return ScreanDrawMode.Background;
                case "保持": return ScreanDrawMode.Preserve;
                default: return ScreanDrawMode.Front;
            }
        }
    }
}
