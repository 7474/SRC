using SRCCore.Events;
using SRCCore.Exceptions;

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
                        // 元の SRC と同じく、同じ名前のホットポイントが複数あるときは最初の 1 つだけを消す
                        for (var i = 0; i < Event.HotPointList.Count; i++)
                        {
                            if (Event.HotPointList[i].Name == oname)
                            {
                                Event.HotPointList.RemoveAt(i);
                                break;
                            }
                        }
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
