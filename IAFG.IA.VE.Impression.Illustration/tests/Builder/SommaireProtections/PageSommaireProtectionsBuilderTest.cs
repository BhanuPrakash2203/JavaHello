using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Builder.SommaireProtections
{
    [TestClass]
    public class PageSommaireProtectionsBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private AutoMapperFactory _autoMapperFactory;
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private readonly IPageSommaireProtections _report = Substitute.For<IPageSommaireProtections>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private readonly IIllustrationReportDataFormatter _reportDataFormatter = Substitute.For<IIllustrationReportDataFormatter>();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessor = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly ISectionIdentificationBuilder _sectionIdentificationBuilder = Substitute.For<ISectionIdentificationBuilder>();
        private readonly ISectionPrimesBuilder _sectionPrimesBuilder = Substitute.For<ISectionPrimesBuilder>();
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder = Substitute.For<ISectionProtectionsBuilder>();
        private readonly ISectionFluxMonetaireBuilder _sectionFluxMonetaireBuilder = Substitute.For<ISectionFluxMonetaireBuilder>();
        private readonly ISectionASLBuilder _sectionAssuranceSupplementaireBuilder = Substitute.For<ISectionASLBuilder>();
        private readonly ISectionSurprimesBuilder _surprimesBuilder = Substitute.For<ISectionSurprimesBuilder>();
        private readonly ISectionDetailParticipationsBuilder _detailParticipationsBuilder = Substitute.For<ISectionDetailParticipationsBuilder>();
        private readonly ISectionDetailEclipseDePrimeBuilder _detailEclipseDePrimeBuilder = Substitute.For<ISectionDetailEclipseDePrimeBuilder>();
        private readonly ISectionScenarioParticipationsBuilder _scenarioParticipationsBuilder = Substitute.For<ISectionScenarioParticipationsBuilder>();
        private readonly ISectionUsageAuConseillerBuilder _scenarioUsageAuConseillerBuilder = Substitute.For<ISectionUsageAuConseillerBuilder>();
        private readonly ISectionAvancesSurPoliceBuilder _sectionAvancesSurPoliceBuilder = Substitute.For<ISectionAvancesSurPoliceBuilder>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<IPageSommaireProtections>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(_reportDataFormatter, _resourcesAccessor, _managerFactory);
        }

        [TestMethod]
        public void PageResultatBuilder_When_Build_Then_ShouldAddItselfToParentReport()
        {
            CallReportBuilder();
            _parentReport.Received(1).AddSubReport(_report);
        }

        [TestMethod]
        public void PageResultatBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            CallReportBuilder();
            _sectionIdentificationBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionIdendificationModel>>());
            _sectionProtectionsBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionProtectionsModel>>());
            _surprimesBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionSurprimesModel>>());
            _sectionPrimesBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionPrimesModel>>());
            _sectionAssuranceSupplementaireBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionASLModel>>());
            _sectionFluxMonetaireBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionFluxMonetaireModel>>());
            _detailParticipationsBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionDetailParticipationsModel>>());
            _sectionAvancesSurPoliceBuilder.Received(1).Build(Arg.Any<BuildParameters<SectionAvancesSurPoliceModel>>());
        }

        private void CallReportBuilder()
        {
            var builder = new PageSommaireProtectionsBuilder(
                _reportFactory, 
                new PageSommaireProtectionsMapper(_autoMapperFactory), 
                _sectionIdentificationBuilder,
                _sectionPrimesBuilder,
                _sectionProtectionsBuilder,
                _sectionFluxMonetaireBuilder,
                _sectionAssuranceSupplementaireBuilder,
                _surprimesBuilder, 
                _detailParticipationsBuilder,
                _detailEclipseDePrimeBuilder,
                _scenarioParticipationsBuilder,
                _scenarioUsageAuConseillerBuilder,
                _sectionAvancesSurPoliceBuilder);

            var buildParam = CreateBuildParameters(_parentReport);
            builder.Build(buildParam);
        }

        private BuildParameters<SectionSommaireProtectionsModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sectionModel = Auto.Create<SectionSommaireProtectionsModel>();
            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionSommaireProtectionsModel>(sectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }

    }
}
