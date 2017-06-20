using System;

namespace DbLocalizationProvider
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
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
