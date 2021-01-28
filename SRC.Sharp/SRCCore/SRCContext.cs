using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core
{
    public static class SRCContext
    {
        public static SRC SRC { get; private set; }

        public static void SetSRC(SRC src)
        {
            SRC = src;
        }
    }
}
