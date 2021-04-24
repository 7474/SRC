using SRCCore.Maps;

namespace SRCCore.Events
{
    public partial class Event
    {
        // インターミッションコマンド「ユニットリスト」におけるユニットリストを作成する
        public static void MakeUnitList([Optional, DefaultParameterValue("")] ref string smode)
        {
            Unit u;
            Pilot p;
            short xx, yy;
            var key_list = default(int[]);
            short max_item;
            int max_value;
            string max_str;
            Unit[] unit_list;
            short i, j;
            ;

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
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;

            // あらかじめ撤退させておく
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                {
                    var withBlock = u;
                    if (withBlock.Status_Renamed == "出撃")
                    {
                        withBlock.Escape();
                    }
                }
            }

            // マップをクリア
            string argfname = "";
            Map.LoadMapData(ref argfname);
            string argdraw_mode = "";
            string argdraw_option = "ステータス";
            int argfilter_color = 0;
            double argfilter_trans_par = 0d;
            GUI.SetupBackground(ref argdraw_mode, ref argdraw_option, filter_color: ref argfilter_color, filter_trans_par: ref argfilter_trans_par);

            // ユニット一覧を作成
            if (key_type != "名称")
            {
                // 配列作成
                unit_list = new Unit[(SRC.UList.Count() + 1)];
                key_list = new int[(SRC.UList.Count() + 1)];
                i = 0;
                foreach (Unit currentU1 in SRC.UList)
                {
                    u = currentU1;
                    {
                        var withBlock1 = u;
                        if (withBlock1.Status_Renamed == "出撃" | withBlock1.Status_Renamed == "待機")
                        {
                            i = (short)(i + 1);
                            unit_list[i] = u;

                            // ソートする項目にあわせてソートの際の優先度を決定
                            switch (key_type ?? "")
                            {
                                case "ランク":
                                    {
                                        key_list[i] = withBlock1.Rank;
                                        break;
                                    }

                                case "ＨＰ":
                                    {
                                        key_list[i] = withBlock1.HP;
                                        break;
                                    }

                                case "ＥＮ":
                                    {
                                        key_list[i] = withBlock1.EN;
                                        break;
                                    }

                                case "装甲":
                                    {
                                        key_list[i] = withBlock1.get_Armor("");
                                        break;
                                    }

                                case "運動性":
                                    {
                                        key_list[i] = withBlock1.get_Mobility("");
                                        break;
                                    }

                                case "移動力":
                                    {
                                        key_list[i] = withBlock1.Speed;
                                        break;
                                    }

                                case "最大攻撃力":
                                    {
                                        var loopTo = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo; j++)
                                        {
                                            string argattr = "合";
                                            if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr))
                                            {
                                                string argtarea1 = "";
                                                if (withBlock1.WeaponPower(j, ref argtarea1) > key_list[i])
                                                {
                                                    string argtarea = "";
                                                    key_list[i] = withBlock1.WeaponPower(j, ref argtarea);
                                                }
                                            }
                                        }

                                        break;
                                    }

                                case "最長射程":
                                    {
                                        var loopTo1 = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo1; j++)
                                        {
                                            string argattr1 = "合";
                                            if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref argattr1))
                                            {
                                                if (withBlock1.WeaponMaxRange(j) > key_list[i])
                                                {
                                                    key_list[i] = withBlock1.WeaponMaxRange(j);
                                                }
                                            }
                                        }

                                        break;
                                    }

