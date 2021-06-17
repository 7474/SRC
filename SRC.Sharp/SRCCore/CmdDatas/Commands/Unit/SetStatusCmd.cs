using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStatusCmd : CmdData
    {
        public SetStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStatusCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            string cname;
            //            switch (ArgNum)
            //            {
            //                case 4:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        {
            //                            var withBlock = u;
            //                            cname = GetArgAsString(3);
            //                            withBlock.AddCondition(cname, GetArgAsLong(4), cdata: "");
            //                            if (withBlock.Status == "出撃")
            //                            {
            //                                GUI.PaintUnitBitmap(u);
            //                            }

            //                            if (cname != "非操作")
            //                            {
            //                                withBlock.Update();
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        if (Event.SelectedUnitForEvent is object)
            //                        {
            //                            {
            //                                var withBlock1 = Event.SelectedUnitForEvent;
            //                                cname = GetArgAsString(2);
            //                                withBlock1.AddCondition(cname, GetArgAsLong(3), cdata: "");
            //                                if (withBlock1.Status == "出撃")
            //                                {
            //                                    GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
            //                                }

            //                                if (cname != "非操作")
            //                                {
            //                                    withBlock1.Update();
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "SetStatusコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 462730


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //return EventData.NextID;
        }
    }
}
