// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.Lib;

namespace SRCCore.Expressions
{
    // 変数のクラス
    // セーブデータに含めるため、private set のプロパティも JsonProperty で読み書きする
    public class VarData
    {
        // 名称
        [JsonProperty]
        public string Name { get; private set; } = "";
        // 型
        [JsonProperty]
        public ValueType VariableType { get; private set; }
        // 文字列値
        [JsonProperty]
        public string StringValue { get; private set; } = "";
        // 数値
        [JsonProperty]
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

        // VB（SetVariable の With new_var ～ End With）と同じく、渡した値をそのまま入れる。
        // もう一方の型の値は作らない（読み出しは VariableType を見て変換する。StringValue・NumericValue を直接読む処理も VB と同じ値を見る）
        public void SetValue(string name, ValueType etype, string str_value, double num_value)
        {
            Name = name;
            VariableType = etype;
            StringValue = str_value;
            NumericValue = num_value;
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
        /// <summary>
        /// Name 以外（型・文字列値・数値）を指定した変数から写す。
        /// </summary>
        /// <param name="v"></param>
        public void SetValueFrom(VarData v)
        {
            VariableType = v.VariableType;
            StringValue = v.StringValue;
            NumericValue = v.NumericValue;
        }

        // VB の CallFunction の「実はシステム定義のグローバル変数？」で値を読む部分
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
                            num_result = GeneralLib.StrToDbl(StringValue);
                        }
                        return ValueType.NumericType;
                    }

                case ValueType.StringType:
                    {
                        // VB と同じく、文字列の変数でなければ（UndefinedType も）数値を書式化する
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
                        // 求めた型が無いときは変数の型の値だけを返し、もう一方の結果は書き換えない（VB と同じ）
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
