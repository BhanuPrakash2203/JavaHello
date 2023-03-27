using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IAFG.IA.VI.Projection.Data.DataClient;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Illustration;
using IAFG.IA.VI.Projection.Data.Parameters;
using IAFG.IA.VI.Projection.Data.Parameters.Taxation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Insured = IAFG.IA.VI.Projection.Data.Contract.Insured;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.Illustration
{
    [TestClass]
    public class ClientsMapperTests
    {
        private const string IndIndividual = "IdIndivid";

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void TestMappageClients_Vide()
        {
            var projection = new Projection();
            var mapper = new ClientsMapper();
            var result = mapper.MapClients(null, projection);
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void TestMappageClients()
        {
            var donneesClients = new List<DonneesClient>
            {
                    CreerDonneesClientJeanne()
            };

            var projection = new Projection();
            var mapper = new ClientsMapper();
            var result = mapper.MapClients(donneesClients, projection);

            using (new AssertionScope())
            {
                result.Should().ContainSingle();
            }
        }

        [TestMethod]
        public void TestMappageClientsAvecDataClient()
        {
            var donneesClients = new List<DonneesClient>
            {
                CreerDonneesClientJeanne()
            };

            var projection = new Projection
            {
                DataClients = new DataClients { List = new List<DataClient> { CreerDataClientJeanne() } }
            };

            var mapper = new ClientsMapper();
            var result = mapper.MapClients(donneesClients, projection);

            result.Should().ContainSingle();
        }

        [TestMethod]
        public void TestMappageClientsAvecIndividu()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = ConstruireJoints(projection);

            var mapper = new ClientsMapper();
            var result = mapper.MapClients(new List<DonneesClient>(), projection);

            result.Should().ContainSingle();
        }

        [TestMethod]
        public void TestMappageClientsAvecDeuxIndividus()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.Individuals = new List<Individual>
            {
                new Individual
                {
                    Identifier = new UniqueIdentifier()
                    {
                        Id = "u1"
                    },
                    Birthdate = new DateTime(1976, 01, 05),
                    IsApplicant = true,
                    SequenceNumber = 1,
                    Sex = Sex.Female
                },
                new Individual
                {
                    Identifier = new UniqueIdentifier()
                    {
                        Id = "u2"
                    },
                    Birthdate = new DateTime(1972, 02, 10),
                    IsApplicant = true,
                    SequenceNumber = 1,
                    Sex = Sex.Male
                }
            };

            var mapper = new ClientsMapper();
            var result = mapper.MapClients(new List<DonneesClient>(), projection);

            using (new AssertionScope())
            {
                result.Should().ContainSingle(x => x.ReferenceExterneId == "u1");
                result.Should().ContainSingle(x => x.ReferenceExterneId == "u2");
            }
        }

        [TestMethod]
        public void Map_WhenMapIsNotAssurableDeDonneesIllustrationFalse_ThenIsNotAssurableMapperFalse()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var joints = ConstruireJoints(projection);
            joints.First().IsNotInsurable = false;
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = joints;

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<VI.Projection.Data.Projections>()).Returns(projection);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var mapper = new ClientsMapper();
            var result = mapper.MapClients(new List<DonneesClient>(), projection);

            using (new AssertionScope())
            {
                result.First().IsNotAssurable.Should().Be(projection.Contract.Insured[0].Coverages[0].Insured.Joints[0].IsNotInsurable);
                result.First().IsNotAssurable.Should().BeFalse();
            }
        }
        
        [TestMethod]
        public void Map_WhenMapIsNotAssurableDeDonneesIllustrationTrue_ThenIsNotAssurableMapperTrue()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var joints = ConstruireJoints(projection);
            joints.First().IsNotInsurable = true;
            projection.Contract.Insured[0].Coverages[0].Insured.Joints = joints;

            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<VI.Projection.Data.Projections>()).Returns(projection);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var mapper = new ClientsMapper();
            var result = mapper.MapClients(new List<DonneesClient>(), projection);

            using (new AssertionScope())
            {
                result.First().IsNotAssurable.Should().Be(projection.Contract.Insured[0].Coverages[0].Insured.Joints[0].IsNotInsurable);
                result.First().IsNotAssurable.Should().BeTrue();
            }
        }

        [TestMethod]
        public void Map_WhenMapJointIsNull_ThenIsNotAssurableMapperNull()
        {
            var client = CreerDonneesClientJeanne();
            client.Id = IndIndividual;
            var donneesClient = new List<DonneesClient> { client };

            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var projectionManager = Substitute.For<IProjectionManager>();
            projectionManager.GetDefaultProjection(Arg.Any<VI.Projection.Data.Projections>()).Returns(projection);

            var projectionMapper = Substitute.For<IProjectionsMapper>();
            projectionMapper.Map(default, default, default).ReturnsForAnyArgs(
                new Types.Models.Projections.Projections { Projection = new Types.Models.Projections.Projection() });

            var mapper = new ClientsMapper();
            var result = mapper.MapClients(donneesClient, projection);
            result.First().IsNotAssurable.Should().BeNull();
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

        private static DataClient CreerDataClientJeanne()
        {
            return new DataClient
            {
                Identifier = new UniqueIdentifier { Id = "u1" },
                Sex = Sex.Female,
                FirstName = "Jeanne",
                LastName = "Client",
                Initial = "A.",
                InsuranceAge = 42
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

            return projection;
        }
        
        private List<Joint> ConstruireJoints(Projection projection)
        {
            var client = CreerDonneesClientJeanne();
            client.Id = IndIndividual;

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
