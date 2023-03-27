using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral
{
    public class PageTitreBuilder : IPageTitreBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageTitreMapper _mapper;

        public PageTitreBuilder(IReportFactory reportFactory, IPageTitreMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<TitreRapportModel> parameters)
        {
            var report = _reportFactory.Create<IPageTitre>();
            ReportBuilderAssembler.Assemble(report, new PageTitreViewModel(), parameters, _mapper);
        }
    }
}
