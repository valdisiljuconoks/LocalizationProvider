// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Cache
{
    /// <summary>
    /// Interface for implementing your own cache manager.
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// You should add given value to the cache under given key.
        /// </summary>
        /// <param name="key">Key identifier of the cached item</param>
        /// <param name="value">Actual value fo the cached item</param>
        /// <param name="insertIntoKnownResourceKeys">This is pretty internal stuff and should be ignored by cache implementers.</param>
        void Insert(string key, object value, bool insertIntoKnownResourceKeys);

        /// <summary>
        /// You should implement this method to get cached item back from the underlying storage
        /// </summary>
        /// <param name="key">Key identifier of the cached item</param>
        /// <returns>Actual value fo the cached item. Take care of casting back to proper type.</returns>
        object Get(string key);

        /// <summary>
        /// If you want to remove the cached item from the storage - this is the method to implement then.
        /// </summary>
        /// <param name="key">Key identifier of the cached item</param>
        void Remove(string key);

        /// <summary>
        /// Event raise is taken care by <see cref="BaseCacheManager" />.
        /// </summary>
        event CacheEventHandler OnInsert;

        /// <summary>
        /// Event raise is taken care by <see cref="BaseCacheManager" />.
        /// </summary>
        event CacheEventHandler OnRemove;
    }
}
