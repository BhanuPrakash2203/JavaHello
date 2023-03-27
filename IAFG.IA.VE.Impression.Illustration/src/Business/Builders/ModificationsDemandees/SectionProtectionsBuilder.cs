using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.ModificationsDemandees;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.ModificationsDemandees
{
    public class SectionProtectionsBuilder : ISectionProtectionsBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly ISectionProtectionsMapper _mapper;

        public SectionProtectionsBuilder(IReportFactory reportFactory, ISectionProtectionsMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionProtectionsModel> parameters)
        {
            var report = _reportFactory.Create<ISectionProtections>();
            ReportBuilderAssembler.Assemble(report, new ProtectionsViewModel(), parameters, _mapper);
        }
    }
}