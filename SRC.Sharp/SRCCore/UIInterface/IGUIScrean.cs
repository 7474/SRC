// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System.Collections.Generic;
using System.Drawing;

namespace SRCCore
{
    // 画面描画のGUIインタフェース
    public interface IGUIScrean
    {
        /// <summary>
        /// 円弧を描く。角度は度で、右向きが 0、反時計回りに増える。VB の Circle メソッドと同じく、
        /// 開始から終了へ反時計回りに描き（開始と終了が同じなら一周）、負の角度はその絶対値の角度から中心へ線を引く。
        /// 開始・終了の両方が負なら閉じた扇形になり、FillStyle で塗りつぶす。
        /// </summary>
        void ArcCmd(ScreanDrawOption option, int x1, int y1, int rad, float start_angle, float end_angle);
        void CircleCmd(ScreanDrawOption option, int x1, int y1, int rad);
        void OvalCmd(ScreanDrawOption option, int x1, int y1, int rad, float oval_ratio);
        void LineCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2);
        void BoxCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2);
        void PSetCmd(ScreanDrawOption option, int x1, int y1);
        void PolygonCmd(ScreanDrawOption option, Point[] points);
    }

    public class ScreanDrawOption
    {
        public ScreanDrawMode DrawOption { get; set; }
        public Color ForeColor { get; set; }
        public int DrawWidth { get; set; }
        public Color FillColor { get; set; }
        public FillStyle FillStyle { get; set; }

        public ScreanDrawOption()
        {
            DrawOption = ScreanDrawMode.Front;
            DrawWidth = 1;
            FillStyle = FillStyle.VbFSTransparent;
        }

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

    /// <summary>
    /// VB の Circle メソッドの円弧の角度（IGUIScrean.ArcCmd の約束）を、描画 API で使う形にしたもの
    /// </summary>
    public readonly struct ScreanArc
    {
        /// <summary>開始の角度（度。右向きが 0 で反時計回り。0 以上）</summary>
        public float StartAngle { get; }
        /// <summary>開始から終了まで反時計回りに進む角度（度。0 より大きく 360 以下）</summary>
        public float SweepAngle { get; }
        /// <summary>開始の角度の点から中心へ線を引くか</summary>
        public bool LineFromStart { get; }
        /// <summary>終了の角度の点から中心へ線を引くか</summary>
        public bool LineToEnd { get; }
        /// <summary>閉じた扇形か（塗りつぶす）</summary>
        public bool IsPie => LineFromStart && LineToEnd;

        public ScreanArc(float startAngle, float endAngle)
        {
            LineFromStart = startAngle < 0f;
            LineToEnd = endAngle < 0f;
            StartAngle = System.Math.Abs(startAngle);
            var sweep = System.Math.Abs(endAngle) - StartAngle;
            // 終了が開始より小さければ 0 度をまたぐ。同じなら一周する（Windows の Arc・Pie と同じ）
            if (sweep <= 0f)
            {
                sweep += 360f;
            }
            SweepAngle = sweep;
        }
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
