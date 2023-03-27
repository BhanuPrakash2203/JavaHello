using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Sommaire;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.Sommaire
{
    public class SectionSommaireBuilder : ISectionSommaireBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionSommaireBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<SectionSommaireViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionSommaire>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
    }
}