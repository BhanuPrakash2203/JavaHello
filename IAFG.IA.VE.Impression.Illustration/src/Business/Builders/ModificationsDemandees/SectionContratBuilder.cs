using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.ModificationsDemandees
{
    public class SectionContratBuilder : ISectionContratBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionContratMapper _mapper;

        public SectionContratBuilder(IReportFactory reportFactory, ISectionContratMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionContratModel> parameters)
        {
            var report = _reportFactory.Create<ISectionContrat>();
            ReportBuilderAssembler.Assemble(report, new ContratViewModel(), parameters, _mapper);
        }
    }
}