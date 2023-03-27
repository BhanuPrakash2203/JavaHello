using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using IAFG.IA.VE.Impression.Illustration.Types.SectionModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class ProjectionModelFactoryExtensionTests
    {
        private DonneesRapportIllustration _donnees;
        private ChoixAnneesRapport _choixAnneesRapport;

        [TestInitialize]
        public void TestInitialize()
        {
            _donnees = new DonneesRapportIllustration
            {
                Projections = new Projections()
            };
            
            _choixAnneesRapport = new ChoixAnneesRapport();
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenChoixAnneesRapportEstNull_ThenSelectionAgesResultatsEstVide_SelectionAnneesResultatsContientAnnees1A100()
        {
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, null);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().BeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 115).ToArray());
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenStandard_ThenSelectionAgesResultatsContient55_60_65_70_85_100_SelectionAnneesResultatsContientAnnees1A20()
        {
            var projections = new Projections
            {
                Projection = new Projection
                {
                    Columns = new List<Column>()
                }
            };

            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.Standard;

            _donnees.Projections = projections;
            var columnAge = new Column {Value = Enumerable.Range(35, 60).Union(new[] {0}).Select(x => (double) x).OrderBy(i => i).ToArray(), Id = 1};
            _donnees.Projections.Projection.Columns.Add(columnAge);
            _donnees.Projections.Projection.Ages = columnAge.Value;

            var columnContractYear = new Column {Value = Enumerable.Range(0, 60).Union(new[] {0}).Select(x => (double) x).OrderBy(i => i).ToArray(), Id = 2};
            _donnees.Projections.Projection.Columns.Add(columnContractYear);
            _donnees.Projections.Projection.AnneesContrat = columnContractYear.Value.Select(x => (int) x).ToArray();

            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAgesResultats.Should().BeEquivalentTo(new[] { 55, 60, 65, 70, 85, 100 });
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 20).Union(new []{21, 26, 31, 36, 51}));
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenToutesLesAnnees_ThenSelectionAgesResultatsEstVide_SelectionAnneesResultatsContientAnnees1A100()
        {
            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.ToutesLesAnnees;
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().BeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 115).ToArray());
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenAnnees1A20_ThenSelectionAgesResultatsContient55_60_65_70_85_100_SelectionAnneesResultatsContientAnnees1A20()
        {
            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.Annee1A20;
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAgesResultats.Should().BeEquivalentTo(new[] { 55, 60, 65, 70, 85, 100 });
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 20).ToArray());
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenAnnees1A10_15_20_ThenSelectionAgesResultatsContient55_60_65_70_85_100_SelectionAnneesResultatsContientAnnees1A10_15_20()
        {
            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.Annee1A10_15_20;
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAgesResultats.Should().BeEquivalentTo(new[] { 55, 60, 65, 70, 85, 100 });
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 10).Union(new[] { 15, 20 }).ToArray());
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenAnnees1A10Tranche5_ThenSelectionAgesResultatsEstVide_SelectionAnneesResultatsContientAnnees1A10Tranche5()
        {
            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.Annee1A10Tranche5;
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().BeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 10).Union(AnneesAgesTranches5()));
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenAnnees1A10Quinquennaux_ThenSelectionAgesResultatsContientAgesQuinquennaux_SelectionAnneesResultatsContientAnnees1A10()
        {
            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.Annee1A10Quinquennaux;
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAgesResultats.Should().BeEquivalentTo(AnneesAgesTranches5().ToArray());
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 10));
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenChoixAnneesRapportEstNull_ThenSelectionAgesResultatsEstVide_ThenSelectionAnneesContientAnnees1A100()
        {
            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, null);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().BeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 115));
            }
        }

        [TestMethod]
        public void CalculerAnneeSelection_WhenStandard_CommenceA35AnsEtAgeEsperanceVieEst83_ThenSelectionAgesResultatsContient55_60_65_70_85_100_SelectionAnneesResultatsContientAnnees1A20_21_26_31_36_49_51()
        {
            var projections = new Projections
            {

                Projection = new Projection
                {
                    Columns = new List<Column>()
                }
            };

            _choixAnneesRapport.ChoixAnnees = TypeChoixAnneesRapport.Standard;
            _donnees.Projections = projections;
            var columnAge = new Column { Value = Enumerable.Range(35, 60).Union(new[] { 0 }).Select(x => (double)x).OrderBy(i => i).ToArray(), Id = 1 };
            _donnees.Projections.Projection.Columns.Add(columnAge);
            _donnees.Projections.Projection.Ages = columnAge.Value;

            var columnContractYear = new Column { Value = Enumerable.Range(0, 60).Union(new[] { 0 }).Select(x => (double)x).OrderBy(i => i).ToArray(), Id = 2 };
            _donnees.Projections.Projection.Columns.Add(columnContractYear);
            _donnees.Projections.Projection.AnneesContrat = columnContractYear.Value.Select(x => (int)x).ToArray();

            _donnees.Protections = new Protections
            {
                ProtectionsAssures = new List<Protection>
                {
                    new Protection
                    {
                        EstProtectionAssurePrincipal = true,
                        EstProtectionBase = true,
                        Assures = new List<Assure>
                        {
                            new Assure
                            {
                                DateNaissance = DateTime.Today,
                                AgeEsperanceVie = 83
                            }
                        }
                    }
                }
            };

            var resultatModel = new SectionResultatModel();
            resultatModel.CalculerAnneesSelection(_donnees, _choixAnneesRapport);

            using (new AssertionScope())
            {
                resultatModel.SelectionAgesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAgesResultats.Should().BeEquivalentTo(new[] { 55, 60, 65, 70, 85, 100 });
                resultatModel.SelectionAnneesResultats.Should().NotBeNullOrEmpty();
                resultatModel.SelectionAnneesResultats.Should().BeEquivalentTo(Enumerable.Range(1, 20).Union(new[] { 21, 26, 31, 36, 51 }));
            }
        }

        private static IEnumerable<int> AnneesAgesTranches5()
        {
            var anneesAgesTranches5 = new List<int>();
            for (var i = 15; i <= 115; i+=5)
            {
                anneesAgesTranches5.Add(i);
            }

            return anneesAgesTranches5;
        }
    }
}