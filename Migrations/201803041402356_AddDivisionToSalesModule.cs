namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDivisionToSalesModule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SaleZones", "ZoneManagerId", "dbo.ZoneManagers");
            DropIndex("dbo.SaleZones", new[] { "ZoneManagerId" });
            CreateTable(
                "dbo.Divisions",
                c => new
                    {
                        DivisionId = c.Int(nullable: false, identity: true),
                        DivisionName = c.String(nullable: false, maxLength: 145),
                        DivisionNameBangla = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.DivisionId);
            
            AddColumn("dbo.SaleZones", "DivisionId", c => c.Int());
            AlterColumn("dbo.SaleZones", "ZoneManagerId", c => c.Int());
            CreateIndex("dbo.SaleZones", "ZoneManagerId");
            CreateIndex("dbo.SaleZones", "DivisionId");
            AddForeignKey("dbo.SaleZones", "DivisionId", "dbo.Divisions", "DivisionId");
            AddForeignKey("dbo.SaleZones", "ZoneManagerId", "dbo.ZoneManagers", "ZoneManagerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleZones", "ZoneManagerId", "dbo.ZoneManagers");
            DropForeignKey("dbo.SaleZones", "DivisionId", "dbo.Divisions");
            DropIndex("dbo.SaleZones", new[] { "DivisionId" });
            DropIndex("dbo.SaleZones", new[] { "ZoneManagerId" });
            AlterColumn("dbo.SaleZones", "ZoneManagerId", c => c.Int(nullable: false));
            DropColumn("dbo.SaleZones", "DivisionId");
            DropTable("dbo.Divisions");
            CreateIndex("dbo.SaleZones", "ZoneManagerId");
            AddForeignKey("dbo.SaleZones", "ZoneManagerId", "dbo.ZoneManagers", "ZoneManagerId", cascadeDelete: true);
        }
    }
}
