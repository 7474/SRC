using SRCCore.Expressions;
using System.Collections.Generic;
using System.Linq;

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
            return SubLocalVars().FirstOrDefault(x => x.Name.ToLower() == vname.ToLower());
        }
    }
}
