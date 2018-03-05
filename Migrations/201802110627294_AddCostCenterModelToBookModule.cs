namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCostCenterModelToBookModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CostCenters",
                c => new
                    {
                        CostCenterId = c.Int(nullable: false, identity: true),
                        CostCenterName = c.String(),
                        LedgerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CostCenterId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CostCenters");
        }
    }
}
