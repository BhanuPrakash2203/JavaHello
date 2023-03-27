using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat.Participations;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.Participations;
using IAFG.IA.VI.Projection.Data.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories.SommaireProtections
{
    [TestClass]
    public class SommaireProtectionsIllustrationModelFactoryTest
    {
        private const string FormattedPercentage = "100,00%";

        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private IIllustrationResourcesAccessorFactory _resourcesAccessorFactory;
        private ImpressionResourcesAccessor _impressionResourcesAccessor;
        private DonneesRapportIllustration _donneesRapportIllustration;
        private DefinitionSection _definition;
        private IDefinitionNoteManager _noteManager;
        private IDefinitionTableauManager _tableauManager;
        private IVecteurManager _vecteurManager;
        private IProductRules _productRules;
        private SommaireProtectionsIllustrationModelFactory _modelFactory;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _noteManager = Substitute.For<IDefinitionNoteManager>();
            _tableauManager = Substitute.For<IDefinitionTableauManager>();
            _vecteurManager = Substitute.For<IVecteurManager>();
            _productRules = Substitute.For<IProductRules>();
            _resourcesAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
            _impressionResourcesAccessor = new ImpressionResourcesAccessor(new IllustrationsResourcesSequence(Helpers.Helpers.CreateCultureAccessor()));
            _resourcesAccessorFactory.GetResourcesAccessor()
                .Returns(_impressionResourcesAccessor);
            _definition = Auto.Create<DefinitionSection>();
            _donneesRapportIllustration = Auto.Create<DonneesRapportIllustration>();
            _donneesRapportIllustration.PourcentagePayableDeces = 1;
            _donneesRapportIllustration.Protections.PrestationDeces = OptionPrestationDeces.CapitalPlusFonds;
            _donneesRapportIllustration.Protections.StatutOacaActif = true;
            _donneesRapportIllustration.ProtectionsGroupees[0].IsApplicant = true;
            _donneesRapportIllustration.ProtectionsGroupees[1].IsApplicant = false;
            _donneesRapportIllustration.ProtectionsGroupees[2].IsApplicant = false;
            _donneesRapportIllustration.Participations.EstChangementOptionParticipationActivee = false;
            _definition.ListSections.First().SectionId = "Contractants";
            _definition.ListSections[1].SectionId = "Assures";
            _definition.ListSections[2].SectionId = "Surprimes";
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "Conseillers" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "Primes" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "CaracteristiquesIllustration" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "PrimesVersees" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "FluxMonetaire" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "ChangementAffectationParticipations" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "DetailEclipseDePrime" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "UsageDuConseiller" });
            

            _donneesRapportIllustration.Clients.First().EstContractant = true;
            InitializeDonnees();
            InitializeDefinition();

            _resourcesAccessorFactory.GetResourcesAccessor().Returns(_impressionResourcesAccessor);
            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(_definition);
            _formatter.FormatterTitre(Arg.Any<DefinitionTitreDescriptionSelonProduit>(), _donneesRapportIllustration).Returns(_definition.Titres.First().Titre);
            _formatter.FormatPercentage(Arg.Any<double>()).Returns(FormattedPercentage);
            _productRules.EstParmiFamilleAssuranceParticipants(Produit.AssuranceParticipant).Returns(true);

            var sectionModelMapper = new SectionModelMapper(_formatter, _noteManager, _tableauManager,
                new DefinitionTitreManager(_formatter), new DefinitionImageManager());

            _modelFactory = new SommaireProtectionsIllustrationModelFactory(
                _configurationRepository,
                _formatter,
                _resourcesAccessorFactory,
                sectionModelMapper,
                _noteManager,
                _productRules, 
                new UsageAuConseillerModelBuilder(sectionModelMapper, _vecteurManager, _productRules),
                new AssuranceSupplementaireLibereeModelBuilder(sectionModelMapper));
        }

        [TestMethod]
        public void GIVEN_ContractTypeTraditionnal_WHEN_Build_Then_MapSectionsCorrectly()
        {
            _donneesRapportIllustration.TypeContrat = ContractType.Traditionnal;
            _donneesRapportIllustration.Produit = Produit.AccesVie;
            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionContractantsModel.Should().NotBeNull();
                model.SectionsAssuresModel.Should().NotBeNull();
                model.SectionSurprimesModel.Should().NotBeNull();
                model.SectionConseillerModel.Should().NotBeNull();
                model.SectionPrimesModel.Should().NotBeNull();
                model.SectionPrimesVerseesModel.Should().BeNull();
                model.SectionCaracteristiquesIllustrationModel.Should().BeNull();
            }
        }

        [TestMethod]
        public void GIVEN_ContractTypeUniversalConjointDernierDeces_WHEN_Build_Then_MapSectionsCorrectly()
        {
            _donneesRapportIllustration.TypeContrat = ContractType.Universal;
            _donneesRapportIllustration.Produit = Produit.Genesis;
            _donneesRapportIllustration.TypeAssurance = TypeAssurance.ConjointeDernierDec;
            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionContractantsModel.Should().NotBeNull();
                model.SectionsAssuresModel.Should().NotBeNull();
                model.SectionSurprimesModel.Should().NotBeNull();
                model.SectionConseillerModel.Should().NotBeNull();
                model.SectionPrimesModel.Should().NotBeNull();
                model.SectionPrimesVerseesModel.Should().NotBeNull(); 

                var prestationDeces = model.SectionCaracteristiquesIllustrationModel.Valeurs.FirstOrDefault(x => x.Key == "PrestationDeces");
                prestationDeces.Should().NotBeNull();
                //prestationDeces.Value[0][0].Should().Be(_impressionResourcesAccessor.GetStringResourceById(OptionPrestationDeces.CapitalPlusFonds.ToString()));

                var optimisationAutomatiqueCapitalAssure = model.SectionCaracteristiquesIllustrationModel.Valeurs.FirstOrDefault(x => x.Key == "OptimisationAutomatiqueCapitalAssure");
                optimisationAutomatiqueCapitalAssure.Should().NotBeNull();
                //optimisationAutomatiqueCapitalAssure.Value[0][0].Should().Be(_impressionResourcesAccessor.GetStringResourceById("_actif"));

                var portionFondsCapitalisationPayable = model.SectionCaracteristiquesIllustrationModel.Valeurs.FirstOrDefault(x => x.Key == "PortionFondsCapitalisationPayable");
                portionFondsCapitalisationPayable.Should().NotBeNull();
                //portionFondsCapitalisationPayable.Value[0][0].Should().Be(_formattedPercentage);
            }
        }

        [TestMethod]
        public void GIVEN_ContractTypeAlternative_WHEN_Build_Then_MapSectionsCorrectly()
        {
            _donneesRapportIllustration.TypeContrat = ContractType.Unspecified;
            _donneesRapportIllustration.Produit = Produit.NonDefini;
            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionContractantsModel.Should().NotBeNull();
                model.SectionsAssuresModel.Should().NotBeNull();
                model.SectionSurprimesModel.Should().NotBeNull();
                model.SectionConseillerModel.Should().NotBeNull();
                model.SectionPrimesModel.Should().NotBeNull();
                model.SectionPrimesVerseesModel.Should().BeNull();
                model.SectionCaracteristiquesIllustrationModel.Should().BeNull();
                model.SectionChangementAffectationParticipationsModel.Should().BeNull();
            }
        }

        [TestMethod]
        public void GIVEN_ContractTypeAssuranceParticipant_WHEN_Build_Then_MapSectionsCorrectly()
        {
            _donneesRapportIllustration.TypeContrat = ContractType.Traditionnal;
            _donneesRapportIllustration.Produit = Produit.AssuranceParticipant;
            _donneesRapportIllustration.Projections.Projection.Columns.Add(new Column { Id = 2007, Value = new double[] { 10000, 2000, 5000, 4000, 5000 } });
            _vecteurManager.ObtenirVecteurMontantNetAuRisque(_donneesRapportIllustration.Projections).Returns(new double[] { 10000, 2000, 5000, 4000, 5000 });
            _vecteurManager.TrouverAnneeSelonIndex(_donneesRapportIllustration.Projections.Projection, 3).Returns(1);

            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                model.TitreSection.Should().Be(_definition.Titres.First().Titre);
                model.SectionContractantsModel.Should().NotBeNull();
                model.SectionsAssuresModel.Should().NotBeNull();
                model.SectionSurprimesModel.Should().NotBeNull();
                model.SectionConseillerModel.Should().NotBeNull();
                model.SectionPrimesModel.Should().NotBeNull();
                model.SectionPrimesVerseesModel.Should().NotBeNull();
                model.SectionCaracteristiquesIllustrationModel.Should().BeNull();
                model.SectionChangementAffectationParticipationsModel.Should().BeNull();
                model.SectionUsageAuConseillerModel.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void GIVEN_ContractTypeAssuranceParticipant_WHEN_Build_And_ColonneProjectionVide_Then_SectionUsageAuConseillerModelIsNull()
        {
            _donneesRapportIllustration.TypeContrat = ContractType.Unspecified;
            _donneesRapportIllustration.Produit = Produit.AssuranceParticipant;
            _donneesRapportIllustration.Projections.Projection.Columns.Add(new Column());
            var model = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                model.SectionUsageAuConseillerModel.Should().BeNull();
            }
        }

        [TestMethod]
        public void Build_WhenEstContratAntidateIsTrue_ThenSectionContractantAndAssuresReciveDateEmission()
        {
            var dateEmission = 28.October(2019);
            _donneesRapportIllustration.TypeContrat = ContractType.Traditionnal;
            _donneesRapportIllustration.Produit = Produit.NonDefini;
            _donneesRapportIllustration.EstContratAntidate = true;
            _donneesRapportIllustration.DateEmission = dateEmission;

            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                result.SectionContractantsModel.DateEffective.Should().Be(dateEmission);
                result.SectionsAssuresModel.Select(x => x.DateEffective).Should().AllBeEquivalentTo(dateEmission);
            }
        }

        [TestMethod]
        public void Build_WhenEstContratAntidateIsFalse_ThenSectionContractantAndAssuresDateEffectiveIsNull()
        {
            _donneesRapportIllustration.TypeContrat = ContractType.Traditionnal;
            _donneesRapportIllustration.EstContratAntidate = false;

            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());

            using (new AssertionScope())
            {
                result.SectionContractantsModel.DateEffective.Should().BeNull();
                result.SectionsAssuresModel.Select(x => x.DateEffective).Should().AllBeEquivalentTo((DateTime?)null);
            }
        }

        [TestMethod]
        public void Build_WhenTransactionChangementAffectationParticipations_ThenSectionChangementAffectationParticipationAndAnnee()
        {
            _donneesRapportIllustration.Participations.EstChangementOptionParticipationActivee = false;
            _donneesRapportIllustration.ModificationsDemandees.Contrat.Transactions.Add(new ChangementOptionParticipation { Annee = 10 });
            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionChangementAffectationParticipationsModel.Annee.Should().Be(10);
        }

        [TestMethod]
        public void Build_WhenConceptChangementAffectationParticipations_ThenSectionChangementAffectationParticipationAndAnnee()
        {
            _donneesRapportIllustration.Participations.EstChangementOptionParticipationActivee = true;
            _donneesRapportIllustration.Participations.AnneeChangementOptionParticipation = 10;
            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionChangementAffectationParticipationsModel.Annee.Should().Be(10);
        }

        [TestMethod]
        public void Build_WhenAucunChangementAffectationParticipations_ThenSectionChangementAffectationParticipationAbsente()
        {
            _donneesRapportIllustration.Participations.EstChangementOptionParticipationActivee = false;            
            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionChangementAffectationParticipationsModel.Should().BeNull();
        }

        [TestMethod]
        public void Build_WhenAucuneSection_ThenSectionChangementAffectationParticipationAbsente()
        {
            var section = _definition.ListSections.FirstOrDefault(s => s.SectionId == "ChangementAffectationParticipations");
            _definition.ListSections.Remove(section);

            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionChangementAffectationParticipationsModel.Should().BeNull();
        }

        [TestMethod]
        public void Build_WhenConceptEclipseDePrimeEtBareme_ThenSectionDetailEclipseDePrimeModelAvecBareme()
        {
            var baremes = new List<Bareme>();
            baremes.Add(new Bareme { Annee = 10, Diminution = 2 });
            _donneesRapportIllustration.Participations.Baremes = baremes;
            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionDetailEclipseDePrimeModel.Baremes[0].Annee.Should().Be(10);
            result.SectionDetailEclipseDePrimeModel.Baremes[0].Diminution.Should().Be(2);
        }

        [TestMethod]
        public void Build_WhenConceptEclipseDePrimeSansBareme_ThenSectionDetailEclipseDePrimeModelAbsente()
        {
            _donneesRapportIllustration.Participations.Baremes = null;
            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionDetailEclipseDePrimeModel.Should().BeNull();
        }

        [TestMethod]
        public void Build_WhenAucuneSection_ThenDetailEclipseDePrimeModelAbsente()
        {
            var section = _definition.ListSections.FirstOrDefault(s => s.SectionId == "DetailEclipseDePrime");
            _definition.ListSections.Remove(section);

            var result = _modelFactory.Build("1", _donneesRapportIllustration, Auto.Create<IReportContext>());
            result.SectionDetailEclipseDePrimeModel.Should().BeNull();
        }

        private void InitializeDonnees()
        {
            _donneesRapportIllustration.PourcentagePayableDeces = 1;
            _donneesRapportIllustration.Protections.PrestationDeces = OptionPrestationDeces.CapitalPlusFonds;
            _donneesRapportIllustration.Protections.StatutOacaActif = true;
            _donneesRapportIllustration.ProtectionsGroupees[0].IsApplicant = true;
            _donneesRapportIllustration.ProtectionsGroupees[1].IsApplicant = false;
            _donneesRapportIllustration.ProtectionsGroupees[2].IsApplicant = false;
        }

        private void InitializeDefinition()
        {
            _definition = Auto.Create<DefinitionSection>();
            _definition.ListSections.First().SectionId = "Contractants";
            _definition.ListSections[1].SectionId = "Assures";
            _definition.ListSections[2].SectionId = "Surprimes";
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "Conseillers" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "Primes" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "CaracteristiquesIllustration" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "PrimesVersees" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "ChangementAffectationParticipations" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "DetailEclipseDePrime" });
            _definition.ListSections.Add(new DefinitionSection() { SectionId = "UsageDuConseiller" });
        }
    }
}
