using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ReturnCmd : CmdData
    {
        public ReturnCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReturnCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (Event.CallDepth <= 0)
            {
                throw new EventErrorException(this, "CallコマンドとReturnコマンドが対応していません");
            }
            else if (Event.CallDepth == 1 && Event.CallStack[Event.CallDepth] == 0)
            {
                throw new EventErrorException(this, "CallコマンドとReturnコマンドが対応していません");
            }

            // 呼び出し階層数をデクリメント
            Event.CallDepth = Event.CallDepth - 1;

            // サブルーチン実行前の状態に復帰
            Event.ArgIndex = Event.ArgIndexStack[Event.CallDepth];
            Event.VarIndex = Event.VarIndexStack[Event.CallDepth];
            Event.ForIndex = Event.ForIndexStack[Event.CallDepth];
            Event.UpVarLevel = Event.UpVarLevelStack[Event.CallDepth];
            return Event.CallStack[Event.CallDepth] + 1;
        }
    }
}
