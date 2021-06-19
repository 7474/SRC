using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class GetOffCmd : CmdData
    {
        public GetOffCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GetOffCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            switch (ArgNum)
            {
                case 1:
                    {
                        u = Event.SelectedUnitForEvent;
                        break;
                    }

                case 2:
                    {
                        u = GetArgAsUnit(2, true);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "GetOffコマンドの引数の数が違います");
            }

            if (u is object)
            {
                if (u.CountPilot() > 0)
                {
                    if (u.Status == "出撃")
                    {
                        // ユニットをマップ上から削除した状態で支援効果を更新
                        Map.MapDataForUnit[u.x, u.y] = null;
                        SRC.PList.UpdateSupportMod(u);
                    }

                    // パイロットを下ろす
                    u.Pilots.First().GetOff(true);
                    if (u.Status == "出撃")
                    {
                        // ユニットをマップ上に戻す
                        Map.MapDataForUnit[u.x, u.y] = u;
                    }
                }
            }

            return EventData.NextID;
        }
    }
}
