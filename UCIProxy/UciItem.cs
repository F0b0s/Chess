using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UCIProxy
{
    public class UciItem
    {
        public UciItem(int multipv)
        {
            Infos = new List<LineInfo>();

            for (int i = 0; i < multipv; i++)
            {
                Infos.Add(new LineInfo());
            }
        }

        public Process Process { get; set; }

        public List<LineInfo> Infos { get; set; }

        public Task ReaderTask { get; set; }
    }
}