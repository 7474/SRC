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
            SRC.LogDebug($"{GetType().Name}({string.Join(", ", Enumerable.Range(1, pcount).Select(x => @params[x]))}) => {str_result}");
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
                    if (p.Unit != null)
                    {
                        {
                            var u = p.Unit;
                            if (u.Status == "oŒ‚" || u.Status == "Ši”[")
                            {
                                unit = u;
                            }
                        }
                    }
                }
            }

            return InvokeInternal(SRC, unit, etype, @params, pcount, is_term, out str_result, out num_result);
        }

        protected abstract ValueType InvokeInternal(SRC SRC, Units.Unit unit, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result);
    }
}
