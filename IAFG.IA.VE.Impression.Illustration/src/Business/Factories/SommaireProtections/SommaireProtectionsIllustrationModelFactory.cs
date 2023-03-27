using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Helper;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees.TransactionContrat.Participations;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.FluxMonetaire;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration;
using IAFG.IA.VI.Projection.Data.Enums;
using Assure = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.Assure;
using DetailPrime = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailPrime;
using SectionFluxMonetaireModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionFluxMonetaireModel;
using SectionPrimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionPrimesModel;
using SectionSurprimesModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionSurprimesModel;
using DetailFluxMonetaire = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.DetailFluxMonetaire;
using SectionDetailEclipseDePrimeModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailEclipseDePrimeModel;
using SectionDetailParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionDetailParticipationsModel;
using SectionChangementAffectationParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionChangementAffectationParticipationsModel;
using SectionScenarioParticipationsModel = IAFG.IA.VE.Impression.Illustration.Types.SectionModels.SommaireProtectionsIllustration.SectionScenarioParticipationsModel;
using Language = IAFG.IA.VE.Impression.Core.Types.Enums.Language;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Factories.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;

namespace IAFG.IA.VE.Impression.Illustration.Business.Factories.SommaireProtections
{
    public class SommaireProtectionsIllustrationModelFactory : ISommaireProtectionsIllustrationModelFactory
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessorFactory;
        private readonly ISectionModelMapper _sectionModelMapper;
        private readonly IResourcesAccessor _resourcesAccessor;
        private readonly IDefinitionNoteManager _noteManager;
        private readonly IProductRules _productRules;
        private readonly IUsageAuConseillerModelBuilder _usageAuConseillerModelBuilder;
        private readonly IAssuranceSupplementaireLibereeModelBuilder _assuranceSupplementaireLibereeModelBuilder;

        public SommaireProtectionsIllustrationModelFactory(IConfigurationRepository configurationRepository,
            IIllustrationReportDataFormatter formatter, 
            IIllustrationResourcesAccessorFactory resourcesAccessorFactoryFactory, 
            ISectionModelMapper sectionModelMapper,
            IDefinitionNoteManager noteManager, 
            IProductRules productRules,
            IUsageAuConseillerModelBuilder usageAuConseillerModelBuilder, 
            IAssuranceSupplementaireLibereeModelBuilder assuranceSupplementaireLibereeModelBuilder)
        {
            _configurationRepository = configurationRepository;
            _formatter = formatter;
            _resourcesAccessorFactory = resourcesAccessorFactoryFactory;
            _resourcesAccessor = resourcesAccessorFactoryFactory.GetResourcesAccessor();
            _sectionModelMapper = sectionModelMapper;
            _noteManager = noteManager;
            _productRules = productRules;
            _usageAuConseillerModelBuilder = usageAuConseillerModelBuilder;
            _assuranceSupplementaireLibereeModelBuilder = assuranceSupplementaireLibereeModelBuilder;
        }

