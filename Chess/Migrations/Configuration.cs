using UCIProxy.DAL;

namespace Chess.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<PositionAnalysisContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "UCIProxy.DAL.PositionAnalysisContext";
        }

        protected override void Seed(PositionAnalysisContext context)
        {
            var engine = new Engine
                           {
                               Name = "Stockfish",
                               Path = @"C:\Program Files\Chess\stockfish-6-64.exe"
                           };
            context.Engines.AddOrUpdate(engine);
        }
    }
}