                                case "レベル":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Level;
                                        break;
                                    }

                                case "ＳＰ":
                                    {
                                        key_list[i] = withBlock1.MainPilot().MaxSP;
                                        break;
                                    }

                                case "格闘":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Infight;
                                        break;
                                    }

                                case "射撃":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Shooting;
                                        break;
                                    }

                                case "命中":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Hit;
                                        break;
                                    }

                                case "回避":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Dodge;
                                        break;
                                    }

                                case "技量":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Technique;
                                        break;
                                    }

                                case "反応":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Intuition;
                                        break;
                                    }
                            }
                        }
                    }
                }

                Array.Resize(ref unit_list, i + 1);
                Array.Resize(ref key_list, i + 1);

                // ソート
                var loopTo2 = (short)(Information.UBound(key_list) - 1);
                for (i = 1; i <= loopTo2; i++)
                {
                    max_item = i;
                    max_value = key_list[i];
                    var loopTo3 = (short)Information.UBound(unit_list);
                    for (j = (short)(i + 1); j <= loopTo3; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        u = unit_list[i];
                        unit_list[i] = unit_list[max_item];
                        unit_list[max_item] = u;
                        max_value = key_list[max_item];
                        key_list[max_item] = key_list[i];
                        key_list[i] = max_value;
                    }
                }
            }
            else
            {
                // 配列作成
                unit_list = new Unit[(SRC.UList.Count() + 1)];
                var strkey_list = new object[(SRC.UList.Count() + 1)];
                i = 0;
                foreach (Unit currentU2 in SRC.UList)
                {
                    u = currentU2;
                    {
                        var withBlock2 = u;
                        if (withBlock2.Status_Renamed == "出撃" | withBlock2.Status_Renamed == "待機")
                        {
                            i = (short)(i + 1);
                            unit_list[i] = u;
                            string argoname = "等身大基準";
                            if (Expression.IsOptionDefined(ref argoname))
                            {
                                // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                strkey_list[i] = withBlock2.MainPilot().KanaName;
                            }
                            else
                            {
                                // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                strkey_list[i] = withBlock2.KanaName;
                            }
                        }
                    }
                }

                Array.Resize(ref unit_list, i + 1);
                Array.Resize(ref strkey_list, i + 1);

                // ソート
                var loopTo4 = (short)(Information.UBound(strkey_list) - 1);
                for (i = 1; i <= loopTo4; i++)
                {
                    max_item = i;
                    // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    max_str = Conversions.ToString(strkey_list[i]);
                    var loopTo5 = (short)Information.UBound(strkey_list);
                    for (j = (short)(i + 1); j <= loopTo5; j++)
                    {
                        // UPGRADE_WARNING: オブジェクト strkey_list() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Strings.StrComp(Conversions.ToString(strkey_list[j]), max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            // UPGRADE_WARNING: オブジェクト strkey_list(j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            max_str = Conversions.ToString(strkey_list[j]);
                        }
                    }

                    if (max_item != i)
                    {
                        u = unit_list[i];
                        unit_list[i] = unit_list[max_item];
                        unit_list[max_item] = u;

                        // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト strkey_list(max_item) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            // Font Regular 9pt 背景
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            {
                var withBlock3 = GUI.MainForm.picMain(0).Font;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock3.Size = 9;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock3.Bold = false;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock3.Italic = false;
            }

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;

            // ユニットのリストを作成
            xx = 1;
            yy = 1;
            var loopTo6 = (short)Information.UBound(unit_list);
            for (i = 1; i <= loopTo6; i++)
            {
                u = unit_list[i];
                {
                    var withBlock4 = u;
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
                    if (withBlock4.CountPilot() == 0)
                    {
                        string argpname = "ステータス表示用ダミーパイロット(ザコ)";
                        string argpparty = "味方";
                        string arggid = "";
                        p = SRC.PList.Add(ref argpname, 1, ref argpparty, gid: ref arggid);
                        p.Ride(ref u);
                    }

                    // 出撃
                    withBlock4.UsedAction = 0;
                    withBlock4.StandBy(xx, yy);

                    // プレイヤーが操作できないように
                    string argcname = "非操作";
                    string argcdata = "";
                    withBlock4.AddCondition(ref argcname, -1, cdata: ref argcdata);

                    // ユニットの愛称を表示
                    string argmsg = withBlock4.Nickname;
                    GUI.DrawString(ref argmsg, 32 * xx + 2, 32 * yy - 31);
                    withBlock4.Nickname = argmsg;

                    // ソート項目にあわせてユニットのステータスを表示
                    switch (key_type ?? "")
                    {
                        case "ランク":
                            {
                                string argtname = "HP";
                                string argtname1 = "EN";
                                string argmsg1 = "RK" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]) + " " + Expression.Term(ref argtname, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + " " + Expression.Term(ref argtname1, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.EN);
                                GUI.DrawString(ref argmsg1, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "ＨＰ":
                        case "ＥＮ":
                        case "名称":
                            {
                                string argtname2 = "HP";
                                string argtname3 = "EN";
                                string argmsg2 = Expression.Term(ref argtname2, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + " " + Expression.Term(ref argtname3, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.EN);
                                GUI.DrawString(ref argmsg2, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "装甲":
                            {
                                string argtname4 = "装甲";
                                string argmsg3 = Expression.Term(ref argtname4, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg3, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "運動性":
                            {
                                string argtname5 = "運動性";
                                string argmsg4 = Expression.Term(ref argtname5, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg4, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "移動力":
                            {
                                string argtname6 = "移動力";
                                string argmsg5 = Expression.Term(ref argtname6, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg5, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "最大攻撃力":
                            {
                                string argmsg6 = "攻撃力" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg6, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "最長射程":
                            {
                                string argmsg7 = "射程" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg7, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "レベル":
                            {
                                string argmsg8 = "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg8, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "ＳＰ":
                            {
                                string argtname7 = "SP";
                                string argmsg9 = Expression.Term(ref argtname7, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg9, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "格闘":
                            {
                                string argtname8 = "格闘";
                                string argmsg10 = Expression.Term(ref argtname8, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg10, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "射撃":
                            {
                                if (withBlock4.MainPilot().HasMana())
                                {
                                    string argtname9 = "魔力";
                                    string argmsg11 = Expression.Term(ref argtname9, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                    GUI.DrawString(ref argmsg11, 32 * xx + 2, 32 * yy - 15);
                                }
                                else
                                {
                                    string argtname10 = "射撃";
                                    string argmsg12 = Expression.Term(ref argtname10, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                    GUI.DrawString(ref argmsg12, 32 * xx + 2, 32 * yy - 15);
                                }

                                break;
                            }

                        case "命中":
                            {
                                string argtname11 = "命中";
                                string argmsg13 = Expression.Term(ref argtname11, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg13, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "回避":
                            {
                                string argtname12 = "回避";
                                string argmsg14 = Expression.Term(ref argtname12, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg14, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "技量":
                            {
                                string argtname13 = "技量";
                                string argmsg15 = Expression.Term(ref argtname13, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg15, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "反応":
                            {
                                string argtname14 = "反応";
                                string argmsg16 = Expression.Term(ref argtname14, ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]);
                                GUI.DrawString(ref argmsg16, 32 * xx + 2, 32 * yy - 15);
                                break;
                            }
                    }

                    // 表示位置を右に5マスずらす
                    xx = (short)(xx + 5);
                }
            }

            // フォントの設定を戻しておく
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            {
                var withBlock5 = GUI.MainForm.picMain(0).Font;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock5.Size = 16;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock5.Bold = true;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock5.Italic = false;
            }

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
        }
    }
}
