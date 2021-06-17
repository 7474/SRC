using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class UpgradeCmd : CmdData
    {
        public UpgradeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UpgradeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u1;
            string uname;
            switch (ArgNum)
            {
                case 2:
                    {
                        u1 = Event.SelectedUnitForEvent.CurrentForm();
                        uname = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        uname = GetArgAsString(2);
                        if (!SRC.UList.IsDefined(uname))
                        {
                            throw new EventErrorException(this, uname + "というユニットはありません");
                        }

                        u1 = SRC.UList.Item(uname).CurrentForm();
                        uname = GetArgAsString(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Upgradeコマンドの引数の数が違います");
            }

            if (!SRC.UDList.IsDefined(uname))
            {
                throw new EventErrorException(this, "ユニット「" + uname + "」のデータが見つかりません");
            }

            var prev_status = u1.Status;
            var u2 = SRC.UList.Add(uname, u1.Rank, u1.Party0);
            if (u2 is null)
            {
                throw new EventErrorException(this, uname + "のユニットデータが不正です");
            }

            if (u1.BossRank > 0)
            {
                u2.BossRank = u1.BossRank;
                u2.FullRecover();
            }

            // パイロットの乗せ換え
            if (u1.CountPilot() > 0)
            {
                var pilot_list = u1.Pilots.CloneList();
                var support_list = u1.Supports.ToList();

                u1.Pilots.First().GetOff();
                foreach (var p in pilot_list)
                {
                    p.Ride(u2);
                }
                foreach (var p in support_list)
                {
                    p.Ride(u2);
                }
            }

            // アイテムの交換
            foreach (var itm in u1.ItemList)
            {
                u2.AddItem(itm);
            }
            foreach (var itm in u1.ItemList.CloneList())
            {
                u1.DeleteItem(itm);
            }

            // リンクの付け替え
            u2.Master = u1.Master;
            u1.Master = null;
            u2.Summoner = u1.Summoner;
            u1.Summoner = null;

            // 召喚ユニットの交換
            foreach (var u in u1.Servants)
            {
                u2.AddServant(u);
            }
            foreach (var u in u1.Servants.CloneList())
            {
                u1.DeleteServant(u.ID);
            }

            // 収納ユニットの交換
            if (u1.IsFeatureAvailable("母艦"))
            {
                foreach (var of in u1.OtherForms.Where(x => x.Status == "格納"))
                {
                    u2.AddOtherForm(of);
                }
                foreach (var of in u2.OtherForms.Where(x => x.Status == "格納"))
                {
                    u1.DeleteOtherForm(of.ID);
                }
            }

            u2.Area = u1.Area;

            // 元のユニットを削除
            u1.Status = "破棄";
            foreach (var of in u1.OtherForms.Where(x => x.Status == "他形態"))
            {
                of.Status = "破棄";
            }

            u2.UsedAction = u1.UsedAction;
            u2.UsedSupportAttack = u1.UsedSupportAttack;
            u2.UsedSupportGuard = u1.UsedSupportGuard;
            u2.UsedSyncAttack = u1.UsedSyncAttack;
            u2.UsedCounterAttack = u1.UsedCounterAttack;
            switch (prev_status ?? "")
            {
                case "出撃":
                    {
                        Map.MapDataForUnit[u1.x, u1.y] = null;
                        u2.StandBy(u1.x, u1.y);
                        if (!GUI.IsPictureVisible)
                        {
                            GUI.RedrawScreen();
                        }

                        break;
                    }

                case "破壊":
                case "破棄":
                    {
                        if (ReferenceEquals(Map.MapDataForUnit[u1.x, u1.y], u1))
                        {
                            Map.MapDataForUnit[u1.x, u1.y] = null;
                        }

                        u2.StandBy(u1.x, u1.y);
                        if (!GUI.IsPictureVisible)
                        {
                            GUI.RedrawScreen();
                        }

                        break;
                    }

                case "格納":
                    {
                        SRC.UList.SwapOnBardUnit(u1, u2);
                        break;
                    }

                default:
                    {
                        u2.Status = prev_status;
                        break;
                    }
            }

            // グローバル変数の更新
            UpdateSelectedState(u1, u2);
            return EventData.NextID;
        }
    }
}
