using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Helper;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.FluxMonetaire;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VI.Projection.Data.Enums;
using Language = IAFG.IA.VE.Impression.Core.Types.Enums.Language;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections
{
    public class SommaireProtectionsModelFactory : ISommaireProtectionsModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IProductRules _productRules;
        private readonly IUsageAuConseillerModelBuilder _usageAuConseillerModelBuilder;
        private readonly IAssuranceSupplementaireLibereeModelBuilder _assuranceSupplementaireLibereeModelBuilder;

        public SommaireProtectionsModelFactory(IConfigurationRepository configurationRepository, 
            IIllustrationReportDataFormatter formatter, 
            ISectionModelMapper sectionModelMapper,
            IProductRules productRules,
            IUsageAuConseillerModelBuilder usageAuConseillerModelBuilder, 
            IAssuranceSupplementaireLibereeModelBuilder assuranceSupplementaireLibereeModelBuilder)
        {
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _sectionModelMapper = sectionModelMapper;
            _productRules = productRules;
            _usageAuConseillerModelBuilder = usageAuConseillerModelBuilder;
            _assuranceSupplementaireLibereeModelBuilder = assuranceSupplementaireLibereeModelBuilder;
        }

        public SectionSommaireProtectionsModel Build(string sectionId, DonneesRapportIllustration donnees, IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SectionSommaireProtectionsModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.SectionIdendification = MapperIdendifications(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Identifications"), donnees, context);
            model.SectionProtections = MapperProtections(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Protections"), donnees, context);
            model.SectionPrimes = MapperPrimes(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Primes"), donnees, context);
            model.SectionAsl = MapperAssuranceSupplementaireLiberee(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "ASL"), donnees, context);
            model.SectionFluxMonetaire = MapperFluxMonetaire(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "FluxMonetaire"), donnees, context);
            model.SectionSurprimes = MapperSurprimes(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Surprimes"), donnees, context);
            model.SectionDetailParticipations = MapperDetailParticipations(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "DetailParticipations"), donnees, context);
            model.SectionEclipseDePrime = MapperEclipseDePrime(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "DetailEclipseDePrime"), donnees, context);
            model.SectionScenarioParticipations = MapperScenarioParticipations(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "DetailScenarios"), donnees, context);
            model.SectionAvancesSurPolice = MapperAvancesSurPolice(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "AvancesSurPolice"), donnees, context);
            model.SectionUsageAuConseiller = MapperUsageAuConseiller(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "UsageDuConseiller"), donnees, context);

            return model;
        }

        private SectionUsageAuConseillerModel MapperUsageAuConseiller(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            return _usageAuConseillerModelBuilder.Build(definition, donnees, context);
        }

        private SectionAvancesSurPoliceModel MapperAvancesSurPolice(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            if (donnees.AvancesSurPolice == null)
            {
                return null;
            }

            var section = new SectionAvancesSurPoliceModel()
            {
                Solde = donnees.AvancesSurPolice.Solde,
                DateDerniereMiseAJour = donnees.AvancesSurPolice.DateDerniereMiseAJour
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionDetailEclipseDePrimeModel MapperEclipseDePrime(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            if (donnees.Participations?.Baremes == null || !donnees.Participations.Baremes.Any())
            {
                return null;
            }

            var section = new SectionDetailEclipseDePrimeModel()
            {
                Baremes = donnees.Participations.Baremes.Select(item => new Bareme() { Annee = item.Annee, Diminution = item.Diminution }).ToList()
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionDetailParticipationsModel MapperDetailParticipations(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            if (donnees.Participations == null ||
                donnees.Participations.OptionParticipation == TypeOptionParticipation.NonSpecifie)
            {
                return null;
            }

            var section = new SectionDetailParticipationsModel
            {
                OptionParticipation = donnees.Participations.OptionParticipation,
                SoldeParticipationsEnDepot = donnees.Participations.SoldeParticipationsEnDepot
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionScenarioParticipationsModel MapperScenarioParticipations(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            if (donnees.Participations?.ReductionBaremeParticipation == null)
            {
                return null;
            }

            var section = new SectionScenarioParticipationsModel
            {
                EcartBaremeParticipation = donnees.Participations.ReductionBaremeParticipation
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionIdendificationModel MapperIdendifications(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionIdendificationModel
            {
                NumeroContrat = donnees.Etat == Etat.EnVigueur ? donnees.NumeroContrat : string.Empty,
                Province = donnees.ProvinceEtat.ToString(),
                ImpotCorporation = donnees.ContractantEstCompagnie
                    ? donnees.TauxCorporatif.TauxMarginaux.TauxCorporation
                    : null,
                ImpotParticulier = donnees.TauxMarginal,
                Clients = CreerClients(donnees)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionProtectionsModel MapperProtections(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            var section = new SectionProtectionsModel
            {
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                MontantPrimeTotal = donnees.Protections.MontantPrimeTotal,
                PrestationDeces = donnees.Protections.PrestationDeces,
                ValeurMaximisee = MapperValeurMaximisee(
                    donnees.Protections.ValeurMaximisee,
                    donnees.Projections.AnneeDebutProjection),
                StatutOacaActif = donnees.Protections.StatutOacaActif,
                Protections = MapperProtections(donnees.Protections.ProtectionsAssures, donnees)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private static Types.SectionModels.SommaireProtections.ValeurMaximisee MapperValeurMaximisee(
            Types.Models.SommaireProtections.ValeurMaximisee valeurMaximisee,
            int anneeDebutProjection)
        {
            if (valeurMaximisee == null)
            {
                return null;
            }

            var dureeMinimisation = valeurMaximisee.DureeDebutMinimisation.HasValue &&
                                    valeurMaximisee.DureeDebutMinimisation.Value >= anneeDebutProjection
                ? valeurMaximisee.DureeDebutMinimisation
                : null;

            var capitalAssurePlafond = valeurMaximisee.DureeDebutMinimisation.HasValue &&
                                       valeurMaximisee.DureeDebutMinimisation.Value < anneeDebutProjection
                ? valeurMaximisee.CapitalAssurePlafond
                : null;

            return new Types.SectionModels.SommaireProtections.ValeurMaximisee
            {
                CapitalAssurePlancher = valeurMaximisee.CapitalAssurePlancher,
                DureeDebutMinimisation = dureeMinimisation,
                CapitalAssurePlafond = capitalAssurePlafond,
            };
        }

        private SectionSurprimesModel MapperSurprimes(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            var protectionSurprimee = donnees.Protections.ProtectionsAssures.Where(x => x.EstSurprimee && !x.EstLiberee).ToList();
            if (!protectionSurprimee.Any())
            {
                return null;
            }

            var section = new SectionSurprimesModel
            {
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                Protections = MapperProtections(protectionSurprimee, donnees),
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private IList<DetailProtection> MapperProtections(IEnumerable<Protection> protections, DonneesRapportIllustration donnees)
        {
            var result = new List<DetailProtection>();
            var noSequence = 1;
            foreach (var item in protections)
            {
                var skip = item.EstNouvelleProtection && item.DateEmission > donnees.DateIllustration;
                if (skip) continue;

                var protection = new DetailProtection
                {
                    SequenceId = noSequence,
                    EstProtectionConjointe = item.EstProtectionConjointe,
                    EstProtectionContractant = item.EstProtectionContractant,
                    EstLiberee = item.EstLiberee,
                    EstProtectionBase = item.EstProtectionBase,
                    EstSurprimee = item.EstSurprimee,
                    Description = donnees.Langue == Language.French ? item.Plan.DescriptionFr : item.Plan.DescriptionAn,
                    TypePrestationPlan = item.Plan.TypePrestationPlan,
                    TypeProtectionComplementaire = item.Plan.TypeProtectionComplementaire,
                    TypeAssurance = item.TypeAssurance,
                    TypeCout = item.TypeCout,
                    Noms = item.Assures?.Select(a => _formatter.FormatFullName(a.Prenom, a.Nom, a.Initiale)).ToList() ?? new List<string>(),
                    MontantCapitalAssureActuel = item.CapitalAssureActuel,
                    MontantPrimeMinimale = item.MontantPrimeSuggeree,
                    DateEmission = item.DateEmission,
                    DateMaturite = item.DateMaturite,
                    DateLiberation = !item.EstNouvelleProtection ? item.DateLiberation : null,
                    DureeProtection = item.EstNouvelleProtection ? item.DureeProtection : null,
                    DureePaiement = item.EstNouvelleProtection ? item.DureePaiement : null,
                    DureeRenouvellement = item.EstNouvelleProtection ? item.DureeRenouvellement : null,
                    AgeMaturitePlan = item.Plan.AgeMaturite,
                    AgeMaturiteProtection = donnees.CalculerAgeMaturiteProtection(item.Assures, item.DateMaturite),
                    Age = !item.Plan.CacherDetailTaux ? item.AgeEmission : null,
                    Sexe = !item.Plan.CacherDetailTaux ? item.Sexe : default(Sexe?),
                    StatutTabagisme = !item.Plan.CacherDetailTaux ? item.StatutTabagisme : default(StatutTabagisme?),
                    ReferenceNotes = item.ReferenceNotes,
                    SansVolumeAssurance = item.Plan.SansVolumeAssurance,
                    Surprimes = MapperSurprimes(item.Surprimes),
                    SurprimeTotal = item.SurprimeTotal,
                    CapitalAssureAsl = item.CapitalAssureASL,
                    CapitalAssureOaca = item.CapitalAssureOaca,
                    EstAvecFraisRachatSelonPrimes = item.EstAvecFraisRachatSelonPrimes,
                    EstAvecFraisRachatSelonFonds = item.EstAvecFraisRachatSelonFonds,
                    EstNouvelleProtection = item.EstNouvelleProtection
                };

                result.Add(protection);
                noSequence += 1;
            }

            return result;
        }

        private static IList<DetailSurprime> MapperSurprimes(IEnumerable<Surprime> surprimes)
        {
            if (surprimes == null) return new List<DetailSurprime>();
            return surprimes.Select(x => new DetailSurprime
            {
                TauxMontant = x.TauxMontant,
                DateLiberation = x.DateLiberation,
                Description = x.Assure,
                EstTypeTemporaire = x.EstTypeTemporaire,
                TauxPourcentage = x.TauxPourcentage
            }).ToList();
        }

        private SectionPrimesModel MapperPrimes(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionPrimesModel
            {
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                Primes = CreerDetailPrimes(donnees.Primes),
                PrimesVersees = CreerPrimesVersees(donnees.Primes, donnees),
                TitreColonnePrimesVersees = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(donnees.Facturation.FrequenceFacturation, donnees.Produit, donnees.Etat)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private IList<DetailPrimeVersee> CreerPrimesVersees(Primes primes, DonneesRapportIllustration donnees)
        {
            var result = new List<DetailPrimeVersee>();
            if (primes.PrimesVersees == null)
            {
                return result;
            }

            if (donnees.TypeContrat != ContractType.Universal && !_productRules.EstParmiFamilleAssuranceParticipants(donnees.Produit))
            {
                return result;
            }

            DetailPrimeVersee detailPrecedent = null;
            foreach (var item in primes.PrimesVersees.OrderBy(x => x.Annee))
            {
                if (detailPrecedent == null ||
                    detailPrecedent.TypeScenarioPrime != item.TypeScenarioPrime ||
                    Math.Abs(detailPrecedent.FacteurMultiplicateur - item.FacteurMultiplicateur) > .000009 ||
                    Math.Abs((detailPrecedent.Montant ?? 0) - (item.Montant ?? 0)) > .009 ||
                    detailPrecedent.FrequenceFacturation != item.FrequenceFacturation)
                {
                    var detail = new DetailPrimeVersee
                    {
                        Annee = item.Annee,
                        Mois = item.Mois,
                        FacteurMultiplicateur = item.FacteurMultiplicateur,
                        TypeScenarioPrime = item.TypeScenarioPrime,
                        Duree = item.Duree,
                        Montant = item.Montant,
                        FrequenceFacturation = item.FrequenceFacturation
                    };

                    result.Add(detail);
                    detailPrecedent = detail;
                }
            }

            return result;
        }

        private static IList<Types.SectionModels.SommaireProtections.DetailPrime> CreerDetailPrimes(Primes primes)
        {
            if (primes?.DetailPrimes == null)
            {
                return new List<Types.SectionModels.SommaireProtections.DetailPrime>();
            }

            return primes.DetailPrimes.Select(item =>
                new Types.SectionModels.SommaireProtections.DetailPrime
                {
                    TypeDetailPrime = item.TypeDetailPrime,
                    Montant = item.Montant,
                    MontantAvecTaxe = item.MontantAvecTaxe
                }).ToList();
        }

        private SectionASLModel MapperAssuranceSupplementaireLiberee(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return _assuranceSupplementaireLibereeModelBuilder.Build(definition, donnees, context);
        }

        private SectionFluxMonetaireModel MapperFluxMonetaire(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionFluxMonetaireModel
            {
                Details = CreerDetailsFluxMonetaire(donnees.FluxMonetaire?.Transactions)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);

            var titres = section.TitreSection.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            section.TitreSection = titres.FirstOrDefault();
            if (titres.Length <= 1) return section;
            var presenceDepots = section.Details.Any(x => x.TypeTransaction == TypeTransactionFluxMonetaire.Depot);
            var presenceRetraits = section.Details.Any(x => x.TypeTransaction == TypeTransactionFluxMonetaire.Retrait);
            if (presenceDepots && !presenceRetraits) section.TitreSection = titres[1];
            if (!presenceDepots && presenceRetraits) section.TitreSection = titres.LastOrDefault();
            return section.Details.Any() ? section : null;
        }

        private static IList<DetailFluxMonetaire> CreerDetailsFluxMonetaire(IList<TransactionFluxMonetaire> transactions)
        {
            var result = new List<DetailFluxMonetaire>();
            if (transactions == null) return result;
            result.AddRange(CreerTransactions(transactions.Where(t => t.TypeTransaction == TypeTransactionFluxMonetaire.Depot)));
            result.AddRange(CreerTransactions(transactions.Where(t => t.TypeTransaction == TypeTransactionFluxMonetaire.Retrait)));
            return result;
        }

        private static IEnumerable<DetailFluxMonetaire> CreerTransactions(
            IEnumerable<TransactionFluxMonetaire> transactions)
        {
            var sumTransactions = transactions.GroupBy(a => new { a.TypeTransaction, a.Type, a.Annee, a.TypeMontant })
                .Select(t => new
                {
                    t.First().Annee,
                    t.First().TypeTransaction,
                    t.First().Type,
                    t.First().TypeMontant,
                    t.First().EstDepotRetraitMaximal,
                    t.First().EstDepotRetraitApresDecheance,
                    Montant = t.Sum(m => m.Montant)
                }).ToList();

            var result = new List<DetailFluxMonetaire>();
            foreach (var item in sumTransactions.OrderBy(t => t.TypeTransaction).ThenBy(t => t.Type)
                .ThenBy(t => t.Annee).ThenBy(t => t.TypeMontant))
            {
                var detailPrecedent = result.LastOrDefault();
                if (detailPrecedent == null ||
                    detailPrecedent.Type != item.Type ||
                    detailPrecedent.AnneeFin != item.Annee - 1 ||
                    detailPrecedent.TypeMontant != item.TypeMontant ||
                    Math.Abs(detailPrecedent.Montant - item.Montant) > .009)
                {
                    result.Add(new DetailFluxMonetaire
                    {
                        AnneeDebut = item.Annee,
                        AnneeFin = item.Annee,
                        Montant = item.Montant,
                        EstDepotRetraitMaximal = item.EstDepotRetraitMaximal,
                        EstDepotRetaitApresDecheance = item.EstDepotRetraitApresDecheance,
                        TypeMontant = item.TypeMontant,
                        Type = item.Type,
                        TypeTransaction = item.TypeTransaction
                    });
                }
                else
                {
                    detailPrecedent.AnneeFin = item.Annee;
                }
            }

            return result;
        }

        private static IList<Types.SectionModels.SommaireProtections.Client> CreerClients(DonneesRapportIllustration donnees)
        {
            var result = donnees.Clients.Where(c => c.EstContractant).OrderBy(c => c.SequenceIndividu).Select(c =>
                new Types.SectionModels.SommaireProtections.Client
                {
                    Age = c.DateNaissance?.CalculerAge(donnees.DatePreparation),
                    Nom = c.Nom,
                    Prenom = c.Prenom,
                    Initiale = c.Initiale,
                    DateNaissance = c.DateNaissance,
                    Sexe = c.Sexe,
                    EstContractant = c.EstContractant,
                    SequenceId = c.SequenceIndividu
                }).ToList();

            result.AddRange(
                donnees.Clients.Where(c =>
                        donnees.Protections?.ProtectionsAssures?.Any(p =>
                            p.Assures?.Any(a => a.ReferenceExterneId == c.ReferenceExterneId) ?? false) ?? false)
                    .OrderBy(c => c.SequenceIndividu)
                    .Select(c => new Types.SectionModels.SommaireProtections.Client
                    {
                        Age = c.DateNaissance?.CalculerAge(donnees.DatePreparation),
                        Nom = c.Nom,
                        Prenom = c.Prenom,
                        Initiale = c.Initiale,
                        DateNaissance = c.DateNaissance,
                        Sexe = c.Sexe,
                        EstContractant = false,
                        SequenceId = c.SequenceIndividu
                    }).ToList());

            return result;
        }
    }
}