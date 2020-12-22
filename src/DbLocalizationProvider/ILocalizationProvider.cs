using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Localization Provider interface. Dragon hides under the hood.
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Gets translation for the resource with specific key.
        /// </summary>
        /// <param name="resourceKey">Key of the resource to look translation for.</param>
        /// <returns>Translation for the resource with specific key.</returns>
        /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
        string GetString(string resourceKey);

        /// <summary>
        /// Gets translation for the resource with specific key.
        /// </summary>
        /// <param name="resourceKey">Key of the resource to look translation for.</param>
        /// <param name="culture">
        /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        /// then specify that language here.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        string GetString(string resourceKey, CultureInfo culture);

        /// <summary>
        /// Gets translation for the resource (reference to the resource is specified as lambda expression).
        /// </summary>
        /// <param name="resource">Lambda expression for the resource.</param>
        /// <param name="formatArguments">
        /// If you have placeholders in translation to replace to - use this argument to specify
        /// those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        /// <remarks><see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
        string GetString(Expression<Func<object>> resource, params object[] formatArguments);

        /// <summary>
        /// Gets translation for the resource (reference to the resource is specified as lambda expression).
        /// </summary>
        /// <param name="resource">Lambda expression for the resource.</param>
        /// <param name="culture">
        /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        /// then specific that language here.
        /// </param>
        /// <param name="formatArguments">
        /// If you have placeholders in translation to replace to - use this argument to specify those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        string GetString(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments);

        /// <summary>
        /// Gets translation for the resource (reference to the resource is specified as lambda expression).
        /// </summary>
        /// <param name="resource">Lambda expression for the resource.</param>
        /// <param name="culture">
        /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        /// then specifiy that language here.
        /// </param>
        /// <param name="formatArguments">
        /// If you have placeholders in translation to replace to - use this argument to specify
        /// those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        string GetStringByCulture(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments);

        /// <summary>
        /// Gets translation for the resource with specific key.
        /// </summary>
        /// <param name="resourceKey">Key of the resource to look translation for.</param>
        /// <param name="culture">
        /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        /// then specify that language here.
        /// </param>
        /// <param name="formatArguments">
        /// If you have placeholders in translation to replace to - use this argument to specify
        /// those.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        string GetStringByCulture(string resourceKey, CultureInfo culture, params object[] formatArguments);

        /// <summary>
        /// Gets keys and translations for the specified culture.
        /// </summary>
        /// <param name="culture">
        /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
        /// then specify that language here.
        /// </param>
        /// <returns>Translation for the resource with specific key.</returns>
        IDictionary<string, string> GetStringsByCulture(CultureInfo culture);

        /// <summary>
        /// Give a type to this method and it will return instance of the type but translated
        /// </summary>
        /// <typeparam name="T">Type of the target class you want to translate</typeparam>
        /// <returns>Translated class based on <see cref="CultureInfo.CurrentUICulture" /> language</returns>
        T Translate<T>();

        /// <summary>
        /// Give a type to this method and it will return instance of the type but translated
        /// </summary>
        /// <typeparam name="T">Type of the target class you want to translate</typeparam>
        /// <param name="language">Language to use during translation</param>
        /// <returns>Translated class</returns>
        T Translate<T>(CultureInfo language);
    }
}
