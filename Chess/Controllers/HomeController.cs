using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using UCIProxy;

namespace Chess.Controllers
{
    public class HomeController : Controller
    {
        static readonly UciProxy UciProxy = new UciProxy();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartAnalyze(string fen, string depth)
        {
            var guid = UciProxy.Start(fen, int.Parse(depth));
            return Content(guid.ToString());
        }

        public ActionResult GetOutput(string guid)
        {
            var output = UciProxy.GetProcessOutput(new Guid(guid));
            return Content(output);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}