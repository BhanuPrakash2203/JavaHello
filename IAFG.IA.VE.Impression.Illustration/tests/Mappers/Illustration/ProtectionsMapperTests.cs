using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Business.ReglesPDF;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract;
using IAFG.IA.VI.Projection.Data.Contract.Coverage;
using IAFG.IA.VI.Projection.Data.Contract.Traditional;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Enums.Traditional;
using IAFG.IA.VI.Projection.Data.Illustration;
using IAFG.IA.VI.Projection.Data.Parameters;
using IAFG.IA.VI.Projection.Data.Parameters.Taxation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Insured = IAFG.IA.VI.Projection.Data.Contract.Insured;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.Illustration
{
    [TestClass]
    public class ProtectionsMapperTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void Map_WhenProtectionRenouvelableEtTransactionTerminaison_ThenMappingDureeRenouvellementAvecInfoTransaction()
        {
            var anneeTerminaisonDefaut = 35;
            var dureeRenouvellementExpected = anneeTerminaisonDefaut - 1;
            var projection = InitialiserProjection(new DateTime(), new DateTime());

            var data = InitialiserDonneesIllustrationAvecInformationRenouvellement(projection,
                isRenewable: true, avecCoverageTerm: true, isResiliable: true,
                avecTransactionTerminaison: true, anneeTerminaisonDefaut);

            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            var mapper = new ProtectionsMapper(null, regleAffaireAccessor);
            var result = mapper.MapperProtections(data, projection, new List<Client>());

            result.ProtectionsAssures.Single().DureeRenouvellement.Should().Be(dureeRenouvellementExpected);
        }

        [TestMethod]
        public void Map_WhenProtectionRenouvelableEtPasTransactionTerminaison_ThenMappingDureeRenouvellementAvecDureeTotalProtection()
        {
            var dureeTotalProtection = 10;
            var dateEmission = DateTime.Now;
            var dateMaturitee = dateEmission.AddYears(dureeTotalProtection);
            var projection = InitialiserProjection(dateEmission, dateMaturitee);

            var data = InitialiserDonneesIllustrationAvecInformationRenouvellement(projection,
                isRenewable: true, avecCoverageTerm: true, isResiliable: true,
                avecTransactionTerminaison: false, 0);

            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            var mapper = new ProtectionsMapper(null, regleAffaireAccessor);
            var result = mapper.MapperProtections(data, projection, new List<Client>());

            using (new AssertionScope())
            {
                var protection = result.ProtectionsAssures.Single();
                protection.DureeProtection.Should().Be(dureeTotalProtection);
                protection.DureeRenouvellement.Should().Be(dureeTotalProtection);
            }
        }

        [TestMethod]
        public void Map_WhenProtectionIsNotRenouvelable_ThenDureeRenouvellementIsNull()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.ContractType = ContractType.Universal;
            var data = InitialiserDonneesIllustrationAvecInformationRenouvellement(projection,
                isRenewable: false, avecCoverageTerm: false, isResiliable: false,
                avecTransactionTerminaison: false, 0);

            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            var mapper = new ProtectionsMapper(null, regleAffaireAccessor);
            var result = mapper.MapperProtections(data, projection, new List<Client>());

            result.ProtectionsAssures.Single().DureeRenouvellement.Should().BeNull();
        }

        [TestMethod]
        public void Map_WhenProtectionIsNotRenouvelable_ThenDureeRenouvellementIsNullTrad()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.ContractType = ContractType.Traditionnal;
            var data = InitialiserDonneesIllustrationAvecInformationRenouvellement(projection,
                isRenewable: false, avecCoverageTerm: false, isResiliable: false,
                avecTransactionTerminaison: false, 0);

            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            var mapper = new ProtectionsMapper(null, regleAffaireAccessor);
            var result = mapper.MapperProtections(data, projection, new List<Client>());

            result.ProtectionsAssures.Single().DureeRenouvellement.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(true, true, true)]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(false, false, false)]
        public void Map_WhenProtectionAvecInformationRenouvellement_ThenBonMappingExisteProtectionTemporaireRenouvelable(bool isRenewable, bool avecCoverageTerm, bool renouvellementIllustreExpected)
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var data = InitialiserDonneesIllustrationAvecInformationRenouvellement(projection, isRenewable, avecCoverageTerm, false, false, 0);
            
            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            var mapper = new ProtectionsMapper(null, regleAffaireAccessor);

            var result = mapper.MapperProtections(data, projection, new List<Client>());
            result.ExisteProtectionTemporaireRenouvelable.Should().Be(renouvellementIllustreExpected);
        }

        [TestMethod]
        public void Map_WhenProtectionEtPasInformationProtectionsPdfDisponible_ThenThrowsArgumentNullException()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var data = InitialiserDonneesIllustrationAvecInformationRenouvellement(projection,
                isRenewable: false, avecCoverageTerm: false, isResiliable: false,
                avecTransactionTerminaison: false, 0);
            data.DonneesPdf.ProtectionsPdf = new List<ProtectionPdf>();

            var regleAffaireAccessor = Substitute.For<IRegleAffaireAccessor>();
            var mapper = new ProtectionsMapper(null, regleAffaireAccessor);
 
            Action act = () => mapper.MapperProtections(data, projection, new List<Client>());
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("ProtectionsPdf");
        }

        [TestMethod]
        public void Map_WithTraditionalOptions_ThenMapParticipations()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.TraditionalOptions = new Options { DividendOption = DividendOption.PaidUpAddition };

            var mapper = new ProtectionsMapper(null, null);
            var result = mapper.MapParticipations(projection, new ParametreRapport());
            
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.OptionParticipation.Should().Be((TypeOptionParticipation)projection.Contract.TraditionalOptions.DividendOption);
                result.ReductionBaremeParticipation.Should().BeNull();
            }
        }

        [TestMethod]
        public void Map_WithTraditionalOptionsAndBalanceOfDividendsOnDepositAccount_ThenMapParticipations()
        {
            const double balance = 200.25D;
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Contract.TraditionalOptions = new Options
            {
                DividendOption = DividendOption.PaidUpAddition,
                Dividends = new Dividends{ BalanceOfDividendsOnDepositAccount = balance}
            };

            var mapper = new ProtectionsMapper(null, null);
            var result = mapper.MapParticipations(projection, new ParametreRapport());

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.OptionParticipation.Should().Be((TypeOptionParticipation)projection.Contract.TraditionalOptions.DividendOption);
                result.SoldeParticipationsEnDepot.Should().Be(balance);
            }
        }

        [TestMethod]
        public void EstEclipseDePrimeActivee_WHEN_ValuePremiumOffsetYearMissing_ShouldBeFalse()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var mapper = new PrivateType(typeof(ProtectionsMapper));
            var result = (bool)mapper.InvokeStatic("EstEclipseDePrimeActivee", projection);
            result.Should().BeFalse();
        }

        [TestMethod]
        public void EstEclipseDePrimeActivee_WHEN_VecteurColumn2022IsNull_ShouldBeFalse()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Values.Add(new KeyValuePair<Characteristic, double>(new Characteristic() { Id = (int)ValueId.PremiumOffSetYear, Flag = CharacteristicFlag.Contract }, 10));

            var mapper = new PrivateType(typeof(ProtectionsMapper));
            var result = (bool)mapper.InvokeStatic("EstEclipseDePrimeActivee", projection);
            result.Should().BeFalse();
        }

        [TestMethod]
        public void EstEclipseDePrimeActivee_WHEN_ValuePremiumOffsetYearHasValue_AND_VecteurColumn2022IsNotEmpty_ShouldBeTrue()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Values.Add(new KeyValuePair<Characteristic, double>(new Characteristic() { Id = (int)ValueId.PremiumOffSetYear, Flag = CharacteristicFlag.Contract }, 10));
            projection.Illustration.Columns.Add(new Data<double[]>() { Id = 2022, Value = Enumerable.Repeat(57.4, 70).ToArray() });
            
            var mapper = new PrivateType(typeof(ProtectionsMapper));
            var result = (bool)mapper.InvokeStatic("EstEclipseDePrimeActivee", projection);
            result.Should().BeTrue();
        }

        [TestMethod]
        public void EstChangementOptionParticipationActivee_WHEN_CashDividendOptionChangeYearMissing_ShouldBeFalse()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            var mapper = new PrivateType(typeof(ProtectionsMapper));
            var result = (bool)mapper.InvokeStatic("EstChangementOptionParticipationActivee", projection);
            result.Should().BeFalse();
        }
      
        [TestMethod]
        public void EstChangementOptionParticipationActivee_WHEN_CashDividendOptionChangeYearHasValue_ShouldBeTrue()
        {
            var projection = InitialiserProjection(new DateTime(), new DateTime());
            projection.Values.Add(new KeyValuePair<Characteristic, double>(new Characteristic() { Id = (int)ValueId.CashDividendOptionChangeYear, Flag = CharacteristicFlag.Contract }, 10));          
            var mapper = new PrivateType(typeof(ProtectionsMapper));
            var result = (bool)mapper.InvokeStatic("EstChangementOptionParticipationActivee", projection);
            result.Should().BeTrue();
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

        private DonneesIllustration InitialiserDonneesIllustrationAvecInformationRenouvellement(Projection projection, 
            bool isRenewable, bool avecCoverageTerm, bool isResiliable, bool avecTransactionTerminaison, 
            int anneeTerminaisonDefaut)
        {
            var result = new DonneesIllustration();
            byte? coverageTerm = null;
            if (avecCoverageTerm) coverageTerm = 33;

            if (avecTransactionTerminaison)
            {
                projection.Transactions = new VI.Projection.Data.Transactions.Transactions
                {
                    Terminations = new List<VI.Projection.Data.Transactions.Coverage.Termination> { new VI.Projection.Data.Transactions.Coverage.Termination { CoverageIdentifier = new UniqueIdentifier { Id = "658" }, StartDate = new GenericDate { Year = anneeTerminaisonDefaut } } }
                };
            }

            result.DonneesPdf = new DonneesPdf
            {
                ProtectionsPdf = new List<ProtectionPdf>() {
                    new ProtectionPdf() {
                        IdProtection = "658",
                        Specification = new SpecificationProtection() {IsRenewable = isRenewable, IsResiliable = isResiliable },
                        TermesDetails = new TermesDetails() { CoverageTerm = coverageTerm } }
                }
            };

            return result;
        }

    }
}
