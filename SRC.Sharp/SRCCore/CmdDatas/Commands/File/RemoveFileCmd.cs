using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;

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
                throw new EventErrorException(this, "RemoveFileコマンドの引数の数が違います");
            }

            var fname = GetArgAsString(2);
            if (Strings.InStr(fname, @"..\") > 0)
            {
                throw new EventErrorException(this, @"ファイル指定に「..\」は使えません");
            }

            if (Strings.InStr(fname, "../") > 0)
            {
                throw new EventErrorException(this, "ファイル指定に「../」は使えません");
            }

            var path = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname);
            if (SRC.FileSystem.FileExists(path))
            {
                SRC.FileSystem.Remove(path);
            }

            return EventData.NextID;
        }
    }
}
