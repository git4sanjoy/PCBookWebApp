namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinishedGoodsName : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinishedGoodImages",
                c => new
                    {
                        FinishedGoodImageId = c.Int(nullable: false, identity: true),
                        FinishedGoodId = c.Int(nullable: false),
                        ImageName = c.String(),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodImageId)
                .ForeignKey("dbo.FinishedGoods", t => t.FinishedGoodId, cascadeDelete: true)
                .Index(t => t.FinishedGoodId);
            
            CreateTable(
                "dbo.FinishedGoods",
                c => new
                    {
                        FinishedGoodId = c.Int(nullable: false, identity: true),
                        FinishedGoodName = c.String(nullable: false, maxLength: 150),
                        DesignNo = c.String(),
                        ProductTypeId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        ShowRoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FinishedGoodId)
                .ForeignKey("dbo.ProductTypes", t => t.ProductTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: false)
                .Index(t => t.ProductTypeId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.FinishedGoodStocks",
                c => new
                    {
                        FinishedGoodStockId = c.Int(nullable: false, identity: true),
                        FinishedGoodId = c.Int(nullable: false),
                        ProcesseLocationId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        OrderQuantity = c.Double(nullable: false),
                        ReceiveQuantity = c.Double(nullable: false),
                        DeliveryQuantity = c.Double(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        ShowRoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FinishedGoodStockId)
                .ForeignKey("dbo.FinishedGoods", t => t.FinishedGoodId, cascadeDelete: true)
                .ForeignKey("dbo.ProcesseLocations", t => t.ProcesseLocationId, cascadeDelete: false)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: false)
                .Index(t => t.FinishedGoodId)
                .Index(t => t.ProcesseLocationId)
                .Index(t => t.ShowRoomId);
            
            AlterColumn("dbo.Banks", "BankName", c => c.String(nullable: false, maxLength: 240));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinishedGoods", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.FinishedGoods", "ProductTypeId", "dbo.ProductTypes");
            DropForeignKey("dbo.FinishedGoodStocks", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.FinishedGoodStocks", "ProcesseLocationId", "dbo.ProcesseLocations");
            DropForeignKey("dbo.FinishedGoodStocks", "FinishedGoodId", "dbo.FinishedGoods");
            DropForeignKey("dbo.FinishedGoodImages", "FinishedGoodId", "dbo.FinishedGoods");
            DropIndex("dbo.FinishedGoodStocks", new[] { "ShowRoomId" });
            DropIndex("dbo.FinishedGoodStocks", new[] { "ProcesseLocationId" });
            DropIndex("dbo.FinishedGoodStocks", new[] { "FinishedGoodId" });
            DropIndex("dbo.FinishedGoods", new[] { "ShowRoomId" });
            DropIndex("dbo.FinishedGoods", new[] { "ProductTypeId" });
            DropIndex("dbo.FinishedGoodImages", new[] { "FinishedGoodId" });
            AlterColumn("dbo.Banks", "BankName", c => c.String(nullable: false, maxLength: 25));
            DropTable("dbo.FinishedGoodStocks");
            DropTable("dbo.FinishedGoods");
            DropTable("dbo.FinishedGoodImages");
        }
    }
}
