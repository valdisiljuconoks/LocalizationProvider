using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using System;

namespace AlloySampleSite.Business
{
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
            var pageTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository<PageType>>();

            return pageTypeRepository.Load(pageType);
        }
    }
}
