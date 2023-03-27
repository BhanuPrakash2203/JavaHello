using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PageBreak;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageConditionsMedicalesBuilder : IPageConditionsMedicalesBuilder
    {

        private readonly IReportFactory _reportFactory;
        private readonly IPageConditionsMedicalesMapper _mapper;
        private readonly ISectionTexteDescriptionBuilder _texteDescriptionBuilder;

        public PageConditionsMedicalesBuilder(IReportFactory reportFactory, 
            IPageConditionsMedicalesMapper mapper,
            ISectionTexteDescriptionBuilder texteDescriptionBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _texteDescriptionBuilder = texteDescriptionBuilder;
        }

        public void Build(BuildParameters<SectionConditionsMedicalesModel> parameters)
        {
            if (parameters.Data.Sections == null || !parameters.Data.Sections.Any()) return;
            var conditionsMedicalesViewModel = new PageConditionsMedicalesViewModel();
            _mapper.Map(parameters.Data, conditionsMedicalesViewModel, parameters.ReportContext);

            var param = new BuildParameters<PageConditionsMedicalesViewModel>(conditionsMedicalesViewModel)
            {
                ParentReport = parameters.ParentReport,
                ReportContext = parameters.ReportContext,
                StyleOverride = parameters.StyleOverride
            };

            var report = _reportFactory.Create<IPageConditionsMedicales>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param, 
                vm => BuildSubparts(report, param.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(IReport report,
                                   PageConditionsMedicalesViewModel pageConditionsMedicalesViewModel,
                                   IReportContext reportContext,
                                   IStyleOverride styleOverride)
        {
            var premierePage = true;
            foreach (var model in pageConditionsMedicalesViewModel.Sections)
            {
                foreach (var detail in model.Details
                    .OrderBy(d => d.SequenceId).ThenBy(d => d.Libelle))
                {
                    if (!premierePage)
                    {
                        report.AddSubReport(_reportFactory.Create<IPageBreakSubReport>());
                    }
                    premierePage = false;

                    _texteDescriptionBuilder.Build(
                        new BuildParameters<ConditionMedicaleViewModel>(detail)
                        {
                            ReportContext = reportContext,
                            ParentReport = report,
                            StyleOverride = styleOverride
                        });
                }
            }
        }
    }
}