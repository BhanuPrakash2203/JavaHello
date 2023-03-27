using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration
{
    public class SectionAssuresBuilder : ISectionAssuresBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionAssuresBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }
        public void Build(BuildParameters<SectionAssuresViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionAssures>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
   }
}