namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConversionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conversions", "MatricId1", c => c.Int());
            AddColumn("dbo.Conversions", "MatricId2", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Conversions", "MatricId2");
            DropColumn("dbo.Conversions", "MatricId1");
        }
    }
}
