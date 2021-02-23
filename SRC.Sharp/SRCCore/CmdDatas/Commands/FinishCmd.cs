using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class FinishCmd : CmdData
    {
        public FinishCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FinishCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            switch (ArgNum)
            {
                case 2:
                    {
                        u = GetArgAsUnit((short)2, true);
                        break;
                    }

                case 1:
                    {
                        u = Event_Renamed.SelectedUnitForEvent;
                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "Finishコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 249131


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            if (u is object)
            {
                {
                    var withBlock = u;
                    switch (withBlock.Action)
                    {
                        case 1:
                            {
                                withBlock.UseAction();
                                if (withBlock.Status_Renamed == "出撃")
                                {
                                    GUI.PaintUnitBitmap(ref u);
                                }

                                break;
                            }
                        // なにもしない
                        case 0:
                            {
                                break;
                            }

                        default:
                            {
                                withBlock.UseAction();
                                break;
                            }
                    }
                }
            }

            return EventData.ID + 1;
        }
    }
}
