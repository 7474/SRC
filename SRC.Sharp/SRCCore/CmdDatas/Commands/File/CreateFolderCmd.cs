using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CreateFolderCmd : CmdData
    {
        public CreateFolderCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CreateFolderCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                Event.EventErrorMessage = "CreateFolderコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 214334


                Input:
                            Error(0)

                 */
            }

            fname = SRC.ScenarioPath + GetArgAsString(2);
            if (Strings.InStr(fname, @"..\") > 0)
            {
                Event.EventErrorMessage = @"フォルダ指定に「..\」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 214579


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname, "../") > 0)
            {
                Event.EventErrorMessage = "フォルダ指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 214749


                Input:
                            Error(0)

                 */
            }

            if (Strings.Right(fname, 1) == @"\")
            {
                fname = Strings.Left(fname, Strings.Len(fname) - 1);
            }

            if (!SRC.FileSystem.FileExists(fname))
            {
                FileSystem.MkDir(fname);
            }

            return EventData.NextID;
        }
    }
}
