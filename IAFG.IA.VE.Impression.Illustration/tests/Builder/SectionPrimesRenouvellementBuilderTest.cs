using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.PrimesRenouvellement;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.PrimesRenouvellement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class SectionPrimesRenouvellementBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionPrimesRenouvellement _report = Substitute.For<ISectionPrimesRenouvellement>();
        private readonly IPagePrimesRenouvellement _parentReport = Substitute.For<IPagePrimesRenouvellement>();
        private readonly IReportContext context = _auto.Create<IReportContext>();

        [TestMethod]
        public void PageResultatBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilder();

            _parentReport.Received(1).AddSubReport(_report);
        }

        

        private void CallReportBuilder()
        {
            _reportFactory.Create<ISectionPrimesRenouvellement>().Returns(_report);
            var builder = new SectionPrimesRenouvellementBuilder(_reportFactory);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);
        }

        private BuildParameters<DetailsPrimeRenouvellementViewModel> CreateBuildParameters(IPagePrimesRenouvellement illustrationMasterReport)
        {
            var sectionModel = _auto.Create<DetailsPrimeRenouvellementViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<DetailsPrimeRenouvellementViewModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }
    }
}