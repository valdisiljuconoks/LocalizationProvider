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
    /// <param name="targetLanguage">Target language code to translate text to.</param>
    /// <returns>Translated text if all is good; otherwise <c>null</c>.</returns>
    Task<string> TranslateAsync(string inputText, string targetLanguage);
}
