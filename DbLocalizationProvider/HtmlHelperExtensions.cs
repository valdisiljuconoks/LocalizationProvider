using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace DbLocalizationProvider
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Translate(this HtmlHelper helper, Expression<Func<object>> model, params object[] formatArguments)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(model, CultureInfo.CurrentUICulture, formatArguments));
        }
    }
}
