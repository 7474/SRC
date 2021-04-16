using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCSharpForm.Extensions
{
    public static class PointFExtension
    {
        public static PointF Add(this PointF p, float x, float y)
        {
            return new PointF(p.X + x, p.Y + y);
        }
        public static PointF AddX(this PointF p, float v)
        {
            return Add(p, v, 0);
        }
        public static PointF AddY(this PointF p, float v)
        {
            return Add(p, 0, v);
        }
    }
}
