using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ItemCmd : CmdData
    {
        public ItemCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ItemCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string iname;
            switch (ArgNum)
            {
                case 2:
                    iname = GetArgAsString(2);
                    break;

                default:
                    throw new EventErrorException(this, "Itemコマンドの引数の数が違います");
            }

            if (!SRC.IDList.IsDefined(iname))
            {
                throw new EventErrorException(this, "「" + iname + "」というアイテムは存在しません");
            }

            SRC.IList.Add(iname);
            return EventData.NextID;
        }
    }
}
