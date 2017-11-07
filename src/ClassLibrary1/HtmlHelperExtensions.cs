// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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

        public static MvcHtmlString TranslateByCulture(this HtmlHelper helper, Expression<Func<object>> model, CultureInfo culture, params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(culture == null)
                throw new ArgumentNullException(nameof(culture));

            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(model, culture, formatArguments));
        }

        public static MvcHtmlString Translate(this HtmlHelper helper, Expression<Func<object>> model, Type customAttribute, params object[] formatArguments)
        {
            return TranslateByCulture(helper, model, customAttribute, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static MvcHtmlString TranslateByCulture(this HtmlHelper helper, Expression<Func<object>> model, Type customAttribute, CultureInfo culture, params object[] formatArguments)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            if(customAttribute == null)
                throw new ArgumentNullException(nameof(customAttribute));

            if(culture == null)
                throw new ArgumentNullException(nameof(culture));

            if(!typeof(Attribute).IsAssignableFrom(customAttribute))
                throw new ArgumentException($"Given type `{customAttribute.FullName}` is not of type `System.Attribute`");

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(ExpressionHelper.GetFullMemberName(model), customAttribute);

            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(resourceKey, culture, formatArguments));
        }

        public static MvcHtmlString TranslateFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, params object[] formatArguments)
        {
            return TranslateForByCulture(html, expression, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static MvcHtmlString TranslateForByCulture<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, CultureInfo culture, params object[] formatArguments)
        {
            return new MvcHtmlString(LocalizationProvider.Current.GetStringByCulture(ExpressionHelper.GetFullMemberName(expression), culture, formatArguments));
        }

        public static MvcHtmlString Translate(this HtmlHelper helper, Enum target, params object[] formatArguments)
        {
            return TranslateByCulture(helper, target, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static MvcHtmlString TranslateByCulture(this HtmlHelper helper, Enum target, CultureInfo culture, params object[] formatArguments)
        {
            return new MvcHtmlString(target.TranslateByCulture(culture, formatArguments));
        }

        public static MvcHtmlString Translate<TModel>(this HtmlHelper<TModel> helper, Enum target, params object[] formatArguments)
        {
            return new MvcHtmlString(target.Translate(formatArguments));
        }

        public static MvcHtmlString TranslateByCulture<TModel>(this HtmlHelper<TModel> helper, Enum target, CultureInfo culture, params object[] formatArguments)
        {
            return new MvcHtmlString(target.TranslateByCulture(culture, formatArguments));
        }

        public static MvcHtmlString TranslateFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                 Expression<Func<TModel, TValue>> expression,
                                                                 Type customAttribute,
                                                                 params object[] formatArguments)
        {
            return TranslateForByCulture(html, expression, customAttribute, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static MvcHtmlString TranslateForByCulture<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                 Expression<Func<TModel, TValue>> expression,
                                                                 Type customAttribute,
                                                                 CultureInfo culture,
                                                                 params object[] formatArguments)
        {
            if(customAttribute == null)
                throw new ArgumentNullException(nameof(customAttribute));

            if(culture == null)
                throw new ArgumentNullException(nameof(culture));

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
                                                                                     culture,
                                                                                     formatArguments));
        }
    }
}
