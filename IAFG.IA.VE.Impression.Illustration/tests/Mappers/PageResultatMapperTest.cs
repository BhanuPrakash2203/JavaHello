using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.ResourcesAccessor;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Extensions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Tests;
using IAFG.IA.VE.Impression.Illustration.Tests.Helpers;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Test.Mappers
{
    [TestClass]
    public class PageResultatMapperTest
    {
        private static readonly IIllustrationResourcesAccessorFactory ResourceAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private static readonly IIllustrationReportDataFormatter ReportDataFormatter = Helpers.CreateIllustrationReportDataFormatter();
        private const int CONTRACT_YEAR_COLID = 1;
        private const int AGE_COLID = 2;
        private static readonly TableauResultat Tableau = Auto.Create<TableauResultat>();
        private static readonly SectionResultatModel Model = Auto.Create<SectionResultatModel>();
        private readonly IModelMapper _modelMapper = new ModelMapper();

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ResourceAccessorFactory.Contexte = ResourcesContexte.Illustration;
            ResourceAccessorFactory.GetResourcesAccessor().Returns(new ImpressionResourcesAccessor(new IllustrationsResourcesSequence(Helpers.CreateCultureAccessor())));
        }

        [TestMethod]
        public void ShouldMapTableauTestSensibilite()
        {
            Tableau.TypeTableau = TypeTableau.TestSensibilite;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(string.Empty) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections = CreerVecteurIllustration(0, 0, 1, 30, new[] { 4 }, 20);
            Model.DonneesIllustration.Projections.IndexFinProjection = 20;

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();

            viewModel.TitreTableau.Should().Be(Tableau.TitreTableau);
            viewModel.Should().BeEquivalentTo(Tableau,
                options => options.Excluding(o => o.Avis)
                    .Excluding(o => o.Notes)
                    .Excluding(o => o.GroupeColonnes)
                    .Excluding(o => o.InclureLigneDecheance)
                    .Excluding(o => o.IdentifiantGroupeAssure)
                    .Excluding(o => o.TypeTableau));

            viewModel.Avis.Should().BeEquivalentTo(Tableau.Avis);

            var notesTriees = Tableau.Notes.Where(x => !x.EnEnteteDeSection).OrderBy(x => x.SequenceId)
                .Select(x => x.Texte).ToList();
            viewModel.Notes.Should().BeEquivalentTo(notesTriees);

            var notesEntetesTriees = Tableau.Notes.Where(x => x.EnEnteteDeSection).OrderBy(x => x.SequenceId)
                .Select(x => x.Texte).ToList();
            viewModel.NotesEnEnteteDeSection.Should().BeEquivalentTo(notesEntetesTriees);
        }

        [TestMethod]
        public void ShouldMapTableau()
        {
            Tableau.TypeTableau = TypeTableau.Contrat;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(string.Empty) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections = CreerVecteurIllustration(0, 0, 1, 30, new[] {4}, 20);
            Model.DonneesIllustration.Projections.IndexFinProjection = 20;

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();

            viewModel.TitreTableau.Should().Be(Tableau.TitreTableau);
            viewModel.Should().BeEquivalentTo(Tableau,
                                           options => options.Excluding(o => o.Avis)
                                                             .Excluding(o => o.Notes)
                                                             .Excluding(o => o.GroupeColonnes)
                                                             .Excluding(o => o.InclureLigneDecheance)
                                                             .Excluding(o => o.IdentifiantGroupeAssure)
                                                             .Excluding(o => o.TypeTableau));

            viewModel.Avis.Should().BeEquivalentTo(Tableau.Avis);

            var notesTriees = Tableau.Notes.Where(x => !x.EnEnteteDeSection).OrderBy(x => x.SequenceId)
                .Select(x => x.Texte).ToList();
            viewModel.Notes.Should().BeEquivalentTo(notesTriees);

            var notesEntetesTriees = Tableau.Notes.Where(x => x.EnEnteteDeSection).OrderBy(x => x.SequenceId)
                .Select(x => x.Texte).ToList();
            viewModel.NotesEnEnteteDeSection.Should().BeEquivalentTo(notesEntetesTriees);
        }

        [TestMethod]
        public void ShouldMapGroupesColonnes()
        {
            Tableau.TypeTableau = TypeTableau.Contrat;
            var gc1 = Auto.Create<GroupeColonne>();
            var gc2 = Auto.Create<GroupeColonne>();
            var gc3 = Auto.Create<GroupeColonne>();
            var gc4 = Auto.Create<GroupeColonne>();
            var gcList = new List<GroupeColonne> { gc1, gc2, gc3, gc4 };
            Tableau.GroupeColonnes = gcList;

            var c11 = Auto.Create<ColonneTableau>();
            var c12 = Auto.Create<ColonneTableau>();
            var c13 = Auto.Create<ColonneTableau>();
            c11.Visible = c12.Visible = c13.Visible = true;
            c11.ColonneMoteur = CONTRACT_YEAR_COLID;
            c11.TypeColonne = TypeColonne.Annee;
            c12.ColonneMoteur = AGE_COLID;
            c12.TypeColonne = TypeColonne.Age;
            gc1.DefinitionColonnes = new[] {c11, c12, c13};

            var c21 = Auto.Create<ColonneTableau>();
            var c22 = Auto.Create<ColonneTableau>();
            var c23 = Auto.Create<ColonneTableau>();
            c21.Visible = c22.Visible = c23.Visible = true;
            c21.TypeColonne = c22.TypeColonne = c23.TypeColonne = TypeColonne.Normale;
            gc2.DefinitionColonnes = new[] { c21, c22, c23 };

            var c31 = Auto.Create<ColonneTableau>();
            var c32 = Auto.Create<ColonneTableau>();
            c31.Visible = true;
            c32.Visible = false;
            c31.TypeColonne = c32.TypeColonne = TypeColonne.Normale;
            gc3.DefinitionColonnes = new[] { c31, c32 };

            gc4.DefinitionColonnes = null;
            Model.DonneesIllustration.Projections = CreerVecteurIllustration(CONTRACT_YEAR_COLID, 1, AGE_COLID, 20, new[] {5001, 5002, 5003, 5004, 5005, 5006}, 20);
            Model.DonneesIllustration.Projections.IndexFinProjection = 19;

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            using (new AssertionScope())
            {
                for (var i = 0; i < viewModel.GroupeColonnes.Count; i++)
                {
                    viewModel.GroupeColonnes[i].Titre.Should().Be(gcList[i].TitreGroupe);
                    for (var c = 0; c < viewModel.GroupeColonnes[i].Colonnes.Count; c++)
                    {
                        var col = viewModel.GroupeColonnes[i].Colonnes[c];
                        col.Titre.Should().Be(gcList[i].DefinitionColonnes[c].TitreColonne);
                        switch (gcList[i].DefinitionColonnes[c].ColonneMoteur)
                        {
                            case CONTRACT_YEAR_COLID:
                                col.TypeColonne.Should().Be(TypeColonne.Annee);
                                break;
                            case AGE_COLID:
                                col.TypeColonne.Should().Be(TypeColonne.Age);
                                break;
                            default:
                                if (col.TypeColonne != TypeColonne.Annee && col.TypeColonne != TypeColonne.Age)
                                {
                                    col.TypeColonne.Should().Be(TypeColonne.Normale);
                                }

                                break;
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldMapGroupesColonnesVisiblesSeulement()
        {
            Tableau.TypeTableau = TypeTableau.Contrat;
            var gc1 = Auto.Create<GroupeColonne>();
            Tableau.GroupeColonnes = new List<GroupeColonne> { gc1 };

            var c11 = Auto.Create<ColonneTableau>();
            var c12 = Auto.Create<ColonneTableau>();
            var c13 = Auto.Create<ColonneTableau>();
            c11.Visible = c12.Visible = true;
            c13.Visible = false;

            c11.ColonneMoteur = CONTRACT_YEAR_COLID;
            c11.TypeColonne = TypeColonne.Annee;
            c12.ColonneMoteur = AGE_COLID;
            c12.TypeColonne = TypeColonne.Age;

            gc1.DefinitionColonnes = new[] { c11, c12, c13 };

            var colonnesVisibles = new[] { c11, c12 };

            Model.DonneesIllustration.Projections = CreerVecteurIllustration(CONTRACT_YEAR_COLID, 1, AGE_COLID, 20, new[] { 5001 }, 20);
            Model.DonneesIllustration.Projections.IndexFinProjection = 19;

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();

            using (new AssertionScope())
            {
                viewModel.GroupeColonnes[0].Titre.Should().Be(gc1.TitreGroupe);
                for (var c = 0; c < viewModel.GroupeColonnes[0].Colonnes.Count; c++)
                {
                    var col = viewModel.GroupeColonnes[0].Colonnes[c];
                    col.Titre.Should().Be(colonnesVisibles[c].TitreColonne);
                }
            }
            
        }

        [TestMethod]
        public void ShouldMapLignes()
        {
            var annees = new double[] {10,11,12};
            var vecteursIllustration = CreerVecteurIllustration(CONTRACT_YEAR_COLID, 10, AGE_COLID, 40, new[] { 9997, 9998, 9999}, 3);
            Model.AnneeDebutProjection = (int)annees.First();
            Model.AnneeFinProjection = (int)annees.Last();
            Model.AgeReferenceFinProjection = 100;
            Model.SelectionAnneesResultats = annees.Select(a => (int)a).ToArray();
            Model.DonneesIllustration.Projections = vecteursIllustration;
            Model.DonneesIllustration.Projections.IndexFinProjection = 3;

            var gc1 = Auto.Create<GroupeColonne>();
            Tableau.GroupeColonnes = new List<GroupeColonne> { gc1 };

            var c1 = Auto.Create<ColonneTableau>();
            var c2 = Auto.Create<ColonneTableau>();
            var c3 = Auto.Create<ColonneTableau>();
            var c4 = Auto.Create<ColonneTableau>();
            var c5 = Auto.Create<ColonneTableau>();
            c1.TypeAffichageValeur = c2.TypeAffichageValeur = TypeAffichageValeur.SansDecimale;
            c3.TypeAffichageValeur = c4.TypeAffichageValeur = c5.TypeAffichageValeur = TypeAffichageValeur.Decimale;
            c1.Visible = c2.Visible = c3.Visible = c4.Visible = c5.Visible = true;

            c1.ColonneMoteur = CONTRACT_YEAR_COLID;
            c1.TypeColonne = TypeColonne.Annee;
            c2.ColonneMoteur = AGE_COLID;
            c2.TypeColonne = TypeColonne.Age;
            c3.ColonneMoteur = 9997;
            c4.ColonneMoteur = 9998;
            c5.ColonneMoteur = 9999;
            c3.TypeColonne = c4.TypeColonne = c5.TypeColonne = TypeColonne.Normale;

            gc1.DefinitionColonnes = new[] { c1, c2, c3, c4, c5 };
            Tableau.TypeTableau = TypeTableau.Contrat;
            Tableau.InclureLigneDecheance = false;

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();

            var vecteurManager = new VecteurManager();
            using (new AssertionScope())
              {
                for (var i = 0; i < viewModel.Lignes.Count; i++)
                {
                    var ligne = viewModel.Lignes[i];
                    ligne.Valeur01.ToString().Should().BeEquivalentTo(ReportDataFormatter.GetValueFormatter(TypeAffichageValeur.SansDecimale).Format(vecteurManager.ObtenirVecteur(vecteursIllustration, c1.ColonneMoteur, TypeProjection.Normal, TypeRendementProjection.Normal)[i + 1]));
                    ligne.Valeur02.ToString().Should().BeEquivalentTo(ReportDataFormatter.GetValueFormatter(TypeAffichageValeur.SansDecimale).Format(vecteurManager.ObtenirVecteur(vecteursIllustration, c2.ColonneMoteur, TypeProjection.Normal, TypeRendementProjection.Normal)[i + 1]));
                    ligne.Valeur03.ToString().Should().BeEquivalentTo(ReportDataFormatter.GetValueFormatter(TypeAffichageValeur.NonDefeni).Format(vecteurManager.ObtenirVecteur(vecteursIllustration, c3.ColonneMoteur, TypeProjection.Normal, TypeRendementProjection.Normal)[i + 1]));
                    ligne.Valeur04.ToString().Should().BeEquivalentTo(ReportDataFormatter.GetValueFormatter(TypeAffichageValeur.NonDefeni).Format(vecteurManager.ObtenirVecteur(vecteursIllustration, c4.ColonneMoteur, TypeProjection.Normal, TypeRendementProjection.Normal)[i + 1]));
                    ligne.Valeur05.ToString().Should().BeEquivalentTo(ReportDataFormatter.GetValueFormatter(TypeAffichageValeur.NonDefeni).Format(vecteurManager.ObtenirVecteur(vecteursIllustration, c5.ColonneMoteur, TypeProjection.Normal, TypeRendementProjection.Normal)[i + 1]));
                }
            }
        }
        
        [TestMethod]
        public void MapperTableau_When32Ans_ToutesLesAnnees_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 32;
            const int nbLignesExpected = 101 - ageDebutAssure;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, nbLignesExpected).ToList();
            var agesExpected = Enumerable.Range(ageDebutAssure, nbLignesExpected).ToList();
            var donneesBidonsExpected = Enumerable.Range(8, nbLignesExpected).ToList();
            var lignesResultatsExpected = new List<LigneResultatViewModel>();

            for (var i = 0; i < nbLignesExpected; i++)
            {
                lignesResultatsExpected.Add(new LigneResultatViewModel
                {
                    Valeurs = {anneesExpected[i], agesExpected[i], donneesBidonsExpected[i]}
                });
            }

            Tableau.InclureLigneDecheance = false;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }
            
            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };
            
            Model.SelectionAnneesResultats = Enumerable.Range(1, 100).ToArray();
            Model.SelectionAgesResultats = new int[0];

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When32Ans_Annees1a20EtAges55_60_65_70_85_100_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 32;

            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 20).Union(new [] { 24, 29, 34, 39, 51, 54, 69 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 20).Union(new [] { 55, 60, 65, 70, 82, 85, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 20).Union(new []{ 31, 36, 41, 46, 58, 61, 76 }).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 20).Union(new [] { 51 }).ToArray();
            Model.SelectionAgesResultats = new [] { 55, 60, 65, 70, 85, 100 };

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When32Ans_Annees1a10_15_20EtAges55_60_65_70_85_100_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 32;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 10).Union(new[] { 15, 20, 24, 29, 34, 39, 51, 54, 69 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 10).Union(new[] { 46, 51, 55, 60, 65, 70, 82, 85, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 10).Union(new[] { 22, 27, 31, 36, 41, 46, 58, 61, 76 }).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new[] { 15, 20, 51 }).ToArray();
            Model.SelectionAgesResultats = new[] { 55, 60, 65, 70, 85, 100 };

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When32Ans_Annees1a10Tranches5_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 32;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 10).Union(new[] { 15, 20, 25, 30, 35, 40, 45, 50, 51, 55, 60, 65, 69 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 10).Union(new[] { 46, 51, 56, 61, 66, 71, 76, 81, 82, 86, 91, 96, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 10).Union(new[] { 22, 27, 32, 37, 42, 47, 52, 57, 58, 62, 67, 72, 76 }).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new[] { 15, 20, 25, 30, 35, 40, 45, 50, 51, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 }).ToArray();
            Model.SelectionAgesResultats = new int[0];

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When32Ans_Annees1a10AgesQuinquennaux_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 32;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 10).Union(new[] { 14, 19, 24, 29, 34, 39, 44, 49, 51, 54, 59, 64, 69 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 10).Union(new[] { 45, 50, 55, 60, 65, 70, 75, 80, 82, 85, 90, 95, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 10).Union(new[] { 21, 26, 31, 36, 41, 46, 51, 56, 58, 61, 66, 71, 76 }).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };
            
            Model.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new [] { 51 }).ToArray();
            Model.SelectionAgesResultats = new [] { 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When60Ans_ToutesLesAnnees_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 60;
            const int nbLignesExpected = 101 - ageDebutAssure;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, nbLignesExpected).ToList();
            var agesExpected = Enumerable.Range(ageDebutAssure, nbLignesExpected).ToList();
            var donneesBidonsExpected = Enumerable.Range(8, nbLignesExpected).ToList();
            var lignesResultatsExpected = new List<LigneResultatViewModel>();

            for (var i = 0; i < nbLignesExpected; i++)
            {
                lignesResultatsExpected.Add(new LigneResultatViewModel
                {
                    Valeurs = { anneesExpected[i], agesExpected[i], donneesBidonsExpected[i] }
                });
            }

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }
            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 100).ToArray();
            Model.SelectionAgesResultats = new int[0];

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When60Ans_Annees1a20EtAges55_60_65_70_85_100_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 60;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 20).Union(new[] { 26, 27, 41 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 20).Union(new[] { 85, 86, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 20).Union(new[] { 33, 34, 48 }).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 20).Union(new[] { 27 }).ToArray();
            Model.SelectionAgesResultats = new[] { 55, 60, 65, 70, 85, 100 };

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When60Ans_Annees1a10_15_20EtAges55_60_65_70_85_100_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 60;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 10).Union(new[] { 11, 15, 20, 26, 27, 41 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 10).Union(new[] { 70, 74, 79, 85, 86, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 10).Union(new[] { 18, 22, 27, 33, 34, 48}).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new[] { 15, 20, 27 }).ToArray();
            Model.SelectionAgesResultats = new[] { 55, 60, 65, 70, 85, 100 };

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When60Ans_Annees1a10Tranches5_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 60;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 10).Union(new[] { 15, 20, 25, 27, 30, 35, 40, 41 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 10).Union(new[] { 74, 79, 84, 86, 89, 94, 99, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 10).Union(new[] { 22, 27, 32, 34, 37, 42, 47, 48 }).ToArray();

            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new[] { 15, 20, 25, 27, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 }).ToArray();
            Model.SelectionAgesResultats = new int[0];

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }

        [TestMethod]
        public void MapperTableau_When60Ans_Annees1a10AgesQuinquennaux_ThenVerifierMappingDesDonnees()
        {
            const string idGroupeAssure = "1";
            const int ageDebutAssure = 60;
            Model.Tableau.InclureLigneDecheance = false;
            Model.AnneeDebutProjection = 1;
            Model.AnneeFinProjection = 115;
            Model.AgeReferenceFinProjection = 100;

            var anneesExpected = Enumerable.Range(1, 10).Union(new[] { 11, 16, 21, 26, 27, 31, 36, 41 }).ToArray();
            var agesExpected = Enumerable.Range(ageDebutAssure, 10).Union(new[] { 70, 75, 80, 85, 86, 90, 95, 100 }).ToArray();
            var donneesBidonsExpected = Enumerable.Range(8, 10).Union(new[] { 18, 23, 28, 33, 34, 38, 43, 48 }).ToArray();
            var lignesResultatsExpected = anneesExpected.Select((t, i) => new LigneResultatViewModel
                {
                    Valeurs = {t, agesExpected[i], donneesBidonsExpected[i]}
                })
                .ToList();

            Tableau.InclureLigneDecheance = false;
            Tableau.IdentifiantGroupeAssure = idGroupeAssure;
            Tableau.TypeTableau = TypeTableau.Assure;
            Tableau.GroupeColonnes = new List<GroupeColonne> { GroupeColonne(idGroupeAssure) };
            foreach (var item in Tableau.Notes)
            {
                item.NumeroReference = null;
            }

            Model.DonneesIllustration.Projections.Projection.Columns = ListeColonnes(idGroupeAssure, ageDebutAssure);
            Model.DonneesIllustration.TypeAssurance = TypeAssurance.Individuelle;

            Model.DonneesIllustration.InformationTableauAssures = new List<InformationTableauAssure>
                                                                  {
                                                                      new InformationTableauAssure
                                                                      {
                                                                          IdAssure = idGroupeAssure,
                                                                          IndexFinProjection = 101 - ageDebutAssure
                                                                      }
                                                                  };

            Model.SelectionAnneesResultats = Enumerable.Range(1, 10).Union(new[] { 27 }).ToArray();
            Model.SelectionAgesResultats = new[] { 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };

            var manager = new TableauResultatManager(new VecteurManager(), _modelMapper, ReportDataFormatter, ResourceAccessorFactory);
            var viewModel = manager.MapperTableaux(Tableau, Model).First();
            AssertTableauMappedInViewModel(Tableau, viewModel, lignesResultatsExpected);
        }
        private static void AssertTableauMappedInViewModel(TableauResultat tableau, TableauResultatViewModel viewModel, IList<LigneResultatViewModel> lignesResultatsExpected)
        {
            using (new AssertionScope())
            {
                viewModel.TitreTableau.Should().Be(tableau.TitreTableau);
                viewModel.Avis.Should().BeEquivalentTo(tableau.Avis);
                viewModel.Notes.All(noteTexte => tableau.Notes.Any(note => note.Texte == noteTexte)).Should().BeTrue();
                viewModel.GroupeColonnes.First().Titre.Should().Be(tableau.GroupeColonnes.First().TitreGroupe);
                viewModel.GroupeColonnes.First().Colonnes
                         .All(definitionColonne => tableau.GroupeColonnes.First().DefinitionColonnes.Any(colonneTableau => colonneTableau.TitreColonne == definitionColonne.Titre)).Should().BeTrue();
                viewModel.GroupeColonnes.First().Colonnes
                         .All(definitionColonne => tableau.GroupeColonnes.First().DefinitionColonnes.Any(colonneTableau => colonneTableau.TypeColonne == definitionColonne.TypeColonne)).Should().BeTrue();
                viewModel.Lignes.Should().HaveCount(lignesResultatsExpected.Count);

                for (var i = 0; i < lignesResultatsExpected.Count; i++)
                {
                    viewModel.Lignes[i].EnSurbrillance.Should().Be(lignesResultatsExpected[i].EnSurbrillance);
                    viewModel.Lignes[i].Valeurs.Should().BeEquivalentTo(lignesResultatsExpected[i].Valeurs);
                }
            }
        }
        
        private static GroupeColonne GroupeColonne(string idGroupeAsure)
        {
            var groupeColonnes = new GroupeColonne
            {
                DefinitionColonnes = new List<ColonneTableau>
                {
                    new ColonneTableau
                    {
                        ColonneMoteur = 0,
                        TitreColonne = "Year",
                        TypeColonne = TypeColonne.Annee,
                        Visible = true,
                        TypeAffichageValeur = TypeAffichageValeur.SansDecimale
                    },
                    new ColonneTableau
                    {
                        ColonneMoteur = 1,
                        TitreColonne = "Age",
                        TypeColonne = TypeColonne.Age,
                        Visible = true,
                        TypeAffichageValeur = TypeAffichageValeur.SansDecimale,
                        IdentifiantGroupeAssure = idGroupeAsure
                    },
                    new ColonneTableau
                    {
                        ColonneMoteur = 4,
                        TitreColonne = "Bidon",
                        Visible = true,
                        TypeAffichageValeur = TypeAffichageValeur.SansDecimale,
                        IdentifiantGroupeAssure = idGroupeAsure
                    }
                }
            };

            return groupeColonnes;
        }
        private static List<Column> ListeColonnes(string idGroupeAssure, int ageDebutAssure)
        {
            var listeColonnes = new List<Column>
            {
                new Column
                {
                    Id = 0,
                    Value = ValuesDonnees(1, 115)
                },
                new Column
                {
                    Id = 1,
                    Insured = idGroupeAssure,
                    Value = ValuesDonnees(ageDebutAssure, 115)
                },
                new Column
                {
                    Id = 4,
                    Insured = idGroupeAssure,
                    Value = ValuesDonnees(8, 115)
                }
            };

            return listeColonnes;
        }

        private static double[] ValuesDonnees(int donneeDebut, int totalDonnees)
        {
            var ages = new List<double> { 0 };
            ages.AddRange(Enumerable.Range(donneeDebut, totalDonnees).Select(x => (double)x));

            return ages.ToArray();
        }

        private static Types.Models.Projections.Projections CreerVecteurIllustration(int idAnnee, int anneeDepart, int idAge, int ageDepart, IEnumerable<int> vectorsId, int lenght)
        {
            var vecteur = new Types.Models.Projections.Projections
            {
                Projection = new Types.Models.Projections.Projection
                {
                    Columns = new List<Column>()
                },
                ProjectionFavorable = new Types.Models.Projections.Projection
                {
                    Columns = new List<Column>()
                },
                ProjectionDefavorable = new Types.Models.Projections.Projection
                {
                    Columns = new List<Column>()
                },
            };

            vecteur.Projection.Columns.Add(new Column { Id = idAnnee, Value = ValuesDonnees(anneeDepart, lenght) });
            vecteur.Projection.Columns.Add(new Column { Id = idAge, Value = ValuesDonnees(ageDepart, lenght) });

            foreach (var id in vectorsId)
            {
                vecteur.Projection.Columns.Add(new Column { Id = id, Value = new List<double> { 0 }.Union(Auto.CreateMany<double>(lenght)).ToArray() });
                vecteur.ProjectionFavorable.Columns.Add(new Column { Id = id, Value = new List<double> { 0 }.Union(Auto.CreateMany<double>(lenght)).ToArray() });
                vecteur.ProjectionDefavorable.Columns.Add(new Column { Id = id, Value = new List<double> { 0 }.Union(Auto.CreateMany<double>(lenght)).ToArray() });
            }

            return vecteur;
        }
        
    }
}