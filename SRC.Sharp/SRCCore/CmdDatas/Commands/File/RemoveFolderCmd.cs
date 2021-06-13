using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemoveFolderCmd : CmdData
    {
        public RemoveFolderCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemoveFolderCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                Event.EventErrorMessage = "RemoveFolderコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 411575


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
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 411820


                Input:
                            Error(0)

                 */
            }

            if (Strings.InStr(fname, "../") > 0)
            {
                Event.EventErrorMessage = "ファイル指定に「../」は使えません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 411990


                Input:
                            Error(0)

                 */
            }

            if (Strings.Right(fname, 1) == @"\")
            {
                fname = Strings.Left(fname, Strings.Len(fname) - 1);
            }

            if (SRC.FileSystem.FileExists(fname))
            {
                fso = Interaction.CreateObject("Scripting.FileSystemObject");

                // UPGRADE_WARNING: オブジェクト fso.DeleteFolder の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                fso.DeleteFolder(fname);

                // UPGRADE_NOTE: オブジェクト fso をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                fso = null;
            }

            return EventData.NextID;
        }
    }
}
