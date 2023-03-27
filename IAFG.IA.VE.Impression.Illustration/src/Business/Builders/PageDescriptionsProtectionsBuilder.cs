using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageDescriptionsProtectionsBuilder : IPageDescriptionsProtectionsBuilder
    {

        private readonly IReportFactory _reportFactory;
        private readonly IPageDescriptionsProtectionsMapper _mapper;
        private readonly ISectionDescriptionBuilder _descriptionBuilder;

        public PageDescriptionsProtectionsBuilder(IReportFactory reportFactory, IPageDescriptionsProtectionsMapper mapper, 
                                                   ISectionDescriptionBuilder descriptionBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _descriptionBuilder = descriptionBuilder;
        }

        public void Build(BuildParameters<SectionDescriptionsProtectionsModel> parameters)
        {
            if (parameters.Data.Details == null || !parameters.Data.Details.Any())
            {
                return;
            }

            var descriptionsProtectionsViewModel = new PageDescriptionsProtectionsViewModel();
            _mapper.Map(parameters.Data, descriptionsProtectionsViewModel, parameters.ReportContext);

            var param = new BuildParameters<PageDescriptionsProtectionsViewModel>(descriptionsProtectionsViewModel)
            {
                ParentReport = parameters.ParentReport,
                ReportContext = parameters.ReportContext,
                StyleOverride = parameters.StyleOverride
            };

            var report = _reportFactory.Create<IPageDescriptionsProtections>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param,
                vm => BuildSubparts(report, param.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(IPageDescriptionsProtections report,
            PageDescriptionsProtectionsViewModel pageDescriptionsProtectionsViewModel,
            IReportContext reportContext,
            IStyleOverride styleOverride)
        {
            foreach (var detailDescriptionProtectionViewModel in pageDescriptionsProtectionsViewModel.Details
                .OrderBy(d => d.SequenceId).ThenBy(d => d.Libelle))
            {
                _descriptionBuilder.Build(
                    new BuildParameters<DescriptionViewModel>(detailDescriptionProtectionViewModel)
                    {
                        ReportContext = reportContext,
                        ParentReport = report,
                        StyleOverride = styleOverride
                    });
            }
        }
    }
}