namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserIdToZoneManager : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ZoneManagers", "Id", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ZoneManagers", "Id");
        }
    }
}
