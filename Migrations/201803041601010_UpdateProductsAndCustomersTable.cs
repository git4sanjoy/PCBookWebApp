namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProductsAndCustomersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Customers", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Products", "ShowRoomId", "dbo.ShowRooms");
            DropIndex("dbo.Customers", new[] { "ShowRoomId" });
            DropIndex("dbo.Products", new[] { "ShowRoomId" });
            AddColumn("dbo.Customers", "UnitId", c => c.Int());
            AlterColumn("dbo.Customers", "ShowRoomId", c => c.Int());
            AlterColumn("dbo.Products", "ShowRoomId", c => c.Int());
            CreateIndex("dbo.Customers", "ShowRoomId");
            CreateIndex("dbo.Customers", "UnitId");
            CreateIndex("dbo.Products", "ShowRoomId");
            AddForeignKey("dbo.Customers", "UnitId", "dbo.Units", "UnitId");
            AddForeignKey("dbo.Customers", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId");
            AddForeignKey("dbo.Products", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Customers", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Customers", "UnitId", "dbo.Units");
            DropIndex("dbo.Products", new[] { "ShowRoomId" });
            DropIndex("dbo.Customers", new[] { "UnitId" });
            DropIndex("dbo.Customers", new[] { "ShowRoomId" });
            AlterColumn("dbo.Products", "ShowRoomId", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "ShowRoomId", c => c.Int(nullable: false));
            DropColumn("dbo.Customers", "UnitId");
            CreateIndex("dbo.Products", "ShowRoomId");
            CreateIndex("dbo.Customers", "ShowRoomId");
            AddForeignKey("dbo.Products", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId", cascadeDelete: true);
            AddForeignKey("dbo.Customers", "ShowRoomId", "dbo.ShowRooms", "ShowRoomId", cascadeDelete: true);
        }
    }
}
