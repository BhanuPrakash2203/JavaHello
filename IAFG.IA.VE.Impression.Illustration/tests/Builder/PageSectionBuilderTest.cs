using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageSectionBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageSection _report = Substitute.For<IPageSection>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IPageSectionMapper _mapper = Substitute.For<IPageSectionMapper>();
        private readonly IReportContext context = _auto.Create<IReportContext>();

        [TestMethod]
        public void PageSectionBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<IPageSection>().Returns(_report);
            var builder = new PageSectionBuilder(_reportFactory, _mapper);
            var buildParam = CreateBuildParameters(_parentReport);
            builder.Build(buildParam);
            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<SectionModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionModel = _auto.Create<SectionModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }

    }
}
