using System;
using System.Drawing;

namespace SRCCore.TestLib
{
    /// <summary>
    /// IGUIScrean のテスト用モック。
    /// ハンドラが注入されていない場合は何もしません。
    /// </summary>
    public class MockGUIScrean : IGUIScrean
    {
        public Action<ScreanDrawOption, int, int, int, float, float> ArcCmdHandler { get; set; }
        public void ArcCmd(ScreanDrawOption option, int x1, int y1, int rad, float start_angle, float end_angle)
        {
            ArcCmdHandler?.Invoke(option, x1, y1, rad, start_angle, end_angle);
        }

        public Action<ScreanDrawOption, int, int, int> CircleCmdHandler { get; set; }
        public void CircleCmd(ScreanDrawOption option, int x1, int y1, int rad)
        {
            CircleCmdHandler?.Invoke(option, x1, y1, rad);
        }

        public Action<ScreanDrawOption, int, int, int, float> OvalCmdHandler { get; set; }
        public void OvalCmd(ScreanDrawOption option, int x1, int y1, int rad, float oval_ratio)
        {
            OvalCmdHandler?.Invoke(option, x1, y1, rad, oval_ratio);
        }

        public Action<ScreanDrawOption, int, int, int, int> LineCmdHandler { get; set; }
        public void LineCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
            LineCmdHandler?.Invoke(option, x1, y1, x2, y2);
        }

        public Action<ScreanDrawOption, int, int, int, int> BoxCmdHandler { get; set; }
        public void BoxCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
            BoxCmdHandler?.Invoke(option, x1, y1, x2, y2);
        }

        public Action<ScreanDrawOption, int, int> PSetCmdHandler { get; set; }
        public void PSetCmd(ScreanDrawOption option, int x1, int y1)
        {
            PSetCmdHandler?.Invoke(option, x1, y1);
        }

        public Action<ScreanDrawOption, Point[]> PolygonCmdHandler { get; set; }
        public void PolygonCmd(ScreanDrawOption option, Point[] points)
        {
            PolygonCmdHandler?.Invoke(option, points);
        }
    }
}
