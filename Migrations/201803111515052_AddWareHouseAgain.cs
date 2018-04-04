namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWareHouseAgain : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WareHouses",
                c => new
                    {
                        WareHouseId = c.Int(nullable: false, identity: true),
                        WareHouseName = c.String(nullable: false, maxLength: 145),
                        WareHouseLocation = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.WareHouseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WareHouses");
        }
    }
}
