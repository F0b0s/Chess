using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace UCIProxy.DAL
{
    public enum AnalysisStatus
    {
        InProcess,
        Completed,
        Faulted
    }

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

    public class AnalysisLine
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

        public AnalysisStatus Status { get; set; }

        public virtual List<AnalysisLine> Lines { get; set; }

        public virtual Engine Engine { get; set; }

        public virtual Position Position { get; set; }
    }

    public class UserPositionAnalysis
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public PositionAnalysis PositionAnalysis { get; set; }
    }

    public class PositionAnalysisContext : DbContext
    {
        public PositionAnalysisContext() : base("ChessDbContext")
        {
            
        }

        public DbSet<Engine> Engines { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionAnalysis> PositionAnalyses { get; set; }
        public DbSet<AnalysisLine> AnalysisLines { get; set; }
        public DbSet<UserPositionAnalysis> UserPositionAnalyses { get; set; }
    }
}