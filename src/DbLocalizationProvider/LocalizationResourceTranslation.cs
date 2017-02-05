using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DbLocalizationProvider
{
    public class TranslationComparer : IEqualityComparer<LocalizationResourceTranslation>
    {
        public bool Equals(LocalizationResourceTranslation x, LocalizationResourceTranslation y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;
            if (x.GetType() != y.GetType())
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

    public class LocalizationResourceTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        [JsonIgnore]
        public LocalizationResource LocalizationResource { get; set; }

        public string Language { get; set; }

        public string Value { get; set; }
        
    }
}
