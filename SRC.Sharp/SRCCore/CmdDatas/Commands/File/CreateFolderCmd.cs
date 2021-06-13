using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;
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
                throw new EventErrorException(this, "CreateFolderコマンドの引数の数が違います");
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

            var dir = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname);

            if (!SRC.FileSystem.FileExists(dir))
            {
                SRC.FileSystem.MkDir(dir);
            }

            return EventData.NextID;
        }
    }
}
