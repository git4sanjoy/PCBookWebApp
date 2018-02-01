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
using PCBookWebApp.Models.SalesModule;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.SalesModule.ViewModel;

namespace PCBookWebApp.Controllers.SalesModule.Api
{
    public class MemoMastersController : ApiController
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();



        [Route("api/MemoMasters/GetMemoId")]
        [HttpGet]
        [ResponseType(typeof(MemoMaster))]
        public IHttpActionResult GetMemoId()
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            DateTime bdate = DateTime.Now;
            string currentMonth = bdate.Month.ToString();
            string currentYear = bdate.Year.ToString();


            MemoMaster voucherObj = new MemoMaster();
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    CAST(ISNULL(MAX(RIGHT(MemoNo, 6)), 0) + 1 AS INT) AS NewId, YEAR(MemoDate) AS Year, ShowRoomId
                                    FROM            
                                    dbo.MemoMasters
                                    GROUP BY YEAR(MemoDate), ShowRoomId
                                    HAVING            
                                    (YEAR(MemoDate) = @year)  AND (ShowRoomId = @showRoomId)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@year", currentYear));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        voucherObj = new MemoMaster();
                        if (reader["NewId"] != DBNull.Value)
                        {
                            voucherObj.MemoMasterId = (int) reader["NewId"];
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(voucherObj);
        }

        [Route("api/MemoMasters/GetMemo/{MemoNo}")]
        [HttpGet]
        [ResponseType(typeof(MemoMaster))]
        public IHttpActionResult GetMemo(string MemoNo)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            //var memoMaster = db.MemoDetails
            //    .Include(a=> a.MemoMaster)
            //    .Where(a=> a.MemoMaster.MemoNo == MemoNo && a.MemoMaster.ShowRoomId == showRoomId);
            //if (memoMaster == null)
            //{
            //    return NotFound();
            //}

            List<MemoView> list = new List<MemoView>();
            MemoView memoObj = new MemoView();


            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.MemoMasters.MemoDate, dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.MemoNo, dbo.MemoMasters.MemoDiscount, dbo.MemoMasters.GatOther, 
                                    dbo.MemoMasters.ExpencessRemarks, dbo.MemoMasters.MemoMasterId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.Address, dbo.Customers.AddressBangla, 
                                    dbo.ShowRooms.ShowRoomName, dbo.ShowRooms.ShowRoomNameBangla
                                    FROM            
                                    dbo.MemoMasters INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.ShowRooms ON dbo.MemoMasters.ShowRoomId = dbo.ShowRooms.ShowRoomId AND dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId       
                                    WHERE 
                                    (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoNo = @memoNo)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@memoNo", MemoNo));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        memoObj = new MemoView();
                        memoObj.MemoMasterId = (int)reader["MemoMasterId"];
                        memoObj.MemoNo = (string)reader["MemoNo"];
                        memoObj.CustomerName = (string)reader["CustomerName"];
                        memoObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        memoObj.MemoDate = (DateTime)reader["MemoDate"];
                        memoObj.Address = (string)reader["Address"];
                        memoObj.AddressBangla = (string)reader["AddressBangla"];
                        memoObj.ShowRoomName = (string)reader["ShowRoomName"];
                        memoObj.ShowRoomNameBangla = (string)reader["ShowRoomNameBangla"];
                        if (reader["MemoDiscount"] != DBNull.Value)
                        {
                            memoObj.MemoDiscount = (double)reader["MemoDiscount"];
                        }
                        if (reader["GatOther"] != DBNull.Value)
                        {
                            memoObj.GatOther = (double)reader["GatOther"];
                        }
                        if (reader["ExpencessRemarks"] != DBNull.Value)
                        {
                            memoObj.ExpencessRemarks = (string)reader["ExpencessRemarks"];
                        }
                        //var collection = db.Payments
                        //                    .Where(a => a.MemoMasterId == (int)reader["MemoMasterId"] && a.ShowRoomId == showRoomId)
                        //                    .GroupBy(a => a.MemoMasterId)
                        //                    .Select(a => new { MemoPaidAmount = a.Sum(b => (b.SCAmount)) })
                        //                    .FirstOrDefault();
                        //var memoPaidAmount = db.Payments
                        //                   .Where(a => a.MemoMasterId == (int)reader["MemoMasterId"] && a.ShowRoomId == showRoomId)
                        //                   .Select(a => a.SCAmount)
                        //                   .FirstOrDefault();
                        double memoPaidAmount = 0;
                        string queryString2 = @"SELECT        
                                                PaymentId, MemoMasterId, CustomerId, ShowRoomId, PaymentDate, SSAmount, TSAmount, SCAmount, TCAmount, SDiscount, TDiscount, PaymentType, HonourDate, CheckNo, BankAccountNo, Remarks
                                                FROM            
                                                dbo.Payments WHERE (MemoMasterId = @memoMasterId)";
                        using (SqlConnection connectionPayment = new SqlConnection(connectionString))
                        {
                            SqlCommand commandPayment = new SqlCommand(queryString2, connectionPayment);
                            connectionPayment.Open();
                            commandPayment.Parameters.Add(new SqlParameter("@memoMasterId", (int)reader["MemoMasterId"]));
                            SqlDataReader readerPayment = commandPayment.ExecuteReader();
                            try
                            {
                                while (readerPayment.Read())
                                {
                                    if (readerPayment["SCAmount"] != DBNull.Value)
                                    {
                                        memoPaidAmount = (double)readerPayment["SCAmount"];
                                    }
                                }
                            }
                            finally
                            {
                                readerPayment.Close();
                            }
                        }
                        memoObj.MemoPaidAmount = memoPaidAmount;


