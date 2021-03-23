using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class InputCmd : CmdData
    {
        public InputCmd(SRC src, EventDataLine eventData) : base(src, CmdType.InputCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecInputCmdRet = default;
            // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            string str_Renamed;
            switch (ArgNum)
            {
                case 3:
                    {
                        str_Renamed = Interaction.InputBox(GetArgAsString((short)3), "SRC");
                        break;
                    }

                case 4:
                    {
                        str_Renamed = Interaction.InputBox(GetArgAsString((short)3), "SRC", GetArgAsString((short)4));
                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "Inputコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 296282


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            string argvname = GetArg(2);
            Expression.SetVariableAsString(ref argvname, ref str_Renamed);
            return EventData.ID + 1;
        }
    }
}
