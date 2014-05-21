using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace UCIProxy
{
    public class UciProxy
    {
        private static  object _sync = new object();
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
                                    FileName =
                                        @"C:\Users\юленька\Downloads\stockfish-dd-win\stockfish-dd-win\Windows\stockfish-dd-64-modern.exe"
                                };

                var process = Process.Start(startInfo);

                process.StandardOutput.ReadLine();
                process.StandardInput.WriteLine("isready");
                var isready = process.StandardOutput.ReadLine();
                if (isready != "readyok")
                {
                    throw new InvalidOperationException();
                }

                process.StandardInput.WriteLine("setoption name Multipv value " + multiPv);
                process.StandardInput.WriteLine("position fen {0}", fen);
                process.StandardInput.WriteLine("go depth {0} infinite", depth);

                var guid = Guid.NewGuid();
                var uciItem = new UciItem(multiPv)
                              {
                                  Process = process
                              };
                _processes.Add(guid.ToString(), uciItem);
                var task = Task.Factory.StartNew(() =>
                                                 {
                                                     ReadLineAsync(uciItem);
                                                 });
                uciItem.ReaderTask = task;

                return guid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public IEnumerable<LineInfo> GetProcessOutput(Guid guid)
        {
            UciItem uciItem;
            if (!_processes.TryGetValue(guid.ToString(), out uciItem))
            {
                throw new ArgumentException(string.Format("The process with '{0}' guid was not founded", guid), "guid");
            }

            return uciItem.Infos;
        }

        private async Task ReadLineAsync(UciItem uciItem)
        {
            var parser = new EngineLineParser();
            string line = "";

            while (line != null)
            {
                line = await uciItem.Process.StandardOutput.ReadLineAsync();
                if (line == null)
                    break;

                Debug.WriteLine(line);

                if(parser.IsIntermediateLine(line))
                    continue;

                if (parser.IsEndLIne(line))
                    break;

                var multiPv = parser.GetMultiPv(line);

                lock (_sync)
                {
                    uciItem.Infos[(int) (multiPv - 1)] = parser.GetLineInfo(line);
                }
            }
        }
    }
}
