using System;

namespace SRCCore.VB
{
    /// <summary>
    /// VB5 の数値変換の挙動を再現するためのヘルパー。
    /// </summary>
    public static class VBMath
    {
        /// <summary>
        /// VB5 の CLng 相当の変換を行う。
        /// Double から Long/Integer 型への暗黙の型変換も同じく偶数丸め（銀行型丸め）になる。
        /// C# の (int) キャストは切り捨てになるため、VB で暗黙に変換していた箇所ではこちらを使う。
        /// </summary>
        public static int CLng(double x)
        {
            return checked((int)Math.Round(x, MidpointRounding.ToEven));
        }

        /// <summary>
        /// VB5 の整数除算（\ 演算子）。割る前に被除数を偶数丸めで整数にしてから、商の小数部を切り捨てる。
        /// C# の (long) キャストは切り捨てになるため、Double を \ で割っていた箇所ではこちらを使う。
        /// https://learn.microsoft.com/office/vba/language/reference/user-interface-help/backslash-operator
        /// </summary>
        public static long IntDiv(double dividend, long divisor)
        {
            return (long)Math.Round(dividend, MidpointRounding.ToEven) / divisor;
        }
    }
}
