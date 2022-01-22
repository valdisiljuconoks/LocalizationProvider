// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

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
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            // invariant culture compare is ignored
            if (x.Language == string.Empty && y.Language == string.Empty && _ignoreInvariantCulture)
            {
                return true;
            }

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
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

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
