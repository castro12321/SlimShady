using System.Collections.Generic;
using System.Linq;

namespace SlimShadyCore.Utilities
{
    public class NkComponentEntity<T>
    {
        private List<T> components = new List<T>();

        protected void AddComponent(T component)
        {
            components.Add(component);
        }

        public bool HasComponent<X>() where X : T
        {
            return components.Any(c => c is X);
        }

        public X GetSingleComponent<X>() where X : T
        {
            return (X)components
                .Where(c => c is X)
                .Single();
        }

        public IEnumerable<T> GetComponents()
        {
            return components;
        }

        public IEnumerable<X> GetComponents<X>() where X : T
        {
            return components
                .Where(c => c is X)
                .Select(c => (X)c);
        }
    }
}
