using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PrintCmd : CmdData
    {
        public PrintCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PrintCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum == 1)
            {
                throw new EventErrorException(this, "Printコマンドの引数の数が違います");
            }

            var f = SRC.FileHandleManager.Get(GetArgAsLong(2));
            var msg = GeneralLib.ListTail(EventData.Data, 3);
            if (Strings.Right(msg, 1) != ";")
            {
                if (Strings.Left(msg, 1) != "`" || Strings.Right(msg, 1) != "`")
                {
                    if (Strings.Left(msg, 2) == "$(")
                    {
                        if (Strings.Right(msg, 1) == ")")
                        {
                            msg = Expression.GetValueAsString(Strings.Mid(msg, 3, Strings.Len(msg) - 3));
                        }
                    }
                    else if (GeneralLib.ListLength(msg) == 1)
                    {
                        msg = Expression.GetValueAsString(msg);
                    }

                    Expression.ReplaceSubExpression(ref msg);
                }
                else
                {
                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                }

                f.Writer.WriteLine(msg);
            }
            else
            {
                msg = Strings.Left(msg, Strings.Len(msg) - 1);
                if (Strings.Left(msg, 1) != "`" || Strings.Right(msg, 1) != "`")
                {
                    if (Strings.Left(msg, 2) == "$(")
                    {
                        if (Strings.Right(msg, 1) == ")")
                        {
                            msg = Expression.GetValueAsString(Strings.Mid(msg, 3, Strings.Len(msg) - 3));
                        }
                    }
                    else if (GeneralLib.ListLength(msg) == 1)
                    {
                        msg = Expression.GetValueAsString(msg);
                    }

                    Expression.ReplaceSubExpression(ref msg);
                }
                else
                {
                    msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                }

                f.Writer.WriteLine(msg);
            }

            return EventData.NextID;
        }
    }
}
