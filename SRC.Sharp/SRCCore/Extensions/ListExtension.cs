
using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class ListExtension
    {
        // VB には無い SRC# の補助（ReDim Preserve で配列を伸ばして足す処理の代わり）。appends は呼んだときに読み切る
        public static IEnumerable<T> AppendRange<T>(this IEnumerable<T> list, IEnumerable<T> appends)
        {
            foreach (var a in appends)
            {
                list = list.Append(a);
            }
            return list;
        }

        public static IList<T> RemoveItem<T>(this IList<T> list, Func<T, bool> predicate)
        {
            list.Where(predicate).ToList().ForEach(item => list.Remove(item));
            return list;
        }

        public static T Dice<T>(this IList<T> list)
        {
            return list[GeneralLib.Dice(list.Count) - 1];
        }

        public static List<T> CloneList<T>(this IList<T> list)
        {
            return list.ToList();
        }

        public static T SafeRefOneOffset<T>(this IList<T> list, int index)
        {
            var i = index - 1;
            if (0 <= i && list.Count > i)
            {
                return list[i];
            }
            else
            {
                return default(T);
            }
        }

        public static T SafeRefZeroOffset<T>(this IList<T> list, int index)
        {
            var i = index;
            if (0 <= i && list.Count > i)
            {
                return list[i];
            }
            else
            {
                return default(T);
            }
        }
    }
}
