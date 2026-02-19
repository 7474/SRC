using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SetWindowColorCmd : CmdData
    {
        public SetWindowColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetWindowColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2 && ArgNum != 3)
            {
                throw new EventErrorException(this, "SetWindowColorコマンドの引数の数が違います");
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

            bool isTargetLine = false;
            bool isTargetBG = false;
            if (ArgNum == 3)
            {
                var target = GetArgAsString(3);
                if (target == "枠")
                {
                    isTargetLine = true;
                }
                else if (target == "背景")
                {
                    isTargetBG = true;
                }
                else
                {
                    throw new EventErrorException(this, "色設定対象の指定が不正です");
                }
            }

            if (isTargetLine || !isTargetBG)
            {
                var vname = "StatusWindow(FrameColor)";
                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }
                Expression.SetVariableAsLong(vname, color);
            }

            if (isTargetBG || !isTargetLine)
            {
                var vname = "StatusWindow(BackBolor)";
                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }
                Expression.SetVariableAsLong(vname, color);
            }

            return EventData.NextID;
        }
    }
}
