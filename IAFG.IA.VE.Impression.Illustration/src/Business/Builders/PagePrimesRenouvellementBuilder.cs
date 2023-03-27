using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PagePrimesRenouvellementBuilder: IPagePrimesRenouvellementBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPagePrimesRenouvellementMapper _mapper;
        private readonly ISectionPrimesRenouvellementBuilder _sectionPrimesRenouvellementBuilder;

        public PagePrimesRenouvellementBuilder(IReportFactory reportFactory, IPagePrimesRenouvellementMapper mapper, ISectionPrimesRenouvellementBuilder sectionPrimesRenouvellementBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionPrimesRenouvellementBuilder = sectionPrimesRenouvellementBuilder;
        }

        public void Build(BuildParameters<PagePrimesRenouvellementModel> parameters)
        {
            if (parameters.Data.SectionPrimesRenouvellementModels == null || !parameters.Data.SectionPrimesRenouvellementModels.Any(x => x.DetailsPrimeRenouvellement.Any(p => p.Periodes.Any()))) return;

            var pagePrimesRenouvellementViewModel = new PagePrimesRenouvellementViewModel();
            _mapper.Map(parameters.Data, pagePrimesRenouvellementViewModel, parameters.ReportContext);

            var param = new BuildParameters<PagePrimesRenouvellementViewModel>(pagePrimesRenouvellementViewModel)
                        {
                            ParentReport = parameters.ParentReport,
                            ReportContext = parameters.ReportContext,
                            StyleOverride = parameters.StyleOverride
                        };

            var report = _reportFactory.Create<IPagePrimesRenouvellement>();
            ReportBuilderAssembler.AssembleWithoutModelMapping(report, param.Data, param, vm => BuildSubParts(report, param.Data, parameters.ReportContext, parameters.StyleOverride));
        }

        private void BuildSubParts(IPagePrimesRenouvellement report, PagePrimesRenouvellementViewModel paramData, IReportContext reportContext, IStyleOverride styleOverride)
        {
            foreach (var sectionPrimeRenouvellementViewModel in paramData.SectionPrimesRenouvellementViewModels)
            {
                foreach (var detailsPrimeRenouvellementViewModel in sectionPrimeRenouvellementViewModel.DetailsPrimeRenouvellement.Where(p => p.Periodes.Any()))
                {
                    _sectionPrimesRenouvellementBuilder.Build(new BuildParameters<DetailsPrimeRenouvellementViewModel>(detailsPrimeRenouvellementViewModel)
                                                              {
                                                                  ReportContext = reportContext,
                                                                  ParentReport = report,
                                                                  StyleOverride = styleOverride
                                                              });
                }
            }
        }
    }
}