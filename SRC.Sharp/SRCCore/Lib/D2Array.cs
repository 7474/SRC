using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Lib
{
    /// <summary>
    /// 1オフセットの2次元配列。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Src2DArray<T>
    {
        private int x;
        private int y;
        private T[,] data;
        public int X => x;
        public int Y => y;
        public IEnumerable<int> XRange => Enumerable.Range(1, X);
        public IEnumerable<int> YRange => Enumerable.Range(1, Y);

        public T this[int x, int y]
        {
            get { return data[x - 1, y - 1]; }
            set { data[x - 1, y - 1] = value; }
        }

        public Src2DArray(int x, int y)
        {
            this.x = x;
            this.y = y;
            data = new T[x, y];
        }
    }
}
