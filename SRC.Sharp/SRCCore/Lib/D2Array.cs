using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Lib
{
    /// <summary>
    /// 1オフセットの2次元配列。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonObject(MemberSerialization.OptIn)]
    public class Src2DArray<T>
    {
        [JsonProperty]
        private int x;
        [JsonProperty]
        private int y;
        [JsonProperty]
        private T[,] data;
        public int X => x;
        public int Y => y;
        public IEnumerable<int> XRange => Enumerable.Range(1, X);
        public IEnumerable<int> YRange => Enumerable.Range(1, Y);
        public IEnumerable<T> All => XRange.SelectMany(x => YRange.Select(y => this[x, y]));

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
