namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnitIdToproducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "UnitId", c => c.Int());
            CreateIndex("dbo.Products", "UnitId");
            AddForeignKey("dbo.Products", "UnitId", "dbo.Units", "UnitId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "UnitId", "dbo.Units");
            DropIndex("dbo.Products", new[] { "UnitId" });
            DropColumn("dbo.Products", "UnitId");
        }
    }
}
