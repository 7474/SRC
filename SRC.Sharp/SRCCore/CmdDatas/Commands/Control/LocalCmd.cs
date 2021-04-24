using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class LocalCmd : CmdData
    {
        public LocalCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LocalCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string vname;
            ValueType etype;
            string str_result = "";
            double num_result = 0d;

            // XXX 定義済を再定義した時

            // 代入式付きの変数定義？
            if (ArgNum >= 4)
            {
                if (GetArg(3) == "=")
                {
                    if (Event.VarIndex >= Event.MaxVarIndex)
                    {
                        Event.VarIndex = Event.MaxVarIndex;
                        throw new EventErrorException(this, Event.MaxVarIndex + "個を超えるサブルーチンローカル変数は作成できません");
                    }

                    vname = GetArg(2);
                    if (Strings.InStr(vname, "\"") > 0)
                    {
                        throw new EventErrorException(this, "変数名「" + vname + "」が不正です");
                    }

                    if (Strings.Asc(vname) == 36) // $
                    {
                        vname = Strings.Mid(vname, 2);
                    }

                    if (ArgNum == 4)
                    {
                        var arg = GetArgRaw(4);
                        switch (arg.argType)
                        {
                            case ValueType.UndefinedType:
                                {
                                    etype = Expression.EvalTerm(arg.strArg, ValueType.UndefinedType, out str_result, out num_result);
                                    Event.VarIndex = (Event.VarIndex + 1);
                                    {
                                        var v = Event.VarStack[Event.VarIndex];
                                        v.Name = vname;
                                        v.VariableType = etype;
                                        v.StringValue = str_result;
                                        v.NumericValue = num_result;
                                    }

                                    break;
                                }

                            case ValueType.StringType:
                                {
                                    Event.VarIndex = (Event.VarIndex + 1);
                                    {
                                        var v = Event.VarStack[Event.VarIndex];
                                        v.Name = vname;
                                        v.VariableType = ValueType.StringType;
                                        v.StringValue = arg.strArg;
                                        v.NumericValue = num_result;
                                    }

                                    break;
                                }

                            case ValueType.NumericType:
                                {
                                    Event.VarIndex = (Event.VarIndex + 1);
                                    {
                                        var v = Event.VarStack[Event.VarIndex];
                                        v.Name = vname;
                                        v.VariableType = ValueType.NumericType;
                                        v.StringValue = str_result;
                                        v.NumericValue = arg.dblArg;
                                    }

                                    break;
                                }
                        }
                    }
                    else
                    {
                        var arg = "(" + string.Join(" ", Enumerable.Range(4, ArgNum - 3).Select(x => GetArg(x))) + ")";
                        etype = Expression.EvalTerm(arg, ValueType.UndefinedType, out str_result, out num_result);
                        Event.VarIndex = (Event.VarIndex + 1);
                        {
                            var v = Event.VarStack[Event.VarIndex];
                            v.Name = vname;
                            v.VariableType = ValueType.NumericType;
                            v.StringValue = str_result;
                            v.NumericValue = num_result;
                        }
                    }

                    return EventData.NextID;
                }
            }
            Event.VarIndex = (Event.VarIndex + ArgNum - 1);
            if (Event.VarIndex > Event.MaxVarIndex)
            {
                Event.VarIndex = Event.MaxVarIndex;
                throw new EventErrorException(this, Event.MaxVarIndex + "個を超えるサブルーチンローカル変数は作成できません");
            }

            for (var i = 2; i <= ArgNum; i++)
            {
                {
                    var v = Event.VarStack[Event.VarIndex - i + 2];
                    vname = GetArg(i);
                    if (Strings.InStr(vname, "\"") > 0)
                    {
                        throw new EventErrorException(this, "変数名「" + vname + "」が不正です");
                    }

                    if (Strings.Asc(vname) == 36) // $
                    {
                        vname = Strings.Mid(vname, 2);
                    }

                    v.Name = vname;
                    v.VariableType = ValueType.StringType;
                    v.StringValue = "";
                }
            }
            return EventData.NextID;
        }
    }
}
