using System;

namespace DbLocalizationProvider
{
    public class TranslationForCultureAttribute : Attribute
    {
        public TranslationForCultureAttribute(string translation, string culture)
        {
            Translation = translation;
            Culture = culture;
        }

        public string Translation { get; }

        public string Culture { get; }
    }
}
