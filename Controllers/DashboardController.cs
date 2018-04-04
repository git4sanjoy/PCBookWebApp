using Microsoft.AspNet.Identity;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.SalesModule;
using PCBookWebApp.Models.SalesModule.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDistrictReportList(int[] ZoneIds)
        {
            var districts = (from p in db.Districts
                            where ZoneIds.Contains((int) p.SaleZoneId)
                            select p)
                            .Select(d=>new {
                                id = d.DistrictId,
                                label = d.DistrictName
                            })
                            .ToList();

            return this.Json(new { listDistricts = districts }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDateSalesInMapJQueryCall(string Date, int? SalesManId = 0)
        {

            //int SalesManId = 0;
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            
            DateTime fdate = DateTime.Parse(Date);
            List<SalesView> list = new List<SalesView>();
            SalesView memoObj = new SalesView();
            string connectionString = ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString;
            string queryString = "";

            if (User.IsInRole("Show Room Sales"))
            {
                if (SalesManId > 0)
                {
                    queryString = @"SELECT        
                                dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) 
                                AS TotalMemoCost, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, dbo.MemoMasters.MemoDate
                                FROM            
                                dbo.MemoMasters INNER JOIN
                                dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                dbo.ShowRooms ON dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                                GROUP BY dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.MemoMasters.MemoDate, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName
                                HAVING        
                                (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate = CONVERT(DATETIME, @date, 102)) AND (dbo.Customers.SalesManId = @salesManId)";
                }
                else
                {
                    queryString = @"SELECT        
                                dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) 
                                AS TotalMemoCost, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, dbo.MemoMasters.MemoDate
                                FROM            
                                dbo.MemoMasters INNER JOIN
                                dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                dbo.ShowRooms ON dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                                GROUP BY dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.MemoMasters.MemoDate, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName
                                HAVING        
                                (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate = CONVERT(DATETIME, @date, 102))";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    //command.Parameters.Add(new SqlParameter("@month", Month));
                    command.Parameters.Add(new SqlParameter("@date", fdate));
                    command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                    if (SalesManId > 0)
                    {
                        command.Parameters.Add(new SqlParameter("@salesManId", SalesManId));
                    }
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            memoObj = new SalesView();
                            int distictId = (int)reader["DistrictId"];
                            int salesManId = 0;

                            if (SalesManId > 0)
                            {
                                salesManId = (int)reader["SalesManId"];
                                memoObj.SalesManName = (string)reader["SalesManName"];
                            }
                            memoObj.ShowRoomName = (string)reader["ShowRoomName"];

                            memoObj.DistrictName = (string)reader["DistrictName"];
                            double totalSaleTaka = (double)reader["TotalMemoCost"];

                            double memoDiscount = 0;
                            double gatOther = 0;

                            if (reader["MemoDiscount"] != DBNull.Value)
                            {
                                memoDiscount = (double)reader["MemoDiscount"];
                            }
                            if (reader["GatOther"] != DBNull.Value)
                            {
                                gatOther = (double)reader["GatOther"];
                            }
                            memoObj.TotalSaleTaka = totalSaleTaka + gatOther - memoDiscount;
                            list.Add(memoObj);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            else if (User.IsInRole("Zone Manager"))
            {
                if (SalesManId > 0)
                {
                    queryString = @"SELECT        
                                dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) 
                                AS TotalMemoCost, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, dbo.MemoMasters.MemoDate
                                FROM            
                                dbo.MemoMasters INNER JOIN
                                dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                dbo.ShowRooms ON dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                                GROUP BY dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.MemoMasters.MemoDate, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName
                                HAVING        
                                (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate = CONVERT(DATETIME, @date, 102)) AND (dbo.Customers.SalesManId = @salesManId)";
                }
                else
                {
                    queryString = @"SELECT        
                                    dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalMemoCost, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) 
                                    AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, dbo.MemoMasters.MemoDate, dbo.MemoMasters.CreatedBy
                                    FROM            
                                    dbo.MemoMasters INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.ShowRooms ON dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                    dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                    dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                    dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                    dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                                    GROUP BY dbo.MemoMasters.MemoDate, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.MemoMasters.CreatedBy
                                    HAVING        
                                    (dbo.MemoMasters.MemoDate = CONVERT(DATETIME, @date, 102)) AND (dbo.MemoMasters.CreatedBy = @CreatedBy)";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    //command.Parameters.Add(new SqlParameter("@month", Month));
                    command.Parameters.Add(new SqlParameter("@date", fdate));
                    command.Parameters.Add(new SqlParameter("@CreatedBy", userName));
                    if (SalesManId > 0)
                    {
                        command.Parameters.Add(new SqlParameter("@salesManId", SalesManId));
                    }
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            memoObj = new SalesView();
                            int distictId = (int)reader["DistrictId"];
                            int salesManId = 0;

                            if (SalesManId > 0)
                            {
                                salesManId = (int)reader["SalesManId"];
                                memoObj.SalesManName = (string)reader["SalesManName"];
                            }
                            //memoObj.ShowRoomName = (string)reader["ShowRoomName"];

                            memoObj.DistrictName = (string)reader["DistrictName"];
                            double totalSaleTaka = (double)reader["TotalMemoCost"];

                            double memoDiscount = 0;
                            double gatOther = 0;

                            if (reader["MemoDiscount"] != DBNull.Value)
                            {
                                memoDiscount = (double)reader["MemoDiscount"];
                            }
                            if (reader["GatOther"] != DBNull.Value)
                            {
                                gatOther = (double)reader["GatOther"];
                            }
                            memoObj.TotalSaleTaka = totalSaleTaka + gatOther - memoDiscount;
                            list.Add(memoObj);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            else if (User.IsInRole("Cash Sale"))
            {

            }
            else if (User.IsInRole("Unit Manager"))
            {
                var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
                if (SalesManId > 0)
                {
                    queryString = @"SELECT        
                                dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) 
                                AS TotalMemoCost, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, dbo.MemoMasters.MemoDate
                                FROM            
                                dbo.MemoMasters INNER JOIN
                                dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                dbo.ShowRooms ON dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                                GROUP BY dbo.MemoMasters.ShowRoomId, dbo.ShowRooms.ShowRoomName, dbo.MemoMasters.MemoDate, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName
                                HAVING        
                                (dbo.MemoMasters.ShowRoomId = @showRoomId) AND (dbo.MemoMasters.MemoDate = CONVERT(DATETIME, @date, 102)) AND (dbo.Customers.SalesManId = @salesManId)";
                }
                else
                {
                    queryString = @"SELECT        
                                    dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, SUM(dbo.MemoDetails.Quantity * (dbo.MemoDetails.Rate - dbo.MemoDetails.Discount)) AS TotalMemoCost, SUM(DISTINCT dbo.MemoMasters.MemoDiscount) 
                                    AS MemoDiscount, SUM(DISTINCT dbo.MemoMasters.GatOther) AS GatOther, dbo.MemoMasters.MemoDate, dbo.ShowRooms.UnitId
                                    FROM            
                                    dbo.MemoMasters 
                                    INNER JOIN
                                    dbo.Customers ON dbo.MemoMasters.CustomerId = dbo.Customers.CustomerId INNER JOIN
                                    dbo.ShowRooms ON dbo.Customers.ShowRoomId = dbo.ShowRooms.ShowRoomId INNER JOIN
                                    dbo.SalesMen ON dbo.Customers.SalesManId = dbo.SalesMen.SalesManId INNER JOIN
                                    dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN
                                    dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN
                                    dbo.MemoDetails ON dbo.MemoMasters.MemoMasterId = dbo.MemoDetails.MemoMasterId
                                    GROUP BY dbo.MemoMasters.MemoDate, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.ShowRooms.UnitId
                                    HAVING        
                                    (dbo.MemoMasters.MemoDate = CONVERT(DATETIME, @date, 102)) AND (dbo.ShowRooms.UnitId = @unitId)";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    //command.Parameters.Add(new SqlParameter("@month", Month));
                    command.Parameters.Add(new SqlParameter("@date", fdate));
                    command.Parameters.Add(new SqlParameter("@unitId", unitId));
                    if (SalesManId > 0)
                    {
                        command.Parameters.Add(new SqlParameter("@salesManId", SalesManId));
                    }
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            memoObj = new SalesView();
                            int distictId = (int)reader["DistrictId"];
                            int salesManId = 0;                          
                            memoObj.DistrictName = (string)reader["DistrictName"];
                            double totalSaleTaka = (double)reader["TotalMemoCost"];
                            double memoDiscount = 0;
                            double gatOther = 0;
                            if (reader["MemoDiscount"] != DBNull.Value)
                            {
                                memoDiscount = (double)reader["MemoDiscount"];
                            }
                            if (reader["GatOther"] != DBNull.Value)
                            {
                                gatOther = (double)reader["GatOther"];
                            }
                            memoObj.TotalSaleTaka = totalSaleTaka + gatOther - memoDiscount;
                            if (SalesManId > 0)
                            {
                                salesManId = (int)reader["SalesManId"];
                                memoObj.SalesManName = (string)reader["SalesManName"];
                            }
                            list.Add(memoObj);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }




            return this.Json(list, JsonRequestBehavior.AllowGet);

        }



    }
}