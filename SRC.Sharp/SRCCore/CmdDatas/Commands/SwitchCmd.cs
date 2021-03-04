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

            a = GetArgAsString(2);
            depth = 1;
            var loopTo = Information.UBound(Event_Renamed.EventCmd);
            for (i = LineNum + 1; i <= loopTo; i++)
            {
                {
                    var withBlock = Event_Renamed.EventCmd[i];
                    switch (withBlock.Name)
                    {
                        case Event_Renamed.CmdType.CaseCmd:
                            {
                                if (depth == 1)
                                {
                                    var loopTo1 = withBlock.ArgNum;
                                    for (j = 2; j <= loopTo1; j++)
                                    {
                                        if (withBlock.GetArgsType(j) == Expression.ValueType.UndefinedType)
                                        {
                                            // 未識別のパラメータは式として処理する
                                            b = withBlock.GetArgAsString(j);
                                            if ((b ?? "") == (withBlock.GetArg(j) ?? ""))
                                            {
                                                // 文字列として識別済みにする
                                                withBlock.SetArgsType(j, Expression.ValueType.StringType);
                                            }
                                        }
                                        else
                                        {
                                            // 識別済みのパラメータは文字列としてそのまま参照する
                                            b = withBlock.GetArg(j);
                                        }

                                        if ((a ?? "") == (b ?? ""))
                                        {
                                            ExecSwitchCmdRet = i + 1;
                                            return ExecSwitchCmdRet;
                                        }
                                    }
                                }

                                break;
                            }

                        case Event_Renamed.CmdType.CaseElseCmd:
                            {
                                if (depth == 1)
                                {
                                    ExecSwitchCmdRet = i + 1;
                                    return ExecSwitchCmdRet;
                                }

                                break;
                            }

                        case Event_Renamed.CmdType.EndSwCmd:
                            {
                                if (depth == 1)
                                {
                                    ExecSwitchCmdRet = i + 1;
                                    return ExecSwitchCmdRet;
                                }
                                else
                                {
                                    depth = (short)(depth - 1);
                                }

                                break;
                            }

                        case Event_Renamed.CmdType.SwitchCmd:
                            {
                                depth = (short)(depth + 1);
                                break;
                            }
                    }
                }
            }

            throw new EventErrorException(this, "SwitchとEndSwが対応していません");
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
