using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.HypothesesInvestissement
{
    public class SectionFondsTransitoireBuilder : ISectionFondsTransitoireBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionFondsTransitoireMapper _mapper;

        public SectionFondsTransitoireBuilder(IReportFactory reportFactory, ISectionFondsTransitoireMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionFondsTransitoireModel> parameters)
        {
            var report = _reportFactory.Create<ISectionFondsTransitoire>();
            ReportBuilderAssembler.Assemble(report, new FondsTransitoireViewModel(), parameters, _mapper);
        }
    }
}