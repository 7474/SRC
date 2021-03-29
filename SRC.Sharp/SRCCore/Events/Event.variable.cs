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
            int i = VarIndexStack[CallDepth - 1];
            return VarStack.Skip(i).Take(VarIndex - i);
        }

        public VarData SubLocalVar(string vname)
        {
            return SubLocalVars().FirstOrDefault(x => x.Name == vname);
        }

        public IEnumerable<VarData> SubLocalArray(string vname)
        {
            return SubLocalVars().Where(x => x.Name.StartsWith(vname + "["));
        }

        public IEnumerable<string> SubLocalArrayIndexes(string vname)
        {
            return SubLocalArray(vname)
                .Select(x => x.Name.ArrayIndexByName())
                .Where(x => !string.IsNullOrEmpty(x));
        }
    }
}
