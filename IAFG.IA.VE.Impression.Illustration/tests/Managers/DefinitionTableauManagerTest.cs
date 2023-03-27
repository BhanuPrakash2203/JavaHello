using System.Collections.Generic;
using FluentAssertions;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Managers
{
    [TestClass]
    public class DefinitionTableauManagerTest
    {
        private PrivateObject _manager;
        private IVecteurManager _vecteurManager;
        private IIllustrationReportDataFormatter _formatter;
        private IDefinitionNoteManager _noteManager;
        private IDefinitionTitreManager _titreManager;

        [TestInitialize]
        public void Initialize()
        {
            _vecteurManager = Substitute.For<IVecteurManager>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _noteManager = Substitute.For<IDefinitionNoteManager>();
            _titreManager = new DefinitionTitreManager(_formatter);
            _manager = new PrivateObject(typeof(DefinitionTableauManager), _vecteurManager, _formatter, _noteManager, _titreManager);
        }

        [TestMethod]
        public void EstVisible_WhenReglesIsAucune_ReturnTrue()
        {
            var regles = new List<RegleColonne> { RegleColonne.Aucune };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), new double[] { }, new DonneesRapportIllustration());
            result.Should().BeTrue();
        }

        [TestMethod]
        public void EstVisible_WhenReglesIsToujoursMasquee_ReturnFalse()
        {
            var regles = new List<RegleColonne> { RegleColonne.ToujoursMasquee };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), new double[] { }, new DonneesRapportIllustration());
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(RegleColonne.ValeurDifferenteZero, 0, false)]
        [DataRow(RegleColonne.ValeurDifferenteZero, 2, true)]
        [DataRow(RegleColonne.ValeurDifferenteZero, -2, true)]
        [DataRow(RegleColonne.ValeurPlusGrandeZero, 0, false)]
        [DataRow(RegleColonne.ValeurPlusGrandeZero, 2, true)]
        [DataRow(RegleColonne.ValeurPlusGrandeZero, -2, false)]
        public void EstVisible_WhenReglesIsValeurDifferenteZeroOrValeurPlusGrandeZero_ReturnTrue(RegleColonne regle, double value, bool expectedValue)
        {
            var regles = new List<RegleColonne> { regle };
            var vector = new[] { value };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), vector, new DonneesRapportIllustration());
            result.Should().Be(expectedValue);
        }

        [TestMethod]
        public void EstVisible_WhenReglesIsVecteurPresentAndVecteurIsEmpty_ReturnFalse()
        {
            var regles = new List<RegleColonne> { RegleColonne.VecteurPresent };
            var vector = new double[] { };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), vector, new DonneesRapportIllustration());
            result.Should().BeFalse();
        }

        [TestMethod]
        public void EstVisible_WhenReglesIsVecteurPresentAndVecteurIsNotEmpty_ReturnTrue()
        {
            var regles = new List<RegleColonne> { RegleColonne.VecteurPresent };
            var vector = new double[] { 0 };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), vector, new DonneesRapportIllustration());
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        public void EstVisible_WhenReglesIsContractantEstCompagnie_ReturnExpectedValue(bool contractantEstCompagnie, bool expectedValue)
        {
            var regles = new List<RegleColonne> { RegleColonne.ContractantEstCompagnie };
            var donnees = new DonneesRapportIllustration { ContractantEstCompagnie = contractantEstCompagnie };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), new double[] { }, donnees);
            result.Should().Be(expectedValue);
        }

        [DataTestMethod]
        [DataRow(RegleColonne.ContratConjoint, true, true)]
        [DataRow(RegleColonne.ContratConjoint, false, false)]
        [DataRow(RegleColonne.ContratIndividuel, true, false)]
        [DataRow(RegleColonne.ContratIndividuel, false, true)]

        public void EstVisible_WhenReglesIsContratConjointOrContratIndividuel_ReturnExpectedValue(RegleColonne regle, bool contratEstConjoint, bool expectedValue)
        {
            var regles = new List<RegleColonne> { regle };
            var donnees = new DonneesRapportIllustration { ContratEstConjoint = contratEstConjoint };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), new double[] { }, donnees);
            result.Should().Be(expectedValue);
        }

        [DataTestMethod]
        [DataRow(Etat.NouvelleVente, true)]
        [DataRow(Etat.EnVigueur, false)]
        [DataRow(Etat.NonDefini, false)]

        public void EstVisible_WhenReglesIsNouvelleVente_ReturnExpectedValue(Etat etat, bool expectedValue)
        {
            var regles = new List<RegleColonne> { RegleColonne.NouvelleVente };
            var donnees = new DonneesRapportIllustration { Etat = etat };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), new double[] { }, donnees);
            result.Should().Be(expectedValue);
        }

        [DataTestMethod]
        [DataRow(Etat.NouvelleVente, false)]
        [DataRow(Etat.EnVigueur, true)]
        [DataRow(Etat.NonDefini, true)]

        public void EstVisible_WhenReglesIsEnVigueur_ReturnExpectedValue(Etat etat, bool expectedValue)
        {
            var regles = new List<RegleColonne> { RegleColonne.EnVigueur };
            var donnees = new DonneesRapportIllustration { Etat = etat };

            var result = (bool)_manager.Invoke("EstVisible", regles, new ColonneTableau(), new double[] { }, donnees);
            result.Should().Be(expectedValue);
        }
    }
}
