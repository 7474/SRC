using SRCCore.Events;
using SRCCore.Exceptions;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、アイテム操作
    public class ReleaseCmd : CmdData
    {
        public ReleaseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReleaseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string buf;
            switch (ArgNum)
            {
                case 1:
                    {
                        buf = Event.SelectedUnitForEvent.Pilots.First().Name;
                        break;
                    }

                case 2:
                    {
                        buf = GetArgAsString(2);
                        if (!SRC.PList.IsDefined(buf) && !SRC.IList.IsDefined(buf))
                        {
                            throw new EventErrorException(this, "パイロット名またはアイテム名" + buf + "が間違っています");
                        }

                        if (SRC.PList.IsDefined(buf))
                        {
                            buf = SRC.PList.Item(buf).Name;
                        }
                        else
                        {
                            buf = SRC.IList.Item(buf).Name;
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "Releaseコマンドの引数の数が違います");
            }

            buf = "Fix(" + buf + ")";
            if (Expression.IsGlobalVariableDefined(buf))
            {
                Event.GlobalVariableList.Remove(buf);
            }

            return EventData.NextID;
        }
    }
}
