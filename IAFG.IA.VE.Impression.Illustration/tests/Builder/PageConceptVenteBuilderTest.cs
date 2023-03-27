using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageConceptVenteBuilderTest
    {
        private static readonly IFixture Fixture = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageSommaire _report = Substitute.For<IPageSommaire>();
        private readonly IPageSommaire _parentReport = Substitute.For<IPageSommaire>();
        private readonly IReportContext _context = Fixture.Create<IReportContext>();
        private readonly ISectionSommaireBuilder _sectionSommaireBuilder = Substitute.For<ISectionSommaireBuilder>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        [TestMethod]
        public void PageBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilder();
            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void PageBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            CallReportBuilder();
            _sectionSommaireBuilder.Received(5).Build(Arg.Any<BuildParameters<SectionSommaireViewModel>>());
        }

        private void CallReportBuilder()
        {
            _reportFactory.Create<IPageSommaire>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);

            var builder = new PageConceptVenteBuilder(_reportFactory, 
                                                     new PageConceptVenteMapper(_autoMapperFactory),
                                                      _sectionSommaireBuilder);

            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);
        }

        private BuildParameters<SectionConceptVenteModel> CreateBuildParameters(IPageSommaire page)
        {
            var model = Fixture.Create<SectionConceptVenteModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionConceptVenteModel>(model)
                   {
                       ParentReport = page,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}
