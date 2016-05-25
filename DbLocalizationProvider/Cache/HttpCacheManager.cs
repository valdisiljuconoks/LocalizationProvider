using System.Web;

namespace DbLocalizationProvider.Cache
{
    public class HttpCacheManager : ICacheManager
    {
        public void Insert(string key, object value)
        {
            if(VerifyInstance())
                HttpContext.Current.Cache.Insert(key, value);
        }

        public object Get(string key)
        {
            return VerifyInstance() ? HttpContext.Current.Cache.Get(key) : null;
        }

        public void Remove(string key)
        {
            if(VerifyInstance())
                HttpContext.Current.Cache.Remove(key);
        }

        private static bool VerifyInstance()
        {
            if(HttpContext.Current == null)
                return false;

            if(HttpContext.Current.Cache == null)
                return false;

            return true;
        }
    }
}
