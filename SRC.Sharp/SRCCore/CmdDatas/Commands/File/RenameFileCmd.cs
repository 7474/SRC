using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;

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
                throw new EventErrorException(this, "RenameFileコマンドの引数の数が違います");
            }

            var fname1 = GetArgAsString(2);
            var fname2 = GetArgAsString(3);
            var sourceFilePath = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname1);
            var destFilePath = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname2);
            if (Strings.InStr(fname1, @"..\") > 0)
            {
                throw new EventErrorException(this, @"ファイル指定に「..\」は使えません");
            }

            if (Strings.InStr(fname1, "../") > 0)
            {
                throw new EventErrorException(this, "ファイル指定に「../」は使えません");
            }
            if (Strings.InStr(fname2, @"..\") > 0)
            {
                throw new EventErrorException(this, @"ファイル指定に「..\」は使えません");
            }

            if (Strings.InStr(fname2, "../") > 0)
            {
                throw new EventErrorException(this, "ファイル指定に「../」は使えません");
            }

            if (!SRC.FileSystem.FileExists(sourceFilePath))
            {
                throw new EventErrorException(this, "元のファイル" + "「" + fname1 + "」が見つかりません");
            }

            if (SRC.FileSystem.FileExists(destFilePath))
            {
                throw new EventErrorException(this, "既に" + "「" + fname2 + "」が存在しています");
            }

            using (var source = SRC.FileSystem.Open(sourceFilePath))
            using (var dest = SRC.FileSystem.OpenSafe(Filesystem.SafeOpenMode.Write, destFilePath))
            {
                source.CopyTo(dest);
            }
            SRC.FileSystem.Remove(sourceFilePath);

            return EventData.NextID;
        }
    }
}
