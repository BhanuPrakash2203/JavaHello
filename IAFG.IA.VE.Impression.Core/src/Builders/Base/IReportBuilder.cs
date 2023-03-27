namespace IAFG.IA.VE.Impression.Core.Builders.Base
{
    public interface IReportBuilder<TBuildData>
    {
        void Build(BuildParameters<TBuildData> parameters);
    }
}