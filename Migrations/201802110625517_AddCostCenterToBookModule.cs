namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCostCenterToBookModule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoucherDetails", "CostCenterId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoucherDetails", "CostCenterId");
        }
    }
}
