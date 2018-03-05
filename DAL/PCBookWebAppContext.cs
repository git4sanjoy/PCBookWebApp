using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace PCBookWebApp.DAL
{
    public class PCBookWebAppContext : DbContext
    {
        public PCBookWebAppContext() : base("name=PCBookWebAppContext")
        {

        }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.Unit> Units { get; set; }

        public IEnumerable<object> UnitUsers { get; internal set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.TransctionType> TransctionTypes { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ShowRoom> ShowRooms { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ShowRoomUser> ShowRoomUsers { get; set; }

        //Book Module
        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.Primary> Primaries { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.VoucherType> VoucherTypes { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.Ledger> Ledgers { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.Voucher> Vouchers { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.VoucherDetail> VoucherDetails { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BankModule.CheckReceive> CheckReceives { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.Provision> Provisions { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.CreditorForExpenses> CreditorForExpenses { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BankModule.Bank> Banks { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BankModule.BankAccount> BankAccounts { get; set; }


        public System.Data.Entity.DbSet<PCBookWebApp.Models.BankModule.Check> Checks { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BankModule.CheckBookPage> CheckBookPages { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BankModule.CheckBook> CheckBooks { get; set; }


        //Sales Module
        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.District> Districts { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.Upazila> Upazilas { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.MainCategory> MainCategories { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.SalesMan> SalesMen { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.SubCategory> SubCategories { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.Product> Products { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.MemoMaster> MemoMasters { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.MemoDetail> MemoDetails { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.ZoneManager> ZoneManagers { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.SaleZone> SaleZones { get; set; }

        //Process Module

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.ProductType> ProductTypes { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.Supplier> Suppliers { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.PurchasedProduct> PurchasedProducts { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.ProcessList> ProcessLists { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.UnitRole> UnitRoles { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.Matric> Matrics { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.ConversionDetail> ConversionDetails { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.ProcesseLocation> ProcesseLocations { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.Conversion> Conversions { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.Purchase> Purchases { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.Process> Processes { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.BookModule.CostCenter> CostCenters { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.FinishedGood> FinishedGoods { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.FinishedGoodImage> FinishedGoodImages { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.ProcessModule.FinishedGoodStock> FinishedGoodStocks { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.MDModule.Deal> Deals { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.MDModule.Deal_Image> Deal_Image { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.MDModule.DealProduction> DealProductions { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.Division> Divisions { get; set; }

        public System.Data.Entity.DbSet<PCBookWebApp.Models.SalesModule.UnitManager> UnitManagers { get; set; }
    }
}