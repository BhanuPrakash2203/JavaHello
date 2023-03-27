using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionScenarioParticipationsBuilder : ISectionScenarioParticipationsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionScenarioParticipationsMapper _mapper;

        public SectionScenarioParticipationsBuilder(IReportFactory reportFactory, ISectionScenarioParticipationsMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionScenarioParticipationsModel> parameters)
        {
            var report = _reportFactory.Create<ISectionScenarioParticipations>();
            ReportBuilderAssembler.Assemble(report, new ScenarioParticipationsViewModel(), parameters, _mapper);
        }
    }
}