                        List<MemoDetailView> memoDetailList = new List<MemoDetailView>();
                        MemoDetailView memoDetailObj = new MemoDetailView();
                        string queryStringDetails = @"SELECT        
                                                        dbo.MemoDetails.ProductId, dbo.MemoDetails.Than, dbo.MemoDetails.Quantity, dbo.MemoDetails.Rate, dbo.MemoDetails.Discount, dbo.MemoDetails.MemoMasterId, dbo.MemoDetails.MemoDetailId, 
                                                        dbo.Products.ProductName, dbo.Products.ProductNameBangla, dbo.Products.Image
                                                        FROM            
                                                        dbo.MemoDetails INNER JOIN
                                                        dbo.Products ON dbo.MemoDetails.ProductId = dbo.Products.ProductId
                                                        WHERE (MemoMasterId = @memoMasterId)";
                        using (SqlConnection connectionDetails = new SqlConnection(connectionString))
                        {
                            SqlCommand commandDetails = new SqlCommand(queryStringDetails, connectionDetails);
                            connectionDetails.Open();

                            commandDetails.Parameters.Add(new SqlParameter("@memoMasterId", (int)memoObj.MemoMasterId));
                            SqlDataReader readerDetails = commandDetails.ExecuteReader();
                            try
                            {
                                while (readerDetails.Read())
                                {
                                    memoDetailObj = new MemoDetailView();
                                    memoDetailObj.MemoDetailId = (int)readerDetails["MemoDetailId"];
                                    memoDetailObj.ProductId = (int)readerDetails["ProductId"];
                                    memoDetailObj.ProductName = (string)readerDetails["ProductName"];
                                    memoDetailObj.ProductNameBangla = (string)readerDetails["ProductNameBangla"];
                                    memoDetailObj.Quantity = (double)readerDetails["Quantity"];
                                    memoDetailObj.Rate = (double)readerDetails["Rate"];
                                    memoDetailObj.Discount = (double)readerDetails["Discount"];
                                    if (readerDetails["Image"] != DBNull.Value)
                                    {
                                        memoDetailObj.Image = (string)reader["Image"];
                                    }
                                    memoDetailList.Add(memoDetailObj);
                                }
                            }
                            finally
                            {
                                readerDetails.Close();
                            }
                        }
                        memoObj.MemoDetailViews = memoDetailList;
                        list.Add(memoObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(list);

        }

