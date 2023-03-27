using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.Sommaire;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Sommaire;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class SectionSommaireBuilderTest
    {
        private static readonly IFixture Fixture = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionSommaire _reportSection = Substitute.For<ISectionSommaire>();
        private readonly IPageSommaire _parentReport = Substitute.For<IPageSommaire>();
        private readonly IReportContext _context = Fixture.Create<IReportContext>();

        [TestMethod]
        public void GIVEN_SectionBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<ISectionSommaire>().Returns(_reportSection);

            var builder = new SectionSommaireBuilder(_reportFactory);
            var buildParam = CreateBuildParameters(_parentReport);
            builder.Build(buildParam);
            _parentReport.Received(1).AddSubReport(_reportSection);
        }

        private BuildParameters<SectionSommaireViewModel> CreateBuildParameters(IPageSommaire page)
        {
            var model = Fixture.Create<SectionSommaireViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionSommaireViewModel>(model)
                   {
                       ParentReport = page,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}
