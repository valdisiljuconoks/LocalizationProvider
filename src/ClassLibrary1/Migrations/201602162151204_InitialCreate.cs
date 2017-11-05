namespace DbLocalizationProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocalizationResources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceKey = c.String(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        Author = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocalizationResourceTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        Language = c.String(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LocalizationResources", t => t.ResourceId, cascadeDelete: true)
                .Index(t => t.ResourceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizationResourceTranslations", "ResourceId", "dbo.LocalizationResources");
            DropIndex("dbo.LocalizationResourceTranslations", new[] { "ResourceId" });
            DropTable("dbo.LocalizationResourceTranslations");
            DropTable("dbo.LocalizationResources");
        }
    }
}
