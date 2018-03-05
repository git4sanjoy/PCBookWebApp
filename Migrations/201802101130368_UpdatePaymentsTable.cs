namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePaymentsTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Payments", "TSAmount");
            DropColumn("dbo.Payments", "TCAmount");
            DropColumn("dbo.Payments", "TDiscount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "TDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.Payments", "TCAmount", c => c.Double(nullable: false));
            AddColumn("dbo.Payments", "TSAmount", c => c.Double(nullable: false));
        }
    }
}
