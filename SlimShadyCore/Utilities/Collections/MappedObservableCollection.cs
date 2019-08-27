using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SlimShadyCore.Utilities.Collections
{
    public class MappedObservableCollection<T, M> : IObservableEnumerable<M>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IObservableEnumerable<T> backingCollection;
        private readonly Func<T, M> mapper;
        private IEnumerable<M> mapped = new List<M>();

        public MappedObservableCollection(IObservableEnumerable<T> backingCollection, Func<T, M> mapper)
        {
            this.backingCollection = backingCollection;
            this.mapper = mapper;
            MapCollection();

            backingCollection.CollectionChanged += Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MapCollection();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void MapCollection()
        {
            mapped = backingCollection.Select(elem => mapper.Invoke(elem));
        }

        public IEnumerator<M> GetEnumerator() => mapped.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => mapped.GetEnumerator();

        public override string ToString() => string.Join(", ", this);
    }
}