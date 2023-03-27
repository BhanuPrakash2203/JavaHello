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
    public class PageGlossaireBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageGlossaire _report = Substitute.For<IPageGlossaire>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IPageGlossaireMapper _mapper = Substitute.For<IPageGlossaireMapper>();
        private readonly IReportContext context = _auto.Create<IReportContext>();

        [TestMethod]
        public void PageGlossaireBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<IPageGlossaire>().Returns(_report);
            var builder = new PageGlossaireBuilder(_reportFactory, _mapper);
            var buildParam = CreateBuildParameters(_parentReport);
            builder.Build(buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<SectionGlossaireModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionModel = _auto.Create<SectionGlossaireModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionGlossaireModel>(sectionModel)
                   {
                       ParentReport = illustrationMasterReport,
                       ReportContext = context,
                       StyleOverride = styleOverride
                   };
        }

    }
}
