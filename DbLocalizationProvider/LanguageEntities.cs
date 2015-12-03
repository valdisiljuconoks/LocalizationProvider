using System.Data.Entity;

namespace TechFellow.DbLocalizationProvider
{
    public class LanguageEntities : DbContext
    {
        public LanguageEntities() : base("LanguageEntities") { }

        public LanguageEntities(string connectionString) : base(connectionString) { }

        public virtual DbSet<LocalizationResource> LocalizationResources { get; set; }

        public virtual DbSet<LocalizationResourceTranslation> LocalizationResourceTranslations { get; set; }
    }
}
