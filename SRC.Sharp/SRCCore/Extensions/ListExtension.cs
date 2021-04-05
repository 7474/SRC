
using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class ListExtension
    {
        public static void RemoveItem<T>(this IList<T> list, Func<T, bool> predicate)
        {
            var item = list.FirstOrDefault(predicate);
            if (item != null)
            {
                list.Remove(item);
            }
        }

        public static T Dice<T>(this IList<T> list)
        {
            return list[GeneralLib.Dice(list.Count) - 1];
        }
    }
}
