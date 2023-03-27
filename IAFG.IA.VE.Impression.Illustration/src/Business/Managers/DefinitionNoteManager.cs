using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Helper;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.HypothesesInvestissement;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using IAFG.IA.VI.Projection.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class DefinitionNoteManager : IDefinitionNoteManager
    {
        private readonly IVecteurManager _vecteurManager;
        private readonly IIllustrationReportDataFormatter _formatter;

        public DefinitionNoteManager(IVecteurManager vecteurManager, IIllustrationReportDataFormatter formatter)
        {
            _vecteurManager = vecteurManager;
            _formatter = formatter;
        }

        public IList<string> CreerAvis(IEnumerable<DefinitionAvis> definitions, DonneesRapportIllustration donnees)
        {
            var result = new List<string>();
            if (definitions == null)
            {
                return result;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var definition in definitions.OrderBy(d => d.SequenceId))
            {
                if (!EstVisible(definition, donnees, new DonneesNote()))
                {
                    continue;
                }

                var avis = _formatter.FormatterAvis(definition, donnees);
                if (!string.IsNullOrEmpty(avis))
                {
                    result.Add(avis);
                }
            }

            return result;
        }

        public IList<DetailNote> CreerNotes(IEnumerable<DefinitionNote> definitions,
            DonneesRapportIllustration donnees, DonneesNote donneesNote)
        {
            return CreerNotes(definitions, donnees, donneesNote, null);
        }

        public IList<DetailNote> CreerNotes(IEnumerable<DefinitionNote> definitions,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, IList<Types.Models.SommaireProtections.Protection> protections)
        {
            var result = new List<DetailNote>();
            if (definitions == null)
            {
                return result;
            }

            foreach (var definition in definitions.OrderBy(d => d.SequenceId))
            {
                switch (definition.TypeNote)
                {
                    case TypeNote.Generique:
                        if (EstVisible(definition, donnees, donneesNote))
                        {
                            CreerNote(result, definition, donnees);
                        }
                        break;
                    case TypeNote.EsperanceVie:
                        AjouterNoteEsperanceVie(result, definition, donnees);
                        break;
                    case TypeNote.MessageMoteur:
                        AjouterNoteMessageMoteur(result, definition, donnees);
                        break;
                    case TypeNote.Protection:
                        AjouterNoteProtections(result, definition, donnees, donneesNote, protections);
                        break;
                    case TypeNote.CompteAVM:
                    case TypeNote.DetailCompteAVM:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(definition.TypeNote));
                }
            }

            return result;
        }

        public DetailNote CreerNote(IList<DetailNote> notes, DefinitionNote definition,
            DonneesRapportIllustration donnees)
        {
            var texte = _formatter.FormatterNote(definition, donnees);
            return CreerNote(notes, definition, texte);
        }

        public DetailNote CreerNote(IList<DetailNote> notes, DefinitionNote definition, string texte)
        {
            if (string.IsNullOrEmpty(texte) || notes.Any(n => n.Texte == texte))
            {
                return null;
            }

            var note = definition.EstReference
                ? MapDetailNote(definition, texte, notes.Count(x => x.NumeroReference.HasValue) + 1)
                : MapDetailNote(definition, texte, null);
            notes.Add(note);
            return note;
        }

        public DetailNote MapDetailNote(DefinitionNote definitionNote, string texte, int? numeroReference)
        {
            return new DetailNote
            {
                Id = definitionNote.Id,
                SequenceId = definitionNote.SequenceId,
                EnEnteteDeSection = definitionNote.EnEnteteDeSection,
                Texte = texte,
                NumeroReference = numeroReference
            };
        }

        public bool EstVisible(DefinitionNote definitionNote,
            DonneesRapportIllustration donnees, DonneesNote donneesNote)
        {
            return EstVisible(definitionNote, donnees, donneesNote, null);
        }

        public bool EstVisible(DefinitionNote definitionNote,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, Types.Models.SommaireProtections.Protection protection)
        {
            return definitionNote.RegleProduits.EstProduitValide(donnees.Produit) &&
                   EstVisible(definitionNote.Regles, definitionNote, donnees, donneesNote, protection);
        }

        private bool EstVisible(List<RegleNote[]> regles, DefinitionNote definitionNote,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, Types.Models.SommaireProtections.Protection protection)
        {
            return regles == null || !regles.Any() ||
                   regles.Any(r => EstVisible(r, definitionNote, donnees, donneesNote, protection));
        }

        private bool EstVisible(IEnumerable<RegleNote> regles, DefinitionNote definitionNote,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, Types.Models.SommaireProtections.Protection protection)
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
                    case RegleNote.Aucune:
                        break;
                    case RegleNote.NonVisibleParDefaut:
                        result = false;
                        break;
                    case RegleNote.CapitalRevenuAppointSuperieurCapitaux:
                        result = ValiderCapitalRevenuAppointSuperieurCapitaux(donnees, protection);
                        break;
                    case RegleNote.CapitalValeur:
                        result = new[] { Produit.CapitalValeur, Produit.CapitalValeur3 }.Contains(donnees.Produit);
                        break;
                    case RegleNote.ConjointeDernierDeces:
                        result = donnees.TypeAssurance == TypeAssurance.ConjointeDernierDec;
                        break;
                    case RegleNote.ConjointeDernierDecesOuLibere1erDeces:
                        result = donnees.ProtectionsGroupees.Any(pg => pg.ProtectionsAssures.Any(p =>
                            (p.TypeAssurance == TypeAssurance.ConjointeDernierDec ||
                             p.TypeAssurance == TypeAssurance.ConjointeDernierDecLib1er) &&
                            p.EstProtectionContractant == false));
                        break;
                    case RegleNote.ConjointePremierDeces:
                        result = donnees.ProtectionsGroupees.Any(pg => pg.ProtectionsAssures.Any(p =>
                            p.TypeAssurance == TypeAssurance.Conjointe1erDec && p.EstProtectionContractant == false));
                        break;
                    case RegleNote.PrimeAnnuelleVersee:
                        result = _vecteurManager.ObtenirVecteurOuDefaut(
                            donnees.Projections, (int)ProjectionVectorId.TotalAnnualDeposit,
                            TypeProjection.Normal, TypeRendementProjection.Normal).Any(v => v > 0);
                        break;
                    case RegleNote.FrequenceFacturationAnnuelle:
                        result = donnees.Facturation.FrequenceFacturation == TypeFrequenceFacturation.Annuelle;
                        break;
                    case RegleNote.FrequenceFacturationMensuelle:
                        result = donnees.Facturation.FrequenceFacturation == TypeFrequenceFacturation.Mensuelle;
                        break;
                    case RegleNote.AllocationASL:
                        result = donnees.AssuranceSupplementaireLiberee?.Allocations != null &&
                                 (donnees.AssuranceSupplementaireLiberee.MontantAllocationInitial > 0 ||
                                  donnees.AssuranceSupplementaireLiberee.Allocations.Any(x => x.Montant > 0));
                        break;
                    case RegleNote.PresenseDeGarantie:
                        result = donnees.PresenceDeGarantie;
                        break;
                    case RegleNote.PresenceDeSurprimeRenouvellement:
                        result = donnees.ProtectionsGroupees.Any(g =>
                            g.PrimesRenouvellement?.Protections != null && g.PrimesRenouvellement.Protections.Any(p =>
                                p.PresenceSurprime && p.Primes != null && p.Primes.Any()));
                        break;
                    case RegleNote.DiminutionCapitalAssure:
                        result = ValiderDiminutionCapitalAssure(donnees, protection);
                        break;
                    case RegleNote.PresenceConjointNonAssurable:
                        result = ValiderPresenceConjointNonAssurable(donnees, protection);
                        break;
                    case RegleNote.MaladieGraveAvecRemboursementPrime:
                        result = ValiderMaladieGraveAvecRemboursementPrime(donnees, protection);
                        break;
                    case RegleNote.RevenuAppointAccident:
                        result = ValiderRevenuAppointAccident(donnees, protection);
                        break;
                    case RegleNote.RevenuAppointAccidentMaladie:
                        result = ValiderRevenuAppointAccidentMaladie(donnees, protection);
                        break;
                    case RegleNote.FondsTransitoireAucuneDIA:
                        result = donnees.HypothesesInvestissement?.FondsTransitoire?.Directives.All(d =>
                                     d.TypeDirective != TypeDirective.Investissement) ?? false;
                        break;
                    case RegleNote.FondsTransitoireNonUtilise:
                        result = !donnees.HypothesesInvestissement?.FondsTransitoire?.FondsUtilise ?? false;
                        break;
                    case RegleNote.ContratEnVigueur:
                        result = donnees.Etat == Etat.EnVigueur;
                        break;
                    case RegleNote.FraisChargeRachatPourcentagePrimes:
                        result = ValiderFraisChargeRachatPourcentagePrimes(donnees, protection);
                        break;
                    case RegleNote.FraisChargeRachatPourcentageFonds:
                        result = ValiderFraisChargeRachatPourcentageFonds(donnees, protection);
                        break;
                    case RegleNote.PresenceCompteCPV:
                        result = donnees.FondsInvestissement.Any(x =>
                            string.Equals(x.AccountType, "Average5Years") &&
                            string.Equals(x.SubType, "LisseAccount_Rule5"));
                        break;
                    case RegleNote.PresenceCompteSRA:
                        result = donnees.FondsInvestissement.Any(x =>
                            string.Equals(x.AccountType, "Average5Years") &&
                            string.Equals(x.SubType, "SRIAAccount"));
                        break;
                    case RegleNote.PresenceCompteSRD:
                        result = donnees.FondsInvestissement.Any(x =>
                            string.Equals(x.AccountType, "Average5Years") &&
                            string.Equals(x.SubType, "LisseAccountIris_Rule6"));
                        break;
                    case RegleNote.PresenceRegleBoni:
                        result = donnees.Boni.BoniFidelite != BoniFidelite.NonApplicable ||
                                 donnees.Boni.BoniInteret != BoniInteret.Regle0_AucunBoni;
                        break;
                    case RegleNote.MaladieGraveTransition:
                        result = donnees.Produit == Produit.Transition;
                        break;
                    case RegleNote.AssuranceVie:
                        result = donnees.Produit != Produit.Transition;
                        break;
                    case RegleNote.ValeurPlusGrandeZero:
                        result = ValiderValeurPlusGrandeZero(definitionNote, donnees, donneesNote, result);
                        break;
                    case RegleNote.TauxPreferentielPresent:
                        result = protection == null
                            ? donnees.HasPreferentialStatus()
                            : donnees.HasPreferentialStatus(protection);
                        break;
                    case RegleNote.ProtectionHospitalisationPresent:
                        if (protection != null)
                        {
                            result = donnees.ProtectionsPDF.FirstOrDefault(p => p.CodePlan == protection.Plan.CodePlan)
                                         ?.Specification.IsHospitalization ?? false;
                        }
                        break;

                    case RegleNote.AssuranceTraditionnelle:
                        result = donnees.Produit == Produit.Traditionnel;
                        break;
                    case RegleNote.PresenceGarantieContractant:
                        result = donnees.ProtectionsGroupees.Any(pg =>
                            pg.ProtectionsAssures.Any(p => p.EstProtectionContractant));
                        break;
                    case RegleNote.PrimeVersee:
                        result = donnees.Primes.PrimesVersees.Any(p => p.Montant.HasValue);
                        break;
                    case RegleNote.AccesViePresent:
                        result = donnees.Produit == Produit.AccesVie;
                        break;
                    case RegleNote.AccesVieGarantiePresent:
                        result = donnees.ProtectionsPDF.Any(x => CodePlanCategorie.EstAccesVieGarantie(x.CodePlan));
                        break;
                    case RegleNote.AccesVieDifferePresent:
                        result = donnees.ProtectionsPDF.Any(x => CodePlanCategorie.EstAccesVieDiffere(x.CodePlan));
                        break;
                    case RegleNote.AccesVieDifferePlusPresent:
                        result = donnees.ProtectionsPDF.Any(x => CodePlanCategorie.EstAccesVieDifferePlus(x.CodePlan));
                        break;
                    case RegleNote.AccesVieImmediatPlusPresent:
                        result = donnees.ProtectionsPDF.Any(x => CodePlanCategorie.EstAccesVieImmediatPLus(x.CodePlan));
                        break;
                    case RegleNote.AssuranceConjointPremierDeces:
                        result = donnees.TypeAssurance == TypeAssurance.Conjointe1erDec;
                        break;
                    case RegleNote.AssuranceConjointDernierDeces:
                        result = donnees.TypeAssurance == TypeAssurance.ConjointeDernierDec ||
                                 donnees.TypeAssurance == TypeAssurance.ConjointeDernierDecLib1er;
                        break;
                    case RegleNote.ValeurSuperieureOuEgaleZero:
                        result = ValiderValeurSuperieureOuEgaleZero(definitionNote, donnees, donneesNote, result);
                        break;
                    case RegleNote.PresenceClientNonAssurable:
                        result = donnees.Clients.Any(x => x.IsNotAssurable.GetValueOrDefault());
                        break;
                    case RegleNote.AssuranceVieUniverselle:
                        result = donnees.TypeContrat == ContractType.Universal;
                        break;
                    case RegleNote.EclipseDePrimeActivee:
                        result = donnees.Participations?.EstEclipseDePrimeActivee ?? false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(RegleNote));
                }

                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValiderFraisChargeRachatPourcentageFonds(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (donnees.Projections.AnneeDebutProjection > 10)
            {
                return false;
            }

            if (protection != null)
            {
                return protection.EstAvecFraisRachatSelonFonds;
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.EstAvecFraisRachatSelonFonds) ?? false;
        }

        private bool ValiderFraisChargeRachatPourcentagePrimes(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (donnees.Projections.AnneeDebutProjection > 10)
            {
                return false;
            }

            if (protection != null)
            {
                return protection.EstAvecFraisRachatSelonPrimes;
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.EstAvecFraisRachatSelonPrimes) ?? false;
        }

        private bool ValiderRevenuAppointAccidentMaladie(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection != null)
            {
                return protection.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointAccident) &&
                       protection.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointMaladie);
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointAccident) &&
                       p.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointMaladie)) ?? false;
        }

        private bool ValiderRevenuAppointAccident(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection != null)
            {
                return protection.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointAccident) &&
                       !protection.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointMaladie);
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointAccident) &&
                       p.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointMaladie)) ?? false;
        }

        private bool ValiderPresenceConjointNonAssurable(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection != null)
            {
                return protection.Assures?.Any(a => a.EstNonAssurable) ?? false;
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.Assures?.Any(a => a.EstNonAssurable) ?? false) ?? false;
        }

        private bool ValiderMaladieGraveAvecRemboursementPrime(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection != null)
            {
                return protection.Plan.InfoProtection.HasFlag(InfoProtection
                    .MaladieGraveAvecRemboursementPrime);
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.Plan.InfoProtection.HasFlag(
                           InfoProtection.MaladieGraveAvecRemboursementPrime)) ?? false;
        }

        private bool ValiderDiminutionCapitalAssure(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection != null)
            {
                return protection.EstProtectionBase && protection.ReductionCapitalAssure > 0;
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       p.EstProtectionBase && p.ReductionCapitalAssure > 0) ?? false;
        }

        private bool ValiderCapitalRevenuAppointSuperieurCapitaux(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection != null)
            {
                return ValiderRegleCapitalRevenuAppointSuperieurCapitaux(donnees, protection);
            }

            return donnees.Protections?.ProtectionsAssures?.Any(p =>
                       ValiderRegleCapitalRevenuAppointSuperieurCapitaux(donnees, p)) ?? false;
        }

        private bool ValiderValeurPlusGrandeZero(DefinitionNote definitionNote,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, bool result)
        {
            if (!definitionNote.Colonne.HasValue)
            {
                return result;
            }

            if (string.IsNullOrEmpty(donneesNote.IdentifiantGroupeAssure))
            {
                var valeurs = _vecteurManager.ObtenirVecteurOuDefaut(donnees.Projections, definitionNote.Colonne.Value,
                        definitionNote.TypeProjection, definitionNote.TypeRendementProjection);
                result = valeurs != null && !valeurs.All(f => f > 0.0000001);
            }
            else
            {
                var valeurs =
                   _vecteurManager.ObtenirVecteurOuDefautPourGroupeAssure(donnees.Projections,
                        definitionNote.Colonne.Value,
                        definitionNote.TypeProjection, definitionNote.TypeRendementProjection,
                        donneesNote.IdentifiantGroupeAssure);
                result = valeurs != null && !valeurs.All(f => f > 0.0000001);
            }

            return result;
        }

        private bool ValiderValeurSuperieureOuEgaleZero(DefinitionNote definitionNote,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, bool result)
        {
            if (!definitionNote.Colonne.HasValue)
            {
                return result;
            }

            if (string.IsNullOrEmpty(donneesNote.IdentifiantGroupeAssure))
            {
                var valeurs = _vecteurManager.ObtenirVecteurOuDefaut(donnees.Projections, definitionNote.Colonne.Value,
                        definitionNote.TypeProjection, definitionNote.TypeRendementProjection);
                result = valeurs != null && valeurs.Any(f => f >= 0);
            }
            else
            {
                var valeurs =
                   _vecteurManager.ObtenirVecteurOuDefautPourGroupeAssure(donnees.Projections,
                        definitionNote.Colonne.Value,
                        definitionNote.TypeProjection, definitionNote.TypeRendementProjection,
                        donneesNote.IdentifiantGroupeAssure);
                result = valeurs != null && valeurs.Any(f => f >= 0);
            }

            return result;
        }

        private bool ValiderRegleCapitalRevenuAppointSuperieurCapitaux(DonneesRapportIllustration donnees,
            Types.Models.SommaireProtections.Protection protection)
        {
            if (protection == null || (!protection.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointMaladie) &&
                                       !protection.Plan.InfoProtection.HasFlag(InfoProtection.RevenuAppointAccident)))
            {
                return false;
            }

            var indexDebut = donnees.Projections.AnneeDebutProjection;
            var indexFin = _vecteurManager.TrouverIndexSelonAnneeCalendrier(
                donnees.Projections.Projection, protection.DateMaturite?.Year ?? 0);

            var faceAmoutValues =
                donnees.Projections.Projection.SommeValeursToutLesGroupesAssures((int)ProjectionVectorId
                    .FaceAmountInsured);
            var ciBenefitValues =
                donnees.Projections.Projection.SommeValeursToutLesGroupesAssures(
                    (int)ProjectionVectorId.CIBenefitInsured);

            for (var index = indexDebut; index <= indexFin; index++)
            {
                var somme = faceAmoutValues.Length > index
                    ? faceAmoutValues[index]
                    : 0 +
                      ciBenefitValues.Length > index
                        ? ciBenefitValues[index]
                        : 0;

                if (protection.CapitalAssureActuel > somme * 0.02)
                {
                    return true;
                }
            }

            return false;
        }

        private void AjouterNoteEsperanceVie(IList<DetailNote> notes, DefinitionNote definition,
            DonneesRapportIllustration donnees)
        {
            var anneeSurbrillance = donnees.CalculerIndexAnneeSurbrillance(_vecteurManager);
            if (!(donnees.Projections.AnneeDebutProjection <= anneeSurbrillance &&
                  donnees.Projections.AnneeFinProjection >= anneeSurbrillance))
            {
                return;
            }

            switch (donnees.TypeAssurance)
            {
                case TypeAssurance.Individuelle:
                    CreerNote(notes, definition, _formatter.FormatterNote(NoteEsperanceVie.Individuelle));
                    break;
                case TypeAssurance.Conjointe1erDec:
                    CreerNote(notes, definition, _formatter.FormatterNote(NoteEsperanceVie.ConjointePremierDeces));
                    break;
                case TypeAssurance.ConjointeDernierDec:
                case TypeAssurance.ConjointeDernierDecLib1er:
                    var texte = donnees.Protections.ProtectionsAssures.SelectMany(x => x.Assures).Any(x => x.EstNonAssurable)
                        ? _formatter.FormatterNote(NoteEsperanceVie.ConjointeDernierDecesExcluantNonAssurable)
                        : _formatter.FormatterNote(NoteEsperanceVie.ConjointeDernierDeces);
                    CreerNote(notes, definition, texte);
                    break;
                case TypeAssurance.NonApplicable:
                case TypeAssurance.NonDefini:
                case TypeAssurance.ConjointeDernierDecNonAssurable:
                case TypeAssurance.Conjoint:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(donnees.TypeAssurance));
            }
        }

        private void AjouterNoteMessageMoteur(IList<DetailNote> notes, DefinitionNote definition,
            DonneesRapportIllustration donnees)
        {
            var messages = donnees.Projections?.Projection?.Messages;
            switch (definition.TypeProjection)
            {
                case TypeProjection.Normal:
                    switch (definition.TypeRendementProjection)
                    {
                        case TypeRendementProjection.Normal:
                            break;
                        case TypeRendementProjection.RendementDefavorable:
                            messages = donnees.Projections?.ProjectionDefavorable?.Messages;
                            break;
                        case TypeRendementProjection.RendementFavorable:
                            messages = donnees.Projections?.ProjectionFavorable?.Messages;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(definition));
                    }
                    break;
                case TypeProjection.BonSuccessoral:
                    messages = donnees.Projections?.BonSuccessoral?.Messages;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(definition));
            }

            if (messages == null)
            {
                return;
            }

            foreach (var item in messages)
            {
                var message = donnees.Langue == Core.Types.Enums.Language.French ? item.FormattedMessageFr : item.FormattedMessageEn;
                if (!string.IsNullOrEmpty(message))
                {
                    CreerNote(notes, definition, message);
                }
            }
        }

        private void AjouterNoteProtections(ICollection<DetailNote> notes, DefinitionNote definition,
            DonneesRapportIllustration donnees, DonneesNote donneesNote, 
            IEnumerable<Types.Models.SommaireProtections.Protection> protections)
        {
            var protectionsAvecNote =
                protections?.Where(p => EstVisible(definition.Regles, definition, donnees, donneesNote, p)).ToList() ??
                donnees.Protections?.ProtectionsAssures?.Where(protection =>
                    EstVisible(definition.Regles, definition, donnees, donneesNote, protection)).ToList();

            if (protectionsAvecNote == null || !protectionsAvecNote.Any())
            {
                return;
            }

            var note = _formatter.FormatterNote(definition, donnees);
            if (string.IsNullOrEmpty(note))
            {
                return;
            }

            var referenceNote = 1;
            if (notes.Any(n => n.NumeroReference.HasValue))
            {
                referenceNote =
                    notes.Where(n => n.NumeroReference.HasValue).Select(n => n.NumeroReference.Value).Max() + 1;
            }

            notes.Add(MapDetailNote(definition, $"{note}", referenceNote));

            foreach (var protection in protectionsAvecNote)
            {
                protection.ReferenceNotes.Add(referenceNote);
            }
        }
    }
}
