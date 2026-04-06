using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.Optimizely.Sandbox.Business;

/// <summary>
/// Provides extension methods for types intended to be used when working with page types
/// </summary>
public static class PageTypeExtensions
{
    /// <summary>
    /// Returns the definition for a specific page type
    /// </summary>
    /// <param name="pageType"></param>
    /// <returns></returns>
    public static PageType GetPageType(this Type pageType)
    {
        var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();

        return (PageType)contentTypeRepository.Load(pageType);
    }
}
