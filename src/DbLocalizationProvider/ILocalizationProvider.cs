using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace DbLocalizationProvider;

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
    /// <remarks>By default <see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
    string GetString(string resourceKey);

    /// <summary>
    /// Gets translation for the resource with specific key.
    /// </summary>
    /// <param name="resourceKey">Key of the resource to look translation for.</param>
    /// <param name="culture">
    /// If you want to get translation for other language as <see cref="CultureInfo.CurrentUICulture" />,
    /// you can pass different language as parameter then..
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
    /// <remarks>By default <see cref="CultureInfo.CurrentUICulture" /> is used as language.</remarks>
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
    /// then specify that language here.
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
    /// Give a type to this method and it will return instance of the type but translated.
    /// </summary>
    /// <typeparam name="T">Type of the target class you want to translate.</typeparam>
    /// <returns>Translated class based on current language.</returns>
    T Translate<T>();

    /// <summary>
    /// Give a type to this method and it will return instance of the type but translated.
    /// </summary>
    /// <typeparam name="T">Type of the target class you want to translate.</typeparam>
    /// <param name="language">Language to use during translation.</param>
    /// <returns>Translated class</returns>
    T Translate<T>(CultureInfo language);

    /// <summary>
    /// Translates the specified enum with some formatting arguments (if needed).
    /// </summary>
    /// <param name="target">The enum to translate.</param>
    /// <param name="formatArguments">The format arguments.</param>
    /// <returns>Translated enum values</returns>
    string Translate(Enum target, params object[] formatArguments);

    /// <summary>
    /// Translates the specified enum with some formatting arguments (if needed).
    /// </summary>
    /// <param name="target">The enum to translate.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="formatArguments">The format arguments.</param>
    /// <returns>Translated enum values</returns>
    /// <exception cref="ArgumentNullException">
    /// target
    /// or
    /// culture
    /// </exception>
    string TranslateByCulture(Enum target, CultureInfo culture, params object[] formatArguments);

    /// <summary>
    /// This method will try to translate resource for current language and if fail will provide you with translation in
    /// <c>CultureInfo.InvariantCulture</c> regardless of what settings are configured for fallback.
    /// </summary>
    /// <param name="resource">Expression of the resource to translate.</param>
    /// <param name="formatArguments">If you need to format the message and substitute placeholders.</param>
    /// <returns>Translation for current language or in invariant language.</returns>
    string GetStringWithInvariantFallback(Expression<Func<object>> resource, params object[] formatArguments);

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <param name="type">The type to retrieve localized resources for.</param>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    IDictionary<string, string> ToDictionary(Type type);

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <typeparam name="T">The type to retrieve localized resources for.</typeparam>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    IDictionary<string, string> ToDictionary<T>();

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <typeparam name="T">The type to retrieve localized resources for.</typeparam>
    /// <param name="culture">Culture to get translations in.</param>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    IDictionary<string, string> ToDictionary<T>(CultureInfo culture);

    /// <summary>
    /// Converts a localized resource dictionary to a translated dictionary based on the specified type.
    /// </summary>
    /// <param name="type">The type to retrieve localized resources for.</param>
    /// <param name = "culture" > Culture to get translations in.</param>
    /// <returns>A dictionary containing the localized resources translated to the current culture.</returns>
    /// <exception cref="ArgumentException">Thrown when the object does not have a LocalizedResourceAttribute.</exception>
    IDictionary<string, string> ToDictionary(Type type, CultureInfo culture);
}
