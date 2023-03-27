using IAFG.IA.VE.Impression.Core.Types.Enums;

namespace IAFG.IA.VE.Impression.Core.Types.Reports.MasterReports
{
    public interface IMasterReport : IReport
    {
        byte[] ToPdf(IsoPdfVersion pdfVersion);

        byte[] ToPdf(IsoPdfVersion pdfVersion, IReport reportToAppend);

        byte[] ToPdf(IsoPdfVersion pdfVersion, IReport[] reportsToAppend);

        string GetVersion();
    }
    public interface IMasterReportWithModel<in TViewModel> : IReportWithModel<TViewModel>, IMasterReport
    {
    }
}
