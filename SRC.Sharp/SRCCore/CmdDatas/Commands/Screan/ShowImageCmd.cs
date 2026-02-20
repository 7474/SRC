// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ShowImageCmd : CmdData
    {
        public ShowImageCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ShowImageCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 2)
            {
                throw new EventErrorException(this, "ShowImageコマンドの引数の数が違います");
            }

            var fname = GetArgAsString(2);
            switch (Strings.Right(Strings.LCase(fname), 4) ?? "")
            {
                case ".bmp":
                case ".jpg":
                case ".gif":
                case ".png":
                    break;

                default:
                    throw new EventErrorException(this, "不正な画像ファイル名「" + fname + "」が指定されています");
            }

            int dw, dh;
            if (ArgNum >= 4)
            {
                dw = GetArgAsLong(3);
                dh = GetArgAsLong(4);
            }
            else
            {
                dw = Constants.DEFAULT_LEVEL;
                dh = Constants.DEFAULT_LEVEL;
            }

            GUI.DrawPicture(fname, Constants.DEFAULT_LEVEL, Constants.DEFAULT_LEVEL, dw, dh, 0, 0, 0, 0, "");
            return EventData.NextID;
        }
    }
}