        public SectionSommaireProtectionsIllustrationModel Build(string sectionId, DonneesRapportIllustration donnees,
            IReportContext context)
        {
            var definitionSection = _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(sectionId, donnees.Produit);
            var model = new SectionSommaireProtectionsIllustrationModel();
            _sectionModelMapper.MapperDefinition(model, definitionSection, donnees, context);
            model.SectionContractantsModel = MapperContractants(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Contractants"), donnees, context);
            model.SectionCaracteristiquesIllustrationModel = MapperCaracteristiquesIllustration(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "CaracteristiquesIllustration"), donnees, context);
            model.SectionsAssuresModel = MapperAssures(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Assures"), donnees, context);
            model.SectionSurprimesModel = MapperSectionSurprimes(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Surprimes"), donnees, context);
            model.SectionConseillerModel = MapperSectionConseiller(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Conseillers"), donnees, context);
            model.SectionPrimesModel = MapperPrimes(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "Primes"), donnees, context);
            model.SectionPrimesVerseesModel = MapperPrimesVersees(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "PrimesVersees"), donnees, context);
            model.SectionASLModel = MapperAssuranceSupplementaireLiberee(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "ASL"), donnees, context);
            model.SectionFluxMonetaireModel = MapperFluxMonetaire(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "FluxMonetaire"), donnees, context);
            model.SectionDetailParticipationsModel = MapperDetailParticipations(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "DetailParticipations"), donnees, context);
            model.SectionChangementAffectationParticipationsModel = MapperChangementAffectationParticipations(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "ChangementAffectationParticipations"), donnees, context);
            model.SectionDetailEclipseDePrimeModel = MapperDetailEclipseDePrime(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "DetailEclipseDePrime"), donnees, context);
            model.SectionScenarioParticipationsModel = MapperScenarioParticipations(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "DetailScenarios"), donnees, context);
            model.SectionUsageAuConseillerModel = MapperUsageAuConseiller(definitionSection.ListSections?.FirstOrDefault(x => x.SectionId == "UsageDuConseiller"), donnees, context);
            return model;
        }

        private SectionASLModel MapperAssuranceSupplementaireLiberee(DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            return _assuranceSupplementaireLibereeModelBuilder.Build(definition, donnees, context);
        }

        private SectionDetailEclipseDePrimeModel MapperDetailEclipseDePrime(DefinitionSection definition,
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
                Baremes = donnees.Participations.Baremes.Select(item =>
                    new Types.SectionModels.SommaireProtectionsIllustration.Bareme()
                    {
                        Annee = item.Annee,
                        Diminution = item.Diminution
                    }).ToList()
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

        private SectionChangementAffectationParticipationsModel MapperChangementAffectationParticipations(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }
           
            int anneeChangementOptionParticipation;
            if (donnees.Participations != null && donnees.Participations.EstChangementOptionParticipationActivee)
            {
                anneeChangementOptionParticipation = donnees.Participations.AnneeChangementOptionParticipation;
            }
            else
            {
                var changementOptionParticipation = donnees.ModificationsDemandees?.Contrat?.Transactions?.OfType<ChangementOptionParticipation>().FirstOrDefault();
                if (changementOptionParticipation == null)
                {
                    return null;
                }
                anneeChangementOptionParticipation = changementOptionParticipation.Annee;
            }
            var section = new SectionChangementAffectationParticipationsModel
            {
                Annee = anneeChangementOptionParticipation
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

        private SectionCaracteristiquesIllustrationModel MapperCaracteristiquesIllustration(
            DefinitionSection definition, DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null || donnees.TypeContrat != ContractType.Universal)
            {
                return null;
            }

            var section = new SectionCaracteristiquesIllustrationModel();
            var resourcesAccessor = _resourcesAccessorFactory.GetResourcesAccessor();

            var valeurs = new List<KeyValuePair<string, string[][]>> {
               new KeyValuePair<string, string[][]>(nameof(donnees.Protections.PrestationDeces), new[] {new[]
               {
                   _formatter.FormatterEnum<OptionPrestationDeces>(donnees.Protections.PrestationDeces.ToString())
               }})
            };

            MapValeurMaximisee(donnees, valeurs);

            var statutOaca = donnees.Protections.StatutOacaActif != null && donnees.Protections.StatutOacaActif.Value;
            valeurs.Add(new KeyValuePair<string, string[][]>("OptimisationAutomatiqueCapitalAssure", new[]
            {
                new[]
                {
                    statutOaca
                        ? resourcesAccessor.GetStringResourceById("_actif")
                        : resourcesAccessor.GetStringResourceById("_inactif")
                }
            }));

            if (donnees.TypeAssurance == TypeAssurance.ConjointeDernierDec ||
                donnees.TypeAssurance == TypeAssurance.ConjointeDernierDecLib1er)
            {
                var keyValuePair = new KeyValuePair<string, string[][]>(
                    "PortionFondsCapitalisationPayable",
                    new[] { new[] { _formatter.FormatPercentage(donnees.PourcentagePayableDeces) } });
                valeurs.Add(keyValuePair);
            }

            section.Libelles = _formatter.FormatterLibellees(definition.Libelles, context);
            section.Valeurs = valeurs;

            return section;
        }

        private void MapValeurMaximisee(DonneesRapportIllustration donnees,
            ICollection<KeyValuePair<string, string[][]>> valeurs)
        {
            if (donnees.Protections.ValeurMaximisee == null)
            {
                return;
            }

            valeurs.Add(new KeyValuePair<string, string[][]>(
                nameof(donnees.Protections.ValeurMaximisee.DureeDebutMinimisation), new[]
                {
                    new[]
                    {
                        donnees.Protections.ValeurMaximisee.DureeDebutMinimisation.HasValue
                            ? _formatter.FormatterDuree(TypeDuree.NombreAnnees, donnees.Protections.ValeurMaximisee.DureeDebutMinimisation.Value)
                            : _resourcesAccessor.GetStringResourceById("_nonActivee")
                    }
                }));

            valeurs.Add(new KeyValuePair<string, string[][]>(
                nameof(donnees.Protections.ValeurMaximisee.CapitalAssurePlancher), new[]
                {
                    new[]
                    {
                        donnees.Protections.ValeurMaximisee.CapitalAssurePlancher.HasValue
                            ? _formatter.FormatCurrency(donnees.Protections.ValeurMaximisee.CapitalAssurePlancher.Value)
                            : _resourcesAccessor.GetStringResourceById("_nonActive")
                    }
                }));
        }

        private SectionSurprimesModel MapperSectionSurprimes(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var protections = donnees.ProtectionsGroupees.OrderBy(x => x.IsApplicant)
                .Select(x => MapperProtections(x.ProtectionsAssures, donnees));

            var section = new SectionSurprimesModel
            {
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                MontantSurprimeTotal = donnees.MontantSurprimeTotal,
                Protections = protections.SelectMany(x => x).ToList()
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionConseillerModel MapperSectionConseiller(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var agents = donnees.Agents.ToList();
            var section = new SectionConseillerModel
            {
                Conseillers = agents
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private IList<SectionAssuresModel> MapperAssures(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            var sections = new List<SectionAssuresModel>();
            var groupesAssures = donnees.ProtectionsGroupees.Where(x => !x.IsApplicant);

            foreach (var groupeAssures in groupesAssures)
            {
                var section = new SectionAssuresModel
                {
                    TypeAssurance = groupeAssures.ProtectionsAssures.First().TypeAssurance,
                    Assures = ObtenirAssures(donnees, groupeAssures.ProtectionsAssures.First().Assures),
                    Protections = ObtenirSectionProtectionAssures(definition, groupeAssures, donnees),
                    DateEffective = donnees.EstContratAntidate ? donnees.DateEmission : (DateTime?)null
                };

                _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
                sections.Add(section);
            }

            return sections;
        }

        private SectionContractantsModel MapperContractants(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionContractantsModel
            {
                NumeroContrat = donnees.Etat == Etat.EnVigueur ? donnees.NumeroContrat : string.Empty,
                Province = _configurationRepository.ObtenirLibelleProvince(donnees.ProvinceEtat.ToString()),
                ImpotCorporation = donnees.ContractantEstCompagnie ? donnees.TauxCorporatif : null,
                ImpotParticulier = donnees.TauxMarginal,
                Contractants = ObtenirContractants(donnees),
                Protections = ObtenirSectionProtectionsContractant(donnees),
                EstCompagnie = donnees.ContractantEstCompagnie,
                DateEffective = donnees.EstContratAntidate ? donnees.DateEmission : (DateTime?)null
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionPrimesModel MapperPrimes(DefinitionSection definition, 
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null) return null;
            var section = new SectionPrimesModel
            {
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                Primes = CreerDetailPrimes(donnees.Primes)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionPrimesVerseesModel MapperPrimesVersees(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            if (definition == null)
            {
                return null;
            }

            if (donnees.TypeContrat != ContractType.Universal && !_productRules.EstParmiFamilleAssuranceParticipants(donnees.Produit))
            {
                return null;
            }

            var section = new SectionPrimesVerseesModel
            {
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                PrimesVersees = CreerDetailPrimesVersees(donnees.Primes),
                TitreColonnePrimesVersees = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(donnees.Facturation.FrequenceFacturation, donnees.Produit)
            };

            _sectionModelMapper.MapperDefinition(section, definition, donnees, context);
            return section;
        }

        private SectionUsageAuConseillerModel MapperUsageAuConseiller(DefinitionSection definition,
            DonneesRapportIllustration donnees, IReportContext context)
        {
            return _usageAuConseillerModelBuilder.Build(definition, donnees, context);
        }

        private static IList<DetailPrime> CreerDetailPrimes(Primes primes)
        {
            if (primes?.DetailPrimes == null) return new List<DetailPrime>();
            return primes.DetailPrimes.Select(item => new DetailPrime
            {
                TypeDetailPrime = item.TypeDetailPrime,
                Montant = item.Montant,
                MontantAvecTaxe = item.MontantAvecTaxe
            }).ToList();
        }

        private static IList<DetailPrimesVersees> CreerDetailPrimesVersees(Primes primes)
        {
            if (primes?.PrimesVersees == null) return new List<DetailPrimesVersees>();
            return primes.PrimesVersees.Select(item => new DetailPrimesVersees()
            {
                Annee = item.Annee,
                Mois = item.Mois,
                FacteurMultiplicateur = item.FacteurMultiplicateur,
                TypeScenarioPrime = item.TypeScenarioPrime,
                Montant = item.Montant,
                Duree = item.Duree,
            }).ToList();
        }

        private SectionProtectionsSommaireModel ObtenirSectionProtectionsContractant(DonneesRapportIllustration donnees)
        {
            var protectionGroupee = donnees.ProtectionsGroupees.Single(x => x.IsApplicant);
            var protections = MapperProtections(protectionGroupee.ProtectionsAssures, donnees).ToList();

            var section = new SectionProtectionsSommaireModel
            {
                MontantPrimeTotal = protections.Sum(x => x.MontantPrimeMinimale.GetValueOrDefault(0)),
                Protections = protections,
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                EstAccesVie = donnees.Produit == Produit.AccesVie
            };

            if (section.Protections.Any())
            {
                section.NomComplet = section.Protections.First().Noms.Aggregate((s1, s2) =>
                    s1 + " " + _resourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("_et") + " " +
                    s2);
            }

            return section;
        }

        private SectionProtectionsSommaireModel ObtenirSectionProtectionAssures(DefinitionSection definition,
            ProtectionsGroupees protectionGroupee, DonneesRapportIllustration donnees)
        {
            var protections = MapperProtections(protectionGroupee.ProtectionsAssures, donnees).ToList();

            var section = new SectionProtectionsSommaireModel
            {
                MontantPrimeTotal = protections.Sum(x => x.MontantPrimeMinimale.GetValueOrDefault(0)),
                Protections = protections,
                FrequenceFacturation = donnees.Facturation.FrequenceFacturation,
                EstAccesVie = donnees.Produit == Produit.AccesVie
            };

            if (section.Protections.Any())
            {
                section.NomComplet = section.Protections.First().Noms.Aggregate((s1, s2) =>
                    s1 + " " + _resourcesAccessorFactory.GetResourcesAccessor().GetStringResourceById("_et") + " " +
                    s2);
            }

            section.Notes = _noteManager.CreerNotes(definition.Notes, donnees, new DonneesNote(),
                protectionGroupee.ProtectionsAssures);

            return section;
        }

        private IList<DetailProtection> MapperProtections(IList<Protection> protectionsAssures,
            DonneesRapportIllustration donnees)
        {
            var result = new List<DetailProtection>();

            foreach (var item in protectionsAssures)
            {
                var skip = item.EstNouvelleProtection && item.DateEmission > donnees.DateIllustration;
                if (skip) continue;

                var description = donnees.Langue == Language.French ? item.Plan.DescriptionFr : item.Plan.DescriptionAn;
                var descriptionCouts = item.TypeCout.GetDescription(_resourcesAccessorFactory.GetResourcesAccessor());
                description = description +
                              (descriptionCouts != string.Empty ? ", " + descriptionCouts : descriptionCouts);

                var protection = new DetailProtection
                {
                    SequenceId = result.Count + 1,
                    EstProtectionConjointe = item.EstProtectionConjointe,
                    EstProtectionContractant = item.EstProtectionContractant,
                    EstLiberee = item.EstLiberee,
                    EstProtectionBase = item.EstProtectionBase,
                    AfficherCapitalAssure = !item.EstAvenantLie,
                    EstSurprimee = item.EstSurprimee,
                    Description = description,
                    TypePrestationPlan = item.Plan.TypePrestationPlan,
                    TypeProtectionComplementaire = item.Plan.TypeProtectionComplementaire,
                    TypeAssurance = item.TypeAssurance,
                    TypeCout = item.TypeCout,
                    Noms = !item.EstProtectionContractant
                        ? item.Assures?.Select(a => _formatter.FormatFullName(a.Prenom, a.Nom, a.Initiale)).ToList() ??
                          new List<string>()
                        : ObtenirContractants(donnees)
                              ?.Select(a => _formatter.FormatFullName(a.Prenom, a.Nom, a.Initiale)).ToList() ??
                          new List<string>(),
                    MontantCapitalAssureActuel = item.CapitalAssureActuel,
                    MontantPrimeMinimale = item.MontantPrime,
                    DateEmission = !item.EstNouvelleProtection ? item.DateEmission : (DateTime?)null,
                    DateMaturite = item.DateMaturite,
                    DateLiberation = !item.EstNouvelleProtection ? item.DateLiberation : null,
                    DureeProtection = item.EstNouvelleProtection && donnees.Produit != Produit.AccesVie
                        ? item.DureeProtection
                        : null,
                    DureePaiement = item.EstNouvelleProtection && donnees.Produit != Produit.AccesVie
                        ? item.DureePaiement
                        : null,
                    DureeRenouvellement = item.EstNouvelleProtection && donnees.Produit != Produit.AccesVie
                        ? item.DureeRenouvellement
                        : null,
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
                    EstAvecFraisRachatSelonFonds = item.EstAvecFraisRachatSelonFonds
                };

                result.Add(protection);
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
                TauxPourcentage = x.TauxPourcentage,
                Terme = x.Terme
            }).ToList();
        }

        private static IList<Contractant> ObtenirContractants(DonneesRapportIllustration donnees)
        {
            return donnees.Clients.Where(c => c.EstContractant)
                .OrderBy(c => c.SequenceIndividu).Select(c =>
                new Contractant
                {
                    Age = c.AgeAssurance,
                    Nom = c.Nom,
                    Prenom = c.Prenom,
                    Initiale = c.Initiale,
                    DateNaissance = c.DateNaissance,
                    Sexe = c.Sexe,
                    EstContractant = c.EstContractant,
                    SequenceId = c.SequenceIndividu,
                    StatutFumeur = c.StatutFumeur,
                    EstCompagnie = donnees.ContractantEstCompagnie
                }).ToList();
        }

        private static IList<Assure> ObtenirAssures(DonneesRapportIllustration donnees,
            IList<Types.Models.SommaireProtections.Assure> assures)
        {
            return donnees.Clients.Where(c => assures.Any(a => a.ReferenceExterneId == c.ReferenceExterneId))
                .OrderBy(c => c.SequenceIndividu).Select(c => new Assure
                {
                    Age = c.AgeAssurance,
                    Nom = c.Nom,
                    Prenom = c.Prenom,
                    Initiale = c.Initiale,
                    DateNaissance = c.DateNaissance,
                    Sexe = c.Sexe,
                    EstContractant = c.EstContractant,
                    SequenceId = c.SequenceIndividu,
                    StatutFumeur = c.StatutFumeur,
                    EstNonAssurable = c.IsNotAssurable.GetValueOrDefault()
                }).ToList();
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

        private static IList<DetailFluxMonetaire> CreerDetailsFluxMonetaire(
            IList<TransactionFluxMonetaire> transactions)
        {
            var result = new List<DetailFluxMonetaire>();
            if (transactions == null)
            {
                return result;
            }

            result.AddRange(
                CreerTransactions(transactions.Where(t => t.TypeTransaction == TypeTransactionFluxMonetaire.Depot)));
            result.AddRange(CreerTransactions(transactions.Where(t =>
                t.TypeTransaction == TypeTransactionFluxMonetaire.Retrait)));
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
            foreach (var item in sumTransactions
                .OrderBy(t => t.TypeTransaction)
                .ThenBy(t => t.Type)
                .ThenBy(t => t.Annee)
                .ThenBy(t => t.TypeMontant))
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
                        EstDepotRetaitApresDecheance =  item.EstDepotRetraitApresDecheance,
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
    }
}