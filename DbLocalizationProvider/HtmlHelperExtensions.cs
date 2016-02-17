using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Translate(this HtmlHelper helper, Expression<Func<object>> model, params object[] formatArguments)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();

            return new MvcHtmlString(localizationService.GetString(model, formatArguments));
        }
    }
}
