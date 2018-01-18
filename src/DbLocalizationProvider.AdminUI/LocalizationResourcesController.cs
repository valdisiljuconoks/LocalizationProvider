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
        private bool _showInvariantCulture;
        private readonly int _maxLength;
        private const string _cookieName = ".DbLocalizationProvider-SelectedLanguages";
        private const string _viewCcookieName = ".DbLocalizationProvider-DefaultView";

        public LocalizationResourcesController()
        {
            _showInvariantCulture = UiConfigurationContext.Current.ShowInvariantCulture;
            _maxLength = UiConfigurationContext.Current.MaxResourceKeyDisplayLength;
        }

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
            var availableLanguagesQuery = new AvailableLanguages.Query { IncludeInvariant = _showInvariantCulture };
            var languages = availableLanguagesQuery.Execute();
            var allResources = GetAllResources();

            var user = HttpContext.User;
            var isAdmin = user.Identity.IsAuthenticated && UiConfigurationContext.Current.AuthorizedAdminRoles.Any(r => user.IsInRole(r));

            // cookies override default view from config
            var isTreeView = UiConfigurationContext.Current.DefaultView == ResourceListView.Tree;
            if(Request.Cookies[_viewCcookieName] != null)
            {
                isTreeView = UiConfigurationContext.Current.IsTableViewDisabled || Request.Cookies[_viewCcookieName]?.Value == "tree";
            }

            var result = new LocalizationResourceViewModel(allResources, languages, GetSelectedLanguages(), _maxLength)
                   {
                       ShowMenu = showMenu,
                       AdminMode = isAdmin,
                       IsTreeView = isTreeView,
                       IsTreeViewEnabled = !UiConfigurationContext.Current.IsTreeViewDisabled,
                       IsTableViewEnabled = !UiConfigurationContext.Current.IsTableViewDisabled
            };

            // build tree
            var builder = new ResourceTreeBuilder();
            var sorter = new ResourceTreeSorter();
            result.Tree = sorter.Sort(builder.BuildTree(allResources, ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode()));

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

        public FileResult ExportResources(string format = "json")
        {
            var exporter = ConfigurationContext.Current.Export.Providers.FindById(format);
            var resources = new GetAllResources.Query().Execute();
            var result = exporter.Export(resources.ToList(), Request.Params);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(result.SerializedData);
            writer.Flush();
            stream.Position = 0;

            return File(stream, result.FileMimeType, result.FileName);
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

        public ActionResult Tree(bool? showMenu)
        {
            var cookie = new HttpCookie(_viewCcookieName, "tree") { HttpOnly = true };
            Response.Cookies.Add(cookie);

            return RedirectToAction(showMenu.HasValue && showMenu.Value ? "Main" : "Index");
        }

        public ActionResult Table(bool? showMenu)
        {
            var cookie = new HttpCookie(_viewCcookieName, "table") { HttpOnly = true };
            Response.Cookies.Add(cookie);

            return RedirectToAction(showMenu.HasValue && showMenu.Value ? "Main" : "Index");
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
                var importer = new ResourceImportWorkflow();
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
            var potentialParser = ConfigurationContext.Current.Import.Providers.FindByExtension(fileInfo.Extension);

            if(potentialParser == null)
            {
                ModelState.AddModelError("file", $"Unknown file extension - `{fileInfo.Extension}`");
                return View("ImportResources", model);
            }

            var workflow = new ResourceImportWorkflow();
            var streamReader = new StreamReader(importFile.InputStream);
            var fileContent = streamReader.ReadToEnd();
            var allLanguages = new AvailableLanguages.Query().Execute();

            try
            {
                var parseResult = potentialParser.Parse(fileContent);
                var differentLanguages = parseResult.DetectedLanguages.Except(allLanguages);
                if(differentLanguages.Any())
                {
                    ModelState.AddModelError("file", $"Importing language `{string.Join(", ", differentLanguages.Select(c => c.Name))}` is not availabe in current EPiServer installation");
                    return View("ImportResources", model);
                }

                if (previewImport.HasValue && previewImport.Value)
                {
                    var changes = workflow.DetectChanges(parseResult.Resources, new GetAllResources.Query().Execute());
                    var changedLanguages = changes.SelectMany(c => c.ChangedLanguages).Distinct().Select(l => new CultureInfo(l));

                    var previewModel = new PreviewImportResourcesViewModel(changes, showMenu ?? false, changedLanguages);

                    return View("ImportPreview", previewModel);
                }

                var result = workflow.Import(parseResult.Resources, previewImport ?? true);
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
                                                resource.Translations
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
            var cookie = new HttpCookie(_cookieName, string.Join("|", languages ?? new[] { string.Empty }))
                         {
                             HttpOnly = true
                         };
            Response.Cookies.Add(cookie);
        }
    }
}
