using AutoFixture;
using FluentAssertions.Execution;
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
    public class SectionContractantsBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionContractants _report = Substitute.For<ISectionContractants>();
        private readonly IPageSommaireProtectionsIllustration _parentReport = Substitute.For<IPageSommaireProtectionsIllustration>();
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder = Substitute.For<ISectionProtectionsBuilder>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();
        private SectionContractantsBuilder _builder;
        private BuildParameters<SectionContractantsViewModel> _buildParam;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<ISectionContractants>().Returns(_report);
            _builder = new SectionContractantsBuilder(_reportFactory, _sectionProtectionsBuilder);
            _buildParam = CreateBuildParameters(_parentReport);
        }

        [TestMethod]
        public void GIVEN_SectionContractantsBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);

            _parentReport.AddSubReport(_report);
        }

        [TestMethod]
        public void GIVEN_SectionContractantsBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            _builder.Build(_buildParam);

            using (new AssertionScope())
            {
                _sectionProtectionsBuilder.Build(Arg.Any<BuildParameters<ProtectionViewModel>>());
            }
        }
        private BuildParameters<SectionContractantsViewModel> CreateBuildParameters(IPageSommaireProtectionsIllustration pageSommaireProtectionsIllustration)
        {
            var sectionContractant = _auto.Create<SectionContractantsViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionContractantsViewModel>(sectionContractant)
            {
                ParentReport = pageSommaireProtectionsIllustration,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }
    }
}