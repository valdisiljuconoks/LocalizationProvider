namespace DbLocalizationProvider.Abstractions
{
    /// <summary>
    /// If you wanna provide additional translations Norwegian (`en`) - use this attribute.
    /// </summary>
    public class TranslationForEnglishAttribute : TranslationForCultureAttribute
    {
        /// <summary>
        /// Initiates new instance of attribute with `en` culture.
        /// </summary>
        /// <param name="translation"></param>
        public TranslationForEnglishAttribute(string translation) : base(translation, "en") { }
    }
}
