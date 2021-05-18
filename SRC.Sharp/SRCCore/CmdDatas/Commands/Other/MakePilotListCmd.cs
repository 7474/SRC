using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class MakePilotListCmd : CmdData
    {
        public MakePilotListCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MakePilotListCmd, eventData)
        {
        }

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
            var target_pilot_list = SRC.PList.Items
                .Where(p => p.Alive && !p.Away)
                // 追加パイロットは勘定に入れない
                // 追加サポートは勘定に入れない
                .Where(p => !p.IsAdditionalPilot && !p.IsAdditionalSupport)
                .ToList();
            IList<Pilot> pilot_list;

            var key_type = GetArgAsString(2);
            if (key_type != "名称")
            {
                pilot_list = target_pilot_list.OrderByDescending(p =>
                {
                    switch (key_type)
                    {
                        case "レベル":
                            return p.Level;
                        case "ＳＰ":
                            return p.MaxSP;
                        case "格闘":
                            return p.Infight;
                        case "射撃":
                            return p.Shooting;
                        case "命中":
                            return p.Hit;
                        case "回避":
                            return p.Dodge;
                        case "技量":
                            return p.Technique;
                        case "反応":
                            return p.Intuition;
                        default:
                            throw new EventErrorException(this, "MakePilotListコマンドの引数が違います");
                    }
                }).ToList();
            }
            else
            {
                pilot_list = target_pilot_list.OrderBy(p => p.KanaName).ToList();
            }

            // Font Regular 9pt 背景
            GUI.SetDrawString(DrawStringMode.Status);

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;
            var xx = 1;
            var yy = 1;
            foreach (var p in pilot_list)
            {
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
                // TODO セルサイズ
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
                            GUI.DrawString(Expression.Term("SP", u) + SrcFormatter.Format(p.MaxSP), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "格闘":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("格闘", u), 1) + SrcFormatter.Format(p.Infight), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "射撃":
                        {
                            if (p.HasMana())
                            {
                                GUI.DrawString(Strings.Left(Expression.Term("魔力", u), 1) + SrcFormatter.Format(p.Shooting), 32 * xx + 2, 32 * yy - 15);
                            }
                            else
                            {
                                GUI.DrawString(Strings.Left(Expression.Term("射撃", u), 1) + SrcFormatter.Format(p.Shooting), 32 * xx + 2, 32 * yy - 15);
                            }

                            break;
                        }

                    case "命中":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("命中", u), 1) + SrcFormatter.Format(p.Hit), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "回避":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("回避", u), 1) + SrcFormatter.Format(p.Dodge), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "技量":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("技量", u), 1) + SrcFormatter.Format(p.Technique), 32 * xx + 2, 32 * yy - 15);
                            break;
                        }

                    case "反応":
                        {
                            GUI.DrawString(Strings.Left(Expression.Term("反応", u), 1) + SrcFormatter.Format(p.Intuition), 32 * xx + 2, 32 * yy - 15);
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

            // マウスカーソルを元に戻す\
            GUI.ChangeStatus(GuiStatus.Default);
            return EventData.NextID;
        }
    }
}
