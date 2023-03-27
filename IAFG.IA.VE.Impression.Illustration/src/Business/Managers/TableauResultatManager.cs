using System;
using System.Collections.Generic;
using System.Linq;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;

namespace IAFG.IA.VE.Impression.Illustration.Business.Managers
{
    public class TableauResultatManager : ITableauResultatManager
    {
        private readonly IVecteurManager _vecteurManager;
        private readonly IModelMapper _modelMapper;
        private readonly IIllustrationReportDataFormatter _formatter;
        private readonly IResourcesAccessor _resourcesAccessor;

        public TableauResultatManager(
            IVecteurManager vecteurManager,
            IModelMapper modelMapper,
            IIllustrationReportDataFormatter formatter,
            IIllustrationResourcesAccessorFactory resourcesAccessorFactory)
        {
            _vecteurManager = vecteurManager;
            _modelMapper = modelMapper;
            _formatter = formatter;
            _resourcesAccessor = resourcesAccessorFactory.GetResourcesAccessor();
        }

        public List<TableauResultatViewModel> MapperTableaux(TableauResultat source, SectionResultatModel model)
        {
            var result = new List<TableauResultatViewModel>();
            var tableau = MapperTableauResultat(source, model);
            if (tableau != null)
            {
                result.Add(tableau);
            }

            return result;
        }

        public List<TableauResultatViewModel> MapperTableaux(SectionResultatParAssureModel model)
        {
            return model.SectionResultatModels
                .Select(m => MapperTableauResultat(m.Tableau, m))
                .Where(tableau => tableau != null).ToList();
        }

        private TableauResultatViewModel MapperTableauResultat(TableauResultat source, SectionResultatModel model)
        {
            var groupesCols = source.GroupeColonnes
                ?.Where(g => g.DefinitionColonnes != null && g.DefinitionColonnes.Any(c => c.Visible)).ToArray();
            var definitionColonnes = new List<ColonneTableau>();

            if (groupesCols != null)
            {
                foreach (var groupe in groupesCols)
                {
                    definitionColonnes.AddRange(groupe.DefinitionColonnes.Where(c => c.Visible));
                }
            }

            var viewModel = new TableauResultatViewModel
            {
                TitreTableau = source.TitreTableau,
                DescriptionTableau = source.DescriptionTableau,
                Avis = source.Avis,
                GroupeColonnes = MapperGroupesColonnes(groupesCols),
                Lignes = MapperLignes(source, model, definitionColonnes)
            };

            //Enlever les notes sans colonne avec la reference...
            var cols = viewModel.GroupeColonnes
                .SelectMany(x => x.Colonnes).ToList();
           
            var colRefs = cols.SelectMany(c => c.NoteReferences).ToList();
            var notes = source.Notes?.Where(x =>
                !x.NumeroReference.HasValue || colRefs.Contains(x.Id)).ToList();

            MapNoteReferences(notes, cols);
            viewModel.NotesEnEnteteDeSection = _modelMapper.MapperNotes(notes, true);
            viewModel.Notes = _modelMapper.MapperNotes(notes, false);
            return viewModel;
        }

        private static List<DefinitionGoupeColonneViewModel> MapperGroupesColonnes(
            IEnumerable<GroupeColonne> groupeColonnes)
        {
            return groupeColonnes.Select(groupe => new DefinitionGoupeColonneViewModel
            {
                Titre = groupe.TitreGroupe,
                CouleurAlternative = groupe.CouleurAlternative,
                LigneVerticaleDroite = groupe.LigneVerticaleDroite,
                LigneVerticaleGauche = groupe.LigneVerticaleGauche ?? !string.IsNullOrWhiteSpace(groupe.TitreGroupe),
                Colonnes = MapperColonnes(groupe.DefinitionColonnes.Where(c => c.Visible))
            }).ToList();
        }

        private static List<DefinitionColonneViewModel> MapperColonnes(IEnumerable<ColonneTableau> colonnes)
        {
            var listGroupes = new List<DefinitionColonneViewModel>();
            foreach (var colonne in colonnes)
            {
                var colonneViewModel = new DefinitionColonneViewModel
                {
                    Titre = colonne.TitreColonne,
                    CouleurAlternative = colonne.CouleurAlternative,
                    TypeColonne = colonne.TypeColonne,
                    NoteReferences = colonne.NoteReferences ?? new List<string>(),
                    References = new List<string>()
                };

                listGroupes.Add(colonneViewModel);
            }

            return listGroupes;
        }

