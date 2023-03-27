using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.SommaireProtections
{
    [TestClass]
    public class SectionAvancesSurPoliceBuilderTests
    {
        private static readonly IFixture AutoFixture = AutoFixtureFactory.Create();
        private SectionAvancesSurPoliceBuilder _builder;
        private IReportFactory _reportFactory;
        private IReportContext _context;
        private IPageSommaireProtections _masterReport;
        private AutoMapperFactory _autoMapperFactory;
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestInitialize]
        public void Initialize()
        {
            _reportFactory = Substitute.For<IReportFactory>();
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);
            _context = AutoFixture.Create<IReportContext>();
            _builder = new SectionAvancesSurPoliceBuilder(_reportFactory, new SectionAvancesSurPoliceMapper(_autoMapperFactory));
        }

        [TestMethod]
        public void Build_WhenBuidParameters_ShouldAssembleReportProperly()
        {
            var sectionModel = AutoFixture.Create<SectionAvancesSurPoliceModel>();

            _masterReport = Substitute.For<IPageSommaireProtections>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            _builder.Build(new BuildParameters<SectionAvancesSurPoliceModel>(sectionModel)
            {
                ParentReport = _masterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            });

            _masterReport.Received(1).AddSubReport(Arg.Any<ISectionAvancesSurPolice>());
        }
    }
}
