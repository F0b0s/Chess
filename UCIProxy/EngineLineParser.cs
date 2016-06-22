using System;
using System.Text.RegularExpressions;

namespace UCIProxy
{
    static class EngineLineParser
    {
        static readonly Regex DepthRegex = new Regex("depth (\\d+)");
        static readonly Regex ScoreRegex = new Regex("cp ([-]?\\d+)");
        static readonly Regex NodesRegex = new Regex("nodes (\\d+)");
        static readonly Regex TimeRegex = new Regex("time (\\d+)");
        static readonly Regex MultipvRegex = new Regex(" multipv (\\d+)");
        static readonly Regex MovesRegex = new Regex(" pv ([\\s\\S]+)");
        static readonly Regex EndLineRegex = new Regex("^bestmove");
        static readonly Regex IntermediateLineRegex = new Regex("currmove");

        public static AnalysisStatistics GetAnalysisStatistic(string engineLine)
        {
            var analysisStatistics = new AnalysisStatistics();
            var match = DepthRegex.Match(engineLine);
            if (match.Success)
            {
                analysisStatistics.Depth = match.Groups[1].Value;
            }

            match = NodesRegex.Match(engineLine);
            if (match.Success)
            {
                analysisStatistics.Nodes = match.Groups[1].Value;
            }

            match = TimeRegex.Match(engineLine);
            if (match.Success)
            {
                analysisStatistics.Time = match.Groups[1].Value;
            }

            return analysisStatistics;
        }

        public static LineInfo GetLineInfo(string engineLine)
        {
            var lineInfo = new LineInfo();

            var match = ScoreRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Score = match.Groups[1].Value;
            }

            match = MovesRegex.Match(engineLine);
            if (match.Success)
            {
                lineInfo.Moves = match.Groups[1].Value;
            }

            return lineInfo;
        }

        public static bool IsLastLine(string engineLine)
        {
            return EndLineRegex.IsMatch(engineLine);
        }

        public static bool IsIntermediateLine(string engineLine)
        {
            return IntermediateLineRegex.IsMatch(engineLine);
        }

        public static short GetMultiPv(string line)
        {
            var matc = MultipvRegex.Match(line);
            if (!matc.Success)
            {
                throw new ArgumentException($"Should contain multipv info '{line}'");
            }

            var multipvStr =  matc.Groups[1].Value;
            short multipv;

            if (short.TryParse(multipvStr, out multipv))
            {
                return multipv;
            }

            throw new ArgumentException($"Can't parse multipv info '{line}'");
        }
    }
}