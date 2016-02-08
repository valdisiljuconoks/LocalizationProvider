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
using DbLocalizationProvider.Import;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.PlugIn;

namespace DbLocalizationProvider.AdminUI
{
    [GuiPlugIn(DisplayName = "Localization Resources", UrlFromModuleFolder = "LocalizationResources", Area = PlugInArea.AdminMenu)]
    [Authorize]
    [AuthorizeRoles("Administrators", "LocalizationAdmins", "LocalizationEditors")]
    public class LocalizationResourcesController : Controller
    {
        private readonly string _cookieName = ".DbLocalizationProvider-SelectedLanguages";
        private readonly ILanguageBranchRepository _languageRepository;
        private readonly LocalizationResourceRepository _resourceRepository;

        public LocalizationResourcesController(ILanguageBranchRepository languageRepository, LocalizationResourceRepository resourceRepository)
        {
            _languageRepository = languageRepository;
            _resourceRepository = resourceRepository;
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
            _resourceRepository.CreateOrUpdateTranslation(resourceKey, new CultureInfo(language), newValue);

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

            var resources = _resourceRepository.GetAllResources();
            writer.Write(serializer.Serialize(resources));
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
            if (importFile == null || importFile.ContentLength == 0)
            {
                return View("ImportResources", new ImportResourcesViewModel());
            }

            var fileInfo = new FileInfo(importFile.FileName);
            if (fileInfo.Extension.ToLower() != ".json")
            {
                ModelState.AddModelError("file", "Uploaded file has different extension. Json file expected");
                return View("ImportResources", new ImportResourcesViewModel());
            }

            var importer = new ResourceImporter(_resourceRepository);
            var serializer = new JsonDataSerializer();
            var streamReader = new StreamReader(importFile.InputStream);
            var fileContent = streamReader.ReadToEnd();

            try
            {
                var newResources = serializer.Deserialize<IEnumerable<LocalizationResource>>(fileContent);
                var result = importer.Import(newResources, importOnlyNewContent ?? true);

                ViewData["LocalizationProvider_ImportResult"] = result;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("importFailed", $"Import failed! Reason: {e.Message}");
            }

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

            var resources = _resourceRepository.GetAllResources();
            foreach (var resource in resources)
            {
                result.Add(new KeyValuePair<string, List<ResourceItem>>(
                               resource.ResourceKey,
                               resource.Translations.Select(t =>
                                                            new ResourceItem(resource.ResourceKey,
                                                                             t.Value,
                                                                             new CultureInfo(t.Language))).ToList()));
            }

            return result;
        }

        private void WriteSelectedLanguages(IEnumerable<string> languages)
        {
            var cookie = new HttpCookie(_cookieName,
                                        string.Join("|", languages ?? new[] { string.Empty }))
                         {
                             HttpOnly = true
                         };
            Response.Cookies.Add(cookie);
        }
    }
}
