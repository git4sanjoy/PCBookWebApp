namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLedgerIdTo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Provisions", "LedgerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Provisions", "LedgerId");
            AddForeignKey("dbo.Provisions", "LedgerId", "dbo.Ledgers", "LedgerId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Provisions", "LedgerId", "dbo.Ledgers");
            DropIndex("dbo.Provisions", new[] { "LedgerId" });
            DropColumn("dbo.Provisions", "LedgerId");
        }
    }
}
