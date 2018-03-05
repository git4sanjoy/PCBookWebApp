namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShowRoomIdToCostCenter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CostCenters", "ShowRoomId", c => c.Int(nullable: false));
            CreateIndex("dbo.CostCenters", "ShowRoomId");
            AddForeignKey("dbo.CostCenters", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CostCenters", "ShowRoomId", "dbo.ShowRooms");
            DropIndex("dbo.CostCenters", new[] { "ShowRoomId" });
            DropColumn("dbo.CostCenters", "ShowRoomId");
        }
    }
}
