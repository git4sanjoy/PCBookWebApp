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
                        Id = c.Guid(nullable: false),
                        DealId = c.Guid(nullable: false),
                        FactoryName = c.String(),
                        Quantity = c.Double(nullable: false),
                        DealProductionDate = c.DateTime(nullable: false),
                        ReProduction = c.String(),
                        DateCreated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Deals", t => t.DealId, cascadeDelete: true)
                .Index(t => t.DealId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DealProductions", "DealId", "dbo.Deals");
            DropIndex("dbo.DealProductions", new[] { "DealId" });
            DropTable("dbo.DealProductions");
        }
    }
}