        private static void MapNoteReferences(IList<DetailNote> notes, IEnumerable<DefinitionColonneViewModel> colonnes)
        {
            foreach (var colonne in colonnes)
            {
                MapNoteReferences(notes, colonne);
            }
        }

        private static void MapNoteReferences(IList<DetailNote> notes, DefinitionColonneViewModel colonneViewModel)
        {
            if (colonneViewModel.NoteReferences == null || !colonneViewModel.NoteReferences.Any() || notes == null)
            {
                return;
            }

            foreach (var note in notes.Where(x => x.NumeroReference.HasValue))
            {
                if (colonneViewModel.NoteReferences.Contains(note.Id))
                {
                    colonneViewModel.References.Add(FormatterSuperScript(note.NumeroReference.GetValueOrDefault()));
                }
            }
        }

        private static string FormatterSuperScript(int reference)
        {
            var values = new[] { "⁰", "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹" };
            return reference > 9 ? $"{values[reference / 10]}{values[reference % 10]}" : values[reference];
        }

        private List<LigneResultatViewModel> MapperLignes(TableauResultat source, SectionResultatModel model,
            IList<ColonneTableau> definitionColonnes)
        {
            var lignes = new List<LigneResultatViewModel>();
            if (model.DonneesIllustration.Projections?.Projection == null)
            {
                return lignes;
            }

            switch (source.TypeTableau)
            {
                case TypeTableau.Base:
                    lignes.AddRange(ConstruireLignesBase(definitionColonnes,
                        model.DonneesIllustration.Projections,
                        new InfoLigne
                        {
                            SelectionAges = model.SelectionAgesResultats,
                            SelectionAnnees = model.SelectionAnneesResultats
                        }));
                    break;
                case TypeTableau.Contrat:
                    lignes.AddRange(ConstruireLignes(definitionColonnes,
                                                     model.DonneesIllustration.Projections,
                                                     new InfoLigne
                                                     {
                                                         AgeReferenceFinProjection = model.AgeReferenceFinProjection,
                                                         IndexFinProjection = model.DonneesIllustration.Projections.IndexFinProjection,
                                                         SelectionAges = model.SelectionAgesResultats,
                                                         SelectionAnnees = model.SelectionAnneesResultats
                                                     }));
                    break;
                case TypeTableau.TestSensibilite:
                    lignes.AddRange(ConstruireLignesTestSensibilite(definitionColonnes,
                                                     model.DonneesIllustration.Projections,
                                                     new InfoLigne
                                                     {
                                                         AgeReferenceFinProjection = model.AgeReferenceFinProjection,
                                                         IndexFinProjection = model.DonneesIllustration.Projections.IndexFinProjection,
                                                         SelectionAges = model.SelectionAgesResultats,
                                                         SelectionAnnees = model.SelectionAnneesResultats
                                                     }));
                    break;
                case TypeTableau.Assure:
                case TypeTableau.AssurePrincipal:
                case TypeTableau.AssureAdditionnel:
                    var infoAssure =
                        model.DonneesIllustration.InformationTableauAssures.FirstOrDefault(i =>
                            i.IdAssure == source.IdentifiantGroupeAssure);

                    lignes.AddRange(ConstruireLignes(definitionColonnes,
                                                     model.DonneesIllustration.Projections,
                                                     new InfoLigne
                                                     {
                                                         EstTableauGroupeAssure = true,
                                                         IdentifiantGroupeAssure = source.IdentifiantGroupeAssure,
                                                         AgeReferenceFinProjection = model.AgeReferenceFinProjection,
                                                         IndexFinProjection = infoAssure?.IndexFinProjection ?? model.DonneesIllustration.Projections.IndexFinProjection,
                                                         SelectionAges = model.SelectionAgesResultats,
                                                         SelectionAnnees = model.SelectionAnneesResultats
                                                     }));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }

            if (source.InclureLigneDecheance)
            {
                lignes.Add(ContruireLigneDecheance(model.DonneesIllustration.Projections.AnneeFinProjection + 1,
                                                   model.DonneesIllustration.Projections.AgeFinProjection + 1,
                                                   definitionColonnes));
            }

            return lignes;
        }

        internal LigneResultatViewModel ContruireLigneDecheance(double annee, double age,
            IEnumerable<ColonneTableau> definitionColonnes)
        {
            var ligne = new LigneResultatViewModel();
            foreach (var colonne in definitionColonnes)
            {
                ContruireLigneDecheance(annee, age, ligne, colonne);
            }

            return ligne;
        }

