using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageSectionBuilder : IPageSectionBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageSectionMapper _mapper;

        public PageSectionBuilder(IReportFactory reportFactory, IPageSectionMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionModel> parameters)
        {
            var report = _reportFactory.Create<IPageSection>();
            ReportBuilderAssembler.Assemble(report, new PageViewModel(), parameters, _mapper);
        }
    }
}
