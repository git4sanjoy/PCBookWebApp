namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWareHouseIdToMemoMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemoMasters", "WareHouseId", c => c.Int());
            AddColumn("dbo.ZoneManagers", "WareHouseId", c => c.Int());
            CreateIndex("dbo.MemoMasters", "WareHouseId");
            AddForeignKey("dbo.MemoMasters", "WareHouseId", "dbo.WareHouses", "WareHouseId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemoMasters", "WareHouseId", "dbo.WareHouses");
            DropIndex("dbo.MemoMasters", new[] { "WareHouseId" });
            DropColumn("dbo.ZoneManagers", "WareHouseId");
            DropColumn("dbo.MemoMasters", "WareHouseId");
        }
    }
}
