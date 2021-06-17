using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearStatusCmd : CmdData
    {
        public ClearStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearStatusCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string sname;
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        sname = GetArgAsString(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        sname = GetArgAsString(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "ClearStatusコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 188579


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            {
            //                var withBlock = u;
            //                if (withBlock.IsConditionSatisfied(sname))
            //                {
            //                    withBlock.DeleteCondition(sname);
            //                    withBlock.Update();
            //                    if (withBlock.Status == "出撃")
            //                    {
            //                        GUI.PaintUnitBitmap(u);
            //                    }
            //                }
            //            }
            //return EventData.NextID;
        }
    }
}
