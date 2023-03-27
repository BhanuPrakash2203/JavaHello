using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral.Sommaire;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral.Sommaire
{
    public class SectionImpositionBuilder : ISectionImpositionBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionImpositionBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<SectionImpositionViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionImposition>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
    }
}