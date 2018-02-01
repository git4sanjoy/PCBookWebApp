namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdjustmentBfToPayments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "AdjustmentBf", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "AdjustmentBf");
        }
    }
}
