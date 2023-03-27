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
    public class SectionPrimesVerseesBuilderTests
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private SectionPrimesVerseesBuilder _sectionPrimesVerseesBuilder;
        private IReportFactory _reportFactoryContainer;
        private IReportContext _context;
        private IPageSommaireProtectionsIllustration _masterReport;
        private SectionPrimesVerseesViewModel _sectionPrimesVerseesViewModel;

        [TestInitialize]
        public void Initialize()
        {
            _sectionPrimesVerseesViewModel = Auto.Create<SectionPrimesVerseesViewModel>();
            _reportFactoryContainer = Substitute.For<IReportFactory>();
            _context = Auto.Create<IReportContext>();
            _sectionPrimesVerseesBuilder = new SectionPrimesVerseesBuilder(_reportFactoryContainer);
        }

        [TestMethod]
        public void Build_WhenBuidParameters_ShouldshouldAssembleReportProperly()
        {
            _masterReport = Substitute.For<IPageSommaireProtectionsIllustration>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            _sectionPrimesVerseesBuilder.Build(new BuildParameters<SectionPrimesVerseesViewModel>(_sectionPrimesVerseesViewModel)
            {
                ParentReport = _masterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            });

            _masterReport.Received(1).AddSubReport(Arg.Any<ISectionPrimesVersees>());
        }
    }
}
