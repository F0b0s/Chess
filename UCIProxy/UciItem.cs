using System.Diagnostics;
using System.Threading.Tasks;

namespace UCIProxy
{
    public class UciItem
    {
        public UciItem(int multipv)
        {
            Infos = new PositionAnalysis
                    {
                        Lines = new LineInfo[multipv]
                    };
        }

        public Process Process { get; set; }

        public PositionAnalysis Infos { get; set; }

        public Task<Task> ReaderTask { get; set; }
    }
}