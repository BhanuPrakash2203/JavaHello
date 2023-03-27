using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Export;

namespace IAFG.IA.VE.Impression.Core.ReportContext
{
    public class ReportContext : IReportContext
    {
        public Language Language { get; set; }
        public ApplicationMode Mode { get; set; }
        public ReportAudienceTypes ReportAudience { get; set; }
        public SaleOrigin SaleOrigin { get; set; }
    }
}