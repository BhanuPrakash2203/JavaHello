using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration
{
    public class SectionContractantsBuilder : ISectionContractantsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder;

        public SectionContractantsBuilder(IReportFactory reportFactory, ISectionProtectionsBuilder sectionProtectionsBuilder)
        {
            _reportFactory = reportFactory;
            _sectionProtectionsBuilder = sectionProtectionsBuilder;
        }
        public void Build(BuildParameters<SectionContractantsViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionContractants>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters, vm => BuildSubparts(report, parameters.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(ISectionContractants report, SectionContractantsViewModel parametersData, IReportContext reportContext, IStyleOverride styleOverride)
        {
            if (parametersData.Protections.Protections.Any())
            {
                _sectionProtectionsBuilder.Build(new BuildParameters<ProtectionViewModel>(parametersData.Protections)
                                                 {
                                                     ReportContext = reportContext,
                                                     ParentReport = report,
                                                     StyleOverride = styleOverride
                                                 });
            }
        }
    }
}