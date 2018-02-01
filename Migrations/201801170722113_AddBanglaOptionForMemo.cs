namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBanglaOptionForMemo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "CustomerNameBangla", c => c.String(nullable: false, maxLength: 145));
            AddColumn("dbo.Customers", "AddressBangla", c => c.String());
            AddColumn("dbo.Products", "ProductNameBangla", c => c.String(nullable: false, maxLength: 145));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ProductNameBangla");
            DropColumn("dbo.Customers", "AddressBangla");
            DropColumn("dbo.Customers", "CustomerNameBangla");
        }
    }
}
