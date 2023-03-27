using IAFG.IA.VE.Impression.Core.Types.Styles;

namespace IAFG.IA.VE.Impression.Core.Types.Reports
{
    public interface IReport
    {
        void AddSubReport(IReport subReport);

        void DisposeReport();
        IStyleOverride StyleOverride { get; set; }
    }

    public interface IReportWithModel<in TViewModel> : ISubReport
    {
        TViewModel Model { set; }
    }
}
