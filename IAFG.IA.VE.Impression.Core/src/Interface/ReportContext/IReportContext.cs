using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Export;

namespace IAFG.IA.VE.Impression.Core.Interface.ReportContext
{
    public interface IReportContext
    {
        ReportAudienceTypes ReportAudience { get; set; }
        Language Language { get; set; }
        ApplicationMode Mode { get; set; }
        SaleOrigin SaleOrigin { get; set; }
    }

}