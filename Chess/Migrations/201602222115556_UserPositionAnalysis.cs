namespace Chess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPositionAnalysis : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPositionAnalysis",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.String(),
                        PositionAnalysis_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PositionAnalysis", t => t.PositionAnalysis_Id)
                .Index(t => t.PositionAnalysis_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPositionAnalysis", "PositionAnalysis_Id", "dbo.PositionAnalysis");
            DropIndex("dbo.UserPositionAnalysis", new[] { "PositionAnalysis_Id" });
            DropTable("dbo.UserPositionAnalysis");
        }
    }
}
