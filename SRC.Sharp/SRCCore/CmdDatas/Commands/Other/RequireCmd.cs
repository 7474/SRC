using SRCCore.Events;
using SRCCore.Exceptions;
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
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Requireコマンドの引数の数が違います");
            }

            var argFname = GetArgAsString(2);

            // Requireコマンドで読み込まれたことを記録済み？
            if (Event.AdditionalEventFileNames.Contains(argFname))
            {
                return EventData.NextID;
            }
            // 読み込んだイベントファイルを記録
            Event.AdditionalEventFileNames.Add(argFname);

            // 読み込むファイル名
            var fname = SRC.FileSystem.PathCombine(SRC.ScenarioPath, argFname);

            // 既に読み込まれている場合はスキップ
            if (Event.EventFileNames.Contains(fname))
            {
                return EventData.NextID;
            }

            // ファイルが存在する？
            if (!SRC.FileSystem.FileExists(fname))
            {
                throw new EventErrorException(this, "指定されたファイル「" + fname + "」が見つかりません。");
            }

            // ファイルをロード
            Event.LoadEventData2(fname, EventDataSource.Scenario);

            // ラベルを登録
            Event.RegisterLabel();

            // XXX 多分要らん
            //// コマンドデータ配列を設定
            //if (Information.UBound(Event.EventData) > Information.UBound(Event.EventCmd))
            //{
            //    Array.Resize(Event.EventCmd, Information.UBound(Event.EventData) + 1);
            //    i = Information.UBound(Event.EventData);
            //    while (Event.EventCmd[i] is null)
            //    {
            //        Event.EventCmd[i] = new CmdData();
            //        Event.EventCmd[i].LineNum = i;
            //        i = i - 1;
            //    }
            //}

            //var loopTo4 = Information.UBound(Event.EventData);
            //for (i = file_head; i <= loopTo4; i++)
            //    Event.EventCmd[i].Name = Event.CmdType.NullCmd;

            return EventData.NextID;
        }
    }
}
