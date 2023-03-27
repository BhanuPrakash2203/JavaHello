using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class SectionTableauSurprimesBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionTableauSurprimes _report = Substitute.For<ISectionTableauSurprimes>();
        private readonly ISectionSurprimes _parentReport = Substitute.For<ISectionSurprimes>();
        private readonly ISectionDetailsSurprimesBuilder _sectionDetailsSurprimesBuilder = Substitute.For<ISectionDetailsSurprimesBuilder>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();

        private SectionTableauSurprimesBuilder _builder;
        private BuildParameters<DetailProtectionViewModel> _buildParam;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<ISectionTableauSurprimes>().Returns(_report);
            _builder = new SectionTableauSurprimesBuilder(_reportFactory, _sectionDetailsSurprimesBuilder);
            _buildParam = CreateBuildParameters(_parentReport);
        }

        [TestMethod]
        public void GIVEN_SectionTableauSurprimesBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void GIVEN_SectionTableauSurprimesBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            _builder.Build(_buildParam);

            _sectionDetailsSurprimesBuilder.Received(3).Build(Arg.Any<BuildParameters<DetailSurprimeViewModel>>());
        }

        private BuildParameters<DetailProtectionViewModel> CreateBuildParameters(ISectionSurprimes sectionSurprimes)
        {
            var detailProtectionViewModel = _auto.Create<DetailProtectionViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<DetailProtectionViewModel>(detailProtectionViewModel)
            {
                ParentReport = sectionSurprimes,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }
    }
}