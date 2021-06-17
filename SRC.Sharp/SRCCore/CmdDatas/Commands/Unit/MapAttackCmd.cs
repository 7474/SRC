using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MapAttackCmd : CmdData
    {
        public MapAttackCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MapAttackCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            short tx, ty;
            //            short w;
            //            short prev_w, prev_tw;
            //            string cur_stage;
            //            bool is_event;
            //            short num;
            //            num = ArgNum;
            //            is_event = true;
            //            if (num <= 6)
            //            {
            //                if (GetArgAsString(num) == "通常戦闘")
            //                {
            //                    is_event = false;
            //                    num = (num - 1);
            //                }
            //            }

            //            switch (num)
            //            {
            //                case 5:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        {
            //                            var withBlock = u;
            //                            var loopTo = withBlock.CountWeapon();
            //                            for (w = 1; w <= loopTo; w++)
            //                            {
            //                                if ((GetArgAsString(3) ?? "") == (withBlock.Weapon(w).Name ?? "") && withBlock.IsWeaponClassifiedAs(w, "Ｍ"))
            //                                {
            //                                    break;
            //                                }
            //                            }

            //                            if (w > withBlock.CountWeapon())
            //                            {
            //                                Event.EventErrorMessage = "マップ攻撃名が間違っています";
            //                                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 341692


            //                                Input:
            //                                                        Error(0)

            //                                 */
            //                            }
            //                        }

            //                        tx = GetArgAsLong(4);
            //                        if (tx < 1)
            //                        {
            //                            tx = 1;
            //                        }
            //                        else if (tx > Map.MapWidth)
            //                        {
            //                            tx = Map.MapWidth;
            //                        }

            //                        ty = GetArgAsLong(5);
            //                        if (ty < 1)
            //                        {
            //                            ty = 1;
            //                        }
            //                        else if (ty > Map.MapHeight)
            //                        {
            //                            ty = Map.MapHeight;
            //                        }

            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        var loopTo1 = u.CountWeapon();
            //                        for (w = 1; w <= loopTo1; w++)
            //                        {
            //                            if ((GetArgAsString(2) ?? "") == (u.Weapon(w).Name ?? "") && u.IsWeaponClassifiedAs(w, "Ｍ"))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (w > u.CountWeapon())
            //                        {
            //                            Event.EventErrorMessage = "マップ攻撃名が間違っています";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 342439


            //                            Input:
            //                                                    Error(0)

            //                             */
            //                        }

            //                        tx = GetArgAsLong(3);
            //                        if (tx < 1)
            //                        {
            //                            tx = 1;
            //                        }
            //                        else if (tx > Map.MapWidth)
            //                        {
            //                            tx = Map.MapWidth;
            //                        }

            //                        ty = GetArgAsLong(4);
            //                        if (ty < 1)
            //                        {
            //                            ty = 1;
            //                        }
            //                        else if (ty > Map.MapHeight)
            //                        {
            //                            ty = Map.MapHeight;
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "MapAttackコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 342921


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u.Status != "出撃")
            //            {
            //                Event.EventErrorMessage = u.Nickname + "は出撃していません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 343078


            //                Input:
            //                                Error(0)

            //                 */
            //            }

            //            // ステージを仮想的に変更しておく
            //            cur_stage = SRC.Stage;
            //            SRC.Stage = u.Party;
            //            prev_w = Commands.SelectedWeapon;
            //            prev_tw = Commands.SelectedTWeapon;
            //            Commands.SelectedWeapon = w;
            //            Commands.SelectedTWeapon = 0;
            //            Commands.SelectedX = tx;
            //            Commands.SelectedY = ty;
            //            u.MapAttack(w, tx, ty, is_event);
            //            Commands.SelectedWeapon = prev_w;
            //            Commands.SelectedTWeapon = prev_tw;
            //            SRC.Stage = cur_stage;
            //            GUI.RedrawScreen();
            //return EventData.NextID;
        }
    }
}
