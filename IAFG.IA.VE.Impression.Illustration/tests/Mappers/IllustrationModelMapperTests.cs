using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IAFG.IA.VI.Projection.Data.Contract.Financial;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Extensions;
using IAFG.IA.VI.Projection.Data.Illustration;
using IAFG.IA.VI.Projection.Data.Parameters;
using IAFG.IA.VI.Projection.Data.Parameters.Taxation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Insured = IAFG.IA.VI.Projection.Data.Contract.Insured;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers
{
    [TestClass]
    public class IllustrationModelMapperTests
    {
        private readonly DonneesIllustration _donneesIllustration = new DonneesIllustration { ParametreRapport = new ParametreRapport() };
        private readonly DonneesRapportIllustration _donneesRapportIllustration = new DonneesRapportIllustration { Clients = new List<Client> { new Client() } };

        private IConfigurationRepository _configurationRepository;

        private const string IndIndividual = "IdIndivid";

        [TestInitialize]
        public void TestInitialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
        }

        [TestMethod]
        public void Map_WhenMapIsSignaturePapierDeDonneesIllustration_ThenIsSignaturePapierMapper()
        {
            InitialiserDonneesIllustration();
            _donneesIllustration.ParametreRapport.IsSignaturePapier = true;
            _donneesRapportIllustration.IsSignaturePapier = _donneesIllustration.ParametreRapport.IsSignaturePapier;

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var result = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                result.IsSignaturePapier.Should().Be(_donneesIllustration.ParametreRapport.IsSignaturePapier);
                result.IsSignaturePapier.Should().BeTrue();
            }
        }

        [TestMethod]
        public void Map_WhenMapIsSignaturePapierDeDonneesIllustrationFalse_ThenIsSignaturePapierMapperFalse()
        {
            InitialiserDonneesIllustration();
            _donneesIllustration.ParametreRapport.IsSignaturePapier = false;
            _donneesRapportIllustration.IsSignaturePapier = _donneesIllustration.ParametreRapport.IsSignaturePapier;

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var result = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                result.IsSignaturePapier.Should().Be(_donneesIllustration.ParametreRapport.IsSignaturePapier);
                result.IsSignaturePapier.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Map_WhenParametreRapportIsNull_ThenException()
        {
            var donnees = new DonneesIllustration();

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns((Projection)null);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            Action action = () => mapper.Map(donnees);
            action.Should().Throw<ArgumentNullException>(nameof(donnees.ParametreRapport));
        }

        [TestMethod]
        public void Map_WhenProjectionIstNull_ThenException()
        {
            var donnees = new DonneesIllustration
            {
                ParametreRapport = new ParametreRapport()
            };

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns((Projection)null);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            Action action = () => mapper.Map(donnees);
            action.Should().Throw<ArgumentNullException>(nameof(donnees.Projections));
        }

        [TestMethod]
        public void Map_WhenParametresIstNull_ThenException()
        {
            var donnees = new DonneesIllustration
            {
                ParametreRapport = new ParametreRapport()
            };

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(new Projection());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            Action action = () => mapper.Map(donnees);
            action.Should().Throw<ArgumentNullException>("Parameters");
        }

        [TestMethod]
        public void Map_WhenProtectionBaseIsNull_ThenException()
        {
            var donnees = new DonneesIllustration
            {
                ParametreRapport = new ParametreRapport(),
            };

            var projection = new Projection
            {
                Parameters = new Parameters()
            };

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            Action action = () => mapper.Map(donnees);
            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Map_ThenGoodProperties()
        {
            InitialiserDonneesIllustration();

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection()});

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var result = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                result.EstContratAntidate.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Map_WhenNotCorporation_ThenTauxCorporatifNull()
        {
            InitialiserDonneesIllustration();
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var model = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                model.TauxCorporatif.Should().BeNull();
            }
        }

        [TestMethod]
        public void Map_WhenCorporation_ThenTauxCorporatifCorrect()
        {
            InitialiserDonneesIllustration();

            var personalRates = new Rates()
            {
                MarginalRates = new List<Rate>() { new Rate() { Value = 43 } },
                DividendRates = new List<Rate>() { new Rate() { Value = 44 } },
                CapitalGainsRates = new List<Rate>() { new Rate() { Value = 45 } }
            };

            var corporateRates = new Rates()
            {
                MarginalRates = new List<Rate>() { new Rate() { Value = 46 } },
                DividendRates = new List<Rate>() { new Rate() { Value = 47 } },
                CapitalGainsRates = new List<Rate>() { new Rate() { Value = 48 } }
            };

            var taxation = new Taxation() { Corporate = corporateRates, Personal = personalRates };
            var parameters = new Parameters() { Taxation = taxation, Compensation = new Compensation() { AgentType = new AgentType(), BonusRate = 8 } };

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.ContractType = ContractType.Universal;
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);
            projection.Parameters = parameters;
            projection.Contract.Individuals = new List<Individual> { CreerIndividual(true) };

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());
            projectionManager.ContractantEstCompagnie(Arg.Any<Projection>()).Returns(true);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
               new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var model = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                model.TauxCorporatif.TauxMarginaux.TauxParticulier.Should().Be(43);
                model.TauxCorporatif.TauxMarginaux.TauxCorporation.Should().Be(46);

                model.TauxCorporatif.TauxDividendes.TauxParticulier.Should().Be(44);
                model.TauxCorporatif.TauxDividendes.TauxCorporation.Should().Be(47);

                model.TauxCorporatif.TauxGainCapital.TauxParticulier.Should().Be(45);
                model.TauxCorporatif.TauxGainCapital.TauxCorporation.Should().Be(48);
            }
        }

        [TestMethod]
        public void Map_WhenProfilInvestisseurElectroniqueComplet_ThenProfilInvestisseurElectroniqueCompletMapped()
        {
            InitialiserDonneesIllustration();
            _donneesIllustration.ParametreRapport.ProfilInvestisseurElectroniqueComplet = true;

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var model = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                model.ProfilInvestisseurElectroniqueComplet.Should().BeTrue();
            }
        }

        [TestMethod]
        public void Map_WhenFondsProtectionPrincipaleHasValues_ThenValuesMapped()
        {
            const string id = "123456";
            const string idVLN = "ABC123";
            const double rate = 0.0025D;

            InitialiserDonneesIllustration();
            _donneesIllustration.DonneesPdf.FondsBoniParticipation = new Fonds
            {
                Id = id,
                Vehicule = idVLN,
                DefaultRate = rate
            };

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<Projections>()).Returns(projection);
            projectionManager.GetMainCoverage(Arg.Any<Projection>()).Returns(projection.GetMainCoverage());

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var protectionMapper = Substitute.For<IProtectionsMapper>();
            var clientMapper = Substitute.For<IClientsMapper>();
            var hypothesesMapper = Substitute.For<IHypothesesMapper>();
            var modificationsMapper = Substitute.For<IModificationsMapper>();
            var conceptVenteMapper = Substitute.For<IConceptVenteMapper>();

            var mapper = new IllustrationModelMapper(_configurationRepository,
                projectionManager, protectionMapper, projectionMapper,
                clientMapper, hypothesesMapper, modificationsMapper, conceptVenteMapper);

            var result = mapper.Map(_donneesIllustration);
            using (new AssertionScope())
            {
                result.FondsProtectionPrincipale.Should().NotBeNull();
                result.FondsProtectionPrincipale.Id.Should().Be(id);
                result.FondsProtectionPrincipale.Vehicule.Should().Be(idVLN);
                result.FondsProtectionPrincipale.DefaultRate.Should().Be(rate);
            }
        }

        private static Individual CreerIndividual(bool isCorporation = false)
        {
            return new Individual()
            {
                IsCorporation = isCorporation,
                IsApplicant = true,
                Sex = Sex.Male,
                SequenceNumber = 1,
                Identifier = new UniqueIdentifier()
                {
                    Id = IndIndividual
                }
            };
        }

        private static DonneesClient CreerDonneesClientJeanne()
        {
            return new DonneesClient
            {
                Id = "u1",
                Sexe = Sexe.Femme,
                Prenom = "Jeanne",
                Nom = "Client",
                Initiale = "A.",
                StatutFumeur = StatutTabagisme.Fumeur,
                AgeAssurance = 42
            };
        }

        private void InitialiserDonneesIllustration()
        {
            _donneesIllustration.Agents = new List<Types.Agent>() { new Types.Agent() };
            _donneesIllustration.Projections = new Projections();
            _donneesIllustration.DonneesPdf = new DonneesPdf
            {
                ProtectionsPdf = new List<ProtectionPdf>()
                {
                    new ProtectionPdf()
                    {
                        IdProtection = "658",
                        Specification = new SpecificationProtection()
                            { IsTermCoverage = true, IsRenewable = true },
                        TermesDetails = new TermesDetails() { CoverageTerm = 33 }
                    }
                }
            };

            _configurationRepository.ObtenirConfigurationRapport(
                    Arg.Any<Produit>(),
                    Arg.Any<Etat>())
                .Returns(new ConfigurationRapport { AgeReferenceProjection = 100 });
        }

        private Projection InitialiserProjection(DateTime dateIssuance, DateTime dateMaturity)
        {
            var marginalRates = new Rates() { MarginalRates = new List<Rate>() { new Rate() { Value = 2 } } };
            var taxation = new Taxation() { Corporate = marginalRates, Personal = marginalRates };
            var parameters = new Parameters() { Taxation = taxation, Compensation = new Compensation() { AgentType = new AgentType(), BonusRate = 8 } };

            var listeCoverage = new List<Coverage>() { new Coverage { IsMain = true, PlanCode = "ABC123", InsuranceType = new VI.Projection.Data.Enums.Coverage.InsuranceType(),
                                                       Identifier = new UniqueIdentifier {Id = "658"},
                                                       Dates = new Dates() {
                                                           Issuance = dateIssuance, Maturity = dateMaturity,
                                                       },
                                                       Insured = new VI.Projection.Data.Contract.Coverage.Insured() { ExtraPremiums = new List<ExtraPremium>(),
                                                                                                                      Age = new Age() { Issuance = 16} }} };

            var listeInsured = new List<Insured> { new Insured() { IsMain = true, Coverages = listeCoverage, Identifier = new UniqueIdentifier() { Id = "658" } } };

            var projection = new Projection() { Parameters = parameters, Contract = new Contract() };
            projection.Contract.Insured = listeInsured;
            projection.Values = new List<KeyValuePair<Characteristic, double>>();
            projection.Illustration = new VI.Projection.Data.Illustration.Illustration
            {
                Columns = new List<Data<double[]>>(),
                ColumnDescriptions = new List<ColumnDescription> { new ColumnDescription { Id = 10, Attributes = new List<string> { "Type:GuaranteedRenewal" } } }
            };

            projection.Contract.FinancialSection = new FinancialSection()
            {
                LastInterestRatesUpdate = DateTime.Today
            };

            return projection;
        }

        private List<Joint> ConstruireJoints(Projection projection)
        {
            var client = CreerDonneesClientJeanne();
            client.Id = IndIndividual;
            var donneesClient = new List<DonneesClient> { client };
            _donneesIllustration.DonneesClients = donneesClient;

            var individual = new UniqueIdentifier() { Id = IndIndividual };
            var insuredIndividual = new InsuredIndividual() { Individual = individual };
            projection.Contract.Individuals = new List<Individual> { CreerIndividual() };

            var joints = new List<Joint>
            {
                new Joint()
                {
                    InsuredIndividual = insuredIndividual,
                    AverageLifeExpectancy = 1,
                    SmokerType = SmokerType.NonApplicable,
                    Age = new Age() {Issuance = 1}
                }
            };

            return joints;
        }
    }
}
