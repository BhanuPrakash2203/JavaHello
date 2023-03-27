using System.Linq;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;


namespace IAFG.IA.VE.Impression.Illustration.Business.Builders
{
    public class PageHypotheseInvestissementBuilder : IPageHypotheseInvestissementBuilder
    {
        private readonly IReportFactory _reportFactory;
        private readonly IPageHypotheseInvestissementMapper _mapper;
        private readonly ISectionFondsCapitalisationBuilder _sectionFondsCapitalisationBuilder;
        private readonly ISectionFondsTransitoireBuilder _sectionFondsTransitoireBuilder;
        private readonly ISectionPretsBuilder _sectionPretsBuilder;
        private readonly ISectionAjustementValeurMarchandeBuilder _sectionAjustementValeurMarchandeBuilder;

        public PageHypotheseInvestissementBuilder(IReportFactory reportFactory,
            IPageHypotheseInvestissementMapper mapper,
            ISectionFondsCapitalisationBuilder sectionFondsCapitalisationBuilder,
            ISectionFondsTransitoireBuilder sectionFondsTransitoireBuilder,
            ISectionPretsBuilder sectionPretsBuilder, 
            ISectionAjustementValeurMarchandeBuilder sectionAjustementValeurMarchandeBuilder)
        {
            _reportFactory = reportFactory;
            _mapper = mapper;
            _sectionFondsCapitalisationBuilder = sectionFondsCapitalisationBuilder;
            _sectionFondsTransitoireBuilder = sectionFondsTransitoireBuilder;
            _sectionPretsBuilder = sectionPretsBuilder;
            _sectionAjustementValeurMarchandeBuilder = sectionAjustementValeurMarchandeBuilder;
        }

        public void Build(BuildParameters<SectionHypothesesInvestissementModel> parameters)
        {
            var report = _reportFactory.Create<IPageHypotheseInvestissement>();
            ReportBuilderAssembler.Assemble(report, new PageHypotheseInvestissementViewModel(), parameters, _mapper, vm => BuildSubParts(report, parameters.Data, parameters.ReportContext));
        }

        private void BuildSubParts(IReport report, SectionHypothesesInvestissementModel sourceObject, IReportContext reportContext)
        {
            _sectionFondsCapitalisationBuilder.Build(new BuildParameters<SectionFondsCapitalisationModel>(sourceObject.SectionFondsCapitalisation)
            {
                ReportContext = reportContext,
                ParentReport = report
            });

            _sectionFondsTransitoireBuilder.Build(new BuildParameters<SectionFondsTransitoireModel>(sourceObject.SectionFondsTransitoire)
            {
                ReportContext = reportContext,
                ParentReport = report
            });

            _sectionAjustementValeurMarchandeBuilder.Build(new BuildParameters<SectionAjustementValeurMarchandeModel>(sourceObject.SectionAjustementValeurMarchande)
            {
                ReportContext = reportContext,
                ParentReport = report
            });

            if (sourceObject.SectionPrets.Prets.Any(x => x.Solde > 0))
            {
                _sectionPretsBuilder.Build(new BuildParameters<SectionPretsModel>(sourceObject.SectionPrets)
                {
                    ReportContext = reportContext,
                    ParentReport = report
                });
            }
        }
    }
}