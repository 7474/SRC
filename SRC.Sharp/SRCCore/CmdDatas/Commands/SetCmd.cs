using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SetCmd : CmdData
    {
        public SetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecSetCmdRet = default;
            Expression.ValueType etype;
            var str_result = default(string);
            var num_result = default(double);
            short num;
            num = ArgNum;
            if ((int)num > 3)
            {
                // 過去のバージョンのシナリオを読み込めるようにするため、
                // Setコマンドの後ろの「#」形式のコメントは無視する
                if (Strings.Left(GetArg((short)4), 1) == "#")
                {
                    num = (short)3;
                }
                else
                {
                    Event_Renamed.EventErrorMessage = "Setコマンドの引数の数が違います";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 449659


                    Input:
                                    Error(0)

                     */
                }
            }

            switch (num)
            {
                case 2:
                    {
                        string argvname = GetArg(2);
                        Expression.SetVariableAsLong(ref argvname, 1);
                        break;
                    }

                case 3:
                    {
                        switch (ArgsType[3])
                        {
                            case Expression.ValueType.UndefinedType:
                                {
                                    etype = Expression.EvalTerm(ref strArgs[3], ref Expression.ValueType.UndefinedType, ref str_result, ref num_result);
                                    if (etype == Expression.ValueType.NumericType)
                                    {
                                        string argvname1 = GetArg(2);
                                        Expression.SetVariableAsDouble(ref argvname1, num_result);
                                    }
                                    else
                                    {
                                        string argvname2 = GetArg(2);
                                        Expression.SetVariableAsString(ref argvname2, ref str_result);
                                    }

                                    break;
                                }

                            case Expression.ValueType.StringType:
                                {
                                    string argvname3 = GetArg(2);
                                    Expression.SetVariableAsString(ref argvname3, ref strArgs[3]);
                                    break;
                                }

                            case Expression.ValueType.NumericType:
                                {
                                    string argvname4 = GetArg(2);
                                    Expression.SetVariableAsDouble(ref argvname4, dblArgs[3]);
                                    break;
                                }
                        }

                        break;
                    }
            }

            ExecSetCmdRet = LineNum + 1;
            return ExecSetCmdRet;

        }
    }
}
