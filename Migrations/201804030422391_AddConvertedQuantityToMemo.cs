namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConvertedQuantityToMemo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemoMasters", "Quantity", c => c.Double());
            AddColumn("dbo.MemoMasters", "QuantityConverted", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MemoMasters", "QuantityConverted");
            DropColumn("dbo.MemoMasters", "Quantity");
        }
    }
}
