using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UCIProxy.Common;
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

            // combine operations
            long analysisId;
            if (AnalysisRepository.TryGetAnalysis(engineId, fen, depth, multiPv, out analysisId))
            {
                return analysisId;
            }
            analysisId = AnalysisRepository.CreateAnalysis(engineId, fen);

            Task.Factory.StartNew(() => AnalysePosition(fen, depth, multiPv, analysisId))
                        .ContinueWith(task => LogHelper.LogError(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
            
            return analysisId;
        }

        private void AnalysePosition(string fen, int depth, int multiPv, long analysisId)
        {
            try
            {
                var process = StartAnalysisProcess();
                PrepareEngine(fen, depth, multiPv, process);
                ReadAnalysis(process.StandardOutput, analysisId)
                    .ContinueWith(task => CloseAnalysisProcess(process));
            }
            catch (ChessException ce)
            {
                LogHelper.LogError(ce);
                AnalysisRepository.MarkAnalisysAsFailed();
            }
        }

        private static Process StartAnalysisProcess()
        {
            var startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = false,
                                RedirectStandardInput = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                FileName = ConfigurationManager.AppSettings["EnginePath"]
                            };

            try
            {
                return Process.Start(startInfo);
            }
            catch (InvalidOperationException ioe)
            {
                throw new ChessException("Unable to start analysis process", ioe);
            }
            catch (FileNotFoundException fnfe)
            {
                throw new ChessException("Unable to start analysis process", fnfe);
            }
            catch (Win32Exception w32e)
            {
                throw new ChessException("Unable to start analysis process", w32e);
            }
        }

        private void CloseAnalysisProcess(Process analysisProcess)
        {
            try
            {
                Debug.WriteLine("Kill process");
                analysisProcess.Kill();
            }
            catch (Win32Exception w32e)
            {
                LogHelper.LogError(w32e);
            }
            catch (InvalidOperationException ioe)
            {
                LogHelper.LogError(ioe);
            }
        }

        private void PrepareEngine(string fen, int depth, int multiPv, Process process)
        {
            ClearProcessOutput(process);
            process.StandardInput.WriteLine("isready");
            var isReady = process.StandardOutput.ReadLine();
            if (isReady != "readyok")
            {
                throw new ChessException("Engine is not ready.");
            }

            process.StandardInput.WriteLine("setoption name Multipv value " + multiPv);
            process.StandardInput.WriteLine("position fen {0}", fen);
            process.StandardInput.WriteLine("go depth {0}", depth);
        }

        private async static Task ReadAnalysis(TextReader input, long analysisId)
        {
            while (true)
            {
                var line =  await ReadLineAsync(input);
                if (line == null)
                {
                    Debug.WriteLine("Unexpected process termination");
                    throw new ChessException("Unable to start analysis process");
                }

                if (EngineLineParser.IsLastLine(line))
                {
                    AnalysisRepository.MarkAnalisysAsCompleted(analysisId);
                    break;
                }

                StoreLine(line, analysisId);
            }
        }

        private static void StoreLine(string line, long analysisId)
        {
            if (EngineLineParser.IsIntermediateLine(line))
                return;

            var multiPv = EngineLineParser.GetMultiPv(line);
            var lineInfo = EngineLineParser.GetLineInfo(line);
            var analysisStatistics = EngineLineParser.GetAnalysisStatistic(line);

            AnalysisRepository.SaveAnalisysLine(analysisId, multiPv, lineInfo, analysisStatistics);
        }

        private static async Task<string> ReadLineAsync(TextReader input)
        {
            try
            {
                var line = await input.ReadLineAsync().ConfigureAwait(false);
                Debug.WriteLine(line);
                return line;
            }
            catch (ArgumentOutOfRangeException aofre)
            {
                LogHelper.LogError(aofre);
                return null;
            }
            catch (ObjectDisposedException ode)
            {
                LogHelper.LogError(ode);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                LogHelper.LogError(ioe);
                return null;
            }
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
