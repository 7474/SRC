using SRCCore.Lib;
using System;
using System.Reflection;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// GeneralLib.Dice が次に参照する乱数値を固定するためのヘルパー。
    /// 破棄時に乱数系列とインデックスを元に戻し、他のテストへ状態を漏らさない。
    /// </summary>
    internal sealed class FixedRandom : IDisposable
    {
        private static readonly FieldInfo HistoryField =
            typeof(GeneralLib).GetField("RndHistory", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly double[] savedHistory;
        private readonly int savedIndex;

        public FixedRandom()
        {
            savedHistory = (double[])History.Clone();
            savedIndex = GeneralLib.RndIndex;
        }

        private static double[] History => (double[])HistoryField.GetValue(null);

        /// <summary>次の Dice 呼び出しが value を使うようにする</summary>
        public void Next(double value)
        {
            History[1] = value;
            GeneralLib.RndIndex = 0;
        }

        public void Dispose()
        {
            Array.Copy(savedHistory, History, savedHistory.Length);
            GeneralLib.RndIndex = savedIndex;
        }
    }
}
