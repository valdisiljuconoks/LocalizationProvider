using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Import
{
    public static class ICollectionOfIResourceImporterExtensions
    {
        public static IResourceFormatParser FindByExtension(this ICollection<IResourceFormatParser> providers, string extension)
        {
            var lowered = extension.ToLower();
            return providers.FirstOrDefault(p => p.SupportedFileExtensions.Contains(lowered));
        }
    }
}
