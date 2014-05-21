using System;
using System.Text.RegularExpressions;

namespace UCIProxy
{
    public class EngineLineParser
    {
        readonly Regex _depthRegex = new Regex("depth (\\d+)");
        readonly Regex _scoreRegex = new Regex("cp (\\d+)");
        readonly Regex _nodesRegex = new Regex("nodes (\\d+)");
        readonly Regex _timeRegex = new Regex("time (\\d+)");
        readonly Regex _multipvRegex = new Regex(" multipv (\\d+)");
        readonly Regex _movesRegex = new Regex(" pv ([\\s\\S]+)");
        readonly Regex _endLineRegex = new Regex("^info nodes \\d+ time \\d+$");
        readonly Regex _intermediateLineRegex = new Regex("currmove");

        public LineInfo GetLineInfo(string engineLine)
        {
            var lineInfo = new LineInfo();

            var match = _depthRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Depth = match.Groups[1].Value;
            }

            match = _scoreRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Score = match.Groups[1].Value;
            }

            match = _nodesRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Nodes = match.Groups[1].Value;
            }

            match = _timeRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Time = match.Groups[1].Value;
            }

            match = _movesRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Moves = match.Groups[1].Value;
            }

            return lineInfo;
        }

        public bool IsEndLIne(string engineLine)
        {
            return _endLineRegex.IsMatch(engineLine);
        }

        public bool IsIntermediateLine(string engineLine)
        {
            return _intermediateLineRegex.IsMatch(engineLine);
        }

        public uint GetMultiPv(string line)
        {
            var matc = _multipvRegex.Match(line);
            if (!matc.Success)
            {
                throw new ArgumentException(string.Format("Should contain multipv info '{0}'", line));
            }

            var multipvStr =  matc.Groups[1].Value;
            uint multipv;

            if (uint.TryParse(multipvStr, out multipv))
            {
                return multipv;
            }

            throw new ArgumentException(string.Format("Can't parse multipv info '{0}'", line));
        }
    }
}
