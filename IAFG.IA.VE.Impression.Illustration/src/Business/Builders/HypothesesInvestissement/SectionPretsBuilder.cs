using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.HypothesesInvestissement
{
    public class SectionPretsBuilder : ISectionPretsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionPretsMapper _mapper;

        public SectionPretsBuilder(IReportFactory reportFactory, ISectionPretsMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionPretsModel> parameters)
        {
            var report = _reportFactory.Create<ISectionPrets>();
            ReportBuilderAssembler.Assemble(report, new PretsViewModel(), parameters, _mapper);
        }
    }
}