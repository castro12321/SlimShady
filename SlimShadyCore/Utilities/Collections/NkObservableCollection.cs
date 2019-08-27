using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyCore.Utilities.Collections
{
    public class NkObservableCollection<T> : INkObservableCollection<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private readonly ICollection<T> wrapped;

        public int Count => wrapped.Count;
        public bool IsReadOnly => wrapped.IsReadOnly;

        public static NkObservableCollection<T> Create() => Create(new ObservableCollection<T>());
        public static NkObservableCollection<T> Create<COLL_TYPE>(COLL_TYPE collection) where COLL_TYPE : ICollection<T>, INotifyCollectionChanged
        {
            return new NkObservableCollection<T>(collection, collection);
        }

        public NkObservableCollection(ICollection<T> wrapped, INotifyCollectionChanged observable)
        {
            this.wrapped = wrapped;
            observable.CollectionChanged += (sender, ev) =>
            {
                CollectionChanged?.Invoke(this, ev);
            };
        }

        public void Add(T item) => wrapped.Add(item);
        public void Clear() => wrapped.Clear();
        public bool Contains(T item) => wrapped.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => wrapped.CopyTo(array, arrayIndex);
        public bool Remove(T item) => wrapped.Remove(item);

        public IEnumerator<T> GetEnumerator() => wrapped.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => wrapped.GetEnumerator();

        public override string ToString() => string.Join(", ", this);
    }
}
