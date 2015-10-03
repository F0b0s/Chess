using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace UCIProxy.DAL
{
    public class Engine
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }
    }

    public class Position
    {
        public long Id { get; set; }

        public string Fen { get; set; }
    }

    public class AnalisysLine
    {
        public long Id { get; set; }

        public short LineNumber { get; set; }

        public string Moves { get; set; }

        public string Score { get; set; }
    }

    public class PositionAnalysis
    {
        public long Id { get; set; }

        public long Time { get; set; }

        public long Nodes { get; set; }

        public short Depth { get; set; }

        public bool Completed { get; set; }

        public virtual List<AnalisysLine> Lines { get; set; }

        public virtual Engine Engine { get; set; }

        public virtual Position Position { get; set; }
    }

    public class PositionAnalysisContext : DbContext
    {
        public PositionAnalysisContext() : base("Chess")
        {
            
        }

        public DbSet<Engine> Engines { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionAnalysis> PositionAnalyses { get; set; }
        public DbSet<AnalisysLine> AnalisysLines { get; set; }
    }
}
