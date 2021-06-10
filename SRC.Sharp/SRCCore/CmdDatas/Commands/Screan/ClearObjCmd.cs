using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearObjCmd : CmdData
    {
        public ClearObjCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearObjCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var without_refresh = false;
            var n = ArgNum;
            if (n > 1)
            {
                if (GetArgAsString(n) == "非同期")
                {
                    n = (n - 1);
                    without_refresh = true;
                }
            }

            switch (n)
            {
                case 2:
                    {
                        var oname = GetArgAsString(2);
                        Event.HotPointList.RemoveItem(x => x.Name == oname);
                        break;
                    }

                case 1:
                    {
                        Event.HotPointList.Clear();
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ClearObjコマンドの引数の数が違います");
            }

            GUI.UpdateHotPoint();
            return EventData.NextID;
        }
    }
}
