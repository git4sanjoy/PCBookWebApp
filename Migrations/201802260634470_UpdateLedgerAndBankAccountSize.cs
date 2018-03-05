namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLedgerAndBankAccountSize : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BankAccounts", "BankAccountNumber", c => c.String(nullable: false, maxLength: 240));
            AlterColumn("dbo.Ledgers", "LedgerName", c => c.String(nullable: false, maxLength: 240));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ledgers", "LedgerName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.BankAccounts", "BankAccountNumber", c => c.String(nullable: false, maxLength: 25));
        }
    }
}
