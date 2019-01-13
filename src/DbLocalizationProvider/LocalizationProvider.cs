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
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Json;
using DbLocalizationProvider.Queries;
using Newtonsoft.Json;

namespace DbLocalizationProvider
{
    /// <summary>
    ///     Main class to use when resource translation is needed.
    /// </summary>
    public class LocalizationProvider
    {
        private static Lazy<LocalizationProvider> _instance =
            new Lazy<LocalizationProvider>(() =>
                                               new LocalizationProvider(ConfigurationContext.Current.EnableInvariantCultureFallback));

        private readonly bool _fallbackEnabled;

        public LocalizationProvider(bool fallbackEnabled)
        {
            _fallbackEnabled = fallbackEnabled;
        }

        /// <summary>
        ///     Access to current instance of the provider. This property can be used in IoC containers to support dependency
        ///     injection.
        /// </summary>
        public static LocalizationProvider Current
        {
            get => _instance.Value;
            set
            {
                _instance = new Lazy<LocalizationProvider>(() => value);
            }
        }

        internal static void Initialize()
        {
            Current = new LocalizationProvider(ConfigurationContext.Current.EnableInvariantCultureFallback);
        }

        /// <summary>
        ///     Gets translation for the resource with specific key.
        /// </summary>
        /// <param name="resourceKey">Key of the resource to look translation for.</param>
        /// <returns>Translation for the resource with specific key.</returns>
        /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
        public virtual string GetString(string resourceKey)
        {
            return GetString(resourceKey, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        ///     Gets translation for the resource with specific key.
        /// </summary>
        /// <param name="resourceKey">Key of the resource to look translation for.</param>
        /// <param name="culture">
        ///     If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        ///     then specifiy that language here.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        public virtual string GetString(string resourceKey, CultureInfo culture)
        {
            return GetStringByCulture(resourceKey, culture);
        }

        /// <summary>
        ///     Gets translation for the resource (reference to the resource is specified as lambda expression).
        /// </summary>
        /// <param name="resource">Lambda expression for the resource.</param>
        /// <param name="formatArguments">
        ///     If you have placeholders in translation to replace to - use this argument to specify
        ///     those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
        public virtual string GetString(Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(resource, CultureInfo.CurrentUICulture, formatArguments);
        }

        /// <summary>
        ///     Gets translation for the resource (reference to the resource is specified as lambda expression).
        /// </summary>
        /// <param name="resource">Lambda expression for the resource.</param>
        /// <param name="culture">
        ///     If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        ///     then specifiy that language here.
        /// </param>
        /// <param name="formatArguments">
        ///     If you have placeholders in translation to replace to - use this argument to specify
        ///     those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        public virtual string GetStringByCulture(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            if(resource == null)
                throw new ArgumentNullException(nameof(resource));

            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            return GetStringByCulture(resourceKey, culture, formatArguments);
        }

        /// <summary>
        ///     Gets translation for the resource with specific key.
        /// </summary>
        /// <param name="resourceKey">Key of the resource to look translation for.</param>
        /// <param name="culture">
        ///     If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        ///     then specify that language here.
        /// </param>
        /// <param name="formatArguments">
        ///     If you have placeholders in translation to replace to - use this argument to specify
        ///     those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        public virtual string GetStringByCulture(string resourceKey, CultureInfo culture, params object[] formatArguments)
        {
            if(string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentNullException(nameof(resourceKey));

            if(culture == null)
                throw new ArgumentNullException(nameof(culture));

            var q = new GetTranslation.Query(resourceKey, culture, _fallbackEnabled);
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

        /// <summary>
        /// Give a type to this method and it will return instance of the type but translated
        /// </summary>
        /// <typeparam name="T">Type of the target class you want to translate</typeparam>
        /// <returns>Translated class based on <see cref="CultureInfo.CurrentUICulture"/> language</returns>
        public T Translate<T>()
        {
            return Translate<T>(CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Give a type to this method and it will return instance of the type but translated
        /// </summary>
        /// <typeparam name="T">Type of the target class you want to translate</typeparam>
        /// <param name="language">Language to use during translation</param>
        /// <returns>Translated class</returns>
        public T Translate<T>(CultureInfo language)
        {
            var converter = new Json.JsonConverter();
            var className = typeof(T).FullName;

            var json = converter.GetJson(className, language.Name);

            // get the actual class Json representation (we need to select token through FQN of the class)
            // to supported nested classes - we need to fix a bit resource key name
            var jsonToken = json.SelectToken(className.Replace("+", "."));

            if(jsonToken == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(jsonToken.ToString(), new JsonSerializerSettings
                                                                          {
                                                                              ContractResolver = new StaticPropertyContractResolver()
                                                                          });
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
