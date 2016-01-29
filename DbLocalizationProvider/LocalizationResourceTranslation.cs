using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DbLocalizationProvider
{
    public class LocalizationResourceTranslation
    {
        [Key]
        public int Id { get; set; }

        public int ResourceId { get; set; }

        [ForeignKey("ResourceId")]
        [JsonIgnore]
        public LocalizationResource LocalizationResource { get; set; }

        public string Language { get; set; }

        public string Value { get; set; }
    }
}
