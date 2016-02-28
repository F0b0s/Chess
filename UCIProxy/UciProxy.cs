using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UCIProxy.Common;
using UCIProxy.DAL;

namespace UCIProxy
{
    public class UciProxy
    {
        private readonly AnalysisRepository _analysisRepository;
        private readonly int _maxAnalisysyDepth;
        private readonly int _maxOutputLines;

        public UciProxy(AnalysisRepository analysisRepository, int maxAnalisysyDepth, int maxOutputLines)
        {
            _analysisRepository = analysisRepository;
            _maxAnalisysyDepth = maxAnalisysyDepth;
            _maxOutputLines = maxOutputLines;
        }

        public void Start(string fen, int depth, int multiPv, long engineId, long analysisId)
        {
            if (string.IsNullOrEmpty(fen))
            {
                var message = $"FEN string can't be null or empty, current value is '{fen}'";
                throw new ArgumentException(message, nameof(fen));
            }

            if (depth <= 0 || depth > _maxAnalisysyDepth)
            {
                var message = $"Analysis depth should be between {1} and {_maxAnalisysyDepth}, current value is '{depth}'";
                throw new ArgumentException(message, nameof(depth));
            }

            if (multiPv <= 0 || multiPv > _maxOutputLines)
            {
                var message = $"Analysis output lines count should be between {1} and {_maxOutputLines}, current value is '{multiPv}'";
                throw new ArgumentException(message, nameof(multiPv));
            }
            
            Task.Factory.StartNew(async () => await AnalysePosition(fen, depth, multiPv, analysisId, engineId))
                        .ContinueWith(task => LogHelper.LogError(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task AnalysePosition(string fen, int depth, int multiPv, long analysisId, long engineId)
        {
            try
            {
                var engine = _analysisRepository.GetEngines().Single(x => x.Id == engineId);
                var process = StartAnalysisProcess(engine);
                PrepareEngine(fen, depth, multiPv, process);
                await ReadAnalysis(process.StandardOutput, analysisId);
                CloseAnalysisProcess(process);
            }
            catch (ChessException ce)
            {
                LogHelper.LogError(ce);
                _analysisRepository.SetAnalisysStatus(analysisId, DAL.AnalysisStatus.Faulted);
            }
        }

        private Process StartAnalysisProcess(Engine engine)
        {
            var startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = false,
                                RedirectStandardInput = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                FileName = engine.Path
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

        private async Task ReadAnalysis(TextReader input, long analysisId)
        {
            while (true)
            {
                var line =  await ReadLineAsync(input);
                if (line == null)
                {
                    Debug.WriteLine("Unexpected process termination");
                    throw new ChessException("Unexpected process termination");
                }

                if (EngineLineParser.IsLastLine(line))
                {
                    _analysisRepository.SetAnalisysStatus(analysisId, DAL.AnalysisStatus.Completed);
                    break;
                }

                StoreLine(line, analysisId);
            }
        }

        private void StoreLine(string line, long analysisId)
        {
            if (EngineLineParser.IsIntermediateLine(line))
                return;

            var multiPv = EngineLineParser.GetMultiPv(line);
            var lineInfo = EngineLineParser.GetLineInfo(line);
            var analysisStatistics = EngineLineParser.GetAnalysisStatistic(line);

            _analysisRepository.SaveAnalisysLine(analysisId, multiPv, lineInfo, analysisStatistics);
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
