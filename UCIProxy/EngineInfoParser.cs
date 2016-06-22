using System.Text.RegularExpressions;

namespace UCIProxy
{
    class EngineInfoParser
    {
        readonly Regex _engineInfoRegex = new Regex("id name ([\\s\\S]+)");

        public bool TryGetEngineName(string line, out string name)
        {
            name = null;

            var match = _engineInfoRegex.Match(line);
            if (match.Success)
            {
                name = match.Groups[1].Value;
                return true;
            }

            return false;
        }
    }
}