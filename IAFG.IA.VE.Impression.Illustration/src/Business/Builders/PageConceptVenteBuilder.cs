using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageConceptVenteBuilder : IPageConceptVenteBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageConceptVenteMapper _mapper;
        private readonly ISectionSommaireBuilder _sectionSommaireBuilder;

        public PageConceptVenteBuilder(IReportFactory reportFactory, IPageConceptVenteMapper mapper, ISectionSommaireBuilder sectionSommaireBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionSommaireBuilder = sectionSommaireBuilder;
        }

        public void Build(BuildParameters<SectionConceptVenteModel> parameters)
        {
            var pageViewModel = new PageSommaireViewModel();
            _mapper.Map(parameters.Data, pageViewModel, parameters.ReportContext);

            var param = new BuildParameters<PageSommaireViewModel>(pageViewModel)
                        {
                            ParentReport = parameters.ParentReport,
                            ReportContext = parameters.ReportContext,
                            StyleOverride = parameters.StyleOverride
                        };

            var report = _reportFactory.Create<IPageSommaire>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param, vm => BuildSubParts(report, param.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubParts(IPageSommaire report, PageSommaireViewModel paramData, IReportContext reportContext, IStyleOverride styleOverride)
        {
            foreach (var section in paramData.Sections)
            {
                _sectionSommaireBuilder.Build(new BuildParameters<SectionSommaireViewModel>(section)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }
        }
    }
}