namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSalesModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        SalesManId = c.Int(nullable: false),
                        UpazilaId = c.Int(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 145),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Image = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.CustomerId)
                .ForeignKey("dbo.SalesMen", t => t.SalesManId, cascadeDelete: true)
                .ForeignKey("dbo.Upazilas", t => t.UpazilaId, cascadeDelete: true)
                .Index(t => t.SalesManId)
                .Index(t => t.UpazilaId);
            
            CreateTable(
                "dbo.SalesMen",
                c => new
                    {
                        SalesManId = c.Int(nullable: false, identity: true),
                        SalesManName = c.String(nullable: false, maxLength: 145),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.SalesManId);
            
            CreateTable(
                "dbo.Upazilas",
                c => new
                    {
                        UpazilaId = c.Int(nullable: false, identity: true),
                        DistrictId = c.Int(nullable: false),
                        UpazilaName = c.String(nullable: false, maxLength: 145),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.UpazilaId)
                .ForeignKey("dbo.Districts", t => t.DistrictId, cascadeDelete: true)
                .Index(t => t.DistrictId);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        DistrictId = c.Int(nullable: false, identity: true),
                        DistrictName = c.String(nullable: false, maxLength: 145),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.DistrictId);
            
            CreateTable(
                "dbo.MainCategories",
                c => new
                    {
                        MainCategoryId = c.Int(nullable: false, identity: true),
                        MainCategoryName = c.String(nullable: false, maxLength: 145),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.MainCategoryId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        SubCategoryId = c.Int(nullable: false),
                        ProductName = c.String(nullable: false, maxLength: 145),
                        Image = c.String(),
                        MultiplyWith = c.Double(nullable: false),
                        Rate = c.Double(nullable: false),
                        Discount = c.Double(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId, cascadeDelete: true)
                .Index(t => t.SubCategoryId);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        SubCategoryId = c.Int(nullable: false, identity: true),
                        MainCategoryId = c.Int(nullable: false),
                        SubCategoryName = c.String(nullable: false, maxLength: 145),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.SubCategoryId)
                .ForeignKey("dbo.MainCategories", t => t.MainCategoryId, cascadeDelete: true)
                .Index(t => t.MainCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.SubCategories", "MainCategoryId", "dbo.MainCategories");
            DropForeignKey("dbo.Customers", "UpazilaId", "dbo.Upazilas");
            DropForeignKey("dbo.Upazilas", "DistrictId", "dbo.Districts");
            DropForeignKey("dbo.Customers", "SalesManId", "dbo.SalesMen");
            DropIndex("dbo.SubCategories", new[] { "MainCategoryId" });
            DropIndex("dbo.Products", new[] { "SubCategoryId" });
            DropIndex("dbo.Upazilas", new[] { "DistrictId" });
            DropIndex("dbo.Customers", new[] { "UpazilaId" });
            DropIndex("dbo.Customers", new[] { "SalesManId" });
            DropTable("dbo.SubCategories");
            DropTable("dbo.Products");
            DropTable("dbo.MainCategories");
            DropTable("dbo.Districts");
            DropTable("dbo.Upazilas");
            DropTable("dbo.SalesMen");
            DropTable("dbo.Customers");
        }
    }
}
