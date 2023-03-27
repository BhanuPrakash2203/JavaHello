using IAFG.IA.VE.Impression.Core.Types.Enums;

namespace IAFG.IA.VE.Impression.Core.Types.Export
{
    public class ExportOptions
    {
        public ReportAudienceTypes Audience { get; set; }
        public IsoPdfVersion PdfVersion { get; set; }
    }
}
