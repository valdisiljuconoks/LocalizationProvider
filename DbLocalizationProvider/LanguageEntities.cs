using System.Data.Entity;
using DbLocalizationProvider.Migrations;

namespace DbLocalizationProvider
{
    internal class LanguageEntities : DbContext
    {
        public LanguageEntities() : this(ConfigurationContext.Current.ConnectionName) { }

        public LanguageEntities(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LanguageEntities, Configuration>());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            Database.Initialize(false);
        }

        public virtual DbSet<LocalizationResource> LocalizationResources { get; set; }

        public virtual DbSet<LocalizationResourceTranslation> LocalizationResourceTranslations { get; set; }
    }
}
