using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DbLocalizationProvider.Commands;
using DbLocalizationProvider.Export;
using DbLocalizationProvider.Import;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.AdminUI
{
    public class JsonServiceResult
    {
        public string Message { get; set; }
    }

    [AuthorizeRoles]
    public class LocalizationResourcesController : Controller
    {
        private const string _cookieName = ".DbLocalizationProvider-SelectedLanguages";

        public ActionResult Index()
        {
            return View(PrepareViewModel(false));
        }

        public ActionResult Main()
        {
            return View("Index", PrepareViewModel(true));
        }

        private LocalizationResourceViewModel PrepareViewModel(bool showMenu)
        {
            var availableLanguagesQuery = new AvailableLanguages.Query();
            var languages = availableLanguagesQuery.Execute();
            var allResources = GetAllResources();

            var user = HttpContext.User;
            var isAdmin = user.Identity.IsAuthenticated && UiConfigurationContext.Current.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            var result = new LocalizationResourceViewModel(allResources, languages, GetSelectedLanguages())
                   {
                       ShowMenu = showMenu,
                       AdminMode = isAdmin
                   };

            // build tree
            var builder = new ResourceTreeBuilder();
            var sorter = new ResourceTreeSorter();
            result.Tree = sorter.Sort(builder.BuildTree(allResources));

            return result;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create([Bind(Prefix = "pk")] string resourceKey)
        {
            try
            {
                var c = new CreateNewResource.Command(resourceKey, HttpContext.User.Identity.Name, false);
                c.Execute();

                return Json("");
            }
            catch (Exception e)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new JsonServiceResult
                            {
                                Message = e.Message
                            });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Delete([Bind(Prefix = "pk")] string resourceKey, string returnUrl)
        {
            try
            {
                var c = new DeleteResource.Command(resourceKey);
                c.Execute();

                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return Json(new JsonServiceResult
                            {
                                Message = e.Message
                            });
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Update([Bind(Prefix = "pk")] string resourceKey,
                                 [Bind(Prefix = "value")] string newValue,
                                 [Bind(Prefix = "name")] string language)
        {
            var c = new CreateOrUpdateTranslation.Command(resourceKey, new CultureInfo(language), newValue);
            c.Execute();

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

            var resources = new GetAllResources.Query().Execute();
            writer.Write(serializer.Serialize(resources));
            writer.Flush();
            stream.Position = 0;

            return File(stream, "application/json", $"localization-resources-{DateTime.Now:yyyyMMdd}.json");
        }

        [AuthorizeRoles(Mode = UiContextMode.Admin)]
        public ViewResult ImportResources(bool? showMenu)
        {
            return View("ImportResources",
                        new ImportResourcesViewModel
                        {
                            ShowMenu = showMenu ?? false
                        });
        }

        [HttpPost]
        [AuthorizeRoles(Mode = UiContextMode.Admin)]
        [ValidateInput(false)]
        public ViewResult CommitImportResources(bool? previewImport, bool? showMenu, ICollection<DetectedImportChange> changes)
        {
            var model = new ImportResourcesViewModel
            {
                ShowMenu = showMenu ?? false
            };

            try
            {
                var importer = new ResourceImporter();
                var result = importer.ImportChanges(changes.Where(c => c.Selected).ToList());

                ViewData["LocalizationProvider_ImportResult"] = string.Join("<br/>", result);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("importFailed", $"Import failed! Reason: {e.Message}");
            }

            return View("ImportResources", model);
        }

        [HttpPost]
        [AuthorizeRoles(Mode = UiContextMode.Admin)]
        [ValidateInput(false)]
        public ViewResult ImportResources(bool? previewImport, HttpPostedFileBase importFile, bool? showMenu)
        {
            var model = new ImportResourcesViewModel { ShowMenu = showMenu ?? false };
            if(importFile == null || importFile.ContentLength == 0)
            {
                return View("ImportResources", model);
            }

            var fileInfo = new FileInfo(importFile.FileName);
            if(fileInfo.Extension.ToLower() != ".json")
            {
                ModelState.AddModelError("file", "Uploaded file has different extension. Json file expected");
                return View("ImportResources", model);
            }

            var importer = new ResourceImporter();
            var serializer = new JsonDataSerializer();
            var streamReader = new StreamReader(importFile.InputStream);
            var fileContent = streamReader.ReadToEnd();

            try
            {
                var newResources = serializer.Deserialize<IEnumerable<LocalizationResource>>(fileContent);

                if (previewImport.HasValue && previewImport.Value)
                {
                    var changes = importer.DetectChanges(newResources, new GetAllResources.Query().Execute());

                    var availableLanguagesQuery = new AvailableLanguages.Query();
                    var languages = availableLanguagesQuery.Execute();

                    var previewModel = new PreviewImportResourcesViewModel(changes, showMenu ?? false, languages);

                    return View("ImportPreview", previewModel);
                }

                var result = importer.Import(newResources, previewImport ?? true);

                ViewData["LocalizationProvider_ImportResult"] = result;
            }
            catch (Exception e)
            {
                ModelState.AddModelError("importFailed", $"Import failed! Reason: {e.Message}");
            }

            return View("ImportResources", model);
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

        private List<ResourceListItem> GetAllResources()
        {
            var result = new List<ResourceListItem>();
            var resources = new GetAllResources.Query().Execute().OrderBy(r => r.ResourceKey);

            foreach (var resource in resources)
            {
                result.Add(new ResourceListItem(resource.ResourceKey,
                                                resource.Translations.Where(t => t.Language != ConfigurationContext.CultureForTranslationsFromCode)
                                                        .Select(t => new ResourceItem(resource.ResourceKey,
                                                                                      t.Value,
                                                                                      new CultureInfo(t.Language))).ToList(),
                                                !resource.FromCode,
                                                resource.IsHidden.HasValue && resource.IsHidden.Value));
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
