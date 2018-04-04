using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PCBookWebApp.DAL;
using PCBookWebApp.Models;
using PCBookWebApp.Models.ViewModels;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.SalesModule;
using PCBookWebApp.Models.SalesModule.ViewModel;
using System.Data.Entity.Migrations;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    [Authorize]
    public class CustomerController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        [Route("api/Customer/CustomerBalanceUsingQuery")]
        [HttpGet]
        [ResponseType(typeof(CustomerBalanceView))]
        public IHttpActionResult GetCustomerBalanceUsingQuery()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            List<CustomerBalanceView> list = new List<CustomerBalanceView>();
            CustomerBalanceView customer = new CustomerBalanceView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    ShowRoomId, SalesManId, UpazilaId, DistrictId, CustomerId, MemoDiscount, GatOther, MemoTotal, BfAmount, TotalCollection, TotalDiscount, SalesManName, UpazilaName, DistrictName, 
                                    ShopName, CustomerName, LastPayment, LastMemo
                                    FROM            
                                    ViewCustomerClosingBalance WHERE       
                                    (ShowRoomId = @showRoomId) AND (CustomerName <> 'Cash Party')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        customer = new CustomerBalanceView();

                        double totalsale = 0;
                        double paymentAmount = 0;
                        double bfAmount = 0;
                        double adjustmentDiscount = 0;
                        double memoDiscount = 0;
                        double otherExpencess = 0;


                        double mDiscount = 0;
                        double eOthers = 0;
                        double tSale = 0;


                        int cid = Convert.ToInt32(reader["CustomerId"]);
                        customer.CustomerName = (string)reader["CustomerName"];
                        customer.ShopName = (string)reader["ShopName"];
                        customer.UpazilaName = (string)reader["UpazilaName"];
                        customer.DistrictName = (string)reader["DistrictName"];
                        customer.SalesManName = (string)reader["SalesManName"];

                        if (reader["BfAmount"] != System.DBNull.Value)
                        {
                            bfAmount = (double)reader["BfAmount"];
                        }

                        if (reader["TotalCollection"] != System.DBNull.Value)
                        {
                            paymentAmount = (double)reader["TotalCollection"];
                        }
                        if (reader["TotalDiscount"] != System.DBNull.Value)
                        {
                            adjustmentDiscount = (double)reader["TotalDiscount"];
                        }
                        if (reader["LastPayment"] != System.DBNull.Value)
                        {
                            customer.PaymentDate = (DateTime)reader["LastPayment"];
                        }
                        customer.PaymentAmount = paymentAmount;
                        customer.Adjustment = adjustmentDiscount;
                        customer.BfAmount = bfAmount;




                        if (reader["MemoTotal"] != System.DBNull.Value)
                        {
                            totalsale = (double)reader["MemoTotal"];
                        }
                        if (reader["GatOther"] != System.DBNull.Value)
                        {
                            otherExpencess = (double)reader["GatOther"];
                        }
                        if (reader["MemoDiscount"] != System.DBNull.Value)
                        {
                            memoDiscount = (double)reader["MemoDiscount"];
                        }
                        if (reader["LastMemo"] != System.DBNull.Value)
                        {
                            customer.MemoDate = (DateTime)reader["LastMemo"];
                        }

                        //if (Convert.ToInt32(reader["CustomerId"]) == 1009)
                        //{
                        //    int check = 1;
                        //}

                        totalsale = tSale;
                        memoDiscount = mDiscount;
                        otherExpencess = eOthers;

                        customer.SaleCost = Math.Round(totalsale - memoDiscount, 0);
                        customer.MemoDiscount = memoDiscount;
                        customer.GatOther = otherExpencess;

                        customer.Balance = Math.Round(bfAmount + totalsale + otherExpencess - memoDiscount - adjustmentDiscount - paymentAmount, 0);
                        list.Add(customer);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(list);
        }

        [Route("api/Customer/GetCustomerBalance")]
        [HttpGet]
        [ResponseType(typeof(CustomerBalanceView))]
        public IHttpActionResult GetCustomerBalance()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            DateTime ceatedAt = DateTime.Now;
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();

            var closeingBalanceList = db.Customers
                .Include(c=> c.ShowRoom)
                .Include(c => c.Upazila)
                .Include(c => c.Unit)
                .Include(c => c.SalesMan)
                .Include(c => c.MemoMasters)
                .Include(c => c.Payments)
                .Include(c => c.Upazila.District)
                .Include(c => c.Upazila.District.SaleZone)
                .Include(c => c.Upazila.District.SaleZone.ZoneManager)
                .Include(c => c.Upazila.District.SaleZone.Division)
                .Select(c => new {
                    UnitId = c.UnitId,
                    c.ShowRoomId,
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    c.CreditLimit,
                    SalesManName = c.SalesMan.SalesManName,
                    ShopName = c.ShopName,
                    Address = c.Address,
                    Phone = c.Phone,
                    UpazilaId = c.Upazila.UpazilaId,
                    UpazilaName = c.Upazila.UpazilaName,
                    DistrictId = c.Upazila.District.DistrictId,
                    DistrictName = c.Upazila.District.DistrictName,
                    SaleZoneId = c.Upazila.District.SaleZone.SaleZoneId,
                    SaleZoneName = c.Upazila.District.SaleZone.SaleZoneName,
                    ZoneManagerId = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerId,
                    ZoneManagerName = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                    DivisionId = c.Upazila.District.SaleZone.Division.DivisionId,
                    DivisionName = c.Upazila.District.SaleZone.Division.DivisionName,
                    MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
                    GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
                    GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?) s.MemoCost) ?? 0,
                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                    TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                })
                .Where(c => (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party Pakiza Print") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party Pakiza Textile") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party Pakiza Fabrics") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party Pakiza Store") 
                                && (c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party PSC Islampur"))
                .ToList().GroupBy(x => new { x.ShowRoomId, x.CustomerId })
                        .Select(
                                g => new
                                {
                                    Key = g.Key,
                                    CreditLimit= g.First().CreditLimit,
                                    SalesManName=g.First().SaleZoneName,
                                    ShowRoomId = g.First().ShowRoomId,
                                    CustomerId = g.First().CustomerId,
                                    CustomerName = g.First().CustomerName,
                                    Address = g.First().Address,
                                    Phone = g.First().Phone,
                                    DistrictName = g.First().DistrictName,
                                    UpazilaName = g.First().UpazilaName,
                                    SaleZoneName = g.First().SaleZoneName,
                                    ZoneManagerName = g.First().ZoneManagerName,
                                    DivisionName = g.First().DivisionName,
                                    TotalGroupCount = g.Count(),
                                    MemoDiscount = g.First().MemoDiscount,
                                    GatOther = g.First().GatOther,
                                    TotalPayments = g.First().TotalPayments,
                                    ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                    TotalBf = g.First().TotalBf,
                                    TotalDiscounts = g.First().TotalDiscounts,
                                    Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments
                                });

            return Ok(closeingBalanceList);


            //List<CustomerBalanceView> list = new List<CustomerBalanceView>();
            //CustomerBalanceView customer = new CustomerBalanceView();

            //string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = @"SELECT 
            //                        dbo.Customers.ShowRoomId, dbo.Customers.SalesManId, dbo.SalesMen.SalesManName, dbo.Customers.UpazilaId, dbo.Upazilas.UpazilaName, dbo.Upazilas.DistrictId, 
            //                        dbo.Districts.DistrictName, dbo.Customers.CustomerId, dbo.Customers.ShopName, dbo.Customers.CustomerName, MAX(dbo.Payments.PaymentDate) AS LastPayment, 
            //                        SUM((CASE AdjustmentBf WHEN 1 THEN SSAmount ELSE 0 END)) AS BfAmount, SUM(dbo.Payments.SCAmount) AS TotalCollection, SUM(dbo.Payments.SDiscount) AS TotalDiscount
            //                        FROM            
            //                        dbo.Customers INNER JOIN
            //                        dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
            //                        dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
            //                        dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId LEFT OUTER JOIN
            //                        dbo.Payments ON dbo.Customers.CustomerId = dbo.Payments.CustomerId
            //                        GROUP BY dbo.Customers.CustomerId, dbo.Customers.CustomerName, dbo.Customers.ShowRoomId, dbo.Customers.SalesManId, dbo.SalesMen.SalesManName, dbo.Customers.UpazilaId, dbo.Upazilas.UpazilaName, 
            //                        dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.Customers.ShopName
            //                        HAVING        
            //                        (dbo.Customers.ShowRoomId = @showRoomId) AND (dbo.Customers.CustomerName <> 'Cash Party')";

            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand command = new SqlCommand(queryString, connection);
            //    connection.Open();
            //    command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
            //    SqlDataReader reader = command.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            customer = new CustomerBalanceView();
                        
            //            double totalsale = 0;
            //            double paymentAmount = 0;
            //            double bfAmount = 0;
            //            double adjustmentDiscount = 0;
            //            double memoDiscount = 0;
            //            double otherExpencess = 0;


            //            double mDiscount = 0;
            //            double eOthers = 0;
            //            double tSale = 0;


            //            int cid = Convert.ToInt32(reader["CustomerId"]);
            //            customer.CustomerName = (string)reader["CustomerName"];
            //            customer.ShopName = (string)reader["ShopName"];
            //            customer.UpazilaName = (string)reader["UpazilaName"];
            //            customer.DistrictName = (string)reader["DistrictName"];
            //            customer.SalesManName = (string)reader["SalesManName"];
                        
            //            if (reader["BfAmount"] != System.DBNull.Value)
            //            {
            //                bfAmount = (double)reader["BfAmount"];
            //            }
                                               
            //            if (reader["TotalCollection"] != System.DBNull.Value)
            //            {
            //                paymentAmount = (double)reader["TotalCollection"];
            //            }
            //            if (reader["TotalDiscount"] != System.DBNull.Value)
            //            {
            //                adjustmentDiscount = (double)reader["TotalDiscount"];
            //            }
            //            if (reader["LastPayment"] != System.DBNull.Value)
            //            {
            //                customer.PaymentDate = (DateTime)reader["LastPayment"];
            //            }
            //            customer.PaymentAmount = paymentAmount;
            //            customer.Adjustment = adjustmentDiscount;
            //            customer.BfAmount = bfAmount;




            //            //if (reader["TotalSale"] != System.DBNull.Value)
            //            //{
            //            //    totalsale = (double)reader["TotalSale"];
            //            //}
            //            //if (reader["GatOther"] != System.DBNull.Value)
            //            //{
            //            //    otherExpencess = (double)reader["GatOther"];
            //            //}
            //            //if (reader["MemoDiscount"] != System.DBNull.Value)
            //            //{
            //            //    memoDiscount = (double)reader["MemoDiscount"];
            //            //}
            //            //if (reader["LastPurchase"] != System.DBNull.Value)
            //            //{
            //            //    customer.MemoDate = (DateTime)reader["LastPurchase"];
            //            //}
            //            /////*******************
            //            string queryStringSales = @"SELECT   
            //                                            dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.CustomerId, dbo.MemoMasters.MemoDate, dbo.MemoMasters.MemoNo, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, 
            //                                            SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalSale, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS MemoDiscount, 
            //                                            SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) - SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS NetSale
            //                                            FROM            
            //                                            dbo.MemoMasters INNER JOIN
            //                                            dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
            //                                            GROUP BY dbo.MemoMasters.CustomerId, dbo.MemoMasters.MemoNo, dbo.MemoMasters.MemoDate, dbo.MemoMasters.ShowRoomId
            //                                            HAVING        
            //                                            (dbo.MemoMasters.CustomerId = @customerId) AND (dbo.MemoMasters.ShowRoomId = @showRoomId)
            //                                            ORDER BY dbo.MemoMasters.MemoDate";

            //            using (SqlConnection connectionSale = new SqlConnection(connectionString))
            //            {
            //                SqlCommand commandSale = new SqlCommand(queryStringSales, connectionSale);
            //                connectionSale.Open();
            //                commandSale.Parameters.Add(new SqlParameter("@customerId", cid));
            //                commandSale.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
            //                SqlDataReader readerSale = commandSale.ExecuteReader();
            //                try
            //                {
            //                    mDiscount = 0;
            //                    eOthers = 0;
            //                    tSale = 0;
            //                    while (readerSale.Read())
            //                    {
            //                        if (readerSale["TotalSale"] != System.DBNull.Value)
            //                        {
            //                            tSale = tSale + (double)readerSale["TotalSale"];
            //                        }
            //                        if (readerSale["GatOther"] != System.DBNull.Value)
            //                        {
            //                            eOthers = eOthers + (double)readerSale["GatOther"];
            //                        }
            //                        if (readerSale["MemoDiscount"] != System.DBNull.Value)
            //                        {
            //                            mDiscount = mDiscount + (double)readerSale["MemoDiscount"];
            //                        }
            //                        if (readerSale["MemoDate"] != System.DBNull.Value)
            //                        {
            //                            customer.MemoDate = (DateTime)readerSale["MemoDate"];
            //                        }
            //                    }
            //                }
            //                finally
            //                {
            //                    readerSale.Close();
            //                }
            //            }

            //            //if (Convert.ToInt32(reader["CustomerId"]) == 1009)
            //            //{
            //            //    int check = 1;
            //            //}

            //             totalsale = tSale;    
            //             memoDiscount = mDiscount;
            //             otherExpencess = eOthers;

            //            customer.SaleCost = Math.Round(totalsale - memoDiscount, 0);
            //            customer.MemoDiscount = memoDiscount;
            //            customer.GatOther = otherExpencess;
                        
            //            customer.Balance = Math.Round(bfAmount + totalsale + otherExpencess - memoDiscount - adjustmentDiscount - paymentAmount, 0);
            //            list.Add(customer);
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
            ////ViewBag.AccountUserList = BankAccounts;
            //return Ok(list);
        }


        // GET: api/Customer/GetCustomerList/
        [Route("api/Customer/GetCustomerList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetCustomerList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;


            var customerList = db.Customers
                .Include(c => c.SalesMan)
                .Include(c => c.Upazila)
                .Include(c => c.Upazila.District)
                .Include(c => c.Upazila.District.SaleZone)
                .Include(c => c.Upazila.District.SaleZone.Division)
                .Include(c => c.Upazila.District.SaleZone.ZoneManager)
                .Select(c => new {
                    id = c.CustomerId,
                    CustomerId = c.CustomerId,
                    name = c.CustomerName,
                    group = c.UpazilaId,
                    groupName = c.Upazila.UpazilaName,
                    status = c.SalesManId,
                    statusName = c.SalesMan.SalesManName,
                    c.Address,
                    c.AddressBangla,
                    c.CustomerNameBangla,
                    c.Phone,
                    c.Email,
                    c.Image,
                    c.CreditLimit,
                    c.ShowRoomId,
                    c.ShowRoom.ShowRoomName,
                    c.ShopName,
                    c.UnitId,
                    c.Active
                })
                .Where(c => c.UnitId== unitId)
                .ToList();


            var salesManList = db.SalesMen
                .Include(s => s.ShowRoom)
                .Include(s => s.ShowRoom.Unit)
                .Where(s => s.ShowRoom.Unit.UnitId == unitId)
                .Select(e => new { value = e.SalesManId, text = e.SalesManName });

            //var upazilaList = db.Upazilas
            //    .Select(e => new { id = e.UpazilaId, text = e.UpazilaName })
            //    .OrderBy(e => e.text);

            return Ok(new { customerList , salesManList });

            //List<ShowRoomView> ImportProductList = new List<ShowRoomView>();
            //ShowRoomView importProduct = new ShowRoomView();

            //string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            //string queryString = @"SELECT        
            //                        dbo.Customers.CustomerId AS id, dbo.Customers.CustomerName AS name, dbo.Customers.CustomerId AS [group], dbo.Upazilas.UpazilaName AS groupName, dbo.Customers.SalesManId AS status, 
            //                        dbo.SalesMen.SalesManName AS statusName, dbo.Customers.Address, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.ShowRoomId, dbo.Customers.CreditLimit, 
            //                        dbo.Customers.CustomerNameBangla, dbo.Customers.AddressBangla, dbo.Customers.ShopName, dbo.Customers.UnitId
            //                        FROM            
            //                        dbo.Customers INNER JOIN
            //                        dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
            //                        dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId
            //                        WHERE        
            //                        (dbo.Customers.UnitId = @unitId)";

            //using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    SqlCommand command = new SqlCommand(queryString, connection);
            //    connection.Open();
            //    command.Parameters.Add(new SqlParameter("@unitId", unitId));
            //    SqlDataReader reader = command.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            int id = (int)reader["id"];
            //            string name = (string)reader["name"];
            //            int group = (int)reader["group"];
            //            string groupName = (string)reader["groupName"];

            //            int status = (int)reader["status"];
            //            string statusName = (string)reader["statusName"];

            //            string address= "";
            //            string phone = "";
            //            string email = "";
            //            string shopName = "";

            //            if (reader["ShopName"] != System.DBNull.Value)
            //            {
            //                shopName = (string) reader["ShopName"];
            //            }
            //            if (reader["Address"] != System.DBNull.Value)
            //            {
            //                address = (string) reader["Address"];
            //            }
            //            if (reader["Phone"] != System.DBNull.Value)
            //            {
            //                phone = (string) reader["Phone"];
            //            }
            //            if (reader["Email"] != System.DBNull.Value)
            //            {
            //                email = (string) reader["Email"];
            //            }

            //            importProduct = new ShowRoomView();
            //            importProduct.id = id;
            //            importProduct.name = name;
            //            importProduct.group = group;
            //            importProduct.groupName = groupName;
            //            importProduct.status = status;
            //            importProduct.statusName = statusName;
            //            importProduct.ShowRoomId = showRoomId;
            //            importProduct.Address = address;
            //            importProduct.Phone = phone;
            //            importProduct.Email = email;
            //            if (User.IsInRole("Show Room Manager") || User.IsInRole("Show Room Sales")) {
            //                importProduct.ShowRoomId = (int) reader["ShowRoomId"];
            //            }


            //            importProduct.ShopName = shopName;
            //            if (reader["AddressBangla"] != DBNull.Value)
            //            {
            //                importProduct.AddressBangla = (string)reader["AddressBangla"];
            //            }
            //            if (reader["CustomerNameBangla"] != DBNull.Value)
            //            {
            //                importProduct.CustomerNameBangla = (string)reader["CustomerNameBangla"];
            //            }
            //            if (reader["CreditLimit"] != DBNull.Value)
            //            {
            //                importProduct.CreditLimit = (double)reader["CreditLimit"];
            //            }
            //            ImportProductList.Add(importProduct);
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
            //ViewBag.AccountUserList = BankAccounts;
            //return Ok(ImportProductList);
        }
        [Route("api/Customer/UpazilaListXEdit")]
        [HttpGet]
        public IHttpActionResult GetUpazilaListXEdit()
        {
            var list = db.Upazilas
                            .Select(e => new { id = e.UpazilaId, text = e.UpazilaName })
                            .OrderBy(e => e.text);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/Customer/GetDropDownList/
        [Route("api/Customer/CustomerDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetCustomerDropDownList()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();
            var zoneManagerId = db.ZoneManagers.Where(a => a.Id == userId).Select(a => a.ZoneManagerIdAlias).FirstOrDefault();
            var managerZoneList = db.SaleZones.Where(a => a.ZoneManagerId == zoneManagerId).Select(a => a.SaleZoneId ).ToArray();
            var inIds = String.Join(",", managerZoneList.Select(x => x.ToString()).ToArray());
            List<CustomerView> list = new List<CustomerView>();

            if (User.IsInRole("Cash Sale"))
            {
                var cashParty = db.Customers
                    .Where(c => c.CustomerName == "Cash Party" && c.ShowRoomId == showRoomId)
                    .Select(c => new {
                        c.CustomerId,
                        c.CustomerName,
                        c.CustomerNameBangla,
                        c.AddressBangla,
                        c.Address,
                        c.CreditLimit,
                        c.ShopName
                    })
                    .ToArray();
                if (cashParty.Length>0) {
                    CustomerView aObj = new CustomerView();
                    aObj.CustomerId = cashParty[0].CustomerId;
                    aObj.CustomerName = cashParty[0].CustomerName;
                    aObj.CustomerNameBangla = cashParty[0].CustomerNameBangla;
                    aObj.AddressBangla = cashParty[0].AddressBangla;
                    aObj.CreditLimit = cashParty[0].CreditLimit;
                    aObj.ShopName = cashParty[0].ShopName;
                    aObj.Address = cashParty[0].Address;
                    list.Add(aObj);
                } 
            }
            else
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
                connection.Open();
                try
                {
                    SqlDataReader reader = null;
                    string sql = "";

                    if (zoneManagerId > 0)
                    {
                        sql = "SELECT dbo.Customers.CustomerId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.ShopName, dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Image, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.CreditLimit, dbo.Customers.Active, dbo.Customers.UnitId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Upazilas.UpazilaName, dbo.Upazilas.UpazilaNameBangla, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.Districts.DistrictNameBangla, dbo.Districts.SaleZoneId, dbo.SaleZones.SaleZoneName, dbo.SaleZones.SaleZoneDescription, dbo.SaleZones.DivisionId, dbo.Divisions.DivisionName, dbo.Divisions.DivisionNameBangla, dbo.SaleZones.ZoneManagerId, dbo.ZoneManagers.ZoneManagerName FROM dbo.Customers INNER JOIN dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN dbo.SaleZones ON dbo.Districts.SaleZoneId = dbo.SaleZones.SaleZoneId INNER JOIN dbo.Divisions ON dbo.SaleZones.DivisionId = dbo.Divisions.DivisionId INNER JOIN dbo.ZoneManagers ON dbo.SaleZones.ZoneManagerId = dbo.ZoneManagers.ZoneManagerId WHERE (dbo.Customers.UnitId = @unitId) AND (dbo.Districts.SaleZoneId IN(" + inIds + "))";
                    }
                    else
                    {
                        sql = @"SELECT        
                            dbo.Customers.CustomerId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.ShopName, 
                            dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.Image, dbo.Customers.CreditLimit, dbo.SalesMen.SalesManName, dbo.Units.UnitName, 
                            dbo.Customers.UnitId
                            FROM            
                            dbo.Customers INNER JOIN
                            dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                            dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                            dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                            dbo.Units ON dbo.Customers.UnitId = dbo.Units.UnitId
                            WHERE        
                            (dbo.Customers.ShowRoomId = @showRoomId) ORDER BY CustomerName ASC";
                    }
                    SqlCommand command = new SqlCommand(sql, connection);
                    //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                    command.Parameters.Add(new SqlParameter("@unitId", unitId));
                    command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                    reader = command.ExecuteReader();
                    if (User.IsInRole("Zone Manager") && unitId == 2)
                    {
                        CustomerView aObj = new CustomerView();
                        aObj.CustomerId = 5735;
                        aObj.CustomerName = "Cash Party";
                        aObj.CustomerNameBangla = "ক্যাশ পার্টি";
                        aObj.AddressBangla = "Dhaka, Bangladesh";
                        aObj.CreditLimit = 0;
                        aObj.ShopName = "Cash Party";
                        list.Add(aObj);
                    }
                    while (reader.Read())
                    {
                        CustomerView aObj = new CustomerView();
                        aObj.CustomerId = (int)reader["CustomerId"];
                        aObj.CustomerName = (string)reader["CustomerName"];
                        aObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        aObj.AddressBangla = (string)reader["AddressBangla"];
                        aObj.CreditLimit = (double)reader["CreditLimit"];
                        aObj.ShopName = (string)reader["ShopName"];
                        if (reader["Image"] != DBNull.Value)
                        {
                            aObj.Image = (string)reader["Image"];
                        }

                        list.Add(aObj);
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                if (list == null)
                {
                    return NotFound();
                }
            }// Cash Sale Logic

            return Ok(list);
        }


        // GET: api/Customer/ZoneWiseCustomerClosingBalanceList/
        [Route("api/Customer/ZoneWiseCustomerClosingBalanceList")]
        [HttpGet]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetZoneWiseCustomerClosingBalanceList()
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();
            var zoneManagerId = db.ZoneManagers.Where(a => a.Id == userId).Select(a => a.ZoneManagerIdAlias).FirstOrDefault();
            var managerZoneList = db.SaleZones.Where(a => a.ZoneManagerId == zoneManagerId).Select(a => a.SaleZoneId).ToArray();

            var inIds = String.Join(",", managerZoneList.Select(x => x.ToString()).ToArray());

            List<CustomerView> list = new List<CustomerView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                SqlDataReader reader = null;
                string sql = "";

                sql = "SELECT dbo.Customers.CustomerId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.ShopName, dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Image, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.CreditLimit, dbo.Customers.Active, dbo.Customers.UnitId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Upazilas.UpazilaName, dbo.Upazilas.UpazilaNameBangla, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.Districts.DistrictNameBangla, dbo.Districts.SaleZoneId, dbo.SaleZones.SaleZoneName, dbo.SaleZones.SaleZoneDescription, dbo.SaleZones.DivisionId, dbo.Divisions.DivisionName, dbo.Divisions.DivisionNameBangla, dbo.SaleZones.ZoneManagerId, dbo.ZoneManagers.ZoneManagerName FROM dbo.Customers INNER JOIN dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN dbo.SaleZones ON dbo.Districts.SaleZoneId = dbo.SaleZones.SaleZoneId INNER JOIN dbo.Divisions ON dbo.SaleZones.DivisionId = dbo.Divisions.DivisionId INNER JOIN dbo.ZoneManagers ON dbo.SaleZones.ZoneManagerId = dbo.ZoneManagers.ZoneManagerId WHERE (dbo.Customers.UnitId = @unitId) AND (dbo.Districts.SaleZoneId IN(" + inIds + "))";
                
                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@unitId", unitId));
                //command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {




                    CustomerView aObj = new CustomerView();
                    aObj.CustomerId = (int)reader["CustomerId"];
                    aObj.CustomerName = (string)reader["CustomerName"];
                    aObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                    aObj.AddressBangla = (string)reader["AddressBangla"];
                    aObj.CreditLimit = (double)reader["CreditLimit"];
                    aObj.ShopName = (string)reader["ShopName"];
                    aObj.UpazilaName = (string)reader["UpazilaName"];
                    aObj.DistrictName = (string)reader["DistrictName"];
                    aObj.SaleZoneName = (string)reader["SaleZoneName"];
                    aObj.ZoneManagerName = (string)reader["ZoneManagerName"];
                    aObj.DivisionName = (string)reader["DivisionName"];
                    if (reader["Image"] != DBNull.Value)
                    {
                        aObj.Image = (string)reader["Image"];
                    }
                    var customerTransctionSum = db.Customers
                            .Include(c => c.MemoMasters)
                            .Include(c => c.Payments)
                            .Select(c => new
                            {
                                c.CustomerId,
                                CustomerName = c.CustomerName,
                                MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
                                GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                                TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                            })
                            .Where(c => c.CustomerId == aObj.CustomerId)
                            .ToArray();

                    aObj.BfAmount = customerTransctionSum[0].TotalBf;
                    aObj.TotalSale = customerTransctionSum[0].GrossSales - customerTransctionSum[0].MemoDiscount + customerTransctionSum[0].GatOther;
                    aObj.TotalCollection = customerTransctionSum[0].TotalPayments;
                    aObj.TotalDiscount = customerTransctionSum[0].TotalDiscounts;
                    aObj.ActualCredit = customerTransctionSum[0].TotalBf+customerTransctionSum[0].GrossSales - customerTransctionSum[0].MemoDiscount + customerTransctionSum[0].GatOther- customerTransctionSum[0].TotalPayments- customerTransctionSum[0].TotalDiscounts;

                    list.Add(aObj);
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }


        //public static Customer[] GetProducts(int[] cusIDs)
        //{
        //    PCBookWebAppContext context = new PCBookWebAppContext();
        //    return (from p in context.Customers
        //            where cusIDs.Contains(p.CustomerId)
        //            select p).ToArray<Customer>();
        //}


        [Route("api/Customer/GetSingleCustomer/{id}")]
        [HttpGet]
        [ResponseType(typeof(CustomerView))]
        public IHttpActionResult GetSingleCustomer(int id)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();

            var list = db.Customers
                .Include(c => c.ShowRoom)
                .Include(c => c.Upazila)
                .Include(c => c.Unit)
                .Include(c => c.SalesMan)
                .Include(c => c.MemoMasters)
                .Include(c => c.Payments)
                .Include(c => c.Upazila.District)
                .Include(c => c.Upazila.District.SaleZone)
                .Include(c => c.Upazila.District.SaleZone.ZoneManager)
                .Include(c => c.Upazila.District.SaleZone.Division)
                .Select(c => new
                {
                    UnitId = c.UnitId,
                    CustonerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    SalesManName = c.SalesMan.SalesManName,
                    ShopName = c.ShopName,
                    Address = c.Address,
                    Phone = c.Phone,
                    CreditLimit = c.CreditLimit,
                    UpazilaId = c.Upazila.UpazilaId,
                    UpazilaName = c.Upazila.UpazilaName,
                    DistrictId = c.Upazila.District.DistrictId,
                    DistrictName = c.Upazila.District.DistrictName,
                    SaleZoneId = c.Upazila.District.SaleZone.SaleZoneId,
                    SaleZoneName = c.Upazila.District.SaleZone.SaleZoneName,
                    ZoneManagerId = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerId,
                    ZoneManagerName = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                    DivisionId = c.Upazila.District.SaleZone.Division.DivisionId,
                    DivisionName = c.Upazila.District.SaleZone.Division.DivisionName,
                    MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
                    GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
                    GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                    BfDate = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.PaymentDate }).Max(s => (DateTime?)s.PaymentDate) ?? null,
                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                    TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                })
                .Where(c => c.CustonerId == id)
                .ToArray();
                
                
                //.GroupBy(x => new { x.CustonerId })
                //        .Select(
                //                g => new
                //                {
                //                    Key = g.Key,
                //                    UnitId = g.First().UnitId,
                //                    CustonerId = g.First().CustonerId,
                //                    CustomerName = g.First().CustomerName,
                //                    Address = g.First().Address,
                //                    Phone = g.First().Phone,
                //                    CreditLimit =g.First().CreditLimit,
                //                    DistrictName = g.First().DistrictName,
                //                    UpazilaName = g.First().UpazilaName,
                //                    SaleZoneId = g.First().SaleZoneId,
                //                    SaleZoneName = g.First().SaleZoneName,
                //                    ZoneManagerName = g.First().ZoneManagerName,
                //                    DivisionName = g.First().DivisionName,
                //                    TotalGroupCount = g.Count(),
                //                    MemoDiscount = g.First().MemoDiscount,
                //                    GatOther = g.First().GatOther,
                //                    TotalPayments = g.First().TotalPayments,
                //                    ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                //                    TotalBf = g.First().TotalBf,
                //                    TotalDiscounts = g.First().TotalDiscounts,
                //                    Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments
                //                });



            return Ok(list);
        }

        [Route("api/Customer/UpdateImageNameToCustomers/{CustomerId}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateImageNameToCustomers(int CustomerId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            var aCustomer = db.Customers
                .Where(x => x.CustomerId == CustomerId)
                .FirstOrDefault();

            if (aCustomer != null)
            {
                aCustomer.Image = CustomerId.ToString()+".jpg";
                db.Customers.AddOrUpdate(aCustomer);
                db.SaveChanges();
            }
            return Ok();
        }


        [Route("api/Customer/GetCustomerTypeAheadList/{SearchTerm}")]
        [HttpGet]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult GetCustomerTypeAheadList(string SearchTerm)
        {
            string userName = User.Identity.GetUserName();
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();
            var zoneManagerId = db.ZoneManagers.Where(a => a.Id == userId).Select(a => a.ZoneManagerIdAlias).FirstOrDefault();
            var managerZoneList = db.SaleZones.Where(a => a.ZoneManagerId == zoneManagerId).Select(a => a.SaleZoneId).ToArray();

            var inIds = String.Join(",", managerZoneList.Select(x => x.ToString()).ToArray());
            List<Customer> typeAheadList = new List<Customer>();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader ledgerReader = null;
                string sql = "";
                if (zoneManagerId > 0)
                {
                    sql = "SELECT dbo.Customers.CustomerId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.ShopName, dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Image, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.CreditLimit, dbo.Customers.Active, dbo.Customers.UnitId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Upazilas.UpazilaName, dbo.Upazilas.UpazilaNameBangla, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.Districts.DistrictNameBangla, dbo.Districts.SaleZoneId, dbo.SaleZones.SaleZoneName, dbo.SaleZones.SaleZoneDescription, dbo.SaleZones.DivisionId, dbo.Divisions.DivisionName, dbo.Divisions.DivisionNameBangla, dbo.SaleZones.ZoneManagerId, dbo.ZoneManagers.ZoneManagerName FROM dbo.Customers INNER JOIN dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN dbo.SaleZones ON dbo.Districts.SaleZoneId = dbo.SaleZones.SaleZoneId INNER JOIN dbo.Divisions ON dbo.SaleZones.DivisionId = dbo.Divisions.DivisionId INNER JOIN dbo.ZoneManagers ON dbo.SaleZones.ZoneManagerId = dbo.ZoneManagers.ZoneManagerId WHERE (dbo.Customers.UnitId = @unitId) AND (dbo.Districts.SaleZoneId IN(" + inIds + ")) AND (dbo.Customers.CustomerName LIKE @searchTerm) ORDER BY CustomerName ASC";
                }
                else {
                    sql = @"SELECT        
                            dbo.Customers.CustomerId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.ShopName, 
                            dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.Image, dbo.Customers.CreditLimit, dbo.SalesMen.SalesManName, dbo.Units.UnitName, 
                            dbo.Customers.UnitId
                            FROM            
                            dbo.Customers INNER JOIN
                            dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                            dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                            dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                            dbo.Units ON dbo.Customers.UnitId = dbo.Units.UnitId
                            WHERE        
                            (dbo.Customers.ShowRoomId = @showRoomId) AND (dbo.Customers.CustomerName LIKE @searchTerm) ORDER BY CustomerName ASC";  
                }
           
                
                string searchTerm = string.Format("{0}%", SearchTerm);

                SqlCommand command = new SqlCommand(sql, checkConnection);
                command.Parameters.Add(new SqlParameter("@unitId", unitId));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));
                ledgerReader = command.ExecuteReader();
                if (User.IsInRole("Zone Manager") && unitId == 2)
                {
                    Customer aObj = new Customer();
                    aObj.CustomerId = 5735;
                    aObj.CustomerName = "Cash Party";
                    aObj.CustomerNameBangla = "ক্যাশ পার্টি";
                    aObj.AddressBangla = "Dhaka, Bangladesh";
                    aObj.CreditLimit = 0;
                    aObj.ShopName = "Cash Party";
                    typeAheadList.Add(aObj);
                }
                while (ledgerReader.Read())
                {
                    Customer customerObj = new Customer();
                    customerObj.CustomerId = (int)ledgerReader["CustomerId"];
                    customerObj.CustomerName = (string)ledgerReader["CustomerName"];
                    customerObj.CustomerNameBangla = (string)ledgerReader["CustomerNameBangla"];
                    customerObj.Address = (string)ledgerReader["Address"];
                    customerObj.AddressBangla = ledgerReader["AddressBangla"].ToString();
                    customerObj.ShopName = ledgerReader["ShopName"].ToString();
                    customerObj.CreditLimit = (double)ledgerReader["CreditLimit"];
                    typeAheadList.Add(customerObj);
                }
                ledgerReader.Close();
                checkConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Ok(typeAheadList);
        }

        // GET: api/Customer/ZoneCustomerList/
        [Route("api/Customer/ZoneCustomerList")]
        [HttpGet]
        [ResponseType(typeof(ZoneCustomerView))]
        public IHttpActionResult GetZoneCustomerList(string FromDate="2018-01-01", string ToDate="2018-05-31")
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);

            //var linkQListDateBeteen = db.Customers
            //                    .Include(c => c.Upazila)
            //                    .Include(c => c.ShowRoom)
            //                    .Include(c => c.SalesMan)
            //                    .Include(c => c.ShowRoom.Unit)
            //                    .Include(c => c.MemoMasters)
            //                    .Include(c => c.Payments)
            //                    .Include(c => c.Upazila.District)
            //                    .Include(c => c.Upazila.District.SaleZone)
            //                    .Include(c => c.Upazila.District.SaleZone.ZoneManager)
            //                    .Include(c => c.Upazila.District.SaleZone.Division)
            //                    .Select(c => new {
            //                        UnitId = c.UnitId,
            //                        CustonerId = c.CustomerId,
            //                        CustomerName = c.CustomerName,
            //                        SalesManName = c.SalesMan.SalesManName,
            //                        ShopName = c.ShopName,
            //                        Address = c.Address,
            //                        Phone = c.Phone,
            //                        UpazilaId = c.Upazila.UpazilaId,
            //                        UpazilaName = c.Upazila.UpazilaName,
            //                        DistrictId = c.Upazila.District.DistrictId,
            //                        DistrictName = c.Upazila.District.DistrictName,
            //                        SaleZoneId = c.Upazila.District.SaleZone.SaleZoneId,
            //                        SaleZoneName = c.Upazila.District.SaleZone.SaleZoneName,
            //                        ZoneManagerId = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerId,
            //                        ZoneManagerName = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
            //                        DivisionId = c.Upazila.District.SaleZone.Division.DivisionId,
            //                        DivisionName = c.Upazila.District.SaleZone.Division.DivisionName,
            //                        MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

            //                        OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
            //                        OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
            //                        OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

            //                        MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
            //                        GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
            //                        GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

            //                        OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate <fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
            //                        OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
            //                        OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

            //                        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
            //                        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
            //                        TotalDiscounts = c.Payments.Where(s=> s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
            //                    })
            //                    .Where(c => (c.UnitId == unitId && c.CustomerName != "Cash Party")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Print")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Textile")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Fabrics")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Store")
            //                                    && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur"))
            //                    .ToList()
            //                    .GroupBy(x => new { x.UnitId, x.CustonerId })
            //                    .Select(
            //                            g => new
            //                            {
            //                                Key = g.Key,
            //                                UnitId = g.First().UnitId,
            //                                CustonerId = g.First().CustonerId,
            //                                CustomerName = g.First().CustomerName,
            //                                Address = g.First().Address,
            //                                Phone = g.First().Phone,
            //                                DistrictName = g.First().DistrictName,
            //                                UpazilaName = g.First().UpazilaName,
            //                                SaleZoneName = g.First().SaleZoneName,
            //                                ZoneManagerName = g.First().ZoneManagerName,
            //                                DivisionName = g.First().DivisionName,
            //                                TotalGroupCount = g.Count(),
            //                                Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
            //                                MemoDiscount = g.First().MemoDiscount,
            //                                GatOther = g.First().GatOther,
            //                                TotalPayments = g.First().TotalPayments,
            //                                ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
            //                                TotalBf = g.First().TotalBf,
            //                                TotalDiscounts = g.First().TotalDiscounts,
            //                                Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
            //                            });






            var linkQList = db.Customers
                                .Include(c => c.Upazila)
                                .Include(c => c.ShowRoom)
                                .Include(c => c.SalesMan)
                                .Include(c => c.ShowRoom.Unit)
                                .Include(c => c.MemoMasters)
                                .Include(c => c.Payments)
                                .Include(c => c.Upazila.District)
                                .Include(c => c.Upazila.District.SaleZone)
                                .Include(c => c.Upazila.District.SaleZone.ZoneManager)
                                .Include(c => c.Upazila.District.SaleZone.Division)
                                .Select(c => new
                                {
                                    UnitId = c.UnitId,
                                    CustonerId = c.CustomerId,
                                    CustomerName = c.CustomerName,
                                    SalesManName = c.SalesMan.SalesManName,
                                    ShopName = c.ShopName,
                                    Address = c.Address,
                                    Phone = c.Phone,
                                    UpazilaId = c.Upazila.UpazilaId,
                                    UpazilaName = c.Upazila.UpazilaName,
                                    DistrictId = c.Upazila.District.DistrictId,
                                    DistrictName = c.Upazila.District.DistrictName,
                                    SaleZoneId = c.Upazila.District.SaleZone.SaleZoneId,
                                    SaleZoneName = c.Upazila.District.SaleZone.SaleZoneName,
                                    ZoneManagerId = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerId,
                                    ZoneManagerName = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                                    DivisionId = c.Upazila.District.SaleZone.Division.DivisionId,
                                    DivisionName = c.Upazila.District.SaleZone.Division.DivisionName,
                                    MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
                                    GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                    GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                    TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                                })
                                .Where(c => (c.UnitId == unitId && c.CustomerName != "Cash Party")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Print")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Textile")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Fabrics")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Store")
                                                && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur"))
                                .ToList()
                                .GroupBy(x => new { x.UnitId, x.CustonerId })
                                .Select(
                                        g => new
                                        {
                                            Key = g.Key,
                                            UnitId = g.First().UnitId,
                                            CustonerId = g.First().CustonerId,
                                            CustomerName = g.First().CustomerName,
                                            Address = g.First().Address,
                                            Phone = g.First().Phone,
                                            DistrictName = g.First().DistrictName,
                                            UpazilaName = g.First().UpazilaName,
                                            SaleZoneName = g.First().SaleZoneName,
                                            ZoneManagerName = g.First().ZoneManagerName,
                                            DivisionName = g.First().DivisionName,
                                            TotalGroupCount = g.Count(),
                                            MemoDiscount = g.First().MemoDiscount,
                                            GatOther = g.First().GatOther,
                                            TotalPayments = g.First().TotalPayments,
                                            ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                            TotalBf = g.First().TotalBf,
                                            TotalDiscounts = g.First().TotalDiscounts,
                                            Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments
                                        });

            //var linkQZoneList = db.Customers
            //    .Include(c => c.Upazila)
            //    .Include(c => c.Unit)
            //    .Include(c => c.SalesMan)
            //    .Include(c => c.MemoMasters)
            //    .Include(c => c.Payments)
            //    .Include(c => c.Upazila.District)
            //    .Include(c => c.Upazila.District.SaleZone)
            //    .Include(c => c.Upazila.District.SaleZone.ZoneManager)
            //    .Include(c => c.Upazila.District.SaleZone.Division)
            //    .Select(c => new {
            //        UnitId = c.UnitId,
            //        SaleZoneId=c.Upazila.District.SaleZone.SaleZoneId,
            //        SaleZoneName = c.Upazila.District.SaleZone.SaleZoneName,
            //        ZoneManagerId = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerId,
            //        ZoneManagerName = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
            //        DivisionId = c.Upazila.District.SaleZone.Division.DivisionId,
            //        DivisionName = c.Upazila.District.SaleZone.Division.DivisionName,
            //        MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
            //        GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
            //        GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
            //        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
            //        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
            //        TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
            //    })
            //    .Where(c => c.UnitId == unitId)
            //    .ToList().GroupBy(x => new { x.UnitId, x.SaleZoneId })
            //            .Select(
            //                    g => new
            //                    {
            //                        Key = g.Key,
            //                        UnitId = g.First().UnitId,
            //                        SaleZoneName = g.First().SaleZoneName,
            //                        ZoneManagerName = g.First().ZoneManagerName,
            //                        DivisionName = g.First().DivisionName,
            //                        TotalGroupCount = g.Count(),
            //                        MemoDiscount = g.First().MemoDiscount,
            //                        GatOther = g.First().GatOther,
            //                        TotalPayments = g.First().TotalPayments,
            //                        ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
            //                        TotalBf = g.First().TotalBf,
            //                        TotalDiscounts = g.First().TotalDiscounts,
            //                        Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments
            //                    });

            //var linkQZoneManagerList = db.Customers
            //    .Include(c => c.Upazila)
            //    .Include(c => c.Unit)
            //    .Include(c => c.SalesMan)
            //    .Include(c => c.MemoMasters)
            //    .Include(c => c.Payments)
            //    .Include(c => c.Upazila.District)
            //    .Include(c => c.Upazila.District.SaleZone)
            //    .Include(c => c.Upazila.District.SaleZone.ZoneManager)
            //    .Include(c => c.Upazila.District.SaleZone.Division)
            //    .Select(c => new {
            //        UnitId = c.UnitId,
            //        ZoneManagerId = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerId,
            //        ZoneManagerName = c.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
            //        MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
            //        GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
            //        GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
            //        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
            //        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
            //        TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
            //    })
            //    .Where(c => c.UnitId == unitId )
            //    .ToList().GroupBy(x => new { x.UnitId, x.ZoneManagerId })
            //            .Select(
            //                    g => new
            //                    {
            //                        Key = g.Key,
            //                        UnitId = g.First().UnitId,
            //                        ZoneManagerName = g.First().ZoneManagerName,
            //                        TotalGroupCount = g.Count(),
            //                        MemoDiscount = g.First().MemoDiscount,
            //                        GatOther = g.First().GatOther,
            //                        TotalPayments = g.First().TotalPayments,
            //                        ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
            //                        TotalBf = g.First().TotalBf,
            //                        TotalDiscounts = g.First().TotalDiscounts,
            //                        Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments
            //                    });


            //var linkQDivisionList = db.Customers
            //        .Include(c => c.Upazila)
            //        .Include(c => c.Unit)
            //        .Include(c => c.SalesMan)
            //        .Include(c => c.MemoMasters)
            //        .Include(c => c.Payments)
            //        .Include(c => c.Upazila.District)
            //        .Include(c => c.Upazila.District.SaleZone)
            //        .Include(c => c.Upazila.District.SaleZone.ZoneManager)
            //        .Include(c => c.Upazila.District.SaleZone.Division)
            //        .Select(c => new
            //        {
            //            UnitId = c.UnitId,
            //            CustomerName = c.CustomerName,
            //            DivisionId = c.Upazila.District.SaleZone.Division.DivisionId,
            //            DivisionName = c.Upazila.District.SaleZone.Division.DivisionName,
            //            MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
            //            GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
            //            GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?) s.MemoCost) ?? 0,
            //            TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
            //            TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
            //            TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
            //        })
            //        .Where(c => (c.UnitId == unitId && c.CustomerName != "Cash Party")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Print")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Textile")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Fabrics")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Store")
            //                    && (c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur"))
            //        .ToList().GroupBy(x => new { x.UnitId, x.DivisionId })
            //                .Select(
            //                        g => new
            //                        {
            //                            Key = g.Key,
            //                            UnitId = g.First().UnitId,
            //                            DivisionName = g.First().DivisionName,
            //                            TotalGroupCount = g.Count(),
            //                            MemoDiscount = g.First().MemoDiscount,
            //                            GatOther = g.First().GatOther,
            //                            TotalPayments = g.First().TotalPayments,
            //                            ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
            //                            TotalBf = g.First().TotalBf,
            //                            TotalDiscounts = g.First().TotalDiscounts,
            //                            Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments
            //                        });

            //return Ok( new { customerList = linkQList,
            //                 zoneWiseCustomerList =linkQZoneList,
            //                 divisionWiseCustomerList = linkQDivisionList ,
            //                 zoneManagerWiseCustomerList = linkQZoneManagerList
            //            });

            //var linkQDivisionList = from client in db.Customers
            //                        join clientDiscount in db.Payments on client.CustomerId equals clientDiscount.CustomerId into d
            //                        join clientOrder in db.MemoMasters on client.CustomerId equals clientOrder.CustomerId into o
            //                        where (client.UnitId == unitId && client.CustomerName != "Cash Party")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party PSC Islampur (Alomgir)")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party Pakiza Print")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party Pakiza Textile")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party Pakiza Fabrics")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party Pakiza Store")
            //                                && (client.UnitId == unitId && client.CustomerName != "Cash Party PSC Islampur")
            //                        //group client by new { client.ShowRoom.Unit.UnitId, client.Upazila.District.SaleZone.Division.DivisionId } into o
            //                        //orderby client.Upazila.District.SaleZone.Division.DivisionId 
            //                        select new
            //                        {
            //                            //Client = client,
            //                            CustomerName = client.CustomerName,
            //                            DivisionName = client.Upazila.District.SaleZone.Division.DivisionName,
            //                            TotalSale = o.Sum(orderSummary => (double?)orderSummary.MemoCost) ?? 0,
            //                            ActualSales = o.Sum(orderSummary => (double?)orderSummary.MemoCost) ?? 0 + (double?)o.Sum(orderSummary => orderSummary.GatOther) ?? 0 - o.Sum(orderSummary => (double?)orderSummary.MemoDiscount) ?? 0,
            //                            TotalPayments = d.Sum(discountSummary => (double?)discountSummary.SCAmount) ?? 0,
            //                            TotalDiscounts = d.Sum(discountSummary => (double?)discountSummary.SDiscount) ?? 0,
            //                            TotalBf = d.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
            //                            Balance = (((double?)o.Sum(orderSummary => (double?)orderSummary.MemoCost) ?? 0 +
            //                                        (double?)o.Sum(orderSummary => (double?)orderSummary.GatOther) ?? 0 -
            //                                        (double?)o.Sum(orderSummary => (double?)orderSummary.MemoDiscount) ?? 0) -
            //                                        ((double?)d.Sum(discountSummary => (double?)discountSummary.SCAmount) ?? 0 +
            //                                        (double?)d.Sum(discountSummary => (double?)discountSummary.SDiscount) ?? 0)),
            //                        };


            return Ok( new
            {
                customerList = linkQList,
            });
        }


        [Route("api/Customer/PartyLedger/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(MemoView))]
        public IHttpActionResult GetPartyLedger(string FromDate, string ToDate, int? CustomerId)
        {
            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);

            var bfList = db.Customers
                .Include(c => c.MemoMasters)
                .Include(c => c.Payments)
                .Select(c => new {
                    CustonerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    MemoDate = c.MemoMasters.Select(a => a.MemoDate),
                    PaymentDate = c.Payments.Select(a => a.PaymentDate),
                    MemoDiscount = c.MemoMasters.Where(a => a.MemoDate < fdate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                    GatOther = c.MemoMasters.Where(a => a.MemoDate < fdate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                    GrossSales = c.MemoMasters.Where(a => a.MemoDate < fdate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fdate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fdate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                    TotalDiscounts = c.Payments.Where(s => s.PaymentDate < fdate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                })
                .Where(c => c.CustonerId == CustomerId) 
                .ToList();

            //Payments
            var paymentsList = db.Payments
                .Include(p => p.Customer)
                .Where(p => p.CustomerId == CustomerId && p.PaymentDate >= fdate && p.PaymentDate <= tdate)
                .Select(p => new
                {
                    p.Customer.CustomerId,
                    p.Customer.CustomerName,
                    p.PaymentId,
                    p.PaymentDate,
                    p.SCAmount,
                    p.SDiscount,
                    p.Remarks,
                    p.CheckNo,
                    p.PaymentType,
                    p.BankAccountNo,
                    p.HonourDate

                }).ToList();

            //Sales
            var saleList = db.MemoMasters
                .Include(m=> m.Customer)
                .Where(m => m.CustomerId == CustomerId && m.MemoDate >= fdate && m.MemoDate <= tdate)
                .Select(m => new {
                    m.MemoMasterId,
                    m.Customer.CustomerName,
                    m.MemoDate,
                    m.MemoNo,
                    m.GatOther,
                    m.MemoDiscount,
                    m.MemoCost,
                    ActualMemoAmount = m.MemoCost,
                    NetMemoAmount = m.MemoCost - m.MemoDiscount + m.GatOther
                })
                .ToList();


            // Grouping List
            var categoryGroupingList = db.MemoDetails
                                            .Include(m => m.MemoMaster)
                                            .Include(m => m.Product)
                                            .Include(m => m.Product.SubCategory)
                                            .Include(m => m.Product.SubCategory.MainCategory)
                                            .Where(m => m.MemoMaster.CustomerId == CustomerId && m.MemoMaster.MemoDate >= fdate && m.MemoMaster.MemoDate <= tdate)
                                            .Select(m => new
                                            {
                                                m.MemoDetailId,
                                                m.ProductId,
                                                m.Product.ProductName,
                                                m.Quantity,
                                                m.Rate,
                                                m.Discount,
                                                m.Product.SubCategoryId,
                                                m.Product.SubCategory.SubCategoryName,
                                                m.Product.SubCategory.MainCategoryId,
                                                m.Product.SubCategory.MainCategory.MainCategoryName
                                            }).ToList();

            return Ok(new { paymentsList, saleList, categoryGroupingList, bfList });
        }





        // GET: api/Customer
        public IQueryable<Customer> GetCustomers()
        {
            return db.Customers;
        }

        // GET: api/Customer/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            //var customer = db.Customers
            //                    .Where(a => a.CustomerId == id)
            //                    .Select(e => new { CustomerId = e.CustomerId, CustomerName = e.CustomerName, Address = e.Address, Phone = e.Phone, Email = e.Email });

            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customer/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomer(int id, Customer customer)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            if (showRoomId != 0)
            {
                customer.ShowRoomId = showRoomId;
            }
            else
            {
                customer.ShowRoomId = null;
            }
            customer.UnitId = unitId;
            customer.DateCreated = ceatedAt;
            customer.DateUpdated = ceatedAt;
            customer.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/Customer/UpdateCustomer/{id}")]
        [ResponseType(typeof(Customer))]
        [HttpPut]
        public IHttpActionResult PutUpdateCustomer(int id, Customer customer)
        {
            string currentUserId = User.Identity.GetUserId();
            string currentUserName = User.Identity.GetUserName();

            var aCustomer = db.Customers.Where(c => c.CustomerId == id).FirstOrDefault();
            aCustomer.CustomerName = customer.CustomerName;
            aCustomer.CustomerNameBangla = customer.CustomerNameBangla;
            aCustomer.Address = customer.Address;
            aCustomer.Phone = customer.Phone;
            aCustomer.Email = customer.Email;
            aCustomer.ShopName = customer.ShopName;
            aCustomer.AddressBangla = customer.AddressBangla;
            aCustomer.SalesManId = customer.SalesManId;
            aCustomer.UpazilaId = customer.UpazilaId;
            aCustomer.Image = null;
            aCustomer.DateUpdated = DateTime.Now;
            db.Customers.AddOrUpdate(aCustomer);
            db.SaveChanges();

            return Ok();
        }


        // POST: api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            if (customer.ShowRoomId == 0)
            {
                customer.ShowRoomId = null;
            }

            customer.UnitId = unitId;
            customer.DateCreated = ceatedAt;
            customer.DateUpdated = ceatedAt;
            customer.CreatedBy = userName;

            if (db.Customers.Any(m => m.CustomerName == customer.CustomerName && m.ShowRoomId == showRoomId))
            {
                ModelState.AddModelError("CustomerName", "Customer Name Already Exists!");
            }



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customer/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}