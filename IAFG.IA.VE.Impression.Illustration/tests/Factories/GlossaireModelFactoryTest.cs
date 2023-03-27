using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using IAFG.IA.VE.Impression.Illustration.Types.Models.SommaireProtections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class GlossaireModelFactoryTest
    {
        private IIllustrationReportDataFormatter _formatter;
        private static readonly IFixture Auto = AutoFixtureFactory.Create();

        [TestInitialize]
        public void Initialize()
        {
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatterLibellee(Arg.Any<DefinitionTexteGlossaire>(), Arg.Any<DonneesRapportIllustration>()).Returns("Libelle :");
            _formatter.FormatterTexte(Arg.Any<DefinitionTexteGlossaire>(), Arg.Any<DonneesRapportIllustration>()).Returns("Texte du libelle");
        }

        [TestMethod]
        public void CreerDetailGlossaire_WhenTextesIsNull_ShouldReturnEmptyList()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var result = factory.CreerDetailGlossaire(new DefinitionSectionGlossaire(), 
                Auto.Create<DonneesRapportIllustration>(), Enumerable.Empty<string>().ToArray());

            using (new AssertionScope())
            {
                result.Should().NotBeNull().And.BeEmpty();
            }
        }

        [TestMethod]
        public void CreerDetailGlossaire_WhenDefinitionCodesIsNull_ShouldReturnList()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var definition = new DefinitionSectionGlossaire()
            {
                GlossaireTextes = new List<DefinitionTexteGlossaire>()
                {
                    new DefinitionTexteGlossaire(){ Libelle = "Libelle", Texte = "Texte", SequenceId = 1}
                }
            };

            var result = factory.CreerDetailGlossaire(definition, 
                Auto.Create<DonneesRapportIllustration>(), Enumerable.Empty<string>().ToArray());

            using (new AssertionScope())
            {
                result.Should().NotBeNull().And.HaveCount(1);
                result.First().SequenceId.Should().Be(1);
                result.First().Texte.Should().Be("Texte du libelle");
                result.First().Libelle.Should().Be("Libelle :");
            }
        }

        [TestMethod]
        public void CreerDetailGlossaire_WhenDefinitionCodesHasValues_ShouldReturnList()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var codes = new string[] { "001", "003" };

            var definition = new DefinitionSectionGlossaire()
            {
                GlossaireTextes = new List<DefinitionTexteGlossaire>()
                {
                    new DefinitionTexteGlossaire{ Libelle = "Libelle", Texte = "Texte", SequenceId = 1, Regles = new List<RegleGlossaire[]>(){new RegleGlossaire[]{RegleGlossaire.Aucune}}},
                    new DefinitionTexteGlossaire{ Libelle = "Libelle", Texte = "Texte", SequenceId = 2, Regles = new List<RegleGlossaire[]>()},
                    new DefinitionTexteGlossaire{ Libelle = "Libelle", Texte = "Texte", SequenceId = 3, Regles = new List<RegleGlossaire[]>()}
            },
                Codes = new List<DefinitionCodeGlossaire>()
                {
                    new DefinitionCodeGlossaire(){ Code = "001", SequenceId = 1},
                    new DefinitionCodeGlossaire(){ Code = "003", SequenceId = 1}
            }
            };

            var result = factory.CreerDetailGlossaire(definition, Auto.Create<DonneesRapportIllustration>(), codes);

            using (new AssertionScope())
            {
                result.Should().NotBeNull().And.HaveCount(2);
            }
        }

        [TestMethod]
        public void EstVisible_WhenReglesIsNull_ShouldBeTrue()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            AssertRegles(factory, null, Auto.Create<DonneesRapportIllustration>(), true);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireAucune_ShouldBeTrue()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.Aucune), Auto.Create<DonneesRapportIllustration>(), true);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireCompteTermeAndPresenceCompteTerme_ShouldBeTrue()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.PresenceCompteTerme = true;
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.CompteTerme), donnees, true);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireCompteTermeAndNotPresenceCompteTerme_ShouldBeTrue()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = Auto.Create<DonneesRapportIllustration>();
            donnees.PresenceCompteTerme = false;
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.CompteTerme), donnees, false);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireEstCapitalPlusFondsAndPrestationDecesCapitalPlusFonds_ShouldBetrue()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = WithPrestationDeces(Init(new DonneesRapportIllustration()), OptionPrestationDeces.CapitalPlusFonds);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.EstCapitalPlusFonds), donnees, true);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireEstCapitalPlusFondsAndNotPrestationDecesCapitalPlusFonds_ShouldBeFalse()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = WithPrestationDeces(Init(new DonneesRapportIllustration()), OptionPrestationDeces.ValeurMaximisee);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.EstCapitalPlusFonds), donnees, false);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireEstCapitalPlusFondsOptionVMaxAndPrestationDecesCapitalPlusFondsValMax()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = WithPrestationDeces(Init(new DonneesRapportIllustration()), OptionPrestationDeces.CapitalPlusFondsValMax);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.EstCapitalPlusFondsOptionVMax), donnees, true);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireEstCapitalPlusFondsOptionVMaxAndNotPrestationDecesCapitalPlusFondsValMax()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = WithPrestationDeces(Init(new DonneesRapportIllustration()), OptionPrestationDeces.CapitalPlusFonds);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.EstCapitalPlusFondsOptionVMax), donnees, false);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireEstCapitalPlusfondsPlusCBRAndPrestationDecesCapitalPlusFondsPlusRemboursementCBR()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = WithPrestationDeces(Init(new DonneesRapportIllustration()), OptionPrestationDeces.CapitalPlusFondsPlusRemboursementCBR);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.EstCapitalPlusFondsPlusCBR), donnees, true);
        }

        [TestMethod]
        public void EstVisible_WhenRegleGlossaireEstCapitalPlusfondsPlusCBRAndNotPrestationDecesCapitalPlusFondsPlusRemboursementCBR()
        {
            var factory = new GlossaireModelFactory(null, _formatter, null);
            var donnees = WithPrestationDeces(Init(new DonneesRapportIllustration()), OptionPrestationDeces.CapitalPlusFonds);
            AssertRegles(factory, MockReglesGlossaire(RegleGlossaire.EstCapitalPlusFondsPlusCBR), donnees, false);
        }

        private IList<RegleGlossaire> MockReglesGlossaire(params RegleGlossaire[] regles)
        {
            return regles.ToList();
        }

        private void AssertRegles(GlossaireModelFactory factory, IList<RegleGlossaire> regles, DonneesRapportIllustration donnees, bool boolAssert)
        {
            if (boolAssert)
            {
                factory.EstVisible(regles, donnees).Should().BeTrue();
            }
            else
            {
                factory.EstVisible(regles, donnees).Should().BeFalse();
            }
        }

        private static DonneesRapportIllustration Init(DonneesRapportIllustration donnees)
        {
            if (donnees == null)
            {
                donnees = new DonneesRapportIllustration();
            }

            donnees.Protections = new Protections();

            return donnees;
        }

        private static DonneesRapportIllustration WithPrestationDeces(DonneesRapportIllustration donnees, OptionPrestationDeces prestationDeces)
        {
            donnees.Protections.PrestationDeces = prestationDeces;
            return donnees;
        }
    }
}
