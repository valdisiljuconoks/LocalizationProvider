using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DbLocalizationProvider
{
    public class LocalizationResourceTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        [JsonIgnore]
        public LocalizationResource LocalizationResource { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string Language { get; set; }

        public string Value { get; set; }
        
    }
}
