﻿using System;
using System.Web.Mvc;
using UCIProxy;

namespace Chess.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        static readonly UciProxy UciProxy = new UciProxy();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartAnalyze(string fen, string depth, int outputLines)
        {
            var guid = UciProxy.Start(fen, int.Parse(depth), outputLines);
            return Content(guid.ToString());
        }

        public ActionResult GetOutput(string guid)
        {
            var output = UciProxy.GetProcessOutput(new Guid(guid));

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}