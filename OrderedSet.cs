using System.Collections;

namespace SimpleEngine
{
    public class OrderedSet<T> : ICollection<T>
    {
        private readonly IDictionary<T, int> m_Dictionary;
        private readonly IDictionary<int, T> m_RDictionary;

        public OrderedSet()
        {
            m_Dictionary = new Dictionary<T, int>();
            m_RDictionary = new Dictionary<int, T>();
        }

        public int Count => m_Dictionary.Count;

        public virtual bool IsReadOnly => m_Dictionary.IsReadOnly;

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            if (m_Dictionary.ContainsKey(item)) return false;
            var count = Count;
            m_Dictionary.Add(item, count);
            m_RDictionary.Add(count, item);
            return true;
        }

        public int IndexOf(T item)
        {
            if (m_Dictionary.TryGetValue(item, out var index)) return index;
            return -1;
        }

        public T ElementAt(int index)
        {
            if (m_RDictionary.TryGetValue(index, out var output)) return output;
            throw new IndexOutOfRangeException($"Index {index} out of range for size {Count}");
        }

        public void Clear()
        {
            m_Dictionary.Clear();
            m_RDictionary.Clear();
        }

        public bool Remove(T item)
        {
            if (m_Dictionary.ContainsKey(item)) return false;
            m_RDictionary.Remove(m_Dictionary[item]);
            m_Dictionary.Remove(item);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_Dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Dictionary.Keys.CopyTo(array, arrayIndex);
        }
    }
}
