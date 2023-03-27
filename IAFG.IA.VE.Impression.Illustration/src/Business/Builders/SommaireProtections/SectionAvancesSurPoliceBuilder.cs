using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionAvancesSurPoliceBuilder : ISectionAvancesSurPoliceBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionAvancesSurPoliceMapper _mapper;

        public SectionAvancesSurPoliceBuilder(IReportFactory reportFactory, ISectionAvancesSurPoliceMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionAvancesSurPoliceModel> parameters)
        {
            var report = _reportFactory.Create<ISectionAvancesSurPolice>();
            ReportBuilderAssembler.Assemble(report, new AvancesSurPoliceViewModel(), parameters, _mapper);
        }
    }
}
