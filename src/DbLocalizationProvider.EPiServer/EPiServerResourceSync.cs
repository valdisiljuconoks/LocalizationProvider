using DbLocalizationProvider.Sync;
using EPiServer.Globalization;

namespace DbLocalizationProvider.EPiServer
{
    internal class EPiServerResourceSync : ResourceSynchronizer
    {
        protected override string DetermineDefaultCulture()
        {
            return ConfigurationContext.Current.DefaultResourceCulture != null
                       ? ConfigurationContext.Current.DefaultResourceCulture.Name
                       : (ContentLanguage.PreferredCulture != null ? ContentLanguage.PreferredCulture.Name : "en");
        }
    }
}
