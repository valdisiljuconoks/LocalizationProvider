using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace DbLocalizationProvider.MvcSample
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString HelpTextFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            //var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var helpText = helper.TranslateFor(expression, typeof(HelpTextAttribute));

            if (MvcHtmlString.IsNullOrEmpty(helpText))
            {
                return null;
            }

            return helpText;
        }
    }
}