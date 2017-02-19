using System.Data.Entity.Migrations;

namespace DbLocalizationProvider.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<LanguageEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "DbLocalizationProvider.LanguageEntities";
        }
    }
}
