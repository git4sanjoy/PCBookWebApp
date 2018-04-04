namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddZoneManagerIdAlias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ZoneManagers", "ZoneManagerIdAlias", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ZoneManagers", "ZoneManagerIdAlias");
        }
    }
}
