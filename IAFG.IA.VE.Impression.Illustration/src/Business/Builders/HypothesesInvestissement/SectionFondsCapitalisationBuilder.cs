using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.HypothesesInvestissement
{
    public class SectionFondsCapitalisationBuilder : ISectionFondsCapitalisationBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionFondsCapitalisationMapper _mapper;

        public SectionFondsCapitalisationBuilder(IReportFactory reportFactory, ISectionFondsCapitalisationMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionFondsCapitalisationModel> parameters)
        {
            var report = _reportFactory.Create<ISectionFondsCapitalisation>();
            ReportBuilderAssembler.Assemble(report, new FondsCapitalisationViewModel(), parameters, _mapper);
        }
    }
}