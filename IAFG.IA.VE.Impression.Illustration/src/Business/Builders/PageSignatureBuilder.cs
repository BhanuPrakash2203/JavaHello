using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageSignatureBuilder : IPageSignatureBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageSignatureMapper _mapper;

        public PageSignatureBuilder(IReportFactory reportFactory, IPageSignatureMapper mapper)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
        }

        public void Build(BuildParameters<SectionSignatureModel> parameters)
        {
            var report = _reportFactory.Create<IPageSignature>();
            ReportBuilderAssembler.Assemble(report, new PageSignatureViewModel(), parameters, _mapper);
        }
    }
}