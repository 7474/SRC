using SRCCore.Events;
using System;

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
                Event.EventErrorMessage = "FillStyleコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 246504


                Input:
                            Error(0)

                 */
            }

            switch (GetArgAsString(2) ?? "")
            {
                case "塗りつぶし":
                    {
                        Event.ObjFillStyle = vbFSSolid;
                        break;
                    }

                case "透明":
                    {
                        Event.ObjFillStyle = vbFSTransparent;
                        break;
                    }

                case "横線":
                    {
                        Event.ObjFillStyle = vbHorizontalLine;
                        break;
                    }

                case "縦線":
                    {
                        Event.ObjFillStyle = vbVerticalLine;
                        break;
                    }

                case "斜線":
                    {
                        Event.ObjFillStyle = vbUpwardDiagonal;
                        break;
                    }

                case "斜線２":
                    {
                        Event.ObjFillStyle = vbDownwardDiagonal;
                        break;
                    }

                case "クロス":
                    {
                        Event.ObjFillStyle = vbCross;
                        break;
                    }

                case "網かけ":
                    {
                        Event.ObjFillStyle = vbDiagonalCross;
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "背景描画方法の指定が不正です";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 248728


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            return EventData.NextID;
        }
    }
}
