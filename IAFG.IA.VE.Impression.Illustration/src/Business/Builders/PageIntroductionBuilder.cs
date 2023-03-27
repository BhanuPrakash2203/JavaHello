using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageIntroductionBuilder : IPageIntroductionBuilder
    {
        private readonly IReportFactory _reportFactory;

        public PageIntroductionBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<DonneesRapportIllustration> parameters)
        {
            var report = _reportFactory.Create<IPageIntroduction>();
            ReportBuilderAssembler.AssembleWithoutData(report, parameters);
        }
    }
}