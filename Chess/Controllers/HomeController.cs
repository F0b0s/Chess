﻿using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Chess.Filters;
using Microsoft.AspNet.Identity;
using UCIProxy;
using UCIProxy.DAL;
using AnalysisStatus = UCIProxy.AnalysisStatus;
using PositionAnalysis = UCIProxy.PositionAnalysis;

namespace Chess.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private readonly AnalysisRepository _analysisRepository;
        private readonly UciProxy _uciProxy;

        public HomeController(AnalysisRepository analysisRepository, UciProxy uciProxy)
        {
            _analysisRepository = analysisRepository;
            _uciProxy = uciProxy;
        }

        [ExceptionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [ExceptionFilter]
        public ActionResult StartAnalyze(string fen, string depth, int outputLines, int engineId)
        {
            bool wasCreated;
            string userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetUserId();
            }
            var analysisId = _analysisRepository.CreateOrGetAnalysis(engineId, fen, int.Parse(depth), outputLines, userId, out wasCreated);
            if (wasCreated)
            {
                _uciProxy.Start(fen, int.Parse(depth), outputLines, 1, analysisId);
            }
            
            return Content(analysisId.ToString(CultureInfo.InvariantCulture));
        }

        public ActionResult GetOutput(int analysisId)
        {
            var analisys = _analysisRepository.GetAnalysis(analysisId);
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