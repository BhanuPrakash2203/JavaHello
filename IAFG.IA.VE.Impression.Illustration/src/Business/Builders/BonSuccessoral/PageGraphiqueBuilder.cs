using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral
{
    public class PageGraphiqueBuilder : IPageGraphiqueBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageGraphiqueMapper _mapper;

        public PageGraphiqueBuilder(IReportFactory reportFactory, IPageGraphiqueMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<PageGraphiqueModel> parameters)
        {
            var report = _reportFactory.Create<IPageGraphique>();
            ReportBuilderAssembler.Assemble(report, new PageGraphiqueViewModel(), parameters, _mapper);
        }
    }
}
