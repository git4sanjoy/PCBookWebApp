namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBankPayment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "PaymentType", c => c.String());
            AddColumn("dbo.Payments", "HonourDate", c => c.DateTime());
            AddColumn("dbo.Payments", "CheckNo", c => c.String());
            AddColumn("dbo.Payments", "BankAccountNo", c => c.String());
            AddColumn("dbo.Payments", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "Remarks");
            DropColumn("dbo.Payments", "BankAccountNo");
            DropColumn("dbo.Payments", "CheckNo");
            DropColumn("dbo.Payments", "HonourDate");
            DropColumn("dbo.Payments", "PaymentType");
        }
    }
}
