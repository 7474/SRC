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
    }
}
