using System.Linq;
using AutoFixture;
using FluentAssertions;
using IAFG.IA.VE.Impression.Core.Interface.ReportContext;
using IAFG.IA.VE.Impression.CoreForTests;
using IAFG.IA.VE.Impression.Illustration.Business.Factories;
using IAFG.IA.VE.Impression.Illustration.Business.Managers;
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
    public class ApercuProtectionsModelFactoryTest
    {
        private static readonly IFixture Auto = AutoFixtureFactory.Create();
        private IConfigurationRepository _configurationRepository;
        private IIllustrationReportDataFormatter _formatter;
        private IDefinitionNoteManager _noteManager;
        private IDefinitionTableauManager _tableauManager;
        private IDefinitionTitreManager _titreManager;

        [TestInitialize]
        public void Initialize()
        {
            _configurationRepository = Substitute.For<IConfigurationRepository>();
            _formatter = Substitute.For<IIllustrationReportDataFormatter>();
            _noteManager = Substitute.For<IDefinitionNoteManager>();
            _tableauManager = Substitute.For<IDefinitionTableauManager>();
            _titreManager = new DefinitionTitreManager(_formatter);
        }

        [TestMethod]
        public void GIVEN_ApercuProtectionsModelFactory_WHEN_Build_Then_ReturnSectionResultatModel()
        {
            var definition = Auto.Create<DefinitionSectionResultats>();

            var donnees = Auto.Create<DonneesRapportIllustration>();
            foreach (var item in donnees.ProtectionsGroupees) 
            {
                foreach (var protection in item.ProtectionsAssures) 
                {
                    protection.EstProtectionContractant = false;
                }
            }

            _configurationRepository.ObtenirDefinitionSection<DefinitionSectionResultats>(Arg.Any<string>(), Arg.Any<Produit>()).Returns(definition);
            _formatter.FormatterTitre(definition.Titres.FirstOrDefault(), donnees).Returns(definition.Titres.First().Titre);

            var factory = new ApercuProtectionsModelFactory(_configurationRepository, _formatter, _noteManager, _titreManager, _tableauManager);
            var model = factory.Build(definition.SectionId, donnees, Auto.Create<IReportContext>());
            model.TitreSection.Should().Be(definition.Titres.First().Titre);
        }
    }
}
