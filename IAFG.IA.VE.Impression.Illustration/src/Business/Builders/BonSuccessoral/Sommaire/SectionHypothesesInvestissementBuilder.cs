using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral.Sommaire;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral.Sommaire
{
    public class SectionHypothesesInvestissementBuilder : ISectionHypothesesInvestissementBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionHypothesesInvestissementBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<SectionHypothesesInvestissementViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionHypothesesInvestissement>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
    }
}