using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Resultats;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.Resultats
{
    public class SectionTableauResultatBuilder: ISectionTableauResultatBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionTableauResultatBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<TableauResultatViewModel> parameters)
        {
            var report = _reportFactory.Create<ISectionTableauResultat>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
        }
    }
}