namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProcessModule1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchasedProducts", "MatricId", c => c.Int(nullable: false));
            AddColumn("dbo.Processes", "PurchaseId", c => c.Int());
            CreateIndex("dbo.PurchasedProducts", "MatricId");
            CreateIndex("dbo.Processes", "PurchaseId");
            AddForeignKey("dbo.PurchasedProducts", "MatricId", "dbo.Matrics", "MatricId", cascadeDelete: false);
            AddForeignKey("dbo.Processes", "PurchaseId", "dbo.Purchases", "PurchaseId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Processes", "PurchaseId", "dbo.Purchases");
            DropForeignKey("dbo.PurchasedProducts", "MatricId", "dbo.Matrics");
            DropIndex("dbo.Processes", new[] { "PurchaseId" });
            DropIndex("dbo.PurchasedProducts", new[] { "MatricId" });
            DropColumn("dbo.Processes", "PurchaseId");
            DropColumn("dbo.PurchasedProducts", "MatricId");
        }
    }
}
