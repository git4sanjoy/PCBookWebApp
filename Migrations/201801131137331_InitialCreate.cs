namespace PCBookWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccounts",
                c => new
                    {
                        BankAccountId = c.Int(nullable: false, identity: true),
                        BankId = c.Int(nullable: false),
                        LedgerId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                        BankAccountNumber = c.String(nullable: false, maxLength: 25),
                        AccountOpenDate = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.BankAccountId)
                .ForeignKey("dbo.Banks", t => t.BankId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId, cascadeDelete: true)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .Index(t => t.BankId)
                .Index(t => t.LedgerId)
                .Index(t => t.GroupId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        BankId = c.Int(nullable: false, identity: true),
                        BankName = c.String(nullable: false, maxLength: 25),
                        Address = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Website = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.BankId);
            
            CreateTable(
                "dbo.Checks",
                c => new
                    {
                        CheckId = c.Int(nullable: false, identity: true),
                        BankAccountId = c.Int(nullable: false),
                        LedgerId = c.Int(nullable: false),
                        VoucherId = c.Int(nullable: false),
                        VoucherDetailId = c.Int(),
                        CheckBookPageId = c.Int(nullable: false),
                        CheckNumber = c.String(nullable: false, maxLength: 25),
                        Amount = c.Double(nullable: false),
                        IssueDate = c.DateTime(nullable: false),
                        CheckDate = c.DateTime(nullable: false),
                        HonourDate = c.DateTime(nullable: false),
                        Remarks = c.String(),
                        ApprovedBy = c.String(),
                        CreatedBy = c.String(),
                        Active = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        Bank_BankId = c.Int(),
                    })
                .PrimaryKey(t => t.CheckId)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.CheckBookPages", t => t.CheckBookPageId)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .ForeignKey("dbo.VoucherDetails", t => t.VoucherDetailId)
                .ForeignKey("dbo.Banks", t => t.Bank_BankId)
                .Index(t => t.BankAccountId)
                .Index(t => t.LedgerId)
                .Index(t => t.VoucherId)
                .Index(t => t.VoucherDetailId)
                .Index(t => t.CheckBookPageId)
                .Index(t => t.Bank_BankId);
            
            CreateTable(
                "dbo.CheckBookPages",
                c => new
                    {
                        CheckBookPageId = c.Int(nullable: false, identity: true),
                        CheckBookId = c.Int(nullable: false),
                        CheckBookPageNo = c.String(nullable: false, maxLength: 45),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.CheckBookPageId)
                .ForeignKey("dbo.CheckBooks", t => t.CheckBookId, cascadeDelete: true)
                .Index(t => t.CheckBookId);
            
            CreateTable(
                "dbo.CheckBooks",
                c => new
                    {
                        CheckBookId = c.Int(nullable: false, identity: true),
                        BankAccountId = c.Int(nullable: false),
                        CheckBookNo = c.String(nullable: false, maxLength: 45),
                        StartSuffices = c.String(),
                        StartNo = c.Double(nullable: false),
                        EndNo = c.Double(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.CheckBookId)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId, cascadeDelete: true)
                .Index(t => t.BankAccountId);
            
            CreateTable(
                "dbo.Ledgers",
                c => new
                    {
                        LedgerId = c.Int(nullable: false, identity: true),
                        LedgerName = c.String(nullable: false, maxLength: 50),
                        GroupId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        TrialBalanceId = c.Int(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                        TrialBalance = c.Boolean(nullable: false),
                        Provision = c.Boolean(nullable: false),
                        BankAccountId = c.Int(),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LedgerId)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId)
                .Index(t => t.GroupId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(nullable: false, maxLength: 145),
                        PrimaryId = c.Int(),
                        ParentId = c.Int(nullable: false),
                        IsParent = c.Boolean(nullable: false),
                        GroupIdStr = c.String(),
                        TrialBalance = c.Boolean(nullable: false),
                        Provision = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.Primaries", t => t.PrimaryId)
                .Index(t => t.PrimaryId);
            
            CreateTable(
                "dbo.Primaries",
                c => new
                    {
                        PrimaryId = c.Int(nullable: false, identity: true),
                        PrimaryName = c.String(nullable: false, maxLength: 145),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.PrimaryId);
            
            CreateTable(
                "dbo.ShowRooms",
                c => new
                    {
                        ShowRoomId = c.Int(nullable: false, identity: true),
                        UnitId = c.Int(nullable: false),
                        ShowRoomName = c.String(nullable: false, maxLength: 145),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ShowRoomId)
                .ForeignKey("dbo.Units", t => t.UnitId, cascadeDelete: true)
                .Index(t => t.UnitId);
            
            CreateTable(
                "dbo.Units",
                c => new
                    {
                        UnitId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        UnitName = c.String(nullable: false, maxLength: 145),
                        Address = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Website = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.UnitId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 145),
                        Address = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Website = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProjectId);
            
            CreateTable(
                "dbo.Vouchers",
                c => new
                    {
                        VoucherId = c.Int(nullable: false, identity: true),
                        VoucherTypeId = c.Int(nullable: false),
                        VoucherDate = c.DateTime(nullable: false),
                        VoucherNo = c.String(nullable: false),
                        Naration = c.String(maxLength: 245),
                        ShowRoomId = c.Int(nullable: false),
                        IsBank = c.Boolean(nullable: false),
                        IsHonored = c.Boolean(nullable: false),
                        HonoredDate = c.DateTime(),
                        Authorized = c.Boolean(nullable: false),
                        AuthorizedBy = c.String(),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VoucherId)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .ForeignKey("dbo.VoucherTypes", t => t.VoucherTypeId, cascadeDelete: true)
                .Index(t => t.VoucherTypeId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.CheckReceives",
                c => new
                    {
                        CheckReceiveId = c.Int(nullable: false, identity: true),
                        VoucherId = c.Int(nullable: false),
                        VoucherDetailId = c.Int(nullable: false),
                        BankOrPartyName = c.String(),
                        Amount = c.Double(nullable: false),
                        CheckOrMoneyReceiptNo = c.String(),
                        HonourDate = c.DateTime(),
                        ShowRoomId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.CheckReceiveId)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .ForeignKey("dbo.VoucherDetails", t => t.VoucherDetailId)
                .Index(t => t.VoucherId)
                .Index(t => t.VoucherDetailId)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.VoucherDetails",
                c => new
                    {
                        VoucherDetailId = c.Int(nullable: false, identity: true),
                        VoucherId = c.Int(nullable: false),
                        TrialBalanceId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                        LedgerId = c.Int(nullable: false),
                        TransctionTypeId = c.Int(nullable: false),
                        DrAmount = c.Double(nullable: false),
                        CrAmount = c.Double(nullable: false),
                        ReceiveOrPayment = c.Boolean(nullable: false),
                        CheckId = c.Int(),
                        BankAccountId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.VoucherDetailId)
                .ForeignKey("dbo.Ledgers", t => t.LedgerId)
                .ForeignKey("dbo.TransctionTypes", t => t.TransctionTypeId)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .Index(t => t.VoucherId)
                .Index(t => t.LedgerId)
                .Index(t => t.TransctionTypeId);
            
            CreateTable(
                "dbo.TransctionTypes",
                c => new
                    {
                        TransctionTypeId = c.Int(nullable: false, identity: true),
                        TransctionTypeName = c.String(nullable: false, maxLength: 50),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TransctionTypeId);
            
            CreateTable(
                "dbo.VoucherTypes",
                c => new
                    {
                        VoucherTypeId = c.Int(nullable: false, identity: true),
                        VoucherTypeName = c.String(nullable: false, maxLength: 50),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VoucherTypeId);
            
            CreateTable(
                "dbo.CreditorForExpenses",
                c => new
                    {
                        CreditorForExpensesId = c.Int(nullable: false, identity: true),
                        ProvisionDate = c.DateTime(nullable: false),
                        OpeningAmount = c.Double(nullable: false),
                        ProvisionAmount = c.Double(nullable: false),
                        ActualAmount = c.Double(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.CreditorForExpensesId)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.Provisions",
                c => new
                    {
                        ProvisionId = c.Int(nullable: false, identity: true),
                        ProvisionDate = c.DateTime(nullable: false),
                        OpeningAmount = c.Double(nullable: false),
                        ProvisionAmount = c.Double(nullable: false),
                        ActualAmount = c.Double(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProvisionId)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .Index(t => t.ShowRoomId);
            
            CreateTable(
                "dbo.ShowRoomUsers",
                c => new
                    {
                        ShowRoomUserId = c.Int(nullable: false, identity: true),
                        Id = c.String(nullable: false),
                        ShowRoomId = c.Int(nullable: false),
                        UserName = c.String(),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.ShowRoomUserId)
                .ForeignKey("dbo.ShowRooms", t => t.ShowRoomId, cascadeDelete: true)
                .Index(t => t.ShowRoomId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShowRoomUsers", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Provisions", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.CreditorForExpenses", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.BankAccounts", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.BankAccounts", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.BankAccounts", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Checks", "Bank_BankId", "dbo.Banks");
            DropForeignKey("dbo.Vouchers", "VoucherTypeId", "dbo.VoucherTypes");
            DropForeignKey("dbo.VoucherDetails", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherDetails", "TransctionTypeId", "dbo.TransctionTypes");
            DropForeignKey("dbo.VoucherDetails", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.Vouchers", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Checks", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.CheckReceives", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.CheckReceives", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Checks", "LedgerId", "dbo.Ledgers");
            DropForeignKey("dbo.ShowRooms", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Units", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Ledgers", "ShowRoomId", "dbo.ShowRooms");
            DropForeignKey("dbo.Ledgers", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "PrimaryId", "dbo.Primaries");
            DropForeignKey("dbo.Checks", "CheckBookPageId", "dbo.CheckBookPages");
            DropForeignKey("dbo.CheckBookPages", "CheckBookId", "dbo.CheckBooks");
            DropForeignKey("dbo.CheckBooks", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.Checks", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.BankAccounts", "BankId", "dbo.Banks");
            DropIndex("dbo.ShowRoomUsers", new[] { "ShowRoomId" });
            DropIndex("dbo.Provisions", new[] { "ShowRoomId" });
            DropIndex("dbo.CreditorForExpenses", new[] { "ShowRoomId" });
            DropIndex("dbo.VoucherDetails", new[] { "TransctionTypeId" });
            DropIndex("dbo.VoucherDetails", new[] { "LedgerId" });
            DropIndex("dbo.VoucherDetails", new[] { "VoucherId" });
            DropIndex("dbo.CheckReceives", new[] { "ShowRoomId" });
            DropIndex("dbo.CheckReceives", new[] { "VoucherDetailId" });
            DropIndex("dbo.CheckReceives", new[] { "VoucherId" });
            DropIndex("dbo.Vouchers", new[] { "ShowRoomId" });
            DropIndex("dbo.Vouchers", new[] { "VoucherTypeId" });
            DropIndex("dbo.Units", new[] { "ProjectId" });
            DropIndex("dbo.ShowRooms", new[] { "UnitId" });
            DropIndex("dbo.Groups", new[] { "PrimaryId" });
            DropIndex("dbo.Ledgers", new[] { "ShowRoomId" });
            DropIndex("dbo.Ledgers", new[] { "GroupId" });
            DropIndex("dbo.CheckBooks", new[] { "BankAccountId" });
            DropIndex("dbo.CheckBookPages", new[] { "CheckBookId" });
            DropIndex("dbo.Checks", new[] { "Bank_BankId" });
            DropIndex("dbo.Checks", new[] { "CheckBookPageId" });
            DropIndex("dbo.Checks", new[] { "VoucherDetailId" });
            DropIndex("dbo.Checks", new[] { "VoucherId" });
            DropIndex("dbo.Checks", new[] { "LedgerId" });
            DropIndex("dbo.Checks", new[] { "BankAccountId" });
            DropIndex("dbo.BankAccounts", new[] { "ShowRoomId" });
            DropIndex("dbo.BankAccounts", new[] { "GroupId" });
            DropIndex("dbo.BankAccounts", new[] { "LedgerId" });
            DropIndex("dbo.BankAccounts", new[] { "BankId" });
            DropTable("dbo.ShowRoomUsers");
            DropTable("dbo.Provisions");
            DropTable("dbo.CreditorForExpenses");
            DropTable("dbo.VoucherTypes");
            DropTable("dbo.TransctionTypes");
            DropTable("dbo.VoucherDetails");
            DropTable("dbo.CheckReceives");
            DropTable("dbo.Vouchers");
            DropTable("dbo.Projects");
            DropTable("dbo.Units");
            DropTable("dbo.ShowRooms");
            DropTable("dbo.Primaries");
            DropTable("dbo.Groups");
            DropTable("dbo.Ledgers");
            DropTable("dbo.CheckBooks");
            DropTable("dbo.CheckBookPages");
            DropTable("dbo.Checks");
            DropTable("dbo.Banks");
            DropTable("dbo.BankAccounts");
        }
    }
}
