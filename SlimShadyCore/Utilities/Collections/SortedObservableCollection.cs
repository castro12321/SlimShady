using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SlimShadyCore.Utilities.Collections
{
    public class SortedObservableCollection<T> : IObservableEnumerable<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IObservableEnumerable<T> backingCollection;
        private readonly IComparer<T> comparer;
        private IEnumerable<T> sorted = new List<T>();

        public SortedObservableCollection(IObservableEnumerable<T> collection)
            : this(collection, Comparer<T>.Default)
        {
        }

        public SortedObservableCollection(IObservableEnumerable<T> backingCollection, IComparer<T> comparer)
        {
            this.backingCollection = backingCollection;
            this.comparer = comparer;
            SortCollection();

            backingCollection.CollectionChanged += Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SortCollection();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void SortCollection()
        {
            sorted = backingCollection.OrderBy(elem => elem, comparer);
        }

        public IEnumerator<T> GetEnumerator() => sorted.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => sorted.GetEnumerator();

        public override string ToString() => string.Join(", ", this);
    }
}