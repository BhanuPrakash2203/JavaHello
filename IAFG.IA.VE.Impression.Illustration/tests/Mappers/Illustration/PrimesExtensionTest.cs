using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Test.TestBuilders;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract.Billing;
using IAFG.IA.VI.Projection.Data.Enums;
using IAFG.IA.VI.Projection.Data.Enums.Billing;
using IAFG.IA.VI.Projection.Data.Transactions;
using IAFG.IA.VI.Projection.Data.Transactions.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Mappers.Illustration
{
    [TestClass]
    public class PrimesExtensionTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private DonneesRapportIllustrationTestBuilder _donneesTestBuilder;

        [TestInitialize]
        public void Setup()
        {
            _donneesTestBuilder = new DonneesRapportIllustrationTestBuilder();
        }

        [TestMethod]
        public void MapperPrimes_WhenDonneesIllustrationIsNull_ThenReturnNull()
        {
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var result = new Primes().MapperPrimes(null, DateTime.Now, donneesRapportIllustration);
            result.Should().BeNull();
        }

        [TestMethod]
        public void MapperPrimes_WhenProjectionIsNull_ThenReturnNull()
        {
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(null, DateTime.Now, donneesRapportIllustration);
            primes.Should().BeNull();
        }

        [TestMethod]
        public void MapperPrimes_WhenPremiumScenarioIsOptimisation_ThenMontantIsOptimizedBilling()
        {
            var expectedMontant = 10.1;
            var values = new List<KeyValuePair<Characteristic, double>>
            {
                new KeyValuePair<Characteristic, double>(
                            new Characteristic
                            {
                                Id = (int) ValueId.OptimizedBilling,
                                Flag = CharacteristicFlag.Contract
                            } , expectedMontant)
            };
            var scenarios = new List<PremiumScenario>
            {
                PremiumScenario.Calculated_QuickPayment,
                PremiumScenario.Calculated_TargetedFund_Duration,
                PremiumScenario.Calculated_KeepInForce,
                PremiumScenario.Calculated_LeveledMaximum
            };

            foreach (var scenario in scenarios)
            {
                var expectedPrime = new PrimeVersee
                {
                    Annee = 1,
                    Duree = 10,
                    FacteurMultiplicateur = 0,
                    Mois = null,
                    Montant = expectedMontant,
                    TypeScenarioPrime = (TypeScenarioPrime)scenario
                };

                var projection = CreateProjectionForGererTypeScenarioPrime(scenario, values, 0, 10);
                var donnees = _donneesTestBuilder.WithProduit(Produit.CapitalValeur).WithFrequenceFacturation(TypeFrequenceFacturation.Annuelle).WithAnneeDebutProjection(1).Build();
                var result = new Primes().MapperPrimes(projection, DateTime.Now, donnees);
                result.PrimesVersees.Should().BeEquivalentTo(expectedPrime);
            }
        }

        [TestMethod]
        public void MapperPrimes_WhenPremiumScenarioIsCalculatedTargetedFundPremium_ThenMontantIsPremiumAmount()
        {
            var expectedMontant = 10.1;
            var values = new List<KeyValuePair<Characteristic, double>>
            {
                new KeyValuePair<Characteristic, double>(
                            new Characteristic
                            {
                                Id = (int) ValueId.OptimizedDuration,
                                Flag = CharacteristicFlag.Contract
                            } , 10)
            };
            var expectedPrime = new PrimeVersee
            {
                Annee = 1,
                Duree = 10,
                FacteurMultiplicateur = 0,
                Mois = null,
                Montant = expectedMontant,
                TypeScenarioPrime = (TypeScenarioPrime)PremiumScenario.Calculated_TargetedFund_Premium
            };

            var projection = CreateProjectionForGererTypeScenarioPrime(PremiumScenario.Calculated_TargetedFund_Premium, values, expectedMontant, 0);
            var donnees = _donneesTestBuilder.WithProduit(Produit.CapitalValeur).WithFrequenceFacturation(TypeFrequenceFacturation.Annuelle).WithAnneeDebutProjection(1).Build();
            var result = new Primes().MapperPrimes(projection, DateTime.Now, donnees);
            result.PrimesVersees.Should().BeEquivalentTo(expectedPrime);
        }

        [TestMethod]
        public void MapperPrimes_WithBillingChanges_ThenPrimesVerseesValid()
        {
            var billingChanges = new List<BillingChange>()
            {
                new BillingChange()
                {
                    Billing = new Billing()
                    {
                        Frequency = Frequency.Annual,
                        Premium = new Premium()
                    },
                    StartDate = new GenericDate()
                    {
                        DateType = DateType.Contract,
                        Month = 5,
                        Year = 10
                    }
                },
                new BillingChange()
                {
                    Billing = new Billing()
                    {
                        Frequency = Frequency.Monthly,
                        Premium = new Premium()
                    },
                    StartDate = new GenericDate()
                    {
                        DateType = DateType.Contract,
                        Month = 1,
                        Year = 15
                    }
                }
            };

            var donnees = _donneesTestBuilder.WithProduit(Produit.CapitalValeur).WithFrequenceFacturation(TypeFrequenceFacturation.Annuelle).WithAnneeDebutProjection(1).Build();
            var projection = CreateProjection(null, billingChanges);
            var result = new Primes().MapperPrimes(projection, DateTime.Now, donnees);

            using (new AssertionScope())
            {
                result.PrimesVersees.Should().NotBeNullOrEmpty();
                result.PrimesVersees.Should().HaveCount(2);
                result.PrimesVersees.Any(p => p.FrequenceFacturation == TypeFrequenceFacturation.Annuelle).Should().BeTrue();
                result.PrimesVersees.Any(p => p.FrequenceFacturation == TypeFrequenceFacturation.Mensuelle).Should().BeTrue();
            }
        }

        [TestMethod]
        public void MapperPrimes_WhenValuesIsNull_ThenReturnEmptyList()
        {
            var donnees = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(new Projection(), DateTime.Now, donnees);
            primes.DetailPrimes.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(Produit.Traditionnel)]
        [DataRow(Produit.Transition)]
        [DataRow(Produit.AccesVie)]
        [DataRow(Produit.Genesis)]
        [DataRow(Produit.CapitalValeur)]
        [DataRow(Produit.RegimeVieMD)]
        [DataRow(Produit.Tendance)]
        [DataRow(Produit.Topaz)]
        public void MapperPrimes_WhenIsNotSupportedProduit_ThenDoNotDisplayContributionOptionDepotSupplementaire(Produit produit)
        {
            var projection = CreateProjection(null);
            var donneesRapport = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapport);
            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.ContributionOptionDepotSupplementaire);
        }

        [TestMethod]
        [DataRow(Produit.AssuranceParticipant)]
        public void MapperPrimes_WhenIsSupportedProduit_ThenDisplayContributionOptionDepotSupplementaire(Produit produit)
        {
            var projection = CreateProjection(null);
            var donneesRapport = _donneesTestBuilder.WithProduit(produit).Build();
            donneesRapport.Facturation.MontantOptionDepotSupplementaire = 25.25;

            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapport);
            primes.DetailPrimes.Should().Contain(x => x.TypeDetailPrime == TypeDetailPrime.ContributionOptionDepotSupplementaire);
        }

        [TestMethod]
        [DataRow(Produit.Traditionnel)]
        [DataRow(Produit.Transition)]
        [DataRow(Produit.AccesVie)]
        [DataRow(Produit.Genesis)]
        [DataRow(Produit.CapitalValeur)]
        [DataRow(Produit.RegimeVieMD)]
        [DataRow(Produit.Tendance)]
        [DataRow(Produit.Topaz)]
        public void MapperPrimes_WhenIsNotSupportedProduit_ThenDoNotDisplayPrimeTotale(Produit produit)
        {
            var projection = CreateProjection(null);
            var donneesRapport = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapport);
            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeTotale);
        }

        [TestMethod]
        [DataRow(Produit.AssuranceParticipant)]
        public void MapperPrimes_WhenIsSupportedProduit_ThenDisplayPrimeTotale(Produit produit)
        {
            var projection = CreateProjection(null);
            var donneesRapport = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapport);
            primes.DetailPrimes.Should().Contain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeTotale);
        }

        [TestMethod]
        [DataRow(Produit.Traditionnel)]
        [DataRow(Produit.Transition)]
        [DataRow(Produit.AccesVie)]
        [DataRow(Produit.Genesis)]
        [DataRow(Produit.CapitalValeur)]
        [DataRow(Produit.RegimeVieMD)]
        [DataRow(Produit.Tendance)]
        [DataRow(Produit.Topaz)]
        public void MapperPrimes_WhenIsNotSupportedProduit_ThenDoNotDisplayPrimeTotaleIncluantOptionDepotSupplementairePremiereAnnee(Produit produit)
        {
            var projection = CreateProjection(null);
            var donneesRapport = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapport);
            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeTotaleIncluantOptionDepotSupplementairePremiereAnnee);
        }

        [TestMethod]
        [DataRow(Produit.AssuranceParticipant)]
        public void MapperPrimes_WhenIsSupportedProduit_ThenDisplayPrimeTotaleIncluantOptionDepotSupplementairePremiereAnneee(Produit produit)
        {
            var projection = CreateProjection(null);
            var donneesRapport = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapport);
            primes.DetailPrimes.Should().Contain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeTotaleIncluantOptionDepotSupplementairePremiereAnnee);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasNoFeeAndEstProduitAvecPrimeReference_ThenDisplayPrimeMinimaleWithSuggestedBilling()
        {
            var montantSuggestedBilling = Auto.Create<double>();
            var fee = CreateProjectionValue(ValueId.Fee, 0);
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, montantSuggestedBilling);
            var projection = CreateProjection(new[] { fee, suggestedBilling });
            var donnees = _donneesTestBuilder.WithEstProduitAvecPrimeReference(true).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donnees);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMinimale);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMinimale).Montant.Should().Be(montantSuggestedBilling);
        }

        [TestMethod]
        [DataRow(2, 1)]
        [DataRow(1, 2)]
        public void MapperPrimes_WhenHasNoFeeAndNotEstProduitAvecPrimeReference_ThenDisplayPrimeMinimaleWithAppropriateMontant(double montantSuggestedBilling, double montantTotalPremium)
        {
            var fee = CreateProjectionValue(ValueId.Fee, 0);
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, montantSuggestedBilling);
            var totalPremium = CreateProjectionValue(ValueId.TotalPremium, montantTotalPremium);
            var projection = CreateProjection(new[] { fee, suggestedBilling, totalPremium });
            var donnees = _donneesTestBuilder.WithEstProduitAvecPrimeReference(false).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donnees);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMinimale);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMinimale).Montant.Should().Be(Math.Max(montantSuggestedBilling, montantTotalPremium));
        }

        [TestMethod]
        public void MapperPrimes_WhenHasFees_ThenDoNotDisplayPrimeMinimale()
        {
            var fee = CreateProjectionValue(ValueId.Fee, Auto.Create<double>());
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, Auto.Create<double>());
            var totalPremium = CreateProjectionValue(ValueId.TotalPremium, Auto.Create<double>());
            var projection = CreateProjection(new[] { fee, suggestedBilling, totalPremium });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMinimale);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasFees_ThenDisplayPrimeTotaleExcluantFraisWithSuggestedBillingWithoutFee()
        {
            MapperPrimesWhenHasFeesThenDisplayPrimeTotaleExcluantFraisWithCorrectMontant(2, 1);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasFees_ThenDisplayPrimeTotaleExcluantFraisWithTotalPremiumWithoutFee()
        {
            MapperPrimesWhenHasFeesThenDisplayPrimeTotaleExcluantFraisWithCorrectMontant(1, 2);
        }

        private void MapperPrimesWhenHasFeesThenDisplayPrimeTotaleExcluantFraisWithCorrectMontant(double montantSuggestedBillingWithoutFee, double montantTotalPremiumWithoutFee)
        {
            var fee = CreateProjectionValue(ValueId.Fee, Auto.Create<double>());
            var suggestedBillingWithoutFee = CreateProjectionValue(ValueId.SuggestedBillingWithoutFee, montantSuggestedBillingWithoutFee);
            var totalPremiumWithoutFee = CreateProjectionValue(ValueId.TotalPremiumWithoutFee, montantTotalPremiumWithoutFee);
            var projection = CreateProjection(new[] { fee, suggestedBillingWithoutFee, totalPremiumWithoutFee });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeSansFrais);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeSansFrais).Montant.Should().Be(Math.Max(montantSuggestedBillingWithoutFee, montantTotalPremiumWithoutFee));
        }

        [TestMethod]
        public void MapperPrimes_WhenHasNoFee_ThenDoNotDisplayPrimeTotaleExcluantFrais()
        {
            var fee = CreateProjectionValue(ValueId.Fee, 0);
            var suggestedBillingWithoutFee = CreateProjectionValue(ValueId.SuggestedBillingWithoutFee, Auto.Create<double>());
            var totalPremiumWithoutFee = CreateProjectionValue(ValueId.TotalPremiumWithoutFee, Auto.Create<double>());
            var projection = CreateProjection(new[] { fee, suggestedBillingWithoutFee, totalPremiumWithoutFee });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeSansFrais);
        }

        [TestMethod]
        public void MapperPrimes_WhenEstProduitAvecPrimeReferenceAndPrimeReferenceIsGreaterOrEqualThanPrimeMinimale_ThenDisplayPrimeReference()
        {
            var montant = Auto.Create<double>();
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, montant - 1);
            var totalPremium = CreateProjectionValue(ValueId.TotalPremium, montant);
            var projection = CreateProjection(new[] { suggestedBilling, totalPremium });
            var donneesRapportIllustration = _donneesTestBuilder.WithEstProduitAvecPrimeReference(true).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeReference);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeReference).Montant.Should().Be(montant);
        }

        [TestMethod]
        public void MapperPrimes_WhenEstProduitAvecPrimeReferenceAndPrimeReferenceIsLowerThanPrimeMinimale_ThenDoNotDisplayPrimeReference()
        {
            var montant = Auto.Create<double>();
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, montant + 1);
            var totalPremium = CreateProjectionValue(ValueId.TotalPremium, montant);
            var projection = CreateProjection(new[] { suggestedBilling, totalPremium });
            var donneesRapportIllustration = _donneesTestBuilder.WithEstProduitAvecPrimeReference(true).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeReference);
        }

        [TestMethod]
        public void MapperPrimes_WhenNotEstProduitAvecPrimeReference_ThenDoNotDisplayPrimeReference()
        {
            var totalPremium = CreateProjectionValue(ValueId.TotalPremium, Auto.Create<double>());
            var projection = CreateProjection(new[] { totalPremium });
            var donneesRapportIllustration = _donneesTestBuilder.WithEstProduitAvecPrimeReference(false).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeReference);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasFees_ThenDisplayFrais()
        {
            var montant = Auto.Create<double>();
            var fee = CreateProjectionValue(ValueId.Fee, montant);
            var projection = CreateProjection(new[] { fee });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.FraisDePolice);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.FraisDePolice).Montant.Should().Be(montant);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasNoFee_ThenDoNotDisplayFrais()
        {
            var fee = CreateProjectionValue(ValueId.Fee, 0);
            var projection = CreateProjection(new[] { fee });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.FraisDePolice);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasFees_ThenDisplayPrimeTotalePremiereAnnee()
        {
            var montant = Auto.Create<double>();
            var fee = CreateProjectionValue(ValueId.Fee, Auto.Create<double>());
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, montant);
            var projection = CreateProjection(new[] { fee, suggestedBilling });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeAvecFrais);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeAvecFrais).Montant.Should().Be(montant);
        }

        [TestMethod]
        public void MapperPrimes_WhenHasNoFee_ThenDoNotDisplayPrimeTotalePremiereAnnee()
        {
            var fee = CreateProjectionValue(ValueId.Fee, 0);
            var suggestedBilling = CreateProjectionValue(ValueId.SuggestedBilling, Auto.Create<double>());
            var projection = CreateProjection(new[] { fee, suggestedBilling });
            var donneesRapportIllustration = _donneesTestBuilder.Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeAvecFrais);
        }

        [TestMethod]
        [DataRow(Produit.CapitalValeur)]
        [DataRow(Produit.Genesis)]
        [DataRow(Produit.RegimeVieMD)]
        [DataRow(Produit.Tendance)]
        [DataRow(Produit.Meridia)]
        [DataRow(Produit.Topaz)]
        public void MapperPrimes_WhenIsSupportedProduit_ThenDisplayPrimeMaximale(Produit produit)
        {
            var montant = Auto.Create<double>();
            var maximumPremium = CreateProjectionValue(ValueId.MaximumPremium, montant);
            var projection = CreateProjection(new[] { maximumPremium });
            var donneesRapportIllustration = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMaximale);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMaximale).Montant.Should().Be(montant);
        }

        [TestMethod]
        [DataRow(Produit.Traditionnel)]
        [DataRow(Produit.Transition)]
        [DataRow(Produit.AccesVie)]
        [DataRow(Produit.AssuranceParticipant)]
        public void MapperPrimes_WhenIsNotSupportedProduit_ThenDoNotDisplayPrimeMaximale(Produit produit)
        {
            var maximumPremium = CreateProjectionValue(ValueId.MaximumPremium, Auto.Create<double>());
            var projection = CreateProjection(new[] { maximumPremium });
            var donneesRapportIllustration = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeMaximale);
        }

        [TestMethod]
        [DataRow(Produit.CapitalValeur)]
        [DataRow(Produit.Genesis)]
        [DataRow(Produit.RegimeVieMD)]
        [DataRow(Produit.Tendance)]
        [DataRow(Produit.Meridia)]
        [DataRow(Produit.Topaz)]
        public void MapperPrimes_WhenIsSupportedProduit_ThenDisplayPrimeSelectionneePremiereAnneeProjection(Produit produit)
        {
            var montant = Auto.Create<double>();
            var chosenBillingAmount = CreateProjectionValue(ValueId.ChosenBillingAmount, montant);
            var projection = CreateProjection(new[] { chosenBillingAmount });
            var donneesRapportIllustration = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().ContainSingle(x => x.TypeDetailPrime == TypeDetailPrime.PrimeSelectionneePremiereAnnee);
            primes.DetailPrimes.Single(x => x.TypeDetailPrime == TypeDetailPrime.PrimeSelectionneePremiereAnnee).Montant.Should().Be(montant);
        }

        [TestMethod]
        [DataRow(Produit.Traditionnel)]
        [DataRow(Produit.Transition)]
        [DataRow(Produit.AccesVie)]
        [DataRow(Produit.AssuranceParticipant)]
        public void MapperPrimes_WhenIsNotSupportedProduit_ThenDoNotDisplayPrimeSelectionneePremiereAnneeProjection(Produit produit)
        {
            var chosenBillingAmount = CreateProjectionValue(ValueId.ChosenBillingAmount, Auto.Create<double>());
            var projection = CreateProjection(new[] { chosenBillingAmount });
            var donneesRapportIllustration = _donneesTestBuilder.WithProduit(produit).Build();
            var primes = new Primes().MapperPrimes(projection, DateTime.Now, donneesRapportIllustration);

            primes.DetailPrimes.Should().NotContain(x => x.TypeDetailPrime == TypeDetailPrime.PrimeSelectionneePremiereAnnee);
        }

        private static Projection CreateProjection(
            KeyValuePair<Characteristic, double>[] projectionValues)
        {
            var billingChange = new BillingChange() { Billing = new Billing() { Premium = new Premium() } };
            return CreateProjection(projectionValues, new List<BillingChange>() { billingChange });
        }

        private static Projection CreateProjection(
            KeyValuePair<Characteristic, double>[] projectionValues,
            List<BillingChange> billingChanges)
        {
            return new Projection
            {
                Values = new List<KeyValuePair<Characteristic, double>>(projectionValues ?? new KeyValuePair<Characteristic, double>[] { }),
                Transactions = new Transactions
                {
                    BillingChanges = billingChanges
                }
            };
        }

        private static KeyValuePair<Characteristic, double> CreateProjectionValue(ValueId valueId, double montant)
        {
            return new KeyValuePair<Characteristic, double>(
                new Characteristic
                {
                    Id = (int)valueId,
                    Flag = CharacteristicFlag.Contract
                },
                montant);
        }

        private static Projection CreateProjectionForGererTypeScenarioPrime(
            PremiumScenario premiumScenario,
            List<KeyValuePair<Characteristic, double>> values, double amount, int paymentDuration)
        {
            return new Projection
            {
                Values = values,
                Contract = new VI.Projection.Data.Contract.Contract
                {
                    Billing = new Billing
                    {
                        Premium = new Premium
                        {
                            Amount = amount,
                            Scenario = premiumScenario,
                            PaymentDuration = paymentDuration
                        }
                    }
                }
            };
        }
    }
}
