using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class EscapeCmd : CmdData
    {
        public EscapeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EscapeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            num = ArgNum;
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
                case 2:
                    {
                        pname = GetArgAsString(2);
                        if (pname == "味方" || pname == "ＮＰＣ" || pname == "敵" || pname == "中立")
                        {
                            uparty = pname;
                            foreach (Unit currentU in SRC.UList)
                            {
                                u = currentU;
                                {
                                    var withBlock = u;
                                    if ((withBlock.Party0 ?? "") == (uparty ?? ""))
                                    {
                                        if (withBlock.Status == "出撃")
                                        {
                                            withBlock.Escape(opt);
                                            ucount = (ucount + 1);
                                        }
                                        else if (withBlock.Status == "破壊")
                                        {
                                            if (1 <= withBlock.x && withBlock.x <= Map.MapWidth && 1 <= withBlock.y && withBlock.y <= Map.MapHeight)
                                            {
                                                if (ReferenceEquals(u, Map.MapDataForUnit[withBlock.x, withBlock.y]))
                                                {
                                                    // 破壊キャンセルで画面上に残っていた
                                                    withBlock.Escape(opt);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            u = SRC.UList.Item2((object)pname);
                            if (u is null)
                            {
                                {
                                    var withBlock1 = SRC.PList;
                                    bool localIsDefined() { object argIndex1 = (object)pname; var ret = withBlock1.IsDefined(argIndex1); return ret; }

                                    if (!localIsDefined())
                                    {
                                        Event.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
                                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 228167


                                        Input:
                                                                        Error(0)

                                         */
                                    }

                                    Pilot localItem() { object argIndex1 = (object)pname; var ret = withBlock1.Item(argIndex1); return ret; }

                                    u = localItem().Unit;
                                }
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
                        Event.EventErrorMessage = "Escapeコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 228757


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            // Escapeコマンドによって全滅したかを判定
            if (uparty != "ＮＰＣ" && uparty != "味方" && ucount > 0)
            {
                foreach (Unit currentU1 in SRC.UList)
                {
                    u = currentU1;
                    if ((u.Party0 ?? "") == (uparty ?? "") && (u.Status == "出撃" || u.Status == "格納") && !u.IsConditionSatisfied("憑依"))
                    {
                        ExecEscapeCmdRet = LineNum + 1;
                        return ExecEscapeCmdRet;
                    }
                }

                // 戦闘時以外のイベント中の撤退は無視
                var loopTo = Information.UBound(Event.EventQue);
                for (i = 1; i <= loopTo; i++)
                {
                    if (Event.EventQue[i] == "プロローグ" || Event.EventQue[i] == "エピローグ" || Event.EventQue[i] == "スタート" || GeneralLib.LIndex(Event.EventQue[i], 1) == "マップ攻撃破壊")
                    {
                        ExecEscapeCmdRet = LineNum + 1;
                        return ExecEscapeCmdRet;
                    }
                }

                // 後で全滅イベントを実行
                Event.RegisterEvent("全滅", uparty);
            }

            return EventData.NextID;
        }
    }
}
