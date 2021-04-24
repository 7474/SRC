using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class RideCmd : CmdData
    {
        public RideCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RideCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string uname;
            var p = GetArgAsPilot(2);
            switch (ArgNum)
            {
                case 3:
                    {
                        uname = GetArgAsString(3);

                        // 指定したユニットに既に乗っている場合は何もしない
                        if (p.Unit is object)
                        {
                            {
                                var withBlock = p.Unit;
                                if ((withBlock.Name ?? "") == (uname ?? "") | (withBlock.ID ?? "") == (uname ?? ""))
                                {
                                    return EventData.NextID;
                                }
                            }
                        }

                        p.GetOff();
                        {
                            var u = SRC.UList.Item(uname);
                            if (u is null)
                            {
                                throw new EventErrorException(this, "ユニット名が間違っています");
                            }

                            // ユニットＩＤで指定された場合
                            if ((u.ID ?? "") == (uname ?? ""))
                            {
                                p.Ride(u.CurrentForm());
                                return EventData.NextID;
                            }
                        }
                        // 大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
                        // 名前をデータのそれとあわせる
                        if (SRC.UDList.IsDefined(uname))
                        {
                            uname = SRC.UDList.Item(uname).Name;
                        }

                        // Rideコマンドで乗せ換えられたユニット＆パイロットの履歴を更新
                        if ((uname ?? "") == (Event.LastUnitName ?? ""))
                        {
                        }
                        else
                        {
                            Event.LastUnitName = uname;
                            Event.LastPilotID.Clear();
                        }

                        Event.LastPilotID.Add(p.ID);

                        // パイロットが足りていないものを優先
                        foreach (Unit u in SRC.UList.Items)
                        {
                            if ((u.Name ?? "") == (uname ?? "") & (u.Party0 ?? "") == (p.Party ?? "") & u.Status != "破棄")
                            {
                                if (p.IsSupport(u) & !u.IsFeatureAvailable("ダミーユニット"))
                                {
                                    p.Ride(u.CurrentForm());
                                    return EventData.NextID;
                                }

                                if (u.CurrentForm().CountPilot() < Math.Abs(u.Data.PilotNum))
                                {
                                    p.Ride(u.CurrentForm());
                                    return EventData.NextID;
                                }
                            }
                        }

                        // 空きがなければ今までRideコマンドで指定されてないユニットに乗り込む
                        foreach (Unit u in SRC.UList.Items)
                        {
                            if ((u.Name ?? "") == (uname ?? "") & (u.Party0 ?? "") == (p.Party ?? "") & u.Status != "破棄")
                            {
                                if ((int)u.CurrentForm().CountPilot() > 0)
                                {
                                    // 今までにRideコマンドで指定されているか判定
                                    if (Event.LastPilotID.Any(x => u.CurrentForm().MainPilot().ID == x))
                                    {
                                        continue;
                                    }

                                    u.CurrentForm().Pilots.First().GetOff(true);
                                }

                                p.Ride(u.CurrentForm());
                                return EventData.NextID;
                            }
                        }

                        // それでも見つからなければ無差別で……
                        foreach (Unit u in SRC.UList.Items)
                        {
                            if ((u.Name ?? "") == (uname ?? "") & (u.Party0 ?? "") == (p.Party ?? "") & u.Status != "破棄")
                            {
                                if (u.CurrentForm().CountPilot() > 0)
                                {
                                    u.CurrentForm().Pilots.First().GetOff(true);
                                }

                                p.Ride(u.CurrentForm());
                                // 乗り込み履歴を初期化
                                Event.LastPilotID.Clear();
                                Event.LastPilotID.Add(p.ID);
                                return EventData.NextID;
                            }
                        }

                        throw new EventErrorException(this, p.Name + "が乗り込むための" + uname + "が存在しません");
                    }

                case 2:
                    {
                        // 指定したユニットに既に乗っている場合は何もしない
                        if (ReferenceEquals(p.Unit, Event.SelectedUnitForEvent))
                        {
                            return EventData.NextID;
                        }

                        {
                            var withBlock2 = Event.SelectedUnitForEvent;
                            if (withBlock2.CountPilot() == Math.Abs(withBlock2.Data.PilotNum) & !p.IsSupport(Event.SelectedUnitForEvent))
                            {
                                withBlock2.Pilots.First().GetOff(true);
                            }
                        }

                        p.GetOff();
                        p.Ride(Event.SelectedUnitForEvent);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Rideコマンドの引数の数が違います");
            }

            return EventData.NextID;
        }
    }
}
