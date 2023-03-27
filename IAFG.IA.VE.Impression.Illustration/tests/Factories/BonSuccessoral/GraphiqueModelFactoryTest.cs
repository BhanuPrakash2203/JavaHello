using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories.BonSuccessoral;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
using IAFG.IA.VE.Impression.Illustration.Business.Mappers;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Configuration;
using IAFG.IA.VE.Impression.Illustration.Interfaces.Business.Formatters;
using IAFG.IA.VE.Impression.Illustration.Types.Definitions;
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories.BonSuccessoral
{
    [TestClass]
    public class GraphiqueModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private IDefinitionNoteManager _noteManager;
        private IDefinitionTableauManager _tableauManager;
        private IDefinitionTitreManager _titreManager;
        private IDefinitionImageManager _imageManager;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _noteManager = Substitute.For<IDefinitionNoteManager>();
            _tableauManager = Substitute.For<IDefinitionTableauManager>();
            _titreManager = new DefinitionTitreManager(_formatter);
            _imageManager = new DefinitionImageManager();
        }

        [TestMethod]
        public void GIVEN_ModelFactory_WHEN_Build_Then_ReturnSectionModel()
        {
            var donnees = Auto.Create<DonneesRapportIllustration>();
            var definition = new DefinitionSection
            {
                SectionId = "TestX",
                Titres = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>(),
                Libelles = new Dictionary<string, DefinitionLibelle>()
            };

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(definition.Titres.FirstOrDefault(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new PageGraphiqueModelFactory(
                _configurationRepository, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, _titreManager, _imageManager), 
                new VecteurManager());
            
            var model = factory.Build(definition.SectionId, donnees, Auto.Create<IReportContext>());
            model.TitreSection.Should().Be(definition.Titres.First().Titre);
        }
    }
}
