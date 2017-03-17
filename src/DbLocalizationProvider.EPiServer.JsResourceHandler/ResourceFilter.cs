using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    public class ResourceFilter
    {
        public ICollection<LocalizationResource> GetResourcesWithStartingKey(IEnumerable<LocalizationResource> resources, string filename)
        {
            return resources.Where(r => r.ResourceKey.StartsWith(filename, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}
