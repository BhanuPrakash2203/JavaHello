using FluentAssertions;
using IAFG.IA.VE.Impression.Illustration.Business.Constants;
using IAFG.IA.VE.Impression.Illustration.Business.Helper;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Helpers
{
    [TestClass]
    public class TitrePrimesVerseesTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceMensuelleEtEstPAR_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesMensuellesVerseesSelectionneesPAR;
            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Mensuelle, Produit.AssuranceParticipant);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceAnnuelleEtEstPAR_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesAnnuellesVerseesSelectionneesPAR;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Annuelle, Produit.AssuranceParticipant);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceAucunModeEtEstPAR_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesVerseesSelectionneesPAR;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.AucunMode, Produit.AssuranceParticipant);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceAutreEtEstPAR_ThenEmpty()
        {
            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Autre, Produit.AssuranceParticipant);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceMensuelleEtNonPAR_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesMensuellesVerseesSelectionnees;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Mensuelle, Produit.Genesis);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceAnnuelleEtNonPAR_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesAnnuellesVerseesSelectionnees;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Annuelle, Produit.Genesis);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceAucunModeEtNonPAR_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesVerseesSelectionnees;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.AucunMode, Produit.Genesis);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenFrequenceAutreEtNonPAR_ThenEmpty()
        {
            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Autre, Produit.Genesis);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenPAREtEnVigueur_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesVerseesSelectionneesPAR;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Annuelle, Produit.AssuranceParticipantValeur, Etat.EnVigueur);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenNonPAREtEnVigueur_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesVerseesSelectionnees;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.Annuelle, Produit.Genesis, Etat.EnVigueur);
            result.Should().Be(expectedLibelle);
        }

        [TestMethod]
        public void ObtenirNomLibelleTitreColonneSelonFrequence_WhenNonPAREtNouvelleVenteEtAucunMode_ThenExpectedLibelle()
        {
            const string expectedLibelle = LibellesPrimeVersee.PrimesVerseesSelectionnees;

            var result = TitrePrimesVersees.ObtenirNomLibelleTitreColonneSelonFrequence(TypeFrequenceFacturation.AucunMode, Produit.Genesis, Etat.NouvelleVente);
            result.Should().Be(expectedLibelle);
        }
    }
}
