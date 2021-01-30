using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.CmdDatas
{
    public class CmdArgument
    {
        // 引数の値
        public int lngArg { get; set; }
        public double dblArg { get; set; }
        public string strArg { get; set; }

        // 引数の型
        public Expressions.ValueType argType { get; set; }
    }
}
