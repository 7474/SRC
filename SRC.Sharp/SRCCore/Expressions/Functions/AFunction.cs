using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