        internal void ContruireLigneDecheance(double annee, double age,
            LigneResultatViewModel ligne, ColonneTableau colonne)
        {
            switch (colonne.TypeColonne)
            {
                case TypeColonne.Annee:
                    ligne.Valeurs.Add(annee);
                    ligne.ValeursAffichage.Add(_formatter.GetValueFormatter(colonne.TypeAffichageValeur).Format(annee));
                    break;
                case TypeColonne.Age:
                    ligne.Valeurs.Add(age);
                    ligne.ValeursAffichage.Add(_formatter.GetValueFormatter(colonne.TypeAffichageValeur).Format(age));
                    break;
                case TypeColonne.Normale:
                    ligne.Valeurs.Add(null);
                    ligne.ValeursAffichage.Add(_resourcesAccessor.GetStringResourceById("Dechue"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(colonne));
            }
        }

        internal IEnumerable<LigneResultatViewModel> ConstruireLignesBase(
            IList<ColonneTableau> definitionColonnes, Projections projections, InfoLigne infoLigne)
        {
            var lignes = new List<LigneResultatViewModel>();
            var annees = projections?.Projection?.AnneesContrat;
            var ages = projections?.Projection?.Ages ?? new double[0];
            if (annees == null || !annees.Any())
            {
                return lignes;
            }

            var anneeFinProjection = annees.LastOrDefault(v => v > 0);
            var indexFinProjection = anneeFinProjection > 0 ? Array.LastIndexOf(annees, anneeFinProjection) : -1;

            var indexAgeMax = Array.LastIndexOf(ages,  projections.AgeReferenceFinProjection);
            indexAgeMax = infoLigne.IndexFinProjection > indexAgeMax ? projections.IndexFinProjection : indexAgeMax;

            for (var indexValeur = 1; indexValeur <= indexFinProjection; indexValeur++)
            {
                var annee = annees[indexValeur];
                var age = ages.Length > indexValeur ? (int)ages[indexValeur] : 0;
                if (infoLigne.SelectionAnnees != null &&
                    infoLigne.SelectionAnnees.Any() &&
                    !infoLigne.SelectionAnnees.Contains(annee) &&
                    infoLigne.SelectionAges != null &&
                    !infoLigne.SelectionAges.Contains(age) &&
                    (indexAgeMax == 0 || indexValeur != indexAgeMax))
                {
                    continue;
                }

                var ligneView = new LigneResultatViewModel();
                foreach (var colonne in definitionColonnes)
                {
                    var valeurs = _vecteurManager.ObtenirVecteur(projections, colonne.ColonneMoteur, colonne.TypeProjection, colonne.TypeRendementProjection);
                    AjouterValeurs(valeurs, indexValeur, colonne, ligneView);
                }

                lignes.Add(ligneView);
            }

            return lignes;
        }

        internal IEnumerable<LigneResultatViewModel> ConstruireLignes(IList<ColonneTableau> definitionColonnes,
            Projections projections, InfoLigne infoLigne)
        {
            var lignes = new List<LigneResultatViewModel>();

            var colonneAnnees =
            (from def in definitionColonnes
             where def.TypeColonne.Equals(TypeColonne.Annee)
             select def).FirstOrDefault();

            if (colonneAnnees == null)
            {
                return lignes;
            }

            var annees = _vecteurManager.ObtenirVecteur(projections, colonneAnnees.ColonneMoteur, colonneAnnees.TypeProjection, colonneAnnees.TypeRendementProjection);
            if (annees == null || !annees.Any())
            {
                return lignes;
            }

            var colonneAges =
                (from def in definitionColonnes
                 where def.TypeColonne.Equals(TypeColonne.Age)
                 select def).FirstOrDefault();

            if (colonneAges == null)
            {
                return lignes;
            }

            var ages = infoLigne.EstTableauGroupeAssure
                ? _vecteurManager.ObtenirVecteurPourGroupeAssure(projections, colonneAges.ColonneMoteur, colonneAges.TypeProjection, colonneAges.TypeRendementProjection,
                    infoLigne.IdentifiantGroupeAssure)
                : _vecteurManager.ObtenirVecteur(projections, colonneAges.ColonneMoteur, colonneAges.TypeProjection, colonneAges.TypeRendementProjection);

            if (ages == null || !ages.Any())
            {
                return lignes;
            }

            var indexAgeMax = Array.LastIndexOf(ages, infoLigne.AgeReferenceFinProjection);
            indexAgeMax = infoLigne.IndexFinProjection > indexAgeMax ? infoLigne.IndexFinProjection : indexAgeMax;

            var anneeDebutProjection = annees.FirstOrDefault(v => v > 0);
            var anneeFinProjection = annees.LastOrDefault(v => v > 0);

            var indexMax = Math.Min(infoLigne.IndexFinProjection, annees.GetUpperBound(0));
            for (var indexValeur = annees.GetLowerBound(0); indexValeur <= indexMax; indexValeur++)
            {
                var annee = (int)annees[indexValeur];
                if (indexAgeMax > 0 && indexValeur > indexAgeMax)
                {
                    break;
                }

                var age = ages.Count() > indexValeur ? (int)ages[indexValeur] : 0;

                if (annee == 0)
                {
                    continue;
                }

                if (infoLigne.SelectionAnnees != null &&
                    infoLigne.SelectionAnnees.Any() &&
                    !infoLigne.SelectionAnnees.Contains(annee) &&
                    infoLigne.SelectionAges != null &&
                    !infoLigne.SelectionAges.Contains(age) &&
                    (indexAgeMax == 0 || indexValeur != indexAgeMax))
                {
                    continue;
                }

                if ((annee < anneeDebutProjection || annee > anneeFinProjection) &&
                    infoLigne.SelectionAges != null &&
                    !infoLigne.SelectionAges.Contains(age))
                {
                    continue;
                }

                var ligneView = new LigneResultatViewModel();
                foreach (var defColonne in definitionColonnes)
                {
                    var vecteurColonne = ObtenirVecteurColonne(projections, defColonne);
                    if (vecteurColonne == null) continue;
                    ValiderTypeAffichage(defColonne, vecteurColonne);
                    AjouterValeurs(vecteurColonne, indexValeur, defColonne, ligneView);
                }

                lignes.Add(ligneView);
            }

            return lignes;
        }

        private IEnumerable<LigneResultatViewModel> ConstruireLignesTestSensibilite(
            IList<ColonneTableau> definitionColonnes, Projections projections,
            InfoLigne infoLigne)
        {
            var lignes = new List<LigneResultatViewModel>();

            var colonneAnnees = (from def in definitionColonnes
                                 where def.TypeColonne.Equals(TypeColonne.Annee)
                                 select def).First();

            var annees = _vecteurManager.ObtenirVecteur(projections, colonneAnnees.ColonneMoteur, colonneAnnees.TypeProjection, colonneAnnees.TypeRendementProjection).ToList();
            if (!annees.Any())
            {
                return lignes;
            }

            var anneesFavorable = _vecteurManager.ObtenirVecteurOuDefaut(projections, colonneAnnees.ColonneMoteur, colonneAnnees.TypeProjection, TypeRendementProjection.RendementFavorable).ToList();
            var anneesDefavorables = _vecteurManager.ObtenirVecteurOuDefaut(projections, colonneAnnees.ColonneMoteur, colonneAnnees.TypeProjection, TypeRendementProjection.RendementDefavorable).ToList();

            var idColonneAges =
                (from def in definitionColonnes
                 where def.TypeColonne.Equals(TypeColonne.Age)
                 select def.ColonneMoteur).FirstOrDefault();

            var ages = _vecteurManager.ObtenirVecteurOuDefaut(projections, idColonneAges, colonneAnnees.TypeProjection, colonneAnnees.TypeRendementProjection).ToList();
            var agesFavorable = _vecteurManager.ObtenirVecteurOuDefaut(projections, idColonneAges, colonneAnnees.TypeProjection, TypeRendementProjection.RendementFavorable).ToList();
            var agesDefavorable = _vecteurManager.ObtenirVecteurOuDefaut(projections, idColonneAges, colonneAnnees.TypeProjection, TypeRendementProjection.RendementDefavorable).ToList();

            if (!infoLigne.SelectionAges.Any() && !infoLigne.SelectionAnnees.Any())
            {
                throw new ArgumentException("La section du tests de sensibilité demande d'avoir des selections d'Années ou d'Ages");
            }

            var lignesAAjouter = new List<int>();
            var anneesManquantes = new List<int>();
            var agesManquants = new List<int>();

            TraiterAnneesManquantes(infoLigne, annees, anneesFavorable, anneesDefavorables,
                lignesAAjouter, anneesManquantes);

            TraiterAgesManquants(infoLigne, ages, agesFavorable, agesDefavorable,
                lignesAAjouter, agesManquants);

            var ageMax = 0;
            var anneeMax = 0;
            var libelleDechue = _resourcesAccessor.GetStringResourceById("Dechue");
            var projectionEnDecheance = projections.Projection.AnneeDecheance.HasValue;
            var projectionFavorableEnDecheance = projections.ProjectionFavorable?.AnneeDecheance.HasValue ?? false;
            var projectionDefavorableEnDecheance = projections.ProjectionDefavorable?.AnneeDecheance.HasValue ?? false;

            foreach (var indexValeur in lignesAAjouter.Distinct().OrderBy(x => x))
            {
                var annee = (int)annees[indexValeur];
                var anneeDefavorable = anneesDefavorables.Count > indexValeur
                    ? (int)anneesDefavorables[indexValeur] : 0;
                var anneeFavorable = anneesFavorable.Count > indexValeur
                    ? (int)anneesFavorable[indexValeur] : 0;

                var ligneView = new LigneResultatViewModel();
                foreach (var defColonne in definitionColonnes)
                {
                    double[] vecteur;
                    bool isDechue;

                    switch (defColonne.TypeRendementProjection)
                    {
                        case TypeRendementProjection.Normal:
                            vecteur = _vecteurManager.ObtenirVecteur(projections, defColonne.ColonneMoteur, defColonne.TypeProjection, defColonne.TypeRendementProjection);
                            isDechue = annee == 0 && (anneeFavorable != 0 || anneeDefavorable != 0) && projectionEnDecheance;

                            if ((defColonne.TypeColonne == TypeColonne.Annee || defColonne.TypeColonne == TypeColonne.Age) && isDechue)
                            {
                                if (anneeFavorable != 0)
                                {
                                    isDechue = false;
                                    vecteur = _vecteurManager.ObtenirVecteur(projections, defColonne.ColonneMoteur, defColonne.TypeProjection, TypeRendementProjection.RendementFavorable);
                                }
                                else if (anneeDefavorable != 0)
                                {
                                    isDechue = false;
                                    vecteur = _vecteurManager.ObtenirVecteur(projections, defColonne.ColonneMoteur, defColonne.TypeProjection, TypeRendementProjection.RendementDefavorable);
                                }
                            }

                            if (vecteur == null)
                            {
                                continue;
                            }

                            if (defColonne.TypeColonne == TypeColonne.Annee)
                            {
                                anneeMax = (int)vecteur[indexValeur];
                            }
                            else if (defColonne.TypeColonne == TypeColonne.Age)
                            {
                                ageMax = (int)vecteur[indexValeur];
                            }

                            break;

                        case TypeRendementProjection.RendementDefavorable:
                            isDechue = anneeDefavorable == 0 && (annee != 0 || anneeFavorable != 0) && projectionDefavorableEnDecheance;
                            vecteur = _vecteurManager.ObtenirVecteur(projections, defColonne.ColonneMoteur, defColonne.TypeProjection, defColonne.TypeRendementProjection);
                            if (vecteur == null) continue;
                            break;

                        case TypeRendementProjection.RendementFavorable:
                            isDechue = anneeFavorable == 0 && (annee != 0 || anneeDefavorable != 0) && projectionFavorableEnDecheance;
                            vecteur = _vecteurManager.ObtenirVecteur(projections, defColonne.ColonneMoteur, defColonne.TypeProjection, defColonne.TypeRendementProjection);
                            if (vecteur == null) continue;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(defColonne.TypeRendementProjection));
                    }

                    if (isDechue)
                    {
                        ligneView.Valeurs.Add(null);
                        ligneView.ValeursAffichage.Add(libelleDechue);
                    }
                    else
                    {
                        ValiderTypeAffichage(defColonne, vecteur);
                        AjouterValeurs(vecteur, indexValeur, defColonne, ligneView);
                    }
                }

                lignes.Add(ligneView);
            }

            var anneeMin = (int)annees.FirstOrDefault(x => (int)x != 0);
            var ageMin = (int)ages.FirstOrDefault(x => (int)x != 0);
            var deltas = new List<int>();
            if (anneeMin > 0) deltas.AddRange(anneesManquantes.Where(x => x > anneeMin).Select(x => x - anneeMax));
            if (ageMin > 0) deltas.AddRange(agesManquants.Where(x => x > ageMin).Select(x => x - ageMax));

            foreach (var delta in deltas.Distinct().OrderBy(x => x))
            {
                lignes.Add(ContruireLigneDecheance(anneeMax + delta, ageMax + delta, definitionColonnes));
            }

            return lignes;
        }

