using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class EndCmd : CmdData
    {
        public EndCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EndCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // VB 版の CmdData.Exec には End・Suspend の分岐が無く、Case Else のエラーになる
            // （Talk などの中の End・Suspend は Talk などのコマンドが読み飛ばす）
            throw new EventErrorException(this, GeneralLib.ListIndex(EventData.Data, 1) + "というコマンドは存在しません");
        }
    }
}
