using SRCCore.Events;
using SRCCore.Exceptions;
using System.Collections.Generic;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class PolygonCmd : CmdData
    {
        public PolygonCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PolygonCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var points = new List<Point>();
            var pnum = 1;
            while (2 * pnum < ArgNum)
            {
                var xx = (GetArgAsLong((2 * pnum)) + Event.BaseX);
                var yy = (GetArgAsLong((2 * pnum + 1)) + Event.BaseY);
                points.Add(new Point(xx, yy));
                pnum++;
            }

            if (pnum <= 1)
            {
                throw new EventErrorException(this, "頂点数が少なすぎます");
            }

            GUI.SaveScreen();
            SRC.GUIScrean.PolygonCmd(new ScreanDrawOption(Event, Event.ObjColor), points.ToArray());

            return EventData.NextID;
        }
    }
}
