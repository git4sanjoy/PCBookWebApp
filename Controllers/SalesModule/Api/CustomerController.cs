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



        [Route("api/Customer/GetCustomerBalance")]
        [HttpGet]
        [ResponseType(typeof(CustomerBalanceView))]
        public IHttpActionResult GetCustomerBalance()
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
                                    dbo.MemoMasters.CustomerId, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, 
                                    SUM(DISTINCT dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalCost, SUM(DISTINCT dbo.Payments.SCAmount) AS SCAmount, SUM(DISTINCT dbo.Payments.SDiscount) 
                                    AS SDiscount, MAX(dbo.MemoMasters.MemoDate) AS MemoDate, MAX(dbo.Payments.PaymentDate) AS PaymentDate, dbo.Upazilas.UpazilaName, dbo.Customers.UpazilaId, dbo.Upazilas.DistrictId, 
                                    dbo.Districts.DistrictName, dbo.SalesMen.SalesManName, dbo.Customers.SalesManId, dbo.MemoMasters.ShowRoomId, dbo.Customers.CustomerName, dbo.Customers.ShopName
                                    FROM            
                                    dbo.MemoMasters INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId INNER JOIN
                                    dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                    dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                    dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId LEFT OUTER JOIN
                                    dbo.Payments ON dbo.Customers.CustomerId = dbo.Payments.CustomerId
                                    GROUP BY dbo.MemoMasters.CustomerId, dbo.Upazilas.UpazilaName, dbo.Customers.UpazilaId, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.SalesMen.SalesManName, dbo.Customers.SalesManId, 
                                    dbo.MemoMasters.ShowRoomId, dbo.Customers.CustomerName, dbo.Customers.ShopName 
                                    HAVING               
                                    (dbo.MemoMasters.ShowRoomId = @showRoomId)";

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
                        double paymentAmount = 0;
                        double sDiscount = 0;
                        customer.CustomerName = (string)reader["CustomerName"];
                        customer.ShopName = (string)reader["ShopName"];
                        customer.UpazilaName = (string)reader["UpazilaName"];
                        customer.DistrictName = (string)reader["DistrictName"];
                        customer.SalesManName = (string)reader["SalesManName"];
                        customer.SaleCost=(double)reader["TotalCost"];
                        customer.MemoDiscount = (double)reader["MemoDiscount"];
                        customer.GatOther = (double)reader["GatOther"];
                        customer.DistrictName= (string)reader["DistrictName"];
                        if (reader["SCAmount"] != System.DBNull.Value)
                        {
                            paymentAmount = (double)reader["SCAmount"];
                        }
                        if (reader["SDiscount"] != System.DBNull.Value)
                        {
                            sDiscount = (double)reader["SDiscount"];
                        }
                        if (reader["MemoDate"] != System.DBNull.Value)
                        {
                            customer.MemoDate = (DateTime)reader["MemoDate"];
                        }
                        if (reader["PaymentDate"] != System.DBNull.Value)
                        {
                            customer.PaymentDate = (DateTime)reader["PaymentDate"];
                        }
                        customer.PaymentAmount = paymentAmount;
                        customer.Adjustment = sDiscount;
                        customer.Balance = (double)reader["TotalCost"] + (double)reader["GatOther"] - (double)reader["MemoDiscount"]- paymentAmount- sDiscount;
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


        // GET: api/Customer/GetCustomerList/
        [Route("api/Customer/GetCustomerList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetCustomerList()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            List<ShowRoomView> ImportProductList = new List<ShowRoomView>();
            ShowRoomView importProduct = new ShowRoomView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Customers.CustomerId AS id, dbo.Customers.CustomerName AS name, dbo.Customers.CustomerId AS [group], dbo.Upazilas.UpazilaName AS groupName, dbo.Customers.SalesManId AS status, 
                                    dbo.SalesMen.SalesManName AS statusName, dbo.Customers.Address, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.ShowRoomId, dbo.Customers.CreditLimit, 
                                    dbo.Customers.CustomerNameBangla, dbo.Customers.AddressBangla, dbo.Customers.ShopName
                                    FROM            
                                    dbo.Customers INNER JOIN
                                    dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                    dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId
                                    WHERE        
                                    (dbo.Customers.ShowRoomId = @showRoomId)";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        string name = (string)reader["name"];
                        int group = (int)reader["group"];
                        string groupName = (string)reader["groupName"];

                        int status = (int)reader["status"];
                        string statusName = (string)reader["statusName"];

                        string address= "";
                        string phone = "";
                        string email = "";
                        string shopName = "";

                        if (reader["ShopName"] != System.DBNull.Value)
                        {
                            shopName = (string) reader["ShopName"];
                        }
                        if (reader["Address"] != System.DBNull.Value)
                        {
                            address = (string) reader["Address"];
                        }
                        if (reader["Phone"] != System.DBNull.Value)
                        {
                            phone = (string) reader["Phone"];
                        }
                        if (reader["Email"] != System.DBNull.Value)
                        {
                            email = (string) reader["Email"];
                        }
                        
                        importProduct = new ShowRoomView();
                        importProduct.id = id;
                        importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        importProduct.status = status;
                        importProduct.statusName = statusName;
                        importProduct.ShowRoomId = showRoomId;
                        importProduct.Address = address;
                        importProduct.Phone = phone;
                        importProduct.Email = email;
                        importProduct.ShowRoomId = (int)reader["ShowRoomId"];
                        importProduct.ShopName = shopName;
                        if (reader["AddressBangla"] != DBNull.Value)
                        {
                            importProduct.AddressBangla = (string)reader["AddressBangla"];
                        }
                        if (reader["CustomerNameBangla"] != DBNull.Value)
                        {
                            importProduct.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        }
                        if (reader["CreditLimit"] != DBNull.Value)
                        {
                            importProduct.CreditLimit = (double)reader["CreditLimit"];
                        }
                        ImportProductList.Add(importProduct);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            //ViewBag.AccountUserList = BankAccounts;
            return Ok(ImportProductList);
        }


        // GET: api/Customer/GetDropDownList/
        [Route("api/Customer/GetDropDownList")]
        [HttpGet]
        [ResponseType(typeof(Unit))]
        public IHttpActionResult GetDropDownList()
        {
            var list = db.Customers
                .OrderBy(a=> a.CustomerName)
                .Select(e => new { CustomerId = e.CustomerId, CustomerName = e.CustomerName, CustomerNameBangla = e.CustomerNameBangla, aAddressBanglaaa = e.AddressBangla, CreditLimit = e.CreditLimit });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/Customer/GetDropDownList/
        [Route("api/Customer/GetSingleCustomer/{id}")]
        [HttpGet]
        [ResponseType(typeof(CustomerView))]
        public IHttpActionResult GetSingleCustomer(int id)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            List<CustomerView> list = new List<CustomerView>();
            CustomerView aObj = new CustomerView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Customers.CustomerId AS id, dbo.Customers.CustomerName AS name, dbo.Customers.CustomerId AS [group], dbo.Upazilas.UpazilaName AS groupName, dbo.Customers.SalesManId AS status, 
                                    dbo.SalesMen.SalesManName AS statusName, dbo.Customers.Address, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.ShowRoomId, dbo.Customers.CreditLimit, 
                                    dbo.Customers.CustomerNameBangla, dbo.Customers.AddressBangla, dbo.Customers.ShopName, dbo.Customers.Image, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName
                                    FROM            
                                    dbo.Customers INNER JOIN
                                    dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                    dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                    dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId
                                    WHERE        
                                    (dbo.Customers.ShowRoomId = @showRoomId) AND (dbo.Customers.CustomerId=@customerId)";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                command.Parameters.Add(new SqlParameter("@customerId", id));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        int idc = (int)reader["id"];
                        string name = (string)reader["name"];
                        int group = (int)reader["group"];
                        string groupName = (string)reader["groupName"];

                        int status = (int)reader["status"];
                        string statusName = (string)reader["statusName"];

                        string address = "";
                        string phone = "";
                        string email = "";
                        string shopName = "";
                        string image = "";
                        if (reader["ShopName"] != DBNull.Value)
                        {
                            shopName = (string)reader["ShopName"];
                        }
                        if (reader["Address"] != DBNull.Value)
                        {
                            address = (string)reader["Address"];
                        }
                        if (reader["Phone"] != DBNull.Value)
                        {
                            phone = (string)reader["Phone"];
                        }
                        if (reader["Email"] != DBNull.Value)
                        {
                            email = (string)reader["Email"];
                        }
                        if (reader["Image"] != DBNull.Value)
                        {
                            image = (string)reader["Image"];
                        }
                        double totalSaleAmount = 0;
                        double totalOtherCost = 0;
                        double totalMemoDiscount = 0;
                        double totalCollectionAmount = 0;
                        double totalDiscountAmount = 0;
                        var sells = db.MemoDetails
                                        .Where(a => a.MemoMaster.CustomerId == id && a.MemoMaster.ShowRoomId == showRoomId)
                                        .GroupBy(a => a.MemoMaster.CustomerId)
                                        .Select(a => new { Amount = a.Sum(b => (b.Quantity * (b.Rate - b.Discount))) })
                                        .FirstOrDefault();

                        var sellsOthers = db.MemoMasters
                                            .Where(a => a.CustomerId == id && a.ShowRoomId == showRoomId)
                                            .GroupBy(a => a.CustomerId)
                                            .Select(a => new { Others = a.Sum(b => (b.GatOther)), MemoDiscount = a.Sum(b => (b.MemoDiscount)) })
                                            .FirstOrDefault();

                        var collection = db.Payments
                                            .Where(a => a.CustomerId == id && a.ShowRoomId == showRoomId)
                                            .GroupBy(a => a.CustomerId)
                                            .Select(a => new { SCAmount = a.Sum(b => (b.SCAmount)), SDiscount = a.Sum(b => (b.SDiscount)) })
                                            .FirstOrDefault();

                        if (sells != null && sells.Amount > 0) {
                            totalSaleAmount = (double) sells.Amount;
                            if (sellsOthers!=null && sellsOthers.Others > 0) {
                                totalOtherCost = (double) sellsOthers.Others;
                            }
                            if (sellsOthers != null && sellsOthers.MemoDiscount > 0)
                            {
                                totalMemoDiscount = (double) sellsOthers.MemoDiscount;
                            }
                            if (collection != null && collection.SCAmount > 0) {
                                totalCollectionAmount = (double) collection.SCAmount;
                            }
                            if (collection != null && collection.SDiscount > 0) {
                                totalDiscountAmount = (double)collection.SDiscount;
                            }
                        }
                        double actualCustomerBalance = totalSaleAmount + totalOtherCost - totalMemoDiscount -(totalCollectionAmount+ totalDiscountAmount);

                        //string aa = @"SELECT        
                        //                dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId, AVG(dbo.MemoMasters.MemoDiscount) AS MemoDiscount, AVG(dbo.MemoMasters.GatOther) AS GatOther, 
                        //                SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalProductCost
                        //                FROM            
                        //                dbo.MemoMasters INNER JOIN
                        //                dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                        //                GROUP BY dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId
                        //                HAVING        
                        //                (dbo.MemoMasters.CustomerId = 1) AND (dbo.MemoMasters.ShowRoomId = 2)";

                        //var query = from p in db.MemoMasters
                        //            join pf in db.MemoDetails on p.MemoMasterId equals pf.MemoMasterId
                        //            where p.CustomerId == id && p.ShowRoomId == showRoomId
                        //            group p by new { p.CustomerId, p.ShowRoomId };



                        aObj = new CustomerView();
                        aObj.id = idc;
                        aObj.name = name;
                        aObj.group = group;
                        aObj.groupName = groupName;
                        aObj.status = status;
                        aObj.statusName = statusName;
                        aObj.ShowRoomId = showRoomId;
                        aObj.Address = address;
                        aObj.Phone = phone;
                        aObj.Email = email;
                        aObj.TotalCredit = actualCustomerBalance;
                        aObj.ShopName = shopName;
                        aObj.Image = image;
                        if (reader["AddressBangla"] != DBNull.Value)
                        {
                            aObj.AddressBangla = (string)reader["AddressBangla"];
                        }
                        if (reader["CustomerNameBangla"] != DBNull.Value)
                        {
                            aObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        }
                        if (reader["CreditLimit"] != DBNull.Value)
                        {
                            aObj.CreditLimit = (double)reader["CreditLimit"];
                        }
                        list.Add(aObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
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
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            List<Customer> typeAheadList = new List<Customer>();
            SqlConnection checkConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            checkConnection.Open();
            try
            {
                SqlDataReader ledgerReader = null;
                string sql = @"SELECT        
                                dbo.Customers.CustomerId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, 
                                dbo.Customers.ShopName, dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.Image, dbo.Customers.CreditLimit, 
                                dbo.SalesMen.SalesManName
                                FROM            
                                dbo.Customers INNER JOIN
                                dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId
                                WHERE        
                                (dbo.Customers.ShowRoomId = @showRoomId) AND (dbo.Customers.CustomerName LIKE @searchTerm) ORDER BY CustomerName ASC";

                string searchTerm = string.Format("{0}%", SearchTerm);

                SqlCommand command = new SqlCommand(sql, checkConnection);
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                command.Parameters.Add(new SqlParameter("@searchTerm", searchTerm));
                ledgerReader = command.ExecuteReader();
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
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            customer.ShowRoomId = showRoomId;
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

        // POST: api/Customer
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            customer.ShowRoomId = showRoomId;
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