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
                        //if (GeneralLib.InStrNotNest(aname, "パイロット一覧(") == 1)
                        //{
                        //    string argstring2 = "(";
                        //    string argstring21 = "(";
                        //    key_type = Strings.Mid(aname, GeneralLib.InStrNotNest(aname, argstring2) + 1, Strings.Len(aname) - GeneralLib.InStrNotNest(aname, argstring21) - 1);
                        //    key_type = Expression.GetValueAsString(key_type);
                        //    if (key_type != "名称")
                        //    {
                        //        // 配列作成
                        //        Event.ForEachSet = new string[(SRC.PList.Count() + 1)];
                        //        key_list = new int[(SRC.PList.Count() + 1)];
                        //        i = 0;
                        //        foreach (Pilot currentP in SRC.PList)
                        //        {
                        //            p = currentP;
                        //            if (!p.Alive || p.Away)
                        //            {
                        //                goto NextPilot1;
                        //            }

                        //            if (p.Unit_Renamed is object)
                        //            {
                        //                {
                        //                    var withBlock = p.Unit_Renamed;
                        //                    if (withBlock.CountPilot() > 0)
                        //                    {
                        //                        object argIndex1 = (object)1;
                        //                        object argIndex2 = (object)1;
                        //                        if (ReferenceEquals(p, withBlock.MainPilot()) && !ReferenceEquals(p, withBlock.Pilot(argIndex2)))
                        //                        {
                        //                            goto NextPilot1;
                        //                        }
                        //                    }
                        //                }
                        //            }

                        //            i = i + 1;
                        //            Event.ForEachSet[i] = p.ID;
                        //            switch (key_type ?? "")
                        //            {
                        //                case "レベル":
                        //                    {
                        //                        key_list[i] = 500 * p.Level + p.Exp;
                        //                        break;
                        //                    }

                        //                case "ＳＰ":
                        //                    {
                        //                        key_list[i] = p.MaxSP;
                        //                        break;
                        //                    }

                        //                case "格闘":
                        //                    {
                        //                        key_list[i] = p.Infight;
                        //                        break;
                        //                    }

                        //                case "射撃":
                        //                    {
                        //                        key_list[i] = p.Shooting;
                        //                        break;
                        //                    }

                        //                case "命中":
                        //                    {
                        //                        key_list[i] = p.Hit;
                        //                        break;
                        //                    }

                        //                case "回避":
                        //                    {
                        //                        key_list[i] = p.Dodge;
                        //                        break;
                        //                    }

                        //                case "技量":
                        //                    {
                        //                        key_list[i] = p.Technique;
                        //                        break;
                        //                    }

                        //                case "反応":
                        //                    {
                        //                        key_list[i] = p.Intuition;
                        //                        break;
                        //                    }
                        //            }

                        //        NextPilot1:
                        //            ;
                        //        }

                        //        Array.Resize(Event.ForEachSet, i + 1);
                        //        Array.Resize(key_list, i + 1);

                        //        // ソート
                        //        var loopTo = Information.UBound(Event.ForEachSet) - 1;
                        //        for (i = 1; i <= loopTo; i++)
                        //        {
                        //            max_item = i;
                        //            max_value = key_list[i];
                        //            var loopTo1 = Information.UBound(Event.ForEachSet);
                        //            for (j = (i + 1); j <= loopTo1; j++)
                        //            {
                        //                if (key_list[j] > max_value)
                        //                {
                        //                    max_item = j;
                        //                    max_value = key_list[j];
                        //                }
                        //            }

                        //            if (max_item != i)
                        //            {
                        //                buf = Event.ForEachSet[i];
                        //                Event.ForEachSet[i] = Event.ForEachSet[max_item];
                        //                Event.ForEachSet[max_item] = buf;
                        //                key_list[max_item] = key_list[i];
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // 配列作成
                        //        Event.ForEachSet = new string[(SRC.PList.Count() + 1)];
                        //        strkey_list = new string[(SRC.PList.Count() + 1)];
                        //        i = 0;
                        //        foreach (Pilot currentP1 in SRC.PList)
                        //        {
                        //            p = currentP1;
                        //            if (!p.Alive || p.Away)
                        //            {
                        //                goto NextPilot2;
                        //            }

                        //            if (p.Unit_Renamed is object)
                        //            {
                        //                {
                        //                    var withBlock1 = p.Unit_Renamed;
                        //                    if (withBlock1.CountPilot() > 0)
                        //                    {
                        //                        object argIndex3 = (object)1;
                        //                        object argIndex4 = (object)1;
                        //                        if (ReferenceEquals(p, withBlock1.MainPilot()) && !ReferenceEquals(p, withBlock1.Pilot(argIndex4)))
                        //                        {
                        //                            goto NextPilot2;
                        //                        }
                        //                    }
                        //                }
                        //            }

                        //            i = i + 1;
                        //            Event.ForEachSet[i] = p.ID;
                        //            strkey_list[i] = p.KanaName;
                        //        NextPilot2:
                        //            ;
                        //        }

                        //        Array.Resize(Event.ForEachSet, i + 1);
                        //        Array.Resize(strkey_list, i + 1);

                        //        // ソート
                        //        var loopTo2 = Information.UBound(Event.ForEachSet) - 1;
                        //        for (i = 1; i <= loopTo2; i++)
                        //        {
                        //            max_item = i;
                        //            max_str = strkey_list[i];
                        //            var loopTo3 = Information.UBound(Event.ForEachSet);
                        //            for (j = (i + 1); j <= loopTo3; j++)
                        //            {
                        //                if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        //                {
                        //                    max_item = j;
                        //                    max_str = strkey_list[j];
                        //                }
                        //            }

                        //            if (max_item != i)
                        //            {
                        //                buf = Event.ForEachSet[i];
                        //                Event.ForEachSet[i] = Event.ForEachSet[max_item];
                        //                Event.ForEachSet[max_item] = buf;
                        //                strkey_list[max_item] = strkey_list[i];
                        //            }
                        //        }
                        //    }
                        //}
                        //else if (GeneralLib.InStrNotNest(aname, "ユニット一覧(") == 1)
                        //{
                        //    string argstring22 = "(";
                        //    string argstring23 = "(";
                        //    key_type = Strings.Mid(aname, GeneralLib.InStrNotNest(aname, argstring22) + 1, Strings.Len(aname) - GeneralLib.InStrNotNest(aname, argstring23) - 1);
                        //    key_type = Expression.GetValueAsString(key_type);
                        //    if (key_type != "名称")
                        //    {
                        //        // 配列作成
                        //        Event.ForEachSet = new string[(SRC.UList.Count() + 1)];
                        //        key_list = new int[(SRC.UList.Count() + 1)];
                        //        i = 0;
                        //        foreach (Unit currentU5 in SRC.UList)
                        //        {
                        //            u = currentU5;
                        //            if (u.Status == "出撃" || u.Status == "格納" || u.Status == "待機")
                        //            {
                        //                i = i + 1;
                        //                Event.ForEachSet[i] = u.ID;
                        //                switch (key_type ?? "")
                        //                {
                        //                    case "ランク":
                        //                        {
                        //                            key_list[i] = u.Rank;
                        //                            break;
                        //                        }

                        //                    case "ＨＰ":
                        //                        {
                        //                            key_list[i] = u.HP;
                        //                            break;
                        //                        }

                        //                    case "ＥＮ":
                        //                        {
                        //                            key_list[i] = u.EN;
                        //                            break;
                        //                        }

                        //                    case "装甲":
                        //                        {
                        //                            key_list[i] = u.get_Armor("");
                        //                            break;
                        //                        }

                        //                    case "運動性":
                        //                        {
                        //                            key_list[i] = u.get_Mobility("");
                        //                            break;
                        //                        }

                        //                    case "移動力":
                        //                        {
                        //                            key_list[i] = u.Speed;
                        //                            break;
                        //                        }

                        //                    case "最大攻撃力":
                        //                        {
                        //                            var loopTo4 = u.CountWeapon();
                        //                            for (j = 1; j <= loopTo4; j++)
                        //                            {
                        //                                string argattr = "合";
                        //                                if (u.IsWeaponMastered(j) && !u.IsDisabled(u.Weapon(j).Name) && !u.IsWeaponClassifiedAs(j, argattr))
                        //                                {
                        //                                    string argtarea1 = "";
                        //                                    if (u.WeaponPower(j, argtarea1) > key_list[i])
                        //                                    {
                        //                                        string argtarea = "";
                        //                                        key_list[i] = u.WeaponPower(j, argtarea);
                        //                                    }
                        //                                }
                        //                            }

                        //                            break;
                        //                        }

                        //                    case "最長射程":
                        //                        {
                        //                            var loopTo5 = u.CountWeapon();
                        //                            for (j = 1; j <= loopTo5; j++)
                        //                            {
                        //                                string argattr1 = "合";
                        //                                if (u.IsWeaponMastered(j) && !u.IsDisabled(u.Weapon(j).Name) && !u.IsWeaponClassifiedAs(j, argattr1))
                        //                                {
                        //                                    if (u.WeaponMaxRange(j) > key_list[i])
                        //                                    {
                        //                                        key_list[i] = u.WeaponMaxRange(j);
                        //                                    }
                        //                                }
                        //                            }

                        //                            break;
                        //                        }
                        //                }
                        //            }
                        //        }

                        //        Array.Resize(Event.ForEachSet, i + 1);
                        //        Array.Resize(key_list, i + 1);

                        //        // ソート
                        //        var loopTo6 = Information.UBound(Event.ForEachSet) - 1;
                        //        for (i = 1; i <= loopTo6; i++)
                        //        {
                        //            max_item = i;
                        //            max_value = key_list[i];
                        //            var loopTo7 = Information.UBound(Event.ForEachSet);
                        //            for (j = (i + 1); j <= loopTo7; j++)
                        //            {
                        //                if (key_list[j] > max_value)
                        //                {
                        //                    max_item = j;
                        //                    max_value = key_list[j];
                        //                }
                        //            }

                        //            if (max_item != i)
                        //            {
                        //                buf = Event.ForEachSet[i];
                        //                Event.ForEachSet[i] = Event.ForEachSet[max_item];
                        //                Event.ForEachSet[max_item] = buf;
                        //                key_list[max_item] = key_list[i];
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // 配列作成
                        //        Event.ForEachSet = new string[(SRC.UList.Count() + 1)];
                        //        strkey_list = new string[(SRC.UList.Count() + 1)];
                        //        i = 0;
                        //        foreach (Unit currentU6 in SRC.UList)
                        //        {
                        //            u = currentU6;
                        //            if (u.Status == "出撃" || u.Status == "格納" || u.Status == "待機")
                        //            {
                        //                i = i + 1;
                        //                Event.ForEachSet[i] = u.ID;
                        //                strkey_list[i] = u.KanaName;
                        //            }
                        //        }

                        //        Array.Resize(Event.ForEachSet, i + 1);
                        //        Array.Resize(strkey_list, i + 1);

                        //        // ソート
                        //        var loopTo8 = Information.UBound(Event.ForEachSet) - 1;
                        //        for (i = 1; i <= loopTo8; i++)
                        //        {
                        //            max_item = i;
                        //            max_str = strkey_list[i];
                        //            var loopTo9 = Information.UBound(Event.ForEachSet);
                        //            for (j = (i + 1); j <= loopTo9; j++)
                        //            {
                        //                if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        //                {
                        //                    max_item = j;
                        //                    max_str = strkey_list[j];
                        //                }
                        //            }

                        //            if (max_item != i)
                        //            {
                        //                buf = Event.ForEachSet[i];
                        //                Event.ForEachSet[i] = Event.ForEachSet[max_item];
                        //                Event.ForEachSet[max_item] = buf;
                        //                strkey_list[max_item] = strkey_list[i];
                        //            }
                        //        }
                        //    }
                        //}
                        //else 
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
