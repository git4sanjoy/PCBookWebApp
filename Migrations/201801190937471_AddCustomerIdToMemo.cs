namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerIdToMemo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemoMasters", "CustomerId", c => c.Int(nullable: false));
            AddColumn("dbo.MemoMasters", "ExpencessRemarks", c => c.String());
            CreateIndex("dbo.MemoMasters", "CustomerId");
            AddForeignKey("dbo.MemoMasters", "CustomerId", "dbo.Customers", "CustomerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemoMasters", "CustomerId", "dbo.Customers");
            DropIndex("dbo.MemoMasters", new[] { "CustomerId" });
            DropColumn("dbo.MemoMasters", "ExpencessRemarks");
            DropColumn("dbo.MemoMasters", "CustomerId");
        }
    }
}
