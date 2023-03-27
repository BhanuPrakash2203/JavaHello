using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VI.Projection.Data.Enums;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class DefinitionTexteManager : IDefinitionTexteManager
    {
        private const string COMPTE_LISSE_SRD080 = "SRD080";
        private const string COMPTE_LISSE_SRD081 = "SRD081";
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly IProductRules _productRules;

        public DefinitionTexteManager(
            IIllustrationReportDataFormatter formatter,
            IProductRules productRules)
        {
            _formatter = formatter;
            _productRules = productRules;
        }

        public string ObtenirTexte(DefinitionTexte definition, DonneesRapportIllustration donnees)
        {
            var texte = _formatter.FormatterTexte(definition, donnees);
            var bullets = CreerBullets(definition?.Bullets, donnees);
            return string.IsNullOrWhiteSpace(bullets) ? texte : texte + $"<html>{bullets}</html>";
        }

        public List<DetailTexte> CreerDetailTextes(List<DefinitionTexte> textes, DonneesRapportIllustration donnees)
        {
            if (textes == null || !textes.Any())
            {
                return new List<DetailTexte>();
            }

            return textes.OrderBy(t => t.SequenceId)
                .Where(i => EstVisible(i, donnees))
                .Select(i => new DetailTexte
                {
                    Texte = ObtenirTexte(i, donnees),
                    SautDeLigneAvant = i.SautDeLigne == SautDeLigne.Avant || i.SautDeLigne == SautDeLigne.AvantEtApres,
                    SautDeLigneApres = i.SautDeLigne == SautDeLigne.Apres || i.SautDeLigne == SautDeLigne.AvantEtApres,
                    SequenceId = i.SequenceId
                })
                .ToList();
        }

        public bool EstVisible(DefinitionTexte definitionTexte, DonneesRapportIllustration donnees)
        {
            return definitionTexte.RegleProduits.EstProduitValide(donnees.Produit) &&
                   EstVisible(definitionTexte.Regles, donnees);
        }

        private bool EstVisible(IReadOnlyCollection<RegleTexte[]> regles, DonneesRapportIllustration donnees)
        {
            return regles == null || !regles.Any() || regles.Any(r => EstVisible(r, donnees));
        }

        private bool EstVisible(IEnumerable<RegleTexte> regles, DonneesRapportIllustration donnees)
        {
            if (regles == null)
            {
                return true;
            }

            foreach (var regle in regles)
            {
                if (!EstVisible(regle, donnees))
                {
                    return false;
                }
            }

            return true;
        }

        private bool EstVisible(RegleTexte regle, DonneesRapportIllustration donnees)
        {
            switch (regle)
            {
                case RegleTexte.Aucune:
                    return true;

                case RegleTexte.TauxPreferentielPresent:
                    return donnees.HasPreferentialStatus();

                case RegleTexte.CapitalAssurePlusDe10M:
                    return donnees.EstCapitalAssurePlusDe10Millions();

                case RegleTexte.ProtectionTemporaireRenouvelable:
                    // La note ProtectionTemporaireRenouvelable doit être affichée seulement
                    // s'il y a la présence de protections temporaires et renouvelables
                    return donnees.Protections.ExisteProtectionTemporaireRenouvelable;

                case RegleTexte.PrimesRenouvellement:
                    // La note PrimesRenouvellement doit être affichée seulement
                    // si le renouvellement d’une protection au minimum était permis.
                    return donnees.PresencePrimeRenouvellable;

                case RegleTexte.MontantsPrestationDeces:
                    // La note MontantsPrestationDeces doit être affichée seulement si une protection Vie [TypeProtection] est présente sur le contrat
                    return donnees.ProtectionsPDF != null &&
                           donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsLifeCoverage);

                case RegleTexte.MontantsValeurRachatAssuranceLiberee:
                    // La note MontantsValeurRachatAssuranceLiberee doit être affichée seulement si une protection Vie [TypeProtection] avec
                    // valeur de rachat [ProtectionAvecValeurRachat] est présente sur le contrat
                    return donnees.ProtectionsPDF != null &&
                           donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsLifeCoverage) &&
                           donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsCashValueAvailable);

                case RegleTexte.MontantsPrestationsMaladieGrave:
                    // La note MontantsPrestationsMaladieGrave doit être affichée seulement si une protection Maladie Grave [TypeProtection] est présente sur le contrat
                    return donnees.ProtectionsPDF != null &&
                           donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsCriticalIllness);

                case RegleTexte.MontantsRemboursementFlexibles:
                    // La note MontantsRemboursementFlexibles doit être affichée seulement si un avenant Remboursement flexible des primes [ListeProtections] est présent sur le contrat
                    return donnees.ProtectionsPDF != null &&
                           donnees.ProtectionsPDF.Any(pdf => pdf.Specification.IsRembFlexPrime);

                case RegleTexte.EstContratUniversel:
                    return donnees.TypeContrat == ContractType.Universal;

                case RegleTexte.NouvelleVente:
                    return donnees.Etat.Equals(Etat.NouvelleVente);

                case RegleTexte.EnVigueur:
                    return donnees.Etat.Equals(Etat.EnVigueur);

                case RegleTexte.CapitalValeur:
                    return donnees.Produit.Equals(Produit.CapitalValeur) || 
                           donnees.Produit.Equals(Produit.CapitalValeur3);

                case RegleTexte.SaufCapitalValeur:
                    return !donnees.Produit.Equals(Produit.CapitalValeur) && 
                           !donnees.Produit.Equals(Produit.CapitalValeur3);

                case RegleTexte.AssuranceParticipantDividendesEnDepot:
                    return _productRules.EstParmiFamilleAssuranceParticipants(donnees.Produit) &&
                           donnees.Participations?.OptionParticipation != null &&
                           donnees.Participations.OptionParticipation.Equals(TypeOptionParticipation.Depot);

                case RegleTexte.BoniPARPresent:
                    return (donnees.FondsProtectionPrincipale?.DefaultRate ?? 0) > 0;

                case RegleTexte.PresenceCompteSRDIris:
                    return donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                        string.Equals(v.Vehicle, COMPTE_LISSE_SRD080) &&
                        string.Equals(f.Vehicule, v.Vehicle)));

                case RegleTexte.PresenceCompteSRDLisse:
                    return
                        donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                        string.Equals(v.Vehicle, COMPTE_LISSE_SRD081) &&
                        string.Equals(f.Vehicule, v.Vehicle)));

                case RegleTexte.PresenceCompteSRA:
                    return donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                        string.Equals(f.Vehicule, v.Vehicle) &&
                        string.Equals(f.AccountType, "Average5Years") &&
                        string.Equals(f.SubType, "SRIAAccount")));

                case RegleTexte.PresenceCompteCPV:
                    return donnees.Vehicules.Any(v => donnees.FondsInvestissement.Any(f =>
                        string.Equals(f.Vehicule, v.Vehicle) &&
                        string.Equals(f.AccountType, "Average5Years") &&
                        string.Equals(f.SubType, "LisseAccount_Rule5")));

                default:
                    throw new ArgumentOutOfRangeException(nameof(regle));
            }
        }

        private string CreerBullets(IReadOnlyCollection<DefinitionTexte> textes, DonneesRapportIllustration donnees)
        {
            if (textes == null || !textes.Any())
            {
                return string.Empty;
            }

            var result = textes.OrderBy(t => t.SequenceId)
                .Where(i => EstVisible(i, donnees))
                .Select(i => ObtenirTexteBullet(i, donnees))
                .ToList();

            return result.Any() ? $"<ul>{string.Join("", result)}</ul>" : string.Empty;
        }

        private string ObtenirTexteBullet(DefinitionTexte definition, DonneesRapportIllustration donnees)
        {
            var texte = _formatter.FormatterTexte(definition, donnees);
            var bullets = CreerBullets(definition?.Bullets, donnees);
            return texte + bullets;
        }
    }
}