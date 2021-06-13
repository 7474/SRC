using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CopyFileCmd : CmdData
    {
        public CopyFileCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CopyFileCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                Event.EventErrorMessage = "CopyFileコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 208824


                Input:
                            Error(0)

                 */
            }

            fname1 = GetArgAsString(2);
            bool localFileExists() { string argfname = SRC.ExtDataPath + fname1; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

            bool localFileExists1() { string argfname = SRC.ExtDataPath2 + fname1; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

            bool localFileExists2() { string argfname = SRC.AppPath + fname1; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + fname1))
            {
                fname1 = SRC.ScenarioPath + fname1;
            }
            else if (localFileExists())
            {
                fname1 = SRC.ExtDataPath + fname1;
            }
            else if (localFileExists1())
            {
                fname1 = SRC.ExtDataPath2 + fname1;
            }
            else if (localFileExists2())
            {
                fname1 = SRC.AppPath + fname1;
            }
            else
            {
                ExecCopyFileCmdRet = LineNum + 1;
                return ExecCopyFileCmdRet;
            }

            if (Strings.InStr(fname1, @"..\") > 0)
            {
                Event.EventErrorMessage = @"ファイル指定に「..\」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 209700


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname1, "../") > 0)
            {
                Event.EventErrorMessage = "ファイル指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 209871


                Input:
                            Error(0)

                 */
            }

            fname2 = SRC.ScenarioPath + GetArgAsString(3);
            if (Strings.InStr(fname2, @"..\") > 0)
            {
                Event.EventErrorMessage = @"ファイル指定に「..\」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 210118


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname2, "../") > 0)
            {
                Event.EventErrorMessage = "ファイル指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 210289


                Input:
                            Error(0)

                 */
            }

            FileSystem.FileCopy(fname1, fname2);
            return EventData.NextID;
        }
    }
}
