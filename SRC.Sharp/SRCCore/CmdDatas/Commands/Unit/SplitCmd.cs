using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SplitCmd : CmdData
    {
        public SplitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SplitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            switch (ArgNum)
            {
                case 1:
                    {
                        u = Event.SelectedUnitForEvent;
                        break;
                    }

                case 2:
                    {
                        u = GetArgAsUnit(2);
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "Splitコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 494229


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            {
                var withBlock = u;
                if (!withBlock.IsFeatureAvailable("分離"))
                {
                    Event.EventErrorMessage = withBlock.Name + "は分離できません";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 494387


                    Input:
                                    Error(0)

                     */
                }

                withBlock.Split();

                // 分離形態の１番目のユニットをメインユニットに設定
                string localLIndex() { object argIndex1 = "分離"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                u = SRC.UList.Item(localLIndex());

                // 変数のアップデート
                if (Commands.SelectedUnit is object)
                {
                    if ((withBlock.ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
                    {
                        Commands.SelectedUnit = u;
                    }
                }

                if (Event.SelectedUnitForEvent is object)
                {
                    if ((withBlock.ID ?? "") == (Event.SelectedUnitForEvent.ID ?? ""))
                    {
                        Event.SelectedUnitForEvent = u;
                    }
                }

                if (Commands.SelectedTarget is object)
                {
                    if ((withBlock.ID ?? "") == (Commands.SelectedTarget.ID ?? ""))
                    {
                        Commands.SelectedTarget = u;
                    }
                }

                if (Event.SelectedTargetForEvent is object)
                {
                    if ((withBlock.ID ?? "") == (Event.SelectedTargetForEvent.ID ?? ""))
                    {
                        Event.SelectedTargetForEvent = u;
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
