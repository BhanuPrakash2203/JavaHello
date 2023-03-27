using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionPrimesBuilder: ISectionPrimesBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionPrimesMapper _mapper;

        public SectionPrimesBuilder(IReportFactory reportFactory, ISectionPrimesMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionPrimesModel> parameters)
        {
            var report = _reportFactory.Create<ISectionPrimes>();
            ReportBuilderAssembler.Assemble(report, new ProtectionPrimesViewModel(), parameters, _mapper);
        }
    }
}