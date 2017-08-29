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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Sync;
using EPiServer.DataAbstraction;

namespace DbLocalizationProvider.EPiServer
{
    public class LocalizedCategoryScanner : IResourceTypeScanner
    {
        public bool ShouldScan(Type target)
        {
            return typeof(Category).IsAssignableFrom(target)
                   && target.GetCustomAttribute<LocalizedCategoryAttribute>() != null;
        }

        public string GetResourceKeyPrefix(Type target, string keyPrefix = null)
        {
            return null;
        }

        public ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix)
        {
            var translation = target.Name;

            try
            {
                var category = (Activator.CreateInstance(target) as Category);
                if(!string.IsNullOrEmpty(category?.Name))
                {
                    translation = category.Name;
                }
            }
            catch(Exception) { }


            return new List<DiscoveredResource>
                   {
                       new DiscoveredResource(null,
                                              $"/categories/category[@name=\"{target.Name}\"]/description",
                                              DiscoveredTranslation.FromSingle(translation),
                                              target.Name,
                                              target,
                                              typeof(string),
                                              true)
                   };
        }

        public ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix)
        {
            return Enumerable.Empty<DiscoveredResource>().ToList();
        }
    }
}