        private void TraiterAgesManquants(InfoLigne infoLigne, IList<double> ages, IList<double> agesFavorable,
            IList<double> agesDefavorable, ICollection<int> lignesAAjouter, ICollection<int> agesManquants)
        {
            foreach (var age in infoLigne.SelectionAges)
            {
                var idx = ages.IndexOf(age);
                if (idx >= 0)
                {
                    lignesAAjouter.Add(idx);
                }
                else
                {
                    idx = agesFavorable.IndexOf(age);
                    if (idx >= 0)
                    {
                        lignesAAjouter.Add(idx);
                    }
                    else
                    {
                        idx = agesDefavorable.IndexOf(age);
                        if (idx >= 0)
                        {
                            lignesAAjouter.Add(idx);
                        }
                        else
                        {
                            agesManquants.Add(age);
                        }
                    }
                }
            }
        }

        private void TraiterAnneesManquantes(InfoLigne infoLigne, IList<double> annees, IList<double> anneesFavorable,
            IList<double> anneesDefavorables, ICollection<int> lignesAAjouter, ICollection<int> anneesManquantes)
        {
            foreach (var annee in infoLigne.SelectionAnnees)
            {
                var idx = annees.IndexOf(annee);
                if (idx >= 0)
                {
                    lignesAAjouter.Add(idx);
                }
                else
                {
                    idx = anneesFavorable.IndexOf(annee);
                    if (idx >= 0)
                    {
                        lignesAAjouter.Add(idx);
                    }
                    else
                    {
                        idx = anneesDefavorables.IndexOf(annee);
                        if (idx >= 0)
                        {
                            lignesAAjouter.Add(idx);
                        }
                        else
                        {
                            anneesManquantes.Add(annee);
                        }
                    }
                }
            }
        }

