using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral.Sommaire;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral.Sommaire
{
    public class SectionContratBuilder : ISectionContratBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionContratBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<SectionContratViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionContrat>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
    }
}