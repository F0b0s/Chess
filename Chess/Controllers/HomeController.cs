using System.Globalization;
using System.Web.Mvc;
using Chess.Filters;
using UCIProxy;

namespace Chess.Controllers
{
    public class HomeController : Controller
    {
        static readonly UciProxy UciProxy = new UciProxy();

        [ExceptionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [ExceptionFilter]
        public ActionResult StartAnalyze(string fen, string depth, int outputLines)
        {
            var analysisId = UciProxy.Start(fen, int.Parse(depth), outputLines, 1);
            return Content(analysisId.ToString(CultureInfo.InvariantCulture));
        }

        public ActionResult GetOutput(int analysisId)
        {
            var output = UciProxy.GetProcessOutput(analysisId);

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [ExceptionFilter]
        public ActionResult Error()
        {
            return View();
        }

        [ExceptionFilter]
        public ActionResult About()
        {
            return View();
        }
    }
}