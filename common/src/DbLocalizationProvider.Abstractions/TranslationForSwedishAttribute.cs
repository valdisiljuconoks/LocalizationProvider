namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// If you wanna provide additional translations Norwegian (`sv`) - use this attribute.
/// </summary>
public class TranslationForSwedishAttribute : TranslationForCultureAttribute
{
    /// <summary>
    /// Initiates new instance of attribute with `sv` culture.
    /// </summary>
    /// <param name="translation"></param>
    public TranslationForSwedishAttribute(string translation) : base(translation, "sv") { }
}
