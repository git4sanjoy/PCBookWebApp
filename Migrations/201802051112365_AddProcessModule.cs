namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProcessModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConversionDetails",
                c => new
                    {
                        ConversionDetailsId = c.Int(nullable: false, identity: true),
                        ConversionId = c.Int(),
                        PurchaseProductId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        PurchaseProduct_PurchasedProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ConversionDetailsId)
                .ForeignKey("dbo.Conversions", t => t.ConversionId)
                .ForeignKey("dbo.PurchasedProducts", t => t.PurchaseProduct_PurchasedProductId)
                .Index(t => t.ConversionId)
                .Index(t => t.PurchaseProduct_PurchasedProductId);
            
            CreateTable(
                "dbo.Conversions",
                c => new
                    {
                        ConversionId = c.Int(nullable: false, identity: true),
                        ConversionName = c.String(nullable: false, maxLength: 100),
                        PurchaseProductId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        PurchaseProduct_PurchasedProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ConversionId)
                .ForeignKey("dbo.PurchasedProducts", t => t.PurchaseProduct_PurchasedProductId)
                .Index(t => t.PurchaseProduct_PurchasedProductId);
            
            CreateTable(
                "dbo.PurchasedProducts",
                c => new
                    {
                        PurchasedProductId = c.Int(nullable: false, identity: true),
                        ProductTypeId = c.Int(nullable: false),
                        PurchasedProductName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.PurchasedProductId)
                .ForeignKey("dbo.ProductTypes", t => t.ProductTypeId, cascadeDelete: true)
                .Index(t => t.ProductTypeId);
            
            CreateTable(
                "dbo.ProductTypes",
                c => new
                    {
                        ProductTypeId = c.Int(nullable: false, identity: true),
                        ProductTypeName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductTypeId);
            
            CreateTable(
                "dbo.Matrics",
                c => new
                    {
                        MatricId = c.Int(nullable: false, identity: true),
                        MatricName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.MatricId);
            
            CreateTable(
                "dbo.ProcesseLocations",
                c => new
                    {
                        ProcesseLocationId = c.Int(nullable: false, identity: true),
                        ProcesseLocationName = c.String(nullable: false, maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProcesseLocationId);
            
            CreateTable(
                "dbo.Processes",
                c => new
                    {
                        ProcessId = c.Int(nullable: false, identity: true),
                        ProcessDate = c.DateTime(nullable: false),
                        LotNo = c.String(),
                        PurchasedProductId = c.Int(nullable: false),
                        ProcessListId = c.Int(nullable: false),
                        ReceiveQuantity = c.Double(nullable: false),
                        DeliveryQuantity = c.Double(nullable: false),
                        SE = c.Double(),
                        Rate = c.Double(),
                        Amount = c.Double(),
                        Discount = c.Double(),
                        ProcesseLocationId = c.Int(nullable: false),
                        ConversionId = c.Int(),
                        ShowRoomId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProcessId)
                .ForeignKey("dbo.Conversions", t => t.ConversionId)
                .ForeignKey("dbo.ProcesseLocations", t => t.ProcesseLocationId, cascadeDelete: true)
                .ForeignKey("dbo.ProcessLists", t => t.ProcessListId, cascadeDelete: true)
                .ForeignKey("dbo.PurchasedProducts", t => t.PurchasedProductId, cascadeDelete: true)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .Index(t => t.PurchasedProductId)
                .Index(t => t.ProcessListId)
                .Index(t => t.ProcesseLocationId)
                .Index(t => t.ConversionId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.ProcessLists",
                c => new
                    {
                        ProcessListId = c.Int(nullable: false, identity: true),
                        UnitRoleId = c.Int(nullable: false),
                        ProcessListName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProcessListId);
            
            CreateTable(
                "dbo.Purchases",
                c => new
                    {
                        PurchaseId = c.Int(nullable: false, identity: true),
                        PurchaseDate = c.DateTime(nullable: false),
                        PChallanNo = c.String(),
                        Quantity = c.Double(nullable: false),
                        SE = c.Double(),
                        Amount = c.Double(nullable: false),
                        Discount = c.Double(),
                        ProcesseLocationId = c.Int(),
                        DeliveryQuantity = c.Double(nullable: false),
                        ProcessListId = c.Int(),
                        SupplierId = c.Int(nullable: false),
                        PurchasedProductId = c.Int(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.PurchaseId)
                .ForeignKey("dbo.ProcesseLocations", t => t.ProcesseLocationId)
                .ForeignKey("dbo.ProcessLists", t => t.ProcessListId)
                .ForeignKey("dbo.PurchasedProducts", t => t.PurchasedProductId, cascadeDelete: true)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.ProcesseLocationId)
                .Index(t => t.ProcessListId)
                .Index(t => t.SupplierId)
                .Index(t => t.PurchasedProductId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierId = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(nullable: false, maxLength: 50),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.SupplierId);
            
            CreateTable(
                "dbo.UnitRoles",
                c => new
                    {
                        UnitRoleId = c.Int(nullable: false, identity: true),
                        UnitRoleName = c.String(nullable: false, maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.UnitRoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchases", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.Purchases", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Purchases", "PurchasedProductId", "dbo.PurchasedProducts");
            DropForeignKey("dbo.Purchases", "ProcessListId", "dbo.ProcessLists");
            DropForeignKey("dbo.Purchases", "ProcesseLocationId", "dbo.ProcesseLocations");
            DropForeignKey("dbo.Processes", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Processes", "PurchasedProductId", "dbo.PurchasedProducts");
            DropForeignKey("dbo.Processes", "ProcessListId", "dbo.ProcessLists");
            DropForeignKey("dbo.Processes", "ProcesseLocationId", "dbo.ProcesseLocations");
            DropForeignKey("dbo.Processes", "ConversionId", "dbo.Conversions");
            DropForeignKey("dbo.ConversionDetails", "PurchaseProduct_PurchasedProductId", "dbo.PurchasedProducts");
            DropForeignKey("dbo.Conversions", "PurchaseProduct_PurchasedProductId", "dbo.PurchasedProducts");
            DropForeignKey("dbo.PurchasedProducts", "ProductTypeId", "dbo.ProductTypes");
            DropForeignKey("dbo.ConversionDetails", "ConversionId", "dbo.Conversions");
            DropIndex("dbo.Purchases", new[] { "ShowRoomId" });
            DropIndex("dbo.Purchases", new[] { "PurchasedProductId" });
            DropIndex("dbo.Purchases", new[] { "SupplierId" });
            DropIndex("dbo.Purchases", new[] { "ProcessListId" });
            DropIndex("dbo.Purchases", new[] { "ProcesseLocationId" });
            DropIndex("dbo.Processes", new[] { "ShowRoomId" });
            DropIndex("dbo.Processes", new[] { "ConversionId" });
            DropIndex("dbo.Processes", new[] { "ProcesseLocationId" });
            DropIndex("dbo.Processes", new[] { "ProcessListId" });
            DropIndex("dbo.Processes", new[] { "PurchasedProductId" });
            DropIndex("dbo.PurchasedProducts", new[] { "ProductTypeId" });
            DropIndex("dbo.Conversions", new[] { "PurchaseProduct_PurchasedProductId" });
            DropIndex("dbo.ConversionDetails", new[] { "PurchaseProduct_PurchasedProductId" });
            DropIndex("dbo.ConversionDetails", new[] { "ConversionId" });
            DropTable("dbo.UnitRoles");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Purchases");
            DropTable("dbo.ProcessLists");
            DropTable("dbo.Processes");
            DropTable("dbo.ProcesseLocations");
            DropTable("dbo.Matrics");
            DropTable("dbo.ProductTypes");
            DropTable("dbo.PurchasedProducts");
            DropTable("dbo.Conversions");
            DropTable("dbo.ConversionDetails");
        }
    }
}
