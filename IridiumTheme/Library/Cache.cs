using System.Collections.Generic;
using MediaBrowser.Library;

namespace Iridium
{
    public class Cache  
    {
        private Dictionary<string, List<Item>> _cache = new Dictionary<string, List<Item>>();

        public List<Item> GetCacheItems(string key)
        {
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }
            return null;
        }

        public void PersistCacheItems(string key, List<Item> items)
        {
            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, items);
            }
            else
            {
                _cache[key] = items;
            }
        }
    }
}