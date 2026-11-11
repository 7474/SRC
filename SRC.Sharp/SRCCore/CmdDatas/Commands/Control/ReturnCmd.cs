using SRCCore.Events;
using SRCCore.Exceptions;

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
                // 元の SRC と同じく、イベントの最上位（HandleEvent が CallDepth を 1 にした階層）で
                // まだ 1 階層目からの Call が無いときの Return は対応の誤りとする
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
