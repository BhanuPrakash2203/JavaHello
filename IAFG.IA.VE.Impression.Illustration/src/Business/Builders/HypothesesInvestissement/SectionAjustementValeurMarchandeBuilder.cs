using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.HypothesesInvestissement;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.HypothesesInvestissement
{
    public class SectionAjustementValeurMarchandeBuilder : ISectionAjustementValeurMarchandeBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionAjustementValeurMarchandeMapper _mapper;

        public SectionAjustementValeurMarchandeBuilder(IReportFactory reportFactory, ISectionAjustementValeurMarchandeMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionAjustementValeurMarchandeModel> parameters)
        {
            var report = _reportFactory.Create<ISectionAjustementValeurMarchande>();
            ReportBuilderAssembler.Assemble(report, new AjustementValeurMarchandeViewModel(), parameters, _mapper);
        }
    }
}
