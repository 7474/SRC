using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeAreaCmd : CmdData
    {
        public ChangeAreaCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeAreaCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string new_area;
            switch (ArgNum)
            {
                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        new_area = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        u = GetArgAsUnit(2);
                        new_area = GetArgAsString(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ChangeAreaコマンドの引数の数が違います");

            }

            {
                switch (Map.Terrain(u.x, u.y).Class)
                {
                    case "陸":
                        {
                            if (new_area != "地上" && new_area != "空中" && new_area != "地中")
                            {
                                throw new EventErrorException(this, "場所の種類が不正です");
                            }

                            break;
                        }

                    case "屋内":
                        {
                            if (new_area != "地上" && new_area != "空中")
                            {
                                throw new EventErrorException(this, "場所の種類が不正です");
                            }

                            break;
                        }

                    case "月面":
                        {
                            if (new_area != "地上" && new_area != "宇宙" && new_area != "地中")
                            {
                                throw new EventErrorException(this, "場所の種類が不正です");
                            }

                            break;
                        }

                    case "水":
                    case "深水":
                        {
                            if (new_area != "水中" && new_area != "水上" && new_area != "空中")
                            {
                                throw new EventErrorException(this, "場所の種類が不正です");
                            }

                            break;
                        }

                    case "空中":
                        {
                            if (new_area != "空中")
                            {
                                throw new EventErrorException(this, "場所の種類が不正です");
                            }

                            break;
                        }

                    case "宇宙":
                        {
                            if (new_area != "宇宙")
                            {
                                throw new EventErrorException(this, "場所の種類が不正です");
                            }

                            break;
                        }
                }

                u.Area = new_area;
                u.Update();
                if (u.Status == "出撃")
                {
                    GUI.PaintUnitBitmap(u);
                }
            }

            GUI.RedrawScreen();

            return EventData.NextID;
        }
    }
}
