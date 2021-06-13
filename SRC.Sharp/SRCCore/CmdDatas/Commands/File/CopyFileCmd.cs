using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;
using System;
using System.Linq;

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
                throw new EventErrorException(this, "CopyFileコマンドの引数の数が違います");
            }

            var fname1 = GetArgAsString(2);
            var sourceFileDirs = new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            };
            var sourceFilePath = sourceFileDirs.Select(x => SRC.FileSystem.PathCombine(x, fname1))
                .FirstOrDefault(x => SRC.FileSystem.FileExists(x));
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                return EventData.NextID;
            }

            if (Strings.InStr(fname1, @"..\") > 0)
            {
                throw new EventErrorException(this, @"ファイル指定に「..\」は使えません");
            }

            if (Strings.InStr(fname1, "../") > 0)
            {
                throw new EventErrorException(this, "ファイル指定に「../」は使えません");
            }

            var fname2 = GetArgAsString(3);
            var destFilePath = SRC.FileSystem.PathCombine(SRC.ScenarioPath, GetArgAsString(3));
            if (Strings.InStr(fname2, @"..\") > 0)
            {
                throw new EventErrorException(this, @"ファイル指定に「..\」は使えません");
            }

            if (Strings.InStr(fname2, "../") > 0)
            {
                throw new EventErrorException(this, "ファイル指定に「../」は使えません");
            }

            using (var source = SRC.FileSystem.Open(sourceFilePath))
            using (var dest = SRC.FileSystem.OpenSafe(Filesystem.SafeOpenMode.Write, destFilePath))
            {
                source.CopyTo(dest);
            }
            return EventData.NextID;
        }
    }
}
