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
    public class SectionChangementAffectationParticipationsBuilderTests
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private SectionChangementAffectationParticipationsBuilder _builder;
        private IReportFactory _reportFactoryContainer;
        private IReportContext _context;
        private IPageSommaireProtectionsIllustration _masterReport;
        private SectionChangementAffectationParticipationsViewModel _sectionViewModel;

        [TestInitialize]
        public void Initialize()
        {
            _sectionViewModel = Auto.Create<SectionChangementAffectationParticipationsViewModel>();
            _reportFactoryContainer = Substitute.For<IReportFactory>();
            _context = Auto.Create<IReportContext>();
            _builder = new SectionChangementAffectationParticipationsBuilder(_reportFactoryContainer);
        }

        [TestMethod]
        public void Build_WhenBuidParameters_ShouldAssembleReportProperly()
        {
            _masterReport = Substitute.For<IPageSommaireProtectionsIllustration>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            _builder.Build(new BuildParameters<SectionChangementAffectationParticipationsViewModel>(_sectionViewModel)
            {
                ParentReport = _masterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            });

            _masterReport.Received(1).AddSubReport(Arg.Any<ISectionChangementAffectationParticipations>());
        }
    }
}
