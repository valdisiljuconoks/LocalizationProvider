using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider.Internal
{
    internal static class ConcurrentDictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue, TArg>(this ConcurrentDictionary<TKey, TValue> dictionary,
                                                          TKey key,
                                                          TArg arg,
                                                          Func<TKey, TArg, TValue> valueFactory)
        {
            if(dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if(key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if(valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            while (true)
            {
                TValue value;
                if(dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                value = valueFactory(key, arg);

                if(dictionary.TryAdd(key, value))
                {
                    return value;
                }
            }
        }
    }
}
