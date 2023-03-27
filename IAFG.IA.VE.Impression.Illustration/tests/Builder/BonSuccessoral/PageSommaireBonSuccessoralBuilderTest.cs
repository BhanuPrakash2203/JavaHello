using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.BonSuccessoral.Sommaire;
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
    public class PageSommaireBonSuccessoralBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageSommaireBonSuccessoral _report = Substitute.For<IPageSommaireBonSuccessoral>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IPageSommaireBonSuccessoralMapper _mapper = Substitute.For<IPageSommaireBonSuccessoralMapper>();
        private readonly IReportContext context = _auto.Create<IReportContext>();

        [TestMethod]
        public void PageBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            var sectionContratBuilder = Substitute.For<ISectionContratBuilder>();
            var sectionHypothesesInvestissementBuilder = Substitute.For<ISectionHypothesesInvestissementBuilder>();
            var sectionImpositionBuilder = Substitute.For<ISectionImpositionBuilder>();

            _reportFactory.Create<IPageSommaireBonSuccessoral>().Returns(_report);

            var builder = new PageSommaireBonSuccessoralBuilder(
                _reportFactory, 
                sectionContratBuilder, 
                sectionHypothesesInvestissementBuilder,
                sectionImpositionBuilder,
                _mapper);

            var buildParam = CreateBuildParameters(_parentReport);

            builder.Build(buildParam);
            _parentReport.Received(1).AddSubReport(_report);
        }

        private BuildParameters<SommaireBonSuccessoralModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionModel = _auto.Create<SommaireBonSuccessoralModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SommaireBonSuccessoralModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = context,
                StyleOverride = styleOverride
            };
        }
    }
}
