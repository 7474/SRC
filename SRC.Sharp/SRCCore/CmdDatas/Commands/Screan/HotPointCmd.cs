using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class HotPointCmd : CmdData
    {
        public HotPointCmd(SRC src, EventDataLine eventData) : base(src, CmdType.HotPointCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string hname, hcaption;
            int hx, hy, hw, hh;
            switch (ArgNum)
            {
                case 6:
                    {
                        hname = GetArgAsString(2);
                        hx = (GetArgAsLong(3) + Event.BaseX);
                        hy = (GetArgAsLong(4) + Event.BaseY);
                        hw = GetArgAsLong(5);
                        hh = GetArgAsLong(6);
                        hcaption = hname;
                        break;
                    }

                case 7:
                    {
                        hname = GetArgAsString(2);
                        hx = (GetArgAsLong(3) + Event.BaseX);
                        hy = (GetArgAsLong(4) + Event.BaseY);
                        hw = GetArgAsLong(5);
                        hh = GetArgAsLong(6);
                        hcaption = GetArgAsString(7);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "HotPointコマンドの引数の数が違います");
            }

            Event.HotPointList.Add(new HotPoint
            {
                Name = hname,
                Left = hx,
                Top = hy,
                Width = hw,
                Height = hh,
                Caption = hcaption,
            });

            return EventData.NextID;
        }
    }
}
