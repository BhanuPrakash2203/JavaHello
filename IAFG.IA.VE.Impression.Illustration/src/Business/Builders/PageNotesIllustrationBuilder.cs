using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.NotesIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageNotesIllustrationBuilder : IPageNotesIllustrationBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageNotesIllustrationMapper _mapper;

        private readonly ISectionTexteDescriptionBuilder _sectionTexteDescriptionBuilder;

        public PageNotesIllustrationBuilder(IReportFactory reportFactory, IPageNotesIllustrationMapper mapper,
                                            ISectionTexteDescriptionBuilder sectionTexteDescriptionBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionTexteDescriptionBuilder = sectionTexteDescriptionBuilder;
        }

        public void Build(BuildParameters<SectionNotesIllustrationModel> parameters)
        {
            IPageNotesIllustration report = _reportFactory.Create<IPageNotesIllustration>();
            PageNotesIllustrationViewModel viewModel = new PageNotesIllustrationViewModel();
            _mapper.Map(parameters.Data, viewModel, parameters.ReportContext);

            ReportBuilderAssembler.AssembleWithoutModelMapping(report, viewModel, parameters, vm => BuildSubparts(vm, report, parameters.ReportContext));
        }

        private void BuildSubparts(PageNotesIllustrationViewModel viewModel, IReport report, IReportContext reportContext)
        {
            foreach (DetailNotesIllustrationViewModel notesViewModel in viewModel.SousSections)
            {
                if (notesViewModel.ListeDescriptions !=null && notesViewModel.ListeDescriptions.Any())
                {
                    _sectionTexteDescriptionBuilder.Build(new BuildParameters<DetailNotesIllustrationViewModel>(notesViewModel)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = report.StyleOverride
                    });
                }
            }
        }

    }
}