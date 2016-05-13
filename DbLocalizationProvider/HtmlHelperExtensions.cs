using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using EPiServer.Framework.Localization;

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

            return new MvcHtmlString(LocalizationService.Current.GetString(model, formatArguments));
        }
    }
}
