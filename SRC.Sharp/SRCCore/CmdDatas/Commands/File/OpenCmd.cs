using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.VB;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OpenCmd : CmdData
    {
        public OpenCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OpenCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 6)
            {
                throw new EventErrorException(this, "Openコマンドの引数の数が違います");
            }

            var fname = SRC.ScenarioPath + GetArgAsString(2);
            if (Strings.InStr(fname, @"..\") > 0)
            {
                throw new EventErrorException(this, @"ファイル指定に「..\」は使えません");
            }

            if (Strings.InStr(fname, "../") > 0)
            {
                throw new EventErrorException(this, "ファイル指定に「../」は使えません");
            }

            var opt = GetArgAsString(4);
            var vname = GetArg(6);
            FileHandle f;
            switch (opt ?? "")
            {
                case "出力":
                    f = SRC.FileHandleManager.Add(
                        SafeOpenMode.Write,
                        SRC.SystemConfig.SRCCompatibilityMode,
                        SRC.FileSystem.OpenSafe(SafeOpenMode.Write, vname));
                    break;

                case "追加出力":
                    f = SRC.FileHandleManager.Add(
                        SafeOpenMode.Append,
                        SRC.SystemConfig.SRCCompatibilityMode,
                        SRC.FileSystem.OpenSafe(SafeOpenMode.Append, vname));
                    break;

                case "入力":
                    {
                        if (!SRC.FileSystem.FileExists(fname))
                        {
                            throw new EventErrorException(this, fname + "というファイルは存在しません");
                        }

                        f = SRC.FileHandleManager.Add(
                            SafeOpenMode.Read,
                            SRC.SystemConfig.SRCCompatibilityMode,
                            SRC.FileSystem.OpenSafe(SafeOpenMode.Read, vname));
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ファイルの入出力モードが不正です");
            }
            Expression.SetVariableAsLong(vname, f.Handle);

            return EventData.NextID;
        }
    }
}
