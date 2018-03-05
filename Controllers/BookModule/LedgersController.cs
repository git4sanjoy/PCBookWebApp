using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers.BookModule
{
    public class LedgersController : Controller
    {
        // GET: Ledgers
        public ActionResult Index()
        {
            return View();
        }
    }
}