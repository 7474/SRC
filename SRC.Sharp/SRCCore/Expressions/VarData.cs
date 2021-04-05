// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Expressions
{
    // 変数のクラス
    public class VarData
    {
        // 名称
        public string Name;
        // 型
        public ValueType VariableType;
        // 文字列値
        public string StringValue;
        // 数値
        public double NumericValue;

        public override string ToString()
        {
            return $"{Name}:{StringValue}({NumericValue})";
        }

        public ValueType ReferenceValue(ValueType etype, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            switch (etype)
            {
                case ValueType.NumericType:
                    {
                        if (VariableType == ValueType.NumericType)
                        {
                            num_result = NumericValue;
                        }
                        else
                        {
                            num_result = Conversions.ToDouble(StringValue);
                        }
                        return ValueType.NumericType;
                    }

                case ValueType.StringType:
                    {
                        if (VariableType == ValueType.StringType)
                        {
                            str_result = StringValue;
                        }
                        else
                        {
                            str_result = GeneralLib.FormatNum(NumericValue);
                        }
                        return ValueType.StringType;
                    }

                case ValueType.UndefinedType:
                default:
                    {
                        // XXX 逆の側の型に値入れなくていいの？
                        if (VariableType == ValueType.StringType)
                        {
                            str_result = StringValue;
                            return ValueType.StringType;
                        }
                        else
                        {
                            num_result = NumericValue;
                            return ValueType.NumericType;
                        }
                    }
            }
        }
    }
}
