using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionDetailEclipseDePrimeBuilder : ISectionDetailEclipseDePrimeBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionDetailEclipseDePrimeMapper _mapper;

        public SectionDetailEclipseDePrimeBuilder(IReportFactory reportFactory, ISectionDetailEclipseDePrimeMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionDetailEclipseDePrimeModel> parameters)
        {
            var report = _reportFactory.Create<ISectionDetailEclipseDePrime>();
            ReportBuilderAssembler.Assemble(report, new DetailEclipseDePrimeViewModel(), parameters, _mapper);
        }
    }
}
