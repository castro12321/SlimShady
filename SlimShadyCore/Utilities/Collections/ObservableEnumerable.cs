using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if false
namespace SlimShadyCore.Utilities.Collections
{
    public class ObservableEnumerable<T> : IObservableEnumerable<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IEnumerable<T> enumerable;

        public static void test()
        {
            ObservableCollection<T> original = new ObservableCollection<T>();

            ObservableEnumerable<T> result;
            result = new ObservableEnumerable<T>(original, original);
            result = Create(original);
            //result = Create2(oc);
        }

        public static ObservableEnumerable<T> Create<COLL_TYPE>(COLL_TYPE collection) where COLL_TYPE : IEnumerable<T>, INotifyCollectionChanged
        {
            return new ObservableEnumerable<T>(collection, collection);
        }

        /*public ObservableEnumerable(ObservableCollection<T> collection)
            : this(collection, collection)
        {}*/

        public ObservableEnumerable(IEnumerable<T> enumerable, INotifyCollectionChanged observable)
        {
            this.enumerable = enumerable;
            observable.CollectionChanged += (sender, ev) => CollectionChanged?.Invoke(this, ev);
        }

        public IEnumerator<T> GetEnumerator() => enumerable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => enumerable.GetEnumerator();
    }
}
#endif