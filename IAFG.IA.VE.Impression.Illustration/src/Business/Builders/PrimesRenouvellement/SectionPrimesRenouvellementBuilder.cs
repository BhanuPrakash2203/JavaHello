using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.PrimesRenouvellement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.PrimesRenouvellement
{
    public class SectionPrimesRenouvellementBuilder : ISectionPrimesRenouvellementBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionPrimesRenouvellementBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }
        
        public void Build(BuildParameters<DetailsPrimeRenouvellementViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionPrimesRenouvellement>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
        
    }
}