// Copyright (c) 2019 Valdis Iljuconoks.
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Queries;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.Json
{
    public class JsonConverter
    {
        public JObject GetJson(string resourceClassName, bool camelCase = false)
        {
            return GetJson(resourceClassName, CultureInfo.CurrentUICulture.Name, camelCase);
        }

        public JObject GetJson(string resourceClassName, string languageName, bool camelCase = false)
        {
            var resources = new GetAllResources.Query().Execute();
            var filteredResources = resources.Where(r => r.ResourceKey.StartsWith(resourceClassName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return Convert(filteredResources, languageName, ConfigurationContext.Current.EnableInvariantCultureFallback, camelCase);
        }

        internal JObject Convert(ICollection<LocalizationResource> resources, string language, bool invariantCultureFallback, bool camelCase)
        {
            var result = new JObject();

            foreach(var resource in resources)
            {
                // we need to process key names and supported nested classes with "+" symbols in keys -> so we replace those with dots to have proper object nesting on client side
                var key = resource.ResourceKey.Replace("+", ".");
                if(!key.Contains("."))
                    continue;

                if(!resource.Translations.ExistsLanguage(language) && !invariantCultureFallback)
                    continue;

                var segments = key.Split(new[] { "." }, StringSplitOptions.None).Select(k => camelCase ? CamelCase(k) : k).ToList();
                var translation = resource.Translations.ByLanguage(language, invariantCultureFallback);

                Aggregate(result,
                          segments,
                          (e, segment) =>
                          {
                              if(e[segment] == null)
                                  e[segment] = new JObject();

                              return e[segment] as JObject;
                          },
                          (o, s) => { o[s] = translation; });
            }

            return result;
        }

        private static void Aggregate(JObject seed, ICollection<string> segments, Func<JObject, string, JObject> act, Action<JObject, string> last)
        {
            if(segments == null || !segments.Any())
                return;

            var lastElement = segments.Last();
            var seqWithNoLast = segments.Take(segments.Count - 1);
            var s = seqWithNoLast.Aggregate(seed, act);

            last(s, lastElement);
        }

        private static string CamelCase(string that)
        {
            if(that.Length > 1)
                return that.Substring(0, 1).ToLower() + that.Substring(1);

            return that.ToLower();
        }
    }
}
