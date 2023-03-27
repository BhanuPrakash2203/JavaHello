using System.Collections.Generic;

namespace IAFG.IA.VE.Impression.ComparaisonRapports.Traces
{
    public class PageTrace
    {
        public int PageIndex { get; set; }
        public string Pagination { get; set; }
        public string Title { get; set; }
        public List<string> Texts { get; set; }
        public List<string> Unmatched { get; set; }
    }
}