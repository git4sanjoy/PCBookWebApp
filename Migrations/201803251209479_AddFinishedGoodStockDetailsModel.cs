namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinishedGoodStockDetailsModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FinishedGoodStocks", "FinishedGoodId", "dbo.FinishedGoods");
            DropIndex("dbo.FinishedGoodStocks", new[] { "FinishedGoodId" });
            CreateTable(
                "dbo.FinishedGoodStockDetails",
                c => new
                    {
                        FinishedGoodStockDetailsId = c.Int(nullable: false, identity: true),
                        FinishedGoodId = c.Int(nullable: false),
                        FinishedGoodStockId = c.Int(nullable: false),
                        OrderQuantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.FinishedGoodStockDetailsId)
                .ForeignKey("dbo.FinishedGoods", t => t.FinishedGoodId, cascadeDelete: true)
                .ForeignKey("dbo.FinishedGoodStocks", t => t.FinishedGoodStockId, cascadeDelete: true)
                .Index(t => t.FinishedGoodId)
                .Index(t => t.FinishedGoodStockId);
            
            AlterColumn("dbo.FinishedGoodStocks", "FinishedGoodId", c => c.Int());
            CreateIndex("dbo.FinishedGoodStocks", "FinishedGoodId");
            AddForeignKey("dbo.FinishedGoodStocks", "FinishedGoodId", "dbo.FinishedGoods", "FinishedGoodId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinishedGoodStocks", "FinishedGoodId", "dbo.FinishedGoods");
            DropForeignKey("dbo.FinishedGoodStockDetails", "FinishedGoodStockId", "dbo.FinishedGoodStocks");
            DropForeignKey("dbo.FinishedGoodStockDetails", "FinishedGoodId", "dbo.FinishedGoods");
            DropIndex("dbo.FinishedGoodStockDetails", new[] { "FinishedGoodStockId" });
            DropIndex("dbo.FinishedGoodStockDetails", new[] { "FinishedGoodId" });
            DropIndex("dbo.FinishedGoodStocks", new[] { "FinishedGoodId" });
            AlterColumn("dbo.FinishedGoodStocks", "FinishedGoodId", c => c.Int(nullable: false));
            DropTable("dbo.FinishedGoodStockDetails");
            CreateIndex("dbo.FinishedGoodStocks", "FinishedGoodId");
            AddForeignKey("dbo.FinishedGoodStocks", "FinishedGoodId", "dbo.FinishedGoods", "FinishedGoodId", cascadeDelete: true);
        }
    }
}
