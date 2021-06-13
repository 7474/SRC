using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveFileCmd : CmdData
    {
        public RemoveFileCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveFileCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                Event.EventErrorMessage = "RemoveFileコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 410742


                Input:
                            Error(0)

                 */
            }

            fname = SRC.ScenarioPath + GetArgAsString(2);
            if (Strings.InStr(fname, @"..\") > 0)
            {
                Event.EventErrorMessage = @"ファイル指定に「..\」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 410987


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname, "../") > 0)
            {
                Event.EventErrorMessage = "ファイル指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 411157


                Input:
                            Error(0)

                 */
            }

            if (SRC.FileSystem.FileExists(fname))
            {
                FileSystem.Kill(fname);
            }

            return EventData.NextID;
        }
    }
}
