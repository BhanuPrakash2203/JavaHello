using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.ConditionsMedicales;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ConditionsMedicales;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.ConditionsMedicales
{
    public class SectionTexteDescriptionBuilder : ISectionTexteDescriptionBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionTableauDescriptionBuilder _sectionTableauDescriptionBuilder;

        public SectionTexteDescriptionBuilder(IReportFactory reportFactory,
                                              ISectionTableauDescriptionBuilder sectionTableauDescriptionBuilder)
        {
            _reportFactory = reportFactory;
            _sectionTableauDescriptionBuilder = sectionTableauDescriptionBuilder;
        }

        public void Build(BuildParameters<ConditionMedicaleViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionTexteDescription>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters, vm => BuildSubparts(report, parameters.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubparts(ISectionTexteDescription report, ConditionMedicaleViewModel model, IReportContext reportContext, IStyleOverride styleOverride)
        {
            if (model.Tableau.Any())
            {
                _sectionTableauDescriptionBuilder.Build(new BuildParameters<ConditionMedicaleViewModel>(model)
                {
                    ReportContext = reportContext,
                    ParentReport = report,
                    StyleOverride = styleOverride
                });
            }
        }
    }
}