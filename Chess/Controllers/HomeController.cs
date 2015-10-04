using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Chess.Filters;
using UCIProxy;
using UCIProxy.DAL;
using AnalysisStatus = UCIProxy.AnalysisStatus;
using PositionAnalysis = UCIProxy.PositionAnalysis;

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
            var analisys = AnalysisRepository.GetAnalysis(analysisId);
            var output = new PositionAnalysisContainer
                         {
                             PositionAnalysis = new PositionAnalysis
                                                {
                                                    Lines = analisys.Lines.Select(x => new LineInfo
                                                                                       {
                                                                                           Moves = x.Moves,
                                                                                           Score = x.Score
                                                                                       }).ToArray(),
                                                    EngneInfo = analisys.Engine.Name,
                                                    AnalysisStatistics = new AnalysisStatistics
                                                                         {
                                                                             Depth = analisys.Depth.ToString(CultureInfo.InvariantCulture),
                                                                             Nodes = analisys.Nodes.ToString(CultureInfo.InvariantCulture),
                                                                             Time = analisys.Time.ToString(CultureInfo.InvariantCulture)
                                                                         }
                                                },
                             AnalysisStatus = (AnalysisStatus) analisys.Status
                         };

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