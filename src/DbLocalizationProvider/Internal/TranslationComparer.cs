// Copyright (c) 2019 Valdis Iljuconoks.
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

namespace DbLocalizationProvider.Internal
{
    internal class TranslationComparer : IEqualityComparer<LocalizationResourceTranslation>
    {
        private readonly bool _ignoreInvariantCulture;

        public TranslationComparer(bool ignoreInvariantCulture)
        {
            _ignoreInvariantCulture = ignoreInvariantCulture;
        }

        public bool Equals(LocalizationResourceTranslation x, LocalizationResourceTranslation y)
        {
            if(ReferenceEquals(x, y))
                return true;
            if(ReferenceEquals(x, null))
                return false;
            if(ReferenceEquals(y, null))
                return false;
            if(x.GetType() != y.GetType())
                return false;

            // invariant culture compare is ignored
            if(x.Language == string.Empty && y.Language == string.Empty && _ignoreInvariantCulture)
                return true;

            return string.Equals(x.Language, y.Language) && string.Equals(x.Value, y.Value);
        }

        public int GetHashCode(LocalizationResourceTranslation obj)
        {
            unchecked
            {
                return obj.Language?.GetHashCode() ?? 0;
            }
        }
    }

    internal class ResourceComparer : IEqualityComparer<LocalizationResource>
    {
        public bool Equals(LocalizationResource x, LocalizationResource y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;
            if (x.GetType() != y.GetType())
                return false;

            return string.Equals(x.ResourceKey, y.ResourceKey) && string.Equals(x.ResourceKey, y.ResourceKey);
        }

        public int GetHashCode(LocalizationResource obj)
        {
            unchecked
            {
                return (obj.ResourceKey.GetHashCode() * 397) ^ obj.ResourceKey.GetHashCode();
            }
        }
    }
}
