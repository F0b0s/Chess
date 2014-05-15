using System.Diagnostics;
using System.Threading.Tasks;

namespace UCIProxy
{
    public class UciItem
    {
        public Process Process { get; set; }

        public LineInfo Info { get; set; }

        public Task ReaderTask { get; set; }
    }
}