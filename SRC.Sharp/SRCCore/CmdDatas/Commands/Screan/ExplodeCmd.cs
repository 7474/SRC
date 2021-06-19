using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ExplodeCmd : CmdData
    {
        public ExplodeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExplodeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string esize;
            int tx, ty;
            switch (ArgNum)
            {
                case 2:
                    {
                        esize = GetArgAsString(2);
                        tx = GUI.MapX;
                        ty = GUI.MapY;
                        break;
                    }

                case 4:
                    {
                        esize = GetArgAsString(2);
                        tx = GetArgAsLong(3);
                        ty = GetArgAsLong(4);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "Explodeコマンドの引数の数が違います");
            }

            // 爆発の表示
            SRC.Effect.ExplodeAnimation(esize, tx, ty);
            return EventData.NextID;
        }
    }
}
