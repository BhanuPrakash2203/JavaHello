using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionASLBuilder: ISectionASLBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionASLMapper _mapper;

        public SectionASLBuilder(IReportFactory reportFactory, ISectionASLMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionASLModel> parameters)
        {
            var report = _reportFactory.Create<ISectionASL>();
            ReportBuilderAssembler.Assemble(report, new ASLViewModel(), parameters, _mapper);
        }

    }
}