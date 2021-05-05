using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;
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

            // XXX これなんで退避してんの？
            var prev_x = GUI.MapX;
            var prev_y = GUI.MapY;

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("白黒", "非同期", filter_color: 0, filter_trans_par: 0d);
            // XXX 動的にやってるはず
            //foreach (Unit u in SRC.UList.Items)
            //{
            //    if (u.Status == "出撃")
            //    {
            //        if (u.BitmapID == 0)
            //        {
            //            {
            //                var withBlock1 = SRC.UList.Item((string)u.Name);
            //                if ((u.Party0 ?? "") == (withBlock1.Party0 ?? "") && withBlock1.BitmapID != 0 && (u.get_Bitmap(false) ?? "") == (withBlock1.get_Bitmap(false) ?? "") && !withBlock1.IsFeatureAvailable("ダミーユニット"))
            //                {
            //                    u.BitmapID = withBlock1.BitmapID;
            //                }
            //                else
            //                {
            //                    u.BitmapID = GUI.MakeUnitBitmap(u);
            //                }
            //            }
            //        }
            //    }
            //}

            GUI.Center(prev_x, prev_y);
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
