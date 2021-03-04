using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SwitchCmd : CmdData
    {
        public SwitchCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SwitchCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }

        internal static int ToEnd(CmdData caseCmd)
        {
            // 対応するEndSwを探す
            var depth = 1;
            for (var i = caseCmd.EventData.ID + 1; i <= caseCmd.Event.EventCmd.Count; i++)
            {
                var cmd = caseCmd.Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.SwitchCmd:
                        depth = depth + 1;
                        break;

                    case CmdType.EndSwCmd:
                        depth = depth - 1;
                        if (depth == 0)
                        {
                            return i + 1;
                        }
                        break;
                }
            }

            throw new EventErrorException(caseCmd, "SwitchとEndSwが対応していません");
        }
    }

    public class CaseCmd : CmdData
    {
        public CaseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CaseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 直前の Switch コマンドの実行の終了を意味する。
            return SwitchCmd.ToEnd(this);
        }
    }

    public class CaseElseCmd : CmdData
    {
        public CaseElseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CaseElseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 直前の Switch コマンドの実行の終了を意味する。
            return SwitchCmd.ToEnd(this);
        }
    }

    public class EndSwCmd : CmdData
    {
        public EndSwCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EndSwCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
