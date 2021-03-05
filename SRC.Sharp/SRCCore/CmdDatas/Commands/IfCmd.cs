using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class IfCmd : AIfCmd
    {
        public IfCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IfCmd, eventData)
        {
            PrepareArgs();
        }

        protected override int ExecInternal()
        {
            bool flag = Evaluate();

            switch (IfCmdType)
            {
                case IfCmdType.Exit:
                    if (flag)
                    {
                        return -1;
                    }
                    else
                    {
                        return EventData.ID + 1;
                    }

                case IfCmdType.GoTo:
                    if (flag)
                    {
                        var ret = Event.FindLabel(GoToLabel);
                        if (ret < 0)
                        {
                            throw new EventErrorException(this, "ラベル「" + GoToLabel + "」がみつかりません");
                        }
                        return ret + 1;
                    }
                    else
                    {
                        return EventData.ID + 1;
                    }

                case IfCmdType.Then:
                    if (flag)
                    {
                        // Then節をそのまま実行
                        return EventData.ID + 1;
                    }

                    // 条件式が成り立たない場合はElse節もしくはEndIfを探す
                    var depth = 1;
                    foreach (var i in AfterEventIdRange())
                    {
                        var cmd = Event.EventCmd[i];
                        switch (cmd.Name)
                        {
                            case CmdType.IfCmd:
                                if ((cmd as AIfCmd).IfCmdType == IfCmdType.Then)
                                {
                                    depth = depth + 1;
                                }
                                break;

                            case CmdType.ElseCmd:
                                if (depth == 1)
                                {
                                    return cmd.EventData.ID + 1;
                                }
                                break;

                            case CmdType.ElseIfCmd:
                                if (depth != 1)
                                {
                                    continue;
                                }

                                if ((cmd as AIfCmd).Evaluate())
                                {
                                    return cmd.EventData.ID + 1;
                                }
                                break;

                            case CmdType.EndIfCmd:
                                depth = (depth - 1);
                                if (depth == 0)
                                {
                                    return cmd.EventData.ID + 1;
                                }
                                break;
                        }
                    }
                    throw new EventErrorException(this, "IfとEndIfが対応していません");

                default:
                    throw new EventErrorException(this, "If行には Goto, Exit, Then のいずれかを指定して下さい");
            }
        }

    }
}
