using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class MonotoneCmd : CmdData
    {
        public MonotoneCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MonotoneCmd, eventData)
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
                            throw new EventErrorException(this, "Monotoneコマンドに不正なオプション「" + buf + "」が使われています");
                        }
                }
            }

            // SetupBackground はマップの表示位置を中央に戻すので、元の位置を覚えておいて戻す
            var prev_x = GUI.MapX;
            var prev_y = GUI.MapY;

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("白黒", "非同期");
            GUI.Center(prev_x, prev_y);
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
