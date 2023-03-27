using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral
{
    public class PageSommaireBonSuccessoralBuilder : IPageSommaireBonSuccessoralBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageSommaireBonSuccessoralMapper _mapper;
        private readonly ISectionContratBuilder _sectionContratBuilder;
        private readonly ISectionHypothesesInvestissementBuilder _sectionHypothesesInvestissementBuilder;
        private readonly ISectionImpositionBuilder _sectionImpositionBuilder;

        public PageSommaireBonSuccessoralBuilder(
            IReportFactory reportFactory,
            ISectionContratBuilder sectionContratBuilder,
            ISectionHypothesesInvestissementBuilder sectionHypothesesInvestissementBuilder,
            ISectionImpositionBuilder sectionImpositionBuilder,
            IPageSommaireBonSuccessoralMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionContratBuilder = sectionContratBuilder;
            _sectionHypothesesInvestissementBuilder = sectionHypothesesInvestissementBuilder;
            _sectionImpositionBuilder = sectionImpositionBuilder;
        }

        public void Build(BuildParameters<SommaireBonSuccessoralModel> parameters)
        {
            var viewModel = new PageSommaireBonSuccessoralViewModel();
            _mapper.Map(parameters.Data, viewModel, parameters.ReportContext);

            var param = new BuildParameters<PageSommaireBonSuccessoralViewModel>(viewModel)
            {
                ParentReport = parameters.ParentReport,
                ReportContext = parameters.ReportContext,
                StyleOverride = parameters.StyleOverride
            };

            var report = _reportFactory.Create<IPageSommaireBonSuccessoral>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param,
                vm => BuildSubparts(report, param.Data, parameters.ReportContext));
        }

        private void BuildSubparts(
            IPageSommaireBonSuccessoral report, 
            PageSommaireBonSuccessoralViewModel viewModel, 
            IReportContext reportContext)
        {
            _sectionContratBuilder.Build(new BuildParameters<SectionContratViewModel>(viewModel.SectionContrat)
            {
                ReportContext = reportContext,
                ParentReport = report
            });

            _sectionHypothesesInvestissementBuilder.Build(new BuildParameters<SectionHypothesesInvestissementViewModel>(viewModel.SectionHypothesesInvestissement)
            {
                ReportContext = reportContext,
                ParentReport = report
            });

            _sectionImpositionBuilder.Build(new BuildParameters<SectionImpositionViewModel>(viewModel.SectionImposition)
            {
                ReportContext = reportContext,
                ParentReport = report
            });
        }
    }
}
