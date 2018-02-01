namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CheckReceives", "VoucherDetailId", "dbo.VoucherDetails");
            DropIndex("dbo.CheckReceives", new[] { "VoucherDetailId" });
            AlterColumn("dbo.CheckReceives", "VoucherDetailId", c => c.Int());
            CreateIndex("dbo.CheckReceives", "VoucherDetailId");
            AddForeignKey("dbo.CheckReceives", "VoucherDetailId", "dbo.VoucherDetails", "VoucherDetailId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CheckReceives", "VoucherDetailId", "dbo.VoucherDetails");
            DropIndex("dbo.CheckReceives", new[] { "VoucherDetailId" });
            AlterColumn("dbo.CheckReceives", "VoucherDetailId", c => c.Int(nullable: false));
            CreateIndex("dbo.CheckReceives", "VoucherDetailId");
            AddForeignKey("dbo.CheckReceives", "VoucherDetailId", "dbo.VoucherDetails", "VoucherDetailId", cascadeDelete: true);
        }
    }
}