        [Route("api/MemoMasters/GetMemoByDate/{MemoDate}")]
        [HttpGet]
        [ResponseType(typeof(MemoMaster))]
        public IHttpActionResult GetMemoByDate(string MemoDate)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            DateTime memoDate = DateTime.Parse(MemoDate);
            List<MemoView> list = new List<MemoView>();
            MemoView memoObj = new MemoView();


            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = @"SELECT        
                                    dbo.MemoMasters.MemoDate, dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.MemoNo, dbo.MemoMasters.MemoDiscount, dbo.MemoMasters.GatOther, 
                                    dbo.MemoMasters.ExpencessRemarks, dbo.MemoMasters.MemoMasterId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.Address, dbo.Customers.AddressBangla, 
                                    dbo.ShowRooms.ShowRoomName, dbo.ShowRooms.ShowRoomNameBangla
                                    FROM            
                                    dbo.MemoMasters INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.ShowRooms ON dbo.MemoMasters.ShowRoomId = dbo.ShowRooms.ShowRoomId AND dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId       
                                    WHERE 
                                    (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate = @memoDate)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@memoDate", memoDate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        memoObj = new MemoView();
                        memoObj.MemoMasterId = (int)reader["MemoMasterId"];
                        memoObj.MemoNo = (string)reader["MemoNo"];
                        memoObj.CustomerName = (string)reader["CustomerName"];
                        memoObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        memoObj.MemoDate = (DateTime)reader["MemoDate"];
                        memoObj.Address = (string)reader["Address"];
                        memoObj.AddressBangla = (string)reader["AddressBangla"];
                        memoObj.ShowRoomName = (string)reader["ShowRoomName"];
                        memoObj.ShowRoomNameBangla = (string)reader["ShowRoomNameBangla"];
                        if (reader["MemoDiscount"] != DBNull.Value)
                        {
                            memoObj.MemoDiscount = (double)reader["MemoDiscount"];
                        }
                        if (reader["GatOther"] != DBNull.Value)
                        {
                            memoObj.GatOther = (double)reader["GatOther"];
                        }
                        if (reader["ExpencessRemarks"] != DBNull.Value)
                        {
                            memoObj.ExpencessRemarks = (string)reader["ExpencessRemarks"];
                        }
                        //var collection = db.Payments
                        //                    .Where(a => a.MemoMasterId == (int)reader["MemoMasterId"] && a.ShowRoomId == showRoomId)
                        //                    .GroupBy(a => a.MemoMasterId)
                        //                    .Select(a => new { MemoPaidAmount = a.Sum(b => (b.SCAmount)) })
                        //                    .FirstOrDefault();
                        //var memoPaidAmount = db.Payments
                        //                   .Where(a => a.MemoMasterId == (int)reader["MemoMasterId"] && a.ShowRoomId == showRoomId)
                        //                   .Select(a => a.SCAmount)
                        //                   .FirstOrDefault();
                        double memoPaidAmount = 0;
                        string queryString2 = @"SELECT        
                                                PaymentId, MemoMasterId, CustomerId, ShowRoomId, PaymentDate, SSAmount, TSAmount, SCAmount, TCAmount, SDiscount, TDiscount, PaymentType, HonourDate, CheckNo, BankAccountNo, Remarks
                                                FROM            
                                                dbo.Payments WHERE (MemoMasterId = @memoMasterId)";
                        using (SqlConnection connectionPayment = new SqlConnection(connectionString))
                        {
                            SqlCommand commandPayment = new SqlCommand(queryString2, connectionPayment);
                            connectionPayment.Open();
                            commandPayment.Parameters.Add(new SqlParameter("@memoMasterId", (int)reader["MemoMasterId"]));
                            SqlDataReader readerPayment = commandPayment.ExecuteReader();
                            try
                            {
                                while (readerPayment.Read())
                                {
                                    if (readerPayment["SCAmount"] != DBNull.Value)
                                    {
                                        memoPaidAmount = (double)readerPayment["SCAmount"];
                                    }
                                }
                            }
                            finally
                            {
                                readerPayment.Close();
                            }
                        }
                        memoObj.MemoPaidAmount = memoPaidAmount;


