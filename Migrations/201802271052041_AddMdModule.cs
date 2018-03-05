namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMdModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Deal_Image",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ImageUrl = c.String(),
                        DealId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Deals", t => t.DealId, cascadeDelete: true)
                .Index(t => t.DealId);
            
            CreateTable(
                "dbo.Deals",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deal_Image", "DealId", "dbo.Deals");
            DropIndex("dbo.Deal_Image", new[] { "DealId" });
            DropTable("dbo.Deals");
            DropTable("dbo.Deal_Image");
        }
    }
}
