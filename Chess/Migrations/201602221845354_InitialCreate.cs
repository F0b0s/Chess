namespace Chess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnalysisLines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LineNumber = c.Short(nullable: false),
                        Moves = c.String(),
                        Score = c.String(),
                        PositionAnalysis_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PositionAnalysis", t => t.PositionAnalysis_Id)
                .Index(t => t.PositionAnalysis_Id);
            
            CreateTable(
                "dbo.Engines",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PositionAnalysis",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Time = c.Long(nullable: false),
                        Nodes = c.Long(nullable: false),
                        Depth = c.Short(nullable: false),
                        Status = c.Int(nullable: false),
                        Engine_Id = c.Long(),
                        Position_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Engines", t => t.Engine_Id)
                .ForeignKey("dbo.Positions", t => t.Position_Id)
                .Index(t => t.Engine_Id)
                .Index(t => t.Position_Id);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Fen = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PositionAnalysis", "Position_Id", "dbo.Positions");
            DropForeignKey("dbo.AnalysisLines", "PositionAnalysis_Id", "dbo.PositionAnalysis");
            DropForeignKey("dbo.PositionAnalysis", "Engine_Id", "dbo.Engines");
            DropIndex("dbo.PositionAnalysis", new[] { "Position_Id" });
            DropIndex("dbo.PositionAnalysis", new[] { "Engine_Id" });
            DropIndex("dbo.AnalysisLines", new[] { "PositionAnalysis_Id" });
            DropTable("dbo.Positions");
            DropTable("dbo.PositionAnalysis");
            DropTable("dbo.Engines");
            DropTable("dbo.AnalysisLines");
        }
    }
}
