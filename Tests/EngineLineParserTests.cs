using NUnit.Framework;
using UCIProxy;

namespace Tests
{
    [TestFixture]
    public class EngineLineParserTests
    {
        [Test]
        public void ParseTest()
        {
            var engineLine = "info depth 5 seldepth 5 score cp 54 nodes 3014 nps 602800 time 5 multipv 1 pv g1f3 d7d5 d2d4 g8f6 b1c3";
            var parser = new EngineLineParser();
            var lineInfo = parser.GetLineInfo(engineLine);

            Assert.AreEqual("5", lineInfo.Depth);
            Assert.AreEqual("54", lineInfo.Score);
            Assert.AreEqual("3014", lineInfo.Nodes);
            Assert.AreEqual("5", lineInfo.Time);
            Assert.AreEqual("g1f3 d7d5 d2d4 g8f6 b1c3", lineInfo.Moves);
        }

        [Test]
        public void WhenEndLineShouldReturnTrue()
        {
            var endLine = "info nodes 32963 time 30";
            var parser = new EngineLineParser();
            Assert.True(parser.IsEndLIne(endLine));
        }

        [Test]
        public void WhenINfoLineShouldReturnFalse()
        {
            var infoLine = "info depth 5 seldepth 5 score cp 54 nodes 3014 nps 602800 time 5 multipv 1 pv g1f3 d7d5 d2d4 g8f6 b1c3";
            var parser = new EngineLineParser();
            Assert.False(parser.IsEndLIne(infoLine));
        }

        [Test]
        public void WhenIntermediateResultItShouldPassIt()
        {
            var engineLine = "info depth 24 currmove d2d3 currmovenumber 6";
            var parser = new EngineLineParser();
            Assert.True(parser.IsIntermediateLine(engineLine));
        }


        [Test]
        public void WhenInfoLIneItShouldFalse()
        {
            var engineLine = "info depth 5 seldepth 5 score cp 54 nodes 3014 nps 602800 time 5 multipv 1 pv g1f3 d7d5 d2d4 g8f6 b1c3";
            var parser = new EngineLineParser();
            Assert.False(parser.IsIntermediateLine(engineLine));
        }
    }
}
