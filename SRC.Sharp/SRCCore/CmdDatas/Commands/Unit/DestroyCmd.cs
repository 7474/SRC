using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class DestroyCmd : CmdData
    {
        public DestroyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DestroyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string uparty;
            short i;
            switch (ArgNum)
            {
                case 2:
                    {
                        u = GetArgAsUnit(2);
                        break;
                    }

                case 1:
                    {
                        u = Event.SelectedUnitForEvent;
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Destroyコマンドの引数の数が違います");
            }

            // 破壊キャンセル状態にある場合は解除しておく
            if (u.IsConditionSatisfied("破壊キャンセル"))
            {
                u.DeleteCondition("破壊キャンセル");
            }

            switch (u.Status ?? "")
            {
                case "出撃":
                    {
                        u.Die();
                        break;
                    }

                case "格納":
                    {
                        u.Escape();
                        u.Status = "破壊";
                        break;
                    }

                case "破壊":
                    {
                        if (ReferenceEquals(Map.MapDataForUnit[u.x, u.y], u))
                        {
                            u.Die();
                            // 既に破壊イベントが発生しているはずなので、ここで終了
                            return EventData.NextID;
                        }

                        break;
                    }

                default:
                    {
                        u.Status = "破壊";
                        break;
                    }
            }

            // ステータス表示中の場合は表示を解除
            if (ReferenceEquals(u, SRC.GUIStatus.DisplayedUnit))
            {
                SRC.GUIStatus.ClearUnitStatus();
            }

            // Destroyコマンドによって全滅したかを判定
            uparty = u.Party0;
            foreach (Unit currentU in SRC.UList.Items)
            {
                if ((currentU.Party0 ?? "") == (uparty ?? "") && (currentU.Status == "出撃" || currentU.Status == "格納") && !currentU.IsConditionSatisfied("憑依"))
                {
                    return EventData.NextID;
                }
            }

            // 戦闘時以外のイベント中の破壊は無視
            foreach (var m in Event.EventQue)
            {
                if (m == "プロローグ" || m == "エピローグ" || m == "スタート" || m == "全滅")
                {
                    return EventData.NextID;
                }
            }

            // 後で全滅イベントを実行
            Event.RegisterEvent("全滅", uparty);
            return EventData.NextID;
        }
    }
}
