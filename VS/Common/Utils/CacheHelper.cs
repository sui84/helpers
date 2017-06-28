using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace Common.Utils
{
    public class CacheHelper
    {
        public static ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }


        public static bool TryGetCache<T>(string key, ref T value)
        {
            object v = null;
            //Type t = typeof(T);
            bool hit;
            hit = TryGetCacheObject(key, ref v);
            if (hit)
                value = (T)v;
            return hit;
        }

        //public static bool TryGetCache(string key, ref bool value)
        //{
        //    return TryGetCacheStruct(key, ref value);
        //}

        //public static bool TryGetCache(string key, ref int value)
        //{
        //    return TryGetCacheStruct(key, ref value);
        //}

        public static bool TryGetCacheStruct<T>(string key, ref T value) where T : struct
        {
            object v = null;
            bool hit = TryGetCacheObject(key, ref v);
            if (hit)
                value = (T)v;
            return hit;
        }

        public static bool TryGetCacheObject(string key, ref object value)
        {
            object v = Cache.Get(key);
            bool hit = false;
            if (v == null)
                hit = false;
            else if (v == DBNull.Value)
            {
                hit = true;
                value = null;
            }
            else
            {
                hit = true;
                value = v;
            }
            //TraceHelper.Trace("Cache", string.Format("TryGetCache({0}) = {1}", key, hit));
            return hit;
        }


        public static bool ContainsCache(string key)
        {
            bool hit = Cache.Contains(key);
            //TraceHelper.Trace("Cache", string.Format("ContainsCache({0}) = {1}", key, hit));
            return hit;
        }

        public static object GetCache(string key)
        {
            object v = Cache.Get(key);
            if (v == DBNull.Value)
            {
                return null;
            }
            //TraceHelper.Trace("Cache", string.Format("GetCache({0}) = {1}", key, v == null ? "null" : v.ToString()));
            return v;
        }

        public static void SetCache(string key, object value)
        {
            SetCache(key, value, CacheItemPolicy);
        }

        public static void SetCache(string key, object value, CacheItemPolicy policy)
        {
            object v = value;
            if (value == null)
                v = DBNull.Value;
            Cache.Set(key, v, policy);
            //TraceHelper.Trace("Cache", string.Format("SetCache({0}) = {1}", key, value == null ? "null" : value.ToString()));
        }

        public static CacheItemPolicy CacheItemPolicy
        {
            get
            {
                int CacheSlidingExpirationInMins = 30;
                int CacheAbsoluteExpirationInMins = 30;
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = new TimeSpan(0, CacheSlidingExpirationInMins, 0);
                return policy;
            }
        }

        public static CacheItemPolicy AbsoluteCacheItemPolicy
        {
            get
            {
                int CacheAbsoluteExpirationInMins = 30;
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(CacheAbsoluteExpirationInMins);
                return policy;
            }
        }

        public static void ClearCacheByPrefix(string prefix)
        {
            List<string> keys = new List<string>();
            foreach (var c in Cache)
            {
                if (c.Key.StartsWith(prefix))
                {
                    keys.Add(c.Key);
                }
            }
            int count = keys.Count;
            foreach (var key in keys)
            {
                Cache.Remove(key);
            }
            //TraceHelper.Trace("Cache", string.Format("ClearCacheByPrefix({0}) = {1}", prefix, count));
        }
    }
}
