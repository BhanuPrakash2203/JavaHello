using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration
{
    public class SectionProtectionsBuilder : ISectionProtectionsBuilder
    {
        private readonly IReportFactory _reportFactory;

        public SectionProtectionsBuilder(IReportFactory reportFactory)
        {
            _reportFactory = reportFactory;
        }

        public void Build(BuildParameters<ProtectionViewModel> parameters)
        {
            if (parameters.Data.EstAccesVie)
            {
                var report = _reportFactory.Create<ISectionProtectionsAccesVie>();
                ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
            }
            else
            {
                var report = _reportFactory.Create<ISectionProtections>();
                ReportBuilderAssembler.AssembleWithoutModelMapping(report, parameters.Data, parameters);
            }           
        }
    }
}