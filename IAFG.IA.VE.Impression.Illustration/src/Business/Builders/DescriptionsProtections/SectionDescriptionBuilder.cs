using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PageBreak;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.DescriptionsProtections
{
    public class SectionDescriptionBuilder : ISectionDescriptionBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionTextesBuilder _sectionTextesBuilder;
        private readonly ISectionTableauBuilder _sectionTableauBuilder;

        public SectionDescriptionBuilder(IReportFactory reportFactory,
                                              ISectionTextesBuilder sectionTextesBuilder,
                                              ISectionTableauBuilder sectionTableauBuilder)
        {
            _reportFactory = reportFactory;
            _sectionTextesBuilder = sectionTextesBuilder;
            _sectionTableauBuilder = sectionTableauBuilder;
        }

        public void Build(BuildParameters<DescriptionViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionDescription>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters,
                vm => BuildSubparts(report, parameters.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(ISectionDescription report, DescriptionViewModel model, IReportContext reportContext,
            IStyleOverride styleOverride)
        {
            if (model.SautPage)
            {
                report.AddSubReport(_reportFactory.Create<IPageBreakSubReport>());
            }

            if (model.Textes.Any())
            {
                _sectionTextesBuilder.Build(new BuildParameters<DescriptionViewModel>(model)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }
            if (model.Tableau.Any())
            {
                _sectionTableauBuilder.Build(new BuildParameters<DescriptionViewModel>(model)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }
        }
    }
}