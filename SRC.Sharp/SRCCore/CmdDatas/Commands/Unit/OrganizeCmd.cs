using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OrganizeCmd : CmdData
    {
        public OrganizeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OrganizeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            short unum;
            //            short ux, uy;
            //            var uclass = default(string);
            //            string buf, opt = default;
            //            short j, i, num;
            //            int tmp;
            //            int min_value;
            //            short max_item;
            //            int max_value;
            //            int[] lv_list;
            //            string[] list;
            //            short ret;
            //            bool without_refresh;
            //            bool without_animation;
            //            num = ArgNum;
            //            var loopTo = num;
            //            for (i = 5; i <= loopTo; i++)
            //            {
            //                // MOD START MARGE
            //                // Select Case GetArgAsString(num)
            //                switch (GetArgAsString(i) ?? "")
            //                {
            //                    // MOD END MARGE
            //                    case "密集":
            //                        {
            //                            opt = opt + " 出撃";
            //                            break;
            //                        }
            //                    // num = num - 1
            //                    case "非同期":
            //                        {
            //                            opt = opt + " 非同期";
            //                            break;
            //                        }
            //                    // num = num - 1
            //                    case "アニメ非表示":
            //                        {
            //                            opt = opt + " アニメ非表示";
            //                            break;
            //                        }
            //                        // num = num - 1
            //                }
            //            }
            //            // MOD START MARGE
            //            // If InStr(opt, "出撃") = 0 Then
            //            if (Strings.InStr(opt, "出撃") <= 0)
            //            {
            //                // MOD END MARGE
            //                opt = opt + " 部隊配置";
            //            }

            //            if (num < 4)
            //            {
            //                Event.EventErrorMessage = "Organizeコマンドの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 355755


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            unum = GetArgAsLong(2);
            //            if (unum < 1)
            //            {
            //                Event.EventErrorMessage = "ユニット数が不正です";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 355898


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            ux = GetArgAsLong(3);
            //            if (ux < 1)
            //            {
            //                ux = 1;
            //            }
            //            else if (ux > Map.MapWidth)
            //            {
            //                ux = Map.MapWidth;
            //            }

            //            uy = GetArgAsLong(4);
            //            if (uy < 1)
            //            {
            //                uy = 1;
            //            }
            //            else if (uy > Map.MapHeight)
            //            {
            //                uy = Map.MapHeight;
            //            }

            //            if (num < 5)
            //            {
            //                uclass = "全て";
            //            }
            //            else
            //            {
            //                var loopTo1 = num;
            //                for (i = 5; i <= loopTo1; i++)
            //                    uclass = uclass + " " + GetArgAsString(i);
            //                uclass = Strings.Trim(uclass);
            //            }

            //            Beginning:
            //            ;
            //            list = new string[1];
            //            GUI.ListItemID = new string[1];
            //            foreach (Unit u in SRC.UList)
            //            {
            //                if (u.Party0 != "味方" || u.Status != "待機" || u.CountPilot() == 0)
            //                {
            //                    goto NextOrganizeLoop;
            //                }

            //                // パイロット数のチェック
            //                if ((u.Data.PilotNum == 1 || Math.Abs(u.Data.PilotNum) == 2) && u.CountPilot() < Math.Abs(u.Data.PilotNum) && !u.IsFeatureAvailable("１人乗り可能"))
            //                {
            //                    goto NextOrganizeLoop;
            //                }

            //                switch (Map.TerrainClass(1, 1) ?? "")
            //                {
            //                    case "宇宙":
            //                    case "月面":
            //                        {
            //                            if (u.get_Adaption(4) == 0)
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            // 宇宙専用ユニットは宇宙でしか活動できない
            //                            if (u.Transportation == "宇宙")
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            // 空中マップか？
            //                            if (Map.TerrainName(1, 1) == "空" && Map.TerrainName((Map.MapWidth / 2), (Map.MapHeight / 2)) == "空" && Map.TerrainName(Map.MapWidth, Map.MapHeight) == "空")
            //                            {
            //                                if (!u.IsTransAvailable("空"))
            //                                {
            //                                    goto NextOrganizeLoop;
            //                                }
            //                            }

            //                            break;
            //                        }
            //                }

            //                switch (uclass ?? "")
            //                {
            //                    case "全て":
            //                    case var @case when @case == "":
            //                        {
            //                            break;
            //                        }
            //                    // 全てのユニット
            //                    case "通常ユニット":
            //                        {
            //                            if (u.IsFeatureAvailable("母艦"))
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "母艦ユニット":
            //                        {
            //                            if (!u.IsFeatureAvailable("母艦"))
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "LL":
            //                        {
            //                            if (u.Size == "XL")
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "L":
            //                        {
            //                            if (u.Size == "XL" || u.Size == "LL")
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "M":
            //                        {
            //                            if (u.Size == "XL" || u.Size == "LL" || u.Size == "L")
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "S":
            //                        {
            //                            if (u.Size == "XL" || u.Size == "LL" || u.Size == "L" || u.Size == "M")
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    case "SS":
            //                        {
            //                            if (u.Size == "XL" || u.Size == "LL" || u.Size == "L" || u.Size == "M" || u.Size == "S")
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            // ユニットクラス指定した場合

            //                            // 指定されたクラスに該当するか
            //                            var loopTo2 = GeneralLib.ListLength(uclass);
            //                            for (i = 1; i <= loopTo2; i++)
            //                            {
            //                                if ((GeneralLib.ListIndex(uclass, i) ?? "") == (u.Class0 ?? ""))
            //                                {
            //                                    break;
            //                                }
            //                            }

            //                            if (i > GeneralLib.ListLength(uclass))
            //                            {
            //                                goto NextOrganizeLoop;
            //                            }

            //                            break;
            //                        }
            //                }

            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                Array.Resize(GUI.ListItemID, Information.UBound(list) + 1);
            //                if (Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    string localLeftPaddedString() { string argbuf = u.MainPilot().Level.ToString(); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

            //                    list[Information.UBound(list)] = u.Nickname0 + Strings.Space(GeneralLib.MaxLng(52 - LenB(Strings.StrConv(u.Nickname0, vbFromUnicode)), 1)) + localLeftPaddedString();
            //                }
            //                else
            //                {
            //                    string localLeftPaddedString1() { string argbuf = u.MainPilot().Level.ToString(); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

            //                    list[Information.UBound(list)] = u.Nickname0 + Strings.Space(GeneralLib.MaxLng(36 - LenB(Strings.StrConv(u.Nickname0, vbFromUnicode)), 1)) + u.MainPilot().get_Nickname(false) + Strings.Space(GeneralLib.MaxLng(17 - LenB(Strings.StrConv(u.MainPilot().get_Nickname(false), vbFromUnicode)), 1)) + localLeftPaddedString1();
            //                }

            //                GUI.ListItemID[Information.UBound(list)] = u.ID;
            //                NextOrganizeLoop:
            //                ;
            //            }

            //            GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

            //            // レベルの一覧と最大値・最小値を求める
            //            lv_list = new int[Information.UBound(list) + 1];
            //            min_value = 100000;
            //            max_value = 0;
            //            var loopTo3 = Information.UBound(list);
            //            for (i = 1; i <= loopTo3; i++)
            //            {
            //                Unit localItem() { var tmp1 = GUI.ListItemID; object argIndex1 = tmp1[i]; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                {
            //                    var withBlock = localItem().MainPilot();
            //                    lv_list[i] = 500 * withBlock.Level + withBlock.Exp;
            //                }

            //                if (lv_list[i] > max_value)
            //                {
            //                    max_value = lv_list[i];
            //                }

            //                if (lv_list[i] < min_value)
            //                {
            //                    min_value = lv_list[i];
            //                }
            //            }

            //            // レベルにばらつきがある時にのみレベルでソート
            //            if (min_value != max_value)
            //            {
            //                var loopTo4 = (Information.UBound(list) - 1);
            //                for (i = 1; i <= loopTo4; i++)
            //                {
            //                    max_item = i;
            //                    max_value = lv_list[i];
            //                    var loopTo5 = Information.UBound(list);
            //                    for (j = (i + 1); j <= loopTo5; j++)
            //                    {
            //                        if (lv_list[j] > max_value)
            //                        {
            //                            max_item = j;
            //                            max_value = lv_list[j];
            //                        }
            //                    }

            //                    if (max_item != i)
            //                    {
            //                        buf = list[i];
            //                        list[i] = list[max_item];
            //                        list[max_item] = buf;
            //                        buf = GUI.ListItemID[i];
            //                        GUI.ListItemID[i] = GUI.ListItemID[max_item];
            //                        GUI.ListItemID[max_item] = buf;
            //                        lv_list[max_item] = lv_list[i];
            //                    }
            //                }
            //            }

            //            if (Information.UBound(list) > 0)
            //            {
            //                do
            //                {
            //                    if (Expression.IsOptionDefined("等身大基準"))
            //                    {
            //                        ret = GUI.MultiSelectListBox("出撃ユニット選択", list, "ユニット                                            Lv", unum);
            //                    }
            //                    else
            //                    {
            //                        ret = GUI.MultiSelectListBox("出撃ユニット選択", list, "ユニット                            パイロット       Lv", unum);
            //                    }

            //                    if (ret == 0)
            //                    {
            //                        Commands.CommandState = "ユニット選択";
            //                        GUI.UnlockGUI();
            //                        Commands.ViewMode = true;
            //                        while (Commands.ViewMode)
            //                        {
            //                            GUI.Sleep(50);
            //                            Application.DoEvents();
            //                        }

            //                        GUI.LockGUI();
            //                        goto Beginning;
            //                    }
            //                }
            //                while (ret == 0);
            //                if (Strings.InStr(opt, "非同期") > 0)
            //                {
            //                    GUI.Center(ux, uy);
            //                    GUI.RefreshScreen();
            //                }

            //                var loopTo6 = Information.UBound(list);
            //                for (i = 1; i <= loopTo6; i++)
            //                {
            //                    if (GUI.ListItemFlag[i])
            //                    {
            //                        var tmp1 = GUI.ListItemID;
            //                        {
            //                            var withBlock1 = SRC.UList.Item(tmp1[i]);
            //                            withBlock1.UsedAction = 0;
            //                            withBlock1.UsedSupportAttack = 0;
            //                            withBlock1.UsedSupportGuard = 0;
            //                            withBlock1.UsedSyncAttack = 0;
            //                            withBlock1.UsedCounterAttack = 0;
            //                            withBlock1.StandBy(ux, uy, opt);
            //                        }
            //                    }
            //                }
            //            }

            //            SRC.UList.CheckAutoHyperMode();
            //            GUI.ListItemID = new string[1];
            //return EventData.NextID;
        }
    }
}
