using System.Linq;

namespace SRCCore.Expressions.Functions
{
    public interface IFunction
    {
        string Name { get; }

        ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result);
    }

    public abstract class AFunction : IFunction
    {
        public virtual string Name => GetType().Name;
        public ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var res = InvokeInternal(SRC, etype, @params, pcount, is_term, out str_result, out num_result);
            SRC.LogTrace("Invoked",
               res.ToString().Substring(0, 1),
               GetType().Name + "(" + string.Join(",", Enumerable.Range(1, pcount).Select(x => @params[x])) + ")",
               str_result,
               num_result.ToString()
            );
            return res;
        }

        protected abstract ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result);
    }

    public abstract class AUnitFunction : AFunction
    {
        protected virtual int OptionArgCount => 0;
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            Units.Unit unit = null;

            if (OptionArgCount <= pcount)
            {
                unit = SRC.Event.SelectedUnitForEvent;
            }
            else
            {
                var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (SRC.UList.IsDefined2(pname))
                {
                    unit = SRC.UList.Item2(pname);
                }
                else if (SRC.PList.IsDefined(pname))
                {
                    var p = SRC.PList.Item(pname);
                    unit = p.Unit;
                }
            }

            return InvokeInternal(SRC, unit, etype, @params, pcount, is_term, out str_result, out num_result);
        }

        protected abstract ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result);
    }

    public abstract class APilotFunction : AFunction
    {
        protected virtual int OptionArgCount => 0;
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            Pilots.Pilot pilot = null;

            if (OptionArgCount <= pcount)
            {
                pilot = SRC.Event.SelectedUnitForEvent?.MainPilot();
            }
            else
            {
                var pname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                if (SRC.UList.IsDefined2(pname))
                {
                    pilot = SRC.UList.Item2(pname).MainPilot();
                }
                else if (SRC.PList.IsDefined(pname))
                {
                    pilot = SRC.PList.Item(pname);
                }
            }

            return InvokeInternal(SRC, pilot, etype, @params, pcount, is_term, out str_result, out num_result);
        }

        protected abstract ValueType InvokeInternal(SRC SRC, Pilots.Pilot pilot, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result);
    }
}
