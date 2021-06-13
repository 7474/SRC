using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;

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
                throw new EventErrorException(this, "RemoveFolderコマンドの引数の数が違います");
            }

            var fname = GetArgAsString(2);
            if (Strings.InStr(fname, @"..\") > 0)
            {
                throw new EventErrorException(this, @"フォルダ指定に「..\」は使えません");
            }

            if (Strings.InStr(fname, "../") > 0)
            {
                throw new EventErrorException(this, "フォルダ指定に「../」は使えません");
            }

            if (Strings.Right(fname, 1) == @"\")
            {
                fname = Strings.Left(fname, Strings.Len(fname) - 1);
            }

            var path = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname);
            SRC.FileSystem.RmDir(path);

            return EventData.NextID;
        }
    }
}
