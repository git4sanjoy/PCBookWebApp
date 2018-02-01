

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers
{
    public class GenericReportViewerController : Controller
    {
        // GET: GenericReportViewer
        public void ShowGenericRpt()
        {
            try
            {
                bool isValid = true;
                string strRptShowType = System.Web.HttpContext.Current.Session["rptShowType"].ToString();       // Setting Rpt Show Type
                string strReportName = System.Web.HttpContext.Current.Session["rptName"].ToString();            // Setting ReportName
                string strFromDate = System.Web.HttpContext.Current.Session["rptFromDate"].ToString();          // Setting FromDate 
                string strToDate = System.Web.HttpContext.Current.Session["rptToDate"].ToString();              // Setting ToDate                  
                string strUnitName = System.Web.HttpContext.Current.Session["rptUnitName"].ToString();          // Setting Unit Name 
                string strTitle = System.Web.HttpContext.Current.Session["rptTitle"].ToString();                // Setting Report Title
                var rptSource = System.Web.HttpContext.Current.Session["rptSource"];

                if (string.IsNullOrEmpty(strReportName))
                {
                    isValid = false;
                }

                if (isValid)
                {
                    ReportDocument rd = new ReportDocument();
                    string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "CrystalReports//" + strReportName;
                    rd.Load(strRptPath);

                    if (rptSource != null && rptSource.GetType().ToString() != "System.String")
                        rd.SetDataSource(rptSource);

                    if (!string.IsNullOrEmpty(strFromDate))
                        rd.SetParameterValue("fromDate", strFromDate);

                    if (!string.IsNullOrEmpty(strToDate))
                        rd.SetParameterValue("toDate", strToDate);

                    //if (!string.IsNullOrEmpty(strRptShowType))
                    //    rd.SetParameterValue("productName", strRptShowType);

                    if (!string.IsNullOrEmpty(strTitle))
                        rd.SetParameterValue("productName", strTitle);

                    if (!string.IsNullOrEmpty(strUnitName))
                        rd.SetParameterValue("unitName", strUnitName);


                    if (!string.IsNullOrEmpty(strRptShowType) && strRptShowType == "Print")
                    {
                        rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "crReport");
                    } else if (!string.IsNullOrEmpty(strRptShowType) && strRptShowType == "Excel")
                    {
                        rd.ExportToHttpResponse(ExportFormatType.Excel, System.Web.HttpContext.Current.Response, false, "crReport");
                    }


                    // Clear all sessions value
                    Session["rptShowType"] = null;
                    Session["rptName"] = null;
                    Session["rptFromDate"] = null;
                    Session["rptToDate"] = null;
                    Session["rptSource"] = null;                    
                    Session["rptUnitName"] = null;
                    Session["rptTitle"] = null;
                }
                else
                {
                    Response.Write("<H2>Nothing Found; No Report Name found</H2>");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

    }
}