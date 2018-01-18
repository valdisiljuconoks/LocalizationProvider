// Copyright (c) 2018 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider
{
    public static class TranslationsExtensions
    {
        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return ByLanguage(translations, language.Name);
        }

        public static LocalizationResourceTranslation FindByLanguage(this ICollection<LocalizationResourceTranslation> translations, CultureInfo language)
        {
            return FindByLanguage(translations, language.Name);
        }

        public static LocalizationResourceTranslation FindByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            return translations.FirstOrDefault(t => t.Language == language);
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language, bool invariantCultureFallback)
        {
            if(translations == null)
                return string.Empty;

            if(string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(language));

            var translation = translations.FindByLanguage(language);
            return translation != null ?
                       translation.Value :
                       invariantCultureFallback ? translations.FindByLanguage(string.Empty)?.Value : string.Empty;
        }

        public static string ByLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            return ByLanguage(translations, language, ConfigurationContext.Current.EnableInvariantCultureFallback);
        }

        public static bool ExistsLanguage(this ICollection<LocalizationResourceTranslation> translations, string language)
        {
            if(string.IsNullOrEmpty(language))
                throw new ArgumentNullException(nameof(language));

            return translations?.FirstOrDefault(t => t.Language == language) != null;
        }
    }
}
