using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class ClsCmd : CmdData
    {
        public ClsCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClsCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Color clsColor = Color.Black;
            switch (ArgNum)
            {
                case 2:
                    {
                        var buf = GetArgAsString(2);
                        if (!ColorExtension.TryFromHexString(buf, out clsColor))
                        {
                            throw new EventErrorException(this, "色指定が不正です");
                        }
                        break;
                    }

                case 1:
                    break;

                default:
                    throw new EventErrorException(this, "Clsコマンドの引数の数が違います");
            }
            SRC.GUIScrean.BoxCmd(new ScreanDrawOption
            {
                ForeColor = clsColor,
                FillColor = clsColor,
                FillStyle = VB.FillStyle.VbFSSolid,
            }, 0, 0, GUI.MainPWidth - 1, GUI.MainPHeight - 1);
            GUI.ScreenIsSaved = true;

            GUI.IsPictureVisible = true;
            GUI.IsCursorVisible = false;
            GUI.PaintedAreaX1 = GUI.MainPWidth;
            GUI.PaintedAreaY1 = GUI.MainPHeight;
            GUI.PaintedAreaX2 = -1;
            GUI.PaintedAreaY2 = -1;
            return EventData.NextID;
        }
    }
}
