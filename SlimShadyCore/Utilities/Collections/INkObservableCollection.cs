using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimShadyCore.Utilities.Collections
{
    public interface INkObservableCollection<T> : ICollection<T>, INotifyCollectionChanged, IObservableEnumerable<T>
    {
    }
}
