using System;
using System.Threading;
using NUnit.Framework;
using UCIProxy;

namespace Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            var uciProxy = new UciProxy();
            var guid = uciProxy.Start("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 10, 2);
            Thread.Sleep(3000);
            Console.WriteLine(uciProxy.GetProcessOutput(guid));
        }
    }
}
