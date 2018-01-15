using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.AdminUI
{
    public class DirtyList<T> : ICollection<T>, IDirtyList
    {
        private readonly List<T> _actuaList = new List<T>();

        public DirtyList(params T[] items)
        {
            if(items != null)
                _actuaList.AddRange(items.ToList());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _actuaList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _actuaList.Add(item);
            IsDirty = true;
        }

        public void Clear()
        {
            _actuaList.Clear();
            IsDirty = true;
        }

        public bool Contains(T item)
        {
            return _actuaList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _actuaList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _actuaList.Remove(item);
        }

        public int Count => _actuaList.Count;

        public bool IsReadOnly => false;

        public bool IsDirty { get; private set; }
    }
}
