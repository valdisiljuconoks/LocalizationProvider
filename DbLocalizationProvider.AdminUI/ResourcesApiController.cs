using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourcesApiController : ApiController
    {
        private const string _cookieName = ".DbLocalizationProvider-SelectedLanguages";

        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, PrepareViewModel(false));
        }

        private LocalizationResourceViewModel PrepareViewModel(bool showMenu)
        {
            var availableLanguagesQuery = new AvailableLanguages.Query();
            var languages = availableLanguagesQuery.Execute();
            var allResources = GetAllResources();
            var user = RequestContext.Principal;
            var isAdmin = user.Identity.IsAuthenticated && UiConfigurationContext.Current.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            return new LocalizationResourceViewModel(allResources, languages, GetSelectedLanguages())
                   {
                       ShowMenu = showMenu,
                       AdminMode = isAdmin
                   };
        }

        private List<ResourceListItem> GetAllResources()
        {
            var result = new List<ResourceListItem>();
            var resources = new GetAllResources.Query().Execute().OrderBy(r => r.ResourceKey);

            foreach (var resource in resources)
            {
                result.Add(new ResourceListItem(
                               resource.ResourceKey,
                               resource.Translations.Select(t => new ResourceItem(resource.ResourceKey,
                                                                                  t.Value,
                                                                                  new CultureInfo(t.Language))).ToArray(),
                               !resource.FromCode));
            }

            return result;
        }

        private IEnumerable<string> GetSelectedLanguages()
        {
            var cookie = Request.Headers.GetCookies(_cookieName).FirstOrDefault();

            return cookie?[_cookieName].Value?.Split(new[]
                                                     {
                                                         "|"
                                                     },
                                                     StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
