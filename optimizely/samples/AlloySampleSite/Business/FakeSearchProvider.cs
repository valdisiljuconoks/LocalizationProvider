//This file should be removed at the end of the story. CMS-13820

using EPiServer;
using EPiServer.Cms.Shell.Search;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Search;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace AlloySampleSite.Business
{
    public class FakeSearchProvider : ContentSearchProviderBase<PageData, PageType>
    {
        private readonly IContentLoader _contentLoader;

        public override string Area => "CMS/pages";

        public override string Category => "Fake provider";

        public override IEnumerable<SearchResult> Search(Query query)
        {
            var searchPhrase = query.SearchQuery.ToLowerInvariant();

            var contentReferences = _contentLoader.GetDescendents(ContentReference.StartPage);
            var result = contentReferences.Select(x => _contentLoader.Get<IContent>(x))
                .OfType<PageData>()
                .Where(x => x.Name.ToLowerInvariant().StartsWith(searchPhrase)).Select(CreateSearchResult);
            return result;
        }

        protected override string IconCssClass => "epi-resourceIcon epi-resourceIcon-page";

        public FakeSearchProvider(LocalizationService localizationService,
            ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository<PageType> contentTypeRepository,
            EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> currentSiteDefinition,
            IContentLanguageAccessor languageResolver, UrlResolver urlResolver, ITemplateResolver templateResolver,
            UIDescriptorRegistry uiDescriptorRegistry, IContentLoader contentLoader) : base(localizationService, siteDefinitionResolver,
            contentTypeRepository, editUrlResolver, currentSiteDefinition, languageResolver, urlResolver,
            templateResolver, uiDescriptorRegistry)
        {
            _contentLoader = contentLoader;
        }
    }

    [InitializableModule]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            //Implementations for custom interfaces can be registered here.

            context.ConfigurationComplete += (o, e) =>
            {
                context.Services.AddTransient<ISearchProvider, FakeSearchProvider>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
