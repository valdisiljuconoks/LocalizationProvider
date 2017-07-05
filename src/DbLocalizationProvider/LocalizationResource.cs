using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace DbLocalizationProvider
{
    [DebuggerDisplay("Key: {" + nameof(ResourceKey) + "}")]
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
        [StringLength(1700)]
        [Column(TypeName = "VARCHAR")]
        [Index(IsUnique = true)]
        public string ResourceKey { get; set; }

        public DateTime ModificationDate { get; set; }

        public string Author { get; set; }

        public bool FromCode { get; set; }

        public bool? IsModified { get; set; }

        public bool? IsHidden { get; set; }

        public ICollection<LocalizationResourceTranslation> Translations { get; set; }

        public static LocalizationResource CreateNonExisting(string key)
        {
            return new LocalizationResource(key) { Translations = null };
        }
    }
}
