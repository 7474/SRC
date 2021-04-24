using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class MakePilotListCmd : CmdData
    {
        public MakePilotListCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MakePilotListCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            Pilot p;
            short xx, yy;
            string key_type;
            var key_list = default(int[]);
            string[] strkey_list;
            short max_item;
            int max_value;
            string max_str;
            Pilot[] pilot_list;
            short i, j;
            string buf;

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // パイロットがどのユニットに乗っていたか記録しておく
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                {
                    var withBlock = u;
                    if (withBlock.Status_Renamed == "出撃")
                    {
                        // あらかじめ撤退させておく
                        withBlock.Escape("非同期");
                    }

                    if (withBlock.Status_Renamed == "待機")
                    {
                        if (Strings.InStr(withBlock.Name, "ステータス表示用") == 0)
                        {
                            var loopTo = withBlock.CountPilot();
                            for (i = 1; i <= loopTo; i++)
                            {
                                Pilot localPilot() { object argIndex1 = i; var ret = withBlock.Pilot(argIndex1); return ret; }

                                Expression.SetVariableAsString("搭乗ユニット[" + localPilot().ID + "]", withBlock.ID);
                            }

                            var loopTo1 = withBlock.CountSupport();
                            for (i = 1; i <= loopTo1; i++)
                            {
                                Pilot localSupport() { object argIndex1 = i; var ret = withBlock.Support(argIndex1); return ret; }

                                Expression.SetVariableAsString("搭乗ユニット[" + localSupport().ID + "]", withBlock.ID);
                            }
                        }
                    }
                }
            }

            // マップをクリア
            Map.LoadMapData("");
            GUI.SetupBackground("", "ステータス", filter_color: 0, filter_trans_par: 0d);

            // ユニット一覧を作成
            key_type = GetArgAsString(2);
            if (key_type != "名称")
            {
                // 配列作成
                pilot_list = new Pilot[(SRC.PList.Count() + 1)];
                key_list = new int[(SRC.PList.Count() + 1)];
                i = 0;
                foreach (Pilot currentP in SRC.PList)
                {
                    p = currentP;
                    {
                        var withBlock1 = p;
                        if (!withBlock1.Alive || withBlock1.Away)
                        {
                            goto NextPilot1;
                        }

                        if (withBlock1.Unit_Renamed is object)
                        {
                            if (withBlock1.IsAdditionalPilot)
                            {
                                // 追加パイロットは勘定に入れない
                                goto NextPilot1;
                            }

                            if (withBlock1.IsAdditionalSupport)
                            {
                                // 追加サポートは勘定に入れない
                                goto NextPilot1;
                            }
                        }

                        i = (i + 1);
                        pilot_list[i] = p;
                        switch (key_type ?? "")
                        {
                            case "レベル":
                                {
                                    key_list[i] = withBlock1.Level;
                                    break;
                                }

                            case "ＳＰ":
                                {
                                    key_list[i] = withBlock1.MaxSP;
                                    break;
                                }

                            case "格闘":
                                {
                                    key_list[i] = withBlock1.Infight;
                                    break;
                                }

                            case "射撃":
                                {
                                    key_list[i] = withBlock1.Shooting;
                                    break;
                                }

                            case "命中":
                                {
                                    key_list[i] = withBlock1.Hit;
                                    break;
                                }

                            case "回避":
                                {
                                    key_list[i] = withBlock1.Dodge;
                                    break;
                                }

                            case "技量":
                                {
                                    key_list[i] = withBlock1.Technique;
                                    break;
                                }

                            case "反応":
                                {
                                    key_list[i] = withBlock1.Intuition;
                                    break;
                                }
                        }
                    }

                NextPilot1:
                    ;
                }

                Array.Resize(pilot_list, i + 1);
                Array.Resize(key_list, i + 1);

                // ソート
                var loopTo2 = (Information.UBound(pilot_list) - 1);
                for (i = 1; i <= loopTo2; i++)
                {
                    max_item = i;
                    max_value = key_list[i];
                    var loopTo3 = Information.UBound(pilot_list);
                    for (j = (i + 1); j <= loopTo3; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        p = pilot_list[i];
                        pilot_list[i] = pilot_list[max_item];
                        pilot_list[max_item] = p;
                        max_value = key_list[max_item];
                        key_list[max_item] = key_list[i];
                        key_list[i] = max_value;
                    }
                }
            }
            else
            {
                // 配列作成
                pilot_list = new Pilot[(SRC.PList.Count() + 1)];
                strkey_list = new string[(SRC.PList.Count() + 1)];
                i = 0;
                foreach (Pilot currentP1 in SRC.PList)
                {
                    p = currentP1;
                    {
                        var withBlock2 = p;
                        if (!withBlock2.Alive || withBlock2.Away)
                        {
                            goto NextPilot2;
                        }

                        if (withBlock2.Unit_Renamed is object)
                        {
                            if ((withBlock2.Name ?? "") == (withBlock2.Unit_Renamed.FeatureData("追加パイロット") ?? ""))
                            {
                                // 追加パイロットは勘定に入れない
                                goto NextPilot2;
                            }
                        }

                        i = (i + 1);
                        pilot_list[i] = p;
                        strkey_list[i] = p.KanaName;
                    }

                NextPilot2:
                    ;
                }

                Array.Resize(pilot_list, i + 1);
                Array.Resize(strkey_list, i + 1);

                // ソート
                var loopTo4 = (Information.UBound(pilot_list) - 1);
                for (i = 1; i <= loopTo4; i++)
                {
                    max_item = i;
                    max_str = strkey_list[max_item];
                    var loopTo5 = Information.UBound(pilot_list);
                    for (j = (i + 1); j <= loopTo5; j++)
                    {
                        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        p = pilot_list[i];
                        pilot_list[i] = pilot_list[max_item];
                        pilot_list[max_item] = p;
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            // Font Regular 9pt 背景
            {
                var withBlock3 = GUI.MainForm.picMain(0).Font;
                withBlock3.Size = 9;
                withBlock3.Bold = false;
                withBlock3.Italic = false;
            }

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;
            xx = 1;
            yy = 1;
            var loopTo6 = Information.UBound(pilot_list);
            for (i = 1; i <= loopTo6; i++)
            {
                p = pilot_list[i];
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
                if (p.Unit_Renamed is null)
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
                else if (!p.Unit_Renamed.IsFeatureAvailable("ダミーユニット"))
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
                    u = p.Unit_Renamed;
                }

                // 出撃
                u.UsedAction = 0;
                u.StandBy(xx, yy, "非同期");

                // プレイヤーが操作できないように
                u.AddCondition("非操作", -1, cdata: "");

                // パイロットの愛称を表示
                GUI.DrawString(p.get_Nickname(false), 32 * xx + 2, 32 * yy - 31);
                p.get_Nickname(false) = argmsg;
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
            {
                var withBlock4 = GUI.MainForm.picMain(0).Font;
                withBlock4.Size = 16;
                withBlock4.Bold = true;
                withBlock4.Italic = false;
            }

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
            return EventData.NextID;
        }
    }
}
