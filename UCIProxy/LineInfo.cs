using System.Runtime.Serialization;

namespace UCIProxy
{
    [DataContract]
    public class LineInfo
    {
        [DataMember]
        public string Depth { get; set; }

        [DataMember]
        public string Score { get; set; }

        [DataMember]
        public string Nodes { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string Moves { get; set; }
    }
}