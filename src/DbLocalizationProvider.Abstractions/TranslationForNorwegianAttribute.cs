namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// If you wanna provide additional translations Norwegian (`no`) - use this attribute.
/// </summary>
public class TranslationForNorwegianAttribute : TranslationForCultureAttribute
{
    /// <summary>
    /// Initiates new instance of attribute with `no` culture.
    /// </summary>
    /// <param name="translation"></param>
    public TranslationForNorwegianAttribute(string translation) : base(translation, "no") { }
}
