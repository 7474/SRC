using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SepiaCmd : CmdData
    {
        public SepiaCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SepiaCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var late_refresh = false;
            Map.MapDrawIsMapOnly = false;
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

                    case "マップ限定":
                        {
                            Map.MapDrawIsMapOnly = true;
                            break;
                        }

                    default:
                        {
                            throw new EventErrorException(this, "Sepiaコマンドに不正なオプション「" + buf + "」が使われています");
                        }
                }
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("セピア", "非同期");
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
