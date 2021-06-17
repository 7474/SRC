using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

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
                    throw new EventErrorException(this, "Transformコマンドの引数の数が違います");
            }

            if ((u.Name ?? "") == (tname ?? ""))
            {
                // 元々指定された形態になっていたので変形の必要なし
                return EventData.NextID;
            }

            // 変形
            u.Transform(tname);

            // グローバル変数の更新
            // XXX 元はSavedStateは更新していなかったけれど更新してる
            UpdateSelectedState(u, u.CurrentForm()); 
            return EventData.NextID;
        }
    }
}
