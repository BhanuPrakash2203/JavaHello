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
using IAFG.IA.VE.Impression.Illustration.Types.Enums;
using IAFG.IA.VE.Impression.Illustration.Types.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IAFG.IA.VE.Impression.Illustration.Tests.Factories
{
    [TestClass]
    public class SectionModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private readonly IDefinitionNoteManager _noteManager = Substitute.For<IDefinitionNoteManager>();
        private readonly IDefinitionTableauManager _tableauManager = Substitute.For<IDefinitionTableauManager>();

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
        }

        [TestMethod]
        public void GIVEN_ModelFactory_WHEN_Build_Then_ReturnSectionModel()
        {
            var donnees = Auto.Create<DonneesRapportIllustration>();
            var definition = new DefinitionSection
            {
                SectionId = "SectionX",
                Titres = Auto.Create<List<DefinitionTitreDescriptionSelonProduit>>(),
                Images = Auto.Create<Dictionary<string, List<DefinitionImageSelonProduit>>>()
            };

            _configurationRepository.ObtenirDefinitionSection<DefinitionSection>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(definition.Titres.FirstOrDefault(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new SectionModelFactory(_configurationRepository, 
                new SectionModelMapper(_formatter, _noteManager, _tableauManager, 
                    new DefinitionTitreManager(_formatter), new DefinitionImageManager()));

            var model = factory.Build(definition.SectionId, donnees, Auto.Create<IReportContext>());

            model.TitreSection.Should().Be(definition.Titres.First().Titre);
        }


    }
}
