// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    // 互換性維持のために残している
    public class ClearImageCmd : CmdData
    {
        public ClearImageCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearImageCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            GUI.ClearPicture();
            return EventData.NextID;
        }
    }
}
