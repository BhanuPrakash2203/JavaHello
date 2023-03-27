using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections
{
    public class SectionFluxMonetaireBuilder: ISectionFluxMonetaireBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionFluxMonetaireMapper _mapper;

        public SectionFluxMonetaireBuilder(IReportFactory reportFactory, ISectionFluxMonetaireMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionFluxMonetaireModel> parameters)
        {
            var report = _reportFactory.Create<ISectionFluxMonetaire>();
            ReportBuilderAssembler.Assemble(report, new FluxMonetaireViewModel(), parameters, _mapper);
        }
    }
}