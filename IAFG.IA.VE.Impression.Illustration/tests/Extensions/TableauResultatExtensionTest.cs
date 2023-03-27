using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.Core.Interface.ResourcesAccessor;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Resources.Interfaces;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.Projections;
using IAFG.IA.VE.Impression.Illustration.Types.Reports.ViewModels.Resultats;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Extensions
{
    [TestClass]
    public class TableauResultatExtensionTest
    {
        private IIllustrationReportDataFormatter _formatter;
        private Core.Interface.Formatters.IValueFormatter _valueFormatter;
        private IResourcesAccessor _resourcesAccessor;
        private IVecteurManager _vecteurManager;
        private TableauResultatManager _tableauResultatManager;
        private readonly IModelMapper _modelMapper = new ModelMapper();
        private readonly IIllustrationResourcesAccessorFactory _resourcesAccessorFactory = Substitute.For<IIllustrationResourcesAccessorFactory>();

        [TestInitialize]
        public void Initialize()
        {
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _valueFormatter = Substitute.For<Core.Interface.Formatters.IValueFormatter>();

            _valueFormatter.Format(Arg.Any<double>()).Returns("double");
            _formatter.GetValueFormatter(Arg.Any<TypeAffichageValeur>()).Returns(_valueFormatter);

            _resourcesAccessor = Substitute.For<IResourcesAccessor>();
            _resourcesAccessor.GetStringResourceById(Arg.Any<string>()).Returns("resource");
            _resourcesAccessorFactory.GetResourcesAccessor().Returns(_resourcesAccessor);

            _vecteurManager = new VecteurManager();
            _tableauResultatManager = new TableauResultatManager(_vecteurManager, _modelMapper, _formatter, _resourcesAccessorFactory);
        }

        [TestMethod]
        public void GIVEN_EmptyDefinitionColonnes_WHEN_ConstruireLigneSBases_ReturnsEmptyList()
        {

            var lignes = _tableauResultatManager.ConstruireLignesBase(
                Enumerable.Empty<ColonneTableau>().ToList(),
                new Projections(),
                new TableauResultatManager.InfoLigne()).ToList();


            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void ConstruireLignesBase_Valid()
        {
            var definitionColonnes = new List<ColonneTableau>()
            {
                new ColonneTableau(){ColonneMoteur = 95},
                new ColonneTableau(){ColonneMoteur = 57}
            };

            var projection = new Projection
            {
                Columns = new List<Column>
                {
                    new Column(){Id = 95, Value = new double[]{0.0,16.01,15.01}},
                    new Column(){Id = 57, Value = new double[]{0.0,35.56,34.56}},
                },
                AnneesContrat = new int[] { 0, 1, 2, 3 }
            };

            var projections = new Projections
            {
                Projection = projection
            };

            var lignes = _tableauResultatManager.ConstruireLignesBase(definitionColonnes, projections,
                new TableauResultatManager.InfoLigne()).ToList();
            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().HaveCount(3);
                lignes.First().Valeur01.Should().Be(16.01);
                lignes.First().Valeur02.Should().Be(35.56);
                lignes.Skip(1).First().Valeur01.Should().Be(15.01);
                lignes.Skip(1).First().Valeur02.Should().Be(34.56);
            }
        }

        [TestMethod]
        public void GIVEN_ProjectionsEstNull_WHEN_ConstruireLignesBase_ReturnsEmptyList()
        {
            var definitionColonnes = new List<ColonneTableau>()
            {
                new ColonneTableau(){ColonneMoteur = 95, TypeColonne = TypeColonne.Annee}
            };

            var lignes = _tableauResultatManager.ConstruireLignesBase(
                definitionColonnes,
                null,
                new TableauResultatManager.InfoLigne()).ToList();

            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GIVEN_DefinitionColonnesSansColonneAges_WHEN_ConstruireLignesBase_ReturnsEmptyList()
        {
            var definitionColonnes = new List<ColonneTableau>()
            {
                new ColonneTableau(){ColonneMoteur = 95, TypeColonne = TypeColonne.Annee}
            };

            var projections = new Projections()
            {
                Projection = new Projection()
                {
                    Columns = new List<Column>()
                    {
                        new Column(){Id = 95, Value = new double[]{0.0,11.57,11.57,11.57,11.57,11.57}}
                    },
                    AnneesContrat = new int[] { 0, 3 }
                }
            };
            var infoLigne = new TableauResultatManager.InfoLigne();
            infoLigne.SelectionAnnees = new int[] { 1, 2 };
            infoLigne.SelectionAges = new int[] { 5, 6 };

            var lignes = _tableauResultatManager.ConstruireLignesBase(
                definitionColonnes,
                projections,
                infoLigne).ToList();

            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GIVEN_DefinitionColonnesSansAnnee_WHEN_ConstruireLignes_ReturnsEmptyList()
        {
            var lignes = _tableauResultatManager.ConstruireLignes(
                Enumerable.Empty<ColonneTableau>().ToList(),
                new Projections(),
                new TableauResultatManager.InfoLigne()).ToList();

            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GIVEN_ProjectionsEstNull_WHEN_ConstruireLignes_ReturnsEmptyList()
        {
            var definitionColonnes = new List<ColonneTableau>()
            {
                new ColonneTableau(){ColonneMoteur = 95, TypeColonne = TypeColonne.Annee}
            };

            var lignes = _tableauResultatManager.ConstruireLignes(
                definitionColonnes,
                null,
                new TableauResultatManager.InfoLigne()).ToList();

            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GIVEN_DefinitionCollonnesSansColonneAges_WHEN_ConstruireLignes_ReturnsEmptyList()
        {
            var definitionColonnes = new List<ColonneTableau>()
            {
                new ColonneTableau(){ColonneMoteur = 95, TypeColonne = TypeColonne.Annee}
            };

            var projections = new Projections()
            {
                Projection = new Projection()
                {
                    Columns = new List<Column>()
                    {
                        new Column(){Id = 95, Value = new double[]{0.0,11.57,11.57,11.57,11.57,11.57}}
                    }
                }
            };

            var lignes = _tableauResultatManager.ConstruireLignes(
                definitionColonnes,
                projections,
                new TableauResultatManager.InfoLigne()).ToList();

            using (new AssertionScope())
            {
                lignes.Should().NotBeNull();
                lignes.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void GIVEN_TypeColonneAnnee_WHEN_ConstruireLigneDecheance_Valid()
        {
            var ligne = new LigneResultatViewModel();
            var colonne = new ColonneTableau() { TypeColonne = TypeColonne.Annee };
            _tableauResultatManager.ContruireLigneDecheance(10, 45, ligne, colonne);

            using (new AssertionScope())
            {
                ligne.Valeurs.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
                ligne.Valeurs.First().Should().Be(10);
                ligne.ValeursAffichage.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
                ligne.ValeursAffichage.First().Should().Be("double");
            }
        }

        [TestMethod]
        public void GIVEN_TypeColonneAge_WHEN_ConstruireLigneDecheance_Valid()
        {
            var ligne = new LigneResultatViewModel();
            var colonne = new ColonneTableau() { TypeColonne = TypeColonne.Age };
            _tableauResultatManager.ContruireLigneDecheance(10, 45, ligne, colonne);

            using (new AssertionScope())
            {
                ligne.Valeurs.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
                ligne.Valeurs.First().Should().Be(45);
                ligne.ValeursAffichage.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
                ligne.ValeursAffichage.First().Should().Be("double");
            }
        }

        [TestMethod]
        public void GIVEN_TypeColonneNormale_WHEN_ConstruireLigneDecheance_Valid()
        {
            var ligne = new LigneResultatViewModel();
            var colonne = new ColonneTableau() { TypeColonne = TypeColonne.Normale };
            _tableauResultatManager.ContruireLigneDecheance(10, 45, ligne, colonne);

            using (new AssertionScope())
            {
                ligne.Valeurs.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
                ligne.Valeurs.First().Should().Be(null);
                ligne.ValeursAffichage.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
                ligne.ValeursAffichage.First().Should().Be("resource");
            }
        }
    }
}
