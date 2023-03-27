using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionUsageAuConseillerBuilder : ISectionUsageAuConseillerBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionUsageAuConseillerMapper _mapper;

        public SectionUsageAuConseillerBuilder(IReportFactory reportFactory, ISectionUsageAuConseillerMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionUsageAuConseillerModel> parameters)
        {
            var report = _reportFactory.Create<ISectionUsageAuConseiller>();
            ReportBuilderAssembler.Assemble(report, new UsageAuConseillerViewModel(), parameters, _mapper);
        }
    }
}
