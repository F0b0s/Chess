using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace UCIProxy
{
    public class UciItem
    {
        public UciItem()
        {
            Lines = new ConcurrentQueue<string>();
        }

        public Process Process { get; set; }
        public ConcurrentQueue<string> Lines { get; set; }
        public Task ReaderTask { get; set; }
    }

    public class UciProxy
    {
        readonly Dictionary<string, UciItem> _processes = new Dictionary<string, UciItem>();

        public Guid Start(string fen, int depth)
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

                process.StandardInput.WriteLine("isready");
                process.StandardInput.WriteLine("position fen {0}", fen);
                process.StandardInput.WriteLine("go depth {0} infinite", depth);

                var guid = Guid.NewGuid();
                var uciItem = new UciItem()
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

        public string GetProcessOutput(Guid guid)
        {
            UciItem uciItem;
            if (!_processes.TryGetValue(guid.ToString(), out uciItem))
            {
                throw new ArgumentException(string.Format("The process with '{0}' guid was not founded", guid), "guid");
            }

            var result = new StringBuilder();
            string line;
            while (uciItem.Lines.TryDequeue(out line))
            {
                result.AppendLine(line);
            }

            return result.ToString();
        }

        private async Task ReadLineAsync(UciItem uciItem)
        {
            string line = "";

            while (line != null)
            {
                line = await uciItem.Process.StandardOutput.ReadLineAsync();
                uciItem.Lines.Enqueue(line);
            }
        }
    }
}
