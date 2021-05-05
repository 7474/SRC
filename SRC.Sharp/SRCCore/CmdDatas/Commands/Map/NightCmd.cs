using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class NightCmd : CmdData
    {
        public NightCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NightCmd, eventData)
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
                            throw new EventErrorException(this, "Nightコマンドに不正なオプション「" + buf + "」が使われています");
                        }
                }
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("夜", "非同期", filter_color: 0, filter_trans_par: 0d);
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