                        List<MemoDetailView> memoDetailList = new List<MemoDetailView>();
                        MemoDetailView memoDetailObj = new MemoDetailView();
                        string queryStringDetails = @"SELECT        
                                                        dbo.MemoDetails.ProductId, dbo.MemoDetails.Than, dbo.MemoDetails.Quantity, dbo.MemoDetails.Rate, dbo.MemoDetails.Discount, dbo.MemoDetails.MemoMasterId, dbo.MemoDetails.MemoDetailId, 
                                                        dbo.Products.ProductName, dbo.Products.ProductNameBangla, dbo.Products.Image
                                                        FROM            
                                                        dbo.MemoDetails INNER JOIN
                                                        dbo.Products ON dbo.MemoDetails.ProductId = dbo.Products.ProductId
                                                        WHERE (MemoMasterId = @memoMasterId)";
                        using (SqlConnection connectionDetails = new SqlConnection(connectionString))
                        {
                            SqlCommand commandDetails = new SqlCommand(queryStringDetails, connectionDetails);
                            connectionDetails.Open();

                            commandDetails.Parameters.Add(new SqlParameter("@memoMasterId", (int)memoObj.MemoMasterId));
                            SqlDataReader readerDetails = commandDetails.ExecuteReader();
                            try
                            {
                                while (readerDetails.Read())
                                {
                                    memoDetailObj = new MemoDetailView();
                                    memoDetailObj.MemoDetailId = (int)readerDetails["MemoDetailId"];
                                    memoDetailObj.ProductId = (int)readerDetails["ProductId"];
                                    memoDetailObj.ProductName = (string)readerDetails["ProductName"];
                                    memoDetailObj.ProductNameBangla = (string)readerDetails["ProductNameBangla"];
                                    memoDetailObj.Quantity = (double)readerDetails["Quantity"];
                                    memoDetailObj.Rate = (double)readerDetails["Rate"];
                                    memoDetailObj.Discount = (double)readerDetails["Discount"];
                                    if (readerDetails["Image"] != DBNull.Value)
                                    {
                                        memoDetailObj.Image = (string)reader["Image"];
                                    }
                                    memoDetailList.Add(memoDetailObj);
                                }
                            }
                            finally
                            {
                                readerDetails.Close();
                            }
                        }
                        memoObj.MemoDetailViews = memoDetailList;
                        list.Add(memoObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(list);

        }



