using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
//using static Taxi.MainWindow;

namespace Taxi
{

    public class CacheEntry
    {
        public string Key { get; set; } = "";
        public JsonElement Value { get; set; }
        public DateTimeOffset? ExpiresAt {  get; set; }
    }
    public class WpfMemoryCache
    {
        private readonly ConcurrentDictionary<string, JsonElement> _cache = new ConcurrentDictionary<string, JsonElement>();
        private readonly ConcurrentDictionary<string, DateTimeOffset?> _expiry = new ConcurrentDictionary<string, DateTimeOffset?>();
        //private static readonly JsonSerializerOptions Opts = new JsonSerializerOptions();
        private readonly string _filePath = "geocode_cache.json";


        public bool TryGet<TItem>(string key, out TItem item)
        {
            item = default(TItem);
            JsonElement json;
            if (!_cache.TryGetValue(key, out json)) return false;
            DateTimeOffset? exp;
            if (_expiry.TryGetValue(key, out exp) && exp.HasValue && exp.Value <= DateTimeOffset.UtcNow)
            {
                Remove(key);
                return false;
            }

            item = JsonSerializer.Deserialize<TItem>(json);
            return true;
        } 

        public void Remove(string key)
        {
            _cache.TryRemove(key, out _);
        }

        public void Set<TItem>(string key,  TItem value, TimeSpan? ttl = null)
        {
            _cache[key] = JsonSerializer.SerializeToElement(value);
            _expiry[key] = ttl is null ? null : DateTimeOffset.UtcNow + ttl;
        }

        public async Task SaveAsync()
        {
            var now = DateTimeOffset.UtcNow;

            var snapshot = _cache.Where(kv => !_expiry.TryGetValue(kv.Key, out var e) || e is null || e > now).Select(kv => 
                new CacheEntry
                {
                    Key = kv.Key,
                    Value = kv.Value,
                    ExpiresAt = _expiry.TryGetValue(kv.Key, out var e) ? e : null,
                }
            ).ToList();

            var getPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Directory.CreateDirectory(getPath);
            //var tmp = _filePath + ".tmp";
            string jsonString = JsonSerializer.Serialize(snapshot);
            File.WriteAllText(_filePath, jsonString);
        }

        public async Task LoadAsync()
        {
            if (!File.Exists(_filePath)) return;
            var now = DateTimeOffset.UtcNow;
            List<CacheEntry> items;
            using (var fs = File.OpenRead(_filePath))
            {
                items = await JsonSerializer.DeserializeAsync<List<CacheEntry>>(fs);
            }
            foreach (var item in items)
            {
                if (item.ExpiresAt <= now) continue;
                var value = item.Value;
                _cache[item.Key] = value;
                _expiry[item.Key] = item.ExpiresAt;
            }
        }
    }

    public class AppCache
    {
        private static WpfMemoryCache _cache = new WpfMemoryCache();
        public static WpfMemoryCache Cache { get { return _cache; } }
    }
}
