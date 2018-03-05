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

namespace PCBookWebApp.Controllers
{
    public class ProcessReportController : Controller
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
                    var inIds = String.Join(",", LedgerIds.Select(x => x.ToString()).ToArray());
                    sql = "SELECTdbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName FROM dbo.Purchases INNER JOIN  dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId WHERE(PurchaseDate >= CONVERT(DATETIME, @fromDate, 102) AND PurchaseDate <= CONVERT(DATETIME, @toDate, 102)) AND (SupplierId IN (" + inIds + ")) AND (ShowRoomId=@showRoomId)";
                }
                else
                {
                    sql = @"SELECT        
                                dbo.Purchases.PurchaseId, dbo.Purchases.PurchaseDate, dbo.Purchases.PChallanNo, dbo.Purchases.Quantity, dbo.Purchases.SE, dbo.Purchases.Amount, dbo.Purchases.Discount, dbo.Purchases.ShowRoomId, 
                                dbo.Purchases.SupplierId, dbo.Suppliers.SupplierName, dbo.Purchases.PurchasedProductId, dbo.PurchasedProducts.PurchasedProductName, dbo.ShowRooms.ShowRoomName
                                FROM            
                                dbo.Purchases INNER JOIN
                                dbo.ShowRooms ON dbo.Purchases.ShowRoomId = dbo.ShowRooms.ShowRoomId LEFT OUTER JOIN
                                dbo.Suppliers ON dbo.Purchases.SupplierId = dbo.Suppliers.SupplierId LEFT OUTER JOIN
                                dbo.PurchasedProducts ON dbo.Purchases.PurchasedProductId = dbo.PurchasedProducts.PurchasedProductId 
                                WHERE (PurchaseDate >= CONVERT(DATETIME, @fromDate, 102) AND PurchaseDate <= CONVERT(DATETIME, @toDate, 102)) AND (ShowRoomId=@showRoomId)";
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
                    aLedger.Quantity = (double)reader["Quantity"];
                    aLedger.SE = (double)reader["SE"];
                    aLedger.Amount = (double)reader["Amount"];
                    aLedger.Discount = (double)reader["Discount"];
                    aLedger.DeliveryQuantity = (double)reader["DeliveryQuantity"];                   
                    aLedger.PurchasedProductName = (string)reader["PurchasedProductName"];
                    aLedger.ShowRoomName = (string)reader["ShowRoomName"];
                    //aLedger.SupplierName = (string)reader["SupplierName"];
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


    }
}