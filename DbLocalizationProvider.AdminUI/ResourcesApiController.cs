using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DbLocalizationProvider.AdminUI.ApiModels;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourcesApiController : ApiController
    {
        private const string _cookieName = ".DbLocalizationProvider-SelectedLanguages";

        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, PrepareViewModel());
        }

        private LocalizationResourceApiModel PrepareViewModel()
        {
            var availableLanguagesQuery = new AvailableLanguages.Query();
            var languages = availableLanguagesQuery.Execute();

            var getResourcesQuery = new GetAllResources.Query();
            var resources = getResourcesQuery.Execute().OrderBy(r => r.ResourceKey).ToList();

            var user = RequestContext.Principal;
            var isAdmin = user.Identity.IsAuthenticated && UiConfigurationContext.Current.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            return new LocalizationResourceApiModel(resources, languages)
                   {
                       AdminMode = isAdmin
                   };
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
