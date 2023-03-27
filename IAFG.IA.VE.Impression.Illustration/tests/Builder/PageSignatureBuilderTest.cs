using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class PageSignatureBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageSignature _report = Substitute.For<IPageSignature>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IPageSignatureMapper _mapper = Substitute.For<IPageSignatureMapper>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();

        [TestMethod]
        public void PageSignatureBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            _reportFactory.Create<IPageSignature>().Returns(_report);

            var builder = new PageSignatureBuilder(_reportFactory, _mapper);
            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<SectionSignatureModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionSignature = Auto.Create<SectionSignatureModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionSignatureModel>(sectionSignature)
                   {
                       ParentReport = illustrationMasterReport,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }

    }
}
