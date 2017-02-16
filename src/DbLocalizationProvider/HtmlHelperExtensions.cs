using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using DbLocalizationProvider.Internal;
using ExpressionHelper = DbLocalizationProvider.Internal.ExpressionHelper;

namespace DbLocalizationProvider
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Translate(this HtmlHelper helper, Expression<Func<object>> model, params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(model, CultureInfo.CurrentUICulture, formatArguments));
        }

        public static MvcHtmlString TranslateFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, params object[] formatArguments)
        {
            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(ExpressionHelper.GetFullMemberName(expression), CultureInfo.CurrentUICulture, formatArguments));
        }


        public static MvcHtmlString Translate(this HtmlHelper helper, Enum target, params object[] formatArguments)
        {
            return new MvcHtmlString(target.Translate(formatArguments));
        }

        public static MvcHtmlString Translate<TModel>(this HtmlHelper<TModel> helper, Enum target, params object[] formatArguments)
        {
            return new MvcHtmlString(target.Translate(formatArguments));
        }

        public static MvcHtmlString TranslateFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                 Expression<Func<TModel, TValue>> expression,
                                                                 Type customAttribute,
                                                                 params object[] formatArguments)
        {
            if(customAttribute == null)
                throw new ArgumentNullException(nameof(customAttribute));

            if(!typeof(Attribute).IsAssignableFrom(customAttribute))
                throw new ArgumentException($"Given type `{customAttribute.FullName}` is not of type `System.Attribute`");

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var pi = metadata.ContainerType.GetProperty(metadata.PropertyName);
            if(pi != null)
            {
                if(pi.GetCustomAttribute(customAttribute) == null)
                    return MvcHtmlString.Empty;
            }

            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(ResourceKeyBuilder.BuildResourceKey(metadata.ContainerType,
                                                                                                                         metadata.PropertyName,
                                                                                                                         customAttribute),
                                                                                     CultureInfo.CurrentUICulture,
                                                                                     formatArguments));
        }
    }
}
