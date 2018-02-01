using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers
{
    public class BookkeepingController : Controller
    {
        // GET: Bookkeeping
        public ActionResult Index()
        {
            return View();
        }
    }
}