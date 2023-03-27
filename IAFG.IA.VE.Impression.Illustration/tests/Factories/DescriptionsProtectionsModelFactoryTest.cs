using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions.DescriptionsProtections;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class DescriptionsProtectionsModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();
        private IDefinitionTitreManager _titreManager;
        private IDefinitionImageManager _imageManager;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _titreManager = new DefinitionTitreManager(_formatter);
            _imageManager = new DefinitionImageManager();
        }

        [TestMethod]
        public void GIVEN_DescriptionsProtectionsModelFactory_WHEN_Build_Then_ReturnSectionDescriptionsProtectionsModel()
        {
            var definition = Auto.Create<DefinitionSectionDescriptionsProtections>();
            var donnees = Auto.Create<DonneesRapportIllustration>();
            definition.Descriptions.First().CodesRegle = null;
            donnees.Protections.ProtectionsAssures.Clear();

            _configurationRepository.ObtenirDefinitionSection(
                Arg.Any<string>(), Arg.Any<Produit>(),
                Arg.Any<Func<DefinitionSectionDescriptionsProtections, 
                             DefinitionSectionDescriptionsProtections,
                             DefinitionSectionDescriptionsProtections>>()).Returns(definition);

            _formatter.FormatterTitre(definition.Titres.First(), donnees).Returns(definition.Titres.First().Titre);

            var factory =
                new DescriptionsProtectionsModelFactory(_configurationRepository, 
                    new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager));

            var model = factory.Build("1", donnees, Auto.Create<IReportContext>());

            model.TitreSection.Should().Be(definition.Titres.First().Titre);
            model.Details.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void GIVEN_DefinitionSectionDescriptionProtection_WHEN_CreerDetailDescriptionProtectionsWithDescriptiveCode_THEN_ReturnDetailsFilterByDescriptiveCode()
        {
            var codesDescriptif = new List<string> { "VieEntiere1" };
            var definitionSection = Auto.Create<DefinitionSectionDescriptionsProtections>();
            var definitionTexte1 = Auto.Create<DefinitionDescriptions>();
            definitionTexte1.CodesRegle = new List<string[]> { new[] { "VieEntiere1" } };
            definitionTexte1.RegleProduits = null;
            var definitionTexte2 = Auto.Create<DefinitionDescriptions>();
            definitionTexte2.CodesRegle = new List<string[]>();
            definitionTexte2.RegleProduits = null;
            var definitionTexte3 = Auto.Create<DefinitionDescriptions>();
            definitionTexte3.CodesRegle = new List<string[]> { new[] { "MaladieGrave1" } };
            definitionTexte3.RegleProduits = null;
            definitionSection.Descriptions = new List<DefinitionDescriptions> { definitionTexte1, definitionTexte2, definitionTexte3 };

            var donnes = Auto.Create<DonneesRapportIllustration>();
            donnes.Langue = Core.Types.Enums.Language.French;
            var result = DescriptionsProtectionsModelFactory.CreerDetailDescriptions(definitionSection, donnes, codesDescriptif);

            result.Should().HaveCount(2);
            result.Any(x => x.Texte == definitionTexte1.Texte).Should().BeTrue();
            result.Any(x => x.Texte == definitionTexte2.Texte).Should().BeTrue();
        }

        [TestMethod]
        public void GIVEN_DefinitionSectionDescriptionProtection_WHEN_CreerDetailDescriptionProtectionsWithDescriptiveCode_THEN_ReturnDetailsFilterByProduit()
        {
            var codesDescriptif = new List<string> { "VieEntiere1" };
            var definitionSection = Auto.Create<DefinitionSectionDescriptionsProtections>();
            var definitionTexte1 = Auto.Create<DefinitionDescriptions>();
            definitionTexte1.CodesRegle = new List<string[]> { new[] { "VieEntiere1" } };
            definitionTexte1.RegleProduits = null;
            var definitionTexte2 = Auto.Create<DefinitionDescriptions>();
            definitionTexte2.CodesRegle = new List<string[]>();
            definitionTexte2.RegleProduits = new RegleProduits { Produits = new[] { Produit.AccesVie } };
            var definitionTexte3 = Auto.Create<DefinitionDescriptions>();
            definitionTexte3.CodesRegle = new List<string[]> { new[] { "MaladieGrave1" } };
            definitionTexte3.RegleProduits = null;
            definitionSection.Descriptions = new List<DefinitionDescriptions> { definitionTexte1, definitionTexte2, definitionTexte3 };

            var donnes = Auto.Create<DonneesRapportIllustration>();
            donnes.Langue = Core.Types.Enums.Language.French;
            donnes.Produit = Produit.CapitalValeur;
            var result = DescriptionsProtectionsModelFactory.CreerDetailDescriptions(definitionSection, donnes, codesDescriptif);

            result.Should().HaveCount(1);
            result.Any(x => x.Texte == definitionTexte1.Texte).Should().BeTrue();
        }
    }
}
