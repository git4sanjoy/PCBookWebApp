using PCBookWebApp.DAL;
using PCBookWebApp.Models.ViewModels;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.BookViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Reports
        private PCBookWebAppContext db = new PCBookWebAppContext();

        public ActionResult Index() {
            return View();
        }
        public ActionResult ShowBookRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
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
            List<LedgerView> inventoryList = new List<LedgerView>();
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
                    sql = "SELECT dbo.Vouchers.VoucherId, dbo.Vouchers.VoucherTypeId, dbo.Vouchers.ShowRoomId, dbo.Vouchers.IsBank, dbo.Vouchers.IsHonored, dbo.Vouchers.HonoredDate, dbo.Vouchers.Authorized, dbo.Vouchers.AuthorizedBy, dbo.VoucherDetails.TransctionTypeId, dbo.VoucherDetails.LedgerId, dbo.VoucherTypes.VoucherTypeName, dbo.Vouchers.VoucherDate, dbo.Vouchers.VoucherNo, dbo.TransctionTypes.TransctionTypeName, dbo.Ledgers.LedgerName, dbo.VoucherDetails.DrAmount, dbo.VoucherDetails.CrAmount, dbo.Vouchers.Naration FROM  dbo.Vouchers INNER JOIN dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId INNER JOIN dbo.Ledgers ON dbo.VoucherDetails.LedgerId = dbo.Ledgers.LedgerId INNER JOIN dbo.TransctionTypes ON dbo.VoucherDetails.TransctionTypeId = dbo.TransctionTypes.TransctionTypeId INNER JOIN dbo.VoucherTypes ON dbo.Vouchers.VoucherTypeId = dbo.VoucherTypes.VoucherTypeId WHERE (VoucherDate >= CONVERT(DATETIME, @fromDate, 102) AND VoucherDate <= CONVERT(DATETIME, @toDate, 102)) AND (dbo.VoucherDetails.LedgerId IN (" + inIds + ")) AND (dbo.Vouchers.ShowRoomId=@showRoomId)";
                }
                else
                {
                    sql = @"SELECT        
                                dbo.Vouchers.VoucherId, dbo.Vouchers.VoucherTypeId, dbo.Vouchers.ShowRoomId, dbo.Vouchers.IsBank, dbo.Vouchers.IsHonored, dbo.Vouchers.HonoredDate, dbo.Vouchers.Authorized, dbo.Vouchers.AuthorizedBy, 
                                dbo.VoucherDetails.TransctionTypeId, dbo.VoucherDetails.LedgerId, dbo.VoucherTypes.VoucherTypeName, dbo.Vouchers.VoucherDate, dbo.Vouchers.VoucherNo, dbo.TransctionTypes.TransctionTypeName, 
                                dbo.Ledgers.LedgerName, dbo.VoucherDetails.DrAmount, dbo.VoucherDetails.CrAmount, dbo.Vouchers.Naration, dbo.Ledgers.TrialBalanceId, dbo.Ledgers.GroupId, dbo.Groups.GroupName
                                FROM            
                                dbo.Vouchers INNER JOIN
                                dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId INNER JOIN
                                dbo.Ledgers ON dbo.VoucherDetails.LedgerId = dbo.Ledgers.LedgerId INNER JOIN
                                dbo.TransctionTypes ON dbo.VoucherDetails.TransctionTypeId = dbo.TransctionTypes.TransctionTypeId INNER JOIN
                                dbo.VoucherTypes ON dbo.Vouchers.VoucherTypeId = dbo.VoucherTypes.VoucherTypeId INNER JOIN
                                dbo.Groups ON dbo.Ledgers.GroupId = dbo.Groups.GroupId
                                WHERE (dbo.Vouchers.VoucherDate >= CONVERT(DATETIME, @fromDate, 102) AND dbo.Vouchers.VoucherDate <= CONVERT(DATETIME, @toDate, 102)) AND (dbo.Vouchers.ShowRoomId=@showRoomId)";
                }

                SqlCommand command = new SqlCommand(sql, connection);
                //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    LedgerView aLedger = new LedgerView();
                    aLedger.VoucherNo = (string) reader["VoucherNo"];
                    aLedger.Date = (DateTime) reader["VoucherDate"];
                    aLedger.DrAmount = (double) reader["DrAmount"];
                    aLedger.CrAmount = (double) reader["CrAmount"];
                    aLedger.LedgerName = (string)reader["LedgerName"];
                    if (reader["Naration"] != DBNull.Value)
                    {
                        aLedger.Narration = (string)reader["Naration"];
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

                this.HttpContext.Session["rptName"] = "Book.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Book";
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

        public ActionResult ShowLedgerRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
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
            List<LedgerView> inventoryList = new List<LedgerView>();
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
                    sql = "SELECT dbo.Vouchers.VoucherId, dbo.Vouchers.VoucherTypeId, dbo.Vouchers.ShowRoomId, dbo.Vouchers.IsBank, dbo.Vouchers.IsHonored, dbo.Vouchers.HonoredDate, dbo.Vouchers.Authorized, dbo.Vouchers.AuthorizedBy, dbo.VoucherDetails.TransctionTypeId, dbo.VoucherDetails.LedgerId, dbo.VoucherTypes.VoucherTypeName, dbo.Vouchers.VoucherDate, dbo.Vouchers.VoucherNo, dbo.TransctionTypes.TransctionTypeName, dbo.Ledgers.LedgerName, dbo.VoucherDetails.DrAmount, dbo.VoucherDetails.CrAmount, dbo.Vouchers.Naration FROM  dbo.Vouchers INNER JOIN dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId INNER JOIN dbo.Ledgers ON dbo.VoucherDetails.LedgerId = dbo.Ledgers.LedgerId INNER JOIN dbo.TransctionTypes ON dbo.VoucherDetails.TransctionTypeId = dbo.TransctionTypes.TransctionTypeId INNER JOIN dbo.VoucherTypes ON dbo.Vouchers.VoucherTypeId = dbo.VoucherTypes.VoucherTypeId WHERE (VoucherDate >= CONVERT(DATETIME, @fromDate, 102) AND VoucherDate <= CONVERT(DATETIME, @toDate, 102)) AND (dbo.VoucherDetails.LedgerId IN (" + inIds + ")) AND (dbo.Vouchers.ShowRoomId=@showRoomId)";
                }
                else
                {
                    sql = @"SELECT        
                            dbo.Vouchers.VoucherId, dbo.Vouchers.VoucherTypeId, dbo.Vouchers.ShowRoomId, dbo.Vouchers.IsBank, dbo.Vouchers.IsHonored, dbo.Vouchers.HonoredDate, dbo.Vouchers.Authorized, dbo.Vouchers.AuthorizedBy, 
                            dbo.VoucherDetails.TransctionTypeId, dbo.VoucherDetails.LedgerId, dbo.VoucherTypes.VoucherTypeName, dbo.Vouchers.VoucherDate, dbo.Vouchers.VoucherNo, dbo.TransctionTypes.TransctionTypeName, 
                            dbo.Ledgers.LedgerName, dbo.VoucherDetails.DrAmount, dbo.VoucherDetails.CrAmount, dbo.Vouchers.Naration, dbo.Ledgers.TrialBalanceId, dbo.Ledgers.GroupId, dbo.Groups.GroupName
                            FROM            
                            dbo.Vouchers INNER JOIN
                            dbo.VoucherDetails ON dbo.Vouchers.VoucherId = dbo.VoucherDetails.VoucherId INNER JOIN
                            dbo.Ledgers ON dbo.VoucherDetails.LedgerId = dbo.Ledgers.LedgerId INNER JOIN
                            dbo.TransctionTypes ON dbo.VoucherDetails.TransctionTypeId = dbo.TransctionTypes.TransctionTypeId INNER JOIN
                            dbo.VoucherTypes ON dbo.Vouchers.VoucherTypeId = dbo.VoucherTypes.VoucherTypeId INNER JOIN
                            dbo.Groups ON dbo.Ledgers.GroupId = dbo.Groups.GroupId
                            WHERE (dbo.Vouchers.VoucherDate >= CONVERT(DATETIME, @fromDate, 102) AND dbo.Vouchers.VoucherDate <= CONVERT(DATETIME, @toDate, 102)) AND (dbo.Vouchers.ShowRoomId=@showRoomId)";
                }
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                command.Parameters.Add(new SqlParameter("@toDate", tdate));
                command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    LedgerView aLedger = new LedgerView();
                    aLedger.VoucherNo = (string)reader["VoucherNo"];
                    aLedger.Date = (DateTime)reader["VoucherDate"];
                    aLedger.DrAmount = (double)reader["DrAmount"];
                    aLedger.CrAmount = (double)reader["CrAmount"];
                    aLedger.LedgerName = (string)reader["LedgerName"];
                    if (reader["Naration"] != DBNull.Value)
                    {
                        aLedger.Narration = (string)reader["Naration"];
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

                this.HttpContext.Session["rptName"] = "Ledger.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Ledger";
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
        public ActionResult ShowMaterialLedgerOnlyTransGenericRptInNewWin(string FromDate, string ToDate, string SelectedReportOption)
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
            List<LedgerView> inventoryList = new List<LedgerView>();
            SqlConnection spConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DCStockWebAppContext"].ConnectionString);
            spConnection.Open();
            try
            {
                DateTime fdate = DateTime.Parse(FromDate);
                DateTime tdate = DateTime.Parse(ToDate);
                SqlDataReader inventoryReader = null;
                string sql = "";

                sql = @"SELECT        
                            InventoryId, MaterialId, TransctionTypeId, MachineId, SupplierId, ProcessId, TDate, BillNo, Quantity, Rate, TotalPrice, LotNo, Remarks, CreatedBy, TransctionTypeName, MaterialName, SubMaterialId, 
                            SubMaterialName, MainMaterialId, MainMateriaName, MatricName, MachineName, ProcessName, YearName, MonthInt, MonthString, PQu, SRQu, IQu, DRQu, CBQu, SupplierName
                            FROM            
                            dbo.InventoriesView
                            WHERE        
                            (TDate <= CONVERT(DATETIME, @toDate, 102)) AND (CreatedBy = @createdBy) AND (MaterialId IN (SELECT DISTINCT MaterialId
                            FROM dbo.InventoriesView AS InventoriesView1 WHERE (TDate >= CONVERT(DATETIME, @fromDate, 102)) AND (TDate <= CONVERT(DATETIME, @toDate, 102))))";

                SqlCommand spCommand = new SqlCommand(sql, spConnection);
                spCommand.Parameters.Add(new SqlParameter("@fromDate", fdate));
                spCommand.Parameters.Add(new SqlParameter("@toDate", tdate));
                spCommand.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                inventoryReader = spCommand.ExecuteReader();

                while (inventoryReader.Read())
                {
                    LedgerView invObj = new LedgerView();
                    //invObj.TransctionTypeId = (int)inventoryReader["TransctionTypeId"];
                    //invObj.TDate = (DateTime)inventoryReader["TDate"];
                    //invObj.Quantity = (double)inventoryReader["Quantity"];
                    //invObj.Matric = (string)inventoryReader["MatricName"];
                    //if (inventoryReader["SupplierName"] != DBNull.Value)
                    //{
                    //    invObj.SupplierName = (string)inventoryReader["SupplierName"];
                    //}
                    inventoryList.Add(invObj);
                    recordCount++;
                }
                inventoryReader.Close();
                spConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (recordCount > 0)
            {

                this.HttpContext.Session["rptName"] = "MaterialLedger.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptProductName"] = SelectedReportOption;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptSource"] = inventoryList;
                return Json("ok", JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
        }

    }
}