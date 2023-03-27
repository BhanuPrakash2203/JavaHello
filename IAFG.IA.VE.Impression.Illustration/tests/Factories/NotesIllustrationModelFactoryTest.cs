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
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Rules;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class NotesIllustrationModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private readonly IDefinitionSectionManager _sectionManager = Substitute.For<IDefinitionSectionManager>();
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();
        private readonly IProductRules _productRules = Substitute.For<IProductRules>();
        private IDefinitionTitreManager _titreManager;
        private IDefinitionTexteManager _texteManager;
        private IDefinitionImageManager _imageManager;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _titreManager = new DefinitionTitreManager(_formatter);
            _texteManager = new DefinitionTexteManager(_formatter, _productRules);
            _imageManager = new DefinitionImageManager();
        }

        [TestMethod]
        public void GIVEN_NotesIllustrationModelFactory_WHEN_Build_Then_ReturnSectionNotesIllustrationModel()
        {
            var donnees = Auto.Create<DonneesRapportIllustration>();
            var definition = Auto.Create<DefinitionSection>();
            definition.SectionId = "NotesIllustration";
            definition.ListSections[0].SectionId = "Resultats";
            definition.ListSections[0].Textes[0].Regles = new List<RegleTexte[]>();
            definition.ListSections[0].Textes[1].Regles = new List<RegleTexte[]>();
            definition.ListSections[0].Textes[2].Regles = new List<RegleTexte[]>();
            definition.ListSections[1].SectionId = "Garanties";
            definition.ListSections[1].Textes[0].Regles = new List<RegleTexte[]>();
            definition.ListSections[1].Textes[1].Regles = new List<RegleTexte[]>();
            definition.ListSections[1].Textes[2].Regles = new List<RegleTexte[]>();
            definition.ListSections.RemoveAt(2);

            _configurationRepository
                .ObtenirDefinitionSection(
                    Arg.Any<string>(), 
                    Arg.Any<Produit>(), 
                    Arg.Any<Func<DefinitionSection, DefinitionSection, DefinitionSection>>())
                .Returns(definition);

            _formatter.FormatterTitre(definition.Titres.FirstOrDefault(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new NotesIllustrationModelFactory(_configurationRepository,  
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager), 
                _sectionManager, _titreManager, _texteManager);

            var model = factory.Build(definition.SectionId, donnees, Auto.Create<IReportContext>());

            model.SousSections.Count.Should().Be(definition.ListSections.Count);
            model.TitreSection.Should().Be(definition.Titres.First().Titre);
        }
    }
}
