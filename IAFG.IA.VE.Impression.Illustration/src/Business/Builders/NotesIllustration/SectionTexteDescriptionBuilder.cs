using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.NotesIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.NotesIllustration
{
    public class SectionTexteDescriptionBuilder : ISectionTexteDescriptionBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionListeDescriptionBuilder _sectionListeDescriptionBuilder;

        public SectionTexteDescriptionBuilder(IReportFactory reportFactory,
                                              ISectionListeDescriptionBuilder sectionListeDescriptionBuilder)
        {
            _reportFactory = reportFactory;
            _sectionListeDescriptionBuilder = sectionListeDescriptionBuilder;
        }

        public void Build(BuildParameters<DetailNotesIllustrationViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionTexteDescription>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters, vm => BuildSubparts(report, parameters.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(ISectionTexteDescription report, DetailNotesIllustrationViewModel model, IReportContext reportContext, IStyleOverride styleOverride)
        {

            if (model.ListeDescriptions.Any())
            {
                SectionListeViewModel sectionListeViewModel = new SectionListeViewModel() { ListeDescriptions = model.ListeDescriptions };
                _sectionListeDescriptionBuilder.Build(new BuildParameters<SectionListeViewModel>(sectionListeViewModel)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }
            
        }
    }
}