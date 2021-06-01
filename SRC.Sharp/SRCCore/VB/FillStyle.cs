using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.VB
{
    /// <summary>
    /// Ref. https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-basic-6/aa267474(v=vs.60)
    /// </summary>
    public enum FillStyle
    {
        VbFSSolid = 0,
        /// <summary>
        /// (Default)
        /// </summary>
        VbFSTransparent = 1,
        VbHorizontalLine = 2,
        VbVerticalLine = 3,
        VbUpwardDiagonal = 4,
        VbDownwardDiagonal = 5,
        VbCross = 6,
        VbDiagonalCross = 7,
    }
}
