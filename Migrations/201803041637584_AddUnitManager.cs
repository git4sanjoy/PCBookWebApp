namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUnitManager : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UnitManagers",
                c => new
                    {
                        UnitManagerId = c.Int(nullable: false, identity: true),
                        Id = c.String(nullable: false),
                        UnitId = c.Int(nullable: false),
                        UnitManagerName = c.String(),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.UnitManagerId)
                .ForeignKey("dbo.Units", t => t.UnitId, cascadeDelete: true)
                .Index(t => t.UnitId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnitManagers", "UnitId", "dbo.Units");
            DropIndex("dbo.UnitManagers", new[] { "UnitId" });
            DropTable("dbo.UnitManagers");
        }
    }
}