        [Route("api/MemoMasters/GetMemoByDateBetween/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(MemoMaster))]
        public IHttpActionResult GetMemoByDateBetween(string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            List<MemoView> list = new List<MemoView>();
            MemoView memoObj = new MemoView();


            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = "";
            if (CustomerId > 0)
            {
                queryString = @"SELECT        
                                    dbo.MemoMasters.MemoDate, dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.MemoNo, dbo.MemoMasters.MemoDiscount, dbo.MemoMasters.GatOther, 
                                    dbo.MemoMasters.ExpencessRemarks, dbo.MemoMasters.MemoMasterId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.Address, dbo.Customers.AddressBangla, 
                                    dbo.ShowRooms.ShowRoomName, dbo.ShowRooms.ShowRoomNameBangla
                                    FROM            
                                    dbo.MemoMasters INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.ShowRooms ON dbo.MemoMasters.ShowRoomId = dbo.ShowRooms.ShowRoomId AND dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId       
                                    WHERE 
                                    (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate >= @fromDate) AND (dbo.MemoMasters.MemoDate <= @toDate) AND (dbo.MemoMasters.CustomerId = @customerId)";
            }
            else {
                queryString = @"SELECT        
                                    dbo.MemoMasters.MemoDate, dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.MemoNo, dbo.MemoMasters.MemoDiscount, dbo.MemoMasters.GatOther, 
                                    dbo.MemoMasters.ExpencessRemarks, dbo.MemoMasters.MemoMasterId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.Address, dbo.Customers.AddressBangla, 
                                    dbo.ShowRooms.ShowRoomName, dbo.ShowRooms.ShowRoomNameBangla
                                    FROM            
                                    dbo.MemoMasters INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.ShowRooms ON dbo.MemoMasters.ShowRoomId = dbo.ShowRooms.ShowRoomId AND dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId       
                                    WHERE 
                                    (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate >= @fromDate) AND (dbo.MemoMasters.MemoDate <= @toDate)";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@fromDate", fromDate));
                command.Parameters.Add(new SqlParameter("@toDate", toDate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                if (CustomerId > 0)
                {
                    command.Parameters.Add(new SqlParameter("@customerId", CustomerId));
                }
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        memoObj = new MemoView();
                        memoObj.MemoMasterId = (int)reader["MemoMasterId"];
                        memoObj.MemoNo = (string)reader["MemoNo"];
                        memoObj.CustomerName = (string)reader["CustomerName"];
                        memoObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        memoObj.MemoDate = (DateTime)reader["MemoDate"];
                        memoObj.Address = (string)reader["Address"];
                        memoObj.AddressBangla = (string)reader["AddressBangla"];
                        memoObj.ShowRoomName = (string)reader["ShowRoomName"];
                        memoObj.ShowRoomNameBangla = (string)reader["ShowRoomNameBangla"];
                        if (reader["MemoDiscount"] != DBNull.Value)
                        {
                            memoObj.MemoDiscount = (double)reader["MemoDiscount"];
                        }
                        if (reader["GatOther"] != DBNull.Value)
                        {
                            memoObj.GatOther = (double)reader["GatOther"];
                        }
                        if (reader["ExpencessRemarks"] != DBNull.Value)
                        {
                            memoObj.ExpencessRemarks = (string)reader["ExpencessRemarks"];
                        }

                        double memoPaidAmount = 0;
                        string queryString2 = @"SELECT        
                                                PaymentId, MemoMasterId, CustomerId, ShowRoomId, PaymentDate, SSAmount, TSAmount, SCAmount, TCAmount, SDiscount, TDiscount, PaymentType, HonourDate, CheckNo, BankAccountNo, Remarks
                                                FROM            
                                                dbo.Payments WHERE (MemoMasterId = @memoMasterId)";
                        using (SqlConnection connectionPayment = new SqlConnection(connectionString))
                        {
                            SqlCommand commandPayment = new SqlCommand(queryString2, connectionPayment);
                            connectionPayment.Open();
                            commandPayment.Parameters.Add(new SqlParameter("@memoMasterId", (int)reader["MemoMasterId"]));
                            SqlDataReader readerPayment = commandPayment.ExecuteReader();
                            try
                            {
                                while (readerPayment.Read())
                                {
                                    if (readerPayment["SCAmount"] != DBNull.Value)
                                    {
                                        memoPaidAmount = (double)readerPayment["SCAmount"];
                                    }
                                }
                            }
                            finally
                            {
                                readerPayment.Close();
                            }
                        }
                        memoObj.MemoPaidAmount = memoPaidAmount;


                        List<MemoDetailView> memoDetailList = new List<MemoDetailView>();
                        MemoDetailView memoDetailObj = new MemoDetailView();
                        string queryStringDetails = @"SELECT        
                                                        dbo.MemoDetails.ProductId, dbo.MemoDetails.Than, dbo.MemoDetails.Quantity, dbo.MemoDetails.Rate, dbo.MemoDetails.Discount, dbo.MemoDetails.MemoMasterId, dbo.MemoDetails.MemoDetailId, 
                                                        dbo.Products.ProductName, dbo.Products.ProductNameBangla, dbo.Products.Image, dbo.Products.SubCategoryId, dbo.SubCategories.SubCategoryName, dbo.SubCategories.MainCategoryId, 
                                                        dbo.MainCategories.MainCategoryName
                                                        FROM            
                                                        dbo.MemoDetails INNER JOIN
                                                        dbo.Products ON dbo.MemoDetails.ProductId = dbo.Products.ProductId INNER JOIN
                                                        dbo.SubCategories ON dbo.Products.SubCategoryId = dbo.SubCategories.SubCategoryId INNER JOIN
                                                        dbo.MainCategories ON dbo.SubCategories.MainCategoryId = dbo.MainCategories.MainCategoryId
                                                        WHERE (MemoMasterId = @memoMasterId)";
                        using (SqlConnection connectionDetails = new SqlConnection(connectionString))
                        {
                            SqlCommand commandDetails = new SqlCommand(queryStringDetails, connectionDetails);
                            connectionDetails.Open();

                            commandDetails.Parameters.Add(new SqlParameter("@memoMasterId", (int)memoObj.MemoMasterId));
                            SqlDataReader readerDetails = commandDetails.ExecuteReader();
                            try
                            {
                                while (readerDetails.Read())
                                {
                                    memoDetailObj = new MemoDetailView();
                                    memoDetailObj.MemoDetailId = (int)readerDetails["MemoDetailId"];
                                    memoDetailObj.ProductId = (int)readerDetails["ProductId"];
                                    memoDetailObj.ProductName = (string)readerDetails["ProductName"];
                                    memoDetailObj.ProductNameBangla = (string)readerDetails["ProductNameBangla"];
                                    memoDetailObj.Quantity = (double)readerDetails["Quantity"];
                                    memoDetailObj.Rate = (double)readerDetails["Rate"];
                                    memoDetailObj.Discount = (double)readerDetails["Discount"];
                                    memoDetailObj.SubCategoryId = (int)readerDetails["SubCategoryId"];
                                    memoDetailObj.MainCategoryId = (int)readerDetails["MainCategoryId"];
                                    memoDetailObj.SubCategoryName = (string)readerDetails["SubCategoryName"];
                                    memoDetailObj.MainCategoryName = (string)readerDetails["MainCategoryName"];
                                    if (readerDetails["Image"] != DBNull.Value)
                                    {
                                        memoDetailObj.Image = (string)reader["Image"];
                                    }
                                    memoDetailList.Add(memoDetailObj);
                                }
                            }
                            finally
                            {
                                readerDetails.Close();
                            }
                        }
                        memoObj.MemoDetailViews = memoDetailList;
                        list.Add(memoObj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return Ok(list);

        }

        [Route("api/MemoMasters/GetMemoByDateBetweenForCategoryWiseGrouping/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(MemoDetailView))]
        public IHttpActionResult GetMemoByDateBetweenForCategoryWiseGrouping(string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
                        List<MemoDetailView> memoDetailList = new List<MemoDetailView>();
                        MemoDetailView memoDetailObj = new MemoDetailView();
            string queryStringDetails = "";
            if (CustomerId > 0) {
                queryStringDetails = @"SELECT        
                                        dbo.MemoDetails.ProductId, dbo.MemoDetails.Than, dbo.MemoDetails.Quantity, dbo.MemoDetails.Rate, dbo.MemoDetails.Discount, dbo.MemoDetails.MemoMasterId, dbo.MemoDetails.MemoDetailId, 
                                        dbo.Products.ProductName, dbo.Products.ProductNameBangla, dbo.Products.Image, dbo.Products.SubCategoryId, dbo.SubCategories.SubCategoryName, dbo.SubCategories.MainCategoryId, 
                                        dbo.MainCategories.MainCategoryName, dbo.MemoMasters.MemoDate, dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId
                                        FROM            
                                        dbo.MemoDetails INNER JOIN
                                        dbo.Products ON dbo.MemoDetails.ProductId = dbo.Products.ProductId INNER JOIN
                                        dbo.SubCategories ON dbo.Products.SubCategoryId = dbo.SubCategories.SubCategoryId INNER JOIN
                                        dbo.MainCategories ON dbo.SubCategories.MainCategoryId = dbo.MainCategories.MainCategoryId INNER JOIN
                                        dbo.MemoMasters ON dbo.MemoDetails.MemoMasterId = dbo.MemoMasters.MemoMasterId
                                        WHERE        
                                       ((dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.CustomerId=@customerId) AND (dbo.MemoMasters.MemoDate >= CONVERT(DATETIME, @fromDate, 102) AND dbo.MemoMasters.MemoDate <= CONVERT(DATETIME, @toDate, 102)))";
            } else {
                queryStringDetails = @"SELECT        
                                        dbo.MemoDetails.ProductId, dbo.MemoDetails.Than, dbo.MemoDetails.Quantity, dbo.MemoDetails.Rate, dbo.MemoDetails.Discount, dbo.MemoDetails.MemoMasterId, dbo.MemoDetails.MemoDetailId, 
                                        dbo.Products.ProductName, dbo.Products.ProductNameBangla, dbo.Products.Image, dbo.Products.SubCategoryId, dbo.SubCategories.SubCategoryName, dbo.SubCategories.MainCategoryId, 
                                        dbo.MainCategories.MainCategoryName, dbo.MemoMasters.MemoDate, dbo.MemoMasters.CustomerId, dbo.MemoMasters.ShowRoomId
                                        FROM            
                                        dbo.MemoDetails INNER JOIN
                                        dbo.Products ON dbo.MemoDetails.ProductId = dbo.Products.ProductId INNER JOIN
                                        dbo.SubCategories ON dbo.Products.SubCategoryId = dbo.SubCategories.SubCategoryId INNER JOIN
                                        dbo.MainCategories ON dbo.SubCategories.MainCategoryId = dbo.MainCategories.MainCategoryId INNER JOIN
                                        dbo.MemoMasters ON dbo.MemoDetails.MemoMasterId = dbo.MemoMasters.MemoMasterId
                                        WHERE        
                                        (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate >= CONVERT(DATETIME, @fromDate, 102) AND dbo.MemoMasters.MemoDate <= CONVERT(DATETIME, @toDate, 102))";
            }

            using (SqlConnection connectionDetails = new SqlConnection(connectionString))
            {
                SqlCommand commandDetails = new SqlCommand(queryStringDetails, connectionDetails);
                connectionDetails.Open();

                commandDetails.Parameters.Add(new SqlParameter("@fromDate", fromDate));
                commandDetails.Parameters.Add(new SqlParameter("@toDate", toDate));
                commandDetails.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                if (CustomerId > 0)
                {
                    commandDetails.Parameters.Add(new SqlParameter("@customerId", CustomerId));
                }
                SqlDataReader readerDetails = commandDetails.ExecuteReader();
                try
                {
                    while (readerDetails.Read())
                    {
                        memoDetailObj = new MemoDetailView();
                        memoDetailObj.MemoDetailId = (int)readerDetails["MemoDetailId"];
                        memoDetailObj.ProductId = (int)readerDetails["ProductId"];
                        memoDetailObj.ProductName = (string)readerDetails["ProductName"];
                        memoDetailObj.ProductNameBangla = (string)readerDetails["ProductNameBangla"];
                        memoDetailObj.Quantity = (double)readerDetails["Quantity"];
                        memoDetailObj.Rate = (double)readerDetails["Rate"];
                        memoDetailObj.Discount = (double)readerDetails["Discount"];
                        memoDetailObj.SubCategoryId = (int)readerDetails["SubCategoryId"];
                        memoDetailObj.MainCategoryId = (int)readerDetails["MainCategoryId"];
                        memoDetailObj.SubCategoryName = (string)readerDetails["SubCategoryName"];
                        memoDetailObj.MainCategoryName = (string)readerDetails["MainCategoryName"];
                        if (readerDetails["Image"] != DBNull.Value)
                        {
                            memoDetailObj.Image = (string)readerDetails["Image"];
                        }
                        memoDetailList.Add(memoDetailObj);
                    }
                }
                finally
                {
                    readerDetails.Close();
                }
            }
  
            return Ok(memoDetailList);

        }

        [Route("api/MemoMasters/GetMemoMastersSummary/{FromDate}/{ToDate}/{CustomerId}")]
        [HttpGet]
        [ResponseType(typeof(MemoView))]
        public IHttpActionResult GetMemoMastersSummary(string FromDate, string ToDate, int? CustomerId)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);
            List<MemoView> list = new List<MemoView>();
            MemoView aObj = new MemoView();

            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = "";
            if (CustomerId > 0)
            {
                queryString = @"SELECT        
                                    dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.CustomerId, dbo.MemoMasters.MemoDate, dbo.MemoMasters.MemoNo, 
                                    SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalCost, AVG(dbo.MemoMasters.MemoDiscount) AS MemoDiscount, AVG(dbo.MemoMasters.GatOther) AS GatOther, 
                                    dbo.Customers.CustomerName
                                    FROM            
                                    dbo.MemoDetails INNER JOIN
                                    dbo.MemoMasters ON dbo.MemoDetails.MemoMasterId = dbo.MemoMasters.MemoMasterId INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId
                                    GROUP BY dbo.MemoMasters.MemoDate, dbo.MemoMasters.MemoNo, dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.CustomerId, dbo.Customers.CustomerName
                                    HAVING        
                                    (dbo.MemoMasters.MemoDate >= CONVERT(DATETIME, '2018-01-01 00:00:00', 102) AND dbo.MemoMasters.MemoDate <= CONVERT(DATETIME, '2018-06-01 00:00:00', 102)) AND 
                                    (dbo.MemoMasters.ShowRoomId = 1) AND (dbo.MemoMasters.CustomerId = 3)";
            }
            else
            {
                queryString = @"SELECT        
                                    dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.CustomerId, dbo.MemoMasters.MemoDate, dbo.MemoMasters.MemoNo, 
                                    SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalCost, AVG(dbo.MemoMasters.MemoDiscount) AS MemoDiscount, AVG(dbo.MemoMasters.GatOther) AS GatOther, 
                                    dbo.Customers.CustomerName
                                    FROM            
                                    dbo.MemoDetails INNER JOIN
                                    dbo.MemoMasters ON dbo.MemoDetails.MemoMasterId = dbo.MemoMasters.MemoMasterId INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId
                                    GROUP BY dbo.MemoMasters.MemoDate, dbo.MemoMasters.MemoNo, dbo.MemoMasters.ShowRoomId, dbo.MemoMasters.CustomerId, dbo.Customers.CustomerName
                                    HAVING        
                                    (dbo.MemoMasters.MemoDate >= CONVERT(DATETIME, '2018-01-01 00:00:00', 102) AND dbo.MemoMasters.MemoDate <= CONVERT(DATETIME, '2018-06-01 00:00:00', 102)) AND 
                                    (dbo.MemoMasters.ShowRoomId = 1)";
            }


            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                if (CustomerId > 0)
                {
                    command.Parameters.Add(new SqlParameter("@customerId", CustomerId));
                }
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        aObj = new MemoView();
                        aObj.CustomerName = (string)reader["CustomerName"];
                        aObj.MemoDate = (DateTime)reader["MemoDate"];
                        aObj.MemoDiscount = (double)reader["MemoDiscount"];
                        aObj.GatOther = (double)reader["GatOther"];
                        aObj.ActualMemoAmount = (double)reader["TotalCost"];
                        aObj.NetMemoAmount = (double)reader["TotalCost"]+ (double)reader["GatOther"]- (double)reader["MemoDiscount"];
                        aObj.MemoNo= (string)reader["MemoNo"];
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






        // GET: api/MemoMasters
        public IQueryable<MemoMaster> GetMemoMasters()
        {
            return db.MemoMasters;
        }

        // GET: api/MemoMasters/5
        [ResponseType(typeof(MemoMaster))]
        public async Task<IHttpActionResult> GetMemoMaster(int id)
        {
            MemoMaster memoMaster = await db.MemoMasters.FindAsync(id);
            if (memoMaster == null)
            {
                return NotFound();
            }

            return Ok(memoMaster);
        }



        // PUT: api/MemoMasters/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMemoMaster(int id, MemoMaster memoMaster)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            memoMaster.ShowRoomId = showRoomId;
            memoMaster.DateCreated = ceatedAt;
            memoMaster.DateUpdated = ceatedAt;
            memoMaster.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != memoMaster.MemoMasterId)
            {
                return BadRequest();
            }

            db.Entry(memoMaster).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemoMasterExists(id))
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

        // POST: api/MemoMasters
        [ResponseType(typeof(MemoMaster))]
        public async Task<IHttpActionResult> PostMemoMaster(MemoMaster memoMaster)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();

            string userName = User.Identity.GetUserName();
            DateTime ceatedAt = DateTime.Now;
            memoMaster.ShowRoomId = showRoomId;
            memoMaster.DateCreated = ceatedAt;
            memoMaster.DateUpdated = ceatedAt;
            memoMaster.CreatedBy = userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MemoMasters.Add(memoMaster);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = memoMaster.MemoMasterId }, memoMaster);
        }

        // DELETE: api/MemoMasters/5
        [ResponseType(typeof(MemoMaster))]
        public async Task<IHttpActionResult> DeleteMemoMaster(int id)
        {
            MemoMaster memoMaster = await db.MemoMasters.FindAsync(id);
            if (memoMaster == null)
            {
                return NotFound();
            }

            db.MemoMasters.Remove(memoMaster);
            await db.SaveChangesAsync();

            return Ok(memoMaster);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemoMasterExists(int id)
        {
            return db.MemoMasters.Count(e => e.MemoMasterId == id) > 0;
        }
    }
}