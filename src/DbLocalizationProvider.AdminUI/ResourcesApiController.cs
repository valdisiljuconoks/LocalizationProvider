using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using DbLocalizationProvider.AdminUI.ApiModels;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Queries;
using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI
{
    //[Authorize]
    public class ResourcesApiController : ApiController
    {
        private const string CookieName = ".DbLocalizationProvider-SelectedLanguages";

        public IHttpActionResult Get()
        {
            return Ok(PrepareViewModel());
        }

        [HttpPost]
        public IHttpActionResult Update(CreateOrUpdateTranslationRequestModel model)
        {
            var cmd = new CreateOrUpdateTranslation.Command(model.Key, new CultureInfo(model.Language), model.Translation);
            cmd.Execute();

            return Ok();
        }

        private LocalizationResourceApiModel PrepareViewModel()
        {
            var availableLanguagesQuery = new AvailableLanguages.Query { IncludeInvariant = true };
            var languages = availableLanguagesQuery.Execute();

            var getResourcesQuery = new GetAllResources.Query();
            var resources = getResourcesQuery.Execute().OrderBy(r => r.ResourceKey).ToList();

            var user = RequestContext.Principal;
            var isAdmin = false;

            if (user != null)
                isAdmin = user.Identity.IsAuthenticated && UiConfigurationContext.Current.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            return new LocalizationResourceApiModel(resources, languages) { AdminMode = isAdmin };
        }

        private IEnumerable<string> GetSelectedLanguages()
        {
            var cookie = Request.Headers.GetCookies(CookieName).FirstOrDefault();

            return cookie?[CookieName].Value?.Split(new[]
                                                     {
                                                         "|"
                                                     },
                                                     StringSplitOptions.RemoveEmptyEntries);
        }
    }

    [JsonObject]
    public class CreateOrUpdateTranslationRequestModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("newTranslation")]
        public string Translation { get; set; }
    }
}
