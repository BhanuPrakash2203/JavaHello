using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Managers
{
    [TestClass]
    public class DefinitionTitreManagerTests
    {
        private DonneesRapportIllustration _donnees;
        private IIllustrationReportDataFormatter _formatter;
        private static readonly IFixture Auto = AutoFixtureFactory.Create();

        [TestInitialize]
        public void Initialize()
        {
            _donnees = Auto.Create<DonneesRapportIllustration>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _formatter.FormatterDescription(Arg.Any<DefinitionTitreDescriptionSelonProduit>(), Arg.Any<DonneesRapportIllustration>()).Returns("description");
            _formatter.FormatterTitre(Arg.Any<DefinitionTitreDescription>(), Arg.Any<DonneesRapportIllustration>()).Returns("titre");
        }

        [TestMethod]
        public void ObtenirDescriptions_WhenDefinitionsIsNull_ReturnsStringEmpty()
        {
            var manager = new DefinitionTitreManager(_formatter);
            manager.ObtenirDescription(null, _donnees).Should().BeEmpty();
        }

        [TestMethod]
        public void ObtenirDescriptions_WhenDefinitionsIsEmpty_ReturnsStringEmpty()
        {
            var definitions = new List<DefinitionTitreDescriptionSelonProduit>();
            var manager = new DefinitionTitreManager(_formatter);
            manager.ObtenirDescription(definitions, _donnees).Should().BeEmpty();
        }

        [TestMethod]
        public void ObtenirDescriptions_WhenDefinitionsContainsElements_ReturnsValidString()
        {
            _donnees.Produit = Produit.AssuranceParticipant;
            var definitions = new List<DefinitionTitreDescriptionSelonProduit>()
            {
                new DefinitionTitreDescriptionSelonProduit(){Produit = Produit.AssuranceParticipant}
            };

            var manager = new DefinitionTitreManager(_formatter);
            manager.ObtenirDescription(definitions, _donnees).Should().Be("description");
        }
    }
}