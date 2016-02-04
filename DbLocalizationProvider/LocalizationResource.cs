using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider
{
    public class LocalizationResource
    {
        public LocalizationResource() : this(null) { }

        public LocalizationResource(string key)
        {
            ResourceKey = key;
            Translations = new List<LocalizationResourceTranslation>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string ResourceKey { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Author { get; set; }

        public ICollection<LocalizationResourceTranslation> Translations { get; set; }
    }
}
