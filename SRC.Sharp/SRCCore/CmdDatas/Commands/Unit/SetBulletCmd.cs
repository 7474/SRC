using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetBulletCmd : CmdData
    {
        public SetBulletCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetBulletCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string wname;
            //            short wid, num;
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 4:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        wname = GetArgAsString(3);
            //                        num = GetArgAsLong(4);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        wname = GetArgAsString(2);
            //                        num = GetArgAsLong(3);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "SetBulletコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 451174


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (Information.IsNumeric(wname))
            //            {
            //                wid = GeneralLib.StrToLng(wname);
            //                if (wid < 1 || u.CountWeapon() < wid)
            //                {
            //                    Event.EventErrorMessage = "武器の番号「" + wname + "」が間違っています";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 451471


            //                    Input:
            //                                        Error(0)

            //                     */
            //                }
            //            }
            //            else
            //            {
            //                var loopTo = u.CountWeapon();
            //                for (wid = 1; wid <= loopTo; wid++)
            //                {
            //                    if ((u.Weapon(wid).Name ?? "") == (wname ?? ""))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (wid < 1 || u.CountWeapon() < wid)
            //                {
            //                    Event.EventErrorMessage = u.Name + "は武器「" + wname + "」を持っていません";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 451756


            //                    Input:
            //                                        Error(0)

            //                     */
            //                }
            //            }

            //            u.SetBullet(wid, GeneralLib.MinLng(num, u.MaxBullet(wid)));
            //return EventData.NextID;
        }
    }
}
