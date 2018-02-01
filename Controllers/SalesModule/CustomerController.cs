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

namespace PCBookWebApp.Controllers
{
    public class CustomerController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();


        // GET: api/Customer/GetCustomerList/
        [Route("api/Customer/GetCustomerList")]
        [HttpGet]
        [ResponseType(typeof(ShowRoomView))]
        public IHttpActionResult GetCustomerList()
        {
            List<ShowRoomView> ImportProductList = new List<ShowRoomView>();
            ShowRoomView importProduct = new ShowRoomView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.Customers.CustomerId AS id, dbo.Customers.CustomerName AS name, dbo.Customers.CustomerId AS [group], dbo.Upazilas.UpazilaName AS groupName, dbo.Customers.SalesManId AS status, 
                                    dbo.SalesMen.SalesManName AS statusName, dbo.Customers.Address, dbo.Customers.Phone, dbo.Customers.Email
                                    FROM            
                                    dbo.Customers 
                                    INNER JOIN
                                    dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                    dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId";

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

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

                        if (reader["Address"] != System.DBNull.Value)
                        {
                            address = (string)reader["Address"];
                        }
                        if (reader["Phone"] != System.DBNull.Value)
                        {
                            phone = (string)reader["Phone"];
                        }
                        if (reader["Email"] != System.DBNull.Value)
                        {
                            email = (string)reader["Email"];
                        }

                        importProduct = new ShowRoomView();
                        importProduct.id = id;
                        importProduct.name = name;
                        importProduct.group = group;
                        importProduct.groupName = groupName;
                        importProduct.status = status;
                        importProduct.statusName = statusName;

                        importProduct.Address = address;
                        importProduct.Phone = phone;
                        importProduct.Email = email;
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
            var list = db.Customers.Select(e => new { CustomerId = e.CustomerId, CustomerName = e.CustomerName });
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
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