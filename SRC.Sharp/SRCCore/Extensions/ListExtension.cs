
using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions
{
    public static class ListExtension
    {
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
            // TODO 浅いコピーでクローンする実装の確認
            return list.ToList();
        }
    }
}
