using System;
using System.IO;
using System.Runtime.Serialization.Json;
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

        public ActionResult StartAnalyze(string fen, string depth, bool whiteToMove)
        {
            var moveTurn = whiteToMove ? " w " : " b";
            fen = fen + moveTurn;
            var guid = UciProxy.Start(fen, int.Parse(depth));
            return Content(guid.ToString());
        }

        public ActionResult GetOutput(string guid)
        {
            var output = UciProxy.GetProcessOutput(new Guid(guid));
            return Json(output, JsonRequestBehavior.AllowGet);
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