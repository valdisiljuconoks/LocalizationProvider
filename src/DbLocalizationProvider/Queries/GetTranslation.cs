// Copyright © 2017 Valdis Iljuconoks.
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

using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetTranslation
    {
        public class Query : IQuery<string>
        {
            public Query(string key, CultureInfo language, bool useFallback)
            {
                Key = key;
                Language = language;
                UseFallback = useFallback;
            }

            public string Key { get; }

            public CultureInfo Language { get; }

            public bool UseFallback { get; }
        }

        public abstract class GetTranslationHandlerBase
        {
            protected virtual LocalizationResourceTranslation GetTranslationFromAvailableList(
                ICollection<LocalizationResourceTranslation> translations,
                CultureInfo language,
                bool queryUseFallback)
            {
                var foundTranslation = translations?.FindByLanguage(language);
                if(foundTranslation == null && queryUseFallback)
                    return translations?.FindByLanguage(CultureInfo.InvariantCulture);

                return foundTranslation;
            }
        }
    }
}
