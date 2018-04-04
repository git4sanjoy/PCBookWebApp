namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddZoneNameToShowRoom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShowRooms", "ZoneName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShowRooms", "ZoneName");
        }
    }
}
