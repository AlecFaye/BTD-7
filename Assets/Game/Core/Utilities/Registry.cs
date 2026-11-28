using System.Collections.Generic;
using System.Linq;

namespace Assets.Game.Core.Utilities
{
    public delegate TItem SelectionStrategy<TItem>(IEnumerable<TItem> items);

    public static class Registry<TItem>
        where TItem : class
    {
        private static readonly HashSet<TItem> _items = new();

        public static IEnumerable<TItem> All => _items;

        public static bool TryAdd(TItem item)
        {
            return item != null && _items.Add(item);
        }

        public static bool Remove(TItem item)
        {
            return _items.Remove(item);
        }

        public static TItem GetFirst()
        {
            return _items.FirstOrDefault();
        }

        public static TItem Get(SelectionStrategy<TItem> strategy) => strategy(_items);
        public static TItem GetAll(SelectionStrategy<TItem> strategy) => strategy(_items);
    }
}
