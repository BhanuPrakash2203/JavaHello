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
    public class SectionTableauSurprimesBuilder : ISectionTableauSurprimesBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionDetailsSurprimesBuilder _sectionDetailsSurprimesBuilder;

        public SectionTableauSurprimesBuilder(IReportFactory reportFactory, ISectionDetailsSurprimesBuilder sectionDetailsSurprimesBuilder)
        {
            _reportFactory = reportFactory;
            _sectionDetailsSurprimesBuilder = sectionDetailsSurprimesBuilder;
        }

        public void Build(BuildParameters<DetailProtectionViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionTableauSurprimes>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters, vm => BuildSubparts(report, parameters.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(ISectionTableauSurprimes report, DetailProtectionViewModel parametersData, IReportContext reportContext, IStyleOverride styleOverride)
        {
            foreach (var detailSurprimeViewModel in parametersData.Surprimes.OrderBy(s => s.Description).ThenBy(s => s.EstTypeTemporaire).ThenBy(s => string.IsNullOrEmpty(s.TauxPourcentage)))
            {
                _sectionDetailsSurprimesBuilder.Build(new BuildParameters<DetailSurprimeViewModel>(detailSurprimeViewModel)
                                                      {
                                                          ReportContext = reportContext,
                                                          ParentReport = report,
                                                          StyleOverride = styleOverride
                                                      });
            }
        }
    }
}