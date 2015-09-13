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

        public Guid Start(string fen, int depth, int multiPv)
        {
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

                process.StandardOutput.ReadLine();
                process.StandardInput.WriteLine("isready");
                var isReady = process.StandardOutput.ReadLine();
                if (isReady != "readyok")
                {
                    throw new InvalidOperationException("Engine is not ready.");
                }

                process.StandardInput.WriteLine("setoption name Multipv value " + multiPv);
                process.StandardInput.WriteLine("position fen {0}", fen);
                process.StandardInput.WriteLine("go depth {0}", depth);

                var guid = Guid.NewGuid();
                var uciItem = new UciItem(multiPv)
                              {
                                  Process = process
                              };
                _processes.Add(guid.ToString(), uciItem);
                var task = Task.Factory.StartNew(async () => await ReadLineAsync(uciItem));
                
                uciItem.ReaderTask = task;

                return guid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
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
    }
}
