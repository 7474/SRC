using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class IncrCmd : CmdData
    {
        public IncrCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IncrCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            double num;
            var vname = GetArg(2);
            Expression.GetVariable(vname, ValueType.NumericType, out _, out num);
            switch (ArgNum)
            {
                case 3:
                        Expression.SetVariableAsDouble(vname, num + GetArgAsDouble(3));
                        break;

                case 2:
                        Expression.SetVariableAsDouble(vname, num + 1d);
                        break;

                default:
                    throw new EventErrorException(this, "Incrコマンドの引数の数が違います");
            }

            return EventData.NextID;
        }
    }
}
