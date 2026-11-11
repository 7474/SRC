using SRCCore.Events;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Commands
{
    public class MakePilotListCmd : CmdData
    {
        public MakePilotListCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MakePilotListCmd, eventData)
        {
        }

        /// <summary>
        /// VB の StrComp(…, vbTextCompare) に当たる比較。日本語版の Windows では
        /// 大文字・小文字、全角・半角、ひらがな・カタカナを区別しない。
        /// </summary>
        private static readonly StringComparer TextCompare = SrcCollection<Pilot>.KeyComparer;

        protected override int ExecInternal()
        {
            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // パイロットがどのユニットに乗っていたか記録しておく
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃")
                {
                    // あらかじめ撤退させておく
                    u.Escape("非同期");
                }

                if (u.Status == "待機")
                {
                    if (Strings.InStr(u.Name, "ステータス表示用") == 0)
                    {
                        foreach (var p in u.AllRawPilots)
                        {
                            Expression.SetVariableAsString("搭乗ユニット[" + p.ID + "]", u.ID);
                        }
                    }
                }
            }

            // マップをクリア
            Map.LoadMapData("");
            GUI.SetupBackground("", "ステータス");

            // ユニット一覧を作成
            var pilot_list = new List<Pilot>();
            // 能力値は並べ替えたときの値（元のユニットに乗っていたときの値）を表示する
            var key_list = new List<int>();
            var key_type = GetArgAsString(2);
            if (key_type != "名称")
            {
                // 配列作成
                foreach (var p in SRC.PList.Items)
                {
                    if (!p.Alive || p.Away)
                    {
                        continue;
                    }

                    if (p.Unit != null)
                    {
                        // 追加パイロットは勘定に入れない
                        if (p.IsAdditionalPilot)
                        {
                            continue;
                        }
                        // 追加サポートは勘定に入れない
                        if (p.IsAdditionalSupport)
                        {
                            continue;
                        }
                    }

                    pilot_list.Add(p);
                    // ソート項目が不明な場合はキーが全て0となり、登録順のままとなる
                    switch (key_type)
                    {
                        case "レベル":
                            key_list.Add(p.Level);
                            break;
                        case "ＳＰ":
                            key_list.Add(p.MaxSP);
                            break;
                        case "格闘":
                            key_list.Add(p.Infight);
                            break;
                        case "射撃":
                            key_list.Add(p.Shooting);
                            break;
                        case "命中":
                            key_list.Add(p.Hit);
                            break;
                        case "回避":
                            key_list.Add(p.Dodge);
                            break;
                        case "技量":
                            key_list.Add(p.Technique);
                            break;
                        case "反応":
                            key_list.Add(p.Intuition);
                            break;
                        default:
                            key_list.Add(0);
                            break;
                    }
                }

                // ソート（VB と同じ選択ソート。同じ値の並びも VB に合わせる）
                for (var i = 0; i < pilot_list.Count - 1; i++)
                {
                    var max_item = i;
                    var max_value = key_list[i];
                    for (var j = i + 1; j < pilot_list.Count; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }
                    if (max_item != i)
                    {
                        (pilot_list[i], pilot_list[max_item]) = (pilot_list[max_item], pilot_list[i]);
                        key_list[max_item] = key_list[i];
                        key_list[i] = max_value;
                    }
                }
            }
            else
            {
                // 配列作成
                var strkey_list = new List<string>();
                foreach (var p in SRC.PList.Items)
                {
                    if (!p.Alive || p.Away)
                    {
                        continue;
                    }

                    if (p.Unit != null)
                    {
                        // 追加パイロットは勘定に入れない
                        if (p.Name == p.Unit.FeatureData("追加パイロット"))
                        {
                            continue;
                        }
                    }

                    pilot_list.Add(p);
                    strkey_list.Add(p.KanaName);
                }

                // ソート（VB と同じ選択ソート。比較は StrComp の vbTextCompare）
                for (var i = 0; i < pilot_list.Count - 1; i++)
                {
                    var max_item = i;
                    var max_str = strkey_list[max_item];
                    for (var j = i + 1; j < pilot_list.Count; j++)
                    {
                        if (TextCompare.Compare(strkey_list[j], max_str) < 0)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }
                    if (max_item != i)
                    {
                        (pilot_list[i], pilot_list[max_item]) = (pilot_list[max_item], pilot_list[i]);
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            // Font Regular 9pt 背景
            GUI.SetDrawString(DrawStringMode.Status);

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;
            var xx = 1;
            var yy = 1;
            for (var i = 0; i < pilot_list.Count; i++)
            {
                var p = pilot_list[i];
                // ユニット出撃位置を折り返す
                if (xx > 15)
                {
                    xx = 1;
                    yy = (yy + 1);
                    if (yy > 40)
                    {
                        // パイロット数が多すぎるため、一部のパイロットが表示出来ません
                        break;
                    }
                }

                // ダミーユニットに載せる
                Unit u;
                if (p.Unit is null)
                {
                    if (SRC.UDList.IsDefined(p.Name + "ステータス表示用ユニット"))
                    {
                        u = SRC.UList.Add(p.Name + "ステータス表示用ユニット", 0, "味方");
                    }
                    else
                    {
                        u = SRC.UList.Add("ステータス表示用ダミーユニット", 0, "味方");
                    }

                    p.Ride(u);
                }
                else if (!p.Unit.IsFeatureAvailable("ダミーユニット"))
                {
                    p.GetOff();
                    if (SRC.UDList.IsDefined(p.Name + "ステータス表示用ユニット"))
                    {
                        u = SRC.UList.Add(p.Name + "ステータス表示用ユニット", 0, "味方");
                    }
                    else
                    {
                        u = SRC.UList.Add("ステータス表示用ダミーユニット", 0, "味方");
                    }

                    p.Ride(u);
                }
                else
                {
                    u = p.Unit;
                }

                // 出撃
                u.UsedAction = 0;
                u.StandBy(xx, yy, "非同期");

                // プレイヤーが操作できないように
                u.AddCondition("非操作", -1, cdata: "");

                // パイロットの愛称を表示
                GUI.DrawString(p.get_Nickname(false), 32 * xx + 2, 32 * yy - 31);
                switch (key_type ?? "")
                {
                    case "レベル":
                    case "名称":
                        {
                            GUI.DrawString("Lv" + SrcFormatter.Format(p.Level), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "ＳＰ":
                        {
                            GUI.DrawString(Expression.Term("SP", u) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "格闘":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("格闘", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "射撃":
                        {
                            if (p.HasMana())
                            {
                                GUI.DrawString(Strings.Left(Expression.Term("魔力", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            }
                            else
                            {
                                GUI.DrawString(Strings.Left(Expression.Term("射撃", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            }

                            break;
                        }

                    case "命中":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("命中", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "回避":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("回避", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "技量":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("技量", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "反応":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("反応", u), 1) + SrcFormatter.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }
                }

                // 表示位置を右に3マスずらす
                xx = (xx + 3);
            }

            // フォントの設定を戻しておく
            GUI.ResetDrawString();

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
            return EventData.NextID;
        }
    }
}
