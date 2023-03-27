using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class DefinitionTableauManager : IDefinitionTableauManager
    {
        private readonly IVecteurManager _vecteurManager;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly IDefinitionNoteManager _noteManager;
        private readonly IDefinitionTitreManager _titreManager;

        public DefinitionTableauManager(
            IVecteurManager vecteurManager,
            IIllustrationReportDataFormatter formatter,
            IDefinitionNoteManager noteManager,
            IDefinitionTitreManager titreManager)
        {
            _vecteurManager = vecteurManager;
            _formatter = formatter;
            _noteManager = noteManager;
            _titreManager = titreManager;
        }

        public TableauResultat CreerTableau(DefinitionTableau definitionTableau, DonneesRapportIllustration donnees)
        {
            if (definitionTableau == null) return null;
            var definition = new TableauResultat
            {
                TitreTableau = _titreManager.ObtenirTitre(definitionTableau.Titres, donnees),
                DescriptionTableau = _titreManager.ObtenirDescription(definitionTableau.Titres, donnees),
                Avis = _noteManager.CreerAvis(definitionTableau.Avis, donnees),
                Notes = _noteManager.CreerNotes(definitionTableau.Notes, donnees, new DonneesNote()),
                GroupeColonnes = CreerGroupeColonnes(definitionTableau.GroupeColonnes, donnees, string.Empty),
                TypeTableau = definitionTableau.TypeTableau
            };

            return definition;
        }

        public TableauResultat CreerTableauPourGroupeAssures(DefinitionTableau definitionTableau,
            string identifiantGroupeAssure, string[] noms, DonneesRapportIllustration donnees)
        {
            if (definitionTableau == null) return new TableauResultat();
            var definition = new TableauResultat
            {
                TitreTableau = CreerTitreTableauAssure(definitionTableau, noms, donnees),
                DescriptionTableau = _titreManager.ObtenirDescription(definitionTableau.Titres, donnees),
                Avis = _noteManager.CreerAvis(definitionTableau.Avis, donnees),
                Notes = _noteManager.CreerNotes(definitionTableau.Notes, donnees, new DonneesNote()),
                GroupeColonnes = CreerGroupeColonnes(definitionTableau.GroupeColonnes, donnees, identifiantGroupeAssure),
                TypeTableau = definitionTableau.TypeTableau,
                IdentifiantGroupeAssure = identifiantGroupeAssure,
            };

            return definition;
        }

        private string CreerTitreTableauAssure(DefinitionTableau definitionTableau, string[] noms, DonneesRapportIllustration donnees)
        {
            var libelleNom = string.Join(" - ", noms);

            if (definitionTableau.TitresConjoint != null && noms.Length > 1)
            {
                return _titreManager.ObtenirTitre(definitionTableau.TitresConjoint, donnees, new[] { libelleNom });
            }

            if (definitionTableau.TitresIndividuel != null)
            {
                return _titreManager.ObtenirTitre(definitionTableau.TitresIndividuel, donnees, new[] { libelleNom });
            }

            return definitionTableau.Titres != null
                ? _titreManager.ObtenirTitre(definitionTableau.Titres, donnees, new[] { libelleNom })
                : libelleNom;
        }

        public List<GroupeColonne> CreerGroupeColonnes(
            IEnumerable<DefinitionGroupeColonne> definitionGroupes,
            DonneesRapportIllustration donnees,
            string identifiantGroupeAssure)
        {
            var result = new List<GroupeColonne>();
            var definitions = definitionGroupes as DefinitionGroupeColonne[] ?? definitionGroupes.ToArray();
            var sequences = definitions.GroupBy(g => g.SequenceId).Select(g => g.First().SequenceId).OrderBy(i => i)
                .ToArray();

            foreach (var id in sequences)
            {
                var groupes = definitions.Where(g =>
                    g.SequenceId == id && g.Produits != null && g.Produits.Any(x => x == donnees.Produit)).ToArray();

                if (!groupes.Any())
                {
                    groupes = definitions.Where(g => g.SequenceId == id && (g.Produits == null || !g.Produits.Any()))
                        .ToArray();
                }

                result.AddRange(groupes.Select(definition => new GroupeColonne
                {
                    TitreGroupe = _formatter.FormatterTitre(definition.Titres, donnees),
                    CouleurAlternative = definition.CouleurAlternative,
                    LigneVerticaleDroite = definition.LigneVerticaleDroite,
                    LigneVerticaleGauche = definition.LigneVerticaleGauche,
                    DefinitionColonnes = CreerColonnes(definition.Colonnes, donnees, identifiantGroupeAssure)
                }).ToList());
            }

            return result;
        }

        private List<ColonneTableau> CreerColonnes(IEnumerable<DefinitionColonne> definitionColonnes,
            DonneesRapportIllustration donnees, string identifiantGroupeAssure)
        {
            var colonnes = new List<ColonneTableau>();
            if (definitionColonnes == null)
            {
                return colonnes;
            }

            colonnes.AddRange(definitionColonnes.Select(definition =>
                CreerColonneTableau(definition, donnees, identifiantGroupeAssure)));

            return colonnes;
        }

        private ColonneTableau CreerColonneTableau(DefinitionColonne definition,
            DonneesRapportIllustration donnees, string identifiantGroupeAssure)
        {
            var result = new ColonneTableau
            {
                ColonneMoteur = definition.Colonne,
                CouleurAlternative = definition.CouleurAlternative,
                IdentifiantGroupeAssure = AppliquerLienGroupeAssurePrincipal(definition, donnees, identifiantGroupeAssure),
                TitreColonne = _formatter.FormatterTitre(definition.Titres, donnees),
                TypeProjection = definition.TypeProjection,
                TypeRendementProjection = definition.TypeRendementProjection,
                TypeAffichageValeur = definition.TypeAffichageValeur,
                TypeColonne = definition.TypeColonne,
                NoteReferences = definition.NoteReferences
            };

            result.Visible = EstVisible(result, definition, donnees);
            return result;
        }

        private string AppliquerLienGroupeAssurePrincipal(DefinitionColonne definition,
            DonneesRapportIllustration donnees, string identifiantGroupeAssure)
        {
            if (!string.IsNullOrEmpty(identifiantGroupeAssure))
            {
                return identifiantGroupeAssure;
            }

            if (definition.Regles != null &&
                definition.Regles.Any(x => x != null && x.Any(r => r == RegleColonne.LieeGroupeAssurePrincipal)))
            {
                return donnees.ObtenirIdentifiantGroupeAssurePrincipal();
            }

            return identifiantGroupeAssure;
        }

        private double[] ObtenirVecteur(DefinitionColonne definition, Projections projections, string identifiantAssure)
        {
            return string.IsNullOrWhiteSpace(identifiantAssure)
                ? _vecteurManager.ObtenirVecteurOuDefaut(projections, definition.Colonne, definition.TypeProjection, definition.TypeRendementProjection)
                : _vecteurManager.ObtenirVecteurOuDefautPourGroupeAssure(projections, definition.Colonne, definition.TypeProjection, definition.TypeRendementProjection, identifiantAssure);
        }

        private bool EstVisible(ColonneTableau colonneTableau, DefinitionColonne definition, DonneesRapportIllustration donnees)
        {
            if (definition.RegleProduits != null && !definition.RegleProduits.EstProduitValide(donnees.Produit))
            {
                return false;
            }

            if (definition.Regles == null)
            {
                return true;
            }

            var vecteur = ObtenirVecteur(definition, donnees.Projections, colonneTableau.IdentifiantGroupeAssure);
            return EstVisible(definition.Regles, colonneTableau, vecteur, donnees);
        }

        private bool EstVisible(IEnumerable<RegleColonne[]> regles, ColonneTableau colonneTableau,
            double[] vecteur, DonneesRapportIllustration donnees)
        {
            return regles == null || regles.Any(r => EstVisible(r, colonneTableau, vecteur, donnees));
        }

        private bool EstVisible(IEnumerable<RegleColonne> regles, ColonneTableau colonneTableau,
            double[] vecteur, DonneesRapportIllustration donnees)
        {
            if (regles == null)
            {
                return true;
            }

            foreach (var item in regles)
            {
                switch (item)
                {
                    case RegleColonne.Aucune:
                        break;
                    case RegleColonne.LieeGroupeAssurePrincipal:
                        if (!donnees.EstGroupeAssurePrincipal(colonneTableau.IdentifiantGroupeAssure)) return false;
                        break;
                    case RegleColonne.ValeurDifferenteZero:
                        if (vecteur == null || !vecteur.Any(v => Math.Abs(v) > 0.0000001)) return false;
                        break;
                    case RegleColonne.ValeurPlusGrandeZero:
                        if (vecteur == null || !vecteur.Any(v => v > 0.0000001)) return false;
                        break;
                    case RegleColonne.ContractantEstCompagnie:
                        if (!donnees.ContractantEstCompagnie) return false;
                        break;
                    case RegleColonne.ContratConjoint:
                        if (!donnees.ContratEstConjoint) return false;
                        break;
                    case RegleColonne.ContratIndividuel:
                        if (donnees.ContratEstConjoint) return false;
                        break;
                    case RegleColonne.VecteurPresent:
                        if (vecteur == null || !vecteur.Any()) return false;
                        break;
                    case RegleColonne.ToujoursMasquee:
                        return false;
                    case RegleColonne.ValeurSuperieureOuEgaleZero:
                        if (vecteur == null || !vecteur.Any(v => v >= 0)) return false;
                        break;
                    case RegleColonne.NouvelleVente:
                        if (donnees.Etat != Etat.NouvelleVente) return false;
                        break;
                    case RegleColonne.CompteAVM:
                        if (!donnees.IsMarketValueAdjustmentApplied()) return false;
                        break;
                    case RegleColonne.EnVigueur:
                        if (donnees.Etat == Etat.NouvelleVente) return false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(regles));
                }
            }

            return true;
        }
    }
}
