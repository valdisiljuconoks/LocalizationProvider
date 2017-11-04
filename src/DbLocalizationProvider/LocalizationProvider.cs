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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider
{
    public class LocalizationProvider
    {
        public static LocalizationProvider Current { get; } = new LocalizationProvider();

        public virtual string GetString(string resourceKey)
        {
            return GetString(resourceKey, CultureInfo.CurrentUICulture);
        }

        public virtual string GetString(string resourceKey, CultureInfo culture)
        {
            return GetStringByCulture(resourceKey, culture);
        }

        public virtual string GetString(Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(resource, CultureInfo.CurrentUICulture);
        }

        public virtual string GetStringByCulture(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            if(resource == null)
                throw new ArgumentNullException(nameof(resource));

            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            return GetStringByCulture(resourceKey, culture, formatArguments);
        }

        public virtual string GetStringByCulture(string resourceKey, CultureInfo culture, params object[] formatArguments)
        {
            if(string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentNullException(nameof(resourceKey));

            if(culture == null)
                throw new ArgumentNullException(nameof(culture));

            var q = new GetTranslation.Query(resourceKey, culture, ConfigurationContext.Current.EnableInvariantCultureFallback);
            var resourceValue = q.Execute();

            if(resourceValue == null)
                return null;

            try
            {
                return Format(resourceValue, formatArguments);
            }
            catch(Exception)
            {
                // TODO: log
            }

            return resourceValue;
        }

        internal static string Format(string message, params object[] formatArguments)
        {
            if(formatArguments == null || !formatArguments.Any())
                return message;

            // check if first element is not scalar - format with named placeholders
            var first = formatArguments.First();
            return !first.GetType().IsSimpleType()
                       ? FormatWithAnonymousObject(message, first)
                       : string.Format(message, formatArguments);
        }

        private static string FormatWithAnonymousObject(string message, object model)
        {
            var type = model.GetType();
            if(type == typeof(string))
                return string.Format(message, model);

            var placeHolders = Regex.Matches(message, "{.*?}").Cast<Match>().Select(m => m.Value).ToList();

            if(!placeHolders.Any())
                return message;

            var placeholderMap = new Dictionary<string, object>();
            var properties = type.GetProperties();

            foreach(var placeHolder in placeHolders)
            {
                var propertyInfo = properties.FirstOrDefault(p => p.Name == placeHolder.Trim('{', '}'));

                // property found - extract value and add to the map
                var val = propertyInfo?.GetValue(model);
                if(val != null)
                    placeholderMap.Add(placeHolder, val);
            }

            return placeholderMap.Aggregate(message, (current, pair) => current.Replace(pair.Key, pair.Value.ToString()));
        }
    }
}
