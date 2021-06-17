using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、アイテム操作
    public class ReleaseCmd : CmdData
    {
        public ReleaseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReleaseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string buf;
            //            switch (ArgNum)
            //            {
            //                case 1:
            //                    {
            //                        buf = Event.SelectedUnitForEvent.MainPilot().Name;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        buf = GetArgAsString(2);
            //                        bool localIsDefined() { object argIndex1 = (object)buf; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                        bool localIsDefined1() { object argIndex1 = (object)buf; var ret = SRC.IList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined() && !localIsDefined1())
            //                        {
            //                            Event.EventErrorMessage = "パイロット名またはアイテム名" + buf + "が間違っています";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 409999


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        if (SRC.PList.IsDefined((object)buf))
            //                        {
            //                            Pilot localItem() { object argIndex1 = (object)buf; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                            buf = localItem().Name;
            //                        }
            //                        else
            //                        {
            //                            Item localItem1() { object argIndex1 = (object)buf; var ret = SRC.IList.Item(argIndex1); return ret; }

            //                            buf = localItem1().Name;
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Releaseコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 410300


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            buf = "Fix(" + buf + ")";
            //            if (Expression.IsGlobalVariableDefined(buf))
            //            {
            //                Event.GlobalVariableList.Remove(buf);
            //            }

            //return EventData.NextID;
        }
    }
}
