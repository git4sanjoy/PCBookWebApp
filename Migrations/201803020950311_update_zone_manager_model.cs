namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_zone_manager_model : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ZoneManagers", "Address", c => c.String());
            AddColumn("dbo.ZoneManagers", "Phone", c => c.String());
            AddColumn("dbo.ZoneManagers", "Email", c => c.String());
            AddColumn("dbo.ZoneManagers", "ShowRoom_ShowRoomId", c => c.Int());
            CreateIndex("dbo.ZoneManagers", "ShowRoom_ShowRoomId");
            AddForeignKey("dbo.ZoneManagers", "ShowRoom_ShowRoomId", "dbo.ShowRooms", "ShowRoomId");
            DropColumn("dbo.ZoneManagers", "PhoneNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ZoneManagers", "PhoneNo", c => c.String());
            DropForeignKey("dbo.ZoneManagers", "ShowRoom_ShowRoomId", "dbo.ShowRooms");
            DropIndex("dbo.ZoneManagers", new[] { "ShowRoom_ShowRoomId" });
            DropColumn("dbo.ZoneManagers", "ShowRoom_ShowRoomId");
            DropColumn("dbo.ZoneManagers", "Email");
            DropColumn("dbo.ZoneManagers", "Phone");
            DropColumn("dbo.ZoneManagers", "Address");
        }
    }
}
