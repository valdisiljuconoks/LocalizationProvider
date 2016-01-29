using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider
{
    public class LocalizationResource
    {
        public LocalizationResource()
        {
            Translations = new List<LocalizationResourceTranslation>();
        }

        [Key]
        public int Id { get; set; }

        public string ResourceKey { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Author { get; set; }

        public ICollection<LocalizationResourceTranslation> Translations { get; set; }
    }
}
