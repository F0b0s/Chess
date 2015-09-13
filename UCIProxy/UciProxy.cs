using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UCIProxy
{
    public class UciProxy
    {
        private static readonly object _sync = new object();
        readonly Dictionary<string, UciItem> _processes = new Dictionary<string, UciItem>();
        private int _maxAnalisysyDepth;
        private int _maxOutputLines;

        public UciProxy()
        {
            _maxAnalisysyDepth = Int32.Parse(ConfigurationManager.AppSettings["MaxAnalysisDepth"]);
            _maxOutputLines = Int32.Parse(ConfigurationManager.AppSettings["MaxOutputLines"]);
        }

        public Guid Start(string fen, int depth, int multiPv)
        {
            if (depth > _maxAnalisysyDepth || multiPv > _maxOutputLines)
            {
                throw new ArgumentException("Max thresold overflow");
            }

            try
            {
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

                var guid = Guid.NewGuid();
                var uciItem = new UciItem(multiPv)
                              {
                                  Process = process
                              };
                uciItem.Infos.EngneInfo = engneInfo;
                _processes.Add(guid.ToString(), uciItem);
                var task = Task.Factory.StartNew(async () =>
                                                       {
                                                           await ReadLineAsync(uciItem);
                                                           if (process != null) process.Close();
                                                       });
                
                uciItem.ReaderTask = task;

                return guid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
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

        public PositionAnalysisContainer GetProcessOutput(Guid guid)
        {
            UciItem uciItem;
            if (!_processes.TryGetValue(guid.ToString(), out uciItem))
            {
                throw new ArgumentException(string.Format("The process with '{0}' guid was not found", guid), "guid");
            }

            return new PositionAnalysisContainer
                                    {
                                        PositionAnalysis = uciItem.Infos,
                                        Completed = uciItem.ReaderTask.Result.IsCompleted
                                    };
        }

        private async Task ReadLineAsync(UciItem uciItem)
        {
            var parser = new EngineLineParser();
            string line = "";

            while (line != null)
            {
                line = await uciItem.Process.StandardOutput.ReadLineAsync();

                Debug.WriteLine(line);

                if (parser.IsIntermediateLine(line))
                    continue;

                if (parser.IsEndLIne(line))
                    break;

                var multiPv = parser.GetMultiPv(line);
                var lineInfo = parser.GetLineInfo(line);
                var analysisStatistics = parser.GetAnalysisStatistic(line);
                
                lock (_sync)
                {
                    uciItem.Infos.Lines[(int) (multiPv - 1)] = lineInfo;
                    uciItem.Infos.AnalysisStatistics = analysisStatistics;
                }
            }
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
                Debug.WriteLine("Discarded line: {0}", line);
            }
        }
    }
}
