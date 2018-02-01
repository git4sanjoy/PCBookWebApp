namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePaymentRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "MemoMasterId", "dbo.MemoMasters");
            DropIndex("dbo.Payments", new[] { "MemoMasterId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Payments", "MemoMasterId");
            AddForeignKey("dbo.Payments", "MemoMasterId", "dbo.MemoMasters", "MemoMasterId");
        }
    }
}
