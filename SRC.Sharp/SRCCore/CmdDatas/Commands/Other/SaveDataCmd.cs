using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Filesystem;

namespace SRCCore.CmdDatas.Commands
{
    public class SaveDataCmd : CmdData
    {
        public SaveDataCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SaveDataCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 1 && ArgNum != 2)
            {
                throw new EventErrorException(this, "SaveDataコマンドの引数の数が違います");
            }

            if (ArgNum == 2)
            {
                // ファイル名指定セーブ
                var fname = GetArgAsString(2);
                using (var saveStream = SRC.FileSystem.OpenSafe(SafeOpenMode.Write, fname))
                {
                    if (saveStream != null)
                    {
                        SRC.UList.Update();
                        SRC.SaveData(saveStream);
                    }
                }
            }
            else
            {
                // ファイル選択ダイアログでセーブ
                using (var saveStream = GUI.SelectSaveStream(SRCSaveKind.Normal))
                {
                    if (saveStream != null)
                    {
                        SRC.UList.Update();
                        SRC.SaveData(saveStream);
                    }
                }
            }

            return EventData.NextID;
        }
    }
}
