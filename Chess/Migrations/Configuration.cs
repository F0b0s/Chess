namespace Chess.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<UCIProxy.DAL.PositionAnalysisContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "UCIProxy.DAL.PositionAnalysisContext";
        }
    }
}