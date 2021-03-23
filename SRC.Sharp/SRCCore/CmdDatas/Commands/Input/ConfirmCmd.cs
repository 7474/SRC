using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ConfirmCmd : CmdData
    {
        public ConfirmCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ConfirmCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecConfirmCmdRet = default;
            short ret;
            if ((int)ArgNum != 2)
            {
                Event_Renamed.EventErrorMessage = "Confirmコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 198480


                Input:
                            Error(0)

                 */
            }

            // 一度イベントを解消しておかないとMsgBoxを連続で使用したときに
            // 動作がおかしくなる（ＶＢのバグ？）
            Application.DoEvents();
            ret = (short)Interaction.MsgBox(GetArgAsString(2), (MsgBoxStyle)((int)MsgBoxStyle.OkCancel + (int)MsgBoxStyle.Question), "選択");
            if (ret == 1)
            {
                Event_Renamed.SelectedAlternative = 1.ToString();
            }
            else
            {
                Event_Renamed.SelectedAlternative = 0.ToString();
            }

            return EventData.ID + 1;
        }
    }
}
