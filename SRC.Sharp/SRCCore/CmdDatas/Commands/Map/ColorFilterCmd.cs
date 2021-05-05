using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class ColorFilterCmd : CmdData
    {
        public ColorFilterCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ColorFilterCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 2)
            {
                throw new EventErrorException(this, "ColorFilterコマンドの引数の数が違います");
            }
            var late_refresh = false;
            Map.MapDrawIsMapOnly = false;
            var trans_par = 0.5d;
            for (var i = 3; i <= ArgNum; i++)
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
                            if (Strings.Right(buf, 1) == "%"
                                && Information.IsNumeric(Strings.Left(buf, Strings.Len(buf) - 1)))
                            {
                                trans_par = GeneralLib.MaxDbl(
                                    0d,
                                    GeneralLib.MinDbl(1d, Conversions.ToDouble(Strings.Left(buf, Strings.Len(buf) - 1)) / 100d));
                            }
                            else
                            {
                                throw new EventErrorException(this, "ColorFilterコマンドに不正なオプション「" + buf + "」が使われています");
                            }

                            break;
                        }
                }
            }
            Color filter_color;
            {

                var buf = GetArgAsString(2);

                if (!ColorExtension.TryFromHexString(buf, out filter_color))
                {
                    throw new EventErrorException(this, "ColorFilterコマンドのカラー指定が不正です");
                }
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);
            GUI.SetupBackground("フィルタ", "非同期", filter_color, trans_par);
            GUI.RedrawScreen(late_refresh);

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);

            return EventData.NextID;
        }
    }
}
