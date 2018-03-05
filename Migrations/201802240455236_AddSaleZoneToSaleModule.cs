namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSaleZoneToSaleModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SaleZones",
                c => new
                    {
                        SaleZoneId = c.Int(nullable: false, identity: true),
                        ZoneManagerId = c.Int(nullable: false),
                        SaleZoneName = c.String(nullable: false, maxLength: 145),
                        SaleZoneDescription = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.SaleZoneId)
                .ForeignKey("dbo.ZoneManagers", t => t.ZoneManagerId, cascadeDelete: true)
                .Index(t => t.ZoneManagerId);
            
            CreateTable(
                "dbo.ZoneManagers",
                c => new
                    {
                        ZoneManagerId = c.Int(nullable: false, identity: true),
                        ZoneManagerName = c.String(nullable: false, maxLength: 145),
                        PhoneNo = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ZoneManagerId);
            
            AddColumn("dbo.Districts", "SaleZoneId", c => c.Int());
            CreateIndex("dbo.Districts", "SaleZoneId");
            AddForeignKey("dbo.Districts", "SaleZoneId", "dbo.SaleZones", "SaleZoneId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Districts", "SaleZoneId", "dbo.SaleZones");
            DropForeignKey("dbo.SaleZones", "ZoneManagerId", "dbo.ZoneManagers");
            DropIndex("dbo.SaleZones", new[] { "ZoneManagerId" });
            DropIndex("dbo.Districts", new[] { "SaleZoneId" });
            DropColumn("dbo.Districts", "SaleZoneId");
            DropTable("dbo.ZoneManagers");
            DropTable("dbo.SaleZones");
        }
    }
}
