using SRCCore.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Events
{
    public partial class Event
    {
        // いまのサブルーチン呼び出しのサブルーチンローカル変数。
        // VB と同じく CallDepth > 0 のときだけ見る（For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex）
        public IEnumerable<VarData> SubLocalVars()
        {
            if (CallDepth > 0)
            {
                int i = VarIndexStack[CallDepth - 1];
                return VarStack.Skip(i + 1).Take(VarIndex - i);
            }
            else
            {
                return Enumerable.Empty<VarData>();
            }
        }

        public VarData SubLocalVar(string vname)
        {
            // VB と同じく名前は大文字小文字を区別して照合する
            return SubLocalVars().FirstOrDefault(x => x.Name == vname);
        }

        public VarData NewSubLocalVar()
        {
            VarIndex += 1;
            return VarStack[VarIndex];
        }
    }
}
