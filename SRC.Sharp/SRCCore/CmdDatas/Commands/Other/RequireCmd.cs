using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RequireCmd : CmdData
    {
        public RequireCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RequireCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string fname;
            //            int file_head;
            //            int i;
            //            string buf;
            //            if (ArgNum != 2)
            //            {
            //                Event.EventErrorMessage = "Requireコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 435110


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            // LoadEventData2内でLineNumは書き換えられるのでここで設定
            //            ExecRequireCmdRet = LineNum + 1;

            //            // ADD START マージ
            //            // Requireコマンドで読み込まれたことを記録済み？
            //            var loopTo = Information.UBound(Event.AdditionalEventFileNames);
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                if ((GetArgAsString(2) ?? "") == (Event.AdditionalEventFileNames[i] ?? ""))
            //                {
            //                    return ExecRequireCmdRet;
            //                }
            //            }

            //            // 読み込んだイベントファイルを記録
            //            Array.Resize(Event.AdditionalEventFileNames, Information.UBound(Event.AdditionalEventFileNames) + 1 + 1);
            //            Event.AdditionalEventFileNames[Information.UBound(Event.AdditionalEventFileNames)] = GetArgAsString(2);
            //            // ADD END マージ

            //            // 読み込むファイル名
            //            fname = SRC.ScenarioPath + GetArgAsString(2);

            //            // 既に読み込まれている場合はスキップ
            //            var loopTo1 = Information.UBound(Event.EventFileNames);
            //            for (i = 1; i <= loopTo1; i++)
            //            {
            //                if ((fname ?? "") == (Event.EventFileNames[i] ?? ""))
            //                {
            //                    return ExecRequireCmdRet;
            //                }
            //            }

            //            // ファイルが存在する？
            //            if (!SRC.FileSystem.FileExists(fname))
            //            {
            //                Event.EventErrorMessage = "指定されたファイル「" + fname + "」が見つかりません。";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 436428


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            // ファイルをロード
            //            file_head = Information.UBound(Event.EventData) + 1;
            //            Event.LoadEventData2(fname, Information.UBound(Event.EventData));

            //            // エラー表示用にサイズを大きく取っておく
            //            Array.Resize(Event.EventData, Information.UBound(Event.EventData) + 1 + 1);
            //            Array.Resize(Event.EventLineNum, Information.UBound(Event.EventData) + 1);
            //            Event.EventData[Information.UBound(Event.EventData)] = "";
            //            Event.EventLineNum[Information.UBound(Event.EventData)] = (Event.EventLineNum[Information.UBound(Event.EventData) - 1] + 1);

            //            // 複数行に分割されたコマンドを結合
            //            var loopTo2 = Information.UBound(Event.EventData) - 1;
            //            for (i = file_head; i <= loopTo2; i++)
            //            {
            //                if (Strings.Right(Event.EventData[i], 1) == "_")
            //                {
            //                    Event.EventData[i + 1] = Strings.Left(Event.EventData[i], Strings.Len(Event.EventData[i]) - 1) + Event.EventData[i + 1];
            //                    Event.EventData[i] = " ";
            //                }
            //            }

            //            // ラベルを登録
            //            var loopTo3 = Information.UBound(Event.EventData);
            //            for (i = file_head; i <= loopTo3; i++)
            //            {
            //                buf = Event.EventData[i];
            //                if (Strings.Right(buf, 1) == ":")
            //                {
            //                    Event.AddLabel(Strings.Left(buf, Strings.Len(buf) - 1), i);
            //                }
            //            }

            //            // コマンドデータ配列を設定
            //            if (Information.UBound(Event.EventData) > Information.UBound(Event.EventCmd))
            //            {
            //                Array.Resize(Event.EventCmd, Information.UBound(Event.EventData) + 1);
            //                i = Information.UBound(Event.EventData);
            //                while (Event.EventCmd[i] is null)
            //                {
            //                    Event.EventCmd[i] = new CmdData();
            //                    Event.EventCmd[i].LineNum = i;
            //                    i = i - 1;
            //                }
            //            }

            //            var loopTo4 = Information.UBound(Event.EventData);
            //            for (i = file_head; i <= loopTo4; i++)
            //                Event.EventCmd[i].Name = Event.CmdType.NullCmd;

            //            // 読み込んだイベントファイルを記録
            //            Array.Resize(Event.AdditionalEventFileNames, Information.UBound(Event.AdditionalEventFileNames) + 1 + 1);
            //            Event.AdditionalEventFileNames[Information.UBound(Event.AdditionalEventFileNames)] = GetArgAsString(2);
            //            
            //return EventData.NextID;
        }
    }
}
