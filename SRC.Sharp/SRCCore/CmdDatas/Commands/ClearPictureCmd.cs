using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearPictureCmd : CmdData
    {
        public ClearPictureCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearPictureCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            switch (ArgNum)
            {
                case 1:
                    GUI.ClearPicture();
                    break;

                case 5:
                    GUI.ClearPicture2(
                        GetArgAsLong(2) + Event.BaseX,
                        GetArgAsLong(3) + Event.BaseY,
                        GetArgAsLong(4) + Event.BaseX,
                        GetArgAsLong(5) + Event.BaseY);
                    break;

                default:
                    throw new EventErrorException(this, "ClearPictureコマンドの引数の数が違います");
            }

            return EventData.ID + 1;
        }
    }
}
