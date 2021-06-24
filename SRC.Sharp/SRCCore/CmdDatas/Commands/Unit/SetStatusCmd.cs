using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class SetStatusCmd : CmdData
    {
        public SetStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetStatusCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string cname;
            switch (ArgNum)
            {
                case 4:
                    {
                        u = GetArgAsUnit(2);
                        {
                            var withBlock = u;
                            cname = GetArgAsString(3);
                            withBlock.AddCondition(cname, GetArgAsLong(4), cdata: "");
                            if (withBlock.Status == "出撃")
                            {
                                GUI.PaintUnitBitmap(u);
                            }

                            if (cname != "非操作")
                            {
                                withBlock.Update();
                            }
                        }

                        break;
                    }

                case 3:
                    {
                        if (Event.SelectedUnitForEvent is object)
                        {
                            {
                                var withBlock1 = Event.SelectedUnitForEvent;
                                cname = GetArgAsString(2);
                                withBlock1.AddCondition(cname, GetArgAsLong(3), cdata: "");
                                if (withBlock1.Status == "出撃")
                                {
                                    GUI.PaintUnitBitmap(Event.SelectedUnitForEvent);
                                }

                                if (cname != "非操作")
                                {
                                    withBlock1.Update();
                                }
                            }
                        }

                        break;
                    }

                default:
                    throw new EventErrorException(this, "SetStatusコマンドの引数の数が違います");
            }

            return EventData.NextID;
        }
    }
}
