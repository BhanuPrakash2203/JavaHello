using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration
{
    public class SectionSurprimesBuilder : ISectionSurprimesBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionTableauSurprimesBuilder _sectionTableauSurprimesBuilder;

        public SectionSurprimesBuilder(IReportFactory reportFactory, ISectionTableauSurprimesBuilder sectionTableauSurprimesBuilder)
        {
            _reportFactory = reportFactory;
            _sectionTableauSurprimesBuilder = sectionTableauSurprimesBuilder;
        }

        public void Build(BuildParameters<SectionSurprimesViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionSurprimes>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters, vm => BuildSubparts(report, parameters.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(ISectionSurprimes report, SectionSurprimesViewModel parametersData, IReportContext reportContext, IStyleOverride styleOverride)
        {
            foreach (var detailProtectionViewModel in parametersData.Protections.Where(p => p.Surprimes.Any()))
            {
                _sectionTableauSurprimesBuilder.Build(new BuildParameters<DetailProtectionViewModel>(detailProtectionViewModel)
                                                      {
                                                          ReportContext = reportContext,
                                                          ParentReport = report,
                                                          StyleOverride = styleOverride
                                                      });
            }
        }
    }
}