using SRCCore.Events;
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
            int ExecChangeMapCmdRet = default;
            string fname;
            var late_refresh = default(bool);
            switch (ArgNum)
            {
                case 2:
                    {
                        break;
                    }
                // ＯＫ
                case 3:
                    {
                        if (GetArgAsString((short)3) == "非同期")
                        {
                            late_refresh = true;
                        }
                        else
                        {
                            Event_Renamed.EventErrorMessage = "ChangeMapコマンドのオプションが不正です";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 131856


                            Input:
                                                Error(0)

                             */
                        }

                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "ChangeMapコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 131977


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;

            // 出撃中のユニットを撤退させる
            foreach (Unit u in SRC.UList)
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
                string argfname = SRC.ScenarioPath + fname;
                Map.LoadMapData(ref argfname);
            }
            else
            {
                string argfname1 = "";
                Map.LoadMapData(ref argfname1);
            }

            if (late_refresh)
            {
                string argdraw_mode = "";
                string argdraw_option = "非同期";
                int argfilter_color = 0;
                double argfilter_trans_par = 0d;
                GUI.SetupBackground(ref argdraw_mode, ref argdraw_option, filter_color: ref argfilter_color, filter_trans_par: ref argfilter_trans_par);
                GUI.RedrawScreen(true);
            }
            else
            {
                string argdraw_mode1 = "";
                string argdraw_option1 = "";
                int argfilter_color1 = 0;
                double argfilter_trans_par1 = 0d;
                GUI.SetupBackground(draw_mode: ref argdraw_mode1, draw_option: ref argdraw_option1, filter_color: ref argfilter_color1, filter_trans_par: ref argfilter_trans_par1);
                GUI.RedrawScreen();
            }

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
            return EventData.ID + 1;
        }
    }
}
