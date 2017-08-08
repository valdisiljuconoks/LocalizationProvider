using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Import
{
    public static class ICollectionOfIResourceImporterExtensions
    {
        public static IResourceImporter FindByExtension(this ICollection<IResourceImporter> providers, string extension)
        {
            var lowered = extension.ToLower();
            return providers.FirstOrDefault(p => p.SupportedFileExtensions.Contains(lowered));
        }
    }
}
