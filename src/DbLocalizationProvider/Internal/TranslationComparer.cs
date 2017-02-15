using System.Collections.Generic;

namespace DbLocalizationProvider.Internal
{
    internal class TranslationComparer : IEqualityComparer<LocalizationResourceTranslation>
    {
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

            return string.Equals(x.Language, y.Language) && string.Equals(x.Value, y.Value);
        }

        public int GetHashCode(LocalizationResourceTranslation obj)
        {
            unchecked
            {
                return ((obj.Language?.GetHashCode() ?? 0) * 397) ^ (obj.Value?.GetHashCode() ?? 0);
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
