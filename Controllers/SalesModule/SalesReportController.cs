using PCBookWebApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using PCBookWebApp.Models.SalesModule.ViewModel;
using System.Data.SqlClient;
using System.Configuration;

namespace PCBookWebApp.Controllers.SalesModule
{
    public class SalesReportController : Controller
    {
        private PCBookWebAppContext db = new PCBookWebAppContext();

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public ActionResult ShowClosingBalanceRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, int[] GroupIds, string ShowType, string UnitManagerReportGroup="None" )
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();

            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            List<ClosingBalanceReportView> reportList = new List<ClosingBalanceReportView>();
            if (User.IsInRole("Show Room Sales"))
            {
                var list = db.Customers
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
                    .Select(c => new {
                        ShowRoomId = c.ShowRoomId,
                        CustomerId = c.CustomerId,
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
                        MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),
                        OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                        OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                        OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                        MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                        GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                        GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                        OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                        OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                        OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                        TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                    })
                    .Where(c => c.ShowRoomId == showRoomId && c.CustomerName != "Cash Party")
                    .ToList()
                    .GroupBy(x => new { x.ShowRoomId, x.CustomerId })
                    .Select(
                            g => new
                            {
                                Key = g.Key,
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
                                Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                MemoDiscount = g.First().MemoDiscount,
                                GatOther = g.First().GatOther,
                                TotalPayments = g.First().TotalPayments,
                                ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                TotalBf = g.First().TotalBf,
                                TotalDiscounts = g.First().TotalDiscounts,
                                Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                            });
                foreach (var item in list)
                {
                    ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                    //aLedger.UnitId = (int) item.UnitId;
                    aLedger.ShowRoomId = (int)item.ShowRoomId;
                    aLedger.CustomerId = (int)item.CustomerId;
                    aLedger.CustomerName = (string)item.CustomerName;
                    aLedger.Address = (string)item.Address;
                    aLedger.Phone = (string)item.Phone;
                    aLedger.Balance = (double)item.Balance;
                    aLedger.TotalDiscounts = (double)item.TotalDiscounts;
                    aLedger.TotalPayments = (double)item.TotalPayments;
                    aLedger.TotalBf = (double)item.TotalBf;
                    aLedger.Opening = (double)item.Opening;
                    aLedger.ActualSales = (double)item.ActualSales;
                    aLedger.GatOther = (double)item.GatOther;
                    aLedger.MemoDiscount = (double)item.MemoDiscount;
                    reportList.Add(aLedger);
                }
            }
            else if (User.IsInRole("Zone Manager"))
            {

                var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();
                var zoneManagerId = db.ZoneManagers.Where(a => a.Id == userId).Select(a => a.ZoneManagerIdAlias).FirstOrDefault();
                var managerZoneList = db.SaleZones.Where(a => a.ZoneManagerId == zoneManagerId).Select(a => a.SaleZoneId).ToArray();

                var inIds = String.Join(",", managerZoneList.Select(x => x.ToString()).ToArray());

                List<ClosingBalanceReportView> reportListZone = new List<ClosingBalanceReportView>();
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PCBookWebAppContext"].ConnectionString);
                connection.Open();
                try
                {
                    SqlDataReader reader = null;
                    string sql = "";
                    sql = @"SELECT dbo.Customers.CustomerId, dbo.Customers.CustomerName, dbo.Customers.CustomerNameBangla, dbo.Customers.ShopName, dbo.Customers.Address, dbo.Customers.AddressBangla, dbo.Customers.Image, dbo.Customers.Phone, dbo.Customers.Email, dbo.Customers.CreditLimit, dbo.Customers.Active, dbo.Customers.UnitId, dbo.Customers.SalesManId, dbo.Customers.UpazilaId, dbo.Customers.ShowRoomId, dbo.Upazilas.UpazilaName, dbo.Upazilas.UpazilaNameBangla, dbo.Upazilas.DistrictId, dbo.Districts.DistrictName, dbo.Districts.DistrictNameBangla, dbo.Districts.SaleZoneId, dbo.SaleZones.SaleZoneName, dbo.SaleZones.SaleZoneDescription, dbo.SaleZones.DivisionId, dbo.Divisions.DivisionName, dbo.Divisions.DivisionNameBangla, dbo.SaleZones.ZoneManagerId, dbo.ZoneManagers.ZoneManagerName FROM dbo.Customers INNER JOIN dbo.Upazilas ON dbo.Customers.UpazilaId = dbo.Upazilas.UpazilaId INNER JOIN dbo.Districts ON dbo.Upazilas.DistrictId = dbo.Districts.DistrictId INNER JOIN dbo.SaleZones ON dbo.Districts.SaleZoneId = dbo.SaleZones.SaleZoneId INNER JOIN dbo.Divisions ON dbo.SaleZones.DivisionId = dbo.Divisions.DivisionId INNER JOIN dbo.ZoneManagers ON dbo.SaleZones.ZoneManagerId = dbo.ZoneManagers.ZoneManagerId WHERE (dbo.Customers.UnitId = @unitId) AND (dbo.Districts.SaleZoneId IN(" + inIds + "))";

                    SqlCommand command = new SqlCommand(sql, connection);
                    //command.Parameters.Add(new SqlParameter("@fromDate", fdate));
                    command.Parameters.Add(new SqlParameter("@unitId", unitId));
                    //command.Parameters.Add(new SqlParameter("@showRoomId", showRoomId));
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int customerId = (int)reader["CustomerId"];

                        //CustomerView aObj = new CustomerView();
                        //aObj.CustomerId = (int)reader["CustomerId"];
                        //aObj.CustomerName = (string)reader["CustomerName"];
                        //aObj.CustomerNameBangla = (string)reader["CustomerNameBangla"];
                        //aObj.AddressBangla = (string)reader["AddressBangla"];
                        //aObj.CreditLimit = (double)reader["CreditLimit"];
                        //aObj.ShopName = (string)reader["ShopName"];
                        //aObj.UpazilaName = (string)reader["UpazilaName"];
                        //aObj.DistrictName = (string)reader["DistrictName"];
                        //aObj.SaleZoneName = (string)reader["SaleZoneName"];
                        //aObj.ZoneManagerName = (string)reader["ZoneManagerName"];
                        //aObj.DivisionName = (string)reader["DivisionName"];
                        //if (reader["Image"] != DBNull.Value)
                        //{
                        //    aObj.Image = (string)reader["Image"];
                        //}
                        //var customerTransctionSum = db.Customers
                        //    .Include(c => c.MemoMasters)
                        //    .Include(c => c.Payments)
                        //    .Select(c => new
                        //    {
                        //        c.CustomerId,
                        //        CustomerName = c.CustomerName,
                        //        MemoDiscount = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.MemoDiscount) ?? 0,
                        //        GatOther = c.MemoMasters.Select(a => new { a.MemoMasterId, a.CustomerId, a.MemoDiscount, a.GatOther }).Sum(s => s.GatOther) ?? 0,
                        //        GrossSales = c.MemoMasters.Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,
                        //        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                        //        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                        //        TotalDiscounts = c.Payments.Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                        //    })
                        //    .Where(c => c.CustomerId == customerId)
                        //    .ToArray();

                        //double bf = customerTransctionSum[0].TotalBf;
                        //double tSale = customerTransctionSum[0].GrossSales - customerTransctionSum[0].MemoDiscount + customerTransctionSum[0].GatOther;
                        //double tPayments = customerTransctionSum[0].TotalPayments;
                        //double tDiscount = customerTransctionSum[0].TotalDiscounts;
                        //double actualCredit = customerTransctionSum[0].TotalBf + customerTransctionSum[0].GrossSales - customerTransctionSum[0].MemoDiscount + customerTransctionSum[0].GatOther - customerTransctionSum[0].TotalPayments - customerTransctionSum[0].TotalDiscounts;



                        var customerWiseTransction = db.Customers
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
                                                    .Select(c => new {
                                                        UnitId = c.UnitId,
                                                        CustomerId = c.CustomerId,
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
                                                        MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

                                                        OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                        OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                        OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                        MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                        GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                        GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                        OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                        OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                        OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

                                                        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                        TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                                                    })
                                                    .Where(c =>  c.CustomerId == customerId)
                                                    .ToList()
                                                    .GroupBy(x => new { x.CustomerId })
                                                    .Select(
                                                            g => new
                                                            {
                                                                Key = g.Key,
                                                                UnitId = g.First().UnitId,
                                                                CustomerId = g.First().CustomerId,
                                                                CustomerName = g.First().CustomerName,
                                                                Address = g.First().Address,
                                                                Phone = g.First().Phone,
                                                                DistrictName = g.First().DistrictName,
                                                                UpazilaName = g.First().UpazilaName,
                                                                SaleZoneName = g.First().SaleZoneName,
                                                                ZoneManagerName = g.First().ZoneManagerName,
                                                                DivisionName = g.First().DivisionName,                                                                
                                                                Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                                                MemoDiscount = g.First().MemoDiscount,
                                                                GatOther = g.First().GatOther,
                                                                TotalPayments = g.First().TotalPayments,
                                                                ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                                                TotalBf = g.First().TotalBf,
                                                                TotalDiscounts = g.First().TotalDiscounts,
                                                                Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                                                            }).ToList();

                        if (customerWiseTransction != null) {

                            ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                            //aLedger.UnitId = (int) item.UnitId;
                            //aLedger.ShowRoomId = (int)item.ShowRoomId;
                            aLedger.CustomerId = (int)customerWiseTransction[0].CustomerId;
                            aLedger.CustomerName = (string)customerWiseTransction[0].CustomerName;
                            aLedger.Address = (string)customerWiseTransction[0].Address;

                            aLedger.DistrictName = (string)customerWiseTransction[0].DistrictName;
                            aLedger.SaleZoneName = (string)customerWiseTransction[0].SaleZoneName;
                            aLedger.ZoneManagerName = (string)customerWiseTransction[0].ZoneManagerName;
                            aLedger.DivisionName = (string)customerWiseTransction[0].DivisionName;
                            aLedger.UpazilaName = (string)customerWiseTransction[0].UpazilaName;
                            //aLedger.Phone = (string)item.Phone;
                            aLedger.Balance = (double)customerWiseTransction[0].Balance;
                            aLedger.TotalDiscounts = (double)customerWiseTransction[0].TotalDiscounts;
                            aLedger.TotalPayments = (double)customerWiseTransction[0].TotalPayments;
                            aLedger.TotalBf = (double)customerWiseTransction[0].TotalBf;
                            aLedger.Opening = (double)customerWiseTransction[0].Opening;
                            aLedger.ActualSales = (double)customerWiseTransction[0].ActualSales;
                            aLedger.GatOther = (double)customerWiseTransction[0].GatOther;
                            aLedger.MemoDiscount = (double)customerWiseTransction[0].MemoDiscount;
                            reportListZone.Add(aLedger);

                        }

                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                if (reportListZone != null)
                {
                    this.HttpContext.Session["rptName"] = "SalesClosingBalance.rpt";
                    this.HttpContext.Session["rptFromDate"] = FromDate;
                    this.HttpContext.Session["rptToDate"] = ToDate;
                    this.HttpContext.Session["rptShowType"] = ShowType;
                    if (User.IsInRole("Unit Manager"))
                    {
                        var unitName = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.Unit.UnitName).FirstOrDefault();
                        this.HttpContext.Session["rptUnitName"] = unitName;
                    }
                    else
                    {
                        this.HttpContext.Session["rptUnitName"] = showRoomName;
                    }

                    //return jsonResult;

                    this.HttpContext.Session["rptTitle"] = "Closing Balance";
                    this.HttpContext.Session["rptSource"] = reportListZone;
                    return Json(reportListZone, JsonRequestBehavior.AllowGet);
                }
                else{
                    return Json("NoRecord", JsonRequestBehavior.AllowGet);
                }
            }
            else if (User.IsInRole("Cash Sale"))
            {

            }
            else if (User.IsInRole("Unit Manager"))
            {
                var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
                if (UnitManagerReportGroup == "Division")
                {
                    var divisionId = GroupIds[0];
                    var linkQListDateBeteen = db.Customers
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
                                                .Select(c => new {
                                                    UnitId = c.UnitId,
                                                    CustomerId = c.CustomerId,
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
                                                    MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

                                                    OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                    OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                    OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                    MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                    GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                    GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                    OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                    OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                    OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

                                                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                    TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                                                })
                                                .Where(c => (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party Pakiza Print")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party Pakiza Textile")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party Pakiza Fabrics")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party Pakiza Store")
                                                                && (c.UnitId == unitId && c.DivisionId == divisionId && c.CustomerName != "Cash Party PSC Islampur"))
                                                .ToList()
                                                .GroupBy(x => new { x.UnitId, x.CustomerId })
                                                .Select(
                                                        g => new
                                                        {
                                                            Key = g.Key,
                                                            UnitId = g.First().UnitId,
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
                                                            Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                                            MemoDiscount = g.First().MemoDiscount,
                                                            GatOther = g.First().GatOther,
                                                            TotalPayments = g.First().TotalPayments,
                                                            ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                                            TotalBf = g.First().TotalBf,
                                                            TotalDiscounts = g.First().TotalDiscounts,
                                                            Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                                                        });

                    foreach (var item in linkQListDateBeteen)
                    {
                        ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                        //aLedger.UnitId = (int) item.UnitId;
                        //aLedger.ShowRoomId = (int)item.ShowRoomId;
                        aLedger.CustomerId = (int)item.CustomerId;
                        aLedger.CustomerName = (string)item.CustomerName;
                        //aLedger.Address = (string)item.Address;

                        aLedger.DistrictName = (string)item.DistrictName;
                        aLedger.SaleZoneName = (string)item.SaleZoneName;
                        aLedger.ZoneManagerName = (string)item.ZoneManagerName;
                        aLedger.DivisionName = (string)item.DivisionName;
                        //aLedger.UpazilaName = (string)item.UpazilaName;
                        //aLedger.Phone = (string)item.Phone;
                        aLedger.Balance = (double)item.Balance;
                        aLedger.TotalDiscounts = (double)item.TotalDiscounts;
                        aLedger.TotalPayments = (double)item.TotalPayments;
                        aLedger.TotalBf = (double)item.TotalBf;
                        aLedger.Opening = (double)item.Opening;
                        aLedger.ActualSales = (double)item.ActualSales;
                        aLedger.GatOther = (double)item.GatOther;
                        aLedger.MemoDiscount = (double)item.MemoDiscount;
                        reportList.Add(aLedger);
                    }
                }
                else if (UnitManagerReportGroup == "ZoneManager")
                {
                    var zoneManagerId = GroupIds[0];
                    var linkQListDateBeteen = db.Customers
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
                                                .Select(c => new {
                                                    UnitId = c.UnitId,
                                                    CustomerId = c.CustomerId,
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
                                                    MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

                                                    OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                    OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                    OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                    MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                    GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                    GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                    OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                    OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                    OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

                                                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                    TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                                                })
                                                .Where(c => (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party Pakiza Print")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party Pakiza Textile")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party Pakiza Fabrics")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party Pakiza Store")
                                                                && (c.UnitId == unitId && c.ZoneManagerId == zoneManagerId && c.CustomerName != "Cash Party PSC Islampur"))
                                                .ToList()
                                                .GroupBy(x => new { x.UnitId, x.CustomerId })
                                                .Select(
                                                        g => new
                                                        {
                                                            Key = g.Key,
                                                            UnitId = g.First().UnitId,
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
                                                            Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                                            MemoDiscount = g.First().MemoDiscount,
                                                            GatOther = g.First().GatOther,
                                                            TotalPayments = g.First().TotalPayments,
                                                            ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                                            TotalBf = g.First().TotalBf,
                                                            TotalDiscounts = g.First().TotalDiscounts,
                                                            Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                                                        });

                    foreach (var item in linkQListDateBeteen)
                    {
                        ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                        //aLedger.UnitId = (int) item.UnitId;
                        //aLedger.ShowRoomId = (int)item.ShowRoomId;
                        aLedger.CustomerId = (int)item.CustomerId;
                        aLedger.CustomerName = (string)item.CustomerName;
                        //aLedger.Address = (string)item.Address;

                        aLedger.DistrictName = (string)item.DistrictName;
                        aLedger.SaleZoneName = (string)item.SaleZoneName;
                        aLedger.ZoneManagerName = (string)item.ZoneManagerName;
                        aLedger.DivisionName = (string)item.DivisionName;
                        //aLedger.UpazilaName = (string)item.UpazilaName;
                        //aLedger.Phone = (string)item.Phone;
                        aLedger.Balance = (double)item.Balance;
                        aLedger.TotalDiscounts = (double)item.TotalDiscounts;
                        aLedger.TotalPayments = (double)item.TotalPayments;
                        aLedger.TotalBf = (double)item.TotalBf;
                        aLedger.Opening = (double)item.Opening;
                        aLedger.ActualSales = (double)item.ActualSales;
                        aLedger.GatOther = (double)item.GatOther;
                        aLedger.MemoDiscount = (double)item.MemoDiscount;
                        reportList.Add(aLedger);
                    }
                }
                else if (UnitManagerReportGroup == "Zone")
                {
                    var linkQListDateBeteen = db.Customers
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
                                                .Select(c => new {
                                                    UnitId = c.UnitId,
                                                    CustomerId = c.CustomerId,
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
                                                    MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

                                                    OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                    OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                    OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                    MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                                    GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                                    GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                                    OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                    OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                    OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

                                                    TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                                    TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                                    TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                                                })
                                                .Where(c => (GroupIds.Contains((int) c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Print")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Textile")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Fabrics")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Store")
                                                                && (GroupIds.Contains((int)c.SaleZoneId) && c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur"))
                                                .ToList()
                                                .GroupBy(x => new { x.UnitId, x.CustomerId })
                                                .Select(
                                                        g => new
                                                        {
                                                            Key = g.Key,
                                                            UnitId = g.First().UnitId,
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
                                                            Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                                            MemoDiscount = g.First().MemoDiscount,
                                                            GatOther = g.First().GatOther,
                                                            TotalPayments = g.First().TotalPayments,
                                                            ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                                            TotalBf = g.First().TotalBf,
                                                            TotalDiscounts = g.First().TotalDiscounts,
                                                            Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                                                        });

                    foreach (var item in linkQListDateBeteen)
                    {
                        ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                        //aLedger.UnitId = (int) item.UnitId;
                        //aLedger.ShowRoomId = (int)item.ShowRoomId;
                        aLedger.CustomerId = (int)item.CustomerId;
                        aLedger.CustomerName = (string)item.CustomerName;
                        //aLedger.Address = (string)item.Address;

                        aLedger.DistrictName = (string)item.DistrictName;
                        aLedger.SaleZoneName = (string)item.SaleZoneName;
                        aLedger.ZoneManagerName = (string)item.ZoneManagerName;
                        aLedger.DivisionName = (string)item.DivisionName;
                        //aLedger.UpazilaName = (string)item.UpazilaName;
                        //aLedger.Phone = (string)item.Phone;
                        aLedger.Balance = (double)item.Balance;
                        aLedger.TotalDiscounts = (double)item.TotalDiscounts;
                        aLedger.TotalPayments = (double)item.TotalPayments;
                        aLedger.TotalBf = (double)item.TotalBf;
                        aLedger.Opening = (double)item.Opening;
                        aLedger.ActualSales = (double)item.ActualSales;
                        aLedger.GatOther = (double)item.GatOther;
                        aLedger.MemoDiscount = (double)item.MemoDiscount;
                        reportList.Add(aLedger);
                    }
                }
                else if (UnitManagerReportGroup == "District")
                {
                    var linkQListDateBeteen = db.Customers
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
                            .Select(c => new {
                                UnitId = c.UnitId,
                                CustomerId = c.CustomerId,
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
                                MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

                                OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                                GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                                GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                                OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

                                TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                                TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                                TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
                            })
                            .Where(c => (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur (Alomgir)")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Print")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Textile")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Fabrics")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party PSC Madhobdi (Alomgir)")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party Pakiza Store")
                                            && (GroupIds.Contains((int)c.DistrictId) && c.UnitId == unitId && c.CustomerName != "Cash Party PSC Islampur"))
                            .ToList()
                            .GroupBy(x => new { x.UnitId, x.CustomerId })
                            .Select(
                                    g => new
                                    {
                                        Key = g.Key,
                                        UnitId = g.First().UnitId,
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
                                        Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                        MemoDiscount = g.First().MemoDiscount,
                                        GatOther = g.First().GatOther,
                                        TotalPayments = g.First().TotalPayments,
                                        ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                        TotalBf = g.First().TotalBf,
                                        TotalDiscounts = g.First().TotalDiscounts,
                                        Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                                    });

                    foreach (var item in linkQListDateBeteen)
                    {
                        ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                        //aLedger.UnitId = (int) item.UnitId;
                        //aLedger.ShowRoomId = (int)item.ShowRoomId;
                        aLedger.CustomerId = (int)item.CustomerId;
                        aLedger.CustomerName = (string)item.CustomerName;
                        //aLedger.Address = (string)item.Address;

                        aLedger.DistrictName = (string)item.DistrictName;
                        aLedger.SaleZoneName = (string)item.SaleZoneName;
                        aLedger.ZoneManagerName = (string)item.ZoneManagerName;
                        aLedger.DivisionName = (string)item.DivisionName;
                        //aLedger.UpazilaName = (string)item.UpazilaName;
                        //aLedger.Phone = (string)item.Phone;
                        aLedger.Balance = (double)item.Balance;
                        aLedger.TotalDiscounts = (double)item.TotalDiscounts;
                        aLedger.TotalPayments = (double)item.TotalPayments;
                        aLedger.TotalBf = (double)item.TotalBf;
                        aLedger.Opening = (double)item.Opening;
                        aLedger.ActualSales = (double)item.ActualSales;
                        aLedger.GatOther = (double)item.GatOther;
                        aLedger.MemoDiscount = (double)item.MemoDiscount;
                        reportList.Add(aLedger);
                    }
                }


            }

            if (reportList != null)
            {                
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                if (User.IsInRole("Unit Manager"))
                {
                    var unitName = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.Unit.UnitName).FirstOrDefault();
                    this.HttpContext.Session["rptName"] = "SalesZoneClosingBalance.rpt";
                    this.HttpContext.Session["rptUnitName"] = unitName;
                }
                else {
                    this.HttpContext.Session["rptName"] = "SalesClosingBalance.rpt";
                    this.HttpContext.Session["rptUnitName"] = showRoomName;
                }
                
                this.HttpContext.Session["rptTitle"] = "Closing Balance";
                this.HttpContext.Session["rptSource"] = reportList;
                return Json(reportList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ShowClosingBalanceSummaryRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();

            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            List<ClosingBalanceReportView> reportList = new List<ClosingBalanceReportView>();

                var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
                var linkQListDateBeteen = db.Customers
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
                    .Select(c => new {
                        UnitId = c.UnitId,
                        CustomerId = c.CustomerId,
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
                        MemoDate = c.MemoMasters.Select(a => new { a.MemoDate }),

                        OpeningMemoDiscount = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                        OpeningGatOther = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                        OpeningGrossSales = c.MemoMasters.Where(s => s.MemoDate < fromDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                        MemoDiscount = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoDiscount }).Sum(s => s.MemoDiscount) ?? 0,
                        GatOther = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.GatOther }).Sum(s => s.GatOther) ?? 0,
                        GrossSales = c.MemoMasters.Where(s => s.MemoDate >= fromDate && s.MemoDate <= toDate).Select(a => new { a.MemoCost }).Sum(s => (double?)s.MemoCost) ?? 0,

                        OpeningTotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate < fromDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                        OpeningTotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate < fromDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                        OpeningTotalDiscounts = c.Payments.Where(s => s.PaymentDate < fromDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,

                        TotalBf = c.Payments.Where(s => s.AdjustmentBf == true && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(s => new { s.SSAmount }).Sum(s => (double?)s.SSAmount) ?? 0,
                        TotalPayments = c.Payments.Where(s => s.AdjustmentBf == false && s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SCAmount }).Sum(s => (double?)s.SCAmount) ?? 0,
                        TotalDiscounts = c.Payments.Where(s => s.PaymentDate >= fromDate && s.PaymentDate <= toDate).Select(a => new { a.SDiscount }).Sum(s => (double?)s.SDiscount) ?? 0,
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
                    .GroupBy(x => new { x.UnitId, x.CustomerId })
                    .Select(
                            g => new
                            {
                                Key = g.Key,
                                UnitId = g.First().UnitId,
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
                                Opening = g.First().OpeningTotalBf + g.First().OpeningGrossSales + g.First().OpeningGatOther - g.First().OpeningMemoDiscount - g.First().OpeningTotalDiscounts - g.First().OpeningTotalPayments,
                                MemoDiscount = g.First().MemoDiscount,
                                GatOther = g.First().GatOther,
                                TotalPayments = g.First().TotalPayments,
                                ActualSales = g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount,
                                TotalBf = g.First().TotalBf,
                                TotalDiscounts = g.First().TotalDiscounts,
                                Balance = g.First().TotalBf + g.First().GrossSales + g.First().GatOther - g.First().MemoDiscount - g.First().TotalDiscounts - g.First().TotalPayments,
                            });
                //var jsonResult = Json(linkQListDateBeteen, JsonRequestBehavior.AllowGet);
                //jsonResult.MaxJsonLength = int.MaxValue;

                foreach (var item in linkQListDateBeteen)
                {
                    ClosingBalanceReportView aLedger = new ClosingBalanceReportView();
                    //aLedger.UnitId = (int) item.UnitId;
                    //aLedger.ShowRoomId = (int)item.ShowRoomId;
                    aLedger.CustomerId = (int)item.CustomerId;
                    aLedger.CustomerName = (string)item.CustomerName;
                    //aLedger.Address = (string)item.Address;

                    aLedger.DistrictName = (string)item.DistrictName;
                    aLedger.SaleZoneName = (string)item.SaleZoneName;
                    aLedger.ZoneManagerName = (string)item.ZoneManagerName;
                    aLedger.DivisionName = (string)item.DivisionName;
                    //aLedger.UpazilaName = (string)item.UpazilaName;
                    //aLedger.Phone = (string)item.Phone;
                    aLedger.Balance = (double)item.Balance;
                    aLedger.TotalDiscounts = (double)item.TotalDiscounts;
                    aLedger.TotalPayments = (double)item.TotalPayments;
                    aLedger.TotalBf = (double)item.TotalBf;
                    aLedger.Opening = (double)item.Opening;
                    aLedger.ActualSales = (double)item.ActualSales;
                    aLedger.GatOther = (double)item.GatOther;
                    aLedger.MemoDiscount = (double)item.MemoDiscount;
                    reportList.Add(aLedger);
                }


            if (reportList != null)
            {
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                if (User.IsInRole("Unit Manager"))
                {
                    var unitName = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.Unit.UnitName).FirstOrDefault();
                    this.HttpContext.Session["rptName"] = "SalesZoneClosingBalanceSummary.rpt";
                    this.HttpContext.Session["rptUnitName"] = unitName;
                }

                this.HttpContext.Session["rptTitle"] = "Closing Balance";
                this.HttpContext.Session["rptSource"] = reportList;
                return Json(reportList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ShowMemoWiseRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            List<MemoReportView> reportList = new List<MemoReportView>();
            if (User.IsInRole("Show Room Sales"))
            {
                var list = db.MemoDetails
                    .Include(m => m.MemoMaster)
                    .Include(m => m.Product)
                    .Include(m => m.Product.SubCategory)
                    .Include(m => m.Product.SubCategory.MainCategory)
                    .Where(m => m.MemoMaster.ShowRoomId == showRoomId && m.MemoMaster.MemoDate >= fromDate && m.MemoMaster.MemoDate <= toDate)
                    .Select(m => new
                    {
                        MemoMasterId = m.MemoMaster.MemoMasterId,
                        ShowRoomId = m.MemoMaster.ShowRoomId,
                        MemoNo = m.MemoMaster.MemoNo,
                        MemoCost = m.MemoMaster.MemoCost != null ? m.MemoMaster.MemoCost : 0,
                        MemoDiscount = m.MemoMaster.MemoDiscount,
                        GatOther = m.MemoMaster.GatOther,
                        m.MemoDetailId,
                        m.ProductId,
                        ProductName = m.Product.ProductName,
                        m.Quantity,
                        m.Rate,
                        m.Discount,
                        MemoDate = m.MemoMaster.MemoDate,
                        SubCategoryId = m.Product.SubCategoryId,
                        SubCategoryName = m.Product.SubCategory.SubCategoryName,
                        MainCategoryId = m.Product.SubCategory.MainCategoryId,
                        MainCategoryName = m.Product.SubCategory.MainCategory.MainCategoryName,
                        PrintQu = m.Product.SubCategoryId == 1 ? m.Quantity : 0,
                        PoplinQu = m.Product.SubCategoryId == 2 ? m.Quantity : 0,
                        VoilQu = m.Product.SubCategoryId == 3 ? m.Quantity : 0,
                        PrintPoplinVoilQu = m.Product.SubCategory.MainCategoryId == 1 ? m.Quantity : 0,
                        ThreePicQu = m.Product.SubCategory.MainCategoryId == 2 ? m.Quantity : 0,
                        ShareeQu = m.Product.SubCategory.MainCategoryId == 3 ? m.Quantity : 0,
                        BedSheetQu = m.Product.SubCategory.MainCategoryId == 4 ? m.Quantity : 0,
                        PordaQu = m.Product.SubCategory.MainCategoryId == 5 ? m.Quantity : 0,
                        BedQu = m.Product.SubCategory.MainCategoryId == 6 ? m.Quantity : 0,
                        BedCoverQu = m.Product.SubCategory.MainCategoryId == 7 ? m.Quantity : 0,
                        LungeeaQu = m.Product.SubCategory.MainCategoryId == 8 ? m.Quantity : 0,
                        OrnaQu = m.Product.SubCategory.MainCategoryId == 9 ? m.Quantity : 0,
                    })
                    .ToList()
                    .GroupBy(x => new { x.ShowRoomId, x.MemoMasterId })
                    .Select(g => new {
                        Key = g.Key,
                        GroupItem = g,
                        MemoMasterId = g.First().MemoMasterId,
                        MemoNo = g.First().MemoNo,
                        MemoCost = g.First().MemoCost,
                        MemoDiscount = g.First().MemoDiscount,
                        GatOther = g.First().GatOther,
                        MemoDate = g.First().MemoDate,
                        TotalGroupCount = g.Count(),
                        PrintQu = g.Sum(i => i.PrintQu),
                        PoplinQu = g.Sum(i => i.PoplinQu),
                        VoilQu = g.Sum(i => i.VoilQu),
                        PrintPoplinVoilQu = g.Sum(i => i.PrintPoplinVoilQu),
                        ThreePicQu = g.Sum(i => i.ThreePicQu),
                        ShareeQu = g.Sum(i => i.ShareeQu),
                        BedSheetQu = g.Sum(i => i.BedSheetQu),
                        PordaQu = g.Sum(i => i.PordaQu),
                        BedQu = g.Sum(i => i.BedQu),
                        BedCoverQu = g.Sum(i => i.BedCoverQu),
                        LungeeaQu = g.Sum(i => i.LungeeaQu),
                        OrnaQu = g.Sum(i => i.OrnaQu),
                    })
                    .OrderBy(g => g.MemoMasterId)
                    .ToList();


                foreach (var item in list)
                {
                    MemoReportView aObj = new MemoReportView();
                    aObj.MemoMasterId = (int)item.MemoMasterId;
                    aObj.MemoDate = (DateTime)item.MemoDate;
                    aObj.MemoNo = (string)item.MemoNo;
                    aObj.MemoCost = (double)item.MemoCost;
                    aObj.MemoDiscount = (double)item.MemoDiscount;
                    aObj.GatOther = (double)item.GatOther;
                    aObj.PrintQu = (double)item.PrintQu;
                    aObj.PoplinQu = (double)item.PoplinQu;
                    aObj.VoilQu = (double)item.VoilQu;
                    aObj.PrintPoplinVoilQu = (double)item.PrintPoplinVoilQu;
                    aObj.ThreePicQu = (double)item.ThreePicQu;
                    aObj.ShareeQu = (double)item.ShareeQu;
                    aObj.BedSheetQu = (double)item.BedSheetQu;
                    aObj.PordaQu = (double)item.PordaQu;
                    aObj.BedQu = (double)item.BedQu;
                    aObj.BedCoverQu = (double)item.BedCoverQu;
                    aObj.LungeeaQu = (double)item.LungeeaQu;
                    aObj.OrnaQu = (double)item.OrnaQu;
                    reportList.Add(aObj);
                }
            }
            else if (User.IsInRole("Zone Manager"))
            {

            }
            else if (User.IsInRole("Cash Sale"))
            {

            }
            else if (User.IsInRole("Unit Manager"))
            {
                var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
                var list = db.MemoDetails
                    .Include(m => m.MemoMaster)
                    .Include(m => m.Product)
                    .Include(m => m.Product.SubCategory)
                    .Include(m => m.Product.SubCategory.MainCategory)
                    .Where(m => m.MemoMaster.ShowRoom.UnitId == unitId && m.MemoMaster.MemoDate >= fromDate && m.MemoMaster.MemoDate <= toDate)
                    .Select(m => new
                    {
                        MemoMasterId = m.MemoMaster.MemoMasterId,
                        UnitId = m.MemoMaster.ShowRoom.UnitId,
                        MemoNo = m.MemoMaster.MemoNo,
                        MemoCost = m.MemoMaster.MemoCost != null ? m.MemoMaster.MemoCost : 0,
                        MemoDiscount = m.MemoMaster.MemoDiscount,
                        GatOther = m.MemoMaster.GatOther,
                        m.MemoDetailId,
                        m.ProductId,
                        ProductName = m.Product.ProductName,
                        m.Quantity,
                        m.Rate,
                        m.Discount,
                        MemoDate = m.MemoMaster.MemoDate,
                        SubCategoryId = m.Product.SubCategoryId,
                        SubCategoryName = m.Product.SubCategory.SubCategoryName,
                        MainCategoryId = m.Product.SubCategory.MainCategoryId,
                        MainCategoryName = m.Product.SubCategory.MainCategory.MainCategoryName,
                        PrintQu = m.Product.SubCategoryId == 1 ? m.Quantity : 0,
                        PoplinQu = m.Product.SubCategoryId == 2 ? m.Quantity : 0,
                        VoilQu = m.Product.SubCategoryId == 3 ? m.Quantity : 0,
                        PrintPoplinVoilQu = m.Product.SubCategory.MainCategoryId == 1 ? m.Quantity : 0,
                        ThreePicQu = m.Product.SubCategory.MainCategoryId == 2 ? m.Quantity : 0,
                        ShareeQu = m.Product.SubCategory.MainCategoryId == 3 ? m.Quantity : 0,
                        BedSheetQu = m.Product.SubCategory.MainCategoryId == 4 ? m.Quantity : 0,
                        PordaQu = m.Product.SubCategory.MainCategoryId == 5 ? m.Quantity : 0,
                        BedQu = m.Product.SubCategory.MainCategoryId == 6 ? m.Quantity : 0,
                        BedCoverQu = m.Product.SubCategory.MainCategoryId == 7 ? m.Quantity : 0,
                        LungeeaQu = m.Product.SubCategory.MainCategoryId == 8 ? m.Quantity : 0,
                        OrnaQu = m.Product.SubCategory.MainCategoryId == 9 ? m.Quantity : 0,
                    })
                    .ToList()
                    .GroupBy(x => new { x.UnitId, x.MemoMasterId })
                    .Select(g => new {
                        Key = g.Key,
                        GroupItem = g,
                        MemoMasterId = g.First().MemoMasterId,
                        MemoNo = g.First().MemoNo,
                        MemoCost = g.First().MemoCost,
                        MemoDiscount = g.First().MemoDiscount,
                        GatOther = g.First().GatOther,
                        MemoDate = g.First().MemoDate,
                        TotalGroupCount = g.Count(),
                        PrintQu = g.Sum(i => i.PrintQu),
                        PoplinQu = g.Sum(i => i.PoplinQu),
                        VoilQu = g.Sum(i => i.VoilQu),
                        PrintPoplinVoilQu = g.Sum(i => i.PrintPoplinVoilQu),
                        ThreePicQu = g.Sum(i => i.ThreePicQu),
                        ShareeQu = g.Sum(i => i.ShareeQu),
                        BedSheetQu = g.Sum(i => i.BedSheetQu),
                        PordaQu = g.Sum(i => i.PordaQu),
                        BedQu = g.Sum(i => i.BedQu),
                        BedCoverQu = g.Sum(i => i.BedCoverQu),
                        LungeeaQu = g.Sum(i => i.LungeeaQu),
                        OrnaQu = g.Sum(i => i.OrnaQu),
                    })
                    .OrderBy(g => g.MemoMasterId)
                    .ToList();


                foreach (var item in list)
                {
                    MemoReportView aObj = new MemoReportView();
                    aObj.MemoMasterId = (int)item.MemoMasterId;
                    aObj.MemoDate = (DateTime)item.MemoDate;
                    aObj.MemoNo = (string)item.MemoNo;
                    aObj.MemoCost = (double)item.MemoCost;
                    aObj.MemoDiscount = (double)item.MemoDiscount;
                    aObj.GatOther = (double)item.GatOther;
                    aObj.PrintQu = (double)item.PrintQu;
                    aObj.PoplinQu = (double)item.PoplinQu;
                    aObj.VoilQu = (double)item.VoilQu;
                    aObj.PrintPoplinVoilQu = (double)item.PrintPoplinVoilQu;
                    aObj.ThreePicQu = (double)item.ThreePicQu;
                    aObj.ShareeQu = (double)item.ShareeQu;
                    aObj.BedSheetQu = (double)item.BedSheetQu;
                    aObj.PordaQu = (double)item.PordaQu;
                    aObj.BedQu = (double)item.BedQu;
                    aObj.BedCoverQu = (double)item.BedCoverQu;
                    aObj.LungeeaQu = (double)item.LungeeaQu;
                    aObj.OrnaQu = (double)item.OrnaQu;
                    reportList.Add(aObj);
                }
            }



            if (reportList != null)
            {

                this.HttpContext.Session["rptName"] = "SalesMemoWise.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                if (User.IsInRole("Unit Manager"))
                {
                    var unitName = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.Unit.UnitName).FirstOrDefault();
                    this.HttpContext.Session["rptUnitName"] = unitName;
                }
                else
                {
                    this.HttpContext.Session["rptUnitName"] = showRoomName;
                }
                this.HttpContext.Session["rptTitle"] = "Closing Balance";
                this.HttpContext.Session["rptSource"] = reportList;
                return Json(reportList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowProductWiseRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms
                            .Where(u => u.ShowRoomId == showRoomId)
                            .Select(u => u.ShowRoomName)
                            .FirstOrDefault();
            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);
            List<SalesReportView> reportList = new List<SalesReportView>();

            if (User.IsInRole("Show Room Sales"))
            {
                var list = db.MemoDetails
                                .Include(m => m.MemoMaster)
                                .Include(m => m.Product)
                                .Include(m => m.Product.SubCategory)
                                .Include(m => m.Product.SubCategory.MainCategory)
                                .Where(m => m.MemoMaster.ShowRoomId == showRoomId && m.MemoMaster.MemoDate >= fromDate && m.MemoMaster.MemoDate <= toDate)
                                .Select(m => new
                                {
                                    ShowRoomId = m.MemoMaster.ShowRoomId,
                                    ProductId = m.ProductId,
                                    ProductName = m.Product.ProductName,
                                    m.Quantity,
                                    SubCategoryId = m.Product.SubCategoryId,
                                    SubCategoryName = m.Product.SubCategory.SubCategoryName,
                                    MainCategoryId = m.Product.SubCategory.MainCategoryId,
                                    MainCategoryName = m.Product.SubCategory.MainCategory.MainCategoryName,
                                    PrintQu = m.Product.SubCategoryId == 1 ? m.Quantity : 0,
                                    PoplinQu = m.Product.SubCategoryId == 2 ? m.Quantity : 0,
                                    VoilQu = m.Product.SubCategoryId == 3 ? m.Quantity : 0,
                                    PrintPoplinVoilQu = m.Product.SubCategory.MainCategoryId == 1 ? m.Quantity : 0,
                                    ThreePicQu = m.Product.SubCategory.MainCategoryId == 2 ? m.Quantity : 0,
                                    ShareeQu = m.Product.SubCategory.MainCategoryId == 3 ? m.Quantity : 0,
                                    BedSheetQu = m.Product.SubCategory.MainCategoryId == 4 ? m.Quantity : 0,
                                    PordaQu = m.Product.SubCategory.MainCategoryId == 5 ? m.Quantity : 0,
                                    BedQu = m.Product.SubCategory.MainCategoryId == 6 ? m.Quantity : 0,
                                    BedCoverQu = m.Product.SubCategory.MainCategoryId == 7 ? m.Quantity : 0,
                                    LungeeaQu = m.Product.SubCategory.MainCategoryId == 8 ? m.Quantity : 0,
                                    OrnaQu = m.Product.SubCategory.MainCategoryId == 9 ? m.Quantity : 0,
                                })
                                .ToList()
                                .GroupBy(x => new { x.ShowRoomId, x.SubCategoryId, x.ProductId })
                                .Select(g => new {
                                    Key = g.Key,
                                    GroupItem = g,
                                    Quantity = g.Sum(i => i.Quantity),
                                    ProductId = g.First().ProductId,
                                    ProductName = g.First().ProductName,
                                    SubCategoryId = g.First().SubCategoryId,
                                    SubCategoryName = g.First().SubCategoryName,
                                    TotalGroupCount = g.Count(),
                                    PrintQu = g.Sum(i => i.PrintQu),
                                    PoplinQu = g.Sum(i => i.PoplinQu),
                                    VoilQu = g.Sum(i => i.VoilQu),
                                    PrintPoplinVoilQu = g.Sum(i => i.PrintPoplinVoilQu),
                                    ThreePicQu = g.Sum(i => i.ThreePicQu),
                                    ShareeQu = g.Sum(i => i.ShareeQu),
                                    BedSheetQu = g.Sum(i => i.BedSheetQu),
                                    PordaQu = g.Sum(i => i.PordaQu),
                                    BedQu = g.Sum(i => i.BedQu),
                                    BedCoverQu = g.Sum(i => i.BedCoverQu),
                                    LungeeaQu = g.Sum(i => i.LungeeaQu),
                                    OrnaQu = g.Sum(i => i.OrnaQu),
                                })
                                .OrderBy(g => g.ProductName)
                                .ToList();
                foreach (var item in list)
                {
                    SalesReportView aObj = new SalesReportView();
                    aObj.ProductId = (int)item.ProductId;
                    aObj.ProductName = (string)item.ProductName;
                    aObj.SubCategoryId = (int)item.SubCategoryId;
                    aObj.SubCategoryName = (string)item.SubCategoryName;
                    aObj.Quantity = item.Quantity;
                    aObj.PrintQu = (double)item.PrintQu;
                    aObj.PoplinQu = (double)item.PoplinQu;
                    aObj.VoilQu = (double)item.VoilQu;
                    aObj.PrintPoplinVoilQu = (double)item.PrintPoplinVoilQu;
                    aObj.ThreePicQu = (double)item.ThreePicQu;
                    aObj.ShareeQu = (double)item.ShareeQu;
                    aObj.BedSheetQu = (double)item.BedSheetQu;
                    aObj.PordaQu = (double)item.PordaQu;
                    aObj.BedQu = (double)item.BedQu;
                    aObj.BedCoverQu = (double)item.BedCoverQu;
                    aObj.LungeeaQu = (double)item.LungeeaQu;
                    aObj.OrnaQu = (double)item.OrnaQu;
                    reportList.Add(aObj);
                }

            }
            else if (User.IsInRole("Zone Manager"))
            {

            }
            else if (User.IsInRole("Cash Sale"))
            {

            }
            else if (User.IsInRole("Unit Manager"))
            {
                var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
                var list = db.MemoDetails
                                .Include(m => m.MemoMaster)
                                .Include(m => m.Product)
                                .Include(m => m.Product.SubCategory)
                                .Include(m => m.Product.SubCategory.MainCategory)
                                .Where(m => m.MemoMaster.ShowRoom.UnitId == unitId && m.MemoMaster.MemoDate >= fromDate && m.MemoMaster.MemoDate <= toDate)
                                .Select(m => new
                                {
                                    CustomerId=m.MemoMaster.CustomerId,
                                    UpazilaId=m.MemoMaster.Customer.UpazilaId,
                                    DistrictId=m.MemoMaster.Customer.Upazila.DistrictId,
                                    UpazilaName=m.MemoMaster.Customer.Upazila.UpazilaName,
                                    DistrictName=m.MemoMaster.Customer.Upazila.District.DistrictName,
                                    SaleZoneId=m.MemoMaster.Customer.Upazila.District.SaleZoneId,
                                    ZoneManagerId=m.MemoMaster.Customer.Upazila.District.SaleZone.ZoneManagerId,
                                    SaleZoneName=m.MemoMaster.Customer.Upazila.District.SaleZone.SaleZoneName,
                                    ZoneManagerName=m.MemoMaster.Customer.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                                    DivisionId=m.MemoMaster.Customer.Upazila.District.SaleZone.DivisionId,
                                    DivisionName=m.MemoMaster.Customer.Upazila.District.SaleZone.Division.DivisionName,
                                    UnitId = m.MemoMaster.ShowRoom.UnitId,
                                    ProductId = m.ProductId,
                                    ProductName = m.Product.ProductName,
                                    Quantity=m.Quantity,
                                    SubCategoryId = m.Product.SubCategoryId,
                                    SubCategoryName = m.Product.SubCategory.SubCategoryName,
                                    MainCategoryId = m.Product.SubCategory.MainCategoryId,
                                    MainCategoryName = m.Product.SubCategory.MainCategory.MainCategoryName,
                                    PrintQu = m.Product.SubCategoryId == 1 ? m.Quantity : 0,
                                    PoplinQu = m.Product.SubCategoryId == 2 ? m.Quantity : 0,
                                    VoilQu = m.Product.SubCategoryId == 3 ? m.Quantity : 0,
                                    PrintPoplinVoilQu = m.Product.SubCategory.MainCategoryId == 1 ? m.Quantity : 0,
                                    ThreePicQu = m.Product.SubCategory.MainCategoryId == 2 ? m.Quantity : 0,
                                    ShareeQu = m.Product.SubCategory.MainCategoryId == 3 ? m.Quantity : 0,
                                    BedSheetQu = m.Product.SubCategory.MainCategoryId == 4 ? m.Quantity : 0,
                                    PordaQu = m.Product.SubCategory.MainCategoryId == 5 ? m.Quantity : 0,
                                    BedQu = m.Product.SubCategory.MainCategoryId == 6 ? m.Quantity : 0,
                                    BedCoverQu = m.Product.SubCategory.MainCategoryId == 7 ? m.Quantity : 0,
                                    LungeeaQu = m.Product.SubCategory.MainCategoryId == 8 ? m.Quantity : 0,
                                    OrnaQu = m.Product.SubCategory.MainCategoryId == 9 ? m.Quantity : 0,
                                })
                                .ToList()
                                .GroupBy(x => new { x.UnitId,x.SubCategoryId, x.DistrictId,x.SaleZoneId,x.DivisionId,x.ProductId })
                                .Select(g => new {
                                    Key = g.Key,
                                    GroupItem = g,
                                    UpazilaId=g.First().UpazilaId,
                                    UpazilaName=g.First().UpazilaName,
                                    g.First().DistrictId,
                                    DistrictName=g.First().DistrictName,
                                    g.First().SaleZoneId,
                                    SaleZoneName=g.First().SaleZoneName,
                                    g.First().ZoneManagerId,
                                    ZoneManagerName=g.First().ZoneManagerName,
                                    g.First().DivisionId,
                                    DivisionName=g.First().DivisionName,
                                    Quantity = g.Sum(i => i.Quantity),
                                    ProductId = g.First().ProductId,
                                    ProductName = g.First().ProductName,
                                    SubCategoryId = g.First().SubCategoryId,
                                    SubCategoryName = g.First().SubCategoryName,
                                    TotalGroupCount = g.Count(),
                                    PrintQu = g.Sum(i => i.PrintQu),
                                    PoplinQu = g.Sum(i => i.PoplinQu),
                                    VoilQu = g.Sum(i => i.VoilQu),
                                    PrintPoplinVoilQu = g.Sum(i => i.PrintPoplinVoilQu),
                                    ThreePicQu = g.Sum(i => i.ThreePicQu),
                                    ShareeQu = g.Sum(i => i.ShareeQu),
                                    BedSheetQu = g.Sum(i => i.BedSheetQu),
                                    PordaQu = g.Sum(i => i.PordaQu),
                                    BedQu = g.Sum(i => i.BedQu),
                                    BedCoverQu = g.Sum(i => i.BedCoverQu),
                                    LungeeaQu = g.Sum(i => i.LungeeaQu),
                                    OrnaQu = g.Sum(i => i.OrnaQu),
                                })
                                .OrderBy(g => g.ProductName)
                                .ToList();
                foreach (var item in list)
                {
                    SalesReportView aObj = new SalesReportView();
                    aObj.ProductId = (int)item.ProductId;
                    aObj.ProductName = (string)item.ProductName;
                    aObj.SubCategoryName = (string) item.SubCategoryName;
                    aObj.UpazilaName = (string)item.UpazilaName;
                    aObj.DistrictName = (string)item.DistrictName;
                    aObj.SaleZoneName = (string)item.SaleZoneName;
                    aObj.ZoneManagerName = (string)item.ZoneManagerName;
                    aObj.DivisionName = (string)item.DivisionName;
                    aObj.Quantity = item.Quantity;

                    reportList.Add(aObj);
                }
            }
            if (reportList != null)
            {               
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                if (User.IsInRole("Unit Manager"))
                {
                    var unitName = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.Unit.UnitName).FirstOrDefault();
                    this.HttpContext.Session["rptName"] = "SalesZoneProductWise.rpt";
                    this.HttpContext.Session["rptUnitName"] = unitName;
                }
                else
                {
                    this.HttpContext.Session["rptName"] = "SalesProductWise.rpt";
                    this.HttpContext.Session["rptUnitName"] = showRoomName;
                }
                this.HttpContext.Session["rptTitle"] = "Product wise sale";
                this.HttpContext.Session["rptSource"] = reportList;
                return Json(reportList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShowSaleCollectionRptInNewWin(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            string userName = User.Identity.GetUserName();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();

            DateTime fromDate = DateTime.Parse(FromDate);
            DateTime toDate = DateTime.Parse(ToDate);


            List<SalesCollectionDiscountReportView> reportList = new List<SalesCollectionDiscountReportView>();
            double cashsale = 0;
            double creditsale = 0;
            double other = 0;

            if (User.IsInRole("Show Room Sales"))
            {
                foreach (DateTime day in EachDay(fromDate, toDate))
                {
                    DateTime qDate = day.Date;

                    var creditCollection = db.Payments
                                                .Include(p => p.Customer)
                                                .Select(p => new
                                                {
                                                    p.CustomerId,
                                                    p.Customer.CustomerName,
                                                    p.PaymentId,
                                                    p.PaymentDate,
                                                    p.PaymentType,
                                                    p.SCAmount,
                                                    p.SDiscount,
                                                    p.ShowRoomId
                                                })
                                            .Where(p => p.PaymentDate == qDate && p.PaymentType == "Cash" && p.ShowRoomId == showRoomId)
                                            .ToList()
                                            .Sum(x => x.SCAmount);

                    var cashDiscount = db.Payments
                                        .Include(p => p.Customer)
                                        .Select(p => new
                                        {
                                            p.CustomerId,
                                            p.Customer.CustomerName,
                                            p.PaymentId,
                                            p.PaymentDate,
                                            p.PaymentType,
                                            p.SCAmount,
                                            p.SDiscount,
                                            p.ShowRoomId
                                        })
                                    .Where(p => p.PaymentDate == qDate && p.PaymentType == "Cash" && p.ShowRoomId == showRoomId)
                                    .ToList()
                                    .Sum(x => x.SDiscount);

                    var saleData = db.MemoMasters
                                .Include(m => m.Customer)
                                .Include(m => m.Customer.Payments)
                                .Select(m => new
                                {
                                    m.MemoMasterId,
                                    m.MemoNo,
                                    m.ShowRoomId,
                                    m.CustomerId,
                                    m.Customer.CustomerName,
                                    SaleType = m.Customer.CustomerName == "Cash Party" ? "Cash Sale" : "Credit Sale",
                                    m.MemoDate,
                                    m.GatOther,
                                    m.MemoDiscount,
                                    m.MemoCost,
                                    CashSale = m.Customer.CustomerName == "Cash Party" ? m.MemoCost : 0,
                                    CreditSale = m.Customer.CustomerName != "Cash Party" ? m.MemoCost : 0,

                                    CashGat = m.Customer.CustomerName == "Cash Party" ? m.GatOther : 0,
                                    CreditGat = m.Customer.CustomerName != "Cash Party" ? m.GatOther : 0,

                                    CashDis = m.Customer.CustomerName == "Cash Party" ? m.MemoDiscount : 0,
                                    CreditDis = m.Customer.CustomerName != "Cash Party" ? m.MemoDiscount : 0,

                                })
                                .Where(m => m.MemoDate == qDate && m.ShowRoomId == showRoomId)
                                .ToList()
                                .GroupBy(g => new { g.ShowRoomId, g.MemoDate })
                                .Select(f => new
                                {
                                    f.Key,
                                    f.First().MemoDate,
                                    TotalOthers = f.Sum(i => i.GatOther) ?? 0,
                                    TotalCashSale = f.Sum(i => (double?)i.CashSale) ?? 0,
                                    TotalCreditSale = f.Sum(i => (double?)i.CreditSale) ?? 0,

                                    CashGat = f.Sum(i => (double?)i.CashGat) ?? 0,
                                    CreditGat = f.Sum(i => (double?)i.CreditGat) ?? 0,

                                    CashDis = f.Sum(i => (double?)i.CashDis) ?? 0,
                                    CreditDis = f.Sum(i => (double?)i.CreditDis) ?? 0,

                                }).ToList()
                                .Select(ii => new {
                                    TotalOthers = ii.TotalOthers,
                                    TotalCashSale = Convert.ToDouble(ii.TotalCashSale) - Convert.ToDouble(ii.CashDis) + Convert.ToDouble(ii.CashGat),
                                    TotalCreditSale = Convert.ToDouble(ii.TotalCreditSale) - Convert.ToDouble(ii.CreditDis) + Convert.ToDouble(ii.CreditGat)
                                }).FirstOrDefault();

                    if (saleData != null)
                    {
                        cashsale = saleData.TotalCashSale;
                        creditsale = saleData.TotalCreditSale;
                        other = saleData.TotalOthers;
                    }


                    if (cashDiscount + creditCollection + other + cashsale + creditsale > 0)
                    {
                        SalesCollectionDiscountReportView aLedger = new SalesCollectionDiscountReportView();
                        aLedger.Date = (DateTime)qDate.Date;
                        aLedger.Discount = (double)cashDiscount;
                        aLedger.Collection = (double)creditCollection;
                        aLedger.Other = (double)other;
                        aLedger.CreditSale = cashsale;
                        aLedger.CashSale = creditsale;
                        reportList.Add(aLedger);
                    }

                    cashsale = 0;
                    creditsale = 0;
                }
            }
            else if (User.IsInRole("Zone Manager"))
            {

            }
            else if (User.IsInRole("Cash Sale"))
            {

            }
            else if (User.IsInRole("Unit Manager"))
            {
                var unitId = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.UnitId).FirstOrDefault();
                foreach (DateTime day in EachDay(fromDate, toDate))
                {
                    DateTime qDate = day.Date;

                    var creditCollection = db.Payments
                                                .Include(p => p.Customer)
                                                .Select(p => new
                                                {
                                                    p.Customer.UpazilaId,
                                                    p.Customer.Upazila.UpazilaName,
                                                    p.Customer.Upazila.DistrictId,
                                                    p.Customer.Upazila.District.DistrictName,
                                                    p.Customer.Upazila.District.SaleZoneId,
                                                    p.Customer.Upazila.District.SaleZone.SaleZoneName,
                                                    p.Customer.Upazila.District.SaleZone.ZoneManagerId,
                                                    p.Customer.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                                                    p.Customer.Upazila.District.SaleZone.DivisionId,
                                                    p.Customer.Upazila.District.SaleZone.Division.DivisionName,
                                                    p.CustomerId,
                                                    p.Customer.CustomerName,
                                                    p.PaymentId,
                                                    p.PaymentDate,
                                                    p.PaymentType,
                                                    p.SCAmount,
                                                    p.SDiscount,
                                                    p.ShowRoomId,
                                                    p.ShowRoom.UnitId
                                                })
                                            .Where(p => p.PaymentDate == qDate && p.PaymentType == "Cash" && p.UnitId == unitId)
                                            .ToList()
                                            .Sum(x => x.SCAmount);

                    var cashDiscount = db.Payments
                                        .Include(p => p.Customer)
                                        .Select(p => new
                                        {
                                            p.Customer.UpazilaId,
                                            p.Customer.Upazila.UpazilaName,
                                            p.Customer.Upazila.DistrictId,
                                            p.Customer.Upazila.District.DistrictName,
                                            p.Customer.Upazila.District.SaleZoneId,
                                            p.Customer.Upazila.District.SaleZone.SaleZoneName,
                                            p.Customer.Upazila.District.SaleZone.ZoneManagerId,
                                            p.Customer.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                                            p.Customer.Upazila.District.SaleZone.DivisionId,
                                            p.Customer.Upazila.District.SaleZone.Division.DivisionName,
                                            p.CustomerId,
                                            p.Customer.CustomerName,
                                            p.PaymentId,
                                            p.PaymentDate,
                                            p.PaymentType,
                                            p.SCAmount,
                                            p.SDiscount,
                                            p.ShowRoomId,
                                            p.ShowRoom.UnitId
                                        })
                                    .Where(p => p.PaymentDate == qDate && p.PaymentType == "Cash" && p.UnitId == unitId)
                                    .ToList()
                                    .Sum(x => x.SDiscount);

                    var saleData = db.MemoMasters
                                .Include(m => m.Customer)
                                .Include(m => m.Customer.Payments)
                                .Select(m => new
                                {
                                    m.MemoMasterId,
                                    m.MemoNo,
                                    m.ShowRoomId,
                                    UnitId=m.ShowRoom.UnitId,
                                    m.CustomerId,
                                    m.Customer.CustomerName,
                                    m.Customer.UpazilaId,
                                    m.Customer.Upazila.UpazilaName,
                                    m.Customer.Upazila.DistrictId,
                                    m.Customer.Upazila.District.DistrictName,
                                    m.Customer.Upazila.District.SaleZoneId,
                                    m.Customer.Upazila.District.SaleZone.SaleZoneName,
                                    m.Customer.Upazila.District.SaleZone.ZoneManagerId,
                                    m.Customer.Upazila.District.SaleZone.ZoneManager.ZoneManagerName,
                                    m.Customer.Upazila.District.SaleZone.DivisionId,
                                    m.Customer.Upazila.District.SaleZone.Division.DivisionName,
                                    SaleType = m.Customer.CustomerName == "Cash Party" ? "Cash Sale" : "Credit Sale",
                                    m.MemoDate,
                                    m.GatOther,
                                    m.MemoDiscount,
                                    m.MemoCost,
                                    CashSale = m.Customer.CustomerName == "Cash Party" ? m.MemoCost : 0,
                                    CreditSale = m.Customer.CustomerName != "Cash Party" ? m.MemoCost : 0,

                                    CashGat = m.Customer.CustomerName == "Cash Party" ? m.GatOther : 0,
                                    CreditGat = m.Customer.CustomerName != "Cash Party" ? m.GatOther : 0,

                                    CashDis = m.Customer.CustomerName == "Cash Party" ? m.MemoDiscount : 0,
                                    CreditDis = m.Customer.CustomerName != "Cash Party" ? m.MemoDiscount : 0,

                                })
                                .Where(m => m.MemoDate == qDate && m.UnitId == unitId)
                                .ToList()
                                .GroupBy(g => new { g.ShowRoomId, g.MemoDate })
                                .Select(f => new
                                {
                                    f.Key,
                                    f.First().MemoDate,
                                    TotalOthers = f.Sum(i => i.GatOther) ?? 0,
                                    TotalCashSale = f.Sum(i => (double?)i.CashSale) ?? 0,
                                    TotalCreditSale = f.Sum(i => (double?)i.CreditSale) ?? 0,

                                    CashGat = f.Sum(i => (double?)i.CashGat) ?? 0,
                                    CreditGat = f.Sum(i => (double?)i.CreditGat) ?? 0,

                                    CashDis = f.Sum(i => (double?)i.CashDis) ?? 0,
                                    CreditDis = f.Sum(i => (double?)i.CreditDis) ?? 0,

                                }).ToList()
                                .Select(ii => new {
                                    TotalOthers = ii.TotalOthers,
                                    TotalCashSale = Convert.ToDouble(ii.TotalCashSale) - Convert.ToDouble(ii.CashDis) + Convert.ToDouble(ii.CashGat),
                                    TotalCreditSale = Convert.ToDouble(ii.TotalCreditSale) - Convert.ToDouble(ii.CreditDis) + Convert.ToDouble(ii.CreditGat)
                                }).FirstOrDefault();

                    if (saleData != null)
                    {
                        cashsale = saleData.TotalCashSale;
                        creditsale = saleData.TotalCreditSale;
                        other = saleData.TotalOthers;
                    }


                    if (cashDiscount + creditCollection + other + cashsale + creditsale > 0)
                    {
                        SalesCollectionDiscountReportView aLedger = new SalesCollectionDiscountReportView();
                        aLedger.Date = (DateTime)qDate.Date;
                        aLedger.Discount = (double)cashDiscount;
                        aLedger.Collection = (double)creditCollection;
                        aLedger.Other = (double)other;
                        aLedger.CreditSale = cashsale;
                        aLedger.CashSale = creditsale;
                        reportList.Add(aLedger);
                    }

                    cashsale = 0;
                    creditsale = 0;
                }
            }


            if (reportList != null)
            {

                this.HttpContext.Session["rptName"] = "SalesDate.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                if (User.IsInRole("Unit Manager"))
                {
                    var unitName = db.UnitManagers.Where(a => a.Id == userId).Select(a => a.Unit.UnitName).FirstOrDefault();
                    this.HttpContext.Session["rptUnitName"] = unitName;
                }
                else
                {
                    this.HttpContext.Session["rptUnitName"] = showRoomName;
                }
                this.HttpContext.Session["rptTitle"] = "Day wise sale";
                this.HttpContext.Session["rptSource"] = reportList;

                return Json(reportList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportCustomers(string FromDate, string ToDate, int[] LedgerIds, string SelectedReportOption, string ShowType, int MemoMasterId)
        {
            //List<Customer> allCustomer = new List<Customer>();
            //allCustomer = db.Customers.ToList();
            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/CrystalReports"), "SalesMemoBangla.rpt"));

            //rd.SetDataSource(allCustomer);

            //Response.Buffer = false;
            //Response.ClearContent();
            //Response.ClearHeaders();


            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //return File(stream, "application/pdf", "PrintMemoBangla.pdf");

            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();
            var showRoomNameBangla = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomNameBangla).FirstOrDefault();
            var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();

            var aMemo = db.MemoMasters
                            .Include(m => m.MemoDetails)
                            .Include(m => m.Customer)
                            .Include(m => m.Customer.Payments)
                            .Select(m => new
                            {
                                MemoMasterId = m.MemoMasterId,
                                MemoNo = m.MemoNo,
                                CustomerId = m.CustomerId,
                                Address = m.Customer.Address,
                                AddressBangla = m.Customer.AddressBangla,
                                CustomerName = m.Customer.CustomerName,
                                CustomerNameBangla = m.Customer.CustomerNameBangla,
                                UpazilaName = m.Customer.Upazila.UpazilaName,
                                UpazilaNameBangla = m.Customer.Upazila.UpazilaNameBangla,
                                DistrictName = m.Customer.Upazila.District.DistrictName,
                                DistrictNameBangla = m.Customer.Upazila.District.DistrictNameBangla,
                                SaleType = m.Customer.CustomerName == "Cash Party" ? "Cash Sale" : "Credit Sale",
                                MemoDate = m.MemoDate,
                                ShowRoomId = m.ShowRoomId,
                                GatOther = m.GatOther,
                                MemoDiscount = m.MemoDiscount,
                                MemoCost = m.MemoCost,
                                MemoPaidAmount = m.Customer.Payments.Where(p => p.MemoMasterId == m.MemoMasterId).Select(o => new { o.SCAmount }).FirstOrDefault(),
                                MemoItems = m.MemoDetails.Where(p => p.MemoMasterId == m.MemoMasterId).Select(mi => new
                                {
                                    MemoDetailId = mi.MemoDetailId,
                                    ProductId = mi.ProductId,
                                    ProductName = mi.Product.ProductName,
                                    ProductNameBangla = mi.Product.ProductNameBangla,
                                    SubCategoryId = mi.Product.SubCategory.SubCategoryId,
                                    SubCategoryName = mi.Product.SubCategory.SubCategoryName,
                                    MainCategoryId = mi.Product.SubCategory.MainCategory.MainCategoryId,
                                    MainCategoryName = mi.Product.SubCategory.MainCategory.MainCategoryName,
                                    Quantity = mi.Quantity,
                                    Rate = mi.Rate,
                                    Discount = mi.Discount
                                })
                            })
                            .Where(m => m.MemoMasterId == MemoMasterId)
                            .ToList();
            int memoMasterId = 0;
            List<MemoBanglaReportView> reportList = new List<MemoBanglaReportView>();
            foreach (var item in aMemo)
            {
                //MemoBanglaReportView aObj = new MemoBanglaReportView();
                foreach (var aProduct in item.MemoItems)
                {
                    MemoBanglaReportView aObj = new MemoBanglaReportView();
                    //List<MemoItemReportView> itemList = new List<MemoItemReportView>();
                    memoMasterId = (int)item.MemoMasterId;
                    aObj.MemoMasterId = (int)item.MemoMasterId;
                    aObj.MemoDate = (DateTime)item.MemoDate;
                    aObj.MemoNo = (string)item.MemoNo;
                    aObj.CustomerName = (string)item.CustomerName;
                    aObj.CustomerNameBangla = (string)item.CustomerNameBangla;
                    aObj.Address = (string)item.Address;
                    aObj.AddressBangla = (string)item.AddressBangla;

                    aObj.DistrictName = (string)item.DistrictName;
                    aObj.DistrictNameBangla = (string)item.DistrictNameBangla;
                    aObj.UpazilaName = (string)item.UpazilaName;
                    aObj.UpazilaNameBangla = (string)item.UpazilaNameBangla;

                    aObj.MemoCost = (double)item.MemoCost;
                    aObj.GatOther = (double)item.GatOther;
                    aObj.MemoDiscount = (double)item.MemoDiscount;

                    //MemoItemReportView aItem = new MemoItemReportView();
                    //aObj.MemoMasterId = memoMasterId;
                    aObj.ProductId = (int)aProduct.ProductId;
                    aObj.ProductName = (string)aProduct.ProductName;
                    aObj.ProductNameBangla = (string)aProduct.ProductNameBangla;
                    aObj.Quantity = (double)aProduct.Quantity;
                    aObj.Rate = (double)aProduct.Rate;
                    aObj.Discount = (double)aProduct.Discount;
                    //itemList.Add(aItem);
                    reportList.Add(aObj);
                }
                //aObj.MemoItems = itemList;

            }
            if (reportList != null)
            {

                this.HttpContext.Session["rptName"] = "SalesMemoBangla.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomNameBangla;
                this.HttpContext.Session["rptTitle"] = "Product wise sale";
                this.HttpContext.Session["rptSource"] = reportList;
                return Json(reportList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ShowCustomerLedgerRptInNewWin(string FromDate, string ToDate, int LedgerIds, string SelectedReportOption, string ShowType)
        {
            string userId = User.Identity.GetUserId();
            var showRoomId = db.ShowRoomUsers.Where(a => a.Id == userId).Select(a => a.ShowRoomId).FirstOrDefault();
            var showRoomName = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomName).FirstOrDefault();
            var showRoomNameBangla = db.ShowRooms.Where(u => u.ShowRoomId == showRoomId).Select(u => u.ShowRoomNameBangla).FirstOrDefault();
            var unitId = db.ShowRooms.Where(a => a.ShowRoomId == showRoomId).Select(a => a.UnitId).FirstOrDefault();

            DateTime fdate = DateTime.Parse(FromDate);
            DateTime tdate = DateTime.Parse(ToDate);
            double bfAmount = 0;

            var bfList = db.Customers
                .Include(c => c.MemoMasters)
                .Include(c => c.Payments)
                .Select(c => new
                {
                    CustomerId = c.CustomerId,
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
                .Where(c => c.CustomerId == LedgerIds)
                .FirstOrDefault();
            if (bfList !=null) {
                bfAmount =Math.Round( bfList.TotalBf + bfList.GrossSales -bfList.MemoDiscount+bfList.GatOther-bfList.TotalPayments-bfList.TotalDiscounts, 0);
            }
            //Payments
            var paymentsList = db.Payments
                .Include(p => p.Customer)
                .Where(p => p.CustomerId == LedgerIds && p.PaymentDate >= fdate && p.PaymentDate <= tdate)
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
                .Include(m => m.Customer)
                .Where(m => m.CustomerId == LedgerIds && m.MemoDate >= fdate && m.MemoDate <= tdate)
                .Select(m => new {
                    m.MemoMasterId,
                    m.CustomerId,
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
            List<CustomerLedgerReportView> reportList = new List<CustomerLedgerReportView>();

            CustomerLedgerReportView aObjBf = new CustomerLedgerReportView();
            aObjBf.CustomerId = (int) bfList.CustomerId;
            aObjBf.CustomerName = (string) bfList.CustomerName;
            aObjBf.Date = (DateTime) fdate;
            aObjBf.MemoNo = (string) "Opening";
            aObjBf.Sales = 0;
            aObjBf.Payments = 0;
            aObjBf.Discounts = 0;
            aObjBf.Opening = bfAmount;
            reportList.Add(aObjBf);

            foreach (var item in saleList)
            {
                CustomerLedgerReportView aObj = new CustomerLedgerReportView();
                aObj.CustomerId = (int)item.CustomerId;
                aObj.CustomerName = (string)item.CustomerName;
                aObj.Date = (DateTime)item.MemoDate;
                aObj.MemoNo = (string)item.MemoNo;
                aObj.Sales = (double)item.NetMemoAmount;
                aObj.Payments = 0;
                aObj.Discounts = 0;
                aObj.Opening = 0;
                reportList.Add(aObj);
            }
            foreach (var item in paymentsList)
            {
                CustomerLedgerReportView aObj = new CustomerLedgerReportView();
                aObj.CustomerId = (int)item.CustomerId;
                aObj.CustomerName = (string)item.CustomerName;
                aObj.Date = (DateTime)item.PaymentDate;
                aObj.Sales = 0;
                aObj.Payments = (double)item.SCAmount;
                aObj.Discounts = (double)item.SDiscount;
                aObj.Opening = 0;
                reportList.Add(aObj);
            }

            if (reportList != null )
            {

                this.HttpContext.Session["rptName"] = "SalesCustomerRegister.rpt";
                this.HttpContext.Session["rptFromDate"] = FromDate;
                this.HttpContext.Session["rptToDate"] = ToDate;
                this.HttpContext.Session["rptShowType"] = ShowType;
                this.HttpContext.Session["rptUnitName"] = showRoomName;
                this.HttpContext.Session["rptTitle"] = "Customer Ledger";
                this.HttpContext.Session["rptSource"] = reportList;
                return Json(reportList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("NoRecord", JsonRequestBehavior.AllowGet);
            }

        }

    }
}