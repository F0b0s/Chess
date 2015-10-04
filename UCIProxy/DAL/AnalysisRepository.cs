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

        public bool TryGetAnalysis(long engineId, string fen, int depth, int lines, out long analisysId)
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
                    analisysId = analysis.Id;
                    return true;
                }
                else
                {
                    analisysId = 0;
                    return false;
                }
            }
        }

        public long CreateAnalysis(long engineId, string fen)
        {
            using (var context = new PositionAnalysisContext())
            {
                var engine = context.Engines.Single(x => x.Id == engineId);
                var position = new Position
                {
                    Fen = fen
                };
                var analisys = new PositionAnalysis
                {
                    Engine = engine,
                    Position = position,

                };
                context.PositionAnalyses.Add(analisys);
                context.SaveChanges();

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
