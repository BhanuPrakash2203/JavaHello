using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using ISectionFluxMonetaire = IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration.ISectionFluxMonetaire;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration
{
    public class SectionFluxMonetaireBuilder : ISectionFluxMonetaireBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionFluxMonetaireBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }
        public void Build(BuildParameters<SectionFluxMonetaireViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionFluxMonetaire>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
    }
}
