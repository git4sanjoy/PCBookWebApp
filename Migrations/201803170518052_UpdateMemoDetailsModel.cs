namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMemoDetailsModel : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MemoDetails", "ProductId");
            //AddForeignKey("dbo.MemoDetails", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.MemoDetails", "ProductId", "dbo.Products");
            DropIndex("dbo.MemoDetails", new[] { "ProductId" });
        }
    }
}
