namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProcessModuleAddOrderForm : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FinishedGoodStocks", "ProcesseLocationId", "dbo.ProcesseLocations");
            DropIndex("dbo.FinishedGoodStocks", new[] { "ProcesseLocationId" });
            AddColumn("dbo.FinishedGoodStocks", "ReceiveDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.FinishedGoodStocks", "DeliveryDate", c => c.DateTime());
            AddColumn("dbo.FinishedGoodStocks", "OrderNumber", c => c.String());
            AddColumn("dbo.FinishedGoodStocks", "BuyerName", c => c.String());
            AddColumn("dbo.Purchases", "OrderNo", c => c.String());
            AlterColumn("dbo.FinishedGoodStocks", "ProcesseLocationId", c => c.Int());
            CreateIndex("dbo.FinishedGoodStocks", "ProcesseLocationId");
            AddForeignKey("dbo.FinishedGoodStocks", "ProcesseLocationId", "dbo.ProcesseLocations", "ProcesseLocationId");
            DropColumn("dbo.FinishedGoodStocks", "Date");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FinishedGoodStocks", "Date", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.FinishedGoodStocks", "ProcesseLocationId", "dbo.ProcesseLocations");
            DropIndex("dbo.FinishedGoodStocks", new[] { "ProcesseLocationId" });
            AlterColumn("dbo.FinishedGoodStocks", "ProcesseLocationId", c => c.Int(nullable: false));
            DropColumn("dbo.Purchases", "OrderNo");
            DropColumn("dbo.FinishedGoodStocks", "BuyerName");
            DropColumn("dbo.FinishedGoodStocks", "OrderNumber");
            DropColumn("dbo.FinishedGoodStocks", "DeliveryDate");
            DropColumn("dbo.FinishedGoodStocks", "ReceiveDate");
            CreateIndex("dbo.FinishedGoodStocks", "ProcesseLocationId");
            AddForeignKey("dbo.FinishedGoodStocks", "ProcesseLocationId", "dbo.ProcesseLocations", "ProcesseLocationId", cascadeDelete: true);
        }
    }
}
