using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers.Illustration;
using IAFG.IA.VE.Impression.Illustration.Types;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.ASL;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections.FluxMonetaire;
using IAFG.IA.VI.AF.IPDFVie.Factory.Interfaces;
using IAFG.IA.VI.AF.IPDFVie.PDF.PDA.ENUMs;
using IAFG.IA.VI.Projection.Data.Extensions;
using IAFG.IA.VI.Projection.DataExtensions;
using Models = IAFG.IA.VE.Impression.Illustration.Types.Models;
using ProjectionData = IAFG.IA.VI.Projection.Data;
using ProjectionEnums = IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Mappers
{
    public class IllustrationModelMapper : IIllustrationModelMapper
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IProjectionManager _projectionManager;
        private readonly IProjectionsMapper _projectionsMapper;
        private readonly IProtectionsMapper _protectionsMapper;
        private readonly IClientsMapper _clientsMapper;
        private readonly IHypothesesMapper _hypothesesMapper;
        private readonly IModificationsMapper _modificationsMapper;
        private readonly IConceptVenteMapper _conceptVenteMapper;

        public IllustrationModelMapper(IConfigurationRepository configurationRepository,
                                       IProjectionManager projectionManager,
                                       IProtectionsMapper protectionsMapper,
                                       IProjectionsMapper projectionsMapper,
                                       IClientsMapper clientsMapper,
                                       IHypothesesMapper hypothesesMapper,
                                       IModificationsMapper modificationsMapper,
                                       IConceptVenteMapper conceptVenteMapper)
        {
            _configurationRepository = configurationRepository;
            _projectionManager = projectionManager;
            _projectionsMapper = projectionsMapper;
            _protectionsMapper = protectionsMapper;
            _clientsMapper = clientsMapper;
            _hypothesesMapper = hypothesesMapper;
            _modificationsMapper = modificationsMapper;
            _conceptVenteMapper = conceptVenteMapper;
        }

        public DonneesRapportIllustration Map(DonneesIllustration data)
        {
            if (data.ParametreRapport == null) throw new ArgumentNullException(nameof(data.ParametreRapport));
            var projection = _projectionManager.GetDefaultProjection(data.Projections);
            if (projection == null) throw new ArgumentNullException(nameof(data.Projections));
            if (projection.Parameters == null) throw new ArgumentNullException(nameof(projection.Parameters));
            var protectionBase = _projectionManager.GetMainCoverage(projection);
            if (protectionBase == null) throw new ArgumentNullException(nameof(protectionBase));
            var dateEmission = protectionBase.Dates.Issuance;
            var pdfCoverage = _projectionManager.GetPdfCoverage(projection, protectionBase);

            var model = new DonneesRapportIllustration
            {
                Langue = data.ParametreRapport.Language,
                PagesSelectionnees = data.ParametreRapport.PagesSelectionnees?.ToList() ?? new List<string>(),
                ChoixAnneesRapport = new Models.ChoixAnneesRapport { ChoixAnnees = data.ParametreRapport.ChoixAnneesRapport },
                NumeroContrat = data.NumeroContrat,
                DateVersionProduit = data.DateVersionProduit,
                Produit = (Produit)projection.Contract.Product,
                VersionProduit = data.VersionProduit,
                DateMiseAJour = DateMiseAJour(data, projection),
                DateEmission = dateEmission,
                DateIllustration = projection.Parameters.IllustrationDate,
                TypeAssurance = protectionBase.InsuranceType.ConvertirTypeAssurance(),
                Etat = _projectionManager.EtatContrat(projection),
                DatePreparation = DateMiseAJour(data, projection),
                DateDerniereModification = DerniereModification(projection),
                ContractantEstCompagnie = _projectionManager.ContractantEstCompagnie(projection),
                ProvinceEtat = _projectionManager.ProvinceEtat(projection),
                Banniere = _projectionManager.Banniere(projection),
                TauxCorporatif = TauxCorporatif(projection),
                TauxMarginal = TauxMarginal(projection),
                Facturation = _projectionsMapper.MapFacturation(projection),
                TabagismePreferentiel = _projectionManager.DeterminerTabagismePreferentiel(projection),
                PresenceDeGarantie = PresenceGarantie(projection),
                PresenceCompteTerme = _projectionManager.DeterminerPresenceCompteTerme(projection),
                Clients = _clientsMapper.MapClients(data.DonneesClients, projection),
                ContratEstConjoint = _projectionManager.ContratEstConjoint(projection),
                ContratEstEnDecheance = _projectionManager.ContratEstEnDecheance(projection),
                Agents = MapperAgent(data.Agents, data.ParametreRapport.Language),
                AgentType = projection.Parameters.Compensation.AgentType,
                BonusRate = projection.Parameters.Compensation.BonusRate,
                ProtectionsPDF = data.DonneesPdf.ProtectionsPdf,
                MontantSurprimeTotal = projection.Values.Search(ProjectionEnums.ValueId.TotalExtraPremium).GetValueOrDefault(0.00D),
                VersionProduitFormattee = data.ParametreRapport.VersionProduitFormattee,
                VersionEVO = data.ParametreRapport.VersionEVO,
                PourcentagePayableDeces = data.PourcentagePayableDeces,
                TypeContrat = projection.Contract.ContractType,
                EstProduitAvecPrimeReference = EstProduitAvecPrimeReference(pdfCoverage),
                IsSignaturePapier = data.ParametreRapport.IsSignaturePapier,
                EstContratAntidate = projection.IsBackdatedNewContract(),
                PresencePrimeRenouvellable = _projectionManager.DeterminerPresencePrimeRenouvellable(projection),
                Participations = _protectionsMapper.MapParticipations(projection, data.ParametreRapport),
                DateDerniereMiseAJourInterets = projection.Contract.FinancialSection?.LastInterestRatesUpdate,
                FondsProtectionPrincipale = data.DonneesPdf.FondsBoniParticipation,
                FondsInvestissement = data.DonneesPdf.FondsInvestissement,
                Vehicules = data.DonneesPdf.Vehicules,
                ProfilInvestisseurElectroniqueComplet = data.ParametreRapport.ProfilInvestisseurElectroniqueComplet,
                AvancesSurPolice = _projectionsMapper.MapAvancesSurPolice(projection),
                MontantMaximumAnnuelOdsPermis = projection.Values.Search(ProjectionEnums.ValueId.MaximumAnnualPaidUpAdditionAllocation)
            };

            var configurationRapport = _configurationRepository.ObtenirConfigurationRapport(model.Produit, model.Etat);
            model.Projections = _projectionsMapper.Map(data.Projections, data.ParametreRapport, configurationRapport);
            model.InformationTableauAssures = InformationTableauParAssure(projection, configurationRapport.AgeReferenceProjection);

            if (string.IsNullOrEmpty(model.VersionProduit))
            {
                model.VersionProduit = configurationRapport.Version;
            }

            model.Boni = MapBoni(projection);
            model.ProtectionsGroupees = _protectionsMapper.MapperProtectionsGroupees(data, projection, model.Clients);
            model.Protections = _protectionsMapper.MapperProtections(data, projection, model.Clients);

            model.Primes = new Models.SommaireProtections.Primes().MapperPrimes(projection, dateEmission, model);
            model.AssuranceSupplementaireLiberee = new AssuranceSupplementaireLiberee().MapperAsl(projection, dateEmission);
            model.FluxMonetaire = new FluxMonetaire().MapperFluxMonetaire(projection, dateEmission);

            model.HypothesesInvestissement = _hypothesesMapper.MapHypothesesInvestissement(projection, dateEmission, model.Projections);
            model.ConceptVente = _conceptVenteMapper.Map(data.Projections, model, dateEmission);
            model.ModificationsDemandees = _modificationsMapper.MapModificationsDemandees(projection, model.Clients, dateEmission);
            return model;
        }

        private static List<Models.Agent> MapperAgent(IEnumerable<Types.Agent> agents, Language language)
        {
            var listeAgents = new List<Models.Agent>();
            if (agents == null)
            {
                return listeAgents;
            }

            listeAgents.AddRange(agents.Select(agent => new Models.Agent
            {
                Agence = agent.Agence,
                Telecopieur = agent.Telecopieur,
                TelephoneBureau = agent.TelephoneBureau,
                TelephonePrincipal = agent.TelephonePrincipal,
                Courriel = agent.Courriel,
                Nom = agent.Nom,
                Prenom = agent.Prenom,
                Initiale = agent.Initiale,
                Genre = agent.Genre,
                Titre = language == Language.French ? agent.TitreFrancais : agent.TitreAnglais
            }));

            return listeAgents;
        }

        private Boni MapBoni(ProjectionData.Projection projection)
        {
            var boni = new Boni();
            var protectionBase = _projectionManager.GetMainCoverage(projection);
            if (protectionBase == null) return boni;

            var bonusType = projection.Contract?.FinancialSection?.Bonus?.BonusType ?? ProjectionEnums.Financial.BonusType.None;
            boni.ChoixBoniInteret = (ChoixBoniInteret)bonusType;

            var pdfCoverage = _projectionManager.GetPdfCoverage(projection, protectionBase);
            boni.DebutBoniInteret = pdfCoverage?.Coverage?.PDB?.BonusRateStartDuration ?? 0;
            boni.BoniInteret = pdfCoverage?.Coverage?.PDB != null
                ? (BoniInteret)pdfCoverage.Coverage.PDB.InterestRateCalculationMethod
                : BoniInteret.Regle0_AucunBoni;

            boni.DebutBoniFidelite = pdfCoverage?.Coverage?.PDB?.StartIncentive ?? 0;
            boni.BoniFidelite = pdfCoverage?.Coverage?.PDB != null
                ? (BoniFidelite)pdfCoverage.Coverage?.PDB?.IncentiveBonusCalculationMethod
                : BoniFidelite.NonApplicable;

            boni.TauxBoni = pdfCoverage?.Coverage?.PDB?.GuaranteedBonusRate / 100;
            return boni;
        }

        private static IList<InformationTableauAssure> InformationTableauParAssure(ProjectionData.Projection projection, int ageReference)
        {
            var informationTableauAssures = new List<InformationTableauAssure>();

            foreach (var insured in projection.Contract.Insured)
            {
                informationTableauAssures.Add(new InformationTableauAssure
                {
                    IdAssure = insured.Identifier.Id,
                    EstPrincipal = insured.IsMain,
                    IndexFinProjection = projection.EndIndexForInsuredGroup(insured.Identifier.Id, ageReference)
                });
            }

            return informationTableauAssures;
        }

        private static bool PresenceGarantie(ProjectionData.Projection projection)
        {
            return projection?.Contract?.Insured?.Any(x => x.Coverages?.Any(y => y.AdditionalBenefits?.Any() ?? false) ?? false) ?? false;
        }

        private static double? TauxMarginal(ProjectionData.Projection projection)
        {
            return projection?.Parameters?.Taxation.Personal?.MarginalRates?.FirstOrDefault()?.Value;
        }

        private TauxImpositionCorporatifs TauxCorporatif(ProjectionData.Projection projection)
        {
            if (!_projectionManager.ContractantEstCompagnie(projection)) return null;
            var impotCorporation = projection?.Parameters?.Taxation.Corporate;
            var impotIndividu = projection?.Parameters?.Taxation.Personal;

            return new TauxImpositionCorporatifs
            {
                TauxMarginaux = new TauxImpositionCorporatifsDetail()
                {
                    TauxCorporation = impotCorporation?.MarginalRates?.FirstOrDefault()?.Value,
                    TauxParticulier = impotIndividu?.MarginalRates?.FirstOrDefault()?.Value
                },
                TauxDividendes = new TauxImpositionCorporatifsDetail()
                {
                    TauxCorporation = impotCorporation?.DividendRates?.FirstOrDefault()?.Value,
                    TauxParticulier = impotIndividu?.DividendRates?.FirstOrDefault()?.Value
                },
                TauxGainCapital = new TauxImpositionCorporatifsDetail()
                {
                    TauxCorporation = impotCorporation?.CapitalGainsRates?.FirstOrDefault()?.Value,
                    TauxParticulier = impotIndividu?.CapitalGainsRates?.FirstOrDefault()?.Value
                }
            };
        }

        private static DateTime DateMiseAJour(DonneesIllustration data, ProjectionData.Projection projection)
        {
            return data.DateMiseAJour ?? DerniereModification(projection);
        }

        private static DateTime DerniereModification(ProjectionData.Projection projection)
        {
            var dates = new List<DateTime?>
                        {
                            projection?.Contract?.LastUpdate,
                            projection?.Parameters?.LastUpdate,
                            projection?.Transactions?.LastUpdate
                        };

            var maxDate = dates.Where(x => x.HasValue).OrderByDescending(x => x.Value).FirstOrDefault();
            return maxDate ?? DateTime.Today;
        }

        private static bool EstProduitAvecPrimeReference(IGetPDFICoverageResponse pdfCoverage)
        {
            return pdfCoverage.Coverage.PDA.MinpremCourCalcmth == MinpremCourCalcmth.Regle2;
        }
    }
}