        private double[] ObtenirVecteurColonne(Projections projections, ColonneTableau colonne)
        {
            var result = !string.IsNullOrEmpty(colonne.IdentifiantGroupeAssure)
                ? ObtenirValeursColonneGroupeAssure(colonne.IdentifiantGroupeAssure, projections, colonne)
                : _vecteurManager.ObtenirVecteurOuDefaut(projections, colonne.ColonneMoteur, colonne.TypeProjection, colonne.TypeRendementProjection);

            return result;
        }

        private double[] ObtenirValeursColonneGroupeAssure(string identifiantGroupeAssure,
            Projections projections, ColonneTableau colonne)
        {
            if (colonne.TypeColonne.Equals(TypeColonne.Annee))
            {
                return _vecteurManager.ObtenirVecteurOuDefaut(projections, 
                    colonne.ColonneMoteur, colonne.TypeProjection, colonne.TypeRendementProjection);
            }

            return _vecteurManager.ObtenirVecteurOuDefautPourGroupeAssure(projections, 
                colonne.ColonneMoteur, colonne.TypeProjection, colonne.TypeRendementProjection,
                identifiantGroupeAssure);
        }

        private static void ValiderTypeAffichage(ColonneTableau defColonne, IEnumerable<double> result)
        {
            if (defColonne.TypeAffichageValeur == TypeAffichageValeur.Decimale && result.Any(x => x > 9000000))
            {
                defColonne.TypeAffichageValeur = TypeAffichageValeur.SansDecimale;
            }
        }

        private void AjouterValeurs(double[] valeurs, int indexValeur,
            ColonneTableau colonne, LigneResultatViewModel ligneView)
        {
            if (valeurs == null || indexValeur > valeurs.GetUpperBound(0))
            {
                ligneView.Valeurs.Add(null);
                ligneView.ValeursAffichage.Add("");
            }
            else
            {
                var valeur = valeurs[indexValeur];
                var valeurFormater = _formatter.GetValueFormatter(colonne.TypeAffichageValeur).Format(valeur);
                ligneView.Valeurs.Add(valeur);
                ligneView.ValeursAffichage.Add(valeurFormater);
            }
        }

        internal class InfoLigne
        {
            public int AgeReferenceFinProjection { get; set; }
            public bool EstTableauGroupeAssure { get; set; }
            public string IdentifiantGroupeAssure { get; set; }
            public int IndexFinProjection { get; set; }
            public int[] SelectionAnnees { get; set; }
            public int[] SelectionAges { get; set; }
        }
    }

}
