using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class TransformCmd : CmdData
    {
        public TransformCmd(SRC src, EventDataLine eventData) : base(src, CmdType.TransformCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit u;
            string tname;
            switch (ArgNum)
            {
                case 3:
                    {
                        u = GetArgAsUnit(2);
                        tname = GetArgAsString(3);
                        break;
                    }

                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        tname = GetArgAsString(2);
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "Transformコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 513842


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            if ((u.Name ?? "") == (tname ?? ""))
            {
                // 元々指定された形態になっていたので変形の必要なし
                ExecTransformCmdRet = LineNum + 1;
                return ExecTransformCmdRet;
            }

            // 変形
            u.Transform(tname);

            // グローバル変数の更新
            if (ReferenceEquals(u, Commands.SelectedUnit))
            {
                Commands.SelectedUnit = u.CurrentForm();
            }

            if (ReferenceEquals(u, Event.SelectedUnitForEvent))
            {
                Event.SelectedUnitForEvent = u.CurrentForm();
            }

            if (ReferenceEquals(u, Commands.SelectedTarget))
            {
                Commands.SelectedTarget = u.CurrentForm();
            }

            if (ReferenceEquals(u, Event.SelectedTargetForEvent))
            {
                Event.SelectedTargetForEvent = u.CurrentForm();
            }

            return EventData.NextID;
        }
    }
}
