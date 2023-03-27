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
    public class SectionConseillerBuilderTest
    {
        private static readonly IFixture _auto = AutoFixtureFactory.Create();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly ISectionConseiller _report = Substitute.For<ISectionConseiller>();
        private readonly IPageSommaireProtectionsIllustration _parentReport = Substitute.For<IPageSommaireProtectionsIllustration>();
        private readonly IReportContext _context = _auto.Create<IReportContext>();
        private SectionConseillerBuilder _builder;
        private BuildParameters<SectionConseillerViewModel> _buildParam;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<ISectionConseiller>().Returns(_report);
            _builder = new SectionConseillerBuilder(_reportFactory);
            _buildParam = CreateBuildParameters(_parentReport);
        }

        [TestMethod]
        public void GIVEN_SectionConseillerBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);

            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void GIVEN_SectionConseillerBuilder_WHEN_BuildWithConseillerWithoutName_THEN_SubReportsAreNotAdded()
        {
            foreach (var agentViewModel in _buildParam.Data.Conseillers)
            {
                agentViewModel.NomComplet = string.Empty;
            }
            _builder.Build(_buildParam);
        }

        private BuildParameters<SectionConseillerViewModel> CreateBuildParameters(IPageSommaireProtectionsIllustration pageSommaireProtectionsIllustration)
        {
            var sectionConseillerViewModel = _auto.Create<SectionConseillerViewModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionConseillerViewModel>(sectionConseillerViewModel)
                   {
                       ParentReport = pageSommaireProtectionsIllustration,
                       ReportContext = _context,
                       StyleOverride = styleOverride
                   };
        }
    }
}