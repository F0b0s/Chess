using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace UCIProxy.DAL
{
    public class AnalysisRepository
    {
        public IEnumerable<Engine> GetEngines()
        {
            using (var context = new PositionAnalysisContext())
            {
                return context.Engines;
            }
        }

        public long CreateOrGetAnalysis(long engineId, string fen, int depth, int lines, string userId, out bool wasCreated)
        {
            using (var context = new PositionAnalysisContext())
            {
                var analysis = context.PositionAnalyses
                    .FirstOrDefault(x => 
                        x.Position.Fen == fen && 
                        x.Depth == depth &&
                        x.Lines.Count == lines &&
                        x.Engine.Id == engineId
                    );

                if (analysis != null)
                {
                    wasCreated = false;
                    if (!string.IsNullOrEmpty(userId) && context.UserPositionAnalyses.Count(x => x.PositionAnalysis.Id == analysis.Id && x.UserId == userId) < 1)
                    {
                        var userpositionAnalysis = new UserPositionAnalysis
                                                   {
                                                       UserId = userId,
                                                       PositionAnalysis = analysis
                                                   };
                        context.UserPositionAnalyses.Add(userpositionAnalysis);
                        context.SaveChanges();
                    }
                    
                    return analysis.Id;
                }

                var engine = context.Engines.Single(x => x.Id == engineId);
                var position = new Position
                               {
                                   Fen = fen
                               };
                var analisys = new PositionAnalysis
                               {
                                   Engine = engine,
                                   Position = position
                               };
                if (!string.IsNullOrEmpty(userId))
                {
                    var userAnalisys = new UserPositionAnalysis
                                       {
                                           UserId = userId,
                                           PositionAnalysis = analisys
                                       };
                    context.UserPositionAnalyses.Add(userAnalisys);
                }
                context.PositionAnalyses.Add(analisys);
                context.SaveChanges();

                wasCreated = true;
                return analisys.Id;
            }
        }

        public PositionAnalysis GetAnalysis(long analysisId)
        {
            using (var context = new PositionAnalysisContext())
            {
                return context.PositionAnalyses
                    .Include(x => x.Engine)
                    .Include(x => x.Lines)
                    .Single(x => x.Id == analysisId);
            }
        }

        public void SaveAnalisysLine(long analisysId, short lineNumber, LineInfo lineInfo, AnalysisStatistics analisysStatistics)
        {
            using (var context = new PositionAnalysisContext())
            {
                var analysis = context.PositionAnalyses
                    .Include(x => x.Lines)
                    .Single(x => x.Id == analisysId);

                var line = analysis.Lines.SingleOrDefault(x => x.LineNumber == lineNumber);
                if (line != null)
                {
                    line.Moves = lineInfo.Moves;
                    line.Score = lineInfo.Score;
                }
                else
                {
                    var analisysLine = new AnalysisLine
                                       {
                                           LineNumber = lineNumber,
                                           Moves = lineInfo.Moves,
                                           Score = lineInfo.Score
                                       };
                    analysis.Lines.Add(analisysLine);
                }
                
                analysis.Nodes = Convert.ToInt64(analisysStatistics.Nodes);
                analysis.Depth = Convert.ToInt16(analisysStatistics.Depth);
                analysis.Time = Convert.ToInt64(analisysStatistics.Time);

                context.SaveChanges();
            }
        }

        public void SetAnalisysStatus(long analysisId, AnalysisStatus status)
        {
            using (var context = new PositionAnalysisContext())
            {
                var analisys = context.PositionAnalyses.Single(x => x.Id == analysisId);
                analisys.Status = status;

                context.SaveChanges();
            }
        }
    }
}
