using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeMapCmd : CmdData
    {
        public ChangeMapCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExitCmd, eventData)
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
                if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納")
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
                // XXX パス解決してストリーム開くのどこでやるか
                using (var stream = SRC.Filesystem.Open(SRC.ScenarioPath, fname))
                {
                    Map.LoadMapData(fname, stream);
                }
            }
            else
            {
                string argfname1 = "";
                Map.LoadMapData(argfname1);
            }

            if (late_refresh)
            {
                string argdraw_mode = "";
                string argdraw_option = "非同期";
                int argfilter_color = 0;
                double argfilter_trans_par = 0d;
                GUI.SetupBackground(argdraw_mode, argdraw_option, filter_color: argfilter_color, filter_trans_par: argfilter_trans_par);
                GUI.RedrawScreen(true);
            }
            else
            {
                string argdraw_mode1 = "";
                string argdraw_option1 = "";
                int argfilter_color1 = 0;
                double argfilter_trans_par1 = 0d;
                GUI.SetupBackground(draw_mode: argdraw_mode1, draw_option: argdraw_option1, filter_color: argfilter_color1, filter_trans_par: argfilter_trans_par1);
                GUI.RedrawScreen();
            }

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.ID + 1;
        }
    }
}
