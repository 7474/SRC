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
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Switchコマンドの引数の数が違います");
            }

            var a = GetArgAsString(2);
            SRC.LogTrace("Switch", a);
            var depth = 1;
            foreach (var i in this.AfterEventIdRange())
            {
                var cmd = Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.CaseCmd:
                        if (depth == 1)
                        {
                            for (var j = 2; j <= cmd.ArgNum; j++)
                            {
                                var arg = cmd.GetArgRaw(j);
                                string b;
                                if (arg.argType == Expressions.ValueType.UndefinedType)
                                {
                                    // 未識別のパラメータは式として処理する
                                    b = cmd.GetArgAsString(j);
                                    if ((b ?? "") == (cmd.GetArg(j) ?? ""))
                                    {
                                        // 文字列として識別済みにする
                                        cmd.GetArgRaw(j).argType = Expressions.ValueType.StringType;
                                    }
                                }
                                else
                                {
                                    // 識別済みのパラメータは文字列としてそのまま参照する
                                    b = cmd.GetArg(j);
                                }

                                if (a == b)
                                {
                                    return i + 1;
                                }
                            }
                        }

                        break;

                    case CmdType.CaseElseCmd:
                        if (depth == 1)
                        {
                            return i + 1;
                        }

                        break;

                    case CmdType.EndSwCmd:
                        if (depth == 1)
                        {
                            return i + 1;
                        }
                        else
                        {
                            depth = depth - 1;
                        }

                        break;

                    case CmdType.SwitchCmd:
                        depth = depth + 1;
                        break;
                }
            }

            throw new EventErrorException(this, "SwitchとEndSwが対応していません");
        }

        internal static int ToEnd(CmdData caseCmd)
        {
            // 対応するEndSwを探す
            var depth = 1;
            foreach (var i in caseCmd.AfterEventIdRange())
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
            return EventData.NextID;
        }
    }
}
