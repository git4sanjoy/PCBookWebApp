﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCBookWebApp.Controllers.ProcessModule
{
    public class PurchasedProductsController : Controller
    {
        // GET: PurchasedProducts
        public ActionResult Index()
        {
            return View();
        }
    }
}