using SRCCore.Events;
using SRCCore.Exceptions;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class RemovePilotCmd : CmdData
    {
        public RemovePilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemovePilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var opt = "";
            var num = ArgNum;
            if (num > 1)
            {
                if (GetArgAsString(num) == "非同期")
                {
                    opt = "非同期";
                    num = (num - 1);
                }
            }

            switch (num)
            {
                case 1:
                    {
                        {
                            var u = Event.SelectedUnitForEvent;
                            if (u.CountPilot() == 0)
                            {
                                throw new EventErrorException(this, "指定されたユニットにパイロットが乗っていません");
                            }

                            if (u.Status == "出撃")
                            {
                                u.Escape(opt);
                            }

                            foreach (var p in u.AllRawPilots)
                            {
                                p.Alive = false;
                            }

                            u.Status = "破棄";
                            foreach (var of in u.OtherForms.Where(x => x.Status == "他形態"))
                            {
                                of.Status = "破棄";
                            }
                        }

                        break;
                    }

                case 2:
                    {
                        var pname = GetArgAsString(2);
                        if (!SRC.PList.IsDefined(pname))
                        {
                            throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                        }

                        var p = SRC.PList.Item(pname);
                        p.Alive = false;
                        if (p.Unit is object)
                        {
                            {
                                var u = p.Unit;
                                if (p.ID == u.MainPilot().ID || p.ID == u.Pilots.First().ID)
                                {
                                    // メインパイロットの場合はパイロット＆サポートを全員削除
                                    // ユニットも削除する
                                    if (u.Status == "出撃" || u.Status == "格納")
                                    {
                                        u.Escape(opt);
                                    }

                                    foreach (var p0 in u.AllRawPilots)
                                    {
                                        p0.Alive = false;
                                    }

                                    u.Status = "破棄";
                                    foreach (var of in u.OtherForms.Where(x => x.Status == "他形態"))
                                    {
                                        of.Status = "破棄";
                                    }
                                }
                                else
                                {
                                    // メインパイロットが対象でなければ指定されたパイロットのみを削除
                                    // XXX いい感じに消す
                                    u.DeletePilot(p);
                                    u.DeleteSupport(p);
                                    p.Unit = null;
                                    return EventData.NextID;
                                }
                            }
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "RemovePilotの引数の数が違います");
            }

            return EventData.NextID;
        }
    }
}
