using SRCCore.Events;
using SRCCore.Exceptions;

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
            switch (ArgNum)
            {
                case 1:
                    // ＯＫ
                    break;

                case 2:
                    if (GetArgAsString(2) == "非同期")
                    {
                        late_refresh = true;
                    }
                    else
                    {
                        throw new EventErrorException(this, "Noonコマンドのオプションが不正です");
                    }
                    break;

                default:
                    throw new EventErrorException(this, "Noonコマンドの引数の数が違います");
            }
            Map.MapDrawIsMapOnly = false;

            // SetupBackground はマップの表示位置を中央に戻すので、元の位置を覚えておいて戻す
            var prev_x = GUI.MapX;
            var prev_y = GUI.MapY;

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("", "非同期");
            GUI.Center(prev_x, prev_y);
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
