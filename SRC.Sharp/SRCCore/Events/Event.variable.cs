using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using SRCCore.CmdDatas;
using SRCCore.Expressions;
using SRCCore.Extensions;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.Events
{
    public partial class Event
    {
        public IEnumerable<VarData> SubLocalVars()
        {
            if (CallDepth > 0)
            {
                int i = VarIndexStack[CallDepth - 1];
                return VarStack.Skip(i + 1).Take(VarIndex - i);
            }
            else if (VarIndex > 0)
            {
                return VarStack.Take(VarIndex + 1);
            }
            else
            {
                return Enumerable.Empty<VarData>();
            }
        }

        public VarData SubLocalVar(string vname)
        {
            return SubLocalVars().FirstOrDefault(x => x.Name == vname);
        }
    }
}
