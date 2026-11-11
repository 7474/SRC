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
        /// VB5 の CInt 相当の変換（Integer は 16 ビット）。Integer の変数・引数への代入も同じ。
        /// 範囲（-32768〜32767）を超えると、VB5 と同じくあふれ（OverflowException）にする
        /// （元の SRC の実機で RankUp・IncreaseMorale に 65541 を渡すとエラーになる。#1550）
        /// </summary>
        public static short CInt(double x)
        {
            return checked((short)Math.Round(x, MidpointRounding.ToEven));
        }

        /// <summary>
        /// VB5 の Int 関数。x を超えない最大の整数（負の数は小さいほうへ。Int(-1.5) = -2）。
        /// https://learn.microsoft.com/office/vba/language/reference/user-interface-help/int-fix-functions
        /// </summary>
        public static int Int(double x)
        {
            return checked((int)Math.Floor(x));
        }

        /// <summary>
        /// VB5 の Fix 関数。小数部を切り捨てる（0 に近いほうへ。Fix(-1.5) = -1）。C# の (int) キャストと同じ。
        /// https://learn.microsoft.com/office/vba/language/reference/user-interface-help/int-fix-functions
        /// </summary>
        public static int Fix(double x)
        {
            return checked((int)Math.Truncate(x));
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

        /// <summary>
        /// VB5 の Double の演算の結果を確かめる。VB5 は結果が Double を超える（無限大）と実行時エラー 6（オーバーフロー）、
        /// 負の数の分数乗・負の数の平方根（非数）は実行時エラー 5 になる（元の SRC の実機で確かめた。
        /// SRC.Sharp/Compat/probe の VB5.浮動小数点.*。#1550）。.NET は無限大・非数を返すので、例外にする
        /// </summary>
        public static double Checked(double x)
        {
            if (double.IsInfinity(x))
            {
                throw new OverflowException("オーバーフローしました。");
            }
            if (double.IsNaN(x))
            {
                throw new ArgumentException("プロシージャの呼び出し、または引数が不正です。");
            }
            return x;
        }
    }
}
