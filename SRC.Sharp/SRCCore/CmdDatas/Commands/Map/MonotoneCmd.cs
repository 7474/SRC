using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MonotoneCmd : CmdData
    {
        public MonotoneCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MonotoneCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            short prev_x, prev_y;
            bool late_refresh;
            short i;
            string buf;
          var  late_refresh = false;
            Map.MapDrawIsMapOnly = false;
            var loopTo = ArgNum;
            for (i = 2; i <= loopTo; i++)
            {
                buf = GetArgAsString(i);
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
                            Event.EventErrorMessage = "Monotoneコマンドに不正なオプション「" + buf + "」が使われています";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 344669


                            Input:
                                                Error(0)

                             */
                            break;
                        }
                }
            }

            prev_x = GUI.MapX;
            prev_y = GUI.MapY;

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;
            GUI.SetupBackground("白黒", "非同期", filter_color: 0, filter_trans_par: 0d);
            foreach (Unit u in SRC.UList)
            {
                {
                    var withBlock = u;
                    if (withBlock.Status_Renamed == "出撃")
                    {
                        if (withBlock.BitmapID == 0)
                        {
                            {
                                var withBlock1 = SRC.UList.Item(withBlock.Name);
                                if ((u.Party0 ?? "") == (withBlock1.Party0 ?? "") && withBlock1.BitmapID != 0 && (u.get_Bitmap(false) ?? "") == (withBlock1.get_Bitmap(false) ?? "") && !withBlock1.IsFeatureAvailable("ダミーユニット"))
                                {
                                    u.BitmapID = withBlock1.BitmapID;
                                }
                                else
                                {
                                    u.BitmapID = GUI.MakeUnitBitmap(u);
                                }
                            }

                            withBlock.Name = Conversions.ToString(argIndex1);
                        }
                    }
                }
            }

            GUI.Center(prev_x, prev_y);
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;

            return EventData.NextID;
        }
    }
}
