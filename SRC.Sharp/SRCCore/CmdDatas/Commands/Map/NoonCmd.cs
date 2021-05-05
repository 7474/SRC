using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class NoonCmd : CmdData
    {
        public NoonCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NoonCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var late_refresh = false;
            for (var i = 2; i <= ArgNum; i++)
            {
                var buf = GetArgAsString(i);
                switch (buf ?? "")
                {
                    case "非同期":
                        {
                            late_refresh = true;
                            break;
                        }

                    default:
                        {
                            throw new EventErrorException(this, "Noonコマンドに不正なオプション「" + buf + "」が使われています");
                        }
                }
            }
            Map.MapDrawIsMapOnly = false;

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("", "非同期");
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
