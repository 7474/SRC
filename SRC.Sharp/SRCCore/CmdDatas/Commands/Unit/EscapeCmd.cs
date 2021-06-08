using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class EscapeCmd : CmdData
    {
        public EscapeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EscapeCmd, eventData)
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

            var pname = "";
            var uparty = "";
            var ucount = 0;
            switch (num)
            {
                case 2:
                    {
                        pname = GetArgAsString(2);
                        if (pname == "味方" || pname == "ＮＰＣ" || pname == "敵" || pname == "中立")
                        {
                            uparty = pname;
                            foreach (Unit u in SRC.UList.Items)
                            {
                                if ((u.Party0 ?? "") == (uparty ?? ""))
                                {
                                    if (u.Status == "出撃")
                                    {
                                        u.Escape(opt);
                                        ucount = (ucount + 1);
                                    }
                                    else if (u.Status == "破壊")
                                    {
                                        if (1 <= u.x && u.x <= Map.MapWidth && 1 <= u.y && u.y <= Map.MapHeight)
                                        {
                                            if (ReferenceEquals(u, Map.MapDataForUnit[u.x, u.y]))
                                            {
                                                // 破壊キャンセルで画面上に残っていた
                                                u.Escape(opt);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var u = SRC.UList.Item2(pname);
                            if (u is null)
                            {
                                if (!SRC.PList.IsDefined(pname))
                                {
                                    throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                                }
                                u = SRC.PList.Item(pname).Unit;
                            }

                            if (u is object)
                            {
                                if (u.Status == "出撃")
                                {
                                    ucount = 1;
                                }

                                u.Escape(opt);
                                uparty = u.Party0;
                            }
                        }

                        break;
                    }

                case 1:
                    {
                        {
                            var withBlock2 = Event.SelectedUnitForEvent;
                            if (withBlock2.Status == "出撃")
                            {
                                ucount = 1;
                            }

                            withBlock2.Escape(opt);
                            uparty = withBlock2.Party0;
                        }

                        break;
                    }

                default:
                    {
                        throw new EventErrorException(this, "Escapeコマンドの引数の数が違います");
                    }
            }

            // Escapeコマンドによって全滅したかを判定
            if (uparty != "ＮＰＣ" && uparty != "味方" && ucount > 0)
            {
                foreach (Unit u in SRC.UList.Items)
                {
                    if ((u.Party0 ?? "") == (uparty ?? "") && (u.Status == "出撃" || u.Status == "格納") && !u.IsConditionSatisfied("憑依"))
                    {
                        return EventData.NextID;
                    }
                }

                // 戦闘時以外のイベント中の撤退は無視
                foreach(var m in Event.EventQue)
                {
                    if (m == "プロローグ" || m == "エピローグ" || m == "スタート" || GeneralLib.LIndex(m, 1) == "マップ攻撃破壊")
                    {
                        return EventData.NextID;
                    }
                }

                // 後で全滅イベントを実行
                Event.RegisterEvent("全滅", uparty);
            }

            return EventData.NextID;
        }
    }
}
