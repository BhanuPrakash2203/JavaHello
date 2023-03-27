using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.NotesIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.NotesIllustration
{
    public class SectionListeDescriptionBuilder : ISectionListeDescriptionBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionListeDescriptionBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<SectionListeViewModel> parameters)
        {
            var param = new BuildParameters<SectionListeViewModel>(parameters.Data)
            {
                ReportContext = parameters.ReportContext,
                ParentReport = parameters.ParentReport,
                StyleOverride = parameters.StyleOverride
            };
            var report = _reportFactory.Create<ISectionListeDescription>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param); 
        }
    }
}