using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveUnitCmd : CmdData
    {
        public RemoveUnitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveUnitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string uname;
            //            Unit u;
            //            short i, num;
            //            var opt = default(string);
            //            num = ArgNum;
            //            if (num > 1)
            //            {
            //                if (GetArgAsString(num) == "非同期")
            //                {
            //                    opt = "非同期";
            //                    num = (num - 1);
            //                }
            //            }

            //            switch (num)
            //            {
            //                case 1:
            //                    {
            //                        {
            //                            var withBlock = Event.SelectedUnitForEvent.CurrentForm();
            //                            withBlock.Escape(opt);
            //                            if (withBlock.CountPilot() > 0)
            //                            {
            //                                withBlock.Pilot((object)1).GetOff();
            //                            }

            //                            withBlock.Status = "破棄";
            //                            var loopTo = withBlock.CountOtherForm();
            //                            for (i = 1; i <= loopTo; i++)
            //                            {
            //                                Unit localOtherForm1() { object argIndex1 = (object)i; var ret = withBlock.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm1().Status == "他形態")
            //                                {
            //                                    Unit localOtherForm() { object argIndex1 = (object)i; var ret = withBlock.OtherForm(argIndex1); return ret; }

            //                                    localOtherForm().Status = "破棄";
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        uname = GetArgAsString(2);
            //                        u = SRC.UList.Item((object)uname);

            //                        // ユニットが存在しなければそのまま終了
            //                        if (u is null)
            //                        {
            //                            ExecRemoveUnitCmdRet = LineNum + 1;
            //                            return ExecRemoveUnitCmdRet;
            //                        }

            //                        // ユニットＩＤで指定された場合
            //                        if ((u.ID ?? "") == (uname ?? ""))
            //                        {
            //                            u.Escape(opt);
            //                            if (u.CountPilot() > 0)
            //                            {
            //                                u.Pilot((object)1).GetOff();
            //                            }

            //                            u.Status = "破棄";
            //                            var loopTo1 = u.CountOtherForm();
            //                            for (i = 1; i <= loopTo1; i++)
            //                            {
            //                                Unit localOtherForm3() { object argIndex1 = (object)i; var ret = u.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm3().Status == "他形態")
            //                                {
            //                                    Unit localOtherForm2() { object argIndex1 = (object)i; var ret = u.OtherForm(argIndex1); return ret; }

            //                                    localOtherForm2().Status = "破棄";
            //                                }
            //                            }

            //                            ExecRemoveUnitCmdRet = LineNum + 1;
            //                            return ExecRemoveUnitCmdRet;
            //                        }

            //                        // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            //                        // 名前をデータのそれとあわせる
            //                        if (SRC.UDList.IsDefined((object)uname))
            //                        {
            //                            UnitData localItem() { object argIndex1 = (object)uname; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                            uname = localItem().Name;
            //                        }

            //                        // パイロットが乗ってないユニットを優先
            //                        foreach (Unit currentU in SRC.UList)
            //                        {
            //                            u = currentU;
            //                            {
            //                                var withBlock1 = u.CurrentForm();
            //                                if ((withBlock1.Name ?? "") == (uname ?? "") && withBlock1.Status != "破棄")
            //                                {
            //                                    if (withBlock1.CountPilot() == 0)
            //                                    {
            //                                        withBlock1.Escape(opt);
            //                                        withBlock1.Status = "破棄";
            //                                        var loopTo2 = withBlock1.CountOtherForm();
            //                                        for (i = 1; i <= loopTo2; i++)
            //                                        {
            //                                            Unit localOtherForm5() { object argIndex1 = (object)i; var ret = withBlock1.OtherForm(argIndex1); return ret; }

            //                                            if (localOtherForm5().Status == "他形態")
            //                                            {
            //                                                Unit localOtherForm4() { object argIndex1 = (object)i; var ret = withBlock1.OtherForm(argIndex1); return ret; }

            //                                                localOtherForm4().Status = "破棄";
            //                                            }
            //                                        }

            //                                        ExecRemoveUnitCmdRet = LineNum + 1;
            //                                        return ExecRemoveUnitCmdRet;
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        // 見つからなければパイロットが乗っているユニットを削除
            //                        foreach (Unit currentU1 in SRC.UList)
            //                        {
            //                            u = currentU1;
            //                            {
            //                                var withBlock2 = u.CurrentForm();
            //                                if ((withBlock2.Name ?? "") == (uname ?? "") && withBlock2.Status != "破棄")
            //                                {
            //                                    withBlock2.Escape(opt);
            //                                    withBlock2.Pilot((object)1).GetOff();
            //                                    withBlock2.Status = "破棄";
            //                                    var loopTo3 = withBlock2.CountOtherForm();
            //                                    for (i = 1; i <= loopTo3; i++)
            //                                    {
            //                                        Unit localOtherForm7() { object argIndex1 = (object)i; var ret = withBlock2.OtherForm(argIndex1); return ret; }

            //                                        if (localOtherForm7().Status == "他形態")
            //                                        {
            //                                            Unit localOtherForm6() { object argIndex1 = (object)i; var ret = withBlock2.OtherForm(argIndex1); return ret; }

            //                                            localOtherForm6().Status = "破棄";
            //                                        }
            //                                    }

            //                                    ExecRemoveUnitCmdRet = LineNum + 1;
            //                                    return ExecRemoveUnitCmdRet;
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "RemoveUnitの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 430173


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
