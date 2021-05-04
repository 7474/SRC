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
        public string Name { get; private set; }
        // 型
        public ValueType VariableType { get; private set; }
        // 文字列値
        public string StringValue { get; private set; }
        // 数値
        public double NumericValue { get; private set; }

        public override string ToString()
        {
            return $"{Name}:{StringValue}({NumericValue})";
        }

        public VarData()
        {
            Clear();
        }
        public VarData(string name, ValueType etype, string str_value, double num_value)
        {
            SetValue(name, etype, str_value, num_value);
        }

        public void Clear()
        {
            Init("");
        }

        public void Init(string name)
        {
            SetValue(name, ValueType.StringType, "", 0d);
        }

        public void SetValue(string name, ValueType etype, string str_value, double num_value)
        {
            Name = name;
            VariableType = etype;
            StringValue = str_value;
            NumericValue = num_value;
            if (etype == ValueType.StringType)
            {
                NumericValue = Conversions.ToDouble(str_value);
            }
            else if (etype == ValueType.NumericType)
            {
                StringValue = Conversions.ToString(num_value);
            }
        }
        /// <summary>
        /// Name も含めて指定した値を設定する。
        /// </summary>
        /// <param name="v"></param>
        public void SetFrom(VarData v)
        {
            Name = v.Name;
            VariableType = v.VariableType;
            StringValue = v.StringValue;
            NumericValue = v.NumericValue;
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
                        // XXX Undefind 時の参照、Numeric？
                        if (VariableType != ValueType.NumericType)
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
