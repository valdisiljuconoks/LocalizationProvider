using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider
{
    public interface IAvailableLanguagesProvider
    {
        IEnumerable<CultureInfo> GetAll();
    }
}
