using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ForEachCmd : CmdData
    {
        public ForEachCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ForEachCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string ustatus;
            string vname;

            Event.ForEachSet = new List<string>();
            switch (ArgNum)
            {
                // ユニットに対するForEach
                case 2:
                case 3:
                    {
                        if (ArgNum == 2)
                        {
                            ustatus = "出撃 格納";
                        }
                        else
                        {
                            ustatus = GetArgAsString(3);
                            if (ustatus == "全て")
                            {
                                ustatus = "全";
                            }
                        }

                        switch (GetArgAsString(2) ?? "")
                        {
                            case "全":
                                if (ustatus == "全")
                                {
                                    foreach (Unit u in SRC.UList.Items)
                                    {
                                        if (u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態" && u.Status != "破棄")
                                        {
                                            Event.ForEachSet.Add(u.ID);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Unit u in SRC.UList.Items)
                                    {
                                        if (Strings.InStr(ustatus, u.Status) > 0)
                                        {
                                            Event.ForEachSet.Add(u.ID);
                                        }
                                    }
                                }
                                break;

                            case "味方":
                            case "ＮＰＣ":
                            case "敵":
                            case "中立":
                                {
                                    var uparty = GetArgAsString(2);
                                    if (ustatus == "全")
                                    {
                                        foreach (Unit u in SRC.UList.Items)
                                        {
                                            if ((u.Party0 ?? "") == (uparty ?? ""))
                                            {
                                                if (u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態" && u.Status != "破棄")
                                                {
                                                    Event.ForEachSet.Add(u.ID);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (Unit u in SRC.UList.Items)
                                        {
                                            if ((u.Party0 ?? "") == (uparty ?? ""))
                                            {
                                                if (Strings.InStr(ustatus, u.Status) > 0)
                                                {
                                                    Event.ForEachSet.Add(u.ID);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }

                            default:
                                var ugroup = GetArgAsString(2);
                                if (ustatus == "全て")
                                {
                                    ustatus = "全";
                                }

                                foreach (Unit u in SRC.UList.Items)
                                {
                                    if (u.CountPilot() > 0)
                                    {
                                        if ((u.MainPilot().ID ?? "") == (ugroup ?? "") || Strings.InStr(u.MainPilot().ID, ugroup + ":") == 1)
                                        {
                                            if (ustatus == "全")
                                            {
                                                if (u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態" && u.Status != "破棄")
                                                {
                                                    Event.ForEachSet.Add(u.ID);
                                                }
                                            }
                                            else if (Strings.InStr(ustatus, u.Status) > 0)
                                            {
                                                Event.ForEachSet.Add(u.ID);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    }

                // 配列の要素に対するForEach
                case 4:
                    {
                        // インデックス用変数名
                        vname = GetArg(2);
                        if (Strings.Left(vname, 1) == "$")
                        {
                            vname = Strings.Mid(vname, 2);
                        }

                        // 配列の変数名
                        var aname = GetArg(4);
                        if (Strings.Left(aname, 1) == "$")
                        {
                            aname = Strings.Mid(aname, 2);
                        }
                        // Eval関数
                        if (Strings.LCase(Strings.Left(aname, 5)) == "eval(")
                        {
                            if (Strings.Right(aname, 1) == ")")
                            {
                                aname = Strings.Mid(aname, 6, Strings.Len(aname) - 6);
                                aname = Expression.GetValueAsString(aname);
                            }
                        }

                        // 配列を検索し、配列要素を見つける
                        if (GeneralLib.InStrNotNest(aname, "パイロット一覧(") == 1)
                        {
                            // 配列作成
                            var pilots = SRC.PList.Items.Where(p => p.Alive && !p.Away)
                                 // XXX メインパイロットが差し変わっていたら除外？
                                 .Where(p => !(p.Unit != null && p.Unit.CountPilot() > 0 && p == p.Unit.MainPilot() && p != p.Unit.Pilots.First()));

                            // ソート
                            var key_type = Expression.GetValueAsString(aname.InsideKakko());
                            if (key_type == "名称")
                            {
                                pilots = pilots.OrderBy(x => x.KanaName);
                            }
                            else
                            {
                                pilots = pilots.OrderByDescending(p =>
                                   {
                                       switch (key_type ?? "")
                                       {
                                           case "レベル": return 500 * p.Level + p.Exp;
                                           case "ＳＰ": return p.MaxSP;
                                           case "格闘": return p.Infight;
                                           case "射撃": return p.Shooting;
                                           case "命中": return p.Hit;
                                           case "回避": return p.Dodge;
                                           case "技量": return p.Technique;
                                           case "反応": return p.Intuition;
                                           default: return 0;
                                       }
                                   });
                            }
                            pilots.ToList().ForEach(p => Event.ForEachSet.Add(p.ID));
                        }
                        else if (GeneralLib.InStrNotNest(aname, "ユニット一覧(") == 1)
                        {
                            // 配列作成
                            var units = SRC.UList.Items.Where(u => u.Status == "出撃" || u.Status == "格納" || u.Status == "待機");

                            var key_type = Expression.GetValueAsString(aname.InsideKakko());
                            if (key_type == "名称")
                            {
                                units = units.OrderBy(x => x.KanaName);
                            }
                            else
                            {
                                units = units.OrderByDescending(u =>
                                {
                                    switch (key_type ?? "")
                                    {
                                        case "ランク": return u.Rank;
                                        case "ＨＰ": return u.HP;
                                        case "ＥＮ": return u.EN;
                                        case "装甲": return u.get_Armor("");
                                        case "運動性": return u.get_Mobility("");
                                        case "移動力": return u.Speed;
                                        case "最大攻撃力":
                                            return u.Weapons
                                                .Where(x => x.IsEnable())
                                                .Where(x => !x.IsWeaponClassifiedAs("合"))
                                                .Select(x => x.WeaponPower(""))
                                                .Append(0)
                                                .Max();

                                        case "最長射程":
                                            return u.Weapons
                                                .Where(x => x.IsEnable())
                                                .Where(x => !x.IsWeaponClassifiedAs("合"))
                                                .Select(x => x.WeaponMaxRange())
                                                .Append(0)
                                                .Max();
                                        default: return 0;
                                    }
                                });
                            }
                            units.ToList().ForEach(u => Event.ForEachSet.Add(u.ID));
                        }
                        else
                        {
                            if (Expression.IsSubLocalVariableDefined(aname))
                            {
                                // サブルーチンローカルな配列に対するForEach
                                Event.SubLocalVars().ArrayIndexesByName(aname).ToList().ForEach(x => Event.ForEachSet.Add(x));
                            }
                            else if (Expression.IsLocalVariableDefined(aname))
                            {
                                // ローカルな配列に対するForEach
                                Event.LocalVariableList.Values.ArrayIndexesByName(aname).ToList().ForEach(x => Event.ForEachSet.Add(x));
                            }
                            else if (Expression.IsGlobalVariableDefined(aname))
                            {
                                // グローバルな配列に対するForEach
                                Event.GlobalVariableList.Values.ArrayIndexesByName(aname).ToList().ForEach(x => Event.ForEachSet.Add(x));
                            }

                            // リストに対するForEach
                            if (Event.ForEachSet.Count == 0)
                            {
                                var buf = Expression.GetValueAsString(aname);
                                Event.ForEachSet = GeneralLib.ToList(buf);
                            }
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "ForEachコマンドの引数の数が違います");
            }

            if (Event.ForEachSet.Count > 0)
            {
                // ForEachの実行要素がある場合
                Event.ForEachIndex = 1;
                Event.ForIndex = (Event.ForIndex + 1);
                if (ArgNum < 4)
                {
                    Event.SelectedUnitForEvent = SRC.UList.Item(Event.ForEachSet[0]);
                }
                else
                {
                    Expression.SetVariableAsString(GetArg(2), Event.ForEachSet[0]);
                }

                return EventData.NextID;
            }
            else
            {
                // ForEachの実行要素がない場合

                // 対応するNextを探す
                var depth = 1;
                foreach (var i in AfterEventIdRange())
                {
                    var cmd = Event.EventCmd[i];
                    switch (cmd.Name)
                    {
                        case CmdType.ForCmd:
                        case CmdType.ForEachCmd:
                            depth = (depth + 1);
                            break;

                        case CmdType.NextCmd:
                            depth = (depth - 1);
                            if (depth == 0)
                            {
                                return cmd.EventData.NextID;
                            }
                            break;
                    }
                }
                throw new EventErrorException(this, "ForまたはForEachとNextが対応していません");
            }
        }
    }
}
