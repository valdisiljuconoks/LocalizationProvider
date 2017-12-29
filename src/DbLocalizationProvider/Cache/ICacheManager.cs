// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
        void Insert(string key, object value);

        /// <summary>
        /// You should implement this method to get cached item back from the underlying storage
        /// </summary>
        /// <param name="key">Key identifier of the cached item</param>
        /// <returns>Actual value fo the cached item. Take care of casting back to proper type.</returns>
        object Get(string key);

        /// <summary>
        /// If you want to remove the cached item from the storage - this is the meethod to implement then.
        /// </summary>
        /// <param name="key">Key identifier of the cached item</param>
        void Remove(string key);

        /// <summary>
        /// Event raise is taken care by <see cref="BaseCacheManager"/>.
        /// </summary>
        event CacheEventHandler OnInsert;

        /// <summary>
        /// Event raise is taken care by <see cref="BaseCacheManager"/>.
        /// </summary>
        event CacheEventHandler OnRemove;
    }
}
