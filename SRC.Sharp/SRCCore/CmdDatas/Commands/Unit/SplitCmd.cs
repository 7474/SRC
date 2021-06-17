using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;

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
                    throw new EventErrorException(this, "Splitコマンドの引数の数が違います");
            }

            if (!u.IsFeatureAvailable("分離"))
            {
                throw new EventErrorException(this, u.Name + "は分離できません");
            }

            u.Split();

            // 分離形態の１番目のユニットをメインユニットに設定
            var su = SRC.UList.Item(GeneralLib.LIndex(u.FeatureData("分離"), 2));

            // 変数のアップデート
            // XXX 元はSavedStateは更新していなかったけれど更新してる
            UpdateSelectedState(u, su);

            return EventData.NextID;
        }
    }
}
