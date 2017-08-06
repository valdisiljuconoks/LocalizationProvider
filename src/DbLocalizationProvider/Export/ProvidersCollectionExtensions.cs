using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Export
{
    public static class ProvidersCollectionExtensions
    {
        public static IResourceExporter FindById(this ICollection<IResourceExporter> list, string id)
        {
            if(string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            return list.Single(p => p.ProviderId == id);
        }
    }
}
