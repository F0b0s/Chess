using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UCIProxy.DAL;

namespace UCIProxy
{
    public class UciProxy
    {
        private readonly int _maxAnalisysyDepth;
        private readonly int _maxOutputLines;

        public UciProxy()
        {
            _maxAnalisysyDepth = Int32.Parse(ConfigurationManager.AppSettings["MaxAnalysisDepth"]);
            _maxOutputLines = Int32.Parse(ConfigurationManager.AppSettings["MaxOutputLines"]);
        }

        public long Start(string fen, int depth, int multiPv, long engineId)
        {
            if (string.IsNullOrEmpty(fen))
            {
                var message = string.Format("FEN string can't be null or empty, current value is '{0}'", fen);
                throw new ArgumentException(message, "fen");
            }

            if (depth <= 0 || depth > _maxAnalisysyDepth)
            {
                var message = string.Format("Analysis depth should be between {0} and {1}, current value is '{2}'", 1, _maxAnalisysyDepth, depth);
                throw new ArgumentException(message, "depth");
            }

            if (multiPv <= 0 || multiPv > _maxOutputLines)
            {
                var message = string.Format("Analysis output lines count should be between {0} and {1}, current value is '{2}'", 1, _maxOutputLines, multiPv);
                throw new ArgumentException(message, "multiPv");
            }

            long analisysId;
            if (AnalisysRepository.TryGetAnalysis(engineId, fen, depth, multiPv, out analisysId))
            {
                return analisysId;
            }

            var startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = false,
                                RedirectStandardInput = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                FileName = ConfigurationManager.AppSettings["EnginePath"]
                            };

            var process = Process.Start(startInfo);
            var engneInfo = GetEngineInfo(process);
            PrepareEngine(fen, depth, multiPv, process);

            analisysId = AnalisysRepository.CreateAnalisys(engineId, fen);

            Task.Factory.StartNew(async () =>
                                        {
                                            await ReadLineAsync(process.StandardOutput, analisysId);
                                            if (process != null)
                                                process.Kill();
                                        });
            return analisysId;
        }

        private void PrepareEngine(string fen, int depth, int multiPv, Process process)
        {
            ClearProcessOutput(process);
            process.StandardInput.WriteLine("isready");
            var isReady = process.StandardOutput.ReadLine();
            if (isReady != "readyok")
            {
                throw new InvalidOperationException("Engine is not ready.");
            }

            process.StandardInput.WriteLine("setoption name Multipv value " + multiPv);
            process.StandardInput.WriteLine("position fen {0}", fen);
            process.StandardInput.WriteLine("go depth {0}", depth);
        }

        public PositionAnalysisContainer GetProcessOutput(long analisysId)
        {
            var analisys = AnalisysRepository.GetAnalysis(analisysId);

            return new PositionAnalysisContainer
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
                                        Completed = analisys.Completed
                                    };
        }

        private async Task ReadLineAsync(StreamReader input, long analysisId)
        {
            var parser = new EngineLineParser();
            string line = "";

            while (line != null)
            {
                line = await input.ReadLineAsync();

                Debug.WriteLine(line);

                if (parser.IsIntermediateLine(line))
                    continue;

                if (parser.IsEndLIne(line))
                    break;

                var multiPv = parser.GetMultiPv(line);
                var lineInfo = parser.GetLineInfo(line);
                var analysisStatistics = parser.GetAnalysisStatistic(line);

                AnalisysRepository.SaveAnalisysLine(analysisId, multiPv, lineInfo, analysisStatistics);
            }

            AnalisysRepository.MarkAnalisysAsCompleted(analysisId);
        }

        private string GetEngineInfo(Process process)
        {
            ClearProcessOutput(process);
            process.StandardInput.WriteLine("uci");
            string engineName;
            var parser = new EngineInfoParser();

            while (true)
            {
                string line = process.StandardOutput.ReadLine();

                if (parser.TryGetEngineName(line, out engineName) || line == "uciok")
                    break;
            }

            return engineName;
        }

        private void ClearProcessOutput(Process process)
        {
            while (process.StandardOutput.Peek() != -1)
            {
                var line = process.StandardOutput.ReadLine();
                Debug.WriteLine("Discarded line: {0}", new object[]{line});
            }
        }
    }
}
