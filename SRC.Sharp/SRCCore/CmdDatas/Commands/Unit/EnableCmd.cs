using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class EnableCmd : CmdData
    {
        public EnableCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EnableCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {

            switch (ArgNum)
            {
                case 2:
                    {
                        aname = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        uname = GetArgAsString(2);
                        aname = GetArgAsString(3);
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "Enableコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 222673


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(uname))
            {
                vname = "Disable(" + uname + "," + aname + ")";
            }
            else
            {
                vname = "Disable(" + aname + ")";
            }

            // Disable用変数を削除
            if (Expression.IsGlobalVariableDefined(vname))
            {
                Expression.UndefineVariable(vname);
            }
            else
            {
                // 既に設定済みであればそのまま終了
                ExecEnableCmdRet = LineNum + 1;
                return ExecEnableCmdRet;
            }

            // ユニットのステータスを更新
            if (!string.IsNullOrEmpty(uname))
            {
                {
                    var withBlock = SRC.UList;
                    if (withBlock.IsDefined(uname))
                    {
                        Unit localItem() { object argIndex1 = uname; var ret = withBlock.Item(argIndex1); return ret; }

                        localItem().CurrentForm().Update();
                    }
                }
            }
            else
            {
                foreach (Unit u in SRC.UList)
                {
                    if (u.Status == "出撃")
                    {
                        u.Update();
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
