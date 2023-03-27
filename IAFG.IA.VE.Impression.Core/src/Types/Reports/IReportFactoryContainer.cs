namespace IAFG.IA.VE.Impression.Core.Types.Reports
{
    public interface IReportFactoryContainer
    {
        TReport Create<TReport>() where TReport : IReport;
    }
}