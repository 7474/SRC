using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class FillStyleCmd : CmdData
    {
        public FillStyleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FillStyleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "FillStyleコマンドの引数の数が違います");
            }

            switch (GetArgAsString(2) ?? "")
            {
                case "塗りつぶし":
                    {
                        Event.ObjFillStyle = FillStyle.VbFSSolid;
                        break;
                    }

                case "透明":
                    {
                        Event.ObjFillStyle = FillStyle.VbFSTransparent;
                        break;
                    }

                case "横線":
                    {
                        Event.ObjFillStyle = FillStyle.VbHorizontalLine;
                        break;
                    }

                case "縦線":
                    {
                        Event.ObjFillStyle = FillStyle.VbVerticalLine;
                        break;
                    }

                case "斜線":
                    {
                        Event.ObjFillStyle = FillStyle.VbUpwardDiagonal;
                        break;
                    }

                case "斜線２":
                    {
                        Event.ObjFillStyle = FillStyle.VbDownwardDiagonal;
                        break;
                    }

                case "クロス":
                    {
                        Event.ObjFillStyle = FillStyle.VbCross;
                        break;
                    }

                case "網かけ":
                    {
                        Event.ObjFillStyle = FillStyle.VbDiagonalCross;
                        break;
                    }

                default:
                    throw new EventErrorException(this, "背景描画方法の指定が不正です");
            }

            return EventData.NextID;
        }
    }
}
