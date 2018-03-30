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
using System.Diagnostics;
using System.Globalization;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Translation: {Translation} / Culture: {Culture}")]
    public class DiscoveredTranslation
    {
        public DiscoveredTranslation(string translation, string culture)
        {
            Translation = translation;
            Culture = culture;
        }

        public string Translation { get; internal set; }

        public string Culture { get; }

        public static List<DiscoveredTranslation> FromSingle(string translation)
        {
            var defaultCulture = new DetermineDefaultCulture.Query().Execute();
            var result = new List<DiscoveredTranslation>
            {
                // invariant translation
                new DiscoveredTranslation(translation, CultureInfo.InvariantCulture.Name)
            };

            // register additional culture if default is not set to invariant
            if(defaultCulture != string.Empty)
                result.Add(new DiscoveredTranslation(translation, defaultCulture));

            return result;
        }
    }
}
