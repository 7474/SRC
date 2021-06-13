using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RenameFileCmd : CmdData
    {
        public RenameFileCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RenameFileCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                Event.EventErrorMessage = "RenameFileコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 431259


                Input:
                            Error(0)

                 */
            }

            fname1 = SRC.ScenarioPath + GetArgAsString(2);
            fname2 = SRC.ScenarioPath + GetArgAsString(3);
            if (Strings.InStr(fname1, @"..\") > 0)
            {
                Event.EventErrorMessage = @"ファイル指定に「..\」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 431574


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname1, "../") > 0)
            {
                Event.EventErrorMessage = "ファイル指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 431745


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname2, @"..\") > 0)
            {
                Event.EventErrorMessage = @"ファイル指定に「..\」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 431916


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname2, "../") > 0)
            {
                Event.EventErrorMessage = "ファイル指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 432087


                Input:
                            Error(0)

                 */
            }

            if (!SRC.FileSystem.FileExists(fname1))
            {
                Event.EventErrorMessage = "元のファイル" + "「" + fname1 + "」が見つかりません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 432267


                Input:
                            Error(0)

                 */
            }

            if (SRC.FileSystem.FileExists(fname2))
            {
                Event.EventErrorMessage = "既に" + "「" + fname2 + "」が存在しています";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 432435


                Input:
                            Error(0)

                 */
            }

            FileSystem.Rename(fname1, fname2);
            return EventData.NextID;
        }
    }
}
