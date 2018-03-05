namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProcessModule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConversionDetails", "Quantity", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConversionDetails", "Quantity");
        }
    }
}
