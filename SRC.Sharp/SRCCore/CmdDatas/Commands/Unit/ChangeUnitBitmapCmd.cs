using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeUnitBitmapCmd : CmdData
    {
        public ChangeUnitBitmapCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeUnitBitmapCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string new_bmp, prev_bmp;
            Unit u;
            switch (ArgNum)
            {
                case 2:
                    {
                        u = Event.SelectedUnitForEvent;
                        new_bmp = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        u = GetArgAsUnit(2);
                        new_bmp = GetArgAsString(3);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ChangeUnitBitmapコマンドの引数の数が違います");

            }

            prev_bmp = u.get_Bitmap(false);
            if (Strings.LCase(Strings.Right(new_bmp, 4)) == ".bmp")
            {
                u.AddCondition("ユニット画像", -1, 0d, "非表示 " + new_bmp);
            }
            else if (new_bmp == "-")
            {
                if (u.IsConditionSatisfied("ユニット画像"))
                {
                    u.DeleteCondition("ユニット画像");
                }
            }
            else if (new_bmp == "非表示")
            {
                u.AddCondition("非表示付加", -1, 0d, "非表示");
                GUI.EraseUnitBitmap(u.x, u.y, false);
            }
            else if (new_bmp == "非表示解除")
            {
                if (u.IsConditionSatisfied("非表示付加"))
                {
                    u.DeleteCondition("非表示付加");
                }
            }
            else
            {
                throw new EventErrorException(this, "ビットマップファイル名が不正です");
            }

            GUI.PaintUnitBitmap(u, "リフレッシュ無し");

            return EventData.NextID;
        }
    }
}
