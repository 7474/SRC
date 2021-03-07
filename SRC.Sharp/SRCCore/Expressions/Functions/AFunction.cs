using System;
using System.Collections.Generic;
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

        public abstract ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result);
    }
}
