using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VI.Projection.Data;
using IAFG.IA.VI.Projection.Data.Characteristics;
using IAFG.IA.VI.Projection.Data.Contract.Billing;
using IAFG.IA.VI.Projection.Data.Extensions;
using IAFG.IA.VI.Projection.Data.Transactions;
using IAFG.IA.VI.Projection.Data.Transactions.Contract;
using EnumProjection = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration
{
    internal static class PrimesExtension
    {
        public static Primes MapperPrimes(this Primes primes, Projection projection, DateTime dateEmission,
            DonneesRapportIllustration donneesRapportIllustration)
        {
            return projection != null
                ? MapperPrimes(projection, dateEmission, donneesRapportIllustration)
                : null;
        }

        private static Primes MapperPrimes(Projection projection, DateTime dateEmission,
            DonneesRapportIllustration donneesRapportIllustration)
        {
            var primeVersees = MapperPrimesVersees(projection.Contract?.Billing,
                projection.Transactions, projection.Values, donneesRapportIllustration);

            primeVersees.AddRange(MapperPrimesVersees(projection.Transactions, projection.Values,
                dateEmission));

            return new Primes
            {
                DetailPrimes = MapperDetailPrimes(donneesRapportIllustration, projection.Values),
                PrimesVersees =
                    primeVersees
                        .GroupBy(x => new
                        {
                            x.Annee,
                            x.Duree,
                            x.FacteurMultiplicateur,
                            x.Mois,
                            x.Montant,
                            x.TypeScenarioPrime
                        }).Select(g => g.First()).OrderBy(x => x.Annee).ThenBy(x => x.Mois.GetValueOrDefault()).ToList()
            };
        }

        private static List<PrimeVersee> MapperPrimesVersees(Billing billing, Transactions transactions,
            IEnumerable<KeyValuePair<Characteristic, double>> values, DonneesRapportIllustration donnees)
        {
            var result = new List<PrimeVersee>();
            if (billing?.Premium == null) return result;
            if (PresenceScenarioPrimeCalculee(billing.Premium, transactions)) return result;

            var primeVersee = new PrimeVersee
            {
                Annee = donnees.Projections.AnneeDebutProjection,
                FacteurMultiplicateur = billing.Premium.MultiplyingFactor,
                TypeScenarioPrime = (TypeScenarioPrime)billing.Premium.Scenario,
                FrequenceFacturation = (TypeFrequenceFacturation)billing.Frequency,
                Montant = billing.Premium.Amount
            };

            GererTypeScenarioPrime(billing.Premium, values, primeVersee);
            result.Add(primeVersee);
            return result;
        }

        private static bool PresenceScenarioPrimeCalculee(Premium premium, Transactions transactions)
        {
            if (transactions?.BillingChanges == null)
            {
                return false;
            }

            return transactions.BillingChanges.Any(x => x.Billing?.Premium != null) &&
                   transactions.BillingChanges.First(x => x.Billing?.Premium != null).Billing.Premium.Scenario == premium.Scenario &&
                   premium.Scenario.EstScenarioPrimeCalculee();
        }

        private static IEnumerable<PrimeVersee> MapperPrimesVersees(Transactions transactions,
            List<KeyValuePair<Characteristic, double>> values, DateTime dateEmission)
        {
            var result = new List<PrimeVersee>();
            if (transactions?.BillingChanges == null)
            {
                return result;
            }

            result.AddRange(from transaction in transactions.BillingChanges
                where transaction.Billing?.Premium != null
                select CreerPrimeVersee(transaction, values, dateEmission));

            return result;
        }

        private static PrimeVersee CreerPrimeVersee(BillingChange transaction,
            IEnumerable<KeyValuePair<Characteristic, double>> values, DateTime dateEmission)
        {
            var annee = transaction.StartDate.CalculerAnneeContratProjection(dateEmission);
            var mois = transaction.StartDate.CalculerMoisContratProjection(dateEmission);

            var primeVersee = new PrimeVersee
            {
                Annee = annee,
                Mois = mois > 1 ? mois : (int?) null,
                FacteurMultiplicateur = transaction.Billing.Premium.MultiplyingFactor,
                TypeScenarioPrime = (TypeScenarioPrime) transaction.Billing.Premium.Scenario,
                Montant = transaction.Billing.Premium.Amount,
                FrequenceFacturation = (TypeFrequenceFacturation) transaction.Billing.Frequency
            };

            GererTypeScenarioPrime(transaction.Billing.Premium, values, primeVersee);
            return primeVersee;
        }

        private static void GererTypeScenarioPrime(Premium premium,
            IEnumerable<KeyValuePair<Characteristic, double>> values, PrimeVersee primeVersee)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (premium.Scenario)
            {
                case EnumProjection.Billing.PremiumScenario.Calculated_QuickPayment:
                case EnumProjection.Billing.PremiumScenario.Calculated_TargetedFund_Duration:
                case EnumProjection.Billing.PremiumScenario.Calculated_KeepInForce:
                case EnumProjection.Billing.PremiumScenario.Calculated_LeveledMaximum:
                    primeVersee.Montant = values.Search(EnumProjection.ValueId.OptimizedBilling);
                    primeVersee.Duree = premium.PaymentDuration;
                    break;
                case EnumProjection.Billing.PremiumScenario.Calculated_TargetedFund_Premium:
                    primeVersee.Montant = premium.Amount;
                    primeVersee.Duree = (int?) values.Search(EnumProjection.ValueId.OptimizedDuration);
                    break;
                case EnumProjection.Billing.PremiumScenario.ModalPlusAdditionalDepositOption:
                case EnumProjection.Billing.PremiumScenario.Fixed:
                    break;
                default:
                    primeVersee.Montant = null;
                    break;
            }
        }

        private static IList<DetailPrime> MapperDetailPrimes(DonneesRapportIllustration donnees,
            List<KeyValuePair<Characteristic, double>> values)
        {
            var detailPrimes = new List<DetailPrime>();
            if (values == null)
            {
                return detailPrimes;
            }

            AddPrimeMinimale(detailPrimes, donnees, values);
            AddPrimeTotale(detailPrimes, donnees, values);
            AddPrimeTotaleExcluantFrais(detailPrimes, values);
            AddPrimeReference(detailPrimes, donnees, values);
            AddFraisDePolice(detailPrimes, values);
            AddPrimeTotalePremiereAnnee(detailPrimes, values);
            AddPrimeMaximale(detailPrimes, donnees, values);
            AddContributionOptionDepotSupplementaire(detailPrimes, donnees);
            AddPrimeSelectionneePremiereAnneeProjection(detailPrimes, donnees, values);
            AddPrimeTotaleIncluantOptionDepotSupplementairePremiereAnnee(detailPrimes, donnees, values);

            return detailPrimes;
        }

        private static void AddContributionOptionDepotSupplementaire(ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees)
        {
            if (!EstParmiProduits(donnees.Produit, ProductRules.ObtenirFamilleAssuranceParticipants()))
            {
                return;
            }

            detailPrimes.Add(new DetailPrime
            {
                TypeDetailPrime = TypeDetailPrime.ContributionOptionDepotSupplementaire,
                Montant = donnees.Facturation.MontantOptionDepotSupplementaire.GetValueOrDefault(0.00)
            });
        }

        private static void AddPrimeMinimale(ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees, List<KeyValuePair<Characteristic, double>> values)
        {
            if (EstParmiProduits(donnees.Produit, ProductRules.ObtenirFamilleAssuranceParticipants()))
            {
                return;
            }

            if (!HasFees(values))
            {
                detailPrimes.Add(new DetailPrime
                {
                    TypeDetailPrime = TypeDetailPrime.PrimeMinimale,
                    Montant = donnees.EstProduitAvecPrimeReference
                        ? values.Search(EnumProjection.ValueId.SuggestedBilling) ?? 0
                        : values.GetMaxValue(EnumProjection.ValueId.SuggestedBilling,
                            EnumProjection.ValueId.TotalPremium)
                });
            }
        }

        private static void AddPrimeTotale(ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees,
            List<KeyValuePair<Characteristic, double>> values)
        {
            if (!EstParmiProduits(donnees.Produit, ProductRules.ObtenirFamilleAssuranceParticipants()))
            {
                return;
            }

            detailPrimes.Add(new DetailPrime
            {
                TypeDetailPrime = TypeDetailPrime.PrimeTotale,
                Montant = values.GetMaxValue(EnumProjection.ValueId.SuggestedBilling,
                    EnumProjection.ValueId.TotalPremium)
            });
        }

        private static void AddPrimeTotaleExcluantFrais(ICollection<DetailPrime> detailPrimes,
            List<KeyValuePair<Characteristic, double>> values)
        {
            if (HasFees(values))
            {
                detailPrimes.Add(new DetailPrime
                {
                    TypeDetailPrime = TypeDetailPrime.PrimeSansFrais,
                    Montant = values.GetMaxValue(EnumProjection.ValueId.SuggestedBillingWithoutFee,
                        EnumProjection.ValueId.TotalPremiumWithoutFee)
                });
            }
        }

        private static void AddPrimeReference(ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees, List<KeyValuePair<Characteristic, double>> values)
        {
            if (!donnees.EstProduitAvecPrimeReference)
            {
                return;
            }

            var suggestedBilling = values.Search(EnumProjection.ValueId.SuggestedBilling) ?? 0;
            var totalPremium = values.Search(EnumProjection.ValueId.TotalPremium) ?? 0;

            if (totalPremium >= suggestedBilling)
            {
                detailPrimes.Add(new DetailPrime
                {
                    TypeDetailPrime = TypeDetailPrime.PrimeReference,
                    Montant = totalPremium
                });
            }
        }

        private static void AddFraisDePolice(ICollection<DetailPrime> detailPrimes,
            List<KeyValuePair<Characteristic, double>> values)
        {
            if (HasFees(values))
            {
                detailPrimes.Add(new DetailPrime
                {
                    TypeDetailPrime = TypeDetailPrime.FraisDePolice,
                    Montant = values.Search(EnumProjection.ValueId.Fee) ?? 0
                });
            }
        }

        private static void AddPrimeTotalePremiereAnnee(ICollection<DetailPrime> detailPrimes,
            List<KeyValuePair<Characteristic, double>> values)
        {
            if (HasFees(values))
            {
                detailPrimes.Add(new DetailPrime
                {
                    TypeDetailPrime = TypeDetailPrime.PrimeAvecFrais,
                    Montant = values.Search(EnumProjection.ValueId.SuggestedBilling) ?? 0
                });
            }
        }

        private static void AddPrimeMaximale(ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees, IEnumerable<KeyValuePair<Characteristic, double>> values)
        {
            var produits = new List<Produit> { Produit.Traditionnel, Produit.Transition, Produit.AccesVie };
            produits.AddRange(ProductRules.ObtenirFamilleAssuranceParticipants());

            if (EstParmiProduits(donnees.Produit, produits))
            {
                return;
            }

            detailPrimes.Add(new DetailPrime
            {
                TypeDetailPrime = TypeDetailPrime.PrimeMaximale,
                Montant = values.Search(EnumProjection.ValueId.MaximumPremium) ?? 0
            });
        }

        private static void AddPrimeSelectionneePremiereAnneeProjection(ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees, IEnumerable<KeyValuePair<Characteristic, double>> values)
        {
            var produits = new List<Produit> { Produit.Traditionnel, Produit.Transition, Produit.AccesVie };
            produits.AddRange(ProductRules.ObtenirFamilleAssuranceParticipants());

            if (EstParmiProduits(donnees.Produit, produits))
            {
                return;
            }

            detailPrimes.Add(new DetailPrime
            {
                TypeDetailPrime = TypeDetailPrime.PrimeSelectionneePremiereAnnee,
                Montant = values.Search(EnumProjection.ValueId.ChosenBillingAmount) ?? 0
            });
        }

        private static void AddPrimeTotaleIncluantOptionDepotSupplementairePremiereAnnee(
            ICollection<DetailPrime> detailPrimes,
            DonneesRapportIllustration donnees, IEnumerable<KeyValuePair<Characteristic, double>> values)
        {
            if (!EstParmiProduits(donnees.Produit, ProductRules.ObtenirFamilleAssuranceParticipants()))
            {
                return;
            }

            detailPrimes.Add(new DetailPrime
            {
                TypeDetailPrime = TypeDetailPrime.PrimeTotaleIncluantOptionDepotSupplementairePremiereAnnee,
                Montant = values.Search(EnumProjection.ValueId.ChosenBillingAmount) ?? 0
            });
        }

        private static bool HasFees(IEnumerable<KeyValuePair<Characteristic, double>> values)
        {
            var fees = values.Search(EnumProjection.ValueId.Fee);
            return fees.HasValue && fees > 0;
        }

        private static bool EstParmiProduits(Produit produit, IEnumerable<Produit> produits)
        {
            return produits != null && produits.Contains(produit);
        }
    }
}