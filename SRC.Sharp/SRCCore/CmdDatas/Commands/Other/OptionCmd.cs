using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class OptionCmd : CmdData
    {
        public OptionCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OptionCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string vname;
            switch (ArgNum)
            {
                case 2:
                    {
                        vname = GetArgAsString(2);
                        vname = "Option(" + vname + ")";
                        if (!Expression.IsGlobalVariableDefined(vname))
                        {
                            Expression.DefineGlobalVariable(vname);
                        }

                        Expression.SetVariableAsLong(vname, 1);
                        // TODO NewGUI
                        //if (vname == "Option(新ＧＵＩ)")
                        //{
                        //    // 新ＧＵＩが指定されたら即反映するためにメイン画面をロードしなおす
                        //    GUI.LoadForms();
                        //}

                        break;
                    }
                case 3:
                    {
                        vname = GetArgAsString(2);
                        vname = "Option(" + vname + ")";
                        if (Expression.IsGlobalVariableDefined(vname))
                        {
                            Expression.UndefineVariable(vname);
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "Optionコマンドの引数の数が違います");
            }
            return EventData.NextID;
        }
    }
}
