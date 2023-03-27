using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.BonSuccessoral;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.BonSuccessoral
{
    [TestClass]
    public class PageGraphiqueBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageGraphique _report = Substitute.For<IPageGraphique>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IPageGraphiqueMapper _mapper = Substitute.For<IPageGraphiqueMapper>();
        private readonly IReportContext context = _auto.Create<IReportContext>();

        [TestMethod]
        public void PageBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<IPageGraphique>().Returns(_report);

            var builder = new PageGraphiqueBuilder(_reportFactory, _mapper);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);
            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<PageGraphiqueModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionModel = _auto.Create<PageGraphiqueModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<PageGraphiqueModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }
    }
}
