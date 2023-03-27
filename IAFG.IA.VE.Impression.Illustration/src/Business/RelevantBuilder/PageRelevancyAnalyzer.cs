using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.RelevantBuilder;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.ModificationsDemandees;

namespace IAFG.IA.VE.Impression.Illustration.Business.RelevantBuilder
{
    public class PageRelevancyAnalyzer : IPageRelevancyAnalyzer
    {
        private readonly IConfigurationRepository _configurationRepository;

        public PageRelevancyAnalyzer(IConfigurationRepository configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public IList<PageRapportInfo> GetRelevantPages(DonneesRapportIllustration donneesIllustration)
        {
            var configuration = _configurationRepository.ObtenirConfigurationRapport(donneesIllustration.Produit, donneesIllustration.Etat);
            return configuration.Pages.Where(p => IsValid(p.SectionId, donneesIllustration)).OrderBy(p => p.OrdreAffichage).ToList();
        }

        public bool IsValid(string sectionId, DonneesRapportIllustration donneesIllustration)
        {
            var section = _configurationRepository.ObtenirConfigurationSection(sectionId, donneesIllustration.GroupeRapport);
            return IsValid(section, donneesIllustration);
        }

        private static bool IsValid(ConfigurationSection section, DonneesRapportIllustration donnees)
        {
            return section != null && 
                donnees.GroupeRapport.GetValueOrDefault(section.GroupeRapport) == section.GroupeRapport && 
                IsValid(section.Regles, donnees);
        }

        private static bool IsValid(List<RegleSection[]> regles, DonneesRapportIllustration donnees)
        {
            return regles == null || !regles.Any() || regles.Any(r => IsValid(r, donnees));
        }

        private static bool IsValid(IEnumerable<RegleSection> regles, DonneesRapportIllustration donnees)
        {
            if (regles == null)
            {
                return true;
            }

            var result = true;
            foreach (var item in regles)
            {
                switch (item)
                {
                    case RegleSection.Aucune:
                        break;
                    case RegleSection.ContractantEstCompagnie:
                        result = donnees.ContractantEstCompagnie;
                        break;
                    case RegleSection.ContractantEstIndividu:
                        result = !donnees.ContractantEstCompagnie;
                        break;
                    case RegleSection.ConceptAvancePret:
                        result = donnees.ConceptVente?.AvancePret != null;
                        break;
                    case RegleSection.ConceptBonSuccessoral:
                        result = donnees.ConceptVente?.BonSuccessoral != null;
                        break;
                    case RegleSection.ConceptPretEnCollateral:
                        result = donnees.ConceptVente?.PretEnCollateral != null;
                        break;
                    case RegleSection.ConceptPretEnCollateralEmprunteurTiercePartie:
                        result = donnees.ConceptVente?.PretEnCollateral?.Data?.Taxation?.Borrower != null &&
                                 donnees.ConceptVente.PretEnCollateral.Data.Taxation.Borrower.BorrowerType == VI.Projection.Data.Enums.Concept.BorrowerType.ThirdParty;
                        break;
                    case RegleSection.ConceptPretEnCollateralEmprunteurAutreQueTiercePartie:
                        result = donnees.ConceptVente?.PretEnCollateral?.Data?.Taxation?.Borrower != null &&
                                 donnees.ConceptVente.PretEnCollateral.Data.Taxation.Borrower.BorrowerType != VI.Projection.Data.Enums.Concept.BorrowerType.ThirdParty;
                        break;
                    case RegleSection.ConceptProprietePartagee:
                        result = donnees.ConceptVente != null && donnees.ConceptVente.ProprietePartagee;
                        break;
                    case RegleSection.PasBannierePPIProduitCapitalValeur:
                        result = !(donnees.Banniere == Banniere.PPI && 
                                   (donnees.Produit == Produit.CapitalValeur || 
                                    donnees.Produit == Produit.CapitalValeur3));
                        break;
                    case RegleSection.IsSignaturePapier:
                        result = donnees.IsSignaturePapier;
                        break;
                    case RegleSection.PresenceModificationDemandee:
                        result = ValiderPresenceModificationDemandee(donnees.ModificationsDemandees);
                        break;
                    case RegleSection.PrimesRenouvellementPresente:
                        result = donnees.ProtectionsPDF.Any(x => x.Specification.IsRenewable);
                        break;
                    case RegleSection.ValeurRachatPresente:
                         result = donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsCashValueAvailable);
                        break;
                    case RegleSection.ProtectionMaladieGravePresente:
                        result = donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsCriticalIllness);
                        break;
                    case RegleSection.ProfilInvestisseurElectroniqueComplet:
                        result = donnees.ProfilInvestisseurElectroniqueComplet;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(regles), @"RegleSection inconnue");
                }

                if (!result) return false;
            }

            return true;
        }

        private static bool ValiderPresenceModificationDemandee(ModificationsDemandees modificationsDemandees)
        {
            if (modificationsDemandees?.Contrat?.Transactions != null &&
                modificationsDemandees.Contrat.Transactions.Any())
            {
                return true;
            }

            return modificationsDemandees?.Protections?.Values != null &&
                   modificationsDemandees.Protections.Values.Any(x => x.Transactions != null && x.Transactions.Any());
        }
    }
}