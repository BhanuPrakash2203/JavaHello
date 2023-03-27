using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using IAFG.IA.VE.Impression.Core.Builders;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Core.Types.Styles;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Builders;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Builders.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.MasterReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.SubReports;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.SommaireProtectionsIllustration;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using DetailFluxMonetaire = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailFluxMonetaire;
using DetailPrime = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailPrime;
using SectionDetailParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailParticipationsModel;
using SectionSurprimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionSurprimesModel;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Builder.SommaireProtectionsIllustration
{
    [TestClass]
    public class PageSommaireProtectionsIllustrationBuilderTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private readonly IIllustrationResourcesAccessorFactory _resourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private readonly IReportFactory _reportFactory = Substitute.For<IReportFactory>();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Auto.Create<IIllustrationReportDataFormatter>();
        private readonly IPageSommaireProtectionsIllustration _report = Substitute.For<IPageSommaireProtectionsIllustration>();
        private readonly ISectionContractantsBuilder _sectionContractantBuilder = Substitute.For<ISectionContractantsBuilder>();
        private readonly ISectionAssuresBuilder _sectionAssuresBuilder = Substitute.For<ISectionAssuresBuilder>();
        private readonly ISectionSurprimesBuilder _sectionSurprimesBuilder = Substitute.For<ISectionSurprimesBuilder>();
        private readonly ISectionConseillerBuilder _sectionConseillerBuilder = Substitute.For<ISectionConseillerBuilder>();
        private readonly ISectionPrimesBuilder _sectionPrimesBuilder = Substitute.For<ISectionPrimesBuilder>();
        private readonly ISectionPrimesVerseesBuilder _sectionPrimesVerseesBuilder = Substitute.For<ISectionPrimesVerseesBuilder>();
        private readonly ISectionASLBuilder _sectionAslBuilder = Substitute.For<ISectionASLBuilder>();
        private readonly ISectionCaracteristiquesIllustrationBuilder _sectionCaracteristiquesIllustrationBuilder = Substitute.For<ISectionCaracteristiquesIllustrationBuilder>();
        private readonly ISectionFluxMonetaireBuilder _sectionFluxMonetaireBuilder = Substitute.For<ISectionFluxMonetaireBuilder>();
        private readonly ISectionDetailParticipationsBuilder _sectionDetailParticipationsBuilder = Substitute.For<ISectionDetailParticipationsBuilder>();
        private readonly ISectionChangementAffectationParticipationsBuilder _sectionChangementAffectationParticipationsBuilder = Substitute.For<ISectionChangementAffectationParticipationsBuilder>();
        private readonly ISectionScenarioParticipationsBuilder _sectionScenarioParticipationsBuilder = Substitute.For<ISectionScenarioParticipationsBuilder>();
        private readonly ISectionDetailEclipseDePrimeBuilder _sectionDetailEclipseDePrimeBuilder = Substitute.For<ISectionDetailEclipseDePrimeBuilder>();
        private readonly ISectionUsageAuConseillerBuilder _sectionUsageAuConseillerPrimeBuilder = Substitute.For<ISectionUsageAuConseillerBuilder>();
        private readonly ISectionProtectionsBuilder _sectionProtectionsBuilder = Substitute.For<ISectionProtectionsBuilder>();
        private readonly IIllustrationMasterReport _parentReport = Substitute.For<IIllustrationMasterReport>();
        private readonly IReportContext _context = Auto.Create<IReportContext>();
        private readonly IManagerFactory _managerFactory = Substitute.For<IManagerFactory>();
        private AutoMapperFactory _autoMapperFactory;

        private PageSommaireProtectionsIllustrationBuilder _builder;
        private BuildParameters<SectionSommaireProtectionsIllustrationModel> _buildParam;
        private PageSommaireProtectionsIllustrationMapper _mapper;

        [TestInitialize]
        public void Initialiser()
        {
            _reportFactory.Create<IPageSommaireProtectionsIllustration>().Returns(_report);
            _autoMapperFactory = new AutoMapperFactory(ReportDataFormatter, _resourceAccessorFactory, _managerFactory);
            _buildParam = CreateBuildParameters(_parentReport);
            _mapper = new PageSommaireProtectionsIllustrationMapper(_autoMapperFactory);
            _builder = new PageSommaireProtectionsIllustrationBuilder(_reportFactory, _mapper,
                _sectionContractantBuilder, _sectionAssuresBuilder,
                _sectionSurprimesBuilder, _sectionConseillerBuilder, _sectionPrimesBuilder,
                _sectionPrimesVerseesBuilder, _sectionCaracteristiquesIllustrationBuilder,
                _sectionFluxMonetaireBuilder, _sectionAslBuilder, _sectionDetailParticipationsBuilder,
                _sectionChangementAffectationParticipationsBuilder, _sectionDetailEclipseDePrimeBuilder,
                _sectionScenarioParticipationsBuilder, _sectionUsageAuConseillerPrimeBuilder,
                _sectionProtectionsBuilder);
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_Build_THEN_ShouldAddItselfToParentReport()
        {
            _builder.Build(_buildParam);
            _parentReport.AddSubReport(_report);
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_Build_THEN_SubReportsAreAdded()
        {
            _builder.Build(_buildParam);
            _sectionContractantBuilder.Build(Arg.Any<BuildParameters<SectionContractantsViewModel>>());
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_BuildWithoutPrimes_THEN_SubReportsAreNotAdded()
        {
            _buildParam.Data.SectionPrimesModel.Primes = new List<DetailPrime>();
            _builder.Build(_buildParam);
            _sectionPrimesBuilder.Build(Arg.Any<BuildParameters<SectionPrimesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_BuildWithSectionPrimesNull_THEN_SubReportsAreNotAdded()
        {
            _buildParam.Data.SectionPrimesModel = null;
            _builder.Build(_buildParam);
            _sectionPrimesBuilder.Build(Arg.Any<BuildParameters<SectionPrimesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_Primes_WHEN_Build_THEN_SectionPrimesAdded()
        {
            _buildParam.Data.SectionPrimesModel.Primes = new List<DetailPrime> { new DetailPrime() };
            _builder.Build(_buildParam);
            _sectionPrimesBuilder.Build(Arg.Any<BuildParameters<SectionPrimesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_DetailParticipations_WHEN_Build_THEN_SectionAdded()
        {
            _buildParam.Data.SectionDetailParticipationsModel = new SectionDetailParticipationsModel();
            _builder.Build(_buildParam);
            _sectionDetailParticipationsBuilder.Build(Arg.Any<BuildParameters<SectionDetailParticipationsViewModel>>());
        }

        [TestMethod]
        public void GIVEN_ChangementAffectationParticipations_WHEN_Build_THEN_SectionAdded()
        {
            _buildParam.Data.SectionChangementAffectationParticipationsModel = new SectionChangementAffectationParticipationsModel();
            _builder.Build(_buildParam);
            _sectionChangementAffectationParticipationsBuilder.Build(Arg.Any<BuildParameters<SectionChangementAffectationParticipationsViewModel>>());
        }

        [TestMethod]
        public void GIVEN_NoConseiller_WHEN_Build_THEN_SectionConseillerNotAdded()
        {
            _buildParam.Data.SectionConseillerModel.Conseillers = new List<Agent>();
            _builder.Build(_buildParam);
            _sectionConseillerBuilder.Build(Arg.Any<BuildParameters<SectionConseillerViewModel>>());
        }

        [TestMethod]
        public void GIVEN_Conseiller_WHEN_Build_THEN_SectionConseillerAdded()
        {
            var agent = Auto.Create<Agent>();
            agent.Nom = "Nom";
            agent.Prenom = "Prenom";

            _buildParam.Data.SectionConseillerModel.Conseillers = new List<Agent> { agent };
            _builder.Build(_buildParam);
            _sectionConseillerBuilder.Build(Arg.Any<BuildParameters<SectionConseillerViewModel>>());
        }

        [TestMethod]
        public void GIVEN_DetailProtection_WHEN_Build_THEN_SectionSurprimesAdded()
        {
            var protection = Auto.Create<DetailProtection>();
            protection.Surprimes = null;

            _buildParam.Data.SectionSurprimesModel.Protections = new List<DetailProtection> { protection };
            _builder.Build(_buildParam);
            _sectionSurprimesBuilder.Build(Arg.Any<BuildParameters<SectionSurprimesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_ProtectionWithSurprime_WHEN_Build_THEN_SectionSurprimesAdded()
        {
            var protection = Auto.Create<DetailProtection>();
            protection.Surprimes = new List<DetailSurprime> { Auto.Create<DetailSurprime>() };

            _buildParam.Data.SectionSurprimesModel.Protections = new List<DetailProtection> { protection };
            _builder.Build(_buildParam);
            _sectionSurprimesBuilder.Build(Arg.Any<BuildParameters<SectionSurprimesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_ProtectionsWithPrimesVersees_WHEN_Build_THEN_SectionsPrimesAdded()
        {
            _buildParam.Data.SectionPrimesVerseesModel.PrimesVersees = Auto.CreateMany<DetailPrimesVersees>().ToList();
            _builder.Build(_buildParam);
            _sectionPrimesVerseesBuilder.Received().Build(Arg.Any<BuildParameters<SectionPrimesVerseesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_NoPrimesVersees_WHEN_Build_THEN_PrimesVerseesSubReportNotAdded()
        {
            _buildParam.Data.SectionPrimesVerseesModel.PrimesVersees = new List<DetailPrimesVersees>();
            _builder.Build(_buildParam);
            _sectionPrimesVerseesBuilder.Received(0).Build(Arg.Any<BuildParameters<SectionPrimesVerseesViewModel>>());
        }

        [TestMethod]
        public void GIVEN_ProtectionsWithFluxMonetaire_WHEN_Build_THEN_SectionFluxMonetaireAdded()
        {
            var buildParam = CreateBuildParameters(_parentReport);
            buildParam.Data.SectionFluxMonetaireModel.Details = Auto.CreateMany<DetailFluxMonetaire>().ToList();

            _builder.Build(_buildParam);
            _sectionFluxMonetaireBuilder.Received().Build(Arg.Any<BuildParameters<SectionFluxMonetaireViewModel>>());
        }

        [TestMethod]
        public void GIVEN_ProtectionWithoutFluxMonetaire_WHEN_Build_THEN_FluxMonetaireNotAdded()
        {
            _buildParam.Data.SectionFluxMonetaireModel.Details = null;
            _builder.Build(_buildParam);
            _sectionFluxMonetaireBuilder.Received(0).Build(Arg.Any<BuildParameters<SectionFluxMonetaireViewModel>>());
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_BuildWithSectionCaracteristiquesIllustration_THEN_SectionCaracteristiqueIllustrationAdded()
        {
            _buildParam.Data.SectionCaracteristiquesIllustrationModel = Auto.Create<SectionCaracteristiquesIllustrationModel>();
            _builder.Build(_buildParam);
            _sectionCaracteristiquesIllustrationBuilder.Received().Build(Arg.Any<BuildParameters<SectionCaracteristiquesIllustrationViewModel>>());
        }

        [TestMethod]
        public void GIVEN_PageSommaireProtectionsIllustrationBuilder_WHEN_BuildWithoutSectionCaracteristiquesIllustration_THEN_SectionCaracteristiquesIllustrationNotAdded()
        {
            var protection = Auto.Create<DetailProtection>();
            protection.Surprimes = new List<DetailSurprime> { Auto.Create<DetailSurprime>() };

            _buildParam.Data.SectionCaracteristiquesIllustrationModel = null;
            _builder.Build(_buildParam);
            _sectionCaracteristiquesIllustrationBuilder.Received(0).Build(Arg.Any<BuildParameters<SectionCaracteristiquesIllustrationViewModel>>());
        }

        private BuildParameters<SectionSommaireProtectionsIllustrationModel> CreateBuildParameters(IIllustrationMasterReport illustrationMasterReport)
        {
            var sommaireProtectionModel = Auto.Create<SectionSommaireProtectionsIllustrationModel>();
            var sectionContractant = Auto.Create<SectionContractantsModel>();
            var sectionSurprime = Auto.Create<SectionSurprimesModel>();
            sommaireProtectionModel.SectionContractantsModel = sectionContractant;
            sommaireProtectionModel.SectionSurprimesModel = sectionSurprime;

            var styleOverride = new StyleOverride { MarginLevel = MarginLevel.Level1, MoveAllLabels = false };

            return new BuildParameters<SectionSommaireProtectionsIllustrationModel>(sommaireProtectionModel)
            {
                ParentReport = illustrationMasterReport,
                ReportContext = _context,
                StyleOverride = styleOverride
            };
        }
    }
}