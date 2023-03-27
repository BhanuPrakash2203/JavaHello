using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.DescriptionsProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder
{
    [TestClass]
    public class SectionTexteDescriptionBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionDescription _report = Substitute.For<ISectionDescription>();
        private readonly IPageDescriptionsProtections _parentReport = Substitute.For<IPageDescriptionsProtections>();
        private readonly ISectionTextesBuilder _sectionTextesBuilder = Substitute.For<ISectionTextesBuilder>();
        private readonly ISectionTableauBuilder _sectionTableauBuilder = Substitute.For<ISectionTableauBuilder>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();

        private SectionDescriptionBuilder _builder;
        private BuildParameters<DescriptionViewModel> _buildParam;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<ISectionDescription>().Returns(_report);
            _builder = new SectionDescriptionBuilder(_reportFactory, _sectionTextesBuilder, _sectionTableauBuilder);
            _buildParam = CreateBuildParameters(_parentReport);
        }


        [TestMethod]
        public void GIVEN_SectionTexteDescriptionBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);
            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void GIVEN_SectionTexteDescriptionBuilderr_WHEN_Build_THEN_SubReportsAreAdded()
        {
            _builder.Build(_buildParam);

            _sectionTextesBuilder.Received(1).Build(Arg.Any<BuildParameters<DescriptionViewModel>>());
            _sectionTableauBuilder.Received(1).Build(Arg.Any<BuildParameters<DescriptionViewModel>>());
        }

        private BuildParameters<DescriptionViewModel> CreateBuildParameters(IPageDescriptionsProtections pageDescriptionsProtections)
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
