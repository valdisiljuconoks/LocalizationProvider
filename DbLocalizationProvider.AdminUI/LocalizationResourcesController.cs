using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DbLocalizationProvider.Export;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.PlugIn;
using Newtonsoft.Json;

namespace DbLocalizationProvider.AdminUI
{
    [GuiPlugIn(DisplayName = "Localization Resources", UrlFromModuleFolder = "", Area = PlugInArea.AdminMenu)]
    public class LocalizationResourcesController : Controller
    {
        private readonly ILanguageBranchRepository _languageRepository;
        private readonly string _cookieName = ".DbLocalizationProvider-SelectedLanguages";

        public LocalizationResourcesController(ILanguageBranchRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public ActionResult Index()
        {
            var languages = _languageRepository.ListEnabled().Select(l => new CultureInfo(l.LanguageID)).ToList();
            var allResources = GetAllStrings();

            return View(new LocalizationResourceViewModel(allResources, languages, GetSelectedLanguages()));
        }

        [HttpPost]
        public ActionResult Update([Bind(Prefix = "pk")] string resourceKey,
                                   [Bind(Prefix = "value")] string newValue,
                                   [Bind(Prefix = "name")] string language)
        {
            using (var db = new LanguageEntities("EPiServerDB"))
            {
                var resource = db.LocalizationResources.Include(r => r.Translations).FirstOrDefault(r => r.ResourceKey == resourceKey);

                if (resource == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Resource not found");
                }

                var translation = resource.Translations.FirstOrDefault(t => t.Language == language);

                if (translation != null)
                {
                    // update existing translation
                    translation.Value = newValue;
                    db.SaveChanges();
                }
                else
                {
                    var newTranslation = new LocalizationResourceTranslation
                    {
                        Value = newValue,
                        Language = language,
                        ResourceId = resource.Id
                    };

                    db.LocalizationResourceTranslations.Add(newTranslation);
                    db.SaveChanges();
                }
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult UpdateLanguages(string[] languages)
        {
            // issue cookie to store selected languages
            WriteSelectedLanguages(languages);

            return RedirectToAction("Index");
        }

        public FileResult ExportResources()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var serializer = new JsonDataSerializer();

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                var resources = db.LocalizationResources.Include(r => r.Translations).OrderByDescending(r => r.ResourceKey);
                writer.Write(serializer.Serialize(resources));
            }

            writer.Flush();
            stream.Position = 0;

            return File(stream, "application/json", $"localization-resources-{DateTime.Now.ToString("yyyyMMdd")}.json");
        }

        public ViewResult ImportResources()
        {
            return View("ImportResources", new ImportResourcesViewModel());
        }

        [HttpPost]
        public ViewResult ImportResources(bool? importOnlyNewContent, HttpPostedFileBase importFile)
        {
            return View("ImportResources", new ImportResourcesViewModel());
        }

        private IEnumerable<string> GetSelectedLanguages()
        {
            var cookie = Request.Cookies[_cookieName];
            return cookie?.Value?.Split(new[]
            {
                "|"
            },
                                        StringSplitOptions.RemoveEmptyEntries);
        }

        private List<KeyValuePair<string, List<ResourceItem>>> GetAllStrings()
        {
            var result = new List<KeyValuePair<string, List<ResourceItem>>>();

            using (var db = new LanguageEntities("EPiServerDB"))
            {
                var resources = db.LocalizationResources.Include(r => r.Translations).OrderByDescending(r => r.ModificationDate);

                foreach (var resource in resources)
                {
                    result.Add(new KeyValuePair<string, List<ResourceItem>>(
                                   resource.ResourceKey,
                                   resource.Translations.Select(t =>
                                                                new ResourceItem(resource.ResourceKey,
                                                                                 t.Value,
                                                                                 new CultureInfo(t.Language))).ToList()));
                }
            }

            return result;
        }

        private void WriteSelectedLanguages(IEnumerable<string> languages)
        {
            var cookie = new HttpCookie(_cookieName,
                                        string.Join("|",
                                                    languages ?? new[]
                                                    {
                                                        string.Empty
                                                    }))
            {
                HttpOnly = true
            };
            Response.Cookies.Add(cookie);
        }
    }
}
