using SRCCore.Maps;
using SRCCore.Units;
using SRCCore.VB;
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
            var target_unit_list = SRC.UList.Items
                .Where(u => u.Status == "出撃" || u.Status == "待機")
                .Select(u =>
                {
                    int key = 0;
                    // ソートする項目にあわせてソートの際の優先度を決定
                    switch (key_type ?? "")
                    {
                        case "ランク":
                            key = u.Rank;
                            break;
                        case "ＨＰ":
                            key = u.HP;
                            break;
                        case "ＥＮ":
                            key = u.EN;
                            break;
                        case "装甲":
                            key = u.get_Armor("");
                            break;
                        case "運動性":
                            key = u.get_Mobility("");
                            break;
                        case "移動力":
                            key = u.Speed;
                            break;
                        case "最大攻撃力":
                            key = u.Weapons.Where(w => w.IsWeaponMastered()
                                    && !u.IsDisabled(w.Name)
                                    && !w.IsWeaponClassifiedAs("合"))
                                .Select(w => w.WeaponPower(""))
                                .Append(0)
                                .Max();
                            break;

                        case "最長射程":
                            key = u.Weapons.Where(w => w.IsWeaponMastered()
                                    && !u.IsDisabled(w.Name)
                                    && !w.IsWeaponClassifiedAs("合"))
                                .Select(w => w.WeaponMaxRange())
                                .Append(0)
                                .Max();
                            break;
                        case "レベル":
                            key = u.MainPilot().Level;
                            break;
                        case "ＳＰ":
                            key = u.MainPilot().MaxSP;
                            break;
                        case "格闘":
                            key = u.MainPilot().Infight;
                            break;
                        case "射撃":
                            key = u.MainPilot().Shooting;
                            break;
                        case "命中":
                            key = u.MainPilot().Hit;
                            break;
                        case "回避":
                            key = u.MainPilot().Dodge;
                            break;
                        case "技量":
                            key = u.MainPilot().Technique;
                            break;
                        case "反応":
                            key = u.MainPilot().Intuition;
                            break;
                    }
                    return new
                    {
                        Unit = u,
                        Key = key,
                        StrKey = Expression.IsOptionDefined("等身大基準")
                            ? u.MainPilot().KanaName
                            : u.KanaName,
                    };
                }).ToList();
            var unit_list = target_unit_list
                .OrderByDescending(x => key_type != "名称" ? x.Key : 0)
                .OrderBy(x => key_type == "名称" ? x.StrKey : "")
                .ToList();

            // TODO フォント
            //// Font Regular 9pt 背景
            //{
            //    var withBlock3 = GUI.MainForm.picMain(0).Font;
            //    withBlock3.Size = 9;
            //    withBlock3.Bold = false;
            //    withBlock3.Italic = false;
            //}

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;

            // ユニットのリストを作成
            var xx = 1;
            var yy = 1;
            foreach (var item in unit_list)
            {
                var u = item.Unit;
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
                var    p = SRC.PList.Add("ステータス表示用ダミーパイロット(ザコ)", 1, "味方", gid: "");
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
                            GUI.DrawString("RK" + SrcFormatter.Format(item.Key) + " " + Expression.Term("HP", u) + SrcFormatter.Format(u.HP) + " " + Expression.Term("EN", u) + SrcFormatter.Format(u.EN), 32 * xx + 2, 32 * yy - 15);
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
                            GUI.DrawString(Expression.Term("装甲", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "運動性":
                        {
                            GUI.DrawString(Expression.Term("運動性", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "移動力":
                        {
                            GUI.DrawString(Expression.Term("移動力", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "最大攻撃力":
                        {
                            GUI.DrawString("攻撃力" + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "最長射程":
                        {
                            GUI.DrawString("射程" + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "レベル":
                        {
                            GUI.DrawString("Lv" + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "ＳＰ":
                        {
                            GUI.DrawString(Expression.Term("SP", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "格闘":
                        {
                            GUI.DrawString(Expression.Term("格闘", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "射撃":
                        {
                            if (u.MainPilot().HasMana())
                            {
                                GUI.DrawString(Expression.Term("魔力", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            }
                            else
                            {
                                GUI.DrawString(Expression.Term("射撃", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            }

                            break;
                        }

                    case "命中":
                        {
                            GUI.DrawString(Expression.Term("命中", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "回避":
                        {
                            GUI.DrawString(Expression.Term("回避", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "技量":
                        {
                            GUI.DrawString(Expression.Term("技量", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "反応":
                        {
                            GUI.DrawString(Expression.Term("反応", u) + SrcFormatter.Format(item.Key), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }
                }

                // 表示位置を右に5マスずらす
                xx = xx + 5;
            }

            //// フォントの設定を戻しておく
            //{
            //    var withBlock5 = GUI.MainForm.picMain(0).Font;
            //    withBlock5.Size = 16;
            //    withBlock5.Bold = true;
            //    withBlock5.Italic = false;
            //}

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
        }
    }
}
