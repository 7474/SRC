using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace SRCCore.VB
{
    // VBのCollectionの代替え実装。
    // （ジェネリクスは欲しいので）
    public class SrcCollection<V> : IDictionary<string, V>
    {
        private OrderedDictionary dict;

        private static readonly StringComparer s_keyComparer = CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(
            CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase
        );

        public SrcCollection()
        {
            dict = new OrderedDictionary(s_keyComparer);
        }

        [OnDeserialized]
        private void Restore(StreamingContext context)
        {
        }

        public IList<V> List => dict.Values.Cast<V>().ToList();

        /// <summary>
        /// 1オフセット
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public V this[int index]
        {
            get => index <= 0 || index > dict.Count
                ? throw new IndexOutOfRangeException($"{index} is out of range.")
                : dict.Values.Cast<V>().Skip(index - 1).First();
            set => throw new NotSupportedException();
        }

        public V this[string key]
        {
            get => ContainsKey(key) ? (V)dict[key] : default(V);
            set => Add(value, key);
        }

        public int Count => dict.Count;

        public bool IsReadOnly => dict.IsReadOnly;

        public ICollection<string> Keys => dict.Keys.Cast<string>().ToList();

        public ICollection<V> Values => List;

        public void Add(V item)
        {
            throw new NotSupportedException();
        }

        // Obsolete して string, string での呼び出しを警告しつつ Value, Key での追加を可能にしておく。
        [Obsolete]
        public void Add(string key, string value)
        {
            throw new NotSupportedException();
        }
        public void Add(V value, string key)
        {
            Add(key, value);
        }
        public void Add(string key, V value)
        {
            dict.Add(key, value);
        }

        public void Add(KeyValuePair<string, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(V item)
        {
            return dict.Values.Cast<V>().Contains(item);
        }

        public bool Contains(KeyValuePair<string, V> item)
        {
            return ContainsKey(item.Key) && Contains(item.Value);
        }

        public bool ContainsKey(string key)
        {
            return key == null ? false : dict.Contains(key);
        }

        public void CopyTo(V[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(KeyValuePair<string, V>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<V> GetEnumerator()
        {
            return dict.Values.Cast<V>().GetEnumerator();
        }

        public int IndexOf(V item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, V item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(V item)
        {
            // XXX 本当にこんな処理になるのか？
            foreach (string key in dict.Keys.Cast<string>())
            {
                if (dict[key].Equals(item))
                {
                    return Remove(key);
                }
            }
            return false;
        }

        public bool Remove(string key)
        {
            if (ContainsKey(key))
            {
                dict.Remove(key);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<string, V> item)
        {
            return Remove(item.Key);
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        public bool TryGetValue(string key, out V value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, V>> IEnumerable<KeyValuePair<string, V>>.GetEnumerator()
        {
            return dict.Keys.Cast<string>()
                .Select(k => new KeyValuePair<string, V>(k, (V)dict[k]))
                .GetEnumerator();
        }
    }
}
