using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using DbLocalizationProvider.EPiServer.Sample.Models.Pages;
using DbLocalizationProvider.EPiServer.Sample.Models.ViewModels;
using DbLocalizationProvider.Sync;
using EPiServer.Framework.Localization;
using EPiServer.Logging;
using EPiServer.Web.Mvc;

namespace DbLocalizationProvider.EPiServer.Sample.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
        private readonly LocalizationService _service;
        private readonly ILocalizationProvider _provider;
        private readonly ISynchronizer _synchronizer;

        public StartPageController(LocalizationService service, ILocalizationProvider provider, ISynchronizer synchronizer)
        {
            _service = service;
            _provider = provider;
            _synchronizer = synchronizer;
        }

        public ActionResult Index(StartPage currentPage, string lg)
        {
            if(!string.IsNullOrEmpty(lg))
                CultureInfo.CurrentUICulture = new CultureInfo(lg);

            LogManager.GetLogger(typeof(StartPageController)).Log(Level.Information, "Test log message");

            // register manually some of the resources
            _synchronizer.RegisterManually(new List<ManualResource>
            {
                new ManualResource("Manual.Resource.1", "English translation", new CultureInfo("en"))
            });

            var tr = _provider.GetString("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue");
            var trSv = _provider.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue", new CultureInfo("sv"));
            var trInv = _provider.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue", CultureInfo.InvariantCulture);
            var trLv = _provider.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue", new CultureInfo("lv"));

            var svc = _service.GetString("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue");
            var svcSv = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue", new CultureInfo("sv"));
            var svcInv = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue", CultureInfo.InvariantCulture);
            var svcLv = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.SecondValue", new CultureInfo("lv"));

            // this will be null
            var nullRes = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Resources.NullResource.NullProperty", new CultureInfo("lv"));

            var enumTr = SomeValuesEnum.FirstValue.Translate();
            var enumSv = SomeValuesEnum.FirstValue.TranslateByCulture(new CultureInfo("sv"));
            var enumInv = SomeValuesEnum.FirstValue.TranslateByCulture(CultureInfo.InvariantCulture);
            var enumLv = SomeValuesEnum.FirstValue.TranslateByCulture(new CultureInfo("lv"));

            var thirdEnumTr = SomeValuesEnum.ThirdOne.Translate();
            var thirdEnumSv = SomeValuesEnum.ThirdOne.TranslateByCulture(new CultureInfo("sv"));
            var thirdEnumInv = SomeValuesEnum.ThirdOne.TranslateByCulture(CultureInfo.InvariantCulture);
            var thirdEnumLv = SomeValuesEnum.ThirdOne.TranslateByCulture(new CultureInfo("lv"));

            var thirdSvc = _service.GetString("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.ThirdOne");
            var thirdSvcSv = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.ThirdOne", new CultureInfo("sv"));
            var thirdSvcInv = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.ThirdOne", CultureInfo.InvariantCulture);
            var thirdSvcLv = _service.GetStringByCulture("DbLocalizationProvider.EPiServer.Sample.Models.Pages.SomeValuesEnum.ThirdOne", new CultureInfo("lv"));

            return View(new StartPageViewModel(currentPage));
        }
    }
}
