using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionSurprimesBuilder : ISectionSurprimesBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionSurprimeMapper _mapper;

        public SectionSurprimesBuilder(IReportFactory reportFactory, ISectionSurprimeMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }
        public void Build(BuildParameters<SectionSurprimesModel> parameters)
        {
            var report = _reportFactory.Create<ISectionSurprimes>();
            ReportBuilderAssembler.Assemble(report, new SurprimesViewModel(), parameters, _mapper);
        }
    }
}
