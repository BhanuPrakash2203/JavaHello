using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Traces
{
    public class DocumentTrace
    {
        public string Document { get; set; }
        public string FileName { get; set; }
        public string ProductTrace { get; set; }
        public List<PageTrace> Pages { get; set; }
        public List<PageTrace> Unmatched { get; set; }
    }
}