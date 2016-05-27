using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider
{
    internal class AvailableLanguagesProvider : IAvailableLanguagesProvider
    {
        public IEnumerable<CultureInfo> GetAll()
        {
            return new List<CultureInfo>
                   {
                       new CultureInfo("en")
                   };
        }
    }
}
