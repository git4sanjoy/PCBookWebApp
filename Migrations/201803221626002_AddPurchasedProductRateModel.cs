namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPurchasedProductRateModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PurchasedProductRates",
                c => new
                    {
                        PurchasedProductRateId = c.Int(nullable: false, identity: true),
                        PurchasedProductId = c.Int(nullable: false),
                        FinishedGoodStockId = c.Int(nullable: false),
                        Quantity = c.Double(nullable: false),
                        AvgRate = c.Double(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PurchasedProductRateId)
                .ForeignKey("dbo.FinishedGoodStocks", t => t.FinishedGoodStockId, cascadeDelete: true)
                .ForeignKey("dbo.PurchasedProducts", t => t.PurchasedProductId, cascadeDelete: false)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: false)
                .Index(t => t.PurchasedProductId)
                .Index(t => t.FinishedGoodStockId)
                .Index(t => t.ShowRoomId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchasedProductRates", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.PurchasedProductRates", "PurchasedProductId", "dbo.PurchasedProducts");
            DropForeignKey("dbo.PurchasedProductRates", "FinishedGoodStockId", "dbo.FinishedGoodStocks");
            DropIndex("dbo.PurchasedProductRates", new[] { "ShowRoomId" });
            DropIndex("dbo.PurchasedProductRates", new[] { "FinishedGoodStockId" });
            DropIndex("dbo.PurchasedProductRates", new[] { "PurchasedProductId" });
            DropTable("dbo.PurchasedProductRates");
        }
    }
}
