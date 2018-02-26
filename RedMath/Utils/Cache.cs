using System;
using System.Collections.Generic;

namespace RedMath.Utils
{
    public class Cache<TKey>
    {
        protected interface ICacheItemBase
        {
            bool UpdateRequired { get; set; }
        }

        protected class CacheItem<TValue, TData> : ICacheItemBase
        {
            public TValue Value { get; set; }

            public bool UpdateRequired { get; set; }

            public Func<TData, TValue> UpdateFunction { get; set; }

            public CacheItem(TValue value, bool updateRequired, Func<TData, TValue> updateFunction)
            {
                Value = value;
                UpdateRequired = updateRequired;
                UpdateFunction = updateFunction;
            }
        }

        protected Dictionary<TKey, ICacheItemBase> items;

        public Cache()
        {
            items = new Dictionary<TKey, ICacheItemBase>();
        }

        public void AddCacheEntry<TValue, TData>(TKey key, TValue value, bool updateRequired, Func<TData, TValue> updateFunc)
        {
            items.Add(key, new CacheItem<TValue, TData>(value, updateRequired, updateFunc));
        }

        public void SetEntryUpdateState(TKey key, bool updateRequired)
        {
            if (items.TryGetValue(key, out ICacheItemBase item))
            {
                item.UpdateRequired = updateRequired;
            }
        }

        public void SetAllUpdateStates(bool updateRequired)
        {
            foreach (ICacheItemBase item in items.Values)
            {
                item.UpdateRequired = updateRequired;
            }
        }

        public void RemoveCacheEntry(TKey key)
        {
            items.Remove(key);
        }

        public TValue RetrieveValue<TValue, TData>(TKey key, TData data)
        {
            if (items.TryGetValue(key, out ICacheItemBase itemBase))
            {
                if (itemBase is CacheItem<TValue, TData> item)
                {
                    if (item.UpdateRequired)
                    {
                        item.Value = item.UpdateFunction(data);
                    }

                    item.UpdateRequired = false;

                    return item.Value;
                }
            }

            return default(TValue);
        }
    }
}
