namespace DbLocalizationProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFromCodeColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalizationResources", "FromCode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalizationResources", "FromCode");
        }
    }
}
