using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using Common.Utils;

namespace Common.Tests
{
    public class ObjectKey
    {
        public string Name { get; set; }
        public decimal No { get; set; }

        public override bool Equals(object obj)
        {
            ObjectKey v = obj as ObjectKey;
            if (v == null) return false;

            return v.Name == this.Name && v.No == this.No;
        }

        public override int GetHashCode()
        {
            int primeNo = 31;
            return (this.Name.GetHashCode() * primeNo + this.No.GetHashCode());
        }
    }

    public class CallCacheHelper
    {
        public static bool IsExist(string name, decimal no)
        {
            ObjectKey key = new ObjectKey();
            key.Name = name;
            key.No = no;
            HashSet<ObjectKey> all = GetAllFromCache();
            if (all.Contains(key))
                return true;
            else
                return false;
        }

        public static HashSet<ObjectKey> GetAllFromCache()
        {
            string cachekey = "AllStopPayments";
            HashSet<ObjectKey> stoppaymentSet = null;
            if (!CacheHelper.TryGetCache(cachekey, ref stoppaymentSet))
            {
                stoppaymentSet = GetAllObjectKeys();
                CacheItemPolicy policy = CacheHelper.AbsoluteCacheItemPolicy;
                //policy.RemovedCallback = (arg) =>
                //{
                //    var newSet = GetAllStopPayments();
                //    CacheHelper.SetCache(cachekey, newSet, policy);
                //};
                CacheHelper.SetCache(cachekey, stoppaymentSet, policy);
            }
            return stoppaymentSet;
        }

        public static HashSet<ObjectKey> GetAllObjectKeys()
        {
            //...
            HashSet<ObjectKey> objectSet = new HashSet<ObjectKey>();
            return objectSet;
        }
    }
}
