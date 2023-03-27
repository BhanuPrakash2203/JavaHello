using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageGlossaireBuilder : IPageGlossaireBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageGlossaireMapper _mapper;

        public PageGlossaireBuilder(IReportFactory reportFactory, IPageGlossaireMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionGlossaireModel> parameters)
        {
            if (parameters.Data.Details == null || !parameters.Data.Details.Any()) return;
            var report = _reportFactory.Create<IPageGlossaire>();
            ReportBuilderAssembler.Assemble(report, new PageGlossaireViewModel(), parameters, _mapper);
        }
    }
}