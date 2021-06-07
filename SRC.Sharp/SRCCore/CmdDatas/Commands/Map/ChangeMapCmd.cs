using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeMapCmd : CmdData
    {
        public ChangeMapCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeMapCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string fname;
            var late_refresh = default(bool);
            switch (ArgNum)
            {
                case 2:
                    break;
                // ＯＫ
                case 3:
                    if (GetArgAsString((short)3) == "非同期")
                    {
                        late_refresh = true;
                    }
                    else
                    {
                        throw new EventErrorException(this, "ChangeMapコマンドのオプションが不正です");
                    }
                    break;
                default:
                    throw new EventErrorException(this, "ChangeMapコマンドの引数の数が違います");
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // 出撃中のユニットを撤退させる
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃" || u.Status == "格納")
                {
                    if (late_refresh)
                    {
                        u.Escape("非同期");
                    }
                    else
                    {
                        u.Escape();
                    }
                }
            }

            fname = GetArgAsString(2);
            if (Strings.Len(fname) > 0)
            {
                Map.LoadMapData(SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname));
            }
            else
            {
                Map.LoadMapData("");
            }

            if (late_refresh)
            {
                GUI.SetupBackground("", "非同期");
                GUI.RedrawScreen(true);
            }
            else
            {
                GUI.SetupBackground(draw_mode: "", draw_option: "");
                GUI.RedrawScreen();
            }

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
