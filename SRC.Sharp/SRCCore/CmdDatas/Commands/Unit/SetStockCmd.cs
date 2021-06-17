using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStockCmd : CmdData
    {
        public SetStockCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStockCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string aname;
            //            short aid, num;
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 4:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        aname = GetArgAsString(3);
            //                        num = GetArgAsLong(4);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        aname = GetArgAsString(2);
            //                        num = GetArgAsLong(3);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "SetStockコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 466229


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (Information.IsNumeric(aname))
            //            {
            //                aid = GeneralLib.StrToLng(aname);
            //                if (aid < 1 || u.CountAbility() < aid)
            //                {
            //                    Event.EventErrorMessage = "アビリティの番号「" + aname + "」が間違っています";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 466530


            //                    Input:
            //                                        Error(0)

            //                     */
            //                }
            //            }
            //            else
            //            {
            //                var loopTo = u.CountAbility();
            //                for (aid = 1; aid <= loopTo; aid++)
            //                {
            //                    if ((u.Ability(aid).Name ?? "") == (aname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (aid < 1 || u.CountAbility() < aid)
            //                {
            //                    Event.EventErrorMessage = u.Name + "はアビリティ「" + aname + "」を持っていません";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 466821


            //                    Input:
            //                                        Error(0)

            //                     */
            //                }
            //            }

            //            u.SetStock(aid, GeneralLib.MinLng(num, u.MaxStock(aid)));
            //return EventData.NextID;
        }
    }
}
