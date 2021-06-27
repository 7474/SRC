using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class OrganizeCmd : CmdData
    {
        public OrganizeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OrganizeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
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
            var opt = "";
            var num = ArgNum;
            for (var i = 5; i <= num; i++)
            {
                switch (GetArgAsString(i) ?? "")
                {
                    case "密集":
                        opt = opt + " 出撃";
                        break;
                    case "非同期":
                        opt = opt + " 非同期";
                        break;
                    case "アニメ非表示":
                        opt = opt + " アニメ非表示";
                        break;
                }
            }
            if (Strings.InStr(opt, "出撃") <= 0)
            {
                opt = opt + " 部隊配置";
            }

            if (num < 4)
            {
                throw new EventErrorException(this, "Organizeコマンドの引数の数が違います");
            }

            var unum = GetArgAsLong(2);
            if (unum < 1)
            {
                throw new EventErrorException(this, "ユニット数が不正です");
            }

            var ux = GetArgAsLong(3);
            if (ux < 1)
            {
                ux = 1;
            }
            else if (ux > Map.MapWidth)
            {
                ux = Map.MapWidth;
            }

            var uy = GetArgAsLong(4);
            if (uy < 1)
            {
                uy = 1;
            }
            else if (uy > Map.MapHeight)
            {
                uy = Map.MapHeight;
            }

            var uclass = "";
            if (num < 5)
            {
                uclass = "全て";
            }
            else
            {
                var loopTo1 = num;
                for (var i = 5; i <= num; i++)
                    uclass = uclass + " " + GetArgAsString(i);
                uclass = Strings.Trim(uclass);
            }

        Beginning:
            ;

            var units = SRC.UList.Items
                .Where(u => !(u.Party0 != "味方" || u.Status != "待機" || u.CountPilot() == 0))
                // パイロット数のチェック
                .Where(u => !((u.Data.PilotNum == 1 || Math.Abs(u.Data.PilotNum) == 2) && u.CountPilot() < Math.Abs(u.Data.PilotNum) && !u.IsFeatureAvailable("１人乗り可能")))
                .Where(u =>
                {
                    var td = Map.Terrain(1, 1);
                    switch (td.Class)
                    {
                        case "宇宙":
                        case "月面":
                            if (u.get_Adaption(4) == 0)
                            {
                                return false;
                            }

                            break;

                        default:
                            // 宇宙専用ユニットは宇宙でしか活動できない
                            if (u.Transportation == "宇宙")
                            {
                                return false;
                            }

                            // 空中マップか？
                            if (td.Name == "空"
                                    && Map.Terrain((Map.MapWidth / 2), (Map.MapHeight / 2)).Name == "空"
                                    && Map.Terrain(Map.MapWidth, Map.MapHeight).Name == "空")
                            {
                                if (!u.IsTransAvailable("空"))
                                {
                                    return false;
                                }
                            }

                            break;
                    }
                    return true;
                })
                .Where(u =>
                {
                    switch (uclass)
                    {
                        case "全て":
                        case "":
                            break;
                        // 全てのユニット
                        case "通常ユニット":
                            if (u.IsFeatureAvailable("母艦"))
                            {
                                return false;
                            }
                            break;

                        case "母艦ユニット":
                            if (!u.IsFeatureAvailable("母艦"))
                            {
                                return false;
                            }
                            break;

                        case "LL":
                            if (u.Size == "XL")
                            {
                                return false;
                            }
                            break;

                        case "L":
                            if (u.Size == "XL" || u.Size == "LL")
                            {
                                return false;
                            }
                            break;

                        case "M":
                            if (u.Size == "XL" || u.Size == "LL" || u.Size == "L")
                            {
                                return false;
                            }
                            break;

                        case "S":
                            if (u.Size == "XL" || u.Size == "LL" || u.Size == "L" || u.Size == "M")
                            {
                                return false;
                            }
                            break;

                        case "SS":
                            if (u.Size == "XL" || u.Size == "LL" || u.Size == "L" || u.Size == "M" || u.Size == "S")
                            {
                                return false;
                            }
                            break;

                        default:
                            // ユニットクラス指定した場合
                            // 指定されたクラスに該当するか
                            return GeneralLib.ToL(uclass).Any(x => x == u.Class0);
                    }
                    return true;
                })
                .ToList();

            if (!units.Any())
            {
                return EventData.NextID;
            }

            // レベルの一覧と最大値・最小値を求める
            var min_value = units.Min(u => u.MainPilot().Level + u.MainPilot().Exp);
            var max_value = units.Max(u => u.MainPilot().Level + u.MainPilot().Exp);

            // レベルにばらつきがある時にのみレベルでソート
            if (min_value != max_value)
            {
                units = units.OrderByDescending(u => u.MainPilot().Level + u.MainPilot().Exp).ToList();
            }
            var list = units
                .Select(u =>
                {
                    var msg = "";
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        msg = u.Nickname0
                            + Strings.Space(GeneralLib.MaxLng(52 - Strings.LenB(u.Nickname0), 1))
                            + GeneralLib.LeftPaddedString("" + u.MainPilot().Level, 2);
                    }
                    else
                    {
                        msg = u.Nickname0
                            + Strings.Space(GeneralLib.MaxLng(36 - Strings.LenB(u.Nickname0), 1))
                            + u.MainPilot().get_Nickname(false)
                            + Strings.Space(GeneralLib.MaxLng(17 - Strings.LenB(u.MainPilot().get_Nickname(false)), 1))
                            + GeneralLib.LeftPaddedString("" + u.MainPilot().Level, 2);
                    }
                    return new ListBoxItem(msg, u.ID);
                }).ToList();

            int ret;
            do
            {
                string info;
                if (Expression.IsOptionDefined("等身大基準"))
                {
                    info = "ユニット                                            Lv";
                }
                else
                {
                    info = "ユニット                            パイロット       Lv";
                }

                ret = GUI.MultiSelectListBox(new ListBoxArgs
                {
                    lb_caption = "出撃ユニット選択",
                    lb_info = info,
                    Items = list,
                }, unum);
                if (ret == 0)
                {
                    Commands.CommandState = "ユニット選択";
                    GUI.UnlockGUI();
                    Commands.ViewMode = true;
                    while (Commands.ViewMode)
                    {
                        GUI.Sleep(50);
                    }

                    GUI.LockGUI();
                    goto Beginning;
                }
            }
            while (ret == 0);
            if (Strings.InStr(opt, "非同期") > 0)
            {
                GUI.Center(ux, uy);
                GUI.RefreshScreen();
            }

            foreach(var u in list.Where(x => x.ListItemFlag)
                .Select(x => SRC.UList.Item(x.ListItemID)))
            {
                u.UsedAction = 0;
                u.UsedSupportAttack = 0;
                u.UsedSupportGuard = 0;
                u.UsedSyncAttack = 0;
                u.UsedCounterAttack = 0;
                u.StandBy(ux, uy, opt);
            }

            SRC.UList.CheckAutoHyperMode();

            return EventData.NextID;
        }
    }
}
