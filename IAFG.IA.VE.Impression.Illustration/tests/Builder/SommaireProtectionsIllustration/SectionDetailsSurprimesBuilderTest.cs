using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.SommaireProtectionsIllustration
{
    [TestClass]
    public class SectionDetailsSurprimesBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionDetailsSurprimes _report = Substitute.For<ISectionDetailsSurprimes>();
        private readonly ISectionTableauSurprimes _parentReport = Substitute.For<ISectionTableauSurprimes>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();

        [TestMethod]
        public void GIVEN_SectionDetailsSurprimesBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<ISectionDetailsSurprimes>().Returns(_report);

            var builder = new SectionDetailsSurprimesBuilder(_reportFactory);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }
        private BuildParameters<DetailSurprimeViewModel> CreateBuildParameters(ISectionTableauSurprimes sectionTableauSurprimes)
        {
            var detailSurprimeViewModel = Auto.Create<DetailSurprimeViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<DetailSurprimeViewModel>(detailSurprimeViewModel)
            {
                ParentReport = sectionTableauSurprimes,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }
    }
}