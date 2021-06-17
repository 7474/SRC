using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class UpgradeCmd : CmdData
    {
        public UpgradeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UpgradeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 2:
                    {
                        u1 = Event.SelectedUnitForEvent.CurrentForm();
                        uname = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        uname = GetArgAsString(2);
                        bool localIsDefined() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                        if (!localIsDefined())
                        {
                            Event.EventErrorMessage = uname + "というユニットはありません";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 516265


                            Input:
                                                Error(0)

                             */
                        }

                        Unit localItem() { object argIndex1 = (object)uname; var ret = SRC.UList.Item(argIndex1); return ret; }

                        u1 = localItem().CurrentForm();
                        uname = GetArgAsString(3);
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "Upgradeコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 516484


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            bool localIsDefined1() { object argIndex1 = uname; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

            if (!localIsDefined1())
            {
                Event.EventErrorMessage = "ユニット「" + uname + "」のデータが見つかりません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 516662


                Input:
                            Error(0)

                 */
            }

            prev_status = u1.Status;
            u2 = SRC.UList.Add(uname, u1.Rank, u1.Party0);
            u1.Party0 = arguparty;
            if (u2 is null)
            {
                Event.EventErrorMessage = uname + "のユニットデータが不正です";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 516898


                Input:
                            Error(0)

                 */
            }

            if (u1.BossRank > 0)
            {
                u2.BossRank = u1.BossRank;
                u2.FullRecover();
            }

            // パイロットの乗せ換え
            Pilot[] pilot_list;
            Pilot[] support_list;
            if (u1.CountPilot() > 0)
            {
                pilot_list = new Pilot[(u1.CountPilot() + 1)];
                support_list = new Pilot[(u1.CountSupport() + 1)];
                var loopTo = Information.UBound(pilot_list);
                for (i = 1; i <= loopTo; i++)
                {
                    pilot_list[i] = u1.Pilot(i);
                }

                var loopTo1 = Information.UBound(support_list);
                for (i = 1; i <= loopTo1; i++)
                {
                    support_list[i] = u1.Support(i);
                }

                u1.Pilot(1).GetOff();
                var loopTo2 = Information.UBound(pilot_list);
                for (i = 1; i <= loopTo2; i++)
                    pilot_list[i].Ride(u2);
                var loopTo3 = Information.UBound(support_list);
                for (i = 1; i <= loopTo3; i++)
                    support_list[i].Ride(u2);
            }

            // アイテムの交換
            var loopTo4 = u1.CountItem();
            for (i = 1; i <= loopTo4; i++)
            {
                Item localItem1() { object argIndex1 = i; var ret = u1.Item(argIndex1); return ret; }

                u2.AddItem(localItem1());
            }

            var loopTo5 = u1.CountItem();
            for (i = 1; i <= loopTo5; i++)
            {
                u1.DeleteItem(1);
            }

            // リンクの付け替え
            u2.Master = u1.Master;
            // UPGRADE_NOTE: オブジェクト u1.Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            u1.Master = null;
            u2.Summoner = u1.Summoner;
            // UPGRADE_NOTE: オブジェクト u1.Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            u1.Summoner = null;

            // 召喚ユニットの交換
            var loopTo6 = u1.CountServant();
            for (i = 1; i <= loopTo6; i++)
            {
                Unit localServant() { object argIndex1 = i; var ret = u1.Servant(argIndex1); return ret; }

                u2.AddServant(localServant());
            }

            var loopTo7 = u1.CountServant();
            for (i = 1; i <= loopTo7; i++)
            {
                u1.DeleteServant(1);
            }

            // 収納ユニットの交換
            if (u1.IsFeatureAvailable("母艦"))
            {
                var loopTo8 = u1.CountOtherForm();
                for (i = 1; i <= loopTo8; i++)
                {
                    Unit localOtherForm1() { object argIndex1 = i; var ret = u1.OtherForm(argIndex1); return ret; }

                    if (localOtherForm1().Status == "格納")
                    {
                        Unit localOtherForm() { object argIndex1 = i; var ret = u1.OtherForm(argIndex1); return ret; }

                        u2.AddOtherForm(localOtherForm());
                    }
                }

                var loopTo9 = u2.CountOtherForm();
                for (i = 1; i <= loopTo9; i++)
                {
                    Unit localOtherForm3() { object argIndex1 = i; var ret = u2.OtherForm(argIndex1); return ret; }

                    if (localOtherForm3().Status == "格納")
                    {
                        Unit localOtherForm2() { object argIndex1 = i; var ret = u2.OtherForm(argIndex1); return ret; }

                        u1.DeleteOtherForm((object)localOtherForm2().ID);
                    }
                }
            }

            u2.Area = u1.Area;

            // 元のユニットを削除
            u1.Status = "破棄";
            var loopTo10 = u1.CountOtherForm();
            for (i = 1; i <= loopTo10; i++)
            {
                Unit localOtherForm5() { object argIndex1 = i; var ret = u1.OtherForm(argIndex1); return ret; }

                if (localOtherForm5().Status == "他形態")
                {
                    Unit localOtherForm4() { object argIndex1 = i; var ret = u1.OtherForm(argIndex1); return ret; }

                    localOtherForm4().Status = "破棄";
                }
            }

            u2.UsedAction = u1.UsedAction;
            u2.UsedSupportAttack = u1.UsedSupportAttack;
            u2.UsedSupportGuard = u1.UsedSupportGuard;
            u2.UsedSyncAttack = u1.UsedSyncAttack;
            u2.UsedCounterAttack = u1.UsedCounterAttack;
            switch (prev_status ?? "")
            {
                case "出撃":
                    {
                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        Map.MapDataForUnit[u1.x, u1.y] = null;
                        u2.StandBy(u1.x, u1.y);
                        if (!GUI.IsPictureVisible)
                        {
                            GUI.RedrawScreen();
                        }

                        break;
                    }

                case "破壊":
                case "破棄":
                    {
                        if (ReferenceEquals(Map.MapDataForUnit[u1.x, u1.y], u1))
                        {
                            // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            Map.MapDataForUnit[u1.x, u1.y] = null;
                        }

                        u2.StandBy(u1.x, u1.y);
                        if (!GUI.IsPictureVisible)
                        {
                            GUI.RedrawScreen();
                        }

                        break;
                    }

                case "格納":
                    {
                        foreach (Unit u in SRC.UList)
                        {
                            {
                                var withBlock = u;
                                var loopTo11 = withBlock.CountUnitOnBoard();
                                for (i = 1; i <= loopTo11; i++)
                                {
                                    Unit localUnitOnBoard() { object argIndex1 = i; var ret = withBlock.UnitOnBoard(argIndex1); return ret; }

                                    if ((u1.ID ?? "") == (localUnitOnBoard().ID ?? ""))
                                    {
                                        withBlock.UnloadUnit((object)u1.ID);
                                        u2.Land(u, true);
                                        goto ExitLoop;
                                    }
                                }
                            }
                        }

                    ExitLoop:
                        ;
                        break;
                    }

                default:
                    {
                        u2.Status = prev_status;
                        break;
                    }
            }

            // グローバル変数の更新
            if (ReferenceEquals(u1, Commands.SelectedUnit))
            {
                Commands.SelectedUnit = u2;
            }

            if (ReferenceEquals(u1, Event.SelectedUnitForEvent))
            {
                Event.SelectedUnitForEvent = u2;
            }

            if (ReferenceEquals(u1, Commands.SelectedTarget))
            {
                Commands.SelectedTarget = u2;
            }

            if (ReferenceEquals(u1, Event.SelectedTargetForEvent))
            {
                Event.SelectedTargetForEvent = u2;
            }

            var loopTo12 = Commands.SelectionStackIndex;
            for (i = 1; i <= loopTo12; i++)
            {
                if (ReferenceEquals(u1, Commands.SavedSelectedUnit[i]))
                {
                    Commands.SavedSelectedUnit[i] = u2;
                }

                if (ReferenceEquals(u1, Commands.SavedSelectedUnitForEvent[i]))
                {
                    Commands.SavedSelectedUnitForEvent[i] = u2;
                }

                if (ReferenceEquals(u1, Commands.SavedSelectedTarget[i]))
                {
                    Commands.SavedSelectedTarget[i] = u2;
                }

                if (ReferenceEquals(u1, Commands.SavedSelectedTargetForEvent[i]))
                {
                    Commands.SavedSelectedTargetForEvent[i] = u2;
                }
            }

            return EventData.NextID;
        }
    }
}
