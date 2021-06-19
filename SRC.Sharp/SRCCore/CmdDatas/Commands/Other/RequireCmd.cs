using SRCCore.Events;
using SRCCore.Exceptions;

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

            // ファイルが存在する？
            if (!SRC.FileSystem.FileExists(fname))
            {
                throw new EventErrorException(this, "指定されたファイル「" + fname + "」が見つかりません。");
            }

            Event.LoadDynamic(fname);

            return EventData.NextID;
        }
    }
}
