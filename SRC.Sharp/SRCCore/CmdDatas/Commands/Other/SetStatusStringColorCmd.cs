using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStatusStringColorCmd : CmdData
    {
        public SetStatusStringColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStatusStringColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "StatusStringColorコマンドの引数の数が違います");
            }

            var opt = GetArgAsString(2);
            if (opt.Length != 7 || opt[0] != '#')
            {
                throw new EventErrorException(this, "色指定が不正です");
            }
            if (!int.TryParse(opt.Substring(1), System.Globalization.NumberStyles.HexNumber, null, out int rgb))
            {
                throw new EventErrorException(this, "色指定が不正です");
            }
            // Convert #RRGGBB to Windows COLORREF (BBGGRR)
            int r = (rgb >> 16) & 0xFF;
            int g = (rgb >> 8) & 0xFF;
            int b = rgb & 0xFF;
            int color = (b << 16) | (g << 8) | r;

            var target = GetArgAsString(3);
            string vname;
            switch (target)
            {
                case "通常":
                    vname = "StatusWindow(StringColor)";
                    break;
                case "能力名":
                    vname = "StatusWindow(ANameColor)";
                    break;
                case "有効":
                    vname = "StatusWindow(EnableColor)";
                    break;
                case "無効":
                    vname = "StatusWindow(DisableColor)";
                    break;
                default:
                    throw new EventErrorException(this, "設定対象の指定が不正です");
            }

            if (!Expression.IsGlobalVariableDefined(vname))
            {
                Expression.DefineGlobalVariable(vname);
            }
            Expression.SetVariableAsLong(vname, color);

            return EventData.NextID;
        }
    }
}
