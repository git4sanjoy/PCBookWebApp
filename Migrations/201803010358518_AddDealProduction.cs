namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDealProduction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DealProductions",
                c => new
                    {
                        DealProductionId = c.Int(nullable: false, identity: true),
                        DealId = c.Guid(nullable: false),
                        FactoryName = c.String(),
                        Quantity = c.Double(nullable: false),
                        DealProductionDate = c.DateTime(nullable: false),
                        ReProduction = c.String(),
                        DateCreated = c.DateTime(),
                    })
                .PrimaryKey(t => t.DealProductionId)
                .ForeignKey("dbo.Deals", t => t.DealId, cascadeDelete: true)
                .Index(t => t.DealId);
            
            AddColumn("dbo.Deals", "DateCreated", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DealProductions", "DealId", "dbo.Deals");
            DropIndex("dbo.DealProductions", new[] { "DealId" });
            DropColumn("dbo.Deals", "DateCreated");
            DropTable("dbo.DealProductions");
        }
    }
}
