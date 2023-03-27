using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.HypothesesInvestissement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{

    [TestClass]
    public class PageHypotheseInvestissementBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageHypotheseInvestissement _report = Substitute.For<IPageHypotheseInvestissement>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportContext context = _auto.Create<IReportContext>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private AutoMapperFactory _autoMapperFactory;
        private readonly ISectionFondsCapitalisationBuilder _sectionFondsCapitalisationBuilder = Substitute.For<ISectionFondsCapitalisationBuilder>();
        private readonly ISectionFondsTransitoireBuilder _sectionFondsTransitoireBuilder = Substitute.For<ISectionFondsTransitoireBuilder>();
        private readonly ISectionPretsBuilder _sectionPretsBuilder = Substitute.For<ISectionPretsBuilder>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private readonly ISectionAjustementValeurMarchandeBuilder _sectionAjustementValeurMarchandeBuilder = Substitute.For<ISectionAjustementValeurMarchandeBuilder>();

        [TestMethod]
        public void PageResultatBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilder();

            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void PageResultatBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            CallReportBuilder();

            _sectionFondsCapitalisationBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionFondsCapitalisationModel>>());
            _sectionFondsTransitoireBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionFondsTransitoireModel>>());
            _sectionPretsBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionPretsModel>>());
        }

        private void CallReportBuilder()
        {
            _reportFactory.Create<IPageHypotheseInvestissement>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);
            var builder = new PageHypotheseInvestissementBuilder(
                _reportFactory, 
                new PageHypotheseInvestissementMapper(_autoMapperFactory),
                _sectionFondsCapitalisationBuilder,
                _sectionFondsTransitoireBuilder,
                _sectionPretsBuilder,
                _sectionAjustementValeurMarchandeBuilder);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);
        }

        private BuildParameters<SectionHypothesesInvestissementModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionModel = _auto.Create<SectionHypothesesInvestissementModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionHypothesesInvestissementModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }

    }
}
