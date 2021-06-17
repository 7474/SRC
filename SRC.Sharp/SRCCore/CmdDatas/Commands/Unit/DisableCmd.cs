using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class DisableCmd : CmdData
    {
        public DisableCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DisableCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string vname, aname, uname = default;
            //            short i;
            //            bool need_update;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        aname = GetArgAsString(2);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        uname = GetArgAsString(2);
            //                        aname = GetArgAsString(3);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Disableコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 217817


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (string.IsNullOrEmpty(aname))
            //            {
            //                Event.EventErrorMessage = "Disableコマンドに指定された能力名が空文字列です";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 217954


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            if (!string.IsNullOrEmpty(uname))
            //            {
            //                vname = "Disable(" + uname + "," + aname + ")";
            //            }
            //            else
            //            {
            //                vname = "Disable(" + aname + ")";
            //            }

            //            // Disable用変数を設定
            //            if (!Expression.IsGlobalVariableDefined(vname))
            //            {
            //                Expression.DefineGlobalVariable(vname);
            //                Expression.SetVariableAsLong(vname, 1);
            //            }
            //            else
            //            {
            //                // 既に設定済みであればそのまま終了
            //                ExecDisableCmdRet = LineNum + 1;
            //                return ExecDisableCmdRet;
            //            }

            //            // ユニットのステータスを更新
            //            if (!string.IsNullOrEmpty(uname))
            //            {
            //                {
            //                    var withBlock = SRC.UList;
            //                    if (withBlock.IsDefined(uname))
            //                    {
            //                        Unit localItem() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

            //                        localItem().CurrentForm().Update();
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                foreach (Unit u in SRC.UList)
            //                {
            //                    if (u.Status == "出撃")
            //                    {
            //                        // ステータスを更新する必要があるかどうかチェックする
            //                        need_update = false;
            //                        if (u.IsFeatureAvailable(aname))
            //                        {
            //                            need_update = true;
            //                        }
            //                        else
            //                        {
            //                            var loopTo = u.CountItem();
            //                            for (i = 1; i <= loopTo; i++)
            //                            {
            //                                Item localItem1() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

            //                                if ((localItem1().Name ?? "") == (aname ?? ""))
            //                                {
            //                                    need_update = true;
            //                                    break;
            //                                }
            //                            }
            //                        }

            //                        // 必要がある場合はステータスを更新
            //                        if (need_update)
            //                        {
            //                            u.Update();
            //                        }
            //                    }
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
