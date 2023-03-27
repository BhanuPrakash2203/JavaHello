using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.SommaireProtectionsIllustration
{
    [TestClass]
    public class SectionCaracteristiquesIllustrationBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionCaracteristiquesIllustration _report = Substitute.For<ISectionCaracteristiquesIllustration>();
        private readonly IPageSommaireProtectionsIllustration _parentReport = Substitute.For<IPageSommaireProtectionsIllustration>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();
        private SectionCaracteristiquesIllustrationBuilder _builder;
        private BuildParameters<SectionCaracteristiquesIllustrationViewModel> _buildParam;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<ISectionCaracteristiquesIllustration>().Returns(_report);
            _builder = new SectionCaracteristiquesIllustrationBuilder(_reportFactory);
            _buildParam = CreateBuildParameters(_parentReport);
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<SectionCaracteristiquesIllustrationViewModel> CreateBuildParameters(IPageSommaireProtectionsIllustration pageSommaireProtectionsIllustration)
        {
            var sectionAssure = _auto.Create<SectionCaracteristiquesIllustrationViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionCaracteristiquesIllustrationViewModel>(sectionAssure)
            {
                ParentReport = pageSommaireProtectionsIllustration,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }
    }
}
