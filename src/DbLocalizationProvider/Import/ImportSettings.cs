using System.Collections.Generic;

namespace DbLocalizationProvider.Import
{
    public class ImportSettings
    {
        public ICollection<IResourceFormatParser> Providers { get; } = new List<IResourceFormatParser>
                                                                   {
                                                                       new JsonResourceFormatParser()
                                                                   };
    }
}
