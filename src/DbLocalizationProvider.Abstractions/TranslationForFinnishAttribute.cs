namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// If you wanna provide additional translations Norwegian (`fi`) - use this attribute.
/// </summary>
public class TranslationForFinnishAttribute : TranslationForCultureAttribute
{
    /// <summary>
    /// Initiates new instance of attribute with `fi` culture.
    /// </summary>
    /// <param name="translation"></param>
    public TranslationForFinnishAttribute(string translation) : base(translation, "fi") { }
}
