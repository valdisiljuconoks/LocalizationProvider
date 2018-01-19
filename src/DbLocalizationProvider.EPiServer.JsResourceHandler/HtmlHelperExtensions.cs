using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using DbLocalizationProvider.Internal;
using ExpressionHelper = DbLocalizationProvider.Internal.ExpressionHelper;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GetTranslations<TModel>(this HtmlHelper<TModel> helper, Type containerType, string language = null, string alias = null, bool debug = false)
        {
            return GetTranslations((HtmlHelper)helper, containerType, language, alias, debug);
        }

        public static MvcHtmlString GetTranslations(this HtmlHelper helper, Type containerType, string language = null, string alias = null, bool debug = false)
        {
            if(containerType == null)
                throw new ArgumentNullException(nameof(containerType));

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(containerType);
            return GenerateScriptTag(language, alias, debug, resourceKey);
        }

        public static MvcHtmlString GetTranslations(this HtmlHelper helper, Expression<Func<object>> model, string language = null, string alias = null, bool debug = false)
        {
            if(model == null)
                throw new ArgumentNullException(nameof(model));

            var resourceKey = ExpressionHelper.GetFullMemberName(model);
            return GenerateScriptTag(language, alias, debug, resourceKey);
        }

        private static MvcHtmlString GenerateScriptTag(string language, string alias, bool debug, string resourceKey)
        {
            // if 1st request
            var mergeScript = string.Empty;
            if(HttpContext.Current?.Items["__DbLocalizationProvider_JsHandler_1stRequest"] == null)
            {
                HttpContext.Current?.Items.Add("__DbLocalizationProvider_JsHandler_1stRequest", false);
                mergeScript = $"<script src=\"/{Constants.PathBase}/{Constants.DeepMergeScriptName}\"></script>";
            }

            var url = $"/{Constants.PathBase}/{resourceKey.Replace("+", "---")}";
            var parameters = new Dictionary<string, string>();

            if(!string.IsNullOrEmpty(language))
                parameters.Add("lang", language);

            if(!string.IsNullOrEmpty(alias))
                parameters.Add("alias", alias);

            if(debug)
                parameters.Add("debug", "true");

            if(parameters.Any())
                url += "?" + ToQueryString(parameters);

            return new MvcHtmlString($"{mergeScript}<script src=\"{url}\"></script>");
        }

        private static string ToQueryString(Dictionary<string, string> parameters)
        {
            if(parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if(!parameters.Any())
                return string.Empty;

            return string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
}
