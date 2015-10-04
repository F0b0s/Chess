using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCIProxy
{
    [DataContract]
    public enum AnalysisStatus
    {
        InProcess,
        Completed,
        Faulted
    }

    [DataContract]
    public class PositionAnalysisContainer
    {
        [DataMember]
        public AnalysisStatus AnalysisStatus { get; set; }

        [DataMember]
        public PositionAnalysis PositionAnalysis { get; set; }
    }

    [DataContract]
    public class PositionAnalysis
    {
        [DataMember]
        public string EngneInfo { get; set; }

        [DataMember]
        public AnalysisStatistics AnalysisStatistics { get; set; }

        [DataMember]
        public LineInfo[] Lines { get; set; }
    }

    [DataContract]
    public class AnalysisStatistics
    {
        [DataMember]
        public string Depth { get; set; }

        [DataMember]
        public string Nodes { get; set; }

        [DataMember]
        public string Time { get; set; }
    }    

    [DataContract]
    public class LineInfo
    {
        [DataMember]
        public string Score { get; set; }
        
        [DataMember]
        public string Moves { get; set; }
    }
}