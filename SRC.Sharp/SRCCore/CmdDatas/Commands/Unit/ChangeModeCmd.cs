using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeModeCmd : CmdData
    {
        public ChangeModeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeModeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var targetUnits = new List<Unit>();
            string new_mode;
            switch (ArgNum)
            {
                case 2:
                    targetUnits.Add(Event.SelectedUnitForEvent);
                    new_mode = GetArgAsString(2);
                    break;

                case 3:
                    if (GetArgAsLong(2) > 0 & GetArgAsLong(3) > 0)
                    {
                        targetUnits.Add(Event.SelectedUnitForEvent);
                        var dst_x = GetArgAsLong(2);
                        var dst_y = GetArgAsLong(3);
                        if (!Map.IsInside(dst_x, dst_y))
                        {
                            throw new EventErrorException(this, "ChangeModeコマンドの目的地の座標が不正です");
                        }
                        new_mode = $"{dst_x} {dst_y}";
                    }
                    else
                    {
                        var pname = GetArgAsString(2);
                        switch (pname ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                            case "敵":
                            case "中立":
                                targetUnits.AddRange(SRC.UList.Items.Where(x => x.Party0 == pname));
                                break;

                            default:
                                var unit = SRC.UList.Item2(pname);
                                if (unit != null)
                                {
                                    targetUnits.Add(unit);
                                }
                                else
                                {
                                    if (!SRC.PList.IsDefined(pname))
                                    {
                                        throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                                    }
                                    targetUnits.Add(SRC.PList.Item(pname).Unit);
                                    targetUnits.AddRange(
                                        SRC.PList.ItemsByGroupId(pname, true).Select(x => x.Unit).Where(x => x != null));
                                }
                                break;
                        }
                        new_mode = GetArgAsString(3);
                    }

                    break;

                case 4:
                    {
                        var pname = GetArgAsString(2);
                        switch (pname ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                            case "敵":
                            case "中立":
                                targetUnits.AddRange(SRC.UList.Items.Where(x => x.Party0 == pname));
                                break;

                            default:
                                var unit = SRC.UList.Item2(pname);
                                if (unit != null)
                                {
                                    targetUnits.Add(unit);
                                }
                                else
                                {
                                    if (!SRC.PList.IsDefined(pname))
                                    {
                                        throw new EventErrorException(this, "「" + pname + "」というパイロットが見つかりません");
                                    }
                                    targetUnits.Add(SRC.PList.Item(pname).Unit);
                                    targetUnits.AddRange(
                                        SRC.PList.ItemsByGroupId(pname, true).Select(x => x.Unit).Where(x => x != null));
                                }
                                break;
                        }

                        var dst_x = GetArgAsLong(3);
                        var dst_y = GetArgAsLong(4);
                        if (!Map.IsInside(dst_x, dst_y))
                        {
                            throw new EventErrorException(this, "ChangeModeコマンドの目的地の座標が不正です");
                        }
                        new_mode = $"{dst_x} {dst_y}";
                        break;
                    }
                default:
                    throw new EventErrorException(this, "ChangeModeコマンドの引数の数が違います");
            }

            foreach (var unit in targetUnits)
            {
                unit.Mode = new_mode;
            }

            return EventData.NextID;
        }
    }
}
