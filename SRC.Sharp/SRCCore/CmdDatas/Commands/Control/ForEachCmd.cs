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
                            var key_type = KeyType(aname);

                            // 配列作成
                            // VB: 生きていて離脱していないパイロット。ユニットのメインパイロットが 1 番目のパイロットと
                            // 違う（メインパイロットが差し替わっている）ときは、そのメインパイロットを除く
                            var pilots = SRC.PList.Items.Where(p => p.Alive && !p.Away)
                                 .Where(p => !(p.Unit != null && p.Unit.CountPilot() > 0 && p == p.Unit.MainPilot() && p != p.Unit.Pilots.First()))
                                 .ToList();

                            // ソート
                            if (key_type == "名称")
                            {
                                Event.ForEachSet = SortAscendingByText(pilots.Select(p => p.ID).ToList(), pilots.Select(p => p.KanaName).ToList());
                            }
                            else
                            {
                                Event.ForEachSet = SortDescending(pilots.Select(p => p.ID).ToList(), pilots.Select(p =>
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
                                   }).ToList());
                            }
                        }
                        else if (GeneralLib.InStrNotNest(aname, "ユニット一覧(") == 1)
                        {
                            var key_type = KeyType(aname);

                            // 配列作成
                            var units = SRC.UList.Items.Where(u => u.Status == "出撃" || u.Status == "格納" || u.Status == "待機").ToList();

                            // ソート
                            if (key_type == "名称")
                            {
                                Event.ForEachSet = SortAscendingByText(units.Select(u => u.ID).ToList(), units.Select(u => u.KanaName).ToList());
                            }
                            else
                            {
                                Event.ForEachSet = SortDescending(units.Select(u => u.ID).ToList(), units.Select(u =>
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
                                }).ToList());
                            }
                        }
                        else
                        {
                            var isVariable = true;
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
                            else
                            {
                                isVariable = false;
                            }

                            // リストに対するForEach
                            // 未定義の名前の場合はリストの形をしている場合のみ評価する（VB版と同様）
                            if (Event.ForEachSet.Count == 0
                                && (isVariable
                                    || (Strings.Left(aname, 1) == "(" && Strings.Right(aname, 1) == ")")
                                    || (Strings.Left(aname, 1) == "\"" && Strings.Right(aname, 1) == "\"")
                                    || (Strings.Left(aname, 1) == "`" && Strings.Right(aname, 1) == "`")
                                    || (Strings.InStr(Strings.LCase(aname), "list(") == 1 && Strings.Right(aname, 1) == ")")))
                            {
                                // VB: ReDim ForEachSet(ListLength(buf))、ForEachSet(i) = ListIndex(buf, i)。
                                // 括弧・クォートの対応が取れていない（ListLength が -1）と ReDim ForEachSet(-1) が実行時エラーになり、
                                // Exec の ErrorHandler が「イベントデータが不正です」を出してイベントを止める
                                var buf = Expression.GetValueAsString(aname);
                                var num = GeneralLib.ListLength(buf);
                                if (num < 0)
                                {
                                    throw new EventErrorException(this, "イベントデータが不正です");
                                }
                                for (var i = 1; i <= num; i++)
                                {
                                    Event.ForEachSet.Add(GeneralLib.ListIndex(buf, i));
                                }
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

        // VB: key_type = Mid$(aname, InStrNotNest(aname, "(") + 1, Len(aname) - InStrNotNest(aname, "(") - 1)
        // （最初の括弧の次から、最後の 1 文字の前まで）を式として評価する
        private string KeyType(string aname)
        {
            var pos = GeneralLib.InStrNotNest(aname, "(");
            return Expression.GetValueAsString(Strings.Mid(aname, pos + 1, Strings.Len(aname) - pos - 1));
        }

        // VB の選択ソートと同じ順に並べる（降順）。
        // 最大の要素（同じ値なら前のもの）を先頭と入れ替えるので、同じ値の要素の順は保たれないことがある
        private static IList<string> SortDescending(IList<string> ids, IList<int> keys)
        {
            for (var i = 0; i < ids.Count - 1; i++)
            {
                var max_item = i;
                var max_value = keys[i];
                for (var j = i + 1; j < ids.Count; j++)
                {
                    if (keys[j] > max_value)
                    {
                        max_item = j;
                        max_value = keys[j];
                    }
                }
                if (max_item != i)
                {
                    (ids[i], ids[max_item]) = (ids[max_item], ids[i]);
                    keys[max_item] = keys[i];
                }
            }
            return ids;
        }

        // VB: StrComp(strkey_list(j), max_str, vbTextCompare) = -1 で最小のものを選ぶ選択ソート（昇順）。
        // 文字列の比較は、日本語版の Windows の VB と同じく大文字・小文字、全角・半角、ひらがな・カタカナを区別しない
        private static IList<string> SortAscendingByText(IList<string> ids, IList<string> keys)
        {
            var comparer = SrcCollection<string>.KeyComparer;
            for (var i = 0; i < ids.Count - 1; i++)
            {
                var max_item = i;
                var max_str = keys[i];
                for (var j = i + 1; j < ids.Count; j++)
                {
                    if (comparer.Compare(keys[j] ?? "", max_str ?? "") < 0)
                    {
                        max_item = j;
                        max_str = keys[j];
                    }
                }
                if (max_item != i)
                {
                    (ids[i], ids[max_item]) = (ids[max_item], ids[i]);
                    keys[max_item] = keys[i];
                }
            }
            return ids;
        }
    }
}
