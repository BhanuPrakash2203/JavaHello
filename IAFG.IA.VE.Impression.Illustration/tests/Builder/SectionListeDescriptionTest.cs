using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class SectionListeDescriptionTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionTextes _report = Substitute.For<ISectionTextes>();
        private readonly ISectionDescription _parentReport = Substitute.For<ISectionDescription>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();

        [TestMethod]
        public void GIVEN_SectionListeDescriptionBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<ISectionTextes>().Returns(_report);

            var builder = new SectionTextesBuilder(_reportFactory);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<DescriptionViewModel> CreateBuildParameters(ISectionDescription pageDescriptionsProtections)
        {
            var descriptionProtectionViewModel = _auto.Create<DescriptionViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<DescriptionViewModel>(descriptionProtectionViewModel)
                   {
                       ParentReport = pageDescriptionsProtections,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}
