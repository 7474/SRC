using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class DestroyCmd : CmdData
    {
        public DestroyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DestroyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            string uparty;
            //            short i;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Destroyコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 215862


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            // 破壊キャンセル状態にある場合は解除しておく
            //            if (u.IsConditionSatisfied("破壊キャンセル"))
            //            {
            //                u.DeleteCondition("破壊キャンセル");
            //            }

            //            switch (u.Status ?? "")
            //            {
            //                case "出撃":
            //                    {
            //                        u.Die();
            //                        break;
            //                    }

            //                case "格納":
            //                    {
            //                        u.Escape();
            //                        u.Status = "破壊";
            //                        break;
            //                    }

            //                case "破壊":
            //                    {
            //                        if (ReferenceEquals(Map.MapDataForUnit[u.x, u.y], u))
            //                        {
            //                            u.Die();
            //                            // 既に破壊イベントが発生しているはずなので、ここで終了
            //                            ExecDestroyCmdRet = LineNum + 1;
            //                            return ExecDestroyCmdRet;
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        u.Status = "破壊";
            //                        break;
            //                    }
            //            }

            //            // ステータス表示中の場合は表示を解除
            //            if (ReferenceEquals(u, Status.DisplayedUnit))
            //            {
            //                Status.ClearUnitStatus();
            //            }

            //            // Destroyコマンドによって全滅したかを判定
            //            uparty = u.Party0;
            //            foreach (Unit currentU in SRC.UList)
            //            {
            //                u = currentU;
            //                if ((u.Party0 ?? "") == (uparty ?? "") && (u.Status == "出撃" || u.Status == "格納") && !u.IsConditionSatisfied("憑依"))
            //                {
            //                    ExecDestroyCmdRet = LineNum + 1;
            //                    return ExecDestroyCmdRet;
            //                }
            //            }

            //            // 戦闘時以外のイベント中の破壊は無視
            //            var loopTo = Information.UBound(Event.EventQue);
            //            for (i = 1; i <= loopTo; i++)
            //            {
            //                if (Event.EventQue[i] == "プロローグ" || Event.EventQue[i] == "エピローグ" || Event.EventQue[i] == "スタート" || Event.EventQue[i] == "全滅")
            //                {
            //                    ExecDestroyCmdRet = LineNum + 1;
            //                    return ExecDestroyCmdRet;
            //                }
            //            }

            //            // 後で全滅イベントを実行
            //            Event.RegisterEvent("全滅", uparty);
            //return EventData.NextID;
        }
    }
}
