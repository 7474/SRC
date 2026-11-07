using SRCCore.Maps;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Events
{
    public partial class Event
    {
        private string key_type = "";
        // インターミッションコマンド「ユニットリスト」におけるユニットリストを作成する
        public void MakeUnitList(string smode = "")
        {
            // リストのソート項目を設定
            if (!string.IsNullOrEmpty(smode))
            {
                key_type = smode;
            }

            if (string.IsNullOrEmpty(key_type))
            {
                key_type = "ＨＰ";
            }

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // あらかじめ撤退させておく
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃")
                {
                    u.Escape();
                }
            }

            // マップをクリア
            Map.LoadMapData("");
            GUI.SetupBackground("", "ステータス");

            // ユニット一覧を作成
            var unit_list = new List<Unit>();
            var key_list = new List<int>();
            var strkey_list = new List<string>();
            foreach (var u in SRC.UList.Items.Where(u => u.Status == "出撃" || u.Status == "待機"))
            {
                unit_list.Add(u);
                if (key_type != "名称")
                {
                    key_list.Add(SortKeyOf(u));
                }
                else
                {
                    // 読み仮名は「名称」のときだけ見る（等身大基準ならメインパイロットのもの）
                    strkey_list.Add(Expression.IsOptionDefined("等身大基準")
                        ? u.MainPilot().KanaName
                        : u.KanaName);
                }
            }

            // ソート（VB と同じ選択ソート。同じ値の並びも VB と同じになる）
            if (key_type != "名称")
            {
                // 大きい順。比較は「>」なので、同じ値なら先にあるものを選ぶ
                for (var i = 0; i < key_list.Count - 1; i++)
                {
                    var max_item = i;
                    var max_value = key_list[i];
                    for (var j = i + 1; j < unit_list.Count; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }
                    if (max_item != i)
                    {
                        (unit_list[i], unit_list[max_item]) = (unit_list[max_item], unit_list[i]);
                        (key_list[i], key_list[max_item]) = (key_list[max_item], key_list[i]);
                    }
                }
            }
            else
            {
                // 小さい順。比較は StrComp(…, vbTextCompare) = -1
                var comparer = SrcCollection<string>.KeyComparer;
                for (var i = 0; i < strkey_list.Count - 1; i++)
                {
                    var max_item = i;
                    var max_str = strkey_list[i];
                    for (var j = i + 1; j < strkey_list.Count; j++)
                    {
                        if (comparer.Compare(strkey_list[j], max_str) < 0)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }
                    if (max_item != i)
                    {
                        (unit_list[i], unit_list[max_item]) = (unit_list[max_item], unit_list[i]);
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            // Font Regular 9pt 背景
            GUI.SetDrawString(DrawStringMode.Status);

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;

            // ユニットのリストを作成
            var xx = 1;
            var yy = 1;
            for (var idx = 0; idx < unit_list.Count; idx++)
            {
                var u = unit_list[idx];
                var key = key_type != "名称" ? key_list[idx] : 0;
                // ユニット出撃位置を折り返す
                if (xx > 15)
                {
                    xx = 1;
                    yy = (short)(yy + 1);
                    if (yy > 40)
                    {
                        // ユニット数が多すぎるため、一部のパイロットが表示出来ません
                        break;
                    }
                }

                // パイロットが乗っていない場合はダミーパイロットを乗せる
                if (u.CountPilot() == 0)
                {
                    var p = SRC.PList.Add("ステータス表示用ダミーパイロット(ザコ)", 1, "味方", gid: "");
                    p.Ride(u);
                }

                // 出撃
                u.UsedAction = 0;
                u.StandBy(xx, yy);

                // プレイヤーが操作できないように
                u.AddCondition("非操作", -1, cdata: "");

                // ユニットの愛称を表示
                GUI.DrawString((string)u.Nickname, 32 * xx + 2, 32 * yy - 31);

                // ソート項目にあわせてユニットのステータスを表示
                switch (key_type ?? "")
                {
                    case "ランク":
                        {
                            GUI.DrawString("RK" + SrcFormatter.Format(key) + " " + Expression.Term("HP", u) + SrcFormatter.Format(u.HP) + " " + Expression.Term("EN", u) + SrcFormatter.Format(u.EN), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "ＨＰ":
                    case "ＥＮ":
                    case "名称":
                        {
                            GUI.DrawString(Expression.Term("HP", u) + SrcFormatter.Format(u.HP) + " " + Expression.Term("EN", u) + SrcFormatter.Format(u.EN), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "装甲":
                        {
                            GUI.DrawString(Expression.Term("装甲", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "運動性":
                        {
                            GUI.DrawString(Expression.Term("運動性", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "移動力":
                        {
                            GUI.DrawString(Expression.Term("移動力", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "最大攻撃力":
                        {
                            GUI.DrawString("攻撃力" + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "最長射程":
                        {
                            GUI.DrawString("射程" + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "レベル":
                        {
                            GUI.DrawString("Lv" + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "ＳＰ":
                        {
                            GUI.DrawString(Expression.Term("SP", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "格闘":
                        {
                            GUI.DrawString(Expression.Term("格闘", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "射撃":
                        {
                            if (u.MainPilot().HasMana())
                            {
                                GUI.DrawString(Expression.Term("魔力", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            }
                            else
                            {
                                GUI.DrawString(Expression.Term("射撃", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            }

                            break;
                        }

                    case "命中":
                        {
                            GUI.DrawString(Expression.Term("命中", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "回避":
                        {
                            GUI.DrawString(Expression.Term("回避", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "技量":
                        {
                            GUI.DrawString(Expression.Term("技量", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "反応":
                        {
                            GUI.DrawString(Expression.Term("反応", u) + SrcFormatter.Format(key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }
                }

                // 表示位置を右に5マスずらす
                xx = xx + 5;
            }

            // フォントの設定を戻しておく
            GUI.ResetDrawString();

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
        }

        // ソートする項目にあわせてソートの際の優先度を決定
        private int SortKeyOf(Unit u)
        {
            switch (key_type ?? "")
            {
                case "ランク":
                    return u.Rank;
                case "ＨＰ":
                    return u.HP;
                case "ＥＮ":
                    return u.EN;
                case "装甲":
                    return u.get_Armor("");
                case "運動性":
                    return u.get_Mobility("");
                case "移動力":
                    return u.Speed;
                case "最大攻撃力":
                    return u.Weapons.Where(w => w.IsWeaponMastered()
                            && !u.IsDisabled(w.Name)
                            && !w.IsWeaponClassifiedAs("合"))
                        .Select(w => w.WeaponPower(""))
                        .Append(0)
                        .Max();
                case "最長射程":
                    return u.Weapons.Where(w => w.IsWeaponMastered()
                            && !u.IsDisabled(w.Name)
                            && !w.IsWeaponClassifiedAs("合"))
                        .Select(w => w.WeaponMaxRange())
                        .Append(0)
                        .Max();
                case "レベル":
                    return u.MainPilot().Level;
                case "ＳＰ":
                    return u.MainPilot().MaxSP;
                case "格闘":
                    return u.MainPilot().Infight;
                case "射撃":
                    return u.MainPilot().Shooting;
                case "命中":
                    return u.MainPilot().Hit;
                case "回避":
                    return u.MainPilot().Dodge;
                case "技量":
                    return u.MainPilot().Technique;
                case "反応":
                    return u.MainPilot().Intuition;
                default:
                    return 0;
            }
        }
    }
}
