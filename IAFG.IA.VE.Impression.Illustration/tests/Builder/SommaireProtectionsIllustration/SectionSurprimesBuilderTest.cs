using System.Collections.Generic;
using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.SommaireProtectionsIllustration
{
    [TestClass]
    public class SectionSurprimesBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionSurprimes _report = Substitute.For<ISectionSurprimes > ();
        private readonly IPageSommaireProtectionsIllustration _parentReport = Substitute.For<IPageSommaireProtectionsIllustration>();
        private readonly ISectionTableauSurprimesBuilder _sectionTableauSurprimesBuilder = Substitute.For<ISectionTableauSurprimesBuilder>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();
        private SectionSurprimesBuilder _builder;
        private BuildParameters<SectionSurprimesViewModel> _buildParam;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<ISectionSurprimes>().Returns(_report);
            _builder = new SectionSurprimesBuilder(_reportFactory, _sectionTableauSurprimesBuilder);
            _buildParam = CreateBuildParameters(_parentReport);
        }

        [TestMethod]
        public void GIVEN_SectionSurprimesBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void GIVEN_SectionSurprimesBuilderr_WHEN_Build_THEN_SubReportsAreAdded()
        {
         
            _builder.Build(_buildParam);

            _sectionTableauSurprimesBuilder.Received(3).Build(Arg.Any<BuildParameters<DetailProtectionViewModel>>());
        }

        [TestMethod]
        public void GIVEN_SectionSurprimesBuilder_WHEN_BuildWithProtectionWithoutSurprime_THEN_SubReportsAreNotAdded()
        {
            foreach (var detailProtectionViewModel in _buildParam.Data.Protections)
            {
                detailProtectionViewModel.Surprimes = new List<DetailSurprimeViewModel>();
            }

            _builder.Build(_buildParam);

            _sectionTableauSurprimesBuilder.DidNotReceive().Build(Arg.Any<BuildParameters<DetailProtectionViewModel>>());
        }

        private BuildParameters<SectionSurprimesViewModel> CreateBuildParameters(IPageSommaireProtectionsIllustration pageSommaireProtectionsIllustration)
        {
            var sectionSurprimesViewModel = _auto.Create<SectionSurprimesViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionSurprimesViewModel>(sectionSurprimesViewModel)
            {
                ParentReport = pageSommaireProtectionsIllustration,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }
    }
}