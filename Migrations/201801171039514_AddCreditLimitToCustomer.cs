namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreditLimitToCustomer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CreditLimit", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "CreditLimit");
        }
    }
}
