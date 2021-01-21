using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SRC.Core.Lib
{
    public class SrcCollection<V> : IList<V>, IDictionary<string, V>
    {
        private OrderedDictionary dict = new OrderedDictionary();

        public V this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public V this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public ICollection<string> Keys => throw new NotImplementedException();

        public ICollection<V> Values => throw new NotImplementedException();

        public void Add(V item)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, V value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<string, V> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(V item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, V> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(V[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<V> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(V item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, V item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(V item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, V> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out V value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, V>> IEnumerable<KeyValuePair<string, V>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
