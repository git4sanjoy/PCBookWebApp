using Microsoft.AspNet.Identity;
using PCBookWebApp.DAL;
using PCBookWebApp.Models.ProcessModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers.ProcessModule
{
    public class ProcessReportsController : Controller
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();
        // GET: ProcessReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowPurchaseRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();

            int recordCount = 0;
            List<PurchaseRptView> inventoryList = new List<PurchaseRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    //var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    //sql = "SELECTdbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName FROM dbo.Purchases INNER JOIN  dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId WHERE(PurchaseDate >= CONVERT(DATETIME, @fromDate, 102) AND PurchaseDate <= CONVERT(DATETIME, @toDate, 102)) AND (SupplierId IN (" + inIds + ")) AND (ShowRoomId=@showRoomId)";

                    sql = @"SELECT        
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName, dbo.Purchases.DeliveryQuantity
                                FROM            
                                dbo.Purchases INNER JOIN
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                WHERE (dbo.Purchases.ShowRoomId = @showRoomId) 
                                AND dbo.Purchases.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' AND dbo.Purchases.SupplierId IN " + '(' + string.Join(",", LedgerIds) + ')';
                }
                else
                {
                    sql = @"SELECT        
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName, dbo.Purchases.DeliveryQuantity
                                FROM            
                                dbo.Purchases INNER JOIN
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                WHERE (dbo.Purchases.PurchaseDate >= @fromDate) AND (dbo.Purchases.PurchaseDate <= @toDate)
                                AND (dbo.Purchases.ShowRoomId = @showRoomId)";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PurchaseRptView aLedger = new PurchaseRptView();
                    aLedger.PChallanNo = (string)reader["PChallanNo"];
                    aLedger.PurchaseDate = (DateTime)reader["PurchaseDate"];
                    if (reader["Quantity"] != DBNull.Value) {
                        aLedger.Quantity = (double)reader["Quantity"];
                    }
                    if (reader["SE"] != DBNull.Value)
                    {
                        aLedger.SE = (double)reader["SE"];
                    }
                    if (reader["Amount"] != DBNull.Value)
                    {
                        aLedger.Amount = (double)reader["Amount"];
                    }
                    if (reader["Discount"] != DBNull.Value)
                    {
                        aLedger.Discount = (double)reader["Discount"];
                    }
                    if (reader["DeliveryQuantity"] != DBNull.Value)
                    {
                        aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];
                    }
                    
                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    aLedger.ShowRoomName = (string)reader["ShowRoomName"];
                    if (reader["SupplierName"] != DBNull.Value)
                    {
                        aLedger.SupplierName = (string)reader["SupplierName"];
                    }
                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "ProcessPurchase.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Purchase";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }

        public ActionResult ShowProductRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();

            int recordCount = 0;
            List<PurchaseRptView> inventoryList = new List<PurchaseRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    //var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    //sql = "SELECTdbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName FROM dbo.Purchases INNER JOIN  dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId WHERE(PurchaseDate >= CONVERT(DATETIME, @fromDate, 102) AND PurchaseDate <= CONVERT(DATETIME, @toDate, 102)) AND (SupplierId IN (" + inIds + ")) AND (ShowRoomId=@showRoomId)";



                    sql = @"SELECT        
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName, dbo.Purchases.DeliveryQuantity
                                FROM            
                                dbo.Purchases INNER JOIN
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                where dbo.Purchases.SupplierId is not null 
                                  and dbo.Purchases.ShowRoomId =  '" + showRoomId + "' and   dbo.Purchases.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' and dbo.PurchasedProducts.PurchasedProductId in " + '(' + string.Join(",", LedgerIds) + ')';
                }
                else
                {
                    sql = @"SELECT        
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName, dbo.Purchases.DeliveryQuantity
                                FROM            
                                dbo.Purchases INNER JOIN
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                where dbo.Purchases.SupplierId is not null
                                and   dbo.Purchases.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' and dbo.Purchases.ShowRoomId = " + showRoomId;
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    PurchaseRptView aLedger = new PurchaseRptView();
                    aLedger.PChallanNo = (string)reader["PChallanNo"];
                    aLedger.PurchaseDate = (DateTime)reader["PurchaseDate"];
                    if (reader["Quantity"] != DBNull.Value)
                    {
                        aLedger.Quantity = (double)reader["Quantity"];
                    }
                    if (reader["SE"] != DBNull.Value)
                    {
                        aLedger.SE = (double)reader["SE"];
                    }
                    if (reader["Amount"] != DBNull.Value)
                    {
                        aLedger.Amount = (double)reader["Amount"];
                    }
                    if (reader["Discount"] != DBNull.Value)
                    {
                        aLedger.Discount = (double)reader["Discount"];
                    }
                    if (reader["DeliveryQuantity"] != DBNull.Value)
                    {
                        aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];
                    }

                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    aLedger.ShowRoomName = (string)reader["ShowRoomName"];
                    if (reader["SupplierName"] != DBNull.Value)
                    {
                        aLedger.SupplierName = (string)reader["SupplierName"];
                    }
                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "ProcessProduct.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Product";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }

        public ActionResult ShowFactoryRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();

            int recordCount = 0;
            List<FactorywisePurchaseRptView> inventoryList = new List<FactorywisePurchaseRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    // var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    sql = @"SELECT    dbo.Processes.Rate,      
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName, dbo.Purchases.DeliveryQuantity, dbo.ProcesseLocations.ProcesseLocationName
                                FROM            
                                dbo.Purchases 
                                INNER JOIN dbo.Processes ON dbo.Processes.PurchasedProductId=Purchases.PurchasedProductId
                                INNER JOIN
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                JOIN dbo.ProcesseLocations ON dbo.ProcesseLocations.ProcesseLocationId = dbo.Purchases.ProcesseLocationId 
                                where dbo.Purchases.ProcesseLocationId is not null  
                                  and dbo.Purchases.ShowRoomId =  '" + showRoomId + "' and   dbo.Purchases.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'  and dbo.Purchases.ProcesseLocationId in " + '(' + string.Join(",", LedgerIds) + ')';
                }
                else
                {
                    sql = @"SELECT  dbo.Processes.Rate,      
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName, dbo.Purchases.DeliveryQuantity, dbo.ProcesseLocations.ProcesseLocationName
                                FROM            
                                dbo.Purchases 
                                INNER JOIN dbo.Processes ON dbo.Processes.PurchasedProductId=Purchases.PurchasedProductId
                                INNER JOIN 
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                JOIN dbo.ProcesseLocations ON dbo.ProcesseLocations.ProcesseLocationId = dbo.Purchases.ProcesseLocationId 
                                where dbo.Purchases.ProcesseLocationId is not null  
                                and   dbo.Purchases.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "' and dbo.Purchases.ShowRoomId = " + showRoomId;
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    FactorywisePurchaseRptView aLedger = new FactorywisePurchaseRptView();
                    aLedger.PChallanNo = (string)reader["PChallanNo"];
                    aLedger.PurchaseDate = (DateTime)reader["PurchaseDate"];
                    if (reader["Rate"] != DBNull.Value)
                    {
                        aLedger.Rate = (double)reader["Rate"];
                    }
                    if (reader["Quantity"] != DBNull.Value)
                    {
                        aLedger.Quantity = (double)reader["Quantity"];
                    }
                    if (reader["SE"] != DBNull.Value)
                    {
                        aLedger.SE = (double)reader["SE"];
                    }
                    if (reader["Amount"] != DBNull.Value)
                    {
                        aLedger.Amount = (double)reader["Amount"];
                    }
                    if (reader["Discount"] != DBNull.Value)
                    {
                        aLedger.Discount = (double)reader["Discount"];
                    }
                    if (reader["DeliveryQuantity"] != DBNull.Value)
                    {
                        aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];
                    }

                    aLedger.ProcesseLocationName = (string)reader["ProcesseLocationName"];
                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    aLedger.ShowRoomName = (string)reader["ShowRoomName"];
                    if (reader["SupplierName"] != DBNull.Value)
                    {
                        aLedger.SupplierName = (string)reader["SupplierName"];
                    }
                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "ProcessFactory.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Factory";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }

        public ActionResult FactoryWisePendingProcess(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();

            int recordCount = 0;
            List<FactoryWisePendingRptView> inventoryList = new List<FactoryWisePendingRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    sql = @"SELECT Del.ProcessDate  AS PurchaseDate, Del.LotNo,  Del.ReceiveQuantity,  ISNULL(Rec.DeliveryQuantity, 0) AS DeliveryQuantity,  Del.ProcesseLocationName 
                            FROM (SELECT pro.ProcessDate, pro.LotNo,  SUM(pro.ReceiveQuantity) AS ReceiveQuantity,  loc.ProcesseLocationName  FROM Processes pro 
                            JOIN ProcesseLocations loc ON loc.ProcesseLocationId=pro.ProcesseLocationId  WHERE pro.DeliveryQuantity = 0  and pro.ShowRoomId='" + showRoomId + "'  And loc.ProcesseLocationId in" + '(' + string.Join(",", LedgerIds) + ')' + " GROUP BY pro.LotNo, pro.ProcessDate, loc.ProcesseLocationName) Del LEFT JOIN  (SELECT Res.LotNo,  Res.DeliveryQuantity  FROM (SELECT pro.LotNo,  SUM(pro.DeliveryQuantity + SE) AS DeliveryQuantity  FROM Processes pro  WHERE DeliveryQuantity + SE != 0 and pro.ShowRoomId='" + showRoomId + "' GROUP BY LotNo) Res) Rec ON Del.LotNo = Rec.LotNo  WHERE Del.ReceiveQuantity > ISNULL(Rec.DeliveryQuantity, 0) and Del.ProcessDate between '" + FromDate + "' and '" + ToDate + "'";
                }
                else
                {
                    sql = @"SELECT Del.ProcessDate  AS PurchaseDate, Del.LotNo,  Del.ReceiveQuantity,  ISNULL(Rec.DeliveryQuantity, 0) AS DeliveryQuantity,  Del.ProcesseLocationName 
                            FROM (SELECT pro.ProcessDate, pro.LotNo,  SUM(pro.ReceiveQuantity) AS ReceiveQuantity,  loc.ProcesseLocationName  FROM Processes pro 
                            JOIN ProcesseLocations loc ON loc.ProcesseLocationId=pro.ProcesseLocationId  WHERE pro.DeliveryQuantity = 0 and pro.ShowRoomId='" + showRoomId + "' GROUP BY pro.LotNo, pro.ProcessDate, loc.ProcesseLocationName) Del LEFT JOIN  (SELECT Res.LotNo,  Res.DeliveryQuantity  FROM  (SELECT pro.LotNo,  SUM(pro.DeliveryQuantity + SE) AS DeliveryQuantity  FROM Processes pro  WHERE DeliveryQuantity + SE != 0 and pro.ShowRoomId='" + showRoomId + "' GROUP BY LotNo) Res) Rec ON Del.LotNo = Rec.LotNo  WHERE Del.ReceiveQuantity > ISNULL(Rec.DeliveryQuantity, 0) and Del.ProcessDate between '" + FromDate + "' and '" + ToDate + "'";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    FactoryWisePendingRptView aLedger = new FactoryWisePendingRptView();
                    aLedger.LotNo = (string)reader["LotNo"];
                    aLedger.ReceiveQuantity = (double)reader["ReceiveQuantity"];
                    aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];
                    aLedger.ProcesseLocationName = (string)reader["ProcesseLocationName"];
                    aLedger.PurchaseDate = (DateTime)reader["PurchaseDate"];

                    // aLedger.PurchaseDate = (DateTime)reader["PurchaseDate"];



                    //if (reader["Quantity"] != DBNull.Value)
                    //{
                    //    aLedger.Quantity = (double)reader["Quantity"];
                    //}
                    //if (reader["SE"] != DBNull.Value)
                    //{
                    //    aLedger.SE = (double)reader["SE"];
                    //}
                    //if (reader["Amount"] != DBNull.Value)
                    //{
                    //    aLedger.Amount = (double)reader["Amount"];
                    //}
                    //if (reader["Discount"] != DBNull.Value)
                    //{
                    //    aLedger.Discount = (double)reader["Discount"];
                    //}
                    //if (reader["DeliveryQuantity"] != DBNull.Value)
                    //{
                    //    aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];
                    //}

                    //aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    //aLedger.ShowRoomName = (string)reader["ShowRoomName"];
                    //if (reader["SupplierName"] != DBNull.Value)
                    //{
                    //    aLedger.SupplierName = (string)reader["SupplierName"];
                    //}
                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "ProcessFactoryWisePendingProcess.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Factory";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }

        public ActionResult FactoryWiseProcessHistory(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();

            int recordCount = 0;
            List<FactoryWiseProcessHistoryRptView> inventoryList = new List<FactoryWiseProcessHistoryRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    sql = @"SELECT pro.ProcessId ,pro.ProcessDate ,pro.LotNo  ,pro.ReceiveQuantity ,pro.DeliveryQuantity ,pro.SE 
	                      ,pro.Rate  ,pro.Amount  ,pro.Discount  ,pp.PurchasedProductName  ,pl.ProcessListName ,loc.ProcesseLocationName 
                            ,sm.ShowRoomName  FROM Processes pro  INNER JOIN PurchasedProducts pp ON pp.PurchasedProductId = pro.PurchasedProductId 
                        INNER JOIN ProcessLists pl ON pl.ProcessListId = pro.ProcessListId  INNER JOIN ProcesseLocations loc ON loc.ProcesseLocationId = pro.ProcesseLocationId 
                        INNER JOIN ShowRooms sm ON sm.ShowRoomId = pro.ShowRoomId  where pro.ProcessDate between '" + FromDate + "' and '" + ToDate + "' and  pro.ShowRoomId =  '" + showRoomId + "' and loc.ProcesseLocationId in " + '(' + string.Join(",", LedgerIds) + ')' + " ORDER BY LotNo ";
                }
                else
                {
                    sql = @"SELECT pro.ProcessId ,pro.ProcessDate ,pro.LotNo  ,pro.ReceiveQuantity ,pro.DeliveryQuantity ,pro.SE 
	                      ,pro.Rate  ,pro.Amount  ,pro.Discount  ,pp.PurchasedProductName  ,pl.ProcessListName ,loc.ProcesseLocationName 
                            ,sm.ShowRoomName  FROM Processes pro  INNER JOIN PurchasedProducts pp ON pp.PurchasedProductId = pro.PurchasedProductId 
                        INNER JOIN ProcessLists pl ON pl.ProcessListId = pro.ProcessListId  INNER JOIN ProcesseLocations loc ON loc.ProcesseLocationId = pro.ProcesseLocationId 
                        INNER JOIN ShowRooms sm ON sm.ShowRoomId = pro.ShowRoomId  where pro.ProcessDate between '" + FromDate + "' and '" + ToDate + "' and pro.ShowRoomId = " + showRoomId + " ORDER BY LotNo ";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    FactoryWiseProcessHistoryRptView aLedger = new FactoryWiseProcessHistoryRptView();
                    aLedger.ProcessDate = (DateTime)reader["ProcessDate"];
                    aLedger.LotNo = (string)reader["LotNo"];
                    aLedger.ReceiveQuantity = (double)reader["ReceiveQuantity"];
                    aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];

                    if (reader["SE"] != DBNull.Value)
                    {
                        aLedger.SE = (double)reader["SE"];
                    }
                    if (reader["Amount"] != DBNull.Value)
                    {
                        aLedger.Amount = (double)reader["Amount"];
                    }
                    if (reader["Discount"] != DBNull.Value)
                    {
                        aLedger.Discount = (double)reader["Discount"];
                    }
                    if (reader["Rate"] != DBNull.Value)
                    {
                        aLedger.Rate = (double)reader["Rate"];
                    }

                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    aLedger.ProcessListName = (string)reader["ProcessListName"];
                    aLedger.ProcesseLocationName = (string)reader["ProcesseLocationName"];
                    aLedger.ShowRoomName = (string)reader["ShowRoomName"];


                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "ProcessFactoryWiseProcessHistory.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Factory";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }

        public ActionResult ProductWiseStoreBalance(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();

            int recordCount = 0;
            List<ProductWiseStoreBalanceRptView> inventoryList = new List<ProductWiseStoreBalanceRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    sql = @"SELECT  Rec.PurchasedProductName,  ISNULL(Rec.PurchaseQuantity, 0) AS PurchaseQuantity, ISNULL(Del.DeliveryQuantity, 0) AS DeliveryQuantity, Rec.ShowRoomName FROM (SELECT  prod.PurchasedProductName,  SUM(pur.Quantity) AS PurchaseQuantity,  sm.ShowRoomName FROM Purchases pur LEFT JOIN PurchasedProducts prod  ON prod.PurchasedProductId = pur.PurchasedProductId  JOIN ShowRooms sm ON sm.ShowRoomId = pur.ShowRoomId WHERE pur.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'   and pur.ShowRoomId='" + showRoomId + "'  and  prod.PurchasedProductId in " + '(' + string.Join(",", LedgerIds) + ')' + " GROUP BY prod.PurchasedProductName, sm.ShowRoomName) Rec LEFT JOIN (SELECT prod.PurchasedProductName, SUM(pur.DeliveryQuantity) AS DeliveryQuantity FROM Purchases pur JOIN PurchasedProducts prod   ON prod.PurchasedProductId = pur.PurchasedProductId WHERE ProcesseLocationId IS NOT NULL and  pur.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'  and prod.ShowRoomId='" + showRoomId + "'  GROUP BY prod.PurchasedProductName) Del  ON Del.PurchasedProductName = Rec.PurchasedProductName ";
                }
                else
                {
                    sql = @"SELECT  Rec.PurchasedProductName,  ISNULL(Rec.PurchaseQuantity, 0) AS PurchaseQuantity, ISNULL(Del.DeliveryQuantity, 0) AS DeliveryQuantity, Rec.ShowRoomName FROM (SELECT  prod.PurchasedProductName,  SUM(pur.Quantity) AS PurchaseQuantity,  sm.ShowRoomName FROM Purchases pur LEFT JOIN PurchasedProducts prod  ON prod.PurchasedProductId = pur.PurchasedProductId  JOIN ShowRooms sm ON sm.ShowRoomId = pur.ShowRoomId WHERE pur.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'  and pur.ShowRoomId='" + showRoomId + "'  GROUP BY prod.PurchasedProductName, sm.ShowRoomName) Rec LEFT JOIN (SELECT prod.PurchasedProductName, SUM(pur.DeliveryQuantity) AS DeliveryQuantity FROM Purchases pur JOIN PurchasedProducts prod   ON prod.PurchasedProductId = pur.PurchasedProductId WHERE ProcesseLocationId IS NOT NULL and  pur.PurchaseDate BETWEEN '" + FromDate + "' AND '" + ToDate + "'  and prod.ShowRoomId='" + showRoomId + "'  GROUP BY prod.PurchasedProductName) Del  ON Del.PurchasedProductName = Rec.PurchasedProductName";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ProductWiseStoreBalanceRptView aLedger = new ProductWiseStoreBalanceRptView();

                    //aLedger.LotNo = (string)reader["LotNo"];
                    aLedger.PurchaseQuantity = (double)reader["PurchaseQuantity"];
                    aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];

                    //if (reader["SE"] != DBNull.Value)
                    //{
                    //    aLedger.SE = (double)reader["SE"];
                    //}
                    //if (reader["Amount"] != DBNull.Value)
                    //{
                    //    aLedger.Amount = (double)reader["Amount"];
                    //}
                    //if (reader["Discount"] != DBNull.Value)
                    //{
                    //    aLedger.Discount = (double)reader["Discount"];
                    //}
                    //if (reader["Rate"] != DBNull.Value)
                    //{
                    //    aLedger.Rate = (double)reader["Rate"];
                    //}

                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    //aLedger.ProcessListName = (string)reader["ProcessListName"];
                    //aLedger.ProcesseLocationName = (string)reader["ProcesseLocationName"];
                    aLedger.ShowRoomName = (string)reader["ShowRoomName"];


                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "ProductWiseStoreBalance.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Factory";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }

        public ActionResult FinishedGoodsStock(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers
                .Where(a => a.Id == userId)
                .Select(a => a.ShowRoomId)
                .FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();

            int recordCount = 0;
            List<FinishedGoodsRptView> inventoryList = new List<FinishedGoodsRptView>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
            connection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader reader = null;
                string sql = "";
                if (LedgerIds != null)
                {
                    var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    // sql = "SELECTdbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName FROM dbo.Purchases INNER JOIN  dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId WHERE(PurchaseDate >= CONVERT(DATETIME, @fromDate, 102) AND PurchaseDate <= CONVERT(DATETIME, @toDate, 102)) AND (SupplierId IN (" + inIds + ")) AND (ShowRoomId=@showRoomId)";
                }
                else
                {
                    sql = @"SELECT Rec. ProcessDate, Rec.LotNo, Rec.ReceiveQuantity, Del.DeliveryQuantity,  Rec.PurchasedProductName FROM (SELECT   pro.ProcessDate,  pro.LotNo,  pro.ReceiveQuantity,  pp.PurchasedProductName  FROM Processes pro  JOIN PurchasedProducts pp ON pp.PurchasedProductId = pro.PurchasedProductId  WHERE pro.ReceiveQuantity != 0 and pro.ProcessDate between '" + FromDate + "' and '" + ToDate + "'  ) Rec  JOIN (SELECT LotNo,  SUM(DeliveryQuantity + SE) AS DeliveryQuantity  FROM Processes  WHERE ReceiveQuantity = 0 AND ProcessDate between '" + FromDate + "' and '" + ToDate + "'  GROUP BY LotNo) Del   ON Rec.LotNo = Del.LotNo  WHERE Rec.ReceiveQuantity = Del.DeliveryQuantity  ";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    FinishedGoodsRptView aLedger = new FinishedGoodsRptView();

                    //aLedger.LotNo = (string)reader["LotNo"];
                    aLedger.ReceiveQuantity = (double)reader["ReceiveQuantity"];
                    aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];
                    aLedger.ProcessDate = (DateTime)reader["ProcessDate"];
                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    //aLedger.ProcessListName = (string)reader["ProcessListName"];
                    //aLedger.ProcesseLocationName = (string)reader["ProcesseLocationName"];
                    aLedger.LotNo = (string)reader["LotNo"];


                    inventoryList.Add(aLedger);
                    recordCount++;
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "FinishedGoodsStock.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Factory";
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
            //var c = (from b in db.Inventories select b).ToList();
            //rpt.SetDataSource(c);
        }
    }
}