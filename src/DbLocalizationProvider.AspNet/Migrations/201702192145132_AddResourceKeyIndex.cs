namespace DbLocalizationProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResourceKeyIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LocalizationResources", new[] { "ResourceKey" });
            AlterColumn("dbo.LocalizationResources", "ResourceKey", c => c.String(nullable: false, maxLength: 1700, unicode: false));
            CreateIndex("dbo.LocalizationResources", "ResourceKey");
        }
        
        public override void Down()
        {
            DropIndex("dbo.LocalizationResources", new[] { "ResourceKey" });
            AlterColumn("dbo.LocalizationResources", "ResourceKey", c => c.String(nullable: false, maxLength: 4000, unicode: false));
            CreateIndex("dbo.LocalizationResources", "ResourceKey");
        }
    }
}
