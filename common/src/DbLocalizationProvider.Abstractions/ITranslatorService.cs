using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Translator base interface.
/// </summary>
public interface ITranslatorService
{
    /// <summary>
    /// Translates text from one language to another.
    /// </summary>
    /// <param name="inputText">Text to translate.</param>
    /// <param name="targetLanguage">Target language to translate text to.</param>
    /// <param name="sourceLanguage">Source language to translate text from.</param>
    /// <returns>Translated text if all is good; otherwise <c>null</c>.</returns>
    Task<TranslationResult> TranslateAsync(string inputText, CultureInfo targetLanguage, CultureInfo sourceLanguage);

    /// <summary>
    /// Translates multiple texts from one language to another in a single batch.
    /// </summary>
    /// <param name="inputTexts">Texts to translate.</param>
    /// <param name="targetLanguage">Target language to translate texts to.</param>
    /// <param name="sourceLanguage">Source language to translate texts from.</param>
    /// <returns>One <see cref="TranslationResult" /> per input text, in the same order as <paramref name="inputTexts" />.</returns>
    Task<IReadOnlyList<TranslationResult>> TranslateAsync(
        IReadOnlyList<string> inputTexts,
        CultureInfo targetLanguage,
        CultureInfo sourceLanguage);
}
