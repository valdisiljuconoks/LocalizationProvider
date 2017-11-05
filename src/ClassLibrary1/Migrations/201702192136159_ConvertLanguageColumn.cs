namespace DbLocalizationProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertLanguageColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LocalizationResourceTranslations", "Language", c => c.String(maxLength: 10, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LocalizationResourceTranslations", "Language", c => c.String(nullable: false));
        }
    }
}
