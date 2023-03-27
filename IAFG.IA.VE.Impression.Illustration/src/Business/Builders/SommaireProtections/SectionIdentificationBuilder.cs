using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionIdentificationBuilder : ISectionIdentificationBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionIdentificationMapper _mapper;

        public SectionIdentificationBuilder(IReportFactory reportFactory, ISectionIdentificationMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionIdendificationModel> parameters)
        {
            var report = _reportFactory.Create<ISectionIdentification>();
            ReportBuilderAssembler.Assemble(report, new IdentificationViewModel(), parameters, _mapper);
        }
    }
}