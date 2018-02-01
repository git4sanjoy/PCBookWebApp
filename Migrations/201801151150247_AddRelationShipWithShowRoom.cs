namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelationShipWithShowRoom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "ShowRoomId", c => c.Int(nullable: false));
            AddColumn("dbo.SalesMen", "ShowRoomId", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "ShowRoomId", c => c.Int(nullable: false));
            CreateIndex("dbo.Customers", "ShowRoomId");
            CreateIndex("dbo.SalesMen", "ShowRoomId");
            CreateIndex("dbo.Products", "ShowRoomId");
            AddForeignKey("dbo.SalesMen", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId");
            AddForeignKey("dbo.Customers", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId");
            AddForeignKey("dbo.Products", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Customers", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.SalesMen", "ShowRoomId", "dbo.ShowRooms");
            DropIndex("dbo.Products", new[] { "ShowRoomId" });
            DropIndex("dbo.SalesMen", new[] { "ShowRoomId" });
            DropIndex("dbo.Customers", new[] { "ShowRoomId" });
            DropColumn("dbo.Products", "ShowRoomId");
            DropColumn("dbo.SalesMen", "ShowRoomId");
            DropColumn("dbo.Customers", "ShowRoomId");
        }
    }
}
