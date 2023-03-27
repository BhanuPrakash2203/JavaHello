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
    public class SectionProtectionsBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionProtections _report = Substitute.For<ISectionProtections>();
        private readonly ISectionContractants _parentReport = Substitute.For<ISectionContractants>();
        private readonly IReportContext context = _auto.Create<IReportContext>();

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<ISectionProtections>().Returns(_report);

            var builder = new SectionProtectionsBuilder(_reportFactory);
            var buildParam = CreateBuildParameters(_parentReport);
            buildParam.Data.EstAccesVie = false;

            builder.Build(buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<ProtectionViewModel> CreateBuildParameters(ISectionContractants sectionContractants)
        {
            var sectionProtections = _auto.Create<ProtectionViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<ProtectionViewModel>(sectionProtections)
            {
                ParentReport = sectionContractants,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }
    }
}