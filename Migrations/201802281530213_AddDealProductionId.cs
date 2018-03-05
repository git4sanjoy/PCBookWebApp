namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDealProductionId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DealProductions");
            AddColumn("dbo.DealProductions", "DealProductionId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DealProductions", "DealProductionId");
            DropColumn("dbo.DealProductions", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DealProductions", "Id", c => c.Guid(nullable: false));
            DropPrimaryKey("dbo.DealProductions");
            DropColumn("dbo.DealProductions", "DealProductionId");
            AddPrimaryKey("dbo.DealProductions", "Id");
        }
    }
}
