using System;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    internal class CacheKeyHelper
    {
        private static readonly string _separator = "_|_";

        public static string GenerateKey(string filename, string language, bool isDebugMode)
        {
            return $"{filename}{_separator}{language}__{(isDebugMode ? "debug" : "release")}";
        }

        public static string GetContainerName(string key)
        {
            if(key == null)
                throw new ArgumentNullException(nameof(key));

            return !key.Contains(_separator) ? null : key.Substring(0, key.IndexOf(_separator, StringComparison.Ordinal));
        }
    }
}
